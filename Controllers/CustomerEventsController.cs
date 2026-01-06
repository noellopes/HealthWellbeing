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

namespace HealthWellbeing.Controllers
{
    [Authorize(Roles = "Utente")]
    public class CustomerEventsController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public CustomerEventsController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Index (Lista de Eventos)
        public async Task<IActionResult> Index(string searchName, int? searchType, int? searchLevel, string searchStatus, int page = 1)
        {
            var userEmail = User.Identity?.Name;
            var customer = await _context.Customer
                .Include(c => c.Level)
                .FirstOrDefaultAsync(c => c.Email == userEmail);

            if (customer == null) return View("Error");

            // --- PROCESSAR EXPIRADOS E VERIFICAR NÍVEL ---
            await ProcessExpiredEvents(customer.CustomerId);
            // ---------------------------------------------

            ViewBag.CustomerLevel = customer.Level?.LevelNumber ?? 0;
            ViewBag.CustomerId = customer.CustomerId;
            ViewBag.SearchName = searchName;
            ViewBag.SearchType = searchType;
            ViewBag.SearchLevel = searchLevel;
            ViewBag.SearchStatus = searchStatus;

            ViewBag.EventTypesList = new SelectList(_context.EventType.OrderBy(t => t.EventTypeName), "EventTypeId", "EventTypeName", searchType);
            ViewBag.StatusList = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "All Statuses" },
                new SelectListItem { Value = "enrolled", Text = "My Enrollments" },
                new SelectListItem { Value = "not_enrolled", Text = "Open for Enrollment" }
            };

            // Lista de IDs já completados para esconder
            var completedEventIds = await _context.CustomerEvent
                .Where(ce => ce.CustomerId == customer.CustomerId && ce.Status == "Completed")
                .Select(ce => ce.EventId)
                .ToListAsync();

            var now = DateTime.Now;
            var eventsQuery = _context.Event
                .Include(e => e.EventType)
                .Where(e => e.EventEnd >= now) // Ativos
                .Where(e => !completedEventIds.Contains(e.EventId)) // Não completados
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchName)) eventsQuery = eventsQuery.Where(e => e.EventName.Contains(searchName));
            if (searchType.HasValue) eventsQuery = eventsQuery.Where(e => e.EventTypeId == searchType);
            if (searchLevel.HasValue) eventsQuery = eventsQuery.Where(e => e.MinLevel == searchLevel);

            var myEnrolledEventIds = await _context.CustomerEvent
                .Where(ue => ue.CustomerId == customer.CustomerId)
                .Select(ue => ue.EventId)
                .ToListAsync();

            if (!string.IsNullOrEmpty(searchStatus))
            {
                if (searchStatus == "enrolled") eventsQuery = eventsQuery.Where(e => myEnrolledEventIds.Contains(e.EventId));
                else if (searchStatus == "not_enrolled") eventsQuery = eventsQuery.Where(e => !myEnrolledEventIds.Contains(e.EventId));
            }

            eventsQuery = eventsQuery.OrderBy(e => e.MinLevel).ThenBy(e => e.EventStart);

            int totalItems = await eventsQuery.CountAsync();
            int pageSize = 6;
            var paginationInfo = new ViewModels.PaginationInfo<Event>(page, totalItems, pageSize);

            paginationInfo.Items = await eventsQuery
                .Skip(paginationInfo.ItemsToSkip)
                .Take(paginationInfo.ItemsPerPage)
                .ToListAsync();

            ViewBag.MyEnrolledIds = myEnrolledEventIds;

            return View(paginationInfo);
        }

        // GET: History (Passados ou Completados)
        public async Task<IActionResult> History()
        {
            var userEmail = User.Identity?.Name;
            var customer = await _context.Customer.FirstOrDefaultAsync(c => c.Email == userEmail);

            if (customer == null) return View("Error");

            var now = DateTime.Now;

            // Histórico: Data passada OU status Completado
            var historyLogs = await _context.CustomerEvent
                .Include(ce => ce.Event)
                    .ThenInclude(e => e.EventType)
                .Where(ce => ce.CustomerId == customer.CustomerId)
                .Where(ce => ce.Event.EventEnd < now || ce.Status == "Completed")
                .OrderByDescending(ce => ce.Event.EventStart)
                .ToListAsync();

            return View(historyLogs);
        }

        // POST: Enroll
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(int eventId)
        {
            var userEmail = User.Identity?.Name;
            var customer = await _context.Customer.Include(c => c.Level).FirstOrDefaultAsync(c => c.Email == userEmail);
            var eventToEnroll = await _context.Event.FindAsync(eventId);

            if (customer == null || eventToEnroll == null) return NotFound();

            int userLevel = customer.Level?.LevelNumber ?? 0;
            if (userLevel < eventToEnroll.MinLevel)
            {
                TempData["EnrollmentError"] = "You need a higher level to join this event.";
                return RedirectToAction(nameof(Index));
            }

            bool alreadyEnrolled = await _context.CustomerEvent
                .AnyAsync(ue => ue.EventId == eventId && ue.CustomerId == customer.CustomerId);

            if (alreadyEnrolled)
            {
                TempData["EnrollmentInfo"] = "You are already registered for this event.";
                return RedirectToAction(nameof(Index));
            }

            var newRegistration = new CustomerEvent
            {
                EventId = eventId,
                CustomerId = customer.CustomerId,
                RegistrationDate = DateTime.Now,
                Status = "Enrolled",
                PointsEarned = 0
            };

            _context.CustomerEvent.Add(newRegistration);
            await _context.SaveChangesAsync();

            TempData["EnrollmentSuccess"] = "Registration successful! Good luck.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Event
                .Include(e => e.EventType)
                .Include(e => e.EventActivities).ThenInclude(ea => ea.Activity)
                .FirstOrDefaultAsync(m => m.EventId == id);

            if (@event == null) return NotFound();

            var userEmail = User.Identity?.Name;
            var customer = await _context.Customer.FirstOrDefaultAsync(c => c.Email == userEmail);

            bool isEnrolled = false;
            List<int> completedActivityIds = new List<int>();

            if (customer != null)
            {
                isEnrolled = await _context.CustomerEvent
                   .AnyAsync(ue => ue.EventId == id && ue.CustomerId == customer.CustomerId);

                if (isEnrolled)
                {
                    completedActivityIds = await _context.CustomerActivity
                        .Where(log => log.CustomerId == customer.CustomerId &&
                                      log.CompletionDate >= @event.EventStart &&
                                      log.CompletionDate <= @event.EventEnd)
                        .Select(log => log.ActivityId)
                        .ToListAsync();
                }
            }

            ViewBag.IsEnrolled = isEnrolled;
            ViewBag.CustomerLevel = customer?.Level?.LevelNumber ?? 0;
            ViewBag.CompletedActivityIds = completedActivityIds;

            return View(@event);
        }

        // --- MÉTODO AUXILIAR: Pagar Pontos Parciais + Check Level Up ---
        private async Task ProcessExpiredEvents(int customerId)
        {
            var now = DateTime.Now;

            // 1. Inscrições 'Enrolled' (não terminadas) que expiraram
            var expiredEnrollments = await _context.CustomerEvent
                .Include(ce => ce.Event).ThenInclude(e => e.EventType)
                .Where(ce => ce.CustomerId == customerId
                       && ce.Status == "Enrolled"
                       && ce.Event.EventEnd < now)
                .ToListAsync();

            if (!expiredEnrollments.Any()) return;

            // Carrega customer com nível para atualizar
            var customer = await _context.Customer
                .Include(c => c.Level)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null) return;

            foreach (var enrollment in expiredEnrollments)
            {
                // Calcular pontos parciais
                var activitiesDoneInEvent = await _context.CustomerActivity
                    .Include(ca => ca.Activity)
                    .Where(ca => ca.CustomerId == customerId && ca.EventId == enrollment.EventId)
                    .ToListAsync();

                int sumPoints = activitiesDoneInEvent.Sum(ca => ca.Activity.ActivityReward);
                decimal multiplier = enrollment.Event.EventType.EventTypeMultiplier;
                int partialPoints = (int)(sumPoints * multiplier);

                customer.TotalPoints += partialPoints;

                enrollment.Status = "Expired (Partial)";
                enrollment.PointsEarned = partialPoints;
                enrollment.CompletionDate = enrollment.Event.EventEnd;

                TempData["InfoMessage"] = $"Event '{enrollment.Event.EventName}' ended! You received {partialPoints} partial points (x{multiplier}).";
            }

            // --- CHECK LEVEL UP ---
            await CheckLevelUp(customer);
            // ----------------------

            await _context.SaveChangesAsync();
        }

        // --- MÉTODO AUXILIAR DE LEVEL UP (VERSÃO CORRIGIDA) ---
        private async Task CheckLevelUp(Customer customer)
        {
            // 1. Encontrar o nível correto baseado nos pontos atuais
            // Ordenamos DESCENDENTE para apanhar o nível mais alto que os pontos permitem
            var correctLevel = await _context.Level
                .OrderByDescending(l => l.LevelPointsLimit)
                .FirstOrDefaultAsync(l => customer.TotalPoints >= l.LevelPointsLimit);

            // Se não encontrar (ex: 0 pontos), assume o nível mais baixo
            if (correctLevel == null)
            {
                correctLevel = await _context.Level.OrderBy(l => l.LevelPointsLimit).FirstOrDefaultAsync();
            }

            if (correctLevel == null) return; // Se não houver níveis na BD, sai.

            // 2. Comparação Robusta
            // Compara o ID do nível atual do cliente com o ID do nível que devia ter
            int currentLevelId = customer.Level?.LevelId ?? 0;

            if (correctLevel.LevelId != currentLevelId)
            {
                // 3. Atualizar Dados
                customer.Level = correctLevel;

                // FORÇAR o Entity Framework a marcar este cliente como "Modificado"
                _context.Update(customer);

                // 4. Mostrar mensagem
                TempData["LevelUpMessage"] = $"🎉 LEVEL UP! You reached Level {correctLevel.LevelNumber}!";
            }
        }
    }
}