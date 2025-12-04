using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static HealthWellbeing.Data.SeedData;

namespace HealthWellBeing.Controllers
{
    [Authorize(Roles = SeedData.Roles.Administrador + "," + SeedData.Roles.Profissional)]
    public class TipoExercicioController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public TipoExercicioController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: TipoExercicio
        public async Task<IActionResult> Index(
            int page = 1,
            string searchNome = "",
            string searchDescricao = "",
            string searchBeneficio = "")
        {
            var query = _context.TipoExercicio
                .Include(t => t.TipoExercicioBeneficios)
                    .ThenInclude(tb => tb.Beneficio)
                .Include(t => t.Contraindicacao)
                    .ThenInclude(tp => tp.ProblemaSaude)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchNome))
            {
                query = query.Where(t => t.NomeTipoExercicios.Contains(searchNome));
            }

            if (!string.IsNullOrEmpty(searchDescricao))
            {
                query = query.Where(t => t.DescricaoTipoExercicios.Contains(searchDescricao));
            }

            if (!string.IsNullOrEmpty(searchBeneficio))
            {
                query = query.Where(t => t.TipoExercicioBeneficios
                    .Any(tb => tb.Beneficio.NomeBeneficio.Contains(searchBeneficio)));
            }

            // Guardar os valores para a View manter os filtros
            ViewBag.SearchNome = searchNome;
            ViewBag.SearchDescricao = searchDescricao;
            ViewBag.SearchBeneficio = searchBeneficio;

            // Paginação
            int total = await query.CountAsync();
            var pagination = new PaginationInfo<TipoExercicio>(page, total);

            if (total > 0)
            {
                pagination.Items = await query
                    .OrderBy(t => t.NomeTipoExercicios)
                    .Skip(pagination.ItemsToSkip)
                    .Take(pagination.ItemsPerPage)
                    .ToListAsync();
            }
            else
            {
                pagination.Items = new List<TipoExercicio>();
            }

            return View(pagination);
        }

        // GET: TipoExercicio/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var tipoExercicio = await _context.TipoExercicio
                .Include(t => t.TipoExercicioBeneficios)
                    .ThenInclude(tb => tb.Beneficio)
                .Include(t => t.Contraindicacao)
                    .ThenInclude(tp => tp.ProblemaSaude)
                .FirstOrDefaultAsync(m => m.TipoExercicioId == id);

            if (tipoExercicio == null)
            {
                return View("InvalidTipoExercicio");
            }

            return View(tipoExercicio);
        }

        // GET: TipoExercicio/Create
        public IActionResult Create()
        {
            ViewData["Beneficios"] = _context.Beneficio.OrderBy(b => b.NomeBeneficio).ToList();
            ViewData["ProblemasSaude"] = _context.ProblemaSaude.OrderBy(p => p.ProblemaNome).ToList();
            return View();
        }

        // POST: TipoExercicio/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("TipoExercicioId,NomeTipoExercicios,DescricaoTipoExercicios,CaracteristicasTipoExercicios")]
            TipoExercicio tipoExercicio,
            int[] selectedBeneficios,
            int[] selectedProblemasSaude)
        {
            var existingTipoExercicio = await _context.TipoExercicio
                .FirstOrDefaultAsync(t => t.NomeTipoExercicios.ToLower() == tipoExercicio.NomeTipoExercicios.ToLower());

            if (existingTipoExercicio != null)
            {

                TempData["StatusMessage"] = $"Erro: O Tipo de Exercício '{tipoExercicio.NomeTipoExercicios}' já existe no sistema.";

                ViewData["Beneficios"] = _context.Beneficio.OrderBy(b => b.NomeBeneficio).ToList();
                ViewData["ProblemasSaude"] = _context.ProblemaSaude.OrderBy(p => p.ProblemaNome).ToList();

                ViewData["SelectedBeneficios"] = selectedBeneficios != null
                    ? selectedBeneficios.ToList()
                    : new List<int>();

                ViewData["SelectedProblemasSaude"] = selectedProblemasSaude != null
                    ? selectedProblemasSaude.ToList()
                    : new List<int>();

                return View(tipoExercicio);
            }

            if (ModelState.IsValid)
            {
                if (selectedBeneficios != null && selectedBeneficios.Any())
                {
                    tipoExercicio.TipoExercicioBeneficios = new List<TipoExercicioBeneficio>();
                    foreach (var beneficioId in selectedBeneficios)
                    {
                        tipoExercicio.TipoExercicioBeneficios.Add(new TipoExercicioBeneficio
                        {
                            BeneficioId = beneficioId
                        });
                    }
                }

                if (selectedProblemasSaude != null && selectedProblemasSaude.Any())
                {
                    tipoExercicio.Contraindicacao = new List<TipoExercicioProblemaSaude>();
                    foreach (var problemaId in selectedProblemasSaude)
                    {
                        tipoExercicio.Contraindicacao.Add(new TipoExercicioProblemaSaude
                        {
                            ProblemaSaudeId = problemaId
                        });
                    }
                }

                _context.Add(tipoExercicio);
                await _context.SaveChangesAsync();

                TempData["StatusMessage"] = $"Sucesso: O Tipo de Exercício '{tipoExercicio.NomeTipoExercicios}' foi criado com sucesso.";

                return RedirectToAction(nameof(Index));
            }

            ViewData["Beneficios"] = _context.Beneficio.OrderBy(b => b.NomeBeneficio).ToList();
            ViewData["ProblemasSaude"] = _context.ProblemaSaude.OrderBy(p => p.ProblemaNome).ToList();
            return View(tipoExercicio);
        }

        // GET: TipoExercicio/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var tipoExercicio = await _context.TipoExercicio
                .Include(t => t.TipoExercicioBeneficios)
                .Include(t => t.Contraindicacao)
                .FirstOrDefaultAsync(m => m.TipoExercicioId == id);

            if (tipoExercicio == null)
            {
                TempData["StatusMessage"] = "Aviso: O Tipo de Exercício que tentou editar não foi encontrado.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["Beneficios"] = _context.Beneficio.OrderBy(b => b.NomeBeneficio).ToList();
            ViewData["ProblemasSaude"] = _context.ProblemaSaude.OrderBy(p => p.ProblemaNome).ToList();

            ViewData["SelectedBeneficios"] = tipoExercicio.TipoExercicioBeneficios?
                .Select(tb => tb.BeneficioId).ToList() ?? new List<int>();

            ViewData["SelectedProblemasSaude"] = tipoExercicio.Contraindicacao?
                .Select(tp => tp.ProblemaSaudeId).ToList() ?? new List<int>();

            return View(tipoExercicio);
        }

        // POST: TipoExercicio/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("TipoExercicioId,NomeTipoExercicios,DescricaoTipoExercicios,CaracteristicasTipoExercicios")]
            TipoExercicio tipoExercicio,
            int[] selectedBeneficios,
            int[] selectedProblemasSaude)
        {
            if (id != tipoExercicio.TipoExercicioId) return NotFound();

            var existingTipoExercicioWithSameName = await _context.TipoExercicio
                .FirstOrDefaultAsync(t =>
                    t.NomeTipoExercicios.ToLower() == tipoExercicio.NomeTipoExercicios.ToLower() &&
                    t.TipoExercicioId != id);

            if (existingTipoExercicioWithSameName != null)
            {

                ViewData["StatusMessage"] = $"Erro: O Tipo de Exercício '{tipoExercicio.NomeTipoExercicios}' já existe para outro registo.";

                ViewData["Beneficios"] = _context.Beneficio.OrderBy(b => b.NomeBeneficio).ToList();
                ViewData["ProblemasSaude"] = _context.ProblemaSaude.OrderBy(p => p.ProblemaNome).ToList();

                ViewData["SelectedBeneficios"] = selectedBeneficios != null
                    ? selectedBeneficios.ToList()
                    : new List<int>();

                ViewData["SelectedProblemasSaude"] = selectedProblemasSaude != null
                    ? selectedProblemasSaude.ToList()
                    : new List<int>();

                return View(tipoExercicio);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var tipoExercicioExistente = await _context.TipoExercicio
                        .Include(t => t.TipoExercicioBeneficios)
                        .Include(t => t.Contraindicacao)
                        .FirstOrDefaultAsync(t => t.TipoExercicioId == id);

                    if (tipoExercicioExistente == null)
                    {
                        TempData["StatusMessage"] = "Erro: O registo foi apagado por outro utilizador.";
                        return RedirectToAction(nameof(Index));
                    }

                    tipoExercicioExistente.NomeTipoExercicios = tipoExercicio.NomeTipoExercicios;
                    tipoExercicioExistente.DescricaoTipoExercicios = tipoExercicio.DescricaoTipoExercicios;
                    tipoExercicioExistente.CaracteristicasTipoExercicios = tipoExercicio.CaracteristicasTipoExercicios;

                    if (tipoExercicioExistente.TipoExercicioBeneficios != null)
                    {
                        tipoExercicioExistente.TipoExercicioBeneficios.Clear();
                    }
                    else
                    {
                        tipoExercicioExistente.TipoExercicioBeneficios = new List<TipoExercicioBeneficio>();
                    }

                    if (selectedBeneficios != null)
                    {
                        foreach (var beneficioId in selectedBeneficios)
                        {
                            tipoExercicioExistente.TipoExercicioBeneficios.Add(new TipoExercicioBeneficio
                            {
                                TipoExercicioId = id,
                                BeneficioId = beneficioId
                            });
                        }
                    }

                    if (tipoExercicioExistente.Contraindicacao != null)
                    {
                        tipoExercicioExistente.Contraindicacao.Clear();
                    }
                    else
                    {
                        tipoExercicioExistente.Contraindicacao = new List<TipoExercicioProblemaSaude>();
                    }

                    if (selectedProblemasSaude != null)
                    {
                        foreach (var problemaId in selectedProblemasSaude)
                        {
                            tipoExercicioExistente.Contraindicacao.Add(new TipoExercicioProblemaSaude
                            {
                                TipoExercicioId = id,
                                ProblemaSaudeId = problemaId
                            });
                        }
                    }

                    _context.Update(tipoExercicioExistente);
                    await _context.SaveChangesAsync();

                    TempData["StatusMessage"] = $"Sucesso: O Tipo de Exercício '{tipoExercicio.NomeTipoExercicios}' foi editado com sucesso.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoExercicioExists(tipoExercicio.TipoExercicioId))
                    {
                        TempData["StatusMessage"] = "Aviso: O registo já não existe (provavelmente foi eliminado por outro utilizador).";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewData["Beneficios"] = _context.Beneficio.OrderBy(b => b.NomeBeneficio).ToList();
            ViewData["ProblemasSaude"] = _context.ProblemaSaude.OrderBy(p => p.ProblemaNome).ToList();

            ViewData["SelectedBeneficios"] = selectedBeneficios != null
                ? selectedBeneficios.ToList()
                : new List<int>();

            ViewData["SelectedProblemasSaude"] = selectedProblemasSaude != null
                ? selectedProblemasSaude.ToList()
                : new List<int>();

            return View(tipoExercicio);
        }

        // GET: TipoExercicio/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var tipoExercicio = await _context.TipoExercicio
                .Include(t => t.TipoExercicioBeneficios)
                    .ThenInclude(tb => tb.Beneficio)
                .Include(t => t.Contraindicacao)
                    .ThenInclude(tp => tp.ProblemaSaude)
                .FirstOrDefaultAsync(m => m.TipoExercicioId == id);

            if (tipoExercicio == null)
            {
                TempData["StatusMessage"] = "Aviso: O registo que tentou eliminar já não existe.";
                return RedirectToAction(nameof(Index));
            }

            return View(tipoExercicio);
        }

        // POST: TipoExercicio/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipoExercicio = await _context.TipoExercicio.FindAsync(id);
            if (tipoExercicio != null)
            {
                _context.TipoExercicio.Remove(tipoExercicio);
                await _context.SaveChangesAsync();

                TempData["StatusMessage"] = $"Sucesso: O Tipo de Exercício '{tipoExercicio.NomeTipoExercicios}' foi eliminado com sucesso.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TipoExercicioExists(int id)
        {
            return _context.TipoExercicio.Any(e => e.TipoExercicioId == id);
        }
    }
}