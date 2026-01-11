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
        new Client { Name = "Mona Lisa", Email = "mona.l@art.com", Phone = "918880008", Address = "The Louvre, Paris", BirthDate = new DateTime(1993, 6, 15), Gender = "Female", RegistrationDate = DateTime.Now.AddDays(-50) },
        new Client { Name = "Neo Anderson", Email = "neo.a@matrix.com", Phone = "919990009", Address = "Nebuchadnezzar St", BirthDate = new DateTime(1971, 9, 13), Gender = "Male", RegistrationDate = DateTime.Now.AddDays(-2) },
        new Client { Name = "Olivia Pope", Email = "olivia.p@gladiator.com", Phone = "910100100", Address = "Washington D.C.", BirthDate = new DateTime(1977, 4, 2), Gender = "Female", RegistrationDate = DateTime.Now.AddDays(-60) },
        new Client { Name = "Peter Parker", Email = "peter.p@bugle.com", Phone = "912020011", Address = "Queens, NY", BirthDate = new DateTime(2001, 8, 10), Gender = "Male", RegistrationDate = DateTime.Now.AddDays(-7) },
        new Client { Name = "Quinn Fabray", Email = "quinn.f@glee.com", Phone = "913030012", Address = "Lima, Ohio", BirthDate = new DateTime(1994, 7, 19), Gender = "Female", RegistrationDate = DateTime.Now.AddDays(-33) },
        new Client { Name = "Rachel Green", Email = "rachel.g@friends.com", Phone = "914040013", Address = "Central Perk, NY", BirthDate = new DateTime(1970, 5, 5), Gender = "Female", RegistrationDate = DateTime.Now.AddDays(-45) },
        new Client { Name = "Steve Rogers", Email = "steve.r@avengers.com", Phone = "915050014", Address = "Brooklyn, NY", BirthDate = new DateTime(1918, 7, 4), Gender = "Male", RegistrationDate = DateTime.Now.AddDays(-11) },
        new Client { Name = "Tony Stark", Email = "tony.s@stark.com", Phone = "916060015", Address = "Malibu Point, CA", BirthDate = new DateTime(1970, 5, 29), Gender = "Male", RegistrationDate = DateTime.Now.AddDays(-90) },
        new Client { Name = "Ursula Buffay", Email = "ursula.b@friends.tv", Phone = "917070016", Address = "Riff's Bar, NY", BirthDate = new DateTime(1968, 2, 22), Gender = "Female", RegistrationDate = DateTime.Now.AddDays(-14) },
        new Client { Name = "Victor Frankenstein", Email = "victor.f@science.ch", Phone = "918080017", Address = "Geneva, Switzerland", BirthDate = new DateTime(1810, 10, 10), Gender = "Male", RegistrationDate = DateTime.Now.AddDays(-200) },
        new Client { Name = "Walter White", Email = "walter.w@heisenberg.com", Phone = "919090018", Address = "Albuquerque, NM", BirthDate = new DateTime(1958, 9, 7), Gender = "Male", RegistrationDate = DateTime.Now.AddDays(-28) },
        new Client { Name = "Xena Warrior", Email = "xena.w@myth.gr", Phone = "910100119", Address = "Amphipolis, Greece", BirthDate = new DateTime(1968, 3, 29), Gender = "Female", RegistrationDate = DateTime.Now.AddDays(-1) },
        new Client { Name = "Yoda Master", Email = "yoda.m@jedi.org", Phone = "912120020", Address = "Dagobah System", BirthDate = new DateTime(1900, 1, 1), Gender = "Male", RegistrationDate = DateTime.Now.AddDays(-500) },
        new Client { Name = "Zelda Hyrule", Email = "zelda.h@nintendo.jp", Phone = "913130021", Address = "Hyrule Castle", BirthDate = new DateTime(1986, 2, 21), Gender = "Female", RegistrationDate = DateTime.Now.AddDays(-120) },
        new Client { Name = "Arthur Pendragon", Email = "arthur.p@camelot.uk", Phone = "914140022", Address = "Round Table", BirthDate = new DateTime(1980, 12, 25), Gender = "Male", RegistrationDate = DateTime.Now.AddDays(-365) },
        new Client { Name = "Beatrix Kiddo", Email = "beatrix.k@viper.com", Phone = "915150023", Address = "El Paso, TX", BirthDate = new DateTime(1976, 5, 10), Gender = "Female", RegistrationDate = DateTime.Now.AddDays(-15) },
        new Client { Name = "Clark Kent", Email = "clark.k@dailyplanet.com", Phone = "916160024", Address = "Metropolis", BirthDate = new DateTime(1978, 6, 18), Gender = "Male", RegistrationDate = DateTime.Now.AddDays(-40) },
        new Client { Name = "Diana Prince", Email = "diana.p@themyscira.gov", Phone = "917170025", Address = "Amazon Island", BirthDate = new DateTime(1985, 3, 22), Gender = "Female", RegistrationDate = DateTime.Now.AddDays(-5) },
        new Client { Name = "Elliot Alderson", Email = "elliot.a@fsociety.org", Phone = "918180026", Address = "Coney Island, NY", BirthDate = new DateTime(1986, 9, 17), Gender = "Male", RegistrationDate = DateTime.Now.AddDays(-22) },
        new Client { Name = "Fiona Gallagher", Email = "fiona.g@southside.chi", Phone = "919190027", Address = "South Side, Chicago", BirthDate = new DateTime(1989, 1, 30), Gender = "Female", RegistrationDate = DateTime.Now.AddDays(-10) },
        new Client { Name = "Geralt Rivia", Email = "geralt.r@witcher.pl", Phone = "910200228", Address = "Kaer Morhen", BirthDate = new DateTime(1975, 5, 5), Gender = "Male", RegistrationDate = DateTime.Now.AddDays(-400) }
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
            new Trainer { Name = "John Smith", Address = "123 HIIT St, New York", Email = "john@gym.com", Phone = "555-111", BirthDate = new DateTime(1988, 7, 10), Gender = "Male" },
            new Trainer { Name = "Emma Johnson", Address = "45 Strength Blvd, London", Email = "emma@gym.com", Phone = "555-222", BirthDate = new DateTime(1992, 11, 25), Gender = "Female" },
            new Trainer { Name = "Carlos Mendes", Address = "8 Yoga Lane, Lisbon", Email = "carlos@gym.com", Phone = "555-333", BirthDate = new DateTime(1975, 4, 1), Gender = "Male" },
            new Trainer { Name = "Sophie Lee", Address = "14 Pilates Rd, Seoul", Email = "sophie@gym.com", Phone = "555-444", BirthDate = new DateTime(1996, 2, 14), Gender = "Female" },
            new Trainer { Name = "Maria Rodriguez", Address = "90 Zumba St, Madrid", Email = "maria@gym.com", Phone = "555-555", BirthDate = new DateTime(1985, 9, 30), Gender = "Female" },
            new Trainer { Name = "David Costa", Address = "22 Cycling Way, Porto", Email = "david@gym.com", Phone = "555-666", BirthDate = new DateTime(1990, 6, 5), Gender = "Male" },
            new Trainer { Name = "Laura Silva", Address = "101 Barre Ave, Coimbra", Email = "laura@gym.com", Phone = "555-777", BirthDate = new DateTime(1994, 3, 18), Gender = "Female" },
            new Trainer { Name = "André Santos", Address = "3 Functional Cir, Braga", Email = "andre@gym.com", Phone = "555-888", BirthDate = new DateTime(1983, 1, 22), Gender = "Male" }
        };

        dbContext.Trainer.AddRange(trainers);
        dbContext.SaveChanges();
        return trainers;
    }

    private static List<Training> PopulateTraining(
    HealthWellbeingDbContext dbContext,
    List<Trainer> trainers)
    {
        if (dbContext.Training.Any())
            return dbContext.Training.ToList();

        var trainings = new List<Training>();
        var trainingTypes = dbContext.TrainingType.ToList();

        int GetTrainerId(string namePart) =>
            trainers.First(t => t.Name.Contains(namePart)).TrainerId;

        int GetTypeId(string namePart) =>
            trainingTypes.First(t => t.Name.Contains(namePart)).TrainingTypeId;

        // -------- BASIC WELLNESS --------
        trainings.Add(new Training
        {
            Name = "Morning Yoga",
            Description = "Start the day right",
            DayOfWeek = WeekDay.Monday,
            StartTime = new TimeSpan(9, 0, 0),
            Duration = 60,
            MaxParticipants = 15,
            TrainerId = GetTrainerId("Carlos"),
            TrainingTypeId = GetTypeId("Yoga")
        });

        trainings.Add(new Training
        {
            Name = "Zumba Latin",
            Description = "Dance workout",
            DayOfWeek = WeekDay.Thursday,
            StartTime = new TimeSpan(18, 0, 0),
            Duration = 55,
            MaxParticipants = 30,
            TrainerId = GetTrainerId("Maria"),
            TrainingTypeId = GetTypeId("Zumba")
        });
        trainings.Add(new Training
        {
            Name = "Zumba Gold",
            Description = "Low impact dance workout",
            DayOfWeek = WeekDay.Friday,
            StartTime = new TimeSpan(17, 0, 0),
            Duration = 55,
            MaxParticipants = 25,
            TrainerId = GetTrainerId("Maria"),
            TrainingTypeId = GetTypeId("Zumba")
        });


        trainings.Add(new Training
        {
            Name = "Gentle Hatha",
            Description = "Relaxing yoga",
            DayOfWeek = WeekDay.Wednesday,
            StartTime = new TimeSpan(19, 0, 0),
            Duration = 60,
            MaxParticipants = 15,
            TrainerId = GetTrainerId("Carlos"),
            TrainingTypeId = GetTypeId("Yoga")
        });

        trainings.Add(new Training
        {
            Name = "Water Circuit",
            Description = "Pool circuit",
            DayOfWeek = WeekDay.Monday,
            StartTime = new TimeSpan(10, 0, 0),
            Duration = 45,
            MaxParticipants = 25,
            TrainerId = GetTrainerId("Maria"),
            TrainingTypeId = GetTypeId("Aqua")
        });

        // -------- ADVANCED FITNESS --------
        trainings.Add(new Training
        {
            Name = "Intense Cardio",
            Description = "High intensity cardio",
            DayOfWeek = WeekDay.Wednesday,
            StartTime = new TimeSpan(7, 30, 0),
            Duration = 45,
            MaxParticipants = 20,
            TrainerId = GetTrainerId("John"),
            TrainingTypeId = GetTypeId("HIIT")
        });

        trainings.Add(new Training
        {
            Name = "Heavy Lifting",
            Description = "Strength training",
            DayOfWeek = WeekDay.Friday,
            StartTime = new TimeSpan(17, 0, 0),
            Duration = 90,
            MaxParticipants = 10,
            TrainerId = GetTrainerId("Emma"),
            TrainingTypeId = GetTypeId("Strength")
        });

        trainings.Add(new Training
        {
            Name = "Endurance Spin",
            Description = "Cycling endurance",
            DayOfWeek = WeekDay.Wednesday,
            StartTime = new TimeSpan(18, 30, 0),
            Duration = 45,
            MaxParticipants = 18,
            TrainerId = GetTrainerId("David"),
            TrainingTypeId = GetTypeId("Cycling")
        });

        trainings.Add(new Training
        {
            Name = "Express Lunch",
            Description = "Quick lunch workout",
            DayOfWeek = WeekDay.Friday,
            StartTime = new TimeSpan(12, 30, 0),
            Duration = 30,
            MaxParticipants = 20,
            TrainerId = GetTrainerId("John"),
            TrainingTypeId = GetTypeId("HIIT")
        });

        trainings.Add(new Training
        {
            Name = "Full Body Circuit",
            Description = "Strength circuit",
            DayOfWeek = WeekDay.Tuesday,
            StartTime = new TimeSpan(18, 0, 0),
            Duration = 60,
            MaxParticipants = 12,
            TrainerId = GetTrainerId("Emma"),
            TrainingTypeId = GetTypeId("Strength")
        });

        // -------- MIND & BODY --------
        trainings.Add(new Training
        {
            Name = "Pilates Core",
            Description = "Core strength",
            DayOfWeek = WeekDay.Tuesday,
            StartTime = new TimeSpan(17, 0, 0),
            Duration = 50,
            MaxParticipants = 12,
            TrainerId = GetTrainerId("Sophie"),
            TrainingTypeId = GetTypeId("Pilates")
        });

        trainings.Add(new Training
        {
            Name = "Power Yoga",
            Description = "Vigorous yoga",
            DayOfWeek = WeekDay.Monday,
            StartTime = new TimeSpan(13, 0, 0),
            Duration = 75,
            MaxParticipants = 12,
            TrainerId = GetTrainerId("Laura"),
            TrainingTypeId = GetTypeId("Yoga")
        });

        trainings.Add(new Training
        {
            Name = "Barre Sculpt",
            Description = "Ballet inspired",
            DayOfWeek = WeekDay.Monday,
            StartTime = new TimeSpan(18, 0, 0),
            Duration = 55,
            MaxParticipants = 15,
            TrainerId = GetTrainerId("Laura"),
            TrainingTypeId = GetTypeId("Barre")
        });

        trainings.Add(new Training
        {
            Name = "Sunday Morning",
            Description = "Meditation session",
            DayOfWeek = WeekDay.Sunday,
            StartTime = new TimeSpan(9, 0, 0),
            Duration = 30,
            MaxParticipants = 40,
            TrainerId = GetTrainerId("Carlos"),
            TrainingTypeId = GetTypeId("Meditation")
        });

        trainings.Add(new Training
        {
            Name = "Reformer Intro",
            Description = "Machine pilates",
            DayOfWeek = WeekDay.Thursday,
            StartTime = new TimeSpan(16, 0, 0),
            Duration = 50,
            MaxParticipants = 8,
            TrainerId = GetTrainerId("Sophie"),
            TrainingTypeId = GetTypeId("Pilates")
        });

        trainings.Add(new Training
        {
            Name = "Deep Stretch",
            Description = "Recovery session",
            DayOfWeek = WeekDay.Sunday,
            StartTime = new TimeSpan(18, 0, 0),
            Duration = 55,
            MaxParticipants = 15,
            TrainerId = GetTrainerId("Laura"),
            TrainingTypeId = GetTypeId("Barre")
        });

        // -------- ULTIMATE --------
        trainings.Add(new Training
        {
            Name = "Lower Body Endurance",
            Description = "Leg focus",
            DayOfWeek = WeekDay.Thursday,
            StartTime = new TimeSpan(19, 0, 0),
            Duration = 60,
            MaxParticipants = 12,
            TrainerId = GetTrainerId("John"),
            TrainingTypeId = GetTypeId("Strength")
        });

        trainings.Add(new Training
        {
            Name = "Boxing Technique",
            Description = "Boxing basics",
            DayOfWeek = WeekDay.Thursday,
            StartTime = new TimeSpan(20, 0, 0),
            Duration = 60,
            MaxParticipants = 16,
            TrainerId = GetTrainerId("David"),
            TrainingTypeId = GetTypeId("Boxing")
        });

        trainings.Add(new Training
        {
            Name = "Kettlebell Power",
            Description = "Dynamic strength",
            DayOfWeek = WeekDay.Wednesday,
            StartTime = new TimeSpan(19, 0, 0),
            Duration = 45,
            MaxParticipants = 14,
            TrainerId = GetTrainerId("André"),
            TrainingTypeId = GetTypeId("Kettlebell")
        });

        trainings.Add(new Training
        {
            Name = "TRX Suspension",
            Description = "Bodyweight training",
            DayOfWeek = WeekDay.Tuesday,
            StartTime = new TimeSpan(20, 0, 0),
            Duration = 50,
            MaxParticipants = 10,
            TrainerId = GetTrainerId("André"),
            TrainingTypeId = GetTypeId("TRX")
        });

        trainings.Add(new Training
        {
            Name = "Power Hour Max",
            Description = "High intensity cycling",
            DayOfWeek = WeekDay.Saturday,
            StartTime = new TimeSpan(10, 0, 0),
            Duration = 60,
            MaxParticipants = 18,
            TrainerId = GetTrainerId("David"),
            TrainingTypeId = GetTypeId("Cycling")
        });

        trainings.Add(new Training
        {
            Name = "Weekend Metabolic",
            Description = "Calorie burn",
            DayOfWeek = WeekDay.Saturday,
            StartTime = new TimeSpan(11, 30, 0),
            Duration = 45,
            MaxParticipants = 20,
            TrainerId = GetTrainerId("André"),
            TrainingTypeId = GetTypeId("HIIT")
        });

        // -------- EXTRA (CORRIGE O ERRO ATUAL) --------
        trainings.Add(new Training
        {
            Name = "Deep Water Toning",
            Description = "Pool toning session",
            DayOfWeek = WeekDay.Saturday,
            StartTime = new TimeSpan(12, 0, 0),
            Duration = 45,
            MaxParticipants = 25,
            TrainerId = GetTrainerId("Maria"),
            TrainingTypeId = GetTypeId("Aqua")
        });

        dbContext.Training.AddRange(trainings);
        dbContext.SaveChanges();
        return trainings;
    }


    private static void PopulateMemberPlan(HealthWellbeingDbContext dbContext, List<Member> members, List<Plan> plans)
    {
        if (dbContext.MemberPlan.Any()) return;

        var memberPlans = new List<MemberPlan>();


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
        int GetTrainingId(string namePart)
        {
            var training = trainings.FirstOrDefault(t => t.Name.Contains(namePart));
            if (training == null)
                throw new Exception($"Training not found in SeedData: {namePart}");

            return training.TrainingId;
        }


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