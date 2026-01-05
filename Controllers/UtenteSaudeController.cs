using System.Text.Json;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        // Helper: dropdown + JSON para autopreencher no Create
        // ================================
        private void LoadClientsForDropdown(int? selectedClientId = null)
        {
            var available = _db.Client.AsNoTracking()
                .Where(c => c.UtenteSaude == null) // se a navigation existir
                .OrderBy(c => c.Name);

            ViewBag.ClientId = new SelectList(available, "ClientId", "Name", selectedClientId);

            var clients = available.Select(c => new
            {
                clientId = c.ClientId,
                name = c.Name,
                email = c.Email,
                phone = c.Phone,
                address = c.Address,
                birthDate = c.BirthDate
            }).ToList();

            ViewBag.ClientsJson = JsonSerializer.Serialize(clients);
        }


        // ================================
        // TESTE UI (UtenteView)
        // ================================
        [HttpGet]
        public IActionResult UtenteView()
        {
            var utenteFixe = new UtenteSaude
            {
                UtenteSaudeId = 1,
                ClientId = 1,
                Nif = "245123987",
                Niss = "12345678901",
                Nus = "123456789",
                Client = new Client
                {
                    ClientId = 1,
                    Name = "Maria Correia",
                    Email = "maria.correia@email.pt",
                    Phone = "912345678",
                    Address = "Rua Exemplo, 10, 3500-000 Viseu",
                    BirthDate = new DateTime(1999, 5, 12),
                    Gender = "Female"
                }
            };

            return View(utenteFixe);
        }

        // ================================
        // RECEPCIONISTA VIEW (Paginação + Pesquisa)
        // ================================
        [HttpGet]
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

            ViewBag.NomeRecepcionista = "Recepcionista";

            return View(utentesInfo);
        }

        // ================================
        // LISTAR (Index)
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
        // DETALHES
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
        // CREATE (GET)
        // ================================
        [HttpGet]
        public IActionResult Create()
        {
            LoadClientsForDropdown();
            return View(new UtenteSaude());
        }

        // ================================
        // CREATE (POST)
        // ================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClientId,Nif,Niss,Nus")] UtenteSaude u)
        {
            // 1) Validação: cliente existe (sem carregar navigation para o EF)
            bool clientExists = await _db.Client.AnyAsync(c => c.ClientId == u.ClientId);
            if (!clientExists)
                ModelState.AddModelError(nameof(UtenteSaude.ClientId), "Selecione um cliente válido.");

            // 2) Regra 1-1 (Cliente só pode ter 1 UtenteSaude)
            if (await _db.UtenteSaude.AnyAsync(x => x.ClientId == u.ClientId))
                ModelState.AddModelError(nameof(UtenteSaude.ClientId), "Este cliente já tem um Utente de Saúde associado.");

            // 3) Unicidades
            if (!string.IsNullOrWhiteSpace(u.Nif) && await _db.UtenteSaude.AnyAsync(x => x.Nif == u.Nif))
                ModelState.AddModelError(nameof(UtenteSaude.Nif), "Já existe um utente com este NIF.");

            if (!string.IsNullOrWhiteSpace(u.Nus) && await _db.UtenteSaude.AnyAsync(x => x.Nus == u.Nus))
                ModelState.AddModelError(nameof(UtenteSaude.Nus), "Já existe um utente com este NUS.");

            if (!string.IsNullOrWhiteSpace(u.Niss) && await _db.UtenteSaude.AnyAsync(x => x.Niss == u.Niss))
                ModelState.AddModelError(nameof(UtenteSaude.Niss), "Já existe um utente com este NISS.");

            // 4) Se falhar -> aí sim carregamos o Client (só para mostrar os campos read-only)
            if (!ModelState.IsValid)
            {
                u.Client = await _db.Client.AsNoTracking().FirstOrDefaultAsync(c => c.ClientId == u.ClientId);
                LoadClientsForDropdown(u.ClientId);
                return View(u);
            }

            // 5) MUITO IMPORTANTE: não levar navigation para o Add
            u.Client = null;

            _db.UtenteSaude.Add(u);
            await _db.SaveChangesAsync();

            TempData["Msg"] = "Utente criado com sucesso.";
            return RedirectToAction(nameof(Details), new { id = u.UtenteSaudeId });
        }

        // ================================
        // EDIT (GET)  -> Cliente read-only
        // ================================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var u = await _db.UtenteSaude
                .Include(x => x.Client)
                .FirstOrDefaultAsync(x => x.UtenteSaudeId == id);

            if (u == null) return NotFound();

            // Não carregamos dropdown/JSON porque no Edit o cliente é read-only
            return View(u);
        }

        // ================================
        // EDIT (POST) -> NÃO ACEITA ClientId (proteção)
        // ================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UtenteSaudeId,ClientId,Nif,Niss,Nus")] UtenteSaude u)
        {
            if (id != u.UtenteSaudeId) return NotFound();

            var existing = await _db.UtenteSaude
                .Include(x => x.Client)
                .FirstOrDefaultAsync(x => x.UtenteSaudeId == id);

            if (existing == null) return NotFound();

            // Unicidade (ignora o próprio registo)
            if (await _db.UtenteSaude.AnyAsync(x => x.UtenteSaudeId != id && x.Nif == u.Nif))
                ModelState.AddModelError(nameof(UtenteSaude.Nif), "Já existe um utente com este NIF.");

            if (await _db.UtenteSaude.AnyAsync(x => x.UtenteSaudeId != id && x.Nus == u.Nus))
                ModelState.AddModelError(nameof(UtenteSaude.Nus), "Já existe um utente com este NUS.");

            if (await _db.UtenteSaude.AnyAsync(x => x.UtenteSaudeId != id && x.Niss == u.Niss))
                ModelState.AddModelError(nameof(UtenteSaude.Niss), "Já existe um utente com este NISS.");

            if (!ModelState.IsValid)
            {
                // para manter valores no ecrã
                existing.Nif = u.Nif;
                existing.Niss = u.Niss;
                existing.Nus = u.Nus;
                return View(existing);
            }

            existing.Nif = u.Nif;
            existing.Niss = u.Niss;
            existing.Nus = u.Nus;

            await _db.SaveChangesAsync();

            TempData["Msg"] = "Utente atualizado com sucesso.";
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
        // DELETE (POST)
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
