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
    public class TherapySessionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TherapySessionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TherapySessions
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.TherapySessions.Include(t => t.Patient).Include(t => t.Professional);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: TherapySessions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var therapySession = await _context.TherapySessions
                .Include(t => t.Patient)
                .Include(t => t.Professional)
                .FirstOrDefaultAsync(m => m.SessionId == id);
            if (therapySession == null)
            {
                return NotFound();
            }

            return View(therapySession);
        }

        // GET: TherapySessions/Create
        public IActionResult Create()
        {
            // Create Patient dropdown with FullName
            var patients = _context.Patients
                .Where(p => p.IsActive)
                .Select(p => new {
                    p.PatientId,
                    FullName = p.FirstName + " " + p.LastName
                })
                .OrderBy(p => p.FullName)
                .ToList();

            ViewData["PatientId"] = new SelectList(patients, "PatientId", "FullName");

            // Create Professional dropdown with FullName and Type
            var professionals = _context.MentalHealthProfessionals
                .Where(m => m.IsActive)
                .ToList()  // Bring to memory first
                .Select(m => new {
                    m.ProfessionalId,
                    FullName = $"{m.FirstName} {m.LastName} ({m.Type})"
                })
                .OrderBy(m => m.FullName)
                .ToList();

            ViewData["ProfessionalId"] = new SelectList(professionals, "ProfessionalId", "FullName");

            return View();
        }

        // POST: TherapySessions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SessionId,PatientId,ProfessionalId,ScheduledDateTime,DurationMinutes,Status,Type,Notes,CompletedDateTime")] TherapySession therapySession)
        {
            if (ModelState.IsValid)
            {
                _context.Add(therapySession);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Re-populate dropdowns if validation fails
            var patients = _context.Patients
                .Where(p => p.IsActive)
                .Select(p => new {
                    p.PatientId,
                    FullName = p.FirstName + " " + p.LastName
                })
                .OrderBy(p => p.FullName)
                .ToList();

            ViewData["PatientId"] = new SelectList(patients, "PatientId", "FullName", therapySession.PatientId);

            var professionals = _context.MentalHealthProfessionals
                .Where(m => m.IsActive)
                .ToList()  // Bring to memory first
                .Select(m => new {
                    m.ProfessionalId,
                    FullName = $"{m.FirstName} {m.LastName} ({m.Type})"
                })
                .OrderBy(m => m.FullName)
                .ToList();

            ViewData["ProfessionalId"] = new SelectList(professionals, "ProfessionalId", "FullName", therapySession.ProfessionalId);

            return View(therapySession);
        }

        // GET: TherapySessions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var therapySession = await _context.TherapySessions.FindAsync(id);
            if (therapySession == null)
            {
                return NotFound();
            }

            // Create Patient dropdown with FullName
            var patients = _context.Patients
                .Where(p => p.IsActive)
                .Select(p => new {
                    p.PatientId,
                    FullName = p.FirstName + " " + p.LastName
                })
                .OrderBy(p => p.FullName)
                .ToList();

            ViewData["PatientId"] = new SelectList(patients, "PatientId", "FullName", therapySession.PatientId);

            // Create Professional dropdown with FullName and Type
            var professionals = _context.MentalHealthProfessionals
                .Where(m => m.IsActive)
                .ToList()  // Bring to memory first
                .Select(m => new {
                    m.ProfessionalId,
                    FullName = $"{m.FirstName} {m.LastName} ({m.Type})"
                })
                .OrderBy(m => m.FullName)
                .ToList();

            ViewData["ProfessionalId"] = new SelectList(professionals, "ProfessionalId", "FullName", therapySession.ProfessionalId);

            return View(therapySession);
        }

        // POST: TherapySessions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SessionId,PatientId,ProfessionalId,ScheduledDateTime,DurationMinutes,Status,Type,Notes,CompletedDateTime")] TherapySession therapySession)
        {
            if (id != therapySession.SessionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(therapySession);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TherapySessionExists(therapySession.SessionId))
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

            // Re-populate dropdowns if validation fails
            // Patient dropdown with FullName
            var patients = _context.Patients
                .Where(p => p.IsActive)
                .Select(p => new {
                    p.PatientId,
                    FullName = p.FirstName + " " + p.LastName
                })
                .OrderBy(p => p.FullName)
                .ToList();

            ViewData["PatientId"] = new SelectList(patients, "PatientId", "FullName", therapySession.PatientId);

            // Professional dropdown with FullName and Type
            var professionals = _context.MentalHealthProfessionals
                .Where(m => m.IsActive)
                .ToList()  // Bring to memory first to properly convert enum
                .Select(m => new {
                    m.ProfessionalId,
                    FullName = $"{m.FirstName} {m.LastName} ({m.Type})"
                })
                .OrderBy(m => m.FullName)
                .ToList();

            ViewData["ProfessionalId"] = new SelectList(professionals, "ProfessionalId", "FullName", therapySession.ProfessionalId);

            return View(therapySession);
        }

        // GET: TherapySessions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var therapySession = await _context.TherapySessions
                .Include(t => t.Patient)
                .Include(t => t.Professional)
                .FirstOrDefaultAsync(m => m.SessionId == id);
            if (therapySession == null)
            {
                return NotFound();
            }

            return View(therapySession);
        }

        // POST: TherapySessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var therapySession = await _context.TherapySessions.FindAsync(id);
            if (therapySession != null)
            {
                _context.TherapySessions.Remove(therapySession);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TherapySessionExists(int id)
        {
            return _context.TherapySessions.Any(e => e.SessionId == id);
        }
    }
}
