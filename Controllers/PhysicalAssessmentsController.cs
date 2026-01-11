using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthWellbeing.Controllers
{
    [Authorize(Roles = "Administrator,Trainer")]
    public class PhysicalAssessmentsController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public PhysicalAssessmentsController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: PhysicalAssessments
        public async Task<IActionResult> Index()
        {
            // Adicionamos o Include(p => p.Member.Client) para conseguir mostrar o Nome do Cliente na lista
            var assessments = _context.PhysicalAssessment
                .Include(p => p.Member)
                    .ThenInclude(m => m.Client)
                .Include(p => p.Trainer);
            return View(await assessments.ToListAsync());
        }

        // GET: PhysicalAssessments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var physicalAssessment = await _context.PhysicalAssessment
                .Include(p => p.Member)
                    .ThenInclude(m => m.Client)
                .Include(p => p.Trainer)
                .FirstOrDefaultAsync(m => m.PhysicalAssessmentId == id);
            if (physicalAssessment == null)
            {
                return NotFound();
            }

            return View(physicalAssessment);
        }

        // GET: PhysicalAssessments/Create
        public IActionResult Create()
        {
            ViewData["MemberId"] = new SelectList(_context.Member.Include(m => m.Client), "MemberId", "Client.Name");
            ViewData["TrainerId"] = new SelectList(_context.Trainer, "TrainerId", "Name");
            return View();
        }

        // POST: PhysicalAssessments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PhysicalAssessmentId,AssessmentDate,Weight,Height,BodyFatPercentage,MuscleMass,Notes,MemberId,TrainerId")] PhysicalAssessment physicalAssessment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(physicalAssessment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MemberId"] = new SelectList(_context.Member.Include(m => m.Client), "MemberId", "Client.Name", physicalAssessment.MemberId);
            ViewData["TrainerId"] = new SelectList(_context.Trainer, "TrainerId", "Name", physicalAssessment.TrainerId);
            return View(physicalAssessment);
        }

        // GET: PhysicalAssessments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var physicalAssessment = await _context.PhysicalAssessment.FindAsync(id);
            if (physicalAssessment == null)
            {
                return NotFound();
            }
            ViewData["MemberId"] = new SelectList(_context.Member.Include(m => m.Client), "MemberId", "Client.Name", physicalAssessment.MemberId);
            ViewData["TrainerId"] = new SelectList(_context.Trainer, "TrainerId", "Name", physicalAssessment.TrainerId);
            return View(physicalAssessment);
        }

        // POST: PhysicalAssessments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PhysicalAssessmentId,AssessmentDate,Weight,Height,BodyFatPercentage,MuscleMass,Notes,MemberId,TrainerId")] PhysicalAssessment physicalAssessment)
        {
            if (id != physicalAssessment.PhysicalAssessmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(physicalAssessment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhysicalAssessmentExists(physicalAssessment.PhysicalAssessmentId))
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
            ViewData["MemberId"] = new SelectList(_context.Member.Include(m => m.Client), "MemberId", "Client.Name", physicalAssessment.MemberId);
            ViewData["TrainerId"] = new SelectList(_context.Trainer, "TrainerId", "Name", physicalAssessment.TrainerId);
            return View(physicalAssessment);
        }

        // GET: PhysicalAssessments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var physicalAssessment = await _context.PhysicalAssessment
                .Include(p => p.Member)
                    .ThenInclude(m => m.Client)
                .Include(p => p.Trainer)
                .FirstOrDefaultAsync(m => m.PhysicalAssessmentId == id);
            if (physicalAssessment == null)
            {
                return NotFound();
            }

            return View(physicalAssessment);
        }

        // POST: PhysicalAssessments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var physicalAssessment = await _context.PhysicalAssessment.FindAsync(id);
            if (physicalAssessment != null)
            {
                _context.PhysicalAssessment.Remove(physicalAssessment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PhysicalAssessmentExists(int id)
        {
            return _context.PhysicalAssessment.Any(e => e.PhysicalAssessmentId == id);
        }
    }
}
