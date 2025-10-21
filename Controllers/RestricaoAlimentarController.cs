using Microsoft.AspNetCore.Mvc;
using HealthWellbeing.Models;
using System.Collections.Generic;
using System.Linq;

namespace HealthWellbeing.Controllers
{
    public class RestricaoAlimentarController : Controller
    {
        // Simula um banco de dados em memória
        private static readonly List<RestricaoAlimentar> _restricoes = new List<RestricaoAlimentar>();

        // GET: RestricaoAlimentar
        public IActionResult Index()
        {
            return View(_restricoes);
        }

        // GET: RestricaoAlimentar/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
                return NotFound();

            var restricao = _restricoes.FirstOrDefault(r => r.Id == id);
            if (restricao == null)
                return NotFound();

            return View(restricao);
        }

        // GET: RestricaoAlimentar/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: RestricaoAlimentar/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Nome,Tipo,Gravidade,Sintomas")] RestricaoAlimentar restricao)
        {
            if (ModelState.IsValid)
            {
                restricao.Id = _restricoes.Any() ? _restricoes.Max(r => r.Id) + 1 : 1;
                _restricoes.Add(restricao);
                return RedirectToAction(nameof(Index));
            }
            return View(restricao);
        }

        // GET: RestricaoAlimentar/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var restricao = _restricoes.FirstOrDefault(r => r.Id == id);
            if (restricao == null)
                return NotFound();

            return View(restricao);
        }

        // POST: RestricaoAlimentar/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Nome,Tipo,Gravidade,Sintomas")] RestricaoAlimentar restricao)
        {
            if (id != restricao.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                var existente = _restricoes.FirstOrDefault(r => r.Id == id);
                if (existente == null)
                    return NotFound();

                existente.Nome = restricao.Nome;
                existente.Tipo = restricao.Tipo;
                existente.Gravidade = restricao.Gravidade;
                existente.Sintomas = restricao.Sintomas;

                return RedirectToAction(nameof(Index));
            }
            return View(restricao);
        }

        // GET: RestricaoAlimentar/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var restricao = _restricoes.FirstOrDefault(r => r.Id == id);
            if (restricao == null)
                return NotFound();

            return View(restricao);
        }

        // POST: RestricaoAlimentar/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var restricao = _restricoes.FirstOrDefault(r => r.Id == id);
            if (restricao != null)
                _restricoes.Remove(restricao);

            return RedirectToAction(nameof(Index));
        }

        private bool RestricaoExists(int id)
        {
            return _restricoes.Any(r => r.Id == id);
        }
    }
}
