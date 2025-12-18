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
    public class LimitacaoMedicasController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public LimitacaoMedicasController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: LimitacaoMedicas
        public async Task<IActionResult> Index(int page = 1)
        {
            var query = _context.LimitacaoMedica.AsQueryable();

            int totalItems = await query.CountAsync();

            var pagination = new PaginationInfo<LimitacaoMedica>(page, totalItems);

            pagination.Items = await query
                .OrderBy(l => l.LimitacaoMedicaId) // ajuste conforme necessário
                .Skip(pagination.ItemsToSkip)
                .Take(pagination.ItemsPerPage)
                .ToListAsync();

            return View(pagination);
        }

        // GET: LimitacaoMedicas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var limitacaoMedica = await _context.LimitacaoMedica
                .FirstOrDefaultAsync(m => m.LimitacaoMedicaId == id);
            if (limitacaoMedica == null)
            {
                return NotFound();
            }

            return View(limitacaoMedica);
        }

        // GET: LimitacaoMedicas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LimitacaoMedicas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LimitacaoMedicaId,Descricao,TipoLimitacao,Observacoes")] LimitacaoMedica limitacaoMedica)
        {
            if (ModelState.IsValid)
            {
                _context.Add(limitacaoMedica);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(limitacaoMedica);
        }

        // GET: LimitacaoMedicas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var limitacaoMedica = await _context.LimitacaoMedica.FindAsync(id);
            if (limitacaoMedica == null)
            {
                return NotFound();
            }
            return View(limitacaoMedica);
        }

        // POST: LimitacaoMedicas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LimitacaoMedicaId,Descricao,TipoLimitacao,Observacoes")] LimitacaoMedica limitacaoMedica)
        {
            if (id != limitacaoMedica.LimitacaoMedicaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(limitacaoMedica);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LimitacaoMedicaExists(limitacaoMedica.LimitacaoMedicaId))
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
            return View(limitacaoMedica);
        }

        // GET: LimitacaoMedicas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var limitacaoMedica = await _context.LimitacaoMedica
                .FirstOrDefaultAsync(m => m.LimitacaoMedicaId == id);
            if (limitacaoMedica == null)
            {
                return NotFound();
            }

            return View(limitacaoMedica);
        }

        // POST: LimitacaoMedicas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var limitacaoMedica = await _context.LimitacaoMedica.FindAsync(id);
            if (limitacaoMedica != null)
            {
                _context.LimitacaoMedica.Remove(limitacaoMedica);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LimitacaoMedicaExists(int id)
        {
            return _context.LimitacaoMedica.Any(e => e.LimitacaoMedicaId == id);
        }
    }
}
