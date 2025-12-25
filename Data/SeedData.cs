using HealthWellbeing.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

            var clients = PopulateClients(dbContext);
            PopulateMember(dbContext, clients);
            PopulateTrainingType(dbContext);
            PopulatePlan(dbContext);

            var trainers = PopulateTrainer(dbContext);
            PopulateTraining(dbContext, trainers);

            PopulateEventTypes(dbContext);
            PopulateBadgeTypes(dbContext);
            PopulateActivityTypes(dbContext); 
            PopulateActivities(dbContext);   
            PopulateBadges(dbContext);
            PopulateEvents(dbContext);
            PopulateLevelCategories(dbContext);
            PopulateLevels(dbContext);

            PopulateEmployees(dbContext);
            PopulateCustomers(dbContext);


            PopulateEventActivities(dbContext);
            PopulateBadgeRequirements(dbContext);
            PopulateCustomerBadges(dbContext);
        }


        private static void PopulateEmployees(HealthWellbeingDbContext dbContext) {
            if (dbContext.Employee.Any()) return;

            var employees = new[]
            {
                new Employee
                {
                    Name = "Sophia Martinez Johnson",
                    Email = "admin@ipg.pt",
                    PhoneNumber = "+351 +351 910000000"
                },
                new Employee
                {
                    Name = "João Marcelo da Silva Luis",
                    Email = "joao@ipg.pt",
                    PhoneNumber = "+351 +351 910000001"
                }
            };

            foreach (var emp in employees) {
                // Avoid duplicates by email
                if (!dbContext.Employee.Any(e => e.Email == emp.Email)) {
                    dbContext.Employee.Add(emp);
                }
            }

            dbContext.SaveChanges();
        }
        private static void PopulateActivityTypes(HealthWellbeingDbContext dbContext)
        {
            if (dbContext.ActivityType.Any()) return;

            var types = new List<ActivityType>
            {
                new ActivityType { Name = "Hábito Diário", Description = "Pequenas ações repetidas todos os dias." },
                new ActivityType { Name = "Treino", Description = "Atividades físicas de média a alta intensidade." },
                new ActivityType { Name = "Nutrição", Description = "Foco na alimentação saudável e hidratação." },
                new ActivityType { Name = "Mindfulness", Description = "Saúde mental, meditação e relaxamento." },
                new ActivityType { Name = "Desafio Semanal", Description = "Metas de longo prazo." }
            };

            dbContext.ActivityType.AddRange(types);
            dbContext.SaveChanges();
        }
        private static void PopulateActivities(HealthWellbeingDbContext dbContext)
        {
            if (dbContext.Activity.Any()) return;

            // Buscar os tipos da BD
            var tHabito = dbContext.ActivityType.FirstOrDefault(t => t.Name == "Hábito Diário");
            var tTreino = dbContext.ActivityType.FirstOrDefault(t => t.Name == "Treino");
            var tNutri = dbContext.ActivityType.FirstOrDefault(t => t.Name == "Nutrição");
            var tMind = dbContext.ActivityType.FirstOrDefault(t => t.Name == "Mindfulness");
            var tDesafio = dbContext.ActivityType.FirstOrDefault(t => t.Name == "Desafio Semanal");

            if (tHabito == null) return;

            var activities = new List<Activity>
            {
                // 1. HÁBITOS DIÁRIOS (8 itens)
                new Activity { ActivityName = "Beber 2L de água", ActivityType = tHabito, ActivityDescription = "Meta diária.", ActivityReward = 20 },
                new Activity { ActivityName = "Dormir 8 horas", ActivityType = tHabito, ActivityDescription = "Sono reparador.", ActivityReward = 25 },
                new Activity { ActivityName = "Subir escadas", ActivityType = tHabito, ActivityDescription = "Evitar elevador.", ActivityReward = 15 },
                new Activity { ActivityName = "Pausa ativa", ActivityType = tHabito, ActivityDescription = "Alongar a cada 2h.", ActivityReward = 15 },
                new Activity { ActivityName = "Zero refrigerantes", ActivityType = tHabito, ActivityDescription = "Sem bebidas açucaradas.", ActivityReward = 20 },
                new Activity { ActivityName = "Caminhada curta", ActivityType = tHabito, ActivityDescription = "15 min a andar.", ActivityReward = 15 },
                new Activity { ActivityName = "Diário de bem-estar", ActivityType = tHabito, ActivityDescription = "Registar dia.", ActivityReward = 10 },
                new Activity { ActivityName = "Ler 10 páginas", ActivityType = tHabito, ActivityDescription = "Hábito de leitura.", ActivityReward = 15 },

                // 2. TREINO (8 itens)
                new Activity { ActivityName = "Treino Força 30min", ActivityType = tTreino, ActivityDescription = "Musculação.", ActivityReward = 40 },
                new Activity { ActivityName = "Cardio 30min", ActivityType = tTreino, ActivityDescription = "Corrida/Bike.", ActivityReward = 35 },
                new Activity { ActivityName = "Aula de Grupo", ActivityType = tTreino, ActivityDescription = "Zumba/Pilates.", ActivityReward = 30 },
                new Activity { ActivityName = "Treino Full Body", ActivityType = tTreino, ActivityDescription = "Corpo inteiro.", ActivityReward = 50 },
                new Activity { ActivityName = "Sessão Alongamentos", ActivityType = tTreino, ActivityDescription = "Flexibilidade.", ActivityReward = 25 },
                new Activity { ActivityName = "Corrida 5km", ActivityType = tTreino, ActivityDescription = "Outdoor.", ActivityReward = 60 },
                new Activity { ActivityName = "Ciclismo 10km", ActivityType = tTreino, ActivityDescription = "Bicicleta.", ActivityReward = 55 },
                new Activity { ActivityName = "Treino Calistenia", ActivityType = tTreino, ActivityDescription = "Peso do corpo.", ActivityReward = 45 },

                // 3. NUTRIÇÃO (8 itens)
                new Activity { ActivityName = "Sem Fast-Food", ActivityType = tNutri, ActivityDescription = "Comer limpo.", ActivityReward = 25 },
                new Activity { ActivityName = "Refeição Equilibrada", ActivityType = tNutri, ActivityDescription = "Prato saudável.", ActivityReward = 20 },
                new Activity { ActivityName = "Registo Alimentar", ActivityType = tNutri, ActivityDescription = "Log refeições.", ActivityReward = 15 },
                new Activity { ActivityName = "Zero Açúcares", ActivityType = tNutri, ActivityDescription = "Sem doces.", ActivityReward = 30 },
                new Activity { ActivityName = "3 Peças de Fruta", ActivityType = tNutri, ActivityDescription = "Vitaminas.", ActivityReward = 15 },
                new Activity { ActivityName = "Jantar Leve", ActivityType = tNutri, ActivityDescription = "Antes das 21h.", ActivityReward = 20 },
                new Activity { ActivityName = "Meal Prep Semanal", ActivityType = tNutri, ActivityDescription = "Planear semana.", ActivityReward = 35 },
                new Activity { ActivityName = "Dia Vegetariano", ActivityType = tNutri, ActivityDescription = "Sem carne.", ActivityReward = 40 },

                // 4. MINDFULNESS (8 itens)
                new Activity { ActivityName = "Meditação 10min", ActivityType = tMind, ActivityDescription = "Foco.", ActivityReward = 15 },
                new Activity { ActivityName = "Respiração Profunda", ActivityType = tMind, ActivityDescription = "5 minutos.", ActivityReward = 10 },
                new Activity { ActivityName = "Pausa Digital", ActivityType = tMind, ActivityDescription = "2h sem ecrãs.", ActivityReward = 20 },
                new Activity { ActivityName = "Passeio Natureza", ActivityType = tMind, ActivityDescription = "Ar livre.", ActivityReward = 20 },
                new Activity { ActivityName = "Gratidão", ActivityType = tMind, ActivityDescription = "3 coisas boas.", ActivityReward = 10 },
                new Activity { ActivityName = "Tempo Hobby", ActivityType = tMind, ActivityDescription = "Lazer criativo.", ActivityReward = 20 },
                new Activity { ActivityName = "Relaxamento Noturno", ActivityType = tMind, ActivityDescription = "Pré-sono.", ActivityReward = 10 },
                new Activity { ActivityName = "Gestão Stress", ActivityType = tMind, ActivityDescription = "Técnicas coping.", ActivityReward = 25 },

                // 5. DESAFIOS (8 itens)
                new Activity { ActivityName = "5 Treinos/Semana", ActivityType = tDesafio, ActivityDescription = "Consistência.", ActivityReward = 100 },
                new Activity { ActivityName = "Hidratação 7 Dias", ActivityType = tDesafio, ActivityDescription = "2L/dia.", ActivityReward = 120 },
                new Activity { ActivityName = "Semana Limpa", ActivityType = tDesafio, ActivityDescription = "Zero processados.", ActivityReward = 130 },
                new Activity { ActivityName = "3 Aulas Grupo", ActivityType = tDesafio, ActivityDescription = "Social.", ActivityReward = 90 },
                new Activity { ActivityName = "Log 5 Dias", ActivityType = tDesafio, ActivityDescription = "Diário alimentar.", ActivityReward = 80 },
                new Activity { ActivityName = "70k Passos", ActivityType = tDesafio, ActivityDescription = "Caminhada.", ActivityReward = 140 },
                new Activity { ActivityName = "3x Mindfulness", ActivityType = tDesafio, ActivityDescription = "Saúde mental.", ActivityReward = 85 },
                new Activity { ActivityName = "Sono 7 Dias", ActivityType = tDesafio, ActivityDescription = "8h/noite.", ActivityReward = 150 }
            };

            dbContext.Activity.AddRange(activities);
            dbContext.SaveChanges();
        }

        private static void PopulateCustomers(HealthWellbeingDbContext dbContext) {
            if (dbContext.Customer.Any()) return;

            var defaultLevel = dbContext.Level.FirstOrDefault(l => l.LevelNumber == 1);

            if (defaultLevel == null) return;

            var customers = new[]
            {
                new Customer { Name = "Ana Pereira", Email = "ana.pereira@ipg.pt", PhoneNumber = "+351 912345678", Gender = "Feminino", RegistrationDate = DateTime.Now.AddMonths(-12), TotalPoints = 1250 },
                new Customer { Name = "Bruno Silva", Email = "bruno.silva@ipg.pt", PhoneNumber = "+351 965874123", Gender = "Masculino", RegistrationDate = DateTime.Now.AddMonths(-1), TotalPoints = 0 },
                new Customer { Name = "Carla Santos", Email = "carla.santos@ipg.pt", PhoneNumber = "+351 932145698", Gender = "Feminino", RegistrationDate = DateTime.Now.AddMonths(-6), TotalPoints = 450 },
                new Customer { Name = "Diogo Costa", Email = "diogo.costa@ipg.pt", PhoneNumber = "+351 918745236", Gender = "Masculino", RegistrationDate = DateTime.Now.AddDays(-15), TotalPoints = 100 },
                new Customer { Name = "Elisa Martins", Email = "elisa.martins@ipg.pt", PhoneNumber = "+351 925632147", Gender = "Feminino", RegistrationDate = DateTime.Now.AddYears(-2), TotalPoints = 3500 },
                new Customer { Name = "Fábio Rocha", Email = "fabio.rocha@ipg.pt", PhoneNumber = "+351 963258741", Gender = "Masculino", RegistrationDate = DateTime.Now.AddMonths(-3), TotalPoints = 20 },
                new Customer { Name = "Gisela Nunes", Email = "gisela.nunes@ipg.pt", PhoneNumber = "+351 914785236", Gender = "Feminino", RegistrationDate = DateTime.Now.AddDays(-2), TotalPoints = 0 },
                new Customer { Name = "Hugo Almeida", Email = "hugo.almeida@ipg.pt", PhoneNumber = "+351 936547896", Gender = "Masculino", RegistrationDate = DateTime.Now.AddMonths(-8), TotalPoints = 890 },
                new Customer { Name = "Inês Rodrigues", Email = "ines.rodrigues@ipg.pt", PhoneNumber = "+351 921456987", Gender = "Feminino", RegistrationDate = DateTime.Now.AddMonths(-4), TotalPoints = 210 },
                new Customer { Name = "João Soares", Email = "joao.soares@ipg.pt", PhoneNumber = "+351 915632478", Gender = "Masculino", RegistrationDate = DateTime.Now.AddDays(-50), TotalPoints = 300 }
            };

            foreach (var cust in customers) {
                if (!dbContext.Customer.Any(c => c.Email == cust.Email)) {
                    cust.Level = defaultLevel;
                    dbContext.Customer.Add(cust);
                }
            }

            dbContext.SaveChanges();
        }

        private static void PopulateEventTypes(HealthWellbeingDbContext dbContext) {

            if (dbContext.EventType.Any()) return;

            dbContext.EventType.AddRange(new List<EventType>() {
        
                // --- Aulas de Grupo ---
                new EventType { EventTypeName = "Yoga Morning Flow", EventTypeDescription = "Sessão matinal de Vinyasa Yoga.", EventTypeMultiplier = 1.15m },
                new EventType { EventTypeName = "Pilates Mat", EventTypeDescription = "Aula de Pilates no solo.", EventTypeMultiplier = 1.12m },
                new EventType { EventTypeName = "Zumba Dance", EventTypeDescription = "Aula de dança aeróbica.", EventTypeMultiplier = 1.25m },
                new EventType { EventTypeName = "BodyPump", EventTypeDescription = "Treino de resistência com pesos.", EventTypeMultiplier = 1.45m },
                new EventType { EventTypeName = "Spinning / Cycling", EventTypeDescription = "Ciclismo indoor intenso.", EventTypeMultiplier = 1.55m },
                new EventType { EventTypeName = "Boxe / Kickboxing", EventTypeDescription = "Treino de combate cardio.", EventTypeMultiplier = 1.65m },
                new EventType { EventTypeName = "Aula de Barre", EventTypeDescription = "Fusão de ballet e pilates.", EventTypeMultiplier = 1.28m },
                new EventType { EventTypeName = "Hidroginástica", EventTypeDescription = "Exercício na piscina.", EventTypeMultiplier = 1.18m },
                new EventType { EventTypeName = "Meditação Guiada", EventTypeDescription = "Prática de mindfulness.", EventTypeMultiplier = 1.05m },
                new EventType { EventTypeName = "CrossFit WOD", EventTypeDescription = "Treino do dia funcional.", EventTypeMultiplier = 1.95m },

                // --- Corrida e Cardio ---
                new EventType { EventTypeName = "Caminhada 5K", EventTypeDescription = "Caminhada vigorosa.", EventTypeMultiplier = 1.10m },
                new EventType { EventTypeName = "Corrida 5K", EventTypeDescription = "Corrida curta distância.", EventTypeMultiplier = 1.45m },
                new EventType { EventTypeName = "Corrida 10K", EventTypeDescription = "Corrida média distância.", EventTypeMultiplier = 1.75m },
                new EventType { EventTypeName = "Meia Maratona", EventTypeDescription = "Prova de 21km.", EventTypeMultiplier = 2.45m },
                new EventType { EventTypeName = "Maratona", EventTypeDescription = "Prova de 42km.", EventTypeMultiplier = 2.95m },
                new EventType { EventTypeName = "Natação Livre", EventTypeDescription = "Natação em piscina.", EventTypeMultiplier = 1.62m },
                new EventType { EventTypeName = "Ciclismo Estrada", EventTypeDescription = "Passeio de bicicleta.", EventTypeMultiplier = 1.68m },
                new EventType { EventTypeName = "Triatlo Sprint", EventTypeDescription = "Natação, bike e corrida.", EventTypeMultiplier = 2.75m },
                new EventType { EventTypeName = "Hiking / Trilho", EventTypeDescription = "Caminhada na natureza.", EventTypeMultiplier = 1.35m },
                new EventType { EventTypeName = "Trail Running", EventTypeDescription = "Corrida em trilhos.", EventTypeMultiplier = 1.98m },

                // --- Ginásio e Força ---
                new EventType { EventTypeName = "Musculação (Hipertrofia)", EventTypeDescription = "Treino de força.", EventTypeMultiplier = 1.48m },
                new EventType { EventTypeName = "Powerlifting", EventTypeDescription = "Treino de força máxima.", EventTypeMultiplier = 2.55m },
                new EventType { EventTypeName = "Treino Funcional", EventTypeDescription = "Circuitos funcionais.", EventTypeMultiplier = 1.38m },
                new EventType { EventTypeName = "Personal Training", EventTypeDescription = "Sessão 1 para 1.", EventTypeMultiplier = 1.95m },
                new EventType { EventTypeName = "Calistenia", EventTypeDescription = "Peso do corpo.", EventTypeMultiplier = 1.58m },
                new EventType { EventTypeName = "Halterofilismo", EventTypeDescription = "Levantamento olímpico.", EventTypeMultiplier = 2.15m },
                new EventType { EventTypeName = "HIIT", EventTypeDescription = "Intervalado alta intensidade.", EventTypeMultiplier = 1.72m },
                new EventType { EventTypeName = "Core / Abdominais", EventTypeDescription = "Foco na zona média.", EventTypeMultiplier = 1.15m },
                new EventType { EventTypeName = "Mobilidade / alongamentos", EventTypeDescription = "Recuperação ativa.", EventTypeMultiplier = 1.02m },
                new EventType { EventTypeName = "Kettlebell Flow", EventTypeDescription = "Treino com kettlebells.", EventTypeMultiplier = 1.42m },

                // --- Desportos e Bem-estar ---
                new EventType { EventTypeName = "Futebol 5", EventTypeDescription = "Jogo com amigos.", EventTypeMultiplier = 1.55m },
                new EventType { EventTypeName = "Ténis", EventTypeDescription = "Partida de ténis.", EventTypeMultiplier = 1.48m },
                new EventType { EventTypeName = "Padel", EventTypeDescription = "Jogo de padel.", EventTypeMultiplier = 1.38m },
                new EventType { EventTypeName = "Basquetebol", EventTypeDescription = "Jogo de basquete.", EventTypeMultiplier = 1.65m },
                new EventType { EventTypeName = "Escalada Indoor", EventTypeDescription = "Bouldering ou corda.", EventTypeMultiplier = 1.85m },
                new EventType { EventTypeName = "Workshop Nutrição", EventTypeDescription = "Aprender a comer bem.", EventTypeMultiplier = 1.08m },
                new EventType { EventTypeName = "Dia Zero Açúcar", EventTypeDescription = "Hábito saudável.", EventTypeMultiplier = 1.15m },
                new EventType { EventTypeName = "Hidratação Meta", EventTypeDescription = "Beber 2L+ água.", EventTypeMultiplier = 1.05m },
                new EventType { EventTypeName = "Higiene do Sono", EventTypeDescription = "Dormir 8 horas.", EventTypeMultiplier = 1.12m },
                new EventType { EventTypeName = "Banho Gelado", EventTypeDescription = "Recuperação a frio.", EventTypeMultiplier = 1.25m }
            });

            dbContext.SaveChanges();
        }

        private static void PopulateEvents(HealthWellbeingDbContext dbContext)
        {
            if (dbContext.Event.Any()) return;

            var eventTypes = dbContext.EventType.ToList();
            if (!eventTypes.Any()) return;

            // 1. Buscar os Objetos REAIS para ter os IDs
            var tTreino = dbContext.ActivityType.FirstOrDefault(t => t.Name == "Treino");
            var tNutri = dbContext.ActivityType.FirstOrDefault(t => t.Name == "Nutrição");
            var tMind = dbContext.ActivityType.FirstOrDefault(t => t.Name == "Mindfulness");
            var tDesafio = dbContext.ActivityType.FirstOrDefault(t => t.Name == "Desafio Semanal");

            var now = DateTime.Now;
            var eventList = new List<Event>();

            // --- EVENTOS DE TREINO ---
            if (tTreino != null)
            {
                eventList.Add(new Event { EventName = "Competição CrossFit", EventDescription = "Teste limites.", EventTypeId = eventTypes[0].EventTypeId, ActivityTypeId = tTreino.ActivityTypeId, MinLevel = 4, EventStart = now.AddDays(-10), EventEnd = now.AddDays(-10).AddHours(4), EventPoints = 0 });
                eventList.Add(new Event { EventName = "Torneio Ténis", EventDescription = "Pares.", EventTypeId = eventTypes[4].EventTypeId, ActivityTypeId = tTreino.ActivityTypeId, MinLevel = 3, EventStart = now.AddDays(-5), EventEnd = now.AddDays(-5).AddHours(3), EventPoints = 0 });
                eventList.Add(new Event { EventName = "Meia Maratona", EventDescription = "21km.", EventTypeId = eventTypes[7].EventTypeId, ActivityTypeId = tTreino.ActivityTypeId, MinLevel = 5, EventStart = now.AddDays(2), EventEnd = now.AddDays(2).AddHours(4), EventPoints = 0 });
                eventList.Add(new Event { EventName = "Campeonato Natação", EventDescription = "Estilos.", EventTypeId = eventTypes[8].EventTypeId, ActivityTypeId = tTreino.ActivityTypeId, MinLevel = 4, EventStart = now.AddDays(5), EventEnd = now.AddDays(5).AddHours(2), EventPoints = 0 });
                eventList.Add(new Event { EventName = "Liga Basquetebol", EventDescription = "Jornada 1.", EventTypeId = eventTypes[16].EventTypeId, ActivityTypeId = tTreino.ActivityTypeId, MinLevel = 2, EventStart = now.AddDays(7), EventEnd = now.AddDays(7).AddHours(2), EventPoints = 0 });
                eventList.Add(new Event { EventName = "Torneio Padel", EventDescription = "Open.", EventTypeId = eventTypes[36].EventTypeId, ActivityTypeId = tTreino.ActivityTypeId, MinLevel = 2, EventStart = now.AddDays(10), EventEnd = now.AddDays(10).AddHours(3), EventPoints = 0 });
                eventList.Add(new Event { EventName = "Desafio Powerlifting", EventDescription = "Peso máximo.", EventTypeId = eventTypes[11].EventTypeId, ActivityTypeId = tTreino.ActivityTypeId, MinLevel = 5, EventStart = now.AddDays(12), EventEnd = now.AddDays(12).AddHours(3), EventPoints = 0 });
                eventList.Add(new Event { EventName = "Competição Remo", EventDescription = "Indoor.", EventTypeId = eventTypes[33].EventTypeId, ActivityTypeId = tTreino.ActivityTypeId, MinLevel = 3, EventStart = now.AddDays(15), EventEnd = now.AddDays(15).AddHours(2), EventPoints = 0 });
                eventList.Add(new Event { EventName = "Torneio Voleibol", EventDescription = "Praia.", EventTypeId = eventTypes[12].EventTypeId, ActivityTypeId = tTreino.ActivityTypeId, MinLevel = 2, EventStart = now.AddDays(18), EventEnd = now.AddDays(18).AddHours(4), EventPoints = 0 });
                eventList.Add(new Event { EventName = "Desafio Sprint", EventDescription = "100m.", EventTypeId = eventTypes[15].EventTypeId, ActivityTypeId = tTreino.ActivityTypeId, MinLevel = 1, EventStart = now.AddDays(20), EventEnd = now.AddDays(20).AddHours(1), EventPoints = 0 });
                eventList.Add(new Event { EventName = "Aula Zumba", EventDescription = "Dança.", EventTypeId = eventTypes[2].EventTypeId, ActivityTypeId = tTreino.ActivityTypeId, MinLevel = 1, EventStart = now.AddDays(1), EventEnd = now.AddDays(1).AddHours(1), EventPoints = 0 });
                eventList.Add(new Event { EventName = "Yoga Morning", EventDescription = "Flexibilidade.", EventTypeId = eventTypes[0].EventTypeId, ActivityTypeId = tTreino.ActivityTypeId, MinLevel = 1, EventStart = now.AddDays(3), EventEnd = now.AddDays(3).AddHours(1), EventPoints = 0 });
                eventList.Add(new Event { EventName = "Treino HIIT", EventDescription = "Intenso.", EventTypeId = eventTypes[18].EventTypeId, ActivityTypeId = tTreino.ActivityTypeId, MinLevel = 3, EventStart = now.AddDays(4), EventEnd = now.AddDays(4).AddHours(1), EventPoints = 0 });
                eventList.Add(new Event { EventName = "Aula Pilates", EventDescription = "Core.", EventTypeId = eventTypes[25].EventTypeId, ActivityTypeId = tTreino.ActivityTypeId, MinLevel = 1, EventStart = now.AddDays(6), EventEnd = now.AddDays(6).AddHours(1), EventPoints = 0 });
                eventList.Add(new Event { EventName = "Spinning", EventDescription = "Montanha.", EventTypeId = eventTypes[21].EventTypeId, ActivityTypeId = tTreino.ActivityTypeId, MinLevel = 2, EventStart = now.AddDays(8), EventEnd = now.AddDays(8).AddHours(1), EventPoints = 0 });
                eventList.Add(new Event { EventName = "Técnica Corrida", EventDescription = "Postura.", EventTypeId = eventTypes[14].EventTypeId, ActivityTypeId = tTreino.ActivityTypeId, MinLevel = 2, EventStart = now.AddDays(9), EventEnd = now.AddDays(9).AddHours(1), EventPoints = 0 });
                eventList.Add(new Event { EventName = "Personal Trainer", EventDescription = "Sessão.", EventTypeId = eventTypes[6].EventTypeId, ActivityTypeId = tTreino.ActivityTypeId, MinLevel = 1, EventStart = now.AddDays(11), EventEnd = now.AddDays(11).AddHours(1), EventPoints = 0 });
                eventList.Add(new Event { EventName = "Artes Marciais", EventDescription = "Defesa.", EventTypeId = eventTypes[17].EventTypeId, ActivityTypeId = tTreino.ActivityTypeId, MinLevel = 2, EventStart = now.AddDays(13), EventEnd = now.AddDays(13).AddHours(1), EventPoints = 0 });
                eventList.Add(new Event { EventName = "Flexibilidade", EventDescription = "Alongar.", EventTypeId = eventTypes[34].EventTypeId, ActivityTypeId = tTreino.ActivityTypeId, MinLevel = 1, EventStart = now.AddDays(14), EventEnd = now.AddDays(14).AddHours(1), EventPoints = 0 });
                eventList.Add(new Event { EventName = "Boxe Fitness", EventDescription = "Cardio.", EventTypeId = eventTypes[38].EventTypeId, ActivityTypeId = tTreino.ActivityTypeId, MinLevel = 2, EventStart = now.AddDays(16), EventEnd = now.AddDays(16).AddHours(1), EventPoints = 0 });
            }

            // --- EVENTOS DE NUTRIÇÃO ---
            if (tNutri != null)
            {
                eventList.Add(new Event { EventName = "Workshop Nutrição", EventDescription = "Básico.", EventTypeId = eventTypes[1].EventTypeId, ActivityTypeId = tNutri.ActivityTypeId, MinLevel = 1, EventStart = now.AddDays(5), EventEnd = now.AddDays(5).AddHours(2), EventPoints = 0 });
                eventList.Add(new Event { EventName = "Cozinha Saudável", EventDescription = "Receitas.", EventTypeId = eventTypes[1].EventTypeId, ActivityTypeId = tNutri.ActivityTypeId, MinLevel = 1, EventStart = now.AddDays(12), EventEnd = now.AddDays(12).AddHours(3), EventPoints = 0 });
                eventList.Add(new Event { EventName = "Detox Day", EventDescription = "Limpeza.", EventTypeId = eventTypes[1].EventTypeId, ActivityTypeId = tNutri.ActivityTypeId, MinLevel = 2, EventStart = now.AddDays(19), EventEnd = now.AddDays(19).AddHours(8), EventPoints = 0 });
                eventList.Add(new Event { EventName = "Seminário Suplementos", EventDescription = "Info.", EventTypeId = eventTypes[9].EventTypeId, ActivityTypeId = tNutri.ActivityTypeId, MinLevel = 3, EventStart = now.AddDays(22), EventEnd = now.AddDays(22).AddHours(2), EventPoints = 0 });
                eventList.Add(new Event { EventName = "Planeamento Refeições", EventDescription = "Meal Prep.", EventTypeId = eventTypes[1].EventTypeId, ActivityTypeId = tNutri.ActivityTypeId, MinLevel = 1, EventStart = now.AddDays(25), EventEnd = now.AddDays(25).AddHours(2), EventPoints = 0 });
                eventList.Add(new Event { EventName = "Conferência Médica", EventDescription = "Saúde.", EventTypeId = eventTypes[24].EventTypeId, ActivityTypeId = tNutri.ActivityTypeId, MinLevel = 4, EventStart = now.AddDays(28), EventEnd = now.AddDays(28).AddHours(4), EventPoints = 0 });
                eventList.Add(new Event { EventName = "Workshop Primeiros Socorros", EventDescription = "Emergência.", EventTypeId = eventTypes[20].EventTypeId, ActivityTypeId = tNutri.ActivityTypeId, MinLevel = 1, EventStart = now.AddDays(30), EventEnd = now.AddDays(30).AddHours(3), EventPoints = 0 });
                eventList.Add(new Event { EventName = "Feira Biológica", EventDescription = "Produtos.", EventTypeId = eventTypes[1].EventTypeId, ActivityTypeId = tNutri.ActivityTypeId, MinLevel = 1, EventStart = now.AddDays(32), EventEnd = now.AddDays(32).AddHours(5), EventPoints = 0 });
                eventList.Add(new Event { EventName = "Aula Hidratação", EventDescription = "Importância.", EventTypeId = eventTypes[9].EventTypeId, ActivityTypeId = tNutri.ActivityTypeId, MinLevel = 1, EventStart = now.AddDays(35), EventEnd = now.AddDays(35).AddHours(1), EventPoints = 0 });
                eventList.Add(new Event { EventName = "Degustação Vegan", EventDescription = "Provais.", EventTypeId = eventTypes[1].EventTypeId, ActivityTypeId = tNutri.ActivityTypeId, MinLevel = 1, EventStart = now.AddDays(38), EventEnd = now.AddDays(38).AddHours(2), EventPoints = 0 });
            }

            // --- EVENTOS DE MINDFULNESS ---
            if (tMind != null)
            {
                eventList.Add(new Event { EventName = "Seminário Mental", EventDescription = "Psicologia.", EventTypeId = eventTypes[5].EventTypeId, ActivityTypeId = tMind.ActivityTypeId, MinLevel = 1, EventStart = now.AddDays(2), EventEnd = now.AddDays(2).AddHours(2), EventPoints = 0 });
                eventList.Add(new Event { EventName = "Retiro Yoga", EventDescription = "Fim de semana.", EventTypeId = eventTypes[10].EventTypeId, ActivityTypeId = tMind.ActivityTypeId, MinLevel = 2, EventStart = now.AddDays(15), EventEnd = now.AddDays(17).AddHours(0), EventPoints = 0 });
                eventList.Add(new Event { EventName = "Meditação Guiada", EventDescription = "Grupo.", EventTypeId = eventTypes[13].EventTypeId, ActivityTypeId = tMind.ActivityTypeId, MinLevel = 1, EventStart = now.AddDays(4), EventEnd = now.AddDays(4).AddHours(1), EventPoints = 0 });
                eventList.Add(new Event { EventName = "Torneio Xadrez", EventDescription = "Foco.", EventTypeId = eventTypes[23].EventTypeId, ActivityTypeId = tMind.ActivityTypeId, MinLevel = 1, EventStart = now.AddDays(18), EventEnd = now.AddDays(18).AddHours(4), EventPoints = 0 });
                eventList.Add(new Event { EventName = "Tiro ao Arco", EventDescription = "Concentração.", EventTypeId = eventTypes[22].EventTypeId, ActivityTypeId = tMind.ActivityTypeId, MinLevel = 2, EventStart = now.AddDays(21), EventEnd = now.AddDays(21).AddHours(2), EventPoints = 0 });
                eventList.Add(new Event { EventName = "Orientação Outdoor", EventDescription = "Natureza.", EventTypeId = eventTypes[37].EventTypeId, ActivityTypeId = tMind.ActivityTypeId, MinLevel = 2, EventStart = now.AddDays(24), EventEnd = now.AddDays(24).AddHours(3), EventPoints = 0 });
                eventList.Add(new Event { EventName = "Workshop Stress", EventDescription = "Gestão.", EventTypeId = eventTypes[5].EventTypeId, ActivityTypeId = tMind.ActivityTypeId, MinLevel = 1, EventStart = now.AddDays(27), EventEnd = now.AddDays(27).AddHours(2), EventPoints = 0 });
                eventList.Add(new Event { EventName = "Dança Contemp.", EventDescription = "Expressão.", EventTypeId = eventTypes[32].EventTypeId, ActivityTypeId = tMind.ActivityTypeId, MinLevel = 1, EventStart = now.AddDays(29), EventEnd = now.AddDays(29).AddHours(2), EventPoints = 0 });
                eventList.Add(new Event { EventName = "Palestra Motivacional", EventDescription = "Coaching.", EventTypeId = eventTypes[9].EventTypeId, ActivityTypeId = tMind.ActivityTypeId, MinLevel = 1, EventStart = now.AddDays(36), EventEnd = now.AddDays(36).AddHours(2), EventPoints = 0 });
            }

            // --- EVENTOS DE DESAFIO ---
            if (tDesafio != null)
            {
                eventList.Add(new Event { EventName = "Competição E-Sports", EventDescription = "FIFA.", EventTypeId = eventTypes[39].EventTypeId, ActivityTypeId = tDesafio.ActivityTypeId, MinLevel = 1, EventStart = now.AddDays(33), EventEnd = now.AddDays(33).AddHours(5), EventPoints = 0 });
            }

            dbContext.Event.AddRange(eventList);
            dbContext.SaveChanges();
        }

        private static void PopulateEventActivities(HealthWellbeingDbContext dbContext)
        {
            if (dbContext.EventActivity.Any()) return;

            var events = dbContext.Event.ToList();
            var activities = dbContext.Activity.ToList();

            if (!events.Any() || !activities.Any()) return;

            var random = new Random();

            foreach (var evt in events)
            {
                if (evt.ActivityTypeId == null) continue;

                // --- CORREÇÃO: Comparar ID com ID ---
                var matchingActivities = activities
                    .Where(a => a.ActivityTypeId == evt.ActivityTypeId)
                    .ToList();

                if (matchingActivities.Any())
                {
                    int numToSelect = random.Next(1, 3);
                    var selected = matchingActivities.OrderBy(x => random.Next()).Take(numToSelect).ToList();
                    int totalPoints = 0;

                    foreach (var act in selected)
                    {
                        dbContext.EventActivity.Add(new EventActivity { EventId = evt.EventId, ActivityId = act.ActivityId });
                        totalPoints += act.ActivityReward;
                    }

                    // Atualiza os pontos
                    evt.EventPoints = totalPoints;
                }
            }
            dbContext.SaveChanges();
        }

        private static void PopulateLevels(HealthWellbeingDbContext dbContext) {
            // 1. Se já existirem níveis, sai
            if (dbContext.Level.Any()) return;

            // 2. Buscar as categorias (que sabemos que existem porque o PopulateLevelCategories correu antes)
            // Usamos um Dicionário ou variáveis locais para facilitar a atribuição abaixo
            var catBeginner = dbContext.LevelCategory.First(c => c.Name == "Beginner");
            var catIntermediate = dbContext.LevelCategory.First(c => c.Name == "Intermediate");
            var catAdvanced = dbContext.LevelCategory.First(c => c.Name == "Advanced");
            var catExpert = dbContext.LevelCategory.First(c => c.Name == "Expert");
            var catMaster = dbContext.LevelCategory.First(c => c.Name == "Master");
            var catGrandmaster = dbContext.LevelCategory.First(c => c.Name == "Grandmaster");
            var catLegendary = dbContext.LevelCategory.First(c => c.Name == "Legendary");
            var catMythic = dbContext.LevelCategory.First(c => c.Name == "Mythic");

            // 3. Criar os Níveis
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

        private static void PopulateLevelCategories(HealthWellbeingDbContext dbContext) {
            
            if (dbContext.LevelCategory.Any()) return;

            dbContext.LevelCategory.AddRange(
                new LevelCategory { Name = "Beginner" },
                new LevelCategory { Name = "Intermediate" },
                new LevelCategory { Name = "Advanced" },
                new LevelCategory { Name = "Expert" },
                new LevelCategory { Name = "Master" },
                new LevelCategory { Name = "Grandmaster" },
                new LevelCategory { Name = "Legendary" },
                new LevelCategory { Name = "Mythic" }
            );

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

        private static void PopulateTraining(HealthWellbeingDbContext dbContext, List<Trainer> trainers) {
            if (dbContext.Training.Any()) return;

            var yogaTypeId = dbContext.TrainingType.FirstOrDefault(t => t.Name == "Yoga Basics")?.TrainingTypeId;
            var hiitTypeId = dbContext.TrainingType.FirstOrDefault(t => t.Name == "HIIT (High Intensity Interval Training)")?.TrainingTypeId;

            var carlosId = trainers.FirstOrDefault(t => t.Name == "Carlos Mendes")?.TrainerId;
            var johnId = trainers.FirstOrDefault(t => t.Name == "John Smith")?.TrainerId;


            if (yogaTypeId.HasValue && hiitTypeId.HasValue && carlosId.HasValue && johnId.HasValue) {
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
        private static void PopulateBadgeTypes(HealthWellbeingDbContext dbContext) {
            // Se já existirem dados, não faz nada para evitar duplicados
            if (dbContext.BadgeType.Any()) return;

            dbContext.BadgeType.AddRange(new List<BadgeType>
            {
                // --- 1. Os Originais (Mantidos para compatibilidade com PopulateBadges) ---
                new BadgeType { BadgeTypeName = "Consistência", BadgeTypeDescription = "Premiar a regularidade e criação de hábitos saudáveis." },
                new BadgeType { BadgeTypeName = "Evento", BadgeTypeDescription = "Participação em eventos especiais do ginásio." },
                new BadgeType { BadgeTypeName = "Performance", BadgeTypeDescription = "Atingir marcos de superação física." },
                new BadgeType { BadgeTypeName = "Social", BadgeTypeDescription = "Interação e espírito de comunidade." },

                // --- 2. Categorias de Treino Específico ---
                new BadgeType { BadgeTypeName = "Força", BadgeTypeDescription = "Conquistas relacionadas com musculação, powerlifting e ganhos de força." },
                new BadgeType { BadgeTypeName = "Cardio", BadgeTypeDescription = "Distinções para atividades de resistência cardiovascular (corrida, ciclismo, natação)." },
                new BadgeType { BadgeTypeName = "Flexibilidade", BadgeTypeDescription = "Evolução em Yoga, Pilates e mobilidade geral." },
                new BadgeType { BadgeTypeName = "Alta Intensidade", BadgeTypeDescription = "Domínio de treinos HIIT e Crossfit." },

                // --- 3. Bem-Estar e Saúde Mental ---
                new BadgeType { BadgeTypeName = "Mindfulness", BadgeTypeDescription = "Foco na saúde mental, meditação e gestão de stress." },
                new BadgeType { BadgeTypeName = "Nutrição", BadgeTypeDescription = "Hábitos alimentares saudáveis, cozinha e escolhas inteligentes." },
                new BadgeType { BadgeTypeName = "Hidratação", BadgeTypeDescription = "Foco exclusivo na ingestão correta de líquidos." },
                new BadgeType { BadgeTypeName = "Sono & Recuperação", BadgeTypeDescription = "Qualidade do descanso e práticas de recuperação ativa." },

                // --- 4. Gamification & Lifestyle ---
                new BadgeType { BadgeTypeName = "Desafio", BadgeTypeDescription = "Conclusão de desafios temporários lançados pela plataforma." },
                new BadgeType { BadgeTypeName = "Fidelidade", BadgeTypeDescription = "Recompensas por tempo de permanência e antiguidade no clube." },
                new BadgeType { BadgeTypeName = "Iniciação", BadgeTypeDescription = "Passos iniciais para novos membros se ambientarem." },
                new BadgeType { BadgeTypeName = "Exploração", BadgeTypeDescription = "Experimentar novas modalidades e sair da zona de conforto." },

                // --- 5. Horários e Hábitos ---
                new BadgeType { BadgeTypeName = "Madrugador", BadgeTypeDescription = "Para quem treina consistentemente nas primeiras horas do dia." },
                new BadgeType { BadgeTypeName = "Notívago", BadgeTypeDescription = "Para quem prefere treinar ao final do dia ou noite." },
                new BadgeType { BadgeTypeName = "Fim de Semana", BadgeTypeDescription = "Atividade física mantida aos sábados e domingos." },
                new BadgeType { BadgeTypeName = "Ao Ar Livre", BadgeTypeDescription = "Conexão com a natureza, trilhos e desporto outdoor." }
            });

            dbContext.SaveChanges();
        }

        private static void PopulateBadges(HealthWellbeingDbContext dbContext) {
            if (dbContext.Badge.Any()) return;

            // 1. Buscar os IDs dos Pais (BadgeTypes) que acabámos de criar
            var tConsistencia = dbContext.BadgeType.FirstOrDefault(t => t.BadgeTypeName == "Consistência");
            var tEvento = dbContext.BadgeType.FirstOrDefault(t => t.BadgeTypeName == "Evento");
            var tPerformance = dbContext.BadgeType.FirstOrDefault(t => t.BadgeTypeName == "Performance");

            // Safety Check: Se não encontrou os tipos, não pode criar badges
            if (tConsistencia == null || tEvento == null || tPerformance == null) return;

            dbContext.Badge.AddRange(new List<Badge>
            {
        // Badges de Consistência
        new Badge {
            BadgeName = "Rato de Ginásio",
            BadgeDescription = "Participar ativamente na vida do clube.",
            RewardPoints = 500,
            BadgeTypeId = tConsistencia.BadgeTypeId
        },
        new Badge {
            BadgeName = "Hidratação Mestra",
            BadgeDescription = "Criar o hábito diário de beber água.",
            RewardPoints = 300,
            BadgeTypeId = tConsistencia.BadgeTypeId
        },

        // Badges de Evento
        new Badge {
            BadgeName = "Maratonista",
            BadgeDescription = "Completar a grande prova de resistência.",
            RewardPoints = 1000,
            BadgeTypeId = tEvento.BadgeTypeId
        },
        new Badge {
            BadgeName = "Yogi Master",
            BadgeDescription = "Dedicação total à mente e corpo.",
            RewardPoints = 600,
            BadgeTypeId = tEvento.BadgeTypeId
        },

        // Badges de Performance
        new Badge {
            BadgeName = "Monstro do Cardio",
            BadgeDescription = "Acumular quilómetros de corrida.",
            RewardPoints = 800,
            BadgeTypeId = tPerformance.BadgeTypeId
        }
    });

            dbContext.SaveChanges();
        }

        private static void PopulateBadgeRequirements(HealthWellbeingDbContext dbContext) {
            if (dbContext.BadgeRequirement.Any()) return;

            // 1. Carregar referências para não termos NullReferenceExceptions
            var bRato = dbContext.Badge.FirstOrDefault(b => b.BadgeName == "Rato de Ginásio");
            var bHidra = dbContext.Badge.FirstOrDefault(b => b.BadgeName == "Hidratação Mestra");
            var bMara = dbContext.Badge.FirstOrDefault(b => b.BadgeName == "Maratonista");
            var bYogi = dbContext.Badge.FirstOrDefault(b => b.BadgeName == "Yogi Master");
            var bCardio = dbContext.Badge.FirstOrDefault(b => b.BadgeName == "Monstro do Cardio");

            var etYoga = dbContext.EventType.FirstOrDefault(e => e.EventTypeName == "Yoga Morning Flow");
            var etMaratona = dbContext.EventType.FirstOrDefault(e => e.EventTypeName == "Maratona");
            var etMeia = dbContext.EventType.FirstOrDefault(e => e.EventTypeName == "Meia Maratona");
            var etZumba = dbContext.EventType.FirstOrDefault(e => e.EventTypeName == "Zumba Dance");
            var etSpinning = dbContext.EventType.FirstOrDefault(e => e.EventTypeName == "Spinning / Cycling");

            var atHabito = dbContext.ActivityType.FirstOrDefault(a => a.Name == "Hábito Diário");
            var atTreino = dbContext.ActivityType.FirstOrDefault(a => a.Name == "Treino");
            var atMind = dbContext.ActivityType.FirstOrDefault(a => a.Name == "Mindfulness");

            // Validação de segurança básica
            if (bRato == null || bHidra == null || bMara == null || bYogi == null || bCardio == null) return;

            var requirements = new List<BadgeRequirement>
            {
                // --- RATO DE GINÁSIO (Consistência Geral) ---
                new BadgeRequirement { BadgeId = bRato.BadgeId, BadgeRequirementName = "Presença Assídua", RequirementDescription = "Participar em qualquer evento do ginásio.", TargetValue = 20, RequirementType = RequirementType.ParticipateAnyEvent },
                new BadgeRequirement { BadgeId = bRato.BadgeId, BadgeRequirementName = "Rei da Pista", RequirementDescription = "Participar nas aulas de Zumba.", TargetValue = 5, RequirementType = RequirementType.ParticipateSpecificEventType, EventTypeId = etZumba?.EventTypeId },
                new BadgeRequirement { BadgeId = bRato.BadgeId, BadgeRequirementName = "Ativo e Saudável", RequirementDescription = "Completar qualquer tipo de atividade registada.", TargetValue = 50, RequirementType = RequirementType.CompleteAnyActivity },

                // --- HIDRATAÇÃO MESTRA (Foco em Hábito) ---
                new BadgeRequirement { BadgeId = bHidra.BadgeId, BadgeRequirementName = "Rotina de Água", RequirementDescription = "Registar atividades de Hábito Diário (ex: Beber Água).", TargetValue = 30, RequirementType = RequirementType.CompleteSpecificActivityType, ActivityTypeId = atHabito?.ActivityTypeId },
                new BadgeRequirement { BadgeId = bHidra.BadgeId, BadgeRequirementName = "Persistência Hídrica", RequirementDescription = "Manter o registo de atividades por 60 vezes.", TargetValue = 60, RequirementType = RequirementType.CompleteAnyActivity },
                new BadgeRequirement { BadgeId = bHidra.BadgeId, BadgeRequirementName = "Participação em Workshops", RequirementDescription = "Ir a qualquer evento para aprender mais.", TargetValue = 3, RequirementType = RequirementType.ParticipateAnyEvent },

                // --- MARATONISTA (Foco em Eventos Específicos) ---
                new BadgeRequirement { BadgeId = bMara.BadgeId, BadgeRequirementName = "A Grande Prova", RequirementDescription = "Completar a Maratona oficial.", TargetValue = 1, RequirementType = RequirementType.ParticipateSpecificEventType, EventTypeId = etMaratona?.EventTypeId },
                new BadgeRequirement { BadgeId = bMara.BadgeId, BadgeRequirementName = "Preparação Meia Distância", RequirementDescription = "Completar a Meia Maratona.", TargetValue = 2, RequirementType = RequirementType.ParticipateSpecificEventType, EventTypeId = etMeia?.EventTypeId },
                new BadgeRequirement { BadgeId = bMara.BadgeId, BadgeRequirementName = "Volume de Treino", RequirementDescription = "Registar 100 atividades de treino.", TargetValue = 100, RequirementType = RequirementType.CompleteSpecificActivityType, ActivityTypeId = atTreino?.ActivityTypeId },

                // --- YOGI MASTER (Mente e Corpo) ---
                new BadgeRequirement { BadgeId = bYogi.BadgeId, BadgeRequirementName = "Manhãs Zen", RequirementDescription = "Participar nas aulas de Yoga Morning Flow.", TargetValue = 10, RequirementType = RequirementType.ParticipateSpecificEventType, EventTypeId = etYoga?.EventTypeId },
                new BadgeRequirement { BadgeId = bYogi.BadgeId, BadgeRequirementName = "Mente Sã", RequirementDescription = "Realizar atividades de Mindfulness.", TargetValue = 20, RequirementType = RequirementType.CompleteSpecificActivityType, ActivityTypeId = atMind?.ActivityTypeId },
                new BadgeRequirement { BadgeId = bYogi.BadgeId, BadgeRequirementName = "Espírito de Grupo", RequirementDescription = "Participar num total de 15 eventos quaisquer.", TargetValue = 15, RequirementType = RequirementType.ParticipateAnyEvent },

                // --- MONSTRO DO CARDIO (Intensidade) ---
                new BadgeRequirement { BadgeId = bCardio.BadgeId, BadgeRequirementName = "Rei do Pedal", RequirementDescription = "Aulas de Spinning intensas.", TargetValue = 15, RequirementType = RequirementType.ParticipateSpecificEventType, EventTypeId = etSpinning?.EventTypeId },
                new BadgeRequirement { BadgeId = bCardio.BadgeId, BadgeRequirementName = "Máquina de Treino", RequirementDescription = "Registar qualquer atividade de treino físico.", TargetValue = 50, RequirementType = RequirementType.CompleteSpecificActivityType, ActivityTypeId = atTreino?.ActivityTypeId },
                new BadgeRequirement { BadgeId = bCardio.BadgeId, BadgeRequirementName = "Dedicação Total", RequirementDescription = "Completar 200 atividades no total.", TargetValue = 200, RequirementType = RequirementType.CompleteAnyActivity }
            };

            if (requirements.Any()) {
                dbContext.BadgeRequirement.AddRange(requirements);
                dbContext.SaveChanges();
            }
        }

        private static void PopulateCustomerBadges(HealthWellbeingDbContext dbContext) {
            // 1. Se já existirem associações, não faz nada
            if (dbContext.CustomerBadge.Any()) return;

            // 2. Carregar dados necessários (Customers e Badges)
            var customers = dbContext.Customer.ToList();
            var badges = dbContext.Badge.ToList();

            if (!customers.Any() || !badges.Any()) return;

            // Referências aos Badges criados anteriormente
            var bRato = badges.FirstOrDefault(b => b.BadgeName == "Rato de Ginásio");
            var bHidra = badges.FirstOrDefault(b => b.BadgeName == "Hidratação Mestra");
            var bMara = badges.FirstOrDefault(b => b.BadgeName == "Maratonista");
            var bYogi = badges.FirstOrDefault(b => b.BadgeName == "Yogi Master");
            var bCardio = badges.FirstOrDefault(b => b.BadgeName == "Monstro do Cardio"); // Vamos deixar este vazio ou quase vazio para testes

            var customerBadges = new List<CustomerBadge>();
            var random = new Random();

            // --- CENÁRIO A: "Rato de Ginásio" (MUITO POPULAR) ---
            // Atribuir aos primeiros 20 clientes (quase toda a gente tem)
            // Isto serve para testar o alerta vermelho no Delete.
            if (bRato != null) {
                foreach (var c in customers.Take(20)) {
                    customerBadges.Add(new CustomerBadge {
                        CustomerId = c.CustomerId,
                        BadgeId = bRato.BadgeId,
                        DateAwarded = DateTime.Now.AddDays(-random.Next(1, 100))
                    });
                }
            }

            // --- CENÁRIO B: "Hidratação Mestra" (MÉDIA POPULARIDADE) ---
            // Atribuir a clientes alternados (índices pares)
            if (bHidra != null) {
                for (int i = 0; i < customers.Count; i += 2) {
                    customerBadges.Add(new CustomerBadge {
                        CustomerId = customers[i].CustomerId,
                        BadgeId = bHidra.BadgeId,
                        DateAwarded = DateTime.Now.AddDays(-random.Next(1, 50))
                    });
                }
            }

            // --- CENÁRIO C: "Maratonista" (ESPECÍFICO / ELITE) ---
            // Atribuir apenas a perfis atléticos (Steve Rogers, Xena, etc.)
            if (bMara != null) {
                var athletes = customers.Where(c =>
                    c.Name.Contains("Steve") ||
                    c.Name.Contains("Xena") ||
                    c.Name.Contains("Wonderland") ||
                    c.Name.Contains("Inês")
                ).ToList();

                foreach (var a in athletes) {
                    customerBadges.Add(new CustomerBadge {
                        CustomerId = a.CustomerId,
                        BadgeId = bMara.BadgeId,
                        DateAwarded = DateTime.Now.AddDays(-10)
                    });
                }
            }

            // --- CENÁRIO D: "Yogi Master" (RARO) ---
            // Atribuir apenas ao Yoda (testar "Used by 1 customer")
            if (bYogi != null) {
                var yoda = customers.FirstOrDefault(c => c.Name.Contains("Yoda"));
                if (yoda != null) {
                    customerBadges.Add(new CustomerBadge {
                        CustomerId = yoda.CustomerId,
                        BadgeId = bYogi.BadgeId,
                        DateAwarded = DateTime.Now.AddYears(-2)
                    });
                }
            }

            // --- CENÁRIO E: "Monstro do Cardio" ---
            // NÃO VAMOS ATRIBUIR A NINGUÉM (ou deixa vazio).
            // Isto serve para quando fores ao Delete deste badge, veres a mensagem verde "Safe to Delete".

            // Garantir que não há duplicados antes de adicionar (caso a lógica acima se sobreponha)
            var uniqueBadges = customerBadges
                .GroupBy(cb => new { cb.CustomerId, cb.BadgeId })
                .Select(g => g.First())
                .ToList();

            dbContext.CustomerBadge.AddRange(uniqueBadges);
            dbContext.SaveChanges();
        }

        internal static void SeedDefaultAdmin(UserManager<IdentityUser> userManager) {
            EnsureUserIsCreatedAsync(userManager, "admin@ipg.pt", "Secret123$", ["Gestor"]).Wait();
        }

        private static async Task EnsureUserIsCreatedAsync(UserManager<IdentityUser> userManager, string username, string password, string[] roles) {
            IdentityUser? user = await userManager.FindByNameAsync(username);

            if (user == null) {
                user = new IdentityUser(username);
                await userManager.CreateAsync(user, password);
            }

            foreach (var role in roles) {
                if (!await userManager.IsInRoleAsync(user, role)) {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }

        internal static void SeedUsers(UserManager<IdentityUser> userManager) {
            EnsureUserIsCreatedAsync(userManager, "joao@ipg.pt", "Secret123$", ["Treinador"]).Wait();
            EnsureUserIsCreatedAsync(userManager, "maria@ipg.pt", "Secret123$", ["Utente"]).Wait();
            EnsureUserIsCreatedAsync(userManager, "paulo@ipg.pt", "Secret123$", ["Utente"]).Wait();
        }

        internal static void SeedRoles(RoleManager<IdentityRole> roleManager) {
            EnsureRoleIsCreatedAsync(roleManager, "Gestor").Wait();
            EnsureRoleIsCreatedAsync(roleManager, "Treinador").Wait();
            EnsureRoleIsCreatedAsync(roleManager, "Utente").Wait();
        }

        private static async Task EnsureRoleIsCreatedAsync(RoleManager<IdentityRole> roleManager, string role) {
            if (!await roleManager.RoleExistsAsync(role)) {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}
