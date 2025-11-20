using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using HealthWellbeingRoom.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeingRoom.Controllers
{
    public class RoomsController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public RoomsController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        private async Task PreencherDropdowns()
        {
            ViewBag.RoomTypes = new SelectList(await _context.RoomType.ToListAsync(), "RoomTypeId", "Name");
            ViewBag.RoomLocations = new SelectList(await _context.RoomLocation.ToListAsync(), "RoomLocationId", "Name");
            ViewBag.RoomStatuses = new SelectList(await _context.RoomStatus.ToListAsync(), "RoomStatusId", "Name");
        }

        public async Task<IActionResult> Index(string? searchName, string? searchStatus, string? searchSpecialty, string? searchLocation, string? searchOpeningTime, string? searchClosingTime, int page = 1)
        {
            var allRooms = await _context.Room
                .Include(r => r.RoomStatus)
                .Include(r => r.RoomLocation)
                .Include(r => r.RoomType)
                .ToListAsync();

            if (!string.IsNullOrWhiteSpace(searchName))
            {
                var normalizedSearch = RemoveDiacritics.Normalize(searchName ?? string.Empty);
                allRooms = allRooms.Where(r => RemoveDiacritics.Normalize(r.Name ?? "").Contains(normalizedSearch)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(searchStatus) && int.TryParse(searchStatus, out var statusId))
                allRooms = allRooms.Where(r => r.RoomStatusId == statusId).ToList();

            if (!string.IsNullOrWhiteSpace(searchSpecialty))
            {
                var normalizedSpecialty = RemoveDiacritics.Normalize(searchSpecialty ?? string.Empty);
                allRooms = allRooms.Where(r => RemoveDiacritics.Normalize(r.Specialty ?? "").Contains(normalizedSpecialty)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(searchLocation))
            {
                var normalizedLocation = RemoveDiacritics.Normalize(searchLocation ?? string.Empty);
                allRooms = allRooms.Where(r => r.RoomLocation != null &&
                                               RemoveDiacritics.Normalize(r.RoomLocation.Name ?? "").Contains(normalizedLocation)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(searchOpeningTime) && TimeSpan.TryParse(searchOpeningTime, out var openingTime))
                allRooms = allRooms.Where(r => r.OpeningTime == openingTime).ToList();

            if (!string.IsNullOrWhiteSpace(searchClosingTime) && TimeSpan.TryParse(searchClosingTime, out var closingTime))
                allRooms = allRooms.Where(r => r.ClosingTime == closingTime).ToList();

            var totalItems = allRooms.Count;
            var pagination = new RPaginationInfo<Room>(page, totalItems);
            pagination.Items = allRooms.OrderBy(r => r.Name ?? "").Skip(pagination.ItemsToSkip).Take(pagination.ItemsPerPage).ToList();

            ViewBag.SearchName = searchName;
            ViewBag.SearchStatus = searchStatus;
            ViewBag.SearchSpecialty = searchSpecialty;
            ViewBag.SearchLocation = searchLocation;
            ViewBag.SearchOpeningTime = searchOpeningTime;
            ViewBag.SearchClosingTime = searchClosingTime;

            ViewBag.Status = new SelectList(
                _context.RoomStatus.Select(s => new { Value = s.RoomStatusId.ToString(), Text = s.Name }),
                "Value", "Text", searchStatus);

            return View(pagination);
        }

        public async Task<IActionResult> Details(int? id, bool fromCreation = false)
        {
            if (id == null) return NotFound();

            var room = await _context.Room
                .Include(r => r.RoomStatus)
                .Include(r => r.RoomLocation)
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(m => m.RoomId == id);

            if (room == null) return NotFound();

            ViewBag.FromCreation = fromCreation;
            return View(room);
        }

        public async Task<IActionResult> Create()
        {
            await PreencherDropdowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Room room)
        {
            if (room == null) return NotFound();

            Console.WriteLine("Valor recebido para Name: " + room.Name);

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    Console.WriteLine(error.ErrorMessage);

                await PreencherDropdowns();
                return View(room);
            }

            var nomeNormalizado = room.Name?.Trim().ToLower() ?? string.Empty;
            var salaExistente = await _context.Room.AnyAsync(r => (r.Name ?? "").Trim().ToLower() == nomeNormalizado);

            if (salaExistente)
            {
                ModelState.AddModelError("Name", "Já existe uma sala com este nome, por favor insira outro nome.");
                await PreencherDropdowns();
                return View(room);
            }

            _context.Add(room);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Sala criada com sucesso!";
            return RedirectToAction("Details", new { id = room.RoomId, fromCreation = true });
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var room = await _context.Room.FindAsync(id);
            if (room == null) return NotFound();

            await PreencherDropdowns();
            return View(room);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Room room)
        {
            if (id != room.RoomId) return NotFound();

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    Console.WriteLine(error.ErrorMessage);

                await PreencherDropdowns();
                return View(room);
            }

            //try
            //{
            //    _context.Update(room);
            //    await _context.SaveChangesAsync();
            //    TempData["SuccessMessageEdit"] = $"Sala \"{room.Name ?? "(sem nome)"}\" atualizada com sucesso!";
            //}

            try
            {
                var roomInDb = await _context.Room.FindAsync(room.RoomId);
                if (roomInDb == null) return NotFound();

                // Atualiza apenas os campos editáveis
                roomInDb.Name = room.Name;
                roomInDb.Specialty = room.Specialty;
                roomInDb.Notes = room.Notes;
                roomInDb.OpeningTime = room.OpeningTime;
                roomInDb.ClosingTime = room.ClosingTime;
                roomInDb.RoomTypeId = room.RoomTypeId;
                roomInDb.RoomLocationId = room.RoomLocationId;
                roomInDb.RoomStatusId = room.RoomStatusId;

                await _context.SaveChangesAsync();
                TempData["SuccessMessageEdit"] = $"Sala \"{room.Name ?? "(sem nome)"}\" atualizada com sucesso!";
            }

            catch (DbUpdateConcurrencyException)
            {
                if (!RoomExists(room.RoomId)) return NotFound();
                else throw;
            }

            return RedirectToAction(nameof(Index));
        }



        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var room = await _context.Room
                .Include(r => r.RoomStatus)
                .Include(r => r.RoomLocation)
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(m => m.RoomId == id);

            if (room == null) return NotFound();

            return View(room);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Room
                .Include(r => r.Equipments)
                .Include(r => r.LocalizacaoDispMedicoMovel)
                .FirstOrDefaultAsync(r => r.RoomId == id);

            if (room == null)
            {
                TempData["ErrorMessageDelete"] = "Sala não encontrada.";
                return View("Delete");
            }

            if (room.Equipments?.Any() == true)
            {
                TempData["ErrorMessageDelete"] = $"A sala \"{room.Name ?? "(sem nome)"}\" não pode ser eliminada porque possui equipamentos.";
                return View("Delete", room);
            }

            if (room.LocalizacaoDispMedicoMovel?.Any() == true)
            {
                TempData["errorMessageDeleteDispositivo"] = $"A sala \"{room.Name ?? "(sem nome)"}\" não pode ser eliminada porque possui dispositivos médicos.";
                return View("Delete", room);
            }

            _context.Room.Remove(room);
            await _context.SaveChangesAsync();
            TempData["SuccessMessageDelete"] = $"Sala \"{room.Name ?? "(sem nome)"}\" eliminada com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        private bool RoomExists(int id)
        {
            return _context.Room.Any(e => e.RoomId == id);
        }
    }
}