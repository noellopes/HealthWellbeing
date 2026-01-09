using HealthWellbeing.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthWellbeing.Data
{
    public static class SeedData
    {
        // ==========================
        // PUBLIC ENTRYPOINT
        // ==========================
        public static void Populate(HealthWellbeingDbContext db)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));

            // Ensure database is up-to-date
            db.Database.Migrate();

            // Ordem correta (dependências)
            PopulateSpecialities(db);
            PopulateDoctors(db);
            PopulateAgendaMedica(db);

            var clients = PopulateClients(db);       // cria/garante clientes (inclui os dos utentes)
            PopulateGoals(db, clients);

            PopulateUtenteSaude(db);                 // depende de clientes (por Email)
            PopulateConsultas(db);                   // depende de doctors + utentes

            PopulateMember(db);                      // depende de clients
            PopulateTrainingType(db);
            PopulatePlan(db);                        // depende de clients

            PopulateNutritionist(db);
            PopulateAlergy(db);

            PopulateFoodCategory(db);
            PopulateFood(db);
            PopulatePortion(db);
            PopulateNutritionalComponent(db);

            PopulateClientAlergy(db);
            PopulateNutritionistClientPlan(db);

            PopulateFoodPlan(db);
            PopulateFoodPlanDay(db);
            PopulateFoodIntake(db);
            PopulateFoodNutritionalComponent(db);

            PopulateEventTypes(db);
            PopulateEvents(db);
            PopulateLevels(db);

            var trainers = PopulateTrainer(db);
            PopulateTraining(db, trainers);
        }

        // ==========================
        // SEED: SPECIALITIES
        // ==========================
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
                new Specialities { Nome = "Fisioterapia", Descricao = "Reabilitação motora e funcional após lesões, cirurgias ou doenças crónicas." }
            };

            db.Specialities.AddRange(especialidades);
            db.SaveChanges();
        }

        // ==========================
        // SEED: DOCTORS
        // ==========================
        private static void PopulateDoctors(HealthWellbeingDbContext db)
        {
            if (db.Doctor.Any()) return;

            var especialidadesDict = db.Specialities.ToDictionary(e => e.Nome, e => e);

            var doctors = new[]
            {
                new Doctor { Nome = "Ana Martins",     Telemovel = "912345678", Email = "ana.martins@healthwellbeing.pt",     Especialidade = especialidadesDict["Cardiologia"] },
                new Doctor { Nome = "Bruno Carvalho",  Telemovel = "913456789", Email = "bruno.carvalho@healthwellbeing.pt",  Especialidade = especialidadesDict["Dermatologia"] },
                new Doctor { Nome = "Carla Ferreira",  Telemovel = "914567890", Email = "carla.ferreira@healthwellbeing.pt",  Especialidade = especialidadesDict["Pediatria"] },
                new Doctor { Nome = "Daniel Sousa",    Telemovel = "915678901", Email = "daniel.sousa@healthwellbeing.pt",    Especialidade = especialidadesDict["Psiquiatria"] },
                new Doctor { Nome = "Eduarda Almeida", Telemovel = "916789012", Email = "eduarda.almeida@healthwellbeing.pt", Especialidade = especialidadesDict["Nutrição"] },
                new Doctor { Nome = "Fábio Pereira",   Telemovel = "917890123", Email = "fabio.pereira@healthwellbeing.pt",   Especialidade = especialidadesDict["Medicina Geral e Familiar"] },
                new Doctor { Nome = "Gabriela Rocha",  Telemovel = "918901234", Email = "gabriela.rocha@healthwellbeing.pt",  Especialidade = especialidadesDict["Ortopedia"] },
                new Doctor { Nome = "Hugo Santos",     Telemovel = "919012345", Email = "hugo.santos@healthwellbeing.pt",     Especialidade = especialidadesDict["Ginecologia e Obstetrícia"] },
                new Doctor { Nome = "Inês Correia",    Telemovel = "920123456", Email = "ines.correia@healthwellbeing.pt",    Especialidade = especialidadesDict["Psicologia"] },
                new Doctor { Nome = "João Ribeiro",    Telemovel = "921234567", Email = "joao.ribeiro@healthwellbeing.pt",    Especialidade = especialidadesDict["Fisioterapia"] },
                new Doctor { Nome = "Luísa Nogueira",  Telemovel = "922345678", Email = "luisa.nogueira@healthwellbeing.pt",  Especialidade = especialidadesDict["Medicina Geral e Familiar"] },
                new Doctor { Nome = "Miguel Costa",    Telemovel = "923456789", Email = "miguel.costa@healthwellbeing.pt",    Especialidade = especialidadesDict["Pediatria"] },
                new Doctor { Nome = "Nádia Gonçalves", Telemovel = "924567890", Email = "nadia.goncalves@healthwellbeing.pt",  Especialidade = especialidadesDict["Cardiologia"] },
                new Doctor { Nome = "Óscar Figueiredo",Telemovel = "925678901", Email = "oscar.figueiredo@healthwellbeing.pt", Especialidade = especialidadesDict["Pediatria"] },
                new Doctor { Nome = "Patrícia Lopes",  Telemovel = "926789012", Email = "patricia.lopes@healthwellbeing.pt",  Especialidade = especialidadesDict["Ginecologia e Obstetrícia"] }
            };

            db.Doctor.AddRange(doctors);
            db.SaveChanges();
        }

        // ==========================
        // SEED: AGENDA MEDICA
        // ==========================
        private static void PopulateAgendaMedica(HealthWellbeingDbContext db)
        {
            // Em vez de depender de "Any()", garantimos que existe agenda para os próximos 15 dias úteis
            var proximosDias = GetProximosDiasUteis(DateOnly.FromDateTime(DateTime.Today), 15);

            var medicos = db.Doctor.ToList()
                .GroupBy(m => m.Nome)
                .ToDictionary(g => g.Key, g => g.First());

            if (!medicos.Any()) return;

            var templateSemanal = new List<(string MedicoNome, DayOfWeek Dia, TimeOnly Ini, TimeOnly Fim)>
            {
                ("João Ribeiro", DayOfWeek.Monday,    new TimeOnly(9, 0),  new TimeOnly(12, 0)),
                ("João Ribeiro", DayOfWeek.Monday,    new TimeOnly(14, 0), new TimeOnly(17, 0)),
                ("João Ribeiro", DayOfWeek.Tuesday,   new TimeOnly(9, 0),  new TimeOnly(12, 0)),
                ("João Ribeiro", DayOfWeek.Wednesday, new TimeOnly(9, 0),  new TimeOnly(12, 0)),
                ("João Ribeiro", DayOfWeek.Thursday,  new TimeOnly(14, 0), new TimeOnly(17, 0)),
                ("João Ribeiro", DayOfWeek.Friday,    new TimeOnly(9, 0),  new TimeOnly(12, 0)),

                ("Carla Ferreira", DayOfWeek.Monday,    new TimeOnly(14, 0), new TimeOnly(18, 0)),
                ("Carla Ferreira", DayOfWeek.Tuesday,   new TimeOnly(9, 0),  new TimeOnly(12, 0)),
                ("Carla Ferreira", DayOfWeek.Tuesday,   new TimeOnly(14, 0), new TimeOnly(16, 0)),
                ("Carla Ferreira", DayOfWeek.Wednesday, new TimeOnly(9, 0),  new TimeOnly(12, 0)),
                ("Carla Ferreira", DayOfWeek.Thursday,  new TimeOnly(9, 0),  new TimeOnly(12, 0)),
                ("Carla Ferreira", DayOfWeek.Thursday,  new TimeOnly(14, 0), new TimeOnly(16, 0)),
                ("Carla Ferreira", DayOfWeek.Friday,    new TimeOnly(14, 0), new TimeOnly(18, 0))
            };

            var novos = new List<AgendaMedica>();

            foreach (var data in proximosDias)
            {
                foreach (var t in templateSemanal.Where(x => x.Dia == data.DayOfWeek))
                {
                    if (!medicos.TryGetValue(t.MedicoNome, out var medico)) continue;

                    var periodo = t.Ini < new TimeOnly(13, 0) ? "Manha" : "Tarde";

                    bool existe = db.AgendaMedica.Any(a =>
                        a.IdMedico == medico.IdMedico &&
                        a.Data == data &&
                        a.HoraInicio == t.Ini &&
                        a.HoraFim == t.Fim);

                    if (!existe)
                    {
                        novos.Add(new AgendaMedica
                        {
                            IdMedico = medico.IdMedico,
                            Data = data,
                            DiaSemana = data.DayOfWeek,
                            Periodo = periodo,
                            HoraInicio = t.Ini,
                            HoraFim = t.Fim
                        });
                    }
                }
            }

            if (novos.Any())
            {
                db.AgendaMedica.AddRange(novos);
                db.SaveChanges();
            }

            // Limpezas de segurança (para erros antigos)
            var antigosDefault = db.AgendaMedica.Where(a => a.Data == default).ToList();
            if (antigosDefault.Any())
            {
                db.AgendaMedica.RemoveRange(antigosDefault);
                db.SaveChanges();
            }

            var semPeriodo = db.AgendaMedica.Where(a => string.IsNullOrEmpty(a.Periodo)).ToList();
            if (semPeriodo.Any())
            {
                foreach (var a in semPeriodo)
                    a.Periodo = a.HoraInicio < new TimeOnly(13, 0) ? "Manha" : "Tarde";
                db.SaveChanges();
            }
        }

        // ==========================
        // SEED: CLIENTS (inclui utentes)
        // ==========================
        private static List<Client> PopulateClients(HealthWellbeingDbContext db)
        {
            var utentesData = GetUtentesSeedData();

            // Guardar emails existentes para não duplicar
            var existingEmails = new HashSet<string>(
                db.Client.Select(c => c.Email).Where(e => !string.IsNullOrWhiteSpace(e))!,
                StringComparer.OrdinalIgnoreCase
            );

            var toAdd = new List<Client>();

            // Clients “genéricos” (portugueses e com datas seguras)
            var baseClients = GetBaseClientsSeedData();

            foreach (var c in baseClients)
            {
                if (string.IsNullOrWhiteSpace(c.Email)) continue;
                if (existingEmails.Contains(c.Email)) continue;

                toAdd.Add(c);
                existingEmails.Add(c.Email);
            }

            // Clients para utentes (1-para-1 com UtenteSaude)
            foreach (var u in utentesData)
            {
                if (existingEmails.Contains(u.Email)) continue;

                toAdd.Add(new Client
                {
                    Name = u.Name,
                    Email = u.Email,
                    Phone = u.Phone,
                    Address = u.Address,
                    BirthDate = u.BirthDate,
                    Gender = u.Gender,
                    RegistrationDate = DateTime.Now
                });

                existingEmails.Add(u.Email);
            }

            if (toAdd.Any())
            {
                db.Client.AddRange(toAdd);
                db.SaveChanges();
            }

            return db.Client.AsNoTracking().ToList();
        }

        // ==========================
        // SEED: GOALS
        // ==========================
        private static void PopulateGoals(HealthWellbeingDbContext db, List<Client> clients)
        {
            if (db.Goal.Any()) return;
            if (clients == null || clients.Count == 0) clients = db.Client.AsNoTracking().ToList();
            if (!clients.Any()) return;

            var rng = new Random(123);
            var goals = new List<Goal>();
            int idx = 0;

            foreach (var client in clients)
            {
                string goalName = idx % 3 == 0 ? "Weight Loss"
                                : idx % 3 == 1 ? "Muscle Gain"
                                : "Maintenance";

                double weight = rng.Next(55, 95);
                double activity = goalName == "Weight Loss" ? 1.3
                               : goalName == "Muscle Gain" ? 1.7
                               : 1.5;

                double calories = weight * 22 * activity;
                double protein = weight * 1.6;
                double fat = calories * 0.27 / 9;
                double proteinCal = protein * 4;
                double hydrates = (calories - proteinCal - (fat * 9)) / 4;

                goals.Add(new Goal
                {
                    ClientId = client.ClientId,
                    GoalName = goalName,
                    DailyCalories = (int)Math.Round(calories),
                    DailyProtein = (int)Math.Round(protein),
                    DailyFat = (int)Math.Round(fat),
                    DailyHydrates = (int)Math.Round(hydrates)
                });

                idx++;
            }

            db.Goal.AddRange(goals);
            db.SaveChanges();
        }

        // ==========================
        // SEED: UTENTE SAUDE
        // ==========================
        private static void PopulateUtenteSaude(HealthWellbeingDbContext db)
        {
            var utentesData = GetUtentesSeedData();

            // se já existem alguns, só adiciona os que faltam
            var existingByClientId = new HashSet<int>(db.UtenteSaude.Select(u => u.ClientId));
            var existingNifs = new HashSet<string>(db.UtenteSaude.Select(u => u.Nif).Where(x => x != null)!);

            var toAdd = new List<UtenteSaude>();

            foreach (var u in utentesData)
            {
                var client = db.Client.FirstOrDefault(c => c.Email == u.Email);
                if (client == null) continue;

                if (existingByClientId.Contains(client.ClientId)) continue;
                if (!string.IsNullOrWhiteSpace(u.Nif) && existingNifs.Contains(u.Nif)) continue;

                toAdd.Add(new UtenteSaude
                {
                    ClientId = client.ClientId,
                    Nif = u.Nif,
                    Niss = u.Niss,
                    Nus = u.Nus
                });

                existingByClientId.Add(client.ClientId);
                if (!string.IsNullOrWhiteSpace(u.Nif)) existingNifs.Add(u.Nif);
            }

            if (toAdd.Any())
            {
                db.UtenteSaude.AddRange(toAdd);
                db.SaveChanges();
            }
        }

        // ==========================
        // SEED: CONSULTAS
        // ==========================
        private static void PopulateConsultas(HealthWellbeingDbContext db)
        {
            if (db.Consulta.Any()) return;

            var doctorsList = db.Doctor.OrderBy(d => d.IdMedico).ToList();
            var utentesList = db.UtenteSaude.OrderBy(u => u.UtenteSaudeId).ToList();

            if (!doctorsList.Any() || !utentesList.Any())
                return;

            var hoje = DateTime.Today;

            var consultas = new List<Consulta>
            {
                new Consulta
                {
                    DataMarcacao = new DateTime(2025, 10, 10, 9, 15, 0),
                    DataConsulta = new DateTime(2026, 1, 5, 9, 0, 0),
                    HoraInicio   = new TimeOnly(9, 0),
                    HoraFim      = new TimeOnly(9, 30)
                },
                new Consulta
                {
                    DataMarcacao = new DateTime(2025, 10, 12, 14, 40, 0),
                    DataConsulta = new DateTime(2026, 1, 10, 11, 15, 0),
                    HoraInicio   = new TimeOnly(11, 15),
                    HoraFim      = new TimeOnly(12, 0)
                },
                new Consulta
                {
                    DataMarcacao = new DateTime(2025, 10, 15, 16, 5, 0),
                    DataConsulta = new DateTime(2026, 1, 10, 15, 0, 0),
                    HoraInicio   = new TimeOnly(15, 0),
                    HoraFim      = new TimeOnly(15, 45)
                },

                // Históricas
                new Consulta
                {
                    DataMarcacao = new DateTime(2025, 7, 5, 13, 10, 0),
                    DataConsulta = new DateTime(2025, 8, 10, 16, 0, 0),
                    HoraInicio   = new TimeOnly(16, 0),
                    HoraFim      = new TimeOnly(16, 30)
                },

                // Canceladas
                new Consulta
                {
                    DataMarcacao     = new DateTime(2025, 10, 1, 10, 0, 0),
                    DataConsulta     = new DateTime(2025, 10, 30, 9, 0, 0),
                    DataCancelamento = new DateTime(2025, 10, 28, 9, 30, 0),
                    HoraInicio       = new TimeOnly(9, 0),
                    HoraFim          = new TimeOnly(9, 30)
                },

                // “Hoje” (consistente!)
                new Consulta
                {
                    DataMarcacao = hoje.AddDays(-2).AddHours(10),
                    DataConsulta = new DateTime(hoje.Year, hoje.Month, hoje.Day, 15, 30, 0),
                    HoraInicio   = new TimeOnly(15, 30),
                    HoraFim      = new TimeOnly(16, 0)
                }
            };

            for (int i = 0; i < consultas.Count; i++)
            {
                var d = doctorsList[i % doctorsList.Count];
                var u = utentesList[i % utentesList.Count];

                consultas[i].IdMedico = d.IdMedico;
                consultas[i].IdEspecialidade = d.IdEspecialidade;
                consultas[i].IdUtenteSaude = u.UtenteSaudeId;
            }

            db.Consulta.AddRange(consultas);
            db.SaveChanges();
        }

        // ==========================
        // SEED: MEMBERS
        // ==========================
        private static void PopulateMember(HealthWellbeingDbContext db)
        {
            if (db.Member.Any()) return;

            var clients = db.Client.OrderBy(c => c.ClientId).Take(3).ToList();
            if (!clients.Any()) return;

            var members = clients.Select(c => new Member { ClientId = c.ClientId }).ToList();
            db.Member.AddRange(members);
            db.SaveChanges();
        }

        // ==========================
        // SEED: TRAINING TYPES
        // ==========================
        private static void PopulateTrainingType(HealthWellbeingDbContext db)
        {
            if (db.TrainingType.Any()) return;

            db.TrainingType.AddRange(new List<TrainingType>
            {
                new TrainingType { Name = "Yoga Basics", Description = "Introdução ao yoga.", DurationMinutes = 60, IsActive = true },
                new TrainingType { Name = "HIIT (High Intensity Interval Training)", Description = "Treino intenso.", DurationMinutes = 45, IsActive = true },
                new TrainingType { Name = "Pilates Core Strength", Description = "Força do core.", DurationMinutes = 50, IsActive = true },
                new TrainingType { Name = "Zumba Dance", Description = "Dança fitness.", DurationMinutes = 55, IsActive = true },
                new TrainingType { Name = "Strength Training", Description = "Treino de força.", DurationMinutes = 120, IsActive = true }
            });

            db.SaveChanges();
        }

        // ==========================
        // SEED: PLANS
        // ==========================
        private static void PopulatePlan(HealthWellbeingDbContext db)
        {
            if (db.Plan.Any()) return;

            var allClients = db.Client.OrderBy(c => c.ClientId).ToList();
            if (!allClients.Any()) return;

            var plans = new List<Plan>();
            DateTime today = DateTime.Today;

            int count = Math.Min(allClients.Count, 30);

            for (int i = 0; i < count; i++)
            {
                var start = today.AddDays(-i * 7);
                plans.Add(new Plan
                {
                    ClientId = allClients[i].ClientId,
                    StartingDate = start,
                    EndingDate = start.AddDays(30),
                    Done = (i % 3 == 0)
                });
            }

            db.Plan.AddRange(plans);
            db.SaveChanges();
        }

        // ==========================
        // SEED: NUTRITIONISTS
        // ==========================
        private static void PopulateNutritionist(HealthWellbeingDbContext db)
        {
            if (db.Nutritionist.Any()) return;

            var nutritionists = new List<Nutritionist>
            {
                new Nutritionist { Name = "Dr. Joao Carvalho", Email = "joao.carvalho@healthwellbeing.com", Gender = "Male" },
                new Nutritionist { Name = "Dr. Sofia Martins", Email = "sofia.martins@healthwellbeing.com", Gender = "Female" },
                new Nutritionist { Name = "Dr. Ricardo Soares", Email = "ricardo.soares@healthwellbeing.com", Gender = "Male" }
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

        // ==========================
        // SEED: ALERGIES
        // ==========================
        private static void PopulateAlergy(HealthWellbeingDbContext db)
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

        // ==========================
        // SEED: FOOD CATEGORIES
        // ==========================
        private static void PopulateFoodCategory(HealthWellbeingDbContext db)
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

        // ==========================
        // SEED: FOODS
        // ==========================
        private static void PopulateFood(HealthWellbeingDbContext db)
        {
            if (db.Food.Any()) return;

            var categories = db.FoodCategory.OrderBy(c => c.CategoryId).ToList();
            if (!categories.Any()) return;

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

            for (int i = foods.Count + 1; i <= 30; i++)
            {
                var cat = categories[(i - 1) % categories.Count];
                foods.Add(new Food { CategoryId = cat.CategoryId, Name = $"Test Food {i}" });
            }

            db.Food.AddRange(foods);
            db.SaveChanges();
        }

        // ==========================
        // SEED: PORTIONS
        // ==========================
        private static void PopulatePortion(HealthWellbeingDbContext db)
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

        // ==========================
        // SEED: NUTRITIONAL COMPONENTS
        // ==========================
        private static void PopulateNutritionalComponent(HealthWellbeingDbContext db)
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

        // ==========================
        // SEED: CLIENT-ALERGY
        // ==========================
        private static void PopulateClientAlergy(HealthWellbeingDbContext db)
        {
            if (db.ClientAlergy.Any()) return;

            var clientsAll = db.Client.OrderBy(c => c.ClientId).ToList();
            var alergiesAll = db.Alergy.OrderBy(a => a.AlergyId).ToList();

            if (!clientsAll.Any() || !alergiesAll.Any()) return;

            var clientAlergies = new List<ClientAlergy>();
            int counter = 0;

            for (int i = 0; i < clientsAll.Count && counter < 40; i++)
            {
                for (int j = 0; j < alergiesAll.Count && counter < 40; j++)
                {
                    if ((i + j) % 4 == 0)
                    {
                        clientAlergies.Add(new ClientAlergy
                        {
                            ClientId = clientsAll[i].ClientId,
                            AlergyId = alergiesAll[j].AlergyId
                        });
                        counter++;
                    }
                }
            }

            if (clientAlergies.Any())
            {
                db.ClientAlergy.AddRange(clientAlergies);
                db.SaveChanges();
            }
        }

        // ==========================
        // SEED: NUTRITIONIST-CLIENT-PLAN
        // ==========================
        private static void PopulateNutritionistClientPlan(HealthWellbeingDbContext db)
        {
            if (db.NutritionistClientPlan.Any()) return;

            var clientsAll = db.Client.OrderBy(c => c.ClientId).ToList();
            var nutritAll = db.Nutritionist.OrderBy(n => n.NutritionistId).ToList();
            var plansAll = db.Plan.OrderBy(p => p.PlanId).ToList();

            if (!clientsAll.Any() || !nutritAll.Any() || !plansAll.Any()) return;

            var linkList = new List<NutritionistClientPlan>();
            int counter = 0;

            for (int i = 0; i < clientsAll.Count && counter < 40; i++)
            {
                for (int j = 0; j < nutritAll.Count && counter < 40; j++)
                {
                    var plan = plansAll[(i + j) % plansAll.Count];
                    linkList.Add(new NutritionistClientPlan
                    {
                        ClientId = clientsAll[i].ClientId,
                        NutritionistId = nutritAll[j].NutritionistId,
                        PlanId = plan.PlanId
                    });

                    counter++;
                }
            }

            if (linkList.Any())
            {
                db.NutritionistClientPlan.AddRange(linkList);
                db.SaveChanges();
            }
        }

        // ==========================
        // SEED: FOOD PLANS
        // ==========================
        private static void PopulateFoodPlan(HealthWellbeingDbContext db)
        {
            if (db.FoodPlan.Any()) return;

            var plansAll = db.Plan.OrderBy(p => p.PlanId).ToList();
            var foodsAll = db.Food.OrderBy(f => f.FoodId).ToList();
            var portionsAll = db.Portion.OrderBy(p => p.PortionId).ToList();

            if (!plansAll.Any() || !foodsAll.Any() || !portionsAll.Any()) return;

            var defaultPortion = portionsAll.First();
            var foodPlans = new List<FoodPlan>();

            void AddFoodsToPlan(Plan plan, int startIndex, int count)
            {
                for (int i = 0; i < count && (startIndex + i) < foodsAll.Count; i++)
                {
                    var food = foodsAll[startIndex + i];
                    foodPlans.Add(new FoodPlan
                    {
                        PlanId = plan.PlanId,
                        FoodId = food.FoodId,
                        PortionId = defaultPortion.PortionId
                    });
                }
            }

            if (plansAll.Count >= 1) AddFoodsToPlan(plansAll[0], 0, 4);
            if (plansAll.Count >= 2) AddFoodsToPlan(plansAll[1], 4, 5);
            if (plansAll.Count >= 3) AddFoodsToPlan(plansAll[2], 9, 3);

            if (foodPlans.Any())
            {
                db.FoodPlan.AddRange(foodPlans);
                db.SaveChanges();
            }
        }

        // ==========================
        // SEED: FOOD PLAN DAYS
        // ==========================
        private static void PopulateFoodPlanDay(HealthWellbeingDbContext db)
        {
            if (db.FoodPlanDay.Any()) return;

            var today = DateTime.Today;
            var plansAll = db.Plan.OrderBy(p => p.PlanId).ToList();
            var baseFoodPlans = db.FoodPlan.AsNoTracking().OrderBy(fp => fp.PlanId).ThenBy(fp => fp.FoodId).ToList();

            if (!plansAll.Any() || !baseFoodPlans.Any()) return;

            var rng = new Random(123);
            var dayPlans = new List<FoodPlanDay>();

            foreach (var plan in plansAll)
            {
                var foodsForPlan = baseFoodPlans.Where(fp => fp.PlanId == plan.PlanId).ToList();
                if (!foodsForPlan.Any()) continue;

                for (int d = 0; d < 7; d++)
                {
                    var date = today.AddDays(d);

                    foreach (var fp in foodsForPlan)
                    {
                        dayPlans.Add(new FoodPlanDay
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

            if (dayPlans.Any())
            {
                db.FoodPlanDay.AddRange(dayPlans);
                db.SaveChanges();
            }
        }

        // ==========================
        // SEED: FOOD INTAKE
        // ==========================
        private static void PopulateFoodIntake(HealthWellbeingDbContext db)
        {
            if (db.FoodIntake.Any()) return;

            var days = db.FoodPlanDay.AsNoTracking().ToList();
            if (!days.Any()) return;

            var intakeList = days.Select(x => new FoodIntake
            {
                PlanId = x.PlanId,
                FoodId = x.FoodId,
                PortionId = x.PortionId,
                Date = x.Date,
                ScheduledTime = x.ScheduledTime ?? x.Date.AddHours(9),
                PortionsPlanned = x.PortionsPlanned,
                PortionsEaten = 0
            }).ToList();

            db.FoodIntake.AddRange(intakeList);
            db.SaveChanges();
        }

        // ==========================
        // SEED: FOOD NUTRITIONAL COMPONENTS
        // ==========================
        private static void PopulateFoodNutritionalComponent(HealthWellbeingDbContext db)
        {
            if (db.FoodNutritionalComponent.Any()) return;

            var foodsAll = db.Food.OrderBy(f => f.FoodId).ToList();
            var compsAll = db.NutritionalComponent.OrderBy(c => c.NutritionalComponentId).ToList();

            if (!foodsAll.Any() || !compsAll.Any()) return;

            var foodComps = new List<FoodNutritionalComponent>();
            int counter = 0;

            foreach (var food in foodsAll)
            {
                foreach (var comp in compsAll)
                {
                    foodComps.Add(new FoodNutritionalComponent
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

            if (foodComps.Any())
            {
                db.FoodNutritionalComponent.AddRange(foodComps);
                db.SaveChanges();
            }
        }

        // ==========================
        // SEED: EVENT TYPES
        // ==========================
        private static void PopulateEventTypes(HealthWellbeingDbContext db)
        {
            if (db.EventType.Any()) return;

            db.EventType.AddRange(new List<EventType>
            {
                new EventType { EventTypeName = "Workshop Educacional", EventTypeScoringMode = "fixed", EventTypeMultiplier = 1.0f },
                new EventType { EventTypeName = "Seminário Temático", EventTypeScoringMode = "fixed", EventTypeMultiplier = 1.1f },
                new EventType { EventTypeName = "Palestra Informativa", EventTypeScoringMode = "fixed", EventTypeMultiplier = 1.0f },
                new EventType { EventTypeName = "Demonstração Técnica", EventTypeScoringMode = "fixed", EventTypeMultiplier = 1.0f },
                new EventType { EventTypeName = "Sessão de Orientação", EventTypeScoringMode = "fixed", EventTypeMultiplier = 1.0f },

                new EventType { EventTypeName = "Sessão de Running", EventTypeScoringMode = "time_based", EventTypeMultiplier = 2.0f },
                new EventType { EventTypeName = "Treino de Cycling", EventTypeScoringMode = "time_based", EventTypeMultiplier = 2.1f },
                new EventType { EventTypeName = "Aula de Cardio-Grupo", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.5f },
                new EventType { EventTypeName = "Treino de Natação", EventTypeScoringMode = "time_based", EventTypeMultiplier = 2.2f },
                new EventType { EventTypeName = "Sessão de HIIT", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.8f },

                new EventType { EventTypeName = "Treino de Musculação", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.7f },
                new EventType { EventTypeName = "Sessão de CrossFit", EventTypeScoringMode = "progressive", EventTypeMultiplier = 2.1f },
                new EventType { EventTypeName = "Treino Funcional", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.6f },
                new EventType { EventTypeName = "Aula de Powerlifting", EventTypeScoringMode = "progressive", EventTypeMultiplier = 2.0f },
                new EventType { EventTypeName = "Treino de Calistenia", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.5f },

                new EventType { EventTypeName = "Aula de Yoga", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.3f },
                new EventType { EventTypeName = "Sessão de Pilates", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.4f },
                new EventType { EventTypeName = "Treino de Flexibilidade", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.5f },
                new EventType { EventTypeName = "Aula de Mobilidade", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.3f },
                new EventType { EventTypeName = "Sessão de Alongamento", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.2f },

                new EventType { EventTypeName = "Aula de Artes Marciais", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.8f },
                new EventType { EventTypeName = "Treino de Boxe", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.9f },
                new EventType { EventTypeName = "Sessão de Lutas", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.8f },
                new EventType { EventTypeName = "Aula de Defesa Pessoal", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.4f },
                new EventType { EventTypeName = "Treino Desportivo Coletivo", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.6f },

                new EventType { EventTypeName = "Competição de Running", EventTypeScoringMode = "time_based", EventTypeMultiplier = 2.5f },
                new EventType { EventTypeName = "Torneio Desportivo", EventTypeScoringMode = "binary", EventTypeMultiplier = 2.3f },
                new EventType { EventTypeName = "Desafio de Resistência", EventTypeScoringMode = "time_based", EventTypeMultiplier = 2.4f },
                new EventType { EventTypeName = "Competição de Força", EventTypeScoringMode = "progressive", EventTypeMultiplier = 2.2f },
                new EventType { EventTypeName = "Desafio de Superação", EventTypeScoringMode = "progressive", EventTypeMultiplier = 2.1f },

                new EventType { EventTypeName = "Aula de Grupo", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.4f },
                new EventType { EventTypeName = "Treino Coletivo", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.5f },
                new EventType { EventTypeName = "Workshop Prático", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.3f },
                new EventType { EventTypeName = "Sessão de Team Building", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.2f },
                new EventType { EventTypeName = "Aula Experimental", EventTypeScoringMode = "binary", EventTypeMultiplier = 1.1f },

                new EventType { EventTypeName = "Treino Técnico", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.6f },
                new EventType { EventTypeName = "Workshop de Técnica", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.3f },
                new EventType { EventTypeName = "Aula Avançada", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.7f },
                new EventType { EventTypeName = "Sessão de Perfeiçoamento", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.5f },
                new EventType { EventTypeName = "Treino Especializado", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.8f }
            });

            db.SaveChanges();
        }

        // ==========================
        // SEED: EVENTS
        // ==========================
        private static void PopulateEvents(HealthWellbeingDbContext db)
        {
            if (db.Event.Any()) return;

            var eventTypes = db.EventType.OrderBy(e => e.EventTypeId).ToList();
            if (!eventTypes.Any()) return;

            var now = DateTime.Now;

            // só crio alguns para não ficar gigante (mantém o espírito do teu seed)
            var eventList = new List<Event>
            {
                new Event { EventName = "Workshop de Nutrição", EventDescription = "Aprenda a comer melhor.", EventTypeId = eventTypes[1].EventTypeId, EventStart = now.AddDays(-28), EventEnd = now.AddDays(-28).AddHours(2), EventPoints = 50, MinLevel = 1 },
                new Event { EventName = "Aula de Zumba", EventDescription = "Dança e diversão.", EventTypeId = eventTypes[3].EventTypeId, EventStart = now.AddDays(-10), EventEnd = now.AddDays(-10).AddHours(1), EventPoints = 75, MinLevel = 1 },
                new Event { EventName = "Desafio de Sprint", EventDescription = "Evento a decorrer agora.", EventTypeId = eventTypes[5].EventTypeId, EventStart = now.AddMinutes(-30), EventEnd = now.AddMinutes(30), EventPoints = 110, MinLevel = 2 },
                new Event { EventName = "Workshop Prático de Primeiros Socorros", EventDescription = "Saiba como agir.", EventTypeId = eventTypes[0].EventTypeId, EventStart = now.AddDays(1), EventEnd = now.AddDays(1).AddHours(3), EventPoints = 75, MinLevel = 1 },
            };

            db.Event.AddRange(eventList);
            db.SaveChanges();
        }

        // ==========================
        // SEED: LEVELS
        // ==========================
        private static void PopulateLevels(HealthWellbeingDbContext db)
        {
            if (db.Level.Any()) return;

            db.Level.AddRange(new List<Level>
            {
                new Level { LevelAtual = 1, LevelCategory = "Iniciante", Description = "Primeiros passos na jornada de saúde" },
                new Level { LevelAtual = 2, LevelCategory = "Iniciante", Description = "Começando a criar rotinas saudáveis" },
                new Level { LevelAtual = 3, LevelCategory = "Iniciante", Description = "Ganhando consistência nos exercícios" },
                new Level { LevelAtual = 4, LevelCategory = "Iniciante", Description = "Progresso constante na saúde" },
                new Level { LevelAtual = 5, LevelCategory = "Iniciante", Description = "Final da fase inicial - bons hábitos estabelecidos" },
                new Level { LevelAtual = 6, LevelCategory = "Intermediário", Description = "Entrando na fase intermediária" },
                new Level { LevelAtual = 7, LevelCategory = "Intermediário", Description = "Desenvolvendo resistência física" },
                new Level { LevelAtual = 8, LevelCategory = "Intermediário", Description = "Melhorando performance geral" },
                new Level { LevelAtual = 9, LevelCategory = "Intermediário", Description = "Consolidação de técnicas avançadas" },
                new Level { LevelAtual = 10, LevelCategory = "Intermediário", Description = "Pronto para desafios maiores" }
            });

            db.SaveChanges();
        }

        // ==========================
        // SEED: TRAINERS
        // ==========================
        private static List<Trainer> PopulateTrainer(HealthWellbeingDbContext db)
        {
            if (!db.Trainer.Any())
            {
                db.Trainer.AddRange(new List<Trainer>
                {
                    new Trainer { Name = "John Smith", Speciality = "HIIT (High Intensity Interval Training)", Email = "john.smith@fitnesspro.com", Phone = "911223344", BirthDate = new DateTime(1988, 7, 10), Gender = "Male" },
                    new Trainer { Name = "Emma Johnson", Speciality = "Strength Training", Email = "emma.johnson@strongfit.net", Phone = "912334455", BirthDate = new DateTime(1992, 11, 25), Gender = "Female" },
                    new Trainer { Name = "Carlos Mendes", Speciality = "Yoga Basics", Email = "carlos.mendes@yogabalance.org", Phone = "913445566", BirthDate = new DateTime(1975, 4, 1), Gender = "Male" },
                    new Trainer { Name = "Sophie Lee", Speciality = "Pilates Core Strength", Email = "sophie.lee@corewellness.com", Phone = "914556677", BirthDate = new DateTime(1996, 2, 14), Gender = "Female" },
                    new Trainer { Name = "Maria Rodriguez", Speciality = "Zumba Dance", Email = "maria.rodriguez@zumbafit.com", Phone = "915667788", BirthDate = new DateTime(1985, 9, 30), Gender = "Female" }
                });

                db.SaveChanges();
            }

            return db.Trainer.ToList();
        }

        // ==========================
        // SEED: TRAININGS
        // ==========================
        private static void PopulateTraining(HealthWellbeingDbContext db, List<Trainer> trainerList)
        {
            if (db.Training.Any()) return;
            if (trainerList == null || trainerList.Count == 0) return;

            var trainerCarlos = trainerList.FirstOrDefault(t => t.Name == "Carlos Mendes");
            var trainerJohn = trainerList.FirstOrDefault(t => t.Name == "John Smith");

            var yogaType = db.TrainingType.FirstOrDefault(t => t.Name == "Yoga Basics");
            var hiitType = db.TrainingType.FirstOrDefault(t => t.Name == "HIIT (High Intensity Interval Training)");

            if (trainerCarlos == null || trainerJohn == null || yogaType == null || hiitType == null) return;

            var trainings = new List<Training>
            {
                new Training { TrainingTypeId = yogaType.TrainingTypeId, TrainerId = trainerCarlos.TrainerId, Name = "Morning Yoga", Duration = 60, DayOfWeek = "Monday", StartTime = new TimeSpan(10, 0, 0), MaxParticipants = 15 },
                new Training { TrainingTypeId = hiitType.TrainingTypeId, TrainerId = trainerJohn.TrainerId, Name = "Intense Cardio HIT", Duration = 45, DayOfWeek = "Wednesday", StartTime = new TimeSpan(18, 30, 0), MaxParticipants = 20 },
                new Training { TrainingTypeId = hiitType.TrainingTypeId, TrainerId = trainerJohn.TrainerId, Name = "Strength Training", Duration = 120, DayOfWeek = "Friday", StartTime = new TimeSpan(16, 0, 0), MaxParticipants = 8 }
            };

            db.Training.AddRange(trainings);
            db.SaveChanges();
        }

        // ==========================
        // HELPERS: DATASETS
        // ==========================
        private sealed record UtenteSeed(
            string Name,
            string Email,
            string Phone,
            string Address,
            DateTime BirthDate,
            string Gender,
            string Nif,
            string Niss,
            string Nus
        );

        private static List<UtenteSeed> GetUtentesSeedData() => new()
        {
            new UtenteSeed("Ana Beatriz Silva", "ana.beatriz.silva@example.pt", "912345670", "Rua das Flores, 12, Guarda", new DateTime(1999, 4, 8), "Female", "245379261", "12345678901", "123456789"),
            new UtenteSeed("Bruno Miguel Pereira", "bruno.miguel.pereira@example.pt", "912345671", "Av. 25 de Abril, 102, Guarda", new DateTime(1987, 11, 23), "Male", "198754326", "22345678901", "223456789"),
            new UtenteSeed("Carla Sofia Fernandes", "carla.sofia.fernandes@example.pt", "912345672", "Rua da Liberdade, 45, Covilhã", new DateTime(1991, 5, 19), "Female", "156987239", "32345678901", "323456789"),
            new UtenteSeed("Daniel Rocha", "daniel.rocha@example.pt", "912345673", "Travessa do Sol, 3, Celorico da Beira", new DateTime(2003, 10, 26), "Male", "268945315", "42345678901", "423456789"),
            new UtenteSeed("Eduarda Nogueira", "eduarda.nogueira@example.pt", "912345674", "Rua do Comércio, 89, Seia", new DateTime(1994, 5, 22), "Female", "296378459", "52345678901", "523456789"),
            new UtenteSeed("Fábio Gonçalves", "fabio.goncalves@example.pt", "912345675", "Rua da Escola, 5, Gouveia", new DateTime(1997, 1, 4), "Male", "165947829", "62345678901", "623456789"),
            new UtenteSeed("Gabriela Santos", "gabriela.santos@example.pt", "912345676", "Av. Dr. Francisco Sá Carneiro, 200, Viseu", new DateTime(1986, 4, 26), "Female", "189567240", "72345678901", "723456789"),
            new UtenteSeed("Hugo Matos", "hugo.matos@example.pt", "912345677", "Rua do Castelo, 7, Belmonte", new DateTime(1993, 11, 22), "Male", "215983747", "82345678901", "823456789"),
            new UtenteSeed("Inês Carvalho", "ines.carvalho@example.pt", "912345678", "Rua do Mercado, 14, Trancoso", new DateTime(2004, 7, 12), "Female", "235679845", "92345678901", "923456789"),
            new UtenteSeed("João Marques", "joao.marques@example.pt", "912345679", "Rua da Estação, 33, Pinhel", new DateTime(1990, 7, 4), "Male", "286754197", "10345678901", "103456789"),
        };

        private static List<Client> GetBaseClientsSeedData()
        {
            var now = DateTime.Now;
            return new List<Client>
            {
                new Client { Name = "Alice Wonderland", Email = "alice.w@example.com", Phone = "910000001", Address = "Lisboa", BirthDate = new DateTime(1990, 5, 15), Gender = "Female", RegistrationDate = now.AddDays(-30) },
                new Client { Name = "Bob The Builder", Email = "bob.builder@work.net", Phone = "910000002", Address = "Porto", BirthDate = new DateTime(1985, 10, 20), Gender = "Male", RegistrationDate = now.AddDays(-15) },
                new Client { Name = "Charlie Brown", Email = "charlie.b@peanuts.com", Phone = "910000003", Address = "Coimbra", BirthDate = new DateTime(2000, 1, 1), Gender = "Male", RegistrationDate = now.AddDays(-5) },
                new Client { Name = "David Copperfield", Email = "david.c@magic.com", Phone = "910000004", Address = "Braga", BirthDate = new DateTime(1960, 9, 16), Gender = "Male", RegistrationDate = now.AddDays(-25) },
                new Client { Name = "Eve Harrington", Email = "eve.h@stage.net", Phone = "910000005", Address = "Viseu", BirthDate = new DateTime(1995, 2, 28), Gender = "Female", RegistrationDate = now.AddDays(-10) },
            };
        }

        // Helper: get next N working days (Mon-Fri) from a start date
        private static List<DateOnly> GetProximosDiasUteis(DateOnly start, int count)
        {
            var res = new List<DateOnly>(count);
            var d = start;

            while (res.Count < count)
            {
                if (d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday)
                    res.Add(d);

                d = d.AddDays(1);
            }

            return res;
        }

        // ==========================
        // IDENTITY SEED (Roles/Users)
        // ==========================
        internal static void SeedDefaultAdmin(UserManager<IdentityUser> userManager)
        {
            EnsureUserIsCreatedAsync(userManager, "admin@jbma.pt", "Secret123$", new[] { "Administrador" })
                .GetAwaiter().GetResult();
        }

        internal static void SeedUsers(UserManager<IdentityUser> userManager)
        {
            EnsureUserIsCreatedAsync(userManager, "anab@jbma.pt", "Secret123$", new[] { "Utente" }).GetAwaiter().GetResult();
            EnsureUserIsCreatedAsync(userManager, "brunoMP@jbma.pt", "Secret123$", new[] { "Utente" }).GetAwaiter().GetResult();
            EnsureUserIsCreatedAsync(userManager, "diretorClinico@healthwellbeing.pt", "Secret123$", new[] { "DiretorClinico" }).GetAwaiter().GetResult();

            EnsureUserIsCreatedAsync(userManager, "carla.ferreira@healthwellbeing.pt", "Secret123$", new[] { "Medico" }).GetAwaiter().GetResult();
            EnsureUserIsCreatedAsync(userManager, "bruno.carvalho@healthwellbeing.pt", "Secret123$", new[] { "Medico" }).GetAwaiter().GetResult();
            EnsureUserIsCreatedAsync(userManager, "ana.beatriz.silva@example.pt", "Secret123$", new[] { "Utente" }).GetAwaiter().GetResult();
            EnsureUserIsCreatedAsync(userManager, "ana.martins@healthwellbeing.pt", "Secret123$", new[] { "Medico" }).GetAwaiter().GetResult();
            EnsureUserIsCreatedAsync(userManager, "rececionista@healthwellbeing.pt", "Secret123$", new[] { "Rececionista" }).GetAwaiter().GetResult();
        }

        internal static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            EnsureRoleIsCreatedAsync(roleManager, "Administrador").GetAwaiter().GetResult();
            EnsureRoleIsCreatedAsync(roleManager, "DiretorClinico").GetAwaiter().GetResult();
            EnsureRoleIsCreatedAsync(roleManager, "Utente").GetAwaiter().GetResult();
            EnsureRoleIsCreatedAsync(roleManager, "Medico").GetAwaiter().GetResult();
            EnsureRoleIsCreatedAsync(roleManager, "Rececionista").GetAwaiter().GetResult();
        }

        private static async Task EnsureRoleIsCreatedAsync(RoleManager<IdentityRole> roleManager, string role)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // ✅ EmailConfirmed + garantir email + reset password se não bater certo
        private static async Task EnsureUserIsCreatedAsync(
            UserManager<IdentityUser> userManager,
            string username,
            string password,
            string[] roles)
        {
            IdentityUser? user =
                await userManager.FindByEmailAsync(username) ??
                await userManager.FindByNameAsync(username);

            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = username,
                    Email = username,
                    EmailConfirmed = true,
                    LockoutEnabled = false
                };

                var createRes = await userManager.CreateAsync(user, password);
                if (!createRes.Succeeded)
                {
                    var errors = string.Join(" | ", createRes.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Erro ao criar utilizador '{username}': {errors}");
                }
            }
            else
            {
                bool changed = false;

                if (string.IsNullOrWhiteSpace(user.Email))
                {
                    user.Email = username;
                    changed = true;
                }

                if (!user.EmailConfirmed)
                {
                    user.EmailConfirmed = true;
                    changed = true;
                }

                if (changed)
                {
                    var updateRes = await userManager.UpdateAsync(user);
                    if (!updateRes.Succeeded)
                    {
                        var errors = string.Join(" | ", updateRes.Errors.Select(e => e.Description));
                        throw new InvalidOperationException($"Erro ao atualizar utilizador '{username}': {errors}");
                    }
                }

                bool passOk = await userManager.CheckPasswordAsync(user, password);
                if (!passOk)
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);
                    var resetRes = await userManager.ResetPasswordAsync(user, token, password);
                    if (!resetRes.Succeeded)
                    {
                        var errors = string.Join(" | ", resetRes.Errors.Select(e => e.Description));
                        throw new InvalidOperationException($"Erro ao resetar password do utilizador '{username}': {errors}");
                    }
                }
            }

            foreach (var role in roles)
            {
                if (!await userManager.IsInRoleAsync(user, role))
                {
                    var roleRes = await userManager.AddToRoleAsync(user, role);
                    if (!roleRes.Succeeded)
                    {
                        var errors = string.Join(" | ", roleRes.Errors.Select(e => e.Description));
                        throw new InvalidOperationException($"Erro ao atribuir role '{role}' ao utilizador '{username}': {errors}");
                    }
                }
            }
        }
    }
}
