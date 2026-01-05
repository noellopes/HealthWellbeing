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
                AvailableFoods = await LoadFoodsAsync(),
                Items = new List<FoodIntakeCardVM>()
            };

            if (clientId <= 0)
                return View(vm);

            vm.SelectedClientName = await GetClientNameAsync(clientId);

            var plan = await GetClientPlanAsync(clientId);
            if (plan == null)
            {
                // sem plano -> sem cards e sem dashboard
                return View(vm);
            }

            // Goal
            var goal = await _context.Goal
                .AsNoTracking()
                .Where(g => g.ClientId == clientId)
                .OrderByDescending(g => g.GoalId)
                .FirstOrDefaultAsync();

            if (goal != null)
            {
                vm.DailyCaloriesGoal = goal.DailyCalories;
                vm.DailyProteinGoal = goal.DailyProtein;
                vm.DailyFatGoal = goal.DailyFat;
                vm.DailyHydratesGoal = goal.DailyHydrates;
            }

            // 1) Plan Validation
            await EnsureDayPlanAsync(plan.PlanId, selectedDate);

            // 1.5) Date validation
            var today = DateTime.Today;

            var range = await _context.FoodPlanDay
                .AsNoTracking()
                .Where(x => x.PlanId == plan.PlanId)
                .GroupBy(x => 1)
                .Select(g => new { Min = g.Min(a => a.Date), Max = g.Max(a => a.Date) })
                .FirstOrDefaultAsync();

            if (range != null)
            {
                var minDate = range.Min.Date;
                var maxDate = range.Max.Date;

                // pass to view (to set min/max in the calendar too)
                ViewBag.MinPlanDate = minDate;
                ViewBag.MaxPlanDate = maxDate;
                ViewBag.Today = today;

                var invalid =
                    selectedDate > today ||
                    selectedDate < minDate ||
                    selectedDate > maxDate;

                if (invalid)
                {
                    // show message and stop (no cards)
                    vm.Items = new List<FoodIntakeCardVM>();
                    ViewBag.DateError = "Choose a valid Date";
                    return View(vm);
                }
            }
            else
            {
                // no plan days at all -> invalid date selection in practice
                vm.Items = new List<FoodIntakeCardVM>();
                ViewBag.DateError = "Choose a valid Date";
                return View(vm);
            }


            // 2) Load Daily Plan
            var dayPlan = await _context.FoodPlanDay
                .AsNoTracking()
                .Include(x => x.Food)
                .Include(x => x.Portion)
                .Where(x => x.PlanId == plan.PlanId && x.Date == selectedDate)

                .OrderBy(x => x.ScheduledTime)
                .ThenBy(x => x.Food != null ? x.Food.Name : "")
                .ToListAsync();

            // 3) FoodIntake Validation
            var intakesMap = await EnsureDayIntakesAsync(plan.PlanId, selectedDate, dayPlan);

            // 4) Portion list
            vm.Portions = await _context.Portion
                .AsNoTracking()
                .OrderBy(p => p.PortionName)
                .Select(p => new SelectListItem
                {
                    Value = p.PortionId.ToString(),
                    Text = p.PortionName
                })
                .ToListAsync();

            // 5) cards
            vm.Items = dayPlan.Select(line =>
            {
                var fi = intakesMap[line.FoodId];

                return new FoodIntakeCardVM
                {
                    FoodIntakeId = fi.FoodIntakeId,
                    FoodId = line.FoodId,
                    FoodName = line.Food?.Name ?? "Food",
                    PortionName = line.Portion?.PortionName,
                    PortionsPlanned = fi.PortionsPlanned,
                    PortionsEaten = fi.PortionsEaten,
                    IsConsumed = fi.PortionsPlanned > 0 && fi.PortionsEaten >= fi.PortionsPlanned,
                    IconClass = GuessFoodIcon(line.Food?.Name),
                    Note = $"Portions: {fi.PortionsEaten}/{fi.PortionsPlanned}"
                };
            }).ToList();

            // 6) dashboard 
            var totals = await CalculateDayTotalsAsync(plan.PlanId, selectedDate);
            vm.CaloriesConsumed = totals.cal;
            vm.ProteinConsumed = totals.prot;
            vm.FatConsumed = totals.fat;
            vm.HydratesConsumed = totals.carb;

            return View(vm);
        }

        // =========================================
        // POST: FoodIntake/SetPortionsEaten
        // =========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPortionsEaten([FromBody] SetPortionsEatenDto dto)
        {
            if (dto.ClientId <= 0 || dto.FoodId <= 0)
                return BadRequest("Invalid data.");

            var day = dto.Date.Date;

            var plan = await GetClientPlanAsync(dto.ClientId);
            if (plan == null) return NotFound("Client has no plan.");

            // linha do plano do dia
            var line = await _context.FoodPlanDay
                .FirstOrDefaultAsync(x => x.PlanId == plan.PlanId && x.Date.Date == day.Date && x.FoodId == dto.FoodId);

            if (line == null) return BadRequest("Food is not on the day plan.");

            // intake
            var fi = await _context.FoodIntake
                .FirstOrDefaultAsync(x => x.PlanId == plan.PlanId && x.Date.Date == day.Date && x.FoodId == dto.FoodId);

            if (fi == null)
            {
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

            // alinhar snapshot com plano do dia
            fi.PortionId = line.PortionId;
            fi.PortionsPlanned = line.PortionsPlanned;

            // clamp
            fi.PortionsEaten = Math.Max(0, Math.Min(dto.PortionsEaten, fi.PortionsPlanned));

            await _context.SaveChangesAsync();

            // recalcular dashboard
            var totals = await CalculateDayTotalsAsync(plan.PlanId, day);

            return Json(new
            {
                foodIntakeId = fi.FoodIntakeId,
                portionsPlanned = fi.PortionsPlanned,
                portionsEaten = fi.PortionsEaten,
                isConsumed = fi.PortionsPlanned > 0 && fi.PortionsEaten >= fi.PortionsPlanned,
                note = $"Portions: {fi.PortionsEaten}/{fi.PortionsPlanned}",
                totals = new
                {
                    calories = totals.cal,
                    protein = totals.prot,
                    fat = totals.fat,
                    hydrates = totals.carb
                }
            });
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
                return BadRequest("Invalid data.");

            var plan = await GetClientPlanAsync(dto.ClientId);
            if (plan == null) return NotFound("Client has no plan.");

            var day = dto.Date.Date;

            if (!dto.PortionId.HasValue || dto.PortionId.Value <= 0)
                return BadRequest("PortionId is required.");

            var portionsPlanned = Math.Max(1, dto.PortionsPlanned);

            // 1) FoodPlanDay (evitar duplicados)
            var line = await _context.FoodPlanDay
                .FirstOrDefaultAsync(x => x.PlanId == plan.PlanId && x.Date == day && x.FoodId == dto.FoodId);


            if (line == null)
            {
                line = new FoodPlanDay
                {
                    PlanId = plan.PlanId,
                    FoodId = dto.FoodId,
                    PortionId = dto.PortionId.Value,
                    Date = day,
                    PortionsPlanned = portionsPlanned,
                    ScheduledTime = DateTime.Now
                };

                _context.FoodPlanDay.Add(line);
                await _context.SaveChangesAsync();
            }
            else
            {
                // se já existe, alinha porção/porções se quiseres:
                line.PortionId = dto.PortionId.Value;
                line.PortionsPlanned = portionsPlanned;
                await _context.SaveChangesAsync();
            }

            // 2) FoodIntake do dia
            var fi = await _context.FoodIntake
                .FirstOrDefaultAsync(x => x.PlanId == plan.PlanId && x.FoodId == dto.FoodId && x.Date == day);

            if (fi == null)
            {
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
                await _context.SaveChangesAsync();
            }
            else
            {
                // alinhar snapshot com o plano do dia
                fi.PortionId = line.PortionId;
                fi.PortionsPlanned = line.PortionsPlanned;
                await _context.SaveChangesAsync();
            }

            var portionName = await _context.Portion
                .Where(p => p.PortionId == fi.PortionId)
                .Select(p => p.PortionName)
                .FirstOrDefaultAsync();

            return Json(new
            {
                foodIntakeId = fi.FoodIntakeId,
                portionName,
                portionsPlanned = fi.PortionsPlanned,
                portionsEaten = fi.PortionsEaten,
                note = $"Portions: {fi.PortionsEaten}/{fi.PortionsPlanned}"
            });
        }

        // =========================
        // Internals: Ensure Day Plan
        // =========================
        private async Task EnsureDayPlanAsync(int planId, DateTime day)
        {
            day = day.Date;

            var baseCount = await _context.FoodPlan
                .AsNoTracking()
                .CountAsync(x => x.PlanId == planId);

            if (baseCount <= 0) return;

            var dayCount = await _context.FoodPlanDay
                .AsNoTracking()
                .CountAsync(x => x.PlanId == planId && x.Date == day);

            if (dayCount >= baseCount) return;

            var baseLines = await _context.FoodPlan
                .AsNoTracking()
                .Where(x => x.PlanId == planId)
                .ToListAsync();

            var existingFoodIds = await _context.FoodPlanDay
                .AsNoTracking()
                .Where(x => x.PlanId == planId && x.Date == day)
                .Select(x => x.FoodId)
                .ToListAsync();

            var missing = baseLines
                .Where(x => !existingFoodIds.Contains(x.FoodId))
                .Select(x => new FoodPlanDay
                {
                    PlanId = planId,
                    FoodId = x.FoodId,
                    PortionId = x.PortionId,
                    Date = day,
                    PortionsPlanned = 1,
                    ScheduledTime = DateTime.Now
                })
                .ToList();

            if (missing.Count == 0) return;

            _context.FoodPlanDay.AddRange(missing);
            await _context.SaveChangesAsync();
        }


        // =========================
        // Internals: Ensure Day Intakes
        // =========================
        private async Task<Dictionary<int, FoodIntake>> EnsureDayIntakesAsync(
            int planId,
            DateTime day,
            List<FoodPlanDay> dayPlan)
        {
            var dayIntakes = await _context.FoodIntake
                .Where(x => x.PlanId == planId && x.Date.Date == day.Date)
                .ToListAsync();

            var map = dayIntakes.ToDictionary(x => x.FoodId, x => x);

            bool changed = false;

            foreach (var line in dayPlan)
            {
                if (!map.TryGetValue(line.FoodId, out var fi))
                {
                    fi = new FoodIntake
                    {
                        PlanId = planId,
                        FoodId = line.FoodId,
                        PortionId = line.PortionId,
                        Date = day.Date,
                        ScheduledTime = line.ScheduledTime ?? DateTime.Now,
                        PortionsPlanned = line.PortionsPlanned,
                        PortionsEaten = 0
                    };
                    _context.FoodIntake.Add(fi);
                    map[line.FoodId] = fi;
                    changed = true;
                }
                else
                {
                    // alinhar snapshot com o plano do dia
                    if (fi.PortionId != line.PortionId)
                    {
                        fi.PortionId = line.PortionId;
                        changed = true;
                    }

                    if (fi.PortionsPlanned != line.PortionsPlanned)
                    {
                        fi.PortionsPlanned = line.PortionsPlanned;
                        // clamp eaten se planeado diminuiu
                        if (fi.PortionsEaten > fi.PortionsPlanned)
                            fi.PortionsEaten = fi.PortionsPlanned;
                        changed = true;
                    }
                }
            }

            if (changed)
                await _context.SaveChangesAsync();

            return map;
        }



        // =========================
        // ToggleConsumed
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleConsumed([FromBody] ToggleConsumedDto dto)
        {
            if (dto.ClientId <= 0 || dto.FoodId <= 0)
                return BadRequest("Invalid data.");

            var plan = await GetClientPlanAsync(dto.ClientId);
            if (plan == null) return NotFound("Client has no plan.");

            var day = dto.Date.Date;

            // Garantir que o alimento existe no plano do dia
            var line = await _context.FoodPlanDay
                .FirstOrDefaultAsync(x => x.PlanId == plan.PlanId
                                      && x.FoodId == dto.FoodId
                                      && x.Date.Date == day);

            if (line == null)
                return BadRequest("Food is not on the day plan.");

            // Buscar (ou criar) o FoodIntake do dia
            var fi = await _context.FoodIntake
                .FirstOrDefaultAsync(x => x.PlanId == plan.PlanId
                                      && x.FoodId == dto.FoodId
                                      && x.Date.Date == day);

            if (fi == null)
            {
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

            // alinhar sempre com o plano do dia
            fi.PortionId = line.PortionId;
            fi.PortionsPlanned = line.PortionsPlanned;

            // Toggle: se consumir => come tudo; se não => 0
            fi.PortionsEaten = dto.Consumed ? fi.PortionsPlanned : 0;

            await _context.SaveChangesAsync();

            return Json(new
            {
                foodIntakeId = fi.FoodIntakeId,
                portionsPlanned = fi.PortionsPlanned,
                portionsEaten = fi.PortionsEaten,
                isConsumed = fi.PortionsEaten >= fi.PortionsPlanned,
                note = $"Portions: {fi.PortionsEaten}/{fi.PortionsPlanned}"
            });
        }


        // =========================
        // Dashboard totals
        // =========================
        private async Task<(double cal, double prot, double fat, double carb)>
        CalculateDayTotalsAsync(int planId, DateTime day)
        {
            var intakes = await _context.FoodIntake
                .AsNoTracking()
                .Where(x => x.PlanId == planId && x.Date.Date == day.Date)
                .Select(x => new { x.FoodId, x.PortionsEaten })
                .ToListAsync();

            if (!intakes.Any())
                return (0, 0, 0, 0);

            var foodIds = intakes.Select(x => x.FoodId).Distinct().ToList();

            var components = await _context.FoodNutritionalComponent
                .AsNoTracking()
                .Include(x => x.NutritionalComponent)
                .Where(x => foodIds.Contains(x.FoodId))
                .ToListAsync();

            double cal = 0, prot = 0, fat = 0, carb = 0;

            foreach (var fi in intakes)
            {
                if (fi.PortionsEaten <= 0) continue;

                var foodComps = components.Where(c => c.FoodId == fi.FoodId);

                foreach (var c in foodComps)
                {
                    var name = (c.NutritionalComponent?.Name ?? "").ToLowerInvariant();
                    var v = c.Value * fi.PortionsEaten;

                    // robust mapping
                    if (name.Contains("energy") || name.Contains("calorie") || name == "kcal") cal += v;
                    else if (name.Contains("protein")) prot += v;
                    else if (name.Contains("fat") || name.Contains("lipid")) fat += v;
                    else if (name.Contains("carb") || name.Contains("hydrate") || name.Contains("carbo")) carb += v;
                }
            }

            return (cal, prot, fat, carb);
        }

        // =========================
        // Helpers
        // =========================
        private async Task<Plan?> GetClientPlanAsync(int clientId)
        {
            return await _context.Plan.FirstOrDefaultAsync(p => p.ClientId == clientId);
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

        // DTOs
        public class SetPortionsEatenDto
        {
            public int ClientId { get; set; }
            public int FoodId { get; set; }
            public DateTime Date { get; set; }
            public int PortionsEaten { get; set; }
            public int? FoodIntakeId { get; set; }
        }

        public class AddFoodDto
        {
            public int ClientId { get; set; }
            public int FoodId { get; set; }
            public DateTime Date { get; set; }
            public int? PortionId { get; set; }
            public int PortionsPlanned { get; set; } = 1;
        }
    }

    public class ToggleConsumedDto
    {
        public int ClientId { get; set; }
        public int FoodId { get; set; }
        public DateTime Date { get; set; }
        public bool Consumed { get; set; }
    }


}
