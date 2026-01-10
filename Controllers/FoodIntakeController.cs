using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;

namespace HealthWellbeing.Controllers
{
    [Authorize]
    public class FoodIntakeController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public FoodIntakeController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        private bool IsNutritionist() => User.IsInRole("Nutricionista") || User.IsInRole("Nutritionist");
        private bool IsAdmin() => User.IsInRole("Administrador") || User.IsInRole("Administrator");
        private bool IsClient() => User.IsInRole("Cliente") || User.IsInRole("Client");

        private IActionResult NoDataPermission() => View("~/Views/Shared/NoDataPermission.cshtml");

        private async Task<int?> GetCurrentClientIdAsync()
        {
            var email = User?.Identity?.Name;
            if (string.IsNullOrWhiteSpace(email)) return null;

            return await _context.Client
                .AsNoTracking()
                .Where(c => c.Email == email)
                .Select(c => (int?)c.ClientId)
                .FirstOrDefaultAsync();
        }

        public async Task<IActionResult> Index(int? clientId, DateTime? date)
        {
            if (IsAdmin()) return NoDataPermission();

            ViewBag.CanEditIntake = IsClient();
            ViewBag.ShowAddCard = IsClient();
            ViewBag.ShowClientSelector = IsNutritionist();
            ViewBag.CanEdit = IsClient();

            var selectedDate = (date ?? DateTime.Today).Date;

            int resolvedClientId = 0;

            if (IsClient())
            {
                var myId = await GetCurrentClientIdAsync();
                if (myId == null) return NoDataPermission();
                resolvedClientId = myId.Value;
            }
            else
            {
                resolvedClientId = clientId.GetValueOrDefault(0);
            }

            var vm = new FoodIntakeIndexVM
            {
                SelectedDate = selectedDate,
                SelectedClientId = resolvedClientId,
                Clients = IsNutritionist() ? await LoadClientsAsync(resolvedClientId) : new List<SelectListItem>(),
                AvailableFoods = IsClient() ? await LoadFoodsAsync() : new List<SelectListItem>(),
                Items = new List<FoodIntakeCardVM>()
            };

            if (resolvedClientId <= 0)
                return View(vm);

            vm.SelectedClientName = await GetClientNameAsync(resolvedClientId);

            var plan = await GetClientPlanAsync(resolvedClientId);
            if (plan == null)
                return View(vm);

            var goal = await _context.Goal
                .AsNoTracking()
                .Where(g => g.ClientId == resolvedClientId)
                .OrderByDescending(g => g.GoalId)
                .FirstOrDefaultAsync();

            if (goal != null)
            {
                vm.DailyCaloriesGoal = goal.DailyCalories;
                vm.DailyProteinGoal = goal.DailyProtein;
                vm.DailyFatGoal = goal.DailyFat;
                vm.DailyHydratesGoal = goal.DailyHydrates;
            }

            await EnsureDayPlanAsync(plan.FoodHabitsPlanId, selectedDate);

            var today = DateTime.Today;

            var range = await _context.FoodPlanDay
                .AsNoTracking()
                .Where(x => x.PlanId == plan.FoodHabitsPlanId)
                .GroupBy(x => 1)
                .Select(g => new { Min = g.Min(a => a.Date), Max = g.Max(a => a.Date) })
                .FirstOrDefaultAsync();

            if (range == null)
            {
                ViewBag.DateError = "Choose a valid Date";
                return View(vm);
            }

            var minDate = range.Min.Date;
            var maxDate = range.Max.Date;

            ViewBag.MinPlanDate = minDate;
            ViewBag.MaxPlanDate = maxDate;
            ViewBag.Today = today;

            var invalid = selectedDate > today || selectedDate < minDate || selectedDate > maxDate;
            if (invalid)
            {
                ViewBag.DateError = "Choose a valid Date";
                return View(vm);
            }

            var dayPlan = await _context.FoodPlanDay
                .AsNoTracking()
                .Include(x => x.Food)
                .Include(x => x.Portion)
                .Where(x => x.PlanId == plan.FoodHabitsPlanId && x.Date == selectedDate)
                .OrderBy(x => x.ScheduledTime)
                .ThenBy(x => x.Food != null ? x.Food.Name : "")
                .ToListAsync();

            var intakesMap = await EnsureDayIntakesAsync(plan.FoodHabitsPlanId, selectedDate, dayPlan);

            vm.Portions = await _context.Portion
                .AsNoTracking()
                .OrderBy(p => p.PortionName)
                .Select(p => new SelectListItem
                {
                    Value = p.PortionId.ToString(),
                    Text = p.PortionName
                })
                .ToListAsync();

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

            var totals = await CalculateDayTotalsAsync(plan.FoodHabitsPlanId, selectedDate);
            vm.CaloriesConsumed = totals.cal;
            vm.ProteinConsumed = totals.prot;
            vm.FatConsumed = totals.fat;
            vm.HydratesConsumed = totals.carb;

            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Cliente,Client")]
        public async Task<IActionResult> SetPortionsEaten([FromBody] SetPortionsEatenDto dto)
        {
            if (IsAdmin()) return Forbid();
            if (dto.FoodId <= 0) return BadRequest("Invalid data.");

            var myId = await GetCurrentClientIdAsync();
            if (myId == null) return Forbid();

            var day = dto.Date.Date;

            var plan = await GetClientPlanAsync(myId.Value);
            if (plan == null) return NotFound("Client has no plan.");

            var line = await _context.FoodPlanDay
                .FirstOrDefaultAsync(x => x.PlanId == plan.FoodHabitsPlanId && x.Date.Date == day && x.FoodId == dto.FoodId);

            if (line == null) return BadRequest("Food is not on the day plan.");

            var fi = await _context.FoodIntake
                .FirstOrDefaultAsync(x => x.PlanId == plan.FoodHabitsPlanId && x.Date.Date == day && x.FoodId == dto.FoodId);

            if (fi == null)
            {
                fi = new FoodIntake
                {
                    PlanId = plan.FoodHabitsPlanId,
                    FoodId = dto.FoodId,
                    PortionId = line.PortionId,
                    Date = day,
                    ScheduledTime = line.ScheduledTime ?? DateTime.Now,
                    PortionsPlanned = line.PortionsPlanned,
                    PortionsEaten = 0
                };
                _context.FoodIntake.Add(fi);
            }

            fi.PortionId = line.PortionId;
            fi.PortionsPlanned = line.PortionsPlanned;
            fi.PortionsEaten = Math.Max(0, Math.Min(dto.PortionsEaten, fi.PortionsPlanned));

            await _context.SaveChangesAsync();

            var totals = await CalculateDayTotalsAsync(plan.FoodHabitsPlanId, day);

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Cliente,Client")]
        public async Task<IActionResult> AddFood([FromBody] AddFoodDto dto)
        {
            if (IsAdmin()) return Forbid();
            if (dto.FoodId <= 0) return BadRequest("Invalid data.");
            if (!dto.PortionId.HasValue || dto.PortionId.Value <= 0) return BadRequest("PortionId is required.");

            var myId = await GetCurrentClientIdAsync();
            if (myId == null) return Forbid();

            var plan = await GetClientPlanAsync(myId.Value);
            if (plan == null) return NotFound("Client has no plan.");

            var day = dto.Date.Date;
            var portionsPlanned = Math.Max(1, dto.PortionsPlanned);

            var line = await _context.FoodPlanDay
                .FirstOrDefaultAsync(x => x.PlanId == plan.FoodHabitsPlanId && x.Date == day && x.FoodId == dto.FoodId);

            if (line == null)
            {
                line = new FoodPlanDay
                {
                    PlanId = plan.FoodHabitsPlanId,
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
                line.PortionId = dto.PortionId.Value;
                line.PortionsPlanned = portionsPlanned;
                await _context.SaveChangesAsync();
            }

            var fi = await _context.FoodIntake
                .FirstOrDefaultAsync(x => x.PlanId == plan.FoodHabitsPlanId && x.FoodId == dto.FoodId && x.Date == day);

            if (fi == null)
            {
                fi = new FoodIntake
                {
                    PlanId = plan.FoodHabitsPlanId,
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
                fi.PortionId = line.PortionId;
                fi.PortionsPlanned = line.PortionsPlanned;
                await _context.SaveChangesAsync();
            }

            var portionName = await _context.Portion
                .AsNoTracking()
                .Where(p => p.PortionId == fi.PortionId)
                .Select(p => p.PortionName)
                .FirstOrDefaultAsync();

            var totals = await CalculateDayTotalsAsync(plan.FoodHabitsPlanId, day);

            return Json(new
            {
                foodIntakeId = fi.FoodIntakeId,
                portionName,
                portionsPlanned = fi.PortionsPlanned,
                portionsEaten = fi.PortionsEaten,
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Cliente,Client")]
        public async Task<IActionResult> ToggleConsumed([FromBody] ToggleConsumedDto dto)
        {
            if (IsAdmin()) return Forbid();
            if (dto.FoodId <= 0) return BadRequest("Invalid data.");

            var myId = await GetCurrentClientIdAsync();
            if (myId == null) return Forbid();

            var plan = await GetClientPlanAsync(myId.Value);
            if (plan == null) return NotFound("Client has no plan.");

            var day = dto.Date.Date;

            var line = await _context.FoodPlanDay
                .FirstOrDefaultAsync(x => x.PlanId == plan.FoodHabitsPlanId && x.FoodId == dto.FoodId && x.Date.Date == day);

            if (line == null) return BadRequest("Food is not on the day plan.");

            var fi = await _context.FoodIntake
                .FirstOrDefaultAsync(x => x.PlanId == plan.FoodHabitsPlanId && x.FoodId == dto.FoodId && x.Date.Date == day);

            if (fi == null)
            {
                fi = new FoodIntake
                {
                    PlanId = plan.FoodHabitsPlanId,
                    FoodId = dto.FoodId,
                    PortionId = line.PortionId,
                    Date = day,
                    ScheduledTime = line.ScheduledTime ?? DateTime.Now,
                    PortionsPlanned = line.PortionsPlanned,
                    PortionsEaten = 0
                };
                _context.FoodIntake.Add(fi);
            }

            fi.PortionId = line.PortionId;
            fi.PortionsPlanned = line.PortionsPlanned;
            fi.PortionsEaten = dto.Consumed ? fi.PortionsPlanned : 0;

            await _context.SaveChangesAsync();

            var totals = await CalculateDayTotalsAsync(plan.FoodHabitsPlanId, day);

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

        private async Task<Dictionary<int, FoodIntake>> EnsureDayIntakesAsync(int planId, DateTime day, List<FoodPlanDay> dayPlan)
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
                    if (fi.PortionId != line.PortionId)
                    {
                        fi.PortionId = line.PortionId;
                        changed = true;
                    }

                    if (fi.PortionsPlanned != line.PortionsPlanned)
                    {
                        fi.PortionsPlanned = line.PortionsPlanned;
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

        private async Task<(double cal, double prot, double fat, double carb)> CalculateDayTotalsAsync(int planId, DateTime day)
        {
            day = day.Date;

            var rows = await (
                from fi in _context.FoodIntake.AsNoTracking()
                join fnc in _context.FoodNutritionalComponent.AsNoTracking()
                    on fi.FoodId equals fnc.FoodId
                where fi.PlanId == planId
                      && fi.Date.Date == day
                      && fi.PortionsEaten > 0
                select new
                {
                    Portions = fi.PortionsEaten,
                    Name = (fnc.NutritionalComponent!.Name ?? "").ToLower(),
                    Value = fnc.Value
                }
            ).ToListAsync();

            double cal = 0, prot = 0, fat = 0, carb = 0;

            foreach (var r in rows)
            {
                var v = r.Value * r.Portions;

                if (r.Name.Contains("energy") || r.Name.Contains("calorie") || r.Name == "kcal") cal += v;
                else if (r.Name.Contains("protein")) prot += v;
                else if (r.Name.Contains("fat") || r.Name.Contains("lipid")) fat += v;
                else if (r.Name.Contains("carb") || r.Name.Contains("hydrate") || r.Name.Contains("carbo")) carb += v;
            }

            return (cal, prot, fat, carb);
        }

        private async Task<FoodHabitsPlan?> GetClientPlanAsync(int clientId)
        {
            return await _context.FoodHabitsPlan.FirstOrDefaultAsync(p => p.ClientId == clientId);
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

        public class SetPortionsEatenDto
        {
            public int FoodId { get; set; }
            public DateTime Date { get; set; }
            public int PortionsEaten { get; set; }
            public int? FoodIntakeId { get; set; }
        }

        public class AddFoodDto
        {
            public int FoodId { get; set; }
            public DateTime Date { get; set; }
            public int? PortionId { get; set; }
            public int PortionsPlanned { get; set; } = 1;
        }

        public class ToggleConsumedDto
        {
            public int FoodId { get; set; }
            public DateTime Date { get; set; }
            public bool Consumed { get; set; }
        }
    }
}
