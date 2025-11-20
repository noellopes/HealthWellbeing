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
    public class PortionController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public PortionController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Portions
        public async Task<IActionResult> Index(string? search, int page = 1, int itemsPerPage = 10)
        {
            var query = _context.Portion.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();

                query = query.Where(p =>
                    p.PortionName.ToLower().Contains(search));
            }

            int totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(p => p.PortionName)
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToListAsync();

            var model = new PaginationInfo<Portion>(items, totalItems, page, itemsPerPage);

            ViewBag.Search = search;

            return View(model);
        }


        // GET: Portions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var portion = await _context.Portion
                .FirstOrDefaultAsync(m => m.PortionId == id);
            if (portion == null)
            {
                return NotFound();
            }

            return View(portion);
        }

        // GET: Portions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Portions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PortionId,PortionName")] Portion portion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(portion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(portion);
        }

        // GET: Portions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var portion = await _context.Portion.FindAsync(id);
            if (portion == null)
            {
                return NotFound();
            }
            return View(portion);
        }

        // POST: Portions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PortionId,PortionName")] Portion portion)
        {
            if (id != portion.PortionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(portion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PortionExists(portion.PortionId))
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
            return View(portion);
        }

        // GET: Portions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var portion = await _context.Portion
                .FirstOrDefaultAsync(m => m.PortionId == id);
            if (portion == null)
            {
                return NotFound();
            }

            return View(portion);
        }

        // POST: Portions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var portion = await _context.Portion.FindAsync(id);
            if (portion != null)
            {
                _context.Portion.Remove(portion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PortionExists(int id)
        {
            return _context.Portion.Any(e => e.PortionId == id);
        }
    }
}
