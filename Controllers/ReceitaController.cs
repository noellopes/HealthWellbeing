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
        public async Task<IActionResult> Index(string searchNome, int? minTempoPreparo, int? maxTempoPreparo, int page = 1, int pageSize = 10)
        {
            // Base query
            var query = _context.Receita
                .Include(r => r.ReceitaComponentes)
                    .ThenInclude(rc => rc.ComponenteReceita)
                .AsQueryable();

            // Filter by name (if provided)
            if (!string.IsNullOrWhiteSpace(searchNome))
            {
                var term = searchNome.Trim().ToLower();
                query = query.Where(r => r.Nome.ToLower().Contains(term) || 
                                        (r.Descricao != null && r.Descricao.ToLower().Contains(term)));
            }

            // Filter by preparation time range (if provided)
            if (minTempoPreparo.HasValue)
            {
                query = query.Where(r => r.TempoPreparo >= minTempoPreparo.Value);
            }
            if (maxTempoPreparo.HasValue)
            {
                query = query.Where(r => r.TempoPreparo <= maxTempoPreparo.Value);
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Calculate pagination
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            if (totalPages < 1) totalPages = 1;
            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            // Apply pagination and execute query
            var receitas = await query
                .OrderBy(r => r.Nome)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Pass pagination and filter info to view
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.SearchNome = searchNome ?? string.Empty;
            ViewBag.MinTempoPreparo = minTempoPreparo;
            ViewBag.MaxTempoPreparo = maxTempoPreparo;

            return View(receitas);
        }

        // GET: Receita/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var receita = await _context.Receita
                .Include(r => r.ReceitaComponentes)
                    .ThenInclude(rc => rc.ComponenteReceita!)
                        .ThenInclude(c => c.Alimento)
                .FirstOrDefaultAsync(m => m.ReceitaId == id);
            
            if (receita == null)
            {
                return NotFound();
            }

            // Carregar componentes relacionados com detalhes do alimento
            var componentesDetalhados = receita.ReceitaComponentes
                .Select(rc => new {
                    rc.ComponenteReceita!.ComponenteReceitaId,
                    AlimentoNome = rc.ComponenteReceita.Alimento!.Name,
                    rc.ComponenteReceita.Quantidade,
                    Unidade = rc.ComponenteReceita.UnidadeMedida.ToString(),
                    rc.ComponenteReceita.IsOpcional
                })
                .ToList<dynamic>();
            
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
        public async Task<IActionResult> Create([Bind("ReceitaId,Nome,Descricao,ModoPreparo,TempoPreparo,Porcoes,Calorias,Proteinas,HidratosCarbono,Gorduras,RestricoesAlimentarId")] Receita receita, int[] ComponentesReceitaId)
        {
            if (ModelState.IsValid)
            {
                _context.Add(receita);
                await _context.SaveChangesAsync();

                // Add the N:N relationships for componentes
                if (ComponentesReceitaId != null && ComponentesReceitaId.Length > 0)
                {
                    foreach (var componenteId in ComponentesReceitaId)
                    {
                        var receitaComponente = new ReceitaComponente
                        {
                            ReceitaId = receita.ReceitaId,
                            ComponenteReceitaId = componenteId
                        };
                        _context.ReceitaComponente.Add(receitaComponente);
                    }
                    await _context.SaveChangesAsync();
                }

                TempData["AlertMessage"] = $"Receita '{receita.Nome}' criada com sucesso!";
                TempData["AlertType"] = "success";

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

            var receita = await _context.Receita
                .Include(r => r.ReceitaComponentes)
                .FirstOrDefaultAsync(r => r.ReceitaId == id);
            
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

            // Get currently selected component IDs
            var selectedComponentes = receita.ReceitaComponentes
                .Select(rc => rc.ComponenteReceitaId)
                .ToList();

            ViewData["ComponentesReceita"] = new MultiSelectList(componentes, "ComponenteReceitaId", "Display", selectedComponentes);
            ViewData["RestricoesAlimentares"] = new MultiSelectList(restricoes, "RestricaoAlimentarId", "Display", receita.RestricoesAlimentarId);
            return View(receita);
        }

        // POST: Receita/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReceitaId,Nome,Descricao,ModoPreparo,TempoPreparo,Porcoes,Calorias,Proteinas,HidratosCarbono,Gorduras,RestricoesAlimentarId")] Receita receita, int[] ComponentesReceitaId)
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
                    
                    // Remove existing component relationships
                    var existingComponentes = await _context.ReceitaComponente
                        .Where(rc => rc.ReceitaId == receita.ReceitaId)
                        .ToListAsync();
                    _context.ReceitaComponente.RemoveRange(existingComponentes);

                    // Add new component relationships
                    if (ComponentesReceitaId != null && ComponentesReceitaId.Length > 0)
                    {
                        foreach (var componenteId in ComponentesReceitaId)
                        {
                            var receitaComponente = new ReceitaComponente
                            {
                                ReceitaId = receita.ReceitaId,
                                ComponenteReceitaId = componenteId
                            };
                            _context.ReceitaComponente.Add(receitaComponente);
                        }
                    }

                    await _context.SaveChangesAsync();

                    TempData["AlertMessage"] = $"Receita '{receita.Nome}' atualizada com sucesso!";
                    TempData["AlertType"] = "warning";
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
                var receitaNome = receita.Nome;
                _context.Receita.Remove(receita);
                await _context.SaveChangesAsync();
                
                TempData["AlertMessage"] = $"Receita '{receitaNome}' apagada com sucesso!";
                TempData["AlertType"] = "danger";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ReceitaExists(int id)
        {
            return _context.Receita.Any(e => e.ReceitaId == id);
        }
    }
}
