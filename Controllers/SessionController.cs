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
    public class SessionController : Controller
    {
        private readonly HealthWellbeingDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public SessionController(HealthWellbeingDbContext context, UserManager<IdentityUser> userManager)
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

        // GET: Sessions/Create?trainingId=5
        public async Task<IActionResult> Create(int? trainingId)
        {
            if (trainingId == null)
            {
                TempData["Error"] = "No training selected.";
                return RedirectToAction("Index", "Training");
            }

            var training = await _context.Training
                .Include(t => t.Trainer)
                .Include(t => t.TrainingType)
                .FirstOrDefaultAsync(m => m.TrainingId == trainingId);

            if (training == null) return NotFound();

            // Passamos o treino para a View para mostrar os detalhes (nome, hora, etc)
            return View(training);
        }

        // POST: Sessions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateConfirmed(int TrainingId, DateTime SessionDate)
        {
            var member = await GetCurrentMemberAsync();
            if (member == null)
            {
                TempData["Error"] = "You must be a member to book a session.";
                return RedirectToAction("PublicIndex", "Plan");
            }

            // Criar o agendamento
            var session = new Session
            {
                TrainingId = TrainingId,
                MemberId = member.MemberId,
                SessionDate = SessionDate,
                Rating = null
            };

            _context.Add(session);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Session booked successfully!";
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