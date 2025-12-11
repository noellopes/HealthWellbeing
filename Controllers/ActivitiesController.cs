using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers
{
    public class ActivitiesController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ActivitiesController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Activities
        public async Task<IActionResult> Index()
        {
            var activities = await _context.Activity.ToListAsync();
            return View(activities);
        }

        // GET: Activities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity = await _context.Activity
                .FirstOrDefaultAsync(m => m.ActivityId == id);

            if (activity == null)
            {
                return NotFound();
            }

            return View(activity);
        }

        // GET: Activities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Activities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("ActivityId,ActivityName,ActivityType,ActivityDescription,ActivityReward")]
            Activity activity)
        {
            if (!ModelState.IsValid)
            {
                return View(activity);
            }

            _context.Add(activity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Activities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity = await _context.Activity.FindAsync(id);
            if (activity == null)
            {
                return NotFound();
            }

            return View(activity);
        }

        // POST: Activities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("ActivityId,ActivityName,ActivityType,ActivityDescription,ActivityReward")]
            Activity activity)
        {
            if (id != activity.ActivityId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(activity);
            }

            try
            {
                _context.Update(activity);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActivityExists(activity.ActivityId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Activities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity = await _context.Activity
                .FirstOrDefaultAsync(m => m.ActivityId == id);

            if (activity == null)
            {
                return NotFound();
            }

            return View(activity);
        }

        // POST: Activities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var activity = await _context.Activity.FindAsync(id);
            if (activity != null)
            {
                _context.Activity.Remove(activity);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ActivityExists(int id)
        {
            return _context.Activity.Any(e => e.ActivityId == id);
        }
    }
}
