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

        // INDEX: Sincroniza stock e gera avisos de proposta
        public async Task<IActionResult> Index(string? searchNome, string? searchCategoria, int page = 1)
        {
            var todosConsumiveisIds = await _context.Consumivel.Select(c => c.ConsumivelId).ToListAsync();

            foreach (var id in todosConsumiveisIds)
            {
                // 1. Atualiza as quantidades reais baseadas nas zonas
                await AtualizarQuantidadeAtualConsumivel(id);
                await AtualizarQuantidadeMaximaConsumivel(id);

                // 2. Verifica se deve mostrar um aviso de proposta de encomenda
                await VerificarNecessidadeProposta(id);
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

            return View(model);
        }

        // AÇÃO PARA ACEITAR A PROPOSTA
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AceitarEncomenda(int id)
        {
            var consumivel = await _context.Consumivel.FindAsync(id);
            if (consumivel == null) return NotFound();

            // Procura o melhor fornecedor
            var melhorFornecedor = await _context.Fornecedor_Consumivel
                .Where(fc => fc.ConsumivelId == id)
                .OrderBy(fc => fc.Preco)
                .ThenBy(fc => fc.TempoEntrega)
                .FirstOrDefaultAsync();

            if (melhorFornecedor != null)
            {
                int qtdAEncomendar = consumivel.QuantidadeMaxima - consumivel.QuantidadeAtual;

                if (qtdAEncomendar > 0)
                {
                    _context.Compra.Add(new Compra
                    {
                        ConsumivelId = id,
                        FornecedorId = melhorFornecedor.FornecedorId,
                        Quantidade = qtdAEncomendar,
                        PrecoUnitario = melhorFornecedor.Preco,
                        TempoEntrega = melhorFornecedor.TempoEntrega ?? 0
                    });

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Encomenda de {qtdAEncomendar} unid. de {consumivel.Nome} realizada com sucesso!";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Não foi possível encontrar um fornecedor para este consumível.";
            }

            return RedirectToAction(nameof(Index));
        }

        // AÇÃO PARA NEGAR A PROPOSTA
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult NegarEncomenda(int id)
        {
            // Apenas remove o aviso desta sessão
            TempData.Remove($"Aviso_{id}");
            return RedirectToAction(nameof(Index));
        }

        // --- MÉTODOS DE SUPORTE ---

        private async Task VerificarNecessidadeProposta(int consumivelId)
        {
            var consumivel = await _context.Consumivel.FindAsync(consumivelId);

            // Se o stock estiver abaixo do mínimo, cria uma mensagem de proposta no TempData
            if (consumivel != null && consumivel.QuantidadeAtual <= consumivel.QuantidadeMinima)
            {
                TempData[$"Aviso_{consumivelId}"] = $"Stock Crítico: {consumivel.Nome} ({consumivel.QuantidadeAtual} unidades). Deseja repor stock?";
            }
        }

        private async Task AtualizarQuantidadeAtualConsumivel(int consumivelId)
        {
            var quantidadeTotal = await _context.ZonaArmazenamento
                .Where(z => z.ConsumivelId == consumivelId)
                .SumAsync(z => z.QuantidadeAtual);

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
            var quantidadeMaximaTotal = await _context.ZonaArmazenamento
                .Where(z => z.ConsumivelId == consumivelId)
                .SumAsync(z => z.CapacidadeMaxima);

            var consumivel = await _context.Consumivel.FindAsync(consumivelId);
            if (consumivel != null)
            {
                consumivel.QuantidadeMaxima = quantidadeMaximaTotal;
                _context.Update(consumivel);
                await _context.SaveChangesAsync();
            }
        }

        // --- MÉTODOS PADRÃO (CREATE, EDIT, DETAILS, DELETE) ---

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return View("InvalidConsumivel");
            var consumivel = await _context.Consumivel.Include(c => c.CategoriaConsumivel).FirstOrDefaultAsync(c => c.ConsumivelId == id);
            return (consumivel == null) ? View("InvalidConsumivel") : View(consumivel);
        }

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
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.CategoriaConsumivel, "CategoriaId", "Nome", consumivel.CategoriaId);
            return View(consumivel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return View("InvalidConsumivel");
            var consumivel = await _context.Consumivel.FindAsync(id);
            ViewData["CategoriaId"] = new SelectList(_context.CategoriaConsumivel, "CategoriaId", "Nome", consumivel?.CategoriaId);
            return View(consumivel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ConsumivelId,Nome,Descricao,CategoriaId,QuantidadeAtual,QuantidadeMinima")] Consumivel consumivel)
        {
            if (id != consumivel.ConsumivelId) return View("InvalidConsumivel");
            if (ModelState.IsValid)
            {
                _context.Update(consumivel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(consumivel);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return View("InvalidConsumivel");
            var consumivel = await _context.Consumivel.Include(c => c.CategoriaConsumivel).FirstOrDefaultAsync(c => c.ConsumivelId == id);
            return View(consumivel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var consumivel = await _context.Consumivel.FindAsync(id);
            if (consumivel != null)
            {
                _context.Consumivel.Remove(consumivel);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}