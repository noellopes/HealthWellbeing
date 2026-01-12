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

            // 3. AULAS
            List<Training> trainings = new List<Training>();
            if (trainers.Any())
            {
                trainings = PopulateTraining(dbContext, trainers, trainingTypes);
                PopulateTrainingPlan(dbContext, plans, trainings);
                PopulateTrainingExercise(dbContext, trainings, exercises);
            }

            // 4. ASSOCIAÇÕES
            PopulateMemberPlan(dbContext, members, plans);
            PopulatePhysicalAssessment(dbContext, members, trainers);

            // 5. SESSÕES
            PopulateSession(dbContext, trainings);
        }

        // ... MÉTODOS DE POPULAÇÃO PADRÃO ...
        private static List<Client> PopulateClients(HealthWellbeingDbContext db)
        {
            if (db.Client.Any()) return db.Client.ToList();
            var clients = new List<Client> {
                new Client { Name="Alice Wonderland", Email="alice.w@example.com", Phone="912345678", Address="London", BirthDate=new DateTime(1990,5,15), Gender="Female", RegistrationDate=DateTime.Now.AddDays(-60) },
                new Client { Name="Bob The Builder", Email="bob.builder@work.net", Phone="919876543", Address="Site 5A", BirthDate=new DateTime(1985,10,20), Gender="Male", RegistrationDate=DateTime.Now.AddDays(-60) }
            };
            db.Client.AddRange(clients); db.SaveChanges(); return clients;
        }

        private static List<Member> PopulateMember(HealthWellbeingDbContext db, List<Client> clients)
        {
            if (db.Member.Any()) return db.Member.ToList();
            var members = new List<Member>();
            foreach (var c in clients.Take(2)) members.Add(new Member { ClientId = c.ClientId });
            db.Member.AddRange(members); db.SaveChanges(); return members;
        }

        // ... (Copie os métodos PopulatePlan, PopulateTrainer, PopulateTrainingType, PopulateExercise, PopulateTraining, PopulateTrainingExercise, PopulateTrainingPlan, PopulateMemberPlan, PopulatePhysicalAssessment dos exemplos anteriores, eles estavam corretos) ...

        private static List<Plan> PopulatePlan(HealthWellbeingDbContext db)
        {
            if (db.Plan.Any()) return db.Plan.ToList();
            var plans = new List<Plan> { new Plan { Name = "Standard", Price = 29.99m, Description = "Basic", DurationDays = 30 } };
            db.Plan.AddRange(plans); db.SaveChanges(); return plans;
        }

        private static List<Trainer> PopulateTrainer(HealthWellbeingDbContext db)
        {
            if (db.Trainer.Any()) return db.Trainer.ToList();
            var trainers = new List<Trainer> { new Trainer { Name = "John Smith", Email = "john@gym.com", Phone = "123", Gender = "Male" } };
            db.Trainer.AddRange(trainers); db.SaveChanges(); return trainers;
        }

        private static List<TrainingType> PopulateTrainingType(HealthWellbeingDbContext db)
        {
            if (db.TrainingType.Any()) return db.TrainingType.ToList();
            var types = new List<TrainingType> { new TrainingType { Name = "Yoga", Description = "Flex", MaxParticipants = 15 }, new TrainingType { Name = "HIIT", Description = "High", MaxParticipants = 20 } };
            db.TrainingType.AddRange(types); db.SaveChanges(); return types;
        }

        private static List<Exercise> PopulateExercise(HealthWellbeingDbContext db)
        {
            if (db.Exercise.Any()) return db.Exercise.ToList();
            var exs = new List<Exercise> { new Exercise { Name = "Squat", MuscleGroup = "Legs", Equipment = "None" } };
            db.Exercise.AddRange(exs); db.SaveChanges(); return exs;
        }

        private static List<Training> PopulateTraining(HealthWellbeingDbContext db, List<Trainer> trainers, List<TrainingType> types)
        {
            if (db.Training.Any()) return db.Training.ToList();
            var trs = new List<Training> { new Training { Name = "Morning Yoga", DayOfWeek = WeekDay.Monday, StartTime = new TimeSpan(9, 0, 0), Duration = 60, TrainerId = trainers.First().TrainerId, TrainingTypeId = types.First().TrainingTypeId } };
            db.Training.AddRange(trs); db.SaveChanges(); return trs;
        }

        private static void PopulateTrainingPlan(HealthWellbeingDbContext db, List<Plan> ps, List<Training> ts)
        {
            if (!db.TrainingPlan.Any() && ps.Any() && ts.Any()) { db.TrainingPlan.Add(new TrainingPlan { PlanId = ps.First().PlanId, TrainingId = ts.First().TrainingId }); db.SaveChanges(); }
        }

        private static void PopulateTrainingExercise(HealthWellbeingDbContext db, List<Training> ts, List<Exercise> es)
        {
            if (!db.TrainingExercise.Any() && ts.Any() && es.Any()) { db.TrainingExercise.Add(new TrainingExercise { TrainingId = ts.First().TrainingId, ExerciseId = es.First().ExerciseId, Sets = 3, Reps = 10 }); db.SaveChanges(); }
        }

        private static void PopulateMemberPlan(HealthWellbeingDbContext db, List<Member> ms, List<Plan> ps)
        {
            if (!db.MemberPlan.Any() && ms.Any() && ps.Any()) { db.MemberPlan.Add(new MemberPlan { MemberId = ms.First().MemberId, PlanId = ps.First().PlanId, StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(30), Status = "Active" }); db.SaveChanges(); }
        }

        private static void PopulatePhysicalAssessment(HealthWellbeingDbContext db, List<Member> ms, List<Trainer> ts)
        {
            if (!db.PhysicalAssessment.Any() && ms.Any() && ts.Any()) { db.PhysicalAssessment.Add(new PhysicalAssessment { MemberId = ms.First().MemberId, TrainerId = ts.First().TrainerId, AssessmentDate = DateTime.Now, Weight = 80, Height = 1.80m, BodyFatPercentage = 15, MuscleMass = 40 }); db.SaveChanges(); }
        }

        // === MÉTODO DE SESSÕES CORRIGIDO ===
        private static void PopulateSession(HealthWellbeingDbContext dbContext, List<Training> trainings)
        {
            if (dbContext.Session.Any()) return;

            // Vai buscar à BD para garantir que trazemos o Cliente incluído
            var alice = dbContext.Member.Include(m => m.Client).FirstOrDefault(m => m.Client.Name == "Alice Wonderland");
            var yoga = trainings.FirstOrDefault(t => t.Name.Contains("Yoga"));

            if (alice != null && yoga != null)
            {
                int daysUntil = ((int)DayOfWeek.Monday - (int)DateTime.Today.DayOfWeek + 7) % 7;
                DateTime sessionDate = DateTime.Today.AddDays(daysUntil == 0 ? 7 : daysUntil).Date + yoga.StartTime;

                dbContext.Session.Add(new Session
                {
                    MemberId = alice.MemberId,
                    TrainingId = yoga.TrainingId,
                    SessionDate = sessionDate,
                    MemberFeedback = "Auto-generated booking",
                    Rating = null
                });
                dbContext.SaveChanges();
            }
        }
    }
}