using System;
using System.Collections.Generic;
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
    public class TrainingPlanController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public TrainingPlanController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: TrainingPlan
        public async Task<IActionResult> Index(int page = 1, string searchPlan = "", string searchTraining = "")
        {
            var trainingPlansQuery = _context.TrainingPlan
                .Include(tp => tp.Plan)
                .Include(tp => tp.Training)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchPlan))
            {
                trainingPlansQuery = trainingPlansQuery.Where(tp => tp.Plan.Name.Contains(searchPlan));
            }

            if (!string.IsNullOrEmpty(searchTraining))
            {
                trainingPlansQuery = trainingPlansQuery.Where(tp => tp.Training.Name.Contains(searchTraining));
            }

            ViewBag.SearchPlan = searchPlan;
            ViewBag.SearchTraining = searchTraining;

            int totalItems = await trainingPlansQuery.CountAsync();
            var paginationInfo = new PaginationInfo<TrainingPlan>(page, totalItems, 5);

            paginationInfo.Items = await trainingPlansQuery
                .OrderBy(tp => tp.Plan.Name) // Order by Plan Name
                .Skip(paginationInfo.ItemsToSkip)
                .Take(paginationInfo.ItemsPerPage)
                .ToListAsync();

            return View(paginationInfo);
        }

        // GET: TrainingPlan/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var trainingPlan = await _context.TrainingPlan
                .Include(tp => tp.Plan)
                .Include(tp => tp.Training)
                .FirstOrDefaultAsync(m => m.TrainingPlanId == id);

            if (trainingPlan == null) return NotFound();

            return View(trainingPlan);
        }

        // GET: TrainingPlan/Create
        public IActionResult Create()
        {
            ViewData["PlanId"] = new SelectList(_context.Plan, "PlanId", "Name");
            ViewData["TrainingId"] = new SelectList(_context.Training, "TrainingId", "Name"); // Assuming Training has a 'Name' property
            return View();
        }

        // POST: TrainingPlan/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PlanId,TrainingId,DaysPerWeek")] TrainingPlan trainingPlan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trainingPlan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PlanId"] = new SelectList(_context.Plan, "PlanId", "Name", trainingPlan.PlanId);
            ViewData["TrainingId"] = new SelectList(_context.Training, "TrainingId", "Name", trainingPlan.TrainingId);
            return View(trainingPlan);
        }

        // GET: TrainingPlan/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var trainingPlan = await _context.TrainingPlan.FindAsync(id);
            if (trainingPlan == null) return NotFound();

            ViewData["PlanId"] = new SelectList(_context.Plan, "PlanId", "Name", trainingPlan.PlanId);
            ViewData["TrainingId"] = new SelectList(_context.Training, "TrainingId", "Name", trainingPlan.TrainingId);
            return View(trainingPlan);
        }

        // POST: TrainingPlan/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TrainingPlanId,PlanId,TrainingId,DaysPerWeek")] TrainingPlan trainingPlan)
        {
            if (id != trainingPlan.TrainingPlanId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trainingPlan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainingPlanExists(trainingPlan.TrainingPlanId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PlanId"] = new SelectList(_context.Plan, "PlanId", "Name", trainingPlan.PlanId);
            ViewData["TrainingId"] = new SelectList(_context.Training, "TrainingId", "Name", trainingPlan.TrainingId);
            return View(trainingPlan);
        }

        // GET: TrainingPlan/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var trainingPlan = await _context.TrainingPlan
                .Include(tp => tp.Plan)
                .Include(tp => tp.Training)
                .FirstOrDefaultAsync(m => m.TrainingPlanId == id);

            if (trainingPlan == null) return NotFound();

            return View(trainingPlan);
        }

        // POST: TrainingPlan/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trainingPlan = await _context.TrainingPlan.FindAsync(id);
            if (trainingPlan != null)
            {
                _context.TrainingPlan.Remove(trainingPlan);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrainingPlanExists(int id)
        {
            return _context.TrainingPlan.Any(e => e.TrainingPlanId == id);
        }
    }
}