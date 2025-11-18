using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthWellbeing.Controllers
{
    [Authorize(Roles = "GrupoMuscular")]
    public class GrupoMuscularController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public GrupoMuscularController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: GrupoMuscular
        public async Task<IActionResult> Index()
        {
            return View(await _context.GrupoMuscular.ToListAsync());
        }

        // GET: GrupoMuscular/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var grupoMuscular = await _context.GrupoMuscular
                .FirstOrDefaultAsync(m => m.GrupoMuscularId == id);
            if (grupoMuscular == null)
            {
                return NotFound();
            }

            return View(grupoMuscular);
        }

        // GET: GrupoMuscular/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GrupoMuscular/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GrupoMuscularId,GrupoMuscularNome,LocalizacaoCorporal")] GrupoMuscular grupoMuscular)
        {
            if (ModelState.IsValid)
            {
                _context.Add(grupoMuscular);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(grupoMuscular);
        }

        // GET: GrupoMuscular/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var grupoMuscular = await _context.GrupoMuscular.FindAsync(id);
            if (grupoMuscular == null)
            {
                return NotFound();
            }
            return View(grupoMuscular);
        }

        // POST: GrupoMuscular/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GrupoMuscularId,GrupoMuscularNome,LocalizacaoCorporal")] GrupoMuscular grupoMuscular)
        {
            if (id != grupoMuscular.GrupoMuscularId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(grupoMuscular);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GrupoMuscularExists(grupoMuscular.GrupoMuscularId))
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
            return View(grupoMuscular);
        }

        // GET: GrupoMuscular/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var grupoMuscular = await _context.GrupoMuscular
                .FirstOrDefaultAsync(m => m.GrupoMuscularId == id);
            if (grupoMuscular == null)
            {
                return NotFound();
            }

            return View(grupoMuscular);
        }

        // POST: GrupoMuscular/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var grupoMuscular = await _context.GrupoMuscular.FindAsync(id);
            if (grupoMuscular != null)
            {
                _context.GrupoMuscular.Remove(grupoMuscular);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GrupoMuscularExists(int id)
        {
            return _context.GrupoMuscular.Any(e => e.GrupoMuscularId == id);
        }
    }
}
