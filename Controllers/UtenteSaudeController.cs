using Microsoft.AspNetCore.Mvc;
using HealthWellbeing.Models;
using System.Linq;

namespace HealthWellbeing.Controllers
{
    public class UtenteSaudeController : Controller
    {
        // “BD” em memória (apenas para esta fase, sem EF/SQL)
        private static readonly List<UtenteSaude> _data = new();
        private static int _nextId = 1;

        // LISTAR
        public IActionResult Index()
        {
            return View(_data);
        }

        // DETALHES
        public IActionResult Details(int id)
        {
            var u = _data.FirstOrDefault(x => x.UtenteSaudeId == id);
            if (u == null) return NotFound();
            return View(u);
        }

        // CRIAR (GET)
        public IActionResult Create()
        {
            return View(new UtenteSaude());
        }

        // CRIAR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(UtenteSaude u)
        {
            // Validações de modelo (DataAnnotations)
            if (!ModelState.IsValid) return View(u);

            // Validações de unicidade (simuladas já que não há BD)
            if (_data.Any(x => x.Nif == u.Nif))
                ModelState.AddModelError(nameof(UtenteSaude.Nif), "Já existe um utente com este NIF.");
            if (_data.Any(x => x.Nus == u.Nus))
                ModelState.AddModelError(nameof(UtenteSaude.Nus), "Já existe um utente com este NUS.");
            if (_data.Any(x => x.Niss == u.Niss))
                ModelState.AddModelError(nameof(UtenteSaude.Niss), "Já existe um utente com este NISS.");

            if (!ModelState.IsValid) return View(u);

            u.UtenteSaudeId = _nextId++;
            _data.Add(u);
            TempData["Msg"] = "Utente criado com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        // EDITAR (GET)
        public IActionResult Edit(int id)
        {
            var u = _data.FirstOrDefault(x => x.UtenteSaudeId == id);
            if (u == null) return NotFound();
            return View(u);
        }

        // EDITAR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, UtenteSaude u)
        {
            if (id != u.UtenteSaudeId) return NotFound();
            if (!ModelState.IsValid) return View(u);

            // Unicidade ignorando o próprio registo
            if (_data.Any(x => x.UtenteSaudeId != id && x.Nif == u.Nif))
                ModelState.AddModelError(nameof(UtenteSaude.Nif), "Já existe um utente com este NIF.");
            if (_data.Any(x => x.UtenteSaudeId != id && x.Nus == u.Nus))
                ModelState.AddModelError(nameof(UtenteSaude.Nus), "Já existe um utente com este NUS.");
            if (_data.Any(x => x.UtenteSaudeId != id && x.Niss == u.Niss))
                ModelState.AddModelError(nameof(UtenteSaude.Niss), "Já existe um utente com este NISS.");

            if (!ModelState.IsValid) return View(u);

            var idx = _data.FindIndex(x => x.UtenteSaudeId == id);
            if (idx < 0) return NotFound();

            _data[idx] = u;
            TempData["Msg"] = "Utente atualizado com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        // APAGAR (GET)
        public IActionResult Delete(int id)
        {
            var u = _data.FirstOrDefault(x => x.UtenteSaudeId == id);
            if (u == null) return NotFound();
            return View(u);
        }

        // APAGAR (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var u = _data.FirstOrDefault(x => x.UtenteSaudeId == id);
            if (u != null) _data.Remove(u);
            TempData["Msg"] = "Utente removido.";
            return RedirectToAction(nameof(Index));
        }
    }
}
