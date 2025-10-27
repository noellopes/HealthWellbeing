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
    public class ProfissionalExecutantesController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ProfissionalExecutantesController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: ProfissionalExecutantes
        public async Task<IActionResult> Index()
        {
            return View(await _context.ProfissionalExecutante.ToListAsync());
        }

        // GET: ProfissionalExecutantes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var profissionalExecutante = await _context.ProfissionalExecutante
                .FirstOrDefaultAsync(m => m.ProfissionalExecutanteId == id);
            if (profissionalExecutante == null)
            {
                return NotFound();
            }

            return View(profissionalExecutante);
        }

        // GET: ProfissionalExecutantes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ProfissionalExecutantes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProfissionalExecutanteId,Nome,Funcao,Telefone,Email")] ProfissionalExecutante profissionalExecutante)
        {
            if (ModelState.IsValid)
            {
                _context.Add(profissionalExecutante);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(profissionalExecutante);
        }

        // GET: ProfissionalExecutantes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var profissionalExecutante = await _context.ProfissionalExecutante.FindAsync(id);
            if (profissionalExecutante == null)
            {
                return NotFound();
            }
            return View(profissionalExecutante);
        }

        // POST: ProfissionalExecutantes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProfissionalExecutanteId,Nome,Funcao,Telefone,Email")] ProfissionalExecutante profissionalExecutante)
        {
            if (id != profissionalExecutante.ProfissionalExecutanteId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(profissionalExecutante);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProfissionalExecutanteExists(profissionalExecutante.ProfissionalExecutanteId))
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
            return View(profissionalExecutante);
        }

        // GET: ProfissionalExecutantes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var profissionalExecutante = await _context.ProfissionalExecutante
                .FirstOrDefaultAsync(m => m.ProfissionalExecutanteId == id);
            if (profissionalExecutante == null)
            {
                return NotFound();
            }

            return View(profissionalExecutante);
        }

        // POST: ProfissionalExecutantes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var profissionalExecutante = await _context.ProfissionalExecutante.FindAsync(id);
            if (profissionalExecutante != null)
            {
                _context.ProfissionalExecutante.Remove(profissionalExecutante);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProfissionalExecutanteExists(int id)
        {
            return _context.ProfissionalExecutante.Any(e => e.ProfissionalExecutanteId == id);
        }
    }
}
