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
            EventTypeName = "Competição Desportiva",
            EventTypeScoringMode = "ranking",
            EventTypeMultiplier = 2.5f
        },
        new EventType {
            EventTypeName = "Workshop Educacional",
            EventTypeScoringMode = "fixed",
            EventTypeMultiplier = 1.0f
        },
        new EventType {
            EventTypeName = "Aula de Grupo",
            EventTypeScoringMode = "completion",
            EventTypeMultiplier = 1.5f
        },
        new EventType {
            EventTypeName = "Desafio de Resistência",
            EventTypeScoringMode = "time_based",
            EventTypeMultiplier = 2.2f
        },
        new EventType {
            EventTypeName = "Torneio",
            EventTypeScoringMode = "ranking",
            EventTypeMultiplier = 2.8f
        },
        new EventType {
            EventTypeName = "Seminário",
            EventTypeScoringMode = "fixed",
            EventTypeMultiplier = 1.1f
        },
        new EventType {
            EventTypeName = "Treino Personalizado",
            EventTypeScoringMode = "progressive",
            EventTypeMultiplier = 1.8f
        },
        new EventType {
            EventTypeName = "Maratona",
            EventTypeScoringMode = "time_based",
            EventTypeMultiplier = 3.0f
        },
        new EventType {
            EventTypeName = "Campeonato",
            EventTypeScoringMode = "ranking",
            EventTypeMultiplier = 2.7f
        },
        new EventType {
            EventTypeName = "Palestra",
            EventTypeScoringMode = "fixed",
            EventTypeMultiplier = 1.0f
        },
        new EventType {
            EventTypeName = "Aula Experimental",
            EventTypeScoringMode = "binary",
            EventTypeMultiplier = 1.2f
        },
        new EventType {
            EventTypeName = "Desafio de Força",
            EventTypeScoringMode = "progressive",
            EventTypeMultiplier = 2.1f
        },
        new EventType {
            EventTypeName = "Competição por Equipas",
            EventTypeScoringMode = "ranking",
            EventTypeMultiplier = 2.4f
        },
        new EventType {
            EventTypeName = "Workshop Interativo",
            EventTypeScoringMode = "completion",
            EventTypeMultiplier = 1.3f
        },
        new EventType {
            EventTypeName = "Aula de Técnica",
            EventTypeScoringMode = "progressive",
            EventTypeMultiplier = 1.6f
        },
        new EventType {
            EventTypeName = "Desafio de Velocidade",
            EventTypeScoringMode = "time_based",
            EventTypeMultiplier = 2.3f
        },
        new EventType {
            EventTypeName = "Liga Desportiva",
            EventTypeScoringMode = "ranking",
            EventTypeMultiplier = 2.9f
        },
        new EventType {
            EventTypeName = "Sessão de Demonstração",
            EventTypeScoringMode = "fixed",
            EventTypeMultiplier = 1.0f
        },
        new EventType {
            EventTypeName = "Treino de Grupo",
            EventTypeScoringMode = "completion",
            EventTypeMultiplier = 1.4f
        },
        new EventType {
            EventTypeName = "Competição Individual",
            EventTypeScoringMode = "ranking",
            EventTypeMultiplier = 2.2f
        },
        new EventType {
            EventTypeName = "Workshop Prático",
            EventTypeScoringMode = "completion",
            EventTypeMultiplier = 1.5f
        },
        new EventType {
            EventTypeName = "Aula Avançada",
            EventTypeScoringMode = "progressive",
            EventTypeMultiplier = 1.9f
        },
        new EventType {
            EventTypeName = "Desafio de Precisão",
            EventTypeScoringMode = "progressive",
            EventTypeMultiplier = 2.0f
        },
        new EventType {
            EventTypeName = "Torneio por Eliminação",
            EventTypeScoringMode = "ranking",
            EventTypeMultiplier = 2.6f
        },
        new EventType {
            EventTypeName = "Conferência",
            EventTypeScoringMode = "fixed",
            EventTypeMultiplier = 1.0f
        },
        new EventType {
            EventTypeName = "Aula para Iniciantes",
            EventTypeScoringMode = "completion",
            EventTypeMultiplier = 1.3f
        },
        new EventType {
            EventTypeName = "Competição por Pontos",
            EventTypeScoringMode = "ranking",
            EventTypeMultiplier = 2.3f
        },
        new EventType {
            EventTypeName = "Workshop Teórico",
            EventTypeScoringMode = "fixed",
            EventTypeMultiplier = 1.0f
        },
        new EventType {
            EventTypeName = "Treino de Resistência",
            EventTypeScoringMode = "time_based",
            EventTypeMultiplier = 2.1f
        },
        new EventType {
            EventTypeName = "Desafio de Equilíbrio",
            EventTypeScoringMode = "progressive",
            EventTypeMultiplier = 1.8f
        },
        new EventType {
            EventTypeName = "Campeonato por Fases",
            EventTypeScoringMode = "ranking",
            EventTypeMultiplier = 2.7f
        },
        new EventType {
            EventTypeName = "Aula Especializada",
            EventTypeScoringMode = "progressive",
            EventTypeMultiplier = 1.7f
        },
        new EventType {
            EventTypeName = "Workshop de Técnica",
            EventTypeScoringMode = "completion",
            EventTypeMultiplier = 1.4f
        },
        new EventType {
            EventTypeName = "Competição por Tempo",
            EventTypeScoringMode = "time_based",
            EventTypeMultiplier = 2.4f
        },
        new EventType {
            EventTypeName = "Treino de Flexibilidade",
            EventTypeScoringMode = "progressive",
            EventTypeMultiplier = 1.5f
        },
        new EventType {
            EventTypeName = "Desafio de Coordenação",
            EventTypeScoringMode = "progressive",
            EventTypeMultiplier = 1.9f
        },
        new EventType {
            EventTypeName = "Torneio Round Robin",
            EventTypeScoringMode = "ranking",
            EventTypeMultiplier = 2.5f
        },
        new EventType {
            EventTypeName = "Sessão de Orientação",
            EventTypeScoringMode = "fixed",
            EventTypeMultiplier = 1.0f
        },
        new EventType {
            EventTypeName = "Aula de Consolidação",
            EventTypeScoringMode = "completion",
            EventTypeMultiplier = 1.3f
        },
        new EventType {
            EventTypeName = "Competição de Habilidades",
            EventTypeScoringMode = "ranking",
            EventTypeMultiplier = 2.2f
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