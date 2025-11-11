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
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Email");
            ViewData["ProfessionalId"] = new SelectList(_context.MentalHealthProfessionals, "ProfessionalId", "Email");
            return View();
        }

        // POST: TherapySessions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Email", therapySession.PatientId);
            ViewData["ProfessionalId"] = new SelectList(_context.MentalHealthProfessionals, "ProfessionalId", "Email", therapySession.ProfessionalId);
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
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Email", therapySession.PatientId);
            ViewData["ProfessionalId"] = new SelectList(_context.MentalHealthProfessionals, "ProfessionalId", "Email", therapySession.ProfessionalId);
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
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "Email", therapySession.PatientId);
            ViewData["ProfessionalId"] = new SelectList(_context.MentalHealthProfessionals, "ProfessionalId", "Email", therapySession.ProfessionalId);
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
