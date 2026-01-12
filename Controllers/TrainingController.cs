using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers
{
    public class TrainingController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public TrainingController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Training
        public async Task<IActionResult> Index(String searchName = "")
        {
            var training = _context.Training
                .Include(t => t.Trainer)
                .Include(t => t.TrainingType)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchName))
            {
                    training = training
                    .Where(t => t.Name.Contains(searchName));
            }

            ViewBag.SearchName = searchName;

            // Alterado para devolver uma Lista simples (sem Paginação) para corrigir o erro
            var healthWellbeingDbContext = _context.Training.Include(t => t.Trainer).Include(t => t.TrainingType);
            return View(await healthWellbeingDbContext.ToListAsync());
        }

        


        // GET: Training/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var training = await _context.Training
                .Include(t => t.Trainer)
                .Include(t => t.TrainingType)
                .FirstOrDefaultAsync(m => m.TrainingId == id);

            if (training == null) return NotFound();

            return View(training);
        }

        // GET: Training/Create
        public IActionResult Create()
        {
            ViewData["TrainerId"] = new SelectList(_context.Trainer, "TrainerId", "Name");
            ViewData["TrainingTypeId"] = new SelectList(_context.TrainingType, "TrainingTypeId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
    [Bind("TrainerId,TrainingTypeId,Name,Description,Duration,DayOfWeek,StartTime")]
    Training training)
        {
            if (ModelState.IsValid)
            {
                _context.Add(training);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["TrainerId"] = new SelectList(_context.Trainer, "TrainerId", "Name");
            ViewData["TrainingTypeId"] = new SelectList(_context.TrainingType, "TrainingTypeId", "Name");

            return View(training);
        }


        // GET: Training/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var training = await _context.Training.FindAsync(id);
            if (training == null) return NotFound();
            ViewData["TrainerId"] = new SelectList(_context.Trainer, "TrainerId", "Name", training.TrainerId);
            ViewData["TrainingTypeId"] = new SelectList(_context.TrainingType, "TrainingTypeId", "Name", training.TrainingTypeId);
            return View(training);
        }

        // POST: Training/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TrainingId,TrainerId,TrainingTypeId,Name,Description,DayOfWeek,StartTime,Duration")] Training training)
        {
            if (id != training.TrainingId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(training);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainingExists(training.TrainingId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["TrainerId"] = new SelectList(_context.Trainer, "TrainerId", "Name", training.TrainerId);
            ViewData["TrainingTypeId"] = new SelectList(_context.TrainingType, "TrainingTypeId", "Name", training.TrainingTypeId);
            return View(training);
        }

        // GET: Training/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var training = await _context.Training
                .Include(t => t.Trainer)
                .Include(t => t.TrainingType)
                .FirstOrDefaultAsync(m => m.TrainingId == id);

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
            return RedirectToAction(nameof(Index));
        }

        private bool TrainingExists(int id)
        {
            return _context.Training.Any(e => e.TrainingId == id);
        }
    }
}