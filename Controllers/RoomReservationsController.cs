using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeingRoom.Models;

namespace HealthWellbeingRoom.Controllers
{
    public class RoomReservationsController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public RoomReservationsController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: RoomReservations
        public async Task<IActionResult> Index()
        {
            var healthWellbeingDbContext = _context.RoomReservations.Include(r => r.Room).Include(r => r.Specialty);
            return View(await healthWellbeingDbContext.ToListAsync());
        }

        // GET: RoomReservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roomReservation = await _context.RoomReservations
                .Include(r => r.Room)
                .Include(r => r.Specialty)
                .FirstOrDefaultAsync(m => m.RoomReservationId == id);
            if (roomReservation == null)
            {
                return NotFound();
            }

            return View(roomReservation);
        }

        // GET: RoomReservations/Create
        public IActionResult Create()
        {
            ViewData["RoomId"] = new SelectList(_context.Room, "RoomId", "Name");
            ViewData["SpecialtyId"] = new SelectList(_context.Specialty, "SpecialtyId", "SpecialtyId");
            return View();
        }

        // POST: RoomReservations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RoomReservationId,ResponsibleName,RoomId,ConsultationId,ConsultationType,PatientId,SpecialtyId,StartTime,EndTime,Status,Notes")] RoomReservation roomReservation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(roomReservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoomId"] = new SelectList(_context.Room, "RoomId", "Name", roomReservation.RoomId);
            ViewData["SpecialtyId"] = new SelectList(_context.Specialty, "SpecialtyId", "SpecialtyId", roomReservation.SpecialtyId);
            return View(roomReservation);
        }

        // GET: RoomReservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roomReservation = await _context.RoomReservations
                .Include(r => r.Room)        // 🔹 carrega a sala associada
                .Include(r => r.Specialty)   // 🔹 carrega a especialidade associada
                .FirstOrDefaultAsync(r => r.RoomReservationId == id);

            if (roomReservation == null)
            {
                return NotFound();
            }

            // Dropdown de salas mostrando o Nome
            ViewData["RoomId"] = new SelectList(_context.Room, "RoomId", "Name", roomReservation.RoomId);

            // Dropdown de especialidades mostrando o Nome
            ViewData["SpecialtyId"] = new SelectList(_context.Specialty, "SpecialtyId", "Name", roomReservation.SpecialtyId);

            return View(roomReservation);
        }

        // POST: RoomReservations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RoomReservationId,ResponsibleName,RoomId,ConsultationId,ConsultationType,PatientId,SpecialtyId,StartTime,EndTime,Status,Notes")] RoomReservation roomReservation)
        {
            if (id != roomReservation.RoomReservationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(roomReservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomReservationExists(roomReservation.RoomReservationId))
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
            ViewData["RoomId"] = new SelectList(_context.Room, "RoomId", "Name", roomReservation.RoomId);
            ViewData["SpecialtyId"] = new SelectList(_context.Specialty, "SpecialtyId", "SpecialtyId", roomReservation.SpecialtyId);
            return View(roomReservation);
        }

        // GET: RoomReservations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roomReservation = await _context.RoomReservations
                .Include(r => r.Room)
                .Include(r => r.Specialty)
                .FirstOrDefaultAsync(m => m.RoomReservationId == id);
            if (roomReservation == null)
            {
                return NotFound();
            }

            return View(roomReservation);
        }

        // POST: RoomReservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var roomReservation = await _context.RoomReservations.FindAsync(id);
            if (roomReservation != null)
            {
                _context.RoomReservations.Remove(roomReservation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoomReservationExists(int id)
        {
            return _context.RoomReservations.Any(e => e.RoomReservationId == id);
        }
    }
}
