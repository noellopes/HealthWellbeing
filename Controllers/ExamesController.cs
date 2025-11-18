using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellBeing.Models;
using HealthWellbeing.Data;

namespace HealthWellbeing.Controllers
{
    public class ExamesController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ExamesController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Exames
        public async Task<IActionResult> Index()
        {
            var healthWellbeingDbContext = _context.Exames.Include(e => e.ExameTipo).Include(e => e.MaterialEquipamentoAssociado).Include(e => e.ProfissionalExecutante).Include(e => e.SalaDeExame).Include(e => e.Utente);
            return View(await healthWellbeingDbContext.ToListAsync());
        }

        // GET: Exames/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exame = await _context.Exames
                .Include(e => e.ExameTipo)
                .Include(e => e.MaterialEquipamentoAssociado)
                .Include(e => e.ProfissionalExecutante)
                .Include(e => e.SalaDeExame)
                .Include(e => e.Utente)
                .FirstOrDefaultAsync(m => m.ExameId == id);
            if (exame == null)
            {
                return NotFound();
            }

            return View(exame);
        }

        // GET: Exames/Create
        public IActionResult Create()
        {
            ViewData["ExameTipoId"] = new SelectList(_context.ExameTipo, "ExameTipoId", "Nome");
            ViewData["MaterialEquipamentoAssociadoId"] = new SelectList(_context.MaterialEquipamentoAssociado, "MaterialEquipamentoAssociadoId", "EstadoComponente");
            ViewData["ProfissionalExecutanteId"] = new SelectList(_context.ProfissionalExecutante, "ProfissionalExecutanteId", "Email");
            ViewData["SalaDeExameId"] = new SelectList(_context.SalaDeExame, "SalaId", "TipoSala");
            ViewData["UtenteId"] = new SelectList(_context.Utentes, "UtenteId", "Nif");
            return View();
        }

        // POST: Exames/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExameId,DataHoraMarcacao,Estado,Notas,UtenteId,ExameTipoId,MedicoId,SalaDeExameId,ProfissionalExecutanteId,MaterialEquipamentoAssociadoId")] Exame exame)
        {
            if (ModelState.IsValid)
            {
                _context.Add(exame);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ExameTipoId"] = new SelectList(_context.ExameTipo, "ExameTipoId", "Nome", exame.ExameTipoId);
            ViewData["MaterialEquipamentoAssociadoId"] = new SelectList(_context.MaterialEquipamentoAssociado, "MaterialEquipamentoAssociadoId", "EstadoComponente", exame.MaterialEquipamentoAssociadoId);
            ViewData["ProfissionalExecutanteId"] = new SelectList(_context.ProfissionalExecutante, "ProfissionalExecutanteId", "Email", exame.ProfissionalExecutanteId);
            ViewData["SalaDeExameId"] = new SelectList(_context.SalaDeExame, "SalaId", "TipoSala", exame.SalaDeExameId);
            ViewData["UtenteId"] = new SelectList(_context.Utentes, "UtenteId", "Nif", exame.UtenteId);
            return View(exame);
        }

        // GET: Exames/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exame = await _context.Exames.FindAsync(id);
            if (exame == null)
            {
                return NotFound();
            }
            ViewData["ExameTipoId"] = new SelectList(_context.ExameTipo, "ExameTipoId", "Nome", exame.ExameTipoId);
            ViewData["MaterialEquipamentoAssociadoId"] = new SelectList(_context.MaterialEquipamentoAssociado, "MaterialEquipamentoAssociadoId", "EstadoComponente", exame.MaterialEquipamentoAssociadoId);
            ViewData["ProfissionalExecutanteId"] = new SelectList(_context.ProfissionalExecutante, "ProfissionalExecutanteId", "Email", exame.ProfissionalExecutanteId);
            ViewData["SalaDeExameId"] = new SelectList(_context.SalaDeExame, "SalaId", "TipoSala", exame.SalaDeExameId);
            ViewData["UtenteId"] = new SelectList(_context.Utentes, "UtenteId", "Nif", exame.UtenteId);
            return View(exame);
        }

        // POST: Exames/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ExameId,DataHoraMarcacao,Estado,Notas,UtenteId,ExameTipoId,MedicoId,SalaDeExameId,ProfissionalExecutanteId,MaterialEquipamentoAssociadoId")] Exame exame)
        {
            if (id != exame.ExameId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(exame);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExameExists(exame.ExameId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ExameTipoId"] = new SelectList(_context.ExameTipo, "ExameTipoId", "Nome", exame.ExameTipoId);
            ViewData["MaterialEquipamentoAssociadoId"] = new SelectList(_context.MaterialEquipamentoAssociado, "MaterialEquipamentoAssociadoId", "EstadoComponente", exame.MaterialEquipamentoAssociadoId);
            ViewData["ProfissionalExecutanteId"] = new SelectList(_context.ProfissionalExecutante, "ProfissionalExecutanteId", "Email", exame.ProfissionalExecutanteId);
            ViewData["SalaDeExameId"] = new SelectList(_context.SalaDeExame, "SalaId", "TipoSala", exame.SalaDeExameId);
            ViewData["UtenteId"] = new SelectList(_context.Utentes, "UtenteId", "Nif", exame.UtenteId);
            return View(exame);
        }

        // GET: Exames/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exame = await _context.Exames
                .Include(e => e.ExameTipo)
                .Include(e => e.MaterialEquipamentoAssociado)
                .Include(e => e.ProfissionalExecutante)
                .Include(e => e.SalaDeExame)
                .Include(e => e.Utente)
                .FirstOrDefaultAsync(m => m.ExameId == id);
            if (exame == null)
            {
                return NotFound();
            }

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
                _context.Exames.Remove(exame);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExameExists(int id)
        {
            return _context.Exames.Any(e => e.ExameId == id);
        }
    }
}
