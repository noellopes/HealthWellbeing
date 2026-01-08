using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

internal class SeedDataGinasio
{
    internal static void Populate(HealthWellbeingDbContext? dbContext)
    {
        if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));

        dbContext.Database.EnsureCreated();

        var clients = PopulateClients(dbContext);
        var members = PopulateMember(dbContext, clients);
        var trainingTypes = PopulateTrainingType(dbContext);
        var plans = PopulatePlan(dbContext);
        var trainers = PopulateTrainer(dbContext);
        var trainings = PopulateTraining(dbContext, trainers);
        PopulateMemberPlan(dbContext, members, plans);
        PopulateTrainingPlan(dbContext, plans, trainings);
    }

    private static List<Client> PopulateClients(HealthWellbeingDbContext dbContext)
    {
        if (dbContext.Client.Any()) return dbContext.Client.ToList();

        var clients = new List<Client>()
        {
            new Client { Name = "Alice Wonderland", Email = "alice.w@example.com", Phone = "912345678", Address = "10 Downing St, London", BirthDate = new DateTime(1990, 5, 15), Gender = "Female", RegistrationDate = DateTime.Now.AddDays(-300) },
            new Client { Name = "Bob The Builder", Email = "bob.builder@work.net", Phone = "919876543", Address = "Construction Site 5A", BirthDate = new DateTime(1985, 10, 20), Gender = "Male", RegistrationDate = DateTime.Now.AddDays(-150) },
            new Client { Name = "Charlie Brown", Email = "charlie.b@peanuts.com", Phone = "914567890", Address = "123 Comic Strip Ave", BirthDate = new DateTime(2000, 1, 1), Gender = "Male", RegistrationDate = DateTime.Now.AddDays(-50) },
            new Client { Name = "David Copperfield", Email = "david.c@magic.com", Phone = "910001002", Address = "Las Vegas Strip", BirthDate = new DateTime(1960, 9, 16), Gender = "Male", RegistrationDate = DateTime.Now.AddDays(-25) },
            new Client { Name = "Eve Harrington", Email = "eve.h@stage.net", Phone = "913330009", Address = "Broadway St", BirthDate = new DateTime(1995, 2, 28), Gender = "Female", RegistrationDate = DateTime.Now.AddDays(-10) },
            new Client { Name = "Frank Castle", Email = "frank.c@punisher.com", Phone = "911110001", Address = "Hells Kitchen, NY", BirthDate = new DateTime(1978, 3, 16), Gender = "Male", RegistrationDate = DateTime.Now.AddDays(-40) },
            new Client { Name = "Grace Hopper", Email = "grace.h@navy.mil", Phone = "912220002", Address = "Arlington, VA", BirthDate = new DateTime(1906, 12, 9), Gender = "Female", RegistrationDate = DateTime.Now.AddDays(-100) },
            new Client { Name = "Harry Potter", Email = "harry.p@hogwarts.wiz", Phone = "913330003", Address = "4 Privet Drive, Surrey", BirthDate = new DateTime(1980, 7, 31), Gender = "Male", RegistrationDate = DateTime.Now.AddDays(-12) },
            new Client { Name = "Jack Sparrow", Email = "jack.s@pirate.sea", Phone = "915550005", Address = "Tortuga", BirthDate = new DateTime(1980, 4, 1), Gender = "Male", RegistrationDate = DateTime.Now.AddDays(-8) },
            new Client { Name = "Mona Lisa", Email = "mona.l@art.com", Phone = "918880008", Address = "The Louvre, Paris", BirthDate = new DateTime(1993, 6, 15), Gender = "Female", RegistrationDate = DateTime.Now.AddDays(-50) }
        };

        dbContext.Client.AddRange(clients);
        dbContext.SaveChanges(); // Aqui a BD gera os IDs (1, 2, 3...) e preenche a lista 'clients'
        return clients;
    }

    private static List<Member> PopulateMember(HealthWellbeingDbContext dbContext, List<Client> clients)
    {
        if (dbContext.Member.Any()) return dbContext.Member.ToList();

        var members = new List<Member>();

        // Associamos os clientes aos membros usando o novo ID inteiro gerado
        foreach (var client in clients.Take(15)) // Convertemos os primeiros 15 em membros
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
            new TrainingType { Name = "Yoga Basics", Description = "Focus on flexibility and balance.", DurationMinutes = 60 },
            new TrainingType { Name = "HIIT", Description = "High Intensity Interval Training.", DurationMinutes = 45 },
            new TrainingType { Name = "Pilates Core Strength", Description = "Core strength and posture.", DurationMinutes = 50 },
            new TrainingType { Name = "Zumba Dance", Description = "Dance workout.", DurationMinutes = 55 },
            new TrainingType { Name = "Strength Training", Description = "Weight lifting.", DurationMinutes = 90 },
            new TrainingType { Name = "Indoor Cycling", Description = "Stationary bike workout.", DurationMinutes = 45 },
            new TrainingType { Name = "Cardio Kickboxing", Description = "High-energy martial arts.", DurationMinutes = 60 },
            new TrainingType { Name = "Power Yoga", Description = "Vigorous fitness-based yoga.", DurationMinutes = 75 },
            new TrainingType { Name = "Aqua Aerobics", Description = "Low-impact pool training.", DurationMinutes = 45 },
            new TrainingType { Name = "TRX Suspension", Description = "Bodyweight exercises.", DurationMinutes = 50 },
            new TrainingType { Name = "CrossFit WOD", Description = "Functional movements.", DurationMinutes = 60 },
            new TrainingType { Name = "Barre Fitness", Description = "Ballet-inspired workout.", DurationMinutes = 55 },
            new TrainingType { Name = "Boxing Technique", Description = "Boxing stance and footwork.", DurationMinutes = 60 },
            new TrainingType { Name = "Meditation & Breathwork", Description = "Reduce stress.", DurationMinutes = 30 },
            new TrainingType { Name = "Kettlebell Flow", Description = "Full-body workout.", DurationMinutes = 45 },
            new TrainingType { Name = "Calisthenics", Description = "Street workout style.", DurationMinutes = 60 },
            new TrainingType { Name = "Senior Mobility", Description = "Gentle exercises.", DurationMinutes = 40 },
            new TrainingType { Name = "Tai Chi", Description = "Internal Chinese martial art.", DurationMinutes = 60 },
            new TrainingType { Name = "Boot Camp", Description = "Military-style circuit.", DurationMinutes = 60 },
            new TrainingType { Name = "Step Aerobics", Description = "Choreographed cardio.", DurationMinutes = 50 }
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
        // Existing 5 plans
        new Plan { Name = "Basic Wellness Plan", Description = "Access to gym + 1 class/week.", Price = 29.99m, DurationDays = 30 },
        new Plan { Name = "Advanced Fitness Plan", Description = "Unlimited gym + 3 classes/week.", Price = 59.99m, DurationDays = 30 },
        new Plan { Name = "Mind & Body Balance", Description = "Yoga and Pilates focus.", Price = 79.99m, DurationDays = 60 },
        new Plan { Name = "Ultimate Transformation Plan", Description = "Personal coaching included.", Price = 99.99m, DurationDays = 90 },
        new Plan { Name = "Corporate Health Boost", Description = "Team-focused plan.", Price = 49.99m, DurationDays = 30 },

        // 17 New plans
        new Plan { Name = "Quick Start 7-Day Trial", Description = "Full gym access for 7 days. One-time fee.", Price = 9.99m, DurationDays = 7 },
        new Plan { Name = "Senior Active Life", Description = "Classes tailored for seniors (low impact).", Price = 35.00m, DurationDays = 30 },
        new Plan { Name = "Student Fit Pass", Description = "Discounted unlimited monthly access for students.", Price = 45.00m, DurationDays = 30 },
        new Plan { Name = "Cardio Focus Pack", Description = "Unlimited spin and aerobic classes.", Price = 65.50m, DurationDays = 60 },
        new Plan { Name = "Strength & Bulk 3 Months", Description = "Access to heavy lifting zone + nutrition guide.", Price = 180.00m, DurationDays = 90 },
        new Plan { Name = "Family Flex Plan (Per Quarter)", Description = "Discounted access for 3 or more family members.", Price = 199.90m, DurationDays = 90 },
        new Plan { Name = "Weekend Warrior", Description = "Access only on Saturdays and Sundays.", Price = 19.99m, DurationDays = 30 },
        new Plan { Name = "Personal Trainer (10 Sessions)", Description = "Bundle of 10 one-on-one training sessions.", Price = 350.00m, DurationDays = 0 }, // Duração 0 para planos baseados em sessões
        new Plan { Name = "Hydro-Therapy Pass", Description = "Access to pool and aquafitness classes only.", Price = 40.00m, DurationDays = 30 },
        new Plan { Name = "Virtual Home Training", Description = "Access to all online live and recorded classes.", Price = 25.99m, DurationDays = 30 },
        new Plan { Name = "Weight Loss Journey (6 months)", Description = "Personal coaching, diet plan, and bi-weekly checkups.", Price = 450.00m, DurationDays = 180 },
        new Plan { Name = "Pilates Only Annual", Description = "Unlimited access to all Pilates classes, paid yearly.", Price = 799.00m, DurationDays = 365 },
        new Plan { Name = "Powerlifting Specialist", Description = "Exclusive access to advanced power racks and certified coaches.", Price = 85.00m, DurationDays = 30 },
        new Plan { Name = "Physical Rehab Support", Description = "Custom low-impact exercises with specialized equipment.", Price = 120.00m, DurationDays = 60 },
        new Plan { Name = "Kids Sports Camp", Description = "Summer program focused on movement and coordination for children.", Price = 150.00m, DurationDays = 14 },
        new Plan { Name = "Monthly Gym Only", Description = "Standard access to all gym equipment, no classes included.", Price = 39.99m, DurationDays = 30 },
        new Plan { Name = "Executive Lunch Break", Description = "Access restricted to 11h00 to 14h00 on weekdays.", Price = 30.00m, DurationDays = 30 }
    };

        dbContext.Plan.AddRange(plans);
        dbContext.SaveChanges();
        return plans;
    }

    private static List<Trainer> PopulateTrainer(HealthWellbeingDbContext dbContext)
    {
        if (dbContext.Trainer.Any()) return dbContext.Trainer.ToList();

        var trainers = new List<Trainer>
        {
            new Trainer { Name = "John Smith", Speciality = "HIIT & Cardio", Email = "john@gym.com", Phone = "555-111", BirthDate = new DateTime(1988, 7, 10), Gender = "Male" },
            new Trainer { Name = "Emma Johnson", Speciality = "Strength", Email = "emma@gym.com", Phone = "555-222", BirthDate = new DateTime(1992, 11, 25), Gender = "Female" },
            new Trainer { Name = "Carlos Mendes", Speciality = "Yoga", Email = "carlos@gym.com", Phone = "555-333", BirthDate = new DateTime(1975, 4, 1), Gender = "Male" },
            new Trainer { Name = "Sophie Lee", Speciality = "Pilates", Email = "sophie@gym.com", Phone = "555-444", BirthDate = new DateTime(1996, 2, 14), Gender = "Female" },
            new Trainer { Name = "Maria Rodriguez", Speciality = "Zumba", Email = "maria@gym.com", Phone = "555-555", BirthDate = new DateTime(1985, 9, 30), Gender = "Female" },
            new Trainer { Name = "David Costa", Speciality = "Cycling", Email = "david@gym.com", Phone = "555-666", BirthDate = new DateTime(1990, 6, 5), Gender = "Male" },
            new Trainer { Name = "Laura Silva", Speciality = "Barre", Email = "laura@gym.com", Phone = "555-777", BirthDate = new DateTime(1994, 3, 18), Gender = "Female" },
            new Trainer { Name = "André Santos", Speciality = "Functional", Email = "andre@gym.com", Phone = "555-888", BirthDate = new DateTime(1983, 1, 22), Gender = "Male" }
        };

        dbContext.Trainer.AddRange(trainers);
        dbContext.SaveChanges();
        return trainers;
    }

    private static List<Training> PopulateTraining(HealthWellbeingDbContext dbContext, List<Trainer> trainers)
    {
        if (dbContext.Training.Any()) return dbContext.Training.ToList();

        var trainings = new List<Training>();

        // Precisamos dos tipos de treino para criar os treinos. Vamos buscá-los à BD.
        var trainingTypes = dbContext.TrainingType.ToList();

        // Helper local para encontrar IDs
        int GetTrainerId(string namePart) => trainers.First(t => t.Name.Contains(namePart)).TrainerId;
        int GetTypeId(string namePart) => trainingTypes.First(t => t.Name.Contains(namePart)).TrainingTypeId;

        // --- CRIAÇÃO DE TREINOS (Mínimo 20 para TrainingPlan usar) ---

        trainings.Add(new Training { Name = "Morning Yoga Flow", Description = "Start the day right", DayOfWeek = "Monday", Duration = 60, MaxParticipants = 15, TrainerId = GetTrainerId("Carlos"), TrainingTypeId = GetTypeId("Yoga") });
        trainings.Add(new Training { Name = "Intense Cardio HIT", Description = "Quick burn", DayOfWeek = "Wednesday", Duration = 45, MaxParticipants = 20, TrainerId = GetTrainerId("John"), TrainingTypeId = GetTypeId("HIIT") });
        trainings.Add(new Training { Name = "Heavy Lifting Focus", Description = "Build muscle", DayOfWeek = "Friday", Duration = 90, MaxParticipants = 10, TrainerId = GetTrainerId("Emma"), TrainingTypeId = GetTypeId("Strength") });
        trainings.Add(new Training { Name = "Pilates Core Stability", Description = "Abs focus", DayOfWeek = "Tuesday", Duration = 50, MaxParticipants = 12, TrainerId = GetTrainerId("Sophie"), TrainingTypeId = GetTypeId("Pilates") });
        trainings.Add(new Training { Name = "Zumba Latin Fiesta", Description = "Dance workout", DayOfWeek = "Thursday", Duration = 55, MaxParticipants = 30, TrainerId = GetTrainerId("Maria"), TrainingTypeId = GetTypeId("Zumba") });
        trainings.Add(new Training { Name = "Power Yoga: Midday", Description = "Vigorous flow", DayOfWeek = "Monday", Duration = 75, MaxParticipants = 12, TrainerId = GetTrainerId("Laura"), TrainingTypeId = GetTypeId("Power Yoga") });
        trainings.Add(new Training { Name = "Morning Endurance Spin", Description = "Cycle hard", DayOfWeek = "Wednesday", Duration = 45, MaxParticipants = 18, TrainerId = GetTrainerId("David"), TrainingTypeId = GetTypeId("Cycling") });
        trainings.Add(new Training { Name = "Deep Water Toning", Description = "Pool workout", DayOfWeek = "Saturday", Duration = 45, MaxParticipants = 25, TrainerId = GetTrainerId("Maria"), TrainingTypeId = GetTypeId("Aqua") });
        trainings.Add(new Training { Name = "TRX Suspension Full Body", Description = "Bodyweight", DayOfWeek = "Tuesday", Duration = 50, MaxParticipants = 10, TrainerId = GetTrainerId("André"), TrainingTypeId = GetTypeId("TRX") });
        trainings.Add(new Training { Name = "Express Lunch HIIT", Description = "Fast workout", DayOfWeek = "Friday", Duration = 30, MaxParticipants = 20, TrainerId = GetTrainerId("John"), TrainingTypeId = GetTypeId("HIIT") });
        trainings.Add(new Training { Name = "Barre Sculpt and Tone", Description = "Ballet style", DayOfWeek = "Monday", Duration = 55, MaxParticipants = 15, TrainerId = GetTrainerId("Laura"), TrainingTypeId = GetTypeId("Barre") });
        trainings.Add(new Training { Name = "Boxing Technique Workshop", Description = "Punching basics", DayOfWeek = "Thursday", Duration = 60, MaxParticipants = 16, TrainerId = GetTrainerId("David"), TrainingTypeId = GetTypeId("Boxing") });
        trainings.Add(new Training { Name = "Sunday Morning Calm", Description = "Meditation", DayOfWeek = "Sunday", Duration = 30, MaxParticipants = 40, TrainerId = GetTrainerId("Carlos"), TrainingTypeId = GetTypeId("Meditation") });
        trainings.Add(new Training { Name = "Kettlebell Power Flow", Description = "Dynamic strength", DayOfWeek = "Wednesday", Duration = 45, MaxParticipants = 14, TrainerId = GetTrainerId("André"), TrainingTypeId = GetTypeId("Kettlebell") });
        trainings.Add(new Training { Name = "Full Body Circuit", Description = "Strength circuit", DayOfWeek = "Tuesday", Duration = 60, MaxParticipants = 12, TrainerId = GetTrainerId("Emma"), TrainingTypeId = GetTypeId("Strength") });
        trainings.Add(new Training { Name = "Lower Body Endurance", Description = "Leg day", DayOfWeek = "Thursday", Duration = 60, MaxParticipants = 12, TrainerId = GetTrainerId("John"), TrainingTypeId = GetTypeId("Strength") });
        trainings.Add(new Training { Name = "Gentle Hatha Evening", Description = "Relaxing yoga", DayOfWeek = "Wednesday", Duration = 60, MaxParticipants = 15, TrainerId = GetTrainerId("Carlos"), TrainingTypeId = GetTypeId("Yoga") });
        trainings.Add(new Training { Name = "Zumba Gold", Description = "Low impact", DayOfWeek = "Friday", Duration = 55, MaxParticipants = 25, TrainerId = GetTrainerId("Maria"), TrainingTypeId = GetTypeId("Zumba") });
        trainings.Add(new Training { Name = "Power Hour Max", Description = "Intense cycle", DayOfWeek = "Saturday", Duration = 60, MaxParticipants = 18, TrainerId = GetTrainerId("David"), TrainingTypeId = GetTypeId("Cycling") });
        trainings.Add(new Training { Name = "Reformer Intro", Description = "Machine pilates", DayOfWeek = "Thursday", Duration = 50, MaxParticipants = 8, TrainerId = GetTrainerId("Sophie"), TrainingTypeId = GetTypeId("Pilates") });
        trainings.Add(new Training { Name = "Weekend Metabolic", Description = "Burn calories", DayOfWeek = "Saturday", Duration = 45, MaxParticipants = 20, TrainerId = GetTrainerId("André"), TrainingTypeId = GetTypeId("HIIT") });
        trainings.Add(new Training { Name = "Water Circuit", Description = "Pool circuit", DayOfWeek = "Monday", Duration = 45, MaxParticipants = 25, TrainerId = GetTrainerId("Maria"), TrainingTypeId = GetTypeId("Aqua") });
        trainings.Add(new Training { Name = "Deep Stretch", Description = "Recovery", DayOfWeek = "Sunday", Duration = 55, MaxParticipants = 15, TrainerId = GetTrainerId("Laura"), TrainingTypeId = GetTypeId("Barre") });

        dbContext.Training.AddRange(trainings);
        dbContext.SaveChanges();
        return trainings;
    }

    private static void PopulateMemberPlan(HealthWellbeingDbContext dbContext, List<Member> members, List<Plan> plans)
    {
        if (dbContext.MemberPlan.Any()) return;

        var memberPlans = new List<MemberPlan>();

        // Helper
        int GetPlanId(string namePart) => plans.First(p => p.Name.Contains(namePart)).PlanId;

        if (members.Count == 0 || plans.Count == 0) return;

        var random = new Random();
        int planIndex = 0;

        // Vamos garantir 20 inscrições
        for (int i = 0; i < 22; i++)
        {
            // Usar membros em ciclo se tivermos menos de 20
            var member = members[i % members.Count];
            var plan = plans[planIndex % plans.Count];

            bool isActive = i < 15; // Primeiros 15 ativos

            memberPlans.Add(new MemberPlan
            {
                MemberId = member.MemberId,
                PlanId = plan.PlanId,
                StartDate = isActive ? DateTime.Now.AddDays(-random.Next(1, 30)) : DateTime.Now.AddDays(-random.Next(100, 200)),
                EndDate = isActive ? DateTime.Now.AddDays(30) : DateTime.Now.AddDays(-10),
                Status = isActive ? "Active" : "Expired"
            });

            planIndex++;
        }

        dbContext.MemberPlan.AddRange(memberPlans);
        dbContext.SaveChanges();
    }

    private static void PopulateTrainingPlan(HealthWellbeingDbContext dbContext, List<Plan> plans, List<Training> trainings)
    {
        if (dbContext.TrainingPlan.Any()) return;

        var trainingPlans = new List<TrainingPlan>();

        // Helper para encontrar IDs
        int GetPlanId(string namePart) => plans.First(p => p.Name.Contains(namePart)).PlanId;
        int GetTrainingId(string namePart) => trainings.First(t => t.Name.Contains(namePart)).TrainingId;

        // --- CRIAÇÃO DE TRAINING PLANS (Associar Planos a Treinos - Mínimo 20) ---

        // Basic Wellness (4 treinos)
        trainingPlans.Add(new TrainingPlan { PlanId = GetPlanId("Basic"), TrainingId = GetTrainingId("Morning Yoga"), DaysPerWeek = 1 });
        trainingPlans.Add(new TrainingPlan { PlanId = GetPlanId("Basic"), TrainingId = GetTrainingId("Zumba Latin"), DaysPerWeek = 1 });
        trainingPlans.Add(new TrainingPlan { PlanId = GetPlanId("Basic"), TrainingId = GetTrainingId("Gentle Hatha"), DaysPerWeek = 1 });
        trainingPlans.Add(new TrainingPlan { PlanId = GetPlanId("Basic"), TrainingId = GetTrainingId("Water Circuit"), DaysPerWeek = 1 });

        // Advanced Fitness (5 treinos)
        trainingPlans.Add(new TrainingPlan { PlanId = GetPlanId("Advanced"), TrainingId = GetTrainingId("Intense Cardio"), DaysPerWeek = 3 });
        trainingPlans.Add(new TrainingPlan { PlanId = GetPlanId("Advanced"), TrainingId = GetTrainingId("Heavy Lifting"), DaysPerWeek = 3 });
        trainingPlans.Add(new TrainingPlan { PlanId = GetPlanId("Advanced"), TrainingId = GetTrainingId("Endurance Spin"), DaysPerWeek = 2 });
        trainingPlans.Add(new TrainingPlan { PlanId = GetPlanId("Advanced"), TrainingId = GetTrainingId("Express Lunch"), DaysPerWeek = 2 });
        trainingPlans.Add(new TrainingPlan { PlanId = GetPlanId("Advanced"), TrainingId = GetTrainingId("Full Body Circuit"), DaysPerWeek = 2 });

        // Mind & Body (6 treinos)
        trainingPlans.Add(new TrainingPlan { PlanId = GetPlanId("Mind"), TrainingId = GetTrainingId("Pilates Core"), DaysPerWeek = 2 });
        trainingPlans.Add(new TrainingPlan { PlanId = GetPlanId("Mind"), TrainingId = GetTrainingId("Power Yoga"), DaysPerWeek = 2 });
        trainingPlans.Add(new TrainingPlan { PlanId = GetPlanId("Mind"), TrainingId = GetTrainingId("Barre Sculpt"), DaysPerWeek = 1 });
        trainingPlans.Add(new TrainingPlan { PlanId = GetPlanId("Mind"), TrainingId = GetTrainingId("Sunday Morning"), DaysPerWeek = 1 });
        trainingPlans.Add(new TrainingPlan { PlanId = GetPlanId("Mind"), TrainingId = GetTrainingId("Reformer Intro"), DaysPerWeek = 1 });
        trainingPlans.Add(new TrainingPlan { PlanId = GetPlanId("Mind"), TrainingId = GetTrainingId("Deep Stretch"), DaysPerWeek = 2 });

        // Ultimate Transformation (6 treinos)
        trainingPlans.Add(new TrainingPlan { PlanId = GetPlanId("Ultimate"), TrainingId = GetTrainingId("Lower Body Endurance"), DaysPerWeek = 2 });
        trainingPlans.Add(new TrainingPlan { PlanId = GetPlanId("Ultimate"), TrainingId = GetTrainingId("Boxing Technique"), DaysPerWeek = 2 });
        trainingPlans.Add(new TrainingPlan { PlanId = GetPlanId("Ultimate"), TrainingId = GetTrainingId("Kettlebell Power"), DaysPerWeek = 2 });
        trainingPlans.Add(new TrainingPlan { PlanId = GetPlanId("Ultimate"), TrainingId = GetTrainingId("TRX Suspension"), DaysPerWeek = 2 });
        trainingPlans.Add(new TrainingPlan { PlanId = GetPlanId("Ultimate"), TrainingId = GetTrainingId("Power Hour Max"), DaysPerWeek = 1 });
        trainingPlans.Add(new TrainingPlan { PlanId = GetPlanId("Ultimate"), TrainingId = GetTrainingId("Weekend Metabolic"), DaysPerWeek = 1 });

        // Corporate (3 treinos)
        trainingPlans.Add(new TrainingPlan { PlanId = GetPlanId("Corporate"), TrainingId = GetTrainingId("Deep Water Toning"), DaysPerWeek = 1 });
        trainingPlans.Add(new TrainingPlan { PlanId = GetPlanId("Corporate"), TrainingId = GetTrainingId("Zumba Gold"), DaysPerWeek = 1 });
        trainingPlans.Add(new TrainingPlan { PlanId = GetPlanId("Corporate"), TrainingId = GetTrainingId("Sunday Morning"), DaysPerWeek = 1 });

        dbContext.TrainingPlan.AddRange(trainingPlans);
        dbContext.SaveChanges();
    }
}