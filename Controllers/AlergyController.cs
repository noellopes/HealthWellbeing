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
    public class AlergyController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public AlergyController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Alergies
        public async Task<IActionResult> Index()
        {
            return View(await _context.Alergy.ToListAsync());
        }

        // GET: Alergies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alergy = await _context.Alergy
                .FirstOrDefaultAsync(m => m.AlergyId == id);
            if (alergy == null)
            {
                return NotFound();
            }

            return View(alergy);
        }

        // GET: Alergies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Alergies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AlergyId,AlergyName")] Alergy alergy)
        {
            if (ModelState.IsValid)
            {
                _context.Add(alergy);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(alergy);
        }

        // GET: Alergies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alergy = await _context.Alergy.FindAsync(id);
            if (alergy == null)
            {
                return NotFound();
            }
            return View(alergy);
        }

        // POST: Alergies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AlergyId,AlergyName")] Alergy alergy)
        {
            if (id != alergy.AlergyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(alergy);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlergyExists(alergy.AlergyId))
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
            return View(alergy);
        }

        // GET: Alergies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alergy = await _context.Alergy
                .FirstOrDefaultAsync(m => m.AlergyId == id);
            if (alergy == null)
            {
                return NotFound();
            }

            return View(alergy);
        }

        // POST: Alergies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var alergy = await _context.Alergy.FindAsync(id);
            if (alergy != null)
            {
                _context.Alergy.Remove(alergy);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AlergyExists(int id)
        {
            return _context.Alergy.Any(e => e.AlergyId == id);
        }
    }
}
