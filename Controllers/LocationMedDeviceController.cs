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
        public async Task<IActionResult> Index(int page = 1)
        {
            int itemsPerPage = 10;

            // Query base (IQueryable) com Includes
            var query = _context.LocationMedDevice
                .Include(l => l.MedicalDevice)
                .Include(l => l.Room)
                .AsQueryable();

            // Total de itens
            int totalItems = await query.CountAsync();

            // Instancia o ViewModel de Paginação
            var paginationInfo = new RPaginationInfo<LocationMedDevice>(page, totalItems, itemsPerPage);

            // Busca os itens da página atual
            var locs = await query
                .OrderBy(l => l.LocationMedDeviceID) // Ordenação consistente
                .Skip(paginationInfo.ItemsToSkip)
                .Take(paginationInfo.ItemsPerPage)
                .ToListAsync();

            // Atribui os itens ao ViewModel
            paginationInfo.Items = locs;

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
                // Garantir que o novo registro seja a localização atual
                locationMedDevice.IsCurrent = true;

                // Buscar o dispositivo médico associado, incluindo suas localizações atuais
                var device = await _context.MedicalDevices
                    .Include(d => d.LocalizacaoDispMedicoMovel)
                    .FirstOrDefaultAsync(d => d.MedicalDeviceID == locationMedDevice.MedicalDeviceID);

                if (device != null)
                {
                    // Marcar todas as outras localizações atuais como não atuais
                    var currentLocations = device.LocalizacaoDispMedicoMovel
                                                .Where(l => l.IsCurrent)
                                                .ToList();

                    foreach (var loc in currentLocations)
                    {
                        loc.IsCurrent = false;
                    }
                }

                // Adicionar a nova localização
                _context.Add(locationMedDevice);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // Recarregar dropdowns se houver erro de validação
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

                    // Buscar o dispositivo para atualizar disponibilidade
                    var device = await _context.MedicalDevices
                        .Include(d => d.LocalizacaoDispMedicoMovel)
                        .FirstOrDefaultAsync(d => d.MedicalDeviceID == locationMedDevice.MedicalDeviceID);

                    if (device != null)
                    {
                        // Se a alocação foi encerrada (tem DataFim) → o equipamento volta a ficar disponível
                        if (locationMedDevice.EndDate != null)
                        {
                            // Nada para gravar, CurrentStatus é calculado dinamicamente.
                        }
                    }

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
                         SuccessMessage = "Localização editado com sucesso."
                     }
                 );


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

            TempData["SuccessMessage"] = "Localização eliminado com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        private bool LocationMedDeviceExists(int id)
        {
            return _context.LocationMedDevice.Any(e => e.LocationMedDeviceID == id);
        }
    }
}
