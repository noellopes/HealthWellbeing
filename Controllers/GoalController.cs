using System.Linq;
using System.Threading.Tasks;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Controllers
{
    public class GoalController : Controller
    {
        private readonly HealthWellbeingDbContext _context;
        public GoalController(HealthWellbeingDbContext context) => _context = context;

        // GET: /Goal
        public async Task<IActionResult> Index(int? patientId)
        {
            var q = _context.Goal.AsNoTracking().AsQueryable();
            if (patientId.HasValue) q = q.Where(g => g.PatientId == patientId.Value);
            ViewBag.PatientId = patientId;
            return View(await q.OrderByDescending(g => g.GoalId).ToListAsync());
        }

        // GET: /Goal/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var g = await _context.Goal
                .Include(x => x.Plan)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.GoalId == id);
            if (g == null) return NotFound();
            return View(g);
        }

        // GET: /Goal/Create
        public IActionResult Create(int? patientId)
        {
            // Troca isto por UtenteSaude quando tiveres a entidade real:
            ViewData["PatientId"] = new SelectList(new[]
            {
                new { Id = patientId ?? 0, Name = patientId.HasValue ? $"Patient #{patientId}" : "Select a patient" }
            }, "Id", "Name", patientId);
            return View();
        }

        // POST: /Goal/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientId,GoalType,DailyCalories,DailyProtein,DailyFats,DailyCarbs,DailyVitamins,DailyMinerals,DailyFibers")] Goal goal)
        {
            if (!ModelState.IsValid)
            {
                ViewData["PatientId"] = new SelectList(new[] { new { Id = goal.PatientId, Name = $"Patient #{goal.PatientId}" } }, "Id", "Name", goal.PatientId);
                return View(goal);
            }

            _context.Add(goal);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Goal created successfully.";
            return RedirectToAction(nameof(Index), new { patientId = goal.PatientId });
        }

        // GET: /Goal/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var g = await _context.Goal.FindAsync(id);
            if (g == null) return NotFound();

            ViewData["PatientId"] = new SelectList(new[] { new { Id = g.PatientId, Name = $"Patient #{g.PatientId}" } }, "Id", "Name", g.PatientId);
            return View(g);
        }

        // POST: /Goal/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GoalId,PatientId,GoalType,DailyCalories,DailyProtein,DailyFats,DailyCarbs,DailyVitamins,DailyMinerals,DailyFibers")] Goal goal)
        {
            if (id != goal.GoalId) return NotFound();
            if (!ModelState.IsValid)
            {
                ViewData["PatientId"] = new SelectList(new[] { new { Id = goal.PatientId, Name = $"Patient #{goal.PatientId}" } }, "Id", "Name", goal.PatientId);
                return View(goal);
            }

            try
            {
                _context.Update(goal);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Goal updated successfully.";
                return RedirectToAction(nameof(Index), new { patientId = goal.PatientId });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Goal.AnyAsync(e => e.GoalId == id)) return NotFound();
                TempData["Error"] = "The record was modified by another user. Please reload and try again.";
                return View(goal);
            }
        }

        // GET: /Goal/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var g = await _context.Goal.AsNoTracking().FirstOrDefaultAsync(x => x.GoalId == id);
            if (g == null) return NotFound();
            return View(g);
        }

        // POST: /Goal/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var g = await _context.Goal.FindAsync(id);
            if (g == null) return NotFound();
            int patientId = g.PatientId;

            _context.Goal.Remove(g);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Goal deleted successfully.";
            return RedirectToAction(nameof(Index), new { patientId });
        }
    }
}
