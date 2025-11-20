using System;
using System.Linq;
using System.Threading.Tasks;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Controllers
{
    public class GoalController : Controller
    {
        private readonly HealthWellbeingDbContext _context;
        public GoalController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // INDEX COM PESQUISA E PAGINAÇÃO SIMPLES
        public async Task<IActionResult> Index(string? searchString, int page = 1)
        {
            int pageSize = 10; // nº de registos por página

            var query = _context.Goal
                .Include(g => g.Client)
                .AsNoTracking()
                .AsQueryable();

            // filtro de pesquisa: por Client ou GoalType
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(g =>
                    (g.Client != null && g.Client.Name.Contains(searchString)) ||
                    g.GoalType.Contains(searchString));
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var list = await query
                .OrderBy(g => g.GoalType)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.SearchString = searchString;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(list);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var goal = await _context.Goal
                .Include(g => g.Client)
                .Include(g => g.FoodPlans)
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.GoalId == id);

            if (goal == null) return NotFound();
            return View(goal);
        }

        public async Task<IActionResult> Create(string? clientId)
        {
            await LoadClientsAsync(clientId);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClientId,GoalType,DailyCalories,DailyProtein,DailyFat,DailyCarbs,DailyFiber,DailyVitamins,DailyMinerals")] Goal goal)
        {
            if (!ModelState.IsValid)
            {
                await LoadClientsAsync(goal.ClientId);
                TempData["Error"] = "Please correct the errors below.";
                return View(goal);
            }

            try
            {
                goal.GoalType = goal.GoalType.Trim();
                _context.Add(goal);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Goal created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                TempData["Error"] = $"Error creating record: {ex.GetBaseException().Message}";
                await LoadClientsAsync(goal.ClientId);
                return View(goal);
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var goal = await _context.Goal.FindAsync(id);
            if (goal == null) return NotFound();

            await LoadClientsAsync(goal.ClientId);
            return View(goal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GoalId,ClientId,GoalType,DailyCalories,DailyProtein,DailyFat,DailyCarbs,DailyFiber,DailyVitamins,DailyMinerals")] Goal goal)
        {
            if (id != goal.GoalId) return NotFound();

            if (!ModelState.IsValid)
            {
                await LoadClientsAsync(goal.ClientId);
                TempData["Error"] = "Please correct the errors below.";
                return View(goal);
            }

            try
            {
                var entity = await _context.Goal.FirstOrDefaultAsync(g => g.GoalId == id);
                if (entity == null) return NotFound();

                entity.ClientId = goal.ClientId;
                entity.GoalType = goal.GoalType.Trim();
                entity.DailyCalories = goal.DailyCalories;
                entity.DailyProtein = goal.DailyProtein;
                entity.DailyFat = goal.DailyFat;
                entity.DailyCarbs = goal.DailyCarbs;
                entity.DailyFiber = goal.DailyFiber;
                entity.DailyVitamins = goal.DailyVitamins;
                entity.DailyMinerals = goal.DailyMinerals;

                await _context.SaveChangesAsync();
                TempData["Success"] = "Goal updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Goal.AnyAsync(g => g.GoalId == id))
                    return NotFound();

                TempData["Error"] = "This record was changed by another user. Please reload.";
                await LoadClientsAsync(goal.ClientId);
                return View(goal);
            }
            catch (DbUpdateException ex)
            {
                TempData["Error"] = $"Could not save changes: {ex.GetBaseException().Message}";
                await LoadClientsAsync(goal.ClientId);
                return View(goal);
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var goal = await _context.Goal
                .Include(g => g.Client)
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.GoalId == id);

            if (goal == null) return NotFound();
            return View(goal);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var goal = await _context.Goal
                .Include(g => g.FoodPlans)
                .FirstOrDefaultAsync(g => g.GoalId == id);

            if (goal == null) return NotFound();

            if (goal.FoodPlans != null && goal.FoodPlans.Any())
            {
                TempData["Error"] = "Cannot delete a goal that has associated food plans.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Goal.Remove(goal);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Goal deleted successfully.";
            }
            catch (DbUpdateException ex)
            {
                TempData["Error"] = $"Could not delete: {ex.GetBaseException().Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadClientsAsync(string? selectedClientId = null)
        {
            ViewBag.ClientId = new SelectList(
                await _context.Client.AsNoTracking().OrderBy(c => c.Name).ToListAsync(),
                "ClientId", "Name", selectedClientId);
        }
    }
}
