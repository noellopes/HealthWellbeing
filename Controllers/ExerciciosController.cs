using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;

namespace HealthWellbeing.Controllers
{
    public class ExerciciosController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ExerciciosController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Exercicios
        public async Task<IActionResult> Index(int page = 1)
        {
            var exerciciosContext = _context.Exercicio.Include(e => e.Genero);

            int numberExercicios = await exerciciosContext.CountAsync();

            var exerciciosInfo = new PaginationInfoExercicios<Exercicio>(page, numberExercicios);

            exerciciosInfo.Items = await exerciciosContext
                .OrderBy(e => e.ExercicioNome)
                .Skip(exerciciosInfo.ItemsToSkip)
                .Take(exerciciosInfo.ItemsPerPage)
                .ToListAsync();

            return View(exerciciosInfo);
        }

        // GET: Exercicios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exercicio = await _context.Exercicio
                .Include(e => e.Genero)
                .Include(e => e.GrupoMuscular)
                .FirstOrDefaultAsync(m => m.ExercicioId == id);

            if (exercicio == null)
            {
                return NotFound();
            }

            return View(exercicio);
        }

        // GET: Exercicios/Create
        public IActionResult Create()
        {
            // Carrega os gêneros para a ViewBag
            ViewBag.Generos = new SelectList(_context.Genero, "GeneroId", "NomeGenero");
            return View();
        }

        // POST: Exercicios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExercicioId,ExercicioNome,Descricao,Duracao,Intencidade,CaloriasGastas,Instrucoes,EquipamentoNecessario,Repeticoes,Series")] Exercicio exercicio, int[] generosIds)
        {
            if (ModelState.IsValid)
            {
                // Adiciona os gêneros selecionados
                if (generosIds != null && generosIds.Length > 0)
                {
                    exercicio.Genero = new List<Genero>();

                    foreach (var generoId in generosIds)
                    {
                        var genero = await _context.Genero.FindAsync(generoId);
                        if (genero != null)
                        {
                            exercicio.Genero.Add(genero);
                        }
                    }
                }

                _context.Add(exercicio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Recarrega os gêneros se houver erro
            ViewBag.Genero = new SelectList(_context.Genero, "GeneroId", "NomeGenero");
            return View(exercicio);
        }

        // GET: Exercicios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exercicio = await _context.Exercicio
                .Include(e => e.Genero)
                .FirstOrDefaultAsync(e => e.ExercicioId == id);

            if (exercicio == null)
            {
                return NotFound();
            }

            // Carrega os gêneros para a ViewBag
            ViewBag.Genero = new SelectList(_context.Genero, "GeneroId", "NomeGenero");

            // Prepara lista de gêneros já selecionados para marcar os checkboxes
            var generosSelecionados = exercicio.Genero?.Select(g => g.GeneroId).ToList();
            ViewBag.GenerosSelecionados = generosSelecionados ?? new List<int>();

            return View(exercicio);
        }

        // POST: Exercicios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ExercicioId,ExercicioNome,Descricao,Duracao,Intencidade,CaloriasGastas,Instrucoes,EquipamentoNecessario,Repeticoes,Series")] Exercicio exercicio, int[] generosIds)
        {
            if (id != exercicio.ExercicioId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Carrega o exercício existente com seus gêneros
                    var exercicioExistente = await _context.Exercicio
                        .Include(e => e.Genero)
                        .FirstOrDefaultAsync(e => e.ExercicioId == id);

                    if (exercicioExistente == null)
                    {
                        return NotFound();
                    }

                    // Atualiza propriedades básicas
                    _context.Entry(exercicioExistente).CurrentValues.SetValues(exercicio);

                    // Limpa gêneros existentes
                    exercicioExistente. Genero?.Clear();

                    // Adiciona novos gêneros selecionados
                    if (generosIds != null && generosIds.Length > 0)
                    {
                        var generosParaAdicionar = await _context.Genero
                            .Where(g => generosIds.Contains(g.GeneroId))
                            .ToListAsync();

                        exercicioExistente.Genero = generosParaAdicionar;
                    }
                    else
                    {
                        exercicioExistente.Genero = new List<Genero>();
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExercicioExists(exercicio.ExercicioId))
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

            // Recarrega os gêneros se houver erro
            ViewBag.Genero = new SelectList(_context.Genero, "GeneroId", "NomeGenero");
            return View(exercicio);
        }

        // GET: Exercicios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exercicio = await _context.Exercicio
                .Include(e => e.Genero)
                .FirstOrDefaultAsync(m => m.ExercicioId == id);

            if (exercicio == null)
            {
                return NotFound();
            }

            return View(exercicio);
        }

        // POST: Exercicios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exercicio = await _context.Exercicio
                .Include(e => e.Genero)
                .FirstOrDefaultAsync(e => e.ExercicioId == id);

            if (exercicio != null)
            {
                // Remove as relações muitos-para-muitos primeiro
                exercicio.Genero?.Clear();

                _context.Exercicio.Remove(exercicio);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExercicioExists(int id)
        {
            return _context.Exercicio.Any(e => e.ExercicioId == id);
        }
    }
}