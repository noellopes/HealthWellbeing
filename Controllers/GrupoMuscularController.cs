using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "GrupoMuscular")]
    public class GrupoMuscularController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public GrupoMuscularController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: GrupoMuscular
        public async Task<IActionResult> Index(
        int page = 1,
        string searchNome = "",
        string searchLocalizacao = "")
        {
            // Começa com a query completa
            var query = _context.GrupoMuscular.AsQueryable();

            // Filtra pelo nome se houver
            if (!string.IsNullOrEmpty(searchNome))
                query = query.Where(g => g.GrupoMuscularNome.Contains(searchNome));

            // Filtra pela localização corporal se houver
            if (!string.IsNullOrEmpty(searchLocalizacao))
                query = query.Where(g => g.LocalizacaoCorporal.Contains(searchLocalizacao));

            // Mantém os valores da pesquisa para a view
            ViewBag.SearchNome = searchNome;
            ViewBag.SearchLocalizacao = searchLocalizacao;

            // Paginação
            int totalItems = await query.CountAsync();
            var paginated = new PaginationInfo<GrupoMuscular>(page, totalItems);
            paginated.Items = await query
                .OrderBy(g => g.GrupoMuscularNome)
                .Skip(paginated.ItemsToSkip)
                .Take(paginated.ItemsPerPage)
                .ToListAsync();

            return View(paginated);
        }

        // GET: GrupoMuscular/InvalidGrupoMuscular
        public IActionResult InvalidGrupoMuscular(GrupoMuscular grupoMuscular)
        {
            // Esta Action apenas retorna a View, passando o objeto GrupoMuscular 
            // (que pode conter os dados a recuperar).
            return View(grupoMuscular);
        }

        // GET: GrupoMuscular/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var grupoMuscular = await _context.GrupoMuscular
                .FirstOrDefaultAsync(m => m.GrupoMuscularId == id);
            if (grupoMuscular == null)
            {
                return RedirectToAction(nameof(InvalidGrupoMuscular), new GrupoMuscular { GrupoMuscularId = id.Value });
            }

            return View(grupoMuscular);
        }

        // GET: GrupoMuscular/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GrupoMuscular/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GrupoMuscularId,GrupoMuscularNome,LocalizacaoCorporal")] GrupoMuscular grupoMuscular)
        {
            if (ModelState.IsValid)
            {
                _context.Add(grupoMuscular);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(grupoMuscular);
        }

        // GET: GrupoMuscular/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var grupoMuscular = await _context.GrupoMuscular.FindAsync(id);
            if (grupoMuscular == null)
            {
                return RedirectToAction(nameof(InvalidGrupoMuscular), new GrupoMuscular { GrupoMuscularId = id.Value });
            }
            return View(grupoMuscular);
        }

        // POST: GrupoMuscular/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GrupoMuscularId,GrupoMuscularNome,LocalizacaoCorporal")] GrupoMuscular grupoMuscular)
        {
            if (id != grupoMuscular.GrupoMuscularId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(grupoMuscular);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GrupoMuscularExists(grupoMuscular.GrupoMuscularId))
                    {
                        return RedirectToAction(nameof(InvalidGrupoMuscular), grupoMuscular);
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(grupoMuscular);
        }

        // GET: GrupoMuscular/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var grupoMuscular = await _context.GrupoMuscular
                .FirstOrDefaultAsync(m => m.GrupoMuscularId == id);
            if (grupoMuscular == null)
            {
                return RedirectToAction(nameof(InvalidGrupoMuscular), new GrupoMuscular { GrupoMuscularId = id.Value });
            }

            return View(grupoMuscular);
        }

        // POST: GrupoMuscular/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var grupoMuscular = await _context.GrupoMuscular.FindAsync(id);
            if (grupoMuscular != null)
            {
                _context.GrupoMuscular.Remove(grupoMuscular);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GrupoMuscularExists(int id)
        {
            return _context.GrupoMuscular.Any(e => e.GrupoMuscularId == id);
        }
    }
}