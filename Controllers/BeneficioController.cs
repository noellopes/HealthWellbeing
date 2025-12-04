using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using static HealthWellbeing.Data.SeedData;

namespace HealthWellbeing.Controllers
{
    [Authorize(Roles = SeedData.Roles.Administrador + "," + SeedData.Roles.Profissional)]

    public class BeneficioController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public BeneficioController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Beneficio
        public async Task<IActionResult> Index(string searchNome, string searchDescricao, int page = 1)
        {
            var query = _context.Beneficio.AsQueryable();

            // --- 1. Filtros de Pesquisa ---
            if (!string.IsNullOrEmpty(searchNome))
            {
                query = query.Where(b => b.NomeBeneficio.Contains(searchNome));
            }

            if (!string.IsNullOrEmpty(searchDescricao))
            {
                query.Where(b => b.DescricaoBeneficio.Contains(searchDescricao));
            }

            // --- 2. Paginação ---
            int totalItems = await query.CountAsync();
            int pageSize = 5;

            var pagination = new PaginationInfo<Beneficio>(page, totalItems, pageSize);

            // --- 3. Obter Dados ---
            var items = await query
                .OrderBy(b => b.NomeBeneficio)
                .Skip(pagination.ItemsToSkip)
                .Take(pagination.ItemsPerPage)
                .ToListAsync();

            pagination.Items = items;

            // --- 4. Manter o estado da pesquisa ---
            ViewBag.SearchNome = searchNome;
            ViewBag.SearchDescricao = searchDescricao;

            return View(pagination);
        }

        // GET: Beneficio/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var beneficio = await _context.Beneficio
                .FirstOrDefaultAsync(m => m.BeneficioId == id);

            if (beneficio == null) return NotFound();

            return View(beneficio);
        }

        // GET: Beneficio/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Beneficio/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BeneficioId,NomeBeneficio,DescricaoBeneficio")] Beneficio beneficio)
        {

            var existingBeneficio = await _context.Beneficio
                .FirstOrDefaultAsync(b => b.NomeBeneficio.ToLower() == beneficio.NomeBeneficio.ToLower());

            if (existingBeneficio != null)
            {
                TempData["StatusMessage"] = $"Erro: O Benefício '{beneficio.NomeBeneficio}' já existe no sistema.";
                return View(beneficio);
            }


            if (ModelState.IsValid)
            {
                _context.Add(beneficio);
                await _context.SaveChangesAsync();
                TempData["StatusMessage"] = $"Sucesso: O Benefício '{beneficio.NomeBeneficio}' foi criado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            return View(beneficio);
        }

        // GET: Beneficio/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var beneficio = await _context.Beneficio.FindAsync(id);
            if (beneficio == null) return NotFound();

            return View(beneficio);
        }

        // POST: Beneficio/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BeneficioId,NomeBeneficio,DescricaoBeneficio")] Beneficio beneficio)
        {
            if (id != beneficio.BeneficioId) return NotFound();


            var existingBeneficioWithSameName = await _context.Beneficio
                .FirstOrDefaultAsync(b =>
                    b.NomeBeneficio.ToLower() == beneficio.NomeBeneficio.ToLower() &&
                    b.BeneficioId != id);

            if (existingBeneficioWithSameName != null)
            {
                
                ViewData["StatusMessage"] = $"Erro: O Benefício '{beneficio.NomeBeneficio}' já existe para outro registo.";
                return View(beneficio);
            }


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(beneficio);
                    await _context.SaveChangesAsync();
                    TempData["StatusMessage"] = "Sucesso: Benefício atualizado com sucesso!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BeneficioExists(beneficio.BeneficioId))
                    {

                        return RedirectToAction(nameof(Invalido), beneficio);
                    }
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(beneficio);
        }


        // GET: Beneficio/Invalido
        public IActionResult Invalido(Beneficio beneficio)
        {
            return View("InvalidBeneficio", beneficio);
        }


        // GET: Beneficio/Delete/5
        public async Task<IActionResult> Delete(int? id, bool concurrencyError = false)
        {
            if (id == null) return NotFound();

            var beneficio = await _context.Beneficio
                .FirstOrDefaultAsync(m => m.BeneficioId == id);

            if (beneficio == null)
            {
                if (!concurrencyError) return NotFound();
                return RedirectToAction(nameof(Index));
            }


            var hasAssociations = await _context.TipoExercicioBeneficio
                .AnyAsync(tb => tb.BeneficioId == id);

            if (hasAssociations)
            {
                ViewData["ErrorMessage"] = $"Não é possível excluir o benefício '{beneficio.NomeBeneficio}', pois está associado a um ou mais Tipos de Exercício.";
                ViewData["DisableDelete"] = true;
            }

            if (concurrencyError)
            {
                ViewData["ErrorMessage"] = ViewData["ErrorMessage"] != null
                    ? ViewData["ErrorMessage"]
                    : "O benefício foi modificado por outro utilizador. Por favor, reveja e tente novamente.";
            }

            return View(beneficio);
        }

        // POST: Beneficio/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var beneficio = await _context.Beneficio.FindAsync(id);
            if (beneficio == null) return RedirectToAction(nameof(Index));


            var hasAssociations = await _context.TipoExercicioBeneficio
                .AnyAsync(tb => tb.BeneficioId == id);

            if (hasAssociations)
            {
                return RedirectToAction(nameof(Delete), new { id = id });
            }

            try
            {
                _context.Beneficio.Remove(beneficio);
                await _context.SaveChangesAsync();
                TempData["StatusMessage"] = "Sucesso: Benefício eliminado com sucesso!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BeneficioExists(id)) return RedirectToAction(nameof(Index));
                else return RedirectToAction(nameof(Delete), new { id = id, concurrencyError = true });
            }

            return RedirectToAction(nameof(Index));
        }

        private bool BeneficioExists(int id)
        {
            return _context.Beneficio.Any(e => e.BeneficioId == id);
        }
    }
}