using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthWellbeing.Controllers
{
    public class UtentesController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public UtentesController(HealthWellbeingDbContext context)
        {
            _context = context;
        }
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
            // Temporariamente: remove validações de objetos que não estão no form
            ModelState.Remove("SeguroSaude");
            ModelState.Remove("DadosMedicos");

            if (!ModelState.IsValid)
            {
                // Se cair aqui, olhe a variável 'ModelState' no Debug para ver o que falta
                return View(utente);
            }

            utente.DataInscricao = DateTime.Now; // Garante a data no servidor
            _context.Utentes.Add(utente);
            _context.SaveChanges();

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
//teste
