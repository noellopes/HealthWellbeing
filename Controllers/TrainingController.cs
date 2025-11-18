using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels; // Adicionar este using

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
        public async Task<IActionResult> Index(int page = 1, string searchName = "", int? searchTrainerId = null, int? searchTrainingTypeId = null, int? searchDuration = null)
        {
            var trainingsQuery = _context.Training
                .Include(t => t.Trainer)
                .Include(t => t.TrainingType)
                .AsQueryable();

            // Filtragem
            if (!string.IsNullOrEmpty(searchName))
            {
                trainingsQuery = trainingsQuery.Where(t => t.Name.Contains(searchName));
            }

            if (searchTrainerId.HasValue)
            {
                trainingsQuery = trainingsQuery.Where(t => t.TrainerId == searchTrainerId.Value);
            }

            if (searchTrainingTypeId.HasValue)
            {
                trainingsQuery = trainingsQuery.Where(t => t.TrainingTypeId == searchTrainingTypeId.Value);
            }

            // Nova lógica para Duração
            if (searchDuration.HasValue)
            {
                trainingsQuery = trainingsQuery.Where(t => t.Duration == searchDuration.Value);
            }

            // Guardar valores de pesquisa para o View
            ViewBag.SearchName = searchName;
            ViewBag.SearchTrainerId = searchTrainerId;
            ViewBag.SearchTrainingTypeId = searchTrainingTypeId;
            ViewBag.SearchDuration = searchDuration; // Guardar duração na ViewBag

            // Para os dropdowns de filtro
            ViewData["TrainerId"] = new SelectList(_context.Trainer.OrderBy(t => t.Name), "TrainerId", "Name", searchTrainerId);
            ViewData["TrainingTypeId"] = new SelectList(_context.TrainingType.OrderBy(tt => tt.Name), "TrainingTypeId", "Name", searchTrainingTypeId);

            int totalTrainings = await trainingsQuery.CountAsync();

            var trainingsInfo = new PaginationInfo<Training>(page, totalTrainings, 8);

            trainingsInfo.Items = await trainingsQuery
                .OrderBy(t => t.DayOfWeek)
                .ThenBy(t => t.StartTime)
                .Skip(trainingsInfo.ItemsToSkip)
                .Take(trainingsInfo.ItemsPerPage)
                .ToListAsync();

            return View(trainingsInfo);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var training = await _context.Training
                .Include(t => t.Trainer)
                .Include(t => t.TrainingType)
                .FirstOrDefaultAsync(m => m.TrainingId == id);
            if (training == null)
            {
                return NotFound();
            }

            return View(training);
        }

        public IActionResult Create()
        {
            ViewData["TrainerId"] = new SelectList(_context.Trainer, "TrainerId", "Email");
            ViewData["TrainingTypeId"] = new SelectList(_context.TrainingType, "TrainingTypeId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TrainingId,TrainerId,TrainingTypeId,Name,Description,Duration,DayOfWeek,StartTime,MaxParticipants")] Training training)
        {
            if (ModelState.IsValid)
            {
                _context.Add(training);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TrainerId"] = new SelectList(_context.Trainer, "TrainerId", "Email", training.TrainerId);
            ViewData["TrainingTypeId"] = new SelectList(_context.TrainingType, "TrainingTypeId", "Name", training.TrainingTypeId);
            return View(training);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var training = await _context.Training.FindAsync(id);
            if (training == null)
            {
                return NotFound();
            }
            ViewData["TrainerId"] = new SelectList(_context.Trainer, "TrainerId", "Email", training.TrainerId);
            ViewData["TrainingTypeId"] = new SelectList(_context.TrainingType, "TrainingTypeId", "Name", training.TrainingTypeId);
            return View(training);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TrainingId,TrainerId,TrainingTypeId,Name,Description,Duration,DayOfWeek,StartTime,MaxParticipants")] Training training)
        {
            if (id != training.TrainingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(training);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainingExists(training.TrainingId))
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
            ViewData["TrainerId"] = new SelectList(_context.Trainer, "TrainerId", "Email", training.TrainerId);
            ViewData["TrainingTypeId"] = new SelectList(_context.TrainingType, "TrainingTypeId", "Name", training.TrainingTypeId);
            return View(training);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var training = await _context.Training
                .Include(t => t.Trainer)
                .Include(t => t.TrainingType)
                .FirstOrDefaultAsync(m => m.TrainingId == id);
            if (training == null)
            {
                return NotFound();
            }

            return View(training);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var training = await _context.Training.FindAsync(id);
            if (training != null)
            {
                _context.Training.Remove(training);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrainingExists(int id)
        {
            return _context.Training.Any(e => e.TrainingId == id);
        }
    }
}