using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new DashboardViewModel
            {
                TotalPatients = await _context.Patients.CountAsync(p => p.IsActive),
                TotalProfessionals = await _context.MentalHealthProfessionals.CountAsync(p => p.IsActive),
                UpcomingSessions = await _context.TherapySessions
                    .Include(s => s.Patient)
                    .Include(s => s.Professional)
                    .Where(s => s.Status == SessionStatus.Scheduled && s.ScheduledDateTime > DateTime.Now)
                    .OrderBy(s => s.ScheduledDateTime)
                    .Take(5)
                    .ToListAsync(),
                RecentMoodEntries = await _context.MoodEntries
                    .Include(m => m.Patient)
                    .OrderByDescending(m => m.EntryDateTime)
                    .Take(10)
                    .ToListAsync(),
                ActiveCrisisAlerts = await _context.CrisisAlerts
                    .Include(c => c.Patient)
                    .Where(c => !c.IsResolved)
                    .OrderByDescending(c => c.AlertDateTime)
                    .ToListAsync(),
                TodaysSessions = await _context.TherapySessions
                    .Include(s => s.Patient)
                    .Include(s => s.Professional)
                    .Where(s => s.ScheduledDateTime.Date == DateTime.Today && s.Status == SessionStatus.Scheduled)
                    .OrderBy(s => s.ScheduledDateTime)
                    .ToListAsync()
            };

            return View(viewModel);
        }
    }
}