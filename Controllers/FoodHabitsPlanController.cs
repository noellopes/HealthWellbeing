using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;

namespace HealthWellbeing.Controllers
{
    [Authorize]
    public class FoodHabitsPlanController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public FoodHabitsPlanController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        private bool IsNutritionist()
            => User.IsInRole("Nutricionista") || User.IsInRole("Nutritionist");

        private bool IsAdmin()
            => User.IsInRole("Administrador") || User.IsInRole("Administrator");

        private bool IsClient()
            => User.IsInRole("Cliente") || User.IsInRole("Client");

        private IActionResult NoDataPermission()
            => View("~/Views/Shared/NoDataPermission.cshtml");

        private static string GetPlanStatus(DateTime today, DateTime start, DateTime end)
        {
            var t = today.Date;
            var s = start.Date;
            var e = end.Date;

            if (t < s) return "Pending";
            if (t > e) return "Finished";
            return "Active";
        }

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

        public async Task<IActionResult> Index(string? search, int page = 1, int itemsPerPage = 10)
        {
            if (IsAdmin()) return NoDataPermission();

            var today = DateTime.Today.Date;

            var query = _context.FoodHabitsPlan
                .Include(p => p.Client)
                .Include(p => p.Goal)
                .AsQueryable();

            if (IsClient())
            {
                var clientId = await GetCurrentClientIdAsync();
                if (clientId == null) return NoDataPermission();

                query = query.Where(p => p.ClientId == clientId.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLowerInvariant();

                query = query.Where(p =>
                    (p.Client != null && p.Client.Name.ToLower().Contains(s)) ||
                    (p.Goal != null && p.Goal.GoalName.ToLower().Contains(s)) ||
                    p.StartingDate.ToString().ToLower().Contains(s) ||
                    p.EndingDate.ToString().ToLower().Contains(s) ||
                    (s == "pending" && today < p.StartingDate.Date) ||
                    (s == "finished" && today > p.EndingDate.Date) ||
                    ((s == "active" || s == "completed") && today >= p.StartingDate.Date && today <= p.EndingDate.Date)
                );
            }

            int totalItems = await query.CountAsync();

            var items = await query
                .OrderByDescending(p => p.StartingDate)
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToListAsync();

            ViewBag.Search = search ?? "";
            return View(new PaginationInfoFoodHabits<FoodHabitsPlan>(items, totalItems, page, itemsPerPage));
        }

        [HttpGet]
        [Authorize(Roles = "Nutricionista,Nutritionist")]
        public async Task<IActionResult> GetGoals(int clientId)
        {
            if (IsAdmin()) return NoDataPermission();

            var goals = await _context.Goal
                .AsNoTracking()
                .Where(g => g.ClientId == clientId)
                .OrderByDescending(g => g.GoalId)
                .Select(g => new { g.GoalId, g.GoalName })
                .ToListAsync();

            return Json(goals);
        }

        [HttpGet]
        [Authorize(Roles = "Nutricionista,Nutritionist")]
        public async Task<IActionResult> GetClientAlergies(int clientId)
        {
            if (IsAdmin()) return NoDataPermission();

            var al = await _context.ClientAlergy
                .AsNoTracking()
                .Where(ca => ca.ClientId == clientId)
                .Include(ca => ca.Alergy)
                .Select(ca => new { ca.AlergyId, Name = ca.Alergy!.AlergyName })
                .ToListAsync();

            return Json(al);
        }

        public async Task<IActionResult> Details(int? id, DateTime? selectedDate)
        {
            if (id == null) return NotFound();

            var plan = await _context.FoodHabitsPlan
                .Include(p => p.Client)
                    .ThenInclude(c => c!.ClientAlergies!)
                        .ThenInclude(ca => ca.Alergy)
                .Include(p => p.Goal)
                .FirstOrDefaultAsync(p => p.FoodHabitsPlanId == id);

            if (plan == null) return NotFound();

            var minPlanDate = plan.StartingDate.Date;
            var maxPlanDate = plan.EndingDate.Date;

            var day = (selectedDate ?? DateTime.Today).Date;
            if (day < minPlanDate) day = minPlanDate;
            if (day > maxPlanDate) day = maxPlanDate;

            ViewBag.PlanStatus = GetPlanStatus(DateTime.Today, plan.StartingDate, plan.EndingDate);

            var planned = await _context.FoodPlanDay
                .AsNoTracking()
                .Where(x => x.PlanId == plan.FoodHabitsPlanId && x.Date.Date == day)
                .Include(x => x.Food)
                    .ThenInclude(f => f!.Category)
                .Include(x => x.Portion)
                .ToListAsync();

            var intakes = await _context.FoodIntake
                .AsNoTracking()
                .Where(x => x.PlanId == plan.FoodHabitsPlanId && x.Date.Date == day)
                .ToListAsync();

            var dayFoods = planned.Select(p =>
            {
                var intake = intakes.FirstOrDefault(i =>
                    i.FoodId == p.FoodId &&
                    i.PortionId == p.PortionId &&
                    i.Date.Date == day);

                var eaten = intake?.PortionsEaten ?? 0;
                var plannedQty = intake?.PortionsPlanned ?? p.PortionsPlanned;

                return new
                {
                    FoodName = p.Food?.Name ?? "",
                    CategoryName = p.Food?.Category?.Category ?? "",
                    PortionName = p.Portion?.PortionName ?? "",
                    PortionsPlanned = plannedQty,
                    PortionsEaten = eaten,
                    Consumed = eaten > 0
                };
            }).ToList();

            int totalItems = dayFoods.Count;
            int consumedCount = dayFoods.Count(x => x.Consumed);
            int notConsumedCount = totalItems - consumedCount;

            double consumedPercentage = totalItems == 0 ? 0 : Math.Round(consumedCount * 100.0 / totalItems, 1);
            double notConsumedPercentage = totalItems == 0 ? 0 : Math.Round(100.0 - consumedPercentage, 1);

            int dayPlanned = dayFoods.Sum(x => (int)x.PortionsPlanned);
            int dayEaten = dayFoods.Sum(x => (int)x.PortionsEaten);

            ViewBag.DayPlanned = dayPlanned;
            ViewBag.DayEaten = dayEaten;

            string dailyStatus;
            if (dayPlanned <= 0) dailyStatus = "No plan for this day";
            else if (dayEaten <= 0) dailyStatus = "Not started";
            else if (dayEaten >= dayPlanned) dailyStatus = "Completed";
            else dailyStatus = "In progress";

            ViewBag.DailyStatus = dailyStatus;

            var plannedByDay = await _context.FoodPlanDay
                .AsNoTracking()
                .Where(x => x.PlanId == plan.FoodHabitsPlanId && x.Date.Date >= minPlanDate && x.Date.Date <= maxPlanDate)
                .GroupBy(x => x.Date.Date)
                .Select(g => new { Day = g.Key, Total = g.Count() })
                .ToListAsync();

            var eatenByDay = await _context.FoodIntake
                .AsNoTracking()
                .Where(x => x.PlanId == plan.FoodHabitsPlanId && x.Date.Date >= minPlanDate && x.Date.Date <= maxPlanDate && x.PortionsEaten > 0)
                .GroupBy(x => x.Date.Date)
                .Select(g => new { Day = g.Key, Done = g.Count() })
                .ToListAsync();

            var plannedDict = plannedByDay.ToDictionary(x => x.Day, x => x.Total);
            var eatenDict = eatenByDay.ToDictionary(x => x.Day, x => x.Done);

            var progressVm = new List<object>();
            for (var d = minPlanDate; d <= maxPlanDate; d = d.AddDays(1))
            {
                plannedDict.TryGetValue(d, out var total);
                eatenDict.TryGetValue(d, out var done);

                var pct = total == 0 ? 0 : Math.Round(done * 100.0 / total, 1);

                progressVm.Add(new
                {
                    Label = d.ToString("MM-dd"),
                    Percentage = pct
                });
            }

            ViewBag.SelectedDate = day;
            ViewBag.MinPlanDate = minPlanDate;
            ViewBag.MaxPlanDate = maxPlanDate;

            ViewBag.DayFoods = dayFoods;

            ViewBag.TotalItems = totalItems;
            ViewBag.ConsumedCount = consumedCount;
            ViewBag.NotConsumedCount = notConsumedCount;
            ViewBag.ConsumedPercentage = consumedPercentage;
            ViewBag.NotConsumedPercentage = notConsumedPercentage;

            ViewBag.Progress = progressVm;

            return View(plan);
        }

        private async Task RegenerateFoodPlanDaysAsync(
            int planId,
            DateTime startDate,
            DateTime endDate,
            List<FoodHabitsPlanFormVM.FoodLineVM> selectedFoods)
        {
            var existing = await _context.FoodPlanDay.Where(x => x.PlanId == planId).ToListAsync();
            if (existing.Count > 0) _context.FoodPlanDay.RemoveRange(existing);

            var days = new List<FoodPlanDay>();
            var start = startDate.Date;
            var end = endDate.Date;

            for (var d = start; d <= end; d = d.AddDays(1))
            {
                foreach (var f in selectedFoods)
                {
                    days.Add(new FoodPlanDay
                    {
                        PlanId = planId,
                        FoodId = f.FoodId,
                        PortionId = f.PortionId,
                        Date = d,
                        PortionsPlanned = 1,
                        ScheduledTime = null,
                        MealType = null
                    });
                }
            }

            if (days.Count > 0) _context.FoodPlanDay.AddRange(days);
        }

        [Authorize(Roles = "Nutricionista,Nutritionist")]
        public async Task<IActionResult> Create()
        {
            if (IsAdmin()) return NoDataPermission();

            var vm = new FoodHabitsPlanFormVM
            {
                StartingDate = DateTime.Today,
                EndingDate = DateTime.Today.AddDays(7)
            };

            await LoadFormListsAsync(vm.ClientId);
            vm.Foods = await BuildFoodsVMAsync(selected: null);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Nutricionista,Nutritionist")]
        public async Task<IActionResult> Create(FoodHabitsPlanFormVM vm)
        {
            if (IsAdmin()) return NoDataPermission();

            if (vm.EndingDate.Date < vm.StartingDate.Date)
                ModelState.AddModelError(nameof(vm.EndingDate), "Ending date must be after (or equal to) starting date.");

            if (!await _context.Client.AnyAsync(c => c.ClientId == vm.ClientId))
                ModelState.AddModelError(nameof(vm.ClientId), "Select a valid client.");

            if (!await _context.Goal.AnyAsync(g => g.GoalId == vm.GoalId && g.ClientId == vm.ClientId))
                ModelState.AddModelError(nameof(vm.GoalId), "Select a valid goal for that client.");

            var selectedFoods = vm.Foods.Where(f => f.Selected).ToList();
            if (selectedFoods.Count == 0)
                ModelState.AddModelError("", "Select at least 1 food.");

            if (!ModelState.IsValid)
            {
                await LoadFormListsAsync(vm.ClientId);
                vm.Foods = await BuildFoodsVMAsync(selectedFoods);
                return View(vm);
            }

            var plan = new FoodHabitsPlan
            {
                ClientId = vm.ClientId,
                GoalId = vm.GoalId,
                StartingDate = vm.StartingDate.Date,
                EndingDate = vm.EndingDate.Date
            };

            _context.FoodHabitsPlan.Add(plan);
            await _context.SaveChangesAsync();

            _context.FoodPlan.AddRange(selectedFoods.Select(f => new PlanFood
            {
                PlanId = plan.FoodHabitsPlanId,
                FoodId = f.FoodId,
                PortionId = f.PortionId
            }));

            await RegenerateFoodPlanDaysAsync(
                plan.FoodHabitsPlanId,
                plan.StartingDate,
                plan.EndingDate,
                selectedFoods);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Nutricionista,Nutritionist")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (IsAdmin()) return NoDataPermission();
            if (id == null) return NotFound();

            var plan = await _context.FoodHabitsPlan
                .Include(p => p.PlanFoods)
                .FirstOrDefaultAsync(p => p.FoodHabitsPlanId == id);

            if (plan == null) return NotFound();

            var vm = new FoodHabitsPlanFormVM
            {
                FoodHabitsPlanId = plan.FoodHabitsPlanId,
                ClientId = plan.ClientId,
                GoalId = plan.GoalId,
                StartingDate = plan.StartingDate,
                EndingDate = plan.EndingDate
            };

            await LoadFormListsAsync(vm.ClientId);

            var selectedFoods = plan.PlanFoods?.Select(fp => new FoodHabitsPlanFormVM.FoodLineVM
            {
                Selected = true,
                FoodId = fp.FoodId,
                PortionId = fp.PortionId
            }).ToList();

            vm.Foods = await BuildFoodsVMAsync(selectedFoods);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Nutricionista,Nutritionist")]
        public async Task<IActionResult> Edit(int id, FoodHabitsPlanFormVM vm)
        {
            if (IsAdmin()) return NoDataPermission();
            if (vm.FoodHabitsPlanId != id) return NotFound();

            if (vm.EndingDate.Date < vm.StartingDate.Date)
                ModelState.AddModelError(nameof(vm.EndingDate), "Ending date must be after (or equal to) starting date.");

            if (!await _context.Client.AnyAsync(c => c.ClientId == vm.ClientId))
                ModelState.AddModelError(nameof(vm.ClientId), "Select a valid client.");

            if (!await _context.Goal.AnyAsync(g => g.GoalId == vm.GoalId && g.ClientId == vm.ClientId))
                ModelState.AddModelError(nameof(vm.GoalId), "Select a valid goal for that client.");

            var selectedFoods = vm.Foods.Where(f => f.Selected).ToList();
            if (selectedFoods.Count == 0)
                ModelState.AddModelError("", "Select at least 1 food.");

            if (!ModelState.IsValid)
            {
                await LoadFormListsAsync(vm.ClientId);
                vm.Foods = await BuildFoodsVMAsync(selectedFoods);
                return View(vm);
            }

            var plan = await _context.FoodHabitsPlan
                .Include(p => p.PlanFoods)
                .FirstOrDefaultAsync(p => p.FoodHabitsPlanId == id);

            if (plan == null) return NotFound();

            plan.ClientId = vm.ClientId;
            plan.GoalId = vm.GoalId;
            plan.StartingDate = vm.StartingDate.Date;
            plan.EndingDate = vm.EndingDate.Date;

            if (plan.PlanFoods != null && plan.PlanFoods.Count > 0)
                _context.FoodPlan.RemoveRange(plan.PlanFoods);

            _context.FoodPlan.AddRange(selectedFoods.Select(f => new PlanFood
            {
                PlanId = plan.FoodHabitsPlanId,
                FoodId = f.FoodId,
                PortionId = f.PortionId
            }));

            await RegenerateFoodPlanDaysAsync(
                plan.FoodHabitsPlanId,
                plan.StartingDate,
                plan.EndingDate,
                selectedFoods);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Nutricionista,Nutritionist")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (IsAdmin()) return NoDataPermission();
            if (id == null) return NotFound();

            var plan = await _context.FoodHabitsPlan
                .Include(p => p.Client)
                .Include(p => p.Goal)
                .FirstOrDefaultAsync(p => p.FoodHabitsPlanId == id);

            if (plan == null) return NotFound();

            return View(plan);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Nutricionista,Nutritionist")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (IsAdmin()) return NoDataPermission();

            var plan = await _context.FoodHabitsPlan.FindAsync(id);
            if (plan == null) return RedirectToAction(nameof(Index));

            var days = await _context.FoodPlanDay.Where(x => x.PlanId == id).ToListAsync();
            var intakes = await _context.FoodIntake.Where(x => x.PlanId == id).ToListAsync();
            var foods = await _context.FoodPlan.Where(x => x.PlanId == id).ToListAsync();
            var ncp = await _context.NutritionistClientPlan.Where(x => x.PlanId == id).ToListAsync();

            if (days.Count > 0) _context.FoodPlanDay.RemoveRange(days);
            if (intakes.Count > 0) _context.FoodIntake.RemoveRange(intakes);
            if (foods.Count > 0) _context.FoodPlan.RemoveRange(foods);
            if (ncp.Count > 0) _context.NutritionistClientPlan.RemoveRange(ncp);

            _context.FoodHabitsPlan.Remove(plan);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadFormListsAsync(int selectedClientId)
        {
            var clients = await _context.Client.AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new { c.ClientId, c.Name })
                .ToListAsync();

            var portions = await _context.Portion.AsNoTracking()
                .OrderBy(p => p.PortionName)
                .Select(p => new { p.PortionId, p.PortionName })
                .ToListAsync();

            ViewBag.ClientId = new SelectList(clients, "ClientId", "Name", selectedClientId);
            ViewBag.PortionId = new SelectList(portions, "PortionId", "PortionName");
        }

        private async Task<List<FoodHabitsPlanFormVM.FoodLineVM>> BuildFoodsVMAsync(List<FoodHabitsPlanFormVM.FoodLineVM>? selected)
        {
            var portionDefault = await _context.Portion.AsNoTracking()
                .Select(p => p.PortionId)
                .FirstOrDefaultAsync();

            var allFoods = await _context.Food
                .AsNoTracking()
                .Include(f => f.Category)
                .OrderBy(f => f.Name)
                .Select(f => new { f.FoodId, f.Name, Category = f.Category!.Category })
                .ToListAsync();

            var map = (selected ?? new()).ToDictionary(x => x.FoodId, x => x);

            return allFoods.Select(f =>
            {
                var has = map.TryGetValue(f.FoodId, out var line);

                return new FoodHabitsPlanFormVM.FoodLineVM
                {
                    Selected = has && line!.Selected,
                    FoodId = f.FoodId,
                    PortionId = has ? line!.PortionId : portionDefault,
                    FoodName = f.Name,
                    CategoryName = f.Category
                };
            }).ToList();
        }
    }
}
