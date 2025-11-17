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
    public class EventsController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public EventsController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchName, string searchType, string searchStatus, int page = 1)
        {
            ViewBag.SearchName = searchName;
            ViewBag.SearchType = searchType;
            ViewBag.SearchStatus = searchStatus;

            var statusList = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "Todos os Estados" },
                new SelectListItem { Value = "Agendado", Text = "Agendado" },
                new SelectListItem { Value = "Adecorrer", Text = "A Decorrer" },
                new SelectListItem { Value = "Realizado", Text = "Realizado" }
            };

            var selectedStatus = statusList.FirstOrDefault(s => s.Value == searchStatus);
            if (selectedStatus != null)
            {
                selectedStatus.Selected = true;
            }
            ViewBag.StatusList = statusList;

            var events = _context.Event.Include(e => e.EventType).Select(e => e);

            if (!string.IsNullOrEmpty(searchName))
            {
                events = events.Where(e => e.EventName.Contains(searchName));
            }

            if (!string.IsNullOrEmpty(searchType))
            {
                events = events.Where(e => e.EventType != null && e.EventType.EventTypeName.Contains(searchType));
            }

            if (!string.IsNullOrEmpty(searchStatus))
            {
                var now = DateTime.Now;
                if (searchStatus == "Agendado")
                {
                    events = events.Where(e => e.EventStart > now);
                }
                else if (searchStatus == "Adecorrer")
                {
                    events = events.Where(e => e.EventStart <= now && e.EventEnd >= now);
                }
                else if (searchStatus == "Realizado")
                {
                    events = events.Where(e => e.EventEnd < now);
                }
            }

            int pageSize = 5;
            int totalItems = await events.CountAsync();

            var pagedEvents = await events
                                    .OrderByDescending(e => e.EventStart)
                                    .Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync();

            var paginationInfo = new PaginationInfo<Event>(pagedEvents, totalItems, pageSize, page);

            return View(paginationInfo);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return View("InvalidEvent");

            var @event = await _context.Event
                .Include(e => e.EventType)
                .FirstOrDefaultAsync(m => m.EventId == id);

            if (@event == null)
                return View("InvalidEvent");

            return View(@event);
        }

        private void PopulateEventTypesDropDownList(object? selectedEventType = null)
        {
            var eventTypesQuery = from et in _context.EventType
                                  orderby et.EventTypeName
                                  select et;
            ViewBag.EventTypeId = new SelectList(eventTypesQuery.AsNoTracking(), "EventTypeId", "EventTypeName", selectedEventType);
        }

        public IActionResult Create()
        {
            PopulateEventTypesDropDownList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventId,EventName,EventDescription,EventTypeId,EventStart,EventEnd,EventPoints,MinLevel")] Event @event)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@event);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event created successfully!";
                return RedirectToAction(nameof(Index));
            }

            PopulateEventTypesDropDownList(@event.EventTypeId);
            return View(@event);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return View("InvalidEvent");

            var @event = await _context.Event.FindAsync(id);
            if (@event == null)
                return View("InvalidEvent");

            PopulateEventTypesDropDownList(@event.EventTypeId);
            return View(@event);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventId,EventName,EventDescription,EventTypeId,EventStart,EventEnd,EventPoints,MinLevel")] Event @event)
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

            PopulateEventTypesDropDownList(@event.EventTypeId);
            return View(@event);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return View("InvalidEvent");

            var @event = await _context.Event
                .Include(e => e.EventType)
                .FirstOrDefaultAsync(m => m.EventId == id);

            if (@event == null)
                return View("InvalidEvent");

            return View(@event);
        }

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