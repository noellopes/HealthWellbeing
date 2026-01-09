using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace HealthWellbeing.Controllers
{
    public class HistoricoComprasController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public HistoricoComprasController(HealthWellbeingDbContext context)
        {
            _context = context;
        }
        
        // INDEX — Histórico de Compras (ENTRADAS)
        
        public IActionResult Index(int page = 1)
        {
            int itemsPerPage = 10;
            var rnd = new Random();

            var query = _context.HistoricoCompras
                .Where(m => m.Tipo == "Entrada")
                .Include(m => m.Stock)
                    .ThenInclude(s => s.Consumivel)
                .Include(m => m.Fornecedor)
                .OrderByDescending(m => m.Data)
                .AsQueryable();

            int totalMovimentos = query.Count();

            var movimentosPagina = query
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToList();

            
            foreach (var m in movimentosPagina)
            {
                if (m.Fornecedor == null && m.Stock?.Consumivel != null)
                {
                    var fornecedoresPossiveis = _context.Fornecedor_Consumivel
                        .Where(fc => fc.ConsumivelId == m.Stock.Consumivel.ConsumivelId)
                        .Include(fc => fc.Fornecedor)
                        .Select(fc => fc.Fornecedor)
                        .ToList();

                    if (fornecedoresPossiveis.Any())
                    {
                        m.Fornecedor =
                            fornecedoresPossiveis[rnd.Next(fornecedoresPossiveis.Count)];
                    }
                }
            }

            var paginated = new PaginationInfo<HistoricoCompras>(
                page,
                totalMovimentos,
                itemsPerPage
            )
            {
                Items = movimentosPagina
            };

            return View(paginated);
        }

        // DETALHES
        public IActionResult Details(int id)
        {
            var movimento = _context.HistoricoCompras
                .Include(m => m.Stock)
                    .ThenInclude(s => s.Consumivel)
                .Include(m => m.Fornecedor)
                .FirstOrDefault(m => m.Id == id);

            if (movimento == null)
                return NotFound();

            return View(movimento);
        }

        // CONFIRMAR REMOÇÃO (GET)
        
        public IActionResult Delete(int id)
        {
            var movimento = _context.HistoricoCompras
                .Include(m => m.Stock)
                    .ThenInclude(s => s.Consumivel)
                .Include(m => m.Fornecedor)
                .FirstOrDefault(m => m.Id == id);

            if (movimento == null)
                return NotFound();

            return View(movimento);
        }

        // REMOVER MOVIMENTO (POST)
        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var movimento = _context.HistoricoCompras
                .Include(m => m.Stock)
                    .ThenInclude(s => s.Consumivel)
                .FirstOrDefault(m => m.Id == id);

            if (movimento == null)
                return NotFound();

            TempData["Success"] =
                $"Registo de {movimento.Quantidade} unidades de '{movimento.Stock?.Consumivel?.Nome}' eliminado com sucesso!";

            _context.HistoricoCompras.Remove(movimento);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // FORMULÁRIO DE ENTRADA (GET)
        
        public IActionResult CreateEntrada()
        {
            ViewBag.Stocks = _context.Stock
                .Include(s => s.Consumivel)
                .ToList();

            ViewBag.Fornecedores = _context.Fornecedor.ToList();

            return View();
        }

        // PROCESSAR ENTRADA (POST)
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateEntrada(HistoricoCompras movimento)
        {
            ViewBag.Stocks = _context.Stock
                .Include(s => s.Consumivel)
                .ToList();

            ViewBag.Fornecedores = _context.Fornecedor.ToList();

            if (!ModelState.IsValid)
                return View(movimento);

            var stock = _context.Stock
                .Include(s => s.Consumivel)
                .FirstOrDefault(s => s.StockId == movimento.StockId);

            if (stock == null)
            {
                ModelState.AddModelError("", "Stock não encontrado.");
                return View(movimento);
            }

            int capacidadeRestante =
                stock.Consumivel.QuantidadeMaxima - stock.QuantidadeAtual;

            if (movimento.Quantidade > capacidadeRestante)
            {
                ModelState.AddModelError(
                    "Quantidade",
                    $"Só pode comprar até {capacidadeRestante} unidades."
                );
                return View(movimento);
            }

            // ✔ Entrada
            movimento.Tipo = "Entrada";
            movimento.Data = DateTime.Now;

            stock.QuantidadeAtual += movimento.Quantidade;
            stock.DataUltimaAtualizacao = DateTime.Now;

            _context.HistoricoCompras.Add(movimento);
            _context.SaveChanges();

            TempData["Success"] = "Compra registada com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // FORMULÁRIO DE SAÍDA (GET)
        
        public IActionResult CreateSaida()
        {
            ViewBag.Stocks = _context.Stock
                .Include(s => s.Consumivel)
                .ToList();

            return View();
        }

        // PROCESSAR SAÍDA (POST)
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateSaida(HistoricoCompras movimento)
        {
            ViewBag.Stocks = _context.Stock
                .Include(s => s.Consumivel)
                .ToList();

            if (!ModelState.IsValid)
                return View(movimento);

            var stock = _context.Stock
                .Include(s => s.Consumivel)
                .FirstOrDefault(s => s.StockId == movimento.StockId);

            if (stock == null)
            {
                ModelState.AddModelError("", "Stock não encontrado.");
                return View(movimento);
            }

            if (movimento.Quantidade > stock.QuantidadeAtual)
            {
                ModelState.AddModelError(
                    "Quantidade",
                    "Não existe stock suficiente."
                );
                return View(movimento);
            }

            // ✔ Saída 
            movimento.Tipo = "Saida";
            movimento.Data = DateTime.Now;
            movimento.FornecedorId = null;

            stock.QuantidadeAtual -= movimento.Quantidade;
            stock.DataUltimaAtualizacao = DateTime.Now;

            _context.HistoricoCompras.Add(movimento);
            _context.SaveChanges();

            TempData["Success"] = "Saída registada com sucesso!";
            return RedirectToAction(nameof(Index));
        }
    }
}
