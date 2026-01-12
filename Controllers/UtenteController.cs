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
        [ValidateAntiForgeryToken] 
        public IActionResult Create(UtenteBalneario utente)
        {
            // Remova as validações de todas as propriedades que NÃO estão no formulário HTML
            ModelState.Remove("SeguroSaude");
            ModelState.Remove("DadosMedicos");
            ModelState.Remove("Agendamentos"); 

            if (!ModelState.IsValid)
            {
                
                return View(utente);
            }

            try
            {
                utente.DataInscricao = DateTime.Now;
                _context.Utentes.Add(utente);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                
                ModelState.AddModelError("", "Erro ao guardar na base de dados: " + ex.Message);
                return View(utente);
            }
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
