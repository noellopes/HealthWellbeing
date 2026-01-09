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
    public class BeneficioController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public BeneficioController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Beneficio
        public async Task<IActionResult> Index()
        {
            return View(await _context.Beneficio.ToListAsync());
        }

        // GET: Beneficio/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var beneficio = await _context.Beneficio
                .FirstOrDefaultAsync(m => m.BeneficioId == id);
            if (beneficio == null)
            {
                return NotFound();
            }

            return View(beneficio);
        }

        // GET: Beneficio/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Beneficio/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BeneficioId,NomeBeneficio,DescricaoBeneficio")] Beneficio beneficio)
        {
            if (ModelState.IsValid)
            {
                _context.Add(beneficio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(beneficio);
        }

        // GET: Beneficio/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var beneficio = await _context.Beneficio.FindAsync(id);
            if (beneficio == null)
            {
                return NotFound();
            }
            return View(beneficio);
        }

        // POST: Beneficio/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BeneficioId,NomeBeneficio,DescricaoBeneficio")] Beneficio beneficio)
        {
            if (id != beneficio.BeneficioId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(beneficio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BeneficioExists(beneficio.BeneficioId))
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
            return View(beneficio);
        }

        // GET: Beneficio/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var beneficio = await _context.Beneficio
                .FirstOrDefaultAsync(m => m.BeneficioId == id);
            if (beneficio == null)
            {
                return NotFound();
            }

            return View(beneficio);
        }

        // POST: Beneficio/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var beneficio = await _context.Beneficio.FindAsync(id);
            if (beneficio != null)
            {
                _context.Beneficio.Remove(beneficio);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BeneficioExists(int id)
        {
            return _context.Beneficio.Any(e => e.BeneficioId == id);
        }
    }
}
