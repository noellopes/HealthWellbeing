using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthWellbeing.Controllers
{
    public class UserFoodRegistrationController : Controller
    {
        private readonly ILogger<UserFoodRegistrationController> _logger;

        public UserFoodRegistrationController(ILogger<UserFoodRegistrationController> logger)
        {
            _logger = logger;
        }

        // In-memory database simulation
        private static List<UserFoodRegistration> _registrations = new();
        private static int _nextId = 1;

        // READ - List all
        public IActionResult Index()
        {
            return View(_registrations);
        }

        // READ - Details
        public IActionResult Details(int id)
        {
            var registration = _registrations.FirstOrDefault(r => r.Id == id);
            if (registration == null) return NotFound();
            return View(registration);
        }

        // CREATE - Form
        public IActionResult Create()
        {
            return View();
        }

        // CREATE - Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(UserFoodRegistration registration)
        {
            if (!ModelState.IsValid)
                return View(registration);

            registration.Id = _nextId++;
            _registrations.Add(registration);

            TempData["SuccessMessage"] = "The food record was successfully added!";
            return RedirectToAction(nameof(Index));
        }

        // UPDATE - Form
        public IActionResult Edit(int id)
        {
            var registration = _registrations.FirstOrDefault(r => r.Id == id);
            if (registration == null) return NotFound();
            return View(registration);
        }

        // UPDATE - Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(UserFoodRegistration registration)
        {
            if (!ModelState.IsValid)
                return View(registration);

            var existing = _registrations.FirstOrDefault(r => r.Id == registration.Id);
            if (existing == null) return NotFound();

            existing.Name = registration.Name;
            existing.UserId = registration.UserId;
            existing.MealDateTime = registration.MealDateTime;
            existing.MealType = registration.MealType;
            existing.FoodName = registration.FoodName;
            existing.Quantity = registration.Quantity;
            existing.Notes = registration.Notes;

            TempData["SuccessMessage"] = "The food record was successfully updated!";
            return RedirectToAction(nameof(Index));
        }

        // DELETE - Confirm
        public IActionResult Delete(int id)
        {
            var registration = _registrations.FirstOrDefault(r => r.Id == id);
            if (registration == null) return NotFound();
            return View(registration);
        }

        // DELETE - Execute
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var registration = _registrations.FirstOrDefault(r => r.Id == id);
            if (registration != null)
                _registrations.Remove(registration);

            TempData["SuccessMessage"] = "The food record was successfully deleted!";
            return RedirectToAction(nameof(Index));
        }
    }
}
