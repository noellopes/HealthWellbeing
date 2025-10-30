using HealthWellbeing.Models;
using HealthWellbeing.Data;

internal class SeedData
{
    internal static void Populate(HealthWellbeingDbContext? dbContext)
    {
        if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));

        dbContext.Database.EnsureCreated();

        // Add seed data here
        PopulateClients(dbContext);
        PopulateTrainingType(dbContext);
		PopulatePlan(dbContext);
        PopulateTrainer(dbContext);
	}

    private static void PopulateClients(HealthWellbeingDbContext dbContext)
    {
        if (dbContext.Client.Any()) return;

        dbContext.Client.AddRange(new List<Client>()
        {
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
            }
        });

        dbContext.SaveChanges();
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
                Intensity = "Low",
                IsActive = true
            },
            new TrainingType
            {
                Name = "HIIT (High Intensity Interval Training)",
                Description = "A fast-paced training session combining cardio and strength exercises for maximum calorie burn.",
                DurationMinutes = 45,
                Intensity = "High",
                IsActive = true
            },
            new TrainingType
            {
                Name = "Pilates Core Strength",
                Description = "Focus on core muscle strength, flexibility, and posture improvement.",
                DurationMinutes = 50,
                Intensity = "Moderate",
                IsActive = true
            },
            new TrainingType
            {
                Name = "Zumba Dance",
                Description = "Fun and energetic dance workout set to upbeat Latin music.",
                DurationMinutes = 55,
                Intensity = "Moderate",
                IsActive = true
            },
            new TrainingType
            {
                Name = "Strength Training",
                Description = "Weight-based training for building muscle mass and endurance.",
                DurationMinutes = 70,
                Intensity = "High",
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

    private static void PopulateTrainer(HealthWellbeingDbContext dbContext)
    {
        // Check if Trainers already exist
        if (dbContext.Trainer.Any()) return;

        dbContext.Trainer.AddRange(new List<Trainer>()
    {
        new Trainer
        {
            Name = "John Smith",
            Speciality = "HIIT (High Intensity Interval Training)",
            Email = "john.smith@fitnesspro.com",
            Phone = "555-1112233"
        },
        new Trainer
        {
            Name = "Emma Johnson",
            Speciality = "Strength Training",
            Email = "emma.johnson@strongfit.net",
            Phone = "555-2223344"
        },
        new Trainer
        {
            Name = "Carlos Mendes",
            Speciality = "Yoga Basics",
            Email = "carlos.mendes@yogabalance.org",
            Phone = "555-3334455"
        },
        new Trainer
        {
            Name = "Sophie Lee",
            Speciality = "Pilates Core Strength",
            Email = "sophie.lee@corewellness.com",
            Phone = "555-4445566"
        },
        new Trainer
        {
            Name = "Maria Rodriguez",
            Speciality = "Zumba Dance",
            Email = "maria.rodriguez@zumbafit.com",
            Phone = "555-5557788"
        }
    });

        dbContext.SaveChanges();
    }


    //POPULATE(ADD) MORE HERE!!!


}
