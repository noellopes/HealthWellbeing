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

        // GET: Levels
        public async Task<IActionResult> Index(int page = 1, string? searchNumber = null, string? searchCategory = null, string? searchDescription = null)
        {
            const int pageSize = 5;

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
                query = query.Where(l => l.LevelCategory != null && l.LevelCategory.Contains(searchCategory));
            }

            if (!string.IsNullOrWhiteSpace(searchDescription))
            {
                query = query.Where(l => l.Description != null && l.Description.Contains(searchDescription));
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
            if (level == null) return NotFound();

            ViewBag.Page = page;
            ViewBag.SearchNumber = searchNumber;
            ViewBag.SearchCategory = searchCategory;
            ViewBag.SearchDescription = searchDescription;

            return View(level);
        }

        // GET: Levels/Create
        public IActionResult Create() => View();

        // POST: Levels/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LevelId,LevelNumber,LevelCategory,Description")] Level level)
        {
            if (ModelState.IsValid)
            {
                _context.Add(level);
                await _context.SaveChangesAsync();

                // revert to simple message (no category appended)
                TempData["SuccessMessage"] = $"Level {level.LevelNumber} created.";

                return RedirectToAction(nameof(Index));
            }
            return View(level);
        }

        // GET: Levels/Edit/5
        public async Task<IActionResult> Edit(int? id, int page = 1, string? searchNumber = null, string? searchCategory = null, string? searchDescription = null)
        {
            if (id == null) return NotFound();

            var level = await _context.Level.FindAsync(id);
            if (level == null) return NotFound();

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
                try
                {
                    _context.Update(level);
                    await _context.SaveChangesAsync();

                    // revert to simple message (no category appended)
                    TempData["SuccessMessage"] = $"Level {level.LevelNumber} updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LevelExists(level.LevelId)) return NotFound();
                    throw;
                }

                // Redirect back to the same index page + filters
                return RedirectToAction(nameof(Index), new { page, searchNumber, searchCategory, searchDescription });
            }

            // If validation fails, preserve the routing values so the form can re-post them
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
            if (level == null) return NotFound();

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
            if (level != null)
            {
                _context.Level.Remove(level);
                await _context.SaveChangesAsync();

                // revert to simple message (no category appended)
                TempData["SuccessMessage"] = $"Level {level.LevelNumber} deleted.";
            }

            // Redirect back to the same index page + filters
            return RedirectToAction(nameof(Index), new { page, searchNumber, searchCategory, searchDescription });
        }

        private bool LevelExists(int id) => _context.Level.Any(e => e.LevelId == id);
    }
}
