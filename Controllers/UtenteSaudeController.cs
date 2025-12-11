using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        // GET: UtenteSaude
      /*  public async Task<IActionResult> Index(
            int page = 1,
            string searchNome = "",
            string searchNif = "")
        {
            // 1) Começamos com um IQueryable
            var utentesQuery = _db.UtenteSaude
                .AsQueryable();

            // 2) Aplicar filtros se tiverem valor
            if (!string.IsNullOrWhiteSpace(searchNome))
            {
                utentesQuery = utentesQuery
                    .Where(u => u.NomeCompleto.Contains(searchNome));
            }

            if (!string.IsNullOrWhiteSpace(searchNif))
            {
                utentesQuery = utentesQuery
                    .Where(u => u.Nif.Contains(searchNif));
            }

            // 3) Guardar valores para a View (inputs ficam preenchidos)
            ViewBag.SearchNome = searchNome;
            ViewBag.SearchNif = searchNif;

            // 4) Paginação
            int numberUtentes = await utentesQuery.CountAsync();

            var utentesInfo = new PaginationInfo<UtenteSaude>(page, numberUtentes);

            utentesInfo.Items = await utentesQuery
                .OrderBy(u => u.NomeCompleto)
                .Skip(utentesInfo.ItemsToSkip)
                .Take(utentesInfo.ItemsPerPage)
                .ToListAsync();

            return View(utentesInfo);
        }*/
        public async Task<IActionResult> Index(
    int page = 1,
    string searchNome = "",
    string searchNif = "")
        {
            // 👉 Garantir que nunca há página < 1
            if (page < 1)
                page = 1;

            // 1) Começamos com um IQueryable
            var utentesQuery = _db.UtenteSaude.AsQueryable();

            // 2) Aplicar filtros se tiverem valor
            if (!string.IsNullOrWhiteSpace(searchNome))
            {
                utentesQuery = utentesQuery
                    .Where(u => u.NomeCompleto.Contains(searchNome));
            }

            if (!string.IsNullOrWhiteSpace(searchNif))
            {
                utentesQuery = utentesQuery
                    .Where(u => u.Nif.Contains(searchNif));
            }

            // 3) Guardar valores para a View (inputs ficam preenchidos)
            ViewBag.SearchNome = searchNome;
            ViewBag.SearchNif = searchNif;

            // 4) Paginação
            int numberUtentes = await utentesQuery.CountAsync();

            // Se quiseres ser ainda mais defensivo:
            // se não houver utentes, mantém page = 1
            if (numberUtentes == 0)
                page = 1;

            var utentesInfo = new PaginationInfo<UtenteSaude>(page, numberUtentes);

            // (Opcional, se ItemsToSkip tiver setter)
            // if (utentesInfo.ItemsToSkip < 0)
            //     utentesInfo.ItemsToSkip = 0;

            utentesInfo.Items = await utentesQuery
                .OrderBy(u => u.NomeCompleto)
                .Skip(utentesInfo.ItemsToSkip)
                .Take(utentesInfo.ItemsPerPage)
                .ToListAsync();

            return View(utentesInfo);
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
