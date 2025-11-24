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

        // ============================
        // GARANTIR STOCK BASE
        // ============================
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
                        QuantidadeAtual = 0,
                        QuantidadeMinima = 5,
                        QuantidadeMaxima = 100, // valor padrão, agora EDITÁVEL
                        DataUltimaAtualizacao = DateTime.Now
                    });
                }
            }

            _context.SaveChanges();
        }

        private void CorrigirStockBase()
        {
            var stocks = _context.Stock.ToList();

            foreach (var s in stocks)
            {
                // Define os teus valores-base corretos
                s.QuantidadeMinima = 50;
                s.QuantidadeMaxima = 150;

                // Se a quantidade atual for maior que a nova máxima, ajusta
                if (s.QuantidadeAtual > s.QuantidadeMaxima)
                    s.QuantidadeAtual = s.QuantidadeMaxima;

                s.DataUltimaAtualizacao = DateTime.Now;
            }

            _context.SaveChanges();
        }

        private void CorrigirQuantidadeAtual()
        {
            var stocks = _context.Stock.ToList();

            foreach (var s in stocks)
            {
                s.QuantidadeAtual = 0;   // Reset
                s.QuantidadeMinima = 50; // Ou outro valor base que queres
                s.QuantidadeMaxima = 150;
                s.DataUltimaAtualizacao = DateTime.Now;
            }

            _context.SaveChanges();
        }



        // ============================
        // INDEX
        // ============================
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

            return View(query.ToList());
        }

        // ============================
        // CREATE GET
        // ============================
        public IActionResult Create()
        {
            ViewBag.Consumiveis = _context.Consumivel.ToList();
            ViewBag.Zonas = _context.ZonaArmazenamento.ToList();
            return View();
        }

        // ============================
        // CREATE POST
        // ============================
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

            stock.DataUltimaAtualizacao = DateTime.Now;

            _context.Stock.Add(stock);
            _context.SaveChanges();

            TempData["Success"] = "Stock criado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // ============================
        // DETAILS
        // ============================
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

        // ============================
        // EDIT GET
        // ============================
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

        // ============================
        // EDIT POST
        // ============================
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

            // VALIDACOES
            if (stock.QuantidadeMinima < 0)
                ModelState.AddModelError("QuantidadeMinima", "A quantidade mínima não pode ser negativa.");

            if (stock.QuantidadeMaxima < stock.QuantidadeMinima)
                ModelState.AddModelError("QuantidadeMaxima", "A quantidade máxima não pode ser menor que a mínima.");

            if (original.QuantidadeAtual > stock.QuantidadeMaxima)
                ModelState.AddModelError("QuantidadeMaxima",
                    "A quantidade atual é maior que a nova quantidade máxima.");

            if (!ModelState.IsValid)
            {
                ViewBag.Consumiveis = _context.Consumivel.ToList();
                ViewBag.Zonas = _context.ZonaArmazenamento.ToList();
                return View(stock);
            }

            // SALVAR ALTERACOES
            original.QuantidadeMinima = stock.QuantidadeMinima;
            original.QuantidadeMaxima = stock.QuantidadeMaxima;
            original.DataUltimaAtualizacao = DateTime.Now;

            _context.SaveChanges();

            TempData["Success"] = "Stock atualizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // ============================
        // DELETE
        // ============================
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
