using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers
{
    public class FoodPortionController : Controller
    {
        // “BD” temporária em memória
        private static readonly List<FoodPortion> _data = new()
        {
            new FoodPortion { UnitId = 1, FoodName = "Bread",      Amount = "1 slice (30 g)" },
            new FoodPortion { UnitId = 2, FoodName = "Milk",       Amount = "200 ml" },
            new FoodPortion { UnitId = 3, FoodName = "Cooked Rice",Amount = "250 g" }
        };

        // GET: /Unidade
        public IActionResult Index() => View(_data.OrderBy(x => x.UnitId).ToList());

        // GET: /Unidade/Details/5
        public IActionResult Details(int id)
        {
            var item = _data.FirstOrDefault(x => x.UnitId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /Unidade/Create
        public IActionResult Create() => View(new FoodPortion());

        // POST: /Unidade/Create
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(FoodPortion model)
        {
            if (!ModelState.IsValid) return View(model);
            model.UnitId = _data.Count == 0 ? 1 : _data.Max(x => x.UnitId) + 1;
            _data.Add(model);
            TempData["Msg"] = "Record Created.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Unidade/Edit/5
        public IActionResult Edit(int id)
        {
            var item = _data.FirstOrDefault(x => x.UnitId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // POST: /Unidade/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(int id, FoodPortion model)
        {
            if (!ModelState.IsValid) return View(model);
            var item = _data.FirstOrDefault(x => x.UnitId == id);
            if (item == null) return NotFound();

            item.FoodName = model.FoodName;
            item.Amount = model.Amount;

            TempData["Msg"] = "Update Record.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Unidade/Delete/5
        public IActionResult Delete(int id)
        {
            var item = _data.FirstOrDefault(x => x.UnitId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // POST: /Unidade/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var item = _data.FirstOrDefault(x => x.UnitId == id);
            if (item != null) _data.Remove(item);
            TempData["Msg"] = "Remove Record.";
            return RedirectToAction(nameof(Index));
        }
    }
}
