using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Authorization;

namespace HealthWellbeing.Controllers
{
    [Authorize(Roles = "Administrator,Trainer")] // Apenas Staff mexe nos planos de treino
    public class TrainingExerciseController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public TrainingExerciseController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: TrainingExercise/Index/5 (Onde 5 é o ID do Treino)
        public async Task<IActionResult> Index(int? trainingId)
        {
            if (trainingId == null) return NotFound();

            var training = await _context.Training.FindAsync(trainingId);
            if (training == null) return NotFound();

            var exercises = _context.TrainingExercise
                .Include(t => t.Exercise)
                .Include(t => t.Training)
                .Where(t => t.TrainingId == trainingId);

            ViewBag.TrainingName = training.Name;
            ViewBag.TrainingId = trainingId;

            return View(await exercises.ToListAsync());
        }

        // GET: TrainingExercise/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var trainingExercise = await _context.TrainingExercise
                .Include(t => t.Exercise)
                .Include(t => t.Training)
                .FirstOrDefaultAsync(m => m.TrainingExerciseId == id);

            if (trainingExercise == null) return NotFound();

            return View(trainingExercise);
        }

        // GET: TrainingExercise/Create?trainingId=5
        public IActionResult Create(int? trainingId)
        {
            if (trainingId == null) return NotFound();

            ViewData["ExerciseId"] = new SelectList(_context.Exercise, "ExerciseId", "Name");
            ViewBag.TrainingId = trainingId; // Passa o ID para a View para não o perdermos

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TrainingExerciseId,TrainingId,ExerciseId,Sets,Reps,RestTime")] TrainingExercise trainingExercise)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trainingExercise);
                await _context.SaveChangesAsync();
                // Redireciona de volta para a lista de exercícios DESTE treino
                return RedirectToAction(nameof(Index), new { trainingId = trainingExercise.TrainingId });
            }

            ViewData["ExerciseId"] = new SelectList(_context.Exercise, "ExerciseId", "Name", trainingExercise.ExerciseId);
            ViewBag.TrainingId = trainingExercise.TrainingId;
            return View(trainingExercise);
        }

        // GET: TrainingExercise/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var trainingExercise = await _context.TrainingExercise.FindAsync(id);
            if (trainingExercise == null) return NotFound();

            ViewData["ExerciseId"] = new SelectList(_context.Exercise, "ExerciseId", "Name", trainingExercise.ExerciseId);
            return View(trainingExercise);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TrainingExerciseId,TrainingId,ExerciseId,Sets,Reps,RestTime")] TrainingExercise trainingExercise)
        {
            if (id != trainingExercise.TrainingExerciseId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trainingExercise);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainingExerciseExists(trainingExercise.TrainingExerciseId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index), new { trainingId = trainingExercise.TrainingId });
            }
            ViewData["ExerciseId"] = new SelectList(_context.Exercise, "ExerciseId", "Name", trainingExercise.ExerciseId);
            return View(trainingExercise);
        }

        // GET: TrainingExercise/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var trainingExercise = await _context.TrainingExercise
                .Include(t => t.Exercise)
                .Include(t => t.Training)
                .FirstOrDefaultAsync(m => m.TrainingExerciseId == id);

            if (trainingExercise == null) return NotFound();

            return View(trainingExercise);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trainingExercise = await _context.TrainingExercise.FindAsync(id);
            int trainingId = 0;

            if (trainingExercise != null)
            {
                trainingId = trainingExercise.TrainingId; // Guardar ID para redirecionar
                _context.TrainingExercise.Remove(trainingExercise);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), new { trainingId = trainingId });
        }

        private bool TrainingExerciseExists(int id)
        {
            return _context.TrainingExercise.Any(e => e.TrainingExerciseId == id);
        }
    }
}