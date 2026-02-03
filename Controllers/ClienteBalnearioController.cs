using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HealthWellbeing.Controllers
{
    
    public class ClienteBalnearioController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClienteBalnearioController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // INDEX
        // =========================
        public async Task<IActionResult> Index(
    string? search,
    bool? ativos,
    string? sort,
    int? minPontos,
    int page = 1)
        {
            int pageSize = 10;

            var query = _context.ClientesBalneario
                .Include(c => c.HistoricoPontos)
                .AsQueryable();

            // =========================
            // FILTROS
            // =========================

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(c => c.Nome.Contains(search));

            if (ativos.HasValue)
                query = query.Where(c => c.Ativo == ativos.Value);

            if (minPontos.HasValue)
                query = query.Where(c =>
                    c.HistoricoPontos.Sum(p => p.Pontos) >= minPontos.Value);

            // =========================
            // DASHBOARD (CONTADORES)
            // =========================

            ViewBag.TotalClientes = await _context.ClientesBalneario.CountAsync();
            ViewBag.TotalAtivos = await _context.ClientesBalneario.CountAsync(c => c.Ativo);
            ViewBag.TotalInativos = await _context.ClientesBalneario.CountAsync(c => !c.Ativo);

            ViewBag.TotalCom50Pontos = await _context.ClientesBalneario
                .Where(c => c.HistoricoPontos.Sum(p => p.Pontos) >= 50)
                .CountAsync();

            // =========================
            // ORDENAÇÃO
            // =========================

            query = sort switch
            {
                "nome_desc" => query.OrderByDescending(c => c.Nome),
                "pontos" => query.OrderBy(c => c.HistoricoPontos.Sum(p => p.Pontos)),
                "pontos_desc" => query.OrderByDescending(c => c.HistoricoPontos.Sum(p => p.Pontos)),
                _ => query.OrderBy(c => c.Nome)
            };

            // =========================
            // PAGINAÇÃO
            // =========================

            var totalItems = await query.CountAsync();

            var clientes = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // =========================
            // VIEWBAGS
            // =========================

            ViewBag.Search = search;
            ViewBag.Ativos = ativos;
            ViewBag.Sort = sort;
            ViewBag.MinPontos = minPontos;

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return View(clientes);
        }



        // =========================
        // DETAILS  
        // =========================
        public async Task<IActionResult> Details(int id)
        {
            var cliente = await _context.ClientesBalneario
                .Include(c => c.Utentes)
                .Include(c => c.HistoricoPontos)
                .Include(c => c.Satisfacoes)
                .Include(c => c.Vouchers)
                .Include(c => c.NivelCliente)
                .AsNoTracking() // 👈 evita estados antigos e bugs visuais
                .FirstOrDefaultAsync(c => c.ClienteBalnearioId == id);

            if (cliente == null)
                return NotFound();

            return View(cliente);
        }

        // =========================
        // CREATE
        // =========================
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClienteBalneario cliente)
        {
            bool emailExiste = await _context.ClientesBalneario
                .AnyAsync(c => c.Email == cliente.Email);

            if (emailExiste)
            {
                ModelState.AddModelError("Email", "Já existe um cliente com este email.");
            }

            if (!ModelState.IsValid)
                return View(cliente);

            cliente.DataRegisto = DateTime.Now;
            cliente.Ativo = true;

            _context.ClientesBalneario.Add(cliente);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cliente criado com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // EDIT
        // =========================
        public async Task<IActionResult> Edit(int id)
        {
            var cliente = await _context.ClientesBalneario.FindAsync(id);

            if (cliente == null)
                return NotFound();

            return View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClienteBalneario cliente)
        {
            if (id != cliente.ClienteBalnearioId)
                return NotFound();

            bool emailDuplicado = await _context.ClientesBalneario
                .AnyAsync(c => c.Email == cliente.Email &&
                               c.ClienteBalnearioId != cliente.ClienteBalnearioId);

            if (emailDuplicado)
            {
                ModelState.AddModelError("Email", "Este email já está associado a outro cliente.");
            }

            if (!ModelState.IsValid)
                return View(cliente);

            var clienteDb = await _context.ClientesBalneario.FindAsync(id);
            if (clienteDb == null)
                return NotFound();

            clienteDb.Nome = cliente.Nome;
            clienteDb.Email = cliente.Email;
            clienteDb.Telemovel = cliente.Telemovel;
            clienteDb.Ativo = cliente.Ativo;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Cliente atualizado com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // ATIVAR / DESATIVAR
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleAtivo(int id)
        {
            var cliente = await _context.ClientesBalneario.FindAsync(id);
            if (cliente == null)
                return NotFound();

            cliente.Ativo = !cliente.Ativo;
            await _context.SaveChangesAsync();

            TempData["Success"] = cliente.Ativo
                ? "Cliente ativado."
                : "Cliente desativado.";

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // HELPER
        // =========================
        private void LoadUtentes(int? selected = null)
        {
            ViewBag.Utentes = new SelectList(
                _context.UtenteBalnearios.OrderBy(u => u.Nome),
                "UtenteBalnearioId",
                "Nome",
                selected
            );
        }
    }
}
