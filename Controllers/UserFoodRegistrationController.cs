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

        public async Task<IActionResult> Index()
        {
            var list = await _context.UserFoodRegistration
                .Include(r => r.Client)
                .Include(r => r.Food)
                .Include(r => r.FoodPortion)
                .AsNoTracking()
                .OrderByDescending(r => r.MealDateTime)
                .ToListAsync();

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
                // calcular energia estimada
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

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

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

        public async Task<IActionResult> Delete(int? id)
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
                    .OrderBy(p => p.Portion)
                    .ToListAsync(),
                "FoodPortionId", "Portion", portionId);
        }

        private async Task<decimal?> CalculateEnergyAsync(int foodId, int portionId, decimal portionsCount)
        {
            var portion = await _context.FoodPortion.AsNoTracking().FirstOrDefaultAsync(p => p.FoodPortionId == portionId);
            var nutrient = await _context.FoodNutrient
                .Include(fn => fn.NutrientComponent)
                .AsNoTracking()
                .FirstOrDefaultAsync(fn => fn.FoodId == foodId && fn.NutrientComponent!.Name == "Energy");

            if (portion == null || nutrient == null) return null;

            // energia = valor * (pesoPorção / 100) * nº porções
            return Math.Round(((nutrient.Value / 100m) * portion.AmountGramsMl * portionsCount), 2);
        }
    }
}
