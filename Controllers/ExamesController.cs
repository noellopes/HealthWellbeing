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
    [Authorize(Roles = "Admin")]
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
            // Lógica do filtro "Hoje na primeira vez"
            string sessaoKey = "FiltroInicialRealizado";
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(sessaoKey)) && !pesquisaData.HasValue && !Request.Query.ContainsKey("pesquisaData"))
            {
                pesquisaData = DateTime.Today;
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

        // GET: Exames/Create
        public IActionResult Create()
        {
            CarregarViewBagDropdowns();
            return View();
        }

        // POST: Exames/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExameId,DataHoraMarcacao,Estado,Notas,UtenteId,ExameTipoId,MedicoSolicitanteId,ProfissionalExecutanteId,SalaDeExameId,MaterialEquipamentoAssociadoId")] Exame exame)
        {
            bool salaOcupada = await _context.Exames.AnyAsync(e =>
                e.SalaDeExameId == exame.SalaDeExameId &&
                e.DataHoraMarcacao == exame.DataHoraMarcacao);

            if (salaOcupada)
            {
                ModelState.AddModelError("DataHoraMarcacao", "A sala selecionada já está ocupada neste horário.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(exame);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Exame marcado com sucesso!";
                return RedirectToAction(nameof(Index));
            }

            CarregarViewBagDropdowns(exame);
            return View(exame);
        }

        // GET: Exames/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var exame = await _context.Exames.FindAsync(id);
            if (exame == null) return NotFound();
            CarregarViewBagDropdowns(exame);
            return View(exame);
        }

        // POST: Exames/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ExameId,DataHoraMarcacao,Estado,Notas,UtenteId,ExameTipoId,MedicoSolicitanteId,ProfissionalExecutanteId,SalaDeExameId,MaterialEquipamentoAssociadoId")] Exame exame)
        {
            if (id != exame.ExameId) return NotFound();

            bool salaOcupadaOutro = await _context.Exames.AnyAsync(e =>
                e.SalaDeExameId == exame.SalaDeExameId &&
                e.DataHoraMarcacao == exame.DataHoraMarcacao &&
                e.ExameId != id);

            if (salaOcupadaOutro)
            {
                ModelState.AddModelError("DataHoraMarcacao", "A sala selecionada já está ocupada neste horário.");
            }

            if (ModelState.IsValid)
            {
                _context.Update(exame);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Exame atualizado com sucesso!";
                return RedirectToAction(nameof(Index));
            }

            CarregarViewBagDropdowns(exame);
            return View(exame);
        }

        // GET: Exames/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var exame = await _context.Exames
                .Include(e => e.ExameTipo).Include(e => e.Utente)
                .FirstOrDefaultAsync(m => m.ExameId == id);
            if (exame == null) return NotFound();
            return View(exame);
        }

        // POST: Exames/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exame = await _context.Exames.FindAsync(id);
            if (exame != null)
            {
                if (exame.Estado == "Realizado")
                {
                    TempData["ErrorMessage"] = "Não é possível apagar um exame já realizado.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Exames.Remove(exame);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Exame eliminado com sucesso!";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Exames/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var exame = await _context.Exames.Include(e => e.Utente).Include(e => e.ExameTipo).Include(e => e.SalaDeExame).Include(e => e.MedicoSolicitante).Include(e => e.ProfissionalExecutante).Include(e => e.MaterialEquipamentoAssociado).FirstOrDefaultAsync(m => m.ExameId == id);
            if (exame == null) return NotFound();
            return View(exame);
        }

        // --- MÉTODO AJAX CORRIGIDO ---
        [HttpGet]
        public async Task<JsonResult> GetMateriaisPorTipo(int id)
        {
            try
            {
                // 1. Tenta buscar materiais específicos na tabela de ligação 'ExameTipoRecursos'
                // CORREÇÃO AQUI: Usar a propriedade '.Recurso' conforme o teu Model
                var materiaisEspecificos = await _context.ExameTipoRecursos
                    .Where(x => x.ExameTipoId == id)
                    .Include(x => x.Recurso) // <--- O teu model chama-lhe 'Recurso'
                    .Select(x => new
                    {
                        // value = ID do material, text = Nome do material
                        value = x.MaterialEquipamentoAssociadoId,
                        text = x.Recurso.NomeEquipamento // <--- Aceder via 'Recurso'
                    })
                    .ToListAsync();

                // Se encontrou, retorna
                if (materiaisEspecificos.Any())
                {
                    return Json(new { isFallback = false, data = materiaisEspecificos });
                }
            }
            catch
            {
                // Ignora erros (tabela vazia, etc) e usa fallback
            }

            // 2. FALLBACK: Retorna TODOS os materiais
            var todosMateriais = await _context.MaterialEquipamentoAssociado
                .Select(x => new
                {
                    value = x.MaterialEquipamentoAssociadoId,
                    text = x.NomeEquipamento
                })
                .ToListAsync();

            return Json(new { isFallback = true, data = todosMateriais });
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