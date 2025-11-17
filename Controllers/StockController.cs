using HealthWellbeing.Data;
using HealthWellbeing.Models;
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

        // =====================================================
        // Garante que existe Stock criado para cada Consumível
        // =====================================================
        private void GarantirStockBase()
        {
            var consumiveis = _context.Consumivel.ToList();
            var zonas = _context.ZonaArmazenamento.ToList();

            // Se faltar consumíveis ou zonas, não faz nada
            if (!consumiveis.Any() || !zonas.Any())
                return;

            var zonaDefault = zonas.First();

            foreach (var c in consumiveis)
            {
                bool existeStock = _context.Stock.Any(s => s.ConsumivelID == c.ConsumivelId);

                if (!existeStock)
                {
                    _context.Stock.Add(new Stock
                    {
                        ConsumivelID = c.ConsumivelId,
                        ZonaID = zonaDefault.Id,
                        QuantidadeAtual = 0,
                        QuantidadeMinima = 5,
                        QuantidadeMaxima = 100,
                        DataUltimaAtualizacao = DateTime.Now
                    });
                }
            }

            _context.SaveChanges();
        }

        // ===========================
        // LISTA + PESQUISA
        // ===========================
        public IActionResult Index(string searchNome = "", string searchZona = "", bool stockCritico = false)
        {
            GarantirStockBase();

            var query = _context.Stock
                .Include(s => s.Consumivel)
                .Include(s => s.Zona)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchNome))
                query = query.Where(s => s.Consumivel.Nome.Contains(searchNome));

            if (!string.IsNullOrWhiteSpace(searchZona))
                query = query.Where(s => s.Zona.Nome.Contains(searchZona));

            if (stockCritico)
                query = query.Where(s => s.QuantidadeAtual < s.QuantidadeMinima);

            ViewBag.SearchNome = searchNome;
            ViewBag.SearchZona = searchZona;
            ViewBag.StockCritico = stockCritico;

            return View(query.ToList());
        }

        // ===========================
        // CREATE GET
        // ===========================
        public IActionResult Create()
        {
            ViewBag.Consumiveis = _context.Consumivel.ToList();
            ViewBag.Zonas = _context.ZonaArmazenamento.ToList();
            return View();
        }

        // ===========================
        // CREATE POST
        // ===========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Stock stock)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Consumiveis = _context.Consumivel.ToList();
                ViewBag.Zonas = _context.ZonaArmazenamento.ToList();
                return View(stock);
            }

            stock.DataUltimaAtualizacao = DateTime.Now;

            _context.Stock.Add(stock);
            _context.SaveChanges();

            TempData["Success"] = "Stock criado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // ===========================
        // DETAILS
        // ===========================
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

        // ===========================
        // EDIT GET
        // ===========================
        public IActionResult Edit(int id)
        {
            var stock = _context.Stock.Find(id);

            if (stock == null)
            {
                TempData["Error"] = "O stock não existe.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Consumiveis = _context.Consumivel.ToList();
            ViewBag.Zonas = _context.ZonaArmazenamento.ToList();

            return View(stock);
        }

        // ===========================
        // EDIT POST
        // ===========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Stock stock)
        {
            if (id != stock.StockId)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Consumiveis = _context.Consumivel.ToList();
                ViewBag.Zonas = _context.ZonaArmazenamento.ToList();
                return View(stock);
            }

            stock.DataUltimaAtualizacao = DateTime.Now;

            _context.Update(stock);
            _context.SaveChanges();

            TempData["Success"] = "Stock atualizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // ===========================
        // DELETE GET
        // ===========================
        public IActionResult Delete(int id)
        {
            var stock = _context.Stock
                .Include(s => s.Consumivel)
                .Include(s => s.Zona)
                .FirstOrDefault(s => s.StockId == id);

            if (stock == null)
            {
                TempData["Error"] = "O stock já não existe.";
                return RedirectToAction(nameof(Index));
            }

            return View(stock);
        }

        // ===========================
        // DELETE POST
        // ===========================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var stock = _context.Stock.Find(id);

            if (stock != null)
            {
                _context.Stock.Remove(stock);
                _context.SaveChanges();
            }

            TempData["Success"] = "Stock removido com sucesso!";
            return RedirectToAction(nameof(Index));
        }
    }
}
