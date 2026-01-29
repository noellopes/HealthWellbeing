using HealthWellbeing.Data;
using HealthWellbeing.Models;
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

        public UtenteBalnearioController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // INDEX COM PAGINAÇÃO, PESQUISA, FILTRO
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

            // Pesquisa
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u => u.Nome.Contains(search));
            }

            // Filtro Ativos
            if (ativos.HasValue)
            {
                query = query.Where(u => u.Ativo == ativos.Value);
            }

            // Ordenação
            query = sort switch
            {
                "nome_desc" => query.OrderByDescending(u => u.Nome),
                "data" => query.OrderBy(u => u.DataInscricao),
                "data_desc" => query.OrderByDescending(u => u.DataInscricao),
                _ => query.OrderBy(u => u.Nome)
            };

            // Paginação
            var totalItems = await query.CountAsync();
            ViewBag.TotalAtivos = await query.CountAsync(u => u.Ativo);
            ViewBag.TotalInativos = await query.CountAsync(u => !u.Ativo);

            var utentes = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // ViewBags
            ViewBag.Search = search;
            ViewBag.Ativos = ativos;
            ViewBag.Sort = sort;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.TotalItems = totalItems;
            ViewBag.PageSize = pageSize;


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
                .FirstOrDefaultAsync(u => u.UtenteBalnearioId == id);


            if (utente == null)
                return NotFound();

            return View(utente);
        }

        // =========================
        // CREATE
        // =========================
        public IActionResult Create()
        {
            LoadGeneros();
            LoadSeguros();
            return View();
        }

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

            TempData["Success"] = "Utente criado com sucesso!";
            return RedirectToAction(nameof(Details), new { id = utente.UtenteBalnearioId });
        }

        // =========================
        // EDIT
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UtenteBalneario utente)
        {
            if (id != utente.UtenteBalnearioId)
                return NotFound();

            if (utente.GeneroId == null || utente.GeneroId <= 0)
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

            // =========================
            // UTENTE
            // =========================

            utenteDb.Nome = utente.Nome;
            utenteDb.DataNascimento = utente.DataNascimento;
            utenteDb.GeneroId = utente.GeneroId;
            utenteDb.NIF = utente.NIF;
            utenteDb.Contacto = utente.Contacto;
            utenteDb.Morada = utente.Morada;

            // =========================
            // DADOS MÉDICOS
            // =========================


            utenteDb.HistoricoClinico = utente.HistoricoClinico;
            utenteDb.IndicacoesTerapeuticas = utente.IndicacoesTerapeuticas;
            utenteDb.ContraIndicacoes = utente.ContraIndicacoes;
            utenteDb.TerapeutaResponsavel = utente.TerapeutaResponsavel;
            utenteDb.SeguroSaudeId = utente.SeguroSaudeId;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Utente atualizado com sucesso!";
            return RedirectToAction(nameof(Details), new { id });
        }



        // =========================
        // ATIVAR/DESATIVAR UTENTE
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

            TempData["Success"] = utente.Ativo
                ? "Utente ativado com sucesso!"
                : "Utente desativado com sucesso!";

            return RedirectToAction(nameof(Details), new { id });
        }




        // =========================
        // HELPERS
        // =========================
        private void LoadGeneros(int? generoSelecionado = null)
        {
            ViewBag.Generos = new SelectList(
                _context.Generos.OrderBy(g => g.Nome),
                "GeneroId",
                "Nome",
                generoSelecionado
            );
        }

        private void LoadSeguros(int? selected = null)
        {
            ViewBag.Seguros = new SelectList(
                _context.SegurosSaude.OrderBy(s => s.Nome),
                "SeguroSaudeId",
                "Nome",
                selected
            );
        }


    }
}
