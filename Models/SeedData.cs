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
                RegistrationDate = DateTime.Now.AddDays(-30),
                CreateMember = true
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
                CreateMember = null
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
                RegistrationDate = DateTime.Now.AddDays(-5),
                CreateMember = false
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
}
