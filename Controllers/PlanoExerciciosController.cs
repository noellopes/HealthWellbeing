using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthWellbeing.Controllers
{
    [Authorize]
    public class PlanoExerciciosController : Controller
    {
        private readonly HealthWellbeingDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public PlanoExerciciosController(HealthWellbeingDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: PlanoExercicios
        public async Task<IActionResult> Index(int page = 1, string searchUtente = "")
        {
            var user = await _userManager.GetUserAsync(User);
            var query = _context.PlanoExercicios
                .Include(p => p.UtenteGrupo7)
                .Include(p => p.PlanoExercicioExercicios)
                    .ThenInclude(pe => pe.Exercicio)
                .AsQueryable();

            // --- LÓGICA DE PERMISSÕES ---
            if (User.IsInRole("Administrador"))
            {
                // Admin vê tudo
            }
            else if (User.IsInRole("ProfissionalSaude"))
            {
                // Profissional vê apenas planos dos SEUS utentes
                query = query.Where(p => p.UtenteGrupo7.ProfissionalSaudeId == user.Id);
            }
            else
            {
                // Utente vê apenas os SEUS próprios planos
                query = query.Where(p => p.UtenteGrupo7.UserId == user.Id);
            }

            // --- PESQUISA (Apenas faz sentido se for Staff) ---
            if (!string.IsNullOrEmpty(searchUtente))
            {
                query = query.Where(p => p.UtenteGrupo7.Nome.Contains(searchUtente));
                ViewBag.SearchUtente = searchUtente;
            }

            // --- PAGINAÇÃO ---
            int total = await query.CountAsync();
            var pagination = new PaginationInfo<PlanoExercicios>(page, total);

            if (total > 0)
            {
                pagination.Items = await query
                    .OrderByDescending(p => p.PlanoExerciciosId)
                    .Skip(pagination.ItemsToSkip)
                    .Take(pagination.ItemsPerPage)
                    .ToListAsync();
            }
            else
            {
                pagination.Items = new List<PlanoExercicios>();
            }

            return View(pagination);
        }

        // GET: PlanoExercicios/Details/5
        public async Task<IActionResult> Details(int? id, bool mostrarAviso = false)
        {
            if (id == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);

            var plano = await _context.PlanoExercicios
                .Include(p => p.UtenteGrupo7)
                .Include(p => p.PlanoExercicioExercicios)
                    .ThenInclude(pe => pe.Exercicio)
                        .ThenInclude(e => e.ExercicioEquipamentos)
                            .ThenInclude(eq => eq.Equipamento)
                .FirstOrDefaultAsync(m => m.PlanoExerciciosId == id);

            if (plano == null) return NotFound();

            // --- SEGURANÇA ---
            if (!User.IsInRole("Administrador"))
            {
                if (User.IsInRole("ProfissionalSaude"))
                {
                    // Profissional só vê se o utente for dele
                    if (plano.UtenteGrupo7.ProfissionalSaudeId != user.Id) return Forbid();
                }
                else
                {
                    // Utente só vê se for dele
                    if (plano.UtenteGrupo7.UserId != user.Id) return Forbid();
                }
            }

            // Ordenar exercícios (Não concluidos primeiro)
            if (plano.PlanoExercicioExercicios != null)
            {
                plano.PlanoExercicioExercicios = plano.PlanoExercicioExercicios
                    .OrderBy(x => x.Concluido)
                    .ToList();
            }

            ViewBag.MostrarAvisoReinicio = mostrarAviso;
            return View(plano);
        }

        // POST: AtualizarProgresso
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AtualizarProgresso(int planoId, int exercicioId, bool concluido, double? pesoUsado)
        {
            var user = await _userManager.GetUserAsync(User);

            var itemTreino = await _context.Set<PlanoExercicioExercicio>()
                .Include(pe => pe.PlanoExercicios)
                .ThenInclude(p => p.UtenteGrupo7)
                .FirstOrDefaultAsync(pe => pe.PlanoExerciciosId == planoId && pe.ExercicioId == exercicioId);

            if (itemTreino == null) return NotFound();

            // --- SEGURANÇA ---
            // Utente pode atualizar o seu próprio. Staff pode atualizar os seus atribuídos.
            if (!User.IsInRole("Administrador"))
            {
                if (User.IsInRole("ProfissionalSaude"))
                {
                    if (itemTreino.PlanoExercicios.UtenteGrupo7.ProfissionalSaudeId != user.Id) return Forbid();
                }
                else
                {
                    if (itemTreino.PlanoExercicios.UtenteGrupo7.UserId != user.Id) return Forbid();
                }
            }

            itemTreino.Concluido = concluido;
            itemTreino.PesoUsado = pesoUsado;

            _context.Update(itemTreino);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = planoId });
        }

        // POST: ReiniciarPlano
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReiniciarPlano(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            var plano = await _context.PlanoExercicios
                .Include(p => p.UtenteGrupo7)
                .FirstOrDefaultAsync(p => p.PlanoExerciciosId == id);

            if (plano == null) return NotFound();

            // --- SEGURANÇA ---
            if (!User.IsInRole("Administrador"))
            {
                if (User.IsInRole("ProfissionalSaude"))
                {
                    if (plano.UtenteGrupo7.ProfissionalSaudeId != user.Id) return Forbid();
                }
                else
                {
                    if (plano.UtenteGrupo7.UserId != user.Id) return Forbid();
                }
            }

            var itens = await _context.Set<PlanoExercicioExercicio>()
                .Where(pe => pe.PlanoExerciciosId == id)
                .ToListAsync();

            foreach (var item in itens)
            {
                item.Concluido = false;
            }

            _context.UpdateRange(itens);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = id });
        }

        // GET: Create (Manual)
        [Authorize(Roles = "Administrador,ProfissionalSaude")]
        public async Task<IActionResult> Create()
        {
            // Passamos o user para filtrar a lista de utentes
            var user = await _userManager.GetUserAsync(User);
            await CarregarViewBagsManual(user);
            return View();
        }

        // POST: Create (Manual)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,ProfissionalSaude")]
        public async Task<IActionResult> Create(PlanoExercicios planoExercicios, int[] exerciciosSelecionados)
        {
            var user = await _userManager.GetUserAsync(User);

            // --- SEGURANÇA: Validar se o utente selecionado pertence ao Profissional ---
            if (!User.IsInRole("Administrador"))
            {
                var utenteValido = await _context.UtenteGrupo7
                    .AnyAsync(u => u.UtenteGrupo7Id == planoExercicios.UtenteGrupo7Id && u.ProfissionalSaudeId == user.Id);

                if (!utenteValido)
                {
                    ModelState.AddModelError("UtenteGrupo7Id", "Não pode criar planos para utentes que não lhe estão atribuídos.");
                }
            }

            if (exerciciosSelecionados == null || !exerciciosSelecionados.Any())
            {
                ModelState.AddModelError("", "Deve selecionar pelo menos um exercício.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(planoExercicios);
                await _context.SaveChangesAsync();

                if (exerciciosSelecionados != null)
                {
                    var juncoes = new List<PlanoExercicioExercicio>();
                    foreach (var exId in exerciciosSelecionados)
                    {
                        juncoes.Add(new PlanoExercicioExercicio
                        {
                            PlanoExerciciosId = planoExercicios.PlanoExerciciosId,
                            ExercicioId = exId,
                            Concluido = false,
                            PesoUsado = null
                        });
                    }
                    _context.AddRange(juncoes);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            await CarregarViewBagsManual(user, planoExercicios.UtenteGrupo7Id);
            return View(planoExercicios);
        }

        // GET: CreateAutomatico
        public async Task<IActionResult> CreateAutomatico()
        {
            var user = await _userManager.GetUserAsync(User);
            bool isStaff = await IsStaff(user);

            if (isStaff)
            {
                // Carregar dropdown filtrada
                IQueryable<UtenteGrupo7> queryUtentes = _context.UtenteGrupo7;

                if (!User.IsInRole("Administrador")) // É Profissional
                {
                    queryUtentes = queryUtentes.Where(u => u.ProfissionalSaudeId == user.Id);
                }

                ViewData["UtenteGrupo7Id"] = new SelectList(await queryUtentes.ToListAsync(), "UtenteGrupo7Id", "Nome");
            }
            else
            {
                // Utente a criar para si mesmo
                var utente = await _context.UtenteGrupo7.FirstOrDefaultAsync(u => u.UserId == user.Id);

                if (utente == null)
                {
                    TempData["MensagemErro"] = "Por favor, finalize o seu registo de Utente antes de gerar um plano.";
                    return RedirectToAction("Create", "UtenteGrupo7");
                }

                ViewBag.UtenteFixoId = utente.UtenteGrupo7Id;
                ViewBag.UtenteFixoNome = utente.Nome;
            }

            ViewBag.IsStaff = isStaff;
            return View();
        }

        // POST: CreateAutomatico
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAutomatico(PlanoExercicios planoInput, int quantidadeExercicios)
        {
            if (quantidadeExercicios <= 0) quantidadeExercicios = 5;

            var user = await _userManager.GetUserAsync(User);
            bool isStaff = await IsStaff(user);
            int targetUtenteId;

            if (isStaff)
            {
                targetUtenteId = planoInput.UtenteGrupo7Id;
                // Segurança extra: verificar se o utente pertence ao profissional
                if (!User.IsInRole("Administrador"))
                {
                    bool pertence = await _context.UtenteGrupo7.AnyAsync(u => u.UtenteGrupo7Id == targetUtenteId && u.ProfissionalSaudeId == user.Id);
                    if (!pertence) return Forbid();
                }
            }
            else
            {
                var utenteDb = await _context.UtenteGrupo7.FirstOrDefaultAsync(u => u.UserId == user.Id);
                if (utenteDb == null) return Unauthorized();
                targetUtenteId = utenteDb.UtenteGrupo7Id;
            }

            // Lógica de geração automática (MANTIDA IGUAL)
            var utenteCompleto = await _context.UtenteGrupo7
                .Include(u => u.UtenteProblemasSaude)
                .Include(u => u.ObjetivoFisico)
                .FirstOrDefaultAsync(u => u.UtenteGrupo7Id == targetUtenteId);

            if (utenteCompleto == null) return NotFound("Utente não encontrado.");

            var idsProblemasSaude = utenteCompleto.UtenteProblemasSaude.Select(up => up.ProblemaSaudeId).ToList();

            var exerciciosQuery = _context.Exercicio
                .Include(e => e.Contraindicacoes)
                .Include(e => e.ExercicioObjetivos)
                .AsQueryable();

            if (idsProblemasSaude.Any())
            {
                exerciciosQuery = exerciciosQuery.Where(e =>
                    !e.Contraindicacoes.Any(c => idsProblemasSaude.Contains(c.ProblemaSaudeId)));
            }

            if (utenteCompleto.ObjetivoFisicoId.HasValue)
            {
                exerciciosQuery = exerciciosQuery.Where(e =>
                    e.ExercicioObjetivos.Any(eo => eo.ObjetivoFisicoId == utenteCompleto.ObjetivoFisicoId));
            }

            var candidatos = await exerciciosQuery.ToListAsync();

            if (!candidatos.Any())
            {
                // Fallback: Tenta buscar sem filtro de objetivo, apenas restrições médicas
                candidatos = await _context.Exercicio
                   .Include(e => e.Contraindicacoes)
                   .Where(e => !e.Contraindicacoes.Any(c => idsProblemasSaude.Contains(c.ProblemaSaudeId)))
                   .ToListAsync();
            }

            var selecionados = candidatos.OrderBy(x => Guid.NewGuid()).Take(quantidadeExercicios).ToList();

            if (!selecionados.Any())
            {
                ModelState.AddModelError("", "Não existem exercícios compatíveis com as restrições médicas.");
                ViewBag.IsStaff = isStaff;
                if (isStaff)
                {
                    IQueryable<UtenteGrupo7> q = _context.UtenteGrupo7;
                    if (!User.IsInRole("Administrador")) q = q.Where(u => u.ProfissionalSaudeId == user.Id);
                    ViewData["UtenteGrupo7Id"] = new SelectList(await q.ToListAsync(), "UtenteGrupo7Id", "Nome");
                }
                else
                {
                    ViewBag.UtenteFixoId = targetUtenteId;
                    ViewBag.UtenteFixoNome = utenteCompleto.Nome;
                }
                return View(planoInput);
            }

            var novoPlano = new PlanoExercicios { UtenteGrupo7Id = targetUtenteId };
            _context.Add(novoPlano);
            await _context.SaveChangesAsync();

            var juncoes = selecionados.Select(ex => new PlanoExercicioExercicio
            {
                PlanoExerciciosId = novoPlano.PlanoExerciciosId,
                ExercicioId = ex.ExercicioId,
                Concluido = false
            }).ToList();

            _context.AddRange(juncoes);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Edit
        [Authorize(Roles = "Administrador,ProfissionalSaude")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            var plano = await _context.PlanoExercicios
                .Include(p => p.UtenteGrupo7) // Necessário para verificar dono
                .Include(p => p.PlanoExercicioExercicios)
                .FirstOrDefaultAsync(p => p.PlanoExerciciosId == id);

            if (plano == null) return NotFound();

            // --- SEGURANÇA ---
            if (!User.IsInRole("Administrador"))
            {
                if (plano.UtenteGrupo7.ProfissionalSaudeId != user.Id) return Forbid();
            }

            await CarregarViewBagsManual(user, plano.UtenteGrupo7Id);

            ViewBag.ExerciciosSelecionados = plano.PlanoExercicioExercicios
                .Select(pe => pe.ExercicioId)
                .ToArray();

            return View(plano);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,ProfissionalSaude")]
        public async Task<IActionResult> Edit(int id, PlanoExercicios plano, int[] exerciciosSelecionados)
        {
            if (id != plano.PlanoExerciciosId) return NotFound();

            var user = await _userManager.GetUserAsync(User);

            // --- SEGURANÇA: Verificar se o utente alvo pertence ao Profissional ---
            if (!User.IsInRole("Administrador"))
            {
                var utenteValido = await _context.UtenteGrupo7
                    .AnyAsync(u => u.UtenteGrupo7Id == plano.UtenteGrupo7Id && u.ProfissionalSaudeId == user.Id);

                if (!utenteValido) return Forbid(); // Tentativa de editar plano para utente alheio
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var planoDb = await _context.PlanoExercicios
                        .Include(p => p.PlanoExercicioExercicios)
                        .FirstOrDefaultAsync(p => p.PlanoExerciciosId == id);

                    if (planoDb == null) return View("InvalidPlanoExercicio", plano);

                    planoDb.UtenteGrupo7Id = plano.UtenteGrupo7Id;

                    // Atualizar exercícios (Remove e Adiciona)
                    _context.RemoveRange(planoDb.PlanoExercicioExercicios);

                    if (exerciciosSelecionados != null)
                    {
                        var novasJuncoes = exerciciosSelecionados.Select(exId => new PlanoExercicioExercicio
                        {
                            PlanoExerciciosId = id,
                            ExercicioId = exId,
                            Concluido = false
                        });
                        await _context.AddRangeAsync(novasJuncoes);
                    }

                    _context.Update(planoDb);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlanoExerciciosExists(plano.PlanoExerciciosId)) return View("InvalidPlano", plano);
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            await CarregarViewBagsManual(user, plano.UtenteGrupo7Id);
            return View(plano);
        }

        // GET: Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            var plano = await _context.PlanoExercicios
                .Include(p => p.UtenteGrupo7)
                .FirstOrDefaultAsync(m => m.PlanoExerciciosId == id);

            if (plano == null) return NotFound();

            // --- SEGURANÇA ---
            if (!User.IsInRole("Administrador"))
            {
                if (User.IsInRole("ProfissionalSaude"))
                {
                    if (plano.UtenteGrupo7.ProfissionalSaudeId != user.Id) return Forbid();
                }
                else
                {
                    // Utente não deve poder apagar planos (normalmente)
                    // Mas se permitires, verificas aqui:
                    if (plano.UtenteGrupo7.UserId != user.Id) return Forbid();
                }
            }

            return View(plano);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var plano = await _context.PlanoExercicios
                .Include(p => p.UtenteGrupo7)
                .FirstOrDefaultAsync(p => p.PlanoExerciciosId == id);

            if (plano != null)
            {
                // --- SEGURANÇA (Dupla verificação no POST) ---
                var user = await _userManager.GetUserAsync(User);
                if (!User.IsInRole("Administrador"))
                {
                    if (User.IsInRole("ProfissionalSaude"))
                    {
                        if (plano.UtenteGrupo7.ProfissionalSaudeId != user.Id) return Forbid();
                    }
                    else
                    {
                        if (plano.UtenteGrupo7.UserId != user.Id) return Forbid();
                    }
                }

                _context.PlanoExercicios.Remove(plano);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // --- Helpers ---

        private bool PlanoExerciciosExists(int id)
        {
            return _context.PlanoExercicios.Any(e => e.PlanoExerciciosId == id);
        }

        private async Task<bool> IsStaff(IdentityUser user)
        {
            if (user == null) return false;
            return await _userManager.IsInRoleAsync(user, "Administrador") ||
                   await _userManager.IsInRoleAsync(user, "ProfissionalSaude");
        }

        // Método auxiliar para carregar a lista de utentes correta (filtrada por profissional)
        private async Task CarregarViewBagsManual(IdentityUser user, int? utenteSelecionado = null)
        {
            IQueryable<UtenteGrupo7> query = _context.UtenteGrupo7;

            if (!await _userManager.IsInRoleAsync(user, "Administrador"))
            {
                // Se for profissional, filtra apenas os seus utentes
                query = query.Where(u => u.ProfissionalSaudeId == user.Id);
            }

            var listaUtentes = await query
                .Select(u => new { u.UtenteGrupo7Id, Nome = $"{u.UtenteGrupo7Id} - {u.Nome}" })
                .ToListAsync();

            ViewData["UtenteGrupo7Id"] = new SelectList(listaUtentes, "UtenteGrupo7Id", "Nome", utenteSelecionado);

            ViewBag.TodosExercicios = await _context.Exercicio
                .Select(e => new SelectListItem { Value = e.ExercicioId.ToString(), Text = e.ExercicioNome })
                .ToListAsync();
        }
    }
}