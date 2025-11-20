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

            if (!consumiveis.Any() || !zonas.Any())
                return;

            int totalZonas = zonas.Count;

            foreach (var c in consumiveis)
            {
                bool existeStock = _context.Stock.Any(s => s.ConsumivelID == c.ConsumivelId);

                if (!existeStock)
                {
                    // Atribui uma zona diferente para cada consumível (em ciclo)
                    var zonaEscolhida = zonas[(c.ConsumivelId - 1) % totalZonas];

                    _context.Stock.Add(new Stock
                    {
                        ConsumivelID = c.ConsumivelId,
                        ZonaID = zonaEscolhida.Id,
                        QuantidadeAtual = 0,
                        QuantidadeMinima = 5,
                        QuantidadeMaxima = (int)zonaEscolhida.CapacidadeMaxima,
                        DataUltimaAtualizacao = DateTime.Now
                    });
                }
            }

            _context.SaveChanges();
        }

        // Opcional: usar apenas uma vez para corrigir dados antigos
        private void CorrigirZonasExistentes()
        {
            var stocks = _context.Stock.ToList();
            var zonas = _context.ZonaArmazenamento.ToList();

            if (!stocks.Any() || !zonas.Any())
                return;

            int totalZonas = zonas.Count;

            for (int i = 0; i < stocks.Count; i++)
            {
                var stock = stocks[i];

                // Distribuir zonas sequencialmente
                var zonaAtual = zonas[i % totalZonas];
                stock.ZonaID = zonaAtual.Id;

                // Atualizar quantidade máxima com base na nova zona
                stock.QuantidadeMaxima = (int)zonaAtual.CapacidadeMaxima;

                stock.DataUltimaAtualizacao = DateTime.Now;
            }

            _context.SaveChanges();
        }

        // ===========================
        // LISTA + PESQUISA
        // ===========================
        public IActionResult Index(string searchNome = "", string searchZona = "", bool stockCritico = false)
        {
            GarantirStockBase();
            // Se precisares de corrigir dados antigos, descomenta a linha abaixo,
            // carrega a Index uma vez e depois comenta de novo.
            // CorrigirZonasExistentes();

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

            // Buscar zona selecionada
            var zona = _context.ZonaArmazenamento.FirstOrDefault(z => z.Id == stock.ZonaID);

            if (zona == null)
            {
                ModelState.AddModelError("", "A zona selecionada não existe.");
                ViewBag.Consumiveis = _context.Consumivel.ToList();
                ViewBag.Zonas = _context.ZonaArmazenamento.ToList();
                return View(stock);
            }

            // Atribui automaticamente a capacidade máxima da zona ao stock
            stock.QuantidadeMaxima = (int)zona.CapacidadeMaxima;
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
            var stock = _context.Stock
                .Include(s => s.Consumivel)
                .Include(s => s.Zona)
                .FirstOrDefault(s => s.StockId == id);

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
            // Os campos complexos (Consumivel/Zona/DataUltimaAtualizacao/QuantidadeMaxima)
            // não são enviados no form quando estão disabled. Removemos a validação
            // para esses campos para que o ModelState não bloqueie o POST.
            ModelState.Remove("Consumivel");
            ModelState.Remove("Zona");
            ModelState.Remove("QuantidadeMaxima");
            ModelState.Remove("DataUltimaAtualizacao");

            if (!ModelState.IsValid)
            {
                ViewBag.Consumiveis = _context.Consumivel.ToList();
                ViewBag.Zonas = _context.ZonaArmazenamento.ToList();
                return View(stock);
            }

            var original = _context.Stock.FirstOrDefault(s => s.StockId == id);
            if (original == null)
                return NotFound();

            // Atualizar apenas os campos editáveis
            original.QuantidadeAtual = stock.QuantidadeAtual;
            original.QuantidadeMinima = stock.QuantidadeMinima;
            original.DataUltimaAtualizacao = DateTime.Now;

            // Atualiza QuantidadeMaxima com base na zona actual guardada
            var zona = _context.ZonaArmazenamento.FirstOrDefault(z => z.Id == original.ZonaID);
            if (zona != null)
                original.QuantidadeMaxima = (int)zona.CapacidadeMaxima;

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
