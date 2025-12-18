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

        // =====================================================
        // INDEX — Histórico de Compras com Paginação
        // =====================================================
        public IActionResult Index(int page = 1)
        {
            int itemsPerPage = 10;

            var query = _context.HistoricoCompras
                .Where(m => m.Tipo == "Entrada")
                .Include(m => m.Stock).ThenInclude(s => s.Consumivel)
                .Include(m => m.Stock).ThenInclude(s => s.Zona)
                .OrderByDescending(m => m.Data)
                .AsQueryable();

            int totalMovimentos = query.Count();

            var movimentosPagina = query
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToList();

            var paginated = new PaginationInfo<HistoricoCompras>(page, totalMovimentos, itemsPerPage)
            {
                Items = movimentosPagina
            };

            return View(paginated);
        }

        // =====================================================
        // DETALHES DE UM MOVIMENTO
        // =====================================================
        public IActionResult Details(int id)
        {
            var movimento = _context.HistoricoCompras
                .Include(m => m.Stock).ThenInclude(s => s.Consumivel)
                .Include(m => m.Stock).ThenInclude(s => s.Zona)
                .FirstOrDefault(m => m.Id == id);

            if (movimento == null)
                return NotFound();

            return View(movimento);
        }

        // =====================================================
        // CONFIRMAR REMOÇÃO (GET)
        // =====================================================
        public IActionResult Delete(int id)
        {
            var movimento = _context.HistoricoCompras
                .Include(m => m.Stock).ThenInclude(s => s.Consumivel)
                .Include(m => m.Stock).ThenInclude(s => s.Zona)
                .FirstOrDefault(m => m.Id == id);

            if (movimento == null)
                return NotFound();

            return View(movimento);
        }

        // =====================================================
        // REMOVER MOVIMENTO (POST)
        // =====================================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var movimento = _context.HistoricoCompras
                .Include(m => m.Stock).ThenInclude(s => s.Consumivel)
                .FirstOrDefault(m => m.Id == id);

            if (movimento == null)
                return NotFound();

            // Mensagem visual para o utilizador
            TempData["Success"] =
                $"Registo da compra de {movimento.Quantidade} unidades de '{movimento.Stock?.Consumivel?.Nome}' foi eliminado com sucesso!";

            _context.HistoricoCompras.Remove(movimento);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


        // =====================================================
        // FORMULÁRIO DE ENTRADA (GET)
        // =====================================================
        public IActionResult CreateEntrada()
        {
            ViewBag.Stocks = _context.Stock
                .Include(s => s.Consumivel)
                .Include(s => s.Zona)
                .ToList();

            return View();
        }

        // =====================================================
        // PROCESSAR ENTRADA (POST)
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateEntrada(HistoricoCompras movimento)
        {
            ViewBag.Stocks = _context.Stock
                .Include(s => s.Consumivel)
                .Include(s => s.Zona)
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

            // 🔥 1 — Impedir compra quando já está no máximo
            if (stock.QuantidadeAtual >= stock.Consumivel.QuantidadeMaxima)
            {
                ModelState.AddModelError("Quantidade",
                    $"Não é possível realizar a compra. O consumível '{stock.Consumivel.Nome}' já atingiu a quantidade máxima ({stock.Consumivel.QuantidadeMaxima}).");

                movimento.Quantidade = 0; // limpar input
                return View(movimento);
            }

            // 🔥 2 — Calcular capacidade restante
            int capacidadeRestante = stock.Consumivel.QuantidadeMaxima - stock.QuantidadeAtual;
            if (capacidadeRestante < 0) capacidadeRestante = 0;

            // 🔥 3 — Impedir compra maior que o permitido
            if (movimento.Quantidade > capacidadeRestante)
            {
                ModelState.AddModelError("Quantidade",
                    $"Só pode comprar até {capacidadeRestante} unidades (capacidade máxima atingida).");
                return View(movimento);
            }

            // 🟢 4 — Registrar compra
            movimento.Tipo = "Entrada";
            movimento.Data = DateTime.Now;

            stock.QuantidadeAtual += movimento.Quantidade;
            stock.DataUltimaAtualizacao = DateTime.Now;

            _context.HistoricoCompras.Add(movimento);
            _context.SaveChanges();

            TempData["Success"] = "Compra registada com sucesso!";
            return RedirectToAction(nameof(Index));
        }


        // =====================================================
        // FORMULÁRIO DE SAÍDA (GET)
        // =====================================================
        public IActionResult CreateSaida()
        {
            ViewBag.Stocks = _context.Stock
                .Include(s => s.Consumivel)
                .Include(s => s.Zona)
                .ToList();

            return View();
        }

        // =====================================================
        // PROCESSAR SAÍDA (POST)
        // =====================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateSaida(HistoricoCompras movimento)
        {
            ViewBag.Stocks = _context.Stock
                .Include(s => s.Consumivel)
                .Include(s => s.Zona)
                .ToList();

            if (!ModelState.IsValid)
                return View(movimento);

            var stock = _context.Stock.FirstOrDefault(s => s.StockId == movimento.StockId);

            if (stock == null)
            {
                ModelState.AddModelError("", "Stock não encontrado.");
                return View(movimento);
            }

            if (movimento.Quantidade > stock.QuantidadeAtual)
            {
                ModelState.AddModelError("Quantidade", "Não existe stock suficiente.");
                return View(movimento);
            }

            movimento.Tipo = "Saida";
            movimento.Data = DateTime.Now;

            stock.QuantidadeAtual -= movimento.Quantidade;
            stock.DataUltimaAtualizacao = DateTime.Now;

            _context.HistoricoCompras.Add(movimento);
            _context.SaveChanges();

            TempData["Success"] = "Saída registada com sucesso!";
            return RedirectToAction(nameof(Index));
        }
    }
}
