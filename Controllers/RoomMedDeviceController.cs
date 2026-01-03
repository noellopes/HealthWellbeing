using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeingRoom.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeingRoom.Controllers
{
    [Authorize(Roles = "logisticsTechnician,Administrator")]
    public class RoomMedDeviceController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public RoomMedDeviceController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: RoomMedDevice/Add?roomId=1
        [HttpGet]
        public async Task<IActionResult> Add(int roomId)
        {
            var room = await _context.Room.FindAsync(roomId);
            if (room == null) return NotFound();

            ViewBag.RoomId = roomId;
            ViewBag.RoomName = room.Name;

            ViewBag.MedicalDeviceID = new SelectList(
                await _context.MedicalDevices
                    .OrderBy(d => d.Name)
                    .ToListAsync(),
                "MedicalDeviceID",
                "Name"
            );

            return View(new LocationMedDevice { RoomId = roomId });
        }

        // POST: RoomMedDevice/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(LocationMedDevice model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.MedicalDeviceID = new SelectList(
                    await _context.MedicalDevices
                        .OrderBy(d => d.Name)
                        .ToListAsync(),
                    "MedicalDeviceID",
                    "Name",
                    model.MedicalDeviceID
                );
                ViewBag.RoomId = model.RoomId;
                ViewBag.RoomName = (await _context.Room.FindAsync(model.RoomId))?.Name;
                return View(model);
            }

            // Fecha localização anterior do dispositivo (se existir)
            var anterior = await _context.LocationMedDevice
                .FirstOrDefaultAsync(l => l.MedicalDeviceID == model.MedicalDeviceID && l.IsCurrent);

            if (anterior != null)
            {
                anterior.IsCurrent = false;
                anterior.EndDate = DateTime.Now;
                _context.LocationMedDevice.Update(anterior);
            }

            // Nova localização
            model.InitialDate = DateTime.Now;
            model.EndDate = null;
            model.IsCurrent = true;
            _context.LocationMedDevice.Add(model);

            await _context.SaveChangesAsync();

            var room = await _context.Room.FindAsync(model.RoomId);
            TempData["SuccessMessage"] = $"Dispositivo adicionado à sala '{room?.Name ?? model.RoomId.ToString()}' com sucesso.";

            return RedirectToAction("RoomMaterials", "RoomReservations", new { id = model.RoomId });
        }
    }
}
