using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Controllers
{
    public class StockController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public StockController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // ==============================================
        // GARANTIR QUE EXISTE STOCK PARA CADA CONSUMÍVEL
        // ==============================================
        private void GarantirStockBase()
        {
            var consumiveis = _context.Consumivel.ToList();
            var zonas = _context.ZonaArmazenamento.ToList();

            if (!consumiveis.Any() || !zonas.Any())
                return;

            int totalZonas = zonas.Count;

            foreach (var c in consumiveis)
            {
                bool existeStock = _context.Stock.Any(s => s.ConsumivelID == c.ConsumivelId);

                if (!existeStock)
                {
                    var zonaEscolhida = zonas[(c.ConsumivelId - 1) % totalZonas];

                    _context.Stock.Add(new Stock
                    {
                        ConsumivelID = c.ConsumivelId,
                        ZonaID = zonaEscolhida.Id,

                        // Estes valores passam a vir do modelo Consumivel
                        QuantidadeAtual = c.QuantidadeAtual,
                        QuantidadeMinima = c.QuantidadeMinima,
                        QuantidadeMaxima = c.QuantidadeMaxima,

                        DataUltimaAtualizacao = DateTime.Now
                    });
                }
            }

            _context.SaveChanges();
        }


        // ==============================================
        // INDEX COM PAGINAÇÃO
        // ==============================================
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

            // FILTROS
            if (!string.IsNullOrWhiteSpace(searchNome))
                query = query.Where(s => s.Consumivel.Nome.Contains(searchNome));

            if (!string.IsNullOrWhiteSpace(searchZona))
                query = query.Where(s => s.Zona.Nome.Contains(searchZona));

            if (stockCritico)
                query = query.Where(s => s.QuantidadeAtual < s.QuantidadeMinima);

            // PAGINAÇÃO
            int totalItems = query.Count();
            var pagination = new PaginationInfo<Stock>(page, totalItems, itemsPerPage: 10);

            pagination.Items = query
                .OrderBy(s => s.Consumivel.Nome)
                .Skip(pagination.ItemsToSkip)
                .Take(pagination.ItemsPerPage)
                .ToList();

            return View(pagination);
        }



        // ==============================================
        // CREATE GET
        // ==============================================
        public IActionResult Create()
        {
            ViewBag.Consumiveis = _context.Consumivel.ToList();
            ViewBag.Zonas = _context.ZonaArmazenamento.ToList();
            return View();
        }



        // ==============================================
        // CREATE POST
        // ==============================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Stock stock)
        {
            ModelState.Remove("Consumivel");
            ModelState.Remove("Zona");

            if (!ModelState.IsValid)
            {
                ViewBag.Consumiveis = _context.Consumivel.ToList();
                ViewBag.Zonas = _context.ZonaArmazenamento.ToList();
                return View(stock);
            }

            // Os limites vêm SEMPRE do consumível
            var cons = _context.Consumivel.Find(stock.ConsumivelID);
            stock.QuantidadeMinima = cons.QuantidadeMinima;
            stock.QuantidadeMaxima = cons.QuantidadeMaxima;

            stock.DataUltimaAtualizacao = DateTime.Now;
            _context.Stock.Add(stock);
            _context.SaveChanges();

            TempData["Success"] = "Stock criado com sucesso!";
            return RedirectToAction(nameof(Index));
        }



        // ==============================================
        // DETAILS
        // ==============================================
        public IActionResult Details(int id)
        {
            var stock = _context.Stock
                .Include(s => s.Consumivel)
                .Include(s => s.Zona)
                .FirstOrDefault(s => s.StockId == id);

            if (stock == null)
            {
                TempData["Error"] = "O stock não foi encontrado.";
                return RedirectToAction(nameof(Index));
            }

            return View(stock);
        }



        // ==============================================
        // EDIT GET
        // ==============================================
        public IActionResult Edit(int id)
        {
            var stock = _context.Stock
                .Include(s => s.Consumivel)
                .Include(s => s.Zona)
                .FirstOrDefault(s => s.StockId == id);

            if (stock == null)
                return RedirectToAction(nameof(Index));

            ViewBag.Consumiveis = _context.Consumivel.ToList();
            ViewBag.Zonas = _context.ZonaArmazenamento.ToList();

            return View(stock);
        }



        // ==============================================
        // EDIT POST
        // ==============================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Stock stock)
        {
            ModelState.Remove("Consumivel");
            ModelState.Remove("Zona");

            if (!ModelState.IsValid)
            {
                ViewBag.Consumiveis = _context.Consumivel.ToList();
                ViewBag.Zonas = _context.ZonaArmazenamento.ToList();
                return View(stock);
            }

            var original = _context.Stock.FirstOrDefault(s => s.StockId == id);
            if (original == null)
                return RedirectToAction(nameof(Index));

            // Limites vêm sempre do Consumível
            var cons = _context.Consumivel.Find(stock.ConsumivelID);
            original.QuantidadeMinima = cons.QuantidadeMinima;
            original.QuantidadeMaxima = cons.QuantidadeMaxima;

            original.DataUltimaAtualizacao = DateTime.Now;

            _context.SaveChanges();

            TempData["Success"] = "Stock atualizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }



        // ==============================================
        // DELETE
        // ==============================================
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

            TempData["Success"] = "Stock removido com sucesso!";
            return RedirectToAction(nameof(Index));
        }
    }
}
