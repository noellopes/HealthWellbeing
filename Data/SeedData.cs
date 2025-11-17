using HealthWellbeing.Data;
using HealthWellbeing.Models;
using System.Collections.Generic;

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