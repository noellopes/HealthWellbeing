using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers
{
    public class HistoricoAtividadesController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public HistoricoAtividadesController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: HistoricoAtividades
        public async Task<IActionResult> Index()
        {
            var healthWellbeingDbContext = _context.HistoricoAtividades.Include(h => h.Exercicio).Include(h => h.PlanoExercicios);
            return View(await healthWellbeingDbContext.ToListAsync());
        }

        // GET: HistoricoAtividades/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var historicoAtividade = await _context.HistoricoAtividades
                .Include(h => h.Exercicio)
                .Include(h => h.PlanoExercicios)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (historicoAtividade == null)
            {
                return NotFound();
            }

            return View(historicoAtividade);
        }

        // GET: HistoricoAtividades/Create
        public IActionResult Create()
        {
            ViewData["ExercicioId"] = new SelectList(_context.Exercicio, "ExercicioId", "Descricao");
            ViewData["PlanoExerciciosId"] = new SelectList(_context.PlanoExercicios, "PlanoExerciciosId", "PlanoExerciciosId");
            return View();
        }

        // POST: HistoricoAtividades/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UtenteGrupo7Id,ExercicioId,PlanoExerciciosId,DataHora")] HistoricoAtividade historicoAtividade)
        {
            if (ModelState.IsValid)
            {
                _context.Add(historicoAtividade);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ExercicioId"] = new SelectList(_context.Exercicio, "ExercicioId", "Descricao", historicoAtividade.ExercicioId);
            ViewData["PlanoExerciciosId"] = new SelectList(_context.PlanoExercicios, "PlanoExerciciosId", "PlanoExerciciosId", historicoAtividade.PlanoExerciciosId);
            return View(historicoAtividade);
        }

        // GET: HistoricoAtividades/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var historicoAtividade = await _context.HistoricoAtividades.FindAsync(id);
            if (historicoAtividade == null)
            {
                return NotFound();
            }
            ViewData["ExercicioId"] = new SelectList(_context.Exercicio, "ExercicioId", "Descricao", historicoAtividade.ExercicioId);
            ViewData["PlanoExerciciosId"] = new SelectList(_context.PlanoExercicios, "PlanoExerciciosId", "PlanoExerciciosId", historicoAtividade.PlanoExerciciosId);
            return View(historicoAtividade);
        }

        // POST: HistoricoAtividades/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UtenteGrupo7Id,ExercicioId,PlanoExerciciosId,DataHora")] HistoricoAtividade historicoAtividade)
        {
            if (id != historicoAtividade.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(historicoAtividade);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HistoricoAtividadeExists(historicoAtividade.Id))
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
            ViewData["ExercicioId"] = new SelectList(_context.Exercicio, "ExercicioId", "Descricao", historicoAtividade.ExercicioId);
            ViewData["PlanoExerciciosId"] = new SelectList(_context.PlanoExercicios, "PlanoExerciciosId", "PlanoExerciciosId", historicoAtividade.PlanoExerciciosId);
            return View(historicoAtividade);
        }

        // GET: HistoricoAtividades/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var historicoAtividade = await _context.HistoricoAtividades
                .Include(h => h.Exercicio)
                .Include(h => h.PlanoExercicios)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (historicoAtividade == null)
            {
                return NotFound();
            }

            return View(historicoAtividade);
        }

        // POST: HistoricoAtividades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var historicoAtividade = await _context.HistoricoAtividades.FindAsync(id);
            if (historicoAtividade != null)
            {
                _context.HistoricoAtividades.Remove(historicoAtividade);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HistoricoAtividadeExists(int id)
        {
            return _context.HistoricoAtividades.Any(e => e.Id == id);
        }
    }
}
