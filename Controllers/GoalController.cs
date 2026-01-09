using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers
{
    public class GoalController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public GoalController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // Calcular idade
        // =====================================================
        private int CalculateAge(DateTime? birthDate)
        {
            if (birthDate == null) return 0;

            var today = DateTime.Today;
            var age = today.Year - birthDate.Value.Year;

            if (birthDate.Value.Date > today.AddYears(-age))
                age--;

            return age;
        }

        // =====================================================
        //  CÁLCULOS NUTRICIONAIS
        // =====================================================
        private double CalculateBMR(Client c)
        {
            var sex = (c.Gender ?? "m").ToLower();
            int age = CalculateAge(c.BirthDate);

            if (sex.StartsWith("m"))
            {
                return 10 * c.WeightKg + 6.25 * c.HeightCm - 5 * age + 5;
            }
            else
            {
                return 10 * c.WeightKg + 6.25 * c.HeightCm - 5 * age - 161;
            }
        }

        private double CalculateMaintenanceCalories(Client c)
        {
            return CalculateBMR(c) * c.ActivityFactor;
        }

        private double CalculateGoalCalories(Client c, string goalName)
        {
            double maintenance = CalculateMaintenanceCalories(c);

            return goalName switch
            {
                "gain" => maintenance + 500,
                "lose" => maintenance - 500,
                _ => maintenance // maintain
            };
        }

        private void CalculateAndFillMacros(Client client, Goal goal)
        {
            goal.DailyCalories = (int)Math.Round(
                CalculateGoalCalories(client, goal.GoalName)
            );

            double proteinFactor = goal.GoalName switch
            {
                "gain" => 1.6,
                "lose" => 1.8,
                _ => 1.2 // maintain
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

        // =====================================================
        //  INDEX (PaginationInfo + search)
        // =====================================================
        public async Task<IActionResult> Index(int page = 1, string? search = "")
        {
            const int pageSize = 10;

            if (page < 1) page = 1;

            var query = _context.Goal
                .Include(g => g.Client)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(g =>
                    g.GoalName.ToLower().Contains(search) ||
                    (g.Client != null && g.Client.Name.ToLower().Contains(search))
                );
            }

            ViewBag.Search = search ?? "";

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            if (totalPages < 1) totalPages = 1;

            if (page > totalPages) page = totalPages;

            var items = await query
                .OrderBy(g => g.Client!.Name)
                .ThenBy(g => g.GoalName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var model = new PaginationInfo<Goal>(items, totalItems, page, pageSize);

            return View(model);
        }

        // =====================================================
        // DETAILS
        // =====================================================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var goal = await _context.Goal
                .Include(g => g.Client)
                .FirstOrDefaultAsync(m => m.GoalId == id);

            if (goal == null) return NotFound();

            return View(goal);
        }

        // =====================================================
        // CREATE
        // =====================================================
        public IActionResult Create()
        {
            ViewData["ClientId"] = new SelectList(_context.Client, "ClientId", "Email");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClientId,GoalName")] Goal goal)
        {
            var client = await _context.Client.FindAsync(goal.ClientId);
            if (client == null)
            {
                ModelState.AddModelError("", "Client not found.");
                ViewData["ClientId"] = new SelectList(_context.Client, "ClientId", "Email", goal.ClientId);
                return View(goal);
            }

            CalculateAndFillMacros(client, goal);

            if (ModelState.IsValid)
            {
                _context.Add(goal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ClientId"] = new SelectList(_context.Client, "ClientId", "Email", goal.ClientId);
            return View(goal);
        }
        // =====================================================
        // EDIT
        // =====================================================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var goal = await _context.Goal.FindAsync(id);
            if (goal == null) return NotFound();

            // Dropdown de clientes
            ViewData["ClientId"] = new SelectList(_context.Client, "ClientId", "Email", goal.ClientId);

            // Dropdown de GoalName
            ViewBag.GoalOptions = new SelectList(
                new[]
                {
            new { Value = "gain", Text = "Gain" },
            new { Value = "lose", Text = "Lose" },
            new { Value = "maintain", Text = "Maintain" }
                }, "Value", "Text", goal.GoalName
            );

            return View(goal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GoalId,ClientId,GoalName")] Goal goal)
        {
            if (id != goal.GoalId) return NotFound();

            var client = await _context.Client.FindAsync(goal.ClientId);
            if (client == null)
            {
                ModelState.AddModelError("", "Client not found.");
                ViewData["ClientId"] = new SelectList(_context.Client, "ClientId", "Email", goal.ClientId);

                
                ViewBag.GoalOptions = new SelectList(
                    new[]
                    {
                 new { Value = "lose", Text = "Weight Loss" },
                 new { Value = "maintain", Text = "Maintenance" },
                 new { Value = "gain", Text = "Muscle Gain" }
                    }, "Value", "Text", goal.GoalName
                );

                return View(goal);
            }

            // Calcula macros automaticamente
            CalculateAndFillMacros(client, goal);

            if (ModelState.IsValid)
            {
                _context.Update(goal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            
            ViewData["ClientId"] = new SelectList(_context.Client, "ClientId", "Email", goal.ClientId);
            ViewBag.GoalOptions = new SelectList(
                new[]
                {
            new { Value = "gain", Text = "Gain" },
            new { Value = "lose", Text = "Lose" },
            new { Value = "maintain", Text = "Maintain" }
                }, "Value", "Text", goal.GoalName
            );

            return View(goal);
        }
        // =====================================================
        // DELETE
        // =====================================================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var goal = await _context.Goal
                .Include(g => g.Client)
                .FirstOrDefaultAsync(m => m.GoalId == id);

            if (goal == null) return NotFound();

            return View(goal);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
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