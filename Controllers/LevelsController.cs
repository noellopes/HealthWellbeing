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

namespace HealthWellbeing.Controllers {
    [Authorize(Roles = "Gestor")]
    public class LevelsController : Controller {
        private readonly HealthWellbeingDbContext _context;

        public LevelsController(HealthWellbeingDbContext context) {
            _context = context;
        }

        // GET: Levels
        // Alterei searchNumber para int? para evitar parsing manual desnecessário
        public async Task<IActionResult> Index(
            int page = 1,
            int? searchNumber = null,
            int? searchCategoryId = null,
            string searchDescription = null,
            int? searchPoints = null) {
            // 1. Iniciar a query
            var levelsQuery = _context.Level
                .Include(l => l.Category)
                .AsQueryable();

            // 2. Aplicar Filtros
            if (searchNumber.HasValue) {
                levelsQuery = levelsQuery.Where(l => l.LevelNumber == searchNumber.Value);
            }

            if (searchCategoryId.HasValue) {
                levelsQuery = levelsQuery.Where(l => l.LevelCategoryId == searchCategoryId);
            }

            if (!string.IsNullOrEmpty(searchDescription)) {
                // Usa o Contains para pesquisa parcial (LIKE '%texto%')
                levelsQuery = levelsQuery.Where(l => l.Description.Contains(searchDescription));
            }

            if (searchPoints.HasValue) {
                levelsQuery = levelsQuery.Where(l => l.LevelPointsLimit == searchPoints.Value);
            }

            // 3. Persistir dados na ViewBag para a View manter os inputs preenchidos
            ViewBag.SearchNumber = searchNumber;
            // Nota: Para SelectList, passamos o valor selecionado diretamente no construtor abaixo
            ViewBag.SearchCategoryId = searchCategoryId;
            ViewBag.SearchDescription = searchDescription;
            ViewBag.SearchPoints = searchPoints;

            // Preencher a dropdown de categorias
            ViewData["LevelCategories"] = new SelectList(_context.LevelCategory, "LevelCategoryId", "Name", searchCategoryId);

            // 4. Paginação
            int totalItems = await levelsQuery.CountAsync();
            var paginationInfo = new ViewModels.PaginationInfo<Level>(page, totalItems);

            // Nota: Se quiseres alterar o tamanho da página, faz-se na classe PaginationInfo,
            // ou podes sobrescrever aqui: paginationInfo.ItemsPerPage = 5;

            paginationInfo.Items = await levelsQuery
                .OrderBy(l => l.LevelNumber)
                .Skip(paginationInfo.ItemsToSkip)
                .Take(paginationInfo.ItemsPerPage)
                .ToListAsync();

            return View(paginationInfo);
        }

        // NOVO: Endpoint para a Calculadora funcionar via AJAX
        [HttpGet]
        public IActionResult CalculateLevel(int points) {
            // --- Lógica de Exemplo ---
            // Ajusta esta matemática para a tua regra de negócio real.
            // Exemplo: Nível 1 = 0-99, Nível 2 = 100-199, etc.

            int level = 1;
            int remaining = 0;

            if (points >= 0) {
                level = (points / 100) + 1; // 150 pts -> nivel 2
                int nextLevelPoints = level * 100;
                remaining = nextLevelPoints - points;
            }

            // Retorna JSON para o JavaScript consumir
            return Json(new { level = level, remaining = remaining });
        }

        // GET: Levels/Details/5
        public async Task<IActionResult> Details(int? id) {
            if (id == null) return NotFound();

            var level = await _context.Level
                .Include(l => l.Category)
                .Include(l => l.Customer)
                .FirstOrDefaultAsync(m => m.LevelId == id);

            if (level == null) return View("InvalidLevel");

            return View(level);
        }

        // GET: Levels/Create
        public IActionResult Create() {
            ViewData["LevelCategoryId"] = new SelectList(_context.LevelCategory, "LevelCategoryId", "Name");
            return View();
        }

        // POST: Levels/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LevelId,LevelNumber,LevelCategoryId,LevelPointsLimit,Description")] Level level) {
            if (await _context.Level.AnyAsync(l => l.LevelNumber == level.LevelNumber)) {
                ModelState.AddModelError("LevelNumber", $"Level {level.LevelNumber} already exists.");
            }

            if (ModelState.IsValid) {
                _context.Add(level);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Details),
                    new {
                        id = level.LevelId,
                        SuccessMessage = "Level created successfully."
                    }
                );
            }

            ViewData["LevelCategoryId"] = new SelectList(_context.LevelCategory, "LevelCategoryId", "Name", level.LevelCategoryId);
            return View(level);
        }

        // GET: Levels/Edit/5
        public async Task<IActionResult> Edit(int? id) {
            if (id == null) return NotFound();

            var level = await _context.Level.FindAsync(id);
            if (level == null) return View("InvalidLevel");

            ViewData["LevelCategoryId"] = new SelectList(_context.LevelCategory, "LevelCategoryId", "Name", level.LevelCategoryId);
            return View(level);
        }

        // POST: Levels/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LevelId,LevelNumber,LevelCategoryId,LevelPointsLimit,Description")] Level level) {
            if (id != level.LevelId) return NotFound();

            if (ModelState.IsValid) {
                try {
                    _context.Update(level);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Details),
                        new {
                            id = level.LevelId,
                            SuccessMessage = "Level updated successfully."
                        }
                    );
                }
                catch (DbUpdateConcurrencyException) {
                    if (!LevelExists(level.LevelId)) {
                        ViewBag.LevelWasDeleted = true;
                    }
                    else {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["LevelCategoryId"] = new SelectList(_context.LevelCategory, "LevelCategoryId", "Name", level.LevelCategoryId);
            return View(level);
        }

        // GET: Levels/Delete/5
        public async Task<IActionResult> Delete(int? id) {
            if (id == null) return NotFound();

            var level = await _context.Level
                .Include(l => l.Category)
                .Include(l => l.Customer)
                .FirstOrDefaultAsync(m => m.LevelId == id);

            if (level == null) {
                TempData["ErrorMessage"] = "The Level is no longer available.";
                return RedirectToAction(nameof(Index));
            }

            return View(level);
        }

        // POST: Levels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var level = await _context.Level.FindAsync(id);

            if (level != null) {
                try {
                    _context.Level.Remove(level);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Level deleted successfully.";
                }
                catch (Exception) {
                    level = await _context.Level
                        .AsNoTracking()
                        .Include(l => l.Category)
                        .Include(l => l.Customer)
                        .FirstOrDefaultAsync(l => l.LevelId == id);

                    int numberCustomers = level?.Customer?.Count ?? 0;

                    if (numberCustomers > 0) {
                        ViewBag.Error = $"Unable to delete Level. It is associated with {numberCustomers} Customers. Reassign them first.";
                    }
                    else {
                        ViewBag.Error = "An error occurred while deleting the Level.";
                    }

                    return View(level);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool LevelExists(int id) {
            return _context.Level.Any(e => e.LevelId == id);
        }
    }
}