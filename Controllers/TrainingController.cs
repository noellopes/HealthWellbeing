using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;

namespace HealthWellbeing.Controllers
{
    [AllowAnonymous]
    public class TrainingController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public TrainingController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Training
        public async Task<IActionResult> Index(
            int page = 1,
            string searchName = "",
            int? searchTrainerId = null,
            int? searchTrainingTypesId = null,
            int? searchDuration = null)
        {
            var trainingsQuery = _context.Training
                .Include(t => t.Trainer)
                .Include(t => t.TrainingType)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchName))
                trainingsQuery = trainingsQuery.Where(t => t.Name.Contains(searchName));

            if (searchTrainerId.HasValue)
                trainingsQuery = trainingsQuery.Where(t => t.TrainerId == searchTrainerId.Value);

            if (searchTrainingTypesId.HasValue)
                trainingsQuery = trainingsQuery.Where(t => t.TrainingTypeId == searchTrainingTypesId.Value);

            if (searchDuration.HasValue)
                trainingsQuery = trainingsQuery.Where(t => t.Duration == searchDuration.Value);

            ViewBag.SearchName = searchName;
            ViewBag.SearchTrainerId = searchTrainerId;
            ViewBag.SearchTrainingTypeId = searchTrainingTypesId;
            ViewBag.SearchDuration = searchDuration;

            ViewData["TrainerId"] = new SelectList(
                _context.Trainer.OrderBy(t => t.Name),
                "TrainerId",
                "Name",
                searchTrainerId);

            ViewData["TrainingTypeId"] = new SelectList(
                _context.TrainingType.OrderBy(tt => tt.Name),
                "TrainingTypeId",
                "Name",
                searchTrainingTypesId);

            int totalTrainings = await trainingsQuery.CountAsync();

            var pagination = new PaginationInfo<Training>(page, totalTrainings);

            pagination.Items = await trainingsQuery
                .OrderBy(t => t.DayOfWeek)
                .ThenBy(t => t.StartTime)
                .Skip(pagination.ItemsToSkip)
                .Take(pagination.ItemsPerPage)
                .ToListAsync();

            return View(pagination);
        }

        // GET: Training/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var training = await _context.Training
                .Include(t => t.Trainer)
                .Include(t => t.TrainingType)
                .FirstOrDefaultAsync(t => t.TrainingId == id);

            if (training == null) return NotFound();

            return View(training);
        }

        // GET: Training/Create
        public IActionResult Create()
        {
            ViewData["TrainerId"] = new SelectList(
                _context.Trainer.OrderBy(t => t.Name),
                "TrainerId",
                "Name");

            ViewData["TrainingTypeId"] = new SelectList(
                _context.TrainingType.OrderBy(tt => tt.Name),
                "TrainingTypeId",
                "Name");

            return View();
        }

        // POST: Training/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("TrainerId,TrainingTypeId,Name,Description,Duration,DayOfWeek,StartTime")]
            Training training)
        {
            if (training.Duration <= 0)
            {
                ModelState.AddModelError(nameof(training.Duration),
                    "A duração do treino deve ser superior a 0 minutos.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(training);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Treino criado com sucesso.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["TrainerId"] = new SelectList(
                _context.Trainer.OrderBy(t => t.Name),
                "TrainerId",
                "Name",
                training.TrainerId);

            ViewData["TrainingTypeId"] = new SelectList(
                _context.TrainingType.OrderBy(tt => tt.Name),
                "TrainingTypeId",
                "Name",
                training.TrainingTypeId);

            return View(training);
        }

        // GET: Training/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var training = await _context.Training.FindAsync(id);
            if (training == null) return NotFound();

            ViewData["TrainerId"] = new SelectList(
                _context.Trainer.OrderBy(t => t.Name),
                "TrainerId",
                "Name",
                training.TrainerId);

            ViewData["TrainingTypeId"] = new SelectList(
                _context.TrainingType.OrderBy(tt => tt.Name),
                "TrainingTypeId",
                "Name",
                training.TrainingTypeId);

            return View(training);
        }

        // POST: Training/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("TrainingId,TrainerId,TrainingTypeId,Name,Description,Duration,DayOfWeek,StartTime")]
            Training training)
        {
            if (id != training.TrainingId) return NotFound();

            if (training.Duration <= 0)
            {
                ModelState.AddModelError(nameof(training.Duration),
                    "A duração do treino deve ser superior a 0 minutos.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(training);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Treino atualizado com sucesso.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Training.Any(t => t.TrainingId == training.TrainingId))
                        return NotFound();
                    throw;
                }
            }

            ViewData["TrainerId"] = new SelectList(
                _context.Trainer.OrderBy(t => t.Name),
                "TrainerId",
                "Name",
                training.TrainerId);

            ViewData["TrainingTypeId"] = new SelectList(
                _context.TrainingType.OrderBy(tt => tt.Name),
                "TrainingTypeId",
                "Name",
                training.TrainingTypeId);

            return View(training);
        }

        // GET: Training/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var training = await _context.Training
                .Include(t => t.Trainer)
                .Include(t => t.TrainingType)
                .FirstOrDefaultAsync(t => t.TrainingId == id);

            if (training == null) return NotFound();

            return View(training);
        }

        // POST: Training/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var training = await _context.Training.FindAsync(id);
            if (training != null)
            {
                _context.Training.Remove(training);
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "Treino eliminado com sucesso.";
            return RedirectToAction(nameof(Index));
        }
    }
}
