using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace HealthWellbeing.Controllers
{
    [Authorize(Roles = "Gestor de armazenamento")]
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
            var todosConsumiveisIds = await _context.Consumivel.Select(c => c.ConsumivelId).ToListAsync();
            foreach (var id in todosConsumiveisIds)
            {
               
                await AtualizarQuantidadeMaximaConsumivel(id);
                await AtualizarQuantidadeAtualConsumivel(id);


            }

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

            //identificar consumíveis em stock crítico (SÓ DA PÁGINA ATUAL)

            var consumiveisCriticos = _context.Consumivel
        .Where(c => c.QuantidadeAtual < c.QuantidadeMinima)
        .Select(c => new {
            c.ConsumivelId,
            c.Nome,
            c.QuantidadeAtual,
            c.QuantidadeMinima
        })
        .ToList()
        .Select(c => (dynamic)c) 
        .ToList();

            //enviar para a View
            ViewBag.ConsumiveisCriticos = consumiveisCriticos;

            return View(model);
        }

        // DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return View("InvalidConsumivel");

            var consumivel = await _context.Consumivel
                .Include(c => c.CategoriaConsumivel)
                .Include(c => c.FornecedoresConsumiveis)
                    .ThenInclude(fc => fc.Fornecedor)
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
        public async Task<IActionResult> Create([Bind("ConsumivelId,Nome,Descricao,CategoriaId,QuantidadeMinima")] Consumivel consumivel)
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
            [Bind("ConsumivelId,Nome,Descricao,CategoriaId,QuantidadeAtual,QuantidadeMinima")] Consumivel consumivel)
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
            var consumivel = await _context.Consumivel
                .Include(c => c.CategoriaConsumivel)
                .FirstOrDefaultAsync(c => c.ConsumivelId == id);

            if (consumivel == null)
                return View("InvalidConsumivel");

            try
            {
                _context.Consumivel.Remove(consumivel);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Consumível eliminado com sucesso!";
            }
            catch (DbUpdateException)
            {
                // conta quantas zonas existem
                int numZonas = await _context.ZonaArmazenamento
                    .Where(z => z.ConsumivelId == id)
                    .CountAsync();

                // Passa a mensagem de erro para a view
                ViewBag.Error = $"Não é possível apagar este consumível. Existem {numZonas} zonas de armazenamento associadas.";
                return View(consumivel);
            }

            return RedirectToAction(nameof(Index));
        }
        private async Task AtualizarQuantidadeAtualConsumivel(int consumivelId)
        {
            // Obter todas as zonas que têm este consumível
            var quantidadeTotal = await _context.ZonaArmazenamento
                .Where(z => z.ConsumivelId == consumivelId)
                .SumAsync(z => z.QuantidadeAtual);

            // Atualizar o Consumível
            var consumivel = await _context.Consumivel.FindAsync(consumivelId);
            if (consumivel != null)
            {
                consumivel.QuantidadeAtual = quantidadeTotal;
                _context.Update(consumivel);
                await _context.SaveChangesAsync();
            }
        }
        private async Task AtualizarQuantidadeMaximaConsumivel(int consumivelId)
        {
            // Soma todas as capacidades máximas das zonas associadas
            var quantidadeMaximaTotal = await _context.ZonaArmazenamento
                .Where(z => z.ConsumivelId == consumivelId)
                .SumAsync(z => z.CapacidadeMaxima);

            // Atualiza o Consumível
            var consumivel = await _context.Consumivel.FindAsync(consumivelId);
            if (consumivel != null)
            {
                consumivel.QuantidadeMaxima = quantidadeMaximaTotal;
                _context.Update(consumivel);
                await _context.SaveChangesAsync();
            }
        }


    }
}
