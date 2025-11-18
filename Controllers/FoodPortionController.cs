using System.Linq;
using System.Threading.Tasks;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Controllers
{
    public class FoodPortionController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public FoodPortionController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _context.FoodPortion
                .Include(p => p.Food)
                .AsNoTracking()
                .OrderBy(p => p.Food!.Name)
                .ThenBy(p => p.AmountGramsMl)
                .ToListAsync();

            return View(list);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var portion = await _context.FoodPortion
                .Include(p => p.Food)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.FoodPortionId == id);

            if (portion == null) return NotFound();
            return View(portion);
        }

        public async Task<IActionResult> Create()
        {
            await LoadFoodsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FoodId,Portion,AmountGramsMl")] FoodPortion model)
        {
            if (await _context.FoodPortion
                .AnyAsync(p => p.FoodId == model.FoodId && p.Portion.ToLower() == model.Portion.Trim().ToLower()))
            {
                ModelState.AddModelError(nameof(FoodPortion.Portion), "This portion label already exists for the selected food.");
            }

            if (!ModelState.IsValid)
            {
                await LoadFoodsAsync(model.FoodId);
                TempData["Error"] = "Please correct the errors below.";
                return View(model);
            }

            try
            {
                model.Portion = model.Portion.Trim();
                _context.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Food portion created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                TempData["Error"] = $"Error creating record: {ex.GetBaseException().Message}";
                await LoadFoodsAsync(model.FoodId);
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var model = await _context.FoodPortion.FindAsync(id);
            if (model == null) return NotFound();

            await LoadFoodsAsync(model.FoodId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FoodPortionId,FoodId,Portion,AmountGramsMl")] FoodPortion model)
        {
            if (id != model.FoodPortionId) return NotFound();

            if (await _context.FoodPortion
                .AnyAsync(p => p.FoodPortionId != id && p.FoodId == model.FoodId && p.Portion.ToLower() == model.Portion.Trim().ToLower()))
            {
                ModelState.AddModelError(nameof(FoodPortion.Portion), "This portion label already exists for the selected food.");
            }

            if (!ModelState.IsValid)
            {
                await LoadFoodsAsync(model.FoodId);
                TempData["Error"] = "Please correct the errors below.";
                return View(model);
            }

            try
            {
                var entity = await _context.FoodPortion.FirstOrDefaultAsync(p => p.FoodPortionId == id);
                if (entity == null) return NotFound();

                entity.FoodId = model.FoodId;
                entity.Portion = model.Portion.Trim();
                entity.AmountGramsMl = model.AmountGramsMl;

                await _context.SaveChangesAsync();
                TempData["Success"] = "Food portion updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                TempData["Error"] = $"Could not save changes: {ex.GetBaseException().Message}";
                await LoadFoodsAsync(model.FoodId);
                return View(model);
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var model = await _context.FoodPortion
                .Include(p => p.Food)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.FoodPortionId == id);

            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var portion = await _context.FoodPortion.FirstOrDefaultAsync(p => p.FoodPortionId == id);
            if (portion == null) return NotFound();

            try
            {
                _context.FoodPortion.Remove(portion);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Food portion deleted successfully.";
            }
            catch (DbUpdateException ex)
            {
                TempData["Error"] = $"Could not delete record: {ex.GetBaseException().Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadFoodsAsync(int? selectedId = null)
        {
            ViewBag.FoodId = new SelectList(
                await _context.Food.AsNoTracking().OrderBy(f => f.Name).ToListAsync(),
                "FoodId", "Name", selectedId);
        }
    }
}
