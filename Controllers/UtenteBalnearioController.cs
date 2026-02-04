using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HealthWellbeing.Controllers
{
    public class UtenteBalnearioController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public UtenteBalnearioController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // =========================
        // INDEX
        // =========================
        public async Task<IActionResult> Index(
            string? search,
            bool? ativos,
            string? sort,
            int page = 1)
        {
            int pageSize = 10;

            var query = _context.UtenteBalnearios
                .Include(u => u.Genero)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(u => u.Nome.Contains(search));

            if (ativos.HasValue)
                query = query.Where(u => u.Ativo == ativos.Value);

            query = sort switch
            {
                "nome_desc" => query.OrderByDescending(u => u.Nome),
                "data" => query.OrderBy(u => u.DataInscricao),
                "data_desc" => query.OrderByDescending(u => u.DataInscricao),
                _ => query.OrderBy(u => u.Nome)
            };

            var totalItems = await query.CountAsync();

            var utentes = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.Ativos = ativos;
            ViewBag.Sort = sort;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return View(utentes);
        }

        // =========================
        // DETAILS
        // =========================
        public async Task<IActionResult> Details(int id)
        {
            var utente = await _context.UtenteBalnearios
                .Include(u => u.Genero)
                .Include(u => u.SeguroSaude)
                .Include(u => u.HistoricosMedicos)
                .ThenInclude(h => h.CriadoPorUser)
                .FirstOrDefaultAsync(u => u.UtenteBalnearioId == id);

            if (utente == null)
                return NotFound();

            return View(utente);
        }

        // =========================
        // CREATE GET
        // =========================
        public IActionResult Create()
        {
            LoadGeneros();
            LoadSeguros();
            return View();
        }

        // =========================
        // CREATE POST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UtenteBalneario utente)
        {
            if (!ModelState.IsValid)
            {
                LoadGeneros(utente.GeneroId);
                LoadSeguros(utente.SeguroSaudeId);
                return View(utente);
            }

            utente.DataInscricao = DateTime.Now;
            utente.Ativo = true;

            _context.Add(utente);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details),
                new { id = utente.UtenteBalnearioId });
        }

        // =========================
        // EDIT GET
        // =========================
        public async Task<IActionResult> Edit(int id)
        {
            var utente = await _context.UtenteBalnearios.FindAsync(id);
            if (utente == null)
                return NotFound();

            LoadGeneros(utente.GeneroId);
            LoadSeguros(utente.SeguroSaudeId);

            return View(utente);
        }

        // =========================
        // EDIT POST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UtenteBalneario utente)
        {
            if (id != utente.UtenteBalnearioId)
                return NotFound();

            if (utente.GeneroId <= 0)
            {
                ModelState.AddModelError("GeneroId", "Selecione um género válido.");
            }

            if (!ModelState.IsValid)
            {
                LoadGeneros(utente.GeneroId);
                LoadSeguros(utente.SeguroSaudeId);
                return View(utente);
            }

            var utenteDb = await _context.UtenteBalnearios
                .FirstOrDefaultAsync(u => u.UtenteBalnearioId == id);

            if (utenteDb == null)
                return NotFound();

            utenteDb.Nome = utente.Nome;
            utenteDb.DataNascimento = utente.DataNascimento;
            utenteDb.GeneroId = utente.GeneroId;
            utenteDb.NIF = utente.NIF;
            utenteDb.Contacto = utente.Contacto;
            utenteDb.Morada = utente.Morada;

            utenteDb.HistoricoClinico = utente.HistoricoClinico;
            utenteDb.IndicacoesTerapeuticas = utente.IndicacoesTerapeuticas;
            utenteDb.ContraIndicacoes = utente.ContraIndicacoes;
            utenteDb.TerapeutaResponsavel = utente.TerapeutaResponsavel;
            utenteDb.SeguroSaudeId = utente.SeguroSaudeId;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id });
        }

        // =========================
        // ATIVAR / DESATIVAR
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleAtivo(int id)
        {
            var utente = await _context.UtenteBalnearios.FindAsync(id);
            if (utente == null)
                return NotFound();

            utente.Ativo = !utente.Ativo;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id });
        }

        // =========================
        // HELPERS
        // =========================
        private void LoadGeneros(int? generoSelecionado = null)
        {
            ViewBag.Generos = new SelectList(
                _context.Generos.OrderBy(g => g.NomeGenero),
                "GeneroId",
                "NomeGenero",
                generoSelecionado
            );
        }

        private void LoadSeguros(int? selecionado = null)
        {
            ViewBag.Seguros = new SelectList(
                _context.SegurosSaude.OrderBy(s => s.Nome),
                "SeguroSaudeId",
                "Nome",
                selecionado
            );
        }
    }
}