using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellBeing.Models;
using HealthWellbeing.Data;

namespace HealthWellbeing.Controllers
{
    public class CategoriaConsumivelController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public CategoriaConsumivelController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: CategoriaConsumivel
        public async Task<IActionResult> Index()
        {
            return View(await _context.CategoriaConsumivel.ToListAsync());
        }

        // GET: CategoriaConsumivel/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoriaConsumivel = await _context.CategoriaConsumivel
                .FirstOrDefaultAsync(m => m.CategoriaId == id);
            if (categoriaConsumivel == null)
            {
                return NotFound();
            }

            return View(categoriaConsumivel);
        }

        // GET: CategoriaConsumivel/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CategoriaConsumivel/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoriaId,Nome,Descricao")] CategoriaConsumivel categoriaConsumivel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(categoriaConsumivel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(categoriaConsumivel);
        }

        // GET: CategoriaConsumivel/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoriaConsumivel = await _context.CategoriaConsumivel.FindAsync(id);
            if (categoriaConsumivel == null)
            {
                return NotFound();
            }
            return View(categoriaConsumivel);
        }

        // POST: CategoriaConsumivel/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoriaId,Nome,Descricao")] CategoriaConsumivel categoriaConsumivel)
        {
            if (id != categoriaConsumivel.CategoriaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categoriaConsumivel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoriaConsumivelExists(categoriaConsumivel.CategoriaId))
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
            return View(categoriaConsumivel);
        }

        // GET: CategoriaConsumivel/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoriaConsumivel = await _context.CategoriaConsumivel
                .FirstOrDefaultAsync(m => m.CategoriaId == id);
            if (categoriaConsumivel == null)
            {
                return NotFound();
            }

            return View(categoriaConsumivel);
        }

        // POST: CategoriaConsumivel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categoriaConsumivel = await _context.CategoriaConsumivel.FindAsync(id);
            if (categoriaConsumivel != null)
            {
                _context.CategoriaConsumivel.Remove(categoriaConsumivel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoriaConsumivelExists(int id)
        {
            return _context.CategoriaConsumivel.Any(e => e.CategoriaId == id);
        }
    }
}
