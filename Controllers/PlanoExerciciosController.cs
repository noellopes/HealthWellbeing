using HealthWellbeing.Data;
using HealthWellbeing.Models;
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
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            bool isStaff = await IsStaff(user);

            var query = _context.PlanoExercicios
                .Include(p => p.UtenteGrupo7)
                // Inclui a tabela de junção e depois o exercício
                .Include(p => p.PlanoExercicioExercicios)
                    .ThenInclude(pe => pe.Exercicio)
                .AsQueryable();

            // Se for Utente normal (não é Admin nem Profissional), só vê os seus planos
            if (!isStaff)
            {
                query = query.Where(p => p.UtenteGrupo7.UserId == user.Id);
            }

            return View(await query.ToListAsync());
        }

        // GET: PlanoExercicios/Details/5
        public async Task<IActionResult> Details(int? id, bool mostrarAviso = false)
        {
            if (id == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            bool isStaff = await IsStaff(user);

            var plano = await _context.PlanoExercicios
                .Include(p => p.UtenteGrupo7)
                .Include(p => p.PlanoExercicioExercicios)
                    .ThenInclude(pe => pe.Exercicio)
                        .ThenInclude(e => e.ExercicioEquipamentos)
                            .ThenInclude(eq => eq.Equipamento)
                .FirstOrDefaultAsync(m => m.PlanoExerciciosId == id);

            if (plano == null) return NotFound();

            if (!isStaff && plano.UtenteGrupo7.UserId != user.Id)
            {
                return NotFound();
            }

            if (plano.PlanoExercicioExercicios != null)
            {
                plano.PlanoExercicioExercicios = plano.PlanoExercicioExercicios
                    .OrderBy(x => x.Concluido)
                    .ToList();
            }

            ViewBag.MostrarAvisoReinicio = mostrarAviso;

            return View(plano);
        }

        // POST: Atualizar estado do exercício (Checkbox e Peso)
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

            bool isStaff = await IsStaff(user);
            if (!isStaff && itemTreino.PlanoExercicios.UtenteGrupo7.UserId != user.Id)
            {
                return Unauthorized();
            }

            itemTreino.Concluido = concluido;
            itemTreino.PesoUsado = pesoUsado;

            _context.Update(itemTreino);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = planoId });
        }

        // POST: Reiniciar todo o plano
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReiniciarPlano(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            var plano = await _context.PlanoExercicios
                .Include(p => p.UtenteGrupo7)
                .FirstOrDefaultAsync(p => p.PlanoExerciciosId == id);

            if (plano == null) return NotFound();

            bool isStaff = await IsStaff(user);
            if (!isStaff && plano.UtenteGrupo7.UserId != user.Id) return Unauthorized();

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

        // GET: PlanoExercicios/Create
        [Authorize(Roles = "Administrador,ProfissionalSaude")]
        public IActionResult Create()
        {
            CarregarViewBagsManual();
            return View();
        }

        // POST: PlanoExercicios/Create (Manual)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,ProfissionalSaude")]
        public async Task<IActionResult> Create(PlanoExercicios planoExercicios, int[] exerciciosSelecionados)
        {
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

            CarregarViewBagsManual(planoExercicios.UtenteGrupo7Id);
            return View(planoExercicios);
        }

        // GET: CreateAutomatico
        public async Task<IActionResult> CreateAutomatico()
        {
            var user = await _userManager.GetUserAsync(User);
            bool isStaff = await IsStaff(user);

            if (isStaff)
            {
                ViewData["UtenteGrupo7Id"] = new SelectList(_context.UtenteGrupo7, "UtenteGrupo7Id", "Nome");
            }
            else
            {
                var utente = await _context.UtenteGrupo7.FirstOrDefaultAsync(u => u.UserId == user.Id);

                if (utente == null)
                {
                    TempData["MensagemErro"] = "Por favor, finalize o seu registo de Utente (objetivos e saúde) antes de gerar um plano.";
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
            }
            else
            {
                var utenteDb = await _context.UtenteGrupo7.FirstOrDefaultAsync(u => u.UserId == user.Id);
                if (utenteDb == null) return Unauthorized();
                targetUtenteId = utenteDb.UtenteGrupo7Id;
            }

            var utenteCompleto = await _context.UtenteGrupo7
                .Include(u => u.UtenteProblemasSaude)
                .Include(u => u.ObjetivoFisico)
                .FirstOrDefaultAsync(u => u.UtenteGrupo7Id == targetUtenteId);

            if (utenteCompleto == null) return NotFound("Utente não encontrado.");

            var idsProblemasSaude = utenteCompleto.UtenteProblemasSaude
                                                    .Select(up => up.ProblemaSaudeId)
                                                    .ToList();

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
                candidatos = await _context.Exercicio
                   .Include(e => e.Contraindicacoes)
                   .Where(e => !e.Contraindicacoes.Any(c => idsProblemasSaude.Contains(c.ProblemaSaudeId)))
                   .ToListAsync();
            }

            var selecionados = candidatos
                .OrderBy(x => Guid.NewGuid())
                .Take(quantidadeExercicios)
                .ToList();

            if (!selecionados.Any())
            {
                ModelState.AddModelError("", "Não existem exercícios compatíveis com as suas restrições médicas.");

                ViewBag.IsStaff = isStaff;
                if (isStaff) ViewData["UtenteGrupo7Id"] = new SelectList(_context.UtenteGrupo7, "UtenteGrupo7Id", "Nome");
                else
                {
                    ViewBag.UtenteFixoId = targetUtenteId;
                    ViewBag.UtenteFixoNome = utenteCompleto.Nome;
                }
                return View(planoInput);
            }

            var novoPlano = new PlanoExercicios
            {
                UtenteGrupo7Id = targetUtenteId
            };

            _context.Add(novoPlano);
            await _context.SaveChangesAsync();

            var juncoes = selecionados.Select(ex => new PlanoExercicioExercicio
            {
                PlanoExerciciosId = novoPlano.PlanoExerciciosId,
                ExercicioId = ex.ExercicioId,
                Concluido = false,
                PesoUsado = null
            }).ToList();

            _context.AddRange(juncoes);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: PlanoExercicios/Edit/5
        [Authorize(Roles = "Administrador,ProfissionalSaude")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var plano = await _context.PlanoExercicios
                .Include(p => p.PlanoExercicioExercicios)
                .FirstOrDefaultAsync(p => p.PlanoExerciciosId == id);

            if (plano == null) return NotFound();

            CarregarViewBagsManual(plano.UtenteGrupo7Id);

            ViewBag.ExerciciosSelecionados = plano.PlanoExercicioExercicios
                .Select(pe => pe.ExercicioId)
                .ToArray();

            return View(plano);
        }

        // POST: PlanoExercicios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,ProfissionalSaude")]
        public async Task<IActionResult> Edit(int id, PlanoExercicios plano, int[] exerciciosSelecionados)
        {
            if (id != plano.PlanoExerciciosId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var planoDb = await _context.PlanoExercicios
                        .Include(p => p.PlanoExercicioExercicios)
                        .FirstOrDefaultAsync(p => p.PlanoExerciciosId == id);

                    if (planoDb == null) return NotFound();

                    planoDb.UtenteGrupo7Id = plano.UtenteGrupo7Id;

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
                    if (!PlanoExerciciosExists(plano.PlanoExerciciosId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            CarregarViewBagsManual(plano.UtenteGrupo7Id);
            return View(plano);
        }

        // GET: PlanoExercicios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            bool isStaff = await IsStaff(user);

            var plano = await _context.PlanoExercicios
                .Include(p => p.UtenteGrupo7)
                .FirstOrDefaultAsync(m => m.PlanoExerciciosId == id);

            if (plano == null) return NotFound();

            if (!isStaff && plano.UtenteGrupo7.UserId != user.Id)
            {
                return NotFound();
            }

            return View(plano);
        }

        // POST: PlanoExercicios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var plano = await _context.PlanoExercicios.FindAsync(id);
            if (plano != null)
            {

                _context.PlanoExercicios.Remove(plano);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

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

        private void CarregarViewBagsManual(int? utenteSelecionado = null)
        {
            ViewData["UtenteGrupo7Id"] = new SelectList(
                _context.UtenteGrupo7.Select(u => new { u.UtenteGrupo7Id, Nome = $"{u.UtenteGrupo7Id} - {u.Nome}" }),
                "UtenteGrupo7Id", "Nome", utenteSelecionado);

            ViewBag.TodosExercicios = _context.Exercicio
                .Select(e => new SelectListItem { Value = e.ExercicioId.ToString(), Text = e.ExercicioNome })
                .ToList();
        }

        // POST: SalvarProgressoGlobal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarProgressoGlobal(List<PlanoExercicioExercicio> exercicios)
        {
            if (exercicios == null || !exercicios.Any())
            {
                return RedirectToAction(nameof(Index));
            }

            int planoId = exercicios.First().PlanoExerciciosId;

            foreach (var itemInput in exercicios)
            {
                var itemDb = await _context.Set<PlanoExercicioExercicio>()
                    .FirstOrDefaultAsync(pe => pe.PlanoExerciciosId == itemInput.PlanoExerciciosId &&
                                               pe.ExercicioId == itemInput.ExercicioId);

                if (itemDb != null)
                {
                    itemDb.Concluido = itemInput.Concluido;
                    itemDb.PesoUsado = itemInput.PesoUsado;
                    _context.Update(itemDb);
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = planoId });
        }
    }
}