using HealthWellbeing.Models;
using HealthWellbeing.Services;
using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Mvc;

public class UtentesController : Controller
{
    private readonly UtenteService _utenteService;

    public UtentesController(UtenteService utenteService)
    {
        _utenteService = utenteService;
    }

    public IActionResult Index(int page = 1)
    {
        int pageSize = 10;

        int total = _utenteService.Count();

        var vm = new Paginacao<UtenteBalneario>(page, total)
        {
            Items = _utenteService.GetPage(page, pageSize)
        };

        return View(vm);
    }

    public IActionResult Create()
    {
        return View(new UtenteBalneario());
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(UtenteBalneario utente)
    {
        utente.DataInscricao = DateTime.Today;

        if (!ModelState.IsValid)
            return View(utente);

        _utenteService.Add(utente);
        return RedirectToAction(nameof(Index));
    }
}
