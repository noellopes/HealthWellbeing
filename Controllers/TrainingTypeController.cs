﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers
{
    public class TrainingTypeController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public TrainingTypeController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: TrainingType
        public async Task<IActionResult> Index()
        {
            return View(await _context.TrainingType.ToListAsync());
        }

        // GET: TrainingType/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainingType = await _context.TrainingType
                .FirstOrDefaultAsync(m => m.TrainingTypeId == id);

            if (trainingType == null)
            {
                return View("InvalidTrainingType"); // Optional custom view
            }

            return View(trainingType);
        }

        // GET: TrainingType/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TrainingType/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,DurationMinutes,Intensity,IsActive")] TrainingType trainingType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trainingType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(trainingType);
        }

        // GET: TrainingType/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainingType = await _context.TrainingType.FindAsync(id);
            if (trainingType == null)
            {
                return NotFound();
            }
            return View(trainingType);
        }

        // POST: TrainingType/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TrainingTypeId,Name,Description,DurationMinutes,Intensity,IsActive")] TrainingType trainingType)
        {
            if (id != trainingType.TrainingTypeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trainingType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainingTypeExists(trainingType.TrainingTypeId))
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
            return View(trainingType);
        }

        // GET: TrainingType/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainingType = await _context.TrainingType
                .FirstOrDefaultAsync(m => m.TrainingTypeId == id);

            if (trainingType == null)
            {
                return NotFound();
            }

            return View(trainingType);
        }

        // POST: TrainingType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trainingType = await _context.TrainingType.FindAsync(id);
            if (trainingType != null)
            {
                _context.TrainingType.Remove(trainingType);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TrainingTypeExists(int id)
        {
            return _context.TrainingType.Any(e => e.TrainingTypeId == id);
        }
    }
}
