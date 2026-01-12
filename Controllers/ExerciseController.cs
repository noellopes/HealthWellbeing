using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels; // <--- Não se esqueça disto para o PaginationInfo
using Microsoft.AspNetCore.Authorization;

namespace HealthWellbeing.Controllers
{
    [Authorize(Roles = "Administrator,Trainer")] // Apenas Staff pode gerir exercícios
    public class ExerciseController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ExerciseController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Exercise
        // CORRIGIDO: Agora envia PaginationInfo em vez de List
        public async Task<IActionResult> Index(int page = 1)
        {
            var exercisesQuery = _context.Exercise.AsQueryable();

            // 1. Contar Total
            int total = await exercisesQuery.CountAsync();

            // 2. Criar Objeto de Paginação
            var pagination = new PaginationInfo<Exercise>(page, total, 10);

            // 3. Obter itens da página atual
            pagination.Items = await exercisesQuery
                .OrderBy(e => e.Name)
                .Skip(pagination.ItemsToSkip)
                .Take(pagination.ItemsPerPage)
                .ToListAsync();

            // 4. Enviar para a View
            return View(pagination);
        }

        // GET: Exercise/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var exercise = await _context.Exercise
                .FirstOrDefaultAsync(m => m.ExerciseId == id);

            if (exercise == null) return NotFound();

            return View(exercise);
        }

        // GET: Exercise/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Exercise/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExerciseId,Name,MuscleGroup,Equipment,Description")] Exercise exercise)
        {
            if (ModelState.IsValid)
            {
                _context.Add(exercise);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(exercise);
        }

        // GET: Exercise/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var exercise = await _context.Exercise.FindAsync(id);
            if (exercise == null) return NotFound();
            return View(exercise);
        }

        // POST: Exercise/Edit/5
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

        // GET: Exercise/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var exercise = await _context.Exercise
                .FirstOrDefaultAsync(m => m.ExerciseId == id);
            if (exercise == null) return NotFound();

            return View(exercise);
        }

        // POST: Exercise/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exercise = await _context.Exercise.FindAsync(id);
            if (exercise != null) _context.Exercise.Remove(exercise);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExerciseExists(int id)
        {
            return _context.Exercise.Any(e => e.ExerciseId == id);
        }
    }
}