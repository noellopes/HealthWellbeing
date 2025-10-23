using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthWellbeing.Controllers
{
    public class ProblemaSaudeController : Controller
    {
        // Simulação de uma base de dados em memória
        private static readonly List<ProblemaSaude> problemas = new List<ProblemaSaude>();

        // GET: /ProblemaSaude
        public IActionResult Index()
        {
            return View(problemas);
        }

        // GET: /ProblemaSaude/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /ProblemaSaude/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProblemaSaude problema)
        {
            if (ModelState.IsValid)
            {
                // Simula salvar em memória
                problema.ProblemaSaudeId = problemas.Count + 1;
                problemas.Add(problema);

                TempData["SuccessMessage"] = $"Problema '{problema.ProblemaNome}' criado com sucesso!";
                return RedirectToAction(nameof(Index));
            }

            return View(problema);
        }

        // GET: /ProblemaSaude/Edit/5
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var problema = problemas.FirstOrDefault(p => p.ProblemaSaudeId == id);
            if (problema == null)
                return NotFound();

            return View(problema);
        }

        // POST: /ProblemaSaude/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProblemaSaude problema)
        {
            if (ModelState.IsValid)
            {
                var existente = problemas.FirstOrDefault(p => p.ProblemaSaudeId == problema.ProblemaSaudeId);
                if (existente == null)
                    return NotFound();

                // Atualiza os campos
                existente.ProblemaCategoria = problema.ProblemaCategoria;
                existente.ProblemaNome = problema.ProblemaNome;
                existente.ZonaAtingida = problema.ZonaAtingida;
                existente.ProfissionalDeApoio = problema.ProfissionalDeApoio;
                existente.Gravidade = problema.Gravidade;

                TempData["SuccessMessage"] = $"Problema '{problema.ProblemaNome}' atualizado com sucesso!";
                return RedirectToAction(nameof(Index));
            }

            return View(problema);
        }

        // GET: /ProblemaSaude/Delete/5
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var problema = problemas.FirstOrDefault(p => p.ProblemaSaudeId == id);
            if (problema == null)
                return NotFound();

            return View(problema);
        }

        // POST: /ProblemaSaude/DeleteConfirmado/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var problema = problemas.FirstOrDefault(p => p.ProblemaSaudeId == id);
            if (problema != null)
                problemas.Remove(problema);

            TempData["SuccessMessage"] = "Problema removido com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /ProblemaSaude/Details/5
        [HttpGet]
        public IActionResult Details(int id)
        {
            var problema = problemas.FirstOrDefault(p => p.ProblemaSaudeId == id);
            if (problema == null)
                return NotFound();

            return View(problema);
        }
    }
}
