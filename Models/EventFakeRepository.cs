namespace HealthWellbeing.Models
{
    public class EventFakeRepository : IEventRepository
    {
        private readonly List<Event> _events = new List<Event>
        {
            new Event
            {
                EventId = 1,
                EventName = "Treino de Cardio Matinal",
                EventDescription = "Sessão de corrida leve e alongamentos para começar o dia com energia.",
                EventType = "Cardio",
                DurationMinutes = 45,
                Intensity = "Média",
                
                EventDate = new DateTime(2025, 10, 10)
            },
            new Event
            {
                EventId = 2,
                EventName = "Aula de Pilates",
                EventDescription = "Treino focado em postura e flexibilidade.",
                EventType = "Pilates",
                DurationMinutes = 60,
                Intensity = "Baixa",
                
                EventDate = new DateTime(2025, 10, 8)
            },
            new Event
            {
                EventId = 3,
                EventName = "Treino Funcional",
                EventDescription = "Exercícios para força e coordenação geral.",
                EventType = "Funcional",
                DurationMinutes = 50,
                Intensity = "Alta",
                
                EventDate = new DateTime(2025, 10, 5)
            }
        };

        public IEnumerable<Event> Events => _events;

        public Event? GetEventById(int id)
        {
            return _events.FirstOrDefault(e => e.EventId == id);
        }
    }
}
