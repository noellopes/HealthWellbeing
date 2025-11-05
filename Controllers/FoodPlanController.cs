using System.Linq;
using System.Threading.Tasks;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Controllers
{
    public class FoodPlanController : Controller
    {
        private readonly HealthWellbeingDbContext _context;
        public FoodPlanController(HealthWellbeingDbContext context) => _context = context;

        // GET: /FoodPlan
        public async Task<IActionResult> Index(int? patientId, int? goalId)
        {
            var q = _context.FoodPlan
                .Include(p => p.Goal)
                .Include(p => p.Food)
                .Include(p => p.Nutritionist)
                .AsNoTracking()
                .AsQueryable();

            if (patientId.HasValue) q = q.Where(p => p.PatientId == patientId.Value);
            if (goalId.HasValue) q = q.Where(p => p.GoalId == goalId.Value);

            ViewBag.PatientId = patientId;
            ViewBag.GoalId = goalId;
            return View(await q.OrderByDescending(p => p.FoodPlanId).ToListAsync());
        }

        // GET: /FoodPlan/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var plan = await _context.FoodPlan
                .Include(p => p.Goal)
                .Include(p => p.Food)
                .Include(p => p.Nutritionist)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.FoodPlanId == id);
            if (plan == null) return NotFound();
            return View(plan);
        }

        // GET: /FoodPlan/Create
        public async Task<IActionResult> Create(int? patientId, int? goalId)
        {
            ViewData["GoalId"] = new SelectList(await _context.Goal
                .Where(g => !patientId.HasValue || g.PatientId == patientId)
                .OrderBy(g => g.GoalType).ToListAsync(), "GoalId", "GoalType", goalId);

            ViewData["PatientId"] = new SelectList(new[]
            { new { Id = patientId ?? 0, Name = patientId.HasValue ? $"Patient #{patientId}" : "Select a patient" } }, "Id", "Name", patientId);

            ViewData["FoodId"] = new SelectList(await _context.Food.OrderBy(f => f.Name).ToListAsync(), "FoodId", "Name");
            ViewData["NutritionistId"] = new SelectList(await _context.Nutritionist.OrderBy(n => n.Name).ToListAsync(), "NutritionistId", "Name");
            return View(new FoodPlan { Quantity = 1 });
        }

        // POST: /FoodPlan/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GoalId,PatientId,FoodId,Description,Quantity,NutritionistId")] FoodPlan plan)
        {
            if (!ModelState.IsValid)
            {
                ViewData["GoalId"] = new SelectList(await _context.Goal.OrderBy(g => g.GoalType).ToListAsync(), "GoalId", "GoalType", plan.GoalId);
                ViewData["PatientId"] = new SelectList(new[] { new { Id = plan.PatientId, Name = $"Patient #{plan.PatientId}" } }, "Id", "Name", plan.PatientId);
                ViewData["FoodId"] = new SelectList(await _context.Food.OrderBy(f => f.Name).ToListAsync(), "FoodId", "Name", plan.FoodId);
                ViewData["NutritionistId"] = new SelectList(await _context.Nutritionist.OrderBy(n => n.Name).ToListAsync(), "NutritionistId", "Name", plan.NutritionistId);
                return View(plan);
            }

            _context.Add(plan);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Plan created successfully.";
            return RedirectToAction(nameof(Index), new { patientId = plan.PatientId, goalId = plan.GoalId });
        }

        // GET: /FoodPlan/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var plan = await _context.FoodPlan.FindAsync(id);
            if (plan == null) return NotFound();

            ViewData["GoalId"] = new SelectList(await _context.Goal.Where(g => g.PatientId == plan.PatientId).OrderBy(g => g.GoalType).ToListAsync(), "GoalId", "GoalType", plan.GoalId);
            ViewData["PatientId"] = new SelectList(new[] { new { Id = plan.PatientId, Name = $"Patient #{plan.PatientId}" } }, "Id", "Name", plan.PatientId);
            ViewData["FoodId"] = new SelectList(await _context.Food.OrderBy(f => f.Name).ToListAsync(), "FoodId", "Name", plan.FoodId);
            ViewData["NutritionistId"] = new SelectList(await _context.Nutritionist.OrderBy(n => n.Name).ToListAsync(), "NutritionistId", "Name", plan.NutritionistId);
            return View(plan);
        }

        // POST: /FoodPlan/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FoodPlanId,GoalId,PatientId,FoodId,Description,Quantity,NutritionistId")] FoodPlan plan)
        {
            if (id != plan.FoodPlanId) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["GoalId"] = new SelectList(await _context.Goal.Where(g => g.PatientId == plan.PatientId).OrderBy(g => g.GoalType).ToListAsync(), "GoalId", "GoalType", plan.GoalId);
                ViewData["PatientId"] = new SelectList(new[] { new { Id = plan.PatientId, Name = $"Patient #{plan.PatientId}" } }, "Id", "Name", plan.PatientId);
                ViewData["FoodId"] = new SelectList(await _context.Food.OrderBy(f => f.Name).ToListAsync(), "FoodId", "Name", plan.FoodId);
                ViewData["NutritionistId"] = new SelectList(await _context.Nutritionist.OrderBy(n => n.Name).ToListAsync(), "NutritionistId", "Name", plan.NutritionistId);
                return View(plan);
            }

            try
            {
                _context.Update(plan);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Plan updated successfully.";
                return RedirectToAction(nameof(Index), new { patientId = plan.PatientId, goalId = plan.GoalId });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.FoodPlan.AnyAsync(e => e.FoodPlanId == id)) return NotFound();
                TempData["Error"] = "The record was modified by another user. Please reload and try again.";
                return View(plan);
            }
        }

        // GET: /FoodPlan/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var plan = await _context.FoodPlan
                .Include(p => p.Goal).Include(p => p.Food).Include(p => p.Nutritionist)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.FoodPlanId == id);
            if (plan == null) return NotFound();
            return View(plan);
        }

        // POST: /FoodPlan/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var plan = await _context.FoodPlan.FindAsync(id);
            if (plan == null) return NotFound();
            int pat = plan.PatientId; int gol = plan.GoalId;

            _context.FoodPlan.Remove(plan);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Plan deleted successfully.";
            return RedirectToAction(nameof(Index), new { patientId = pat, goalId = gol });
        }
    }
}
