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

        public FoodController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Food
        public async Task<IActionResult> Index()
        {
            var foods = await _context.Food.Include(f => f.Category).ToListAsync();
            return View(foods);
        }

        // GET: Food/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var food = await _context.Food
                .Include(f => f.Category)
                .FirstOrDefaultAsync(m => m.FoodId == id);
            if (food == null) return NotFound();

            return View(food);
        }

        // GET: Food/Create
        public IActionResult Create()
        {
            ViewData["FoodCategoryId"] = new SelectList(_context.FoodCategory, "FoodCategoryId", "Name");
            return View();
        }

        // POST: Food/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FoodId,Name,Description,KcalPer100g,ProteinPer100g,CarbsPer100g,FatPer100g,FoodCategoryId")] Food food)
        {
            if (ModelState.IsValid)
            {
                _context.Add(food);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FoodCategoryId"] = new SelectList(_context.FoodCategory, "FoodCategoryId", "Name", food.FoodCategoryId);
            return View(food);
        }

        // GET: Food/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var food = await _context.Food.FindAsync(id);
            if (food == null) return NotFound();

            ViewData["FoodCategoryId"] = new SelectList(_context.FoodCategory, "FoodCategoryId", "Name", food.FoodCategoryId);
            return View(food);
        }

        // POST: Food/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FoodId,Name,Description,KcalPer100g,ProteinPer100g,CarbsPer100g,FatPer100g,FoodCategoryId")] Food food)
        {
            if (id != food.FoodId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(food);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Food.Any(e => e.FoodId == food.FoodId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["FoodCategoryId"] = new SelectList(_context.FoodCategory, "FoodCategoryId", "Name", food.FoodCategoryId);
            return View(food);
        }

        // GET: Food/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var food = await _context.Food
                .Include(f => f.Category)
                .FirstOrDefaultAsync(m => m.FoodId == id);
            if (food == null) return NotFound();

            return View(food);
        }

        // POST: Food/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var food = await _context.Food.FindAsync(id);
            if (food != null)
            {
                _context.Food.Remove(food);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
