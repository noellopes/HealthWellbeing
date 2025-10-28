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
    public class PathologiesController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public PathologiesController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Pathologies
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Lista de patologias";
            ViewBag.ModelType = typeof(Pathology);
            ViewBag.Properties = new List<string> { "Name", "Description", "Severity" };
            return View("~/Views/Shared/Group1/Actions/Index.cshtml", await _context.Pathology.ToListAsync());
        }

        // GET: Pathologies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pathology = await _context.Pathology
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pathology == null)
            {
                return NotFound();
            }

            return View(pathology);
        }

        // GET: Pathologies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Pathologies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PathologyId,Name,Description,Severity")] Pathology pathology)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pathology);
                await _context.SaveChangesAsync();
                TempData["AlertType"] = "success";
                TempData["IconClass"] = "bi bi-check-circle";
                TempData["Message"] = "Pathology created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(pathology);
        }

        // GET: Pathologies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pathology = await _context.Pathology.FindAsync(id);
            if (pathology == null)
            {
                return NotFound();
            }
            return View(pathology);
        }

        // POST: Pathologies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Severity")] Pathology pathology)
        {
            if (id != pathology.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pathology);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PathologyExists(pathology.Id))
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
            return View(pathology);
        }

        // GET: Pathologies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pathology = await _context.Pathology
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pathology == null)
            {
                return NotFound();
            }

            return View(pathology);
        }

        // POST: Pathologies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pathology = await _context.Pathology.FindAsync(id);
            if (pathology != null)
            {
                _context.Pathology.Remove(pathology);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PathologyExists(int id)
        {
            return _context.Pathology.Any(e => e.Id == id);
        }
    }
}
