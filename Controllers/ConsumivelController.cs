using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;

namespace HealthWellbeing.Controllers
{
    public class ConsumivelController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ConsumivelController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // INDEX COM PAGINAÇÃO + PESQUISA
        public async Task<IActionResult> Index(string? searchNome, string? searchCategoria, int page = 1)
        {
            if (!_context.Consumivel.Any())
            {
                var categorias = _context.CategoriaConsumivel.ToList();

                var consumiveis = new List<Consumivel>
    {
        new Consumivel { Nome = "Luvas Cirúrgicas Pequenas", Descricao = "Pacote de luvas pequenas", CategoriaId = categorias.First(c => c.Nome == "Luvas").CategoriaId, QuantidadeMaxima = 100, QuantidadeAtual = 50, QuantidadeMinima = 10 },
        new Consumivel { Nome = "Máscara N95", Descricao = "Máscara respiratória N95", CategoriaId = categorias.First(c => c.Nome == "Máscaras").CategoriaId, QuantidadeMaxima = 200, QuantidadeAtual = 150, QuantidadeMinima = 20 },
        new Consumivel { Nome = "Seringa 5ml", Descricao = "Seringa descartável de 5ml", CategoriaId = categorias.First(c => c.Nome == "Seringas e Agulhas").CategoriaId, QuantidadeMaxima = 300, QuantidadeAtual = 200, QuantidadeMinima = 30 },
        new Consumivel { Nome = "Compressa Estéril", Descricao = "Pacote de compressas estéreis", CategoriaId = categorias.First(c => c.Nome == "Compressas").CategoriaId, QuantidadeMaxima = 150, QuantidadeAtual = 100, QuantidadeMinima = 15 },
        new Consumivel { Nome = "Gaze Esterilizada", Descricao = "Pacote de gazes estéreis", CategoriaId = categorias.First(c => c.Nome == "Gazes").CategoriaId, QuantidadeMaxima = 200, QuantidadeAtual = 120, QuantidadeMinima = 20 },
        new Consumivel { Nome = "Álcool 70%", Descricao = "Frasco de álcool 70%", CategoriaId = categorias.First(c => c.Nome == "Desinfetantes").CategoriaId, QuantidadeMaxima = 50, QuantidadeAtual = 30, QuantidadeMinima = 5 }
    };

                _context.Consumivel.AddRange(consumiveis);
                await _context.SaveChangesAsync();
            }

            var query = _context.Consumivel.Include(c => c.CategoriaConsumivel).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchNome))
                query = query.Where(c => c.Nome.Contains(searchNome));

            if (!string.IsNullOrWhiteSpace(searchCategoria))
                query = query.Where(c => c.CategoriaConsumivel.Nome.Contains(searchCategoria));

            int totalItems = await query.CountAsync();
            var model = new PaginationInfo<Consumivel>(page, totalItems);

            model.Items = await query
                .OrderBy(c => c.Nome)
                .Skip(model.ItemsToSkip)
                .Take(model.ItemsPerPage)
                .ToListAsync();

            ViewBag.SearchNome = searchNome;
            ViewBag.SearchCategoria = searchCategoria;

            return View(model);
        }

        // DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return View("InvalidConsumivel");

            var consumivel = await _context.Consumivel
                .Include(c => c.CategoriaConsumivel)
                .FirstOrDefaultAsync(c => c.ConsumivelId == id);

            if (consumivel == null) return View("InvalidConsumivel");

            return View(consumivel);
        }

        // CREATE
        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_context.CategoriaConsumivel, "CategoriaId", "Nome");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ConsumivelId,Nome,Descricao,CategoriaId,QuantidadeMaxima,QuantidadeAtual,QuantidadeMinima")] Consumivel consumivel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(consumivel);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Consumível criado com sucesso!";
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoriaId"] = new SelectList(_context.CategoriaConsumivel, "CategoriaId", "Nome", consumivel.CategoriaId);
            return View(consumivel);
        }

        // EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return View("InvalidConsumivel");

            var consumivel = await _context.Consumivel.FindAsync(id);
            if (consumivel == null) return View("InvalidConsumivel");

            ViewData["CategoriaId"] = new SelectList(_context.CategoriaConsumivel, "CategoriaId", "Nome", consumivel.CategoriaId);
            return View(consumivel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("ConsumivelId,Nome,Descricao,CategoriaId,QuantidadeMaxima,QuantidadeAtual,QuantidadeMinima")] Consumivel consumivel)
        {
            if (id != consumivel.ConsumivelId)
                return View("InvalidConsumivel");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(consumivel);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Consumível atualizado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Consumivel.Any(e => e.ConsumivelId == consumivel.ConsumivelId))
                        return View("InvalidConsumivel");
                    else
                        throw;
                }
            }

            ViewData["CategoriaId"] = new SelectList(_context.CategoriaConsumivel, "CategoriaId", "Nome", consumivel.CategoriaId);
            return View(consumivel);
        }


        // DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return View("InvalidConsumivel");

            var consumivel = await _context.Consumivel
                .Include(c => c.CategoriaConsumivel)
                .FirstOrDefaultAsync(c => c.ConsumivelId == id);

            if (consumivel == null) return View("InvalidConsumivel");

            return View(consumivel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var consumivel = await _context.Consumivel.FindAsync(id);
            if (consumivel != null)
            {
                // Aqui podemos verificar se há Auditorias associadas, opcional
                _context.Consumivel.Remove(consumivel);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Consumível eliminado com sucesso!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
