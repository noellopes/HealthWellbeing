using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;

namespace HealthWellbeing.Controllers
{
    public class AuditoriaConsumivelController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public AuditoriaConsumivelController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // INDEX COM PAGINAÇÃO + PESQUISA
        public async Task<IActionResult> Index(
            int page = 1,
            int pageSize = 10,
            string? searchConsumivel = "")
        {
            var query = _context.AuditoriaConsumivel
                .Include(a => a.Consumivel)
                .AsQueryable();

            // Sala só incluída se existir DbSet<Sala>
            if (_context.GetType().GetProperty("Sala") != null)
            {
                query = query.Include(a => a.Sala);
            }

            // FILTROS
            if (!string.IsNullOrWhiteSpace(searchConsumivel))
                query = query.Where(a => a.Consumivel != null && a.Consumivel.Nome.Contains(searchConsumivel));

            int totalItems = await query.CountAsync();
            var pagination = new PaginationInfo<AuditoriaConsumivel>(page, totalItems, pageSize);

            pagination.Items = await query
                .OrderBy(a => a.DataConsumo)
                .Skip(pagination.ItemsToSkip)
                .Take(pagination.ItemsPerPage)
                .ToListAsync();

            ViewBag.SearchConsumivel = searchConsumivel;

            return View(pagination);
        }

        // DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var auditoria = _context.AuditoriaConsumivel.AsQueryable();

            auditoria = auditoria.Include(a => a.Consumivel);
            if (_context.GetType().GetProperty("Sala") != null)
                auditoria = auditoria.Include(a => a.Sala);

            var item = await auditoria.FirstOrDefaultAsync(a => a.AuditoriaConsumivelId == id);
            if (item == null)
                return NotFound();

            return View(item);
        }

        // CREATE
        public IActionResult Create()
        {
            ViewData["ConsumivelID"] = new SelectList(_context.Consumivel, "ConsumivelId", "Nome");

            // Sala opcional
            if (_context.GetType().GetProperty("Sala") != null)
                ViewData["SalaId"] = new SelectList(_context.Set<Sala>(), "SalaId", "TipoSala");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AuditoriaConsumivel auditoria)
        {
            if (ModelState.IsValid)
            {
                _context.Add(auditoria);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Auditoria criada com sucesso!";
                return RedirectToAction(nameof(Index));
            }

            ViewData["ConsumivelID"] = new SelectList(_context.Consumivel, "ConsumivelId", "Nome", auditoria.ConsumivelID);

            if (_context.GetType().GetProperty("Sala") != null)
                ViewData["SalaId"] = new SelectList(_context.Set<Sala>(), "SalaId", "TipoSala", auditoria.SalaId);

            return View(auditoria);
        }

        // EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var auditoria = await _context.AuditoriaConsumivel.FindAsync(id);
            if (auditoria == null) return NotFound();

            ViewData["ConsumivelID"] = new SelectList(_context.Consumivel, "ConsumivelId", "Nome", auditoria.ConsumivelID);

            if (_context.GetType().GetProperty("Sala") != null)
                ViewData["SalaId"] = new SelectList(_context.Set<Sala>(), "SalaId", "TipoSala", auditoria.SalaId);

            return View(auditoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AuditoriaConsumivel auditoria)
        {
            if (id != auditoria.AuditoriaConsumivelId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(auditoria);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Auditoria atualizada com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.AuditoriaConsumivel.Any(a => a.AuditoriaConsumivelId == auditoria.AuditoriaConsumivelId))
                        return NotFound();
                    throw;
                }
            }

            ViewData["ConsumivelID"] = new SelectList(_context.Consumivel, "ConsumivelId", "Nome", auditoria.ConsumivelID);

            if (_context.GetType().GetProperty("Sala") != null)
                ViewData["SalaId"] = new SelectList(_context.Set<Sala>(), "SalaId", "TipoSala", auditoria.SalaId);

            return View(auditoria);
        }

        // DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var auditoria = _context.AuditoriaConsumivel.AsQueryable();
            auditoria = auditoria.Include(a => a.Consumivel);

            if (_context.GetType().GetProperty("Sala") != null)
                auditoria = auditoria.Include(a => a.Sala);

            var item = await auditoria.FirstOrDefaultAsync(a => a.AuditoriaConsumivelId == id);
            if (item == null) return NotFound();

            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var auditoria = await _context.AuditoriaConsumivel.FindAsync(id);
            if (auditoria != null)
            {
                _context.AuditoriaConsumivel.Remove(auditoria);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Auditoria eliminada com sucesso!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
