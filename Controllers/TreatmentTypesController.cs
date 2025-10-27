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
    public class TreatmentTypesController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public TreatmentTypesController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: TreatmentTypes
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Tipos de tratamentos";
            ViewBag.Properties = new List<string> { "Name", "Description", "EstimatedDuration", "Priority" };
            return View("~/Views/Shared/Group1/_ListViewLayout.cshtml", await _context.TreatmentType.ToListAsync());
        }

        // GET: TreatmentTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treatmentType = await _context.TreatmentType
                .FirstOrDefaultAsync(m => m.Id == id);
            if (treatmentType == null)
            {
                return NotFound();
            }

            return View(treatmentType);
        }

        // GET: TreatmentTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TreatmentTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,EstimatedDuration,Priority")] TreatmentType treatmentType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(treatmentType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(treatmentType);
        }

        // GET: TreatmentTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treatmentType = await _context.TreatmentType.FindAsync(id);
            if (treatmentType == null)
            {
                return NotFound();
            }
            return View(treatmentType);
        }

        // POST: TreatmentTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,EstimatedDuration,Priority")] TreatmentType treatmentType)
        {
            if (id != treatmentType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(treatmentType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TreatmentTypeExists(treatmentType.Id))
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
            return View(treatmentType);
        }

        // GET: TreatmentTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treatmentType = await _context.TreatmentType
                .FirstOrDefaultAsync(m => m.Id == id);
            if (treatmentType == null)
            {
                return NotFound();
            }

            return View(treatmentType);
        }

        // POST: TreatmentTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var treatmentType = await _context.TreatmentType.FindAsync(id);
            if (treatmentType != null)
            {
                _context.TreatmentType.Remove(treatmentType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TreatmentTypeExists(int id)
        {
            return _context.TreatmentType.Any(e => e.Id == id);
        }
    }
}
