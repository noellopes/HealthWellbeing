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
    public class Activity_Controller : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public Activity_Controller(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Activity_
        public async Task<IActionResult> Index()
        {
            return View(await _context.Activity.ToListAsync());
        }

        // GET: Activity_/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity_ = await _context.Activity
                .FirstOrDefaultAsync(m => m.Activity_Id == id);
            if (activity_ == null)
            {
                return NotFound();
            }

            return View(activity_);
        }

        // GET: Activity_/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Activity_/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Activity_Id,Activity_Name,Activity_Description,Activity_Type,NumberSets,NumberReps,Weigth")] Activity_ activity_)
        {
            if (ModelState.IsValid)
            {
                _context.Add(activity_);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(activity_);
        }

        // GET: Activity_/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity_ = await _context.Activity.FindAsync(id);
            if (activity_ == null)
            {
                return NotFound();
            }
            return View(activity_);
        }

        // POST: Activity_/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Activity_Id,Activity_Name,Activity_Description,Activity_Type,NumberSets,NumberReps,Weigth")] Activity_ activity_)
        {
            if (id != activity_.Activity_Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(activity_);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Activity_Exists(activity_.Activity_Id))
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
            return View(activity_);
        }

        // GET: Activity_/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity_ = await _context.Activity
                .FirstOrDefaultAsync(m => m.Activity_Id == id);
            if (activity_ == null)
            {
                return NotFound();
            }

            return View(activity_);
        }

        // POST: Activity_/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var activity_ = await _context.Activity.FindAsync(id);
            if (activity_ != null)
            {
                _context.Activity.Remove(activity_);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Activity_Exists(int id)
        {
            return _context.Activity.Any(e => e.Activity_Id == id);
        }
    }
}
