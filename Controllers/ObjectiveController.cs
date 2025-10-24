using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers
{
    public class ObjectiveController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        private static readonly string[] _categories = new[]
        {
            "Lose weight","Gain weight","Build muscle","Lose fat","Maintain weight","Improve endurance"
        };

        public ObjectiveController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        private void LoadCategories(object? selected = null)
            => ViewBag.Categories = new SelectList(_categories, selected);

        // GET: /Objective
        public async Task<IActionResult> Index()
        {
            var data = await _context.Objective.AsNoTracking().OrderBy(o => o.Name).ToListAsync();
            return View(data);
        }

        // GET: /Objective/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var item = await _context.Objective.AsNoTracking().FirstOrDefaultAsync(o => o.ObjectiveId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /Objective/Create
        public IActionResult Create()
        {
            LoadCategories();
            return View(new Objective());
        }

        // POST: /Objective/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Objective obj)
        {
            // Server-side uniqueness check (defensive - Remote handles client side)
            if (await _context.Objective.AnyAsync(o => o.Name == obj.Name && o.Category == obj.Category))
                ModelState.AddModelError(nameof(Objective.Name), "An objective with this name already exists in the selected category.");

            if (!ModelState.IsValid)
            {
                LoadCategories(obj.Category);
                return View(obj);
            }

            _context.Add(obj);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Objective/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var item = await _context.Objective.FindAsync(id);
            if (item == null) return NotFound();
            LoadCategories(item.Category);
            return View(item);
        }

        // POST: /Objective/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Objective obj)
        {
            if (id != obj.ObjectiveId) return NotFound();

            // Server-side uniqueness (ignore the same row)
            if (await _context.Objective.AnyAsync(o => o.ObjectiveId != id && o.Name == obj.Name && o.Category == obj.Category))
                ModelState.AddModelError(nameof(Objective.Name), "An objective with this name already exists in the selected category.");

            if (!ModelState.IsValid)
            {
                LoadCategories(obj.Category);
                return View(obj);
            }

            try
            {
                _context.Update(obj);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Objective.AnyAsync(e => e.ObjectiveId == id)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Objective/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var item = await _context.Objective.AsNoTracking().FirstOrDefaultAsync(o => o.ObjectiveId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // POST: /Objective/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.Objective.FindAsync(id);
            if (item != null)
            {
                _context.Objective.Remove(item);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Remote validation endpoint (AJAX) for Name uniqueness within Category
        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> CheckNameUnique(string name, string category, int? objectiveId)
        {
            var exists = await _context.Objective
                .AnyAsync(o => o.Name == name && o.Category == category && o.ObjectiveId != objectiveId);

            return Json(!exists); // true => valid
        }
    }
}
