using HealthWellbeing.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthWellbeing.Data
{
    internal class SeedData
    {
        internal static void Populate(HealthWellbeingDbContext? dbContext)
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));

            // dbContext.Database.EnsureCreated(); // <- Comentado, como falámos

            PopulateEventTypes(dbContext);
            PopulateEvents(dbContext);
        }

        private static void PopulateEventTypes(HealthWellbeingDbContext dbContext)
        {
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

            var eventList = new List<Event>();
            var now = DateTime.Now;
            var eventTypes = new[] { "Cardio", "Spinning", "Funcional", "Pilates", "Zumba" };

            // Gerar 15 eventos 'Realizado' (passado)
            for (int i = 1; i <= 15; i++)
            {
                eventList.Add(new Event
                {
                    EventName = $"Evento Passado {i}",
                    EventDescription = $"Descrição do evento passado {i}.",
                    EventType = eventTypes[i % 5],
                    EventStart = now.AddDays(-i).Date.AddHours(10),
                    EventEnd = now.AddDays(-i).Date.AddHours(11),
                    EventPoints = 50 + (i * 5),
                    MinLevel = (i % 3) + 1
                });
            }

            // Gerar 5 eventos 'A Decorrer' (agora)
            for (int i = 1; i <= 5; i++)
            {
                eventList.Add(new Event
                {
                    EventName = $"Evento A Decorrer {i}",
                    EventDescription = $"Descrição do evento a decorrer {i}.",
                    EventType = eventTypes[i % 5],
                    EventStart = now.AddHours(-1), // Começou há 1 hora
                    EventEnd = now.AddHours(i),   // Termina daqui a 'i' horas
                    EventPoints = 100 + (i * 10),
                    MinLevel = (i % 2) + 1
                });
            }

            // Gerar 20 eventos 'Agendado' (futuro)
            for (int i = 1; i <= 20; i++)
            {
                eventList.Add(new Event
                {
                    EventName = $"Evento Futuro {i}",
                    EventDescription = $"Descrição do evento futuro {i}.",
                    EventType = eventTypes[i % 5],
                    EventStart = now.AddDays(i).Date.AddHours(18),
                    EventEnd = now.AddDays(i).Date.AddHours(19),
                    EventPoints = 75 + (i * 2),
                    MinLevel = (i % 4) + 1
                });
            }

            dbContext.Event.AddRange(eventList);
            dbContext.SaveChanges();
        }
    }
}