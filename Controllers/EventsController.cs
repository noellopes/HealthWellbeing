using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;

namespace HealthWellbeing.Controllers
{
    public class EventsController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public EventsController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // ALTERADO: Recebe int em vez de string
        [HttpGet]
        public async Task<IActionResult> GetActivitiesByType(int typeId)
        {
            var activities = await _context.Activity
                .Where(a => a.ActivityTypeId == typeId)
                .Select(a => new {
                    id = a.ActivityId,
                    name = a.ActivityName,
                    points = a.ActivityReward
                })
                .ToListAsync();

            return Json(activities);
        }

        public async Task<IActionResult> Index(string searchName, int? searchType, string searchStatus, int page = 1)
        {
            ViewBag.SearchName = searchName;
            ViewBag.SearchType = searchType;
            ViewBag.SearchStatus = searchStatus;

            var statusList = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "All Statuses" },
                new SelectListItem { Value = "Agendado", Text = "Agendado" },
                new SelectListItem { Value = "Adecorrer", Text = "A decorrer" },
                new SelectListItem { Value = "Realizado", Text = "Realizado" }
            };

            var selectedStatus = statusList.FirstOrDefault(s => s.Value == searchStatus);
            if (selectedStatus != null) selectedStatus.Selected = true;
            ViewBag.StatusList = statusList;

            var eventTypes = _context.EventType.OrderBy(t => t.EventTypeName);
            ViewBag.EventTypesList = new SelectList(eventTypes, "EventTypeId", "EventTypeName", searchType);

            var eventsQuery = _context.Event.Include(e => e.EventType).AsQueryable();

            if (!string.IsNullOrEmpty(searchName)) eventsQuery = eventsQuery.Where(e => e.EventName.Contains(searchName));
            if (searchType.HasValue) eventsQuery = eventsQuery.Where(e => e.EventTypeId == searchType);

            if (!string.IsNullOrEmpty(searchStatus))
            {
                var now = DateTime.Now;
                if (searchStatus == "Agendado") eventsQuery = eventsQuery.Where(e => e.EventStart > now);
                else if (searchStatus == "Adecorrer") eventsQuery = eventsQuery.Where(e => e.EventStart <= now && e.EventEnd >= now);
                else if (searchStatus == "Realizado") eventsQuery = eventsQuery.Where(e => e.EventEnd < now);
            }

            int totalItems = await eventsQuery.CountAsync();
            int pageSize = 10;
            var paginationInfo = new ViewModels.PaginationInfo<Event>(page, totalItems, pageSize);

            var pagedEvents = await eventsQuery
                .OrderByDescending(e => e.EventStart)
                .Skip(paginationInfo.ItemsToSkip)
                .Take(paginationInfo.ItemsPerPage)
                .ToListAsync();

            paginationInfo.Items = pagedEvents;
            return View(paginationInfo);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return View("InvalidEvent");

            var @event = await _context.Event
                .Include(e => e.EventType)
                .Include(e => e.ActivityType)
                .Include(e => e.EventActivities)
                    .ThenInclude(ea => ea.Activity)
                .FirstOrDefaultAsync(m => m.EventId == id);

            if (@event == null) return View("InvalidEvent");

            return View(@event);
        }

        private void PopulateEventTypesDropDownList(object? selectedEventType = null)
        {
            var eventTypesQuery = from et in _context.EventType orderby et.EventTypeName select et;
            ViewBag.EventTypeId = new SelectList(eventTypesQuery.AsNoTracking(), "EventTypeId", "EventTypeName", selectedEventType);
        }

        // --- CORREÇÃO DO ERRO ---
        // Recebe object? para aceitar int ou null. Configura ID como Valor e Name como Texto.
        private void PopulateActivityCategories(object? selectedTypeId = null)
        {
            var typesQuery = _context.ActivityType.OrderBy(t => t.Name);
            ViewBag.ActivityCategory = new SelectList(typesQuery.AsNoTracking(), "ActivityTypeId", "Name", selectedTypeId);
        }

        public IActionResult Create()
        {
            PopulateEventTypesDropDownList();
            PopulateActivityCategories();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // ALTERADO: Bind ActivityTypeId
        public async Task<IActionResult> Create([Bind("EventId,EventName,EventDescription,EventTypeId,ActivityTypeId,EventStart,EventEnd,MinLevel")] Event @event, int[] selectedActivities)
        {
            if (selectedActivities != null && selectedActivities.Length > 0)
            {
                var pointsSum = await _context.Activity
                    .Where(a => selectedActivities.Contains(a.ActivityId))
                    .SumAsync(a => a.ActivityReward);
                @event.EventPoints = pointsSum;
            }
            else { @event.EventPoints = 0; }

            if (ModelState.IsValid)
            {
                _context.Add(@event);
                await _context.SaveChangesAsync();

                if (selectedActivities != null)
                {
                    foreach (var actId in selectedActivities)
                    {
                        _context.EventActivity.Add(new EventActivity { EventId = @event.EventId, ActivityId = actId });
                    }
                    await _context.SaveChangesAsync();
                }
                TempData["SuccessMessage"] = "Event created successfully!";
                return RedirectToAction(nameof(Index));
            }

            PopulateEventTypesDropDownList(@event.EventTypeId);
            PopulateActivityCategories(@event.ActivityTypeId);
            return View(@event);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return View("InvalidEvent");

            var @event = await _context.Event
                .Include(e => e.EventActivities)
                .FirstOrDefaultAsync(e => e.EventId == id);

            if (@event == null) return View("InvalidEvent");

            PopulateEventTypesDropDownList(@event.EventTypeId);
            PopulateActivityCategories(@event.ActivityTypeId); // Passa o ID

            ViewBag.SelectedActivityIds = @event.EventActivities.Select(ea => ea.ActivityId).ToList();
            return View(@event);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventId,EventName,EventDescription,EventTypeId,ActivityTypeId,EventStart,EventEnd,MinLevel")] Event @event, int[] selectedActivities)
        {
            if (id != @event.EventId) return View("InvalidEvent");

            if (selectedActivities != null && selectedActivities.Length > 0)
            {
                @event.EventPoints = await _context.Activity
                    .Where(a => selectedActivities.Contains(a.ActivityId))
                    .SumAsync(a => a.ActivityReward);
            }
            else { @event.EventPoints = 0; }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    var oldRelations = _context.EventActivity.Where(ea => ea.EventId == id);
                    _context.EventActivity.RemoveRange(oldRelations);

                    if (selectedActivities != null)
                    {
                        foreach (var actId in selectedActivities)
                        {
                            _context.EventActivity.Add(new EventActivity { EventId = id, ActivityId = actId });
                        }
                    }
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Event updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.EventId)) return View("InvalidEvent");
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            PopulateEventTypesDropDownList(@event.EventTypeId);
            PopulateActivityCategories(@event.ActivityTypeId);
            ViewBag.SelectedActivityIds = selectedActivities.ToList();
            return View(@event);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return View("InvalidEvent");
            var @event = await _context.Event
                .Include(e => e.EventType)
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (@event == null) return View("InvalidEvent");
            return View(@event);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Event.FindAsync(id);
            if (@event == null) return View("InvalidEvent");
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