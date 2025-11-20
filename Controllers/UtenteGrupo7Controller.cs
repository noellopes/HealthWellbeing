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
    public class UtenteGrupo7Controller : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public UtenteGrupo7Controller(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: UtenteGrupo7
        public async Task<IActionResult> Index()
        {
            return View(await _context.UtenteGrupo7.ToListAsync());
        }

        // GET: UtenteGrupo7/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utenteGrupo7 = await _context.UtenteGrupo7
                .FirstOrDefaultAsync(m => m.UtenteGrupo7Id == id);
            if (utenteGrupo7 == null)
            {
                return NotFound();
            }

            return View(utenteGrupo7);
        }

        // GET: UtenteGrupo7/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UtenteGrupo7/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UtenteGrupo7Id,Nome")] UtenteGrupo7 utenteGrupo7)
        {
            if (ModelState.IsValid)
            {
                _context.Add(utenteGrupo7);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(utenteGrupo7);
        }

        // GET: UtenteGrupo7/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utenteGrupo7 = await _context.UtenteGrupo7.FindAsync(id);
            if (utenteGrupo7 == null)
            {
                return NotFound();
            }
            return View(utenteGrupo7);
        }

        // POST: UtenteGrupo7/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UtenteGrupo7Id,Nome")] UtenteGrupo7 utenteGrupo7)
        {
            if (id != utenteGrupo7.UtenteGrupo7Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(utenteGrupo7);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UtenteGrupo7Exists(utenteGrupo7.UtenteGrupo7Id))
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
            return View(utenteGrupo7);
        }

        // GET: UtenteGrupo7/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utenteGrupo7 = await _context.UtenteGrupo7
                .FirstOrDefaultAsync(m => m.UtenteGrupo7Id == id);
            if (utenteGrupo7 == null)
            {
                return NotFound();
            }

            return View(utenteGrupo7);
        }

        // POST: UtenteGrupo7/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var utenteGrupo7 = await _context.UtenteGrupo7.FindAsync(id);
            if (utenteGrupo7 != null)
            {
                _context.UtenteGrupo7.Remove(utenteGrupo7);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UtenteGrupo7Exists(int id)
        {
            return _context.UtenteGrupo7.Any(e => e.UtenteGrupo7Id == id);
        }
    }
}
