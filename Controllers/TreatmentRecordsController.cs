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
    public class TreatmentRecordsController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public TreatmentRecordsController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: TreatmentRecords
        public async Task<IActionResult> Index()
        {
            var healthWellbeingDbContext = _context.TreatmentRecord.Include(t => t.Nurse).Include(t => t.Pathology).Include(t => t.TreatmentType);
            return View(await healthWellbeingDbContext.ToListAsync());
        }

        // GET: TreatmentRecords/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treatmentRecord = await _context.TreatmentRecord
                .Include(t => t.Nurse)
                .Include(t => t.Pathology)
                .Include(t => t.TreatmentType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (treatmentRecord == null)
            {
                return NotFound();
            }

            return View(treatmentRecord);
        }

        // GET: TreatmentRecords/Create
        public IActionResult Create()
        {
            ViewData["NurseId"] = new SelectList(_context.Nurse, "Id", "Name");
            ViewData["PathologyId"] = new SelectList(_context.Pathology, "Id", "Name");
            ViewData["TreatmentId"] = new SelectList(_context.TreatmentType, "Id", "Name");
            return View();
        }

        // POST: TreatmentRecords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NurseId,TreatmentId,PathologyId,TreatmentDate,DurationMinutes,Remarks,Result,Status,CreatedAt")] TreatmentRecord treatmentRecord)
        {
            if (ModelState.IsValid)
            {
                _context.Add(treatmentRecord);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["NurseId"] = new SelectList(_context.Nurse, "Id", "Email", treatmentRecord.NurseId);
            ViewData["PathologyId"] = new SelectList(_context.Pathology, "Id", "Description", treatmentRecord.PathologyId);
            ViewData["TreatmentId"] = new SelectList(_context.TreatmentType, "Id", "Description", treatmentRecord.TreatmentId);
            return View(treatmentRecord);
        }

        // GET: TreatmentRecords/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treatmentRecord = await _context.TreatmentRecord.FindAsync(id);
            if (treatmentRecord == null)
            {
                return NotFound();
            }
            ViewData["NurseId"] = new SelectList(_context.Nurse, "Id", "Email", treatmentRecord.NurseId);
            ViewData["PathologyId"] = new SelectList(_context.Pathology, "Id", "Description", treatmentRecord.PathologyId);
            ViewData["TreatmentId"] = new SelectList(_context.TreatmentType, "Id", "Description", treatmentRecord.TreatmentId);
            return View(treatmentRecord);
        }

        // POST: TreatmentRecords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NurseId,TreatmentId,PathologyId,TreatmentDate,DurationMinutes,Remarks,Result,Status,CreatedAt")] TreatmentRecord treatmentRecord)
        {
            if (id != treatmentRecord.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(treatmentRecord);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TreatmentRecordExists(treatmentRecord.Id))
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
            ViewData["NurseId"] = new SelectList(_context.Nurse, "Id", "Email", treatmentRecord.NurseId);
            ViewData["PathologyId"] = new SelectList(_context.Pathology, "Id", "Description", treatmentRecord.PathologyId);
            ViewData["TreatmentId"] = new SelectList(_context.TreatmentType, "Id", "Description", treatmentRecord.TreatmentId);
            return View(treatmentRecord);
        }

        // GET: TreatmentRecords/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treatmentRecord = await _context.TreatmentRecord
                .Include(t => t.Nurse)
                .Include(t => t.Pathology)
                .Include(t => t.TreatmentType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (treatmentRecord == null)
            {
                return NotFound();
            }

            return View(treatmentRecord);
        }

        // POST: TreatmentRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var treatmentRecord = await _context.TreatmentRecord.FindAsync(id);
            if (treatmentRecord != null)
            {
                _context.TreatmentRecord.Remove(treatmentRecord);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TreatmentRecordExists(int id)
        {
            return _context.TreatmentRecord.Any(e => e.Id == id);
        }
    }
}
