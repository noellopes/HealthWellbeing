using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers
{
    public class AlimentoSubstitutoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AlimentoSubstitutoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AlimentoSubstitutoController
        public async Task<IActionResult> Index()
        {
            var substitutos = await _context.AlimentoSubstitutos.ToListAsync();
            return View(substitutos);
        }

        // GET: AlimentoSubstitutoController/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var substituto = await _context.AlimentoSubstitutos
                .FirstOrDefaultAsync(m => m.AlimentoSubstitutoId == id);

            if (substituto == null)
                return NotFound();

            return View(substituto);
        }

        // GET: AlimentoSubstitutoController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AlimentoSubstitutoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AlimentoSubstitutoId,AlimentoId,SubstitutoIds,Motivo,ProporcaoEquivalente,Observacoes")] AlimentoSubstituto alimentoSubstituto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(alimentoSubstituto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(alimentoSubstituto);
        }

        // GET: AlimentoSubstitutoController/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var substituto = await _context.AlimentoSubstitutos.FindAsync(id);
            if (substituto == null)
                return NotFound();

            return View(substituto);
        }

        // POST: AlimentoSubstitutoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AlimentoSubstitutoId,AlimentoId,SubstitutoIds,Motivo,ProporcaoEquivalente,Observacoes")] AlimentoSubstituto alimentoSubstituto)
        {
            if (id != alimentoSubstituto.AlimentoSubstitutoId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(alimentoSubstituto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlimentoSubstitutoExists(alimentoSubstituto.AlimentoSubstitutoId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(alimentoSubstituto);
        }

        // GET: AlimentoSubstitutoController/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var substituto = await _context.AlimentoSubstitutos
                .FirstOrDefaultAsync(m => m.AlimentoSubstitutoId == id);

            if (substituto == null)
                return NotFound();

            return View(substituto);
        }

        // POST: AlimentoSubstitutoController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var substituto = await _context.AlimentoSubstitutos.FindAsync(id);
            if (substituto != null)
            {
                _context.AlimentoSubstitutos.Remove(substituto);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool AlimentoSubstitutoExists(int id)
        {
            return _context.AlimentoSubstitutos.Any(e => e.AlimentoSubstitutoId == id);
        }
    }
}
