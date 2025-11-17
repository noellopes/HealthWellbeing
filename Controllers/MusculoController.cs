using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthWellbeing.Controllers
{
    public class MusculoController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public MusculoController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Musculo
        public async Task<IActionResult> Index(int page = 1)
        {
            var musculoContext = _context.Musculo.Include(b => b.GrupoMuscular);

            int numberMusculos = await musculoContext.CountAsync();

            var musculoInfo = new PaginationInfoExercicios<Musculo>(page, numberMusculos);

            musculoInfo.Items = await musculoContext
                .OrderBy(m => m.Nome_Musculo)
                .Skip(musculoInfo.ItemsToSkip)
                .Take(musculoInfo.ItemsPerPage)
                .ToListAsync();

            return View(musculoInfo);
        }

        // GET: Musculo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var musculo = await _context.Musculo
                .Include(m => m.GrupoMuscular)
                .FirstOrDefaultAsync(m => m.MusculoId == id);
            if (musculo == null)
            {
                return NotFound();
            }

            return View(musculo);
        }

        // GET: Musculo/Create
        public IActionResult Create()
        {
            ViewData["GrupoMuscularId"] = new SelectList(_context.Set<GrupoMuscular>(), "GrupoMuscularId", "GrupoMuscularNome");
            return View();
        }

        // POST: Musculo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MusculoId,Nome_Musculo,GrupoMuscularId")] Musculo musculo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(musculo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GrupoMuscularId"] = new SelectList(_context.Set<GrupoMuscular>(), "GrupoMuscularId", "GrupoMuscularNome", musculo.GrupoMuscularId);
            return View(musculo);
        }

        // GET: Musculo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var musculo = await _context.Musculo.FindAsync(id);
            if (musculo == null)
            {
                return NotFound();
            }
            ViewData["GrupoMuscularId"] = new SelectList(_context.Set<GrupoMuscular>(), "GrupoMuscularId", "GrupoMuscularNome", musculo.GrupoMuscularId);
            return View(musculo);
        }

        // POST: Musculo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MusculoId,Nome_Musculo,GrupoMuscularId")] Musculo musculo)
        {
            if (id != musculo.MusculoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(musculo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MusculoExists(musculo.MusculoId))
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
            ViewData["GrupoMuscularId"] = new SelectList(_context.Set<GrupoMuscular>(), "GrupoMuscularId", "GrupoMuscularNome", musculo.GrupoMuscularId);
            return View(musculo);
        }

        // GET: Musculo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var musculo = await _context.Musculo
                .Include(m => m.GrupoMuscular)
                .FirstOrDefaultAsync(m => m.MusculoId == id);
            if (musculo == null)
            {
                return NotFound();
            }

            return View(musculo);
        }

        // POST: Musculo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var musculo = await _context.Musculo.FindAsync(id);
            if (musculo != null)
            {
                _context.Musculo.Remove(musculo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MusculoExists(int id)
        {
            return _context.Musculo.Any(e => e.MusculoId == id);
        }
    }
}
