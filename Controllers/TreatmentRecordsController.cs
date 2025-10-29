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
            ViewData["Title"] = "Lista de tratamentos";
            ViewBag.ModelType = typeof(TreatmentRecord);
            ViewBag.Properties = new List<string> { "Nurse.Name", "TreatmentType.Name", "Pathology.Name", "TreatmentDate", "DurationMinutes", "Remarks", "Result", "Status", "CreatedAt" };
            return View("~/Views/Shared/Group1/Actions/Index.cshtml", await healthWellbeingDbContext.ToListAsync());
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

            ViewData["Title"] = "Detalhes do tratamento";
            ViewBag.ModelType = typeof(TreatmentRecord);
            ViewBag.Properties = new List<string> { "Nurse.Name", "TreatmentType.Name", "Pathology.Name", "TreatmentDate", "DurationMinutes", "Remarks", "Result", "Status", "CreatedAt" };
            return View("~/Views/Shared/Group1/Actions/Details.cshtml", treatmentRecord);
        }

        // GET: TreatmentRecords/Create
        public IActionResult Create()
        {
            ViewData["Title"] = "Marcação de tratamento";
            ViewBag.ModelType = typeof(TreatmentRecord);
            ViewBag.Properties = new List<string> { "NurseId", "PathologyId", "TreatmentId", "TreatmentDate", "DurationMinutes", "Remarks", "Result", "Status" };
            ViewBag.NurseId = new SelectList(_context.Nurse, "Id", "Name");
            ViewBag.PathologyId = new SelectList(_context.Pathology, "Id", "Name");
            ViewBag.TreatmentId = new SelectList(_context.TreatmentType, "Id", "Name");
            return View("~/Views/Shared/Group1/Actions/Edit.cshtml");
        }

        // POST: TreatmentRecords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NurseId,TreatmentId,PathologyId,TreatmentDate,DurationMinutes,Remarks,Result,Status")] TreatmentRecord treatmentRecord)
        {
            ViewData["Title"] = "Marcação de tratamento";
            treatmentRecord.CreatedAt = DateTime.Now;

            if (ModelState.IsValid)
            {
                _context.Add(treatmentRecord);
                await _context.SaveChangesAsync();
                TempData["AlertType"] = "success";
                TempData["IconClass"] = "bi bi-check-circle";
                TempData["Message"] = "Treatment record created successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ModelType = typeof(TreatmentRecord);
            ViewBag.Properties = new List<string> { "NurseId", "PathologyId", "TreatmentId", "TreatmentDate", "DurationMinutes", "Remarks", "Result", "Status" };
            ViewData["NurseId"] = new SelectList(_context.Nurse, "Id", "Name", treatmentRecord.NurseId);
            ViewData["PathologyId"] = new SelectList(_context.Pathology, "Id", "Name", treatmentRecord.PathologyId);
            ViewData["TreatmentId"] = new SelectList(_context.TreatmentType, "Id", "Name", treatmentRecord.TreatmentId);
            return View("~/Views/Shared/Group1/Actions/Edit.cshtml", treatmentRecord);
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

            ViewData["Title"] = "Editar marcação de tratamento";
            ViewBag.ModelType = typeof(TreatmentRecord);
            ViewBag.Properties = new List<string> { "NurseId", "PathologyId", "TreatmentId", "TreatmentDate", "DurationMinutes", "Remarks", "Result", "Status" };
            ViewBag.NurseId = new SelectList(_context.Nurse, "Id", "Name");
            ViewBag.PathologyId = new SelectList(_context.Pathology, "Id", "Name");
            ViewBag.TreatmentId = new SelectList(_context.TreatmentType, "Id", "Name");
            return View("~/Views/Shared/Group1/Actions/Edit.cshtml", treatmentRecord);
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

            ViewData["Title"] = "Editar marcação de tratamento";

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
                TempData["AlertType"] = "success";
                TempData["IconClass"] = "bi bi-check-circle";
                TempData["Message"] = "Treatment record edited successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ModelType = typeof(TreatmentRecord);
            ViewBag.Properties = new List<string> { "NurseId", "PathologyId", "TreatmentId", "TreatmentDate", "DurationMinutes", "Remarks", "Result", "Status" };
            ViewData["NurseId"] = new SelectList(_context.Nurse, "Id", "Name", treatmentRecord.NurseId);
            ViewData["PathologyId"] = new SelectList(_context.Pathology, "Id", "Name", treatmentRecord.PathologyId);
            ViewData["TreatmentId"] = new SelectList(_context.TreatmentType, "Id", "Name", treatmentRecord.TreatmentId);
            return View("~/Views/Shared/Group1/Actions/Edit.cshtml", treatmentRecord);
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
