using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers
{
    public class UtenteSaudeController : Controller
    {
        private readonly HealthWellbeingDbContext _db;

        public UtenteSaudeController(HealthWellbeingDbContext db)
        {
            _db = db;
        }

        // ================================
        // LISTAR
        // URL: /UtenteSaude
        // View: Views/UtenteSaude/Index.cshtml
        // ================================
        public async Task<IActionResult> Index()
        {
            var lista = await _db.UtenteSaude
                                 .AsNoTracking()
                                 .OrderBy(u => u.NomeCompleto)
                                 .ToListAsync();
            return View(lista);
        }

        // ================================
        // DETALHES
        // URL: /UtenteSaude/Details/{id}
        // Exemplo: /UtenteSaude/Details/3
        // View: Views/UtenteSaude/Details.cshtml
        // ================================
        public async Task<IActionResult> Details(int id)
        {
            var u = await _db.UtenteSaude
                             .AsNoTracking()
                             .FirstOrDefaultAsync(x => x.UtenteSaudeId == id);
            if (u == null) return NotFound();
            return View(u);
        }

        // ================================
        // CRIAR (GET)
        // URL: /UtenteSaude/Create
        // View: Views/UtenteSaude/Create.cshtml
        // ================================
        public IActionResult Create()
        {
            return View(new UtenteSaude());
        }

        // ================================
        // CRIAR (POST)
        // URL: /UtenteSaude/Create
        // (é chamada automaticamente quando se submete o formulário da View Create)
        // ================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UtenteSaude u)
        {
            if (!ModelState.IsValid) return View(u);

            // Validações de unicidade
            if (await _db.UtenteSaude.AnyAsync(x => x.Nif == u.Nif))
                ModelState.AddModelError(nameof(UtenteSaude.Nif), "Já existe um utente com este NIF.");
            if (await _db.UtenteSaude.AnyAsync(x => x.Nus == u.Nus))
                ModelState.AddModelError(nameof(UtenteSaude.Nus), "Já existe um utente com este NUS.");
            if (await _db.UtenteSaude.AnyAsync(x => x.Niss == u.Niss))
                ModelState.AddModelError(nameof(UtenteSaude.Niss), "Já existe um utente com este NISS.");

            if (!ModelState.IsValid) return View(u);

            _db.UtenteSaude.Add(u);
            await _db.SaveChangesAsync();

            TempData["Msg"] = "Utente criado com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        // ================================
        // EDITAR (GET)
        // URL: /UtenteSaude/Edit/{id}
        // Exemplo: /UtenteSaude/Edit/2
        // View: Views/UtenteSaude/Edit.cshtml
        // ================================
        public async Task<IActionResult> Edit(int id)
        {
            var u = await _db.UtenteSaude.FindAsync(id);
            if (u == null) return NotFound();
            return View(u);
        }

        // ================================
        // EDITAR (POST)
        // URL: /UtenteSaude/Edit/{id}
        // (submetido via formulário da View Edit)
        // ================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UtenteSaude u)
        {
            if (id != u.UtenteSaudeId) return NotFound();
            if (!ModelState.IsValid) return View(u);

            // Unicidade (ignora o próprio registo)
            if (await _db.UtenteSaude.AnyAsync(x => x.UtenteSaudeId != id && x.Nif == u.Nif))
                ModelState.AddModelError(nameof(UtenteSaude.Nif), "Já existe um utente com este NIF.");
            if (await _db.UtenteSaude.AnyAsync(x => x.UtenteSaudeId != id && x.Nus == u.Nus))
                ModelState.AddModelError(nameof(UtenteSaude.Nus), "Já existe um utente com este NUS.");
            if (await _db.UtenteSaude.AnyAsync(x => x.UtenteSaudeId != id && x.Niss == u.Niss))
                ModelState.AddModelError(nameof(UtenteSaude.Niss), "Já existe um utente com este NISS.");

            if (!ModelState.IsValid) return View(u);

            try
            {
                _db.Update(u);
                await _db.SaveChangesAsync();
                TempData["Msg"] = "Utente atualizado com sucesso.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _db.UtenteSaude.AnyAsync(x => x.UtenteSaudeId == id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // ================================
        // APAGAR (GET)
        // URL: /UtenteSaude/Delete/{id}
        // Exemplo: /UtenteSaude/Delete/5
        // View: Views/UtenteSaude/Delete.cshtml
        // ================================
        public async Task<IActionResult> Delete(int id)
        {
            var u = await _db.UtenteSaude
                             .AsNoTracking()
                             .FirstOrDefaultAsync(x => x.UtenteSaudeId == id);
            if (u == null) return NotFound();
            return View(u);
        }

        // ================================
        // APAGAR (POST)
        // URL: /UtenteSaude/Delete/{id}
        // (submetido via formulário da View Delete)
        // ================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var u = await _db.UtenteSaude.FindAsync(id);
            if (u != null)
            {
                _db.UtenteSaude.Remove(u);
                await _db.SaveChangesAsync();
                TempData["Msg"] = "Utente removido.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
