using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeingRoom.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
        //------------------------------------------------------PREENCHER DROPDOWNS-------------------------------------------------------------------------------------
        private async Task PreencherDropdowns(
            int? selectedRoomId = null,
            int? selectedSpecialityId = null,
            DateTime? start = null,
            DateTime? end = null,
            int? excludeReservationId = null
        ){
            var startRaw = Request.Form["StartTime"].FirstOrDefault() ?? Request.Query["start"].FirstOrDefault();
            var endRaw = Request.Form["EndTime"].FirstOrDefault() ?? Request.Query["end"].FirstOrDefault();
            var excludeRaw = Request.Form["RoomReservationId"].FirstOrDefault() ?? Request.Query["excludeReservationId"].FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(startRaw) && DateTime.TryParse(startRaw, out var s))
                start = s;

            if (!string.IsNullOrWhiteSpace(endRaw) && DateTime.TryParse(endRaw, out var e))
                end = e;

            if (!string.IsNullOrWhiteSpace(excludeRaw) && int.TryParse(excludeRaw, out var ex))
                excludeReservationId = ex;

            // Query base de salas
            IQueryable<Room> roomsQuery = _context.Room.AsNoTracking();

            // Se houver intervalo válido, filtra apenas salas sem reservas que se sobreponham
            if (start.HasValue && end.HasValue)
            {
                var sVal = start.Value;
                var eVal = end.Value;

                roomsQuery = roomsQuery.Where(r =>
                    !_context.RoomReservations
                        .AsNoTracking()
                        .Where(rr => rr.RoomId == r.RoomId
                                     && (excludeReservationId == null || rr.RoomReservationId != excludeReservationId.Value))
                        .Any(rr => rr.StartTime < eVal && sVal < rr.EndTime)
                );
            }

            var rooms = await roomsQuery.OrderBy(r => r.Name).ToListAsync();
            ViewBag.Rooms = new SelectList(rooms, "RoomId", "Name");
        }



        // GET: RoomReservations
        public async Task<IActionResult> Index()
        {
            var reservations = _context.RoomReservations
                .Include(r => r.Room)
                .Include(r => r.Specialty);
            return View(await reservations.ToListAsync());
        }

        // GET: RoomReservations/Details/5
        public async Task<IActionResult> Details(int? id, int roomId)
        {
            // Carregar nome da sala
            var reservation = await _context.RoomReservations
                .Include(r => r.Room)
                .FirstOrDefaultAsync(r => r.RoomReservationId == id);

            if (reservation == null) return NotFound();
            ViewData["RoomName"] = reservation.Room?.Name ?? "—";

            if (id == null) return NotFound();

            // Carregar detalhes da reserva
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
            // Normalizar valores de data
            if (roomReservation.StartTime == default) ModelState.AddModelError(nameof(roomReservation.StartTime), "A data/hora de início é obrigatória.");
            if (roomReservation.EndTime == default) ModelState.AddModelError(nameof(roomReservation.EndTime), "A data/hora de fim é obrigatória.");

            // Início < Fim
            if (roomReservation.StartTime != default && roomReservation.EndTime != default && roomReservation.StartTime >= roomReservation.EndTime)
                ModelState.AddModelError(string.Empty, "A data/hora de início deve ser anterior à data/hora de fim.");

            // Sala obrigatória
            if (roomReservation.RoomId <= 0) ModelState.AddModelError(nameof(roomReservation.RoomId), "A sala é obrigatória.");

            // Validação de conflito (revalidação no servidor)
            if (roomReservation.RoomId > 0 && roomReservation.StartTime != default && roomReservation.EndTime != default)
            {
                var hasConflict = await _context.RoomReservations
                    .AsNoTracking()
                    .AnyAsync(r =>
                        r.RoomId == roomReservation.RoomId &&
                        r.StartTime < roomReservation.EndTime &&
                        roomReservation.StartTime < r.EndTime);

                if (hasConflict)
                {
                    ModelState.AddModelError(string.Empty, "Já existe uma reserva para esta sala no período selecionado.");
                }
            }

            if (!ModelState.IsValid)
            {
                // Repopula selects; opcionalmente o método PreencherDropdowns pode filtrar salas disponíveis
                await PreencherDropdowns(selectedRoomId: null, selectedSpecialityId: null, start: null, end: null, excludeReservationId: null);
                return View(roomReservation);
            }

            try
            {
                _context.Add(roomReservation);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Reserva criada com sucesso.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                // Possível condição de corrida ou restrição de BD
                ModelState.AddModelError(string.Empty, "Ocorreu um erro ao gravar a reserva. Verifique se a sala ainda está disponível e tente novamente.");
                await PreencherDropdowns(selectedRoomId: null, selectedSpecialityId: null, start: null, end: null, excludeReservationId: null);
                return View(roomReservation);
            }
        }

        // GET: RoomReservations/Edit/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var roomReservation = await _context.RoomReservations
                .Include(r => r.Room)
                .Include(r => r.Specialty)
                .FirstOrDefaultAsync(r => r.RoomReservationId == id);

            if (roomReservation == null) return NotFound();

            await PreencherDropdowns(roomReservation.RoomId, roomReservation.SpecialtyId);
            return View(roomReservation);
        }

        // POST: RoomReservations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id, [Bind("RoomReservationId,ResponsibleName,RoomId,ConsultationId,ConsultationType,PatientId,SpecialtyId,StartTime,EndTime,Status,Notes")] RoomReservation roomReservation)
        {
            if (id != roomReservation.RoomReservationId) return NotFound();

            // Validações básicas
            if (roomReservation.StartTime == default) ModelState.AddModelError(nameof(roomReservation.StartTime), "A data/hora de início é obrigatória.");
            if (roomReservation.EndTime == default) ModelState.AddModelError(nameof(roomReservation.EndTime), "A data/hora de fim é obrigatória.");
            if (roomReservation.StartTime != default && roomReservation.EndTime != default && roomReservation.StartTime >= roomReservation.EndTime)
                ModelState.AddModelError(string.Empty, "A data/hora de início deve ser anterior à data/hora de fim.");

            if (roomReservation.RoomId <= 0) ModelState.AddModelError(nameof(roomReservation.RoomId), "A sala é obrigatória.");

            // Verifica conflito de horários (exclui a própria reserva)
            if (roomReservation.RoomId > 0 && roomReservation.StartTime != default && roomReservation.EndTime != default)
            {
                var hasConflict = await _context.RoomReservations
                    .AsNoTracking()
                    .AnyAsync(r =>
                        r.RoomId == roomReservation.RoomId &&
                        r.RoomReservationId != roomReservation.RoomReservationId &&
                        r.StartTime < roomReservation.EndTime &&
                        roomReservation.StartTime < r.EndTime);

                if (hasConflict) ModelState.AddModelError(string.Empty, "Já existe uma reserva para esta sala no período selecionado.");
            }

            if (!ModelState.IsValid)
            {
                await PreencherDropdowns(roomReservation.RoomId, roomReservation.SpecialtyId);
                return View(roomReservation);
            }

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

        // GET: RoomReservations/Delete/5
        [Authorize(Roles = "Administrator")]
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
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var roomReservation = await _context.RoomReservations.FindAsync(id);
            if (roomReservation == null) return NotFound();

            try
            {
                _context.RoomReservations.Remove(roomReservation);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Reserva eliminada com sucesso.";
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Não foi possível eliminar a reserva. Verifique dependências.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            return RedirectToAction(nameof(Index));
        }

        // Preenche dropdowns usados em Create/Edit
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

            var roomsSelect = new SelectList(rooms, "RoomId", "Name", selectedRoomId);
            ViewData["RoomId"] = roomsSelect;
            ViewBag.Rooms = roomsSelect;

            var specialitySelect = new SelectList(specialities, "SpecialityId", "Name", selectedSpecialityId);
            ViewData["SpecialityId"] = specialitySelect;
            ViewData["SpecialtyId"] = specialitySelect;
            ViewBag.RoomSpecialty = specialitySelect;
        }

        private bool RoomReservationExists(int id)
        {
            return _context.RoomReservations.Any(e => e.RoomReservationId == id);
        }
    }
}