using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers
{
    public class EventsController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public EventsController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Events
        public async Task<IActionResult> Index(string searchName, string searchType)
        {
            // Guarda os filtros na ViewBag para manter o texto nas caixas de pesquisa
            ViewBag.SearchName = searchName;
            ViewBag.SearchType = searchType;

            var events = from e in _context.Event select e;

            // Filtra por nome
            if (!string.IsNullOrEmpty(searchName))
            {
                events = events.Where(e => e.EventName.Contains(searchName));
            }

            // Filtra por tipo
            if (!string.IsNullOrEmpty(searchType))
            {
                events = events.Where(e => e.EventType.Contains(searchType));
            }

            var eventList = await events.ToListAsync();
            return View(eventList);
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return View("InvalidEvent");

            var @event = await _context.Event.FirstOrDefaultAsync(m => m.EventId == id);
            if (@event == null)
                return View("InvalidEvent");

            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventId,EventName,EventDescription,EventType,DurationMinutes,Intensity,EventDate")] Event @event)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@event);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return View("InvalidEvent");

            var @event = await _context.Event.FindAsync(id);
            if (@event == null)
                return View("InvalidEvent");

            return View(@event);
        }

        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventId,EventName,EventDescription,EventType,DurationMinutes,Intensity,EventDate")] Event @event)
        {
            if (id != @event.EventId)
                return View("InvalidEvent");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Event updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.EventId))
                        return View("InvalidEvent");
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return View("InvalidEvent");

            var @event = await _context.Event.FirstOrDefaultAsync(m => m.EventId == id);
            if (@event == null)
                return View("InvalidEvent");

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Event.FindAsync(id);
            if (@event == null)
                return View("InvalidEvent");

            _context.Event.Remove(@event);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Event deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Event.Any(e => e.EventId == id);
        }
    }
}
