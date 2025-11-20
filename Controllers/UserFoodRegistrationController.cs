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
    public class UserFoodRegistrationController : Controller
    {
        private readonly HealthWellbeingDbContext _context;
        public UserFoodRegistrationController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // INDEX COM PESQUISA E PAGINAÇÃO
        public async Task<IActionResult> Index(string? searchString, int page = 1)
        {
            int pageSize = 10; // nº de registros por página

            var query = _context.UserFoodRegistration
                .Include(r => r.Client)
                .Include(r => r.Food)
                .Include(r => r.FoodPortion)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                // tenta converter searchString em data
                bool isDate = DateTime.TryParse(searchString, out DateTime searchDate);

                query = query.Where(r =>
                    (isDate && r.MealDateTime.Date == searchDate.Date) ||      // pesquisa por data exata
                    r.Client!.Name.Contains(searchString) ||
                    r.Food!.Name.Contains(searchString) ||
                    r.FoodPortion!.Label.Contains(searchString) ||
                    r.PortionsCount.ToString().Contains(searchString) ||
                    r.MealType.Contains(searchString) ||
                    (r.EstimatedEnergyKcal.HasValue && r.EstimatedEnergyKcal.Value.ToString().Contains(searchString))
                );
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var list = await query
                .OrderByDescending(r => r.MealDateTime)
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

            var model = await _context.UserFoodRegistration
                .Include(r => r.Client)
                .Include(r => r.Food)
                .Include(r => r.FoodPortion)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.UserFoodRegistrationId == id);

            if (model == null) return NotFound();
            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            await LoadDropDownsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClientId,FoodId,FoodPortionId,PortionsCount,MealType,MealDateTime,Notes")] UserFoodRegistration model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropDownsAsync(model.ClientId, model.FoodId, model.FoodPortionId);
                TempData["Error"] = "Please correct the errors below.";
                return View(model);
            }

            try
            {
                model.EstimatedEnergyKcal = await CalculateEnergyAsync(model.FoodId, model.FoodPortionId, model.PortionsCount);

                _context.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Food record created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Could not create record: {ex.GetBaseException().Message}";
                await LoadDropDownsAsync(model.ClientId, model.FoodId, model.FoodPortionId);
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var model = await _context.UserFoodRegistration.FindAsync(id);
            if (model == null) return NotFound();

            await LoadDropDownsAsync(model.ClientId, model.FoodId, model.FoodPortionId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserFoodRegistrationId,ClientId,FoodId,FoodPortionId,PortionsCount,MealType,MealDateTime,Notes")] UserFoodRegistration model)
        {
            if (id != model.UserFoodRegistrationId) return NotFound();

            if (!ModelState.IsValid)
            {
                await LoadDropDownsAsync(model.ClientId, model.FoodId, model.FoodPortionId);
                TempData["Error"] = "Please correct the errors below.";
                return View(model);
            }

            try
            {
                var entity = await _context.UserFoodRegistration.FirstOrDefaultAsync(r => r.UserFoodRegistrationId == id);
                if (entity == null) return NotFound();

                entity.ClientId = model.ClientId;
                entity.FoodId = model.FoodId;
                entity.FoodPortionId = model.FoodPortionId;
                entity.PortionsCount = model.PortionsCount;
                entity.MealType = model.MealType.Trim();
                entity.MealDateTime = model.MealDateTime;
                entity.Notes = model.Notes?.Trim();
                entity.EstimatedEnergyKcal = await CalculateEnergyAsync(model.FoodId, model.FoodPortionId, model.PortionsCount);

                await _context.SaveChangesAsync();
                TempData["Success"] = "Record updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                TempData["Error"] = $"Could not save changes: {ex.GetBaseException().Message}";
                await LoadDropDownsAsync(model.ClientId, model.FoodId, model.FoodPortionId);
                return View(model);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var model = await _context.UserFoodRegistration
                .Include(r => r.Client)
                .Include(r => r.Food)
                .Include(r => r.FoodPortion)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.UserFoodRegistrationId == id);

            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entity = await _context.UserFoodRegistration.FirstOrDefaultAsync(r => r.UserFoodRegistrationId == id);
            if (entity == null) return NotFound();

            try
            {
                _context.UserFoodRegistration.Remove(entity);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Record deleted successfully.";
            }
            catch (DbUpdateException ex)
            {
                TempData["Error"] = $"Could not delete record: {ex.GetBaseException().Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadDropDownsAsync(string? clientId = null, int? foodId = null, int? portionId = null)
        {
            ViewBag.ClientId = new SelectList(
                await _context.Client.AsNoTracking().OrderBy(c => c.Name).ToListAsync(),
                "ClientId", "Name", clientId);

            ViewBag.FoodId = new SelectList(
                await _context.Food.AsNoTracking().OrderBy(f => f.Name).ToListAsync(),
                "FoodId", "Name", foodId);

            ViewBag.FoodPortionId = new SelectList(
                await _context.FoodPortion
                    .Include(p => p.Food)
                    .AsNoTracking()
                    .OrderBy(p => p.Label)
                    .ToListAsync(),
                "FoodPortionId", "Label", portionId);
        }

        private async Task<decimal?> CalculateEnergyAsync(int foodId, int portionId, decimal portionsCount)
        {
            var portion = await _context.FoodPortion.AsNoTracking().FirstOrDefaultAsync(p => p.FoodPortionId == portionId);
            var nutrient = await _context.FoodNutrient
                .Include(fn => fn.NutrientComponent)
                .AsNoTracking()
                .FirstOrDefaultAsync(fn => fn.FoodId == foodId && fn.NutrientComponent!.Name == "Energy");

            if (portion == null || nutrient == null) return null;

            return Math.Round(((nutrient.Value / 100m) * portion.AmountGramsMl * portionsCount), 2);
        }
    }
}
