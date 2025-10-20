using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace HealthWellbeing.Controllers
{
    public class AlimentoController : Controller
    {
        // Lista em memória — substitui a BD
        private static List<Alimento> _alimentos = new()
        {
            new Alimento { AlimentoId = 1, Name = "Maçã", KcalPor100g = 52 },
            new Alimento { AlimentoId = 2, Name = "Arroz", KcalPor100g = 130 },
        };

        // LISTAR
        public IActionResult Index()
        {
            return View(_alimentos);
        }

        // CRIAR (GET)
        public IActionResult Create() => View();

        // CRIAR (POST)
        [HttpPost]
        public IActionResult Create(Alimento a)
        {
            a.AlimentoId = _alimentos.Any() ? _alimentos.Max(x => x.AlimentoId) + 1 : 1;
            _alimentos.Add(a);
            return RedirectToAction("Index");
        }

        // EDITAR (GET)
        public IActionResult Edit(int id)
        {
            var a = _alimentos.FirstOrDefault(x => x.AlimentoId == id);
            if (a == null) return NotFound();
            return View(a);
        }

        // EDITAR (POST)
        [HttpPost]
        public IActionResult Edit(Alimento a)
        {
            var old = _alimentos.FirstOrDefault(x => x.AlimentoId == a.AlimentoId);
            if (old == null) return NotFound();
            old.Name = a.Name;
            old.KcalPor100g = a.KcalPor100g;
            return RedirectToAction("Index");
        }

        // ELIMINAR (GET) — mostra a página de confirmação
        public IActionResult Delete(int id)
        {
            var a = _alimentos.FirstOrDefault(x => x.AlimentoId == id);
            if (a == null) return NotFound();
            return View(a);
        }

        // ELIMINAR (POST) — confirma e remove
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var a = _alimentos.FirstOrDefault(x => x.AlimentoId == id);
            if (a != null) _alimentos.Remove(a);
            return RedirectToAction("Index");
        }


        // DETALHES (GET)
        public IActionResult Details(int id)
        {
            var a = _alimentos.FirstOrDefault(x => x.AlimentoId == id);
            if (a == null) return NotFound();
            return View(a);
        }

    }
}
