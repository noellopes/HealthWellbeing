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
    public class ComponenteReceitaController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ComponenteReceitaController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: ComponenteReceita
        public async Task<IActionResult> Index()
        {
            var healthWellbeingDbContext = _context.ComponenteReceita.Include(c => c.Alimento);
            return View(await healthWellbeingDbContext.ToListAsync());
        }

        // GET: ComponenteReceita/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var componenteReceita = await _context.ComponenteReceita
                .Include(c => c.Alimento)
                .FirstOrDefaultAsync(m => m.ComponenteReceitaId == id);
            if (componenteReceita == null)
            {
                return NotFound();
            }

            return View(componenteReceita);
        }

        // GET: ComponenteReceita/Create
        public IActionResult Create()
        {
            ViewData["AlimentoId"] = new SelectList(_context.Alimentos, "AlimentoId", "Name");
            return View();
        }

        // POST: ComponenteReceita/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ComponenteReceitaId,AlimentoId,UnidadeMedida,Quantidade,IsOpcional")] ComponenteReceita componenteReceita)
        {
            if (ModelState.IsValid)
            {
                _context.Add(componenteReceita);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AlimentoId"] = new SelectList(_context.Alimentos, "AlimentoId", "Name", componenteReceita.AlimentoId);
            return View(componenteReceita);
        }

        // GET: ComponenteReceita/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var componenteReceita = await _context.ComponenteReceita.FindAsync(id);
            if (componenteReceita == null)
            {
                return NotFound();
            }
            ViewData["AlimentoId"] = new SelectList(_context.Alimentos, "AlimentoId", "Name", componenteReceita.AlimentoId);
            return View(componenteReceita);
        }

        // POST: ComponenteReceita/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ComponenteReceitaId,AlimentoId,UnidadeMedida,Quantidade,IsOpcional")] ComponenteReceita componenteReceita)
        {
            if (id != componenteReceita.ComponenteReceitaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(componenteReceita);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ComponenteReceitaExists(componenteReceita.ComponenteReceitaId))
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
            ViewData["AlimentoId"] = new SelectList(_context.Alimentos, "AlimentoId", "Name", componenteReceita.AlimentoId);
            return View(componenteReceita);
        }

        // GET: ComponenteReceita/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var componenteReceita = await _context.ComponenteReceita
                .Include(c => c.Alimento)
                .FirstOrDefaultAsync(m => m.ComponenteReceitaId == id);
            if (componenteReceita == null)
            {
                return NotFound();
            }

            return View(componenteReceita);
        }

        // POST: ComponenteReceita/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var componenteReceita = await _context.ComponenteReceita.FindAsync(id);
            if (componenteReceita != null)
            {
                _context.ComponenteReceita.Remove(componenteReceita);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ComponenteReceitaExists(int id)
        {
            return _context.ComponenteReceita.Any(e => e.ComponenteReceitaId == id);
        }
    }
}
