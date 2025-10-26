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
    public class MentalHealthProfessionalsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MentalHealthProfessionalsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MentalHealthProfessionals
        public async Task<IActionResult> Index()
        {
            return View(await _context.MentalHealthProfessionals.ToListAsync());
        }

        // GET: MentalHealthProfessionals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mentalHealthProfessional = await _context.MentalHealthProfessionals
                .FirstOrDefaultAsync(m => m.ProfessionalId == id);
            if (mentalHealthProfessional == null)
            {
                return NotFound();
            }

            return View(mentalHealthProfessional);
        }

        // GET: MentalHealthProfessionals/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MentalHealthProfessionals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProfessionalId,UserId,FirstName,LastName,Email,Phone,Type,Specialization,LicenseNumber,IsActive")] MentalHealthProfessional mentalHealthProfessional)
        {
            if (ModelState.IsValid)
            {
                _context.Add(mentalHealthProfessional);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(mentalHealthProfessional);
        }

        // GET: MentalHealthProfessionals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mentalHealthProfessional = await _context.MentalHealthProfessionals.FindAsync(id);
            if (mentalHealthProfessional == null)
            {
                return NotFound();
            }
            return View(mentalHealthProfessional);
        }

        // POST: MentalHealthProfessionals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProfessionalId,UserId,FirstName,LastName,Email,Phone,Type,Specialization,LicenseNumber,IsActive")] MentalHealthProfessional mentalHealthProfessional)
        {
            if (id != mentalHealthProfessional.ProfessionalId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mentalHealthProfessional);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MentalHealthProfessionalExists(mentalHealthProfessional.ProfessionalId))
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
            return View(mentalHealthProfessional);
        }

        // GET: MentalHealthProfessionals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mentalHealthProfessional = await _context.MentalHealthProfessionals
                .FirstOrDefaultAsync(m => m.ProfessionalId == id);
            if (mentalHealthProfessional == null)
            {
                return NotFound();
            }

            return View(mentalHealthProfessional);
        }

        // POST: MentalHealthProfessionals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mentalHealthProfessional = await _context.MentalHealthProfessionals.FindAsync(id);
            if (mentalHealthProfessional != null)
            {
                _context.MentalHealthProfessionals.Remove(mentalHealthProfessional);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MentalHealthProfessionalExists(int id)
        {
            return _context.MentalHealthProfessionals.Any(e => e.ProfessionalId == id);
        }
    }
}
