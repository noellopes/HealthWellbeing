using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthWellbeing.Data
{
    internal class SeedDataGinasio
    {
        internal static void Populate(HealthWellbeingDbContext? dbContext)
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));

            dbContext.Database.EnsureCreated();

            // 1. DADOS BASE
            var clients = PopulateClients(dbContext);
            var plans = PopulatePlan(dbContext);
            var trainers = PopulateTrainer(dbContext);
            var trainingTypes = PopulateTrainingType(dbContext);
            var exercises = PopulateExercise(dbContext);

            // 2. MEMBROS
            var members = PopulateMember(dbContext, clients);

            // 3. AULAS (TRAININGS)
            List<Training> trainings = new List<Training>();
            if (trainers.Any())
            {
                trainings = PopulateTraining(dbContext, trainers, trainingTypes);

                // Associa Planos a Aulas
                PopulateTrainingPlan(dbContext, plans, trainings);

                PopulateTrainingExercise(dbContext, trainings, exercises);
            }

            // 4. ASSOCIAÇÕES FINAIS
            PopulateMemberPlan(dbContext, members, plans);
            PopulatePhysicalAssessment(dbContext, members, trainers);

            //Falta o session
        }

        private static List<Client> PopulateClients(HealthWellbeingDbContext dbContext)
        {
            // Se já tiver clientes, devolve a lista existente para não duplicar
            if (dbContext.Client.Any()) return dbContext.Client.ToList();

            var clients = new List<Client>()
            {
                new Client { Name = "Alice Wonderland", Email = "alice.w@example.com", Phone = "912345678", Address = "London", BirthDate = new DateTime(1990, 5, 15), Gender = "Female", RegistrationDate = DateTime.Now.AddDays(-300) },
                new Client { Name = "Bob The Builder", Email = "bob.builder@work.net", Phone = "919876543", Address = "Construction Site 5A", BirthDate = new DateTime(1985, 10, 20), Gender = "Male", RegistrationDate = DateTime.Now.AddDays(-150) },
                // ... Mantive a sua lista resumida para poupar espaço, mas pode colar a sua lista gigante aqui ...
                new Client { Name = "Charlie Brown", Email = "charlie.b@peanuts.com", Phone = "914567890", Address = "123 Comic Strip Ave", BirthDate = new DateTime(2000, 1, 1), Gender = "Male", RegistrationDate = DateTime.Now.AddDays(-50) },
                new Client { Name = "Frank Castle", Email = "frank.c@punisher.com", Phone = "911110001", Address = "Hells Kitchen, NY", BirthDate = new DateTime(1978, 3, 16), Gender = "Male", RegistrationDate = DateTime.Now.AddDays(-40) },
                new Client { Name = "Tony Stark", Email = "tony.s@stark.com", Phone = "916060015", Address = "Malibu Point, CA", BirthDate = new DateTime(1970, 5, 29), Gender = "Male", RegistrationDate = DateTime.Now.AddDays(-90) }
            };

            dbContext.Client.AddRange(clients);
            dbContext.SaveChanges();
            return clients;
        }

        private static List<Member> PopulateMember(HealthWellbeingDbContext dbContext, List<Client> clients)
        {
            if (dbContext.Member.Any()) return dbContext.Member.ToList();

            var members = new List<Member>();
            // Cria membros a partir dos primeiros 5 clientes
            foreach (var client in clients.Take(5))
            {
                members.Add(new Member { ClientId = client.ClientId });
            }

            dbContext.Member.AddRange(members);
            dbContext.SaveChanges();
            return members;
        }

        private static List<TrainingType> PopulateTrainingType(HealthWellbeingDbContext dbContext)
        {
            if (dbContext.TrainingType.Any()) return dbContext.TrainingType.ToList();

            var types = new List<TrainingType>
            {
                new TrainingType { Name = "Yoga Basics", Description = "Focus on flexibility.", MaxParticipants = 15, IsActive = true },
                new TrainingType { Name = "HIIT", Description = "High Intensity.", MaxParticipants = 20, IsActive = true },
                new TrainingType { Name = "Pilates Core", Description = "Core strength.", MaxParticipants = 12, IsActive = true },
                new TrainingType { Name = "Zumba Dance", Description = "Dance workout.", MaxParticipants = 30, IsActive = true },
                new TrainingType { Name = "Strength Training", Description = "Weight lifting.", MaxParticipants = 10, IsActive = true },
                new TrainingType { Name = "Indoor Cycling", Description = "Stationary bike.", MaxParticipants = 18, IsActive = true },
                new TrainingType { Name = "Boxing", Description = "Technique.", MaxParticipants = 16, IsActive = true },
                new TrainingType { Name = "Aqua Aerobics", Description = "Pool workout.", MaxParticipants = 25, IsActive = true }
            };

            dbContext.TrainingType.AddRange(types);
            dbContext.SaveChanges();
            return types;
        }

        private static List<Plan> PopulatePlan(HealthWellbeingDbContext dbContext)
        {
            if (dbContext.Plan.Any()) return dbContext.Plan.ToList();

            var plans = new List<Plan>
            {
                new Plan { Name = "Quick Start 7-Day Trial", Description = "7 days access", Price = 9.99m, DurationDays = 7 },
                new Plan { Name = "Basic Wellness Plan", Description = "Access to gym + 1 class/week.", Price = 29.99m, DurationDays = 30 },
                new Plan { Name = "Advanced Fitness Plan", Description = "Unlimited gym + 3 classes/week.", Price = 59.99m, DurationDays = 30 },
                new Plan { Name = "Mind & Body Balance", Description = "Yoga and Pilates focus.", Price = 79.99m, DurationDays = 60 },
                new Plan { Name = "Ultimate Transformation", Description = "Personal coaching included.", Price = 99.99m, DurationDays = 90 },
                new Plan { Name = "Corporate Health Boost", Description = "Team-focused plan.", Price = 49.99m, DurationDays = 30 }
            };

            dbContext.Plan.AddRange(plans);
            dbContext.SaveChanges();
            return plans;
        }

        private static List<Trainer> PopulateTrainer(HealthWellbeingDbContext dbContext)
        {
            // Se já existir John Smith, devolve todos para não duplicar
            if (dbContext.Trainer.Any(t => t.Name == "John Smith"))
                return dbContext.Trainer.ToList();

            var trainers = new List<Trainer>
            {
                new Trainer { Name = "John Smith", Email = "john@gym.com", Phone = "555-111", Gender = "Male" },
                new Trainer { Name = "Emma Johnson", Email = "emma@gym.com", Phone = "555-222", Gender = "Female" },
                new Trainer { Name = "Carlos Mendes", Email = "carlos@gym.com", Phone = "555-333", Gender = "Male" },
                new Trainer { Name = "Maria Rodriguez", Email = "maria@gym.com", Phone = "555-555", Gender = "Female" },
                new Trainer { Name = "David Costa", Email = "david@gym.com", Phone = "555-666", Gender = "Male" },
                new Trainer { Name = "Sophie Lee", Email = "sophie@gym.com", Phone = "555-444", Gender = "Female" }
            };

            dbContext.Trainer.AddRange(trainers);
            dbContext.SaveChanges();
            return dbContext.Trainer.ToList();
        }

        private static List<Exercise> PopulateExercise(HealthWellbeingDbContext dbContext)
        {
            if (dbContext.Exercise.Any()) return dbContext.Exercise.ToList();

            var exercises = new List<Exercise>
            {
                new Exercise { Name = "Bench Press", MuscleGroup = "Chest", Equipment = "Barbell", Description = "Lie on back, press bar up." },
                new Exercise { Name = "Back Squat", MuscleGroup = "Legs", Equipment = "Barbell", Description = "Squat with bar on back." },
                new Exercise { Name = "Deadlift", MuscleGroup = "Back/Legs", Equipment = "Barbell", Description = "Lift bar from ground." },
                new Exercise { Name = "Shoulder Press", MuscleGroup = "Shoulders", Equipment = "Dumbbells", Description = "Press overhead." },
                new Exercise { Name = "Burpees", MuscleGroup = "Full Body", Equipment = "None", Description = "Jump and pushup." },
                new Exercise { Name = "Mountain Climbers", MuscleGroup = "Core", Equipment = "None", Description = "Run in plank position." },
                new Exercise { Name = "Downward Dog", MuscleGroup = "Full Body", Equipment = "Mat", Description = "Yoga stretch pose." },
                new Exercise { Name = "Cobra Pose", MuscleGroup = "Back", Equipment = "Mat", Description = "Back extension." },
                new Exercise { Name = "Plank", MuscleGroup = "Core", Equipment = "None", Description = "Hold position." }
            };

            dbContext.Exercise.AddRange(exercises);
            dbContext.SaveChanges();
            return exercises;
        }

        private static List<Training> PopulateTraining(HealthWellbeingDbContext dbContext, List<Trainer> trainers, List<TrainingType> types)
        {
            if (dbContext.Training.Any()) return dbContext.Training.ToList();

            var trainings = new List<Training>();

            // Helpers seguros para IDs (usa o primeiro se não encontrar por nome)
            int GetTrainerId(string namePart) => trainers.FirstOrDefault(t => t.Name.Contains(namePart))?.TrainerId ?? trainers.First().TrainerId;
            int GetTypeId(string namePart) => types.FirstOrDefault(t => t.Name.Contains(namePart))?.TrainingTypeId ?? types.First().TrainingTypeId;

            trainings.Add(new Training { Name = "Morning Yoga", Description = "Start right", DayOfWeek = WeekDay.Monday, StartTime = new TimeSpan(9, 0, 0), Duration = 60, TrainerId = GetTrainerId("Carlos"), TrainingTypeId = GetTypeId("Yoga") });
            trainings.Add(new Training { Name = "Intense Cardio", Description = "High intensity", DayOfWeek = WeekDay.Wednesday, StartTime = new TimeSpan(18, 0, 0), Duration = 45, TrainerId = GetTrainerId("John"), TrainingTypeId = GetTypeId("HIIT") });
            trainings.Add(new Training { Name = "Heavy Lifting", Description = "Strength", DayOfWeek = WeekDay.Friday, StartTime = new TimeSpan(17, 0, 0), Duration = 90, TrainerId = GetTrainerId("Emma"), TrainingTypeId = GetTypeId("Strength") });
            trainings.Add(new Training { Name = "Zumba Latin", Description = "Dance workout", DayOfWeek = WeekDay.Thursday, StartTime = new TimeSpan(18, 0, 0), Duration = 55, TrainerId = GetTrainerId("Maria"), TrainingTypeId = GetTypeId("Zumba") });
            trainings.Add(new Training { Name = "Pilates Core", Description = "Core strength", DayOfWeek = WeekDay.Tuesday, StartTime = new TimeSpan(17, 0, 0), Duration = 50, TrainerId = GetTrainerId("Sophie"), TrainingTypeId = GetTypeId("Pilates") });

            dbContext.Training.AddRange(trainings);
            dbContext.SaveChanges();
            return trainings;
        }

        private static void PopulateTrainingExercise(HealthWellbeingDbContext dbContext, List<Training> trainings, List<Exercise> exercises)
        {
            if (dbContext.TrainingExercise.Any()) return;

            var trainingExercises = new List<TrainingExercise>();

            // Configurar "Heavy Lifting"
            var heavyLifting = trainings.FirstOrDefault(t => t.Name == "Heavy Lifting");
            if (heavyLifting != null)
            {
                var bench = exercises.FirstOrDefault(e => e.Name == "Bench Press");
                var squat = exercises.FirstOrDefault(e => e.Name == "Back Squat");
                if (bench != null) trainingExercises.Add(new TrainingExercise { TrainingId = heavyLifting.TrainingId, ExerciseId = bench.ExerciseId, Sets = 4, Reps = 8, RestTime = "90s" });
                if (squat != null) trainingExercises.Add(new TrainingExercise { TrainingId = heavyLifting.TrainingId, ExerciseId = squat.ExerciseId, Sets = 5, Reps = 5, RestTime = "120s" });
            }

            // Configurar "Morning Yoga"
            var yoga = trainings.FirstOrDefault(t => t.Name == "Morning Yoga");
            if (yoga != null)
            {
                var dog = exercises.FirstOrDefault(e => e.Name == "Downward Dog");
                if (dog != null) trainingExercises.Add(new TrainingExercise { TrainingId = yoga.TrainingId, ExerciseId = dog.ExerciseId, Sets = 3, Reps = 1, RestTime = "30s" });
            }

            dbContext.TrainingExercise.AddRange(trainingExercises);
            dbContext.SaveChanges();
        }

        private static void PopulateTrainingPlan(HealthWellbeingDbContext dbContext, List<Plan> plans, List<Training> trainings)
        {
            if (dbContext.TrainingPlan.Any()) return;

            var trainingPlans = new List<TrainingPlan>();
            int GetPlanId(string namePart) => plans.FirstOrDefault(p => p.Name.Contains(namePart))?.PlanId ?? plans.First().PlanId;
            int GetTrainingId(string namePart) => trainings.FirstOrDefault(t => t.Name.Contains(namePart))?.TrainingId ?? trainings.First().TrainingId;

            // Associações de teste
            trainingPlans.Add(new TrainingPlan { PlanId = GetPlanId("Basic"), TrainingId = GetTrainingId("Morning Yoga"), DaysPerWeek = 1 });
            trainingPlans.Add(new TrainingPlan { PlanId = GetPlanId("Advanced"), TrainingId = GetTrainingId("Intense Cardio"), DaysPerWeek = 2 });
            trainingPlans.Add(new TrainingPlan { PlanId = GetPlanId("Advanced"), TrainingId = GetTrainingId("Heavy Lifting"), DaysPerWeek = 3 });

            dbContext.TrainingPlan.AddRange(trainingPlans);
            dbContext.SaveChanges();
        }

        private static void PopulateMemberPlan(HealthWellbeingDbContext dbContext, List<Member> members, List<Plan> plans)
        {
            if (dbContext.MemberPlan.Any()) return;
            if (!members.Any() || !plans.Any()) return;

            var memberPlans = new List<MemberPlan>();
            var random = new Random();

            for (int i = 0; i < members.Count; i++)
            {
                var member = members[i];
                var plan = plans[i % plans.Count];
                bool isActive = i % 2 == 0;

                memberPlans.Add(new MemberPlan
                {
                    MemberId = member.MemberId,
                    PlanId = plan.PlanId,
                    StartDate = DateTime.Now.AddDays(-random.Next(1, 30)),
                    EndDate = isActive ? DateTime.Now.AddDays(30) : DateTime.Now.AddDays(-10),
                    Status = isActive ? "Active" : "Expired"
                });
            }

            dbContext.MemberPlan.AddRange(memberPlans);
            dbContext.SaveChanges();
        }

        private static void PopulatePhysicalAssessment(HealthWellbeingDbContext dbContext, List<Member> members, List<Trainer> trainers)
        {
            if (dbContext.PhysicalAssessment.Any()) return;
            if (!members.Any() || !trainers.Any()) return;

            var assessments = new List<PhysicalAssessment>
            {
                new PhysicalAssessment
                {
                    AssessmentDate = DateTime.Now.AddMonths(-2),
                    Weight = 85.5m, Height = 1.80m, BodyFatPercentage = 22.5m, MuscleMass = 38.0m,
                    Notes = "Initial assessment.", MemberId = members[0].MemberId, TrainerId = trainers[0].TrainerId
                },
                new PhysicalAssessment
                {
                    AssessmentDate = DateTime.Now.AddDays(-15),
                    Weight = 83.2m, Height = 1.80m, BodyFatPercentage = 21.0m, MuscleMass = 38.5m,
                    Notes = "Progress check.", MemberId = members[0].MemberId, TrainerId = trainers[0].TrainerId
                }
            };

            dbContext.PhysicalAssessment.AddRange(assessments);
            dbContext.SaveChanges();
        }

        // Metodo Sessions
    }

    // Cole isto no fundo da classe, antes de fechar a chaveta final
    private static void PopulateSession(HealthWellbeingDbContext dbContext, List<Member> members, List<Training> trainings)
    {
        if (dbContext.Session.Any()) return;
        if (!members.Any() || !trainings.Any()) return;

        var alice = members.FirstOrDefault(m => m.Client != null && m.Client.Name == "Alice Wonderland");
        var bob = members.FirstOrDefault(m => m.Client != null && m.Client.Name == "Bob The Builder");

        var yoga = trainings.FirstOrDefault(t => t.Name == "Morning Yoga");
        var hiit = trainings.FirstOrDefault(t => t.Name == "HIIT Blast");

        var sessions = new List<Session>();

        // Helper local para calcular datas (pode copiar para aqui também)
        DateTime GetNextWeekday(DateTime start, DayOfWeek day)
        {
            int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;
            if (daysToAdd == 0) daysToAdd = 7;
            return start.AddDays(daysToAdd);
        }

        // 1. Agendar Alice para Yoga na PRÓXIMA SEGUNDA
        if (alice != null && yoga != null)
        {
            DateTime nextMonday = GetNextWeekday(DateTime.Today, DayOfWeek.Monday);
            sessions.Add(new Session
            {
                MemberId = alice.MemberId,
                TrainingId = yoga.TrainingId,
                SessionDate = nextMonday.Date + yoga.StartTime,
                Rating = null
            });
        }

        // 2. Agendar Bob para HIIT na PRÓXIMA QUARTA
        if (bob != null && hiit != null)
        {
            DateTime nextWed = GetNextWeekday(DateTime.Today, DayOfWeek.Wednesday);
            sessions.Add(new Session
            {
                MemberId = bob.MemberId,
                TrainingId = hiit.TrainingId,
                SessionDate = nextWed.Date + hiit.StartTime,
                Rating = null
            });
        }

        dbContext.Session.AddRange(sessions);
        dbContext.SaveChanges();
    }
}