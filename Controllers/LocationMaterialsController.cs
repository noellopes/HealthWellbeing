using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;

namespace HealthWellbeingRoom.Controllers
{
    public class LocationMaterialsController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public LocationMaterialsController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: LocationMaterials
        public async Task<IActionResult> Index()
        {
            return View(await _context.LocationMaterial.ToListAsync());
        }

        // GET: LocationMaterials/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var locationMaterial = await _context.LocationMaterial
                .FirstOrDefaultAsync(m => m.LocationMaterialID == id);
            if (locationMaterial == null)
            {
                return NotFound();
            }

            return View(locationMaterial);
        }

        // GET: LocationMaterials/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LocationMaterials/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LocationMaterialID,Sector,Room,Cabinet,Drawer,Shelf,IdentificationCode,Observation")] LocationMaterial locationMaterial)
        {
            if (ModelState.IsValid)
            {
                _context.Add(locationMaterial);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(locationMaterial);
        }

        // GET: LocationMaterials/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var locationMaterial = await _context.LocationMaterial.FindAsync(id);
            if (locationMaterial == null)
            {
                return NotFound();
            }
            return View(locationMaterial);
        }

        // POST: LocationMaterials/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LocationMaterialID,Sector,Room,Cabinet,Drawer,Shelf,IdentificationCode,Observation")] LocationMaterial locationMaterial)
        {
            if (id != locationMaterial.LocationMaterialID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(locationMaterial);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LocationMaterialExists(locationMaterial.LocationMaterialID))
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
            return View(locationMaterial);
        }

        // GET: LocationMaterials/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var locationMaterial = await _context.LocationMaterial
                .FirstOrDefaultAsync(m => m.LocationMaterialID == id);
            if (locationMaterial == null)
            {
                return NotFound();
            }

            return View(locationMaterial);
        }

        // POST: LocationMaterials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var locationMaterial = await _context.LocationMaterial.FindAsync(id);
            if (locationMaterial != null)
            {
                _context.LocationMaterial.Remove(locationMaterial);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LocationMaterialExists(int id)
        {
            return _context.LocationMaterial.Any(e => e.LocationMaterialID == id);
        }
    }
}
