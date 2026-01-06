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
    public class ActivitiesController : Controller {
        private readonly HealthWellbeingDbContext _context;

        public ActivitiesController(HealthWellbeingDbContext context) {
            _context = context;
        }

        // GET: Activities
        public async Task<IActionResult> Index(string searchName, int? searchTypeId, int? searchPoints, int page = 1) {
            var query = _context.Activity
                .Include(a => a.ActivityType)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchName))
                query = query.Where(a => a.ActivityName.Contains(searchName));

            if (searchTypeId.HasValue)
                query = query.Where(a => a.ActivityTypeId == searchTypeId);

            if (searchPoints.HasValue)
                query = query.Where(a => a.ActivityReward == searchPoints);

            ViewBag.SearchName = searchName;
            ViewBag.SearchTypeId = searchTypeId;
            ViewBag.SearchPoints = searchPoints;

            ViewData["ActivityTypes"] = new SelectList(_context.ActivityType, "ActivityTypeId", "Name", searchTypeId);

            int totalItems = await query.CountAsync();
            var paginationInfo = new ViewModels.PaginationInfo<Activity>(page, totalItems);

            paginationInfo.Items = await query
                .OrderBy(a => a.ActivityName)
                .Skip(paginationInfo.ItemsToSkip)
                .Take(paginationInfo.ItemsPerPage)
                .ToListAsync();

            return View(paginationInfo);
        }

        // GET: Activities/Details/5
        public async Task<IActionResult> Details(int? id) {
            // ALTERADO: Se id for nulo, mostra a view personalizada
            if (id == null) return View("InvalidActivity");

            var activity = await _context.Activity
                .Include(a => a.ActivityType)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ActivityId == id);

            // ALTERADO: Se não encontrar a atividade
            if (activity == null) return View("InvalidActivity");

            return View(activity);
        }

        // GET: Activities/Create
        public IActionResult Create() {
            ViewData["ActivityTypeId"] = new SelectList(_context.ActivityType, "ActivityTypeId", "Name");
            return View();
        }

        // POST: Activities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ActivityId,ActivityName,ActivityDescription,ActivityReward,ActivityTypeId")] Activity activity) {
            if (ModelState.IsValid) {
                _context.Add(activity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ActivityTypeId"] = new SelectList(_context.ActivityType, "ActivityTypeId", "Name", activity.ActivityTypeId);
            return View(activity);
        }

        // GET: Activities/Edit/5
        public async Task<IActionResult> Edit(int? id) {
            // ALTERADO
            if (id == null) return View("InvalidActivity");

            var activity = await _context.Activity.FindAsync(id);
            // ALTERADO
            if (activity == null) return View("InvalidActivity");

            ViewData["ActivityTypeId"] = new SelectList(_context.ActivityType, "ActivityTypeId", "Name", activity.ActivityTypeId);
            return View(activity);
        }

        // POST: Activities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ActivityId,ActivityName,ActivityDescription,ActivityReward,ActivityTypeId")] Activity activity) {
            // ALTERADO: Proteção contra ID adulterado
            if (id != activity.ActivityId) return View("InvalidActivity");

            if (ModelState.IsValid) {
                try {
                    _context.Update(activity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException) {
                    if (!ActivityExists(activity.ActivityId)) return View("InvalidActivity"); // ALTERADO
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ActivityTypeId"] = new SelectList(_context.ActivityType, "ActivityTypeId", "Name", activity.ActivityTypeId);
            return View(activity);
        }

        // GET: Activities/Delete/5
        public async Task<IActionResult> Delete(int? id) {
            // ALTERADO
            if (id == null) return View("InvalidActivity");

            var activity = await _context.Activity
                .Include(a => a.ActivityType)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ActivityId == id);

            // ALTERADO
            if (activity == null) return View("InvalidActivity");

            return View(activity);
        }

        // POST: Activities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var activity = await _context.Activity
                .Include(a => a.CustomerActivities)
                .Include(a => a.EventActivities)
                .FirstOrDefaultAsync(a => a.ActivityId == id);

            // ALTERADO: Caso alguém apague ao mesmo tempo que tu
            if (activity == null) return View("InvalidActivity");

            // Validações de integridade
            bool hasUserHistory = activity.CustomerActivities != null && activity.CustomerActivities.Any();
            bool isLinkedToEvents = activity.EventActivities != null && activity.EventActivities.Any();

            if (hasUserHistory || isLinkedToEvents) {
                ViewBag.ErrorTitle = "Cannot Delete Activity";

                if (hasUserHistory)
                    ViewBag.ErrorMessage = "This activity exists in customer history records. Deleting it would compromise data integrity.";
                else
                    ViewBag.ErrorMessage = "This activity is currently linked to one or more Events. Please remove it from the events first.";

                // Reload Type to prevent view crash
                await _context.Entry(activity).Reference(a => a.ActivityType).LoadAsync();
                return View(activity);
            }

            try {
                _context.Activity.Remove(activity);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException) {
                ViewBag.ErrorTitle = "Database Error";
                ViewBag.ErrorMessage = "A technical dependency prevents deletion.";
                return View(activity);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ActivityExists(int id) {
            return _context.Activity.Any(e => e.ActivityId == id);
        }
    }
}