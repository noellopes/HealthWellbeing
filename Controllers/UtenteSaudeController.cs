using System.Text.Json;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Controllers
{
    [Authorize]
    public class UtenteSaudeController : Controller
    {
        private readonly HealthWellbeingDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public UtenteSaudeController(HealthWellbeingDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

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
        // UTENTE VIEW (só Utente) - dados pelo email logado
        // ================================
        [HttpGet]
        [Authorize(Roles = "Utente")]
        public async Task<IActionResult> UtenteView()
        {
            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null) return Challenge();

            var email = await _userManager.GetEmailAsync(identityUser) ?? identityUser.UserName;
            if (string.IsNullOrWhiteSpace(email)) return Forbid();

            var utente = await _db.UtenteSaude
                .Include(u => u.Client)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Client != null && u.Client.Email == email);

            if (utente == null)
            {
                TempData["Msg"] = "Não existe um Utente de Saúde associado ao seu email.";
                return RedirectToAction("Index", "Home");
            }

            return View(utente);
        }

        // ================================
        // RECECIONISTA VIEW (só Rececionista)
        // ================================
        [HttpGet]
        [Authorize(Roles = "Rececionista")]
        public async Task<IActionResult> RecepcionistaView(int page = 1, string searchNome = "", string searchNif = "")
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

            ViewBag.NomeRecepcionista = "Rececionista";
            return View(utentesInfo);
        }

        // ================================
        // GERIR UTENTES (DiretorClinico)
        // ================================
        [Authorize(Roles = "DiretorClinico")]
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

        [Authorize(Roles = "DiretorClinico,Rececionista")]
       
        public async Task<IActionResult> Details(int id)
        {
            var u = await _db.UtenteSaude
                .Include(x => x.Client)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UtenteSaudeId == id);

            if (u == null) return NotFound();
            return View(u);
        }

        [HttpGet]
        [Authorize(Roles = "DiretorClinico")]
        public IActionResult Create()
        {
            LoadClientsJsonAvailable();

            var vm = new UtenteSaudeFormVM
            {
                IsEdit = false
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "DiretorClinico")]
        public async Task<IActionResult> Create(UtenteSaudeFormVM vm)
        {
            vm.IsEdit = false;

            if (!string.IsNullOrWhiteSpace(vm.Nif) && await _db.UtenteSaude.AnyAsync(x => x.Nif == vm.Nif))
                ModelState.AddModelError(nameof(vm.Nif), "Já existe um utente com este NIF.");

            if (!string.IsNullOrWhiteSpace(vm.Nus) && await _db.UtenteSaude.AnyAsync(x => x.Nus == vm.Nus))
                ModelState.AddModelError(nameof(vm.Nus), "Já existe um utente com este NUS.");

            if (!string.IsNullOrWhiteSpace(vm.Niss) && await _db.UtenteSaude.AnyAsync(x => x.Niss == vm.Niss))
                ModelState.AddModelError(nameof(vm.Niss), "Já existe um utente com este NISS.");

            if (vm.ClientId.HasValue && vm.ClientId.Value > 0)
            {
                bool clientExists = await _db.Client.AnyAsync(c => c.ClientId == vm.ClientId.Value);
                if (!clientExists)
                    ModelState.AddModelError(nameof(vm.ClientId), "Selecione um cliente válido (da lista).");

                if (await _db.UtenteSaude.AnyAsync(x => x.ClientId == vm.ClientId.Value))
                    ModelState.AddModelError(nameof(vm.ClientId), "Este cliente já tem um Utente de Saúde associado.");

                if (!ModelState.IsValid)
                {
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

        [HttpGet]
        [Authorize(Roles = "DiretorClinico")]
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
                Name = u.Client.Name,
                Email = u.Client.Email,
                Phone = u.Client.Phone,
                Address = u.Client.Address,
                BirthDate = u.Client.BirthDate,
                Gender = u.Client.Gender,
                Nif = u.Nif,
                Niss = u.Niss,
                Nus = u.Nus
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "DiretorClinico")]
        public async Task<IActionResult> Edit(int id, UtenteSaudeFormVM vm)
        {
            vm.IsEdit = true;

            if (id != vm.UtenteSaudeId) return NotFound();

            var existing = await _db.UtenteSaude
                .Include(x => x.Client)
                .FirstOrDefaultAsync(x => x.UtenteSaudeId == id);

            if (existing == null) return NotFound();
            if (existing.Client == null) return NotFound();

            if (!vm.ClientId.HasValue || vm.ClientId.Value != existing.ClientId)
                ModelState.AddModelError(nameof(vm.ClientId), "Não é permitido alterar o cliente associado.");

            if (await _db.UtenteSaude.AnyAsync(x => x.UtenteSaudeId != id && x.Nif == vm.Nif))
                ModelState.AddModelError(nameof(vm.Nif), "Já existe um utente com este NIF.");

            if (await _db.UtenteSaude.AnyAsync(x => x.UtenteSaudeId != id && x.Nus == vm.Nus))
                ModelState.AddModelError(nameof(vm.Nus), "Já existe um utente com este NUS.");

            if (await _db.UtenteSaude.AnyAsync(x => x.UtenteSaudeId != id && x.Niss == vm.Niss))
                ModelState.AddModelError(nameof(vm.Niss), "Já existe um utente com este NISS.");

            if (!ModelState.IsValid)
                return View(vm);

            existing.Nif = vm.Nif;
            existing.Niss = vm.Niss;
            existing.Nus = vm.Nus;

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

        [HttpGet]
        [Authorize(Roles = "DiretorClinico")]
        public async Task<IActionResult> Delete(int id)
        {
            var u = await _db.UtenteSaude
                .Include(x => x.Client)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UtenteSaudeId == id);

            if (u == null) return NotFound();
            return View(u);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "DiretorClinico")]
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
        [HttpGet]
        [Authorize]
        //Consulta BTN ACESS
        [HttpGet]
        [Authorize]
        public IActionResult GestaoConsultas()
        {
            if (User.IsInRole("Administrador"))
                return RedirectToAction("Index", "Admin");

            if (User.IsInRole("DiretorClinico"))
                return RedirectToAction(nameof(Index), "UtenteSaude");

            if (User.IsInRole("Rececionista"))
                return RedirectToAction(nameof(RecepcionistaView), "UtenteSaude");

            if (User.IsInRole("Utente"))
                return RedirectToAction(nameof(UtenteView), "UtenteSaude");

            if (User.IsInRole("Medico"))
                return RedirectToAction("Index", "Consultas");

            return Forbid();
        }

    }
}
