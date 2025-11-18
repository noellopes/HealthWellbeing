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
    public class MemberPlanController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public MemberPlanController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: MemberPlan
        public async Task<IActionResult> Index(int page = 1, string searchClientName = "", string searchPlanName = "", string searchStatus = "")
        {
            var memberPlansQuery = _context.MemberPlan
                .Include(mp => mp.Member)
                    .ThenInclude(m => m.Client) // Incluir Cliente para ter o Nome
                .Include(mp => mp.Plan)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchClientName))
            {
                memberPlansQuery = memberPlansQuery.Where(mp => mp.Member.Client.Name.Contains(searchClientName));
            }

            if (!string.IsNullOrEmpty(searchPlanName))
            {
                memberPlansQuery = memberPlansQuery.Where(mp => mp.Plan.Name.Contains(searchPlanName));
            }

            // Filtro de Estado (Status)
            if (!string.IsNullOrEmpty(searchStatus))
            {
                memberPlansQuery = memberPlansQuery.Where(mp => mp.Status == searchStatus);
            }

            ViewBag.SearchClientName = searchClientName;
            ViewBag.SearchPlanName = searchPlanName;
            ViewBag.SearchStatus = searchStatus;

            int totalItems = await memberPlansQuery.CountAsync();
            var paginationInfo = new PaginationInfo<MemberPlan>(page, totalItems, 5);

            paginationInfo.Items = await memberPlansQuery
                .OrderByDescending(mp => mp.StartDate) // Ordenar por data mais recente
                .Skip(paginationInfo.ItemsToSkip)
                .Take(paginationInfo.ItemsPerPage)
                .ToListAsync();

            return View(paginationInfo);
        }

        // GET: MemberPlan/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var memberPlan = await _context.MemberPlan
                .Include(mp => mp.Member).ThenInclude(m => m.Client)
                .Include(mp => mp.Plan)
                .FirstOrDefaultAsync(m => m.MemberPlanId == id);

            if (memberPlan == null) return NotFound();

            return View(memberPlan);
        }

        // GET: MemberPlan/Create
        public IActionResult Create()
        {
            // Dropdowns para escolher o Membro (mostrando o nome do Cliente) e o Plano
            ViewData["MemberId"] = new SelectList(_context.Member.Include(m => m.Client), "MemberId", "Client.Name");
            ViewData["PlanId"] = new SelectList(_context.Plan, "PlanId", "Name");
            return View();
        }

        // POST: MemberPlan/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MemberId,PlanId,StartDate,EndDate,Status")] MemberPlan memberPlan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(memberPlan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["MemberId"] = new SelectList(_context.Member.Include(m => m.Client), "MemberId", "Client.Name", memberPlan.MemberId);
            ViewData["PlanId"] = new SelectList(_context.Plan, "PlanId", "Name", memberPlan.PlanId);
            return View(memberPlan);
        }

        // GET: MemberPlan/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var memberPlan = await _context.MemberPlan.FindAsync(id);
            if (memberPlan == null) return NotFound();

            ViewData["MemberId"] = new SelectList(_context.Member.Include(m => m.Client), "MemberId", "Client.Name", memberPlan.MemberId);
            ViewData["PlanId"] = new SelectList(_context.Plan, "PlanId", "Name", memberPlan.PlanId);
            return View(memberPlan);
        }

        // POST: MemberPlan/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MemberPlanId,MemberId,PlanId,StartDate,EndDate,Status")] MemberPlan memberPlan)
        {
            if (id != memberPlan.MemberPlanId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(memberPlan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MemberPlanExists(memberPlan.MemberPlanId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MemberId"] = new SelectList(_context.Member.Include(m => m.Client), "MemberId", "Client.Name", memberPlan.MemberId);
            ViewData["PlanId"] = new SelectList(_context.Plan, "PlanId", "Name", memberPlan.PlanId);
            return View(memberPlan);
        }

        // GET: MemberPlan/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var memberPlan = await _context.MemberPlan
                .Include(mp => mp.Member).ThenInclude(m => m.Client)
                .Include(mp => mp.Plan)
                .FirstOrDefaultAsync(m => m.MemberPlanId == id);

            if (memberPlan == null) return NotFound();

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