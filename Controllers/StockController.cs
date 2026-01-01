using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Controllers
{
    public class StockController : Controller
    {
        private static readonly Random rnd = new Random();
        private readonly HealthWellbeingDbContext _context;

        public StockController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // 🔒 GARANTE STOCK = ESPELHO DO CONSUMÍVEL
        // =====================================================
        private void GarantirStockBase()
        {
            var consumiveis = _context.Consumivel.ToList();
            var zonas = _context.ZonaArmazenamento.ToList();

            if (!consumiveis.Any() || !zonas.Any())
                return;

            foreach (var c in consumiveis)
            {
                var stock = _context.Stock
                    .FirstOrDefault(s => s.ConsumivelID == c.ConsumivelId);

                if (stock == null)
                {
                    // cria stock
                    var zonaAleatoria = zonas[rnd.Next(zonas.Count)];

                    _context.Stock.Add(new Stock
                    {
                        ConsumivelID = c.ConsumivelId,
                        ZonaID = zonaAleatoria.Id,
                        QuantidadeAtual = c.QuantidadeAtual,
                        QuantidadeMinima = c.QuantidadeMinima,
                        QuantidadeMaxima = c.QuantidadeMaxima,
                        UsaValoresDoConsumivel = true,
                        DataUltimaAtualizacao = DateTime.Now
                    });
                }
                else
                {
                    // 🔁 sincroniza stock existente
                    stock.QuantidadeAtual = c.QuantidadeAtual;
                    stock.QuantidadeMinima = c.QuantidadeMinima;
                    stock.QuantidadeMaxima = c.QuantidadeMaxima;
                    stock.UsaValoresDoConsumivel = true;
                    stock.DataUltimaAtualizacao = DateTime.Now;
                }
            }

            _context.SaveChanges();
        }

        // =====================================================
        // 🔄 RESET TOTAL DO STOCK
        // =====================================================
        public IActionResult ResetStock()
        {
            _context.Stock.RemoveRange(_context.Stock);
            _context.SaveChanges();

            GarantirStockBase();

            TempData["Success"] = "Stock reiniciado com base nos consumíveis.";
            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // INDEX
        // =====================================================
        public IActionResult Index(
            int page = 1,
            string searchNome = "",
            string searchZona = "",
            bool stockCritico = false)
        {
            GarantirStockBase();

            var query = _context.Stock
                .Include(s => s.Consumivel)
                .Include(s => s.Zona)
                .AsQueryable();

            if (stockCritico)
            {
                query = query.Where(s =>
                    s.QuantidadeAtual <= s.Consumivel.QuantidadeMinima + 10
                );

                ViewBag.StockCritico = true;
            }


            if (!string.IsNullOrWhiteSpace(searchNome))
            {
                query = query.Where(s => s.Consumivel.Nome.Contains(searchNome));
                ViewBag.SearchNome = searchNome;
            }

            if (!string.IsNullOrWhiteSpace(searchZona))
            {
                query = query.Where(s => s.Zona.Nome.Contains(searchZona));
                ViewBag.SearchZona = searchZona;
            }

            int totalItems = query.Count();
            var pagination = new PaginationInfo<Stock>(page, totalItems, 10);

            pagination.Items = query
                .OrderBy(s => s.Consumivel.Nome)
                .Skip(pagination.ItemsToSkip)
                .Take(pagination.ItemsPerPage)
                .ToList();

            return View(pagination);
        }

        // =====================================================
        // CREATE ❌ (DESATIVADO)
        // =====================================================
        public IActionResult Create()
        {
            TempData["Error"] = "O stock é criado automaticamente a partir dos consumíveis.";
            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // EDIT → APENAS ZONA
        // =====================================================
        public IActionResult Edit(int id)
        {
            var stock = _context.Stock
                .Include(s => s.Consumivel)
                .Include(s => s.Zona)
                .FirstOrDefault(s => s.StockId == id);

            if (stock == null)
                return RedirectToAction(nameof(Index));

            ViewBag.Zonas = _context.ZonaArmazenamento.ToList();
            return View(stock);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, int ZonaID)
        {
            var stock = _context.Stock.FirstOrDefault(s => s.StockId == id);

            if (stock == null)
                return RedirectToAction(nameof(Index));

            stock.ZonaID = ZonaID;
            stock.DataUltimaAtualizacao = DateTime.Now;

            _context.SaveChanges();

            TempData["Success"] = "Zona do stock atualizada.";
            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // DELETE
        // =====================================================
        public IActionResult Delete(int id)
        {
            var stock = _context.Stock
                .Include(s => s.Consumivel)
                .Include(s => s.Zona)
                .FirstOrDefault(s => s.StockId == id);

            if (stock == null)
                return RedirectToAction(nameof(Index));

            return View(stock);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var stock = _context.Stock.Find(id);
            if (stock != null)
                _context.Stock.Remove(stock);

            _context.SaveChanges();

            TempData["Success"] = "Stock removido.";
            return RedirectToAction(nameof(Index));
        }
    }
}
