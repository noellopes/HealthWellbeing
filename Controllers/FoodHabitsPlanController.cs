using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers
{
    public class FoodHabitsPlanController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public FoodHabitsPlanController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Plans
        public async Task<IActionResult> Index(string? search, int page = 1, int itemsPerPage = 10)
        {
            var query = _context.FoodHabitsPlan.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();

                query = query.Where(p =>
                    p.StartingDate.ToString().ToLower().Contains(search) ||
                    p.EndingDate.ToString().ToLower().Contains(search) ||
                    (p.Done ? "done" : "in progress").Contains(search));
            }

            int totalItems = await query.CountAsync();

            var items = await query
                .OrderByDescending(p => p.StartingDate)
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToListAsync();

            var model = new PaginationInfo<FoodHabitsPlan>(items, totalItems, page, itemsPerPage);

            ViewBag.Search = search;

            return View(model);
        }


        // GET: Plans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plan = await _context.FoodHabitsPlan
                .Include(p => p.NutritionistClientPlans)
                    .ThenInclude(ncp => ncp.Client)
                .Include(p => p.NutritionistClientPlans)
                    .ThenInclude(ncp => ncp.Nutritionist)
                .Include(p => p.FoodPlans)
                    .ThenInclude(fp => fp.Food)
                .Include(p => p.FoodPlans)
                    .ThenInclude(fp => fp.Portion)
                .FirstOrDefaultAsync(p => p.FoodHabitsPlanId == id);

			if (plan == null)
			{
				return NotFound();
			}

            //var Plan = await _context.Plan
            //    .Include(p => p.FoodPlans)
            //    .ThenInclude(fp => fp.Food)
            //    .Include(p => p.FoodPlans)
            //    .ThenInclude(fp => fp.Portion)
            //    .FirstOrDefaultAsync(p => p.PlanId == id);

            //if (plan == null) return NotFound();

            var start = plan.StartingDate.Date;
            var end = plan.EndingDate.Date;

            var intakes = await _context.FoodIntake
                .Where(fi => fi.PlanId == plan.FoodHabitsPlanId &&
                             fi.Date >= start && fi.Date <= end)
                .ToListAsync();

            ViewBag.Intakes = intakes;

            return View(plan);
        }

		// GET: Plan/Create
		public IActionResult Create()
		{
			return View();
		}

        // POST: Plans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PlanId,StartingDate,EndingDate,Done")] FoodHabitsPlan plan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(plan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }

		// GET: Plan/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

            var plan = await _context.FoodHabitsPlan.FindAsync(id);
            if (plan == null)
            {
                return NotFound();
            }
            return View(plan);
        }

        // POST: Plans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PlanId,StartingDate,EndingDate,Done")] FoodHabitsPlan plan)
        {
            if (id != plan.FoodHabitsPlanId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(plan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlanExists(plan.FoodHabitsPlanId))
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
            return View(plan);
        }

		// GET: Plan/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

            var plan = await _context.FoodHabitsPlan
                .FirstOrDefaultAsync(m => m.FoodHabitsPlanId == id);
            if (plan == null)
            {
                return NotFound();
            }

			return View(plan);
		}

        // POST: Plans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var plan = await _context.FoodHabitsPlan.FindAsync(id);
            if (plan != null)
            {
                _context.FoodHabitsPlan.Remove(plan);
            }

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

        private bool PlanExists(int id)
        {
            return _context.FoodHabitsPlan.Any(e => e.FoodHabitsPlanId == id);
        }
    }
}
