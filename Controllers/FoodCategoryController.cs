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
    public class FoodCategoryController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public FoodCategoryController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: FoodCategories
        public async Task<IActionResult> Index(string? search, int page = 1, int itemsPerPage = 10)
        {
            var query = _context.FoodCategory.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();

                query = query.Where(fc =>
                    fc.Category.ToLower().Contains(search) ||
                    (!string.IsNullOrEmpty(fc.Description) && fc.Description.ToLower().Contains(search)));
            }

            int totalItems = await query.CountAsync();

            var items = await query
                .OrderBy(fc => fc.Category)
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToListAsync();

            var model = new PaginationInfos<FoodCategory>(items, totalItems, page, itemsPerPage);

            ViewBag.Search = search;

            return View(model);
        }


        // GET: FoodCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var foodCategory = await _context.FoodCategory
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (foodCategory == null)
            {
                return NotFound();
            }

            return View(foodCategory);
        }

        // GET: FoodCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FoodCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,Category,Description")] FoodCategory foodCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(foodCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(foodCategory);
        }

        // GET: FoodCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var foodCategory = await _context.FoodCategory.FindAsync(id);
            if (foodCategory == null)
            {
                return NotFound();
            }
            return View(foodCategory);
        }

        // POST: FoodCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId,Category,Description")] FoodCategory foodCategory)
        {
            if (id != foodCategory.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(foodCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FoodCategoryExists(foodCategory.CategoryId))
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
            return View(foodCategory);
        }

        // GET: FoodCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var foodCategory = await _context.FoodCategory
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (foodCategory == null)
            {
                return NotFound();
            }

            return View(foodCategory);
        }

        // POST: FoodCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var foodCategory = await _context.FoodCategory.FindAsync(id);
            if (foodCategory != null)
            {
                _context.FoodCategory.Remove(foodCategory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FoodCategoryExists(int id)
        {
            return _context.FoodCategory.Any(e => e.CategoryId == id);
        }
    }
}
