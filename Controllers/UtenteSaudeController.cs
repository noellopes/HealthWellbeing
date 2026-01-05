using System.Text.Json;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Controllers
{
    public class UtenteSaudeController : Controller
    {
        private readonly HealthWellbeingDbContext _db;

        public UtenteSaudeController(HealthWellbeingDbContext db)
        {
            _db = db;
        }

        // ================================
        // Helper: JSON de clients disponíveis (para autocomplete no Create)
        // ================================
        private void LoadClientsJsonAvailable()
        {
            var available = _db.Client.AsNoTracking()
                .Where(c => c.UtenteSaude == null)
                .OrderBy(c => c.Name)
                .Select(c => new
                {
                    clientId = c.ClientId,
                    name = c.Name,
                    email = c.Email,
                    phone = c.Phone,
                    address = c.Address,
                    birthDate = c.BirthDate.HasValue ? c.BirthDate.Value.ToString("yyyy-MM-dd") : "",
                    gender = c.Gender
                })
                .ToList();

            ViewBag.ClientsJson = JsonSerializer.Serialize(available);
        }

        // ================================
        // LISTAR (Index) - igual ao teu
        // ================================
        public async Task<IActionResult> Index(int page = 1, string searchNome = "", string searchNif = "")
        {
            if (page < 1) page = 1;

            var utentesQuery = _db.UtenteSaude
                .Include(u => u.Client)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchNome))
                utentesQuery = utentesQuery.Where(u => u.Client != null && u.Client.Name.Contains(searchNome));

            if (!string.IsNullOrWhiteSpace(searchNif))
                utentesQuery = utentesQuery.Where(u => u.Nif.Contains(searchNif));

            ViewBag.SearchNome = searchNome;
            ViewBag.SearchNif = searchNif;

            int numberUtentes = await utentesQuery.CountAsync();
            if (numberUtentes == 0) page = 1;

            var utentesInfo = new PaginationInfo<UtenteSaude>(page, numberUtentes);

            utentesInfo.Items = await utentesQuery
                .AsNoTracking()
                .OrderBy(u => u.Client!.Name)
                .Skip(utentesInfo.ItemsToSkip)
                .Take(utentesInfo.ItemsPerPage)
                .ToListAsync();

            return View(utentesInfo);
        }

        // ================================
        // DETALHES - igual ao teu
        // ================================
        public async Task<IActionResult> Details(int id)
        {
            var u = await _db.UtenteSaude
                .Include(x => x.Client)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UtenteSaudeId == id);

            if (u == null) return NotFound();
            return View(u);
        }

        // ================================
        // CREATE (GET) - com autocomplete
        // ================================
        [HttpGet]
        public IActionResult Create()
        {
            LoadClientsJsonAvailable();

            var vm = new UtenteSaudeFormVM
            {
                IsEdit = false
            };

            return View(vm);
        }

        // ================================
        // CREATE (POST)
        // - Se ClientId vem preenchido => cria só UtenteSaude e associa a cliente existente
        // - Se ClientId vem vazio => cria Client + UtenteSaude associados
        // ================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UtenteSaudeFormVM vm)
        {
            vm.IsEdit = false;

            // Unicidades do Utente
            if (!string.IsNullOrWhiteSpace(vm.Nif) && await _db.UtenteSaude.AnyAsync(x => x.Nif == vm.Nif))
                ModelState.AddModelError(nameof(vm.Nif), "Já existe um utente com este NIF.");

            if (!string.IsNullOrWhiteSpace(vm.Nus) && await _db.UtenteSaude.AnyAsync(x => x.Nus == vm.Nus))
                ModelState.AddModelError(nameof(vm.Nus), "Já existe um utente com este NUS.");

            if (!string.IsNullOrWhiteSpace(vm.Niss) && await _db.UtenteSaude.AnyAsync(x => x.Niss == vm.Niss))
                ModelState.AddModelError(nameof(vm.Niss), "Já existe um utente com este NISS.");

            // ===============================
            // CASO A) escolheu cliente existente
            // ===============================
            if (vm.ClientId.HasValue && vm.ClientId.Value > 0)
            {
                bool clientExists = await _db.Client.AnyAsync(c => c.ClientId == vm.ClientId.Value);
                if (!clientExists)
                    ModelState.AddModelError(nameof(vm.ClientId), "Selecione um cliente válido (da lista).");

                if (await _db.UtenteSaude.AnyAsync(x => x.ClientId == vm.ClientId.Value))
                    ModelState.AddModelError(nameof(vm.ClientId), "Este cliente já tem um Utente de Saúde associado.");

                if (!ModelState.IsValid)
                {
                    // repõe dados do cliente (para não depender do JS no postback)
                    var c = await _db.Client.AsNoTracking().FirstOrDefaultAsync(x => x.ClientId == vm.ClientId.Value);
                    if (c != null)
                    {
                        vm.Name = c.Name;
                        vm.Email = c.Email;
                        vm.Phone = c.Phone;
                        vm.Address = c.Address;
                        vm.BirthDate = c.BirthDate;
                        vm.Gender = c.Gender;
                    }

                    LoadClientsJsonAvailable();
                    return View(vm);
                }

                var u = new UtenteSaude
                {
                    ClientId = vm.ClientId.Value,
                    Nif = vm.Nif,
                    Niss = vm.Niss,
                    Nus = vm.Nus,
                    Client = null
                };

                _db.UtenteSaude.Add(u);
                await _db.SaveChangesAsync();

                TempData["Msg"] = "Utente criado e associado a um cliente existente.";
                return RedirectToAction(nameof(Details), new { id = u.UtenteSaudeId });
            }

            // ===============================
            // CASO B) não escolheu cliente => criar Client + Utente
            // ===============================
            if (!ModelState.IsValid)
            {
                LoadClientsJsonAvailable();
                return View(vm);
            }

            var newClient = new Client
            {
                Name = vm.Name!,
                Email = vm.Email!,
                Phone = vm.Phone!,
                Address = vm.Address ?? "",
                BirthDate = vm.BirthDate,
                Gender = vm.Gender ?? ""
            };

            var newUtente = new UtenteSaude
            {
                Client = newClient,
                Nif = vm.Nif,
                Niss = vm.Niss,
                Nus = vm.Nus
            };

            _db.UtenteSaude.Add(newUtente);
            await _db.SaveChangesAsync();

            TempData["Msg"] = "Cliente e utente criados com sucesso.";
            return RedirectToAction(nameof(Details), new { id = newUtente.UtenteSaudeId });
        }

        // ================================
        // EDIT (GET) - devolve VM para editar Client + Utente
        // ================================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var u = await _db.UtenteSaude
                .Include(x => x.Client)
                .FirstOrDefaultAsync(x => x.UtenteSaudeId == id);

            if (u == null) return NotFound();
            if (u.Client == null) return NotFound();

            var vm = new UtenteSaudeFormVM
            {
                IsEdit = true,
                UtenteSaudeId = u.UtenteSaudeId,
                ClientId = u.ClientId,

                // Client
                Name = u.Client.Name,
                Email = u.Client.Email,
                Phone = u.Client.Phone,
                Address = u.Client.Address,
                BirthDate = u.Client.BirthDate,
                Gender = u.Client.Gender,

                // Utente
                Nif = u.Nif,
                Niss = u.Niss,
                Nus = u.Nus
            };

            return View(vm);
        }

        // ================================
        // EDIT (POST) - atualiza Client + Utente
        // ================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UtenteSaudeFormVM vm)
        {
            vm.IsEdit = true;

            if (id != vm.UtenteSaudeId) return NotFound();

            var existing = await _db.UtenteSaude
                .Include(x => x.Client)
                .FirstOrDefaultAsync(x => x.UtenteSaudeId == id);

            if (existing == null) return NotFound();
            if (existing.Client == null) return NotFound();

            // proteção: não permitir trocar o ClientId
            if (!vm.ClientId.HasValue || vm.ClientId.Value != existing.ClientId)
                ModelState.AddModelError(nameof(vm.ClientId), "Não é permitido alterar o cliente associado.");

            // unicidade (ignora o próprio)
            if (await _db.UtenteSaude.AnyAsync(x => x.UtenteSaudeId != id && x.Nif == vm.Nif))
                ModelState.AddModelError(nameof(vm.Nif), "Já existe um utente com este NIF.");

            if (await _db.UtenteSaude.AnyAsync(x => x.UtenteSaudeId != id && x.Nus == vm.Nus))
                ModelState.AddModelError(nameof(vm.Nus), "Já existe um utente com este NUS.");

            if (await _db.UtenteSaude.AnyAsync(x => x.UtenteSaudeId != id && x.Niss == vm.Niss))
                ModelState.AddModelError(nameof(vm.Niss), "Já existe um utente com este NISS.");

            if (!ModelState.IsValid)
                return View(vm);

            // Utente
            existing.Nif = vm.Nif;
            existing.Niss = vm.Niss;
            existing.Nus = vm.Nus;

            // Client
            existing.Client.Name = vm.Name ?? "";
            existing.Client.Email = vm.Email ?? "";
            existing.Client.Phone = vm.Phone ?? "";
            existing.Client.Address = vm.Address ?? "";
            existing.Client.BirthDate = vm.BirthDate;
            existing.Client.Gender = vm.Gender ?? "";

            await _db.SaveChangesAsync();

            TempData["Msg"] = "Utente e cliente atualizados com sucesso.";
            return RedirectToAction(nameof(Details), new { id = existing.UtenteSaudeId });
        }

        // ================================
        // DELETE (GET)
        // ================================
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var u = await _db.UtenteSaude
                .Include(x => x.Client)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UtenteSaudeId == id);

            if (u == null) return NotFound();
            return View(u);
        }

        // ================================
        // DELETE (POST) - remove só Utente (Client fica)
        // ================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var u = await _db.UtenteSaude.FindAsync(id);
            if (u != null)
            {
                _db.UtenteSaude.Remove(u);
                await _db.SaveChangesAsync();
                TempData["Msg"] = "Utente removido.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
