using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers
{
    [Authorize]
    public class ComponenteReceitaController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ComponenteReceitaController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: ComponenteReceita
        public async Task<IActionResult> Index(string alimentoName, string opcional, string receitaNome, int page = 1, int pageSize = 10)
        {
            // Monta query base incluindo Alimento e Receita
            var query = _context.ComponenteReceita
                .Include(c => c.Alimento)
                .Include(c => c.Receita)
                .AsQueryable();

            // Filtra por nome do alimento (se informado)
            if (!string.IsNullOrWhiteSpace(alimentoName))
            {
                var term = alimentoName.Trim().ToLower();
                query = query.Where(c => c.Alimento != null && c.Alimento.Name != null && c.Alimento.Name.ToLower().Contains(term));
            }

            // Filtra por opcional (se informado: "true"/"false")
            if (!string.IsNullOrWhiteSpace(opcional))
            {
                if (bool.TryParse(opcional, out bool opc))
                {
                    query = query.Where(c => c.IsOpcional == opc);
                }
            }

            // Filtra por nome da receita (se informado)
            if (!string.IsNullOrWhiteSpace(receitaNome))
            {
                var rTerm = receitaNome.Trim().ToLower();
                query = query.Where(c => c.Receita != null && c.Receita.Nome.ToLower().Contains(rTerm));
            }

            // Total antes da paginação
            var totalCount = await query.CountAsync();

            // Calcula páginas
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            if (totalPages < 1) totalPages = 1;
            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var items = await query
                .OrderBy(c => c.Alimento != null ? c.Alimento.Name : string.Empty)
                .ThenBy(c => c.ComponenteReceitaId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Passa informações para a view
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.SearchAlimentoName = alimentoName ?? string.Empty;
            ViewBag.SearchOpcional = opcional ?? string.Empty;
            ViewBag.SearchReceitaNome = receitaNome ?? string.Empty;

            return View(items);
        }

        // GET: ComponenteReceita/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var componenteReceita = await _context.ComponenteReceita
                .Include(c => c.Alimento)
                .FirstOrDefaultAsync(m => m.ComponenteReceitaId == id);
            if (componenteReceita == null)
            {
                return NotFound();
            }

            return View(componenteReceita);
        }

        // GET: ComponenteReceita/Create
        [Authorize(Roles = "Nutricionista")]
        public IActionResult Create()
        {
            ViewData["AlimentoId"] = new SelectList(_context.Alimentos, "AlimentoId", "Name");
            ViewData["ReceitaId"] = new SelectList(_context.Receita, "ReceitaId", "Nome");
            return View();
        }

        // POST: ComponenteReceita/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Nutricionista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ComponenteReceitaId,AlimentoId,ReceitaId,UnidadeMedida,Quantidade,IsOpcional")] ComponenteReceita componenteReceita)
        {
            if (ModelState.IsValid)
            {
                _context.Add(componenteReceita);
                await _context.SaveChangesAsync();

                // Mensagem de sucesso em português
                TempData["AlertMessage"] = "Componente criado com sucesso.";
                TempData["AlertType"] = "success";

                return RedirectToAction(nameof(Index));
            }
            ViewData["AlimentoId"] = new SelectList(_context.Alimentos, "AlimentoId", "Name", componenteReceita.AlimentoId);
            ViewData["ReceitaId"] = new SelectList(_context.Receita, "ReceitaId", "Nome", componenteReceita.ReceitaId);
            return View(componenteReceita);
        }

        // GET: ComponenteReceita/Edit/5
        [Authorize(Roles = "Nutricionista")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var componenteReceita = await _context.ComponenteReceita.FindAsync(id);
            if (componenteReceita == null)
            {
                return NotFound();
            }
            ViewData["AlimentoId"] = new SelectList(_context.Alimentos, "AlimentoId", "Name", componenteReceita.AlimentoId);
            ViewData["ReceitaId"] = new SelectList(_context.Receita, "ReceitaId", "Nome", componenteReceita.ReceitaId);
            return View(componenteReceita);
        }

        // POST: ComponenteReceita/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Nutricionista")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ComponenteReceitaId,AlimentoId,ReceitaId,UnidadeMedida,Quantidade,IsOpcional")] ComponenteReceita componenteReceita)
        {
            if (id != componenteReceita.ComponenteReceitaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(componenteReceita);
                    await _context.SaveChangesAsync();

                    TempData["AlertMessage"] = "Componente atualizado com sucesso.";
                    TempData["AlertType"] = "success";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ComponenteReceitaExists(componenteReceita.ComponenteReceitaId))
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
            ViewData["AlimentoId"] = new SelectList(_context.Alimentos, "AlimentoId", "Name", componenteReceita.AlimentoId);
            ViewData["ReceitaId"] = new SelectList(_context.Receita, "ReceitaId", "Nome", componenteReceita.ReceitaId);
            return View(componenteReceita);
        }

        // GET: ComponenteReceita/Delete/5
        [Authorize(Roles = "Nutricionista")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var componenteReceita = await _context.ComponenteReceita
                .Include(c => c.Alimento)
                .Include(c => c.Receita)
                .FirstOrDefaultAsync(m => m.ComponenteReceitaId == id);
            if (componenteReceita == null)
            {
                return NotFound();
            }

            return View(componenteReceita);
        }

        // POST: ComponenteReceita/Delete/5
        [Authorize(Roles = "Nutricionista")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var componenteReceita = await _context.ComponenteReceita.FindAsync(id);
            if (componenteReceita != null)
            {
                _context.ComponenteReceita.Remove(componenteReceita);
                TempData["AlertMessage"] = "Componente apagado com sucesso.";
                TempData["AlertType"] = "warning";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ComponenteReceitaExists(int id)
        {
            return _context.ComponenteReceita.Any(e => e.ComponenteReceitaId == id);
        }
    }
}
