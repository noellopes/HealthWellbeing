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
    public class ReceitasController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ReceitasController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Receitas
        public async Task<IActionResult> Index()
        {
            return View(await _context.Receita.ToListAsync());
        }

        // GET: Receitas/Details/5
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

            return View(receita);
        }

        // GET: Receitas/Create
        public IActionResult Create()
        {
            ViewData["Restricoes"] = new MultiSelectList(_context.RestricaoAlimentar, "RestricaoAlimentarId", "Nome");
            ViewData["Componentes"] = new MultiSelectList(_context.ComponentesDaReceita, "ComponentesDaReceitaId", "Nome");
            return View();
        }

        // POST: Receitas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReceitaId,Nome,Descricao,ModoPreparo,TempoPreparo,Porcoes,CaloriasPorPorcao,Proteinas,HidratosCarbono,Gorduras")] Receita receita, int[] selectedRestricaoIds, int[] selectedComponenteIds)
        {
            if (ModelState.IsValid)
            {
                _context.Add(receita);
                await _context.SaveChangesAsync();

                // Assign selected restrictions
                if (selectedRestricaoIds != null)
                {
                    foreach (var id in selectedRestricaoIds)
                    {
                        var restricao = await _context.RestricaoAlimentar.FindAsync(id);
                        if (restricao != null)
                        {
                            receita.RestricoesAlimentares ??= new List<RestricaoAlimentar>();
                            receita.RestricoesAlimentares.Add(restricao);
                        }
                    }
                }

                // Assign selected components
                if (selectedComponenteIds != null)
                {
                    foreach (var id in selectedComponenteIds)
                    {
                        var componente = await _context.ComponentesDaReceita.FindAsync(id);
                        if (componente != null)
                        {
                            componente.ReceitaId = receita.ReceitaId;
                            _context.Update(componente);
                        }
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Restricoes"] = new MultiSelectList(_context.RestricaoAlimentar, "RestricaoAlimentarId", "Nome");
            ViewData["Componentes"] = new MultiSelectList(_context.ComponentesDaReceita, "ComponentesDaReceitaId", "Nome");
            return View(receita);
        }

        // GET: Receitas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var receita = await _context.Receita
                .Include(r => r.RestricoesAlimentares)
                .Include(r => r.Componentes)
                .FirstOrDefaultAsync(m => m.ReceitaId == id);
            if (receita == null)
            {
                return NotFound();
            }
            ViewData["Restricoes"] = new MultiSelectList(_context.RestricaoAlimentar, "RestricaoAlimentarId", "Nome", receita.RestricoesAlimentares?.Select(r => r.RestricaoAlimentarId));
            ViewData["Componentes"] = new MultiSelectList(_context.ComponentesDaReceita, "ComponentesDaReceitaId", "Nome", receita.Componentes?.Select(c => c.ComponentesDaReceitaId));
            return View(receita);
        }

        // POST: Receitas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReceitaId,Nome,Descricao,ModoPreparo,TempoPreparo,Porcoes,CaloriasPorPorcao,Proteinas,HidratosCarbono,Gorduras")] Receita receita, int[] selectedRestricaoIds, int[] selectedComponenteIds)
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

                    // Clear existing restrictions
                    receita.RestricoesAlimentares?.Clear();

                    // Assign selected restrictions
                    if (selectedRestricaoIds != null)
                    {
                        foreach (var restricaoId in selectedRestricaoIds)
                        {
                            var restricao = await _context.RestricaoAlimentar.FindAsync(restricaoId);
                            if (restricao != null)
                            {
                                receita.RestricoesAlimentares ??= new List<RestricaoAlimentar>();
                                receita.RestricoesAlimentares.Add(restricao);
                            }
                        }
                    }

                    // Clear existing components
                    var existingComponentes = _context.ComponentesDaReceita.Where(c => c.ReceitaId == receita.ReceitaId);
                    foreach (var componente in existingComponentes)
                    {
                        componente.ReceitaId = 0;
                        _context.Update(componente);
                    }

                    // Assign selected components
                    if (selectedComponenteIds != null)
                    {
                        foreach (var componenteId in selectedComponenteIds)
                        {
                            var componente = await _context.ComponentesDaReceita.FindAsync(componenteId);
                            if (componente != null)
                            {
                                componente.ReceitaId = receita.ReceitaId;
                                _context.Update(componente);
                            }
                        }
                    }

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
            ViewData["Restricoes"] = new MultiSelectList(_context.RestricaoAlimentar, "RestricaoAlimentarId", "Nome", receita.RestricoesAlimentares?.Select(r => r.RestricaoAlimentarId));
            ViewData["Componentes"] = new MultiSelectList(_context.ComponentesDaReceita, "ComponentesDaReceitaId", "Nome", receita.Componentes?.Select(c => c.ComponentesDaReceitaId));
            return View(receita);
        }

        // GET: Receitas/Delete/5
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

        // POST: Receitas/Delete/5
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
