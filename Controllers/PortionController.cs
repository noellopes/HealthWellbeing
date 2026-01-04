using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;

namespace HealthWellbeing.Controllers
{
    public class PortionController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public PortionController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Portion
        public async Task<IActionResult> Index(int page = 1, string search = "")
        {
            const int pageSize = 10;
            search ??= "";

            var query = _context.Portion.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(p => p.PortionName.ToLower().Contains(s));
            }

            var totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(p => p.PortionName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Search = search;

            return View(new PaginationInfo<Portion>(items, totalItems, page, pageSize));
        }

        // GET: Portion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var portion = await _context.Portion
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.PortionId == id);

            if (portion == null) return NotFound();

            return View(portion);
        }

        // GET: Portion/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Portion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PortionId,PortionName")] Portion portion)
        {
            if (!ModelState.IsValid) return View(portion);

            _context.Add(portion);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Portion created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Portion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var portion = await _context.Portion.FindAsync(id);
            if (portion == null) return NotFound();

            return View(portion);
        }

        // POST: Portion/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PortionId,PortionName")] Portion portion)
        {
            if (id != portion.PortionId) return NotFound();
            if (!ModelState.IsValid) return View(portion);

            try
            {
                _context.Update(portion);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Portion updated successfully.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PortionExists(portion.PortionId)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Portion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var portion = await _context.Portion
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.PortionId == id);

            if (portion == null) return NotFound();

            return View(portion);
        }

        // POST: Portion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var portion = await _context.Portion.FindAsync(id);
            if (portion == null) return RedirectToAction(nameof(Index));

            try
            {
                _context.Portion.Remove(portion);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Portion deleted successfully.";
            }
            catch
            {
                TempData["Error"] = "This portion cannot be deleted because it is being used.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PortionExists(int id)
        {
            return _context.Portion.Any(e => e.PortionId == id);
        }
    }
}
