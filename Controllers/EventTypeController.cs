using HealthWellbeing.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealthWellbeing.Controllers {

    [Route("api/[controller]")]
    public class EventTypeController : Controller {

        static private List<EventType> EventTypes = new List<EventType> {
            new EventType {
                EventTypeId = 1,
                EventTypeName = "Corrida 5K",
                EventTypeDescription = "Evento de corrida ao ar livre, objetivo completar 5 km.",
                IsPublished = true,
            },
            new EventType {
                EventTypeId = 2,
                EventTypeName = "Treino de Ginásio",
                EventTypeDescription = "Sessões de musculação e cardio supervisionadas.",
                IsPublished = true,

            },
            new EventType {
                EventTypeId = 3,
                EventTypeName = "Aula de Yoga",
                EventTypeDescription = "Foco em alongamentos e respiração para reduzir stress.",
                IsPublished = false,

            },
            new EventType {
                EventTypeId = 4,
                EventTypeName = "Sessão de Mindfulness",
                EventTypeDescription = "Prática guiada de atenção plena (20–30 min).",
                IsPublished = false,

            },
        };

        
        [HttpGet]
        public ActionResult<List<EventType>> GetEventTypes() {
            return Ok(EventTypes);

        }

        [HttpGet("{id}")]
        public ActionResult<List<EventType>> GetEventTypesById(int id) {
            var EventType = EventTypes.FirstOrDefault(x => x.EventTypeId == id);
            if (EventType is null) {
                return NotFound();
            }

            return Ok(EventType);
        }

        [HttpPost]
        public ActionResult<List<EventType>> AddEventType(EventType newEventType) {
            if (newEventType is null) {
                return BadRequest();
            }

            newEventType.EventTypeId = EventTypes.Max(x => x.EventTypeId) + 1;
            EventTypes.Add(newEventType);
            return CreatedAtAction(nameof(GetEventTypesById), new { id = newEventType.EventTypeId }, newEventType);
        }
    }
}
