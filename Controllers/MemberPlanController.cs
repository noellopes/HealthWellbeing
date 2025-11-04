using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers
{
    public class MemberPlanController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public MemberPlanController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: MemberPlan
        public async Task<IActionResult> Index()
        {
            var healthWellbeingDbContext = _context.MemberPlan.Include(m => m.Member).Include(m => m.Plan);
            return View(await healthWellbeingDbContext.ToListAsync());
        }

        // GET: MemberPlan/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var memberPlan = await _context.MemberPlan
                .Include(m => m.Member)
                .Include(m => m.Plan)
                .FirstOrDefaultAsync(m => m.MemberPlanId == id);
            if (memberPlan == null)
            {
                return NotFound();
            }

            return View(memberPlan);
        }

        // GET: MemberPlan/Create
        public IActionResult Create(int memberId)
        {
            ViewData["PlanId"] = new SelectList(_context.Plan, "PlanId", "Name");
            ViewData["MemberId"] = memberId;
            return View();
        }

        // POST: MemberPlan/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MemberPlanId,MemberId,PlanId,StartDate,EndDate,Status")] MemberPlan memberPlan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(memberPlan);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Plan created successfully";
                return RedirectToAction(nameof(Index));
            }
            ViewData["MemberId"] = new SelectList(_context.Member, "MemberId", "MemberId", memberPlan.MemberId);
            ViewData["PlanId"] = new SelectList(_context.Plan, "PlanId", "Name", memberPlan.PlanId);
            return View(memberPlan);
        }

        // GET: MemberPlan/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var memberPlan = await _context.MemberPlan.FindAsync(id);
            if (memberPlan == null)
            {
                return NotFound();
            }
            ViewData["MemberId"] = new SelectList(_context.Member, "MemberId", "MemberId", memberPlan.MemberId);
            ViewData["PlanId"] = new SelectList(_context.Plan, "PlanId", "Name", memberPlan.PlanId);
            return View(memberPlan);
        }

        // POST: MemberPlan/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MemberPlanId,MemberId,PlanId,StartDate,EndDate,Status")] MemberPlan memberPlan)
        {
            if (id != memberPlan.MemberPlanId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(memberPlan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MemberPlanExists(memberPlan.MemberPlanId))
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
            ViewData["MemberId"] = new SelectList(_context.Member, "MemberId", "MemberId", memberPlan.MemberId);
            ViewData["PlanId"] = new SelectList(_context.Plan, "PlanId", "Name", memberPlan.PlanId);
            return View(memberPlan);
        }

        // GET: MemberPlan/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var memberPlan = await _context.MemberPlan
                .Include(m => m.Member)
                .Include(m => m.Plan)
                .FirstOrDefaultAsync(m => m.MemberPlanId == id);
            if (memberPlan == null)
            {
                return NotFound();
            }

            return View(memberPlan);
        }

        // POST: MemberPlan/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var memberPlan = await _context.MemberPlan.FindAsync(id);
            if (memberPlan != null)
            {
                _context.MemberPlan.Remove(memberPlan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MemberPlanExists(int id)
        {
            return _context.MemberPlan.Any(e => e.MemberPlanId == id);
        }
    }
}
