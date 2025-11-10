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
    public class LocationMedDeviceController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public LocationMedDeviceController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: LocationMedDevices
        public async Task<IActionResult> Index()
        {
            return View(await _context.LocationMedDevice.ToListAsync());
        }

        // GET: LocationMedDevices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var LocationMedDevice = await _context.LocationMedDevice
                .FirstOrDefaultAsync(m => m.LocationMedDeviceID == id);
            if (LocationMedDevice == null)
            {
                return NotFound();
            }

            return View(LocationMedDevice);
        }

        // GET: LocationMedDevices/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LocationMedDevices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LocationMedDeviceID,MedicalDeviceID,RoomID,InitialDate,EndDate")] LocationMedDevice locationMedDevice)
        {
            if (ModelState.IsValid)
            {
                _context.Add(locationMedDevice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(locationMedDevice);
        }


        // GET: LocationMedDevices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var LocationMedDevice = await _context.LocationMedDevice.FindAsync(id);
            if (LocationMedDevice == null)
            {
                return NotFound();
            }
            return View(LocationMedDevice);
        }

        // POST: LocationMedDevices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LocationMedDeviceID,MedicalDeviceID,RoomID,InitialDate,EndDate")] LocationMedDevice locationMedDevice)
        {
            if (id != locationMedDevice.LocationMedDeviceID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(locationMedDevice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LocationMedDeviceExists(locationMedDevice.LocationMedDeviceID))
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
            return View(locationMedDevice);
        }

        // GET: LocationMedDevices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var LocationMedDevice = await _context.LocationMedDevice
                .FirstOrDefaultAsync(m => m.LocationMedDeviceID == id);
            if (LocationMedDevice == null)
            {
                return NotFound();
            }

            return View(LocationMedDevice);
        }

        // POST: LocationMedDevices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var LocationMedDevice = await _context.LocationMedDevice.FindAsync(id);
            if (LocationMedDevice != null)
            {
                _context.LocationMedDevice.Remove(LocationMedDevice);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LocationMedDeviceExists(int id)
        {
            return _context.LocationMedDevice.Any(e => e.LocationMedDeviceID == id);
        }
    }
}
