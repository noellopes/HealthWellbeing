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
    public class ReceitaController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ReceitaController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Receita
        public async Task<IActionResult> Index()
        {
            return View(await _context.Receita.ToListAsync());
        }

        // GET: Receita/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var receita = await _context.Receita
                .FirstOrDefaultAsync(m => m.ReceitaId == id);
            if (receita == null)
            {
                return NotFound();
            }

            // Carregar componentes relacionados com detalhes do alimento
            var componentesDetalhados = new List<dynamic>();
            if (receita.ComponentesReceitaId != null && receita.ComponentesReceitaId.Any())
            {
                componentesDetalhados = await _context.ComponenteReceita
                    .Include(c => c.Alimento)
                    .Where(c => receita.ComponentesReceitaId.Contains(c.ComponenteReceitaId))
                    .Select(c => new {
                        c.ComponenteReceitaId,
                        AlimentoNome = c.Alimento!.Name,
                        c.Quantidade,
                        Unidade = c.UnidadeMedida.ToString(),
                        c.IsOpcional
                    })
                    .ToListAsync<dynamic>();
            }
            ViewBag.ComponentesDetalhados = componentesDetalhados;

            // Carregar restrições relacionadas
            var restricoesDetalhadas = new List<dynamic>();
            if (receita.RestricoesAlimentarId != null && receita.RestricoesAlimentarId.Any())
            {
                restricoesDetalhadas = await _context.RestricaoAlimentar
                    .Where(r => receita.RestricoesAlimentarId.Contains(r.RestricaoAlimentarId))
                    .Select(r => new {
                        r.RestricaoAlimentarId,
                        r.Nome,
                        Tipo = r.Tipo.ToString(),
                        Gravidade = r.Gravidade.ToString(),
                        r.Descricao
                    })
                    .ToListAsync<dynamic>();
            }
            ViewBag.RestricoesDetalhadas = restricoesDetalhadas;

            return View(receita);
        }

        // GET: Receita/Create
        public IActionResult Create()
        {
            // Carregar componentes com informações do alimento
            var componentes = _context.ComponenteReceita
                .Include(c => c.Alimento)
                .Select(c => new {
                    c.ComponenteReceitaId,
                    Display = $"{c.Alimento!.Name} - {c.Quantidade} {c.UnidadeMedida}" + 
                              (c.IsOpcional ? " (Opcional)" : "")
                })
                .ToList();
            
            // Carregar restrições com tipo e gravidade
            var restricoes = _context.RestricaoAlimentar
                .Select(r => new {
                    r.RestricaoAlimentarId,
                    Display = $"{r.Nome} - {r.Tipo} ({r.Gravidade})"
                })
                .ToList();
           
            ViewData["ComponentesReceita"] = new MultiSelectList(componentes, "ComponenteReceitaId", "Display");
            ViewData["RestricoesAlimentares"] = new MultiSelectList(restricoes, "RestricaoAlimentarId", "Display");
            return View();
        }

        // POST: Receita/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReceitaId,Nome,Descricao,ModoPreparo,TempoPreparo,Porcoes,Calorias,Proteinas,HidratosCarbono,Gorduras,ComponentesReceitaId,RestricoesAlimentarId")] Receita receita)
        {
            if (ModelState.IsValid)
            {
                _context.Add(receita);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(receita);
        }

        // GET: Receita/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var receita = await _context.Receita.FindAsync(id);
            if (receita == null)
            {
                return NotFound();
            }

            if (_context.ComponenteReceita == null || _context.RestricaoAlimentar == null)
            {
                return Problem("Required data is missing in the database.");
            }

            // Carregar componentes com informações do alimento
            var componentes = _context.ComponenteReceita
                .Include(c => c.Alimento)
                .Select(c => new {
                    c.ComponenteReceitaId,
                    Display = $"{c.Alimento!.Name} - {c.Quantidade} {c.UnidadeMedida}" + 
                              (c.IsOpcional ? " (Opcional)" : "")
                })
                .ToList();
            
            // Carregar restrições com tipo e gravidade
            var restricoes = _context.RestricaoAlimentar
                .Select(r => new {
                    r.RestricaoAlimentarId,
                    Display = $"{r.Nome} - {r.Tipo} ({r.Gravidade})"
                })
                .ToList();

            ViewData["ComponentesReceita"] = new MultiSelectList(componentes, "ComponenteReceitaId", "Display", receita.ComponentesReceitaId);
            ViewData["RestricoesAlimentares"] = new MultiSelectList(restricoes, "RestricaoAlimentarId", "Display", receita.RestricoesAlimentarId);
            return View(receita);
        }

        // POST: Receita/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReceitaId,Nome,Descricao,ModoPreparo,TempoPreparo,Porcoes,Calorias,Proteinas,HidratosCarbono,Gorduras,ComponentesReceitaId,RestricoesAlimentarId")] Receita receita)
        {
            if (id != receita.ReceitaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(receita);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReceitaExists(receita.ReceitaId))
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
            return View(receita);
        }

        // GET: Receita/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var receita = await _context.Receita
                .FirstOrDefaultAsync(m => m.ReceitaId == id);
            if (receita == null)
            {
                return NotFound();
            }

            return View(receita);
        }

        // POST: Receita/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var receita = await _context.Receita.FindAsync(id);
            if (receita != null)
            {
                _context.Receita.Remove(receita);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReceitaExists(int id)
        {
            return _context.Receita.Any(e => e.ReceitaId == id);
        }
    }
}
