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

        // GET: LocationMedDevice/Create/16
        // O parâmetro 'id' é o ID do dispositivo que está a ser movido.
        public async Task<IActionResult> Create(int? id)
        {
            if (id == null)
            {
                // Se a chamada for genérica (sem ID), carrega a lista completa
                ViewBag.MedicalDeviceID = new SelectList(_context.MedicalDevices, "MedicalDeviceID", "Name");
            }
            else
            {
                // 1. Encontra o dispositivo para garantir que existe e obter o nome
                var dispositivo = await _context.MedicalDevices.FindAsync(id.Value);

                if (dispositivo == null)
                {
                    return NotFound();
                }

                // 2. Passa o ID e o Nome do dispositivo para a View.
                // O ID é passado no Model/ViewBag. O utilizador NÃO PODE ESCOLHER OUTRO DISPOSITIVO.
                ViewBag.DispositivoMedicoID = id.Value;
                ViewBag.DeviceName = dispositivo.Name;

                // 3. Cria um SelectList com APENAS o dispositivo atual (opcional, mas claro)
                ViewBag.MedicalDeviceID = new SelectList(
                    new List<MedicalDevice> { dispositivo }, // Lista de 1 item
                    "MedicalDeviceID",
                    "Name",
                    id.Value
                );
            }

            // Carrega a lista de Salas (Rooms)
            ViewBag.RoomId = new SelectList(_context.Room, "RoomId", "Name");

            // Retorna a View
            return View(new LocationMedDevice { MedicalDeviceID = id.GetValueOrDefault() });
        }

        // POST: LocationMedDevice/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MedicalDeviceID,RoomId")] LocationMedDevice locationMedDevice)
        {
            if (ModelState.IsValid)
            {
                // 1. Encerrar o registo anterior (UPDATE LocationMedDevice)
                var registoAnterior = await _context.LocationMedDevice
                    .FirstOrDefaultAsync(l => l.MedicalDeviceID == locationMedDevice.MedicalDeviceID && l.EndDate == null);

                if (registoAnterior != null)
                {
                    registoAnterior.EndDate = DateTime.Now;
                    _context.Update(registoAnterior);
                }

                // 2. Criar o novo registo ATIVO (INSERT LocationMedDevice)
                locationMedDevice.InitialDate = DateTime.Now;
                locationMedDevice.EndDate = null;
                locationMedDevice.IsCurrent = true; // Mantemos a propriedade do seu colega, se necessária

                _context.Add(locationMedDevice);

                // 3. 🎯 CORREÇÃO: Atualizar o Status do Dispositivo Médico (UPDATE MedicalDevice)
                var dispositivo = await _context.MedicalDevices
                    .FirstOrDefaultAsync(d => d.MedicalDeviceID == locationMedDevice.MedicalDeviceID);

                if (dispositivo != null)
                {
                    if (dispositivo.IsUnderMaintenance)
                    {
                        dispositivo.IsUnderMaintenance = false; // Tira da manutenção para poder ser alocado/movido
                        _context.Update(dispositivo);
                    }
                    _context.Update(dispositivo);
                }

                // 4. Salvar todas as alterações (Antiga Localização, Nova Localização e Status do Dispositivo)
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Dispositivo '{dispositivo.Name}' movido com sucesso para a Sala {locationMedDevice.RoomId}!";

                // Redireciona para os Detalhes do Dispositivo (para ver o novo estado)
                return RedirectToAction("Details", "MedicalDevice", new { id = locationMedDevice.MedicalDeviceID });
            }

            // Recarregar ViewBags em caso de falha
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
