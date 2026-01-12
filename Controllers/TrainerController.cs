using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HealthWellbeing.Controllers
{
    [Authorize]
    public class TrainerController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public TrainerController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Trainer
        [Authorize(Roles = "Administrator,Trainer")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Trainer.ToListAsync());
        }

        // GET: Trainer/Details/5
        [Authorize(Roles = "Administrator,Trainer")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainer = await _context.Trainer
                .FirstOrDefaultAsync(m => m.TrainerId == id);

            if (trainer == null)
            {
                return View("InvalidTrainer");
            }

            return View(trainer);
        }

        // GET: Trainer/Create
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Trainer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([Bind("Name,Email,Phone,Address,BirthDate,Gender,IsAdministrator,IsTrainer")] Trainer trainer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trainer);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Trainer successfully created!";
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }

        // GET: Trainer/Edit/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainer = await _context.Trainer.FindAsync(id);
            if (trainer == null)
            {
                return NotFound();
            }
            return View(trainer);
        }

        // POST: Trainer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Edit(int id, [Bind("TrainerId,Name,Email,Phone,Address,BirthDate,Gender,IsAdministrator,IsTrainer,IsActive")] Trainer trainer)
        {
            if (id != trainer.TrainerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trainer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainerExists(trainer.TrainerId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["SuccessMessage"] = "Trainer information updated.";
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }

        // GET: Trainer/Delete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainer = await _context.Trainer
                .FirstOrDefaultAsync(m => m.TrainerId == id);
            if (trainer == null)
            {
                return NotFound();
            }

            return View(trainer);
        }

        // POST: Trainer/Delete/5
        [Authorize(Roles = "Administrator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trainer = await _context.Trainer.FindAsync(id);
            if (trainer != null)
            {
                _context.Trainer.Remove(trainer);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TrainerExists(int id)
        {
            return _context.Trainer.Any(e => e.TrainerId == id);
        }
    }
}
