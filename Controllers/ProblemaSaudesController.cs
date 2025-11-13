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
    public class ProblemaSaudesController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ProblemaSaudesController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: ProblemaSaudes
        public async Task<IActionResult> Index(
    string categoria,
    string nome,
    string zona,
    string profissional,
    string gravidade,
    int page = 1)
        {
            int pageSize = 10;

            // --- Query base ---
            var query = _context.ProblemaSaude.AsQueryable();

            // --- Filtros ---
            if (!string.IsNullOrWhiteSpace(categoria))
                query = query.Where(p => p.ProblemaCategoria.ToLower().Contains(categoria.ToLower()));

            if (!string.IsNullOrWhiteSpace(nome))
                query = query.Where(p => p.ProblemaNome.ToLower().Contains(nome.ToLower()));

            if (!string.IsNullOrWhiteSpace(zona))
                query = query.Where(p => p.ZonaAtingida.ToLower().Contains(zona.ToLower()));

            if (!string.IsNullOrWhiteSpace(profissional))
                query = query.Where(p => p.ProfissionalDeApoio.ToLower().Contains(profissional.ToLower()));

            if (!string.IsNullOrWhiteSpace(gravidade))
                query = query.Where(p => p.Gravidade.ToString() == gravidade);

            // --- Paginação ---
            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var items = await query
                .OrderBy(p => p.ProblemaNome)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // --- Passar dados para a View ---
            ViewData["CurrentPage"] = page;
            ViewData["TotalPages"] = totalPages;
            ViewData["Categoria"] = categoria;
            ViewData["Nome"] = nome;
            ViewData["Zona"] = zona;
            ViewData["Profissional"] = profissional;
            ViewData["Gravidade"] = gravidade;

            return View(items);
        }


        // GET: ProblemaSaudes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var problemaSaude = await _context.ProblemaSaude
                .FirstOrDefaultAsync(m => m.ProblemaSaudeId == id);
            if (problemaSaude == null)
            {
                return NotFound();
            }

            return View(problemaSaude);
        }

        // GET: ProblemaSaudes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ProblemaSaudes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProblemaSaudeId,ProblemaCategoria,ProblemaNome,ZonaAtingida,ProfissionalDeApoio,Gravidade")] ProblemaSaude problemaSaude)
        {
            if (ModelState.IsValid)
            {
                _context.Add(problemaSaude);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(problemaSaude);
        }

        // GET: ProblemaSaudes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var problemaSaude = await _context.ProblemaSaude.FindAsync(id);
            if (problemaSaude == null)
            {
                return NotFound();
            }
            return View(problemaSaude);
        }

        // POST: ProblemaSaudes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProblemaSaudeId,ProblemaCategoria,ProblemaNome,ZonaAtingida,ProfissionalDeApoio,Gravidade")] ProblemaSaude problemaSaude)
        {
            if (id != problemaSaude.ProblemaSaudeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(problemaSaude);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProblemaSaudeExists(problemaSaude.ProblemaSaudeId))
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
            return View(problemaSaude);
        }

        // GET: ProblemaSaudes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var problemaSaude = await _context.ProblemaSaude
                .FirstOrDefaultAsync(m => m.ProblemaSaudeId == id);
            if (problemaSaude == null)
            {
                return NotFound();
            }

            return View(problemaSaude);
        }

        // POST: ProblemaSaudes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var problemaSaude = await _context.ProblemaSaude.FindAsync(id);
            if (problemaSaude != null)
            {
                _context.ProblemaSaude.Remove(problemaSaude);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProblemaSaudeExists(int id)
        {
            return _context.ProblemaSaude.Any(e => e.ProblemaSaudeId == id);
        }
    }
}
