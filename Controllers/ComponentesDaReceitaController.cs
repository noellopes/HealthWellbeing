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
    public class ComponentesDaReceitaController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ComponentesDaReceitaController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: ComponentesDaReceita
        public async Task<IActionResult> Index()
        {
            var healthWellbeingDbContext = _context.ComponentesDaReceita.Include(c => c.Receita);
            return View(await healthWellbeingDbContext.ToListAsync());
        }

        // GET: ComponentesDaReceita/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var componentesDaReceita = await _context.ComponentesDaReceita
                .Include(c => c.Receita)
                .FirstOrDefaultAsync(m => m.ComponentesDaReceitaId == id);
            if (componentesDaReceita == null)
            {
                return NotFound();
            }

            return View(componentesDaReceita);
        }

        // GET: ComponentesDaReceita/Create
        public IActionResult Create()
        {
            ViewData["ReceitaId"] = new SelectList(_context.Receita, "ReceitaId", "ReceitaId");
            return View();
        }

        // POST: ComponentesDaReceita/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ComponentesDaReceitaId,ReceitaId,Nome,Descricao,UnidadeMedida,Quantidade,Calorias,Proteinas,HidratosCarbono,Gorduras,IsOpcional")] ComponentesDaReceita componentesDaReceita)
        {
            if (ModelState.IsValid)
            {
                _context.Add(componentesDaReceita);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ReceitaId"] = new SelectList(_context.Receita, "ReceitaId", "ReceitaId", componentesDaReceita.ReceitaId);
            return View(componentesDaReceita);
        }

        // GET: ComponentesDaReceita/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var componentesDaReceita = await _context.ComponentesDaReceita.FindAsync(id);
            if (componentesDaReceita == null)
            {
                return NotFound();
            }
            ViewData["ReceitaId"] = new SelectList(_context.Receita, "ReceitaId", "ReceitaId", componentesDaReceita.ReceitaId);
            return View(componentesDaReceita);
        }

        // POST: ComponentesDaReceita/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ComponentesDaReceitaId,ReceitaId,Nome,Descricao,UnidadeMedida,Quantidade,Calorias,Proteinas,HidratosCarbono,Gorduras,IsOpcional")] ComponentesDaReceita componentesDaReceita)
        {
            if (id != componentesDaReceita.ComponentesDaReceitaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(componentesDaReceita);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ComponentesDaReceitaExists(componentesDaReceita.ComponentesDaReceitaId))
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
            ViewData["ReceitaId"] = new SelectList(_context.Receita, "ReceitaId", "ReceitaId", componentesDaReceita.ReceitaId);
            return View(componentesDaReceita);
        }

        // GET: ComponentesDaReceita/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var componentesDaReceita = await _context.ComponentesDaReceita
                .Include(c => c.Receita)
                .FirstOrDefaultAsync(m => m.ComponentesDaReceitaId == id);
            if (componentesDaReceita == null)
            {
                return NotFound();
            }

            return View(componentesDaReceita);
        }

        // POST: ComponentesDaReceita/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var componentesDaReceita = await _context.ComponentesDaReceita.FindAsync(id);
            if (componentesDaReceita != null)
            {
                _context.ComponentesDaReceita.Remove(componentesDaReceita);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ComponentesDaReceitaExists(int id)
        {
            return _context.ComponentesDaReceita.Any(e => e.ComponentesDaReceitaId == id);
        }
    }
}
