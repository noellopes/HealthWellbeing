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

        // LISTA DE MOVIMENTOS (HISTÓRICO)
        public IActionResult Index()
        {
            var movimentos = _context.StockMovimento
                .Include(m => m.Stock)
                .ThenInclude(s => s.Consumivel)
                .Include(m => m.Stock.Zona)
                .OrderByDescending(m => m.Data)
                .ToList();

            return View(movimentos);
        }

        // FORMULÁRIO DE COMPRA
        public IActionResult CreateEntrada()
        {
            ViewBag.Stocks = _context.Stock
                .Include(s => s.Consumivel)
                .Include(s => s.Zona)
                .ToList();

            return View();
        }

        // PROCESSAR COMPRA
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

            // MARCAR ESTE MOVIMENTO COMO ENTRADA
            movimento.Tipo = "Entrada";
            movimento.Data = DateTime.Now;

            // Obter o stock afetado
            var stock = _context.Stock.FirstOrDefault(s => s.StockId == movimento.StockId);

            if (stock == null)
            {
                ModelState.AddModelError("", "Stock não encontrado.");
                return View(movimento);
            }

            // Aumentar quantidade atual
            stock.QuantidadeAtual += movimento.Quantidade;
            stock.DataUltimaAtualizacao = DateTime.Now;

            // Guardar no histórico
            _context.StockMovimento.Add(movimento);
            _context.SaveChanges();

            TempData["Success"] = "Entrada registada com sucesso!";
            return RedirectToAction("Index", "Stock");
        }


        // DETALHES
        public IActionResult Details(int id)
        {
            var movimento = _context.StockMovimento
                .Include(m => m.Stock)
                .ThenInclude(s => s.Consumivel)
                .Include(m => m.Stock.Zona)
                .FirstOrDefault(m => m.Id == id);

            if (movimento == null)
                return NotFound();

            return View(movimento);
        }

        // DELETE
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

            TempData["Success"] = "Movimento removido do histórico.";
            return RedirectToAction(nameof(Index));
        }
    }
}
