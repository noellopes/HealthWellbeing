using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthWellbeing.Controllers
{
    // Acesso básico à lista (Index) e Detalhes permitido a Admin, Gestor e Tecnico
    [Authorize(Roles = "Admin, Supervisor Tecnico, Tecnico")]
    public class ProfissionalExecutantesController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ProfissionalExecutantesController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: ProfissionalExecutantes
        public async Task<IActionResult> Index(int page = 1, string searchNome = null, string searchFuncao = null)
        {
            int itemsPerPage = 5;

            IQueryable<ProfissionalExecutante> profissionaisQuery =
                _context.ProfissionalExecutante
                .Include(p => p.Funcao)
                .OrderBy(p => p.Nome);

            if (!string.IsNullOrEmpty(searchNome))
                profissionaisQuery = profissionaisQuery.Where(p => p.Nome.Contains(searchNome));

            if (!string.IsNullOrEmpty(searchFuncao))
                profissionaisQuery = profissionaisQuery.Where(p => p.Funcao.NomeFuncao.Contains(searchFuncao));

            int totalItems = await profissionaisQuery.CountAsync();
            var paginationInfo = new PaginationInfo<ProfissionalExecutante>(page, totalItems, itemsPerPage);

            ViewBag.TotalProfissionais = await _context.ProfissionalExecutante.CountAsync();
            ViewBag.TotalFuncoes = await _context.Funcoes.CountAsync();
            ViewBag.SearchNome = searchNome;
            ViewBag.SearchFuncao = searchFuncao;

            paginationInfo.Items = await profissionaisQuery
                .Skip(paginationInfo.ItemsToSkip)
                .Take(paginationInfo.ItemsPerPage)
                .ToListAsync();

            return View(paginationInfo);
        }

        // GET: ProfissionalExecutantes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var profissional = await _context.ProfissionalExecutante
                .Include(p => p.Funcao)
                .FirstOrDefaultAsync(m => m.ProfissionalExecutanteId == id);

            if (profissional == null) return NotFound();

            return View(profissional);
        }

        // ============================================================
        // ACÇÕES RESTRITAS APENAS A ADMIN E GESTOR (Tecnico não entra)
        // ============================================================

        // GET: ProfissionalExecutantes/Create
        [Authorize(Roles = "Admin, Gestor")]
        public IActionResult Create()
        {
            ViewData["FuncaoId"] = new SelectList(_context.Funcoes, "FuncaoId", "NomeFuncao");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Gestor")]
        public async Task<IActionResult> Create([Bind("ProfissionalExecutanteId,Nome,Telefone,Email,FuncaoId")] ProfissionalExecutante profissionalExecutante)
        {
            if (ModelState.IsValid)
            {
                _context.Add(profissionalExecutante);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Novo profissional executante criado!";
                return RedirectToAction(nameof(Index));
            }

            ViewData["FuncaoId"] = new SelectList(_context.Funcoes, "FuncaoId", "NomeFuncao", profissionalExecutante.FuncaoId);
            return View(profissionalExecutante);
        }

        // GET: ProfissionalExecutantes/Edit/5
        [Authorize(Roles = "Admin, Gestor")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var profissional = await _context.ProfissionalExecutante
                .Include(p => p.Funcao)
                .FirstOrDefaultAsync(p => p.ProfissionalExecutanteId == id);

            if (profissional == null) return NotFound();

            ViewData["FuncaoId"] = new SelectList(_context.Funcoes, "FuncaoId", "NomeFuncao", profissional.FuncaoId);
            return View(profissional);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Gestor")]
        public async Task<IActionResult> Edit(int id, [Bind("ProfissionalExecutanteId,Nome,Telefone,Email,FuncaoId")] ProfissionalExecutante profissionalExecutante)
        {
            if (id != profissionalExecutante.ProfissionalExecutanteId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(profissionalExecutante);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Dados do Profissional editados!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ProfissionalExecutante.Any(e => e.ProfissionalExecutanteId == profissionalExecutante.ProfissionalExecutanteId))
                        return NotFound();
                    else
                        throw;
                }
            }

            ViewData["FuncaoId"] = new SelectList(_context.Funcoes, "FuncaoId", "NomeFuncao", profissionalExecutante.FuncaoId);
            return View(profissionalExecutante);
        }

        // GET: ProfissionalExecutantes/Delete/5
        [Authorize(Roles = "Admin, Gestor")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var profissional = await _context.ProfissionalExecutante
                .Include(p => p.Funcao)
                .FirstOrDefaultAsync(m => m.ProfissionalExecutanteId == id);

            if (profissional == null) return NotFound();

            return View(profissional);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Gestor")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var profissional = await _context.ProfissionalExecutante.FindAsync(id);
            if (profissional != null)
            {
                _context.ProfissionalExecutante.Remove(profissional);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Profissional executante removido!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}