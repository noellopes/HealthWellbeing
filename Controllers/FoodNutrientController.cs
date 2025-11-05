using System.Linq;
using System.Threading.Tasks;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Controllers
{
    public class FoodNutrientController : Controller
    {
        private readonly HealthWellbeingDbContext _context;
        public FoodNutrientController(HealthWellbeingDbContext context) => _context = context;

        // GET: /FoodNutrient
        public async Task<IActionResult> Index(int? foodId)
        {
            var q = _context.FoodNutrient
                .Include(fn => fn.Food).ThenInclude(f => f.FoodCategory)
                .Include(fn => fn.NutrientComponent)
                .AsNoTracking()
                .AsQueryable();

            if (foodId.HasValue) q = q.Where(x => x.FoodId == foodId.Value);

            ViewBag.FoodId = new SelectList(await _context.Food.OrderBy(f => f.Name).ToListAsync(), "FoodId", "Name", foodId);
            return View(await q.OrderBy(x => x.Food!.Name).ThenBy(x => x.NutrientComponent!.Name).ToListAsync());
        }

        // GET: /FoodNutrient/Create
        public async Task<IActionResult> Create(int? foodId)
        {
            ViewData["FoodId"] = new SelectList(await _context.Food.OrderBy(f => f.Name).ToListAsync(), "FoodId", "Name", foodId);
            ViewData["NutrientComponentId"] = new SelectList(await _context.NutrientComponent.OrderBy(n => n.Name).ToListAsync(), "NutrientComponentId", "Name");
            return View(new FoodNutrient { Basis = "per100g" });
        }

        // POST: /FoodNutrient/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FoodId,NutrientComponentId,Value,Unit,Basis")] FoodNutrient fn)
        {
            if (ModelState.IsValid)
            {
                bool exists = await _context.FoodNutrient
                    .AnyAsync(x => x.FoodId == fn.FoodId && x.NutrientComponentId == fn.NutrientComponentId && x.Basis == fn.Basis);
                if (exists)
                    ModelState.AddModelError(string.Empty, "This nutrient already exists for the selected food and basis.");
            }

            if (!ModelState.IsValid)
            {
                ViewData["FoodId"] = new SelectList(await _context.Food.OrderBy(f => f.Name).ToListAsync(), "FoodId", "Name", fn.FoodId);
                ViewData["NutrientComponentId"] = new SelectList(await _context.NutrientComponent.OrderBy(n => n.Name).ToListAsync(), "NutrientComponentId", "Name", fn.NutrientComponentId);
                return View(fn);
            }

            _context.Add(fn);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Nutrient value added to food.";
            return RedirectToAction(nameof(Index), new { foodId = fn.FoodId });
        }

        // GET: /FoodNutrient/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var fn = await _context.FoodNutrient.FindAsync(id);
            if (fn == null) return NotFound();

            ViewData["FoodId"] = new SelectList(await _context.Food.OrderBy(f => f.Name).ToListAsync(), "FoodId", "Name", fn.FoodId);
            ViewData["NutrientComponentId"] = new SelectList(await _context.NutrientComponent.OrderBy(n => n.Name).ToListAsync(), "NutrientComponentId", "Name", fn.NutrientComponentId);
            return View(fn);
        }

        // POST: /FoodNutrient/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FoodNutrientId,FoodId,NutrientComponentId,Value,Unit,Basis")] FoodNutrient fn)
        {
            if (id != fn.FoodNutrientId) return NotFound();

            if (ModelState.IsValid)
            {
                bool duplicate = await _context.FoodNutrient
                    .AnyAsync(x => x.FoodNutrientId != fn.FoodNutrientId &&
                                   x.FoodId == fn.FoodId &&
                                   x.NutrientComponentId == fn.NutrientComponentId &&
                                   x.Basis == fn.Basis);
                if (duplicate)
                    ModelState.AddModelError(string.Empty, "Duplicate entry for this food/nutrient/basis.");
            }

            if (!ModelState.IsValid)
            {
                ViewData["FoodId"] = new SelectList(await _context.Food.OrderBy(f => f.Name).ToListAsync(), "FoodId", "Name", fn.FoodId);
                ViewData["NutrientComponentId"] = new SelectList(await _context.NutrientComponent.OrderBy(n => n.Name).ToListAsync(), "NutrientComponentId", "Name", fn.NutrientComponentId);
                return View(fn);
            }

            try
            {
                _context.Update(fn);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Nutrient value updated.";
                return RedirectToAction(nameof(Index), new { foodId = fn.FoodId });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.FoodNutrient.AnyAsync(e => e.FoodNutrientId == id)) return NotFound();
                TempData["Error"] = "The record was modified by another user. Please reload and try again.";
                return View(fn);
            }
        }

        // GET: /FoodNutrient/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var fn = await _context.FoodNutrient
                .Include(x => x.Food)
                .Include(x => x.NutrientComponent)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.FoodNutrientId == id);
            if (fn == null) return NotFound();
            return View(fn);
        }

        // POST: /FoodNutrient/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fn = await _context.FoodNutrient.FindAsync(id);
            if (fn == null) return NotFound();
            int? foodId = fn.FoodId;

            _context.FoodNutrient.Remove(fn);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Nutrient value removed from food.";
            return RedirectToAction(nameof(Index), new { foodId });
        }
    }
}
