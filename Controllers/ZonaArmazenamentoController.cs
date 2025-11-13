using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;

namespace HealthWellbeing.Controllers
{
    public class ZonaArmazenamentoController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ZonaArmazenamentoController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: ZonaArmazenamento
        public async Task<IActionResult> Index(int page = 1, string searchNome = "", string searchLocalizacao = "")
        {
            var zonasQuery = _context.ZonaArmazenamento.AsQueryable();

            // Pesquisa
            if (!string.IsNullOrEmpty(searchNome))
                zonasQuery = zonasQuery.Where(z => z.Nome.Contains(searchNome));

            if (!string.IsNullOrEmpty(searchLocalizacao))
                zonasQuery = zonasQuery.Where(z => z.Localizacao.Contains(searchLocalizacao));

            ViewBag.SearchNome = searchNome;
            ViewBag.SearchLocalizacao = searchLocalizacao;

            // Paginação
            int totalZonas = await zonasQuery.CountAsync();

            var pagination = new PaginationInfo<ZonaArmazenamento>(page, totalZonas, 20);

            pagination.Items = await zonasQuery
                .OrderBy(z => z.Nome)
                .Skip(pagination.ItemsToSkip)
                .Take(pagination.ItemsPerPage)
                .ToListAsync();

            return View(pagination);
        }

        // GET: ZonaArmazenamento/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var zona = await _context.ZonaArmazenamento.FirstOrDefaultAsync(m => m.Id == id);
            if (zona == null)
                return NotFound();

            return View(zona);
        }

        // GET: Create
        public IActionResult Create() => View();

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Descricao,Localizacao,CapacidadeMaxima,Ativa")] ZonaArmazenamento zona)
        {
            if (ModelState.IsValid)
            {
                _context.Add(zona);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "✅ Zona criada com sucesso!";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "❌ Erro ao criar a zona.";
            return View(zona);
        }

        // GET: Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var zona = await _context.ZonaArmazenamento.FindAsync(id);
            if (zona == null)
                return NotFound();

            return View(zona);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Descricao,Localizacao,CapacidadeMaxima,Ativa")] ZonaArmazenamento zona)
        {
            if (id != zona.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(zona);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "💾 Alterações guardadas com sucesso!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ZonaArmazenamento.Any(e => e.Id == id))
                        return NotFound();

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "❌ Erro ao editar a zona.";
            return View(zona);
        }

        // GET: Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var zona = await _context.ZonaArmazenamento.FirstOrDefaultAsync(m => m.Id == id);
            if (zona == null)
                return NotFound();

            return View(zona);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var zona = await _context.ZonaArmazenamento.FindAsync(id);

            if (zona != null)
            {
                _context.ZonaArmazenamento.Remove(zona);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "🗑️ Zona eliminada com sucesso!";
            }
            else
            {
                TempData["ErrorMessage"] = "❌ Erro ao eliminar a zona.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
