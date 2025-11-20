using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;

namespace HealthWellBeing.Controllers
{
    public class ProfissionalExecutantesController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ProfissionalExecutantesController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1, string searchNome = null, string searchFuncao = null)
//                                                   ^^^^^^^^^^^^^^^^^^^^^^  ^^^^^^^^^^^^^^^^^^^^^^^^
        {
            int itemsPerPage = 5;

            IQueryable<ProfissionalExecutante> profissionaisQuery = _context.ProfissionalExecutante
                                                                    .OrderBy(p => p.Nome)
                                                                    .AsQueryable();

            if (!string.IsNullOrEmpty(searchNome))
            {
                profissionaisQuery = profissionaisQuery.Where(p => p.Nome.Contains(searchNome));
                ViewBag.SearchNome = searchNome;
            }

            if (!string.IsNullOrEmpty(searchFuncao))
            {
                profissionaisQuery = profissionaisQuery.Where(p => p.Funcao.Contains(searchFuncao));
                ViewBag.SearchFuncao = searchFuncao;
            }

            int totalItems = await profissionaisQuery.CountAsync();

            var paginationInfo = new PaginationInfo<ProfissionalExecutante>(page, totalItems, itemsPerPage);

            paginationInfo.Items = await profissionaisQuery
                                            .Skip(paginationInfo.ItemsToSkip)
                                            .Take(paginationInfo.ItemsPerPage)
                                            .ToListAsync();

            if (!string.IsNullOrEmpty(searchNome) || !string.IsNullOrEmpty(searchFuncao))
            {
                ViewBag.Collapse = "show";
            }

            return View(paginationInfo);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var profissionalExecutante = await _context.ProfissionalExecutante
                .FirstOrDefaultAsync(m => m.ProfissionalExecutanteId == id);
            if (profissionalExecutante == null)
            {
                return NotFound();
            }

            return View(profissionalExecutante);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProfissionalExecutanteId,Nome,Funcao,Telefone,Email")] ProfissionalExecutante profissionalExecutante)
        {
            if (ModelState.IsValid)
            {
                TempData["SuccessMessage"] = $"Profissional '{profissionalExecutante.Nome}' criado com sucesso!";

                _context.Add(profissionalExecutante);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(profissionalExecutante);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var profissionalExecutante = await _context.ProfissionalExecutante.FindAsync(id);
            if (profissionalExecutante == null)
            {
                return NotFound();
            }
            return View(profissionalExecutante);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProfissionalExecutanteId,Nome,Funcao,Telefone,Email")] ProfissionalExecutante profissionalExecutante)
        {
            if (id != profissionalExecutante.ProfissionalExecutanteId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(profissionalExecutante);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"Profissional '{profissionalExecutante.Nome}' editado com sucesso!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProfissionalExecutanteExists(profissionalExecutante.ProfissionalExecutanteId))
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
            return View(profissionalExecutante);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var profissionalExecutante = await _context.ProfissionalExecutante
                .FirstOrDefaultAsync(m => m.ProfissionalExecutanteId == id);
            if (profissionalExecutante == null)
            {
                return NotFound();
            }

            return View(profissionalExecutante);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var profissionalExecutante = await _context.ProfissionalExecutante.FindAsync(id);
            if (profissionalExecutante != null)
            {
                var nome = profissionalExecutante.Nome;
                _context.ProfissionalExecutante.Remove(profissionalExecutante);

                TempData["SuccessMessage"] = $"Profissional '{nome}' eliminado com sucesso!";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProfissionalExecutanteExists(int id)
        {
            return _context.ProfissionalExecutante.Any(e => e.ProfissionalExecutanteId == id);
        }
    }
}