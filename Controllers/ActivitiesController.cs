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

namespace HealthWellbeing.Controllers {
    [Authorize(Roles = "Gestor")] // SECURITY: Only Managers can manage activities
    public class ActivitiesController : Controller {
        private readonly HealthWellbeingDbContext _context;

        public ActivitiesController(HealthWellbeingDbContext context) {
            _context = context;
        }

        // GET: Activities
        public async Task<IActionResult> Index() {
            // PERFORMANCE: AsNoTracking for read-only lists
            var activities = _context.Activity
                .Include(a => a.ActivityType)
                .AsNoTracking();

            return View(await activities.ToListAsync());
        }

        // GET: Activities/Details/5
        public async Task<IActionResult> Details(int? id) {
            if (id == null) return NotFound();

            var activity = await _context.Activity
                .Include(a => a.ActivityType)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ActivityId == id);

            if (activity == null) return NotFound();

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
            if (id == null) return NotFound();

            var activity = await _context.Activity.FindAsync(id);
            if (activity == null) return NotFound();

            ViewData["ActivityTypeId"] = new SelectList(_context.ActivityType, "ActivityTypeId", "Name", activity.ActivityTypeId);
            return View(activity);
        }

        // POST: Activities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ActivityId,ActivityName,ActivityDescription,ActivityReward,ActivityTypeId")] Activity activity) {
            if (id != activity.ActivityId) return NotFound();

            if (ModelState.IsValid) {
                try {
                    _context.Update(activity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException) {
                    if (!ActivityExists(activity.ActivityId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ActivityTypeId"] = new SelectList(_context.ActivityType, "ActivityTypeId", "Name", activity.ActivityTypeId);
            return View(activity);
        }

        // GET: Activities/Delete/5
        public async Task<IActionResult> Delete(int? id) {
            if (id == null) return NotFound();

            var activity = await _context.Activity
                .Include(a => a.ActivityType)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ActivityId == id);

            if (activity == null) return NotFound();

            return View(activity);
        }

        // POST: Activities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            // Fetch Entity 
            var activity = await _context.Activity
                .Include(a => a.CustomerActivities) 
                .Include(a => a.EventActivities)    
                .FirstOrDefaultAsync(a => a.ActivityId == id);

            if (activity == null) return NotFound();

            // DATA INTEGRITY CHECK (The "Badge" Logic)
            // If this activity has ever been performed by a user OR is linked to an event, DO NOT DELETE.
            bool hasUserHistory = activity.CustomerActivities != null && activity.CustomerActivities.Any();
            bool isLinkedToEvents = activity.EventActivities != null && activity.EventActivities.Any();

            if (hasUserHistory || isLinkedToEvents) {
                // Send specific error message to the View
                ViewBag.ErrorTitle = "Cannot Delete Activity";

                if (hasUserHistory)
                    ViewBag.ErrorMessage = "This activity exists in customer history records. Deleting it would compromise data integrity.";
                else
                    ViewBag.ErrorMessage = "This activity is currently linked to one or more Events. Please remove it from the events first.";

                return View(activity);
            }

            // Safe Delete
            try {
                _context.Activity.Remove(activity);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException) {
                // Fallback for database-level constraints
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