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

        // GET: /FoodCategories
        public async Task<IActionResult> Index()
        {
            var categories = await _context.FoodCategory
                .Include(c => c.ParentCategory)
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(categories);
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

        // GET: /FoodCategories/Create
        public async Task<IActionResult> Create()
        {
            await PopulateParentCategoriesDropDownList();
            return View();
        }

        // POST: /FoodCategories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,ParentCategoryId")] FoodCategory category)
        {
            if (ModelState.IsValid)
            {
                // Unique name check (opcional — remove se não quiseres unicidade)
                bool nameExists = await _context.FoodCategory
                    .AnyAsync(c => c.Name.ToLower() == category.Name.Trim().ToLower());
                if (nameExists)
                {
                    ModelState.AddModelError(nameof(FoodCategory.Name), "A category with this name already exists.");
                }
            }

            if (!ModelState.IsValid)
            {
                await PopulateParentCategoriesDropDownList(category.ParentCategoryId);
                TempData["Error"] = "Please fix the validation errors.";
                return View(category);
            }

            try
            {
                // trim
                category.Name = category.Name.Trim();
                category.Description = category.Description?.Trim();

                _context.Add(category);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Category created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                // por ex., violação de índice único
                ModelState.AddModelError(string.Empty, $"Could not save changes: {ex.GetBaseException().Message}");
                TempData["Error"] = "An error occurred while creating the category.";
                await PopulateParentCategoriesDropDownList(category.ParentCategoryId);
                return View(category);
            }
        }

        // GET: /FoodCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var category = await _context.FoodCategory.FindAsync(id);
            if (category == null) return NotFound();

            await PopulateParentCategoriesDropDownList(category.ParentCategoryId, excludeId: category.FoodCategoryId);
            return View(category);
        }

        // POST: /FoodCategories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FoodCategoryId,Name,Description,ParentCategoryId")] FoodCategory category)
        {
            if (id != category.FoodCategoryId) return NotFound();

            // Validações personalizadas de hierarquia (evitar self/loops)
            if (category.ParentCategoryId == category.FoodCategoryId)
            {
                ModelState.AddModelError(nameof(FoodCategory.ParentCategoryId),
                    "A category cannot be its own parent.");
            }
            else if (category.ParentCategoryId.HasValue)
            {
                // Evitar ciclos: não permitir definir um descendente como pai
                bool createsCycle = await IsDescendantAsync(category.ParentCategoryId.Value, category.FoodCategoryId);
                if (createsCycle)
                {
                    ModelState.AddModelError(nameof(FoodCategory.ParentCategoryId),
                        "Invalid parent: this would create a circular hierarchy.");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.FoodCategory
                        .FirstOrDefaultAsync(c => c.FoodCategoryId == id);

                    if (existing == null) return NotFound();

                    // Unique name check (excluindo o próprio)
                    bool nameExists = await _context.FoodCategory
                        .AnyAsync(c => c.FoodCategoryId != id &&
                                       c.Name.ToLower() == category.Name.Trim().ToLower());
                    if (nameExists)
                    {
                        ModelState.AddModelError(nameof(FoodCategory.Name), "A category with this name already exists.");
                    }

                    if (!ModelState.IsValid)
                        throw new DbUpdateException("Validation errors.");

                    existing.Name = category.Name.Trim();
                    existing.Description = category.Description?.Trim();
                    existing.ParentCategoryId = category.ParentCategoryId;

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Category updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.FoodCategory.AnyAsync(e => e.FoodCategoryId == id))
                        return NotFound();

                    TempData["Error"] = "The record was modified by another user. Please reload and try again.";
                }
                catch (DbUpdateException ex)
                {
                    TempData["Error"] = $"Could not save changes: {ex.GetBaseException().Message}";
                }
            }

            await PopulateParentCategoriesDropDownList(category.ParentCategoryId, excludeId: category.FoodCategoryId);
            return View(category);
        }

        // GET: /FoodCategories/Delete/5
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

        // POST: /FoodCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.FoodCategory
                .Include(c => c.SubCategory)
                .Include(c => c.Foods!)
                .FirstOrDefaultAsync(c => c.FoodCategoryId == id);

            if (category == null) return NotFound();

            if (category.SubCategory.Any())
            {
                TempData["Error"] = "Cannot delete a category that has subcategories. Reassign or remove them first.";
                return RedirectToAction(nameof(Index));
            }

            if (category.Foods != null && category.Foods.Any())
            {
                TempData["Error"] = "Cannot delete a category that is assigned to foods.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.FoodCategory.Remove(category);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Category deleted successfully.";
            }
            catch (DbUpdateException ex)
            {
                TempData["Error"] = $"Could not delete the category: {ex.GetBaseException().Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // ===== Helpers =====

        private async Task PopulateParentCategoriesDropDownList(int? selectedId = null, int? excludeId = null)
        {
            var query = _context.FoodCategory.AsNoTracking().OrderBy(c => c.Name);

            if (excludeId.HasValue)
                ViewBag.ParentCategoryId = new SelectList(
                    await query.Where(c => c.FoodCategoryId != excludeId.Value).ToListAsync(),
                    "FoodCategoryId", "Name", selectedId);
            else
                ViewBag.ParentCategoryId = new SelectList(
                    await query.ToListAsync(),
                    "FoodCategoryId", "Name", selectedId);
        }

        /// <summary>
        /// Returns true if candidateParentId is a descendant of categoryId (which would create a cycle).
        /// </summary>
        private async Task<bool> IsDescendantAsync(int candidateParentId, int categoryId)
        {
            if (candidateParentId == categoryId) return true;

            // BFS/DFS upwards: sobe na árvore a partir do candidato a pai
            int? current = candidateParentId;
            while (current != null)
            {
                if (current == categoryId) return true;
                current = await _context.FoodCategory
                    .Where(c => c.FoodCategoryId == current)
                    .Select(c => c.ParentCategoryId)
                    .FirstOrDefaultAsync();
            }
            return false;
        }
    }
}
