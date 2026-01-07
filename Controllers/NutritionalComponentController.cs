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
    public class NutritionalComponentController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public NutritionalComponentController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: NutritionalComponents
        public async Task<IActionResult> Index(string? search, int page = 1, int itemsPerPage = 10)
        {
            var query = _context.NutritionalComponent.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();

                query = query.Where(c =>
                    c.Name.ToLower().Contains(search) ||
                    (!string.IsNullOrEmpty(c.Unit) && c.Unit.ToLower().Contains(search)) ||
                    (!string.IsNullOrEmpty(c.Basis) && c.Basis.ToLower().Contains(search)));
            }

            int totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(c => c.Name)
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToListAsync();

            var model = new PaginationInfoFoodHabits<NutritionalComponent>(items, totalItems, page, itemsPerPage);

            ViewBag.Search = search;

            return View(model);
        }


        // GET: NutritionalComponents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nutritionalComponent = await _context.NutritionalComponent
                .FirstOrDefaultAsync(m => m.NutritionalComponentId == id);
            if (nutritionalComponent == null)
            {
                return NotFound();
            }

            return View(nutritionalComponent);
        }

        // GET: NutritionalComponents/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: NutritionalComponents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NutritionalComponentId,Name,Unit,Basis")] NutritionalComponent nutritionalComponent)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nutritionalComponent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(nutritionalComponent);
        }

        // GET: NutritionalComponents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nutritionalComponent = await _context.NutritionalComponent.FindAsync(id);
            if (nutritionalComponent == null)
            {
                return NotFound();
            }
            return View(nutritionalComponent);
        }

        // POST: NutritionalComponents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NutritionalComponentId,Name,Unit,Basis")] NutritionalComponent nutritionalComponent)
        {
            if (id != nutritionalComponent.NutritionalComponentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nutritionalComponent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NutritionalComponentExists(nutritionalComponent.NutritionalComponentId))
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
            return View(nutritionalComponent);
        }

        // GET: NutritionalComponents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nutritionalComponent = await _context.NutritionalComponent
                .FirstOrDefaultAsync(m => m.NutritionalComponentId == id);
            if (nutritionalComponent == null)
            {
                return NotFound();
            }

            return View(nutritionalComponent);
        }

        // POST: NutritionalComponents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nutritionalComponent = await _context.NutritionalComponent.FindAsync(id);
            if (nutritionalComponent != null)
            {
                _context.NutritionalComponent.Remove(nutritionalComponent);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NutritionalComponentExists(int id)
        {
            return _context.NutritionalComponent.Any(e => e.NutritionalComponentId == id);
        }
    }
}
