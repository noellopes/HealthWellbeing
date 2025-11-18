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
            // CORREÇÃO: Include com ThenInclude conforme PDF
            var query = _context.Exercicio
                .Include(e => e.ExercicioGeneros)
                    .ThenInclude(eg => eg.Genero)
                .Include(e => e.ExercicioGrupoMusculares)
                    .ThenInclude(gm => gm.GrupoMuscular)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchNome))
                query = query.Where(e => e.ExercicioNome.Contains(searchNome));

            if (!string.IsNullOrEmpty(searchDescricao))
                query = query.Where(e => e.Descricao.Contains(searchDescricao));

            // CORREÇÃO: Filtrar através da tabela de junção
            if (!string.IsNullOrEmpty(searchGenero))
                query = query.Where(e => e.ExercicioGeneros.Any(eg => eg.Genero.NomeGenero.Contains(searchGenero)));

            if (!string.IsNullOrEmpty(searchGrupoMuscular))
                query = query.Where(e => e.ExercicioGrupoMusculares.Any(gm => gm.GrupoMuscular.GrupoMuscularNome.Contains(searchGrupoMuscular)));

            int totalItems = await query.CountAsync();

            var pagination = new PaginationInfoExercicios<Exercicio>(page, totalItems);

            pagination.Items = await query
                .OrderBy(e => e.ExercicioNome)
                .Skip(pagination.ItemsToSkip)
                .Take(pagination.ItemsPerPage)
                .ToListAsync();

            ViewBag.SearchNome = searchNome;
            ViewBag.SearchDescricao = searchDescricao;
            ViewBag.SearchGenero = searchGenero;
            ViewBag.SearchGrupoMuscular = searchGrupoMuscular;

            return View(pagination);
        }

        // GET: Exercicios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            // CORREÇÃO: Carregamento dos dados relacionados via tabelas intermédias
            var exercicio = await _context.Exercicio
                .Include(e => e.ExercicioGeneros).ThenInclude(eg => eg.Genero)
                .Include(e => e.ExercicioGrupoMusculares).ThenInclude(egm => egm.GrupoMuscular)
                .FirstOrDefaultAsync(m => m.ExercicioId == id);

            if (exercicio == null) return NotFound();

            return View(exercicio);
        }

        // GET: Exercicios/Create
        public IActionResult Create()
        {
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
            // Removemos as validações das coleções para evitar erros de ModelState
            ModelState.Remove("ExercicioGeneros");
            ModelState.Remove("ExercicioGrupoMusculares");

            if (ModelState.IsValid)
            {
                // CORREÇÃO: Criar explicitamente as entradas na tabela de junção
                if (generosIds != null)
                {
                    exercicio.ExercicioGeneros = new List<ExercicioGenero>();
                    foreach (var id in generosIds)
                    {
                        exercicio.ExercicioGeneros.Add(new ExercicioGenero { GeneroId = id });
                    }
                }

                if (GrupoMuscularIds != null)
                {
                    exercicio.ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular>();
                    foreach (var id in GrupoMuscularIds)
                    {
                        exercicio.ExercicioGrupoMusculares.Add(new ExercicioGrupoMuscular { GrupoMuscularId = id });
                    }
                }

                _context.Add(exercicio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Generos = new SelectList(_context.Genero, "GeneroId", "NomeGenero");
            ViewBag.GruposMusculares = new SelectList(_context.GrupoMuscular, "GrupoMuscularId", "GrupoMuscularNome");
            return View(exercicio);
        }

        // GET: Exercicios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            // Carregar o exercício com as relações existentes
            var exercicio = await _context.Exercicio
                .Include(e => e.ExercicioGeneros)
                .Include(e => e.ExercicioGrupoMusculares)
                .FirstOrDefaultAsync(e => e.ExercicioId == id);

            if (exercicio == null) return NotFound();

            ViewBag.Generos = _context.Genero.ToList();
            ViewBag.GruposMusculares = _context.GrupoMuscular.ToList();

            // Enviar IDs selecionados para a View (Select)
            ViewBag.GenerosSelecionados = exercicio.ExercicioGeneros?.Select(g => g.GeneroId).ToList() ?? new List<int>();
            ViewBag.GruposSelecionados = exercicio.ExercicioGrupoMusculares?.Select(g => g.GrupoMuscularId).ToList() ?? new List<int>();

            return View(exercicio);
        }

        // POST: Exercicios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
        int id,
        [Bind("ExercicioId,ExercicioNome,Descricao,Duracao,Intencidade,CaloriasGastas,Instrucoes,EquipamentoNecessario,Repeticoes,Series")]
        Exercicio exercicio,
        int[] generosIds,
        int[] gruposMuscularesIds)
        {
            if (id != exercicio.ExercicioId) return NotFound();

            ModelState.Remove("ExercicioGeneros");
            ModelState.Remove("ExercicioGrupoMusculares");

            if (ModelState.IsValid)
            {
                // Carregamos o exercício da BD para atualizar as relações
                var exercicioExistente = await _context.Exercicio
                    .Include(e => e.ExercicioGeneros)
                    .Include(e => e.ExercicioGrupoMusculares)
                    .FirstOrDefaultAsync(e => e.ExercicioId == id);

                if (exercicioExistente == null) return NotFound();

                // Atualizar valores simples
                _context.Entry(exercicioExistente).CurrentValues.SetValues(exercicio);

                // --- ATUALIZAR GÉNEROS ---
                // 1. Limpar os atuais
                exercicioExistente.ExercicioGeneros.Clear();
                // 2. Adicionar os novos
                if (generosIds != null)
                {
                    foreach (var gId in generosIds)
                    {
                        exercicioExistente.ExercicioGeneros.Add(new ExercicioGenero { ExercicioId = id, GeneroId = gId });
                    }
                }

                // --- ATUALIZAR GRUPOS MUSCULARES ---
                exercicioExistente.ExercicioGrupoMusculares.Clear();
                if (gruposMuscularesIds != null)
                {
                    foreach (var gmId in gruposMuscularesIds)
                    {
                        exercicioExistente.ExercicioGrupoMusculares.Add(new ExercicioGrupoMuscular { ExercicioId = id, GrupoMuscularId = gmId });
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Generos = _context.Genero.ToList();
            ViewBag.GruposMusculares = _context.GrupoMuscular.ToList();
            return View(exercicio);
        }

        // GET: Exercicios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var exercicio = await _context.Exercicio
                .Include(e => e.ExercicioGeneros).ThenInclude(eg => eg.Genero)
                .Include(e => e.ExercicioGrupoMusculares).ThenInclude(gm => gm.GrupoMuscular)
                .FirstOrDefaultAsync(m => m.ExercicioId == id);

            if (exercicio == null) return NotFound();

            return View(exercicio);
        }

        // POST: Exercicios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exercicio = await _context.Exercicio
                .Include(e => e.ExercicioGeneros)
                .Include(e => e.ExercicioGrupoMusculares)
                .FirstOrDefaultAsync(e => e.ExercicioId == id);

            if (exercicio != null)
            {
                // Opcional: Se o DeleteBehavior for Cascade (ver PDF página 9), isto acontece automaticamente,
                // mas limpar manualmente é mais seguro se a configuração mudar.
                _context.Exercicio.Remove(exercicio);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}