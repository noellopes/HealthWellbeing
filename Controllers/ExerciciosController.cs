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
            string searchGrupoMuscular = "",
            string searchEquipamento = "",
            string searchProblemaSaude = "")
        {
            // Consulta base com todos os Includes necessários para as "Badges" da View
            var query = _context.Exercicio
                .Include(e => e.ExercicioGeneros).ThenInclude(eg => eg.Genero)
                .Include(e => e.ExercicioGrupoMusculares).ThenInclude(gm => gm.GrupoMuscular)
                .Include(e => e.ExercicioEquipamentos).ThenInclude(eq => eq.Equipamento)
                .Include(e => e.Contraindicacoes).ThenInclude(cp => cp.ProblemaSaude)
                .AsQueryable();

            // Filtros de Texto Simples
            if (!string.IsNullOrEmpty(searchNome))
                query = query.Where(e => e.ExercicioNome.Contains(searchNome));

            if (!string.IsNullOrEmpty(searchDescricao))
                query = query.Where(e => e.Descricao.Contains(searchDescricao));

            // Filtros de Relação (Many-to-Many)
            if (!string.IsNullOrEmpty(searchGenero))
                query = query.Where(e => e.ExercicioGeneros.Any(eg => eg.Genero.NomeGenero.Contains(searchGenero)));

            if (!string.IsNullOrEmpty(searchGrupoMuscular))
                query = query.Where(e => e.ExercicioGrupoMusculares.Any(gm => gm.GrupoMuscular.GrupoMuscularNome.Contains(searchGrupoMuscular)));

            // CORREÇÃO: Filtro de Equipamento corrigido (estava a filtrar por descrição)
            if (!string.IsNullOrEmpty(searchEquipamento))
                query = query.Where(e => e.ExercicioEquipamentos.Any(eq => eq.Equipamento.NomeEquipamento.Contains(searchEquipamento)));

            if (!string.IsNullOrEmpty(searchProblemaSaude))
            {
                query = query.Where(e => e.Contraindicacoes
                    .Any(cp => cp.ProblemaSaude.ProblemaNome.Contains(searchProblemaSaude)));
            }

            // Paginação
            int totalItems = await query.CountAsync();
            var pagination = new PaginationInfo<Exercicio>(page, totalItems);

            pagination.Items = await query
                .OrderBy(e => e.ExercicioNome)
                .Skip(pagination.ItemsToSkip)
                .Take(pagination.ItemsPerPage)
                .ToListAsync();

            // Manter os valores de pesquisa na View
            ViewBag.SearchNome = searchNome;
            ViewBag.SearchDescricao = searchDescricao;
            ViewBag.SearchGenero = searchGenero;
            ViewBag.SearchGrupoMuscular = searchGrupoMuscular;
            ViewBag.SearchEquipamento = searchEquipamento;
            ViewBag.SearchProblemaSaude = searchProblemaSaude;

            return View(pagination);
        }

        // GET: Exercicios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var exercicio = await _context.Exercicio
                .Include(e => e.ExercicioGeneros).ThenInclude(eg => eg.Genero)
                .Include(e => e.ExercicioGrupoMusculares).ThenInclude(egm => egm.GrupoMuscular)
                .Include(e => e.ExercicioEquipamentos).ThenInclude(eeq => eeq.Equipamento)
                .Include(e => e.Contraindicacoes).ThenInclude(cp => cp.ProblemaSaude)
                .FirstOrDefaultAsync(m => m.ExercicioId == id);

            if (exercicio == null) return NotFound();

            return View(exercicio);
        }

        // GET: Exercicios/Create
        public IActionResult Create()
        {
            // Carregar listas para as checkboxes
            ViewBag.Generos = new SelectList(_context.Genero, "GeneroId", "NomeGenero");
            ViewBag.GruposMusculares = new SelectList(_context.GrupoMuscular, "GrupoMuscularId", "GrupoMuscularNome");
            ViewBag.Equipamentos = new SelectList(_context.Equipamento, "EquipamentoId", "NomeEquipamento");
            ViewBag.Contraindicacoes = new SelectList(_context.ProblemaSaude, "ProblemaSaudeId", "ProblemaNome");
            return View();
        }

        // POST: Exercicios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("ExercicioId,ExercicioNome,Descricao,Duracao,Intencidade,CaloriasGastas,Instrucoes,Repeticoes,Series")] Exercicio exercicio,
            int[] generosIds,
            int[] GrupoMuscularIds,
            int[] equipamentosIds,
            int[] problemasSaudeIds)

        {
            // Remover validação das propriedades de navegação
            ModelState.Remove("ExercicioGeneros");
            ModelState.Remove("ExercicioGrupoMusculares");
            ModelState.Remove("ExercicioEquipamentos");
            ModelState.Remove("Contraindicacoes");

            // Verificar se já existe um exercício com o mesmo nome (ignorar maiúsculas/minúsculas)
            bool existeExercicio = await _context.Exercicio
            .AnyAsync(e => e.ExercicioNome.ToLower() == exercicio.ExercicioNome.ToLower());

            if (existeExercicio)
            {
                // Adiciona um erro específico ao campo "ExercicioNome"
                ModelState.AddModelError("ExercicioNome", "Já existe um exercício com este nome.");
            }

            if (ModelState.IsValid)
            {
                // 1. Associar Gêneros
                if (generosIds != null)
                {
                    exercicio.ExercicioGeneros = new List<ExercicioGenero>();
                    foreach (var id in generosIds)
                    {
                        exercicio.ExercicioGeneros.Add(new ExercicioGenero { GeneroId = id });
                    }
                }

                // 2. Associar Grupos Musculares
                if (GrupoMuscularIds != null)
                {
                    exercicio.ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular>();
                    foreach (var id in GrupoMuscularIds)
                    {
                        exercicio.ExercicioGrupoMusculares.Add(new ExercicioGrupoMuscular { GrupoMuscularId = id });
                    }
                }

                // 3. Associar Equipamentos (NOVO)
                if (equipamentosIds != null)
                {
                    exercicio.ExercicioEquipamentos = new List<ExercicioEquipamento>();
                    foreach (var id in equipamentosIds)
                    {
                        exercicio.ExercicioEquipamentos.Add(new ExercicioEquipamento { EquipamentoId = id });
                    }
                }

                // 4. Associar Contraindicações
                if (problemasSaudeIds != null)
                {
                    exercicio.Contraindicacoes = new List<ExercicioProblemaSaude>();
                    foreach (var idProblema in problemasSaudeIds)
                    {
                        exercicio.Contraindicacoes.Add(new ExercicioProblemaSaude
                        {
                            ProblemaSaudeId = idProblema
                        });
                    }
                }

                _context.Add(exercicio);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Details), new { id = exercicio.ExercicioId, SuccessMessage = "Exercicio criado com sucesso" });
            }

            // Recarregar listas em caso de erro
            ViewBag.Generos = new SelectList(_context.Genero, "GeneroId", "NomeGenero");
            ViewBag.GruposMusculares = new SelectList(_context.GrupoMuscular, "GrupoMuscularId", "GrupoMuscularNome");
            ViewBag.Equipamentos = new SelectList(_context.Equipamento, "EquipamentoId", "NomeEquipamento");
            ViewBag.ProblemasSaude = new SelectList(_context.ProblemaSaude, "ProblemaSaudeId", "ProblemaNome");
            ViewBag.Contraindicacoes = new SelectList(_context.ProblemaSaude, "ProblemaSaudeId", "ProblemaNome");

            return View(exercicio);
        }

        // GET: Exercicios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var exercicio = await _context.Exercicio
                .Include(e => e.ExercicioGeneros)
                .Include(e => e.ExercicioGrupoMusculares)
                .Include(e => e.ExercicioEquipamentos)
                .Include(e => e.Contraindicacoes)
                .FirstOrDefaultAsync(e => e.ExercicioId == id);

            if (exercicio == null) return NotFound();

            // Carregar todas as opções disponíveis
            ViewBag.Generos = _context.Genero.ToList();
            ViewBag.GruposMusculares = _context.GrupoMuscular.ToList();
            ViewBag.Equipamentos = _context.Equipamento.ToList();
            ViewBag.ProblemasSaude = _context.ProblemaSaude.ToList();

            // Identificar quais estão selecionados (para marcar as checkboxes)
            ViewBag.GenerosSelecionados = exercicio.ExercicioGeneros?.Select(g => g.GeneroId).ToList() ?? new List<int>();
            ViewBag.GruposSelecionados = exercicio.ExercicioGrupoMusculares?.Select(g => g.GrupoMuscularId).ToList() ?? new List<int>();
            ViewBag.EquipamentosSelecionados = exercicio.ExercicioEquipamentos?.Select(e => e.EquipamentoId).ToList() ?? new List<int>();
            ViewBag.ProblemasSelecionados = exercicio.Contraindicacoes?.Select(c => c.ProblemaSaudeId).ToList() ?? new List<int>();

            return View(exercicio);
        }

        // POST: Exercicios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("ExercicioId,ExercicioNome,Descricao,Duracao,Intencidade,CaloriasGastas,Instrucoes,Repeticoes,Series")] Exercicio exercicio,
            int[] generosIds,
            int[] gruposMuscularesIds,
            int[] equipamentosIds,
            int[] problemasSaudeIds)
        {
            if (id != exercicio.ExercicioId) return NotFound();

            ModelState.Remove("ExercicioGeneros");
            ModelState.Remove("ExercicioGrupoMusculares");
            ModelState.Remove("ExercicioEquipamentos");
            ModelState.Remove("Contraindicacoes");

            // Verificar se já existe outro exercício com o mesmo nome (ignorar maiúsculas/minúsculas)
            bool existeOutro = await _context.Exercicio
            .AnyAsync(e => e.ExercicioNome.ToLower() == exercicio.ExercicioNome.ToLower() && e.ExercicioId != id);

            if (existeOutro)
            {
                ModelState.AddModelError("ExercicioNome", "Já existe outro exercício com este nome.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var exercicioExistente = await _context.Exercicio
                        .Include(e => e.ExercicioGeneros)
                        .Include(e => e.ExercicioGrupoMusculares)
                        .Include(e => e.ExercicioEquipamentos)
                        .Include(e => e.Contraindicacoes)
                        .FirstOrDefaultAsync(e => e.ExercicioId == id);


                    if (exercicioExistente == null)
                    {
                        // Passar IDs para a View de recuperação
                        ViewData["GenerosIds"] = generosIds ?? new int[0];
                        ViewData["GruposMuscularesIds"] = gruposMuscularesIds ?? new int[0];
                        ViewData["EquipamentosIds"] = equipamentosIds ?? new int[0];
                        ViewData["ProblemasSaudeIds"] = problemasSaudeIds ?? new int[0];
                        return View("InvalidExercicio", exercicio);
                    }

                    // 1. Atualizar dados simples
                    _context.Entry(exercicioExistente).CurrentValues.SetValues(exercicio);

                    // 2. Atualizar Géneros
                    exercicioExistente.ExercicioGeneros.Clear();
                    if (generosIds != null)
                    {
                        foreach (var gId in generosIds)
                            exercicioExistente.ExercicioGeneros.Add(new ExercicioGenero { ExercicioId = id, GeneroId = gId });
                    }

                    // 3. Atualizar Grupos Musculares
                    exercicioExistente.ExercicioGrupoMusculares.Clear();
                    if (gruposMuscularesIds != null)
                    {
                        foreach (var gmId in gruposMuscularesIds)
                            exercicioExistente.ExercicioGrupoMusculares.Add(new ExercicioGrupoMuscular { ExercicioId = id, GrupoMuscularId = gmId });
                    }

                    // 4. Atualizar Equipamentos
                    exercicioExistente.ExercicioEquipamentos.Clear();
                    if (equipamentosIds != null)
                    {
                        foreach (var eqId in equipamentosIds)
                            exercicioExistente.ExercicioEquipamentos.Add(new ExercicioEquipamento { ExercicioId = id, EquipamentoId = eqId });
                    }

                    // 5. Atualizar Contraindicações
                    exercicioExistente.Contraindicacoes.Clear();
                    if (problemasSaudeIds != null)
                    {
                        foreach (var ps in problemasSaudeIds)
                        {
                            exercicioExistente.Contraindicacoes.Add(new ExercicioProblemaSaude{ExercicioId = id, ProblemaSaudeId = ps});
                        }
                    }


                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details), new { id = exercicio.ExercicioId, SuccessMessage = "Exercicio editado com sucesso" });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExercicioExists(exercicio.ExercicioId))
                    {
                        ViewData["GenerosIds"] = generosIds ?? new int[0];
                        ViewData["GruposMuscularesIds"] = gruposMuscularesIds ?? new int[0];
                        ViewData["EquipamentosIds"] = equipamentosIds ?? new int[0];
                        ViewData["ProblemasSaudeIds"] = problemasSaudeIds ?? new int[0];
                        return View("ExercicioDeleted", exercicio);
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewBag.Generos = _context.Genero.ToList();
            ViewBag.GruposMusculares = _context.GrupoMuscular.ToList();
            ViewBag.Equipamentos = _context.Equipamento.ToList();
            ViewBag.ProblemasSaude = _context.ProblemaSaude.ToList();
            return View(exercicio);
        }

        // GET: Exercicios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var exercicio = await _context.Exercicio
                .Include(e => e.ExercicioGeneros).ThenInclude(eg => eg.Genero)
                .Include(e => e.ExercicioGrupoMusculares).ThenInclude(gm => gm.GrupoMuscular)
                .Include(e => e.ExercicioEquipamentos).ThenInclude(eq => eq.Equipamento)
                .Include(e => e.Contraindicacoes).ThenInclude(cp => cp.ProblemaSaude)
                .FirstOrDefaultAsync(m => m.ExercicioId == id);

            if (exercicio == null)
            {
                TempData["SuccessMessage"] = "Este exercicio já foi eliminado";
                return RedirectToAction(nameof(Index));
            }

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
                .Include(e => e.ExercicioEquipamentos)
                .Include(e => e.Contraindicacoes)
                .FirstOrDefaultAsync(e => e.ExercicioId == id);

            if (exercicio != null)
            {
                _context.Exercicio.Remove(exercicio);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Exercicio foi apagado com sucesso";
            }
            else
            {
                TempData["SuccessMessage"] = "Este exercicio já tinha sido eliminado";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ExercicioExists(int id)
        {
            return _context.Exercicio.Any(e => e.ExercicioId == id);
        }
    }
}