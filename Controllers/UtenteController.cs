using Microsoft.AspNetCore.Mvc;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers
{
    public class UtentesController : Controller
    {
        public IActionResult Index()
        {
            var lista = UtenteService.GetAll();
            return View(lista);
        }

        public IActionResult Details(int id)
        {
            var utente = UtenteService.GetById(id);
            if (utente == null) return NotFound();
            return View(utente);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public IActionResult Create(UtenteBalneario utente)
        {
            if (!ModelState.IsValid) return View(utente);
            UtenteService.Add(utente);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var utente = UtenteService.GetById(id);
            if (utente == null) return NotFound();
            return View(utente);
        }

        [HttpPost]
        public IActionResult Edit(UtenteBalneario utente)
        {
            if (!ModelState.IsValid) return View(utente);
            UtenteService.Update(utente);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            UtenteService.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
