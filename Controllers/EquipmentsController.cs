using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeingRoom.Models;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;

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
        public async Task<IActionResult> Index(int page = 1)
        {
            var myContext = _context.Equipment
                .Include(r => r.Room)
                .Include(m => m.Manufacturer)
                .Include(et => et.EquipmentType)
                .Include(es => es.EquipmentStatus);

            var equipmentInfo = new RPaginationInfo<Equipment>(page, await myContext.CountAsync());
            equipmentInfo.Items = await myContext
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
                .Include(m => m.Manufacturer)
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
            ViewData["ManufacturerId"] = new SelectList(_context.Set<Manufacturer>(), "ManufacturerId", "Name");
            ViewData["EquipmentTypeId"] = new SelectList(_context.Set<EquipmentType>(), "EquipmentTypeId", "Name");
            ViewData["EquipmentStatusId"] = new SelectList(_context.Set<EquipmentStatus>(), "EquipmentStatusId", "Name");
            return View();
        }

        // POST: Equipments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EquipmentId,Name,Description,SerialNumber,RoomId,PurchaseDate,ManufacturerId,EquipmentTypeId,EquipmentStatusId")] Equipment equipment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(equipment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoomId"] = new SelectList(_context.Set<Room>(), "RoomId", "Name");
            ViewData["ManufacturerId"] = new SelectList(_context.Set<Manufacturer>(), "ManufacturerId", "Name");
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
            ViewData["ManufacturerId"] = new SelectList(_context.Set<Manufacturer>(), "ManufacturerId", "Name");
            ViewData["EquipmentTypeId"] = new SelectList(_context.Set<EquipmentType>(), "EquipmentTypeId", "Name");
            ViewData["EquipmentStatusId"] = new SelectList(_context.Set<EquipmentStatus>(), "EquipmentStatusId", "Name");
            return View(equipment);
        }

        // POST: Equipments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EquipmentId,Name,Description,SerialNumber,RoomId,PurchaseDate,ManufacturerId,EquipmentTypeId,EquipmentStatusId")] Equipment equipment)
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

                TempData["SuccessMessage"] = "Equipamento editado com sucesso.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoomId"] = new SelectList(_context.Set<Room>(), "RoomId", "Name");
            ViewData["ManufacturerId"] = new SelectList(_context.Set<Manufacturer>(), "ManufacturerId", "Name");
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
                .Include(m => m.Manufacturer)
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
