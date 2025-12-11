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
    public class CrisisAlertsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CrisisAlertsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CrisisAlerts
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.CrisisAlerts.Include(c => c.Patient);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: CrisisAlerts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var crisisAlert = await _context.CrisisAlerts
                .Include(c => c.Patient)
                .FirstOrDefaultAsync(m => m.AlertId == id);
            if (crisisAlert == null)
            {
                return NotFound();
            }

            return View(crisisAlert);
        }

        // GET: CrisisAlerts/Create
        public IActionResult Create()
        {
            // Prepare patient dropdown with FullName
            var patients = _context.Patients
                .Where(p => p.IsActive)
                .Select(p => new {
                    p.PatientId,
                    FullName = p.FirstName + " " + p.LastName
                })
                .OrderBy(p => p.FullName)
                .ToList();

            ViewData["PatientId"] = new SelectList(patients, "PatientId", "FullName");

            return View();
        }

        // POST: CrisisAlerts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AlertId,PatientId,AlertDateTime,Level,Description,IsResolved,ResolvedDateTime,Resolution")] CrisisAlert crisisAlert)
        {
            if (ModelState.IsValid)
            {
                _context.Add(crisisAlert);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Re-populate dropdown if validation fails
            var patients = _context.Patients
                .Where(p => p.IsActive)
                .Select(p => new {
                    p.PatientId,
                    FullName = p.FirstName + " " + p.LastName
                })
                .OrderBy(p => p.FullName)
                .ToList();

            ViewData["PatientId"] = new SelectList(patients, "PatientId", "FullName", crisisAlert.PatientId);

            return View(crisisAlert);
        }

        // GET: CrisisAlerts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var crisisAlert = await _context.CrisisAlerts.FindAsync(id);
            if (crisisAlert == null)
            {
                return NotFound();
            }

            // Prepare patient dropdown with FullName
            var patients = _context.Patients
                .Where(p => p.IsActive)
                .Select(p => new {
                    p.PatientId,
                    FullName = p.FirstName + " " + p.LastName
                })
                .OrderBy(p => p.FullName)
                .ToList();

            ViewData["PatientId"] = new SelectList(patients, "PatientId", "FullName", crisisAlert.PatientId);

            return View(crisisAlert);
        }

        // POST: CrisisAlerts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AlertId,PatientId,AlertDateTime,Level,Description,IsResolved,ResolvedDateTime,Resolution")] CrisisAlert crisisAlert)
        {
            if (id != crisisAlert.AlertId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(crisisAlert);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CrisisAlertExists(crisisAlert.AlertId))
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

            // Re-populate dropdown if validation fails
            var patients = _context.Patients
                .Where(p => p.IsActive)
                .Select(p => new {
                    p.PatientId,
                    FullName = p.FirstName + " " + p.LastName
                })
                .OrderBy(p => p.FullName)
                .ToList();

            ViewData["PatientId"] = new SelectList(patients, "PatientId", "FullName", crisisAlert.PatientId);

            return View(crisisAlert);
        }

        // GET: CrisisAlerts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var crisisAlert = await _context.CrisisAlerts
                .Include(c => c.Patient)
                .FirstOrDefaultAsync(m => m.AlertId == id);
            if (crisisAlert == null)
            {
                return NotFound();
            }

            return View(crisisAlert);
        }

        // POST: CrisisAlerts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var crisisAlert = await _context.CrisisAlerts.FindAsync(id);
            if (crisisAlert != null)
            {
                _context.CrisisAlerts.Remove(crisisAlert);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CrisisAlertExists(int id)
        {
            return _context.CrisisAlerts.Any(e => e.AlertId == id);
        }
    }
}