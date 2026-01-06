using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Controllers
{
    [Authorize(Roles = "Gestor")]
    public class LevelsController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public LevelsController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Levels
        public async Task<IActionResult> Index(
            int page = 1,
            int? searchNumber = null,
            int? searchCategoryId = null,
            string searchDescription = null,
            int? searchPoints = null)
        {

            var levelsQuery = _context.Level
                .Include(l => l.Category)
                .AsQueryable();

            if (searchNumber.HasValue)
            {
                levelsQuery = levelsQuery.Where(l => l.LevelNumber == searchNumber.Value);
            }

            if (searchCategoryId.HasValue)
            {
                levelsQuery = levelsQuery.Where(l => l.LevelCategoryId == searchCategoryId);
            }

            if (!string.IsNullOrEmpty(searchDescription))
            {
                levelsQuery = levelsQuery.Where(l => l.Description.Contains(searchDescription));
            }

            if (searchPoints.HasValue)
            {
                levelsQuery = levelsQuery.Where(l => l.LevelPointsLimit == searchPoints.Value);
            }

            ViewBag.SearchNumber = searchNumber;
            ViewBag.SearchCategoryId = searchCategoryId;
            ViewBag.SearchDescription = searchDescription;
            ViewBag.SearchPoints = searchPoints;

            ViewData["LevelCategories"] = new SelectList(_context.LevelCategory, "LevelCategoryId", "Name", searchCategoryId);

            int totalItems = await levelsQuery.CountAsync();
            var paginationInfo = new ViewModels.PaginationInfo<Level>(page, totalItems);

            paginationInfo.Items = await levelsQuery
                .OrderBy(l => l.LevelNumber)
                .Skip(paginationInfo.ItemsToSkip)
                .Take(paginationInfo.ItemsPerPage)
                .ToListAsync();

            return View(paginationInfo);
        }

        // --- CALCULADORA CORRIGIDA ---
        // Agora consulta a BD para saber os limites reais
        [HttpGet]
        public async Task<IActionResult> CalculateLevel(int points)
        {
            // Obtém todos os níveis ordenados por número (1, 2, 3...)
            // Usamos AsNoTracking para performance, pois é apenas leitura
            var levels = await _context.Level
                .OrderBy(l => l.LevelNumber)
                .AsNoTracking()
                .ToListAsync();

            if (!levels.Any())
            {
                return Json(new { level = "N/A", remaining = 0 });
            }

            int currentLevelNum = 1;
            int remaining = 0;
            bool found = false;

            // Lógica: Percorre os níveis. Se os pontos do utilizador forem MENORES
            // que o limite do nível, então o utilizador está nesse nível atual.
            foreach (var lvl in levels)
            {
                if (points < lvl.LevelPointsLimit)
                {
                    currentLevelNum = lvl.LevelNumber;
                    remaining = lvl.LevelPointsLimit - points;
                    found = true;
                    break;
                }
            }

            // Se o loop terminou e não encontrou (found == false), 
            // significa que os pontos são maiores que o limite do último nível (Max Level)
            if (!found)
            {
                var maxLevel = levels.Last();
                currentLevelNum = maxLevel.LevelNumber;
                remaining = 0; // Já atingiu o máximo
            }

            return Json(new { level = currentLevelNum, remaining = remaining });
        }
        // -----------------------------

        // GET: Levels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var level = await _context.Level
                .Include(l => l.Category)
                .Include(l => l.Customer)
                .FirstOrDefaultAsync(m => m.LevelId == id);

            if (level == null) return View("InvalidLevel");

            return View(level);
        }

        // GET: Levels/Create
        public IActionResult Create()
        {
            ViewData["LevelCategoryId"] = new SelectList(_context.LevelCategory, "LevelCategoryId", "Name");
            return View(new Level());
        }

        // POST: Levels/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LevelId,LevelNumber,LevelCategoryId,LevelPointsLimit,Description")] Level level)
        {

            // Corrige o problema do formulário não submeter
            ModelState.Remove("Category");
            ModelState.Remove("Customer");

            if (await _context.Level.AnyAsync(l => l.LevelNumber == level.LevelNumber))
            {
                ModelState.AddModelError("LevelNumber", $"Level {level.LevelNumber} already exists.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(level);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Details),
                    new
                    {
                        id = level.LevelId,
                        SuccessMessage = "Level created successfully."
                    }
                );
            }

            ViewData["LevelCategoryId"] = new SelectList(_context.LevelCategory, "LevelCategoryId", "Name", level.LevelCategoryId);
            return View(level);
        }

        // GET: Levels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var level = await _context.Level.FindAsync(id);
            if (level == null) return View("InvalidLevel");

            ViewData["LevelCategoryId"] = new SelectList(_context.LevelCategory, "LevelCategoryId", "Name", level.LevelCategoryId);
            return View(level);
        }

        // POST: Levels/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LevelId,LevelNumber,LevelCategoryId,LevelPointsLimit,Description")] Level level)
        {
            if (id != level.LevelId) return NotFound();

            // Corrige validação no Edit também
            ModelState.Remove("Category");
            ModelState.Remove("Customer");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(level);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Details),
                        new
                        {
                            id = level.LevelId,
                            SuccessMessage = "Level updated successfully."
                        }
                    );
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LevelExists(level.LevelId))
                    {
                        ViewBag.LevelWasDeleted = true;
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["LevelCategoryId"] = new SelectList(_context.LevelCategory, "LevelCategoryId", "Name", level.LevelCategoryId);
            return View(level);
        }

        // GET: Levels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var level = await _context.Level
                .Include(l => l.Category)
                .Include(l => l.Customer)
                .FirstOrDefaultAsync(m => m.LevelId == id);

            if (level == null)
            {
                TempData["ErrorMessage"] = "The Level is no longer available.";
                return RedirectToAction(nameof(Index));
            }

            return View(level);
        }

        // POST: Levels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var level = await _context.Level.FindAsync(id);

            if (level != null)
            {
                try
                {
                    _context.Level.Remove(level);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Level deleted successfully.";
                }
                catch (Exception)
                {
                    level = await _context.Level
                        .AsNoTracking()
                        .Include(l => l.Category)
                        .Include(l => l.Customer)
                        .FirstOrDefaultAsync(l => l.LevelId == id);

                    int numberCustomers = level?.Customer?.Count ?? 0;

                    if (numberCustomers > 0)
                    {
                        ViewBag.Error = $"Unable to delete Level. It is associated with {numberCustomers} Customers. Reassign them first.";
                    }
                    else
                    {
                        ViewBag.Error = "An error occurred while deleting the Level.";
                    }

                    return View(level);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool LevelExists(int id)
        {
            return _context.Level.Any(e => e.LevelId == id);
        }
    }
}