using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Controllers {
    [Authorize(Roles = "Gestor")]
    public class EventTypesController : Controller {
        private readonly HealthWellbeingDbContext _context;

        public EventTypesController(HealthWellbeingDbContext context) {
            _context = context;
        }

        // GET: EventTypes
        public async Task<IActionResult> Index(int page = 1, string searchName = "", int? searchScoringStrategyId = null) {
            // Preparar a Query base com o Include necessário
            var eventTypesQuery = _context.EventType
                .Include(e => e.ScoringStrategy)
                .AsQueryable();

            // Aplicar filtros se existirem
            if (!string.IsNullOrEmpty(searchName)) {
                eventTypesQuery = eventTypesQuery.Where(e => e.EventTypeName.Contains(searchName));
            }

            if (searchScoringStrategyId.HasValue) {
                eventTypesQuery = eventTypesQuery.Where(e => e.ScoringStrategyId == searchScoringStrategyId);
            }

            // Guardar os valores de pesquisa na ViewBag para manter o formulário preenchido
            ViewBag.SearchName = searchName;

            // Carregar a lista de estratégias de classificação para o dropdown de pesquisa
            ViewData["ScoringStrategies"] = new SelectList(_context.Set<ScoringStrategy>(), "ScoringStrategyId", "ScoringStrategyName", searchScoringStrategyId);

            // Paginação
            int numberEventTypes = await eventTypesQuery.CountAsync();
            var eventTypesInfo = new ViewModels.PaginationInfo<EventType>(page, numberEventTypes);

            eventTypesInfo.Items = await eventTypesQuery
                .OrderBy(e => e.EventTypeName)
                .Skip(eventTypesInfo.ItemsToSkip)
                .Take(eventTypesInfo.ItemsPerPage)
                .ToListAsync();

            return View(eventTypesInfo);
        }

        // GET: EventTypes/Details/5
        public async Task<IActionResult> Details(int? id) {
            if (id == null) {
                return NotFound();
            }

            var eventType = await _context.EventType
                .Include(e => e.ScoringStrategy)
                .Include(e => e.Events)
                .FirstOrDefaultAsync(m => m.EventTypeId == id);

            if (eventType == null) {
                
                return View("InvalidEventType");
            }

            return View(eventType);
        }

        // GET: EventTypes/Create
        public IActionResult Create() {
            // Dropdown para escolher a ScoringStrategy.
            ViewData["ScoringStrategyId"] = new SelectList(_context.Set<ScoringStrategy>(), "ScoringStrategyId", "ScoringStrategyName");
            return View();
        }

        // POST: EventTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventTypeId,EventTypeName,EventTypeDescription,ScoringStrategyId,EventTypeMultiplier")] EventType eventType) {
            if (ModelState.IsValid) {
                _context.Add(eventType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details),
                    new {
                        id = eventType.EventTypeId,
                        // Feedback de sucesso
                        SuccessMessage = "Event type created successfully."
                    }
                );
            }

            // Se falhar, recarregar o dropdown
            ViewData["ScoringStrategyId"] = new SelectList(_context.Set<ScoringStrategy>(), "ScoringStrategyId", "ScoringStrategyName", eventType.ScoringStrategyId);
            return View(eventType);
        }

        // GET: EventTypes/Edit/5
        public async Task<IActionResult> Edit(int? id) {
            if (id == null) {
                return NotFound();
            }

            var eventType = await _context.EventType.FindAsync(id);
            if (eventType == null) {
                return View("InvalidEventType");
            }

            ViewData["ScoringStrategyId"] = new SelectList(_context.Set<ScoringStrategy>(), "ScoringStrategyId", "ScoringStrategyName", eventType.ScoringStrategyId);
            return View(eventType);
        }

        // POST: EventTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventTypeId,EventTypeName,EventTypeDescription,ScoringStrategyId,EventTypeMultiplier")] EventType eventType) {
            if (id != eventType.EventTypeId) {
                return NotFound();
            }

            if (ModelState.IsValid) {
                try {
                    _context.Update(eventType);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details),
                        new {
                            id = eventType.EventTypeId,
                            SuccessMessage = "Event Type updated successfully."
                        }
                    );
                }
                catch (DbUpdateConcurrencyException) {
                    if (!EventTypeExists(eventType.EventTypeId)) {
                        ViewBag.EventTypeWasDeleted = true;
                    }
                    else {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["ScoringStrategyId"] = new SelectList(_context.Set<ScoringStrategy>(), "ScoringStrategyId", "ScoringStrategyName", eventType.ScoringStrategyId);
            return View(eventType);
        }

        // GET: EventTypes/Delete/5
        public async Task<IActionResult> Delete(int? id) {
            if (id == null) {
                return NotFound();
            }

            var eventType = await _context.EventType
                .Include(e => e.ScoringStrategy)
                .Include(e => e.Events)
                .FirstOrDefaultAsync(m => m.EventTypeId == id);

            if (eventType == null) {
                TempData["ErrorMessage"] = "The Event Type is no longer available.";
                return RedirectToAction(nameof(Index));
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
                    TempData["SuccessMessage"] = "Event Type deleted successfully.";
                }
                catch (Exception) {
                    eventType = await _context.EventType
                        .AsNoTracking()
                        .Include(e => e.ScoringStrategy)
                        .Include(e => e.Events)
                        .FirstOrDefaultAsync(e => e.EventTypeId == id);

                    // Verifica se existem Eventos associados a este Tipo antes de apagar
                    int numberEvents = eventType?.Events?.Count ?? 0;

                    if (numberEvents > 0) {
                        ViewBag.Error = $"Unable to delete Event Type. It is associated with {numberEvents} Events. Remove the events first.";
                    }
                    else {
                        ViewBag.Error = "An error occurred while deleting the Event Type.";
                    }

                    return View(eventType);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool EventTypeExists(int id) {
            return _context.EventType.Any(e => e.EventTypeId == id);
        }
    }
}