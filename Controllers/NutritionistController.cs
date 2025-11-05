using System.Linq;
using System.Threading.Tasks;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Controllers
{
    public class NutritionistController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public NutritionistController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: /Nutritionist
        public async Task<IActionResult> Index()
        {
            var list = await _context.Nutritionist
                .AsNoTracking()
                .OrderBy(n => n.Name)
                .ToListAsync();

            return View(list);
        }

        // GET: /Nutritionist/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var item = await _context.Nutritionist
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.NutritionistId == id);

            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /Nutritionist/Create
        public IActionResult Create() => View();

        // POST: /Nutritionist/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Nutritionist model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                model.Name = model.Name.Trim();
                _context.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Nutritionist created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                TempData["Error"] = $"Could not create: {ex.GetBaseException().Message}";
                return View(model);
            }
        }

        // GET: /Nutritionist/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var model = await _context.Nutritionist.FindAsync(id);
            if (model == null) return NotFound();
            return View(model);
        }

        // POST: /Nutritionist/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NutritionistId,Name")] Nutritionist model)
        {
            if (id != model.NutritionistId) return NotFound();
            if (!ModelState.IsValid) return View(model);

            try
            {
                var existing = await _context.Nutritionist
                    .FirstOrDefaultAsync(n => n.NutritionistId == id);

                if (existing == null) return NotFound();

                existing.Name = model.Name.Trim();
                await _context.SaveChangesAsync();

                TempData["Success"] = "Nutritionist updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Nutritionist.AnyAsync(n => n.NutritionistId == id))
                    return NotFound();

                TempData["Error"] = "Concurrency error. Reload and try again.";
                return View(model);
            }
            catch (DbUpdateException ex)
            {
                TempData["Error"] = $"Could not save changes: {ex.GetBaseException().Message}";
                return View(model);
            }
        }

        // GET: /Nutritionist/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var model = await _context.Nutritionist
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.NutritionistId == id);

            if (model == null) return NotFound();
            return View(model);
        }

        // POST: /Nutritionist/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entity = await _context.Nutritionist
                .FirstOrDefaultAsync(n => n.NutritionistId == id);

            if (entity == null) return NotFound();

            try
            {
                _context.Nutritionist.Remove(entity);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Nutritionist deleted successfully.";
            }
            catch (DbUpdateException ex)
            {
                TempData["Error"] = $"Could not delete: {ex.GetBaseException().Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
