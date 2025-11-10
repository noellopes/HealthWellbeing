using HealthWellbeing.Models;

namespace HealthWellbeing.Data {
    internal class SeedData {

        internal static void Populate(HealthWellbeingDbContext? dbContext) {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));

            dbContext.Database.EnsureCreated();

            PopulateEventTypes(dbContext);
            PopulateEvents(dbContext);
        }

        private static void PopulateEventTypes(HealthWellbeingDbContext dbContext) {
            if (dbContext.EventType.Any()) return;

            dbContext.EventType.AddRange(new List<EventType>() {
                new EventType {
                EventTypeName = "Treino de Cardio",
                EventTypeDescription = "Sessão intensa de treino cardiovascular com música motivadora."
                },
                new EventType {
                    EventTypeName = "Aula de Spinning",
                    EventTypeDescription = "Aula em bicicletas estáticas, ideal para queimar calorias e melhorar a resistência."
                },
                new EventType {
                    EventTypeName = "Treino Funcional",
                    EventTypeDescription = "Exercícios dinâmicos que trabalham força, equilíbrio e coordenação."
                },
                new EventType {
                    EventTypeName = "Aula de Pilates",
                    EventTypeDescription = "Treino focado em alongamento, postura e fortalecimento do core."
                },
                new EventType {
                    EventTypeName = "Aula de Zumba",
                    EventTypeDescription = "Sessão divertida de dança com ritmos latinos para melhorar a forma física."
                }
            });

            dbContext.SaveChanges();
        }

        private static void PopulateEvents(HealthWellbeingDbContext dbContext) {
            if (dbContext.Event.Any()) return;

            dbContext.Event.AddRange(new List<Event>() {
                new Event
                {
                    EventName = "Treino de Cardio Matinal",
                    EventDescription = "Sessão de corrida leve e alongamentos para começar o dia com energia.",
                    EventType = "Cardio",
                    DurationMinutes = 45,
                    Intensity = "Média",

                    EventDate = new DateTime(2025, 10, 10)
                },
                new Event
                {
                    EventName = "Aula de Pilates",
                    EventDescription = "Treino focado em postura e flexibilidade.",
                    EventType = "Pilates",
                    DurationMinutes = 60,
                    Intensity = "Baixa",

                    EventDate = new DateTime(2025, 10, 8)
                },
                new Event
                {
                    EventName = "Treino Funcional",
                    EventDescription = "Exercícios para força e coordenação geral.",
                    EventType = "Funcional",
                    DurationMinutes = 50,
                    Intensity = "Alta",

                    EventDate = new DateTime(2025, 10, 5)
                }
            });
        
            dbContext.SaveChanges();
        }
    }
}
