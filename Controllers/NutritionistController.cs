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
    public class NutritionistController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public NutritionistController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Nutritionists
        public async Task<IActionResult> Index(string? search, int page = 1, int itemsPerPage = 10)
        {
            var query = _context.Nutritionist.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();

                query = query.Where(n =>
                    n.Name.ToLower().Contains(search) ||
                    (!string.IsNullOrEmpty(n.Email) && n.Email.ToLower().Contains(search)) ||
                    (!string.IsNullOrEmpty(n.Gender) && n.Gender.ToLower().Contains(search)));
            }

            int totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(n => n.Name)
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToListAsync();

            var model = new PaginationInfo<Nutritionist>(items, totalItems, page, itemsPerPage);

            ViewBag.Search = search;

            return View(model);
        }


        // GET: Nutritionists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nutritionist = await _context.Nutritionist
                .FirstOrDefaultAsync(m => m.NutritionistId == id);
            if (nutritionist == null)
            {
                return NotFound();
            }

            return View(nutritionist);
        }

        // GET: Nutritionists/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Nutritionists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NutritionistId,Name,Gender,Email")] Nutritionist nutritionist)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nutritionist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(nutritionist);
        }

        // GET: Nutritionists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nutritionist = await _context.Nutritionist.FindAsync(id);
            if (nutritionist == null)
            {
                return NotFound();
            }
            return View(nutritionist);
        }

        // POST: Nutritionists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NutritionistId,Name,Gender,Email")] Nutritionist nutritionist)
        {
            if (id != nutritionist.NutritionistId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nutritionist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NutritionistExists(nutritionist.NutritionistId))
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
            return View(nutritionist);
        }

        // GET: Nutritionists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nutritionist = await _context.Nutritionist
                .FirstOrDefaultAsync(m => m.NutritionistId == id);
            if (nutritionist == null)
            {
                return NotFound();
            }

            return View(nutritionist);
        }

        // POST: Nutritionists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nutritionist = await _context.Nutritionist.FindAsync(id);
            if (nutritionist != null)
            {
                _context.Nutritionist.Remove(nutritionist);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NutritionistExists(int id)
        {
            return _context.Nutritionist.Any(e => e.NutritionistId == id);
        }
    }
}
