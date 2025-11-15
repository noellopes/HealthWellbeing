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


namespace HealthWellbeingRoom.Controllers
{
    // Controlador responsável pela gestão das salas
    public class RoomsController : Controller
    {
        // Injeção do contexto da base de dados
        private readonly HealthWellbeingDbContext _context;

        public RoomsController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Rooms
        // Lista paginada das salas existentes
        public async Task<IActionResult> Index(string searchName, string searchStatus, string searchSpecialty, string searchLocation, int page = 1)
        {
            // Traz todos os registos para memória para permitir filtragem com métodos C#
            var allRooms = await _context.Room.ToListAsync();

            // Filtro por nome (insensível a acentos e capitalização)
            if (!string.IsNullOrEmpty(searchName))
            {
                var normalizedSearch = RemoveDiacritics.Normalize(searchName);
                allRooms = allRooms
                    .Where(r => RemoveDiacritics.Normalize(r.Name).Contains(normalizedSearch))
                    .ToList();
            }

            // Filtro por estado (aplicado apenas se o utilizador escolher um valor)
            if (!string.IsNullOrEmpty(searchStatus) && Enum.TryParse<Room.RoomStatus>(searchStatus, out var status))
            {
                allRooms = allRooms
                    .Where(r => r.Status == status)
                    .ToList();
            }

            // Filtro por especialidade
            if (!string.IsNullOrEmpty(searchSpecialty))
            {
                var normalizedSpecialty = RemoveDiacritics.Normalize(searchSpecialty);
                allRooms = allRooms
                    .Where(r => RemoveDiacritics.Normalize(r.Specialty).Contains(normalizedSpecialty))
                    .ToList();
            }

            // Filtro por localização
            if (!string.IsNullOrEmpty(searchLocation))
            {
                var normalizedLocation = RemoveDiacritics.Normalize(searchLocation);
                allRooms = allRooms
                    .Where(r => RemoveDiacritics.Normalize(r.Location).Contains(normalizedLocation))
                    .ToList();
            }

            // Total de registos após filtros
            var totalItems = allRooms.Count;
            var pagination = new RPaginationInfo<Room>(page, totalItems);

            // Dados paginados
            pagination.Items = allRooms
                .OrderBy(r => r.Name)
                .Skip(pagination.ItemsToSkip)
                .Take(pagination.ItemsPerPage)
                .ToList();

            // Persistência dos filtros na view
            ViewBag.SearchName = searchName;
            ViewBag.SearchStatus = searchStatus;

            // Lista de estados com nomes amigáveis
            ViewBag.Status = new SelectList(
                Enum.GetValues(typeof(Room.RoomStatus))
                    .Cast<Room.RoomStatus>()
                    .Select(s => new { Value = s.ToString(), Text = GetDisplayName(s) }),
                "Value", "Text", searchStatus
            );

            return View(pagination);
        }

        // Função auxiliar para extrair o nome amigável do enum
        private string GetDisplayName(Enum value)
        {
            var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
            var attr = member != null
                ? Attribute.GetCustomAttribute(member, typeof(DisplayAttribute)) as DisplayAttribute
                : null;
            return attr?.Name ?? value.ToString();
        }


        // GET: Rooms/Details/5
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

        // GET: Rooms/Create
        public IActionResult Create()
        {
            ViewBag.RoomsTypeList = new SelectList(new List<string> { "Consulta", "Tratamentos" });
            return View();
        }

        // POST: Rooms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RoomId,RoomsType,Specialty,Name,Capacity,Location,OperatingHours,Status,Notes")] Room room)
        {
            if (ModelState.IsValid)
            {
                _context.Add(room);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.RoomsTypeList = new SelectList(Enum.GetValues(typeof(RoomType)));
            return View(room);
        }

        // GET: Rooms/Edit/5
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

        // POST: Rooms/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RoomId,RoomsType,Specialty,Name,Capacity,Location,OperatingHours,Status,Notes")] Room room)
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

        // GET: Rooms/Delete/5
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

        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Room.FindAsync(id);
            if (room != null)
            {
                _context.Room.Remove(room);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoomExists(int id)
        {
            return _context.Room.Any(e => e.RoomId == id);
        }
    }
}