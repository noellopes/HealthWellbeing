using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Models;

namespace HealthWellbeing.Data
{
    public static class SeedData
    {
        public static void Populate(HealthWellbeingDbContext db)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));

            // Se houver migrations, aplica; caso contrário, cria BD
            try { db.Database.Migrate(); }
            catch { db.Database.EnsureCreated(); }

            PopulateSpecialities(db);
            PopulateDoctors(db);

            PopulateClients(db);
            PopulateConsultas(db);
            PopulateGoals(db);

            PopulateUtenteSaude(db);
            PopulateNutritionists(db);

            PopulateAlergies(db);

            PopulateFoodCategories(db);
            PopulateFoods(db);
            PopulatePortions(db);

            PopulateNutritionalComponents(db);

            PopulatePlans(db);

            PopulateClientAlergies(db);
            PopulateNutritionistClientPlans(db);

            PopulateFoodPlans(db);
            PopulateFoodPlanDays(db);

            PopulateFoodIntakeFromDays(db);
            PopulateFoodNutritionalComponents(db);
        }

        // ---------------------------------------------------------------------
        // Helpers (para setar FKs opcionais sem rebentar caso a propriedade não exista)
        // ---------------------------------------------------------------------
        private static void SetIfExists(object entity, string propertyName, object? value)
        {
            var prop = entity.GetType().GetProperty(propertyName);
            if (prop == null || !prop.CanWrite) return;

            if (value == null)
            {
                if (!prop.PropertyType.IsValueType || Nullable.GetUnderlyingType(prop.PropertyType) != null)
                    prop.SetValue(entity, null);
                return;
            }

            var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

            try
            {
                var converted = Convert.ChangeType(value, targetType);
                prop.SetValue(entity, converted);
            }
            catch
            {
                if (targetType.IsAssignableFrom(value.GetType()))
                    prop.SetValue(entity, value);
            }
        }

        // ---------------------------------------------------------------------
        // SPECIALITIES
        // ---------------------------------------------------------------------
        private static void PopulateSpecialities(HealthWellbeingDbContext db)
        {
            if (db.Specialities.Any()) return;

            var especialidades = new[]
            {
                new Specialities { Nome = "Cardiologia", Descricao = "Avaliação, diagnóstico e tratamento de doenças do coração e sistema cardiovascular." },
                new Specialities { Nome = "Dermatologia", Descricao = "Prevenção, diagnóstico e tratamento de doenças da pele, cabelo e unhas." },
                new Specialities { Nome = "Pediatria", Descricao = "Cuidados de saúde para bebés, crianças e adolescentes." },
                new Specialities { Nome = "Psiquiatria", Descricao = "Avaliação e tratamento de perturbações mentais, emocionais e comportamentais." },
                new Specialities { Nome = "Nutrição", Descricao = "Aconselhamento alimentar e planos de nutrição para promoção da saúde e bem-estar." },
                new Specialities { Nome = "Medicina Geral e Familiar", Descricao = "Acompanhamento global e contínuo da saúde de utentes e famílias." },
                new Specialities { Nome = "Ortopedia", Descricao = "Tratamento de doenças e lesões dos ossos, articulações, músculos e tendões." },
                new Specialities { Nome = "Ginecologia e Obstetrícia", Descricao = "Saúde da mulher, sistema reprodutor e acompanhamento da gravidez e parto." },
                new Specialities { Nome = "Psicologia", Descricao = "Apoio psicológico, gestão emocional e acompanhamento em saúde mental." },
                new Specialities { Nome = "Fisioterapia", Descricao = "Reabilitação motora e funcional após lesões, cirurgias ou doenças crónicas." },
            };

            db.Specialities.AddRange(especialidades);
            db.SaveChanges();
        }

        // ---------------------------------------------------------------------
        // DOCTORS
        // ---------------------------------------------------------------------
        private static void PopulateDoctors(HealthWellbeingDbContext db)
        {
            if (db.Doctor.Any()) return;

            var doctors = new[]
            {
                new Doctor { Nome = "Ana Martins",      Telemovel = "912345678", Email = "ana.martins@healthwellbeing.pt" },
                new Doctor { Nome = "Bruno Carvalho",   Telemovel = "913456789", Email = "bruno.carvalho@healthwellbeing.pt" },
                new Doctor { Nome = "Carla Ferreira",   Telemovel = "914567890", Email = "carla.ferreira@healthwellbeing.pt" },
                new Doctor { Nome = "Daniel Sousa",     Telemovel = "915678901", Email = "daniel.sousa@healthwellbeing.pt" },
                new Doctor { Nome = "Eduarda Almeida",  Telemovel = "916789012", Email = "eduarda.almeida@healthwellbeing.pt" },
                new Doctor { Nome = "Fábio Pereira",    Telemovel = "917890123", Email = "fabio.pereira@healthwellbeing.pt" },
                new Doctor { Nome = "Gabriela Rocha",   Telemovel = "918901234", Email = "gabriela.rocha@healthwellbeing.pt" },
                new Doctor { Nome = "Hugo Santos",      Telemovel = "919012345", Email = "hugo.santos@healthwellbeing.pt" },
                new Doctor { Nome = "Inês Correia",     Telemovel = "920123456", Email = "ines.correia@healthwellbeing.pt" },
                new Doctor { Nome = "João Ribeiro",     Telemovel = "921234567", Email = "joao.ribeiro@healthwellbeing.pt" },
                new Doctor { Nome = "Luísa Nogueira",   Telemovel = "922345678", Email = "luisa.nogueira@healthwellbeing.pt" },
                new Doctor { Nome = "Miguel Costa",     Telemovel = "923456789", Email = "miguel.costa@healthwellbeing.pt" },
                new Doctor { Nome = "Nádia Gonçalves",  Telemovel = "924567890", Email = "nadia.goncalves@healthwellbeing.pt" },
                new Doctor { Nome = "Óscar Figueiredo", Telemovel = "925678901", Email = "oscar.figueiredo@healthwellbeing.pt" },
                new Doctor { Nome = "Patrícia Lopes",   Telemovel = "926789012", Email = "patricia.lopes@healthwellbeing.pt" },
            };

            db.Doctor.AddRange(doctors);
            db.SaveChanges();
        }

        // ---------------------------------------------------------------------
        // CLIENTS
        // ---------------------------------------------------------------------
        private static void PopulateClients(HealthWellbeingDbContext db)
        {
            if (db.Client.Any()) return;

            var clients = new List<Client>
            {
                new Client { Name = "Alice Wonder",  Email = "alice@example.com",  BirthDate = new DateTime(1992, 5, 14),  Gender = "Female" },
                new Client { Name = "Bob Strong",    Email = "bob@example.com",    BirthDate = new DateTime(1987, 2, 8),   Gender = "Male"   },
                new Client { Name = "Charlie Fit",   Email = "charlie@example.com",BirthDate = new DateTime(1998, 10, 20), Gender = "Male"   },
            };

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
        }

        // ---------------------------------------------------------------------
        // CONSULTAS (sem assumir FKs obrigatórias; se existirem, tenta atribuir por reflexão)
        // ---------------------------------------------------------------------
        private static void PopulateConsultas(HealthWellbeingDbContext db)
        {
            if (db.Consulta.Any()) return;

            var hoje = DateTime.Today;

            var consultas = new List<Consulta>
            {
                // Exemplo base
                new Consulta
                {
                    DataMarcacao = new DateTime(2024, 4, 21, 10, 30, 0, DateTimeKind.Unspecified),
                    DataConsulta = new DateTime(2025, 4, 21, 10, 30, 0, DateTimeKind.Unspecified),
                    HoraInicio = new TimeOnly(10, 30),
                    HoraFim = new TimeOnly(11, 30),
                },

                // Futura (Agendada)
                new Consulta
                {
                    DataMarcacao = new DateTime(2025, 10, 10, 9, 15, 0, DateTimeKind.Unspecified),
                    DataConsulta = new DateTime(2025, 11, 5, 9, 0, 0, DateTimeKind.Unspecified),
                    HoraInicio = new TimeOnly(9, 0),
                    HoraFim = new TimeOnly(9, 30),
                },

                // Hoje
                new Consulta
                {
                    DataMarcacao = hoje.AddDays(-2).AddHours(10),
                    DataConsulta = new DateTime(hoje.Year, hoje.Month, hoje.Day, 9, 30, 0, DateTimeKind.Unspecified),
                    HoraInicio = new TimeOnly(9, 30),
                    HoraFim = new TimeOnly(10, 0),
                },

                // Expirada
                new Consulta
                {
                    DataMarcacao = new DateTime(2025, 8, 20, 11, 25, 0, DateTimeKind.Unspecified),
                    DataConsulta = new DateTime(2025, 9, 25, 11, 45, 0, DateTimeKind.Unspecified),
                    HoraInicio = new TimeOnly(11, 45),
                    HoraFim = new TimeOnly(12, 15),
                },

                // Cancelada
                new Consulta
                {
                    DataMarcacao = new DateTime(2025, 9, 15, 11, 30, 0, DateTimeKind.Unspecified),
                    DataConsulta = new DateTime(2025, 10, 10, 15, 0, 0, DateTimeKind.Unspecified),
                    DataCancelamento = new DateTime(2025, 10, 8, 10, 0, 0, DateTimeKind.Unspecified),
                    HoraInicio = new TimeOnly(15, 0),
                    HoraFim = new TimeOnly(15, 45),
                },
            };

            // Se existirem FKs (ClientId/DoctorId/SpecialitiesId), tenta preencher
            var clients = db.Client.OrderBy(c => c.ClientId).ToList();
            var doctors = db.Doctor.ToList();
            var specialities = db.Specialities.ToList();

            for (int i = 0; i < consultas.Count; i++)
            {
                if (clients.Count > 0) SetIfExists(consultas[i], "ClientId", clients[i % clients.Count].ClientId);
                if (doctors.Count > 0) SetIfExists(consultas[i], "DoctorId", GetIntProp(doctors[i % doctors.Count], "DoctorId", "Id"));
                if (specialities.Count > 0) SetIfExists(consultas[i], "SpecialitiesId", GetIntProp(specialities[i % specialities.Count], "SpecialitiesId", "Id"));
            }

            db.Consulta.AddRange(consultas);
            db.SaveChanges();
        }

        private static int? GetIntProp(object entity, params string[] names)
        {
            foreach (var name in names)
            {
                var p = entity.GetType().GetProperty(name);
                if (p == null) continue;

                if (p.PropertyType == typeof(int)) return (int)p.GetValue(entity)!;
                if (p.PropertyType == typeof(int?)) return (int?)p.GetValue(entity);
            }
            return null;
        }

        // ---------------------------------------------------------------------
        // GOALS (automáticos por cliente)
        // ---------------------------------------------------------------------
        private static void PopulateGoals(HealthWellbeingDbContext db)
        {
            if (db.Goal.Any()) return;

            var clients = db.Client.OrderBy(c => c.ClientId).ToList();
            if (!clients.Any()) return;

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
                    DailyHydrates = (int)hydrates
                });

                index++;
            }

            db.Goal.AddRange(goals);
            db.SaveChanges();
        }

        // ---------------------------------------------------------------------
        // UTENTE SAÚDE
        // ---------------------------------------------------------------------
        private static void PopulateUtenteSaude(HealthWellbeingDbContext db)
        {
            if (db.UtenteSaude.Any()) return;

            var clients = db.Client.OrderBy(c => c.ClientId).Take(30).ToList();
            if (!clients.Any()) return;

            var utentes = new List<UtenteSaude>();

            for (int i = 0; i < clients.Count; i++)
            {
                utentes.Add(new UtenteSaude
                {
                    ClientId = clients[i].ClientId,
                    Nif = GenerateValidNif(i),
                    Niss = (11000000000L + i).ToString(),     // 11 dígitos
                    Nus = (100000000 + i).ToString()         // 9 dígitos
                });
            }

            db.UtenteSaude.AddRange(utentes);
            db.SaveChanges();
        }

        private static string GenerateValidNif(int i)
        {
            // 1º dígito válido: 1/2/3/5/6/8/9 -> vamos usar "2"
            string first8 = "2" + (1000000 + i).ToString("D7"); // 8 dígitos

            int sum = 0;
            for (int k = 0; k < 8; k++)
            {
                int digit = first8[k] - '0';
                int weight = 9 - k;
                sum += digit * weight;
            }

            int remainder = sum % 11;
            int check = remainder < 2 ? 0 : 11 - remainder;

            return first8 + check.ToString();
        }


        // ---------------------------------------------------------------------
        // NUTRITIONISTS
        // ---------------------------------------------------------------------
        private static void PopulateNutritionists(HealthWellbeingDbContext db)
        {
            if (db.Nutritionist.Any()) return;

            var nutritionists = new List<Nutritionist>
    {
        new Nutritionist
        {
            Name = "Daniel Rocha",
            Gender = "Male",
            Email = "daniel.rocha@healthwellbeing.pt"
        },
        new Nutritionist
        {
            Name = "Eduarda Nogueira",
            Gender = "Female",
            Email = "eduarda.nogueira@healthwellbeing.pt"
        },
        new Nutritionist
        {
            Name = "Fábio Gonçalves",
            Gender = "Male",
            Email = "fabio.goncalves@healthwellbeing.pt"
        }
    };

            for (int i = 4; i <= 30; i++)
            {
                nutritionists.Add(new Nutritionist
                {
                    Name = $"Nutritionist {i}",
                    Gender = i % 2 == 0 ? "Male" : "Female",
                    Email = $"nutritionist{i}@healthwellbeing.pt"
                });
            }

            db.Nutritionist.AddRange(nutritionists);
            db.SaveChanges();
        }


        // ---------------------------------------------------------------------
        // ALERGIES
        // ---------------------------------------------------------------------
        private static void PopulateAlergies(HealthWellbeingDbContext db)
        {
            if (db.Alergy.Any()) return;

            var alergies = new List<Alergy>
            {
                new Alergy { AlergyName = "Peanuts" },
                new Alergy { AlergyName = "Tree nuts" },
                new Alergy { AlergyName = "Lactose" },
                new Alergy { AlergyName = "Gluten" },
                new Alergy { AlergyName = "Seafood" },
                new Alergy { AlergyName = "Eggs" },
                new Alergy { AlergyName = "Soy" },
                new Alergy { AlergyName = "Sesame" },
                new Alergy { AlergyName = "Strawberries" },
                new Alergy { AlergyName = "Kiwi" }
            };

            for (int i = alergies.Count + 1; i <= 30; i++)
                alergies.Add(new Alergy { AlergyName = $"Test Allergy {i}" });

            db.Alergy.AddRange(alergies);
            db.SaveChanges();
        }

        // ---------------------------------------------------------------------
        // FOOD CATEGORIES
        // ---------------------------------------------------------------------
        private static void PopulateFoodCategories(HealthWellbeingDbContext db)
        {
            if (db.FoodCategory.Any()) return;

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
                categories.Add(new FoodCategory { Category = $"Category {i}", Description = "Auto-generated test category" });

            db.FoodCategory.AddRange(categories);
            db.SaveChanges();
        }

        // ---------------------------------------------------------------------
        // FOODS
        // ---------------------------------------------------------------------
        private static void PopulateFoods(HealthWellbeingDbContext db)
        {
            if (db.Food.Any()) return;

            var categories = db.FoodCategory.OrderBy(c => c.CategoryId).ToList();
            if (!categories.Any()) return;

            int GetCatId(string name) => categories.First(c => c.Category == name).CategoryId;

            int fruitsId = GetCatId("Fruits");
            int vegetablesId = GetCatId("Vegetables");
            int grainsId = GetCatId("Grains");
            int proteinsId = GetCatId("Proteins");
            int dairyId = GetCatId("Dairy");

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

            for (int i = foods.Count + 1; i <= 30; i++)
            {
                var cat = categories[i % categories.Count];
                foods.Add(new Food { CategoryId = cat.CategoryId, Name = $"Test Food {i}" });
            }

            db.Food.AddRange(foods);
            db.SaveChanges();
        }

        // ---------------------------------------------------------------------
        // PORTIONS
        // ---------------------------------------------------------------------
        private static void PopulatePortions(HealthWellbeingDbContext db)
        {
            if (db.Portion.Any()) return;

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

        // ---------------------------------------------------------------------
        // NUTRITIONAL COMPONENTS
        // ---------------------------------------------------------------------
        private static void PopulateNutritionalComponents(HealthWellbeingDbContext db)
        {
            if (db.NutritionalComponent.Any()) return;

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

        // ---------------------------------------------------------------------
        // PLANS
        // ---------------------------------------------------------------------
        private static void PopulatePlans(HealthWellbeingDbContext db)
        {
            if (db.Plan.Any()) return;

            var clients = db.Client.OrderBy(c => c.ClientId).ToList();
            if (!clients.Any()) return;

            var plans = new List<Plan>();
            DateTime today = DateTime.Today;

            for (int i = 0; i < 30 && i < clients.Count; i++)
            {
                plans.Add(new Plan
                {
                    ClientId = clients[i].ClientId,
                    StartingDate = today.AddDays(-i * 7),
                    EndingDate = today.AddDays(-i * 7 + 30),
                    Done = i % 3 == 0
                });
            }

            db.Plan.AddRange(plans);
            db.SaveChanges();
        }

        // ---------------------------------------------------------------------
        // JOIN: CLIENT ALERGY
        // ---------------------------------------------------------------------
        private static void PopulateClientAlergies(HealthWellbeingDbContext db)
        {
            if (db.ClientAlergy.Any()) return;

            var clients = db.Client.OrderBy(c => c.ClientId).ToList();
            var alergies = db.Alergy.OrderBy(a => a.AlergyId).ToList();
            if (!clients.Any() || !alergies.Any()) return;

            var list = new List<ClientAlergy>();
            int counter = 0;

            for (int i = 0; i < clients.Count; i++)
            {
                for (int j = 0; j < alergies.Count; j++)
                {
                    if ((i + j) % 4 != 0) continue;

                    list.Add(new ClientAlergy
                    {
                        ClientId = clients[i].ClientId,
                        AlergyId = alergies[j].AlergyId
                    });

                    counter++;
                    if (counter >= 40) break;
                }
                if (counter >= 40) break;
            }

            db.ClientAlergy.AddRange(list);
            db.SaveChanges();
        }

        // ---------------------------------------------------------------------
        // JOIN: NUTRITIONIST / CLIENT / PLAN
        // ---------------------------------------------------------------------
        private static void PopulateNutritionistClientPlans(HealthWellbeingDbContext db)
        {
            if (db.NutritionistClientPlan.Any()) return;

            var clients = db.Client.OrderBy(c => c.ClientId).ToList();
            var nutritionists = db.Nutritionist.OrderBy(n => n.NutritionistId).ToList();
            var plans = db.Plan.OrderBy(p => p.PlanId).ToList();

            if (!clients.Any() || !nutritionists.Any() || !plans.Any()) return;

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
                        PlanId = plan.PlanId
                    });

                    counter++;
                    if (counter >= 40) break;
                }
                if (counter >= 40) break;
            }

            db.NutritionistClientPlan.AddRange(list);
            db.SaveChanges();
        }

        // ---------------------------------------------------------------------
        // FOOD PLANS (PLAN + FOOD + PORTION)
        // ---------------------------------------------------------------------
        private static void PopulateFoodPlans(HealthWellbeingDbContext db)
        {
            if (db.FoodPlan.Any()) return;

            var plans = db.Plan.OrderBy(p => p.PlanId).ToList();
            var foods = db.Food.OrderBy(f => f.FoodId).ToList();
            var portions = db.Portion.OrderBy(p => p.PortionId).ToList();

            if (!plans.Any() || !foods.Any() || !portions.Any()) return;

            var defaultPortion = portions.First();
            var foodPlans = new List<FoodPlan>();

            void AddFoodsToPlan(Plan plan, int startIndex, int count)
            {
                for (int i = 0; i < count && (startIndex + i) < foods.Count; i++)
                {
                    var food = foods[startIndex + i];

                    foodPlans.Add(new FoodPlan
                    {
                        PlanId = plan.PlanId,
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

        // ---------------------------------------------------------------------
        // FOOD PLAN DAY
        // ---------------------------------------------------------------------
        private static void PopulateFoodPlanDays(HealthWellbeingDbContext db)
        {
            if (db.FoodPlanDay.Any()) return;

            var today = DateTime.Today;
            var plans = db.Plan.OrderBy(p => p.PlanId).ToList();

            var baseFoodPlans = db.FoodPlan
                .AsNoTracking()
                .OrderBy(fp => fp.PlanId)
                .ThenBy(fp => fp.FoodId)
                .ToList();

            if (!plans.Any() || !baseFoodPlans.Any()) return;

            var rng = new Random();
            var list = new List<FoodPlanDay>();

            foreach (var plan in plans)
            {
                var foodsForPlan = baseFoodPlans.Where(fp => fp.PlanId == plan.PlanId).ToList();
                if (!foodsForPlan.Any()) continue;

                for (int d = 0; d < 7; d++)
                {
                    var date = today.AddDays(d).Date;

                    foreach (var fp in foodsForPlan)
                    {
                        list.Add(new FoodPlanDay
                        {
                            PlanId = plan.PlanId,
                            FoodId = fp.FoodId,
                            PortionId = fp.PortionId,
                            Date = date,
                            PortionsPlanned = rng.Next(1, 4),
                            ScheduledTime = date.AddHours(9),
                            MealType = "Daily"
                        });
                    }
                }
            }

            db.FoodPlanDay.AddRange(list);
            db.SaveChanges();
        }

        // ---------------------------------------------------------------------
        // FOOD INTAKE (pré-criado a partir do FoodPlanDay)
        // ---------------------------------------------------------------------
        private static void PopulateFoodIntakeFromDays(HealthWellbeingDbContext db)
        {
            if (db.FoodIntake.Any()) return;

            var days = db.FoodPlanDay.AsNoTracking().ToList();
            if (!days.Any()) return;

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

        // ---------------------------------------------------------------------
        // JOIN: FOOD / NUTRITIONAL COMPONENT
        // ---------------------------------------------------------------------
        private static void PopulateFoodNutritionalComponents(HealthWellbeingDbContext db)
        {
            if (db.FoodNutritionalComponent.Any()) return;

            var foods = db.Food.OrderBy(f => f.FoodId).ToList();
            var comps = db.NutritionalComponent.OrderBy(c => c.NutritionalComponentId).ToList();

            if (!foods.Any() || !comps.Any()) return;

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
