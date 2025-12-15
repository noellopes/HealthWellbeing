using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace HealthWellbeing.Controllers
{
    public class PlanoExerciciosController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public PlanoExerciciosController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: PlanoExercicios
        public async Task<IActionResult> Index()
        {
            var healthWellbeingDbContext = _context.PlanoExercicios
                .Include(o => o.Exercicios)
                .Include(p => p.UtenteGrupo7);
            return View(await healthWellbeingDbContext.ToListAsync());
        }

        // GET: PlanoExercicios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var planoExercicios = await _context.PlanoExercicios
                .Include(o => o.Exercicios)
                .Include(p => p.UtenteGrupo7)
                .FirstOrDefaultAsync(m => m.PlanoExerciciosId == id);
            if (planoExercicios == null)
            {
                return NotFound();
            }

            return View(planoExercicios);
        }

        // GET: PlanoExercicios/Create
        public IActionResult Create()
        {
            ViewData["UtenteGrupo7Id"] = new SelectList(_context.UtenteGrupo7, "UtenteGrupo7Id", "UtenteGrupo7Id");

            ViewBag.Exercicio = _context.Exercicio
        .Select(e => new SelectListItem
        {
            Value = e.ExercicioId.ToString(),
            Text = e.ExercicioNome
        })
        .ToList();

            return View();
        }

        // POST: PlanoExercicios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
    PlanoExercicios planoExercicios,
    int[] exerciciosSelecionados)
        {
            if (exerciciosSelecionados == null || !exerciciosSelecionados.Any())
            {
                ModelState.AddModelError("", "Deve selecionar pelo menos um exercício.");
            }

            if (ModelState.IsValid)
            {
                planoExercicios.Exercicios = _context.Exercicio
                    .Where(e => exerciciosSelecionados.Contains(e.ExercicioId))
                    .ToList();

                _context.Add(planoExercicios);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            ViewData["UtenteGrupo7Id"] = new SelectList(_context.UtenteGrupo7, "UtenteGrupo7Id", "UtenteGrupo7Id", planoExercicios.UtenteGrupo7Id);

            ViewBag.Exercicio = _context.Exercicio
                .Select(e => new SelectListItem
                {
                    Value = e.ExercicioId.ToString(),
                    Text = e.ExercicioNome
                }).ToList();

            return View(planoExercicios);
        }

        // GET: PlanoExercicios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var planoExercicios = await _context.PlanoExercicios.Include(p => p.Exercicios).FirstOrDefaultAsync(p => p.PlanoExerciciosId == id);
            if (planoExercicios == null)
            {
                return NotFound();
            }
            ViewData["UtenteGrupo7Id"] = new SelectList(_context.UtenteGrupo7, "UtenteGrupo7Id", "UtenteGrupo7Id", planoExercicios.UtenteGrupo7Id);

            ViewBag.Exercicio = _context.Exercicio
                .Select(e => new SelectListItem
                {
                    Value = e.ExercicioId.ToString(),
                    Text = e.ExercicioNome
                }).ToList();

            return View(planoExercicios);
        }

        // POST: PlanoExercicios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PlanoExerciciosId,UtenteGrupo7Id")] PlanoExercicios planoExercicios)
        {
            if (id != planoExercicios.PlanoExerciciosId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(planoExercicios);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlanoExerciciosExists(planoExercicios.PlanoExerciciosId))
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
            ViewData["UtenteGrupo7Id"] = new SelectList(_context.UtenteGrupo7, "UtenteGrupo7Id", "UtenteGrupo7Id", planoExercicios.UtenteGrupo7Id);
            return View(planoExercicios);
        }

        // GET: PlanoExercicios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var planoExercicios = await _context.PlanoExercicios
                .Include(p => p.UtenteGrupo7)
                .FirstOrDefaultAsync(m => m.PlanoExerciciosId == id);
            if (planoExercicios == null)
            {
                return NotFound();
            }

            return View(planoExercicios);
        }

        // POST: PlanoExercicios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var planoExercicios = await _context.PlanoExercicios.FindAsync(id);
            if (planoExercicios != null)
            {
                _context.PlanoExercicios.Remove(planoExercicios);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlanoExerciciosExists(int id)
        {
            return _context.PlanoExercicios.Any(e => e.PlanoExerciciosId == id);
        }
    }
}
