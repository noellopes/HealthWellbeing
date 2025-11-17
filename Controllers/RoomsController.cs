using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static HealthWellbeing.Models.Room;
using HealthWellbeingRoom.ViewModels;

// ... (usings mantidos)

namespace HealthWellbeingRoom.Controllers
{
    public class RoomsController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public RoomsController(HealthWellbeingDbContext context)
        {
            _context = context;
        }





        public async Task<IActionResult> Index(string searchName, string searchStatus, string searchSpecialty, string searchLocation, string searchOperatingHours, int page = 1)
        {
            var allRooms = await _context.Room.ToListAsync();

            if (!string.IsNullOrEmpty(searchName))
            {
                var normalizedSearch = RemoveDiacritics.Normalize(searchName);
                allRooms = allRooms
                    .Where(r => RemoveDiacritics.Normalize(r.Name).Contains(normalizedSearch))
                    .ToList();
            }

            if (!string.IsNullOrEmpty(searchStatus) && Enum.TryParse<Room.RoomStatus>(searchStatus, out var status))
            {
                allRooms = allRooms
                    .Where(r => r.Status == status)
                    .ToList();
            }

            if (!string.IsNullOrEmpty(searchSpecialty))
            {
                var normalizedSpecialty = RemoveDiacritics.Normalize(searchSpecialty);
                allRooms = allRooms
                    .Where(r => RemoveDiacritics.Normalize(r.Specialty).Contains(normalizedSpecialty))
                    .ToList();
            }

            if (!string.IsNullOrEmpty(searchLocation))
            {
                var normalizedLocation = RemoveDiacritics.Normalize(searchLocation);
                allRooms = allRooms
                    .Where(r => RemoveDiacritics.Normalize(r.Location).Contains(normalizedLocation))
                    .ToList();
            }

            if (!string.IsNullOrEmpty(searchOperatingHours))
            {
                var normalizedOperatingHours = RemoveDiacritics.Normalize(searchOperatingHours);
                allRooms = allRooms
                    .Where(r => RemoveDiacritics.Normalize(r.OperatingHours).Contains(searchOperatingHours))
                    .ToList();
            }

            var totalItems = allRooms.Count;
            var pagination = new RPaginationInfo<Room>(page, totalItems);

            pagination.Items = allRooms
                .OrderBy(r => r.Name)
                .Skip(pagination.ItemsToSkip)
                .Take(pagination.ItemsPerPage)
                .ToList();

            ViewBag.SearchName = searchName;
            ViewBag.SearchStatus = searchStatus;

            ViewBag.Status = new SelectList(
                Enum.GetValues(typeof(Room.RoomStatus))
                    .Cast<Room.RoomStatus>()
                    .Select(s => new { Value = s.ToString(), Text = GetDisplayName(s) }),
                "Value", "Text", searchStatus
            );

            return View(pagination);
        }




        private string GetDisplayName(Enum value)
        {
            var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
            var attr = member != null
                ? Attribute.GetCustomAttribute(member, typeof(DisplayAttribute)) as DisplayAttribute
                : null;
            return attr?.Name ?? value.ToString();
        }





        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Room
                .FirstOrDefaultAsync(m => m.RoomId == id);

            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }





        public IActionResult Create()
        {
            return View();
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RoomId,Specialty,Name,Location,OperatingHours,Status,Notes")] Room room)
        {
            if (ModelState.IsValid)
            {
                // Verifica se já existe uma sala com o mesmo nome (ignorando maiúsculas/minúsculas e espaços)
                var nomeNormalizado = room.Name.Trim().ToLower();
                var salaExistente = await _context.Room
                    .AnyAsync(r => r.Name.Trim().ToLower() == nomeNormalizado);

                if (salaExistente)
                {
                    ModelState.AddModelError("Name", "Já existe uma sala com este nome, por favor insira outro nome.");
                    return View(room);
                }

                _context.Add(room);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Sala criada com sucesso!";
                TempData["FromRoomCreation"] = true;

                return RedirectToAction("Details", new { id = room.RoomId, fromCreation = true });
            }


            return View(room);
        }



        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Room.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }
            return View(room);
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RoomId,Specialty,Name,Location,OperatingHours,Status,Notes")] Room room)
        {
            if (id != room.RoomId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(room);
                    await _context.SaveChangesAsync();

                    //Notificação de atualizacao com sucesso
                    TempData["SuccessMessageEdit"] = $"Sala \"{room.Name}\" atualizada com sucesso!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomExists(room.RoomId))
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

            return View(room);
        }





        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Room
                .FirstOrDefaultAsync(m => m.RoomId == id);
            if (room == null)
            {
                return NotFound();
            }

            return View(room);
        }




        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Room
                .Include(r => r.Equipments)
                .Include(r => r.LocalizacaoDispMedicoMovel) // include para dispositivos medicos moveis
                .FirstOrDefaultAsync(r => r.RoomId == id);

            //verificar se a sala existe
            if (room == null)
            {
                TempData["ErrorMessageDelete"] = "Sala não encontrada.";
                return View("Delete"); // ou return NotFound();
            }

            //verificar se tem equipamentos associados
            if (room.Equipments.Any())
            {
                TempData["ErrorMessageDelete"] = $"A sala \"{room.Name}\" não pode ser eliminada porque possui equipamentos.";
                return View("Delete", room); //permanece na mesma página
            }

            //verificar se tem dispositivos moveis associados
            if (room.LocalizacaoDispMedicoMovel.Any())
            {
                TempData["errorMessageDeleteDispositivo"] = $"A sala \"{room.Name}\" não pode ser eliminada porque possui dispositivos medicos.";
                return View("Delete", room); //permanece na mesma página
            }

            _context.Room.Remove(room);
            await _context.SaveChangesAsync();
            TempData["SuccessMessageDelete"] = $"Sala \"{room.Name}\" eliminada com sucesso!";
            return RedirectToAction(nameof(Index));
        }





        private bool RoomExists(int id)
        {
            return _context.Room.Any(e => e.RoomId == id);
        }
    }
}