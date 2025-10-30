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
    public class TerapeutaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TerapeutaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Terapeuta
        public async Task<IActionResult> Index()
        {
            return View(await _context.Terapeutas.ToListAsync());
        }

        // GET: Terapeuta/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var terapeutaModel = await _context.Terapeutas
                .FirstOrDefaultAsync(m => m.TerapeutaId == id);
            if (terapeutaModel == null)
            {
                return NotFound();
            }

            return View(terapeutaModel);
        }

        // GET: Terapeuta/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Terapeuta/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TerapeutaId,Nome,Especialidade,Telefone,Email,AnosExperiencia,Ativo")] TerapeutaModel terapeutaModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(terapeutaModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(terapeutaModel);
        }

        // GET: Terapeuta/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var terapeutaModel = await _context.Terapeutas.FindAsync(id);
            if (terapeutaModel == null)
            {
                return NotFound();
            }
            return View(terapeutaModel);
        }

        // POST: Terapeuta/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TerapeutaId,Nome,Especialidade,Telefone,Email,AnosExperiencia,Ativo")] TerapeutaModel terapeutaModel)
        {
            if (id != terapeutaModel.TerapeutaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(terapeutaModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TerapeutaModelExists(terapeutaModel.TerapeutaId))
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
            return View(terapeutaModel);
        }

        // GET: Terapeuta/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var terapeutaModel = await _context.Terapeutas
                .FirstOrDefaultAsync(m => m.TerapeutaId == id);
            if (terapeutaModel == null)
            {
                return NotFound();
            }

            return View(terapeutaModel);
        }

        // POST: Terapeuta/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var terapeutaModel = await _context.Terapeutas.FindAsync(id);
            if (terapeutaModel != null)
            {
                _context.Terapeutas.Remove(terapeutaModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TerapeutaModelExists(int id)
        {
            return _context.Terapeutas.Any(e => e.TerapeutaId == id);
        }
    }
}
