using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers
{
    public class ConsumivelController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ConsumivelController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Consumivel
        public async Task<IActionResult> Index()
        {
            // Se a tabela de consumíveis estiver vazia, insere registos fictícios automaticamente
            if (!_context.Consumivel.Any())
            {
                var consumiveis = new List<Consumivel>
                {
                    new Consumivel { Nome = "Luva Cirúrgica Estéril", Descricao = "Luva estéril de látex para procedimentos cirúrgicos.", CategoriaId = 1, QuantidadeMaxima = 200, QuantidadeAtual = 120, QuantidadeMinima = 50 },
                    new Consumivel { Nome = "Máscara N95", Descricao = "Máscara respiratória de alta filtragem para uso hospitalar.", CategoriaId = 2, QuantidadeMaxima = 300, QuantidadeAtual = 150, QuantidadeMinima = 75 },
                    new Consumivel { Nome = "Seringa de 10ml", Descricao = "Seringa descartável com graduação precisa.", CategoriaId = 3, QuantidadeMaxima = 500, QuantidadeAtual = 320, QuantidadeMinima = 100 },
                    new Consumivel { Nome = "Cateter Venoso Periférico", Descricao = "Cateter para acesso venoso em terapias intravenosas.", CategoriaId = 4, QuantidadeMaxima = 250, QuantidadeAtual = 180, QuantidadeMinima = 50 },
                    new Consumivel { Nome = "Compressa Estéril 10x10", Descricao = "Compressa cirúrgica esterilizada para curativos.", CategoriaId = 5, QuantidadeMaxima = 400, QuantidadeAtual = 220, QuantidadeMinima = 80 }
                };

                _context.Consumivel.AddRange(consumiveis);
                await _context.SaveChangesAsync();
            }

            var healthWellbeingDbContext = _context.Consumivel.Include(c => c.CategoriaConsumivel);
            return View(await healthWellbeingDbContext.ToListAsync());
        }

        // GET: Consumivel/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consumivel = await _context.Consumivel
                .Include(c => c.CategoriaConsumivel)
                .FirstOrDefaultAsync(m => m.ConsumivelId == id);
            if (consumivel == null)
            {
                return NotFound();
            }

            return View(consumivel);
        }

        // GET: Consumivel/Create
        public IActionResult Create()
        {
            ViewData["CategoriaId"] = new SelectList(_context.CategoriaConsumivel, "CategoriaId", "Nome");
            return View();
        }

        // POST: Consumivel/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ConsumivelId,Nome,Descricao,CategoriaId,QuantidadeMaxima,QuantidadeAtual,QuantidadeMinima")] Consumivel consumivel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(consumivel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.CategoriaConsumivel, "CategoriaId", "Nome", consumivel.CategoriaId);
            return View(consumivel);
        }

        // GET: Consumivel/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consumivel = await _context.Consumivel.FindAsync(id);
            if (consumivel == null)
            {
                return NotFound();
            }
            ViewData["CategoriaId"] = new SelectList(_context.CategoriaConsumivel, "CategoriaId", "Nome", consumivel.CategoriaId);
            return View(consumivel);
        }

        // POST: Consumivel/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ConsumivelId,Nome,Descricao,CategoriaId,QuantidadeMaxima,QuantidadeAtual,QuantidadeMinima")] Consumivel consumivel)
        {
            if (id != consumivel.ConsumivelId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(consumivel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConsumivelExists(consumivel.ConsumivelId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoriaId"] = new SelectList(_context.CategoriaConsumivel, "CategoriaId", "Nome", consumivel.CategoriaId);
            return View(consumivel);
        }

        // GET: Consumivel/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var consumivel = await _context.Consumivel
                .Include(c => c.CategoriaConsumivel)
                .FirstOrDefaultAsync(m => m.ConsumivelId == id);
            if (consumivel == null)
            {
                return NotFound();
            }

            return View(consumivel);
        }

        // POST: Consumivel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var consumivel = await _context.Consumivel.FindAsync(id);
            if (consumivel != null)
            {
                _context.Consumivel.Remove(consumivel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ConsumivelExists(int id)
        {
            return _context.Consumivel.Any(e => e.ConsumivelId == id);
        }
    }
}
