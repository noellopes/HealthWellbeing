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

namespace HealthWellbeing.Controllers {
    [Authorize(Roles = "Utente")]
    public class CustomerEventController : Controller {
        private readonly HealthWellbeingDbContext _context;

        public CustomerEventController(HealthWellbeingDbContext context) {
            _context = context;
        }

        // GET: UtenteEvents
        public async Task<IActionResult> Index(string searchName, int? searchType, int? searchLevel, string searchStatus, int page = 1) {
            var userEmail = User.Identity?.Name;

            // 1. Carregar dados do Utente (com Nível)
            var customer = await _context.Customer
                .Include(c => c.Level)
                .FirstOrDefaultAsync(c => c.Email == userEmail);

            if (customer == null) return View("Error");

            // Setup da ViewBag para a View saber quem é o user
            ViewBag.CustomerLevel = customer.Level?.LevelNumber ?? 0;
            ViewBag.CustomerId = customer.CustomerId;

            // Manter os filtros na UI
            ViewBag.SearchName = searchName;
            ViewBag.SearchType = searchType;
            ViewBag.SearchLevel = searchLevel;
            ViewBag.SearchStatus = searchStatus;

            // 2. Dropdowns
            ViewBag.EventTypesList = new SelectList(_context.EventType.OrderBy(t => t.EventTypeName), "EventTypeId", "EventTypeName", searchType);

            // Lista estática de estados para filtro de visualização
            ViewBag.StatusList = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "All Statuses" },
                new SelectListItem { Value = "enrolled", Text = "My Enrollments" },
                new SelectListItem { Value = "not_enrolled", Text = "Open for Enrollment" }
            };

            // 3. Construção da Query (Apenas eventos futuros)
            var now = DateTime.Now;
            var eventsQuery = _context.Event
                .Include(e => e.EventType)
                .Where(e => e.EventEnd >= now) // Apenas eventos que ainda não acabaram
                .AsQueryable();

            // 4. Filtros
            if (!string.IsNullOrEmpty(searchName))
                eventsQuery = eventsQuery.Where(e => e.EventName.Contains(searchName));

            if (searchType.HasValue)
                eventsQuery = eventsQuery.Where(e => e.EventTypeId == searchType);

            if (searchLevel.HasValue)
                eventsQuery = eventsQuery.Where(e => e.MinLevel == searchLevel);

            // 5. Lógica complexa: Filtrar por "Os meus eventos" vs "Novos eventos"
            // Primeiro, sacamos os IDs onde EU estou inscrito
            var myEnrolledEventIds = await _context.CustomerEvent
                .Where(ue => ue.CustomerId == customer.CustomerId)
                .Select(ue => ue.EventId)
                .ToListAsync();

            if (!string.IsNullOrEmpty(searchStatus)) {
                if (searchStatus == "enrolled")
                    eventsQuery = eventsQuery.Where(e => myEnrolledEventIds.Contains(e.EventId));
                else if (searchStatus == "not_enrolled")
                    eventsQuery = eventsQuery.Where(e => !myEnrolledEventIds.Contains(e.EventId));
            }

            // 6. Paginação e Ordenação
            // Ordena por: Nível necessário (crescente) e depois Data (próximos primeiro)
            eventsQuery = eventsQuery.OrderBy(e => e.MinLevel).ThenBy(e => e.EventStart);

            int totalItems = await eventsQuery.CountAsync();
            int pageSize = 6;
            var paginationInfo = new ViewModels.PaginationInfo<Event>(page, totalItems, pageSize);

            paginationInfo.Items = await eventsQuery
                .Skip(paginationInfo.ItemsToSkip)
                .Take(paginationInfo.ItemsPerPage)
                .ToListAsync();

            // Passamos a lista de inscritos para a View pintar os botões corretamente
            ViewBag.MyEnrolledIds = myEnrolledEventIds;

            return View(paginationInfo);
        }

        // POST: UtenteEvents/Enroll
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(int eventId) {
            var userEmail = User.Identity?.Name;
            var customer = await _context.Customer.Include(c => c.Level).FirstOrDefaultAsync(c => c.Email == userEmail);
            var eventToEnroll = await _context.Event.FindAsync(eventId);

            if (customer == null || eventToEnroll == null) return NotFound();

            // A. Validação de Nível
            int userLevel = customer.Level?.LevelNumber ?? 0;
            if (userLevel < eventToEnroll.MinLevel) {
                TempData["ErrorMessage"] = "You need a higher level to join this event.";
                return RedirectToAction(nameof(Index));
            }

            // B. Verificar se já existe inscrição (Evitar duplicados)
            bool alreadyEnrolled = await _context.CustomerEvent
                .AnyAsync(ue => ue.EventId == eventId && ue.CustomerId == customer.CustomerId);

            if (alreadyEnrolled) {
                TempData["InfoMessage"] = "You are already registered for this event.";
                return RedirectToAction(nameof(Index));
            }

            // C. Criar Inscrição (AQUI ESTAVA A FALTA DE DADOS)
            var newRegistration = new CustomerEvent {
                EventId = eventId,
                CustomerId = customer.CustomerId,
                RegistrationDate = DateTime.Now,
                Status = "Enrolled",     // Define o estado inicial!
                PointsEarned = 0,        // Começa com 0 pontos
                CompletionDate = null    // Ainda não realizou
            };

            _context.CustomerEvent.Add(newRegistration);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Registration successful! Good luck.";
            return RedirectToAction(nameof(Index));
        }

        // Mantém o método Details como estava, parecia ok.
        public async Task<IActionResult> Details(int? id) {
            if (id == null) return NotFound();

            var @event = await _context.Event
                .Include(e => e.EventType)
                .Include(e => e.EventActivities).ThenInclude(ea => ea.Activity)
                .FirstOrDefaultAsync(m => m.EventId == id);

            if (@event == null) return NotFound();

            var userEmail = User.Identity?.Name;
            var customer = await _context.Customer.FirstOrDefaultAsync(c => c.Email == userEmail);

            // Verifica se está inscrito para mostrar botão diferente na View
            bool isEnrolled = false;
            if (customer != null) {
                isEnrolled = await _context.CustomerEvent
                   .AnyAsync(ue => ue.EventId == id && ue.CustomerId == customer.CustomerId);
            }

            ViewBag.IsEnrolled = isEnrolled;
            ViewBag.CustomerLevel = customer?.Level?.LevelNumber ?? 0;

            return View(@event);
        }
    }
}