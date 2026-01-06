using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.ViewModels;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers {
    [Authorize]
    public class MyBadgesController : Controller {
        private readonly HealthWellbeingDbContext _context;

        public MyBadgesController(HealthWellbeingDbContext context) {
            _context = context;
        }

        public async Task<IActionResult> Index() {
            var userEmail = User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail)) return RedirectToAction("Login", "Account");

            // Get Customer (ID only, Read-only for performance)
            var customer = await _context.Customer
                .AsNoTracking()
                .Select(c => new { c.Email, c.CustomerId })
                .FirstOrDefaultAsync(c => c.Email == userEmail);

            if (customer == null) {
                TempData["ErrorMessage"] = "Customer profile not found.";
                return RedirectToAction("Index", "Home");
            }

            // Get Earned Badges (Dictionary for O(1) lookup performance)
            var earnedBadgesDict = await _context.CustomerBadge
                .AsNoTracking()
                .Where(cb => cb.CustomerId == customer.CustomerId)
                .ToDictionaryAsync(cb => cb.BadgeId, cb => cb.DateAwarded);

            // Get Full Badge Catalog (Including Types)
            var badges = await _context.Badge
                .Include(b => b.BadgeType)
                .OrderBy(b => b.BadgeType.BadgeTypeName)
                .ThenBy(b => b.RewardPoints)
                .AsNoTracking()
                .ToListAsync();

            // Map to ViewModel
            var model = badges.Select(b => new MyBadge {
                Badge = b,
                IsEarned = earnedBadgesDict.ContainsKey(b.BadgeId),
                DateAwarded = earnedBadgesDict.ContainsKey(b.BadgeId) ? earnedBadgesDict[b.BadgeId] : null
            }).ToList();

            return View(model);
        }

        public async Task<IActionResult> Details(int? id) {
            if (id == null) return NotFound();

            var userEmail = User.Identity?.Name;

            // Validate Customer existence
            var customer = await _context.Customer
                .Select(c => new { c.Email, c.CustomerId })
                .FirstOrDefaultAsync(c => c.Email == userEmail);

            if (customer == null) return RedirectToAction("Index", "Home");

            // Load Badge and specific Requirements
            var badgeEntity = await _context.Badge
                .Include(b => b.BadgeType)
                .Include(b => b.BadgeRequirements)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.BadgeId == id);

            if (badgeEntity == null) return NotFound();

            // Check if already earned
            var earnedInfo = await _context.CustomerBadge
                .Where(cb => cb.CustomerId == customer.CustomerId && cb.BadgeId == id)
                .Select(cb => cb.DateAwarded)
                .FirstOrDefaultAsync();

            bool isEarned = earnedInfo != default(DateTime);

            // Progress Calculation Logic
            var progressDict = new Dictionary<int, int>();

            if (badgeEntity.BadgeRequirements != null) {
                foreach (var req in badgeEntity.BadgeRequirements) {
                    // If earned, we assume 100% completion
                    if (isEarned) {
                        progressDict[req.BadgeRequirementId] = req.TargetValue;
                    }
                    else {
                        // Calculate real-time progress from DB
                        progressDict[req.BadgeRequirementId] = await CalculateRealProgressAsync(customer.CustomerId, req);
                    }
                }
            }

            // Initialize ViewModel
            var model = new MyBadge {
                Badge = badgeEntity,
                IsEarned = isEarned,
                DateAwarded = isEarned ? earnedInfo : null,
                RequirementsProgress = progressDict
            };

            return View(model);
        }

        private async Task<int> CalculateRealProgressAsync(int customerId, BadgeRequirement req) {
            // Switch logic based on RequirementType Enum
            switch (req.RequirementType) {
                // CASE 1: Participate in ANY Event
                // Validation: Must be 'Completed' to count
                case RequirementType.ParticipateAnyEvent:
                    return await _context.CustomerEvent
                        .CountAsync(ce => ce.CustomerId == customerId
                                       && ce.Status == "Completed");

                // CASE 2: Participate in SPECIFIC Event Type
                // Validation: Must be 'Completed' AND match EventTypeId
                case RequirementType.ParticipateSpecificEventType:
                    if (req.EventTypeId == null) return 0;

                    return await _context.CustomerEvent
                        .Include(ce => ce.Event)
                        .CountAsync(ce => ce.CustomerId == customerId
                                       && ce.Status == "Completed"
                                       && ce.Event.EventTypeId == req.EventTypeId);

                // CASE 3: Complete ANY Activity
                // No status check needed for activities (existence implies completion)
                case RequirementType.CompleteAnyActivity:
                    return await _context.CustomerActivity
                        .CountAsync(ca => ca.CustomerId == customerId);

                // CASE 4: Complete SPECIFIC Activity Type
                case RequirementType.CompleteSpecificActivityType:
                    if (req.ActivityTypeId == null) return 0;

                    return await _context.CustomerActivity
                        .Include(ca => ca.Activity)
                        .CountAsync(ca => ca.CustomerId == customerId
                                       && ca.Activity.ActivityTypeId == req.ActivityTypeId);

                default:
                    return 0;
            }
        }
    }
}