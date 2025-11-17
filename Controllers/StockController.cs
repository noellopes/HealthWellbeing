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

        // LISTA DE STOCK
        public IActionResult Index()
        {
            var stock = _context.Stock
                .Include(s => s.Consumivel)
                .Include(s => s.Zona)
                .ToList();

            return View(stock);
        }

        // CRIAR NOVO STOCK (GET)
        public IActionResult Create()
        {
            ViewBag.Consumiveis = _context.Consumivel.ToList();
            ViewBag.Zonas = _context.ZonaArmazenamento.ToList();
            return View();
        }

        // CRIAR NOVO STOCK (POST)
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

        // DETALHES
        public IActionResult Details(int id)
        {
            var stock = _context.Stock
                .Include(s => s.Consumivel)
                .Include(s => s.Zona)
                .FirstOrDefault(s => s.StockId == id);

            if (stock == null)
                return NotFound();

            return View(stock);
        }

        // EDITAR (GET)
        public IActionResult Edit(int id)
        {
            var stock = _context.Stock.Find(id);

            if (stock == null)
                return NotFound();

            ViewBag.Consumiveis = _context.Consumivel.ToList();
            ViewBag.Zonas = _context.ZonaArmazenamento.ToList();

            return View(stock);
        }

        // EDITAR (POST)
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

        // ELIMINAR (GET)
        public IActionResult Delete(int id)
        {
            var stock = _context.Stock
                .Include(s => s.Consumivel)
                .Include(s => s.Zona)
                .FirstOrDefault(s => s.StockId == id);

            if (stock == null)
                return NotFound();

            return View(stock);
        }

        // ELIMINAR (POST)
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

