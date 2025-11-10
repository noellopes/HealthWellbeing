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

        private static void PopulateEvents(HealthWellbeingDbContext dbContext)
        {
            if (dbContext.Event.Any()) return;

            dbContext.Event.AddRange(new List<Event>() {
                new Event { EventName = "Treino de Cardio Matinal", EventDescription = "Sessão de corrida leve e alongamentos para começar o dia com energia.", EventType = "Cardio", DurationMinutes = 45, Intensity = "Média", EventDate = new DateTime(2025, 10, 10) },
                new Event { EventName = "Aula de Pilates", EventDescription = "Treino focado em postura e flexibilidade.", EventType = "Pilates", DurationMinutes = 60, Intensity = "Baixa", EventDate = new DateTime(2025, 10, 8) },
                new Event { EventName = "Treino Funcional", EventDescription = "Exercícios para força e coordenação geral.", EventType = "Funcional", DurationMinutes = 50, Intensity = "Alta", EventDate = new DateTime(2025, 10, 5) },
                new Event { EventName = "Aula de Spinning", EventDescription = "Pedaladas intensas para melhorar resistência e queimar calorias.", EventType = "Spinning", DurationMinutes = 55, Intensity = "Alta", EventDate = new DateTime(2025, 10, 12) },
                new Event { EventName = "Treino Vespertino", EventDescription = "Sessão rápida para manter a forma depois do trabalho.", EventType = "Cardio", DurationMinutes = 30, Intensity = "Média", EventDate = new DateTime(2025, 10, 11) },
                new Event { EventName = "Zumba Energética", EventDescription = "Aula animada de dança para liberar endorfinas e tonificar o corpo.", EventType = "Zumba", DurationMinutes = 50, Intensity = "Alta", EventDate = new DateTime(2025, 10, 13) },
                new Event { EventName = "Pilates Core", EventDescription = "Foco no fortalecimento do core e melhoria da postura.", EventType = "Pilates", DurationMinutes = 45, Intensity = "Média", EventDate = new DateTime(2025, 10, 14) },
                new Event { EventName = "Treino Funcional Matinal", EventDescription = "Atividades funcionais para começar o dia com energia.", EventType = "Funcional", DurationMinutes = 40, Intensity = "Média", EventDate = new DateTime(2025, 10, 15) },
                new Event { EventName = "Spinning Intenso", EventDescription = "Pedaladas de alta intensidade para desafiar o corpo.", EventType = "Spinning", DurationMinutes = 60, Intensity = "Alta", EventDate = new DateTime(2025, 10, 16) },
                new Event { EventName = "Cardio Mix", EventDescription = "Combinação de exercícios cardiovasculares variados.", EventType = "Cardio", DurationMinutes = 50, Intensity = "Média", EventDate = new DateTime(2025, 10, 17) }
            });

            dbContext.SaveChanges();
        }
    }
}
