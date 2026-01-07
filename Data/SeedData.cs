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

        // =====================================================================
        // CLIENTS (≈ 30)
        // =====================================================================
        if (!db.Client.Any())
        {
            var clients = new List<Client>
    {
        new Client
        {
            Name      = "Alice Wonder",
            Email     = "alice@example.com",
            BirthDate = new DateTime(1992, 5, 14),
            Gender    = "Female"
        },
        new Client
        {
            Name      = "Bob Strong",
            Email     = "bob@example.com",
            BirthDate = new DateTime(1987, 2, 8),
            Gender    = "Male"
        },
        new Client
        {
            Name      = "Charlie Fit",
            Email     = "charlie@example.com",
            BirthDate = new DateTime(1998, 10, 20),
            Gender    = "Male"
        }
    };

            // Criar clientes de teste 4–30
            for (int i = 4; i <= 30; i++)
            {
                clients.Add(new Client
                {
                    Name = $"Test Client {i}",
                    Email = $"testclient{i}@example.com",
                    BirthDate = new DateTime(1990, 1, 1).AddDays(i * 20),
                    Gender = i % 2 == 0 ? "Male" : "Female"
                });
            }

            db.Client.AddRange(clients);
            db.SaveChanges();

            // =====================================================================
            // GOALS AUTOMÁTICOS
            // =====================================================================
            if (db.Goal != null && !db.Goal.Any())
            {
                var allclients = db.Client.OrderBy(c => c.ClientId).ToList();

                if (clients.Any())
                {

                    var rng = new Random();
                    double RandomWeight() => rng.Next(55, 95); // 55–95 kg

                    var goals = new List<Goal>();
                    int index = 0;

                    foreach (var client in clients)
                    {
                        double weight = RandomWeight();

                        string goalName =
                            index % 3 == 0 ? "Weight Loss"
                          : index % 3 == 1 ? "Muscle Gain"
                          : "Maintenance";

                        double activity =
                            goalName == "Weight Loss" ? 1.3 :
                            goalName == "Muscle Gain" ? 1.7 :
                            1.5;

                        double calories = weight * 22 * activity;
                        double protein = weight * 1.6;
                        double fat = calories * 0.27 / 9;
                        double proteinCal = protein * 4;
                        double hydrates = (calories - proteinCal - (fat * 9)) / 4;

                        goals.Add(new Goal
                        {
                            ClientId = client.ClientId,
                            GoalName = goalName,
                            DailyCalories = (int)calories,
                            DailyProtein = (int)protein,
                            DailyFat = (int)fat,
                            DailyHydrates = (int)hydrates,
                        });

                        index++;
                    }

                    db.Goal.AddRange(goals);
                    db.SaveChanges();
                }
            }
        }


            // =====================================================================
            // NUTRITIONISTS (≈ 30)
            // =====================================================================
            if (!db.Nutritionist.Any())
        {
            var nutritionists = new List<Nutritionist>
            {
                new Nutritionist
                {
                    Name   = "Dr. Joao Carvalho",
                    Email  = "joao.carvalho@healthwellbeing.com",
                    Gender = "Male"
                },
                new Nutritionist
                {
                    Name   = "Dr. Sofia Martins",
                    Email  = "sofia.martins@healthwellbeing.com",
                    Gender = "Female"
                },
                new Nutritionist
                {
                    Name   = "Dr. Ricardo Soares",
                    Email  = "ricardo.soares@healthwellbeing.com",
                    Gender = "Male"
                }
            };

            for (int i = 4; i <= 30; i++)
            {
                nutritionists.Add(new Nutritionist
                {
                    Name = $"Nutritionist {i}",
                    Email = $"nutritionist{i}@healthwellbeing.com",
                    Gender = i % 2 == 0 ? "Male" : "Female"
                });
            }

            db.Nutritionist.AddRange(nutritionists);
            db.SaveChanges();
        }

        // =====================================================================
        // ALERGIES (≈ 30)
        // =====================================================================
        if (db.Alergy != null && !db.Alergy.Any())
        {
            var alergies = new List<Alergy>
            {
                new Alergy { AlergyId = 0, AlergyName = "Peanuts" },
                new Alergy { AlergyId = 0, AlergyName = "Tree nuts" },
                new Alergy { AlergyId = 0, AlergyName = "Lactose" },
                new Alergy { AlergyId = 0, AlergyName = "Gluten" },
                new Alergy { AlergyId = 0, AlergyName = "Seafood" },
                new Alergy { AlergyId = 0, AlergyName = "Eggs" },
                new Alergy { AlergyId = 0, AlergyName = "Soy" },
                new Alergy { AlergyId = 0, AlergyName = "Sesame" },
                new Alergy { AlergyId = 0, AlergyName = "Strawberries" },
                new Alergy { AlergyId = 0, AlergyName = "Kiwi" }
            };

            for (int i = alergies.Count + 1; i <= 30; i++)
            {
                alergies.Add(new Alergy
                {
                    AlergyName = $"Test Allergy {i}"
                });
            }

            db.Alergy.AddRange(alergies);
            db.SaveChanges();
        }

        // =====================================================================
        // FOOD CATEGORIES (≈ 30, mas com base em categorias reais)
        // =====================================================================
        if (!db.FoodCategory.Any())
        {
            var categories = new List<FoodCategory>
            {
                new FoodCategory { Category = "Fruits",      Description = "Fresh fruits and berries" },
                new FoodCategory { Category = "Vegetables",  Description = "Fresh and cooked vegetables" },
                new FoodCategory { Category = "Grains",      Description = "Cereals, bread and pasta" },
                new FoodCategory { Category = "Proteins",    Description = "Meat, fish, eggs and legumes" },
                new FoodCategory { Category = "Dairy",       Description = "Milk and dairy products" },
                new FoodCategory { Category = "Fats & Oils", Description = "Healthy fats and oils" },
                new FoodCategory { Category = "Snacks",      Description = "Snack foods" },
                new FoodCategory { Category = "Drinks",      Description = "Non-alcoholic beverages" },
                new FoodCategory { Category = "Breakfast",   Description = "Breakfast foods" },
                new FoodCategory { Category = "Desserts",    Description = "Desserts and sweets" }
            };

            for (int i = categories.Count + 1; i <= 30; i++)
            {
                categories.Add(new FoodCategory
                {
                    Category = $"Category {i}",
                    Description = "Auto-generated test category"
                });
            }

            db.FoodCategory.AddRange(categories);
            db.SaveChanges();
        }

        // =====================================================================
        // FOODS (≈ 30)
        // =====================================================================
        if (!db.Food.Any())
        {
            var categories = db.FoodCategory
                .OrderBy(c => c.CategoryId)
                .ToList();

            if (categories.Any())
            {
                int fruitsId = categories.First(c => c.Category == "Fruits").CategoryId;
                int vegetablesId = categories.First(c => c.Category == "Vegetables").CategoryId;
                int grainsId = categories.First(c => c.Category == "Grains").CategoryId;
                int proteinsId = categories.First(c => c.Category == "Proteins").CategoryId;
                int dairyId = categories.First(c => c.Category == "Dairy").CategoryId;

                var foods = new List<Food>
                {
                    new Food { CategoryId = fruitsId,     Name = "Apple" },
                    new Food { CategoryId = fruitsId,     Name = "Banana" },
                    new Food { CategoryId = fruitsId,     Name = "Orange" },
                    new Food { CategoryId = fruitsId,     Name = "Strawberries" },
                    new Food { CategoryId = fruitsId,     Name = "Blueberries" },

                    new Food { CategoryId = vegetablesId, Name = "Broccoli" },
                    new Food { CategoryId = vegetablesId, Name = "Carrots" },
                    new Food { CategoryId = vegetablesId, Name = "Spinach" },
                    new Food { CategoryId = vegetablesId, Name = "Tomato" },
                    new Food { CategoryId = vegetablesId, Name = "Cucumber" },

                    new Food { CategoryId = grainsId,     Name = "White Rice" },
                    new Food { CategoryId = grainsId,     Name = "Brown Rice" },
                    new Food { CategoryId = grainsId,     Name = "Whole Wheat Bread" },
                    new Food { CategoryId = grainsId,     Name = "Oatmeal" },
                    new Food { CategoryId = grainsId,     Name = "Pasta" },

                    new Food { CategoryId = proteinsId,   Name = "Chicken Breast" },
                    new Food { CategoryId = proteinsId,   Name = "Salmon" },
                    new Food { CategoryId = proteinsId,   Name = "Tofu" },
                    new Food { CategoryId = proteinsId,   Name = "Eggs" },
                    new Food { CategoryId = proteinsId,   Name = "Lentils" },

                    new Food { CategoryId = dairyId,      Name = "Milk" },
                    new Food { CategoryId = dairyId,      Name = "Yogurt" },
                    new Food { CategoryId = dairyId,      Name = "Cheddar Cheese" },
                    new Food { CategoryId = dairyId,      Name = "Cottage Cheese" }
                };

                // completa até 30 com alimentos genéricos
                for (int i = foods.Count + 1; i <= 30; i++)
                {
                    var cat = categories[i % categories.Count];
                    foods.Add(new Food
                    {
                        CategoryId = cat.CategoryId,
                        Name = $"Test Food {i}"
                    });
                }

                db.Food.AddRange(foods);
                db.SaveChanges();
            }
        }

        // =====================================================================
        // PORTIONS
        // =====================================================================
        if (db.Portion != null && !db.Portion.Any())
        {
            var portions = new List<Portion>
            {
                new Portion { PortionName = "Small portion (50 g)" },
                new Portion { PortionName = "Medium portion (100 g)" },
                new Portion { PortionName = "Large portion (150 g)" },
                new Portion { PortionName = "Cup cooked" },
                new Portion { PortionName = "Cup raw" },
                new Portion { PortionName = "Slice(s)" },
                new Portion { PortionName = "Glass (200 ml)" },
                new Portion { PortionName = "Tablespoon" },
                new Portion { PortionName = "Teaspoon" }
            };

                    db.Portion.AddRange(portions);
                    db.SaveChanges();
                }


        // =====================================================================
        // NUTRITIONAL COMPONENTS
        // =====================================================================
        if (db.NutritionalComponent != null && !db.NutritionalComponent.Any())
        {
            var comps = new List<NutritionalComponent>
            {
                new NutritionalComponent { Name = "Energy",       Unit = "kcal", Basis = "per 100 g" },
                new NutritionalComponent { Name = "Protein",      Unit = "g",    Basis = "per 100 g" },
                new NutritionalComponent { Name = "Carbohydrate", Unit = "g",    Basis = "per 100 g" },
                new NutritionalComponent { Name = "Fat",          Unit = "g",    Basis = "per 100 g" },
                new NutritionalComponent { Name = "Fiber",        Unit = "g",    Basis = "per 100 g" },
                new NutritionalComponent { Name = "Sugar",        Unit = "g",    Basis = "per 100 g" },
                new NutritionalComponent { Name = "Sodium",       Unit = "mg",   Basis = "per 100 g" },
                new NutritionalComponent { Name = "Calcium",      Unit = "mg",   Basis = "per 100 g" },
                new NutritionalComponent { Name = "Vitamin C",    Unit = "mg",   Basis = "per 100 g" },
                new NutritionalComponent { Name = "Iron",         Unit = "mg",   Basis = "per 100 g" }
            };

            db.NutritionalComponent.AddRange(comps);
            db.SaveChanges();
        }

        // =====================================================================
        // PLANS (≈ 30)
        // =====================================================================
        if (db.FoodHabitsPlan != null && !db.FoodHabitsPlan.Any())
        {
            var plans = new List<FoodHabitsPlan>();
            DateTime today = DateTime.Today;
            var clients = db.Client.OrderBy(c => c.ClientId).ToList();

            for (int i = 0; i < 30; i++)
            {
                plans.Add(new FoodHabitsPlan
                {
                    ClientId = clients[i].ClientId,
                    StartingDate = today.AddDays(-i * 7),
                    EndingDate = today.AddDays(-i * 7 + 30),
                    Done = i % 3 == 0
                });
            }

            db.FoodHabitsPlan.AddRange(plans);
            db.SaveChanges();
        }

        // =====================================================================
        // JOIN TABLES – CLIENT ALERGIES
        // =====================================================================
        if (db.ClientAlergy != null && !db.ClientAlergy.Any())
        {
            var clients = db.Client.OrderBy(c => c.ClientId).ToList();
            var alergies = db.Alergy.OrderBy(a => a.AlergyId).ToList();

            if (clients.Any() && alergies.Any())
            {
                var list = new List<ClientAlergy>();

                int counter = 0;
                for (int i = 0; i < clients.Count; i++)
                {
                    for (int j = 0; j < alergies.Count; j++)
                    {
                        if ((i + j) % 4 == 0) // nem todos os clientes têm todas as alergias
                        {
                            list.Add(new ClientAlergy
                            {
                                ClientId = clients[i].ClientId,
                                AlergyId = alergies[j].AlergyId
                            });
                            counter++;
                            if (counter >= 40) break;
                        }
                    }
                    if (counter >= 40) break;
                }

                db.ClientAlergy.AddRange(list);
                db.SaveChanges();
            }
        }

        // =====================================================================
        // JOIN TABLES – NUTRITIONIST / CLIENT / PLAN
        // =====================================================================
        if (db.NutritionistClientPlan != null && !db.NutritionistClientPlan.Any())
        {
            var clients = db.Client.OrderBy(c => c.ClientId).ToList();
            var nutritionists = db.Nutritionist.OrderBy(n => n.NutritionistId).ToList();
            var plans = db.FoodHabitsPlan.OrderBy(p => p.FoodHabitsPlanId).ToList();

            if (clients.Any() && nutritionists.Any() && plans.Any())
            {
                var list = new List<NutritionistClientPlan>();
                int counter = 0;

                for (int i = 0; i < clients.Count; i++)
                {
                    for (int j = 0; j < nutritionists.Count; j++)
                    {
                        var plan = plans[(i + j) % plans.Count];

                        list.Add(new NutritionistClientPlan
                        {
                            ClientId = clients[i].ClientId,
                            NutritionistId = nutritionists[j].NutritionistId,
                            PlanId = plan.FoodHabitsPlanId
                        });

                        counter++;
                        if (counter >= 40) break;
                    }
                    if (counter >= 40) break;
                }

                db.NutritionistClientPlan.AddRange(list);
                db.SaveChanges();
            }
        }

        // =====================================================================
        // JOIN TABLES – FOOD PLANS  (PLAN + FOOD + PORTION)
        // =====================================================================
        if (!db.FoodPlan.Any())
        {
            var plans = db.FoodHabitsPlan.OrderBy(p => p.FoodHabitsPlanId).ToList();
            var foods = db.Food.OrderBy(f => f.FoodId).ToList();
            var portions = db.Portion.OrderBy(p => p.PortionId).ToList();

            if (plans.Any() && foods.Any() && portions.Any())
            {
                var defaultPortion = portions.First();
                var foodPlans = new List<FoodPlan>();

                void AddFoodsToPlan(FoodHabitsPlan plan, int startIndex, int count)
                {
                    for (int i = 0; i < count && (startIndex + i) < foods.Count; i++)
                    {
                        var food = foods[startIndex + i];

                        foodPlans.Add(new FoodPlan
                        {
                            PlanId = plan.FoodHabitsPlanId,
                            FoodId = food.FoodId,
                            PortionId = defaultPortion.PortionId
                        });
                    }
                }

                if (plans.Count >= 1) AddFoodsToPlan(plans[0], 0, 4);
                if (plans.Count >= 2) AddFoodsToPlan(plans[1], 4, 5);
                if (plans.Count >= 3) AddFoodsToPlan(plans[2], 9, 3);

                db.FoodPlan.AddRange(foodPlans);
                db.SaveChanges();
            }
        }

        

        // =====================================================================
        // FOOD PLAN DAY (PlanId + Date + FoodId + PortionsPlanned)
        // =====================================================================
        if (db.FoodPlanDay != null && !db.FoodPlanDay.Any())
        {
            var today = DateTime.Today;
            var plans = db.FoodHabitsPlan.OrderBy(p => p.FoodHabitsPlanId).ToList();
            var baseFoodPlans = db.FoodPlan
                .AsNoTracking()
                .OrderBy(fp => fp.PlanId)
                .ThenBy(fp => fp.FoodId)
                .ToList();

            if (plans.Any() && baseFoodPlans.Any())
            {
                var rng = new Random();
                var list = new List<FoodPlanDay>();

                foreach (var plan in plans)
                {
                    var foodsForPlan = baseFoodPlans.Where(fp => fp.PlanId == plan.FoodHabitsPlanId).ToList();
                    if (!foodsForPlan.Any()) continue;

                    // cria 7 dias de plano
                    for (int d = 0; d < 7; d++)
                    {
                        var date = today.AddDays(d).Date;

                        foreach (var fp in foodsForPlan)
                        {
                            list.Add(new FoodPlanDay
                            {
                                PlanId = plan.FoodHabitsPlanId,
                                FoodId = fp.FoodId,
                                PortionId = fp.PortionId,
                                Date = date,
                                PortionsPlanned = rng.Next(1, 4), // 1..3 porções
                                ScheduledTime = date.AddHours(9), // opcional
                                MealType = "Daily"
                            });
                        }
                    }
                }

                db.FoodPlanDay.AddRange(list);
                db.SaveChanges();
            }
        }

        // =====================================================================
        // FOOD INTAKE (pre-create from FoodPlanDay, not consumed)
        // =====================================================================
        if (db.FoodIntake != null && !db.FoodIntake.Any())
        {
            var days = db.FoodPlanDay.AsNoTracking().ToList();

            var list = days.Select(x => new FoodIntake
            {
                PlanId = x.PlanId,
                FoodId = x.FoodId,
                PortionId = x.PortionId,
                Date = x.Date,
                ScheduledTime = x.ScheduledTime ?? x.Date.AddHours(9),
                PortionsPlanned = x.PortionsPlanned,
                PortionsEaten = 0
            }).ToList();

            db.FoodIntake.AddRange(list);
            db.SaveChanges();
        }

        // =====================================================================
        // JOIN TABLES – FOOD / NUTRITIONAL COMPONENT
        // =====================================================================
        if (db.FoodNutritionalComponent != null && !db.FoodNutritionalComponent.Any())
        {
            var foods = db.Food.OrderBy(f => f.FoodId).ToList();
            var comps = db.NutritionalComponent.OrderBy(c => c.NutritionalComponentId).ToList();

            if (foods.Any() && comps.Any())
            {
                var list = new List<FoodNutritionalComponent>();
                int counter = 0;

                foreach (var food in foods)
                {
                    foreach (var comp in comps)
                    {
                        list.Add(new FoodNutritionalComponent
                        {
                            FoodId = food.FoodId,
                            NutritionalComponentId = comp.NutritionalComponentId,
                            Value = 5 + (counter % 20)
                        });

                        counter++;
                        if (counter >= 80) break;
                    }

                    if (counter >= 80) break;
                }

                db.FoodNutritionalComponent.AddRange(list);
                db.SaveChanges();
            }
        }
    }
}
