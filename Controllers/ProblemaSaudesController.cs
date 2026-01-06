using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Controllers
{
    [Authorize(Roles = SeedData.Roles.Administrador + "," + SeedData.Roles.Profissional)]
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
            string gravidade,
            int page = 1)
        {
            int pageSize = 10;

            var query = _context.ProblemaSaude.AsQueryable();

            if (!string.IsNullOrWhiteSpace(categoria))
                query = query.Where(p => p.ProblemaCategoria.Contains(categoria));

            if (!string.IsNullOrWhiteSpace(nome))
                query = query.Where(p => p.ProblemaNome.Contains(nome));

            if (!string.IsNullOrWhiteSpace(zona))
                query = query.Where(p => p.ZonaAtingida.Contains(zona));

            if (!string.IsNullOrWhiteSpace(gravidade))
                query = query.Where(p => p.Gravidade.ToString() == gravidade);

            int totalItems = await query.CountAsync();

            var pagination = new PaginationInfo<ProblemaSaude>(page, totalItems, pageSize);

            pagination.Items = await query
                .OrderBy(p => p.ProblemaNome)
                .Skip(pagination.ItemsToSkip)
                .Take(pagination.ItemsPerPage)
                .ToListAsync();

            ViewBag.Categoria = categoria;
            ViewBag.Nome = nome;
            ViewBag.Zona = zona;
            ViewBag.Gravidade = gravidade;

            return View(pagination);
        }

        // GET: ProblemaSaudes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return View("invalidProblemaSaude");

            var problemaSaude = await _context.ProblemaSaude
                .FirstOrDefaultAsync(m => m.ProblemaSaudeId == id);

            if (problemaSaude == null)
                return View("invalidProblemaSaude");

            return View(problemaSaude);
        }

        // GET: ProblemaSaudes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ProblemaSaudes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProblemaSaude problemaSaude)
        {
            if (ModelState.IsValid)
            {
                _context.Add(problemaSaude);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // 🔥 dados existem → permitir recuperação
            return View("invalidProblemaSaude", problemaSaude);
        }

        // GET: ProblemaSaudes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return View("invalidProblemaSaude");

            var problemaSaude = await _context.ProblemaSaude.FindAsync(id);

            if (problemaSaude == null)
                return View("invalidProblemaSaude");

            return View(problemaSaude);
        }

        // POST: ProblemaSaudes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProblemaSaude problemaSaude)
        {
            if (id != problemaSaude.ProblemaSaudeId)
                return View("invalidProblemaSaude", problemaSaude);

            if (!ModelState.IsValid)
                return View("invalidProblemaSaude", problemaSaude);

            try
            {
                _context.Update(problemaSaude);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProblemaSaudeExists(problemaSaude.ProblemaSaudeId))
                    return View("invalidProblemaSaude", problemaSaude);

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: ProblemaSaudes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var problemaSaude = await _context.ProblemaSaude
                .Include(p => p.ExercicioAfetado)
                .FirstOrDefaultAsync(m => m.ProblemaSaudeId == id);

            if (problemaSaude == null)
            {
                TempData["ErrorMessage"] = "Este problema de saúde já não existe.";
                return RedirectToAction(nameof(Index));
            }

            int numExercicios = problemaSaude.ExercicioAfetado?.Count ?? 0;

            ViewBag.NumExercicios = numExercicios;
            ViewBag.PodeEliminar = numExercicios == 0;

            return View(problemaSaude);
        }

        // POST: ProblemaSaudes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int ProblemaSaudeId)
        {
            var problemaSaude = await _context.ProblemaSaude.FindAsync(ProblemaSaudeId);

            if (problemaSaude == null) return RedirectToAction(nameof(Index));

            try
            {
                _context.ProblemaSaude.Remove(problemaSaude);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Problema de saúde eliminado com sucesso.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "Não é possível eliminar este problema de saúde porque está associado a exercícios (contraindicação).";
                return RedirectToAction(nameof(Delete), new { id = ProblemaSaudeId });
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Ocorreu um erro inesperado ao tentar eliminar o registo.";
                return RedirectToAction(nameof(Delete), new { id = ProblemaSaudeId });
            }
        }

        private bool ProblemaSaudeExists(int id)
        {
            return _context.ProblemaSaude.Any(e => e.ProblemaSaudeId == id);
        }
    }
}

