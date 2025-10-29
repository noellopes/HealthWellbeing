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
    public class TipoExercicioController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public TipoExercicioController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: TipoExercicios
        public async Task<IActionResult> Index()
        {
            return View(await _context.TipoExercicio.ToListAsync());
        }

        // GET: TipoExercicios/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoExercicio = await _context.TipoExercicio
                .FirstOrDefaultAsync(m => m.TipoExercicioId == id);
            if (tipoExercicio == null)
            {
                return NotFound();
            }

            return View(tipoExercicio);
        }

        // GET: TipoExercicios/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TipoExercicios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TipoExercicioId,CategoriaTipoExercicios,DescricaoTipoExercicios,NivelDificuldadeTipoExercicios,BeneficioTipoExercicios,GruposMuscularesTrabalhadosTipoExercicios")] TipoExercicio tipoExercicio)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tipoExercicio);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tipoExercicio);
        }

        // GET: TipoExercicios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoExercicio = await _context.TipoExercicio.FindAsync(id);
            if (tipoExercicio == null)
            {
                return NotFound();
            }
            return View(tipoExercicio);
        }

        // POST: TipoExercicios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TipoExercicioId,CategoriaTipoExercicios,DescricaoTipoExercicios,NivelDificuldadeTipoExercicios,BeneficioTipoExercicios,GruposMuscularesTrabalhadosTipoExercicios")] TipoExercicio tipoExercicio)
        {
            if (id != tipoExercicio.TipoExercicioId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipoExercicio);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoExercicioExists(tipoExercicio.TipoExercicioId))
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
            return View(tipoExercicio);
        }

        // GET: TipoExercicios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoExercicio = await _context.TipoExercicio
                .FirstOrDefaultAsync(m => m.TipoExercicioId == id);
            if (tipoExercicio == null)
            {
                return NotFound();
            }

            return View(tipoExercicio);
        }

        // POST: TipoExercicios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipoExercicio = await _context.TipoExercicio.FindAsync(id);
            if (tipoExercicio != null)
            {
                _context.TipoExercicio.Remove(tipoExercicio);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TipoExercicioExists(int id)
        {
            return _context.TipoExercicio.Any(e => e.TipoExercicioId == id);
        }
    }
}
