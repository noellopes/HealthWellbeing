using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using HealthWellbeingRoom.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace HealthWellbeingRoom.Controllers
{
    public class LocationMedDeviceController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public LocationMedDeviceController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: LocationMedDevice
        public async Task<IActionResult> Index(
            int page = 1,
            string searchDevice = "",
            string searchRoom = "",
            string searchDate = "")
        {
            int itemsPerPage = 10;

            var query = _context.LocationMedDevice
                .Include(l => l.MedicalDevice)
                .Include(l => l.Room)
                .AsQueryable();

            // -------------------------
            //        FILTROS
            // -------------------------

            if (!string.IsNullOrEmpty(searchDevice))
            {
                query = query.Where(l => l.MedicalDevice.Name.Contains(searchDevice));
            }

            if (!string.IsNullOrEmpty(searchRoom))
            {
                query = query.Where(l => l.Room.Name.Contains(searchRoom));
            }

            if (!string.IsNullOrEmpty(searchDate))
            {
                if (DateTime.TryParse(searchDate, out DateTime parsedDate))
                {
                    query = query.Where(l => l.InitialDate.Date == parsedDate.Date);
                }
            }

            // Enviar valores para a View
            ViewBag.SearchDevice = searchDevice;
            ViewBag.SearchRoom = searchRoom;
            ViewBag.SearchDate = searchDate;

            // Total de itens
            int totalItems = await query.CountAsync();

            // Paginação
            var paginationInfo = new RPaginationInfo<LocationMedDevice>(page, totalItems, itemsPerPage);

            paginationInfo.Items = await query
                .OrderBy(l => l.LocationMedDeviceID)
                .Skip(paginationInfo.ItemsToSkip)
                .Take(paginationInfo.ItemsPerPage)
                .ToListAsync();

            return View(paginationInfo);
        }

        // GET: LocationMedDevice/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var location = await _context.LocationMedDevice
                .Include(l => l.MedicalDevice)
                .Include(l => l.Room)
                .FirstOrDefaultAsync(m => m.LocationMedDeviceID == id);

            if (location == null)
                return NotFound();

            return View(location);
        }

        // GET: LocationMedDevice/Create
        public IActionResult Create()
        {
            ViewBag.MedicalDeviceID = new SelectList(_context.MedicalDevices, "MedicalDeviceID", "Name");
            ViewBag.RoomId = new SelectList(_context.Room, "RoomId", "Name");
            return View(new LocationMedDevice());
        }

        // POST: LocationMedDevice/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MedicalDeviceID,RoomId,InitialDate,EndDate")] LocationMedDevice locationMedDevice)
        {
            if (ModelState.IsValid)
            {
                locationMedDevice.IsCurrent = true;

                var device = await _context.MedicalDevices
                    .Include(d => d.LocalizacaoDispMedicoMovel)
                    .FirstOrDefaultAsync(d => d.MedicalDeviceID == locationMedDevice.MedicalDeviceID);

                if (device != null)
                {
                    var currentLocations = device.LocalizacaoDispMedicoMovel
                        .Where(l => l.IsCurrent)
                        .ToList();

                    foreach (var loc in currentLocations)
                        loc.IsCurrent = false;
                }

                _context.Add(locationMedDevice);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.MedicalDeviceID = new SelectList(_context.MedicalDevices, "MedicalDeviceID", "Name", locationMedDevice.MedicalDeviceID);
            ViewBag.RoomId = new SelectList(_context.Room, "RoomId", "Name", locationMedDevice.RoomId);

            return View(locationMedDevice);
        }

        // GET: LocationMedDevice/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var location = await _context.LocationMedDevice.FindAsync(id);
            if (location == null)
                return NotFound();

            ViewBag.MedicalDeviceID = new SelectList(_context.MedicalDevices, "MedicalDeviceID", "Name", location.MedicalDeviceID);
            ViewBag.RoomId = new SelectList(_context.Room, "RoomId", "Name", location.RoomId);

            return View(location);
        }

        // POST: LocationMedDevice/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LocationMedDeviceID,MedicalDeviceID,RoomId,InitialDate,EndDate")] LocationMedDevice locationMedDevice)
        {
            if (id != locationMedDevice.LocationMedDeviceID)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(locationMedDevice);

                    var device = await _context.MedicalDevices
                        .Include(d => d.LocalizacaoDispMedicoMovel)
                        .FirstOrDefaultAsync(d => d.MedicalDeviceID == locationMedDevice.MedicalDeviceID);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LocationMedDeviceExists(locationMedDevice.LocationMedDeviceID))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Details),
                    new
                    {
                        id = locationMedDevice.LocationMedDeviceID,
                        SuccessMessage = "Localização editada com sucesso."
                    });
            }

            ViewBag.MedicalDeviceID = new SelectList(_context.MedicalDevices, "MedicalDeviceID", "Name", locationMedDevice.MedicalDeviceID);
            ViewBag.RoomId = new SelectList(_context.Room, "RoomId", "Name", locationMedDevice.RoomId);

            return View(locationMedDevice);
        }

        // GET: LocationMedDevice/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var location = await _context.LocationMedDevice
                .Include(l => l.MedicalDevice)
                .Include(l => l.Room)
                .FirstOrDefaultAsync(m => m.LocationMedDeviceID == id);

            if (location == null)
                return NotFound();

            return View(location);
        }

        // POST: LocationMedDevice/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var location = await _context.LocationMedDevice.FindAsync(id);
            if (location != null)
            {
                _context.LocationMedDevice.Remove(location);
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "Localização eliminada com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        private bool LocationMedDeviceExists(int id)
        {
            return _context.LocationMedDevice.Any(e => e.LocationMedDeviceID == id);
        }
    }
}
