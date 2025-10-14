using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers
{
    public class UnidadeController : Controller
    {
        // “BD” temporária em memória
        private static readonly List<Unidade> _data = new()
        {
            new Unidade { UnidadeId = 1, NomeComida = "Pão",         Quantidade = "1 fatia (30 g)" },
            new Unidade { UnidadeId = 2, NomeComida = "Leite",       Quantidade = "200 ml" },
            new Unidade { UnidadeId = 3, NomeComida = "Arroz cozido",Quantidade = "250 g" }
        };

        // GET: /Unidade
        public IActionResult Index() => View(_data.OrderBy(x => x.UnidadeId).ToList());

        // GET: /Unidade/Details/5
        public IActionResult Details(int id)
        {
            var item = _data.FirstOrDefault(x => x.UnidadeId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: /Unidade/Create
        public IActionResult Create() => View(new Unidade());

        // POST: /Unidade/Create
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(Unidade model)
        {
            if (!ModelState.IsValid) return View(model);
            model.UnidadeId = _data.Count == 0 ? 1 : _data.Max(x => x.UnidadeId) + 1;
            _data.Add(model);
            TempData["Msg"] = "Registo criado.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Unidade/Edit/5
        public IActionResult Edit(int id)
        {
            var item = _data.FirstOrDefault(x => x.UnidadeId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // POST: /Unidade/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Unidade model)
        {
            if (!ModelState.IsValid) return View(model);
            var item = _data.FirstOrDefault(x => x.UnidadeId == id);
            if (item == null) return NotFound();

            item.NomeComida = model.NomeComida;
            item.Quantidade = model.Quantidade;

            TempData["Msg"] = "Registo atualizado.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Unidade/Delete/5
        public IActionResult Delete(int id)
        {
            var item = _data.FirstOrDefault(x => x.UnidadeId == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // POST: /Unidade/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var item = _data.FirstOrDefault(x => x.UnidadeId == id);
            if (item != null) _data.Remove(item);
            TempData["Msg"] = "Registo removido.";
            return RedirectToAction(nameof(Index));
        }
    }
}
