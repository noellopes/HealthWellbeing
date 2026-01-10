using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;

namespace HealthWellbeing.Controllers
{
    [Authorize]
    public class FoodController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public FoodController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin()
            => User.IsInRole("Administrador") || User.IsInRole("Administrator");

        private IActionResult NoDataPermission()
            => View("~/Views/Shared/NoDataPermission.cshtml");

        // TODOS podem ver
        [Authorize(Roles = "Administrador,Administrator,Nutricionista,Nutritionist,Cliente,Client")]
        public async Task<IActionResult> Index(string? search, int page = 1, int itemsPerPage = 10)
        {
            if (page < 1) page = 1;
            if (itemsPerPage < 1) itemsPerPage = 10;

            var query = _context.Food
                .AsNoTracking()
                .Include(f => f.Category)
                .AsQueryable();

            var s = (search ?? "").Trim();
            if (!string.IsNullOrWhiteSpace(s))
            {
                var sl = s.ToLower();
                query = query.Where(f =>
                    (f.Name != null && f.Name.ToLower().Contains(sl)) ||
                    (f.Category != null && f.Category.Category != null && f.Category.Category.ToLower().Contains(sl)));
            }

            int totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(f => f.Name)
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToListAsync();

            ViewBag.Search = s;

            return View(new PaginationInfoFoodHabits<Food>(items, totalItems, page, itemsPerPage));
        }

        // TODOS podem ver
        [Authorize(Roles = "Administrador,Administrator,Nutricionista,Nutritionist,Cliente,Client")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var food = await _context.Food
                .AsNoTracking()
                .Include(f => f.Category)
                .FirstOrDefaultAsync(f => f.FoodId == id);

            if (food == null) return NotFound();

            return View(food);
        }

        // SÓ ADMIN
        [Authorize(Roles = "Administrador,Administrator")]
        public async Task<IActionResult> Create()
        {
            await LoadCategoriesAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Administrator")]
        public async Task<IActionResult> Create(Food food)
        {
            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync(food.CategoryId);
                return View(food);
            }

            _context.Food.Add(food);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Food created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // SÓ ADMIN
        [Authorize(Roles = "Administrador,Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var food = await _context.Food.FindAsync(id);
            if (food == null) return NotFound();

            await LoadCategoriesAsync(food.CategoryId);
            return View(food);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Administrator")]
        public async Task<IActionResult> Edit(int id, Food food)
        {
            if (id != food.FoodId) return NotFound();

            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync(food.CategoryId);
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
                var exists = await _context.Food.AnyAsync(f => f.FoodId == food.FoodId);
                if (!exists) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // SÓ ADMIN
        [Authorize(Roles = "Administrador,Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var food = await _context.Food
                .AsNoTracking()
                .Include(f => f.Category)
                .FirstOrDefaultAsync(f => f.FoodId == id);

            if (food == null) return NotFound();

            return View(food);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var food = await _context.Food.FindAsync(id);
            if (food == null)
            {
                TempData["Error"] = "Food not found.";
                return RedirectToAction(nameof(Index));
            }

            _context.Food.Remove(food);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Food deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadCategoriesAsync(int? selectedId = null)
        {
            var cats = await _context.FoodCategory
                .AsNoTracking()
                .OrderBy(c => c.Category)
                .Select(c => new { c.CategoryId, c.Category })
                .ToListAsync();

            ViewBag.FoodCategoryId = new SelectList(cats, "FoodCategoryId", "Category", selectedId);
        }
    }
}
