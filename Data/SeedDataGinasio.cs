using HealthWellbeing.Data;
using HealthWellbeing.Models;
using System.Collections.Generic;

internal class SeedDataGinasio
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
        var trainers = PopulateTrainer(dbContext);
        PopulateTraining(dbContext, trainers);
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
        },
        new TrainingType
        {
            Name = "Indoor Cycling",
            Description = "Intense stationary bike workout focused on endurance and lower body strength.",
            DurationMinutes = 45,
            IsActive = true
        },
        new TrainingType
        {
            Name = "Cardio Kickboxing",
            Description = "High-energy martial arts inspired workout with punches and kicks.",
            DurationMinutes = 60,
            IsActive = true
        },
        new TrainingType
        {
            Name = "Power Yoga",
            Description = "Vigorous fitness-based approach to vinyasa-style yoga.",
            DurationMinutes = 75,
            IsActive = true
        },
        new TrainingType
        {
            Name = "Aqua Aerobics",
            Description = "Low-impact resistance training performed in a swimming pool.",
            DurationMinutes = 45,
            IsActive = true
        },
        new TrainingType
        {
            Name = "TRX Suspension",
            Description = "Bodyweight exercises using suspension straps to develop strength and balance.",
            DurationMinutes = 50,
            IsActive = true
        },
        new TrainingType
        {
            Name = "CrossFit WOD",
            Description = "Constantly varied functional movements performed at high intensity.",
            DurationMinutes = 60,
            IsActive = true
        },
        new TrainingType
        {
            Name = "Barre Fitness",
            Description = "Ballet-inspired workout focusing on isometric strength and high repetitions.",
            DurationMinutes = 55,
            IsActive = true
        },
        new TrainingType
        {
            Name = "Boxing Technique",
            Description = "Learn the fundamentals of boxing stance, footwork, and punching combinations.",
            DurationMinutes = 60,
            IsActive = true
        },
        new TrainingType
        {
            Name = "Meditation & Breathwork",
            Description = "Guided session to reduce stress and improve mental clarity through breathing.",
            DurationMinutes = 30,
            IsActive = true
        },
        new TrainingType
        {
            Name = "Kettlebell Flow",
            Description = "Dynamic full-body workout using kettlebells to improve power and grip strength.",
            DurationMinutes = 45,
            IsActive = true
        },
        new TrainingType
        {
            Name = "Calisthenics",
            Description = "Street workout style using body weight for gymnastics and strength exercises.",
            DurationMinutes = 60,
            IsActive = true
        },
        new TrainingType
        {
            Name = "Senior Mobility",
            Description = "Gentle exercises designed to maintain range of motion and balance for seniors.",
            DurationMinutes = 40,
            IsActive = true
        },
        new TrainingType
        {
            Name = "Tai Chi",
            Description = "Internal Chinese martial art practiced for defense training and health benefits.",
            DurationMinutes = 60,
            IsActive = true
        },
        new TrainingType
        {
            Name = "Boot Camp",
            Description = "Military-style circuit training outdoors involving running and bodyweight drills.",
            DurationMinutes = 60,
            IsActive = true
        },
        new TrainingType
        {
            Name = "Step Aerobics",
            Description = "Choreographed cardio routine using an elevated platform.",
            DurationMinutes = 50,
            IsActive = true
        },
        new TrainingType
        {
            Name = "Pre-Natal Yoga",
            Description = "Safe yoga postures and breathing techniques for expectant mothers.",
            DurationMinutes = 60,
            IsActive = true
        },
        new TrainingType
        {
            Name = "Tabata Torch",
            Description = "Ultra-high intensity interval training: 20 seconds work, 10 seconds rest.",
            DurationMinutes = 30,
            IsActive = true
        },
        new TrainingType
        {
            Name = "Powerlifting Basics",
            Description = "Focus on the three big lifts: Squat, Bench Press, and Deadlift.",
            DurationMinutes = 90,
            IsActive = true
        },
        new TrainingType
        {
            Name = "Deep Stretch Recovery",
            Description = "Slow-paced class holding stretches for longer periods to release muscle tension.",
            DurationMinutes = 45,
            IsActive = true
        },
        new TrainingType
        {
            Name = "Functional Circuit",
            Description = "Station-based workout mimicking daily life movements.",
            DurationMinutes = 50,
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
        // 1. John Smith (HIIT)
        new Trainer
        {
            Name = "John Smith",
            Speciality = "HIIT (High Intensity Interval Training) & Cardio",
            Email = "john.smith@fitnesspro.com",
            Phone = "555-1112233",
            BirthDate = new DateTime(1988, 7, 10),
            Gender = "Male",
            //IsActive = true
        },
        // 2. Emma Johnson (Strength)
        new Trainer
        {
            Name = "Emma Johnson",
            Speciality = "Strength Training & Powerlifting",
            Email = "emma.johnson@strongfit.net",
            Phone = "555-2223344",
            BirthDate = new DateTime(1992, 11, 25),
            Gender = "Female",
            //IsActive = true
        },
        // 3. Carlos Mendes (Yoga)
        new Trainer
        {
            Name = "Carlos Mendes",
            Speciality = "Yoga Basics & Meditation",
            Email = "carlos.mendes@yogabalance.org",
            Phone = "555-3334455",
            BirthDate = new DateTime(1975, 4, 1),
            Gender = "Male",
            //IsActive = true
        },
        // 4. Sophie Lee (Pilates)
        new Trainer
        {
            Name = "Sophie Lee",
            Speciality = "Pilates Core Strength & Deep Stretch",
            Email = "sophie.lee@corewellness.com",
            Phone = "555-4445566",
            BirthDate = new DateTime(1996, 2, 14),
            Gender = "Female",
            //IsActive = true
        },
        // 5. Maria Rodriguez (Zumba)
        new Trainer
        {
            Name = "Maria Rodriguez",
            Speciality = "Zumba Dance & Aqua Aerobics",
            Email = "maria.rodriguez@zumbafit.com",
            Phone = "555-5557788",
            BirthDate = new DateTime(1985, 9, 30),
            Gender = "Female",
            //IsActive = true
        },
        // 6. NOVO: David Costa (Cycling/Boxing)
        new Trainer
        {
            Name = "David Costa",
            Speciality = "Indoor Cycling & Boxing Technique",
            Email = "david.costa@cyclenbox.com",
            Phone = "555-6668899",
            BirthDate = new DateTime(1990, 6, 5),
            Gender = "Male",
            //IsActive = true
        },
        // 7. NOVO: Laura Silva (Barre/Power Yoga)
        new Trainer
        {
            Name = "Laura Silva",
            Speciality = "Barre Fitness & Power Yoga",
            Email = "laura.silva@barrenergy.com",
            Phone = "555-7770011",
            BirthDate = new DateTime(1994, 3, 18),
            Gender = "Female",
            //IsActive = true
        },
        // 8. NOVO: André Santos (Functional/Kettlebell)
        new Trainer
        {
            Name = "André Santos",
            Speciality = "TRX Suspension & Kettlebell Flow",
            Email = "andre.santos@functional.com",
            Phone = "555-8889900",
            BirthDate = new DateTime(1983, 1, 22),
            Gender = "Male",
            //IsActive = true
        }
    });

        dbContext.SaveChanges();
        return dbContext.Trainer.ToList();
    }

    private static void PopulateTraining(HealthWellbeingDbContext dbContext, List<Trainer> trainers)
    {
        if (dbContext.Training.Any()) return;

        // --- 1. GET TRAINER IDs (8 TRAINERS) ---
        var carlosId = trainers.FirstOrDefault(t => t.Name == "Carlos Mendes")?.TrainerId;
        var johnId = trainers.FirstOrDefault(t => t.Name == "John Smith")?.TrainerId;
        var emmaId = trainers.FirstOrDefault(t => t.Name == "Emma Johnson")?.TrainerId; // Novo
        var sophieId = trainers.FirstOrDefault(t => t.Name == "Sophie Lee")?.TrainerId;
        var mariaId = trainers.FirstOrDefault(t => t.Name == "Maria Rodriguez")?.TrainerId;
        var davidId = trainers.FirstOrDefault(t => t.Name == "David Costa")?.TrainerId; // Novo
        var lauraId = trainers.FirstOrDefault(t => t.Name == "Laura Silva")?.TrainerId; // Novo
        var andreId = trainers.FirstOrDefault(t => t.Name == "André Santos")?.TrainerId; // Novo

        // --- 2. GET TRAINING TYPE IDs ---
        var yogaTypeId = dbContext.TrainingType.FirstOrDefault(t => t.Name == "Yoga Basics")?.TrainingTypeId;
        var hiitTypeId = dbContext.TrainingType.FirstOrDefault(t => t.Name == "HIIT (High Intensity Interval Training)")?.TrainingTypeId;
        var pilatesTypeId = dbContext.TrainingType.FirstOrDefault(t => t.Name == "Pilates Core Strength")?.TrainingTypeId;
        var zumbaTypeId = dbContext.TrainingType.FirstOrDefault(t => t.Name == "Zumba Dance")?.TrainingTypeId;
        var strengthTypeId = dbContext.TrainingType.FirstOrDefault(t => t.Name == "Strength Training")?.TrainingTypeId;
        var powerYogaTypeId = dbContext.TrainingType.FirstOrDefault(tt => tt.Name == "Power Yoga")?.TrainingTypeId;
        var indoorCyclingTypeId = dbContext.TrainingType.FirstOrDefault(tt => tt.Name == "Indoor Cycling")?.TrainingTypeId;
        var aquaAerobicsTypeId = dbContext.TrainingType.FirstOrDefault(tt => tt.Name == "Aqua Aerobics")?.TrainingTypeId;
        var trxTypeId = dbContext.TrainingType.FirstOrDefault(tt => tt.Name == "TRX Suspension")?.TrainingTypeId;
        var barreTypeId = dbContext.TrainingType.FirstOrDefault(tt => tt.Name == "Barre Fitness")?.TrainingTypeId;
        var boxingTypeId = dbContext.TrainingType.FirstOrDefault(tt => tt.Name == "Boxing Technique")?.TrainingTypeId;
        var meditationTypeId = dbContext.TrainingType.FirstOrDefault(tt => tt.Name == "Meditation & Breathwork")?.TrainingTypeId;
        var kettlebellTypeId = dbContext.TrainingType.FirstOrDefault(tt => tt.Name == "Kettlebell Flow")?.TrainingTypeId;

        // Verificação de segurança (verifica se os IDs essenciais existem)
        if (carlosId.HasValue && johnId.HasValue && emmaId.HasValue && sophieId.HasValue && mariaId.HasValue && davidId.HasValue && lauraId.HasValue && andreId.HasValue &&
            yogaTypeId.HasValue && hiitTypeId.HasValue && strengthTypeId.HasValue && pilatesTypeId.HasValue && zumbaTypeId.HasValue)
        {
            dbContext.Training.AddRange(new List<Training>()
        {
            // --- TREINOS ORIGINAIS (ajustados ao novo Trainer ID) ---
            new Training
            {
                TrainingTypeId = yogaTypeId.Value,
                TrainerId = carlosId.Value,
                Name = "Morning Yoga Flow",
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
                TrainingTypeId = strengthTypeId.Value, // Ajustado para Strength (Emma)
                TrainerId = emmaId.Value,
                Name = "Heavy Lifting Focus (Squat/Bench)",
                Duration = 120,
                DayOfWeek = "Friday",
                StartTime = new TimeSpan(16, 0, 0),
                MaxParticipants = 8
            },
            
            // --- 20 NOVOS TREINOS BALANCEADOS ---
            new Training
            {
                TrainingTypeId = pilatesTypeId.Value,
                TrainerId = sophieId.Value, // Sophie Lee
                Name = "Pilates Core Stability",
                Duration = 50,
                DayOfWeek = "Tuesday",
                StartTime = new TimeSpan(9, 0, 0),
                MaxParticipants = 10
            },
            new Training
            {
                TrainingTypeId = zumbaTypeId.Value,
                TrainerId = mariaId.Value, // Maria Rodriguez
                Name = "Zumba Latin Fiesta",
                Duration = 55,
                DayOfWeek = "Thursday",
                StartTime = new TimeSpan(19, 0, 0),
                MaxParticipants = 30
            },
            new Training
            {
                TrainingTypeId = powerYogaTypeId.Value,
                TrainerId = lauraId.Value, // Laura Silva
                Name = "Power Yoga: Midday Vinyasa",
                Duration = 75,
                DayOfWeek = "Monday",
                StartTime = new TimeSpan(12, 30, 0),
                MaxParticipants = 12
            },
            new Training
            {
                TrainingTypeId = indoorCyclingTypeId.Value,
                TrainerId = davidId.Value, // David Costa
                Name = "Morning Endurance Spin",
                Duration = 45,
                DayOfWeek = "Wednesday",
                StartTime = new TimeSpan(7, 0, 0),
                MaxParticipants = 18
            },
            new Training
            {
                TrainingTypeId = aquaAerobicsTypeId.Value,
                TrainerId = mariaId.Value, // Maria Rodriguez
                Name = "Deep Water Toning",
                Duration = 45,
                DayOfWeek = "Saturday",
                StartTime = new TimeSpan(11, 0, 0),
                MaxParticipants = 25
            },
            new Training
            {
                TrainingTypeId = trxTypeId.Value,
                TrainerId = andreId.Value, // André Santos
                Name = "TRX Suspension Full Body",
                Duration = 50,
                DayOfWeek = "Tuesday",
                StartTime = new TimeSpan(17, 30, 0),
                MaxParticipants = 10
            },
            new Training
            {
                TrainingTypeId = hiitTypeId.Value,
                TrainerId = johnId.Value,
                Name = "Express Lunch HIIT",
                Duration = 30,
                DayOfWeek = "Friday",
                StartTime = new TimeSpan(13, 0, 0),
                MaxParticipants = 20
            },
            new Training
            {
                TrainingTypeId = barreTypeId.Value,
                TrainerId = lauraId.Value, // Laura Silva
                Name = "Barre Sculpt and Tone",
                Duration = 55,
                DayOfWeek = "Monday",
                StartTime = new TimeSpan(19, 30, 0),
                MaxParticipants = 15
            },
            new Training
            {
                TrainingTypeId = boxingTypeId.Value,
                TrainerId = davidId.Value, // David Costa
                Name = "Boxing Technique Workshop",
                Duration = 60,
                DayOfWeek = "Thursday",
                StartTime = new TimeSpan(18, 0, 0),
                MaxParticipants = 16
            },
            new Training
            {
                TrainingTypeId = meditationTypeId.Value,
                TrainerId = carlosId.Value, // Carlos Mendes
                Name = "Sunday Morning Calm",
                Duration = 30,
                DayOfWeek = "Sunday",
                StartTime = new TimeSpan(10, 0, 0),
                MaxParticipants = 40
            },
            new Training
            {
                TrainingTypeId = kettlebellTypeId.Value,
                TrainerId = andreId.Value, // André Santos
                Name = "Kettlebell Power Flow",
                Duration = 45,
                DayOfWeek = "Wednesday",
                StartTime = new TimeSpan(17, 30, 0),
                MaxParticipants = 14
            },
            new Training
            {
                TrainingTypeId = strengthTypeId.Value,
                TrainerId = emmaId.Value, // Emma Johnson
                Name = "Full Body Circuit (Strength)",
                Duration = 60,
                DayOfWeek = "Tuesday",
                StartTime = new TimeSpan(18, 30, 0),
                MaxParticipants = 12
            },
            new Training
            {
                TrainingTypeId = strengthTypeId.Value,
                TrainerId = johnId.Value,
                Name = "Lower Body Endurance",
                Duration = 60,
                DayOfWeek = "Thursday",
                StartTime = new TimeSpan(17, 0, 0),
                MaxParticipants = 12
            },
            new Training
            {
                TrainingTypeId = yogaTypeId.Value,
                TrainerId = carlosId.Value,
                Name = "Gentle Hatha Evening",
                Duration = 60,
                DayOfWeek = "Wednesday",
                StartTime = new TimeSpan(20, 0, 0),
                MaxParticipants = 15
            },
            new Training
            {
                TrainingTypeId = zumbaTypeId.Value,
                TrainerId = mariaId.Value,
                Name = "Zumba Gold (Low Impact)",
                Duration = 55,
                DayOfWeek = "Friday",
                StartTime = new TimeSpan(9, 30, 0),
                MaxParticipants = 25
            },
            new Training
            {
                TrainingTypeId = indoorCyclingTypeId.Value,
                TrainerId = davidId.Value,
                Name = "Power Hour Max Cycle",
                Duration = 60,
                DayOfWeek = "Saturday",
                StartTime = new TimeSpan(9, 0, 0),
                MaxParticipants = 18
            },
            new Training
            {
                TrainingTypeId = pilatesTypeId.Value,
                TrainerId = sophieId.Value,
                Name = "Reformer Intro Session",
                Duration = 50,
                DayOfWeek = "Thursday",
                StartTime = new TimeSpan(12, 0, 0),
                MaxParticipants = 8
            },
            new Training
            {
                TrainingTypeId = hiitTypeId.Value,
                TrainerId = andreId.Value, // André Santos
                Name = "Weekend Metabolic Burn",
                Duration = 45,
                DayOfWeek = "Saturday",
                StartTime = new TimeSpan(14, 0, 0),
                MaxParticipants = 20
            },
            new Training
            {
                TrainingTypeId = aquaAerobicsTypeId.Value,
                TrainerId = mariaId.Value, // Maria Rodriguez
                Name = "Water Circuit Training",
                Duration = 45,
                DayOfWeek = "Monday",
                StartTime = new TimeSpan(16, 0, 0),
                MaxParticipants = 25
            },
            new Training
            {
                TrainingTypeId = barreTypeId.Value,
                TrainerId = lauraId.Value, // Laura Silva
                Name = "Deep Stretch & Barre",
                Duration = 55,
                DayOfWeek = "Sunday",
                StartTime = new TimeSpan(17, 0, 0),
                MaxParticipants = 15
            }
        });

            dbContext.SaveChanges();
        }
    }
}