using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // Required for SelectList
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers
{
    public class LevelsController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public LevelsController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // Helper to render the InvalidLevel view and set 404 status code.
        private IActionResult InvalidLevelView(Level? attempted = null)
        {
            Response.StatusCode = 404;
            if (attempted != null && attempted.LevelCategoryId != 0)
            {
                var category = _context.LevelCategory.Find(attempted.LevelCategoryId);
                if (category != null)
                {
                    attempted.Category = category;
                }
            }

            return View("InvalidLevel", attempted);
        }

        // UPDATED: Populate categories for Dropdown (ForeignKey)
        // Uses the new table LevelCategory instead of distinct strings
        private void PopulateCategoriesDropdown(object? selectedCategory = null)
        {
            var categoriesQuery = _context.LevelCategory.OrderBy(c => c.Name);
            ViewData["LevelCategoryId"] = new SelectList(categoriesQuery.AsNoTracking(), "LevelCategoryId", "Name", selectedCategory);
        }

        // GET: Levels
        public async Task<IActionResult> Index(int page = 1, string? searchNumber = null, string? searchCategory = null, string? searchDescription = null, string? searchPoints = null)
        {
            const int pageSize = 10;

            var query = _context.Level.Include(l => l.Category).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchNumber))
            {
                if (int.TryParse(searchNumber, out var parsedNumber))
                {
                    query = query.Where(l => l.LevelNumber == parsedNumber);
                }
            }

            if (!string.IsNullOrWhiteSpace(searchCategory))
            {
                var cat = searchCategory!.Trim().ToLower();
                // FIX: Search inside the related Category object
                query = query.Where(l => l.Category != null && l.Category.Name.ToLower() == cat);
            }

            if (!string.IsNullOrWhiteSpace(searchDescription))
            {
                var desc = searchDescription.Trim();
                query = query.Where(l => l.Description != null && l.Description.Contains(desc));
            }
            if (!string.IsNullOrWhiteSpace(searchPoints))
            {
                if (int.TryParse(searchPoints, out var parsedPoints))
                {
                    query = query.Where(l => l.LevelPointsLimit == parsedPoints);
                }
            }

            var totalItems = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            if (totalPages == 0) totalPages = 1;
            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var levels = await query
                .OrderBy(l => l.LevelNumber)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var vm = new PaginationInfo<Level>(levels, totalItems, pageSize, page);

            ViewBag.SearchNumber = searchNumber;
            ViewBag.SearchCategory = searchCategory;
            ViewBag.SearchDescription = searchDescription;

            ViewBag.CategoriesList = await _context.LevelCategory
                .OrderBy(c => c.Name)
                .Select(c => c.Name)
                .ToListAsync();

            return View(vm);
        }

        // GET: Levels/Details/5
        public async Task<IActionResult> Details(int? id, int page = 1)
        {
            if (id == null) return NotFound();

            // FIX: Include Category
            var level = await _context.Level
                .Include(l => l.Category)
                .FirstOrDefaultAsync(m => m.LevelId == id);

            if (level == null) return InvalidLevelView();

            ViewBag.Page = page;
            return View(level);
        }

        // GET: Levels/Create
        public IActionResult Create(int? levelNumber = null, int? levelCategoryId = null, string? description = null, int ? levelPointsLimit = null)
        {
            
            PopulateCategoriesDropdown(levelCategoryId);

            var model = new Level
            {
                Description = description ?? string.Empty
            };

            if (levelNumber.HasValue) model.LevelNumber = levelNumber.Value;
            if (levelCategoryId.HasValue) model.LevelCategoryId = levelCategoryId.Value;
            if (levelPointsLimit.HasValue) model.LevelPointsLimit = levelPointsLimit.Value;

            return View(model);
        }

        // POST: Levels/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LevelId,LevelNumber,LevelCategoryId,LevelPointsLimit,Description")] Level level)
        {
            bool levelExists = await _context.Level.AnyAsync(l => l.LevelNumber == level.LevelNumber);

            if (levelExists)
            {
                ModelState.AddModelError("LevelNumber", $"Level {level.LevelNumber} already exists. Please choose a different number.");
            }

            ModelState.Remove(nameof(level.Category));

            if (ModelState.IsValid)
            {
                _context.Add(level);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Level {level.LevelNumber} created.";
                return RedirectToAction(nameof(Index));
            }

            PopulateCategoriesDropdown(level.LevelCategoryId);
            return View(level);
        }

        // GET: Levels/CalculateLevel
        [HttpGet]
        public async Task<IActionResult> CalculateLevel(int? points)
        {
            if (points == null || points < 0)
                return Json(new { level = "-", remaining = points });

            var levels = await _context.Level
                .OrderBy(l => l.LevelNumber)
                .ToListAsync();

            int remaining = points.Value;
            int level = 0;

            foreach (var l in levels)
            {
                if (remaining >= l.LevelPointsLimit)
                {
                    remaining -= l.LevelPointsLimit;
                    level = l.LevelNumber;
                }
                else
                {
                    break;
                }
            }

            return Json(new { level, remaining });
        }

        // GET: Levels/Edit/5
        public async Task<IActionResult> Edit(int? id, int page = 1)
        {
            if (id == null) return NotFound();

            var level = await _context.Level
                .Include(l => l.Category) // <--- Loads the related data
                .FirstOrDefaultAsync(m => m.LevelId == id);


            if (level == null) return InvalidLevelView();

            // FIX: Load dropdown with the current value selected
            PopulateCategoriesDropdown(level.LevelCategoryId);

            ViewBag.Page = page;
            return View(level);
        }

        // POST: Levels/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LevelId,LevelNumber,LevelCategoryId,LevelPointsLimit,Description")] Level level, int page = 1)
        {
            if (id != level.LevelId) return NotFound();

            // Remove navigation property from validation
            ModelState.Remove(nameof(level.Category));

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(level);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Level {level.LevelNumber} updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LevelExists(level.LevelId)) return InvalidLevelView(level);
                    throw;
                }
                return RedirectToAction(nameof(Index), new { page });
            }

            // Reload dropdown if validation fails
            PopulateCategoriesDropdown(level.LevelCategoryId);
            ViewBag.Page = page;
            return View(level);
        }

        // GET: Levels/Delete/5
        public async Task<IActionResult> Delete(int? id, int page = 1)
        {
            if (id == null) return NotFound();

            // FIX: Include Category to show the name in the delete confirmation
            var level = await _context.Level
                .Include(l => l.Category)
                .FirstOrDefaultAsync(m => m.LevelId == id);

            if (level == null) return InvalidLevelView();

            ViewBag.Page = page;
            return View(level);
        }

        // POST: Levels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int page = 1)
        {
            var level = await _context.Level.FindAsync(id);
            if (level == null) return InvalidLevelView();

            _context.Level.Remove(level);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Level {level.LevelNumber} deleted.";
            return RedirectToAction(nameof(Index), new { page });
        }

        private bool LevelExists(int id) => _context.Level.Any(e => e.LevelId == id);
    }
}