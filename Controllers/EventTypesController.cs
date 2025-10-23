using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthWellbeing.Controllers {
    public class EventTypesController : Controller {

        private IEventTypeRepository _eventTypeRepository;

        public EventTypesController(IEventTypeRepository eventTypeRepository) {
            _eventTypeRepository = eventTypeRepository;
        }
        public ActionResult Index() {
            return View(_eventTypeRepository.EventTypes);
        }
    }
}
