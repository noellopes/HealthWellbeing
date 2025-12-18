using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModel;

namespace HealthWellbeing.Controllers
{
    public class SpecialitiesController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public SpecialitiesController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Specialities
        public async Task<IActionResult> Index(string? q, int page = 1)
        {
            // 1) Nunca deixar page <= 0 (evita OFFSET negativo)
            if (page < 1) page = 1;

            var query = _context.Specialities.AsQueryable();

            // Filtro de pesquisa
            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();

                query = query.Where(s =>
                    s.Nome.Contains(q) ||
                    s.Descricao.Contains(q));
            }

            var totalItems = await query.CountAsync();

            // 2) Se não houver resultados, devolve já com paginação "safe"
            //    (e evita cálculos estranhos de páginas)
            if (totalItems == 0)
            {
                var emptyPagination = new PaginationInfo<Specialities>(
                    currentPage: 1,
                    totalItems: 0,
                    itemsPerPage: 9
                );

                emptyPagination.Items = new System.Collections.Generic.List<Specialities>();

                ViewBag.SearchQuery = q;
                ViewBag.NoResults = !string.IsNullOrWhiteSpace(q); // para mostrar mensagem "não encontrado"
                return View(emptyPagination);
            }

            // 3) Criar paginação
            var pagination = new PaginationInfo<Specialities>(
                currentPage: page,
                totalItems: totalItems,
                itemsPerPage: 9
            );

            // 4) Se pedirem uma página acima do total, “prende” na última
            if (pagination.TotalPages > 0 && page > pagination.TotalPages)
            {
                pagination = new PaginationInfo<Specialities>(
                    currentPage: pagination.TotalPages,
                    totalItems: totalItems,
                    itemsPerPage: 9
                );
            }

            var items = await query
                .OrderBy(s => s.Nome)
                .Skip(pagination.ItemsToSkip)
                .Take(pagination.ItemsPerPage)
                .ToListAsync();

            pagination.Items = items;

            ViewBag.SearchQuery = q;
            ViewBag.NoResults = false;

            return View(pagination);
        }

        // GET: Specialities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var specialities = await _context.Specialities
                .FirstOrDefaultAsync(m => m.IdEspecialidade == id);

            if (specialities == null) return NotFound();

            return View(specialities);
        }

        // GET: Specialities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Specialities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("IdEspecialidade,Nome,Descricao,OqueEDescricao")]
            Specialities specialities)
        {
            if (ModelState.IsValid)
            {
                _context.Add(specialities);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(specialities);
        }

        // GET: Specialities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var specialities = await _context.Specialities.FindAsync(id);
            if (specialities == null) return NotFound();

            return View(specialities);
        }

        // POST: Specialities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("IdEspecialidade,Nome,Descricao,OqueEDescricao")]
            Specialities specialities)
        {
            if (id != specialities.IdEspecialidade) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(specialities);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpecialitiesExists(specialities.IdEspecialidade))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(specialities);
        }

        // GET: Specialities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var specialities = await _context.Specialities
                .FirstOrDefaultAsync(m => m.IdEspecialidade == id);

            if (specialities == null) return NotFound();

            return View(specialities);
        }

        // POST: Specialities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var specialities = await _context.Specialities.FindAsync(id);
            if (specialities != null)
            {
                _context.Specialities.Remove(specialities);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SpecialitiesExists(int id)
        {
            return _context.Specialities.Any(e => e.IdEspecialidade == id);
        }
    }
}
