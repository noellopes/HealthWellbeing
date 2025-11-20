using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using System.Linq;
using System.Threading.Tasks;

namespace HealthWellbeing.Controllers
{
    public class FoodController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public FoodController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        private void LoadCategories(object? selected = null)
            => ViewBag.FoodCategoryId = new SelectList(_context.FoodCategory.AsNoTracking().OrderBy(c => c.Name), "FoodCategoryId", "Name", selected);

        // GET: Food
        public async Task<IActionResult> Index(string? search, int page = 1)
        {
            var query = _context.Food
                .Include(f => f.FoodCategory)
                .AsNoTracking()
                .OrderBy(f => f.Name)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(f =>
                    f.Name.Contains(search) ||
                    (f.FoodCategory != null && f.FoodCategory.Name.Contains(search))
                );
            }

            var totalItems = await query.CountAsync();
            var pageSize = 10;
            var totalPages = (int)System.Math.Ceiling(totalItems / (double)pageSize);

            var foods = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(foods);
        }

        // GET: Food/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var food = await _context.Food
                .Include(f => f.FoodCategory)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.FoodId == id);

            if (food == null) return NotFound();
            return View(food);
        }

        // GET: Food/Create
        public IActionResult Create()
        {
            LoadCategories();
            return View(new Food());
        }

        // POST: Food/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Food food)
        {
            // Uniqueness: Name within Category
            if (await _context.Food.AnyAsync(f => f.Name == food.Name && f.FoodCategoryId == food.FoodCategoryId))
                ModelState.AddModelError(nameof(Food.Name), "A food with this name already exists in the selected category.");

            if (!ModelState.IsValid)
            {
                LoadCategories(food.FoodCategoryId);
                return View(food);
            }

            _context.Add(food);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Food created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Food/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var food = await _context.Food.FindAsync(id);
            if (food == null) return NotFound();

            LoadCategories(food.FoodCategoryId);
            return View(food);
        }

        // POST: Food/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Food food)
        {
            if (id != food.FoodId) return NotFound();

            if (await _context.Food.AnyAsync(f => f.FoodId != id && f.Name == food.Name && f.FoodCategoryId == food.FoodCategoryId))
                ModelState.AddModelError(nameof(Food.Name), "A food with this name already exists in the selected category.");

            if (!ModelState.IsValid)
            {
                LoadCategories(food.FoodCategoryId);
                return View(food);
            }

            try
            {
                _context.Update(food);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Food updated successfully.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Food.AnyAsync(e => e.FoodId == id)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Food/Delete/5
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
                TempData["Success"] = "Food deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        // ===== Remote Validation =====
        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> CheckNameUnique(string name, int? foodCategoryId, int? foodId)
        {
            var exists = await _context.Food.AnyAsync(f =>
                f.Name == name &&
                f.FoodCategoryId == foodCategoryId &&
                f.FoodId != (foodId ?? 0));

            return Json(!exists); // true => valid
        }
    }
}
