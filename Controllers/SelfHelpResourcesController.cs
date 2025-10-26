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
    public class SelfHelpResourcesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SelfHelpResourcesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SelfHelpResources
        public async Task<IActionResult> Index()
        {
            return View(await _context.SelfHelpResources.ToListAsync());
        }

        // GET: SelfHelpResources/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var selfHelpResource = await _context.SelfHelpResources
                .FirstOrDefaultAsync(m => m.ResourceId == id);
            if (selfHelpResource == null)
            {
                return NotFound();
            }

            return View(selfHelpResource);
        }

        // GET: SelfHelpResources/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SelfHelpResources/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ResourceId,Type,Title,Description,Content,Url,DurationMinutes,Category,IsActive")] SelfHelpResource selfHelpResource)
        {
            if (ModelState.IsValid)
            {
                _context.Add(selfHelpResource);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(selfHelpResource);
        }

        // GET: SelfHelpResources/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var selfHelpResource = await _context.SelfHelpResources.FindAsync(id);
            if (selfHelpResource == null)
            {
                return NotFound();
            }
            return View(selfHelpResource);
        }

        // POST: SelfHelpResources/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ResourceId,Type,Title,Description,Content,Url,DurationMinutes,Category,IsActive")] SelfHelpResource selfHelpResource)
        {
            if (id != selfHelpResource.ResourceId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(selfHelpResource);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SelfHelpResourceExists(selfHelpResource.ResourceId))
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
            return View(selfHelpResource);
        }

        // GET: SelfHelpResources/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var selfHelpResource = await _context.SelfHelpResources
                .FirstOrDefaultAsync(m => m.ResourceId == id);
            if (selfHelpResource == null)
            {
                return NotFound();
            }

            return View(selfHelpResource);
        }

        // POST: SelfHelpResources/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var selfHelpResource = await _context.SelfHelpResources.FindAsync(id);
            if (selfHelpResource != null)
            {
                _context.SelfHelpResources.Remove(selfHelpResource);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SelfHelpResourceExists(int id)
        {
            return _context.SelfHelpResources.Any(e => e.ResourceId == id);
        }
    }
}
