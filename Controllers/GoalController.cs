using System;
using System.Linq;
using System.Threading.Tasks;
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
    public class GoalController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public GoalController(HealthWellbeingDbContext context)
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

        private int CalculateAge(DateTime? birthDate)
        {
            if (birthDate == null) return 0;

            var today = DateTime.Today;
            var age = today.Year - birthDate.Value.Year;

            if (birthDate.Value.Date > today.AddYears(-age))
                age--;

            return age;
        }

        private double CalculateBMR(Client c)
        {
            var sex = (c.Gender ?? "m").ToLowerInvariant();
            int age = CalculateAge(c.BirthDate);

            if (sex.StartsWith("m"))
                return 10 * c.WeightKg + 6.25 * c.HeightCm - 5 * age + 5;

            return 10 * c.WeightKg + 6.25 * c.HeightCm - 5 * age - 161;
        }

        private double CalculateMaintenanceCalories(Client c)
            => CalculateBMR(c) * c.ActivityFactor;

        private double CalculateGoalCalories(Client c, string goalName)
        {
            double maintenance = CalculateMaintenanceCalories(c);

            return goalName switch
            {
                "gain" => maintenance + 500,
                "lose" => maintenance - 500,
                _ => maintenance
            };
        }

        private void CalculateAndFillMacros(Client client, Goal goal)
        {
            goal.DailyCalories = (int)Math.Round(CalculateGoalCalories(client, goal.GoalName));

            double proteinFactor = goal.GoalName switch
            {
                "gain" => 1.6,
                "lose" => 1.8,
                _ => 1.2
            };

            double proteinGrams = client.WeightKg * proteinFactor;
            goal.DailyProtein = (int)Math.Round(proteinGrams);
            double proteinKcal = proteinGrams * 4;

            double fatKcal = goal.DailyCalories * 0.30;
            double fatGrams = fatKcal / 9;
            goal.DailyFat = (int)Math.Round(fatGrams);

            double carbsKcal = goal.DailyCalories - (proteinKcal + fatKcal);
            if (carbsKcal < 0) carbsKcal = 0;

            goal.DailyHydrates = (int)Math.Round(carbsKcal / 4);
        }

        // LISTA: Nutricionista vê tudo; Cliente vê só os dele; Admin NÃO pode ver
        [Authorize(Roles = "Administrador,Administrator,Nutricionista,Nutritionist,Cliente,Client")]
        public async Task<IActionResult> Index(int page = 1, int itemsPerPage = 10, string? search = "")
        {
            if (IsAdmin()) return NoDataPermission();

            if (page < 1) page = 1;
            if (itemsPerPage < 1) itemsPerPage = 10;

            var query = _context.Goal
                .AsNoTracking()
                .Include(g => g.Client)
                .AsQueryable();

            if (IsClient())
            {
                var myId = await GetCurrentClientIdAsync();
                if (myId == null) return NoDataPermission();
                query = query.Where(g => g.ClientId == myId.Value);
            }

            var s = (search ?? "").Trim();
            if (!string.IsNullOrWhiteSpace(s))
            {
                var sl = s.ToLowerInvariant();
                query = query.Where(g =>
                    (g.GoalName != null && g.GoalName.ToLower().Contains(sl)) ||
                    (g.Client != null && g.Client.Name != null && g.Client.Name.ToLower().Contains(sl))
                );
            }

            int totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(g => g.Client != null ? g.Client.Name : "")
                .ThenBy(g => g.GoalName)
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToListAsync();

            ViewBag.Search = s;

            return View(new PaginationInfoFoodHabits<Goal>(items, totalItems, page, itemsPerPage));
        }

        // DETAILS: Nutricionista vê; Cliente só vê o dele; Admin NÃO pode ver
        [Authorize(Roles = "Administrador,Administrator,Nutricionista,Nutritionist,Cliente,Client")]
        public async Task<IActionResult> Details(int? id)
        {
            if (IsAdmin()) return NoDataPermission();
            if (id == null) return NotFound();

            var goal = await _context.Goal
                .AsNoTracking()
                .Include(g => g.Client)
                .FirstOrDefaultAsync(g => g.GoalId == id);

            if (goal == null) return NotFound();

            if (IsClient())
            {
                var myId = await GetCurrentClientIdAsync();
                if (myId == null) return NoDataPermission();
                if (goal.ClientId != myId.Value) return NoDataPermission();
            }

            return View(goal);
        }

        // CREATE/EDIT/DELETE: só Nutricionista; Admin NÃO pode ver; Cliente NÃO pode
        [Authorize(Roles = "Administrador,Administrator,Nutricionista,Nutritionist")]
        public async Task<IActionResult> Create()
        {
            if (IsAdmin()) return NoDataPermission();

            var clients = await _context.Client
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new { c.ClientId, c.Name })
                .ToListAsync();

            ViewData["ClientId"] = new SelectList(clients, "ClientId", "Name");

            ViewBag.GoalOptions = new SelectList(
                new[]
                {
                    new { Value = "lose", Text = "Weight Loss" },
                    new { Value = "maintain", Text = "Maintenance" },
                    new { Value = "gain", Text = "Muscle Gain" }
                },
                "Value", "Text"
            );

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Administrator,Nutricionista,Nutritionist")]
        public async Task<IActionResult> Create([Bind("ClientId,GoalName")] Goal goal)
        {
            if (IsAdmin()) return NoDataPermission();

            var client = await _context.Client.FindAsync(goal.ClientId);
            if (client == null)
                ModelState.AddModelError(nameof(goal.ClientId), "Client not found.");

            if (!new[] { "lose", "maintain", "gain" }.Contains(goal.GoalName ?? ""))
                ModelState.AddModelError(nameof(goal.GoalName), "Invalid goal.");

            if (ModelState.IsValid && client != null)
            {
                CalculateAndFillMacros(client, goal);

                _context.Goal.Add(goal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var clients = await _context.Client
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new { c.ClientId, c.Name })
                .ToListAsync();

            ViewData["ClientId"] = new SelectList(clients, "ClientId", "Name", goal.ClientId);

            ViewBag.GoalOptions = new SelectList(
                new[]
                {
                    new { Value = "lose", Text = "Weight Loss" },
                    new { Value = "maintain", Text = "Maintenance" },
                    new { Value = "gain", Text = "Muscle Gain" }
                },
                "Value", "Text", goal.GoalName
            );

            return View(goal);
        }

        [Authorize(Roles = "Administrador,Administrator,Nutricionista,Nutritionist")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (IsAdmin()) return NoDataPermission();
            if (id == null) return NotFound();

            var goal = await _context.Goal
                .Include(g => g.Client)
                .FirstOrDefaultAsync(g => g.GoalId == id);

            if (goal == null) return NotFound();

            var clients = await _context.Client
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new { c.ClientId, c.Name })
                .ToListAsync();

            ViewData["ClientId"] = new SelectList(clients, "ClientId", "Name", goal.ClientId);

            ViewBag.GoalOptions = new SelectList(
                new[]
                {
                    new { Value = "lose", Text = "Weight Loss" },
                    new { Value = "maintain", Text = "Maintenance" },
                    new { Value = "gain", Text = "Muscle Gain" }
                },
                "Value", "Text", goal.GoalName
            );

            return View(goal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Administrator,Nutricionista,Nutritionist")]
        public async Task<IActionResult> Edit(int id, [Bind("GoalId,ClientId,GoalName")] Goal goal)
        {
            if (IsAdmin()) return NoDataPermission();
            if (id != goal.GoalId) return NotFound();

            var client = await _context.Client.FindAsync(goal.ClientId);
            if (client == null)
                ModelState.AddModelError(nameof(goal.ClientId), "Client not found.");

            if (!new[] { "lose", "maintain", "gain" }.Contains(goal.GoalName ?? ""))
                ModelState.AddModelError(nameof(goal.GoalName), "Invalid goal.");

            if (ModelState.IsValid && client != null)
            {
                CalculateAndFillMacros(client, goal);

                _context.Update(goal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var clients = await _context.Client
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new { c.ClientId, c.Name })
                .ToListAsync();

            ViewData["ClientId"] = new SelectList(clients, "ClientId", "Name", goal.ClientId);

            ViewBag.GoalOptions = new SelectList(
                new[]
                {
                    new { Value = "lose", Text = "Weight Loss" },
                    new { Value = "maintain", Text = "Maintenance" },
                    new { Value = "gain", Text = "Muscle Gain" }
                },
                "Value", "Text", goal.GoalName
            );

            return View(goal);
        }

        [Authorize(Roles = "Administrador,Administrator,Nutricionista,Nutritionist")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (IsAdmin()) return NoDataPermission();
            if (id == null) return NotFound();

            var goal = await _context.Goal
                .AsNoTracking()
                .Include(g => g.Client)
                .FirstOrDefaultAsync(g => g.GoalId == id);

            if (goal == null) return NotFound();

            return View(goal);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Administrator,Nutricionista,Nutritionist")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (IsAdmin()) return NoDataPermission();

            var goal = await _context.Goal.FindAsync(id);
            if (goal != null)
            {
                _context.Goal.Remove(goal);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
