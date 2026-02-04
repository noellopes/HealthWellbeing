using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        // GET: Terapeutas
        public async Task<IActionResult> Index(
            string nome,
            string especialidade,
            bool? ativo,
            int page = 1)
        {
            var query = _context.Terapeutas.AsQueryable();

            // Lista de especialidades para o dropdown
            ViewBag.Especialidades = await _context.Terapeutas
                .Select(t => t.Especialidade)
                .Distinct()
                .OrderBy(e => e)
                .ToListAsync();

            // Filtros
            if (!string.IsNullOrWhiteSpace(nome))
            {
                query = query.Where(t => t.Nome.Contains(nome));
            }

            if (!string.IsNullOrWhiteSpace(especialidade))
            {
                query = query.Where(t => t.Especialidade == especialidade);
            }

            if (ativo.HasValue)
            {
                query = query.Where(t => t.Ativo == ativo.Value);
            }

            const int pageSize = 10;
            int total = await query.CountAsync();

            var terapeutas = await query
                .OrderBy(t => t.Nome)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);

            // Manter filtros
            ViewBag.Nome = nome;
            ViewBag.EspecialidadeSelecionada = especialidade;
            ViewBag.Ativo = ativo;

            return View(terapeutas);
        }

        // GET: Terapeutas/Details/5
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

        // GET: Terapeutas/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Terapeutas/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Terapeuta terapeuta)
        {
            if (ModelState.IsValid)
            {
                _context.Add(terapeuta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(terapeuta);
        }

        // GET: Terapeutas/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var terapeuta = await _context.Terapeutas.FindAsync(id);

            if (terapeuta == null)
                return NotFound();

            return View(terapeuta);
        }

        // POST: Terapeutas/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Terapeuta terapeuta)
        {
            if (id != terapeuta.TerapeutaId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(terapeuta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TerapeutaExists(terapeuta.TerapeutaId))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(terapeuta);
        }

        // GET: Terapeutas/Delete/5
        [Authorize(Roles = "Admin")]
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

        // POST: Terapeutas/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
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

        private bool TerapeutaExists(int id)
        {
            return _context.Terapeutas.Any(t => t.TerapeutaId == id);
        }
    }
}