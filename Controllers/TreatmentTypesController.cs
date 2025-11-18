using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.Utils.Group1;
using HealthWellbeing.Utils.Group1.Interfaces;
using HealthWellbeing.Utils.Group1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Controllers
{
    public class TreatmentTypesController : Controller
    {
        private readonly HealthWellbeingDbContext _context;
        private readonly IRecordFilterService<TreatmentType> _filterService;

        public TreatmentTypesController(HealthWellbeingDbContext context, IRecordFilterService<TreatmentType> filterService) 
        {
            _context = context;
            _filterService = filterService;
        }

        // GET: TreatmentTypes
        public async Task<IActionResult> Index(string searchBy, string searchString, string sortOrder, int page = 1)
        {
            // Controla quantos itens por pagina
            var MAX_ITEMS_PER_PAGE = Constants.MAX_ITEMS_PER_PAGE<TreatmentType>();

            // Define as propriadades visiveis do modelo
            IReadOnlyList<string> baseProperties = ["Name", "Description", "EstimatedDuration", "Priority"];

            // Query Base para otimizar as consultas
            IQueryable<TreatmentType> treatment_types = _context.TreatmentType.AsNoTracking();

            // Aplica filtragem com os parametros atuais
            treatment_types = _filterService.ApplyFilter(treatment_types, searchBy, searchString);
            treatment_types = _filterService.ApplySorting(treatment_types, sortOrder);

            // Popula os dados necessarios a view
            ViewData["Title"] = "Tipos de Tratamento";
            ViewBag.ModelType = typeof(TreatmentType);
            ViewBag.Properties = baseProperties.ToList();
            ViewBag.SearchProperties = _filterService.SearchableProperties;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.CurrentSearchString = searchString;
            ViewBag.CurrentPage = page;

            // Aplica paginação e devolve a view com o modelo paginado
            var paginatedList = await PaginatedList<TreatmentType>.CreateAsync(treatment_types, page, MAX_ITEMS_PER_PAGE);
            return View(paginatedList);
        }

        // GET: TreatmentTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treatmentType = await _context.TreatmentType
                .FirstOrDefaultAsync(m => m.Id == id);
            if (treatmentType == null)
            {
                return NotFound();
            }

            return View(treatmentType);
        }

        // GET: TreatmentTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TreatmentTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,EstimatedDuration,Priority")] TreatmentType treatmentType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(treatmentType);
                await _context.SaveChangesAsync();
                TempData["Alert"] = AlertItem.CreateAlert("success", "bi bi-check-circle", "O tipo de tratamento foi criado com sucesso", true);
                return RedirectToAction(nameof(Index));
            }
            return View(treatmentType);
        }

        // GET: TreatmentTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treatmentType = await _context.TreatmentType.FindAsync(id);
            if (treatmentType == null)
            {
                return NotFound();
            }
            return View(treatmentType);
        }

        // POST: TreatmentTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,EstimatedDuration,Priority")] TreatmentType treatmentType)
        {
            if (id != treatmentType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(treatmentType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TreatmentTypeExists(treatmentType.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["Alert"] = AlertItem.CreateAlert("success", "bi bi-check-circle", "O tipo de tratamento foi editado com sucesso", true);
                return RedirectToAction(nameof(Index));
            }
            return View(treatmentType);
        }

        // GET: TreatmentTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treatmentType = await _context.TreatmentType
                .FirstOrDefaultAsync(m => m.Id == id);
            if (treatmentType == null)
            {
                return NotFound();
            }

            return View(treatmentType);
        }

        // POST: TreatmentTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var treatmentType = await _context.TreatmentType.FindAsync(id);
            if (treatmentType != null)
            {
                _context.TreatmentType.Remove(treatmentType);
            }

            await _context.SaveChangesAsync();
            TempData["Alert"] = AlertItem.CreateAlert("success", "bi bi-check-circle", "O tipo de tratamento foi eliminado com sucesso", true);
            return RedirectToAction(nameof(Index));
        }

        private bool TreatmentTypeExists(int id)
        {
            return _context.TreatmentType.Any(e => e.Id == id);
        }
    }
}
