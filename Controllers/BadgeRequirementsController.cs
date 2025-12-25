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
using Microsoft.AspNetCore.Authorization;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Controllers {
    [Authorize(Roles = "Gestor")]
    public class BadgeRequirementsController : Controller {
        private readonly HealthWellbeingDbContext _context;

        public BadgeRequirementsController(HealthWellbeingDbContext context) {
            _context = context;
        }

        // GET: BadgeRequirements
        public async Task<IActionResult> Index(
            int page = 1,
            string searchName = "",
            int? searchBadgeId = null,
            RequirementType? searchType = null,
            int? searchTargetValue = null) {

            var requirementsQuery = _context.BadgeRequirement
                .Include(b => b.Badge)
                .Include(b => b.EventTypes)
                .Include(b => b.ActivityTypes)
                .AsQueryable();

            // --- FILTERS ---
            if (!string.IsNullOrEmpty(searchName)) {
                requirementsQuery = requirementsQuery.Where(r => r.BadgeRequirementName.Contains(searchName));
            }

            if (searchBadgeId.HasValue) {
                requirementsQuery = requirementsQuery.Where(r => r.BadgeId == searchBadgeId);
            }

            if (searchType.HasValue) {
                requirementsQuery = requirementsQuery.Where(r => r.RequirementType == searchType.Value);
            }

            if (searchTargetValue.HasValue) {
                requirementsQuery = requirementsQuery.Where(r => r.TargetValue == searchTargetValue.Value);
            }

            // --- DATA PREPARATION FOR VIEW ---

            ViewBag.SearchName = searchName;
            ViewBag.SearchTargetValue = searchTargetValue;

            // Badges Dropdown
            ViewData["Badges"] = new SelectList(_context.Badge, "BadgeId", "BadgeName", searchBadgeId);

            // Requirement Types Dropdown
            // Convert Enum to a list for SelectList to work
            var enumList = Enum.GetValues(typeof(RequirementType))
                .Cast<RequirementType>()
                .Select(e => new {
                    Id = (int)e,
                    // Try to get [Display] name or use code name
                    Name = e.GetType().GetMember(e.ToString()).First()
                            .GetCustomAttribute<DisplayAttribute>()?.Name ?? e.ToString()
                });

            // Pass '(int?)searchType' at the end to mark selected item
            ViewBag.SearchTypes = new SelectList(enumList, "Id", "Name", (int?)searchType);


            // --- PAGINATION ---
            int numberRequirements = await requirementsQuery.CountAsync();
            var paginationInfo = new ViewModels.PaginationInfo<BadgeRequirement>(page, numberRequirements);

            paginationInfo.Items = await requirementsQuery
                .OrderBy(r => r.BadgeRequirementName)
                .ThenBy(r => r.Badge.BadgeName)
                .Skip(paginationInfo.ItemsToSkip)
                .Take(paginationInfo.ItemsPerPage)
                .ToListAsync();

            return View(paginationInfo);
        }

        // GET: BadgeRequirements/Details/5
        public async Task<IActionResult> Details(int? id) {
            if (id == null) return View("InvalidBadgeRequirement");

            var badgeRequirement = await _context.BadgeRequirement
                .Include(b => b.Badge)
                .Include(b => b.EventTypes)
                .Include(b => b.ActivityTypes)
                .FirstOrDefaultAsync(m => m.BadgeRequirementId == id);

            if (badgeRequirement == null) return View("InvalidBadgeRequirement");

            return View(badgeRequirement);
        }

        // GET: BadgeRequirements/Create
        public IActionResult Create() {
            PopulateDropdowns();
            return View();
        }

        // POST: BadgeRequirements/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BadgeRequirementId,BadgeId,BadgeRequirementName,RequirementDescription,TargetValue,RequirementType,EventTypeId,ActivityTypeId")] BadgeRequirement badgeRequirement) {
            EnsureCorrectIdsForType(badgeRequirement);

            if (ModelState.IsValid) {
                _context.Add(badgeRequirement);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new {
                    id = badgeRequirement.BadgeRequirementId,
                    SuccessMessage = "Requirement created successfully."
                });
            }

            PopulateDropdowns(badgeRequirement);
            return View(badgeRequirement);
        }

        // GET: BadgeRequirements/Edit/5
        public async Task<IActionResult> Edit(int? id) {
            if (id == null) return View("InvalidBadgeRequirement");

            var badgeRequirement = await _context.BadgeRequirement.FindAsync(id);
            if (badgeRequirement == null) return View("InvalidBadgeRequirement");

            PopulateDropdowns(badgeRequirement);
            return View(badgeRequirement);
        }

        // POST: BadgeRequirements/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BadgeRequirementId,BadgeId,BadgeRequirementName,RequirementDescription,TargetValue,RequirementType,EventTypeId,ActivityTypeId")] BadgeRequirement badgeRequirement) {
            if (id != badgeRequirement.BadgeRequirementId) return View("InvalidBadgeRequirement");

            EnsureCorrectIdsForType(badgeRequirement);

            if (ModelState.IsValid) {
                try {
                    _context.Update(badgeRequirement);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details), new {
                        id = badgeRequirement.BadgeRequirementId,
                        SuccessMessage = "Requirement updated successfully."
                    });
                }
                catch (DbUpdateConcurrencyException) {
                    if (!BadgeRequirementExists(badgeRequirement.BadgeRequirementId)) return View("InvalidBadgeRequirement");
                    else throw;
                }
            }

            PopulateDropdowns(badgeRequirement);
            return View(badgeRequirement);
        }

        // GET: BadgeRequirements/Delete/5
        public async Task<IActionResult> Delete(int? id) {
            if (id == null) return View("InvalidBadgeRequirement");

            var badgeRequirement = await _context.BadgeRequirement
                .Include(b => b.Badge)
                .Include(b => b.EventTypes)
                .Include(b => b.ActivityTypes)
                .FirstOrDefaultAsync(m => m.BadgeRequirementId == id);

            if (badgeRequirement == null) {
                TempData["ErrorMessage"] = "The Requirement is no longer available.";
                return RedirectToAction(nameof(Index));
            }

            return View(badgeRequirement);
        }

        // POST: BadgeRequirements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) {
            var badgeRequirement = await _context.BadgeRequirement
                .Include(b => b.Badge)
                .Include(b => b.EventTypes)
                .Include(b => b.ActivityTypes)
                .FirstOrDefaultAsync(m => m.BadgeRequirementId == id);

            if (badgeRequirement != null) {
                try {
                    _context.BadgeRequirement.Remove(badgeRequirement);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Requirement deleted successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException) {
                    ViewBag.Error = "Unable to delete requirement. It is likely associated with user progress or other records.";
                    return View(badgeRequirement);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        private bool BadgeRequirementExists(int id) {
            return _context.BadgeRequirement.Any(e => e.BadgeRequirementId == id);
        }

        private void PopulateDropdowns(BadgeRequirement? model = null) {
            ViewData["BadgeId"] = new SelectList(_context.Badge, "BadgeId", "BadgeName", model?.BadgeId);
            ViewData["EventTypeId"] = new SelectList(_context.EventType, "EventTypeId", "EventTypeName", model?.EventTypeId);
            ViewData["ActivityTypeId"] = new SelectList(_context.ActivityType, "ActivityTypeId", "Name", model?.ActivityTypeId);
        }

        private void EnsureCorrectIdsForType(BadgeRequirement model) {
            if (model.RequirementType == RequirementType.ParticipateSpecificEventType) {
                model.ActivityTypeId = null;
                if (model.EventTypeId == null) ModelState.AddModelError("EventTypeId", "Event Type required.");
            }
            else if (model.RequirementType == RequirementType.CompleteSpecificActivityType) {
                model.EventTypeId = null;
                if (model.ActivityTypeId == null) ModelState.AddModelError("ActivityTypeId", "Activity Type required.");
            }
            else {
                model.EventTypeId = null;
                model.ActivityTypeId = null;
            }
        }
    }
}