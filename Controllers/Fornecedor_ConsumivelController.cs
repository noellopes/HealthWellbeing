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
    public class Fornecedor_ConsumivelController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public Fornecedor_ConsumivelController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Fornecedor_Consumivel
        public async Task<IActionResult> Index()
        {
            var healthWellbeingDbContext = _context.Fornecedor_Consumivel.Include(f => f.Consumivel).Include(f => f.Fornecedor);
            return View(await healthWellbeingDbContext.ToListAsync());
        }

        // GET: Fornecedor_Consumivel/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fornecedor_Consumivel = await _context.Fornecedor_Consumivel
                .Include(f => f.Consumivel)
                .Include(f => f.Fornecedor)
                .FirstOrDefaultAsync(m => m.FornecedorConsumivelId == id);
            if (fornecedor_Consumivel == null)
            {
                return NotFound();
            }

            return View(fornecedor_Consumivel);
        }

        // GET: Fornecedor_Consumivel/Create
        public IActionResult Create()
        {
            ViewData["ConsumivelId"] = new SelectList(_context.Consumivel, "ConsumivelId", "Nome");
            ViewData["FornecedorId"] = new SelectList(_context.Fornecedor, "FornecedorId", "NomeEmpresa");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FornecedorConsumivelId,FornecedorId,ConsumivelId,TempoEntrega,Preco")] Fornecedor_Consumivel fornecedor_Consumivel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fornecedor_Consumivel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ConsumivelId"] = new SelectList(_context.Consumivel, "ConsumivelId", "Nome", fornecedor_Consumivel.ConsumivelId);
            ViewData["FornecedorId"] = new SelectList(_context.Fornecedor, "FornecedorId", "NomeEmpresa", fornecedor_Consumivel.FornecedorId);
            return View(fornecedor_Consumivel);
        }

        // GET: Fornecedor_Consumivel/Edit/5
        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fornecedor_Consumivel = await _context.Fornecedor_Consumivel
                .Include(fc => fc.Fornecedor)
                .Include(fc => fc.Consumivel)
                .FirstOrDefaultAsync(fc => fc.FornecedorConsumivelId == id);

            if (fornecedor_Consumivel == null)
            {
                return NotFound();
            }

            return View(fornecedor_Consumivel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FornecedorConsumivelId,FornecedorId,ConsumivelId,TempoEntrega,Preco")] Fornecedor_Consumivel fornecedor_Consumivel)
        {
            if (id != fornecedor_Consumivel.FornecedorConsumivelId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fornecedor_Consumivel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Fornecedor_ConsumivelExists(fornecedor_Consumivel.FornecedorConsumivelId))
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
            ViewData["ConsumivelId"] = new SelectList(_context.Consumivel, "ConsumivelId", "Nome", fornecedor_Consumivel.ConsumivelId);
            ViewData["FornecedorId"] = new SelectList(_context.Fornecedor, "FornecedorId", "NomeEmpresa", fornecedor_Consumivel.FornecedorId);
            return View(fornecedor_Consumivel);
        }

        // GET: Fornecedor_Consumivel/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fornecedor_Consumivel = await _context.Fornecedor_Consumivel
            .Include(fc => fc.Fornecedor)
            .Include(fc => fc.Consumivel)
            .FirstOrDefaultAsync(fc => fc.FornecedorConsumivelId == id);

            if (fornecedor_Consumivel == null)
            {
                return NotFound();
            }

            return View(fornecedor_Consumivel);
        }

        // POST: Fornecedor_Consumivel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fornecedor_Consumivel = await _context.Fornecedor_Consumivel.FindAsync(id);
            if (fornecedor_Consumivel != null)
            {
                _context.Fornecedor_Consumivel.Remove(fornecedor_Consumivel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Fornecedor_ConsumivelExists(int id)
        {
            return _context.Fornecedor_Consumivel.Any(e => e.FornecedorConsumivelId == id);
        }
    }
}
