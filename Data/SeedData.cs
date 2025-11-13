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
