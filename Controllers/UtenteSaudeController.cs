using System.Text.Json;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        private const string SessUtenteId = "SelectedUtenteSaudeId";
        private const string SessUtenteEmail = "SelectedUtenteEmail";
        private const string SessUtenteNome = "SelectedUtenteNome";

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
        [HttpGet]
        [Authorize(Roles = "DiretorClinico")]
        /*  public async Task<IActionResult> Index(int page = 1, string searchNome = "", string searchNif = "")
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
          }*/
        /*
        [HttpGet]
        [Authorize(Roles = "DiretorClinico")]
        public async Task<IActionResult> Index(int page = 1, string searchNome = "", string searchNif = "")
        {
            if (page < 1) page = 1;

            var clientsQuery = _db.Client
                .Include(c => c.UtenteSaude)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchNome))
                clientsQuery = clientsQuery.Where(c => c.Name.Contains(searchNome));

            if (!string.IsNullOrWhiteSpace(searchNif))
                clientsQuery = clientsQuery.Where(c => c.UtenteSaude != null && c.UtenteSaude.Nif.Contains(searchNif));

            ViewBag.SearchNome = searchNome;
            ViewBag.SearchNif = searchNif;

            int numberClients = await clientsQuery.CountAsync();
            if (numberClients == 0) page = 1;

            var info = new PaginationInfo<Client>(page, numberClients);

            info.Items = await clientsQuery
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Skip(info.ItemsToSkip)
                .Take(info.ItemsPerPage)
                .ToListAsync();

            return View(info);
        }*/
        [HttpGet]
        [Authorize(Roles = "DiretorClinico")]
        public async Task<IActionResult> Index(int page = 1, string searchNome = "", string searchNif = "", bool onlyUtentes = false)
        {
            if (page < 1) page = 1;

            var clientsQuery = _db.Client
                .Include(c => c.UtenteSaude)
                .AsQueryable();

            // ✅ filtro do botão
            if (onlyUtentes)
                clientsQuery = clientsQuery.Where(c => c.UtenteSaude != null);

            if (!string.IsNullOrWhiteSpace(searchNome))
                clientsQuery = clientsQuery.Where(c => c.Name.Contains(searchNome));

            if (!string.IsNullOrWhiteSpace(searchNif))
                clientsQuery = clientsQuery.Where(c => c.UtenteSaude != null && c.UtenteSaude.Nif.Contains(searchNif));

            ViewBag.SearchNome = searchNome;
            ViewBag.SearchNif = searchNif;
            ViewBag.OnlyUtentes = onlyUtentes;

            int numberClients = await clientsQuery.CountAsync();
            if (numberClients == 0) page = 1;

            var info = new PaginationInfo<Client>(page, numberClients);

            info.Items = await clientsQuery
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Skip(info.ItemsToSkip)
                .Take(info.ItemsPerPage)
                .ToListAsync();

            return View(info);
        }


        // ================================
        // DETAILS (DiretorClinico e Rececionista)
        // Se for Rececionista, guarda o utente selecionado em Session
        // ================================
        [HttpGet]
        [Authorize(Roles = "DiretorClinico,Rececionista")]
        public async Task<IActionResult> Details(int id)
        {
            var u = await _db.UtenteSaude
                .Include(x => x.Client)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UtenteSaudeId == id);

            if (u == null) return NotFound();

            if (User.IsInRole("Rececionista"))
            {
                HttpContext.Session.SetInt32(SessUtenteId, u.UtenteSaudeId);

                var email = u.Client?.Email?.Trim() ?? "";
                var nome = u.Client?.Name?.Trim() ?? (u.NomeCompleto?.Trim() ?? "");

                if (!string.IsNullOrWhiteSpace(email))
                    HttpContext.Session.SetString(SessUtenteEmail, email);

                if (!string.IsNullOrWhiteSpace(nome))
                    HttpContext.Session.SetString(SessUtenteNome, nome);
            }

            return View(u);
        }

        // ================================
        // CREATE (DiretorClinico)
        // ================================
        [HttpGet]
        [Authorize(Roles = "DiretorClinico")]
        /*
        public IActionResult Create()
        {
            LoadClientsJsonAvailable();

            var vm = new UtenteSaudeFormVM
            {
                IsEdit = false
            };

            return View(vm);
        }*/
        [HttpGet]
        [Authorize(Roles = "DiretorClinico")]
        public async Task<IActionResult> Create(int? clientId)
        {
            LoadClientsJsonAvailable();

            var vm = new UtenteSaudeFormVM { IsEdit = false };

            if (clientId.HasValue && clientId.Value > 0)
            {
                var c = await _db.Client.AsNoTracking().FirstOrDefaultAsync(x => x.ClientId == clientId.Value);
                if (c == null)
                {
                    TempData["Msg"] = "Cliente não encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                bool jaTemUtente = await _db.UtenteSaude.AnyAsync(u => u.ClientId == c.ClientId);
                if (jaTemUtente)
                {
                    TempData["Msg"] = "Este cliente já tem um Utente de Saúde associado.";
                    return RedirectToAction(nameof(Index));
                }

                vm.ClientId = c.ClientId;
                vm.Name = c.Name;
                vm.Email = c.Email;
                vm.Phone = c.Phone;
                vm.Address = c.Address;
                vm.BirthDate = c.BirthDate;
                vm.Gender = c.Gender;
            }

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

            // associar a um Client existente
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

            // criar Client + Utente novos
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
        // EDIT (DiretorClinico)
        // ================================
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

        // ================================
        // DELETE (DiretorClinico)
        // ================================
      
        [HttpGet]
        [Authorize(Roles = "DiretorClinico")]
        public async Task<IActionResult> Delete(int id)
        {
            var u = await _db.UtenteSaude
                .Include(x => x.Client)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UtenteSaudeId == id);

            if (u == null) return NotFound();

            ViewBag.HasConsultas = await _db.Consulta
                .AsNoTracking()
                .AnyAsync(c => c.IdUtenteSaude == u.UtenteSaudeId);

            return View(u);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "DiretorClinico")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var u = await _db.UtenteSaude
                .FirstOrDefaultAsync(x => x.UtenteSaudeId == id);

            if (u == null)
            {
                TempData["Msg"] = "Utente não encontrado.";
                return RedirectToAction(nameof(Index));
            }
            /*
            var hasConsultas = await _db.Consulta
                .AnyAsync(c => c.IdUtenteSaude == u.UtenteSaudeId);
            
            if (hasConsultas)
            {
                TempData["Msg"] = "Não é possível apagar o utente porque existem consultas associadas.";
                return RedirectToAction(nameof(Delete), new { id = u.UtenteSaudeId });
            }
            */

            _db.UtenteSaude.Remove(u);
            await _db.SaveChangesAsync();

            TempData["Msg"] = "Utente removido.";
            return RedirectToAction(nameof(Index));
        }


        // ================================
        // REDIRECT por Role (Gestão Consultas)
        // ================================
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
