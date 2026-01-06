using HealthWellbeing.Models;
using HealthWellbeing.Services;
using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

[Authorize]
public class UtentesController : Controller
{
    private readonly UtenteService _utenteService;

    public UtentesController(UtenteService utenteService)
    {
        _utenteService = utenteService;
    }

    public IActionResult Index(int page = 1, string? search = null, bool? ativo = null)
    {
        int pageSize = 10;

        int total = _utenteService.Count(search, ativo);

        var vm = new Paginacao<UtenteBalneario>(page, total)
        {
            Items = _utenteService.GetPage(page, pageSize, search, ativo)
        };

        ViewData["Search"] = search;
        ViewData["Ativo"] = ativo;

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
        if (!ModelState.IsValid)
            return View(utente);

        utente.DataInscricao = DateTime.Today;

        utente.DadosMedicos = new DadosMedicos
        {
            HistoricoClinico = "",
            IndicacoesTerapeuticas = "",
            ContraIndicacoes = "",
            MedicoResponsavel = ""
        };

        utente.SeguroSaude = new SeguroSaude
        {
            NomeSeguradora = "N/A",
            NumeroApolice = ""
        };

        _utenteService.Add(utente);

        TempData["Success"] = "Cliente criado com sucesso.";

        return RedirectToAction(nameof(Index));
    }



    public IActionResult Edit(int id)
    {
        var utente = _utenteService.GetById(id);
        if (utente == null)
            return NotFound();

        return View(utente);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(UtenteBalneario utente)
    {
        if (!ModelState.IsValid)
            return View(utente);

        _utenteService.Update(utente);
        return RedirectToAction(nameof(Index));
    }


    public IActionResult Details(int id)
    {
        var utente = _utenteService.GetById(id);
        if (utente == null)
            return NotFound();

        return View(utente);
    }


    [Authorize(Roles = "Admin")]
    public IActionResult Delete(int id)
    {
        var utente = _utenteService.GetById(id);
        if (utente == null)
            return NotFound();

        return View(utente);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    [ActionName("Delete")]
    public IActionResult DeleteConfirmed(int utenteBalnearioId)
    {
        _utenteService.Delete(utenteBalnearioId);
        TempData["Success"] = "Cliente eliminado com sucesso.";
        return RedirectToAction(nameof(Index));
    }



}
