using HealthWellbeing.Data;

namespace HealthWellbeing.Models {
    internal class EventTypeSeedData {

        internal static void Populate(HealthWellbeingDbContext? dbContext) {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));

            dbContext.Database.EnsureCreated();

            PopulateEventTypes(dbContext);
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
    }
}
