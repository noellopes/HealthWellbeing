using System.Linq;
using System.Threading.Tasks;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Controllers
{
    public class FoodController : Controller
    {
        private readonly HealthWellbeingDbContext _context;
        public FoodController(HealthWellbeingDbContext context) => _context = context;

        // GET: /Food
        public async Task<IActionResult> Index(string? search)
        {
            var q = _context.Food.Include(f => f.FoodCategory).AsNoTracking();
            if (!string.IsNullOrWhiteSpace(search))
                q = q.Where(f => f.Name.Contains(search));
            return View(await q.OrderBy(f => f.Name).ToListAsync());
        }

        // GET: /Food/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var food = await _context.Food
                .Include(f => f.FoodCategory)
                .Include(f => f.FoodNutrients).ThenInclude(fn => fn.NutrientComponent)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.FoodId == id);
            if (food == null) return NotFound();
            return View(food);
        }

        // GET: /Food/Create
        public async Task<IActionResult> Create()
        {
            ViewData["FoodCategoryId"] = new SelectList(await _context.FoodCategory.OrderBy(c => c.Name).ToListAsync(), "FoodCategoryId", "Name");
            return View();
        }

        // POST: /Food/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,FoodCategoryId")] Food food)
        {
            if (!ModelState.IsValid)
            {
                ViewData["FoodCategoryId"] = new SelectList(await _context.FoodCategory.OrderBy(c => c.Name).ToListAsync(), "FoodCategoryId", "Name", food.FoodCategoryId);
                return View(food);
            }

            food.Name = food.Name.Trim();
            food.Description = food.Description?.Trim();

            _context.Add(food);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Food created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Food/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var food = await _context.Food.FindAsync(id);
            if (food == null) return NotFound();

            ViewData["FoodCategoryId"] = new SelectList(await _context.FoodCategory.OrderBy(c => c.Name).ToListAsync(), "FoodCategoryId", "Name", food.FoodCategoryId);
            return View(food);
        }

        // POST: /Food/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FoodId,Name,Description,FoodCategoryId")] Food food)
        {
            if (id != food.FoodId) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["FoodCategoryId"] = new SelectList(await _context.FoodCategory.OrderBy(c => c.Name).ToListAsync(), "FoodCategoryId", "Name", food.FoodCategoryId);
                return View(food);
            }

            try
            {
                var existing = await _context.Food.FirstOrDefaultAsync(f => f.FoodId == id);
                if (existing == null) return NotFound();

                existing.Name = food.Name.Trim();
                existing.Description = food.Description?.Trim();
                existing.FoodCategoryId = food.FoodCategoryId;

                await _context.SaveChangesAsync();
                TempData["Success"] = "Food updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Food.AnyAsync(e => e.FoodId == id)) return NotFound();
                TempData["Error"] = "The record was modified by another user. Please reload and try again.";
                return View(food);
            }
        }

        // GET: /Food/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var food = await _context.Food
                .Include(f => f.FoodCategory)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.FoodId == id);
            if (food == null) return NotFound();
            return View(food);
        }

        // POST: /Food/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var food = await _context.Food.FindAsync(id);
            if (food == null) return NotFound();

            _context.Food.Remove(food);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Food deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
