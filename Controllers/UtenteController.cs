using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Controllers
{
    public class UtentesController : Controller
    {

       public IActionResult Index(int page = 1)
    {
        var lista = UtenteService.GetAll();
        int total = lista.Count;

        var vm = new Paginacao<UtenteBalneario>(page, total);
        vm.Items = lista
            .OrderBy(u => u.NomeCompleto)
            .Skip(vm.ItemsToSkip)
            .Take(vm.ItemsPerPage)
            .ToList();

        return View(vm);
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
            Console.WriteLine("Entrou no POST Create"); // confirmar que o POST chegou

            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState inválido. Erros:");
                foreach (var kv in ModelState)
                {
                    var key = kv.Key;
                    foreach (var err in kv.Value.Errors)
                    {
                        Console.WriteLine($"- Campo: {key} | Erro: {err.ErrorMessage} | Ex: {err.Exception?.Message}");
                    }
                }

                utente.DataInscricao = DateTime.Now; // 👈 define automaticamente
                utente.DadosMedicos ??= new DadosMedicos();
                utente.SeguroSaude ??= new SeguroSaude();
                utente.SeguroSaude.NomeSeguradora = "N/A"; // evita erro do [Required]
                UtenteService.Add(utente);

                return View(utente);
            }

            UtenteService.Add(utente);
            Console.WriteLine($"Adicionado: {utente.NomeCompleto}");
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
