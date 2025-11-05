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

        public FoodPlanController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        private async Task LoadDropDownsAsync(string? clientId = null, int? goalId = null, int? foodId = null, int? nutritionistId = null)
        {
            ViewBag.ClientId = new SelectList(
                await _context.Client.AsNoTracking().OrderBy(c => c.Name).ToListAsync(),
                "ClientId", "Name", clientId
            );

            // Goals (filtra pelo cliente se existir)
            var goalsQuery = _context.Goal.AsNoTracking().OrderBy(g => g.GoalType).AsQueryable();
            if (!string.IsNullOrWhiteSpace(clientId))
                goalsQuery = goalsQuery.Where(g => g.ClientId == clientId);

            ViewBag.GoalId = new SelectList(
                await goalsQuery.ToListAsync(),
                "GoalId", "GoalType", goalId
            );

            ViewBag.FoodId = new SelectList(
                await _context.Food.AsNoTracking().OrderBy(f => f.Name).ToListAsync(),
                "FoodId", "Name", foodId
            );

            ViewBag.NutritionistId = new SelectList(
                await _context.Nutritionist.AsNoTracking().OrderBy(n => n.Name).ToListAsync(),
                "NutritionistId", "Name", nutritionistId
            );
        }

        public async Task<IActionResult> Index()
        {
            var list = await _context.FoodPlan
                .Include(p => p.Client)
                .Include(p => p.Goal)
                .Include(p => p.Food)
                .Include(p => p.Nutritionist)
                .AsNoTracking()
                .OrderByDescending(p => p.FoodPlanId)
                .ToListAsync();

            return View(list);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var model = await _context.FoodPlan
                .Include(p => p.Client)
                .Include(p => p.Goal)
                .Include(p => p.Food)
                .Include(p => p.Nutritionist)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.FoodPlanId == id);

            if (model == null) return NotFound();
            return View(model);
        }

        public async Task<IActionResult> Create(string? clientId = null)
        {
            await LoadDropDownsAsync(clientId, null, null, null);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClientId,GoalId,FoodId,Quantity,Description,NutritionistId")] FoodPlan model)
        {
            if (!string.IsNullOrWhiteSpace(model.ClientId))
            {
                bool goalMatchesClient = await _context.Goal.AnyAsync(g => g.GoalId == model.GoalId && g.ClientId == model.ClientId);
                if (!goalMatchesClient)
                    ModelState.AddModelError(nameof(FoodPlan.GoalId), "Selected goal does not belong to this client.");
            }

            if (!ModelState.IsValid)
            {
                await LoadDropDownsAsync(model.ClientId, model.GoalId, model.FoodId, model.NutritionistId);
                TempData["Error"] = "Please correct the errors below.";
                return View(model);
            }

            try
            {
                model.Description = model.Description?.Trim();
                _context.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Food plan created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                TempData["Error"] = $"Error creating record: {ex.GetBaseException().Message}";
                await LoadDropDownsAsync(model.ClientId, model.GoalId, model.FoodId, model.NutritionistId);
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var model = await _context.FoodPlan.FindAsync(id);
            if (model == null) return NotFound();

            await LoadDropDownsAsync(model.ClientId, model.GoalId, model.FoodId, model.NutritionistId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FoodPlanId,ClientId,GoalId,FoodId,Quantity,Description,NutritionistId")] FoodPlan model)
        {
            if (id != model.FoodPlanId) return NotFound();

            if (!string.IsNullOrWhiteSpace(model.ClientId))
            {
                bool goalMatchesClient = await _context.Goal.AnyAsync(g => g.GoalId == model.GoalId && g.ClientId == model.ClientId);
                if (!goalMatchesClient)
                    ModelState.AddModelError(nameof(FoodPlan.GoalId), "Selected goal does not belong to this client.");
            }

            if (!ModelState.IsValid)
            {
                await LoadDropDownsAsync(model.ClientId, model.GoalId, model.FoodId, model.NutritionistId);
                TempData["Error"] = "Please correct the errors below.";
                return View(model);
            }

            try
            {
                var entity = await _context.FoodPlan.FirstOrDefaultAsync(p => p.FoodPlanId == id);
                if (entity == null) return NotFound();

                entity.ClientId = model.ClientId;
                entity.GoalId = model.GoalId;
                entity.FoodId = model.FoodId;
                entity.Quantity = model.Quantity;
                entity.Description = model.Description?.Trim();
                entity.NutritionistId = model.NutritionistId;

                await _context.SaveChangesAsync();
                TempData["Success"] = "Food plan updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.FoodPlan.AnyAsync(p => p.FoodPlanId == id))
                    return NotFound();

                TempData["Error"] = "Concurrency conflict. Please reload and try again.";
                await LoadDropDownsAsync(model.ClientId, model.GoalId, model.FoodId, model.NutritionistId);
                return View(model);
            }
            catch (DbUpdateException ex)
            {
                TempData["Error"] = $"Could not save: {ex.GetBaseException().Message}";
                await LoadDropDownsAsync(model.ClientId, model.GoalId, model.FoodId, model.NutritionistId);
                return View(model);
            }
        }

      
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var model = await _context.FoodPlan
                .Include(p => p.Client)
                .Include(p => p.Goal)
                .Include(p => p.Food)
                .Include(p => p.Nutritionist)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.FoodPlanId == id);

            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entity = await _context.FoodPlan.FirstOrDefaultAsync(p => p.FoodPlanId == id);
            if (entity == null) return NotFound();

            try
            {
                _context.FoodPlan.Remove(entity);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Food plan deleted successfully.";
            }
            catch (DbUpdateException ex)
            {
                TempData["Error"] = $"Could not delete: {ex.GetBaseException().Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GoalsByClient(string clientId)
        {
            var data = await _context.Goal
                .Where(g => g.ClientId == clientId)
                .OrderBy(g => g.GoalType)
                .Select(g => new { g.GoalId, g.GoalType })
                .ToListAsync();

            return Json(data);
        }
    }
}
