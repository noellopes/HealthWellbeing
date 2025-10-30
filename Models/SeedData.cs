using HealthWellbeing.Models;
using HealthWellbeing.Data;

internal class SeedData
{
    internal static void Populate(HealthWellbeingDbContext? dbContext)
    {
        if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));

        dbContext.Database.EnsureCreated();

        PopulateClients(dbContext);
        PopulateUserFoodRegistrations(dbContext);
    }

    private static void PopulateClients(HealthWellbeingDbContext dbContext)
    {
        if (dbContext.Client.Any()) return;

        var clients = new List<Client>
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
        };

        dbContext.Client.AddRange(clients);
        dbContext.SaveChanges();

        // Store for reuse in UserFoodRegistration
        _clientCache = clients;
    }

    // Static cache to reuse client IDs when creating food registrations
    private static List<Client>? _clientCache;

    private static void PopulateUserFoodRegistrations(HealthWellbeingDbContext dbContext)
    {
        if (dbContext.UserFoodRegistration.Any() || _clientCache == null) return;

        var alice = _clientCache.First(c => c.Name.Contains("Alice"));
        var bob = _clientCache.First(c => c.Name.Contains("Bob"));
        var charlie = _clientCache.First(c => c.Name.Contains("Charlie"));

        dbContext.UserFoodRegistration.AddRange(new List<UserFoodRegistration>()
        {
            new UserFoodRegistration
            {
                
                ClientId = alice.ClientId,
                MealDateTime = DateTime.Now.AddHours(-6),
                MealType = "Breakfast",
                FoodName = "Oatmeal with banana",
                Quantity = 250,
                Notes = "Added honey and cinnamon"
            },
            new UserFoodRegistration
            {
                
                ClientId = alice.ClientId,
                MealDateTime = DateTime.Now.AddHours(-2),
                MealType = "Lunch",
                FoodName = "Grilled chicken salad",
                Quantity = 350,
                Notes = "Used olive oil dressing"
            },
            new UserFoodRegistration
            {
              
                ClientId = bob.ClientId,
                MealDateTime = DateTime.Now.AddDays(-1).AddHours(8),
                MealType = "Breakfast",
                FoodName = "Scrambled eggs with toast",
                Quantity = 300,
                Notes = "Added avocado slices"
            },
            new UserFoodRegistration
            {
                
                ClientId = bob.ClientId,
                MealDateTime = DateTime.Now.AddDays(-1).AddHours(13),
                MealType = "Lunch",
                FoodName = "Beef stir-fry with rice",
                Quantity = 500,
                Notes = "Included vegetables: broccoli, bell peppers, carrots"
            },
            new UserFoodRegistration
            {
                
                ClientId = charlie.ClientId,
                MealDateTime = DateTime.Now.AddDays(-2).AddHours(19),
                MealType = "Dinner",
                FoodName = "Spaghetti Bolognese",
                Quantity = 400,
                Notes = "Homemade sauce"
            },
            new UserFoodRegistration
            {
                
                ClientId = charlie.ClientId,
                MealDateTime = DateTime.Now.AddDays(-2).AddHours(10),
                MealType = "Snack",
                FoodName = "Apple and peanut butter",
                Quantity = 150,
                Notes = "Healthy snack choice"
            }
        });

        dbContext.SaveChanges();
    }
}
