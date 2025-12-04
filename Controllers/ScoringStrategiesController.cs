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
    public class ScoringStrategiesController : Controller {
        private readonly HealthWellbeingDbContext _context;

        public ScoringStrategiesController(HealthWellbeingDbContext context) {
            _context = context;
        }

        // GET: ScoringStrategies
        public async Task<IActionResult> Index(int page = 1, string searchName = "", string searchCode = "") {
            var strategiesQuery = _context.ScoringStrategy.AsQueryable();

            // 1. Aplicar filtros
            if (!string.IsNullOrEmpty(searchName)) {
                strategiesQuery = strategiesQuery.Where(s => s.ScoringStrategyName.Contains(searchName));
            }

            if (!string.IsNullOrEmpty(searchCode)) {
                strategiesQuery = strategiesQuery.Where(s => s.ScoringStrategyCode.Contains(searchCode));
            }

            // Guardar valores na ViewBag para a View
            ViewBag.SearchName = searchName;
            ViewBag.SearchCode = searchCode;

            // 2. Paginação
            int totalItems = await strategiesQuery.CountAsync();
            var paginationInfo = new ViewModels.PaginationInfo<ScoringStrategy>(page, totalItems);

            paginationInfo.Items = await strategiesQuery
                .OrderBy(s => s.ScoringStrategyName)
                .Skip(paginationInfo.ItemsToSkip)
                .Take(paginationInfo.ItemsPerPage)
                .ToListAsync();

            return View(paginationInfo);
        }

        // GET: ScoringStrategies/Details/5
        public async Task<IActionResult> Details(int? id) {
            if (id == null) {
                return NotFound();
            }

            var scoringStrategy = await _context.ScoringStrategy
                .Include(s => s.EventTypes)
                .FirstOrDefaultAsync(m => m.ScoringStrategyId == id);

            if (scoringStrategy == null) {
                return View("InvalidScoringStrategy");
            }

            return View(scoringStrategy);
        }

        // GET: ScoringStrategies/Create
        public IActionResult Create() {
            return View();
        }

        // POST: ScoringStrategies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ScoringStrategyId,ScoringStrategyName,ScoringStrategyCode,ScoringStrategyDescription")] ScoringStrategy scoringStrategy) {
            if (ModelState.IsValid) {
                _context.Add(scoringStrategy);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Details),
                    new {
                        id = scoringStrategy.ScoringStrategyId,
                        SuccessMessage = "Scoring Strategy created successfully."
                    }
                );
            }
            return View(scoringStrategy);
        }

        // GET: ScoringStrategies/Edit/5
        public async Task<IActionResult> Edit(int? id) {
            if (id == null) {
                return NotFound();
            }

            var scoringStrategy = await _context.ScoringStrategy.FindAsync(id);
            if (scoringStrategy == null) {
                return View("InvalidScoringStrategy");
            }
            return View(scoringStrategy);
        }

        // POST: ScoringStrategies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ScoringStrategyId,ScoringStrategyName,ScoringStrategyCode,ScoringStrategyDescription")] ScoringStrategy scoringStrategy) {
            if (id != scoringStrategy.ScoringStrategyId) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                try {
                    _context.Update(scoringStrategy);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Details),
                        new {
                            id = scoringStrategy.ScoringStrategyId,
                            SuccessMessage = "Scoring Strategy updated successfully."
                        }
                    );
                }
                catch (DbUpdateConcurrencyException) {
                    if (!ScoringStrategyExists(scoringStrategy.ScoringStrategyId)) {
                        ViewBag.ScoringStrategyWasDeleted = true; // Flag para a View tratar concorrência
                    }
                    else {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index)); // Em caso de erro de concorrência tratado na view, ou sucesso sem redirect para details
            }
            return View(scoringStrategy);
        }

        // GET: ScoringStrategies/Delete/5
        public async Task<IActionResult> Delete(int? id) {
            if (id == null) {
                return NotFound();
            }

            var scoringStrategy = await _context.ScoringStrategy
                .Include(s => s.EventTypes)
                .FirstOrDefaultAsync(m => m.ScoringStrategyId == id);

            if (scoringStrategy == null) {
                TempData["ErrorMessage"] = "The Scoring Strategy is no longer available.";
                return RedirectToAction(nameof(Index));
            }

            return View(scoringStrategy);
        }

        // POST: ScoringStrategies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var scoringStrategy = await _context.ScoringStrategy.FindAsync(id);

            if (scoringStrategy != null) {
                try {
                    _context.ScoringStrategy.Remove(scoringStrategy);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Scoring Strategy deleted successfully.";
                }
                catch (Exception) {
                    scoringStrategy = await _context.ScoringStrategy
                        .Include(s => s.EventTypes)
                        .FirstOrDefaultAsync(s => s.ScoringStrategyId == id);

                    // Calcular o número para a mensagem de erro textual
                    int numberEventTypes = scoringStrategy?.EventTypes?.Count ?? 0;

                    if (numberEventTypes > 0) {
                        ViewBag.Error = $"Unable to delete. This Strategy is used by {numberEventTypes} Event Types. Please reassign or delete the Event Types first.";
                    }
                    else {
                        ViewBag.Error = "An error occurred while deleting the Scoring Strategy.";
                    }

                    // Recarregar o objeto para a View
                    return View(scoringStrategy);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ScoringStrategyExists(int id) {
            return _context.ScoringStrategy.Any(e => e.ScoringStrategyId == id);
        }
    }
}