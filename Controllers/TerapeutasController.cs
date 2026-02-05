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

        [AllowAnonymous]
        public async Task<IActionResult> Index(
            string nome,
            string especialidade,
            bool? ativo,
            int page = 1)
        {
            var query = _context.Terapeutas.AsQueryable();

            if (!string.IsNullOrWhiteSpace(nome))
                query = query.Where(t => t.Nome.Contains(nome));

            if (!string.IsNullOrWhiteSpace(especialidade))
                query = query.Where(t => t.Especialidade == especialidade);

            if (ativo.HasValue)
                query = query.Where(t => t.Ativo == ativo.Value);

            ViewBag.Especialidades = await _context.Terapeutas
                .Select(t => t.Especialidade)
                .Distinct()
                .OrderBy(e => e)
                .ToListAsync();

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            var terapeutas = await query
                .OrderBy(t => t.Nome)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewBag.Nome = nome;
            ViewBag.EspecialidadeSelecionada = especialidade;
            ViewBag.Ativo = ativo;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(terapeutas);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var terapeuta = await _context.Terapeutas
                .FirstOrDefaultAsync(t => t.TerapeutaId == id);

            if (terapeuta == null)
                return NotFound();

            return View(terapeuta);
        }

        [Authorize(Roles = "Admin,Funcionario")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<IActionResult> Create(Terapeuta terapeuta)
        {
            if (!ModelState.IsValid)
                return View(terapeuta);

            _context.Add(terapeuta);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var terapeuta = await _context.Terapeutas.FindAsync(id);

            if (terapeuta == null)
                return NotFound();

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
                return View(terapeuta);

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

        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var terapeuta = await _context.Terapeutas
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