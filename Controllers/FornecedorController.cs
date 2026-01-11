using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace HealthWellbeing.Controllers
{
    [Authorize(Roles = "Gestor de armazenamento")]
    public class FornecedorController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public FornecedorController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // INDEX COM PAGINAÇÃO + PESQUISA
        public async Task<IActionResult> Index(string? searchNome, int page = 1)
        {
            var query = _context.Fornecedor.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchNome))
                query = query.Where(f => f.NomeEmpresa.Contains(searchNome));

            int totalItems = await query.CountAsync();
            var model = new PaginationInfo<Fornecedor>(page, totalItems);

            model.Items = await query
                .OrderBy(f => f.NomeEmpresa)
                .Skip(model.ItemsToSkip)
                .Take(model.ItemsPerPage)
                .ToListAsync();

            ViewBag.SearchNome = searchNome;

            return View(model);
        }

        // DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return View("InvalidFornecedor");

            var fornecedor = await _context.Fornecedor
                    .Include(c => c.FornecedoresConsumiveis)
                    .ThenInclude(fc => fc.Consumivel)
                .FirstOrDefaultAsync(f => f.FornecedorId == id);

            if (fornecedor == null) return View("InvalidFornecedor");

            return View(fornecedor);
        }

        // CREATE
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FornecedorId,NomeEmpresa,NIF,Morada,Telefone,Email")] Fornecedor fornecedor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fornecedor);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Fornecedor criado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            return View(fornecedor);
        }

        // EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return View("InvalidFornecedor");

            var fornecedor = await _context.Fornecedor.FindAsync(id);
            if (fornecedor == null) return View("InvalidFornecedor");

            return View(fornecedor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FornecedorId,NomeEmpresa,NIF,Morada,Telefone,Email")] Fornecedor fornecedor)
        {
            if (id != fornecedor.FornecedorId)
                return View("InvalidFornecedor");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fornecedor);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Fornecedor atualizado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Fornecedor.Any(e => e.FornecedorId == fornecedor.FornecedorId))
                        return View("InvalidFornecedor");
                    else
                        throw;
                }
            }
            return View(fornecedor);
        }

        // DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return View("InvalidFornecedor");

            var fornecedor = await _context.Fornecedor
                .Include(f => f.FornecedoresConsumiveis)
                .FirstOrDefaultAsync(f => f.FornecedorId == id);

            if (fornecedor == null)
                return View("InvalidFornecedor");

            if (fornecedor.FornecedoresConsumiveis.Any())
            {
                TempData["ErrorMessage"] =
                    "Não é possível eliminar este fornecedor porque está associado a um ou mais consumíveis.";
                return RedirectToAction(nameof(Index));
            }

            return View(fornecedor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fornecedor = await _context.Fornecedor
                .Include(f => f.FornecedoresConsumiveis)
                .FirstOrDefaultAsync(f => f.FornecedorId == id);

            if (fornecedor == null)
                return RedirectToAction(nameof(Index));

            // BLOQUEIO REAL
            if (fornecedor.FornecedoresConsumiveis.Any())
            {
                TempData["ErrorMessage"] =
                    "Não é possível eliminar o fornecedor porque está associado a um ou mais consumíveis.";
                return RedirectToAction(nameof(Index));
            }

            _context.Fornecedor.Remove(fornecedor);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Fornecedor eliminado com sucesso!";
            return RedirectToAction(nameof(Index));
        }
    }
}
