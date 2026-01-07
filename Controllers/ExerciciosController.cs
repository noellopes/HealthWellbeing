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
        public async Task<IActionResult> Index(
        int page = 1,
        string searchNome = "",
        string searchDescricao = "",
        string searchGenero = "",
        string searchGrupoMuscular = "")
        {
            var query = _context.Exercicios
                .Include(e => e.Genero)        // coleção de Genero
                .Include(e => e.GrupoMuscular) // coleção de GrupoMuscular
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchNome))
                query = query.Where(e => e.ExercicioNome.Contains(searchNome));

            if (!string.IsNullOrEmpty(searchDescricao))
                query = query.Where(e => e.Descricao.Contains(searchDescricao));

            // Pesquisar dentro da coleção de gêneros
            if (!string.IsNullOrEmpty(searchGenero))
                query = query.Where(e => e.Genero.Any(g => g.NomeGenero.Contains(searchGenero)));

            // Pesquisar dentro da coleção de grupos musculares
            if (!string.IsNullOrEmpty(searchGrupoMuscular))
                query = query.Where(e => e.GrupoMuscular.Any(gm => gm.GrupoMuscularNome.Contains(searchGrupoMuscular)));

            int totalItems = await query.CountAsync();

            var pagination = new PaginationInfoExercicios<Exercicio>(page, totalItems);

            pagination.Items = await query
                .OrderBy(e => e.ExercicioNome)
                .Skip(pagination.ItemsToSkip)
                .Take(pagination.ItemsPerPage)
                .ToListAsync();

            // Guardar filtros para a view
            ViewBag.SearchNome = searchNome;
            ViewBag.SearchDescricao = searchDescricao;
            ViewBag.SearchGenero = searchGenero;
            ViewBag.SearchGrupoMuscular = searchGrupoMuscular;

            return View(pagination);
        }


        // GET: Exercicios/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exercicio = await _context.Exercicios
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
            ViewBag.GruposMusculares = new SelectList(_context.GrupoMuscular, "GrupoMuscularId", "GrupoMuscularNome");
            return View();
        }

        // POST: Exercicios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("ExercicioId,ExercicioNome,Descricao,Duracao,Intencidade,CaloriasGastas,Instrucoes,EquipamentoNecessario,Repeticoes,Series")]
            Exercicio exercicio,
            int[] generosIds,
            int[] GrupoMuscularIds)
        {
            if (ModelState.IsValid)
            {
                // --- Adiciona os gêneros selecionados ---
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

                // --- Adiciona os grupos musculares selecionados ---
                if (GrupoMuscularIds != null && GrupoMuscularIds.Length > 0)
                {
                    exercicio.GrupoMuscular = new List<GrupoMuscular>();

                    foreach (var grupoMuscularId in GrupoMuscularIds)
                    {
                        var grupo = await _context.GrupoMuscular.FindAsync(grupoMuscularId);
                        if (grupo != null)
                        {
                            exercicio.GrupoMuscular.Add(grupo);
                        }
                    }
                }

                // --- Salva o exercício ---
                _context.Add(exercicio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // --- Se houver erro, recarrega selects e retorna view ---
            ViewBag.Generos = new SelectList(_context.Genero, "GeneroId", "NomeGenero");
            ViewBag.GruposMusculares = new SelectList(_context.GrupoMuscular, "GrupoMuscularId", "GrupoMuscularNome");

            return View(exercicio);
        }


        // GET: Exercicios/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exercicio = await _context.Exercicios
                .Include(e => e.Genero)
                .Include(e => e.GrupoMuscular)
                .FirstOrDefaultAsync(e => e.ExercicioId == id);

            if (exercicio == null)
            {
                return NotFound();
            }

            // Carrega todos os gêneros e grupos musculares
            ViewBag.Generos = _context.Genero.ToList();
            ViewBag.GruposMusculares = _context.GrupoMuscular.ToList();

            // Prepara listas de selecionados
            ViewBag.GenerosSelecionados = exercicio.Genero?.Select(g => g.GeneroId).ToList() ?? new List<int>();
            ViewBag.GruposSelecionados = exercicio.GrupoMuscular?.Select(g => g.GrupoMuscularId).ToList() ?? new List<int>();

            return View(exercicio);
        }


        // POST: Exercicios/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
        int id,
        [Bind("ExercicioId,ExercicioNome,Descricao,Duracao,Intencidade,CaloriasGastas,Instrucoes,EquipamentoNecessario,Repeticoes,Series")]
        Exercicio exercicio,
        int[] generosIds,
        int[] gruposMuscularesIds)
        {
            if (id != exercicio.ExercicioId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var exercicioExistente = await _context.Exercicios
                    .Include(e => e.Genero)
                    .Include(e => e.GrupoMuscular)
                    .FirstOrDefaultAsync(e => e.ExercicioId == id);

                if (exercicioExistente == null)
                {
                    return NotFound();
                }

                // Atualiza dados básicos
                _context.Entry(exercicioExistente).CurrentValues.SetValues(exercicio);

                // Atualiza gêneros
                exercicioExistente.Genero.Clear();
                if (generosIds != null && generosIds.Length > 0)
                {
                    var generos = await _context.Genero
                        .Where(g => generosIds.Contains(g.GeneroId))
                        .ToListAsync();
                    exercicioExistente.Genero = generos;
                }

                // Atualiza grupos musculares
                exercicioExistente.GrupoMuscular.Clear();
                if (gruposMuscularesIds != null && gruposMuscularesIds.Length > 0)
                {
                    var grupos = await _context.GrupoMuscular
                        .Where(g => gruposMuscularesIds.Contains(g.GrupoMuscularId))
                        .ToListAsync();
                    exercicioExistente.GrupoMuscular = grupos;
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Se der erro, recarrega listas
            ViewBag.Generos = _context.Genero.ToList();
            ViewBag.GruposMusculares = _context.GrupoMuscular.ToList();

            return View(exercicio);
        }


        // GET: Exercicios/Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exercicio = await _context.Exercicios
                .Include(e => e.Genero)
                .Include(e => e.GrupoMuscular)
                .FirstOrDefaultAsync(m => m.ExercicioId == id);

            if (exercicio == null)
            {
                return NotFound();
            }

            return View(exercicio);
        }


        // POST: Exercicios/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exercicio = await _context.Exercicios
                .Include(e => e.Genero)
                .FirstOrDefaultAsync(e => e.ExercicioId == id);

            if (exercicio != null)
            {
                // Remove as relações muitos-para-muitos primeiro
                exercicio.Genero?.Clear();

                _context.Exercicios.Remove(exercicio);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExercicioExists(int id)
        {
            return _context.Exercicios.Any(e => e.ExercicioId == id);
        }
    }
}