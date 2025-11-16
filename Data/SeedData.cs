using HealthWellbeing.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthWellbeing.Data
{
    internal class SeedData {
        internal static void Populate(HealthWellbeingDbContext? dbContext) {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));

            PopulateEventTypes(dbContext);
            PopulateEvents(dbContext);
            PopulateLevels(dbContext);
        }

        private static void PopulateEventTypes(HealthWellbeingDbContext dbContext) {
            if (dbContext.EventType.Any()) return;

            dbContext.EventType.AddRange(new List<EventType>() {
                //EDUCAÇÃO E FORMAÇÃO
                new EventType { EventTypeName = "Workshop Educacional", EventTypeScoringMode = "fixed", EventTypeMultiplier = 1.0f },
                new EventType { EventTypeName = "Seminário Temático", EventTypeScoringMode = "fixed", EventTypeMultiplier = 1.1f },
                new EventType { EventTypeName = "Palestra Informativa", EventTypeScoringMode = "fixed", EventTypeMultiplier = 1.0f },
                new EventType { EventTypeName = "Demonstração Técnica", EventTypeScoringMode = "fixed", EventTypeMultiplier = 1.0f },
                new EventType { EventTypeName = "Sessão de Orientação", EventTypeScoringMode = "fixed", EventTypeMultiplier = 1.0f },

                //TREINO CARDIOVASCULAR
                new EventType { EventTypeName = "Sessão de Running", EventTypeScoringMode = "time_based", EventTypeMultiplier = 2.0f },
                new EventType { EventTypeName = "Treino de Cycling", EventTypeScoringMode = "time_based", EventTypeMultiplier = 2.1f },
                new EventType { EventTypeName = "Aula de Cardio-Grupo", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.5f },
                new EventType { EventTypeName = "Treino de Natação", EventTypeScoringMode = "time_based", EventTypeMultiplier = 2.2f },
                new EventType { EventTypeName = "Sessão de HIIT", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.8f },

                //TREINO DE FORÇA
                new EventType { EventTypeName = "Treino de Musculação", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.7f },
                new EventType { EventTypeName = "Sessão de CrossFit", EventTypeScoringMode = "progressive", EventTypeMultiplier = 2.1f },
                new EventType { EventTypeName = "Treino Funcional", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.6f },
                new EventType { EventTypeName = "Aula de Powerlifting", EventTypeScoringMode = "progressive", EventTypeMultiplier = 2.0f },
                new EventType { EventTypeName = "Treino de Calistenia", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.5f },

                //BEM-ESTAR E MOBILIDADE
                new EventType { EventTypeName = "Aula de Yoga", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.3f },
                new EventType { EventTypeName = "Sessão de Pilates", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.4f },
                new EventType { EventTypeName = "Treino de Flexibilidade", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.5f },
                new EventType { EventTypeName = "Aula de Mobilidade", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.3f },
                new EventType { EventTypeName = "Sessão de Alongamento", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.2f },

                //DESPORTOS E ARTES MARCIAS
                new EventType { EventTypeName = "Aula de Artes Marciais", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.8f },
                new EventType { EventTypeName = "Treino de Boxe", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.9f },
                new EventType { EventTypeName = "Sessão de Lutas", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.8f },
                new EventType { EventTypeName = "Aula de Defesa Pessoal", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.4f },
                new EventType { EventTypeName = "Treino Desportivo Coletivo", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.6f },

                //DESAFIOS E COMPETIÇÕES
                new EventType { EventTypeName = "Competição de Running", EventTypeScoringMode = "time_based", EventTypeMultiplier = 2.5f },
                new EventType { EventTypeName = "Torneio Desportivo", EventTypeScoringMode = "binary", EventTypeMultiplier = 2.3f },
                new EventType { EventTypeName = "Desafio de Resistência", EventTypeScoringMode = "time_based", EventTypeMultiplier = 2.4f },
                new EventType { EventTypeName = "Competição de Força", EventTypeScoringMode = "progressive", EventTypeMultiplier = 2.2f },
                new EventType { EventTypeName = "Desafio de Superação", EventTypeScoringMode = "progressive", EventTypeMultiplier = 2.1f },

                //ATIVIDADES EM GRUPO
                new EventType { EventTypeName = "Aula de Grupo", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.4f },
                new EventType { EventTypeName = "Treino Coletivo", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.5f },
                new EventType { EventTypeName = "Workshop Prático", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.3f },
                new EventType { EventTypeName = "Sessão de Team Building", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.2f },
                new EventType { EventTypeName = "Aula Experimental", EventTypeScoringMode = "binary", EventTypeMultiplier = 1.1f },

                //ESPECIALIZADOS E TÉCNICOS
                new EventType { EventTypeName = "Treino Técnico", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.6f },
                new EventType { EventTypeName = "Workshop de Técnica", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.3f },
                new EventType { EventTypeName = "Aula Avançada", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.7f },
                new EventType { EventTypeName = "Sessão de Perfeiçoamento", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.5f },
                new EventType { EventTypeName = "Treino Especializado", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.8f }
            });

            dbContext.SaveChanges();
        }

        private static void PopulateEvents(HealthWellbeingDbContext dbContext) {
            if (dbContext.Event.Any()) return;

            var eventTypes = dbContext.EventType.ToList();
            if (!eventTypes.Any()) return;

            var eventList = new List<Event>();
            var now = DateTime.Now;

            eventList.Add(new Event { EventName = "Competição Anual", EventDescription = "Competição de final de ano.", EventTypeId = eventTypes[0].EventTypeId, EventStart = now.AddDays(-30), EventEnd = now.AddDays(-30).AddHours(3), EventPoints = 200, MinLevel = 3 });
            eventList.Add(new Event { EventName = "Workshop de Nutrição", EventDescription = "Aprenda a comer melhor.", EventTypeId = eventTypes[1].EventTypeId, EventStart = now.AddDays(-28), EventEnd = now.AddDays(-28).AddHours(2), EventPoints = 50, MinLevel = 1 });
            eventList.Add(new Event { EventName = "Aula de Zumba", EventDescription = "Dança e diversão.", EventTypeId = eventTypes[2].EventTypeId, EventStart = now.AddDays(-26), EventEnd = now.AddDays(-26).AddHours(1), EventPoints = 75, MinLevel = 1 });
            eventList.Add(new Event { EventName = "Desafio CrossFit", EventDescription = "Teste os seus limites.", EventTypeId = eventTypes[3].EventTypeId, EventStart = now.AddDays(-24), EventEnd = now.AddDays(-24).AddHours(2), EventPoints = 150, MinLevel = 4 });
            eventList.Add(new Event { EventName = "Torneio de Ténis", EventDescription = "Torneio de pares.", EventTypeId = eventTypes[4].EventTypeId, EventStart = now.AddDays(-22), EventEnd = now.AddDays(-22).AddHours(5), EventPoints = 250, MinLevel = 3 });
            eventList.Add(new Event { EventName = "Seminário de Saúde Mental", EventDescription = "Bem-estar psicológico.", EventTypeId = eventTypes[5].EventTypeId, EventStart = now.AddDays(-20), EventEnd = now.AddDays(-20).AddHours(2), EventPoints = 55, MinLevel = 1 });
            eventList.Add(new Event { EventName = "Sessão de Personal Trainer", EventDescription = "Foco nos seus objetivos.", EventTypeId = eventTypes[6].EventTypeId, EventStart = now.AddDays(-18), EventEnd = now.AddDays(-18).AddHours(1), EventPoints = 90, MinLevel = 2 });
            eventList.Add(new Event { EventName = "Meia Maratona", EventDescription = "Corrida de 21km.", EventTypeId = eventTypes[7].EventTypeId, EventStart = now.AddDays(-16), EventEnd = now.AddDays(-16).AddHours(4), EventPoints = 300, MinLevel = 5 });
            eventList.Add(new Event { EventName = "Campeonato de Natação", EventDescription = "Vários estilos.", EventTypeId = eventTypes[8].EventTypeId, EventStart = now.AddDays(-14), EventEnd = now.AddDays(-14).AddHours(3), EventPoints = 280, MinLevel = 4 });
            eventList.Add(new Event { EventName = "Palestra Motivacional", EventDescription = "Alcance o seu potencial.", EventTypeId = eventTypes[9].EventTypeId, EventStart = now.AddDays(-12), EventEnd = now.AddDays(-12).AddHours(1), EventPoints = 50, MinLevel = 1 });
            eventList.Add(new Event { EventName = "Aula de Yoga Experimental", EventDescription = "Descubra o Yoga.", EventTypeId = eventTypes[10].EventTypeId, EventStart = now.AddDays(-10), EventEnd = now.AddDays(-10).AddHours(1), EventPoints = 60, MinLevel = 1 });
            eventList.Add(new Event { EventName = "Desafio de Powerlifting", EventDescription = "Supino, Agachamento e Peso Morto.", EventTypeId = eventTypes[11].EventTypeId, EventStart = now.AddDays(-8), EventEnd = now.AddDays(-8).AddHours(3), EventPoints = 180, MinLevel = 4 });
            eventList.Add(new Event { EventName = "Torneio de Voleibol", EventDescription = "Equipas de 4.", EventTypeId = eventTypes[12].EventTypeId, EventStart = now.AddDays(-6), EventEnd = now.AddDays(-6).AddHours(4), EventPoints = 220, MinLevel = 2 });
            eventList.Add(new Event { EventName = "Workshop de Mindfulness", EventDescription = "Técnicas de relaxamento.", EventTypeId = eventTypes[13].EventTypeId, EventStart = now.AddDays(-4), EventEnd = now.AddDays(-4).AddHours(2), EventPoints = 65, MinLevel = 1 });
            eventList.Add(new Event { EventName = "Aula de Técnica de Corrida", EventDescription = "Corra de forma eficiente.", EventTypeId = eventTypes[14].EventTypeId, EventStart = now.AddDays(-2), EventEnd = now.AddDays(-2).AddHours(1), EventPoints = 80, MinLevel = 2 });

            eventList.Add(new Event { EventName = "Desafio de Sprint", EventDescription = "Evento a decorrer agora.", EventTypeId = eventTypes[15].EventTypeId, EventStart = now.AddMinutes(-30), EventEnd = now.AddMinutes(30), EventPoints = 110, MinLevel = 2 });
            eventList.Add(new Event { EventName = "Liga de Basquetebol", EventDescription = "Jogo semanal.", EventTypeId = eventTypes[16].EventTypeId, EventStart = now.AddHours(-1), EventEnd = now.AddHours(1), EventPoints = 290, MinLevel = 3 });
            eventList.Add(new Event { EventName = "Demonstração de Artes Marciais", EventDescription = "Apresentação de técnicas.", EventTypeId = eventTypes[17].EventTypeId, EventStart = now.AddMinutes(-15), EventEnd = now.AddHours(1), EventPoints = 50, MinLevel = 1 });
            eventList.Add(new Event { EventName = "Treino de HIIT", EventDescription = "Alta intensidade.", EventTypeId = eventTypes[18].EventTypeId, EventStart = now.AddMinutes(-10), EventEnd = now.AddMinutes(45), EventPoints = 70, MinLevel = 2 });
            eventList.Add(new Event { EventName = "Competição de Crossfit", EventDescription = "WOD especial.", EventTypeId = eventTypes[19].EventTypeId, EventStart = now.AddHours(-2), EventEnd = now.AddHours(1), EventPoints = 190, MinLevel = 4 });

            eventList.Add(new Event { EventName = "Workshop Prático de Primeiros Socorros", EventDescription = "Saiba como agir.", EventTypeId = eventTypes[20].EventTypeId, EventStart = now.AddDays(1), EventEnd = now.AddDays(1).AddHours(3), EventPoints = 75, MinLevel = 1 });
            eventList.Add(new Event { EventName = "Aula Avançada de Spinning", EventDescription = "Suba a montanha.", EventTypeId = eventTypes[21].EventTypeId, EventStart = now.AddDays(2), EventEnd = now.AddDays(2).AddHours(1), EventPoints = 95, MinLevel = 3 });
            eventList.Add(new Event { EventName = "Desafio de Tiro ao Arco", EventDescription = "Teste a sua mira.", EventTypeId = eventTypes[22].EventTypeId, EventStart = now.AddDays(3), EventEnd = now.AddDays(3).AddHours(2), EventPoints = 100, MinLevel = 2 });
            eventList.Add(new Event { EventName = "Torneio de Xadrez", EventDescription = "Eliminatórias.", EventTypeId = eventTypes[23].EventTypeId, EventStart = now.AddDays(4), EventEnd = now.AddDays(4).AddHours(4), EventPoints = 260, MinLevel = 1 });
            eventList.Add(new Event { EventName = "Conferência de Medicina Desportiva", EventDescription = "Novas tendências.", EventTypeId = eventTypes[24].EventTypeId, EventStart = now.AddDays(5), EventEnd = now.AddDays(5).AddHours(6), EventPoints = 50, MinLevel = 1 });
            eventList.Add(new Event { EventName = "Aula de Pilates para Iniciantes", EventDescription = "Controle o seu corpo.", EventTypeId = eventTypes[25].EventTypeId, EventStart = now.AddDays(6), EventEnd = now.AddDays(6).AddHours(1), EventPoints = 65, MinLevel = 1 });
            eventList.Add(new Event { EventName = "Competição de Skate", EventDescription = "Melhor manobra.", EventTypeId = eventTypes[26].EventTypeId, EventStart = now.AddDays(7), EventEnd = now.AddDays(7).AddHours(3), EventPoints = 230, MinLevel = 2 });
            eventList.Add(new Event { EventName = "Workshop Teórico de Treino", EventDescription = "Planeamento de treino.", EventTypeId = eventTypes[27].EventTypeId, EventStart = now.AddDays(8), EventEnd = now.AddDays(8).AddHours(2), EventPoints = 50, MinLevel = 1 });
            eventList.Add(new Event { EventName = "Treino de Maratona (Grupo)", EventDescription = "Preparação conjunta.", EventTypeId = eventTypes[28].EventTypeId, EventStart = now.AddDays(9), EventEnd = now.AddDays(9).AddHours(2), EventPoints = 150, MinLevel = 3 });
            eventList.Add(new Event { EventName = "Desafio de Slackline", EventDescription = "Teste o seu equilíbrio.", EventTypeId = eventTypes[29].EventTypeId, EventStart = now.AddDays(10), EventEnd = now.AddDays(10).AddHours(2), EventPoints = 90, MinLevel = 2 });
            eventList.Add(new Event { EventName = "Campeonato de Judo", EventDescription = "Fases de grupos.", EventTypeId = eventTypes[30].EventTypeId, EventStart = now.AddDays(11), EventEnd = now.AddDays(11).AddHours(5), EventPoints = 270, MinLevel = 3 });
            eventList.Add(new Event { EventName = "Aula Especializada de Defesa Pessoal", EventDescription = "Técnicas essenciais.", EventTypeId = eventTypes[31].EventTypeId, EventStart = now.AddDays(12), EventEnd = now.AddDays(12).AddHours(2), EventPoints = 85, MinLevel = 1 });
            eventList.Add(new Event { EventName = "Workshop de Dança Contemporânea", EventDescription = "Movimento e expressão.", EventTypeId = eventTypes[32].EventTypeId, EventStart = now.AddDays(13), EventEnd = now.AddDays(13).AddHours(2), EventPoints = 70, MinLevel = 1 });
            eventList.Add(new Event { EventName = "Competição de Remo", EventDescription = "Contra-relógio.", EventTypeId = eventTypes[33].EventTypeId, EventStart = now.AddDays(14), EventEnd = now.AddDays(14).AddHours(3), EventPoints = 240, MinLevel = 3 });
            eventList.Add(new Event { EventName = "Treino de Flexibilidade (Grupo)", EventDescription = "Alongamentos profundos.", EventTypeId = eventTypes[34].EventTypeId, EventStart = now.AddDays(15), EventEnd = now.AddDays(15).AddHours(1), EventPoints = 75, MinLevel = 1 });
            eventList.Add(new Event { EventName = "Desafio de Parkour", EventDescription = "Circuito de obstáculos.", EventTypeId = eventTypes[35].EventTypeId, EventStart = now.AddDays(16), EventEnd = now.AddDays(16).AddHours(2), EventPoints = 160, MinLevel = 4 });
            eventList.Add(new Event { EventName = "Torneio de Padel", EventDescription = "Sistema Round Robin.", EventTypeId = eventTypes[36].EventTypeId, EventStart = now.AddDays(17), EventEnd = now.AddDays(17).AddHours(4), EventPoints = 250, MinLevel = 2 });
            eventList.Add(new Event { EventName = "Sessão de Orientação (Outdoor)", EventDescription = "Navegação e natureza.", EventTypeId = eventTypes[37].EventTypeId, EventStart = now.AddDays(18), EventEnd = now.AddDays(18).AddHours(3), EventPoints = 50, MinLevel = 1 });
            eventList.Add(new Event { EventName = "Aula de Boxe (Consolidação)", EventDescription = "Revisão de técnicas.", EventTypeId = eventTypes[38].EventTypeId, EventStart = now.AddDays(19), EventEnd = now.AddDays(19).AddHours(1), EventPoints = 65, MinLevel = 2 });
            eventList.Add(new Event { EventName = "Competição de E-Sports", EventDescription = "Torneio de FIFA.", EventTypeId = eventTypes[39].EventTypeId, EventStart = now.AddDays(20), EventEnd = now.AddDays(20).AddHours(5), EventPoints = 220, MinLevel = 1 });

            dbContext.Event.AddRange(eventList);
            dbContext.SaveChanges();
        }

        private static void PopulateLevels(HealthWellbeingDbContext dbContext) {
            if (dbContext.Level.Any()) return;

            dbContext.Level.AddRange(new List<Level>() {
                new Level { LevelAtual = 1, LevelCategory = "Iniciante", Description = "Primeiros passos na jornada de saúde" },
                new Level { LevelAtual = 2, LevelCategory = "Iniciante", Description = "Começando a criar rotinas saudáveis" },
                new Level { LevelAtual = 3, LevelCategory = "Iniciante", Description = "Ganhando consistência nos exercícios" },
                new Level { LevelAtual = 4, LevelCategory = "Iniciante", Description = "Progresso constante na saúde" },
                new Level { LevelAtual = 5, LevelCategory = "Iniciante", Description = "Final da fase inicial - bons hábitos estabelecidos" },
                new Level { LevelAtual = 6, LevelCategory = "Intermediário", Description = "Entrando na fase intermediária" },
                new Level { LevelAtual = 7, LevelCategory = "Intermediário", Description = "Desenvolvendo resistência física" },
                new Level { LevelAtual = 8, LevelCategory = "Intermediário", Description = "Melhorando performance geral" },
                new Level { LevelAtual = 9, LevelCategory = "Intermediário", Description = "Consolidação de técnicas avançadas" },
                new Level { LevelAtual = 10, LevelCategory = "Intermediário", Description = "Pronto para desafios maiores" },
                new Level { LevelAtual = 11, LevelCategory = "Avançado", Description = "Início da jornada avançada" },
                new Level { LevelAtual = 12, LevelCategory = "Avançado", Description = "Domínio de exercícios complexos" },
                new Level { LevelAtual = 13, LevelCategory = "Avançado", Description = "Excelência em treino cardiovascular" },
                new Level { LevelAtual = 14, LevelCategory = "Avançado", Description = "Especialização em força e resistência" },
                new Level { LevelAtual = 15, LevelCategory = "Avançado", Description = "Atleta completo em formação" },
                new Level { LevelAtual = 16, LevelCategory = "Especialista", Description = "Primeiro nível de especialista" },
                new Level { LevelAtual = 17, LevelCategory = "Especialista", Description = "Técnicas avançadas de condicionamento" },
                new Level { LevelAtual = 18, LevelCategory = "Especialista", Description = "Mestre em rotinas personalizadas" },
                new Level { LevelAtual = 19, LevelCategory = "Especialista", Description = "Referência na comunidade fitness" },
                new Level { LevelAtual = 20, LevelCategory = "Especialista", Description = "Especialista consolidado" },
                new Level { LevelAtual = 21, LevelCategory = "Mestre", Description = "Iniciando o caminho de mestre" },
                new Level { LevelAtual = 22, LevelCategory = "Mestre", Description = "Domínio completo de múltiplas modalidades" },
                new Level { LevelAtual = 23, LevelCategory = "Mestre", Description = "Liderança natural em treinos em grupo" },
                new Level { LevelAtual = 24, LevelCategory = "Mestre", Description = "Inspiração para outros utilizadores" },
                new Level { LevelAtual = 25, LevelCategory = "Mestre", Description = "Mestre em saúde e bem-estar" },
                new Level { LevelAtual = 26, LevelCategory = "Grão-Mestre", Description = "Primeiro nível de grão-mestre" },
                new Level { LevelAtual = 27, LevelCategory = "Grão-Mestre", Description = "Excelência em todos os aspectos do fitness" },
                new Level { LevelAtual = 28, LevelCategory = "Grão-Mestre", Description = "Conhecimento profundo de nutrição e exercício" },
                new Level { LevelAtual = 29, LevelCategory = "Grão-Mestre", Description = "Lenda em formação na aplicação" },
                new Level { LevelAtual = 30, LevelCategory = "Grão-Mestre", Description = "Grão-mestre consolidado" },
                new Level { LevelAtual = 31, LevelCategory = "Lendário", Description = "Entrada no hall lendário" },
                new Level { LevelAtual = 32, LevelCategory = "Lendário", Description = "Consistência lendária nos treinos" },
                new Level { LevelAtual = 33, LevelCategory = "Lendário", Description = "Performance excecional continuada" },
                new Level { LevelAtual = 34, LevelCategory = "Lendário", Description = "Ícone da aplicação" },
                new Level { LevelAtual = 35, LevelCategory = "Lendário", Description = "Lenda viva do fitness" },
                new Level { LevelAtual = 36, LevelCategory = "Mítico", Description = "Alcançando status mítico" },
                new Level { LevelAtual = 37, LevelCategory = "Mítico", Description = "Força e determinação sobre-humanas" },
                new Level { LevelAtual = 38, LevelCategory = "Mítico", Description = "Lenda entre lendas" },
                new Level { LevelAtual = 39, LevelCategory = "Mítico", Description = "Próximo do nível máximo" },
                new Level { LevelAtual = 40, LevelCategory = "Mítico", Description = "Nível máximo - Mito vivo da aplicação" }
            });
            dbContext.SaveChanges();
        }
    }
}