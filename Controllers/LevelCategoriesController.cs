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
    public class LevelCategoriesController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public LevelCategoriesController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: LevelCategories
        public async Task<IActionResult> Index()
        {
            return View(await _context.LevelCategory.ToListAsync());
        }

        // GET: LevelCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var levelCategory = await _context.LevelCategory
                .FirstOrDefaultAsync(m => m.LevelCategoryId == id);
            if (levelCategory == null)
            {
                return NotFound();
            }

            return View(levelCategory);
        }

        // GET: LevelCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LevelCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LevelCategoryId,Name")] LevelCategory levelCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(levelCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(levelCategory);
        }

        // GET: LevelCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var levelCategory = await _context.LevelCategory.FindAsync(id);
            if (levelCategory == null)
            {
                return NotFound();
            }
            return View(levelCategory);
        }

        // POST: LevelCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LevelCategoryId,Name")] LevelCategory levelCategory)
        {
            if (id != levelCategory.LevelCategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(levelCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LevelCategoryExists(levelCategory.LevelCategoryId))
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
            return View(levelCategory);
        }

        // GET: LevelCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var levelCategory = await _context.LevelCategory
                .FirstOrDefaultAsync(m => m.LevelCategoryId == id);
            if (levelCategory == null)
            {
                return NotFound();
            }

            return View(levelCategory);
        }

        // POST: LevelCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var levelCategory = await _context.LevelCategory.FindAsync(id);
            if (levelCategory != null)
            {
                _context.LevelCategory.Remove(levelCategory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LevelCategoryExists(int id)
        {
            return _context.LevelCategory.Any(e => e.LevelCategoryId == id);
        }
    }
}
