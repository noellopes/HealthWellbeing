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

        public FoodController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: /Food
        public async Task<IActionResult> Index()
        {
            var foods = await _context.Foods
                .Include(f => f.Category)
                .AsNoTracking()
                .ToListAsync();

            return View(foods);
        }

        // GET: /Food/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var food = await _context.Foods
                .Include(f => f.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.FoodId == id);

            if (food == null) return NotFound();

            return View(food);
        }

        // GET: /Food/Create
        public IActionResult Create()
        {
            ViewData["FoodCategoryId"] = new SelectList(_context.FoodCategories.AsNoTracking(), "FoodCategoryId", "Name");
            return View();
        }

        // POST: /Food/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FoodId,Name,Description,KcalPer100g,ProteinPer100g,CarbsPer100g,FatPer100g,FoodCategoryId")] Food food)
        {
            if (!ModelState.IsValid)
            {
                ViewData["FoodCategoryId"] = new SelectList(_context.FoodCategories, "FoodCategoryId", "Name", food.FoodCategoryId);
                return View(food);
            }

            _context.Add(food);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Food/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var food = await _context.Foods.FindAsync(id);
            if (food == null) return NotFound();

            ViewData["FoodCategoryId"] = new SelectList(_context.FoodCategories.AsNoTracking(), "FoodCategoryId", "Name", food.FoodCategoryId);
            return View(food);
        }

        // POST: /Food/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FoodId,Name,Description,KcalPer100g,ProteinPer100g,CarbsPer100g,FatPer100g,FoodCategoryId")] Food food)
        {
            if (id != food.FoodId) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["FoodCategoryId"] = new SelectList(_context.FoodCategories, "FoodCategoryId", "Name", food.FoodCategoryId);
                return View(food);
            }

            try
            {
                _context.Update(food);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Foods.AnyAsync(e => e.FoodId == food.FoodId))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Food/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var food = await _context.Foods
                .Include(f => f.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.FoodId == id);

            if (food == null) return NotFound();

            return View(food);
        }

        // POST: /Food/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var food = await _context.Foods.FindAsync(id);
            if (food != null)
            {
                _context.Foods.Remove(food);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
