using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthWellBeing.Controllers
{
    [Authorize]
    public class TipoExercicioController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public TipoExercicioController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: TipoExercicio
        public async Task<IActionResult> Index(int page = 1, string searchNome = "", string searchDescricao = "", string searchBeneficio = "")
        {
            var query = _context.TipoExercicio
                .Include(t => t.TipoExercicioBeneficios).ThenInclude(tb => tb.Beneficio)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchNome)) query = query.Where(t => t.NomeTipoExercicios.Contains(searchNome));
            if (!string.IsNullOrEmpty(searchDescricao)) query = query.Where(t => t.DescricaoTipoExercicios.Contains(searchDescricao));
            if (!string.IsNullOrEmpty(searchBeneficio))
            {
                query = query.Where(t => t.TipoExercicioBeneficios.Any(tb => tb.Beneficio.NomeBeneficio.Contains(searchBeneficio)));
            }

            ViewBag.SearchNome = searchNome;
            ViewBag.SearchDescricao = searchDescricao;
            ViewBag.SearchBeneficio = searchBeneficio;

            int total = await query.CountAsync();
            var pagination = new PaginationInfo<TipoExercicio>(page, total);

            if (total > 0)
            {
                pagination.Items = await query.OrderBy(t => t.NomeTipoExercicios)
                    .Skip(pagination.ItemsToSkip).Take(pagination.ItemsPerPage).ToListAsync();
            }
            else pagination.Items = new List<TipoExercicio>();

            return View(pagination);
        }

        // GET: TipoExercicio/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var tipoExercicio = await _context.TipoExercicio
                .Include(t => t.TipoExercicioBeneficios).ThenInclude(tb => tb.Beneficio)
                .FirstOrDefaultAsync(m => m.TipoExercicioId == id);
            if (tipoExercicio == null) return View("InvalidTipoExercicio");
            return View(tipoExercicio);
        }

        // GET: TipoExercicio/Create
        [Authorize(Roles = SeedData.Roles.Administrador + "," + SeedData.Roles.Profissional)]
        public IActionResult Create()
        {
            ViewData["Beneficios"] = _context.Beneficio.OrderBy(b => b.NomeBeneficio).ToList();
            return View();
        }

        // POST: TipoExercicio/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SeedData.Roles.Administrador + "," + SeedData.Roles.Profissional)]
        public async Task<IActionResult> Create(TipoExercicio tipoExercicio, int[] selectedBeneficios)
        {
            var existing = await _context.TipoExercicio
                .FirstOrDefaultAsync(t => t.NomeTipoExercicios.ToLower() == tipoExercicio.NomeTipoExercicios.ToLower());

            if (existing != null)
            {
                TempData["StatusMessage"] = $"Erro: O Tipo '{tipoExercicio.NomeTipoExercicios}' já existe.";
                ViewData["Beneficios"] = _context.Beneficio.OrderBy(b => b.NomeBeneficio).ToList();
                return View(tipoExercicio);
            }

            if (ModelState.IsValid)
            {
                if (selectedBeneficios != null)
                {
                    tipoExercicio.TipoExercicioBeneficios = new List<TipoExercicioBeneficio>();
                    foreach (var id in selectedBeneficios)
                    {
                        tipoExercicio.TipoExercicioBeneficios.Add(new TipoExercicioBeneficio { BeneficioId = id });
                    }
                }
                _context.Add(tipoExercicio);
                await _context.SaveChangesAsync();
                TempData["StatusMessage"] = "Sucesso: Tipo de Exercício criado.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["Beneficios"] = _context.Beneficio.OrderBy(b => b.NomeBeneficio).ToList();
            return View(tipoExercicio);
        }

        // GET: TipoExercicio/Edit/5
        [Authorize(Roles = SeedData.Roles.Administrador + "," + SeedData.Roles.Profissional)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var tipoExercicio = await _context.TipoExercicio
                .Include(t => t.TipoExercicioBeneficios)
                .FirstOrDefaultAsync(m => m.TipoExercicioId == id);

            if (tipoExercicio == null) return NotFound();

            ViewData["Beneficios"] = _context.Beneficio.OrderBy(b => b.NomeBeneficio).ToList();
            ViewData["SelectedBeneficios"] = tipoExercicio.TipoExercicioBeneficios?.Select(tb => tb.BeneficioId).ToList() ?? new List<int>();
            return View(tipoExercicio);
        }

        // POST: TipoExercicio/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SeedData.Roles.Administrador + "," + SeedData.Roles.Profissional)]
        public async Task<IActionResult> Edit(int id, TipoExercicio tipoExercicio, int[] selectedBeneficios)
        {
            if (id != tipoExercicio.TipoExercicioId) return NotFound();

            var existingName = await _context.TipoExercicio.AnyAsync(t => t.NomeTipoExercicios.ToLower() == tipoExercicio.NomeTipoExercicios.ToLower() && t.TipoExercicioId != id);
            if (existingName)
            {
                ViewData["StatusMessage"] = $"Erro: Nome duplicado.";
                ViewData["Beneficios"] = _context.Beneficio.OrderBy(b => b.NomeBeneficio).ToList();
                ViewData["SelectedBeneficios"] = selectedBeneficios?.ToList();
                return View(tipoExercicio);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existing = await _context.TipoExercicio.Include(t => t.TipoExercicioBeneficios).FirstOrDefaultAsync(t => t.TipoExercicioId == id);
                    if (existing == null) return RedirectToAction(nameof(Index));

                    existing.NomeTipoExercicios = tipoExercicio.NomeTipoExercicios;
                    existing.DescricaoTipoExercicios = tipoExercicio.DescricaoTipoExercicios;
                    existing.CaracteristicasTipoExercicios = tipoExercicio.CaracteristicasTipoExercicios;

                    existing.TipoExercicioBeneficios.Clear();
                    if (selectedBeneficios != null)
                    {
                        foreach (var bId in selectedBeneficios)
                        {
                            existing.TipoExercicioBeneficios.Add(new TipoExercicioBeneficio { TipoExercicioId = id, BeneficioId = bId });
                        }
                    }
                    _context.Update(existing);
                    await _context.SaveChangesAsync();
                    TempData["StatusMessage"] = "Sucesso: Editado.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoExercicioExists(tipoExercicio.TipoExercicioId)) return RedirectToAction(nameof(Index));
                    else throw;
                }
            }
            ViewData["Beneficios"] = _context.Beneficio.OrderBy(b => b.NomeBeneficio).ToList();
            return View(tipoExercicio);
        }

        // GET: TipoExercicio/Delete/5
        [Authorize(Roles = SeedData.Roles.Administrador + "," + SeedData.Roles.Profissional)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var tipoExercicio = await _context.TipoExercicio
                .Include(t => t.ExercicioTipoExercicios)
                .Include(t => t.TipoExercicioBeneficios)
                .FirstOrDefaultAsync(m => m.TipoExercicioId == id);

            if (tipoExercicio == null)
            {
                TempData["StatusMessage"] = "Aviso: Registo não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            int numExercicios = tipoExercicio.ExercicioTipoExercicios?.Count ?? 0;
            int numBeneficios = tipoExercicio.TipoExercicioBeneficios?.Count ?? 0;

            if (numExercicios > 0)
            {
                ViewBag.PodeEliminar = false;
                ViewBag.MensagemErro = $"Não pode eliminar o tipo '{tipoExercicio.NomeTipoExercicios}' porque existem {numExercicios} Exercício(s) registados com este tipo.";
            }
            else if (numBeneficios > 0)
            {
                ViewBag.PodeEliminar = false;
                ViewBag.MensagemErro = $"Não pode eliminar o tipo '{tipoExercicio.NomeTipoExercicios}' porque ainda possui {numBeneficios} Benefício(s) associados.";
            }
            else
            {
                ViewBag.PodeEliminar = true;
            }

            ViewBag.NumExercicios = numExercicios;
            ViewBag.NumBeneficios = numBeneficios;

            return View(tipoExercicio);
        }

        // POST: TipoExercicio/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = SeedData.Roles.Administrador + "," + SeedData.Roles.Profissional)]
        public async Task<IActionResult> DeleteConfirmed(int TipoExercicioId)
        {
            var tipoExercicio = await _context.TipoExercicio.FindAsync(TipoExercicioId);
            if (tipoExercicio == null) return RedirectToAction(nameof(Index));

            try
            {
                _context.TipoExercicio.Remove(tipoExercicio);
                await _context.SaveChangesAsync();
                TempData["StatusMessage"] = $"Sucesso: Registo eliminado.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TipoExercicioExists(TipoExercicioId)) return RedirectToAction(nameof(Index));
                else throw; 
            }
            catch (DbUpdateException)
            {
                TempData["StatusMessage"] = "Erro: Existem dependências que impedem a eliminação.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TipoExercicioExists(int id)
        {
            return _context.TipoExercicio.Any(e => e.TipoExercicioId == id);
        }
    }
}