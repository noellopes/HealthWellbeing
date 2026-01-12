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

        // =============================================================
        // GET: Sessions (A Minha Agenda)
        // =============================================================
        public async Task<IActionResult> Index()
        {
            var member = await GetCurrentMemberAsync();
            if (member == null) return RedirectToAction("Register", "Account");

            var sessions = _context.Session
                .Include(s => s.Training)
                .Where(s => s.MemberId == member.MemberId)
                .OrderBy(s => s.SessionDate);

            return View(await sessions.ToListAsync());
        }

        // =============================================================
        // PASSO 1 & 2: Confirmar Agendamento
        // GET: Sessions/Create?trainingId=5
        // =============================================================
        public async Task<IActionResult> Create(int? trainingId)
        {
            if (trainingId == null) return NotFound();

            var training = await _context.Training
                .Include(t => t.TrainingType)
                .Include(t => t.Trainer)
                .FirstOrDefaultAsync(m => m.TrainingId == trainingId);

            if (training == null) return NotFound();

            // Sugere a próxima data válida para este treino
            DateTime nextDate = GetNextWeekday(DateTime.Today, training.DayOfWeek);

            ViewBag.SuggestedDate = nextDate.ToString("yyyy-MM-dd");
            ViewBag.DayName = training.DayOfWeek.ToString();

            return View(training);
        }

        // =============================================================
        // PASSO 3, 4 & 5: Verificar e Gravar
        // POST: Sessions/Create
        // =============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int TrainingId, DateTime SessionDate)
        {
            var member = await GetCurrentMemberAsync();
            if (member == null) return Forbid();

            var training = await _context.Training
                .Include(t => t.TrainingType)
                .FirstOrDefaultAsync(t => t.TrainingId == TrainingId);

            if (training == null) return NotFound();

            // 1. Verificação de Ocupação (Lotação)
            int currentParticipants = await _context.Session
                .CountAsync(s => s.TrainingId == TrainingId && s.SessionDate.Date == SessionDate.Date);

            // 2. Verifica se já está inscrito
            bool alreadyBooked = await _context.Session
                .AnyAsync(s => s.TrainingId == TrainingId && s.SessionDate.Date == SessionDate.Date && s.MemberId == member.MemberId);

            if (alreadyBooked)
            {
                TempData["Error"] = "Já está inscrito nesta sessão.";
                return RedirectToAction(nameof(Index));
            }

            // 3. Decisão: Lotação Esgotada
            if (currentParticipants >= training.TrainingType.MaxParticipants)
            {
                // Procura alternativas do mesmo tipo
                var alternatives = await _context.Training
                    .Where(t => t.TrainingTypeId == training.TrainingTypeId && t.TrainingId != training.TrainingId)
                    .ToListAsync();

                ViewBag.ErrorMessage = "Lotação Esgotada!";
                ViewBag.Alternatives = alternatives;

                return View("FullCapacity", training);
            }

            // 4. Decisão: Disponível (Gravar)
            var session = new Session
            {
                TrainingId = TrainingId,
                MemberId = member.MemberId,
                SessionDate = SessionDate.Date + training.StartTime, // Junta Data e Hora
                Rating = null
            };

            _context.Add(session);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Sessão agendada com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // =============================================================
        // CANCELAR SESSÃO
        // =============================================================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var session = await _context.Session.FindAsync(id);
            if (session != null)
            {
                _context.Session.Remove(session);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // --- Helpers ---

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