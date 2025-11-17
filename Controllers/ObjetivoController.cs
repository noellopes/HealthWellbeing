using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers
{
    public class ObjetivoController : Controller
    {
        // Armazenamento temporário em memória
        private static readonly List<Objetivo> _data = new()
        {
            new Objetivo { ObjetivoId = 1, Name = "Perder 5 kg", Category = "Perda de Peso", Details = "Caminhar 30 min/dia" }
        };

        // GET: /Objetivo
        public IActionResult Index()
        {
            return View(_data);
        }

        // GET: /Objetivo/Details/5
        public IActionResult Details(int id)
        {
            var item = _data.FirstOrDefault(o => o.ObjetivoId == id);
            return View(item);
        }

        // GET: /Objetivo/Create
        public IActionResult Create()
        {
            return View(new Objetivo());
        }

        // POST: /Objetivo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Objetivo obj)
        {
            if (!ModelState.IsValid) return View(obj);

            obj.ObjetivoId = (_data.Count == 0) ? 1 : _data.Max(o => o.ObjetivoId) + 1;
            _data.Add(obj);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Objetivo/Edit/5
        public IActionResult Edit(int id)
        {
            var item = _data.FirstOrDefault(o => o.ObjetivoId == id);
            return View(item);
        }

        // POST: /Objetivo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Objetivo obj)
        {
            if (!ModelState.IsValid) return View(obj);

            var cur = _data.FirstOrDefault(o => o.ObjetivoId == id);
            if (cur == null) return NotFound();

            cur.Name = obj.Name;
            cur.Category = obj.Category;
            cur.Details = obj.Details;
            return RedirectToAction(nameof(Index));
        }

        // GET: /Objetivo/Delete/5
        public IActionResult Delete(int id)
        {
            var item = _data.FirstOrDefault(o => o.ObjetivoId == id);
            return View(item);
        }

        // POST: /Objetivo/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id, Objetivo _)
        {
            var cur = _data.FirstOrDefault(o => o.ObjetivoId == id);
            if (cur != null) _data.Remove(cur);
            return RedirectToAction(nameof(Index));
        }
    }
}
