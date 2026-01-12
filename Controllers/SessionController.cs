using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels; // Necessário para PaginationInfo
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

        // =========================================================
        // 1. INDEX (LISTAGEM) com Paginação e Pesquisa
        // =========================================================
        public async Task<IActionResult> Index(int page = 1, string searchClient = "", string searchDate = "")
        {
            var query = _context.Session
                .Include(s => s.Training).ThenInclude(t => t.Trainer)
                .Include(s => s.Training).ThenInclude(t => t.TrainingType)
                .Include(s => s.Member).ThenInclude(m => m.Client)
                .AsQueryable();

            // A) Filtro de Segurança (Quem vê o quê?)
            if (User.IsInRole("Administrator") || User.IsInRole("Trainer"))
            {
                // Staff vê tudo, ordenado por data (mais recente primeiro)
                query = query.OrderByDescending(s => s.SessionDate);
            }
            else
            {
                // Cliente vê apenas as suas
                var member = await GetCurrentMemberAsync();
                if (member == null)
                {
                    TempData["Error"] = "You need a membership plan to view sessions.";
                    return RedirectToAction("PublicIndex", "Plan");
                }
                query = query.Where(s => s.MemberId == member.MemberId).OrderByDescending(s => s.SessionDate);
            }

            // B) Pesquisa
            if (!string.IsNullOrEmpty(searchClient) && (User.IsInRole("Administrator") || User.IsInRole("Trainer")))
            {
                query = query.Where(s => s.Member.Client.Name.Contains(searchClient));
            }

            if (!string.IsNullOrEmpty(searchDate) && DateTime.TryParse(searchDate, out DateTime parsedDate))
            {
                query = query.Where(s => s.SessionDate.Date == parsedDate.Date);
            }

            // C) Paginação
            int totalItems = await query.CountAsync();
            var pagination = new PaginationInfo<Session>(page, totalItems, 10);

            pagination.Items = await query
                .Skip(pagination.ItemsToSkip)
                .Take(pagination.ItemsPerPage)
                .ToListAsync();

            ViewBag.SearchClient = searchClient;
            ViewBag.SearchDate = searchDate;

            return View(pagination);
        }

        // =========================================================
        // 2. DETAILS
        // =========================================================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var session = await _context.Session
                .Include(s => s.Training).ThenInclude(t => t.Trainer)
                .Include(s => s.Training).ThenInclude(t => t.TrainingType)
                .Include(s => s.Member).ThenInclude(m => m.Client)
                .FirstOrDefaultAsync(m => m.SessionId == id);

            if (session == null) return NotFound();

            // Segurança: Cliente só vê a sua aula
            if (!User.IsInRole("Administrator") && !User.IsInRole("Trainer"))
            {
                var member = await GetCurrentMemberAsync();
                if (member == null || session.MemberId != member.MemberId) return Forbid();
            }

            return View(session);
        }

        // =========================================================
        // 3. CREATE (AGENDAMENTO)
        // =========================================================

        // GET: Create?trainingId=5 (Mostra confirmação)
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

            // Calcular próxima data disponível baseada no dia da semana da aula
            DayOfWeek targetDay = (DayOfWeek)training.DayOfWeek;
            DateTime nextDate = GetNextWeekday(DateTime.Today, targetDay);

            ViewBag.SuggestedDate = nextDate.ToString("yyyy-MM-dd");
            return View(training);
        }

        // POST: Create (Confirmação final)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int TrainingId, DateTime SessionDate)
        {
            var member = await GetCurrentMemberAsync();
            if (member == null)
            {
                TempData["Error"] = "You must be a member to book a session.";
                return RedirectToAction("PublicIndex", "Plan");
            }

            var training = await _context.Training
                .Include(t => t.TrainingType) // Necessário para MaxParticipants
                .Include(t => t.Trainer)
                .FirstOrDefaultAsync(t => t.TrainingId == TrainingId);

            if (training == null) return NotFound();

            // A) Verificar Lotação (Full Capacity)
            int currentParticipants = await _context.Session
                .CountAsync(s => s.TrainingId == TrainingId && s.SessionDate.Date == SessionDate.Date);

            if (currentParticipants >= training.TrainingType.MaxParticipants)
            {
                // Lógica de Alternativas
                var alternatives = await _context.Training
                    .Include(t => t.Trainer)
                    .Where(t => t.TrainingTypeId == training.TrainingTypeId && t.TrainingId != training.TrainingId)
                    .Take(3)
                    .ToListAsync();

                ViewBag.Alternatives = alternatives;
                return View("FullCapacity", training);
            }

            // B) Verificar Duplicados
            bool alreadyBooked = await _context.Session
                .AnyAsync(s => s.TrainingId == TrainingId && s.SessionDate.Date == SessionDate.Date && s.MemberId == member.MemberId);

            if (alreadyBooked)
            {
                TempData["Error"] = "You have already booked this session.";
                return RedirectToAction(nameof(Index));
            }

            // C) Criar Sessão
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

        // =========================================================
        // 4. EDIT (FEEDBACK E REMARCAÇÃO)
        // =========================================================

        // GET: Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var session = await _context.Session
                .Include(s => s.Training).ThenInclude(t => t.TrainingType)
                .FirstOrDefaultAsync(s => s.SessionId == id);

            if (session == null) return NotFound();

            // Segurança
            if (!User.IsInRole("Administrator") && !User.IsInRole("Trainer"))
            {
                var member = await GetCurrentMemberAsync();
                if (member == null || session.MemberId != member.MemberId) return Forbid();
            }

            return View(session);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SessionId,MemberId,TrainingId,SessionDate,MemberFeedback,Rating")] Session session)
        {
            if (id != session.SessionId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Impedir que Clientes mudem a Data (apenas Staff pode)
                    if (!User.IsInRole("Administrator") && !User.IsInRole("Trainer"))
                    {
                        var originalSession = await _context.Session.AsNoTracking().FirstOrDefaultAsync(s => s.SessionId == id);
                        if (originalSession != null)
                        {
                            session.SessionDate = originalSession.SessionDate; // Força a data original
                        }
                    }

                    _context.Update(session);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Session.Any(e => e.SessionId == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(session);
        }

        // =========================================================
        // 5. DELETE (CANCELAR)
        // =========================================================

        // GET: Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var session = await _context.Session
                .Include(s => s.Training).ThenInclude(t => t.Trainer)
                .FirstOrDefaultAsync(m => m.SessionId == id);

            if (session == null) return NotFound();

            return View(session);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var session = await _context.Session.FindAsync(id);
            if (session != null)
            {
                _context.Session.Remove(session);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Booking cancelled successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        // =========================================================
        // 6. HELPER METHODS (O QUE ESTAVA A FALTAR!)
        // =========================================================

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
            int d = ((int)day - (int)start.DayOfWeek + 7) % 7;
            return start.AddDays(d == 0 ? 7 : d);
        }
    }
}