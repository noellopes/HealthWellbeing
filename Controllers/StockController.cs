using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers
{
    public class StockController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public StockController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Stock
        public async Task<IActionResult> Index()
        {
            var stocks = await _context.Stock
                .OrderByDescending(s => s.DataUltimaAtualizacao)
                .ToListAsync();

            // Passar dicionários para converter IDs em nomes na View
            ViewBag.Consumiveis = _context.Consumivel
                .ToDictionary(c => c.ConsumivelId, c => c.Nome);

            ViewBag.Zonas = _context.ZonaArmazenamento
                .ToDictionary(z => z.Id, z => z.Nome);

            return View(stocks);
        }

        // GET: Stock/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Stock/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StockId,ConsumivelID,ZonaID,QuantidadeAtual,QuantidadeMinima,QuantidadeMaxima")] Stock stock)
        {
            if (ModelState.IsValid)
            {
                stock.DataUltimaAtualizacao = DateTime.Now;
                _context.Add(stock);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(stock);
        }

        // GET: Stock/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var stock = await _context.Stock.FindAsync(id);
            if (stock == null)
                return NotFound();

            return View(stock);
        }

        // POST: Stock/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StockId,ConsumivelID,ZonaID,QuantidadeAtual,QuantidadeMinima,QuantidadeMaxima")] Stock stock)
        {
            if (id != stock.StockId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    stock.DataUltimaAtualizacao = DateTime.Now;
                    _context.Update(stock);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StockExists(stock.StockId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(stock);
        }

        // GET: Stock/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var stock = await _context.Stock
                .FirstOrDefaultAsync(m => m.StockId == id);

            if (stock == null)
                return NotFound();

            return View(stock);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var stock = await _context.Stock.FindAsync(id);
            if (stock != null)
                _context.Stock.Remove(stock);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool StockExists(int id)
        {
            return _context.Stock.Any(e => e.StockId == id);
        }
    }
}
