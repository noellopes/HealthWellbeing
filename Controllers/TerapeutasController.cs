using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HealthWellbeing.Controllers
{
    public class TerapeutasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 10;

        public TerapeutasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // INDEX
        // =========================
        [AllowAnonymous]
        public async Task<IActionResult> Index(
            string nome,
            int? especialidadeId,
            bool? ativo,
            int page = 1)
        {
            var query = _context.Terapeutas
                .Include(t => t.Especialidade)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(nome))
                query = query.Where(t => t.Nome.Contains(nome));

            if (especialidadeId.HasValue)
                query = query.Where(t => t.EspecialidadeId == especialidadeId.Value);

            if (ativo.HasValue)
                query = query.Where(t => t.Ativo == ativo.Value);

            ViewBag.Especialidades = await _context.Especialidades
                .OrderBy(e => e.Nome)
                .ToListAsync();

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            var terapeutas = await query
                .OrderBy(t => t.Nome)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewBag.Nome = nome;
            ViewBag.EspecialidadeSelecionada = especialidadeId;
            ViewBag.Ativo = ativo;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(terapeutas);
        }

        // =========================
        // DETAILS
        // =========================
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var terapeuta = await _context.Terapeutas
                .Include(t => t.Especialidade)
                .FirstOrDefaultAsync(t => t.TerapeutaId == id);

            if (terapeuta == null)
                return NotFound();

            return View(terapeuta);
        }

        // =========================
        // CREATE
        // =========================
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Especialidades = await _context.Especialidades
                .OrderBy(e => e.Nome)
                .ToListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<IActionResult> Create(Terapeuta terapeuta)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Especialidades = await _context.Especialidades
                    .OrderBy(e => e.Nome)
                    .ToListAsync();

                return View(terapeuta);
            }

            _context.Add(terapeuta);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // EDIT
        // =========================
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var terapeuta = await _context.Terapeutas.FindAsync(id);

            if (terapeuta == null)
                return NotFound();

            ViewBag.Especialidades = await _context.Especialidades
                .OrderBy(e => e.Nome)
                .ToListAsync();

            return View(terapeuta);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<IActionResult> Edit(int id, Terapeuta terapeuta)
        {
            if (id != terapeuta.TerapeutaId)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Especialidades = await _context.Especialidades
                    .OrderBy(e => e.Nome)
                    .ToListAsync();

                return View(terapeuta);
            }

            try
            {
                _context.Update(terapeuta);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Terapeutas.Any(t => t.TerapeutaId == id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DELETE
        // =========================
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var terapeuta = await _context.Terapeutas
                .Include(t => t.Especialidade)
                .FirstOrDefaultAsync(t => t.TerapeutaId == id);

            if (terapeuta == null)
                return NotFound();

            return View(terapeuta);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var terapeuta = await _context.Terapeutas.FindAsync(id);

            if (terapeuta != null)
            {
                _context.Terapeutas.Remove(terapeuta);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}