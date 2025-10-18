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
            new ComponentesAlimento {  CompFoodID = 2, Name = "Arroz", Description = ""},
        };


        // GET: ComponentesAlimentoController
        public ActionResult Index()
        {
            return View(_Calimento);
        }

        // GET: ComponentesAlimentoController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ComponentesAlimentoController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ComponentesAlimentoController/Create
        [HttpPost]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(ComponentesAlimento));
            }
            catch
            {
                return View();
            }
        }

        // GET: ComponentesAlimentoController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ComponentesAlimentoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(ComponentesAlimento));
            }
            catch
            {
                return View();
            }
        }

        // GET: ComponentesAlimentoController/Delete/5
        public ActionResult Delete(int id)
        {
            var a = _Calimento.FirstOrDefault(x => x.CompFoodID == id);
            if (a != null) _Calimento.Remove(a);
            return RedirectToAction("Index");
        }
    }
}
