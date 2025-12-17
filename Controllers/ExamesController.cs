using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellBeing.Models;
using HealthWellbeing.Data;
using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace HealthWellbeing.Controllers
{
    [Authorize(Roles = "Admin")] // Só Admins entram
    public class ExamesController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ExamesController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Exames
        public async Task<IActionResult> Index(int pagina = 1, string pesquisaUtente = "", DateTime? pesquisaData = null)
        {
            // --- LÓGICA DE FILTRO "SÓ NA PRIMEIRA VEZ" ---
            // Usamos a sessão para marcar que o utilizador já visitou esta página nesta sessão.
            string sessaoKey = "FiltroInicialRealizado";

            // Verifica: Se a sessão está vazia (nunca veio aqui) E não pediu uma data específica
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(sessaoKey)) && !pesquisaData.HasValue && !Request.Query.ContainsKey("pesquisaData"))
            {
                // Aplica o filtro de HOJE automaticamente
                pesquisaData = DateTime.Today;

                // Marca na sessão que já entrámos. Da próxima vez (F5, voltar, ou limpar), isto é ignorado.
                HttpContext.Session.SetString(sessaoKey, "true");
            }

            ViewBag.PesquisaUtente = pesquisaUtente;
            ViewBag.PesquisaData = pesquisaData;

            var examesQuery = _context.Exames
                .Include(e => e.ExameTipo)
                .Include(e => e.MaterialEquipamentoAssociado)
                .Include(e => e.MedicoSolicitante)
                .Include(e => e.ProfissionalExecutante)
                .Include(e => e.SalaDeExame)
                .Include(e => e.Utente)
                .AsQueryable();

            if (!string.IsNullOrEmpty(pesquisaUtente))
            {
                examesQuery = examesQuery.Where(e => e.Utente.Nome.Contains(pesquisaUtente));
            }

            if (pesquisaData.HasValue)
            {
                examesQuery = examesQuery.Where(e => e.DataHoraMarcacao.Date == pesquisaData.Value.Date);
            }

            int totalExames = await examesQuery.CountAsync();
            var paginationInfo = new PaginationInfo<Exame>(pagina, totalExames, itemsPerPage: 10);

            paginationInfo.Items = await examesQuery
                .OrderByDescending(e => e.DataHoraMarcacao)
                .Skip(paginationInfo.ItemsToSkip)
                .Take(paginationInfo.ItemsPerPage)
                .ToListAsync();

            return View(paginationInfo);
        }

        // --- RESTANTES MÉTODOS (Create, Edit, Delete, Details) MANTÊM-SE IGUAIS ---
        // (Estão omitidos aqui para poupar espaço, mas deves manter o código que já tinhas para eles)

        // GET: Exames/Create
        public IActionResult Create()
        {
            CarregarViewBagDropdowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Exame exame)
        {
            // ... (Código igual ao anterior) ...
            if (ModelState.IsValid) { _context.Add(exame); await _context.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
            CarregarViewBagDropdowns(exame); return View(exame);
        }

        // ... Incluir aqui Edit, Details, Delete e CarregarViewBagDropdowns ...

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var exame = await _context.Exames.FindAsync(id);
            if (exame == null) return NotFound();
            CarregarViewBagDropdowns(exame);
            return View(exame);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Exame exame)
        {
            if (id != exame.ExameId) return NotFound();
            if (ModelState.IsValid) { _context.Update(exame); await _context.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
            CarregarViewBagDropdowns(exame); return View(exame);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var exame = await _context.Exames.Include(e => e.Utente).Include(e => e.ExameTipo).Include(e => e.SalaDeExame).Include(e => e.MedicoSolicitante).Include(e => e.ProfissionalExecutante).Include(e => e.MaterialEquipamentoAssociado).FirstOrDefaultAsync(m => m.ExameId == id);
            if (exame == null) return NotFound();
            return View(exame);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var exame = await _context.Exames.Include(e => e.Utente).Include(e => e.ExameTipo).FirstOrDefaultAsync(m => m.ExameId == id);
            if (exame == null) return NotFound();
            return View(exame);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exame = await _context.Exames.FindAsync(id);
            if (exame != null) { _context.Exames.Remove(exame); await _context.SaveChangesAsync(); }
            return RedirectToAction(nameof(Index));
        }

        private void CarregarViewBagDropdowns(Exame exame = null)
        {
            ViewData["ExameTipoId"] = new SelectList(_context.ExameTipo, "ExameTipoId", "Nome", exame?.ExameTipoId);
            ViewData["MaterialEquipamentoAssociadoId"] = new SelectList(_context.MaterialEquipamentoAssociado, "MaterialEquipamentoAssociadoId", "NomeEquipamento", exame?.MaterialEquipamentoAssociadoId);
            ViewData["MedicoSolicitanteId"] = new SelectList(_context.Medicos, "Id", "Nome", exame?.MedicoSolicitanteId);
            ViewData["ProfissionalExecutanteId"] = new SelectList(_context.ProfissionalExecutante, "ProfissionalExecutanteId", "Nome", exame?.ProfissionalExecutanteId);
            ViewData["SalaDeExameId"] = new SelectList(_context.SalaDeExame, "SalaId", "TipoSala", exame?.SalaDeExameId);
            ViewData["UtenteId"] = new SelectList(_context.Utentes, "UtenteId", "Nome", exame?.UtenteId);
        }
    }
}