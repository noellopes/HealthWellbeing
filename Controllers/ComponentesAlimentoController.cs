using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers
{
    public class ComponentesAlimentoController : Controller
    {
        private static List<ComponentesAlimento> _Calimento = new()
        {
            new ComponentesAlimento { CompFoodID  = 1, Name = "Maçã", Description = "Water, Carbohydrates, Fibers, Vitamins, Minerals, Antioxidant, Maleic Acid"},
            new ComponentesAlimento { CompFoodID = 2, Name = "Arroz", Description = ""},
        };


        // GET: ComponentesAlimentoController
        public IActionResult Index()
        {
            return View(_Calimento);
        }    

        // GET: ComponentesAlimentoController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ComponentesAlimentoController/Create
        [HttpPost]
        public IActionResult Create(ComponentesAlimento a)
        {
            a.CompFoodID = _Calimento.Any() ? _Calimento.Max(x => x.CompFoodID) + 1 : 1;
            _Calimento.Add(a);
            return RedirectToAction("Index");
        }

        // GET: ComponentesAlimentoController/Edit/5
        public IActionResult Edit(int id)
        {
            var a = _Calimento.FirstOrDefault(x => x.CompFoodID == id);
            if (a == null) return NotFound();
            return View(a);
        }

        // POST: ComponentesAlimentoController/Edit/5
        [HttpPost]
        public IActionResult Edit(ComponentesAlimento a)
        {
            var old = _Calimento.FirstOrDefault(x => x.CompFoodID == a.CompFoodID);
            if (old == null) return NotFound();
            old.Name = a.Name;
            old.Description = a.Description;
            return RedirectToAction("Index");
        }

        // GET: ComponentesAlimentoController/Delete/5
        public IActionResult Delete(int id)
        {
            var a = _Calimento.FirstOrDefault(x => x.CompFoodID == id);
            if (a != null) _Calimento.Remove(a);
            return RedirectToAction("Index");
        }
    }
}
