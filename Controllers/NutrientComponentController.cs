using System.Linq;
using System.Threading.Tasks;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Controllers
{
    public class NutrientComponentController : Controller
    {
        private readonly HealthWellbeingDbContext _context;
        public NutrientComponentController(HealthWellbeingDbContext context) => _context = context;

        // GET: /NutrientComponent
        public async Task<IActionResult> Index()
        {
            return View(await _context.NutrientComponent
                .AsNoTracking()
                .OrderBy(n => n.Name)
                .ToListAsync());
        }

        // GET: /NutrientComponent/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var nutrient = await _context.NutrientComponent
                .Include(n => n.FoodNutrient).ThenInclude(fn => fn.Food)
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.NutrientComponentId == id);
            if (nutrient == null) return NotFound();
            return View(nutrient);
        }

        // GET: /NutrientComponent/Create
        public IActionResult Create() => View();

        // POST: /NutrientComponent/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Code,DefaultUnit,Description,IsMacro")] NutrientComponent nutrient)
        {
            if (!ModelState.IsValid) return View(nutrient);

            nutrient.Name = nutrient.Name.Trim();
            nutrient.Code = nutrient.Code?.Trim();
            nutrient.Description = nutrient.Description?.Trim();

            _context.Add(nutrient);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Nutrient created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /NutrientComponent/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var n = await _context.NutrientComponent.FindAsync(id);
            if (n == null) return NotFound();
            return View(n);
        }

        // POST: /NutrientComponent/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NutrientComponentId,Name,Code,DefaultUnit,Description,IsMacro")] NutrientComponent nutrient)
        {
            if (id != nutrient.NutrientComponentId) return NotFound();
            if (!ModelState.IsValid) return View(nutrient);

            try
            {
                var existing = await _context.NutrientComponent.FirstOrDefaultAsync(x => x.NutrientComponentId == id);
                if (existing == null) return NotFound();

                existing.Name = nutrient.Name.Trim();
                existing.Code = nutrient.Code?.Trim();
                existing.DefaultUnit = nutrient.DefaultUnit?.Trim();
                existing.Description = nutrient.Description?.Trim();
                existing.IsMacro = nutrient.IsMacro;

                await _context.SaveChangesAsync();
                TempData["Success"] = "Nutrient updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.NutrientComponent.AnyAsync(e => e.NutrientComponentId == id)) return NotFound();
                TempData["Error"] = "The record was modified by another user. Please reload and try again.";
                return View(nutrient);
            }
        }

        // GET: /NutrientComponent/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var n = await _context.NutrientComponent.AsNoTracking().FirstOrDefaultAsync(x => x.NutrientComponentId == id);
            if (n == null) return NotFound();
            return View(n);
        }

        // POST: /NutrientComponent/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var n = await _context.NutrientComponent
                .Include(x => x.FoodNutrient)
                .FirstOrDefaultAsync(x => x.NutrientComponentId == id);
            if (n == null) return NotFound();

            if (n.FoodNutrient?.Any() == true)
            {
                TempData["Error"] = "Cannot delete: nutrient is used by foods.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            _context.NutrientComponent.Remove(n);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Nutrient deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
