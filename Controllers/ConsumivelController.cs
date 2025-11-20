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
            // Já Existentes
            new Consumivel { Nome = "Luvas Cirúrgicas Pequenas", Descricao = "Pacote de luvas pequenas", CategoriaId = categorias.First(c => c.Nome == "Luvas").CategoriaId, QuantidadeMaxima = 100, QuantidadeAtual = 50, QuantidadeMinima = 10 },
            new Consumivel { Nome = "Máscara N95", Descricao = "Máscara respiratória N95", CategoriaId = categorias.First(c => c.Nome == "Máscaras").CategoriaId, QuantidadeMaxima = 200, QuantidadeAtual = 150, QuantidadeMinima = 20 },
            new Consumivel { Nome = "Seringa 5ml", Descricao = "Seringa descartável de 5ml", CategoriaId = categorias.First(c => c.Nome == "Seringas e Agulhas").CategoriaId, QuantidadeMaxima = 300, QuantidadeAtual = 200, QuantidadeMinima = 30 },
            new Consumivel { Nome = "Compressa Estéril", Descricao = "Pacote de compressas estéreis", CategoriaId = categorias.First(c => c.Nome == "Compressas").CategoriaId, QuantidadeMaxima = 150, QuantidadeAtual = 100, QuantidadeMinima = 15 },
            new Consumivel { Nome = "Gaze Esterilizada", Descricao = "Pacote de gazes estéreis", CategoriaId = categorias.First(c => c.Nome == "Gazes").CategoriaId, QuantidadeMaxima = 200, QuantidadeAtual = 120, QuantidadeMinima = 20 },
            new Consumivel { Nome = "Álcool 70%", Descricao = "Frasco de álcool 70%", CategoriaId = categorias.First(c => c.Nome == "Desinfetantes").CategoriaId, QuantidadeMaxima = 50, QuantidadeAtual = 30, QuantidadeMinima = 5 },

            // ---- Luvas ----
            new Consumivel { Nome = "Luvas Cirúrgicas Médias", Descricao = "Pacote de luvas médias", CategoriaId = categorias.First(c => c.Nome == "Luvas").CategoriaId, QuantidadeMaxima = 120, QuantidadeAtual = 60, QuantidadeMinima = 15 },
            new Consumivel { Nome = "Luvas Cirúrgicas Grandes", Descricao = "Pacote de luvas grandes", CategoriaId = categorias.First(c => c.Nome == "Luvas").CategoriaId, QuantidadeMaxima = 120, QuantidadeAtual = 70, QuantidadeMinima = 15 },
            new Consumivel { Nome = "Luvas de Procedimento", Descricao = "Luvas de procedimento não estéreis", CategoriaId = categorias.First(c => c.Nome == "Luvas").CategoriaId, QuantidadeMaxima = 300, QuantidadeAtual = 200, QuantidadeMinima = 40 },
            new Consumivel { Nome = "Luvas de Nitrilo", Descricao = "Luvas resistentes de nitrilo", CategoriaId = categorias.First(c => c.Nome == "Luvas").CategoriaId, QuantidadeMaxima = 150, QuantidadeAtual = 80, QuantidadeMinima = 20 },

            // ---- Máscaras ----
            new Consumivel { Nome = "Máscara Cirúrgica", Descricao = "Máscara tripla camada", CategoriaId = categorias.First(c => c.Nome == "Máscaras").CategoriaId, QuantidadeMaxima = 400, QuantidadeAtual = 250, QuantidadeMinima = 50 },
            new Consumivel { Nome = "Máscara PFF2", Descricao = "Máscara tipo PFF2", CategoriaId = categorias.First(c => c.Nome == "Máscaras").CategoriaId, QuantidadeMaxima = 180, QuantidadeAtual = 100, QuantidadeMinima = 20 },
            new Consumivel { Nome = "Máscara Descartável Infantil", Descricao = "Máscaras pequenas para crianças", CategoriaId = categorias.First(c => c.Nome == "Máscaras").CategoriaId, QuantidadeMaxima = 150, QuantidadeAtual = 90, QuantidadeMinima = 15 },
            new Consumivel { Nome = "Máscara Tecido", Descricao = "Máscaras de tecido reutilizáveis", CategoriaId = categorias.First(c => c.Nome == "Máscaras").CategoriaId, QuantidadeMaxima = 80, QuantidadeAtual = 40, QuantidadeMinima = 10 },

            // ---- Seringas e Agulhas ----
            new Consumivel { Nome = "Seringa 10ml", Descricao = "Seringa descartável de 10ml", CategoriaId = categorias.First(c => c.Nome == "Seringas e Agulhas").CategoriaId, QuantidadeMaxima = 250, QuantidadeAtual = 140, QuantidadeMinima = 20 },
            new Consumivel { Nome = "Seringa 20ml", Descricao = "Seringa de 20ml", CategoriaId = categorias.First(c => c.Nome == "Seringas e Agulhas").CategoriaId, QuantidadeMaxima = 200, QuantidadeAtual = 100, QuantidadeMinima = 20 },
            new Consumivel { Nome = "Seringa Insulina", Descricao = "Seringa para insulina", CategoriaId = categorias.First(c => c.Nome == "Seringas e Agulhas").CategoriaId, QuantidadeMaxima = 300, QuantidadeAtual = 180, QuantidadeMinima = 30 },
            new Consumivel { Nome = "Agulha 25x7", Descricao = "Agulha hipodérmica 25x7", CategoriaId = categorias.First(c => c.Nome == "Seringas e Agulhas").CategoriaId, QuantidadeMaxima = 400, QuantidadeAtual = 250, QuantidadeMinima = 40 },
            new Consumivel { Nome = "Agulha 30x7", Descricao = "Agulha hipodérmica 30x7", CategoriaId = categorias.First(c => c.Nome == "Seringas e Agulhas").CategoriaId, QuantidadeMaxima = 400, QuantidadeAtual = 240, QuantidadeMinima = 40 },
            new Consumivel { Nome = "Agulha 40x12", Descricao = "Agulha de grosso calibre", CategoriaId = categorias.First(c => c.Nome == "Seringas e Agulhas").CategoriaId, QuantidadeMaxima = 300, QuantidadeAtual = 150, QuantidadeMinima = 30 },

            // ---- Compressas ----
            new Consumivel { Nome = "Compressa Não Estéril", Descricao = "Pacote de compressas comuns", CategoriaId = categorias.First(c => c.Nome == "Compressas").CategoriaId, QuantidadeMaxima = 150, QuantidadeAtual = 80, QuantidadeMinima = 20 },
            new Consumivel { Nome = "Compressa 10x10", Descricao = "Pacote de compressas 10x10", CategoriaId = categorias.First(c => c.Nome == "Compressas").CategoriaId, QuantidadeMaxima = 200, QuantidadeAtual = 120, QuantidadeMinima = 20 },
            new Consumivel { Nome = "Compressa 7,5x7,5", Descricao = "Compressas pequenas", CategoriaId = categorias.First(c => c.Nome == "Compressas").CategoriaId, QuantidadeMaxima = 200, QuantidadeAtual = 100, QuantidadeMinima = 20 },

            // ---- Gazes ----
            new Consumivel { Nome = "Gaze 7,5cm", Descricao = "Gaze estéril 7,5cm", CategoriaId = categorias.First(c => c.Nome == "Gazes").CategoriaId, QuantidadeMaxima = 220, QuantidadeAtual = 120, QuantidadeMinima = 25 },
            new Consumivel { Nome = "Gaze 10cm", Descricao = "Gaze estéril 10cm", CategoriaId = categorias.First(c => c.Nome == "Gazes").CategoriaId, QuantidadeMaxima = 220, QuantidadeAtual = 130, QuantidadeMinima = 25 },
            new Consumivel { Nome = "Gaze Não Estéril", Descricao = "Gaze comum", CategoriaId = categorias.First(c => c.Nome == "Gazes").CategoriaId, QuantidadeMaxima = 180, QuantidadeAtual = 90, QuantidadeMinima = 20 },
            new Consumivel { Nome = "Algodão Hidrófilo", Descricao = "Rolo de algodão", CategoriaId = categorias.First(c => c.Nome == "Gazes").CategoriaId, QuantidadeMaxima = 100, QuantidadeAtual = 60, QuantidadeMinima = 10 },

            // ---- Desinfetantes ----
            new Consumivel { Nome = "Álcool 96%", Descricao = "Frasco de álcool 96%", CategoriaId = categorias.First(c => c.Nome == "Desinfetantes").CategoriaId, QuantidadeMaxima = 40, QuantidadeAtual = 20, QuantidadeMinima = 5 },
            new Consumivel { Nome = "Clorexidina 2%", Descricao = "Clorexidina degermante", CategoriaId = categorias.First(c => c.Nome == "Desinfetantes").CategoriaId, QuantidadeMaxima = 60, QuantidadeAtual = 30, QuantidadeMinima = 10 },
            new Consumivel { Nome = "Hipoclorito de Sódio", Descricao = "Solução desinfetante", CategoriaId = categorias.First(c => c.Nome == "Desinfetantes").CategoriaId, QuantidadeMaxima = 45, QuantidadeAtual = 25, QuantidadeMinima = 5 },
            new Consumivel { Nome = "Álcool Gel 70%", Descricao = "Gel antisséptico", CategoriaId = categorias.First(c => c.Nome == "Desinfetantes").CategoriaId, QuantidadeMaxima = 80, QuantidadeAtual = 40, QuantidadeMinima = 10 }
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
