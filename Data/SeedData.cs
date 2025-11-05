using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Data;
using HealthWellbeing.Models;

internal static class SeedData
{
    internal static void Populate(HealthWellbeingDbContext? db)
    {
        if (db == null) throw new ArgumentNullException(nameof(db));
        db.Database.EnsureCreated();

        PopulateClients(db);
        PopulateNutritionists(db);
        PopulateFoodCategories(db);
        PopulateFoods(db);
        PopulateNutrientComponents(db);
        PopulateFoodNutrients(db);
        PopulateFoodPortions(db);
        PopulateGoals(db);
        PopulateFoodPlans(db);
        PopulateUserFoodRegistrations(db);
    }

    // ================================================================
    // CLIENTS
    // ================================================================
    private static void PopulateClients(HealthWellbeingDbContext db)
    {
        if (db.Client.Any()) return;

        db.Client.AddRange(
            new Client { ClientId = Guid.NewGuid().ToString("N"), Name = "Alice Wonder", Email = "alice@example.com", BirthDate = new DateTime(1992, 5, 14), Gender = "Female" },
            new Client { ClientId = Guid.NewGuid().ToString("N"), Name = "Bob Strong", Email = "bob@example.com", BirthDate = new DateTime(1987, 2, 8), Gender = "Male" },
            new Client { ClientId = Guid.NewGuid().ToString("N"), Name = "Charlie Fit", Email = "charlie@example.com", BirthDate = new DateTime(1998, 10, 20), Gender = "Male" }
        );

        db.SaveChanges();
    }

    // ================================================================
    // NUTRITIONISTS
    // ================================================================
    private static void PopulateNutritionists(HealthWellbeingDbContext db)
    {
        if (db.Nutritionist.Any()) return;

        db.Nutritionist.AddRange(
            new Nutritionist { Name = "Dr. Sara Healthy" },
            new Nutritionist { Name = "Dr. Miguel Fit" }
        );

        db.SaveChanges();
    }

    // ================================================================
    // FOOD CATEGORY
    // ================================================================
    private static void PopulateFoodCategories(HealthWellbeingDbContext db)
    {
        if (db.FoodCategory.Any()) return;

        var fruits = new FoodCategory { Name = "Fruits", Description = "Fresh fruits" };
        var grains = new FoodCategory { Name = "Grains", Description = "Cereals and grains" };
        var proteins = new FoodCategory { Name = "Proteins", Description = "Meat, fish, eggs" };
        var vegetables = new FoodCategory { Name = "Vegetables", Description = "Fresh vegetables" };
        var dairy = new FoodCategory { Name = "Dairy", Description = "Milk and dairy products" };

        db.FoodCategory.AddRange(fruits, grains, proteins, vegetables, dairy);
        db.SaveChanges();
    }

    // ================================================================
    // FOODS
    // ================================================================
    private static void PopulateFoods(HealthWellbeingDbContext db)
    {
        if (db.Food.Any()) return;

        var fruits = db.FoodCategory.First(c => c.Name == "Fruits");
        var grains = db.FoodCategory.First(c => c.Name == "Grains");
        var proteins = db.FoodCategory.First(c => c.Name == "Proteins");
        var vegetables = db.FoodCategory.First(c => c.Name == "Vegetables");
        var dairy = db.FoodCategory.First(c => c.Name == "Dairy");

        db.Food.AddRange(
            new Food { Name = "Apple", Description = "Fresh apple", FoodCategory = fruits },
            new Food { Name = "Banana", Description = "Yellow banana", FoodCategory = fruits },
            new Food { Name = "Rice (white, cooked)", Description = "Cooked white rice", FoodCategory = grains },
            new Food { Name = "Salmon", Description = "Fresh salmon fish", FoodCategory = proteins },
            new Food { Name = "Broccoli", Description = "Green broccoli florets", FoodCategory = vegetables },
            new Food { Name = "Milk (whole)", Description = "Whole cow milk", FoodCategory = dairy }
        );

        db.SaveChanges();
    }

    // ================================================================
    // NUTRIENT COMPONENTS
    // ================================================================
    private static void PopulateNutrientComponents(HealthWellbeingDbContext db)
    {
        if (db.NutrientComponent.Any()) return;

        db.NutrientComponent.AddRange(
            new NutrientComponent { Name = "Energy", DefaultUnit = "kcal", Description = "Energy content" },
            new NutrientComponent { Name = "Protein", DefaultUnit = "g", Description = "Protein amount" },
            new NutrientComponent { Name = "Carbohydrates", DefaultUnit = "g", Description = "Carbohydrates" },
            new NutrientComponent { Name = "Fat", DefaultUnit = "g", Description = "Fat content" },
            new NutrientComponent { Name = "Fiber", DefaultUnit = "g", Description = "Dietary fiber" },
            new NutrientComponent { Name = "Sugar", DefaultUnit = "g", Description = "Total sugars" }
        );

        db.SaveChanges();
    }

    // ================================================================
    // FOOD NUTRIENTS
    // ================================================================
    private static void PopulateFoodNutrients(HealthWellbeingDbContext db)
    {
        if (db.FoodNutrient.Any()) return;

        var comps = db.NutrientComponent.ToDictionary(c => c.Name, c => c.NutrientComponentId);
        int Get(string name) => comps[name];

        var apple = db.Food.First(f => f.Name == "Apple");
        var rice = db.Food.First(f => f.Name == "Rice (white, cooked)");
        var salmon = db.Food.First(f => f.Name == "Salmon");
        var milk = db.Food.First(f => f.Name == "Milk (whole)");

        db.FoodNutrient.AddRange(
            new FoodNutrient { Food = apple, NutrientComponentId = Get("Energy"), Value = 52m, Unit = "kcal", Basis = "per100g" },
            new FoodNutrient { Food = apple, NutrientComponentId = Get("Carbohydrates"), Value = 14m, Unit = "g", Basis = "per100g" },
            new FoodNutrient { Food = rice, NutrientComponentId = Get("Energy"), Value = 130m, Unit = "kcal", Basis = "per100g" },
            new FoodNutrient { Food = rice, NutrientComponentId = Get("Carbohydrates"), Value = 28m, Unit = "g", Basis = "per100g" },
            new FoodNutrient { Food = salmon, NutrientComponentId = Get("Energy"), Value = 208m, Unit = "kcal", Basis = "per100g" },
            new FoodNutrient { Food = salmon, NutrientComponentId = Get("Protein"), Value = 20.4m, Unit = "g", Basis = "per100g" },
            new FoodNutrient { Food = milk, NutrientComponentId = Get("Energy"), Value = 60m, Unit = "kcal", Basis = "per100ml" },
            new FoodNutrient { Food = milk, NutrientComponentId = Get("Protein"), Value = 3.2m, Unit = "g", Basis = "per100ml" }
        );

        db.SaveChanges();
    }

    // ================================================================
    // FOOD PORTIONS
    // ================================================================
    private static void PopulateFoodPortions(HealthWellbeingDbContext db)
    {
        if (db.FoodPortion.Any()) return;

        var apple = db.Food.First(f => f.Name == "Apple");
        var rice = db.Food.First(f => f.Name == "Rice (white, cooked)");
        var milk = db.Food.First(f => f.Name == "Milk (whole)");

        db.FoodPortion.AddRange(
            new FoodPortion { Food = apple, Label = "1 medium (150 g)", AmountGramsMl = 150 },
            new FoodPortion { Food = rice, Label = "1 cup cooked (150 g)", AmountGramsMl = 150 },
            new FoodPortion { Food = milk, Label = "1 glass (200 ml)", AmountGramsMl = 200 }
        );

        db.SaveChanges();
    }

    // ================================================================
    // GOALS
    // ================================================================
    private static void PopulateGoals(HealthWellbeingDbContext db)
    {
        if (db.Goal.Any()) return;

        var alice = db.Client.First(c => c.Name == "Alice Wonder");
        var bob = db.Client.First(c => c.Name == "Bob Strong");

        db.Goal.AddRange(
            new Goal
            {
                ClientId = alice.ClientId,
                GoalType = "Lose Weight",
                DailyCalories = 1600,
                DailyProtein = 90,
                DailyCarbs = 150,
                DailyFat = 50
            },
            new Goal
            {
                ClientId = bob.ClientId,
                GoalType = "Muscle Gain",
                DailyCalories = 2500,
                DailyProtein = 150,
                DailyCarbs = 250,
                DailyFat = 70
            }
        );

        db.SaveChanges();
    }

    // ================================================================
    // FOOD PLANS
    // ================================================================
    private static void PopulateFoodPlans(HealthWellbeingDbContext db)
    {
        if (db.FoodPlan.Any()) return;

        var goal1 = db.Goal.First(g => g.GoalType == "Lose Weight");
        var goal2 = db.Goal.First(g => g.GoalType == "Muscle Gain");
        var alice = db.Client.First(c => c.Name == "Alice Wonder");
        var bob = db.Client.First(c => c.Name == "Bob Strong");
        var rice = db.Food.First(f => f.Name == "Rice (white, cooked)");
        var salmon = db.Food.First(f => f.Name == "Salmon");
        var nutri = db.Nutritionist.First();

        db.FoodPlan.AddRange(
            new FoodPlan
            {
                ClientId = alice.ClientId,
                GoalId = goal1.GoalId,
                FoodId = rice.FoodId,
                Quantity = 150,
                Description = "Lunch portion of rice",
                NutritionistId = nutri.NutritionistId
            },
            new FoodPlan
            {
                ClientId = bob.ClientId,
                GoalId = goal2.GoalId,
                FoodId = salmon.FoodId,
                Quantity = 200,
                Description = "Dinner portion of salmon",
                NutritionistId = nutri.NutritionistId
            }
        );

        db.SaveChanges();
    }

    // ================================================================
    // USER FOOD REGISTRATIONS
    // ================================================================
    private static void PopulateUserFoodRegistrations(HealthWellbeingDbContext db)
    {
        if (db.UserFoodRegistration.Any()) return;

        var alice = db.Client.First(c => c.Name == "Alice Wonder");
        var apple = db.Food.First(f => f.Name == "Apple");
        var applePortion = db.FoodPortion.First(p => p.FoodId == apple.FoodId);
        var energy = db.FoodNutrient
            .Include(n => n.NutrientComponent)
            .First(n => n.FoodId == apple.FoodId && n.NutrientComponent!.Name == "Energy");

        var kcal = (energy.Value / 100m) * applePortion.AmountGramsMl;

        db.UserFoodRegistration.Add(new UserFoodRegistration
        {
            ClientId = alice.ClientId,
            FoodId = apple.FoodId,
            FoodPortionId = applePortion.FoodPortionId,
            PortionsCount = 1,
            MealType = "Breakfast",
            MealDateTime = DateTime.Now.AddHours(-2),
            EstimatedEnergyKcal = kcal,
            Notes = "Ate an apple before class"
        });

        db.SaveChanges();
    }
}
