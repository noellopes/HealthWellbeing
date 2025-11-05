using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

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
        PopulateNutrientComponents(dbContext);
        PopulateFoodNutrients(dbContext);
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
            new FoodCategory { Name = "Fruits",            Description = "Fresh fruits and fruit-based items." },
            new FoodCategory { Name = "Grains & Cereals", Description = "Rice, oats, bread and other grain based foods." },
            new FoodCategory { Name = "Proteins",         Description = "Meats, fish, eggs and other protein rich foods." },
            new FoodCategory { Name = "Vegetables",       Description = "Fresh vegetables and vegetable-based items." },
            new FoodCategory { Name = "Nuts & Seeds",     Description = "Nuts, seeds and related products." },
            new FoodCategory { Name = "Dairy & Eggs",     Description = "Milk, cheese, yogurt and eggs." },
            new FoodCategory { Name = "Other",            Description = "Miscellaneous foods and ingredients." }
        };

        dbContext.FoodCategory.AddRange(categories);
        dbContext.SaveChanges();

        _categoryCache = categories;
    }

    private static void PopulateFood(HealthWellbeingDbContext dbContext)
    {
        if (dbContext.Food.Any()) return;

        if (_categoryCache == null || !_categoryCache.Any())
        {
            _categoryCache = dbContext.FoodCategory.ToList();
            if (!_categoryCache.Any()) return;
        }

        var fruits = _categoryCache.FirstOrDefault(c => c.Name == "Fruits");
        var grains = _categoryCache.FirstOrDefault(c => c.Name == "Grains & Cereals");
        var proteins = _categoryCache.FirstOrDefault(c => c.Name == "Proteins");
        var vegetables = _categoryCache.FirstOrDefault(c => c.Name == "Vegetables");
        var nuts = _categoryCache.FirstOrDefault(c => c.Name == "Nuts & Seeds");
        var dairy = _categoryCache.FirstOrDefault(c => c.Name == "Dairy & Eggs");
        var other = _categoryCache.FirstOrDefault(c => c.Name == "Other");

        var foods = new List<Food>
    {
        new Food { Name = "Apple",                 Description = "Fresh apple, sweet-tart flavor.",                    FoodCategory = fruits },
        new Food { Name = "Banana",                Description = "Ripe banana, good source of potassium.",            FoodCategory = fruits },
        new Food { Name = "Rice (white, cooked)",  Description = "Cooked white rice.",                                FoodCategory = grains },
        new Food { Name = "Salmon (raw)",          Description = "Raw salmon rich in omega-3 fatty acids.",           FoodCategory = proteins },
        new Food { Name = "Broccoli",              Description = "Fresh broccoli florets.",                           FoodCategory = vegetables },
        new Food { Name = "Almonds",               Description = "Raw almonds, high in healthy fats and protein.",   FoodCategory = nuts },
        new Food { Name = "Egg (whole)",           Description = "Whole chicken egg, boiled or cooked.",              FoodCategory = dairy },
        new Food { Name = "Oats (dry)",            Description = "Rolled oats dry weight.",                           FoodCategory = grains },
        new Food { Name = "Tomato",                Description = "Fresh tomato, commonly used in salads and sauces.", FoodCategory = vegetables },
        new Food { Name = "Spinach",               Description = "Fresh spinach leaves, rich in iron and vitamins.",  FoodCategory = vegetables },
        new Food { Name = "Mixed Dish",            Description = "Used for general mixed meals or unknown foods.",    FoodCategory = other }
    };

        dbContext.Food.AddRange(foods);
        dbContext.SaveChanges();
    }

    private static void PopulateNutrientComponents(HealthWellbeingDbContext dbContext)
    {
        if (dbContext.NutrientComponent.Any()) return;

        var components = new List<NutrientComponent>
        {
            new NutrientComponent { Name = "Energy",  DefaultUnit = "kcal", Code = "ENER", Description = "Energy (kcal)" },
            new NutrientComponent { Name = "Protein", DefaultUnit = "g",    Code = "PROT" },
            new NutrientComponent { Name = "Carbs",   DefaultUnit = "g",    Code = "CARB" },
            new NutrientComponent { Name = "Fat",     DefaultUnit = "g",    Code = "FAT"  },
            new NutrientComponent { Name = "Fiber",   DefaultUnit = "g",    Code = "FIBR" },
            new NutrientComponent { Name = "Sugar",   DefaultUnit = "g",    Code = "SUGR" },
            new NutrientComponent { Name = "Sodium",  DefaultUnit = "mg",   Code = "NA"   }
        };

        dbContext.NutrientComponent.AddRange(components);
        dbContext.SaveChanges();
    }

    private static void PopulateFoodNutrients(HealthWellbeingDbContext dbContext)
    {
        if (dbContext.FoodNutrient.Any()) return;

        // Mapear componentes por nome para facilitar
        var comp = dbContext.NutrientComponent.AsNoTracking().ToDictionary(n => n.Name, n => n.NutrientComponentId);

        int GetCompId(string name) =>
            comp.TryGetValue(name, out var id) ? id
            : dbContext.NutrientComponent.First(n => n.Name == name).NutrientComponentId;

        void AddFN(Food food, string componentName, decimal value, string unit, string basis = "per100g")
        {
            dbContext.FoodNutrient.Add(new FoodNutrient
            {
                FoodId = food.FoodId,
                NutrientComponentId = GetCompId(componentName),
                Value = value,
                Unit = unit,
                Basis = basis
            });
        }

        var apple = dbContext.Food.FirstOrDefault(f => f.Name == "Apple");
        var banana = dbContext.Food.FirstOrDefault(f => f.Name == "Banana");
        var rice = dbContext.Food.FirstOrDefault(f => f.Name == "Rice (white, cooked)");
        var salmon = dbContext.Food.FirstOrDefault(f => f.Name == "Salmon (raw)");

        if (apple != null)
        {
            AddFN(apple, "Energy", 52m, "kcal");
            AddFN(apple, "Carbs", 14.0m, "g");
            AddFN(apple, "Protein", 0.26m, "g");
            AddFN(apple, "Fat", 0.17m, "g");
            AddFN(apple, "Fiber", 2.4m, "g");
        }

        if (banana != null)
        {
            AddFN(banana, "Energy", 89m, "kcal");
            AddFN(banana, "Carbs", 23.0m, "g");
            AddFN(banana, "Protein", 1.09m, "g");
            AddFN(banana, "Fat", 0.33m, "g");
            AddFN(banana, "Fiber", 2.6m, "g");
        }

        if (rice != null)
        {
            AddFN(rice, "Energy", 130m, "kcal");
            AddFN(rice, "Carbs", 28.0m, "g");
            AddFN(rice, "Protein", 2.69m, "g");
            AddFN(rice, "Fat", 0.28m, "g");
        }

        if (salmon != null)
        {
            AddFN(salmon, "Energy", 208m, "kcal");
            AddFN(salmon, "Protein", 20.42m, "g");
            AddFN(salmon, "Fat", 13.42m, "g");
            AddFN(salmon, "Carbs", 0m, "g");
        }

        dbContext.SaveChanges();
    }

    private static void PopulateUserFoodRegistrations(HealthWellbeingDbContext dbContext)
    {
        if (dbContext.UserFoodRegistration.Any()) return;

        if (_clientCache == null || !_clientCache.Any())
        {
            _clientCache = dbContext.Client.ToList();
            if (!_clientCache.Any()) return; // nothing to reference
        }

        var alice = _clientCache.FirstOrDefault(c => c.Name.Contains("Alice", StringComparison.OrdinalIgnoreCase));
        var bob = _client_cache_or_null() ?? _clientCache.FirstOrDefault(c => c.Name.Contains("Bob", StringComparison.OrdinalIgnoreCase));
        var charlie = _clientCache.FirstOrDefault(c => c.Name.Contains("Charlie", StringComparison.OrdinalIgnoreCase));

        static Client? _client_cache_or_null() => null;

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
            new FoodComponent { Name = "Apple",   Description = "Water, carbohydrates, fibers, vitamins, minerals, and antioxidants." },
            new FoodComponent { Name = "Rice",    Description = "Primarily carbohydrates, small amounts of protein and minerals." },
            new FoodComponent { Name = "Salmon",  Description = "Proteins, healthy fats (omega-3), vitamins (D, B12), minerals." },
            new FoodComponent { Name = "Broccoli",Description = "Fiber, vitamins (C, K), minerals (iron, potassium), phytonutrients." },
            new FoodComponent { Name = "Almonds", Description = "Healthy fats, protein, fiber, vitamin E, magnesium." },
            new FoodComponent { Name = "Eggs",    Description = "High-quality protein, fats, vitamins (A, D, B12), minerals." },
            new FoodComponent { Name = "Oats",    Description = "Carbohydrates (including soluble fiber beta-glucan), protein, minerals." },
            new FoodComponent { Name = "Tomato",  Description = "Water, carbohydrates, vitamins (C, A), lycopene (antioxidant)."},
            new FoodComponent { Name = "Spinach", Description = "Water, iron, calcium, magnesium, vitamins (A, C, K)."},
            new FoodComponent { Name = "Banana",  Description = "Carbohydrates, potassium, vitamin B6, fiber, natural sugars." }
        });

        dbContext.SaveChanges();
    }
}