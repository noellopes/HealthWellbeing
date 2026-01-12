using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels; // Necessário para a Paginação
using Microsoft.AspNetCore.Authorization;

namespace HealthWellbeing.Controllers
{
    [Authorize(Roles = "Administrator,Trainer")] // Apenas Staff acede à gestão de exercícios
    public class ExercisesController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ExercisesController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1, string searchName = "", string searchMuscle = "")
        {
            var exercisesQuery = _context.Exercise.AsQueryable();

            // Filtros de Pesquisa
            if (!string.IsNullOrEmpty(searchName))
                exercisesQuery = exercisesQuery.Where(e => e.Name.Contains(searchName));

            if (!string.IsNullOrEmpty(searchMuscle))
                exercisesQuery = exercisesQuery.Where(e => e.MuscleGroup.Contains(searchMuscle));

            // Paginação
            int totalItems = await exercisesQuery.CountAsync();
            var pagination = new PaginationInfo<Exercise>(page, totalItems, 10);

            pagination.Items = await exercisesQuery
                .OrderBy(e => e.Name)
                .Skip(pagination.ItemsToSkip)
                .Take(pagination.ItemsPerPage)
                .ToListAsync();

            ViewBag.SearchName = searchName;
            ViewBag.SearchMuscle = searchMuscle;

            return View(pagination);
        }

        // GET: Exercises/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var exercise = await _context.Exercise
                .FirstOrDefaultAsync(m => m.ExerciseId == id);

            if (exercise == null) return NotFound();

            return View(exercise);
        }

        // GET: Exercises/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExerciseId,Name,MuscleGroup,Equipment,Description")] Exercise exercise)
        {
            if (ModelState.IsValid)
            {
                _context.Add(exercise);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Exercise added to the library.";
                return RedirectToAction(nameof(Index));
            }
            return View(exercise);
        }

        // GET: Exercises/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var exercise = await _context.Exercise.FindAsync(id);
            if (exercise == null) return NotFound();

            return View(exercise);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ExerciseId,Name,MuscleGroup,Equipment,Description")] Exercise exercise)
        {
            if (id != exercise.ExerciseId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(exercise);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Exercise updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExerciseExists(exercise.ExerciseId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(exercise);
        }

        // GET: Exercises/Delete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var exercise = await _context.Exercise
                .FirstOrDefaultAsync(m => m.ExerciseId == id);

            if (exercise == null) return NotFound();

            return View(exercise);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exercise = await _context.Exercise.FindAsync(id);
            if (exercise != null)
            {
                _context.Exercise.Remove(exercise);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Exercise removed from the library.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ExerciseExists(int id)
        {
            return _context.Exercise.Any(e => e.ExerciseId == id);
        }

        // Helper para consistência com os outros controllers
        private bool IsStaff() => User.IsInRole("Administrator") || User.IsInRole("Trainer");
    }
}