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
    public class AuditoriaConsumivelController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public AuditoriaConsumivelController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: AuditoriaConsumivel
        public async Task<IActionResult> Index()
        {
            var healthWellbeingDbContext = _context.AuditoriaConsumivel.Include(a => a.Consumivel).Include(a => a.Sala);
            return View(await healthWellbeingDbContext.ToListAsync());
        }

        // GET: AuditoriaConsumivel/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auditoriaConsumivel = await _context.AuditoriaConsumivel
                .Include(a => a.Consumivel)
                .Include(a => a.Sala)
                .FirstOrDefaultAsync(m => m.AuditoriaConsumivelId == id);
            if (auditoriaConsumivel == null)
            {
                return NotFound();
            }

            return View(auditoriaConsumivel);
        }

        // GET: AuditoriaConsumivel/Create
        public IActionResult Create()
        {
            ViewData["ConsumivelID"] = new SelectList(_context.Consumivel, "ConsumivelId", "Nome");
            ViewData["SalaId"] = new SelectList(_context.Set<Sala>(), "SalaId", "TipoSala");
            return View();
        }

        // POST: AuditoriaConsumivel/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AuditoriaConsumivelId,SalaId,ConsumivelID,QuantidadeUsada,DataConsumo")] AuditoriaConsumivel auditoriaConsumivel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(auditoriaConsumivel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ConsumivelID"] = new SelectList(_context.Consumivel, "ConsumivelId", "Nome", auditoriaConsumivel.ConsumivelID);
            ViewData["SalaId"] = new SelectList(_context.Set<Sala>(), "SalaId", "TipoSala", auditoriaConsumivel.SalaId);
            return View(auditoriaConsumivel);
        }

        // GET: AuditoriaConsumivel/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auditoriaConsumivel = await _context.AuditoriaConsumivel.FindAsync(id);
            if (auditoriaConsumivel == null)
            {
                return NotFound();
            }
            ViewData["ConsumivelID"] = new SelectList(_context.Consumivel, "ConsumivelId", "Nome", auditoriaConsumivel.ConsumivelID);
            ViewData["SalaId"] = new SelectList(_context.Set<Sala>(), "SalaId", "TipoSala", auditoriaConsumivel.SalaId);
            return View(auditoriaConsumivel);
        }

        // POST: AuditoriaConsumivel/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AuditoriaConsumivelId,SalaId,ConsumivelID,QuantidadeUsada,DataConsumo")] AuditoriaConsumivel auditoriaConsumivel)
        {
            if (id != auditoriaConsumivel.AuditoriaConsumivelId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(auditoriaConsumivel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuditoriaConsumivelExists(auditoriaConsumivel.AuditoriaConsumivelId))
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
            ViewData["ConsumivelID"] = new SelectList(_context.Consumivel, "ConsumivelId", "Nome", auditoriaConsumivel.ConsumivelID);
            ViewData["SalaId"] = new SelectList(_context.Set<Sala>(), "SalaId", "TipoSala", auditoriaConsumivel.SalaId);
            return View(auditoriaConsumivel);
        }

        // GET: AuditoriaConsumivel/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auditoriaConsumivel = await _context.AuditoriaConsumivel
                .Include(a => a.Consumivel)
                .Include(a => a.Sala)
                .FirstOrDefaultAsync(m => m.AuditoriaConsumivelId == id);
            if (auditoriaConsumivel == null)
            {
                return NotFound();
            }

            return View(auditoriaConsumivel);
        }

        // POST: AuditoriaConsumivel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var auditoriaConsumivel = await _context.AuditoriaConsumivel.FindAsync(id);
            if (auditoriaConsumivel != null)
            {
                _context.AuditoriaConsumivel.Remove(auditoriaConsumivel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AuditoriaConsumivelExists(int id)
        {
            return _context.AuditoriaConsumivel.Any(e => e.AuditoriaConsumivelId == id);
        }
    }
}
