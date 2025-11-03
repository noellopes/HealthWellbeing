using System;
using System.Collections.Generic;
using System.Linq;
using HealthWellbeing.Data;
using HealthWellbeing.Models;

internal static class SeedData
{
    internal static void Populate(HealthWellbeingDbContext? dbContext)
    {
        if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));

        dbContext.Database.EnsureCreated();

        PopulateClients(dbContext);
        PopulateFoodCategory(dbContext);
        PopulateFood(dbContext);
        PopulateFoodComponent(dbContext);
        PopulateUserFoodRegistrations(dbContext);
    }

    // Static caches to reuse items when creating related data
    private static List<Client>? _clientCache;
    private static List<FoodCategory>? _categoryCache;

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

    private static void PopulateFoodCategory(HealthWellbeingDbContext dbContext)
    {
        if (dbContext.FoodCategory.Any()) return;

        var categories = new List<FoodCategory>
        {
            new FoodCategory { Name = "Fruits", Description = "Fresh fruits and fruit-based items." },
            new FoodCategory { Name = "Grains & Cereals", Description = "Rice, oats, bread and other grain based foods." },
            new FoodCategory { Name = "Proteins", Description = "Meats, fish, eggs and other protein rich foods." },
            new FoodCategory { Name = "Vegetables", Description = "Fresh vegetables and vegetable-based items." },
            new FoodCategory { Name = "Nuts & Seeds", Description = "Nuts, seeds and related products." },
            new FoodCategory { Name = "Dairy & Eggs", Description = "Milk, cheese, yogurt and eggs." },
            new FoodCategory { Name = "Other", Description = "Miscellaneous foods and ingredients." }
        };

        dbContext.FoodCategory.AddRange(categories);
        dbContext.SaveChanges();

        _categoryCache = categories;
    }

    private static void PopulateFood(HealthWellbeingDbContext dbContext)
    {
        if (dbContext.Food.Any()) return;

        // Ensure categories exist and load them
        if (_categoryCache == null || !_categoryCache.Any())
        {
            _categoryCache = dbContext.FoodCategory.ToList();
            if (!_categoryCache.Any()) return;
        }

        FoodCategory? fruits = _categoryCache.FirstOrDefault(c => c.Name == "Fruits");
        FoodCategory? grains = _categoryCache.FirstOrDefault(c => c.Name == "Grains & Cereals");
        FoodCategory? proteins = _categoryCache.FirstOrDefault(c => c.Name == "Proteins");
        FoodCategory? vegetables = _categoryCache.FirstOrDefault(c => c.Name == "Vegetables");
        FoodCategory? nuts = _category_cache_or_null() ?? _categoryCache.FirstOrDefault(c => c.Name == "Nuts & Seeds");
        FoodCategory? dairy = _categoryCache.FirstOrDefault(c => c.Name == "Dairy & Eggs");

        // Helper to avoid compiler warning when variable not used in some setups
        static FoodCategory? _category_cache_or_null() => null;

        var foods = new List<Food>
        {
            new Food
            {
                Name = "Apple",
                Description = "Fresh apple, sweet-tart flavor.",
                KcalPer100g = 52m,
                ProteinPer100g = 0.26m,
                CarbsPer100g = 14.0m,
                FatPer100g = 0.17m,
                FoodCategoryId = fruits?.FoodCategoryId
            },
            new Food
            {
                Name = "Banana",
                Description = "Ripe banana, good source of potassium.",
                KcalPer100g = 89m,
                ProteinPer100g = 1.09m,
                CarbsPer100g = 23.0m,
                FatPer100g = 0.33m,
                FoodCategoryId = fruits?.FoodCategoryId
            },
            new Food
            {
                Name = "Rice (white, cooked)",
                Description = "Cooked white rice.",
                KcalPer100g = 130m,
                ProteinPer100g = 2.69m,
                CarbsPer100g = 28.0m,
                FatPer100g = 0.28m,
                FoodCategoryId = grains?.FoodCategoryId
            },
            new Food
            {
                Name = "Salmon (raw)",
                Description = "Raw salmon rich in omega-3 fatty acids.",
                KcalPer100g = 208m,
                ProteinPer100g = 20.42m,
                CarbsPer100g = 0m,
                FatPer100g = 13.42m,
                FoodCategoryId = proteins?.FoodCategoryId
            },
            new Food
            {
                Name = "Broccoli",
                Description = "Fresh broccoli florets.",
                KcalPer100g = 55m,
                ProteinPer100g = 3.7m,
                CarbsPer100g = 11.1m,
                FatPer100g = 0.6m,
                FoodCategoryId = vegetables?.FoodCategoryId
            },
            new Food
            {
                Name = "Almonds",
                Description = "Raw almonds, high in healthy fats and protein.",
                KcalPer100g = 579m,
                ProteinPer100g = 21.15m,
                CarbsPer100g = 21.55m,
                FatPer100g = 49.93m,
                FoodCategoryId = nuts?.FoodCategoryId
            },
            new Food
            {
                Name = "Egg (whole)",
                Description = "Whole chicken egg, boiled or cooked.",
                KcalPer100g = 155m,
                ProteinPer100g = 13.0m,
                CarbsPer100g = 1.1m,
                FatPer100g = 11.0m,
                FoodCategoryId = dairy?.FoodCategoryId
            },
            new Food
            {
                Name = "Oats (dry)",
                Description = "Rolled oats dry weight.",
                KcalPer100g = 389m,
                ProteinPer100g = 16.9m,
                CarbsPer100g = 66.3m,
                FatPer100g = 6.9m,
                FoodCategoryId = grains?.FoodCategoryId
            },
            new Food
            {
                Name = "Tomato",
                Description = "Fresh tomato, commonly used in salads and sauces.",
                KcalPer100g = 18m,
                ProteinPer100g = 0.9m,
                CarbsPer100g = 3.9m,
                FatPer100g = 0.2m,
                FoodCategoryId = vegetables?.FoodCategoryId
            },
            new Food
            {
                Name = "Spinach",
                Description = "Fresh spinach leaves, rich in iron and vitamins.",
                KcalPer100g = 23m,
                ProteinPer100g = 2.9m,
                CarbsPer100g = 3.6m,
                FatPer100g = 0.4m,
                FoodCategoryId = vegetables?.FoodCategoryId
            }
        };

        dbContext.Food.AddRange(foods);
        dbContext.SaveChanges();
    }

    private static void PopulateUserFoodRegistrations(HealthWellbeingDbContext dbContext)
    {
        if (dbContext.UserFoodRegistration.Any()) return;

        // Ensure we have client data to reference
        if (_clientCache == null || !_clientCache.Any())
        {
            _clientCache = dbContext.Client.ToList();
            if (!_clientCache.Any()) return; // nothing to reference
        }

        var alice = _clientCache.FirstOrDefault(c => c.Name.Contains("Alice", StringComparison.OrdinalIgnoreCase));
        var bob = _client_cache_or_null() ?? _clientCache.FirstOrDefault(c => c.Name.Contains("Bob", StringComparison.OrdinalIgnoreCase));
        var charlie = _clientCache.FirstOrDefault(c => c.Name.Contains("Charlie", StringComparison.OrdinalIgnoreCase));

        // Helper to avoid compiler warning when variable not used in some setups
        static Client? _client_cache_or_null() => null;

        // require at least one of them to proceed
        if (alice == null && bob == null && charlie == null) return;

        var registrations = new List<UserFoodRegistration>();

        if (alice != null)
        {
            registrations.AddRange(new[]
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
                }
            });
        }

        if (bob != null)
        {
            registrations.AddRange(new[]
            {
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
                }
            });
        }

        if (charlie != null)
        {
            registrations.AddRange(new[]
            {
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
        }

        if (registrations.Any())
        {
            dbContext.UserFoodRegistration.AddRange(registrations);
            dbContext.SaveChanges();
        }
    }

    private static void PopulateFoodComponent(HealthWellbeingDbContext dbContext)
    {
        if (dbContext.FoodComponent.Any()) return;

        dbContext.FoodComponent.AddRange(new List<FoodComponent>
        {
            new FoodComponent { Name = "Apple", Description = "Water, carbohydrates, fibers, vitamins, minerals, and antioxidants." },
            new FoodComponent { Name = "Rice", Description = "Primarily carbohydrates, small amounts of protein and minerals." },
            new FoodComponent { Name = "Salmon", Description = "Proteins, healthy fats (omega-3), vitamins (D, B12), minerals." },
            new FoodComponent { Name = "Broccoli", Description = "Fiber, vitamins (C, K), minerals (iron, potassium), phytonutrients." },
            new FoodComponent { Name = "Almonds", Description = "Healthy fats, protein, fiber, vitamin E, magnesium." },
            new FoodComponent { Name = "Eggs", Description = "High-quality protein, fats, vitamins (A, D, B12), minerals." },
            new FoodComponent { Name = "Oats", Description = "Carbohydrates (including soluble fiber beta-glucan), protein, minerals." },
            new FoodComponent { Name = "Tomato", Description = "Water, carbohydrates, vitamins (C, A), lycopene (antioxidant)."},
            new FoodComponent { Name = "Spinach", Description = "Water, iron, calcium, magnesium, vitamins (A, C, K)."},
            new FoodComponent { Name = "Banana", Description = "Carbohydrates, potassium, vitamin B6, fiber, natural sugars." }
        });

        dbContext.SaveChanges();
    }
}
