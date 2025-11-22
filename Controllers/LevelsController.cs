using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
        // Accepts an optional attempted Level so the view can offer "Create with these values".
        private IActionResult InvalidLevelView(Level? attempted = null)
        {
            Response.StatusCode = 404;
            return View("InvalidLevel", attempted);
        }

        // Populate distinct categories into ViewBag.CategoriesList
        private async Task PopulateCategoriesAsync()
        {
            var categories = await _context.Level
                .Where(l => !string.IsNullOrWhiteSpace(l.LevelCategory))
                .Select(l => l.LevelCategory!.Trim())
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            ViewBag.CategoriesList = categories;
        }

        // GET: Levels
        public async Task<IActionResult> Index(int page = 1, string? searchNumber = null, string? searchCategory = null, string? searchDescription = null)
        {
            const int pageSize = 10; // show 10 rows per page

            await PopulateCategoriesAsync();

            var query = _context.Level.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchNumber))
            {
                if (int.TryParse(searchNumber, out var parsedNumber))
                {
                    query = query.Where(l => l.LevelNumber == parsedNumber);
                }
            }

            if (!string.IsNullOrWhiteSpace(searchCategory))
            {
                // exact match (case-insensitive) — prevents "Master" matching "Grandmaster"
                var cat = searchCategory!.Trim().ToLower();
                query = query.Where(l => l.LevelCategory != null && l.LevelCategory.ToLower() == cat);
            }

            if (!string.IsNullOrWhiteSpace(searchDescription))
            {
                var desc = searchDescription.Trim();
                query = query.Where(l => l.Description != null && l.Description.Contains(desc));
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

            // Preserve search inputs for the view
            ViewBag.SearchNumber = searchNumber;
            ViewBag.SearchCategory = searchCategory;
            ViewBag.SearchDescription = searchDescription;

            return View(vm);
        }

        // GET: Levels/Details/5
        public async Task<IActionResult> Details(int? id, int page = 1, string? searchNumber = null, string? searchCategory = null, string? searchDescription = null)
        {
            if (id == null) return NotFound();

            var level = await _context.Level.FirstOrDefaultAsync(m => m.LevelId == id);
            if (level == null) return InvalidLevelView();

            ViewBag.Page = page;
            ViewBag.SearchNumber = searchNumber;
            ViewBag.SearchCategory = searchCategory;
            ViewBag.SearchDescription = searchDescription;

            return View(level);
        }

        // GET: Levels/Create
        // Optional parameters allow pre-populating the Create form (used when recreating a deleted level).
        public async Task<IActionResult> Create(int? levelNumber = null, string? levelCategory = null, string? description = null)
        {
            await PopulateCategoriesAsync();

            var model = new Level
            {
                LevelCategory = levelCategory ?? string.Empty,
                Description = description ?? string.Empty
            };

            if (levelNumber.HasValue)
            {
                model.LevelNumber = levelNumber.Value;
            }

            return View(model);
        }

        // POST: Levels/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LevelId,LevelNumber,LevelCategory,Description")] Level level)
        {
            if (ModelState.IsValid)
            {
                _context.Add(level);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Level {level.LevelNumber} created.";
                return RedirectToAction(nameof(Index));
            }

            // repopulate categories when returning view due to validation errors
            await PopulateCategoriesAsync();
            return View(level);
        }

        // GET: Levels/Edit/5
        public async Task<IActionResult> Edit(int? id, int page = 1, string? searchNumber = null, string? searchCategory = null, string? searchDescription = null)
        {
            if (id == null) return NotFound();

            var level = await _context.Level.FindAsync(id);
            if (level == null) return InvalidLevelView();

            await PopulateCategoriesAsync();

            ViewBag.Page = page;
            ViewBag.SearchNumber = searchNumber;
            ViewBag.SearchCategory = searchCategory;
            ViewBag.SearchDescription = searchDescription;
            return View(level);
        }

        // POST: Levels/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LevelId,LevelNumber,LevelCategory,Description")] Level level, int page = 1, string? searchNumber = null, string? searchCategory = null, string? searchDescription = null)
        {
            if (id != level.LevelId) return NotFound();

            if (ModelState.IsValid)
            {
                // Check existence first — if the level was deleted meanwhile, offer recreate option.
                var exists = await _context.Level.AnyAsync(l => l.LevelId == level.LevelId);
                if (!exists)
                {
                    return InvalidLevelView(level);
                }

                try
                {
                    _context.Update(level);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"Level {level.LevelNumber} updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    // If record was removed by another user, show InvalidLevel and offer recreate
                    if (!LevelExists(level.LevelId)) return InvalidLevelView(level);
                    throw;
                }

                // Redirect back to the same index page + filters
                return RedirectToAction(nameof(Index), new { page, searchNumber, searchCategory, searchDescription });
            }

            // If validation fails, preserve the routing values so the form can re-post them
            await PopulateCategoriesAsync();
            ViewBag.Page = page;
            ViewBag.SearchNumber = searchNumber;
            ViewBag.SearchCategory = searchCategory;
            ViewBag.SearchDescription = searchDescription;
            return View(level);
        }

        // GET: Levels/Delete/5
        public async Task<IActionResult> Delete(int? id, int page = 1, string? searchNumber = null, string? searchCategory = null, string? searchDescription = null)
        {
            if (id == null) return NotFound();

            var level = await _context.Level.FirstOrDefaultAsync(m => m.LevelId == id);
            if (level == null) return InvalidLevelView();

            ViewBag.Page = page;
            ViewBag.SearchNumber = searchNumber;
            ViewBag.SearchCategory = searchCategory;
            ViewBag.SearchDescription = searchDescription;
            return View(level);
        }

        // POST: Levels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int page = 1, string? searchNumber = null, string? searchCategory = null, string? searchDescription = null)
        {
            var level = await _context.Level.FindAsync(id);
            if (level == null)
            {
                // Show the InvalidLevel page if the level is already gone
                return InvalidLevelView();
            }

            _context.Level.Remove(level);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Level {level.LevelNumber} deleted.";

            // Redirect back to the same index page + filters
            return RedirectToAction(nameof(Index), new { page, searchNumber, searchCategory, searchDescription });
        }

        private bool LevelExists(int id) => _context.Level.Any(e => e.LevelId == id);
    }
}
