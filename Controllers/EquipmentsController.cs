using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using HealthWellbeingRoom.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;

namespace HealthWellbeingRoom.Controllers
{
    public class EquipmentsController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public EquipmentsController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Equipments
        public async Task<IActionResult> Index(int page = 1, string searchName = "", int searchType = -1, int searchStatus = -1, int searchRoom = -1)
        {
            // Construir a consulta inicial incluindo as entidades relacionadas
            var equipmentQuery = _context.Equipment
                .Include(r => r.Room)
                .Include(et => et.EquipmentType)
                .Include(es => es.EquipmentStatus)
                .AsQueryable();

            // Validar se os parâmetros de pesquisa não estão vazios e aplicar os filtros correspondentes
            if (!string.IsNullOrEmpty(searchName))
            {
                equipmentQuery = equipmentQuery.Where(e => e.Name.Contains(searchName));
            }

            if (searchType > 0)
            {
                equipmentQuery = equipmentQuery.Where(e => e.EquipmentTypeId == searchType);
            }

            if (searchStatus > 0)
            {
                equipmentQuery = equipmentQuery.Where(e => e.EquipmentStatusId == searchStatus);
            }

            if (searchRoom > 0)
            {
                equipmentQuery = equipmentQuery.Where(e => e.RoomId == searchRoom);
            }

            // Manter os valores de pesquisa na ViewBag para uso na View
            ViewBag.SearchName = searchName;
            ViewBag.SearchType = searchType;
            ViewBag.SearchStatus = searchStatus;
            ViewBag.SearchRoom = searchRoom;

            // Buscar dados para os filtros ordenados alfabeticamente
            var rooms = await _context.Set<Room>().OrderBy(e => e.Name).ToListAsync();
            var types = await _context.Set<EquipmentType>().OrderBy(e => e.Name).ToListAsync();
            var status = await _context.Set<EquipmentStatus>().OrderBy(e => e.Name).ToListAsync();

            // Atribuir 1ª opção "Todos" aos filtros
            rooms.Insert(0, new Room { RoomId = -1, Name = "-- Todas Salas --" });
            types.Insert(0, new EquipmentType { EquipmentTypeId = -1, Name = "-- Todos Tipos --" });
            status.Insert(0, new EquipmentStatus { EquipmentStatusId = -1, Name = "-- Todos Estados --" });


            // Salvar os filtros na ViewBag
            ViewBag.Rooms = new SelectList(rooms, "RoomId", "Name", searchRoom);
            ViewBag.Types = new SelectList(types, "EquipmentTypeId", "Name", searchType);
            ViewBag.Status = new SelectList(status, "EquipmentStatusId", "Name", searchStatus);


            // Implementar paginação
            var equipmentInfo = new RPaginationInfo<Equipment>(page, await equipmentQuery.CountAsync());
            equipmentInfo.Items = await equipmentQuery
                .OrderBy(e => e.Name)
                .Skip(equipmentInfo.ItemsToSkip)
                .Take(equipmentInfo.ItemsPerPage)
                .ToListAsync();

            return View(equipmentInfo);
        }

        // GET: Equipments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                /* Retornar para minha page do not found */
                return View("NotFound");
            }

            var equipment = await _context.Equipment
                .Include(r => r.Room)
                .Include(et => et.EquipmentType)
                .Include(es => es.EquipmentStatus)
                .FirstOrDefaultAsync(m => m.EquipmentId == id);
            if (equipment == null)
            {
                /* Retornar para minha page do not found */
                return View("NotFound");
            }

            return View(equipment);
        }

        // GET: Equipments/Create
        public IActionResult Create()
        {
            ViewData["RoomId"] = new SelectList(_context.Set<Room>(), "RoomId", "Name");
            ViewData["EquipmentTypeId"] = new SelectList(_context.Set<EquipmentType>(), "EquipmentTypeId", "Name");
            ViewData["EquipmentStatusId"] = new SelectList(_context.Set<EquipmentStatus>(), "EquipmentStatusId", "Name");
            return View();
        }

        // POST: Equipments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EquipmentId,Name,Description,SerialNumber,RoomId,PurchaseDate,EquipmentTypeId,EquipmentStatusId")] Equipment equipment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(equipment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details),
                    new
                    {
                        id = equipment.EquipmentId,
                        SuccessMessage = "Equipamento criado com sucesso."
                    }
                );
            }
            ViewData["RoomId"] = new SelectList(_context.Set<Room>(), "RoomId", "Name");
            ViewData["EquipmentTypeId"] = new SelectList(_context.Set<EquipmentType>(), "EquipmentTypeId", "Name");
            ViewData["EquipmentStatusId"] = new SelectList(_context.Set<EquipmentStatus>(), "EquipmentStatusId", "Name");
            return View(equipment);
        }

        // GET: Equipments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                /* Retornar para minha page do not found */
                return View("NotFound");
            }

            var equipment = await _context.Equipment.FindAsync(id);
            if (equipment == null)
            {
                /* Retornar para minha page do not found */
                return View("NotFound");
            }
            ViewData["RoomId"] = new SelectList(_context.Set<Room>(), "RoomId", "Name");
            ViewData["EquipmentTypeId"] = new SelectList(_context.Set<EquipmentType>(), "EquipmentTypeId", "Name");
            ViewData["EquipmentStatusId"] = new SelectList(_context.Set<EquipmentStatus>(), "EquipmentStatusId", "Name");
            return View(equipment);
        }

        // POST: Equipments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EquipmentId,Name,Description,SerialNumber,RoomId,PurchaseDate,EquipmentTypeId,EquipmentStatusId")] Equipment equipment)
        {
            if (id != equipment.EquipmentId)
            {
                /* Retornar para minha page do not found */
                return View("NotFound");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(equipment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EquipmentExists(equipment.EquipmentId))
                    {
                        /* Retornar para minha page do not found */
                        return View("NotFound");
                    }
                    else
                    {
                        throw;
                    }
                }

                // Redirecionar para a página de detalhes com mensagem de sucesso
                return RedirectToAction(nameof(Details),
                    new
                    {
                        id = equipment.EquipmentId,
                        SuccessMessage = "Equipamento editado com sucesso."
                    }
                );
            }
            ViewData["RoomId"] = new SelectList(_context.Set<Room>(), "RoomId", "Name");
            ViewData["EquipmentTypeId"] = new SelectList(_context.Set<EquipmentType>(), "EquipmentTypeId", "Name");
            ViewData["EquipmentStatusId"] = new SelectList(_context.Set<EquipmentStatus>(), "EquipmentStatusId", "Name");
            return View(equipment);
        }

        // GET: Equipments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                /* Retornar para minha page do not found */
                return View("NotFound");
            }

            var equipment = await _context.Equipment
                .Include(r => r.Room)
                .Include(et => et.EquipmentType)
                .Include(es => es.EquipmentStatus)
                .FirstOrDefaultAsync(m => m.EquipmentId == id);
            if (equipment == null)
            {
                /* Retornar para minha page do not found */
                return View("NotFound");
            }

            return View(equipment);
        }

        // POST: Equipments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var equipment = await _context.Equipment.FindAsync(id);
            if (equipment != null)
            {
                _context.Equipment.Remove(equipment);
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "Equipamento eliminado com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        private bool EquipmentExists(int id)
        {
            return _context.Equipment.Any(e => e.EquipmentId == id);
        }
    }
}
