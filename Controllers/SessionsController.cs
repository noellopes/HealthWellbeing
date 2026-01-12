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

        // GET: Sessions (Minha Agenda)
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

        // GET: Sessions/Book/5 (ID do Treino)
        public async Task<IActionResult> Create(int? trainingId)
        {
            if (trainingId == null) return NotFound();

            var training = await _context.Training
                .Include(t => t.TrainingType)
                .Include(t => t.Trainer)
                .FirstOrDefaultAsync(m => m.TrainingId == trainingId);

            if (training == null) return NotFound();

            // CORREÇÃO 1: Conversão do Enum WeekDay para System.DayOfWeek
            DayOfWeek targetDay = (DayOfWeek)training.DayOfWeek;

            // Calcula a próxima data válida
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

            // Precisamos do TrainingType para ver o MaxParticipants
            var training = await _context.Training
                .Include(t => t.TrainingType)
                .FirstOrDefaultAsync(t => t.TrainingId == TrainingId);

            if (training == null) return NotFound();

            // 1. VERIFICAR SE JÁ ESTÁ INSCRITO
            bool alreadyBooked = await _context.Session
                .AnyAsync(s => s.TrainingId == TrainingId && s.SessionDate.Date == SessionDate.Date && s.MemberId == member.MemberId);

            if (alreadyBooked)
            {
                TempData["Error"] = "You are already booked for this session.";
                return RedirectToAction(nameof(Index));
            }

            // 2. VERIFICAR CAPACIDADE (CORREÇÃO AQUI)
            // Conta quantas pessoas vão a esta aula neste dia
            int currentParticipants = await _context.Session
                .CountAsync(s => s.TrainingId == TrainingId && s.SessionDate.Date == SessionDate.Date);

            // CORREÇÃO: Acessa MaxParticipants através de TrainingType
            if (currentParticipants >= training.TrainingType.MaxParticipants)
            {
                TempData["Error"] = $"Class is full! Max capacity is {training.TrainingType.MaxParticipants}.";
                return RedirectToAction("Index", "Training");
            }

            // 3. CRIAR AGENDAMENTO
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

            return await _context.Member
                .Include(m => m.Client)
                .FirstOrDefaultAsync(m => m.Client.Email == user.Email);
        }

        private DateTime GetNextWeekday(DateTime start, DayOfWeek day)
        {
            int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;
            if (daysToAdd == 0) daysToAdd = 7;
            return start.AddDays(daysToAdd);
        }
    }
}