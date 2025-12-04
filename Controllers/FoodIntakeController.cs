using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace HealthWellbeing.Controllers
{
    public class FoodIntakeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FoodIntakeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FoodIntake
        public async Task<IActionResult> Index()
        {
            var list = await _context.FoodIntake
                .OrderBy(f => f.Date)
                .ToListAsync();

            return View(list);
        }

        // POST: Toggle eaten status
        [HttpPost]
        public async Task<IActionResult> ToggleEaten(int id)
        {
            var foodIntake = await _context.FoodIntake.FindAsync(id);
            if (foodIntake == null)
            {
                return NotFound();
            }

            // Toggle true/false
            foodIntake.Eaten = !foodIntake.Eaten;

            _context.Update(foodIntake);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}