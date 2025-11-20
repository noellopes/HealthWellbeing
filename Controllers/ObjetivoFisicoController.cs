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
    public class ObjetivoFisicoController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ObjetivoFisicoController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: ObjetivoFisico
        public async Task<IActionResult> Index()
        {
            return View(await _context.ObjetivoFisico.ToListAsync());
        }

        // GET: ObjetivoFisico/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var objetivoFisico = await _context.ObjetivoFisico
                .FirstOrDefaultAsync(m => m.ObjetivoFisicoId == id);
            if (objetivoFisico == null)
            {
                return NotFound();
            }

            return View(objetivoFisico);
        }

        // GET: ObjetivoFisico/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ObjetivoFisico/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ObjetivoFisicoId,NomeObjetivo")] ObjetivoFisico objetivoFisico)
        {
            if (ModelState.IsValid)
            {
                _context.Add(objetivoFisico);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(objetivoFisico);
        }

        // GET: ObjetivoFisico/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var objetivoFisico = await _context.ObjetivoFisico.FindAsync(id);
            if (objetivoFisico == null)
            {
                return NotFound();
            }
            return View(objetivoFisico);
        }

        // POST: ObjetivoFisico/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ObjetivoFisicoId,NomeObjetivo")] ObjetivoFisico objetivoFisico)
        {
            if (id != objetivoFisico.ObjetivoFisicoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(objetivoFisico);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ObjetivoFisicoExists(objetivoFisico.ObjetivoFisicoId))
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
            return View(objetivoFisico);
        }

        // GET: ObjetivoFisico/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var objetivoFisico = await _context.ObjetivoFisico
                .FirstOrDefaultAsync(m => m.ObjetivoFisicoId == id);
            if (objetivoFisico == null)
            {
                return NotFound();
            }

            return View(objetivoFisico);
        }

        // POST: ObjetivoFisico/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var objetivoFisico = await _context.ObjetivoFisico.FindAsync(id);
            if (objetivoFisico != null)
            {
                _context.ObjetivoFisico.Remove(objetivoFisico);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ObjetivoFisicoExists(int id)
        {
            return _context.ObjetivoFisico.Any(e => e.ObjetivoFisicoId == id);
        }
    }
}
