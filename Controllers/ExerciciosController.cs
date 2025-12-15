using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static HealthWellbeing.Data.SeedData;

namespace HealthWellbeing.Controllers
{
    [Authorize]
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
            string searchProblemaSaude = "",
            // --- NOVOS PARÂMETROS DE PESQUISA ---
            string searchTipoExercicio = "",
            string searchObjetivo = "")
        {
            // 1. Consulta base com todos os Includes (necessários para mostrar os nomes na tabela)
            var query = _context.Exercicio
                .Include(e => e.ExercicioGeneros).ThenInclude(eg => eg.Genero)
                .Include(e => e.ExercicioGrupoMusculares).ThenInclude(gm => gm.GrupoMuscular)
                .Include(e => e.ExercicioEquipamentos).ThenInclude(eq => eq.Equipamento)
                .Include(e => e.Contraindicacoes).ThenInclude(cp => cp.ProblemaSaude)
                .Include(e => e.ExercicioTipoExercicios).ThenInclude(et => et.TipoExercicio) // Include Tipos
                .Include(e => e.ExercicioObjetivos).ThenInclude(eb => eb.ObjetivoFisico)     // Include Objetivos
                .AsQueryable();

            // 2. Filtros de Texto Simples
            if (!string.IsNullOrEmpty(searchNome))
                query = query.Where(e => e.ExercicioNome.Contains(searchNome));

            if (!string.IsNullOrEmpty(searchDescricao))
                query = query.Where(e => e.Descricao.Contains(searchDescricao));

            // 3. Filtros de Relação (Já existentes)
            if (!string.IsNullOrEmpty(searchGenero))
                query = query.Where(e => e.ExercicioGeneros.Any(eg => eg.Genero.NomeGenero.Contains(searchGenero)));

            if (!string.IsNullOrEmpty(searchGrupoMuscular))
                query = query.Where(e => e.ExercicioGrupoMusculares.Any(gm => gm.GrupoMuscular.GrupoMuscularNome.Contains(searchGrupoMuscular)));

            if (!string.IsNullOrEmpty(searchEquipamento))
                query = query.Where(e => e.ExercicioEquipamentos.Any(eq => eq.Equipamento.NomeEquipamento.Contains(searchEquipamento)));

            if (!string.IsNullOrEmpty(searchProblemaSaude))
                query = query.Where(e => e.Contraindicacoes.Any(cp => cp.ProblemaSaude.ProblemaNome.Contains(searchProblemaSaude)));

            // --- 4. NOVOS FILTROS DE PESQUISA ---

            // Filtrar por Tipo de Exercício (Navega pela tabela de junção ExercicioTipoExercicios)
            if (!string.IsNullOrEmpty(searchTipoExercicio))
            {
                query = query.Where(e => e.ExercicioTipoExercicios
                    .Any(et => et.TipoExercicio.NomeTipoExercicios.Contains(searchTipoExercicio)));
            }

            // Filtrar por Objetivo Físico (Navega pela tabela de junção ExercicioObjetivos)
            if (!string.IsNullOrEmpty(searchObjetivo))
            {
                query = query.Where(e => e.ExercicioObjetivos
                    .Any(eo => eo.ObjetivoFisico.NomeObjetivo.Contains(searchObjetivo)));
            }

            // 5. Paginação
            int totalItems = await query.CountAsync();
            var pagination = new PaginationInfo<Exercicio>(page, totalItems);

            pagination.Items = await query
                .OrderBy(e => e.ExercicioNome)
                .Skip(pagination.ItemsToSkip)
                .Take(pagination.ItemsPerPage)
                .ToListAsync();

            // 6. Manter os valores na View (ViewBag) para os inputs não ficarem vazios após pesquisar
            ViewBag.SearchNome = searchNome;
            ViewBag.SearchDescricao = searchDescricao;
            ViewBag.SearchGenero = searchGenero;
            ViewBag.SearchGrupoMuscular = searchGrupoMuscular;
            ViewBag.SearchEquipamento = searchEquipamento;
            ViewBag.SearchProblemaSaude = searchProblemaSaude;

            // Novas ViewBags
            ViewBag.SearchTipoExercicio = searchTipoExercicio;
            ViewBag.SearchObjetivo = searchObjetivo;

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
                // --- CORRIGIDO: ExercicioObjetivos ---
                .Include(e => e.ExercicioTipoExercicios).ThenInclude(et => et.TipoExercicio)
                .Include(e => e.ExercicioObjetivos).ThenInclude(eb => eb.ObjetivoFisico)
                .FirstOrDefaultAsync(m => m.ExercicioId == id);

            if (exercicio == null) return NotFound();

            return View(exercicio);
        }

        // GET: Exercicios/Create
        [Authorize(Roles = SeedData.Roles.Administrador + "," + SeedData.Roles.Profissional)]
        public IActionResult Create()
        {
            ViewBag.Generos = new SelectList(_context.Genero, "GeneroId", "NomeGenero");
            ViewBag.GruposMusculares = new SelectList(_context.GrupoMuscular, "GrupoMuscularId", "GrupoMuscularNome");
            ViewBag.Equipamentos = new SelectList(_context.Equipamento, "EquipamentoId", "NomeEquipamento");
            ViewBag.Contraindicacoes = new SelectList(_context.ProblemaSaude, "ProblemaSaudeId", "ProblemaNome");

            // Listas para Checkboxes
            ViewBag.TiposExercicios = _context.TipoExercicio.ToList();
            ViewBag.ObjetivosFisicos = _context.ObjetivoFisico.ToList();

            return View();
        }

        // POST: Exercicios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SeedData.Roles.Administrador + "," + SeedData.Roles.Profissional)]
        public async Task<IActionResult> Create(
            [Bind("ExercicioId,ExercicioNome,Descricao,Duracao,Intencidade,CaloriasGastas,Instrucoes,Repeticoes,Series")] Exercicio exercicio,
            int[] generosIds,
            int[] GrupoMuscularIds,
            int[] equipamentosIds,
            int[] problemasSaudeIds,
            int[] objetivosFisicosIds, // Checkboxes Objetivos
            int[] tipoExerciciosIds)   // Checkboxes Tipos
        {
            ModelState.Remove("ExercicioGeneros");
            ModelState.Remove("ExercicioGrupoMusculares");
            ModelState.Remove("ExercicioEquipamentos");
            ModelState.Remove("Contraindicacoes");

            // --- CORRIGIDO: Remover validação da propriedade correta ---
            ModelState.Remove("ExercicioObjetivos");
            ModelState.Remove("ExercicioTipoExercicios");

            ModelState.Remove("TipoExercicioId");
            ModelState.Remove("TipoExercicio");

            bool existeExercicio = await _context.Exercicio
                .AnyAsync(e => e.ExercicioNome.ToLower() == exercicio.ExercicioNome.ToLower());

            if (existeExercicio)
            {
                ModelState.AddModelError("ExercicioNome", "Já existe um exercício com este nome.");
            }

            if (ModelState.IsValid)
            {
                // 1. Gêneros
                if (generosIds != null)
                {
                    exercicio.ExercicioGeneros = new List<ExercicioGenero>();
                    foreach (var id in generosIds)
                        exercicio.ExercicioGeneros.Add(new ExercicioGenero { GeneroId = id });
                }

                // 2. Grupos Musculares
                if (GrupoMuscularIds != null)
                {
                    exercicio.ExercicioGrupoMusculares = new List<ExercicioGrupoMuscular>();
                    foreach (var id in GrupoMuscularIds)
                        exercicio.ExercicioGrupoMusculares.Add(new ExercicioGrupoMuscular { GrupoMuscularId = id });
                }

                // 3. Equipamentos
                if (equipamentosIds != null)
                {
                    exercicio.ExercicioEquipamentos = new List<ExercicioEquipamento>();
                    foreach (var id in equipamentosIds)
                        exercicio.ExercicioEquipamentos.Add(new ExercicioEquipamento { EquipamentoId = id });
                }

                // 4. Contraindicações
                if (problemasSaudeIds != null)
                {
                    exercicio.Contraindicacoes = new List<ExercicioProblemaSaude>();
                    foreach (var idProblema in problemasSaudeIds)
                        exercicio.Contraindicacoes.Add(new ExercicioProblemaSaude { ProblemaSaudeId = idProblema });
                }

                // 5. Associar Objetivos Físicos (CORRIGIDO: ExercicioObjetivos)
                if (objetivosFisicosIds != null)
                {
                    exercicio.ExercicioObjetivos = new List<ExercicioObjetivoFisico>();
                    foreach (var idObj in objetivosFisicosIds)
                        exercicio.ExercicioObjetivos.Add(new ExercicioObjetivoFisico { ObjetivoFisicoId = idObj });
                }

                // 6. Associar Tipos de Exercício
                if (tipoExerciciosIds != null)
                {
                    exercicio.ExercicioTipoExercicios = new List<ExercicioTipoExercicio>();
                    foreach (var idTipo in tipoExerciciosIds)
                        exercicio.ExercicioTipoExercicios.Add(new ExercicioTipoExercicio { TipoExercicioId = idTipo });
                }

                _context.Add(exercicio);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Details), new { id = exercicio.ExercicioId, SuccessMessage = "Exercicio criado com sucesso" });
            }

            // Recarregar listas
            ViewBag.Generos = new SelectList(_context.Genero, "GeneroId", "NomeGenero");
            ViewBag.GruposMusculares = new SelectList(_context.GrupoMuscular, "GrupoMuscularId", "GrupoMuscularNome");
            ViewBag.Equipamentos = new SelectList(_context.Equipamento, "EquipamentoId", "NomeEquipamento");
            ViewBag.Contraindicacoes = new SelectList(_context.ProblemaSaude, "ProblemaSaudeId", "ProblemaNome");
            ViewBag.TiposExercicios = _context.TipoExercicio.ToList();
            ViewBag.ObjetivosFisicos = _context.ObjetivoFisico.ToList();

            return View(exercicio);
        }

        // GET: Exercicios/Edit/5
        [Authorize(Roles = SeedData.Roles.Administrador + "," + SeedData.Roles.Profissional)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var exercicio = await _context.Exercicio
                .Include(e => e.ExercicioGeneros)
                .Include(e => e.ExercicioGrupoMusculares)
                .Include(e => e.ExercicioEquipamentos)
                .Include(e => e.Contraindicacoes)
                // --- CORRIGIDO: ExercicioObjetivos ---
                .Include(e => e.ExercicioObjetivos)
                .Include(e => e.ExercicioTipoExercicios)
                .FirstOrDefaultAsync(e => e.ExercicioId == id);

            if (exercicio == null) return NotFound();

            // Carregar listas completas
            ViewBag.Generos = _context.Genero.ToList();
            ViewBag.GruposMusculares = _context.GrupoMuscular.ToList();
            ViewBag.Equipamentos = _context.Equipamento.ToList();
            ViewBag.ProblemasSaude = _context.ProblemaSaude.ToList();
            ViewBag.TiposExercicios = _context.TipoExercicio.ToList();
            ViewBag.ObjetivosFisicos = _context.ObjetivoFisico.ToList();

            // Identificar selecionados
            ViewBag.GenerosSelecionados = exercicio.ExercicioGeneros?.Select(g => g.GeneroId).ToList() ?? new List<int>();
            ViewBag.GruposSelecionados = exercicio.ExercicioGrupoMusculares?.Select(g => g.GrupoMuscularId).ToList() ?? new List<int>();
            ViewBag.EquipamentosSelecionados = exercicio.ExercicioEquipamentos?.Select(e => e.EquipamentoId).ToList() ?? new List<int>();
            ViewBag.ProblemasSelecionados = exercicio.Contraindicacoes?.Select(c => c.ProblemaSaudeId).ToList() ?? new List<int>();

            // --- CORRIGIDO: ExercicioObjetivos ---
            ViewBag.ObjetivosSelecionados = exercicio.ExercicioObjetivos?.Select(o => o.ObjetivoFisicoId).ToList() ?? new List<int>();
            ViewBag.TiposSelecionados = exercicio.ExercicioTipoExercicios?.Select(t => t.TipoExercicioId).ToList() ?? new List<int>();

            return View(exercicio);
        }

        // POST: Exercicios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SeedData.Roles.Administrador + "," + SeedData.Roles.Profissional)]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("ExercicioId,ExercicioNome,Descricao,Duracao,Intencidade,CaloriasGastas,Instrucoes,Repeticoes,Series")] Exercicio exercicio,
            int[] generosIds,
            int[] gruposMuscularesIds,
            int[] equipamentosIds,
            int[] problemasSaudeIds,
            int[] objetivosFisicosIds,
            int[] tipoExerciciosIds)
        {
            if (id != exercicio.ExercicioId) return NotFound();

            // Remover validações de navegação
            ModelState.Remove("ExercicioGeneros");
            ModelState.Remove("ExercicioGrupoMusculares");
            ModelState.Remove("ExercicioEquipamentos");
            ModelState.Remove("Contraindicacoes");
            ModelState.Remove("ExercicioObjetivos");
            ModelState.Remove("ExercicioTipoExercicios");
            ModelState.Remove("TipoExercicioId");
            ModelState.Remove("TipoExercicio");

            // Validação de nome duplicado (excluindo o próprio ID se existisse)
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
                    // Tentar carregar o exercício da BD
                    var exercicioExistente = await _context.Exercicio
                        .Include(e => e.ExercicioGeneros)
                        .Include(e => e.ExercicioGrupoMusculares)
                        .Include(e => e.ExercicioEquipamentos)
                        .Include(e => e.Contraindicacoes)
                        .Include(e => e.ExercicioObjetivos)
                        .Include(e => e.ExercicioTipoExercicios)
                        .FirstOrDefaultAsync(e => e.ExercicioId == id);

                    // --- AQUI ESTAVA O PROBLEMA DO 404 ---
                    // Se for null, significa que foi apagado. Vamos mostrar a página de recuperação.
                    if (exercicioExistente == null)
                    {
                        // Carregar os dados para a View de Recuperação
                        ViewData["GenerosIds"] = generosIds ?? new int[0];
                        ViewData["GruposMuscularesIds"] = gruposMuscularesIds ?? new int[0];
                        ViewData["EquipamentosIds"] = equipamentosIds ?? new int[0];
                        ViewData["ProblemasSaudeIds"] = problemasSaudeIds ?? new int[0];
                        ViewData["ObjetivosFisicosIds"] = objetivosFisicosIds ?? new int[0];
                        ViewData["TipoExerciciosIds"] = tipoExerciciosIds ?? new int[0];

                        return View("InvalidExercicio", exercicio);
                    }

                    // Se o exercício existe, atualizamos os dados
                    _context.Entry(exercicioExistente).CurrentValues.SetValues(exercicio);

                    // --- ATUALIZAR LISTAS ---

                    // 1. Géneros
                    exercicioExistente.ExercicioGeneros.Clear();
                    if (generosIds != null)
                        foreach (var idG in generosIds)
                            exercicioExistente.ExercicioGeneros.Add(new ExercicioGenero { ExercicioId = id, GeneroId = idG });

                    // 2. Grupos Musculares
                    exercicioExistente.ExercicioGrupoMusculares.Clear();
                    if (gruposMuscularesIds != null)
                        foreach (var idGM in gruposMuscularesIds)
                            exercicioExistente.ExercicioGrupoMusculares.Add(new ExercicioGrupoMuscular { ExercicioId = id, GrupoMuscularId = idGM });

                    // 3. Equipamentos
                    exercicioExistente.ExercicioEquipamentos.Clear();
                    if (equipamentosIds != null)
                        foreach (var idEq in equipamentosIds)
                            exercicioExistente.ExercicioEquipamentos.Add(new ExercicioEquipamento { ExercicioId = id, EquipamentoId = idEq });

                    // 4. Contraindicações
                    exercicioExistente.Contraindicacoes.Clear();
                    if (problemasSaudeIds != null)
                        foreach (var idProb in problemasSaudeIds)
                            exercicioExistente.Contraindicacoes.Add(new ExercicioProblemaSaude { ExercicioId = id, ProblemaSaudeId = idProb });

                    // 5. Objetivos Físicos
                    exercicioExistente.ExercicioObjetivos.Clear();
                    if (objetivosFisicosIds != null)
                        foreach (var idObj in objetivosFisicosIds)
                            exercicioExistente.ExercicioObjetivos.Add(new ExercicioObjetivoFisico { ExercicioId = id, ObjetivoFisicoId = idObj });

                    // 6. Tipos de Exercício
                    exercicioExistente.ExercicioTipoExercicios.Clear();
                    if (tipoExerciciosIds != null)
                        foreach (var idTipo in tipoExerciciosIds)
                            exercicioExistente.ExercicioTipoExercicios.Add(new ExercicioTipoExercicio { ExercicioId = id, TipoExercicioId = idTipo });

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details), new { id = exercicio.ExercicioId, SuccessMessage = "Exercicio editado com sucesso" });
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Backup caso o erro venha do SaveChanges (menos provável com a lógica acima, mas seguro manter)
                    if (!ExercicioExists(exercicio.ExercicioId))
                    {
                        ViewData["GenerosIds"] = generosIds ?? new int[0];
                        ViewData["GruposMuscularesIds"] = gruposMuscularesIds ?? new int[0];
                        ViewData["EquipamentosIds"] = equipamentosIds ?? new int[0];
                        ViewData["ProblemasSaudeIds"] = problemasSaudeIds ?? new int[0];
                        ViewData["ObjetivosFisicosIds"] = objetivosFisicosIds ?? new int[0];
                        ViewData["TipoExerciciosIds"] = tipoExerciciosIds ?? new int[0];

                        return View("InvalidExercicio", exercicio);
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // Recarregar listas em caso de erro de validação
            ViewBag.Generos = _context.Genero.ToList();
            ViewBag.GruposMusculares = _context.GrupoMuscular.ToList();
            ViewBag.Equipamentos = _context.Equipamento.ToList();
            ViewBag.ProblemasSaude = _context.ProblemaSaude.ToList();
            ViewBag.TiposExercicios = _context.TipoExercicio.ToList();
            ViewBag.ObjetivosFisicos = _context.ObjetivoFisico.ToList();

            return View(exercicio);
        }

        // GET: Exercicios/Delete/5
        [Authorize(Roles = SeedData.Roles.Administrador + "," + SeedData.Roles.Profissional)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var exercicio = await _context.Exercicio
                .Include(e => e.ExercicioGeneros).ThenInclude(eg => eg.Genero)
                .Include(e => e.ExercicioGrupoMusculares).ThenInclude(gm => gm.GrupoMuscular)
                .Include(e => e.ExercicioEquipamentos).ThenInclude(eq => eq.Equipamento)
                .Include(e => e.Contraindicacoes).ThenInclude(cp => cp.ProblemaSaude)
                // --- CORRIGIDO: ExercicioObjetivos ---
                .Include(e => e.ExercicioTipoExercicios).ThenInclude(et => et.TipoExercicio)
                .Include(e => e.ExercicioObjetivos).ThenInclude(eb => eb.ObjetivoFisico)
                .FirstOrDefaultAsync(m => m.ExercicioId == id);

            if (exercicio == null)
            {
                TempData["SuccessMessage"] = "Este exercicio já foi eliminado";
                return RedirectToAction(nameof(Index));
            }

            return View(exercicio);
        }

        // POST: Exercicios/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SeedData.Roles.Administrador + "," + SeedData.Roles.Profissional)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exercicio = await _context.Exercicio
                .Include(e => e.ExercicioGeneros)
                .Include(e => e.ExercicioGrupoMusculares)
                .Include(e => e.ExercicioEquipamentos)
                .Include(e => e.Contraindicacoes)
                // --- CORRIGIDO: ExercicioObjetivos ---
                .Include(e => e.ExercicioObjetivos)
                .Include(e => e.ExercicioTipoExercicios)
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