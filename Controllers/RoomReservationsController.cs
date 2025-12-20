using HealthWellbeing.Data;
using HealthWellbeingRoom.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            var healthWellbeingDbContext = _context.RoomReservations
                .Include(r => r.Room)
                .Include(r => r.Specialty);
            return View(await healthWellbeingDbContext.ToListAsync());
        }

        // GET: RoomReservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var roomReservation = await _context.RoomReservations
                .Include(r => r.Room)
                .Include(r => r.Specialty)
                .FirstOrDefaultAsync(m => m.RoomReservationId == id);

            if (roomReservation == null) return NotFound();

            return View(roomReservation);
        }

        // GET: RoomReservations/Create
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create(int? roomId)
        {
            await PreencherDropdowns(roomId, null);
            var model = new RoomReservation();
            if (roomId.HasValue) model.RoomId = roomId.Value;
            return View(model);
        }

        // POST: RoomReservations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([Bind("RoomReservationId,ResponsibleName,RoomId,ConsultationId,ConsultationType,PatientId,SpecialtyId,StartTime,EndTime,Status,Notes")] RoomReservation roomReservation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(roomReservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Se houver erro, repopula os dropdowns antes de retornar a view
            await PreencherDropdowns(roomReservation.RoomId, roomReservation.SpecialtyId);
            return View(roomReservation);
        }

        // Método auxiliar para popular os SelectList usados nas views
        private async Task PreencherDropdowns(int? selectedRoomId = null, int? selectedSpecialityId = null)
        {
            var rooms = await _context.Room
                .AsNoTracking()
                .OrderBy(r => r.Name)
                .ToListAsync();

            var specialities = await _context.Specialty
                .AsNoTracking()
                .OrderBy(s => s.Name)
                .ToListAsync();

            // SelectList para salas (value = RoomId, text = Name)
            var roomsSelect = new SelectList(rooms, "RoomId", "Name", selectedRoomId);
            ViewData["RoomId"] = roomsSelect;
            ViewBag.Rooms = roomsSelect; // compatibilidade com a view que usa ViewBag.Rooms

            // SelectList para especialidades (value = SpecialityId, text = Name)
            var specialitySelect = new SelectList(specialities, "SpecialityId", "Name", selectedSpecialityId);
            ViewData["SpecialtyId"] = specialitySelect; // compatibilidade com variações de nome
            ViewBag.RoomSpecialty = specialitySelect;    // compatibilidade com a view que usa ViewBag.RoomSpecialty
        }

        // GET: RoomReservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var roomReservation = await _context.RoomReservations
                .Include(r => r.Room)
                .Include(r => r.Specialty)
                .FirstOrDefaultAsync(r => r.RoomReservationId == id);

            if (roomReservation == null) return NotFound();

            var rooms = await _context.Room.OrderBy(r => r.Name).ToListAsync();
            var specialities = await _context.Specialty.OrderBy(s => s.Name).ToListAsync();

            ViewData["RoomId"] = new SelectList(rooms, "RoomId", "Name", roomReservation.RoomId);
            var specialitySelect = new SelectList(specialities, "SpecialityId", "Name", roomReservation.SpecialtyId);
            ViewData["SpecialityId"] = specialitySelect;
            ViewData["SpecialtyId"] = specialitySelect;

            return View(roomReservation);
        }

        // POST: RoomReservations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RoomReservationId,ResponsibleName,RoomId,ConsultationId,ConsultationType,PatientId,SpecialtyId,StartTime,EndTime,Status,Notes")] RoomReservation roomReservation)
        {
            if (id != roomReservation.RoomReservationId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(roomReservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomReservationExists(roomReservation.RoomReservationId)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            // repopula selects em caso de erro
            var rooms = await _context.Room.OrderBy(r => r.Name).ToListAsync();
            var specialities = await _context.Specialty.OrderBy(s => s.Name).ToListAsync();

            ViewData["RoomId"] = new SelectList(rooms, "RoomId", "Name", roomReservation.RoomId);
            var specialitySelect = new SelectList(specialities, "SpecialityId", "Name", roomReservation.SpecialtyId);
            ViewData["SpecialityId"] = specialitySelect;
            ViewData["SpecialtyId"] = specialitySelect;

            return View(roomReservation);
        }

        // GET: RoomReservations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var roomReservation = await _context.RoomReservations
                .Include(r => r.Room)
                .Include(r => r.Specialty)
                .FirstOrDefaultAsync(m => m.RoomReservationId == id);

            if (roomReservation == null) return NotFound();

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
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool RoomReservationExists(int id)
        {
            return _context.RoomReservations.Any(e => e.RoomReservationId == id);
        }
    }
}