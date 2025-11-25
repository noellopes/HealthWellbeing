using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;

namespace HealthWellbeing.Controllers
{
    public class ConsumivelController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ConsumivelController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // INDEX COM PAGINAÇÃO + PESQUISA
        public async Task<IActionResult> Index(string? searchNome, string? searchCategoria, int page = 1)
        {

            var query = _context.Consumivel.Include(c => c.CategoriaConsumivel).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchNome))
                query = query.Where(c => c.Nome.Contains(searchNome));

            if (!string.IsNullOrWhiteSpace(searchCategoria))
                query = query.Where(c => c.CategoriaConsumivel.Nome.Contains(searchCategoria));

            int totalItems = await query.CountAsync();
            var model = new PaginationInfo<Consumivel>(page, totalItems);

            model.Items = await query
                .OrderBy(c => c.Nome)
                .Skip(model.ItemsToSkip)
                .Take(model.ItemsPerPage)
                .ToListAsync();

            ViewBag.SearchNome = searchNome;
            ViewBag.SearchCategoria = searchCategoria;

            return View(model);
        }

        // DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return View("InvalidConsumivel");

            var consumivel = await _context.Consumivel
                .Include(c => c.CategoriaConsumivel)
                .FirstOrDefaultAsync(c => c.ConsumivelId == id);

            if (consumivel == null) return View("InvalidConsumivel");

            return View(consumivel);
        }

        // CREATE
        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_context.CategoriaConsumivel, "CategoriaId", "Nome");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ConsumivelId,Nome,Descricao,CategoriaId,QuantidadeMaxima,QuantidadeAtual,QuantidadeMinima")] Consumivel consumivel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(consumivel);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Consumível criado com sucesso!";
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoriaId"] = new SelectList(_context.CategoriaConsumivel, "CategoriaId", "Nome", consumivel.CategoriaId);
            return View(consumivel);
        }

        // EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return View("InvalidConsumivel");

            var consumivel = await _context.Consumivel.FindAsync(id);
            if (consumivel == null) return View("InvalidConsumivel");

            ViewData["CategoriaId"] = new SelectList(_context.CategoriaConsumivel, "CategoriaId", "Nome", consumivel.CategoriaId);
            return View(consumivel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("ConsumivelId,Nome,Descricao,CategoriaId,QuantidadeMaxima,QuantidadeAtual,QuantidadeMinima")] Consumivel consumivel)
        {
            if (id != consumivel.ConsumivelId)
                return View("InvalidConsumivel");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(consumivel);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Consumível atualizado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Consumivel.Any(e => e.ConsumivelId == consumivel.ConsumivelId))
                        return View("InvalidConsumivel");
                    else
                        throw;
                }
            }

            ViewData["CategoriaId"] = new SelectList(_context.CategoriaConsumivel, "CategoriaId", "Nome", consumivel.CategoriaId);
            return View(consumivel);
        }


        // DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return View("InvalidConsumivel");

            var consumivel = await _context.Consumivel
                .Include(c => c.CategoriaConsumivel)
                .FirstOrDefaultAsync(c => c.ConsumivelId == id);

            if (consumivel == null) return View("InvalidConsumivel");

            return View(consumivel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var consumivel = await _context.Consumivel.FindAsync(id);
            if (consumivel != null)
            {
                // Aqui podemos verificar se há Auditorias associadas, opcional
                _context.Consumivel.Remove(consumivel);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Consumível eliminado com sucesso!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
