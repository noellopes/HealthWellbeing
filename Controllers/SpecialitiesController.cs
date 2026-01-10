using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModel;

namespace HealthWellbeing.Controllers
{
    public class SpecialitiesController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public SpecialitiesController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Specialities
        public async Task<IActionResult> Index(string? q, int page = 1)
        {
            var query = _context.Specialities.AsQueryable();

            // Filtro de pesquisa
            if (!string.IsNullOrWhiteSpace(q))
            {
                query = query.Where(s =>
                    s.Nome.Contains(q) ||
                    s.Descricao.Contains(q));
            }

            var totalItems = await query.CountAsync();

            var pagination = new PaginationInfo<Specialities>(
                currentPage: page,
                totalItems: totalItems,
                itemsPerPage: 9
            );

            var items = await query
                .OrderBy(s => s.Nome)
                .Skip(pagination.ItemsToSkip)
                .Take(pagination.ItemsPerPage)
                .ToListAsync();

            pagination.Items = items;

            ViewBag.SearchQuery = q;

            // Título para o layout (se o layout usar ViewData["Title"])
            ViewData["Title"] = "Especialidades";

            return View(pagination);
        }

        // GET: Specialities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var specialities = await _context.Specialities
                .FirstOrDefaultAsync(m => m.IdEspecialidade == id);

            if (specialities == null) return NotFound();

            // Título da página com o nome da especialidade
            ViewData["Title"] = specialities.Nome;

            return View(specialities);
        }

        // GET: Specialities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Specialities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("IdEspecialidade,Nome,Descricao,OqueEDescricao")]
            Specialities specialities)
        {
            // Impedir criar duas especialidades com o mesmo nome (case-insensitive)
            if (await _context.Specialities
                    .AnyAsync(s => s.Nome.ToLower() == specialities.Nome.ToLower()))
            {
                ModelState.AddModelError("Nome",
                    "Já existe uma especialidade com este nome.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(specialities);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(specialities);
        }

        // GET: Specialities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var specialities = await _context.Specialities.FindAsync(id);
            if (specialities == null) return NotFound();

            return View(specialities);
        }

        // POST: Specialities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("IdEspecialidade,Nome,Descricao,OqueEDescricao")]
            Specialities specialities)
        {
            if (id != specialities.IdEspecialidade) return NotFound();

            // Impedir renomear para um nome que já exista noutra especialidade
            if (await _context.Specialities
                    .AnyAsync(s => s.Nome.ToLower() == specialities.Nome.ToLower()
                                   && s.IdEspecialidade != specialities.IdEspecialidade))
            {
                ModelState.AddModelError("Nome",
                    "Já existe outra especialidade com este nome.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(specialities);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpecialitiesExists(specialities.IdEspecialidade))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(specialities);
        }

        // GET: Specialities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var specialities = await _context.Specialities
                .FirstOrDefaultAsync(m => m.IdEspecialidade == id);

            if (specialities == null) return NotFound();

            return View(specialities);
        }

        // POST: Specialities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Carregar a especialidade com os médicos associados
            var specialities = await _context.Specialities
                .Include(s => s.Medicos)
                .FirstOrDefaultAsync(s => s.IdEspecialidade == id);

            if (specialities == null)
            {
                return NotFound();
            }

            // “Cascade no papel”: se tiver médicos, NÃO apaga e mostra mensagem
            if (specialities.Medicos != null && specialities.Medicos.Any())
            {
                ModelState.AddModelError(string.Empty,
                    "Não é possível apagar esta especialidade porque existem médicos associados a ela.");

                // Reapresenta a view Delete com a mensagem de erro
                return View("Delete", specialities);
            }

            // Se não tiver médicos, então pode apagar mesmo
            _context.Specialities.Remove(specialities);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SpecialitiesExists(int id)
        {
            return _context.Specialities.Any(e => e.IdEspecialidade == id);
        }
    }
}
