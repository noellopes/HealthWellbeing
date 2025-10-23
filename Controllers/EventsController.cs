using Microsoft.AspNetCore.Mvc;
using HealthWellbeing.Models;

namespace HealthWellbeing.Controllers
{
    public class EventsController : Controller
    {
        // Simula uma base de dados em memória
        private static List<Event> _events = new List<Event>();
        private static int _nextId = 1;

        // GET: Events
        public IActionResult Index()
        {
            return View(_events);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Event @event)
        {
            if (ModelState.IsValid)
            {
                @event.EventId = _nextId++;
                _events.Add(@event);
                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }

        // GET: Events/Details/5
        public IActionResult Details(int id)
        {
            var ev = _events.FirstOrDefault(e => e.EventId == id);
            if (ev == null)
                return NotFound();

            return View(ev);
        }

        // GET: Events/Edit/5
        public IActionResult Edit(int id)
        {
            var ev = _events.FirstOrDefault(e => e.EventId == id);
            if (ev == null)
                return NotFound();

            return View(ev);
        }

        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Event updatedEvent)
        {
            var ev = _events.FirstOrDefault(e => e.EventId == id);
            if (ev == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                ev.EventName = updatedEvent.EventName;
                ev.EventDescription = updatedEvent.EventDescription;
                ev.EventType = updatedEvent.EventType;
                ev.DurationMinutes = updatedEvent.DurationMinutes;
                ev.Intensity = updatedEvent.Intensity;
                ev.CaloriesBurned = updatedEvent.CaloriesBurned;
                ev.EventDate = updatedEvent.EventDate;

                return RedirectToAction(nameof(Index));
            }

            return View(updatedEvent);
        }

        // GET: Events/Delete/5
        public IActionResult Delete(int id)
        {
            var ev = _events.FirstOrDefault(e => e.EventId == id);
            if (ev == null)
                return NotFound();

            return View(ev);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var ev = _events.FirstOrDefault(e => e.EventId == id);
            if (ev != null)
                _events.Remove(ev);

            return RedirectToAction(nameof(Index));
        }
    }
}
