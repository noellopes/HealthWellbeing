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
    public class SalaDeExamesController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public SalaDeExamesController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: SalaDeExames
        public async Task<IActionResult> Index()
        {
            return View(await _context.SalaDeExame.ToListAsync());
        }

        // GET: SalaDeExames/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salaDeExames = await _context.SalaDeExame
                .FirstOrDefaultAsync(m => m.SalaId == id);
            if (salaDeExames == null)
            {
                return NotFound();
            }

            return View(salaDeExames);
        }

        // GET: SalaDeExames/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SalaDeExames/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SalaId,TipoSala,Laboratorio")] SalaDeExames salaDeExames)
        {
            if (ModelState.IsValid)
            {
                _context.Add(salaDeExames);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(salaDeExames);
        }

        // GET: SalaDeExames/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salaDeExames = await _context.SalaDeExame.FindAsync(id);
            if (salaDeExames == null)
            {
                return NotFound();
            }
            return View(salaDeExames);
        }

        // POST: SalaDeExames/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SalaId,TipoSala,Laboratorio")] SalaDeExames salaDeExames)
        {
            if (id != salaDeExames.SalaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(salaDeExames);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalaDeExamesExists(salaDeExames.SalaId))
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
            return View(salaDeExames);
        }

        // GET: SalaDeExames/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var salaDeExames = await _context.SalaDeExame
                .FirstOrDefaultAsync(m => m.SalaId == id);
            if (salaDeExames == null)
            {
                return NotFound();
            }

            return View(salaDeExames);
        }

        // POST: SalaDeExames/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var salaDeExames = await _context.SalaDeExame.FindAsync(id);
            if (salaDeExames != null)
            {
                _context.SalaDeExame.Remove(salaDeExames);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SalaDeExamesExists(int id)
        {
            return _context.SalaDeExame.Any(e => e.SalaId == id);
        }
    }
}
