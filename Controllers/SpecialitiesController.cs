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
    public class SpecialitiesController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public SpecialitiesController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Specialities
        public async Task<IActionResult> Index()
        {
            return View(await _context.Specialities.ToListAsync());
        }

        // GET: Specialities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialities = await _context.Specialities
                .FirstOrDefaultAsync(m => m.IdEspecialidade == id);
            if (specialities == null)
            {
                return NotFound();
            }

            return View(specialities);
        }

        // GET: Specialities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Specialities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdEspecialidade,Nome,Descricao")] Specialities specialities)
        {
            if (ModelState.IsValid)
            {
                _context.Add(specialities);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(specialities);
        }

        // GET: Specialities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialities = await _context.Specialities.FindAsync(id);
            if (specialities == null)
            {
                return NotFound();
            }
            return View(specialities);
        }

        // POST: Specialities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdEspecialidade,Nome,Descricao")] Specialities specialities)
        {
            if (id != specialities.IdEspecialidade)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(specialities);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpecialitiesExists(specialities.IdEspecialidade))
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
            return View(specialities);
        }

        // GET: Specialities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialities = await _context.Specialities
                .FirstOrDefaultAsync(m => m.IdEspecialidade == id);
            if (specialities == null)
            {
                return NotFound();
            }

            return View(specialities);
        }

        // POST: Specialities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var specialities = await _context.Specialities.FindAsync(id);
            if (specialities != null)
            {
                _context.Specialities.Remove(specialities);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SpecialitiesExists(int id)
        {
            return _context.Specialities.Any(e => e.IdEspecialidade == id);
        }
    }
}
