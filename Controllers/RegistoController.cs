using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthWellbeing.Controllers
{
    public class RegistoController : Controller
    {
        private readonly ILogger<RegistoController> _logger;

        public RegistoController(ILogger<RegistoController> logger)
        {
            _logger = logger;
        }

        // Simula base de dados em memória
        private static List<Registo> _registos = new List<Registo>();
        private static int _nextId = 1;

        // READ - List all
        public IActionResult Index()
        {
            return View(_registos);
        }

        // READ - Details
        public IActionResult Details(int id)
        {
            var registo = _registos.FirstOrDefault(r => r.RegistoID == id);
            if (registo == null) return NotFound();
            return View(registo);
        }

        // CREATE - Form
        public IActionResult Create()
        {
            return View();
        }

        // CREATE - Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Registo registo)
        {
            _logger.LogInformation("POST Create chamado. ModelState válido? " + ModelState.IsValid);

            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogError("Erro de validação: " + error.ErrorMessage);
                }
                return View(registo);
            }

            registo.RegistoID = _nextId++;
            _registos.Add(registo);
            _logger.LogInformation($"Registo adicionado: {registo.FoodName} ({registo.RegistoID})");

            return RedirectToAction("Index");
        }

        // UPDATE - Form
        public IActionResult Edit(int id)
        {
            var registo = _registos.FirstOrDefault(r => r.RegistoID == id);
            if (registo == null) return NotFound();
            return View(registo);
        }

        // UPDATE - Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Registo registo)
        {
            if (!ModelState.IsValid) return View(registo);

            var existing = _registos.FirstOrDefault(r => r.RegistoID == registo.RegistoID);
            if (existing == null) return NotFound();

            existing.UserId = registo.UserId;
            existing.MealDateTime = registo.MealDateTime;
            existing.MealType = registo.MealType;
            existing.FoodName = registo.FoodName;
            existing.Quantity = registo.Quantity;
            existing.Notes = registo.Notes;

            return RedirectToAction("Index");
        }

        // DELETE - Confirm
        public IActionResult Delete(int id)
        {
            var registo = _registos.FirstOrDefault(r => r.RegistoID == id);
            if (registo == null) return NotFound();
            return View(registo);
        }

        // DELETE - Execute
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var registo = _registos.FirstOrDefault(r => r.RegistoID == id);
            if (registo != null)
                _registos.Remove(registo);

            return RedirectToAction("Index");
        }
    }
}