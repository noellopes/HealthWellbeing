using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.Utils.Group1;
using HealthWellbeing.Utils.Group1.Interfaces;
using HealthWellbeing.Utils.Group1.Models;

namespace HealthWellbeing.Controllers
{
    public class PathologiesController : Controller
    {
        private readonly HealthWellbeingDbContext _context;
        private readonly IRecordFilterService<Pathology> _filterService;

        public PathologiesController(HealthWellbeingDbContext context, IRecordFilterService<Pathology> filterService)
        {
            _context = context;
            _filterService = filterService;
        }

        // GET: Pathologies
        public async Task<IActionResult> Index(string searchBy, string searchString, string sortOrder, int page = 1)
        {
            // Controla quantos itens por pagina
            var MAX_ITEMS_PER_PAGE = Constants.MAX_ITEMS_PER_PAGE<Pathology>();

            // Define as propriadades visiveis do modelo
            IReadOnlyList<string> baseProperties = ["Name", "Severity", "Description"];

            // Query Base para otimizar as consultas
            IQueryable<Pathology> pathologies = _context.Pathology.AsNoTracking();

            // Aplica filtragem com os parametros atuais
            pathologies = _filterService.ApplyFilter(pathologies, searchBy, searchString);
            pathologies = _filterService.ApplySorting(pathologies, sortOrder);

            // Popula os dados necessarios a view
            ViewData["Title"] = "Patologias";
            ViewBag.ModelType = typeof(Pathology);
            ViewBag.Properties = baseProperties.ToList();
            ViewBag.SearchProperties = _filterService.SearchableProperties;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.CurrentSearchString = searchString;
            ViewBag.CurrentPage = page;

            // Aplica paginação e devolve a view com o modelo paginado
            var paginatedList = await PaginatedList<Pathology>.CreateAsync(pathologies, page, MAX_ITEMS_PER_PAGE);
            return View(paginatedList);
        }

        // GET: Pathologies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pathology = await _context.Pathology
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pathology == null)
            {
                return NotFound();
            }

            return View(pathology);
        }

        // GET: Pathologies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Pathologies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Severity")] Pathology pathology)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pathology);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pathology);
        }

        // GET: Pathologies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pathology = await _context.Pathology.FindAsync(id);
            if (pathology == null)
            {
                return NotFound();
            }
            return View(pathology);
        }

        // POST: Pathologies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Severity")] Pathology pathology)
        {
            if (id != pathology.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pathology);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PathologyExists(pathology.Id))
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
            return View(pathology);
        }

        // GET: Pathologies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pathology = await _context.Pathology
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pathology == null)
            {
                return NotFound();
            }

            return View(pathology);
        }

        // POST: Pathologies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pathology = await _context.Pathology.FindAsync(id);
            if (pathology != null)
            {
                _context.Pathology.Remove(pathology);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PathologyExists(int id)
        {
            return _context.Pathology.Any(e => e.Id == id);
        }
    }
}
