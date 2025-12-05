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

            dbContext.Database.EnsureCreated();

            // Add seed data here
            var clients = PopulateClients(dbContext);
            PopulateMember(dbContext, clients);
            PopulateTrainingType(dbContext);
            PopulatePlan(dbContext);

            // --- ALTERAÇÃO AQUI: Capturamos a lista de Trainers ---
            var trainers = PopulateTrainer(dbContext);

            // --- NOVO MÉTODO: Povoamento dos Treinos Agendados ---
            PopulateTraining(dbContext, trainers);

            PopulateEventTypes(dbContext);
            PopulateEvents(dbContext);
            PopulateLevels(dbContext);
        }

        private static void PopulateEventTypes(HealthWellbeingDbContext dbContext)
        {
            if (dbContext.EventType.Any()) return;

            dbContext.EventType.AddRange(new List<EventType>() {
                //EDUCAÇÃO E FORMAÇÃO
                new EventType { EventTypeName = "Workshop Educacional", EventTypeScoringMode = "fixed", EventTypeMultiplier = 1.0m },
                new EventType { EventTypeName = "Seminário Temático", EventTypeScoringMode = "fixed", EventTypeMultiplier = 1.1m },
                new EventType { EventTypeName = "Palestra Informativa", EventTypeScoringMode = "fixed", EventTypeMultiplier = 1.0m },
                new EventType { EventTypeName = "Demonstração Técnica", EventTypeScoringMode = "fixed", EventTypeMultiplier = 1.0m },
                new EventType { EventTypeName = "Sessão de Orientação", EventTypeScoringMode = "fixed", EventTypeMultiplier = 1.0m },

                //TREINO CARDIOVASCULAR
                new EventType { EventTypeName = "Sessão de Running", EventTypeScoringMode = "time_based", EventTypeMultiplier = 2.0m },
                new EventType { EventTypeName = "Treino de Cycling", EventTypeScoringMode = "time_based", EventTypeMultiplier = 2.1m },
                new EventType { EventTypeName = "Aula de Cardio-Grupo", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.5m },
                new EventType { EventTypeName = "Treino de Natação", EventTypeScoringMode = "time_based", EventTypeMultiplier = 2.2m },
                new EventType { EventTypeName = "Sessão de HIIT", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.8m },

                //TREINO DE FORÇA
                new EventType { EventTypeName = "Treino de Musculação", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.7m },
                new EventType { EventTypeName = "Sessão de CrossFit", EventTypeScoringMode = "progressive", EventTypeMultiplier = 2.1m },
                new EventType { EventTypeName = "Treino Funcional", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.6m },
                new EventType { EventTypeName = "Aula de Powerlifting", EventTypeScoringMode = "progressive", EventTypeMultiplier = 2.0m },
                new EventType { EventTypeName = "Treino de Calistenia", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.5m },

                //BEM-ESTAR E MOBILIDADE
                new EventType { EventTypeName = "Aula de Yoga", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.3m },
                new EventType { EventTypeName = "Sessão de Pilates", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.4m },
                new EventType { EventTypeName = "Treino de Flexibilidade", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.5m },
                new EventType { EventTypeName = "Aula de Mobilidade", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.3m },
                new EventType { EventTypeName = "Sessão de Alongamento", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.2m },

                //DESPORTOS E ARTES MARCIAS
                new EventType { EventTypeName = "Aula de Artes Marciais", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.8m },
                new EventType { EventTypeName = "Treino de Boxe", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.9m },
                new EventType { EventTypeName = "Sessão de Lutas", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.8m },
                new EventType { EventTypeName = "Aula de Defesa Pessoal", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.4m },
                new EventType { EventTypeName = "Treino Desportivo Coletivo", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.6m },

                //DESAFIOS E COMPETIÇÕES
                new EventType { EventTypeName = "Competição de Running", EventTypeScoringMode = "time_based", EventTypeMultiplier = 2.5m },
                new EventType { EventTypeName = "Torneio Desportivo", EventTypeScoringMode = "binary", EventTypeMultiplier = 2.3m },
                new EventType { EventTypeName = "Desafio de Resistência", EventTypeScoringMode = "time_based", EventTypeMultiplier = 2.4m },
                new EventType { EventTypeName = "Competição de Força", EventTypeScoringMode = "progressive", EventTypeMultiplier = 2.2m },
                new EventType { EventTypeName = "Desafio de Superação", EventTypeScoringMode = "progressive", EventTypeMultiplier = 2.1m },

                //ATIVIDADES EM GRUPO
                new EventType { EventTypeName = "Aula de Grupo", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.4m },
                new EventType { EventTypeName = "Treino Coletivo", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.5m },
                new EventType { EventTypeName = "Workshop Prático", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.3m },
                new EventType { EventTypeName = "Sessão de Team Building", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.2m },
                new EventType { EventTypeName = "Aula Experimental", EventTypeScoringMode = "binary", EventTypeMultiplier = 1.1m },

                //ESPECIALIZADOS E TÉCNICOS
                new EventType { EventTypeName = "Treino Técnico", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.6m },
                new EventType { EventTypeName = "Workshop de Técnica", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.3m },
                new EventType { EventTypeName = "Aula Avançada", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.7m },
                new EventType { EventTypeName = "Sessão de Perfeiçoamento", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.5m },
                new EventType { EventTypeName = "Treino Especializado", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.8m }
            });

            dbContext.SaveChanges();
        }

        private static void PopulateEvents(HealthWellbeingDbContext dbContext)
        {
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

        private static void PopulateLevels(HealthWellbeingDbContext dbContext)
        {
            // 1. Check if data already exists to prevent duplication
            if (dbContext.Level.Any()) return;

            // 2. Create the Category Objects first
            var catBeginner = new LevelCategory { Name = "Beginner" };
            var catIntermediate = new LevelCategory { Name = "Intermediate" };
            var catAdvanced = new LevelCategory { Name = "Advanced" };
            var catExpert = new LevelCategory { Name = "Expert" };
            var catMaster = new LevelCategory { Name = "Master" };
            var catGrandmaster = new LevelCategory { Name = "Grandmaster" };
            var catLegendary = new LevelCategory { Name = "Legendary" };
            var catMythic = new LevelCategory { Name = "Mythic" };

            // 3. Add Categories to DB and Save (so they get IDs)
            if (!dbContext.LevelCategory.Any())
            {
                dbContext.LevelCategory.AddRange(catBeginner, catIntermediate, catAdvanced, catExpert, catMaster, catGrandmaster, catLegendary, catMythic);
                dbContext.SaveChanges();
            }
            else
            {
                // If categories already exist (e.g. from previous run), fetch them to get the IDs
                catBeginner = dbContext.LevelCategory.First(c => c.Name == "Beginner");
                catIntermediate = dbContext.LevelCategory.First(c => c.Name == "Intermediate");
                catAdvanced = dbContext.LevelCategory.First(c => c.Name == "Advanced");
                catExpert = dbContext.LevelCategory.First(c => c.Name == "Expert");
                catMaster = dbContext.LevelCategory.First(c => c.Name == "Master");
                catGrandmaster = dbContext.LevelCategory.First(c => c.Name == "Grandmaster");
                catLegendary = dbContext.LevelCategory.First(c => c.Name == "Legendary");
                catMythic = dbContext.LevelCategory.First(c => c.Name == "Mythic");
            }

            // 4. Create Levels using the Category IDs
            dbContext.Level.AddRange(new List<Level>() {
        // Beginner
        new Level { LevelNumber = 1, LevelCategoryId = catBeginner.LevelCategoryId, LevelPointsLimit = 1000, Description = "First steps in the health journey" },
        new Level { LevelNumber = 2, LevelCategoryId = catBeginner.LevelCategoryId, LevelPointsLimit = 1200, Description = "Starting to build healthy routines" },
        new Level { LevelNumber = 3, LevelCategoryId = catBeginner.LevelCategoryId, LevelPointsLimit = 1400, Description = "Gaining consistency in workouts" },
        new Level { LevelNumber = 4, LevelCategoryId = catBeginner.LevelCategoryId, LevelPointsLimit = 1600, Description = "Steady progress in your well-being" },
        new Level { LevelNumber = 5, LevelCategoryId = catBeginner.LevelCategoryId, LevelPointsLimit = 1800, Description = "End of beginner phase – solid habits established" },

        // Intermediate
        new Level { LevelNumber = 6, LevelCategoryId = catIntermediate.LevelCategoryId, LevelPointsLimit = 2000, Description = "Entering the intermediate stage" },
        new Level { LevelNumber = 7, LevelCategoryId = catIntermediate.LevelCategoryId, LevelPointsLimit = 2600, Description = "Developing physical endurance" },
        new Level { LevelNumber = 8, LevelCategoryId = catIntermediate.LevelCategoryId, LevelPointsLimit = 3200, Description = "Improving overall performance" },
        new Level { LevelNumber = 9, LevelCategoryId = catIntermediate.LevelCategoryId, LevelPointsLimit = 3800, Description = "Consolidating advanced techniques" },
        new Level { LevelNumber = 10, LevelCategoryId = catIntermediate.LevelCategoryId, LevelPointsLimit = 4400, Description = "Ready for tougher challenges" },

        // Advanced
        new Level { LevelNumber = 11, LevelCategoryId = catAdvanced.LevelCategoryId, LevelPointsLimit = 5000, Description = "Beginning the advanced journey" },
        new Level { LevelNumber = 12, LevelCategoryId = catAdvanced.LevelCategoryId, LevelPointsLimit = 6000, Description = "Mastering complex exercises" },
        new Level { LevelNumber = 13, LevelCategoryId = catAdvanced.LevelCategoryId, LevelPointsLimit = 7000, Description = "Excellence in cardiovascular training" },
        new Level { LevelNumber = 14, LevelCategoryId = catAdvanced.LevelCategoryId, LevelPointsLimit = 8000, Description = "Specializing in strength and endurance" },
        new Level { LevelNumber = 15, LevelCategoryId = catAdvanced.LevelCategoryId, LevelPointsLimit = 9000, Description = "Forming a complete athlete" },

        // Expert
        new Level { LevelNumber = 16, LevelCategoryId = catExpert.LevelCategoryId, LevelPointsLimit = 10000, Description = "First expert-level achievement" },
        new Level { LevelNumber = 17, LevelCategoryId = catExpert.LevelCategoryId, LevelPointsLimit = 11000, Description = "Advanced conditioning techniques" },
        new Level { LevelNumber = 18, LevelCategoryId = catExpert.LevelCategoryId, LevelPointsLimit = 12000, Description = "Master of personalized routines" },
        new Level { LevelNumber = 19, LevelCategoryId = catExpert.LevelCategoryId, LevelPointsLimit = 13000, Description = "A reference within the fitness community" },
        new Level { LevelNumber = 20, LevelCategoryId = catExpert.LevelCategoryId, LevelPointsLimit = 14000, Description = "Established expert" },

        // Master
        new Level { LevelNumber = 21, LevelCategoryId = catMaster.LevelCategoryId, LevelPointsLimit = 15000, Description = "Beginning the path of a master" },
        new Level { LevelNumber = 22, LevelCategoryId = catMaster.LevelCategoryId, LevelPointsLimit = 16000, Description = "Complete mastery across multiple disciplines" },
        new Level { LevelNumber = 23, LevelCategoryId = catMaster.LevelCategoryId, LevelPointsLimit = 17000, Description = "Natural leader in group workouts" },
        new Level { LevelNumber = 24, LevelCategoryId = catMaster.LevelCategoryId, LevelPointsLimit = 18000, Description = "An inspiration to other users" },
        new Level { LevelNumber = 25, LevelCategoryId = catMaster.LevelCategoryId, LevelPointsLimit = 19000, Description = "Recognized master in health and wellness" },

        // Grandmaster
        new Level { LevelNumber = 26, LevelCategoryId = catGrandmaster.LevelCategoryId, LevelPointsLimit = 20000, Description = "First grandmaster level" },
        new Level { LevelNumber = 27, LevelCategoryId = catGrandmaster.LevelCategoryId, LevelPointsLimit = 22000, Description = "Excellence in all aspects of fitness" },
        new Level { LevelNumber = 28, LevelCategoryId = catGrandmaster.LevelCategoryId, LevelPointsLimit = 24000, Description = "Deep knowledge of nutrition and exercise" },
        new Level { LevelNumber = 29, LevelCategoryId = catGrandmaster.LevelCategoryId, LevelPointsLimit = 26000, Description = "A legend in the making" },
        new Level { LevelNumber = 30, LevelCategoryId = catGrandmaster.LevelCategoryId, LevelPointsLimit = 28000, Description = "A fully established grandmaster" },

        // Legendary
        new Level { LevelNumber = 31, LevelCategoryId = catLegendary.LevelCategoryId, LevelPointsLimit = 30000, Description = "Entering the legendary hall" },
        new Level { LevelNumber = 32, LevelCategoryId = catLegendary.LevelCategoryId, LevelPointsLimit = 34000, Description = "Legendary consistency in training" },
        new Level { LevelNumber = 33, LevelCategoryId = catLegendary.LevelCategoryId, LevelPointsLimit = 38000, Description = "Outstanding long-term performance" },
        new Level { LevelNumber = 34, LevelCategoryId = catLegendary.LevelCategoryId, LevelPointsLimit = 42000, Description = "A true icon of the app" },
        new Level { LevelNumber = 35, LevelCategoryId = catLegendary.LevelCategoryId, LevelPointsLimit = 46000, Description = "A living fitness legend" },

        // Mythic
        new Level { LevelNumber = 36, LevelCategoryId = catMythic.LevelCategoryId, LevelPointsLimit = 50000, Description = "Achieving mythic status" },
        new Level { LevelNumber = 37, LevelCategoryId = catMythic.LevelCategoryId, LevelPointsLimit = 55000, Description = "Superhuman strength and determination" },
        new Level { LevelNumber = 38, LevelCategoryId = catMythic.LevelCategoryId, LevelPointsLimit = 60000, Description = "A legend among legends" },
        new Level { LevelNumber = 39, LevelCategoryId = catMythic.LevelCategoryId, LevelPointsLimit = 65000, Description = "Close to the maximum level" },
        new Level { LevelNumber = 40, LevelCategoryId = catMythic.LevelCategoryId, LevelPointsLimit = 70000, Description = "Maximum level – A living myth of the app" }
    });

            dbContext.SaveChanges();
        }

        private static List<Client> PopulateClients(HealthWellbeingDbContext dbContext)
        {
            // Verifica se já existem clientes para não duplicar
            if (dbContext.Client.Any())
            {
                return dbContext.Client.ToList();
            }

            // Lista com 25 clientes
            var clients = new List<Client>()
            {
                // Os seus 5 clientes originais
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Alice Wonderland",
                    Email = "alice.w@example.com",
                    Phone = "555-1234567",
                    Address = "10 Downing St, London",
                    BirthDate = new DateTime(1990, 5, 15),
                    Gender = "Female",
                    RegistrationDate = DateTime.Now.AddDays(-30)
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Bob The Builder",
                    Email = "bob.builder@work.net",
                    Phone = "555-9876543",
                    Address = "Construction Site 5A",
                    BirthDate = new DateTime(1985, 10, 20),
                    Gender = "Male",
                    RegistrationDate = DateTime.Now.AddDays(-15),
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Charlie Brown",
                    Email = "charlie.b@peanuts.com",
                    Phone = "555-4567890",
                    Address = "123 Comic Strip Ave",
                    BirthDate = new DateTime(2000, 1, 1),
                    Gender = "Male",
                    RegistrationDate = DateTime.Now.AddDays(-5)
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "David Copperfield",
                    Email = "david.c@magic.com",
                    Phone = "555-9001002",
                    Address = "Las Vegas Strip",
                    BirthDate = new DateTime(1960, 9, 16),
                    Gender = "Male",
                    RegistrationDate = DateTime.Now.AddDays(-25)
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Eve Harrington",
                    Email = "eve.h@stage.net",
                    Phone = "555-3330009",
                    Address = "Broadway St",
                    BirthDate = new DateTime(1995, 2, 28),
                    Gender = "Female",
                    RegistrationDate = DateTime.Now.AddDays(-10)
                },
        
                // Mais 20 clientes para teste
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Frank Castle",
                    Email = "frank.c@punisher.com",
                    Phone = "555-1110001",
                    Address = "Hells Kitchen, NY",
                    BirthDate = new DateTime(1978, 3, 16),
                    Gender = "Male",
                    RegistrationDate = DateTime.Now.AddDays(-40)
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Grace Hopper",
                    Email = "grace.h@navy.mil",
                    Phone = "555-2220002",
                    Address = "Arlington, VA",
                    BirthDate = new DateTime(1906, 12, 9),
                    Gender = "Female",
                    RegistrationDate = DateTime.Now.AddDays(-100)
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Harry Potter",
                    Email = "harry.p@hogwarts.wiz",
                    Phone = "555-3330003",
                    Address = "4 Privet Drive, Surrey",
                    BirthDate = new DateTime(1980, 7, 31),
                    Gender = "Male",
                    RegistrationDate = DateTime.Now.AddDays(-12)
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Ivy Poison",
                    Email = "ivy.p@gotham.bio",
                    Phone = "555-4440004",
                    Address = "Gotham Gardens",
                    BirthDate = new DateTime(1988, 11, 2),
                    Gender = "Female",
                    RegistrationDate = DateTime.Now.AddDays(-3)
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Jack Sparrow",
                    Email = "jack.s@pirate.sea",
                    Phone = "555-5550005",
                    Address = "Tortuga",
                    BirthDate = new DateTime(1700, 4, 1),
                    Gender = "Male",
                    RegistrationDate = DateTime.Now.AddDays(-8)
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Kara Danvers",
                    Email = "kara.d@catco.com",
                    Phone = "555-6660006",
                    Address = "National City",
                    BirthDate = new DateTime(1993, 9, 22),
                    Gender = "Female",
                    RegistrationDate = DateTime.Now.AddDays(-22)
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Luke Skywalker",
                    Email = "luke.s@jedi.org",
                    Phone = "555-7770007",
                    Address = "Tatooine",
                    BirthDate = new DateTime(1977, 5, 25),
                    Gender = "Male",
                    RegistrationDate = DateTime.Now.AddDays(-18)
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Mona Lisa",
                    Email = "mona.l@art.com",
                    Phone = "555-8880008",
                    Address = "The Louvre, Paris",
                    BirthDate = new DateTime(1503, 6, 15),
                    Gender = "Female",
                    RegistrationDate = DateTime.Now.AddDays(-50)
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Neo Anderson",
                    Email = "neo.a@matrix.com",
                    Phone = "555-9990009",
                    Address = "Zion",
                    BirthDate = new DateTime(1971, 9, 13),
                    Gender = "Male",
                    RegistrationDate = DateTime.Now.AddDays(-2)
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Olivia Pope",
                    Email = "olivia.p@gladiator.com",
                    Phone = "555-1010010",
                    Address = "Washington D.C.",
                    BirthDate = new DateTime(1977, 4, 2),
                    Gender = "Female",
                    RegistrationDate = DateTime.Now.AddDays(-60)
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Peter Parker",
                    Email = "peter.p@bugle.com",
                    Phone = "555-2020011",
                    Address = "Queens, NY",
                    BirthDate = new DateTime(2001, 8, 10),
                    Gender = "Male",
                    RegistrationDate = DateTime.Now.AddDays(-7)
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Quinn Fabray",
                    Email = "quinn.f@glee.com",
                    Phone = "555-3030012",
                    Address = "Lima, Ohio",
                    BirthDate = new DateTime(1994, 7, 19),
                    Gender = "Female",
                    RegistrationDate = DateTime.Now.AddDays(-33)
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Rachel Green",
                    Email = "rachel.g@friends.com",
                    Phone = "555-4040013",
                    Address = "Central Perk, NY",
                    BirthDate = new DateTime(1970, 5, 5),
                    Gender = "Female",
                    RegistrationDate = DateTime.Now.AddDays(-45)
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Steve Rogers",
                    Email = "steve.r@avengers.com",
                    Phone = "555-5050014",
                    Address = "Brooklyn, NY",
                    BirthDate = new DateTime(1918, 7, 4),
                    Gender = "Male",
                    RegistrationDate = DateTime.Now.AddDays(-11)
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Tony Stark",
                    Email = "tony.s@stark.com",
                    Phone = "555-6060015",
                    Address = "Malibu Point, CA",
                    BirthDate = new DateTime(1970, 5, 29),
                    Gender = "Male",
                    RegistrationDate = DateTime.Now.AddDays(-90)
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Ursula Buffay",
                    Email = "ursula.b@friends.tv",
                    Phone = "555-7070016",
                    Address = "Riff's Bar, NY",
                    BirthDate = new DateTime(1968, 2, 22),
                    Gender = "Female",
                    RegistrationDate = DateTime.Now.AddDays(-14)
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Victor Frankenstein",
                    Email = "victor.f@science.ch",
                    Phone = "555-8080017",
                    Address = "Geneva, Switzerland",
                    BirthDate = new DateTime(1790, 10, 10),
                    Gender = "Male",
                    RegistrationDate = DateTime.Now.AddDays(-200)
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Walter White",
                    Email = "walter.w@heisenberg.com",
                    Phone = "555-9090018",
                    Address = "Albuquerque, NM",
                    BirthDate = new DateTime(1958, 9, 7),
                    Gender = "Male",
                    RegistrationDate = DateTime.Now.AddDays(-28)
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Xena Warrior",
                    Email = "xena.w@myth.gr",
                    Phone = "555-0100019",
                    Address = "Amphipolis, Greece",
                    BirthDate = new DateTime(1968, 3, 29),
                    Gender = "Female",
                    RegistrationDate = DateTime.Now.AddDays(-1)
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Yoda Master",
                    Email = "yoda.m@jedi.org",
                    Phone = "555-1210020",
                    Address = "Dagobah System",
                    BirthDate = new DateTime(1000, 1, 1),
                    Gender = "Male",
                    RegistrationDate = DateTime.Now.AddDays(-500)
                }
            };

            // Adiciona todos os clientes à base de dados
            dbContext.Client.AddRange(clients);
            dbContext.SaveChanges();

            return clients;
        }

        private static void PopulateMember(HealthWellbeingDbContext dbContext, List<Client> clients)
        {
            if (dbContext.Member.Any()) return;

            var clientNamesToMakeMembers = new List<string> { "Alice Wonderland", "Charlie Brown", "David Copperfield" };

            var members = clients
                .Where(c => clientNamesToMakeMembers.Contains(c.Name))
                .Select(c => new Member
                {
                    ClientId = c.ClientId,
                })
                .ToList();

            if (members.Any())
            {
                dbContext.Member.AddRange(members);
                dbContext.SaveChanges();
            }
        }

        private static void PopulateTrainingType(HealthWellbeingDbContext dbContext)
        {
            // Check if the TrainingType table already contains data
            if (dbContext.TrainingType.Any()) return;

            dbContext.TrainingType.AddRange(new List<TrainingType>()
            {
                new TrainingType
                {
                    Name = "Yoga Basics",
                    Description = "A gentle introduction to yoga, focusing on flexibility, balance, and relaxation.",
                    DurationMinutes = 60,
                    IsActive = true
                },
                new TrainingType
                {
                    Name = "HIIT (High Intensity Interval Training)",
                    Description = "A fast-paced training session combining cardio and strength exercises for maximum calorie burn.",
                    DurationMinutes = 45,
                    IsActive = true
                },
                new TrainingType
                {
                    Name = "Pilates Core Strength",
                    Description = "Focus on core muscle strength, flexibility, and posture improvement.",
                    DurationMinutes = 50,
                    IsActive = true
                },
                new TrainingType
                {
                    Name = "Zumba Dance",
                    Description = "Fun and energetic dance workout set to upbeat Latin music.",
                    DurationMinutes = 55,
                    IsActive = true
                },
                new TrainingType
                {
                    Name = "Strength Training",
                    Description = "Weight-based training for building muscle mass and endurance.",
                    DurationMinutes = 120,
                    IsActive = true
                }
            });

            dbContext.SaveChanges();
        }

        private static void PopulatePlan(HealthWellbeingDbContext dbContext)
        {
            // Check if the Plan table already contains data
            if (dbContext.Plan.Any()) return;

            dbContext.Plan.AddRange(new List<Plan>()
            {
                new Plan
                {
                    Name = "Basic Wellness Plan",
                    Description = "A beginner-friendly plan including 3 workouts per week focused on flexibility and general health.",
                    Price = 29.99m,
                    DurationDays = 30
                },
                new Plan
                {
                    Name = "Advanced Fitness Plan",
                    Description = "An intensive 6-week plan designed for strength, endurance, and fat loss.",
                    Price = 59.99m,
                    DurationDays = 45
                },
                new Plan
                {
                    Name = "Mind & Body Balance",
                    Description = "A 2-month program combining yoga, meditation, and Pilates for mental and physical harmony.",
                    Price = 79.99m,
                    DurationDays = 60
                },
                new Plan
                {
                    Name = "Ultimate Transformation Plan",
                    Description = "A 3-month premium plan featuring personal coaching, nutrition guidance, and high-intensity training.",
                    Price = 99.99m,
                    DurationDays = 90
                },
                new Plan
                {
                    Name = "Corporate Health Boost",
                    Description = "A 1-month team-focused plan to improve workplace wellness, stress management, and physical activity.",
                    Price = 49.99m,
                    DurationDays = 30
                }
            });

            dbContext.SaveChanges();
        }

        // --- ALTERAÇÃO AQUI: O método agora retorna List<Trainer> ---
        private static List<Trainer> PopulateTrainer(HealthWellbeingDbContext dbContext)
        {
            // Check if Trainers already exist
            if (dbContext.Trainer.Any()) return dbContext.Trainer.ToList(); // Retorna se existirem

            dbContext.Trainer.AddRange(new List<Trainer>()
            {
                new Trainer
                {
                    Name = "John Smith",
                    Speciality = "HIIT (High Intensity Interval Training)",
                    Email = "john.smith@fitnesspro.com",
                    Phone = "555-1112233",
                    BirthDate = new DateTime(1988, 7, 10),
                    Gender = "Male"
                },
                new Trainer
                {
                    Name = "Emma Johnson",
                    Speciality = "Strength Training",
                    Email = "emma.johnson@strongfit.net",
                    Phone = "555-2223344",
                    BirthDate = new DateTime(1992, 11, 25),
                    Gender = "Female"
                },
                new Trainer
                {
                    Name = "Carlos Mendes",
                    Speciality = "Yoga Basics",
                    Email = "carlos.mendes@yogabalance.org",
                    Phone = "555-3334455",
                    BirthDate = new DateTime(1975, 4, 1),
                    Gender = "Male"
                },
                new Trainer
                {
                    Name = "Sophie Lee",
                    Speciality = "Pilates Core Strength",
                    Email = "sophie.lee@corewellness.com",
                    Phone = "555-4445566",
                    BirthDate = new DateTime(1996, 2, 14),
                    Gender = "Female"
                },
                new Trainer
                {
                    Name = "Maria Rodriguez",
                    Speciality = "Zumba Dance",
                    Email = "maria.rodriguez@zumbafit.com",
                    Phone = "555-5557788",
                    BirthDate = new DateTime(1985, 9, 30),
                    Gender = "Female"
                }
            });

            dbContext.SaveChanges();
            return dbContext.Trainer.ToList();
        }

        private static void PopulateTraining(HealthWellbeingDbContext dbContext, List<Trainer> trainers)
        {
            if (dbContext.Training.Any()) return;

            var yogaTypeId = dbContext.TrainingType.FirstOrDefault(t => t.Name == "Yoga Basics")?.TrainingTypeId;
            var hiitTypeId = dbContext.TrainingType.FirstOrDefault(t => t.Name == "HIIT (High Intensity Interval Training)")?.TrainingTypeId;

            var carlosId = trainers.FirstOrDefault(t => t.Name == "Carlos Mendes")?.TrainerId;
            var johnId = trainers.FirstOrDefault(t => t.Name == "John Smith")?.TrainerId;


            if (yogaTypeId.HasValue && hiitTypeId.HasValue && carlosId.HasValue && johnId.HasValue)
            {
                dbContext.Training.AddRange(new List<Training>()
                {
                    new Training
                    {
                        TrainingTypeId = yogaTypeId.Value,
                        TrainerId = carlosId.Value,
                        Name = "Morning Yoga",
                        Duration = 60,
                        DayOfWeek = "Monday",
                        StartTime = new TimeSpan(10, 0, 0),
                        MaxParticipants = 15
                    },
                    new Training
                    {
                        TrainingTypeId = hiitTypeId.Value,
                        TrainerId = johnId.Value,
                        Name = "Intense Cardio HIT",
                        Duration = 45,
                        DayOfWeek = "Wednesday",
                        StartTime = new TimeSpan(18, 30, 0),
                        MaxParticipants = 20
                    },
                     new Training
                    {
                        TrainingTypeId = hiitTypeId.Value,
                        TrainerId = johnId.Value,
                        Name = "Strength Training",
                        Duration = 120,
                        DayOfWeek = "Friday",
                        StartTime = new TimeSpan(16, 0, 0),
                        MaxParticipants = 8
                    }
                });

                dbContext.SaveChanges();
            }
        }
    }
}