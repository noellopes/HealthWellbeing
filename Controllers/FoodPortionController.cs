using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers
{
    public class FoodPortionController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public FoodPortionController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: /FoodPortion
        public async Task<IActionResult> Index()
        {
            var list = await _context.FoodPortion
                .AsNoTracking()
                .OrderBy(x => x.FoodName)
                .ThenBy(x => x.Amount)
                .ToListAsync();

            return View(list);
        }

        // GET: /FoodPortion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var item = await _context.FoodPortion
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.FoodPortionId == id);

            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /FoodPortion/Create
        public IActionResult Create()
        {
            return View(new FoodPortion());
        }

        // POST: /FoodPortion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FoodPortion model)
        {
            // Uniqueness defensive check (também garantido por índice único na BD)
            if (await _context.FoodPortion.AnyAsync(p => p.FoodName == model.FoodName && p.Amount == model.Amount))
                ModelState.AddModelError(nameof(FoodPortion.FoodName), "This portion already exists for the given food.");

            if (!ModelState.IsValid) return View(model);

            _context.Add(model);
            await _context.SaveChangesAsync();
            TempData["Msg"] = "Record created.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /FoodPortion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var item = await _context.FoodPortion.FindAsync(id);
            if (item == null) return NotFound();

            return View(item);
        }

        // POST: /FoodPortion/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FoodPortion model)
        {
            if (id != model.FoodPortionId) return NotFound();

            if (await _context.FoodPortion
                .AnyAsync(p => p.FoodPortionId != id && p.FoodName == model.FoodName && p.Amount == model.Amount))
                ModelState.AddModelError(nameof(FoodPortion.FoodName), "This portion already exists for the given food.");

            if (!ModelState.IsValid) return View(model);

            try
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
                TempData["Msg"] = "Record updated.";
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await _context.FoodPortion.AnyAsync(e => e.FoodPortionId == id);
                if (!exists) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /FoodPortion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var item = await _context.FoodPortion
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.FoodPortionId == id);

            if (item == null) return NotFound();
            return View(item);
        }

        // POST: /FoodPortion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.FoodPortion.FindAsync(id);
            if (item != null)
            {
                _context.FoodPortion.Remove(item);
                await _context.SaveChangesAsync();
                TempData["Msg"] = "Record removed.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
