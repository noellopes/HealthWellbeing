using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using System.Data;

namespace HealthWellbeing.Controllers {
    public class EventTypesController : Controller {
        private readonly HealthWellbeingDbContext _context;

        public EventTypesController(HealthWellbeingDbContext context) {
            _context = context;
        }

        // GET: EventTypes
        public async Task<IActionResult> Index(string searchName, string searchScoringMode, int page = 1) {
            ViewBag.SearchName = searchName;
            ViewBag.SearchScoringMode = searchScoringMode;

            
            var scoringModeList = new List<SelectListItem>
            {
                new SelectListItem { Value = "fixed", Text = "Fixo" },
                new SelectListItem { Value = "progressive", Text = "Progressivo" },
                new SelectListItem { Value = "time_based", Text = "Baseado em Tempo" },
                new SelectListItem { Value = "completion", Text = "Conclusão" },
                new SelectListItem { Value = "binary", Text = "Binário" }
            };

            
            var selectedScoringMode = scoringModeList.FirstOrDefault(s => s.Value == searchScoringMode);
            if (selectedScoringMode != null) {
                selectedScoringMode.Selected = true;
            }
            ViewBag.ScoringModeList = scoringModeList;

            var eventTypes = from et in _context.EventType select et;

            if (!string.IsNullOrEmpty(searchName)) {
                eventTypes = eventTypes.Where(et => et.EventTypeName.Contains(searchName));
            }

            if (!string.IsNullOrEmpty(searchScoringMode)) {
                eventTypes = eventTypes.Where(et => et.EventTypeScoringMode == searchScoringMode);
            }

            int pageSize = 5;
            int totalItems = await eventTypes.CountAsync();

            var pagedEventTypes = await eventTypes
                                    .OrderBy(et => et.EventTypeName)
                                    .Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync();

            var paginationInfo = new PaginationInfoGroup8<EventType>(pagedEventTypes, totalItems, pageSize, page);

            return View(paginationInfo);
        }

        // GET: EventTypes/Details/5
        public async Task<IActionResult> Details(int? id) {
            if (id == null)
                return NotFound();

            var eventType = await _context.EventType.FirstOrDefaultAsync(m => m.EventTypeId == id);
            if (eventType == null)
                return View("InvalidEventType");

            return View(eventType);
        }

        // GET: EventTypes/Create
        public IActionResult Create() {
            
            ViewBag.ScoringModes = new List<SelectListItem>
            {
                new SelectListItem { Value = "fixed", Text = "Fixo" },
                new SelectListItem { Value = "progressive", Text = "Progressivo" },
                new SelectListItem { Value = "time_based", Text = "Baseado em Tempo" },
                new SelectListItem { Value = "completion", Text = "Conclusão" },
                new SelectListItem { Value = "binary", Text = "Binário" }
            };

            return View();
        }

        // POST: EventTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventTypeId,EventTypeName,EventTypeScoringMode,EventTypeMultiplier")] EventType eventType) {
            if (ModelState.IsValid) {
                _context.Add(eventType);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event type created successfully!";
                return RedirectToAction(nameof(Index));
            }

            
            ViewBag.ScoringModes = new List<SelectListItem>
            {
                new SelectListItem { Value = "fixed", Text = "Fixo" },
                new SelectListItem { Value = "progressive", Text = "Progressivo" },
                new SelectListItem { Value = "time_based", Text = "Baseado em Tempo" },
                new SelectListItem { Value = "completion", Text = "Conclusão" },
                new SelectListItem { Value = "binary", Text = "Binário" }
            };

            return View(eventType);
        }

        // GET: EventTypes/Edit/5
        public async Task<IActionResult> Edit(int? id) {
            if (id == null)
                return NotFound();

            var eventType = await _context.EventType.FindAsync(id);
            if (eventType == null)
                return View("InvalidEventType");

            
            ViewBag.ScoringModes = new List<SelectListItem>
            {
                new SelectListItem { Value = "fixed", Text = "Fixo", Selected = eventType.EventTypeScoringMode == "fixed" },
                new SelectListItem { Value = "progressive", Text = "Progressivo", Selected = eventType.EventTypeScoringMode == "progressive" },
                new SelectListItem { Value = "time_based", Text = "Baseado em Tempo", Selected = eventType.EventTypeScoringMode == "time_based" },
                new SelectListItem { Value = "completion", Text = "Conclusão", Selected = eventType.EventTypeScoringMode == "completion" },
                new SelectListItem { Value = "binary", Text = "Binário", Selected = eventType.EventTypeScoringMode == "binary" }
            };

            return View(eventType);
        }

        // POST: EventTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventTypeId,EventTypeName,EventTypeScoringMode,EventTypeMultiplier")] EventType eventType) {
            if (id != eventType.EventTypeId)
                return NotFound();

            if (ModelState.IsValid) {
                try {
                    _context.Update(eventType);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Event type updated successfully!";
                }
                catch (DbUpdateConcurrencyException) {
                    if (!EventTypeExists(eventType.EventTypeId)) {
                        return NotFound();
                    }
                    else {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            
            ViewBag.ScoringModes = new List<SelectListItem>
            {
                new SelectListItem { Value = "fixed", Text = "Fixo", Selected = eventType.EventTypeScoringMode == "fixed" },
                new SelectListItem { Value = "progressive", Text = "Progressivo", Selected = eventType.EventTypeScoringMode == "progressive" },
                new SelectListItem { Value = "time_based", Text = "Baseado em Tempo", Selected = eventType.EventTypeScoringMode == "time_based" },
                new SelectListItem { Value = "completion", Text = "Conclusão", Selected = eventType.EventTypeScoringMode == "completion" },
                new SelectListItem { Value = "binary", Text = "Binário", Selected = eventType.EventTypeScoringMode == "binary" }
            };

            return View(eventType);
        }

        // GET: EventTypes/Delete/5
        public async Task<IActionResult> Delete(int? id) {
            if (id == null)
                return NotFound();

            var eventType = await _context.EventType.FirstOrDefaultAsync(m => m.EventTypeId == id);
            if (eventType == null) {
                TempData["ErrorMessage"] = "This Event Type is no longer available, as it has already been removed.";
                return NotFound();
            }
                

            return View(eventType);
        }

        // POST: EventTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var eventType = await _context.EventType.FindAsync(id);

            if (eventType != null) {
                try {
                    _context.EventType.Remove(eventType);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Event type deleted successfully!";
                }
                catch(Exception) {
                    int numberEvents = await _context.Event.Where(e => e.EventTypeId == id).CountAsync();

                    if (numberEvents > 0) {
                        ViewBag.Error = $"Unable to delete Event Type. This Event Type is associated with {numberEvents} Events. To proceed with deletion, please remove all Events linked to this Event Type first.";
                    }
                    else {
                        ViewBag.Error = "Could not delete this Event Type. An error occurred while deleting the Event Type.";
                    }
                    return View("Delete", eventType);

                }
            }
            return RedirectToAction(nameof(Index));

        }

        private bool EventTypeExists(int id) {
            return _context.EventType.Any(e => e.EventTypeId == id);
        }
    }
}