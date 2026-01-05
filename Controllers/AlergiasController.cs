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
    public class AlergiasController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public AlergiasController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Alergias
        public async Task<IActionResult> Index()
        {
            var healthWellbeingDbContext = _context.Alergia.Include(a => a.Food);
            return View(await healthWellbeingDbContext.ToListAsync());
        }

        // GET: Alergias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alergia = await _context.Alergia
                .Include(a => a.Food)
                .FirstOrDefaultAsync(m => m.AlergiaID == id);
            if (alergia == null)
            {
                return NotFound();
            }

            return View(alergia);
        }

        // GET: Alergias/Create
        public IActionResult Create()
        {
            ViewData["FoodId"] = new SelectList(_context.Set<Food>(), "FoodId", "FoodId");
            return View();
        }

        // POST: Alergias/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AlergiaID,Nome,Descricao,Gravidade,Sintomas,FoodId")] Alergia alergia)
        {
            if (ModelState.IsValid)
            {
                _context.Add(alergia);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FoodId"] = new SelectList(_context.Set<Food>(), "FoodId", "FoodId", alergia.FoodId);
            return View(alergia);
        }

        // GET: Alergias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alergia = await _context.Alergia.FindAsync(id);
            if (alergia == null)
            {
                return NotFound();
            }
            ViewData["FoodId"] = new SelectList(_context.Set<Food>(), "FoodId", "FoodId", alergia.FoodId);
            return View(alergia);
        }

        // POST: Alergias/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AlergiaID,Nome,Descricao,Gravidade,Sintomas,FoodId")] Alergia alergia)
        {
            if (id != alergia.AlergiaID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(alergia);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlergiaExists(alergia.AlergiaID))
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
            ViewData["FoodId"] = new SelectList(_context.Set<Food>(), "FoodId", "FoodId", alergia.FoodId);
            return View(alergia);
        }

        // GET: Alergias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alergia = await _context.Alergia
                .Include(a => a.Food)
                .FirstOrDefaultAsync(m => m.AlergiaID == id);
            if (alergia == null)
            {
                return NotFound();
            }

            return View(alergia);
        }

        // POST: Alergias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var alergia = await _context.Alergia.FindAsync(id);
            if (alergia != null)
            {
                _context.Alergia.Remove(alergia);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AlergiaExists(int id)
        {
            return _context.Alergia.Any(e => e.AlergiaID == id);
        }
    }
}
