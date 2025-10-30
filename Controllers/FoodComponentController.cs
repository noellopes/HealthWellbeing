using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Controllers
{
    public class FoodComponentController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public FoodComponentController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: FoodComponentController
        public async Task<IActionResult> Index()
        {
            var foodComponents = await _context.FoodComponent.ToListAsync();
            return View(foodComponents);
        }

        // GET: FoodComponentController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var foodComponent = await _context.FoodComponent
                .FirstOrDefaultAsync(m => m.FoodComponentId == id);
            if (foodComponent == null)
            {
                return NotFound();
            }
            return View(foodComponent);
        }

        // GET: FoodComponentController/Create
        public IActionResult Create()
        {
            return View(new FoodComponent());
        }

        // POST: FoodComponentController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FoodComponentId,Name,Description")] FoodComponent foodComponent)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(foodComponent);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Food Component created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    TempData["ErrorMessage"] = "Error creating Food Component. Please try again.";
                    return View(foodComponent);
                }
            }
            return View(foodComponent);
        }

        // GET: FoodComponentController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var foodComponent = await _context.FoodComponent.FindAsync(id);
            if (foodComponent == null)
            {
                return NotFound();
            }
            return View(foodComponent);
        }

        // POST: FoodComponentController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FoodComponentId,Name,Description")] FoodComponent foodComponent)
        {
            if (id != foodComponent.FoodComponentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(foodComponent);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Food Component updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FoodComponentExists(foodComponent.FoodComponentId))
                    {
                        TempData["ErrorMessage"] = "Food Component not found!";
                        return NotFound();
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Error updating Food Component. Please try again.";
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(foodComponent);
        }

        // GET: FoodComponentController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var foodComponent = await _context.FoodComponent
                .FirstOrDefaultAsync(m => m.FoodComponentId == id);
            if (foodComponent == null)
            {
                return NotFound();
            }
            return View(foodComponent);
        }

        // POST: FoodComponentController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var foodComponent = await _context.FoodComponent.FindAsync(id);
            if (foodComponent != null)
            {
                _context.FoodComponent.Remove(foodComponent);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Food Component deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Food Component not found!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool FoodComponentExists(int id)
        {
            return _context.FoodComponent.Any(e => e.FoodComponentId == id);
        }
    }
}
