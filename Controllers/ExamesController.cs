using HealthWellbeing.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoSaude.Data;
using ProjetoSaude.Models;
using System.Xml.Linq;

namespace ProjetoSaude.Controllers
{
    public class ExamesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExamesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Exames
        public async Task<IActionResult> Index()
        {
            var exames = await _context.Exames.ToListAsync();
            return View(exames);
        }

        // GET: Exames/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Exames/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Exame exame)
        {
            if (ModelState.IsValid)
            {
                _context.Add(exame);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(exame);
        }

        // GET: Exames/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var exame = await _context.Exames.FindAsync(id);
            if (exame == null) return NotFound();

            return View(exame);
        }

        // POST: Exames/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Exame exame)
        {
            if (id != exame.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(exame);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Exames.Any(e => e.Id == exame.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(exame);
        }

        // GET: Exames/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var exame = await _context.Exames.FirstOrDefaultAsync(m => m.Id == id);
            if (exame == null) return NotFound();

            return View(exame);
        }

        // POST: Exames/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exame = await _context.Exames.FindAsync(id);
            if (exame != null)
            {
                exame.Estado = "Cancelado";
                _context.Exames.Update(exame);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Exames/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var exame = await _context.Exames.FirstOrDefaultAsync(m => m.Id == id);
            if (exame == null) return NotFound();

            return View(exame);
        }
    }
}
