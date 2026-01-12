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
                PopulateTrainingPlan(dbContext, plans, trainings);
                PopulateTrainingExercise(dbContext, trainings, exercises);
            }

            // 4. ASSOCIAÇÕES FINAIS
            PopulateMemberPlan(dbContext, members, plans);
            PopulatePhysicalAssessment(dbContext, members, trainers);

            // 5. SESSÕES (AGENDAMENTOS)
            PopulateSession(dbContext, members, trainings);
        }

        private static List<Client> PopulateClients(HealthWellbeingDbContext dbContext)
        {
            if (dbContext.Client.Any()) return dbContext.Client.ToList();
            var clients = new List<Client>() {
                new Client { Name = "Alice Wonderland", Email = "alice.w@example.com", Phone = "912345678", Address = "London", BirthDate = new DateTime(1990, 5, 15), Gender = "Female", RegistrationDate = DateTime.Now.AddDays(-60) },
                new Client { Name = "Bob The Builder", Email = "bob.builder@work.net", Phone = "919876543", Address = "Site 5A", BirthDate = new DateTime(1985, 10, 20), Gender = "Male", RegistrationDate = DateTime.Now.AddDays(-60) }
            };
            dbContext.Client.AddRange(clients); dbContext.SaveChanges(); return clients;
        }

        private static List<Member> PopulateMember(HealthWellbeingDbContext dbContext, List<Client> clients)
        {
            if (dbContext.Member.Any()) return dbContext.Member.ToList();
            var members = new List<Member>();
            foreach (var client in clients.Take(2)) members.Add(new Member { ClientId = client.ClientId });
            dbContext.Member.AddRange(members); dbContext.SaveChanges(); return members;
        }

        private static List<Plan> PopulatePlan(HealthWellbeingDbContext dbContext)
        {
            if (dbContext.Plan.Any()) return dbContext.Plan.ToList();
            var plans = new List<Plan> {
                new Plan { Name = "Standard", Description = "Access", Price = 29.99m, DurationDays = 30 },
                new Plan { Name = "Premium", Description = "All classes", Price = 49.99m, DurationDays = 30 }
            };
            dbContext.Plan.AddRange(plans); dbContext.SaveChanges(); return plans;
        }

        private static List<Trainer> PopulateTrainer(HealthWellbeingDbContext dbContext)
        {
            if (dbContext.Trainer.Any()) return dbContext.Trainer.ToList();
            var trainers = new List<Trainer> {
                new Trainer { Name = "John Smith", Email = "john@gym.com", Phone = "555-111", Gender = "Male" },
                new Trainer { Name = "Emma Johnson", Email = "emma@gym.com", Phone = "555-222", Gender = "Female" }
            };
            dbContext.Trainer.AddRange(trainers); dbContext.SaveChanges(); return trainers;
        }

        private static List<TrainingType> PopulateTrainingType(HealthWellbeingDbContext dbContext)
        {
            if (dbContext.TrainingType.Any()) return dbContext.TrainingType.ToList();
            var types = new List<TrainingType> {
                new TrainingType { Name = "Yoga Basics", Description = "Flexibility", MaxParticipants = 15 },
                new TrainingType { Name = "HIIT", Description = "High Intensity", MaxParticipants = 20 },
                new TrainingType { Name = "Strength", Description = "Weights", MaxParticipants = 10 }
            };
            dbContext.TrainingType.AddRange(types); dbContext.SaveChanges(); return types;
        }

        private static List<Exercise> PopulateExercise(HealthWellbeingDbContext dbContext)
        {
            if (dbContext.Exercise.Any()) return dbContext.Exercise.ToList();
            var exercises = new List<Exercise> {
                new Exercise { Name = "Bench Press", MuscleGroup = "Chest", Equipment = "Barbell" },
                new Exercise { Name = "Squat", MuscleGroup = "Legs", Equipment = "Barbell" },
                new Exercise { Name = "Downward Dog", MuscleGroup = "Full Body", Equipment = "Mat" }
            };
            dbContext.Exercise.AddRange(exercises); dbContext.SaveChanges(); return exercises;
        }

        private static List<Training> PopulateTraining(HealthWellbeingDbContext dbContext, List<Trainer> trainers, List<TrainingType> types)
        {
            if (dbContext.Training.Any()) return dbContext.Training.ToList();
            var trainings = new List<Training>();
            int tr1 = trainers.First().TrainerId;
            int typeYoga = types.First(t => t.Name.Contains("Yoga")).TrainingTypeId;
            int typeHiit = types.First(t => t.Name.Contains("HIIT")).TrainingTypeId;

            trainings.Add(new Training { Name = "Morning Yoga", DayOfWeek = WeekDay.Monday, StartTime = new TimeSpan(9, 0, 0), Duration = 60, TrainerId = tr1, TrainingTypeId = typeYoga });
            trainings.Add(new Training { Name = "HIIT Blast", DayOfWeek = WeekDay.Wednesday, StartTime = new TimeSpan(18, 0, 0), Duration = 45, TrainerId = tr1, TrainingTypeId = typeHiit });

            dbContext.Training.AddRange(trainings); dbContext.SaveChanges(); return trainings;
        }

        private static void PopulateTrainingPlan(HealthWellbeingDbContext dbContext, List<Plan> plans, List<Training> trainings)
        {
            if (!dbContext.TrainingPlan.Any() && plans.Any() && trainings.Any())
            {
                dbContext.TrainingPlan.Add(new TrainingPlan { PlanId = plans.First().PlanId, TrainingId = trainings.First().TrainingId, DaysPerWeek = 2 });
                dbContext.SaveChanges();
            }
        }

        private static void PopulateTrainingExercise(HealthWellbeingDbContext dbContext, List<Training> trainings, List<Exercise> exercises)
        {
            if (!dbContext.TrainingExercise.Any() && trainings.Any() && exercises.Any())
            {
                dbContext.TrainingExercise.Add(new TrainingExercise { TrainingId = trainings.First().TrainingId, ExerciseId = exercises.First().ExerciseId, Sets = 3, Reps = 10, RestTime = "60s" });
                dbContext.SaveChanges();
            }
        }

        private static void PopulateMemberPlan(HealthWellbeingDbContext dbContext, List<Member> members, List<Plan> plans)
        {
            if (!dbContext.MemberPlan.Any() && members.Any() && plans.Any())
            {
                dbContext.MemberPlan.Add(new MemberPlan { MemberId = members.First().MemberId, PlanId = plans.First().PlanId, StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(30), Status = "Active" });
                dbContext.SaveChanges();
            }
        }

        private static void PopulatePhysicalAssessment(HealthWellbeingDbContext dbContext, List<Member> members, List<Trainer> trainers)
        {
            if (!dbContext.PhysicalAssessment.Any() && members.Any() && trainers.Any())
            {
                dbContext.PhysicalAssessment.Add(new PhysicalAssessment { MemberId = members.First().MemberId, TrainerId = trainers.First().TrainerId, AssessmentDate = DateTime.Now.AddDays(-10), Weight = 70m, Height = 1.75m, BodyFatPercentage = 15m, MuscleMass = 40m });
                dbContext.SaveChanges();
            }
        }

        // ==============================================================
        //  POPULAR SESSÕES
        // ==============================================================
        private static void PopulateSession(HealthWellbeingDbContext dbContext, List<Member> members, List<Training> trainings)
        {
            if (dbContext.Session.Any()) return;
            if (!members.Any() || !trainings.Any()) return;

            var alice = members.FirstOrDefault(m => m.Client != null && m.Client.Name == "Alice Wonderland");
            var yoga = trainings.FirstOrDefault(t => t.Name == "Morning Yoga");

            if (alice != null && yoga != null)
            {
                // Calcula a data da próxima Segunda-Feira
                int daysUntil = ((int)DayOfWeek.Monday - (int)DateTime.Today.DayOfWeek + 7) % 7;
                if (daysUntil == 0) daysUntil = 7;
                DateTime sessionDate = DateTime.Today.AddDays(daysUntil).Date + yoga.StartTime;

                dbContext.Session.Add(new Session
                {
                    MemberId = alice.MemberId,
                    TrainingId = yoga.TrainingId,
                    SessionDate = sessionDate,
                    MemberFeedback = "First session booked via Seed!",
                    Rating = null
                });
                dbContext.SaveChanges();
            }
        }
    }
}