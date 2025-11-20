using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellBeing.Models;
using HealthWellbeing.Data;

namespace HealthWellBeing.Controllers
{
    public class ExamesController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ExamesController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // --- FUNÇÃO AUXILIAR PARA PREENCHER O ViewData ---
        private void PopulateViewData(Exame exame = null)
        {
            // 1. ESTADO
            var estadosValidos = new List<string>
            {
                "Pendente", "Realizado", "A Realizar", "Cancelado"
            };
            ViewData["Estado"] = new SelectList(estadosValidos, exame?.Estado);

            // 2. CHAVES ESTRANGEIRAS
            ViewData["UtenteId"] = new SelectList(_context.Utentes, "UtenteId", "Nif", exame?.UtenteId);

            // --- CORREÇÃO AQUI: Mudado de "Especialidade" para "Nome" ---
            // O ExameTipo tem uma propriedade 'Nome', é essa que queremos ver na lista.
            ViewData["ExameTipoId"] = new SelectList(_context.ExameTipo, "ExameTipoId", "Nome", exame?.ExameTipoId);

            ViewData["ProfissionalExecutanteId"] = new SelectList(_context.ProfissionalExecutante, "ProfissionalExecutanteId", "Nome", exame?.ProfissionalExecutanteId);
            ViewData["MaterialEquipamentoAssociadoId"] = new SelectList(_context.MaterialEquipamentoAssociado, "MaterialEquipamentoAssociadoId", "EstadoComponente", exame?.MaterialEquipamentoAssociadoId);
            ViewData["SalaDeExameId"] = new SelectList(_context.SalaDeExame, "SalaId", "TipoSala", exame?.SalaDeExameId);
        }

        // --- MÉTODOS CRUD ---

        // GET: Exames/Index
        public async Task<IActionResult> Index()
        {
            var healthWellBeingDbContext = _context.Exames.Include(e => e.ExameTipo).Include(e => e.MaterialEquipamentoAssociado).Include(e => e.ProfissionalExecutante).Include(e => e.SalaDeExame).Include(e => e.Utente);
            return View(await healthWellBeingDbContext.ToListAsync());
        }

        // GET: Exames/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var exame = await _context.Exames
                .Include(e => e.ExameTipo).Include(e => e.MaterialEquipamentoAssociado)
                .Include(e => e.ProfissionalExecutante).Include(e => e.SalaDeExame).Include(e => e.Utente)
                .FirstOrDefaultAsync(m => m.ExameId == id);
            if (exame == null) return NotFound();
            return View(exame);
        }

        // GET: Exames/Create
        public IActionResult Create()
        {
            PopulateViewData();
            return View();
        }

        // POST: Exames/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExameId,DataHoraMarcacao,Estado,Notas,UtenteId,ExameTipoId,MedicoId,SalaDeExameId,ProfissionalExecutanteId,MaterialEquipamentoAssociadoId")] Exame exame)
        {
            if (ModelState.IsValid)
            {
                // Incluir a Especialidade para poder mostrar o nome correto na mensagem de sucesso
                // Precisamos carregar o ExameTipo e incluir a sua Especialidade (se existir essa relação)
                // Se ExameTipo tiver apenas o campo 'Nome', usamos 'Nome'.

                var exameTipo = await _context.ExameTipo
                    .Include(et => et.Especialidade) // Tenta incluir se existir navegação
                    .FirstOrDefaultAsync(et => et.ExameTipoId == exame.ExameTipoId);

                _context.Add(exame);
                await _context.SaveChangesAsync();

                // Tenta obter o nome da especialidade, ou o nome do exame tipo
                string nomeParaMensagem = "N/A";
                if (exameTipo != null)
                {
                    // Prioridade: Especialidade.Nome -> ExameTipo.Nome -> "N/A"
                    if (exameTipo.Especialidade != null) nomeParaMensagem = exameTipo.Especialidade.Nome;
                    else if (!string.IsNullOrEmpty(exameTipo.Nome)) nomeParaMensagem = exameTipo.Nome;
                }

                TempData["SuccessMessage"] = $"O exame '{nomeParaMensagem}', Data '{exame.DataHoraMarcacao:dd/MM/yyyy HH:mm}', foi Criado com Sucesso!";

                return RedirectToAction(nameof(Index));
            }

            PopulateViewData(exame);
            return View(exame);
        }

        // GET: Exames/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var exame = await _context.Exames.FindAsync(id);
            if (exame == null) return NotFound();

            PopulateViewData(exame);
            return View(exame);
        }

        // POST: Exames/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ExameId,DataHoraMarcacao,Estado,Notas,UtenteId,ExameTipoId,MedicoId,SalaDeExameId,ProfissionalExecutanteId,MaterialEquipamentoAssociadoId")] Exame exame)
        {
            if (id != exame.ExameId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(exame);
                    await _context.SaveChangesAsync();

                    var exameTipo = await _context.ExameTipo
                        .Include(et => et.Especialidade)
                        .FirstOrDefaultAsync(et => et.ExameTipoId == exame.ExameTipoId);

                    string nomeParaMensagem = "N/A";
                    if (exameTipo != null)
                    {
                        if (exameTipo.Especialidade != null) nomeParaMensagem = exameTipo.Especialidade.Nome;
                        else if (!string.IsNullOrEmpty(exameTipo.Nome)) nomeParaMensagem = exameTipo.Nome;
                    }

                    TempData["SuccessMessage"] = $"O exame '{nomeParaMensagem}', Data '{exame.DataHoraMarcacao:dd/MM/yyyy HH:mm}', foi Editado com Sucesso!";

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExameExists(exame.ExameId)) return NotFound();
                    else throw;
                }
            }

            PopulateViewData(exame);
            return View(exame);
        }

        // GET: Exames/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var exame = await _context.Exames
                .Include(e => e.ExameTipo).Include(e => e.MaterialEquipamentoAssociado)
                .Include(e => e.ProfissionalExecutante).Include(e => e.SalaDeExame).Include(e => e.Utente)
                .FirstOrDefaultAsync(m => m.ExameId == id);
            if (exame == null) return NotFound();
            return View(exame);
        }

        // POST: Exames/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exame = await _context.Exames
                .Include(e => e.ExameTipo)
                    .ThenInclude(et => et.Especialidade)
                .FirstOrDefaultAsync(m => m.ExameId == id);

            if (exame != null)
            {
                string nomeParaMensagem = "N/A";
                if (exame.ExameTipo != null)
                {
                    if (exame.ExameTipo.Especialidade != null) nomeParaMensagem = exame.ExameTipo.Especialidade.Nome;
                    else if (!string.IsNullOrEmpty(exame.ExameTipo.Nome)) nomeParaMensagem = exame.ExameTipo.Nome;
                }

                var dataExame = exame.DataHoraMarcacao.ToString("dd/MM/yyyy HH:mm");

                _context.Exames.Remove(exame);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"O exame '{nomeParaMensagem}', Data '{dataExame}', foi Eliminado com Sucesso!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ExameExists(int id)
        {
            return _context.Exames.Any(e => e.ExameId == id);
        }
    }
}