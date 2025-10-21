using Microsoft.AspNetCore.Mvc;
using HealthWellbeing.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthWellbeing.Controllers
{
    public class RestricoesAlimentaresController : Controller
    {
        // Simula um banco de dados em memória
        private static List<RestricoesAlimentares> _restricoes = new List<RestricoesAlimentares>();

        // GET: RestricoesAlimentares
        public async Task<IActionResult> Index()
        {
            return await Task.FromResult(View(_restricoes));
        }

        // GET: RestricoesAlimentares/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var restricao = _restricoes.FirstOrDefault(r => r.Id == id);
            if (restricao == null)
                return NotFound();

            return await Task.FromResult(View(restricao));
        }

        // GET: RestricoesAlimentares/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: RestricoesAlimentares/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Tipo,Gravidade,Sintomas")] RestricoesAlimentares restricao)
        {
            if (ModelState.IsValid)
            {
                restricao.Id = _restricoes.Any() ? _restricoes.Max(r => r.Id) + 1 : 1;
                _restricoes.Add(restricao);
                return await Task.FromResult(RedirectToAction(nameof(Index)));
            }
            return View(restricao);
        }

        // GET: RestricoesAlimentares/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var restricao = _restricoes.FirstOrDefault(r => r.Id == id);
            if (restricao == null)
                return NotFound();

            return await Task.FromResult(View(restricao));
        }

        // POST: RestricoesAlimentares/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Tipo,Gravidade,Sintomas")] RestricoesAlimentares restricao)
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

                return await Task.FromResult(RedirectToAction(nameof(Index)));
            }
            return View(restricao);
        }

        // GET: RestricoesAlimentares/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var restricao = _restricoes.FirstOrDefault(r => r.Id == id);
            if (restricao == null)
                return NotFound();

            return await Task.FromResult(View(restricao));
        }

        // POST: RestricoesAlimentares/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var restricao = _restricoes.FirstOrDefault(r => r.Id == id);
            if (restricao != null)
                _restricoes.Remove(restricao);

            return await Task.FromResult(RedirectToAction(nameof(Index)));
        }

        private bool RestricaoExists(int id)
        {
            return _restricoes.Any(r => r.Id == id);
        }
    }
}
