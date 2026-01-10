using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;

namespace HealthWellbeing.Controllers
{
    public class TrainingTypeController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public TrainingTypeController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: TrainingType
        public async Task<IActionResult> Index(
            int page = 1,
            string searchActive = "",
            int? searchDuration = null)
        {
            var trainingTypesQuery = _context.TrainingType.AsQueryable();

            // Filtro por estado ativo
            if (!string.IsNullOrEmpty(searchActive))
            {
                if (searchActive == "Yes")
                    trainingTypesQuery = trainingTypesQuery.Where(t => t.IsActive);
                else if (searchActive == "No")
                    trainingTypesQuery = trainingTypesQuery.Where(t => !t.IsActive);
            }

            // Filtro por duração
            if (searchDuration.HasValue)
            {
                trainingTypesQuery = trainingTypesQuery
                    .Where(t => t.DurationMinutes == searchDuration.Value);
            }

            ViewBag.SearchActive = searchActive;
            ViewBag.SearchDuration = searchDuration;

            int totalItems = await trainingTypesQuery.CountAsync();

            var pagination = new PaginationInfo<TrainingType>(page, totalItems, 8);

            pagination.Items = await trainingTypesQuery
                .OrderBy(t => t.Name)
                .Skip(pagination.ItemsToSkip)
                .Take(pagination.ItemsPerPage)
                .ToListAsync();

            return View(pagination);
        }

        // GET: TrainingType/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var trainingType = await _context.TrainingType
                .FirstOrDefaultAsync(t => t.TrainingTypeId == id);

            if (trainingType == null)
                return NotFound();

            return View(trainingType);
        }

        // GET: TrainingType/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TrainingType/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Name,Description,DurationMinutes,IsActive")] TrainingType trainingType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trainingType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(trainingType);
        }

        // GET: TrainingType/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var trainingType = await _context.TrainingType.FindAsync(id);

            if (trainingType == null)
                return NotFound();

            return View(trainingType);
        }

        // POST: TrainingType/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("TrainingTypeId,Name,Description,DurationMinutes,IsActive")] TrainingType trainingType)
        {
            if (id != trainingType.TrainingTypeId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trainingType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainingTypeExists(trainingType.TrainingTypeId))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(trainingType);
        }

        // GET: TrainingType/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var trainingType = await _context.TrainingType
                .FirstOrDefaultAsync(t => t.TrainingTypeId == id);

            if (trainingType == null)
                return NotFound();

            return View(trainingType);
        }

        // POST: TrainingType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trainingType = await _context.TrainingType.FindAsync(id);

            if (trainingType != null)
            {
                _context.TrainingType.Remove(trainingType);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TrainingTypeExists(int id)
        {
            return _context.TrainingType.Any(e => e.TrainingTypeId == id);
        }
    }
}
