namespace HealthWellbeing.Models {
    internal class EventTypeSeedData {

        internal static void Populate(EventTypeDbContext? dbContext) {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));

            dbContext.Database.EnsureCreated();

            PopulateEventTypes(dbContext);
        }

        private static void PopulateEventTypes(EventTypeDbContext dbContext) {
            if (dbContext.EventTypes.Any()) return;

            dbContext.EventTypes.AddRange(new List<EventType>() {
                new EventType {
                EventTypeId = 1,
                EventTypeName = "Treino de Cardio",
                EventTypeDescription = "Sessão intensa de treino cardiovascular com música motivadora."
                },
                new EventType {
                    EventTypeId = 2,
                    EventTypeName = "Aula de Spinning",
                    EventTypeDescription = "Aula em bicicletas estáticas, ideal para queimar calorias e melhorar a resistência."
                },
                new EventType {
                    EventTypeId = 3,
                    EventTypeName = "Treino Funcional",
                    EventTypeDescription = "Exercícios dinâmicos que trabalham força, equilíbrio e coordenação."
                },
                new EventType {
                    EventTypeId = 4,
                    EventTypeName = "Aula de Pilates",
                    EventTypeDescription = "Treino focado em alongamento, postura e fortalecimento do core."
                },
                new EventType {
                    EventTypeId = 5,
                    EventTypeName = "Aula de Zumba",
                    EventTypeDescription = "Sessão divertida de dança com ritmos latinos para melhorar a forma física."
                }
            });

            dbContext.SaveChanges();
        }
    }
}
