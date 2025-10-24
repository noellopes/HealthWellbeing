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
    public class AlimentoSubstitutoController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public AlimentoSubstitutoController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: AlimentoSubstituto
        public async Task<IActionResult> Index()
        {
            var healthWellbeingDbContext = _context.AlimentoSubstitutos.Include(a => a.AlimentoOriginal).Include(a => a.AlimentoSubstitutoRef);
            return View(await healthWellbeingDbContext.ToListAsync());
        }

        // GET: AlimentoSubstituto/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alimentoSubstituto = await _context.AlimentoSubstitutos
                .Include(a => a.AlimentoOriginal)
                .Include(a => a.AlimentoSubstitutoRef)
                .FirstOrDefaultAsync(m => m.AlimentoSubstitutoId == id);
            if (alimentoSubstituto == null)
            {
                return NotFound();
            }

            return View(alimentoSubstituto);
        }

        // GET: AlimentoSubstituto/Create
        public IActionResult Create()
        {
            ViewData["AlimentoOriginalId"] = new SelectList(_context.Alimentos, "AlimentoId", "AlimentoId");
            ViewData["AlimentoSubstitutoRefId"] = new SelectList(_context.Alimentos, "AlimentoId", "AlimentoId");
            return View();
        }

        // POST: AlimentoSubstituto/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AlimentoSubstitutoId,AlimentoOriginalId,AlimentoSubstitutoRefId,Motivo,ProporcaoEquivalente,Observacoes,FatorSimilaridade")] AlimentoSubstituto alimentoSubstituto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(alimentoSubstituto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AlimentoOriginalId"] = new SelectList(_context.Alimentos, "AlimentoId", "AlimentoId", alimentoSubstituto.AlimentoOriginalId);
            ViewData["AlimentoSubstitutoRefId"] = new SelectList(_context.Alimentos, "AlimentoId", "AlimentoId", alimentoSubstituto.AlimentoSubstitutoRefId);
            return View(alimentoSubstituto);
        }

        // GET: AlimentoSubstituto/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alimentoSubstituto = await _context.AlimentoSubstitutos.FindAsync(id);
            if (alimentoSubstituto == null)
            {
                return NotFound();
            }
            ViewData["AlimentoOriginalId"] = new SelectList(_context.Alimentos, "AlimentoId", "AlimentoId", alimentoSubstituto.AlimentoOriginalId);
            ViewData["AlimentoSubstitutoRefId"] = new SelectList(_context.Alimentos, "AlimentoId", "AlimentoId", alimentoSubstituto.AlimentoSubstitutoRefId);
            return View(alimentoSubstituto);
        }

        // POST: AlimentoSubstituto/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AlimentoSubstitutoId,AlimentoOriginalId,AlimentoSubstitutoRefId,Motivo,ProporcaoEquivalente,Observacoes,FatorSimilaridade")] AlimentoSubstituto alimentoSubstituto)
        {
            if (id != alimentoSubstituto.AlimentoSubstitutoId)
            {
                return NotFound();
            }

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
            ViewData["AlimentoOriginalId"] = new SelectList(_context.Alimentos, "AlimentoId", "AlimentoId", alimentoSubstituto.AlimentoOriginalId);
            ViewData["AlimentoSubstitutoRefId"] = new SelectList(_context.Alimentos, "AlimentoId", "AlimentoId", alimentoSubstituto.AlimentoSubstitutoRefId);
            return View(alimentoSubstituto);
        }

        // GET: AlimentoSubstituto/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alimentoSubstituto = await _context.AlimentoSubstitutos
                .Include(a => a.AlimentoOriginal)
                .Include(a => a.AlimentoSubstitutoRef)
                .FirstOrDefaultAsync(m => m.AlimentoSubstitutoId == id);
            if (alimentoSubstituto == null)
            {
                return NotFound();
            }

            return View(alimentoSubstituto);
        }

        // POST: AlimentoSubstituto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var alimentoSubstituto = await _context.AlimentoSubstitutos.FindAsync(id);
            if (alimentoSubstituto != null)
            {
                _context.AlimentoSubstitutos.Remove(alimentoSubstituto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AlimentoSubstitutoExists(int id)
        {
            return _context.AlimentoSubstitutos.Any(e => e.AlimentoSubstitutoId == id);
        }
    }
}
