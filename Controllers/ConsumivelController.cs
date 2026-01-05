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

        // INDEX: Sincroniza stock, gera avisos e lista os consumíveis
        public async Task<IActionResult> Index(string? searchNome, string? searchCategoria, int page = 1)
        {
            var todosConsumiveisIds = await _context.Consumivel.Select(c => c.ConsumivelId).ToListAsync();

            foreach (var id in todosConsumiveisIds)
            {
                // 1. Atualiza as quantidades reais baseadas na soma das zonas
                await AtualizarQuantidadeAtualConsumivel(id);
                await AtualizarQuantidadeMaximaConsumivel(id);

                // 2. Verifica se deve gerar o TempData para o alerta na View
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

        // AÇÃO PARA ACEITAR A PROPOSTA: Regista a compra e atualiza o stock nas zonas (Respeitando Capacidades)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AceitarEncomenda(int id)
        {
            var consumivel = await _context.Consumivel.FindAsync(id);
            if (consumivel == null) return NotFound();

            // Procura o melhor fornecedor (Mais barato, depois mais rápido)
            var melhorFornecedor = await _context.Fornecedor_Consumivel
                .Where(fc => fc.ConsumivelId == id)
                .OrderBy(fc => fc.Preco)
                .ThenBy(fc => fc.TempoEntrega)
                .FirstOrDefaultAsync();

            if (melhorFornecedor != null)
            {
                // Calcula quanto falta para atingir o stock máximo ideal do consumível
                int qtdAEncomendar = consumivel.QuantidadeMaxima - consumivel.QuantidadeAtual;

                if (qtdAEncomendar > 0)
                {
                    // 1. Cria o registo na tabela Compra
                    var novaCompra = new Compra
                    {
                        ConsumivelId = id,
                        FornecedorId = melhorFornecedor.FornecedorId,
                        Quantidade = qtdAEncomendar,
                        PrecoUnitario = melhorFornecedor.Preco,
                        TempoEntrega = melhorFornecedor.TempoEntrega ?? 0
                    };
                    _context.Compra.Add(novaCompra);

                    // 2. DISTRIBUIÇÃO INTELIGENTE PELAS ZONAS
                    // Busca todas as zonas ativas para este produto
                    var zonasDisponiveis = await _context.ZonaArmazenamento
                        .Where(z => z.ConsumivelId == id && z.Ativa == true)
                        .OrderBy(z => z.NomeZona) // Opcional: ordem de preenchimento
                        .ToListAsync();

                    int quantidadeParaDistribuir = qtdAEncomendar;
                    int quantidadeAlocada = 0;

                    foreach (var zona in zonasDisponiveis)
                    {
                        // Se já não há nada para distribuir, para o loop
                        if (quantidadeParaDistribuir <= 0) break;

                        // Calcula o espaço livre nesta gaveta/zona
                        int espacoLivre = zona.CapacidadeMaxima - zona.QuantidadeAtual;

                        if (espacoLivre > 0)
                        {
                            // Define quanto vamos pôr nesta zona: o que for menor entre o que falta e o espaço livre
                            int aAdicionar = Math.Min(quantidadeParaDistribuir, espacoLivre);

                            zona.QuantidadeAtual += aAdicionar;
                            _context.Update(zona);

                            // Atualiza os contadores
                            quantidadeParaDistribuir -= aAdicionar;
                            quantidadeAlocada += aAdicionar;
                        }
                    }

                    await _context.SaveChangesAsync();

                    // 3. Sincroniza o total do Consumível
                    await AtualizarQuantidadeAtualConsumivel(id);

                    // Mensagem de feedback
                    if (quantidadeParaDistribuir > 0)
                    {
                        // Caso tenhamos encomendado mais do que cabe no armazém
                        TempData["SuccessMessage"] = $"⚠️ Encomenda processada, mas {quantidadeParaDistribuir} unidades não couberam nas zonas disponíveis (Armazém Cheio). Stock físico atualizado em {quantidadeAlocada} un.";
                    }
                    else
                    {
                        TempData["SuccessMessage"] = $"✅ Stock atualizado! Encomenda de {quantidadeAlocada} unid. de {consumivel.Nome} distribuída pelas zonas.";
                    }
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
            TempData.Remove($"Aviso_{id}");
            return RedirectToAction(nameof(Index));
        }

        // --- MÉTODOS DE SUPORTE ---

        private async Task VerificarNecessidadeProposta(int consumivelId)
        {
            var consumivel = await _context.Consumivel.FindAsync(consumivelId);
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

        // --- MÉTODOS PADRÃO ---

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