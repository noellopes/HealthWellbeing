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
    public class MoodTrackingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MoodTrackingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MoodTracking
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.MoodEntries.Include(m => m.Client);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: MoodTracking/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var moodEntry = await _context.MoodEntries
                .Include(m => m.Client)
                .FirstOrDefaultAsync(m => m.MoodEntryId == id);
            if (moodEntry == null)
            {
                return NotFound();
            }

            return View(moodEntry);
        }

        // GET: MoodTracking/Create
        public IActionResult Create()
        {
            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "Email");
            return View();
        }

        // POST: MoodTracking/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MoodEntryId,ClientId,EntryDateTime,MoodScore,Emotion,Notes,Triggers")] MoodEntry moodEntry)
        {
            if (ModelState.IsValid)
            {
                _context.Add(moodEntry);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "Email", moodEntry.ClientId);
            return View(moodEntry);
        }

        // GET: MoodTracking/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var moodEntry = await _context.MoodEntries.FindAsync(id);
            if (moodEntry == null)
            {
                return NotFound();
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "Email", moodEntry.ClientId);
            return View(moodEntry);
        }

        // POST: MoodTracking/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MoodEntryId,ClientId,EntryDateTime,MoodScore,Emotion,Notes,Triggers")] MoodEntry moodEntry)
        {
            if (id != moodEntry.MoodEntryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(moodEntry);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MoodEntryExists(moodEntry.MoodEntryId))
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
            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "Email", moodEntry.ClientId);
            return View(moodEntry);
        }

        // GET: MoodTracking/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var moodEntry = await _context.MoodEntries
                .Include(m => m.Client)
                .FirstOrDefaultAsync(m => m.MoodEntryId == id);
            if (moodEntry == null)
            {
                return NotFound();
            }

            return View(moodEntry);
        }

        // POST: MoodTracking/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var moodEntry = await _context.MoodEntries.FindAsync(id);
            if (moodEntry != null)
            {
                _context.MoodEntries.Remove(moodEntry);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MoodEntryExists(int id)
        {
            return _context.MoodEntries.Any(e => e.MoodEntryId == id);
        }

        // GET: MoodTracking/Chart/5
        public async Task<IActionResult> Chart(int clientId, int days = 30)
        {
            var startDate = DateTime.Now.AddDays(-days);

            var moodData = await _context.MoodEntries
                .Where(m => m.ClientId == clientId && m.EntryDateTime >= startDate)
                .OrderBy(m => m.EntryDateTime)
                .Select(m => new {
                    Date = m.EntryDateTime.ToString("MM/dd"),
                    Score = m.MoodScore,
                    Emotion = m.Emotion
                })
                .ToListAsync();

            var client = await _context.Clients.FindAsync(clientId);

            ViewBag.ClientName = $"{client?.FirstName} {client?.LastName}";
            ViewBag.MoodData = moodData;

            return View();
        }
    }
}
