using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;

namespace HealthWellbeing.Controllers
{
    public class FoodIntakeController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public FoodIntakeController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // =========================
        // GET: FoodIntake/Index
        // =========================
        public async Task<IActionResult> Index(int clientId, DateTime? date)
        {
            var selectedDate = (date ?? DateTime.Today).Date;

            var vm = new FoodIntakeIndexVM
            {
                SelectedDate = selectedDate,
                SelectedClientId = clientId,
                Clients = await LoadClientsAsync(clientId),
                AvailableFoods = await LoadFoodsAsync()
            };

            if (clientId <= 0)
                return View(vm);

            vm.SelectedClientName = await GetClientNameAsync(clientId);

            var plan = await GetClientPlanAsync(clientId);
            if (plan == null)
            {
                vm.Items = [];
                // sem plano -> sem metas/progresso
                return View(vm);
            }

            // Goal do cliente (assumo 1 goal ativo; escolho o "mais recente")
            var goal = await _context.Goal
                .AsNoTracking()
                .Where(g => g.ClientId == clientId)
                .OrderByDescending(g => g.GoalId)
                .FirstOrDefaultAsync();

            // 1) Plano do dia (FoodPlanDay)
            var dayPlan = await _context.FoodPlanDay
                .AsNoTracking()
                .Include(x => x.Food)
                .Include(x => x.Portion)
                .Where(x => x.PlanId == plan.PlanId && x.Date.Date == selectedDate)
                .OrderBy(x => x.ScheduledTime)
                .ThenBy(x => x.Food!.Name)
                .ToListAsync();

            // 2) Intakes do dia
            var dayIntakes = await _context.FoodIntake
                .Where(x => x.PlanId == plan.PlanId && x.Date.Date == selectedDate)
                .ToListAsync();

            var intakeByFoodId = dayIntakes.ToDictionary(x => x.FoodId, x => x);

            // 3) Merge: garantir que existe FoodIntake para cada linha do plano do dia
            bool created = false;
            foreach (var line in dayPlan)
            {
                if (!intakeByFoodId.TryGetValue(line.FoodId, out var fi))
                {
                    // cria intake
                    fi = new FoodIntake
                    {
                        PlanId = plan.PlanId,
                        FoodId = line.FoodId,
                        PortionId = line.PortionId,
                        Date = selectedDate,
                        ScheduledTime = line.ScheduledTime ?? DateTime.Now,

                        // se aplicaste a alteração recomendada:
                        PortionsPlanned = line.PortionsPlanned,
                        PortionsEaten = 0
                    };

                    _context.FoodIntake.Add(fi);
                    intakeByFoodId[line.FoodId] = fi;
                    created = true;
                }
                else
                {
                    // se mudou a quantidade planeada no plano, atualiza snapshot
                    if (fi.PortionsPlanned != line.PortionsPlanned)
                        fi.PortionsPlanned = line.PortionsPlanned;

                    // se mudou porção no plano, alinha
                    if (fi.PortionId != line.PortionId)
                        fi.PortionId = line.PortionId;
                }
            }

            if (created)
                await _context.SaveChangesAsync();

            // 4) Cards (base = dayPlan)
            vm.Items = dayPlan.Select(line =>
            {
                var fi = intakeByFoodId[line.FoodId];

                return new FoodIntakeCardVM
                {
                    FoodIntakeId = fi.FoodIntakeId,
                    FoodId = line.FoodId,
                    FoodName = line.Food?.Name ?? "Food",
                    PortionName = line.Portion?.PortionName,
                    IsConsumed = fi.PortionsEaten >= fi.PortionsPlanned,
                    IconClass = GuessFoodIcon(line.Food?.Name),
                    Note = $"Porções: {fi.PortionsEaten}/{fi.PortionsPlanned}"
                };
            }).ToList();

            // 5) Calcular progresso do Goal (planeado vs consumido)
            var progress = await CalculateProgressAsync(plan.PlanId, selectedDate);

            // passa no ViewBag (para não mexer no teu VM se não quiseres)
            ViewBag.Goal = goal;
            ViewBag.ProgressPlanned = progress.Planned;
            ViewBag.ProgressEaten = progress.Eaten;

            return View(vm);
        }

        // =========================================
        // POST: FoodIntake/ToggleConsumed
        // =========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleConsumed([FromBody] ToggleConsumedDto dto)
        {
            if (dto.ClientId <= 0 || dto.FoodId <= 0)
                return BadRequest("Dados inválidos.");

            var plan = await GetClientPlanAsync(dto.ClientId);
            if (plan == null) return NotFound("Cliente sem plano.");

            var day = dto.Date.Date;

            var fi = await _context.FoodIntake
                .FirstOrDefaultAsync(x => x.PlanId == plan.PlanId && x.FoodId == dto.FoodId && x.Date.Date == day);

            if (fi == null)
            {
                // tem de existir no plano do dia
                var line = await _context.FoodPlanDay
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.PlanId == plan.PlanId && x.FoodId == dto.FoodId && x.Date.Date == day);

                if (line == null)
                    return BadRequest("Esse alimento não está no plano desse dia.");

                fi = new FoodIntake
                {
                    PlanId = plan.PlanId,
                    FoodId = dto.FoodId,
                    PortionId = line.PortionId,
                    Date = day,
                    ScheduledTime = line.ScheduledTime ?? DateTime.Now,
                    PortionsPlanned = line.PortionsPlanned,
                    PortionsEaten = 0
                };
                _context.FoodIntake.Add(fi);
            }

            // Toggle simples: Consumido = comeu todas as porções; Not = 0
            fi.PortionsEaten = dto.Consumed ? fi.PortionsPlanned : 0;

            await _context.SaveChangesAsync();

            return Json(new { foodIntakeId = fi.FoodIntakeId });
        }

        // =========================================
        // POST: FoodIntake/AddFood
        // adiciona ao plano do dia (FoodPlanDay) + cria intake
        // =========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFood([FromBody] AddFoodDto dto)
        {
            if (dto.ClientId <= 0 || dto.FoodId <= 0)
                return BadRequest("Dados inválidos.");

            var plan = await GetClientPlanAsync(dto.ClientId);
            if (plan == null) return NotFound("Cliente sem plano.");

            var day = dto.Date.Date;

            // Porção tem de existir
            if (!dto.PortionId.HasValue || dto.PortionId.Value <= 0)
                return BadRequest("Tens de indicar a PortionId para adicionar.");

            // 1) criar FoodPlanDay (evitar duplicados)
            var existingDayLine = await _context.FoodPlanDay
                .FirstOrDefaultAsync(x => x.PlanId == plan.PlanId && x.Date.Date == day && x.FoodId == dto.FoodId);

            if (existingDayLine == null)
            {
                existingDayLine = new FoodPlanDay
                {
                    PlanId = plan.PlanId,
                    FoodId = dto.FoodId,
                    PortionId = dto.PortionId.Value,
                    Date = day,
                    PortionsPlanned = Math.Max(1, dto.PortionsPlanned),
                    ScheduledTime = DateTime.Now
                };

                _context.FoodPlanDay.Add(existingDayLine);
                await _context.SaveChangesAsync();
            }

            // 2) criar/garantir FoodIntake do dia
            var fi = await _context.FoodIntake
                .FirstOrDefaultAsync(x => x.PlanId == plan.PlanId && x.FoodId == dto.FoodId && x.Date.Date == day);

            if (fi == null)
            {
                fi = new FoodIntake
                {
                    PlanId = plan.PlanId,
                    FoodId = dto.FoodId,
                    PortionId = existingDayLine.PortionId,
                    Date = day,
                    ScheduledTime = existingDayLine.ScheduledTime ?? DateTime.Now,
                    PortionsPlanned = existingDayLine.PortionsPlanned,
                    PortionsEaten = 0
                };
                _context.FoodIntake.Add(fi);
                await _context.SaveChangesAsync();
            }

            var portionName = await _context.Portion
                .Where(p => p.PortionId == fi.PortionId)
                .Select(p => p.PortionName)
                .FirstOrDefaultAsync();

            return Json(new { foodIntakeId = fi.FoodIntakeId, portionName });
        }

        // =========================
        // Progress (Goal)
        // =========================
        private async Task<(MacroTotals Planned, MacroTotals Eaten)> CalculateProgressAsync(int planId, DateTime day)
        {
            // buscar linhas do plano do dia e nutrientes
            var lines = await _context.FoodPlanDay
                .AsNoTracking()
                .Where(x => x.PlanId == planId && x.Date.Date == day.Date)
                .Select(x => new { x.FoodId, x.PortionsPlanned })
                .ToListAsync();

            var foodIds = lines.Select(x => x.FoodId).Distinct().ToList();

            // Nutrientes por FoodId
            var nutrients = await _context.FoodNutritionalComponent
                .AsNoTracking()
                .Include(x => x.NutritionalComponent)
                .Where(x => foodIds.Contains(x.FoodId))
                .ToListAsync();

            // intake (porções consumidas)
            var intakes = await _context.FoodIntake
                .AsNoTracking()
                .Where(x => x.PlanId == planId && x.Date.Date == day.Date)
                .Select(x => new { x.FoodId, x.PortionsPlanned, x.PortionsEaten })
                .ToListAsync();

            var intakeByFoodId = intakes.ToDictionary(x => x.FoodId, x => x);

            // Mapeamento por nome
            double GetValueFor(int foodId, string key)
            {
                return nutrients
                    .Where(n => n.FoodId == foodId && n.NutritionalComponent != null)
                    .Where(n => n.NutritionalComponent!.Name != null &&
                                n.NutritionalComponent.Name.ToLower().Contains(key))
                    .Select(n => n.Value)
                    .FirstOrDefault();
            }

            var planned = new MacroTotals();
            var eaten = new MacroTotals();

            foreach (var line in lines)
            {
                var fId = line.FoodId;
                var plannedPortions = line.PortionsPlanned;

                var calories = GetValueFor(fId, "cal");      // calories/energy
                var protein = GetValueFor(fId, "protein");
                var fat = GetValueFor(fId, "fat");
                var carbs = GetValueFor(fId, "hydra");       // hydrates/carbs
                var vitamins = GetValueFor(fId, "vit");      // vitamins

                planned.Calories += calories * plannedPortions;
                planned.Protein += protein * plannedPortions;
                planned.Fat += fat * plannedPortions;
                planned.Hydrates += carbs * plannedPortions;
                planned.Vitamins += vitamins * plannedPortions;

                var eatenPortions = intakeByFoodId.TryGetValue(fId, out var i) ? i.PortionsEaten : 0;

                eaten.Calories += calories * eatenPortions;
                eaten.Protein += protein * eatenPortions;
                eaten.Fat += fat * eatenPortions;
                eaten.Hydrates += carbs * eatenPortions;
                eaten.Vitamins += vitamins * eatenPortions;
            }

            return (planned, eaten);
        }

        // =========================
        // Helpers
        // =========================
        private async Task<Plan?> GetClientPlanAsync(int clientId)
        {
            return await _context.Plan
                .FirstOrDefaultAsync(p => p.ClientId == clientId);
        }

        private async Task<List<SelectListItem>> LoadClientsAsync(int selectedClientId)
        {
            return await _context.Client
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new SelectListItem
                {
                    Value = c.ClientId.ToString(),
                    Text = c.Name,
                    Selected = c.ClientId == selectedClientId
                })
                .ToListAsync();
        }

        private async Task<List<SelectListItem>> LoadFoodsAsync()
        {
            return await _context.Food
                .AsNoTracking()
                .OrderBy(f => f.Name)
                .Select(f => new SelectListItem
                {
                    Value = f.FoodId.ToString(),
                    Text = f.Name
                })
                .ToListAsync();
        }

        private async Task<string?> GetClientNameAsync(int clientId)
        {
            return await _context.Client
                .AsNoTracking()
                .Where(c => c.ClientId == clientId)
                .Select(c => c.Name)
                .FirstOrDefaultAsync();
        }

        private static string GuessFoodIcon(string? foodName)
        {
            if (string.IsNullOrWhiteSpace(foodName)) return "bi-egg-fried";
            var n = foodName.ToLowerInvariant();
            if (n.Contains("maç") || n.Contains("banana") || n.Contains("fruta")) return "bi-apple";
            if (n.Contains("peixe") || n.Contains("salm")) return "bi-water";
            if (n.Contains("frango") || n.Contains("carne")) return "bi-fire";
            if (n.Contains("leite") || n.Contains("iogur")) return "bi-cup-straw";
            if (n.Contains("pão") || n.Contains("arroz") || n.Contains("massa")) return "bi-basket2";
            if (n.Contains("salada") || n.Contains("legum")) return "bi-flower1";
            return "bi-egg-fried";
        }
    }

    // DTOs
    public class ToggleConsumedDto
    {
        public int ClientId { get; set; }
        public int FoodId { get; set; }
        public DateTime Date { get; set; }
        public bool Consumed { get; set; }
    }

    public class AddFoodDto
    {
        public int ClientId { get; set; }
        public int FoodId { get; set; }
        public DateTime Date { get; set; }

        public int? PortionId { get; set; }
        public int PortionsPlanned { get; set; } = 1;
    }

    public class MacroTotals
    {
        public double Calories { get; set; }
        public double Protein { get; set; }
        public double Fat { get; set; }
        public double Hydrates { get; set; }
        public double Vitamins { get; set; }
    }
}
