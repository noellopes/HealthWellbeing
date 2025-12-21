using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace HealthWellbeing.Controllers {
    [Authorize(Roles = "Gestor")] // 1. Segurança aplicada
    public class BadgeTypesController : Controller {
        private readonly HealthWellbeingDbContext _context;

        public BadgeTypesController(HealthWellbeingDbContext context) {
            _context = context;
        }

        // GET: BadgeTypes
        // 2. Adicionada Pesquisa e Paginação
        public async Task<IActionResult> Index(int page = 1, string searchName = "") {
            var query = _context.BadgeType
                .AsNoTracking() // Otimização de performance para leitura
                .AsQueryable();

            // Filtro por nome
            if (!string.IsNullOrEmpty(searchName)) {
                query = query.Where(b => b.BadgeTypeName.Contains(searchName));
            }

            // Persistir a pesquisa na View
            ViewBag.SearchName = searchName;

            // Paginação
            int totalItems = await query.CountAsync();
            var paginationInfo = new ViewModels.PaginationInfo<BadgeType>(page, totalItems);

            paginationInfo.Items = await query
                .OrderBy(b => b.BadgeTypeName)
                .Skip(paginationInfo.ItemsToSkip)
                .Take(paginationInfo.ItemsPerPage)
                .ToListAsync();

            return View(paginationInfo);
        }

        // GET: BadgeTypes/Details/5
        public async Task<IActionResult> Details(int? id) {
            if (id == null) return NotFound();

            var badgeType = await _context.BadgeType
                .Include(bt => bt.Badges) // Incluir Badges para mostrar contagem se necessário
                .FirstOrDefaultAsync(m => m.BadgeTypeId == id);

            if (badgeType == null) return NotFound();

            return View(badgeType);
        }

        // GET: BadgeTypes/Create
        public IActionResult Create() {
            return View();
        }

        // POST: BadgeTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BadgeTypeId,BadgeTypeName,BadgeTypeDescription")] BadgeType badgeType) {
            if (ModelState.IsValid) {
                _context.Add(badgeType);
                await _context.SaveChangesAsync();

                // 3. Mensagem de Feedback
                TempData["SuccessMessage"] = "Badge Type created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(badgeType);
        }

        // GET: BadgeTypes/Edit/5
        public async Task<IActionResult> Edit(int? id) {
            if (id == null) return NotFound();

            var badgeType = await _context.BadgeType.FindAsync(id);
            if (badgeType == null) return NotFound();

            return View(badgeType);
        }

        // POST: BadgeTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BadgeTypeId,BadgeTypeName,BadgeTypeDescription")] BadgeType badgeType) {
            if (id != badgeType.BadgeTypeId) return NotFound();

            if (ModelState.IsValid) {
                try {
                    _context.Update(badgeType);
                    await _context.SaveChangesAsync();

                    // 3. Mensagem de Feedback
                    TempData["SuccessMessage"] = "Badge Type updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException) {
                    if (!BadgeTypeExists(badgeType.BadgeTypeId)) return NotFound();
                    else throw;
                }
            }
            return View(badgeType);
        }

        // GET: BadgeTypes/Delete/5
        public async Task<IActionResult> Delete(int? id) {
            if (id == null) return NotFound();

            var badgeType = await _context.BadgeType
                .Include(bt => bt.Badges) // Incluir Badges para verificar dependências
                .FirstOrDefaultAsync(m => m.BadgeTypeId == id);

            if (badgeType == null) {
                TempData["ErrorMessage"] = "Badge Type not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(badgeType);
        }

        // POST: BadgeTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            // Carregamos com Include para verificar dependências
            var badgeType = await _context.BadgeType
                .Include(bt => bt.Badges)
                .FirstOrDefaultAsync(m => m.BadgeTypeId == id);

            if (badgeType != null) {
                // 4. Validação de Integridade (Crucial para NoAction)
                // Se já existirem Badges deste tipo, impedimos a eliminação.
                if (badgeType.Badges != null && badgeType.Badges.Any()) {
                    ViewBag.Error = "Unable to delete. This badge type is currently associated with active badges.";
                    return View(badgeType); // Retorna à página de delete com o erro
                }

                try {
                    _context.BadgeType.Remove(badgeType);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Badge Type deleted successfully.";
                }
                catch (DbUpdateException) {
                    ViewBag.Error = "Unable to delete due to database constraints.";
                    return View(badgeType);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool BadgeTypeExists(int id) {
            return _context.BadgeType.Any(e => e.BadgeTypeId == id);
        }
    }
}