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
    public class ClientController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public ClientController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Client
        public async Task<IActionResult> Index()
        {
            return View(await _context.Client.ToListAsync());
        }

        // GET: Client/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Client
                .FirstOrDefaultAsync(m => m.ClientID == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Client/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Client/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClientID,Name,Email,Phone,Address,BirthDate,Gender,RegistrationDate,CreateMember")] Client client)
        {
            if (ModelState.IsValid)
            {
                _context.Add(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // GET: Client/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Client.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        // POST: Client/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ClientID,Name,Email,Phone,Address,BirthDate,Gender,RegistrationDate,CreateMember")] Client client)
        {
            if (id != client.ClientID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.ClientID))
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
            return View(client);
        }

        // GET: Client/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Client
                .FirstOrDefaultAsync(m => m.ClientID == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Client/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var client = await _context.Client.FindAsync(id);
            if (client != null)
            {
                _context.Client.Remove(client);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(string id)
        {
            return _context.Client.Any(e => e.ClientID == id);
        }
    }
}
