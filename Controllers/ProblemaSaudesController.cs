using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

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

            // Removido o .Include(ProfissionalExecutante)
            var query = _context.ProblemaSaude.AsQueryable();

            if (!string.IsNullOrWhiteSpace(categoria))
                query = query.Where(p => p.ProblemaCategoria.ToLower().Contains(categoria.ToLower()));

            if (!string.IsNullOrWhiteSpace(nome))
                query = query.Where(p => p.ProblemaNome.ToLower().Contains(nome.ToLower()));

            if (!string.IsNullOrWhiteSpace(zona))
                query = query.Where(p => p.ZonaAtingida.ToLower().Contains(zona.ToLower()));

            // Removido o bloco de filtro por Profissional

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
            // ViewBag.Profissional removido
            ViewBag.Gravidade = gravidade;

            return View(pagination);
        }

        // GET: ProblemaSaudes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Removido o .Include
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
            // Removida a carga da lista de profissionais para ViewData
            return View();
        }

        // POST: ProblemaSaudes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("ProblemaSaudeId,ProblemaCategoria,ProblemaNome,ZonaAtingida,Gravidade")] ProblemaSaude problemaSaude)
        // Removido o argumento int[] selectedProfissionais
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

            // Removido o .Include
            var problemaSaude = await _context.ProblemaSaude.FindAsync(id);

            if (problemaSaude == null)
            {
                return NotFound();
            }

            // Removida a lógica de carregar checkboxes
            return View(problemaSaude);
        }

        // POST: ProblemaSaudes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("ProblemaSaudeId,ProblemaCategoria,ProblemaNome,ZonaAtingida,Gravidade")] ProblemaSaude problemaSaude)
        // Removido o argumento int[] selectedProfissionais
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

            // Removido o .Include
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