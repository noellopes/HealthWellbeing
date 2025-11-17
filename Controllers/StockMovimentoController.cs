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
                    .ThenInclude(s => s.Consumivel)
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
                .Include(s => s.Consumivel)
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
            if (!ModelState.IsValid)
            {
                ViewBag.Stocks = _context.Stock
                    .Include(s => s.Consumivel)
                    .Include(s => s.Zona)
                    .ToList();

                return View(movimento);
            }

            // Definir tipo como compra (entrada)
            movimento.Tipo = "Entrada";
            movimento.Data = DateTime.Now;

            // Obter stock associado
            var stock = _context.Stock.FirstOrDefault(s => s.StockId == movimento.StockId);

            if (stock == null)
            {
                ModelState.AddModelError("", "Stock não encontrado.");
                return View(movimento);
            }

            // Atualizar quantidade atual
            stock.QuantidadeAtual += movimento.Quantidade;
            stock.DataUltimaAtualizacao = DateTime.Now;

            // Guardar o movimento de entrada
            _context.StockMovimento.Add(movimento);
            _context.SaveChanges();

            TempData["Success"] = "Compra registada com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // ===============================
        // DETALHES DE UM MOVIMENTO
        // ===============================
        public IActionResult Details(int id)
        {
            var movimento = _context.StockMovimento
                .Include(m => m.Stock)
                    .ThenInclude(s => s.Consumivel)
                .Include(m => m.Stock)
                    .ThenInclude(s => s.Zona)
                .FirstOrDefault(m => m.Id == id);

            if (movimento == null)
                return NotFound();

            return View(movimento);
        }

        // ===============================
        // APAGAR UM MOVIMENTO
        // ===============================
        public IActionResult Delete(int id)
        {
            var movimento = _context.StockMovimento
                .Include(m => m.Stock)
                .FirstOrDefault(m => m.Id == id);

            if (movimento == null)
                return NotFound();

            return View(movimento);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var movimento = _context.StockMovimento.Find(id);

            if (movimento != null)
            {
                _context.StockMovimento.Remove(movimento);
                _context.SaveChanges();
            }

            TempData["Success"] = "Movimento removido com sucesso!";
            return RedirectToAction(nameof(Index));
        }
    }
}
