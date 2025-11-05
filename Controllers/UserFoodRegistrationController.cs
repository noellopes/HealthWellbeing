using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HealthWellbeing.Controllers
{
    public class UserFoodRegistrationController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public UserFoodRegistrationController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // LIST
        public async Task<IActionResult> Index()
        {
            var registrations = await _context.UserFoodRegistration
                .Include(u => u.Client)
                .ToListAsync();
            return View(registrations);
        }

        // DETAILS
        public async Task<IActionResult> Details(int id)
        {
            var registration = await _context.UserFoodRegistration
                .Include(u => u.Client)
                .FirstOrDefaultAsync(u => u.UserFoodRegId == id);

            if (registration == null)
            {
                TempData["ErrorMessage"] = "Registration not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(registration);
        }

        // CREATE GET
        public IActionResult Create()
        {
            ViewBag.Clients = new SelectList(_context.Client.ToList(), "ClientId", "Name");
            return View();
        }

        // CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserFoodRegistration registration)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Clients = new SelectList(_context.Client.ToList(), "ClientId", "Name");
                TempData["ErrorMessage"] = "Please fix the errors and try again.";
                return View(registration);
            }

            try
            {
                _context.Add(registration);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Registration saved successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ViewBag.Clients = new SelectList(_context.Client.ToList(), "ClientId", "Name");
                TempData["ErrorMessage"] = "Error saving registration.";
                return View(registration);
            }
        }

        // EDIT GET
        public async Task<IActionResult> Edit(int id)
        {
            var registration = await _context.UserFoodRegistration.FindAsync(id);
            if (registration == null)
            {
                TempData["ErrorMessage"] = "Registration not found.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Clients = new SelectList(_context.Client.ToList(), "ClientId", "Name", registration.ClientId);
            return View(registration);
        }

        // EDIT POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserFoodRegistration registration)
        {
            if (id != registration.UserFoodRegId)
            {
                TempData["ErrorMessage"] = "Invalid registration.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Clients = new SelectList(_context.Client.ToList(), "ClientId", "Name", registration.ClientId);
                TempData["ErrorMessage"] = "Please fix the errors and try again.";
                return View(registration);
            }

            try
            {
                var regInDb = await _context.UserFoodRegistration.FindAsync(id);
                if (regInDb == null)
                {
                    TempData["ErrorMessage"] = "Registration not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Atualizar campos
                regInDb.ClientId = registration.ClientId;
                regInDb.MealDateTime = registration.MealDateTime;
                regInDb.MealType = registration.MealType;
                regInDb.FoodName = registration.FoodName;
                regInDb.Quantity = registration.Quantity;
                regInDb.Notes = registration.Notes;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Registration updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ViewBag.Clients = new SelectList(_context.Client.ToList(), "ClientId", "Name", registration.ClientId);
                TempData["ErrorMessage"] = "Error updating registration.";
                return View(registration);
            }
        }

        // DELETE GET
        public async Task<IActionResult> Delete(int id)
        {
            var registration = await _context.UserFoodRegistration
                .Include(u => u.Client)
                .FirstOrDefaultAsync(u => u.UserFoodRegId == id);

            if (registration == null)
            {
                TempData["ErrorMessage"] = "Registration not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(registration);
        }

        // DELETE POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var registration = await _context.UserFoodRegistration.FindAsync(id);
            if (registration != null)
            {
                try
                {
                    _context.UserFoodRegistration.Remove(registration);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Registration deleted successfully!";
                }
                catch
                {
                    TempData["ErrorMessage"] = "Error deleting registration.";
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
