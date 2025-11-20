using System.Linq;
using System.Threading.Tasks;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Controllers
{
    public class FoodCategoryController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public FoodCategoryController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: /FoodCategory
        public async Task<IActionResult> Index(string? searchString)
        {
            var categories = _context.FoodCategory
                .Include(c => c.ParentCategory)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                searchString = searchString.Trim().ToLower();
                categories = categories.Where(c => c.Name.ToLower().Contains(searchString)
                                                || (c.ParentCategory != null && c.ParentCategory.Name.ToLower().Contains(searchString)));
            }

            var list = await categories.OrderBy(c => c.Name).ToListAsync();
            return View(list);
        }

        // GET: /FoodCategory/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var category = await _context.FoodCategory
                .Include(c => c.ParentCategory)
                .Include(c => c.SubCategory)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.FoodCategoryId == id);

            if (category == null) return NotFound();

            return View(category);
        }

        // GET: /FoodCategory/Create
        public async Task<IActionResult> Create()
        {
            await PopulateParentCategoriesDropDownList();
            return View();
        }

        // POST: /FoodCategory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,ParentCategoryId")] FoodCategory category)
        {
            if (ModelState.IsValid)
            {
                bool nameExists = await _context.FoodCategory
                    .AnyAsync(c => c.Name.ToLower().Trim() == category.Name.ToLower().Trim());
                if (nameExists)
                    ModelState.AddModelError(nameof(FoodCategory.Name), "A category with this name already exists.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateParentCategoriesDropDownList(category.ParentCategoryId);
                TempData["Error"] = "Please fix the validation errors.";
                return View(category);
            }

            try
            {
                category.Name = category.Name.Trim();
                category.Description = category.Description?.Trim();
                _context.Add(category);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Category created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError(string.Empty, $"Could not save changes: {ex.GetBaseException().Message}");
                TempData["Error"] = "An error occurred while creating the category.";
                await PopulateParentCategoriesDropDownList(category.ParentCategoryId);
                return View(category);
            }
        }

        // GET: /FoodCategory/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var category = await _context.FoodCategory.FindAsync(id);
            if (category == null) return NotFound();

            await PopulateParentCategoriesDropDownList(category.ParentCategoryId, excludeId: category.FoodCategoryId);
            return View(category);
        }

        // POST: /FoodCategory/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FoodCategoryId,Name,Description,ParentCategoryId")] FoodCategory category)
        {
            if (id != category.FoodCategoryId) return NotFound();

            // Hierarchy rules
            if (category.ParentCategoryId == category.FoodCategoryId)
            {
                ModelState.AddModelError(nameof(FoodCategory.ParentCategoryId),
                    "A category cannot be parent of itself.");
            }
            else if (category.ParentCategoryId.HasValue)
            {
                bool createsCycle = await IsDescendantAsync(category.ParentCategoryId.Value, category.FoodCategoryId);
                if (createsCycle)
                    ModelState.AddModelError(nameof(FoodCategory.ParentCategoryId),
                        "Invalid parent: this assignment would create a cycle in the hierarchy.");
            }

            // Uniqueness
            if (ModelState.IsValid)
            {
                bool nameExists = await _context.FoodCategory
                    .AnyAsync(c => c.FoodCategoryId != category.FoodCategoryId &&
                                   c.Name.ToLower().Trim() == category.Name.ToLower().Trim());
                if (nameExists)
                    ModelState.AddModelError(nameof(FoodCategory.Name), "A category with this name already exists.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateParentCategoriesDropDownList(category.ParentCategoryId, excludeId: category.FoodCategoryId);
                TempData["Error"] = "Please fix the validation errors.";
                return View(category);
            }

            try
            {
                var existing = await _context.FoodCategory.FirstAsync(c => c.FoodCategoryId == id);
                existing.Name = category.Name.Trim();
                existing.Description = category.Description?.Trim();
                existing.ParentCategoryId = category.ParentCategoryId;

                await _context.SaveChangesAsync();
                TempData["Success"] = "Category updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.FoodCategory.AnyAsync(e => e.FoodCategoryId == category.FoodCategoryId))
                    return NotFound();

                ModelState.AddModelError(string.Empty, "Concurrency error. Please try again.");
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError(string.Empty, $"Could not save changes: {ex.GetBaseException().Message}");
            }

            await PopulateParentCategoriesDropDownList(category.ParentCategoryId, excludeId: category.FoodCategoryId);
            TempData["Error"] = "An error occurred while updating the category.";
            return View(category);
        }

        // GET: /FoodCategory/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var category = await _context.FoodCategory
                .Include(c => c.ParentCategory)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.FoodCategoryId == id);

            if (category == null) return NotFound();

            return View(category);
        }

        // POST: /FoodCategory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.FoodCategory
                .Include(c => c.SubCategory)
                .FirstOrDefaultAsync(c => c.FoodCategoryId == id);

            if (category == null) return NotFound();

            if (category.SubCategory.Any())
            {
                TempData["Error"] = "You cannot delete a category that still has subcategories. Move or delete them first.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            try
            {
                _context.FoodCategory.Remove(category);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Category deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                TempData["Error"] = $"Delete failed: {ex.GetBaseException().Message}";
                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        // -------- Helpers --------

        private async Task PopulateParentCategoriesDropDownList(int? selectedId = null, int? excludeId = null)
        {
            var query = _context.FoodCategory.AsNoTracking().OrderBy(c => c.Name).AsQueryable();
            if (excludeId.HasValue)
            {
                var descendants = await _context.FoodCategory
                    .Where(c => c.FoodCategoryId == excludeId.Value)
                    .SelectMany(c => c.SubCategory)
                    .Select(c => c.FoodCategoryId)
                    .ToListAsync();

                query = query.Where(c => c.FoodCategoryId != excludeId.Value && !descendants.Contains(c.FoodCategoryId));
            }

            ViewData["ParentCategoryId"] = new SelectList(await query.ToListAsync(), "FoodCategoryId", "Name", selectedId);
        }

        private async Task<bool> IsDescendantAsync(int candidateParentId, int currentId)
        {
            if (candidateParentId == currentId) return true;

            var children = await _context.FoodCategory
                .Where(c => c.ParentCategoryId == currentId)
                .Select(c => c.FoodCategoryId)
                .ToListAsync();

            foreach (var childId in children)
            {
                if (candidateParentId == childId) return true;
                if (await IsDescendantAsync(candidateParentId, childId)) return true;
            }
            return false;
        }
    }
}
