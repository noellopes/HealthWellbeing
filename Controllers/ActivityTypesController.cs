using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Controllers {
    [Authorize(Roles = "Gestor")]
    public class ActivityTypesController : Controller {
        private readonly HealthWellbeingDbContext _context;

        public ActivityTypesController(HealthWellbeingDbContext context) {
            _context = context;
        }

        // GET: ActivityTypes
        public async Task<IActionResult> Index(string searchName, int page = 1) {

            var query = _context.ActivityType
                .Include(at => at.Activities)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchName)) {
                query = query.Where(at => at.Name.Contains(searchName));
            }

            ViewBag.SearchName = searchName;

            int totalItems = await query.CountAsync();
            var paginationInfo = new ViewModels.PaginationInfo<ActivityType>(page, totalItems);

            paginationInfo.Items = await query
                .OrderBy(at => at.Name)
                .Skip(paginationInfo.ItemsToSkip)
                .Take(paginationInfo.ItemsPerPage)
                .ToListAsync();

            return View(paginationInfo);
        }

        // GET: ActivityTypes/Details/5
        public async Task<IActionResult> Details(int? id) {
            if (id == null) return View("InvalidActivityType");

            var activityType = await _context.ActivityType
                .Include(at => at.Activities)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ActivityTypeId == id);

            if (activityType == null) return View("InvalidActivityType");

            return View(activityType);
        }

        // GET: ActivityTypes/Create
        public IActionResult Create() {
            return View();
        }

        // POST: ActivityTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ActivityTypeId,Name,Description")] ActivityType activityType) {
            if (ModelState.IsValid) {
                _context.Add(activityType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(activityType);
        }

        // GET: ActivityTypes/Edit/5
        public async Task<IActionResult> Edit(int? id) {
            if (id == null) return View("InvalidActivityType");

            var activityType = await _context.ActivityType.FindAsync(id);
            if (activityType == null) return View("InvalidActivityType");

            return View(activityType);
        }

        // POST: ActivityTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ActivityTypeId,Name,Description")] ActivityType activityType) {
            if (id != activityType.ActivityTypeId) return View("InvalidActivityType");

            if (ModelState.IsValid) {
                try {
                    _context.Update(activityType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException) {
                    if (!ActivityTypeExists(activityType.ActivityTypeId)) return View("InvalidActivityType");
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(activityType);
        }

        // GET: ActivityTypes/Delete/5
        public async Task<IActionResult> Delete(int? id) {
            if (id == null) return View("InvalidActivityType");

            var activityType = await _context.ActivityType
                .Include(at => at.Activities)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ActivityTypeId == id);

            if (activityType == null) return View("InvalidActivityType");

            return View(activityType);
        }

        // POST: ActivityTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var activityType = await _context.ActivityType
                .Include(at => at.Activities)
                    .ThenInclude(a => a.CustomerActivities)
                .Include(at => at.Activities)
                    .ThenInclude(a => a.EventActivities)
                .FirstOrDefaultAsync(m => m.ActivityTypeId == id);

            if (activityType == null) return View("InvalidActivityType");

            // Check deep history
            bool hasDeepHistory = activityType.Activities.Any(a =>
                (a.CustomerActivities != null && a.CustomerActivities.Any()) ||
                (a.EventActivities != null && a.EventActivities.Any())
            );

            if (hasDeepHistory) {
                ViewBag.ErrorTitle = "Cannot Delete Category";
                ViewBag.ErrorMessage = "This Category contains Activities that have active history (users or events). Deleting it would destroy data integrity. You must archive them instead.";
                return View(activityType);
            }

            try {
                // Delete children first (safe because we checked history above)
                if (activityType.Activities != null && activityType.Activities.Any()) {
                    _context.Activity.RemoveRange(activityType.Activities);
                }

                _context.ActivityType.Remove(activityType);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Category and its unused activities were deleted successfully.";
            }
            catch (DbUpdateException) {
                ViewBag.ErrorTitle = "Database Error";
                ViewBag.ErrorMessage = "An unexpected technical error occurred.";
                return View(activityType);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ActivityTypeExists(int id) {
            return _context.ActivityType.Any(e => e.ActivityTypeId == id);
        }
    }
}