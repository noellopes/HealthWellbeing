using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers
{
    public class BadgesController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public BadgesController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Badges
        public async Task<IActionResult> Index()
        {
            var healthWellbeingDbContext = _context.Badge.Include(b => b.BadgeType);
            return View(await healthWellbeingDbContext.ToListAsync());
        }

        // GET: Badges/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var badge = await _context.Badge
                .Include(b => b.BadgeType)
                .FirstOrDefaultAsync(m => m.BadgeId == id);
            if (badge == null)
            {
                return NotFound();
            }

            return View(badge);
        }

        // GET: Badges/Create
        public IActionResult Create()
        {
            ViewData["BadgeTypeId"] = new SelectList(_context.BadgeType, "BadgeTypeId", "BadgeTypeName");
            return View();
        }

        // POST: Badges/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BadgeId,BadgeTypeId,BadgeName,BadgeDescription,RewardPoints")] Badge badge)
        {
            if (ModelState.IsValid)
            {
                _context.Add(badge);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BadgeTypeId"] = new SelectList(_context.BadgeType, "BadgeTypeId", "BadgeTypeName", badge.BadgeTypeId);
            return View(badge);
        }

        // GET: Badges/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var badge = await _context.Badge.FindAsync(id);
            if (badge == null)
            {
                return NotFound();
            }
            ViewData["BadgeTypeId"] = new SelectList(_context.BadgeType, "BadgeTypeId", "BadgeTypeName", badge.BadgeTypeId);
            return View(badge);
        }

        // POST: Badges/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BadgeId,BadgeTypeId,BadgeName,BadgeDescription,RewardPoints")] Badge badge)
        {
            if (id != badge.BadgeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(badge);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BadgeExists(badge.BadgeId))
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
            ViewData["BadgeTypeId"] = new SelectList(_context.BadgeType, "BadgeTypeId", "BadgeTypeName", badge.BadgeTypeId);
            return View(badge);
        }

        // GET: Badges/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var badge = await _context.Badge
                .Include(b => b.BadgeType)
                .FirstOrDefaultAsync(m => m.BadgeId == id);
            if (badge == null)
            {
                return NotFound();
            }

            return View(badge);
        }

        // POST: Badges/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var badge = await _context.Badge.FindAsync(id);
            if (badge != null)
            {
                _context.Badge.Remove(badge);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BadgeExists(int id)
        {
            return _context.Badge.Any(e => e.BadgeId == id);
        }
    }
}
