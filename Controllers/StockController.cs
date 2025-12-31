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

        private void GarantirStockBase()
        {
            var consumiveis = _context.Consumivel.ToList();
            var zonas = _context.ZonaArmazenamento.ToList();

            if (!consumiveis.Any() || !zonas.Any())
                return;

            foreach (var c in consumiveis)
            {
                bool existe = _context.Stock.Any(s => s.ConsumivelID == c.ConsumivelId);

                if (!existe)
                {
                    // usar o Random GLOBAL definido no topo do controller
                    var zonaAleatoria = zonas[rnd.Next(zonas.Count)];

                    _context.Stock.Add(new Stock
                    {
                        ConsumivelID = c.ConsumivelId,
                        ZonaID = zonaAleatoria.Id,

                        // ✅ AGORA COMEÇA COM A QUANTIDADE MÍNIMA DO CONSUMÍVEL
                        QuantidadeAtual = c.QuantidadeMinima,

                        QuantidadeMinima = c.QuantidadeMinima,
                        QuantidadeMaxima = c.QuantidadeMaxima,
                        UsaValoresDoConsumivel = true,
                        DataUltimaAtualizacao = DateTime.Now
                    });
                }
            }

            _context.SaveChanges();
        }


        // ===========================================
        // 🔄 RESET TOTAL DO STOCK
        // ===========================================
        public IActionResult ResetStock()
        {
            // 1. Apaga tudo da tabela Stock
            var todos = _context.Stock.ToList();
            _context.Stock.RemoveRange(todos);
            _context.SaveChanges();

            // 2. Recria o stock base COMO DEVE SER
            GarantirStockBase();

            TempData["Success"] = "O stock foi totalmente reiniciado com sucesso!";
            return RedirectToAction("Index");
        }


        // ==============================================
        // INDEX COM FILTROS E PAGINAÇÃO
        // ==============================================
        public IActionResult Index(
            int page = 1,
            string searchNome = "",
            string searchZona = "",
            bool stockCritico = false)

        {

            

            var query = _context.Stock
                .Include(s => s.Consumivel)
                .Include(s => s.Zona)
                .AsQueryable();

            // Stock crítico → usando Min DO STOCK (independente)
            const int margemCritica = 10;

            if (stockCritico)
            {
                query = query.Where(s =>
                    s.QuantidadeAtual <=
                    (
                        (s.UsaValoresDoConsumivel
                            ? s.Consumivel.QuantidadeMinima
                            : s.QuantidadeMinima)
                        + margemCritica
                    )
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
            var pagination = new PaginationInfo<Stock>(page, totalItems, itemsPerPage: 10);

            pagination.Items = query
                .OrderBy(s => s.Consumivel.Nome)
                .Skip(pagination.ItemsToSkip)
                .Take(pagination.ItemsPerPage)
                .ToList();

            return View(pagination);
        }

        // ==============================================
        // CREATE GET — inicializa stock independente
        // ==============================================
        public IActionResult Create(int? consumivelId, int? zonaId)
        {
            ViewBag.Consumiveis = _context.Consumivel.ToList();
            ViewBag.Zonas = _context.ZonaArmazenamento.ToList();

            var stock = new Stock
            {
                QuantidadeAtual = 0,
                DataUltimaAtualizacao = DateTime.Now,
                UsaValoresDoConsumivel = false  // <- fundamental
            };

            if (consumivelId.HasValue)
            {
                var consumivel = _context.Consumivel
                    .FirstOrDefault(c => c.ConsumivelId == consumivelId.Value);

                if (consumivel != null)
                {
                    stock.ConsumivelID = consumivel.ConsumivelId;
                    stock.QuantidadeMinima = consumivel.QuantidadeMinima;
                    stock.QuantidadeMaxima = consumivel.QuantidadeMaxima;
                }
            }

            if (zonaId.HasValue)
            {
                var zona = _context.ZonaArmazenamento
                    .FirstOrDefault(z => z.Id == zonaId.Value);

                if (zona != null)
                {
                    stock.ZonaID = zona.Id;
                }
            }

            return View(stock);
        }

        // ==============================================
        // CREATE POST — stock independente
        // ==============================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Stock stock)
        {
            ModelState.Remove("Consumivel");
            ModelState.Remove("Zona");

            var consumivel = _context.Consumivel.FirstOrDefault(c => c.ConsumivelId == stock.ConsumivelID);
            if (consumivel == null)
            {
                ModelState.AddModelError("", "Consumível inválido.");
            }
            else
            {
                int somaExistente = _context.Stock
                    .Where(s => s.ConsumivelID == stock.ConsumivelID)
                    .Sum(s => s.QuantidadeAtual);

                int somaFinal = somaExistente + stock.QuantidadeAtual;

                if (somaFinal > consumivel.QuantidadeMaxima)
                {
                    ModelState.AddModelError("QuantidadeAtual",
                        $"A quantidade total ({somaFinal}) excede o máximo ({consumivel.QuantidadeMaxima}).");
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Consumiveis = _context.Consumivel.ToList();
                ViewBag.Zonas = _context.ZonaArmazenamento.ToList();
                return View(stock);
            }

            stock.UsaValoresDoConsumivel = false;
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

            var original = _context.Stock.FirstOrDefault(s => s.StockId == id);
            if (original == null)
                return RedirectToAction(nameof(Index));

            var consumivel = _context.Consumivel
                .FirstOrDefault(c => c.ConsumivelId == original.ConsumivelID);

            int somaOutros = _context.Stock
                .Where(s => s.ConsumivelID == original.ConsumivelID && s.StockId != id)
                .Sum(s => s.QuantidadeAtual);

            int somaFinal = somaOutros + stock.QuantidadeAtual;

            if (somaFinal > consumivel.QuantidadeMaxima)
            {
                ModelState.AddModelError("QuantidadeAtual",
                    $"A quantidade total ({somaFinal}) excede o máximo ({consumivel.QuantidadeMaxima}).");

                ViewBag.Consumiveis = _context.Consumivel.ToList();
                ViewBag.Zonas = _context.ZonaArmazenamento.ToList();
                return View(stock);
            }

            original.ZonaID = stock.ZonaID;
            original.QuantidadeAtual = stock.QuantidadeAtual;
            original.DataUltimaAtualizacao = DateTime.Now;

            // Atualiza min/max só se for stock independente
            if (!original.UsaValoresDoConsumivel)
            {
                original.QuantidadeMinima = stock.QuantidadeMinima;
                original.QuantidadeMaxima = stock.QuantidadeMaxima;
            }

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
