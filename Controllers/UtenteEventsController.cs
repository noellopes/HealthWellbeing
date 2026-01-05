using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // Necessário para SelectList
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.ViewModels; // Necessário para PaginationInfo
using Microsoft.AspNetCore.Authorization;

namespace HealthWellbeing.Controllers
{
    [Authorize(Roles = "Utente")]
    public class UtenteEventsController : Controller
    {
        private readonly HealthWellbeingDbContext _context;

        public UtenteEventsController(HealthWellbeingDbContext context)
        {
            _context = context;
        }

        // GET: Lista de Eventos (Com Pesquisa, Filtros e Paginação)
        public async Task<IActionResult> Index(string searchName, int? searchType, int? searchLevel, string searchStatus, int page = 1)
        {
            var userEmail = User.Identity?.Name;

            // 1. Identificar o Utente Logado e o seu Nível
            var customer = await _context.Customer
                .Include(c => c.Level)
                .FirstOrDefaultAsync(c => c.Email == userEmail);

            if (customer == null) return View("Error");

            ViewBag.CustomerLevel = customer.Level?.LevelNumber ?? 0;
            ViewBag.CustomerId = customer.CustomerId;

            // Guardar os filtros na ViewBag para manter os valores na barra de pesquisa
            ViewBag.SearchName = searchName;
            ViewBag.SearchType = searchType;
            ViewBag.SearchLevel = searchLevel;
            ViewBag.SearchStatus = searchStatus;

            // 2. Preencher as Dropdowns (Categorias e Status)
            var eventTypes = _context.EventType.OrderBy(t => t.EventTypeName);
            ViewBag.EventTypesList = new SelectList(eventTypes, "EventTypeId", "EventTypeName", searchType);

            var statusList = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "All Statuses" },
                new SelectListItem { Value = "enrolled", Text = "Enrolled" },
                new SelectListItem { Value = "not_enrolled", Text = "Not Enrolled" }
            };

            // Selecionar o item correto na dropdown de status
            var selectedStatus = statusList.FirstOrDefault(s => s.Value == searchStatus);
            if (selectedStatus != null) selectedStatus.Selected = true;
            ViewBag.StatusList = statusList;

            // 3. Query Base (Apenas eventos futuros ou a decorrer)
            var now = DateTime.Now;
            var eventsQuery = _context.Event
                .Include(e => e.EventType)
                .Where(e => e.EventEnd >= now)
                .AsQueryable();

            // 4. Aplicar Filtros
            if (!string.IsNullOrEmpty(searchName))
            {
                eventsQuery = eventsQuery.Where(e => e.EventName.Contains(searchName));
            }

            if (searchType.HasValue)
            {
                eventsQuery = eventsQuery.Where(e => e.EventTypeId == searchType);
            }

            if (searchLevel.HasValue)
            {
                eventsQuery = eventsQuery.Where(e => e.MinLevel == searchLevel);
            }

            // 5. Lógica de Filtro "Inscrito / Não Inscrito"
            // Primeiro, obter a lista de IDs onde o utilizador já está inscrito
            var myEnrolledIds = await _context.UtenteEvent
                .Where(ue => ue.CustomerId == customer.CustomerId)
                .Select(ue => ue.EventId)
                .ToListAsync();

            if (!string.IsNullOrEmpty(searchStatus))
            {
                if (searchStatus == "enrolled")
                {
                    eventsQuery = eventsQuery.Where(e => myEnrolledIds.Contains(e.EventId));
                }
                else if (searchStatus == "not_enrolled")
                {
                    eventsQuery = eventsQuery.Where(e => !myEnrolledIds.Contains(e.EventId));
                }
            }

            // 6. Paginação e Ordenação
            int totalItems = await eventsQuery.CountAsync();
            int pageSize = 6; // Mostra 6 cartões por página
            var paginationInfo = new ViewModels.PaginationInfo<Event>(page, totalItems, pageSize);

            var pagedEvents = await eventsQuery
                .OrderBy(e => e.MinLevel)      // Ordenar primeiro por Nível
                .ThenBy(e => e.EventStart)     // Depois por Data
                .Skip(paginationInfo.ItemsToSkip)
                .Take(paginationInfo.ItemsPerPage)
                .ToListAsync();

            paginationInfo.Items = pagedEvents;

            // Passar a lista de inscrições para a View (para pintar os cartões de verde/azul)
            ViewBag.MyEnrolledIds = myEnrolledIds;

            return View(paginationInfo);
        }

        // GET: Detalhes do Evento
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Event
                .Include(e => e.EventType)
                .Include(e => e.EventActivities).ThenInclude(ea => ea.Activity)
                .FirstOrDefaultAsync(m => m.EventId == id);

            if (@event == null) return NotFound();

            var userEmail = User.Identity?.Name;
            var customer = await _context.Customer.Include(c => c.Level).FirstOrDefaultAsync(c => c.Email == userEmail);

            ViewBag.CustomerLevel = customer?.Level?.LevelNumber ?? 0;

            var isEnrolled = await _context.UtenteEvent
                .AnyAsync(ue => ue.EventId == id && ue.CustomerId == customer.CustomerId);

            ViewBag.IsEnrolled = isEnrolled;

            return View(@event);
        }

        // POST: Inscrever
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(int eventId)
        {
            var userEmail = User.Identity?.Name;
            var customer = await _context.Customer.Include(c => c.Level).FirstOrDefaultAsync(c => c.Email == userEmail);
            var @event = await _context.Event.FindAsync(eventId);

            if (customer == null || @event == null) return NotFound();

            // Validação de Nível
            int userLevel = customer.Level?.LevelNumber ?? 0;
            if (userLevel < @event.MinLevel)
            {
                TempData["ErrorMessage"] = "Your level is not high enough for this event.";
                return RedirectToAction(nameof(Index));
            }

            // Verificar Duplicação
            bool alreadyEnrolled = await _context.UtenteEvent
                .AnyAsync(ue => ue.EventId == eventId && ue.CustomerId == customer.CustomerId);

            if (alreadyEnrolled)
            {
                TempData["InfoMessage"] = "You are already enrolled in this event.";
                return RedirectToAction(nameof(Index));
            }

            // Criar Inscrição
            var newRegistration = new UtenteEvent
            {
                EventId = eventId,
                CustomerId = customer.CustomerId,
                RegistrationDate = DateTime.Now
            };

            _context.UtenteEvent.Add(newRegistration);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Enrollment successful! Get ready to train.";
            return RedirectToAction(nameof(Index));
        }
    }
}