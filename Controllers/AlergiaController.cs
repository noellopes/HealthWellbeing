using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers
{
    public class AlergiaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AlergiaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AlergiaController
        public async Task<IActionResult> Index()
        {
            return View(await _context.Alergias.ToListAsync());
        }

        // GET: AlergiaController/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alergia = await _context.Alergias.FindAsync(id);
            if (alergia == null)
            {
                return NotFound();
            }

            return View(alergia);
        }

        // GET: AlergiaController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AlergiaController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AlergiaID,Nome,Descricao,Gravidade,Sintomas")] Alergia alergia)
        {
            if (ModelState.IsValid)
            {
                _context.Add(alergia);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(alergia);
        }

        // GET: AlergiaController/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alergia = await _context.Alergias.FindAsync(id);
            if (alergia == null)
            {
                return NotFound();
            }
            return View(alergia);
        }

        // POST: AlergiaController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AlergiaID,Nome,Descricao,Gravidade,Sintomas")] Alergia alergia)
        {
            if (id != alergia.AlergiaID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(alergia);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlergiaExists(alergia.AlergiaID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(alergia);
        }

        // GET: AlergiaController/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alergia = await _context.Alergias.FindAsync(id);
            if (alergia == null)
            {
                return NotFound();
            }

            return View(alergia);
        }

        // POST: AlergiaController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var alergia = await _context.Alergias.FindAsync(id);
            if (alergia != null)
            {
                _context.Alergias.Remove(alergia);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool AlergiaExists(int id)
        {
            return _context.Alergias.Any(e => e.AlergiaID == id);
        }
    }
}
