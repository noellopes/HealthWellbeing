using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace HealthWellbeing.Controllers
{
    [Authorize]
    public class SessionsController : Controller
    {
        private readonly HealthWellbeingDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public SessionsController(HealthWellbeingDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Sessions
        public async Task<IActionResult> Index()
        {
            var member = await GetCurrentMemberAsync();
            if (member == null) return RedirectToAction("Index", "Home");

            var sessions = _context.Session
                .Include(s => s.Training)
                .Where(s => s.MemberId == member.MemberId)
                .OrderBy(s => s.SessionDate);

            return View(await sessions.ToListAsync());
        }

        // GET: Sessions/Book
        public async Task<IActionResult> Create(int? trainingId)
        {
            if (trainingId == null) return NotFound();
            var training = await _context.Training
                .Include(t => t.TrainingType).Include(t => t.Trainer)
                .FirstOrDefaultAsync(m => m.TrainingId == trainingId);
            if (training == null) return NotFound();

            DayOfWeek targetDay = (DayOfWeek)training.DayOfWeek;
            DateTime nextDate = GetNextWeekday(DateTime.Today, targetDay);

            ViewBag.SuggestedDate = nextDate.ToString("yyyy-MM-dd");
            ViewBag.DayName = training.DayOfWeek.ToString();
            return View(training);
        }

        // POST: Sessions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int TrainingId, DateTime SessionDate)
        {
            var member = await GetCurrentMemberAsync();
            if (member == null) return Forbid();

            var training = await _context.Training
                .Include(t => t.TrainingType) // Importante para MaxParticipants
                .FirstOrDefaultAsync(t => t.TrainingId == TrainingId);
            if (training == null) return NotFound();

            bool alreadyBooked = await _context.Session
                .AnyAsync(s => s.TrainingId == TrainingId && s.SessionDate.Date == SessionDate.Date && s.MemberId == member.MemberId);
            if (alreadyBooked) { TempData["Error"] = "Already booked."; return RedirectToAction(nameof(Index)); }

            int currentParticipants = await _context.Session
                .CountAsync(s => s.TrainingId == TrainingId && s.SessionDate.Date == SessionDate.Date);

            // CORREÇÃO: Usar TrainingType.MaxParticipants
            if (currentParticipants >= training.TrainingType.MaxParticipants)
            {
                TempData["Error"] = "Class is full.";
                return RedirectToAction("Index", "Training");
            }

            var session = new Session { TrainingId = TrainingId, MemberId = member.MemberId, SessionDate = SessionDate, Rating = null };
            _context.Add(session);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<Member?> GetCurrentMemberAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return null;
            return await _context.Member.Include(m => m.Client).FirstOrDefaultAsync(m => m.Client.Email == user.Email);
        }

        private DateTime GetNextWeekday(DateTime start, DayOfWeek day)
        {
            int d = ((int)day - (int)start.DayOfWeek + 7) % 7;
            return start.AddDays(d == 0 ? 7 : d);
        }
    }
}