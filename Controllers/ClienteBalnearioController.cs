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
    [Authorize]
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
        public async Task<IActionResult> Index()
        {
            var clientes = await _context.ClientesBalneario
                .OrderBy(c => c.Nome)
                .ToListAsync();

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
