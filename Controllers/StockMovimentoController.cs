using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Controllers
{
    public class StockMovimentoController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public StockMovimentoController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // ===============================
        // LISTA DE COMPRAS (ENTRADAS)
        // ===============================
        public IActionResult Index()
        {
            var movimentos = _context.StockMovimento
                .Where(m => m.Tipo == "Entrada")
                .Include(m => m.Stock)
                    .ThenInclude(s => s.Consumivel)   // Sem fornecedor
                .Include(m => m.Stock)
                    .ThenInclude(s => s.Zona)
                .OrderByDescending(m => m.Data)
                .ToList();

            return View(movimentos);
        }

        // ===============================
        // FORMULÁRIO PARA REGISTAR COMPRA
        // ===============================
        public IActionResult CreateEntrada()
        {
            ViewBag.Stocks = _context.Stock
                .Include(s => s.Consumivel)  // Sem fornecedor
                .Include(s => s.Zona)
                .ToList();

            return View();
        }

        // ===============================
        // PROCESSAR COMPRA (POST)
        // ===============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateEntrada(StockMovimento movimento)
        {
            ViewBag.Stocks = _context.Stock
                .Include(s => s.Consumivel)
                .Include(s => s.Zona)
                .ToList();

            if (movimento.StockId <= 0)
            {
                ModelState.AddModelError("StockId", "Selecione um consumível.");
                return View(movimento);
            }

            if (!ModelState.IsValid)
                return View(movimento);

            var stock = _context.Stock.FirstOrDefault(s => s.StockId == movimento.StockId);

            if (stock == null)
            {
                ModelState.AddModelError("", "Stock não encontrado.");
                return View(movimento);
            }

            if (movimento.Quantidade <= 0)
            {
                ModelState.AddModelError("Quantidade", "A quantidade deve ser maior que zero.");
                return View(movimento);
            }

            int quantidadeDisponivel = stock.QuantidadeMaxima - stock.QuantidadeAtual;

            if (movimento.Quantidade > quantidadeDisponivel)
            {
                ModelState.AddModelError("Quantidade",
                    $"Só pode comprar até {quantidadeDisponivel} unidades.");
                return View(movimento);
            }

            movimento.Tipo = "Entrada";
            movimento.Data = DateTime.Now;

            stock.QuantidadeAtual += movimento.Quantidade;
            stock.DataUltimaAtualizacao = DateTime.Now;

            _context.StockMovimento.Add(movimento);
            _context.SaveChanges();

            TempData["Success"] = "Compra registada com sucesso!";

            return RedirectToAction(nameof(Index));
        }

        // ===============================
        // SAÍDAS DE STOCK (FORM)
        // ===============================
        public IActionResult CreateSaida()
        {
            ViewBag.Stocks = _context.Stock
                .Include(s => s.Consumivel)
                .Include(s => s.Zona)
                .ToList();

            return View();
        }

        // ===============================
        // PROCESSAR SAÍDA (POST)
        // ===============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateSaida(StockMovimento movimento)
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

            _context.StockMovimento.Add(movimento);
            _context.SaveChanges();

            TempData["Success"] = "Saída registada com sucesso!";
            return RedirectToAction("Index");
        }
    }
}
