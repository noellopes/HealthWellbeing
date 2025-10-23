using Microsoft.AspNetCore.Mvc;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers
{
    public class EventsController : Controller
    {
        private readonly IEventRepository _eventRepository;

        // Construtor - usa o repositório fake
        public EventsController()
        {
            _eventRepository = new EventFakeRepository();
        }

        // GET: Events
        public IActionResult Index()
        {
            var events = _eventRepository.Events;
            return View(events);
        }

        // GET: Events/Details/5
        public IActionResult Details(int id)
        {
            var selectedEvent = _eventRepository.GetEventById(id);
            if (selectedEvent == null)
                return NotFound();

            return View(selectedEvent);
        }
    }
}
