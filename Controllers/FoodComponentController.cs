using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers
{
    public class FoodComponentController : Controller
    {
        private static List<FoodComponent> _CFood = new()
        {
            new FoodComponent { FoodComponentId  = 1, Name = "Apple", Description = "Water, Carbohydrates, Fibers, Vitamins, Minerals, Antioxidant, Maleic Acid"},
            new FoodComponent { FoodComponentId = 2, Name = "Rice", Description = ""},
        };


        // GET: FoodComponentController
        public IActionResult Index()
        {
            return View(_CFood);
        }

        // GET: FoodComponentController
        public IActionResult Details(int id)
        {
            var a = _CFood.FirstOrDefault(x => x.FoodComponentId == id);
            if (a == null) return NotFound();
            return View(a);
        }

        // GET: FoodComponentController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FoodComponentController/Create
        [HttpPost]
        public IActionResult Create(FoodComponent a)
        {
            a.FoodComponentId = _CFood.Any() ? _CFood.Max(x => x.FoodComponentId) + 1 : 1;
            _CFood.Add(a);
            return RedirectToAction("Index");
        }

        // GET: FoodComponentController/Edit/5
        public IActionResult Edit(int id)
        {
            var a = _CFood.FirstOrDefault(x => x.FoodComponentId == id);
            if (a == null) return NotFound();
            return View(a);
        }

        // POST: FoodComponentController/Edit/5
        [HttpPost]
        public IActionResult Edit(FoodComponent a)
        {
            var old = _CFood.FirstOrDefault(x => x.FoodComponentId == a.FoodComponentId);
            if (old == null) return NotFound();
            old.Name = a.Name;
            old.Description = a.Description;
            return RedirectToAction("Index");
        }

        // GET: FoodComponentController/Delete/5
        public IActionResult Delete(int id)
        {
            var a = _CFood.FirstOrDefault(x => x.FoodComponentId == id);
            if (a != null) _CFood.Remove(a);
            return RedirectToAction("Index");
        }
    }
}
