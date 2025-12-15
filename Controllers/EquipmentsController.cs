using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using HealthWellbeingRoom.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;

namespace HealthWellbeingRoom.Controllers
{
    [Authorize(Roles = "logisticsTechnician,Administrator")]
    public class EquipmentsController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public EquipmentsController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Equipments
        public async Task<IActionResult> Index(int page = 1, string searchName = "", string searchSerie = "", int searchType = -1, int searchStatus = -1, int searchRoom = -1)
        {
            // Construir a consulta inicial incluindo as entidades relacionadas
            var equipmentQuery = _context.Equipment
                .Include(r => r.Room)
                .Include(et => et.EquipmentType)
                .Include(es => es.EquipmentStatus)
                .AsQueryable()
                .Where(e => e.EquipmentStatus.Name != "Excluído");

            // Validar se os parâmetros de pesquisa não estão vazios e aplicar os filtros correspondentes
            if (!string.IsNullOrEmpty(searchName))
            {
                equipmentQuery = equipmentQuery.Where(e => e.Name.Contains(searchName));
            }

            if (!string.IsNullOrEmpty(searchSerie))
            {
                equipmentQuery = equipmentQuery.Where(e => e.SerialNumber.Contains(searchSerie));
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
            ViewBag.SearchSerie = searchSerie;
            ViewBag.SearchType = searchType;
            ViewBag.SearchStatus = searchStatus;
            ViewBag.SearchRoom = searchRoom;

            // Buscar dados para os filtros ordenados alfabeticamente
            var rooms = await _context.Set<Room>().OrderBy(e => e.Name).ToListAsync();
            var types = await _context.Set<EquipmentType>().OrderBy(e => e.Name).ToListAsync();
            // Retornar os estados sem o Excluído
            var status = await _context.Set<EquipmentStatus>().Where(e => e.Name != "Excluído").OrderBy(e => e.Name).ToListAsync();

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
        [Authorize(Roles = "logisticsTechnician,Administrator")]
        public async Task<IActionResult> Details(int? id, int? roomId, string origem)
        {
            if (id == null)
            {
                // Retornar para a página personalizada de NotFound
                return View("NotFound");
            }

            var equipment = await _context.Equipment
                .Include(r => r.Room)
                .Include(et => et.EquipmentType)
                .Include(es => es.EquipmentStatus)
                .FirstOrDefaultAsync(m => m.EquipmentId == id);

            if (equipment == null)
            {
                return View("NotFound");
            }

            // Contexto adicional para navegação
            ViewBag.Origem = origem;
            ViewBag.RoomId = roomId ?? equipment.RoomId; // garante que tens sempre RoomId

            return View(equipment);
        }

        // GET: Equipments/Create
        public IActionResult Create()
        {
            ViewData["RoomId"] = new SelectList(_context.Set<Room>(), "RoomId", "Name");
            ViewData["EquipmentTypeId"] = new SelectList(_context.Set<EquipmentType>(), "EquipmentTypeId", "Name");
            ViewData["EquipmentStatusId"] = new SelectList(_context.Set<EquipmentStatus>().Where(e => e.Name != "Excluído"), "EquipmentStatusId", "Name");
            return View();
        }

        // POST: Equipments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EquipmentId,Name,Description,SerialNumber,RoomId,PurchaseDate,EquipmentTypeId,EquipmentStatusId")] Equipment equipment)
        {
            // Verificar se já existe um equipamento com o mesmo número de série e tipo de equipamento (evtando duplicados)
            bool exists = _context.Equipment.Any(e => e.SerialNumber == equipment.SerialNumber && e.EquipmentTypeId == equipment.EquipmentTypeId);

            if(exists)
            {
                ModelState.AddModelError("SerialNumber", "Já existe um equipamento com este número de série para o mesmo Tipo de Equipamento.");
            }

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
            ViewData["EquipmentStatusId"] = new SelectList(_context.Set<EquipmentStatus>().Where(e => e.Name != "Excluído"), "EquipmentStatusId", "Name");
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

            //var equipment = await _context.Equipment.FindAsync(id);
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

            // Não permitir editar Equipamentos excluidos
            if (equipment.EquipmentStatus.Name == "Excluído")
            {
                return RedirectToAction(nameof(Details),
                    new
                    {
                        id = equipment.EquipmentId,
                        SuccessMessage = "Não é possível editar esse equipamento porque foi excluído."
                    }
                );
            }

            ViewData["RoomId"] = new SelectList(_context.Set<Room>(), "RoomId", "Name");
            ViewData["EquipmentTypeId"] = new SelectList(_context.Set<EquipmentType>(), "EquipmentTypeId", "Name");
            ViewData["EquipmentStatusId"] = new SelectList(_context.Set<EquipmentStatus>().Where(e => e.Name != "Excluído"), "EquipmentStatusId", "Name");
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

            // Verificar se já existe um equipamento com o mesmo número de série e tipo de equipamento (evitando duplicados)
            bool exists = _context.Equipment.Any(e => e.SerialNumber == equipment.SerialNumber 
            && e.EquipmentTypeId == equipment.EquipmentTypeId && e.EquipmentId != equipment.EquipmentId);

            if (exists)
            {
                ModelState.AddModelError("SerialNumber", "Já existe um Equipamento com este número de série para o mesmo Tipo de Equipamento.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar se hoveram alterações nos dados
                    var existingEquipment = await _context.Equipment.AsNoTracking().FirstOrDefaultAsync(e => e.EquipmentId == equipment.EquipmentId);
                    if (existingEquipment == null)
                    {
                        /* Retornar para minha page do not found */
                        return View("NotFound");
                    }
                    if (existingEquipment.Name == equipment.Name &&
                        existingEquipment.Description == equipment.Description &&
                        existingEquipment.SerialNumber == equipment.SerialNumber &&
                        existingEquipment.RoomId == equipment.RoomId &&
                        existingEquipment.PurchaseDate == equipment.PurchaseDate &&
                        existingEquipment.EquipmentTypeId == equipment.EquipmentTypeId &&
                        existingEquipment.EquipmentStatusId == equipment.EquipmentStatusId)
                    {
                        // Nenhuma alteração foi feita, retornar com uma mensagem informativa
                        ModelState.AddModelError(string.Empty, "Nenhuma alteração foi feita nos dados do equipamento.");
                        ViewData["RoomId"] = new SelectList(_context.Set<Room>(), "RoomId", "Name", equipment.RoomId);
                        ViewData["EquipmentTypeId"] = new SelectList(_context.Set<EquipmentType>(), "EquipmentTypeId", "Name", equipment.EquipmentTypeId);
                        ViewData["EquipmentStatusId"] = new SelectList(_context.Set<EquipmentStatus>().Where(e => e.Name != "Excluído"), "EquipmentStatusId", "Name", equipment.EquipmentStatusId);
                        return View(equipment);
                    }

                    // Salvar as alteraçoes no equipamento
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
            ViewData["EquipmentStatusId"] = new SelectList(_context.Set<EquipmentStatus>().Where(e => e.Name != "Excluído"), "EquipmentStatusId", "Name");
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
                //_context.Equipment.Remove(equipment);


                // Alterar o estado para "Excluído"
                equipment.EquipmentStatusId = _context.EquipmentStatus
                    .Where(es => es.Name == "Excluído")
                    .Select(es => es.EquipmentStatusId)
                    .FirstOrDefault();
                _context.Update(equipment);
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
