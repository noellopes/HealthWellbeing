using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.ViewModels;
using HealthWellBeing.Models;

namespace HealthWellbeing.Controllers
{
    public class RegistoMateriaisController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public RegistoMateriaisController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: RegistoMateriais
        // Adicionados parâmetros: searchString (pesquisa) e page (página atual)
        public async Task<IActionResult> Index(string searchString, int page = 1)
        {
            // 1. Query base com Includes
            var query = _context.RegistoMateriais
                .Include(r => r.Estado)
                .Include(r => r.Exame)
                .ThenInclude(e => e.Utente)
                .AsQueryable();

            // 2. Filtro de Pesquisa por Nome do Material
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(s => s.Nome.Contains(searchString));
            }

            // 3. Configuração de Paginação
            int pageSize = 10; // Itens por página
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // Garante que a página atual é válida
            page = page < 1 ? 1 : page;

            // 4. Executa a paginação no Banco de Dados
            var itensPaginados = await query
                .OrderByDescending(r => r.Exame.DataHoraMarcacao)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // 5. Passa os dados de controle para a View via ViewBag
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(itensPaginados);
        }

        // GET: RegistoMateriais/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var registoMateriais = await _context.RegistoMateriais
                .Include(r => r.Estado)
                .Include(r => r.Exame)
                .ThenInclude(e => e.Utente)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (registoMateriais == null) return NotFound();

            return View(registoMateriais);
        }

        // GET: RegistoMateriais/Create
        public IActionResult Create()
        {
            CarregarListasDropdowns();
            return View();
        }

        // POST: RegistoMateriais/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Tamanho,Quantidade,MaterialStatusId,ExameId")] RegistoMateriais registoMateriais)
        {
            if (ModelState.IsValid)
            {
                _context.Add(registoMateriais);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            CarregarListasDropdowns(registoMateriais);
            return View(registoMateriais);
        }

        // GET: RegistoMateriais/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var registoMateriais = await _context.RegistoMateriais.FindAsync(id);
            if (registoMateriais == null) return NotFound();

            CarregarListasDropdowns(registoMateriais);
            return View(registoMateriais);
        }

        // POST: RegistoMateriais/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Tamanho,Quantidade,MaterialStatusId,ExameId")] RegistoMateriais registoMateriais)
        {
            if (id != registoMateriais.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(registoMateriais);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RegistoMateriaisExists(registoMateriais.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            CarregarListasDropdowns(registoMateriais);
            return View(registoMateriais);
        }

        // GET: RegistoMateriais/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var registoMateriais = await _context.RegistoMateriais
                .Include(r => r.Estado)
                .Include(r => r.Exame)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (registoMateriais == null) return NotFound();

            return View(registoMateriais);
        }

        // POST: RegistoMateriais/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var registoMateriais = await _context.RegistoMateriais.FindAsync(id);
            if (registoMateriais != null)
            {
                _context.RegistoMateriais.Remove(registoMateriais);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private void CarregarListasDropdowns(RegistoMateriais registoMateriais = null)
        {
            ViewData["MaterialStatusId"] = new SelectList(_context.EstadosMaterial, "MaterialStatusId", "Nome", registoMateriais?.MaterialStatusId);

            var listaExames = _context.Exames
                .Include(e => e.Utente)
                .ToList()
                .Select(e => new
                {
                    ExameId = e.ExameId,
                    DescricaoExame = $"{(e.Utente != null ? e.Utente.Nome : "Sem Utente")} - {e.DataHoraMarcacao:dd/MM/yyyy HH:mm}"
                })
                .OrderBy(e => e.DescricaoExame);

            ViewData["ExameId"] = new SelectList(listaExames, "ExameId", "DescricaoExame", registoMateriais?.ExameId);
        }

        private bool RegistoMateriaisExists(int id)
        {
            return _context.RegistoMateriais.Any(e => e.Id == id);
        }
    }
}