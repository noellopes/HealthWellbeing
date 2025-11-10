using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers
{
    public class ConsumivelController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ConsumivelController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Consumivel
        public async Task<IActionResult> Index()
        {
            var healthWellbeingDbContext = _context.Consumivel.Include(c => c.CategoriaConsumivel);
            return View(await healthWellbeingDbContext.ToListAsync());
        }

        // GET: Consumivel/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consumivel = await _context.Consumivel
                .Include(c => c.CategoriaConsumivel)
                .FirstOrDefaultAsync(m => m.ConsumivelId == id);
            if (consumivel == null)
            {
                return NotFound();
            }

            return View(consumivel);
        }

        // GET: Consumivel/Create
        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_context.CategoriaConsumivel, "CategoriaId", "Nome");
            return View();
        }

        // POST: Consumivel/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ConsumivelId,Nome,Descricao,CategoriaId")] Consumivel consumivel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(consumivel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.CategoriaConsumivel, "CategoriaId", "Nome", consumivel.CategoriaId);
            return View(consumivel);
        }

        // GET: Consumivel/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consumivel = await _context.Consumivel.FindAsync(id);
            if (consumivel == null)
            {
                return NotFound();
            }
            ViewData["CategoriaId"] = new SelectList(_context.CategoriaConsumivel, "CategoriaId", "Nome", consumivel.CategoriaId);
            return View(consumivel);
        }

        // POST: Consumivel/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ConsumivelId,Nome,Descricao,CategoriaId")] Consumivel consumivel)
        {
            if (id != consumivel.ConsumivelId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(consumivel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConsumivelExists(consumivel.ConsumivelId))
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
            ViewData["CategoriaId"] = new SelectList(_context.CategoriaConsumivel, "CategoriaId", "Nome", consumivel.CategoriaId);
            return View(consumivel);
        }

        // GET: Consumivel/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consumivel = await _context.Consumivel
                .Include(c => c.CategoriaConsumivel)
                .FirstOrDefaultAsync(m => m.ConsumivelId == id);
            if (consumivel == null)
            {
                return NotFound();
            }

            return View(consumivel);
        }

        // POST: Consumivel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var consumivel = await _context.Consumivel.FindAsync(id);
            if (consumivel != null)
            {
                _context.Consumivel.Remove(consumivel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ConsumivelExists(int id)
        {
            return _context.Consumivel.Any(e => e.ConsumivelId == id);
        }
    }
}
