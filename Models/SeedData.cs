using HealthWellbeing.Data;
using HealthWellbeing.Models;

internal class SeedData
{
    internal static void Populate(HealthWellbeingDbContext? dbContext)
    {
        if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));

        dbContext.Database.EnsureCreated();

        PopulateClients(dbContext);
        PopulateUserFoodRegistrations(dbContext);
    }
		PopulateClients(dbContext);
		PopulateFoodComponent(dbContext);
	}

    private static void PopulateClients(HealthWellbeingDbContext dbContext)
    {
        if (dbContext.Client.Any()) return;
    private static void PopulateClients(HealthWellbeingDbContext dbContext)
    {
        // Check if the Client table already contains data
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
        dbContext.Client.AddRange(new List<Client>() {
			// Client 1: Fully Populated
			new Client
            {
                ClientId = Guid.NewGuid().ToString("N"), // Use a GUID for a unique ID
				Name = "Alice Wonderland",
                Email = "alice.w@example.com",
                Phone = "555-1234567",
                Address = "10 Downing St, London",
                BirthDate = new DateTime(1990, 5, 15),
                Gender = "Female",
                RegistrationDate = DateTime.Now.AddDays(-30), // Registered a month ago
				CreateMember = true
            },

			// Client 2: Minimal required fields + Pending Member
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
                CreateMember = null // Pending Acceptation
			},

			// Client 3: Member Creation Rejected
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
                CreateMember = false // Rejected/Denied
			}
        });
        dbContext.SaveChanges();
    }



        private static void PopulateFoodComponent(HealthWellbeingDbContext dbContext)
        {
        // Check if the FoodComponent table already contains data
        if (dbContext.FoodComponent.Any()) return;

        dbContext.FoodComponent.AddRange(new List<FoodComponent>()
        {
            new FoodComponent
            {
                Name = "Apple",
                Description = "Contains water, carbohydrates, fibers, vitamins, minerals, and antioxidants."
            },
            new FoodComponent
            {
                Name = "Rice",
                Description = "Rich in carbohydrates and provides a quick source of energy."
            },
            new FoodComponent
            {

                Name = "Salmon",
                Description = "Contains omega-3 fatty acids, proteins, and essential minerals."
            },
            new FoodComponent
            {
                Name = "Broccoli",
                Description = "Contains fiber, vitamins C and K, iron, and potassium."
            },
            new FoodComponent
            {
                Name = "Almonds",
                Description = "Rich in healthy fats, proteins, and vitamin E."
            },
            new FoodComponent
            {
                Name = "Eggs",
                Description = "Excellent source of protein and essential amino acids."
            },
            new FoodComponent
            {
                Name = "Oats",
                Description = "Contain fiber, vitamins, and minerals that help reduce cholesterol."
            },
            new FoodComponent
            {
                Name = "Tomato",
                Description = "Contains lycopene, an antioxidant that helps prevent heart disease."
            },
            new FoodComponent
            {
                Name = "Spinach",
                Description = "Rich in iron, calcium, magnesium, and vitamin A."
            },
            new FoodComponent
            {
                Name = "Banana",
                Description = "Good source of potassium, vitamin B6, and natural sugars for energy."
            }
        });

        dbContext.SaveChanges();
    }
}
        dbContext.SaveChanges();
    }
}