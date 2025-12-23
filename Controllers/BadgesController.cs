using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Authorization;
using HealthWellbeing.ViewModels;

namespace HealthWellbeing.Controllers {
    [Authorize(Roles = "Gestor")]
    public class BadgesController : Controller {
        private readonly HealthWellbeingDbContext _context;

        public BadgesController(HealthWellbeingDbContext context) {
            _context = context;
        }

        // GET: Badges
        public async Task<IActionResult> Index(int page = 1, string searchName = "", int? searchTypeId = null, int? searchPoints = null, string searchStatus = "") {
            var query = _context.Badge
                .Include(b => b.BadgeType)
                .AsNoTracking()
                .AsQueryable();

            // Filtro por Nome
            if (!string.IsNullOrEmpty(searchName)) {
                query = query.Where(b => b.BadgeName.Contains(searchName));
            }

            // Filtro por Tipo
            if (searchTypeId.HasValue) {
                query = query.Where(b => b.BadgeTypeId == searchTypeId);
            }

            // Filtro por Pontos
            if (searchPoints.HasValue) {
                query = query.Where(b => b.RewardPoints == searchPoints);
            }

            // Filtro por Status
            if (!string.IsNullOrEmpty(searchStatus)) {
                if (searchStatus == "active") {
                    query = query.Where(b => b.IsActive);
                }
                else if (searchStatus == "archived") {
                    query = query.Where(b => !b.IsActive);
                }
            }

            // Manter Estado na View
            ViewBag.SearchName = searchName;
            ViewBag.SearchTypeId = searchTypeId;
            ViewBag.SearchPoints = searchPoints;
            ViewBag.SearchStatus = searchStatus;

            // Popular Dropdown de Tipos
            ViewData["BadgeTypes"] = new SelectList(_context.BadgeType, "BadgeTypeId", "BadgeTypeName", searchTypeId);

            // Paginação
            int totalItems = await query.CountAsync();
            var paginationInfo = new ViewModels.PaginationInfo<Badge>(page, totalItems);

            paginationInfo.Items = await query
                .OrderBy(b => b.BadgeName)
                .Skip(paginationInfo.ItemsToSkip)
                .Take(paginationInfo.ItemsPerPage)
                .ToListAsync();

            return View(paginationInfo);
        }

        // GET: Badges/Details/5
        public async Task<IActionResult> Details(int? id) {
            if (id == null) return NotFound();

            var badge = await _context.Badge
                .Include(b => b.BadgeType)
                .Include(b => b.BadgeRequirements) 
                .Include(b => b.CustomerBadges)   
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.BadgeId == id);

            if (badge == null) return NotFound();

            return View(badge);
        }

        // GET: Badges/Create
        public IActionResult Create() {
            ViewData["BadgeTypeId"] = new SelectList(_context.BadgeType, "BadgeTypeId", "BadgeTypeName");
            return View();
        }

        // POST: Badges/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BadgeId,BadgeTypeId,BadgeName,BadgeDescription,RewardPoints,IsActive")] Badge badge) {
            if (ModelState.IsValid) {
                _context.Add(badge);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Badge created successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["BadgeTypeId"] = new SelectList(_context.BadgeType, "BadgeTypeId", "BadgeTypeName", badge.BadgeTypeId);
            return View(badge);
        }

        // GET: Badges/Edit/5
        public async Task<IActionResult> Edit(int? id) {
            if (id == null) return NotFound();

            var badge = await _context.Badge.FindAsync(id);
            if (badge == null) return NotFound();

            ViewData["BadgeTypeId"] = new SelectList(_context.BadgeType, "BadgeTypeId", "BadgeTypeName", badge.BadgeTypeId);
            return View(badge);
        }

        // POST: Badges/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BadgeId,BadgeTypeId,BadgeName,BadgeDescription,RewardPoints,IsActive")] Badge badge) {
            if (id != badge.BadgeId) return NotFound();

            if (ModelState.IsValid) {
                try {
                    _context.Update(badge);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Badge updated successfully.";
                }
                catch (DbUpdateConcurrencyException) {
                    if (!BadgeExists(badge.BadgeId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BadgeTypeId"] = new SelectList(_context.BadgeType, "BadgeTypeId", "BadgeTypeName", badge.BadgeTypeId);
            return View(badge);
        }

        // GET: Badges/Delete/5
        public async Task<IActionResult> Delete(int? id) {
            if (id == null) return NotFound();

            var badge = await _context.Badge
                .Include(b => b.BadgeType)
                .Include(b => b.CustomerBadges)
                .FirstOrDefaultAsync(m => m.BadgeId == id);

            if (badge == null) {
                TempData["ErrorMessage"] = "Badge not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(badge);
        }

        // POST: Badges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var badge = await _context.Badge
                .Include(b => b.BadgeType)
                .Include(b => b.CustomerBadges) 
                .FirstOrDefaultAsync(b => b.BadgeId == id);

            if (badge != null) {
                // Validação de Integridade: Clientes
                if (badge.CustomerBadges != null && badge.CustomerBadges.Any()) {
                    ViewBag.Error = "This badge has been awarded to customers. Please prefer deactivating it via the 'Edit' page to preserve user records.";
                    return View(badge);
                }

                try {
                    _context.Badge.Remove(badge);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Badge deleted successfully.";
                }
                catch (DbUpdateException) {
                    ViewBag.Error = "There are technical dependencies linked to this badge.";
                    return View(badge);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool BadgeExists(int id) {
            return _context.Badge.Any(e => e.BadgeId == id);
        }
    }
}