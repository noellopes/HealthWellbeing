using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthWellbeing.Data
{
    internal class SeedDataGinasio
    {
        internal static void Populate(HealthWellbeingDbContext? dbContext)
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));

            // dbContext.Database.EnsureCreated();
            dbContext.Database.Migrate();

            // 1. DADOS BASE
            var clients = PopulateClients(dbContext);
            var plans = PopulatePlan(dbContext);
            var trainers = PopulateTrainer(dbContext);
            var trainingTypes = PopulateTrainingType(dbContext);
            var exercises = PopulateExercise(dbContext);

            // 2. MEMBROS
            var members = PopulateMember(dbContext, clients);

            // 3. AULAS (TRAININGS)
            List<Training> trainings = new List<Training>();
            if (trainers.Any())
            {
                trainings = PopulateTraining(dbContext, trainers, trainingTypes);

                // Associa Planos a Aulas
                PopulateTrainingPlan(dbContext, plans, trainings);

                // CORREÇÃO AQUI: Associa Exercícios a Aulas (com RestTime)
                PopulateTrainingExercise(dbContext, trainings, exercises);
            }

            // 4. ASSOCIAÇÕES FINAIS
            PopulateMemberPlan(dbContext, members, plans);
            PopulatePhysicalAssessment(dbContext, members, trainers);

            // 5. SESSÕES
            PopulateSession(dbContext, members, trainings);
        }

        // --- MÉTODOS DE POPULAÇÃO ---

        private static List<Client> PopulateClients(HealthWellbeingDbContext db)
        {
            if (db.Client.Any()) return db.Client.ToList();
            var clients = new List<Client> {
                new Client { Name="Alice Wonderland", Email="alice.w@example.com", Phone="912345678", Address="London", BirthDate=new DateTime(1990,5,15), Gender="Female", RegistrationDate=DateTime.Now.AddDays(-60) },
                new Client { Name="Bob The Builder", Email="bob.builder@work.net", Phone="919876543", Address="Site 5A", BirthDate=new DateTime(1985,10,20), Gender="Male", RegistrationDate=DateTime.Now.AddDays(-60) },
                new Client { Name="Tiago Silva", Email="tiago.silva@email.pt", Phone="912345111", Address="Rua da Liberdade, 10, Guarda", BirthDate=new DateTime(1995,3,12), Gender="Male", RegistrationDate=DateTime.Now.AddDays(-50) },
                new Client { Name="Mariana Costa", Email="mariana.costa@test.com", Phone="961234222", Address="Av. dos Aliados, 45, Porto", BirthDate=new DateTime(1998,7,22), Gender="Female", RegistrationDate=DateTime.Now.AddDays(-48) },
                new Client { Name="Pedro Santos", Email="pedro.santos@work.net", Phone="934567333", Address="Largo da Sé, 5, Viseu", BirthDate=new DateTime(1989,1,15), Gender="Male", RegistrationDate=DateTime.Now.AddDays(-45) },
                new Client { Name="Catarina Ferreira", Email="catarina.f@gym.pt", Phone="921234444", Address="Rua do Comércio, 12, Guarda", BirthDate=new DateTime(2000,5,30), Gender="Female", RegistrationDate=DateTime.Now.AddDays(-40) },
                new Client { Name="João Mendes", Email="joao.mendes@email.com", Phone="912345555", Address="Estrada Nacional 1, Coimbra", BirthDate=new DateTime(1982,11,10), Gender="Male", RegistrationDate=DateTime.Now.AddDays(-38) },
                new Client { Name="Sofia Rodrigues", Email="sofia.rodrigues@test.pt", Phone="965432666", Address="Praceta das Flores, Lisboa", BirthDate=new DateTime(1992,9,05), Gender="Female", RegistrationDate=DateTime.Now.AddDays(-35) },
                new Client { Name="Ricardo Almeida", Email="ricardo.a@sapo.pt", Phone="934567777", Address="Rua Direita, Aveiro", BirthDate=new DateTime(1990,4,18), Gender="Male", RegistrationDate=DateTime.Now.AddDays(-30) },
                new Client { Name="Beatriz Gomes", Email="beatriz.gomes@gmail.com", Phone="918765888", Address="Rua 25 de Abril, Guarda", BirthDate=new DateTime(2001,2,14), Gender="Female", RegistrationDate=DateTime.Now.AddDays(-28) },
                new Client { Name="André Martins", Email="andre.martins@outlook.com", Phone="922345999", Address="Rua da Estação, Covilhã", BirthDate=new DateTime(1987,6,25), Gender="Male", RegistrationDate=DateTime.Now.AddDays(-25) },
                new Client { Name="Inês Pereira", Email="ines.pereira@yahoo.com", Phone="961234000", Address="Av. Brasil, Figueira da Foz", BirthDate=new DateTime(1996,12,01), Gender="Female", RegistrationDate=DateTime.Now.AddDays(-22) },
                new Client { Name="Gonçalo Oliveira", Email="goncalo.oli@email.pt", Phone="919876111", Address="Rua do Castelo, Guimarães", BirthDate=new DateTime(1993,8,20), Gender="Male", RegistrationDate=DateTime.Now.AddDays(-20) },
                new Client { Name="Rita Sousa", Email="rita.sousa@gym.com", Phone="934567222", Address="Bairro do Liceu, Évora", BirthDate=new DateTime(1999,3,10), Gender="Female", RegistrationDate=DateTime.Now.AddDays(-18) },
                new Client { Name="Miguel Fernandes", Email="miguel.fernandes@work.pt", Phone="921234333", Address="Rua de Baixo, Braga", BirthDate=new DateTime(1985,5,05), Gender="Male", RegistrationDate=DateTime.Now.AddDays(-15) },
                new Client { Name="Diana Lopes", Email="diana.lopes@sapo.pt", Phone="967890444", Address="Av. da República, Faro", BirthDate=new DateTime(1994,10,12), Gender="Female", RegistrationDate=DateTime.Now.AddDays(-14) },
                new Client { Name="Vasco Ramos", Email="vasco.ramos@gmail.com", Phone="912345555", Address="Rua da Misericórdia, Guarda", BirthDate=new DateTime(1980,1,30), Gender="Male", RegistrationDate=DateTime.Now.AddDays(-12) },
                new Client { Name="Cláudia Pinto", Email="claudia.pinto@hotmail.com", Phone="934567666", Address="Rua dos Pescadores, Setúbal", BirthDate=new DateTime(1997,7,15), Gender="Female", RegistrationDate=DateTime.Now.AddDays(-10) },
                new Client { Name="Hugo Lima", Email="hugo.lima@email.com", Phone="921234777", Address="Urbanização Quinta Nova, Leiria", BirthDate=new DateTime(1988,4,22), Gender="Male", RegistrationDate=DateTime.Now.AddDays(-9) },
                new Client { Name="Patrícia Vieira", Email="patricia.vieira@test.com", Phone="961234888", Address="Rua das Oliveiras, Castelo Branco", BirthDate=new DateTime(1991,11,08), Gender="Female", RegistrationDate=DateTime.Now.AddDays(-8) },
                new Client { Name="Bruno Carvalho", Email="bruno.carvalho@sapo.pt", Phone="918765999", Address="Rua Principal, Seia", BirthDate=new DateTime(1995,6,18), Gender="Male", RegistrationDate=DateTime.Now.AddDays(-7) },
                new Client { Name="Daniela Ribeiro", Email="daniela.ribeiro@gmail.com", Phone="934567000", Address="Largo do Pelourinho, Bragança", BirthDate=new DateTime(2002,2,28), Gender="Female", RegistrationDate=DateTime.Now.AddDays(-6) },
                new Client { Name="Fábio Antunes", Email="fabio.antunes@outlook.com", Phone="921234123", Address="Rua da Fonte, Viana do Castelo", BirthDate=new DateTime(1990,9,14), Gender="Male", RegistrationDate=DateTime.Now.AddDays(-5) },
                new Client { Name="Helena Marques", Email="helena.marques@yahoo.com", Phone="961234234", Address="Av. 5 de Outubro, Lisboa", BirthDate=new DateTime(1986,12,20), Gender="Female", RegistrationDate=DateTime.Now.AddDays(-4) },
                new Client { Name="Nuno Correia", Email="nuno.correia@email.pt", Phone="919876345", Address="Rua do Sol, Portimão", BirthDate=new DateTime(1983,3,25), Gender="Male", RegistrationDate=DateTime.Now.AddDays(-3) },
                new Client { Name="Joana Batista", Email="joana.batista@gym.pt", Phone="934567456", Address="Rua das Flores, Santarém", BirthDate=new DateTime(1998,8,05), Gender="Female", RegistrationDate=DateTime.Now.AddDays(-3) },
                new Client { Name="Mário Neves", Email="mario.neves@work.net", Phone="921234567", Address="Bairro Novo, Guarda", BirthDate=new DateTime(1992,5,10), Gender="Male", RegistrationDate=DateTime.Now.AddDays(-2) },
                new Client { Name="Teresa Cruz", Email="teresa.cruz@sapo.pt", Phone="967890678", Address="Rua do Porto, Vila Real", BirthDate=new DateTime(1981,1,12), Gender="Female", RegistrationDate=DateTime.Now.AddDays(-2) },
                new Client { Name="Luís Pires", Email="luis.pires@gmail.com", Phone="912345789", Address="Av. Central, Chaves", BirthDate=new DateTime(1994,7,30), Gender="Male", RegistrationDate=DateTime.Now.AddDays(-1) },
                new Client { Name="Marta Jesus", Email="marta.jesus@hotmail.com", Phone="934567890", Address="Rua da Praia, Peniche", BirthDate=new DateTime(1999,10,02), Gender="Female", RegistrationDate=DateTime.Now.AddDays(-1) },
                new Client { Name="Sérgio Mendes", Email="sergio.mendes@email.com", Phone="921234901", Address="Rua do Mercado, Tomar", BirthDate=new DateTime(1989,4,15), Gender="Male", RegistrationDate=DateTime.Now },
                new Client { Name="Ana Luísa", Email="ana.luisa@test.pt", Phone="961234012", Address="Largo da Igreja, Guarda", BirthDate=new DateTime(2003,6,20), Gender="Female", RegistrationDate=DateTime.Now }
            };
            db.Client.AddRange(clients); db.SaveChanges(); return clients;
        }

        private static List<Member> PopulateMember(HealthWellbeingDbContext db, List<Client> clients)
        {
            if (db.Member.Any()) return db.Member.ToList();
            var members = new List<Member>();
            foreach (var c in clients.Take(15)) members.Add(new Member { ClientId = c.ClientId });
            db.Member.AddRange(members); db.SaveChanges(); return members;
        }

        private static List<Trainer> PopulateTrainer(HealthWellbeingDbContext dbContext)
        {
            // Verifica se já existem dados para não duplicar
            if (dbContext.Trainer.Any()) return dbContext.Trainer.ToList();

            var trainers = new List<Trainer>
            {
                // 2 Originais do teu código
                new Trainer { Name = "John Smith", Email = "john@gym.com", Phone = "555-111", Gender = "Male", Address = "Gym St 1", BirthDate = new DateTime(1985,5,20), IsActive = true },
                new Trainer { Name = "Emma Johnson", Email = "emma@gym.com", Phone = "555-222", Gender = "Female", Address = "Fit Ave 2", BirthDate = new DateTime(1990,8,15), IsActive = true },

                // 2 Treinadores retirados da Tabela 17 do Relatório (Página 45)
                new Trainer { Name = "Pedro Costa", Email = "pedro.costa@ginasio.pt", Phone = "912345768", Gender = "Male", Address = "Rua do Treino, 12, Guarda", BirthDate = new DateTime(1985,03,15), IsActive = true },
                new Trainer { Name = "Miguel Ferreira", Email = "miguel.ferreira@ginasio.pt", Phone = "962345178", Gender = "Male", Address = "Av. do Desporto, 8, Guarda", BirthDate = new DateTime(1990,07,22), IsActive = true },

                // 13 Novos Treinadores (Contexto Português/Guarda)
                new Trainer { Name = "Sara Matos", Email = "sara.matos@healthwellbeing.pt", Phone = "910000001", Gender = "Female", Address = "Rua da Sé, 5, Guarda", BirthDate = new DateTime(1992,11,10), IsActive = true },
                new Trainer { Name = "Rui Patrício", Email = "rui.patricio@healthwellbeing.pt", Phone = "930000002", Gender = "Male", Address = "Av. Monsenhor Mendes, Guarda", BirthDate = new DateTime(1988,04,25), IsActive = true },
                new Trainer { Name = "Ana Gomes", Email = "ana.gomes@healthwellbeing.pt", Phone = "960000003", Gender = "Female", Address = "Bairro da Luz, Guarda", BirthDate = new DateTime(1995,09,05), IsActive = true },
                new Trainer { Name = "Carlos Silva", Email = "carlos.silva@healthwellbeing.pt", Phone = "920000004", Gender = "Male", Address = "Rua 25 de Abril, Seia", BirthDate = new DateTime(1983,01,30), IsActive = true },
                new Trainer { Name = "Inês Rocha", Email = "ines.rocha@healthwellbeing.pt", Phone = "910000005", Gender = "Female", Address = "Praceta das Flores, Covilhã", BirthDate = new DateTime(1998,06,12), IsActive = true },
                new Trainer { Name = "Diogo Alves", Email = "diogo.alves@healthwellbeing.pt", Phone = "930000006", Gender = "Male", Address = "Rua Direita, Viseu", BirthDate = new DateTime(1991,02,18), IsActive = true },
                new Trainer { Name = "Lara Mendes", Email = "lara.mendes@healthwellbeing.pt", Phone = "960000007", Gender = "Female", Address = "Av. Cidade de Salamanca, Guarda", BirthDate = new DateTime(1994,12,22), IsActive = true },
                new Trainer { Name = "Vítor Hugo", Email = "vitor.hugo@healthwellbeing.pt", Phone = "920000008", Gender = "Male", Address = "Rua do Comércio, Guarda", BirthDate = new DateTime(1986,08,14), IsActive = true },
                new Trainer { Name = "Cátia Abreu", Email = "catia.abreu@healthwellbeing.pt", Phone = "910000009", Gender = "Female", Address = "Rua dos Bombeiros, Pinhel", BirthDate = new DateTime(1993,03,08), IsActive = true },
                new Trainer { Name = "Jorge Jesus", Email = "jorge.jesus@healthwellbeing.pt", Phone = "930000010", Gender = "Male", Address = "Largo do Paço, Celorico", BirthDate = new DateTime(1979,07,17), IsActive = true },
                new Trainer { Name = "Mónica Belchior", Email = "monica.b@healthwellbeing.pt", Phone = "960000011", Gender = "Female", Address = "Rua da Estação, Guarda", BirthDate = new DateTime(1996,10,30), IsActive = true },
                new Trainer { Name = "Filipe Luis", Email = "filipe.luis@healthwellbeing.pt", Phone = "920000012", Gender = "Male", Address = "Urb. Quinta Nova, Guarda", BirthDate = new DateTime(1990,05,20), IsActive = true },
                new Trainer { Name = "Beatriz Costa", Email = "beatriz.costa@healthwellbeing.pt", Phone = "910000013", Gender = "Female", Address = "Rua da Torre, Guarda", BirthDate = new DateTime(1997,01,15), IsActive = true }
            };

            dbContext.Trainer.AddRange(trainers);
            dbContext.SaveChanges();

            return trainers;
        }

        private static List<Plan> PopulatePlan(HealthWellbeingDbContext db)
        {
            // Check if plans already exist to avoid duplicates
            if (db.Plan.Any()) return db.Plan.ToList();

            var plans = new List<Plan>
            {
                // 1. Monthly Plans (Based on Report Table 15)
                new Plan { Name = "Basic Monthly", Price = 29.99m, Description = "Access to gym floor and locker rooms. No loyalty period.", DurationDays = 30 },
                new Plan { Name = "Premium Monthly", Price = 49.99m, Description = "Full access, group classes and towel service included.", DurationDays = 30 },

                // 2. Demographic Specific Plans
                new Plan { Name = "Student", Price = 19.99m, Description = "Discounted access for students (Under 25) with valid ID.", DurationDays = 30 },
                new Plan { Name = "Senior (+65)", Price = 24.99m, Description = "Adapted plan for seniors with monitored assistance.", DurationDays = 30 },
                new Plan { Name = "Off-Peak", Price = 22.50m, Description = "Limited access during off-peak hours (09:00 - 16:00).", DurationDays = 30 },

                // 3. Short Term Plans
                new Plan { Name = "Day Pass", Price = 9.99m, Description = "Single entry valid for 1 day only.", DurationDays = 1 },
                new Plan { Name = "Trial Week", Price = 24.99m, Description = "Full access for 7 consecutive days to try the facilities.", DurationDays = 7 },

                // 4. Quarterly Plans (Short Loyalty)
                new Plan { Name = "Basic Quarterly", Price = 79.99m, Description = "Billed every 3 months. Saves on monthly fees.", DurationDays = 90 },
                new Plan { Name = "Premium Quarterly", Price = 135.00m, Description = "Full quarterly access including physical assessment.", DurationDays = 90 },

                // 5. Long Term Plans (Long Loyalty)
                new Plan { Name = "Semi-Annual Saver", Price = 149.99m, Description = "6-month commitment with reduced monthly rate.", DurationDays = 180 },
                new Plan { Name = "Annual Basic", Price = 299.99m, Description = "One-time annual payment. Equivalent to 25/month.", DurationDays = 365 },
                new Plan { Name = "Annual VIP Total", Price = 499.99m, Description = "Unlimited annual access, includes 1 PT session/month and nutrition.", DurationDays = 365 }
            };

            db.Plan.AddRange(plans);
            db.SaveChanges();

            return plans;
        }

        private static List<TrainingType> PopulateTrainingType(HealthWellbeingDbContext db)
        {
            // Check if training types already exist
            if (db.TrainingType.Any()) return db.TrainingType.ToList();

            var types = new List<TrainingType>
            {
                // 1. Requested "Sessão Personalizada" (Personalized Session)
                // MaxParticipants = 1 ensures it is a private, 1-on-1 session
                new TrainingType { Name = "Personal Training", Description = "One-on-one personalized session tailored to member goals.", MaxParticipants = 1 },

                // 2. Types explicitly defined in Report Table 18 (Page 46)
                new TrainingType { Name = "Functional", Description = "Full body functional training.", MaxParticipants = 15 },
                new TrainingType { Name = "Hypertrophy", Description = "Training focused on increasing muscle mass.", MaxParticipants = 10 },
                new TrainingType { Name = "Cardio", Description = "Cardiovascular resistance training.", MaxParticipants = 20 },

                // 3. Types mentioned in Test Cases (Page 59 - TC-03 & TC-04)
                new TrainingType { Name = "CrossFit", Description = "High-intensity functional movements and strength.", MaxParticipants = 15 },
                new TrainingType { Name = "Pilates", Description = "Low-impact flexibility, muscular strength, and endurance.", MaxParticipants = 20 },

                // 4. Other common gym classes
                new TrainingType { Name = "Yoga", Description = "Flexibility, balance and breathing exercises.", MaxParticipants = 15 },
                new TrainingType { Name = "HIIT", Description = "High Intensity Interval Training.", MaxParticipants = 20 },
                new TrainingType { Name = "Spinning", Description = "Indoor cycling focusing on endurance and strength.", MaxParticipants = 25 },
                new TrainingType { Name = "Boxing", Description = "Combat sport training focusing on technique and conditioning.", MaxParticipants = 12 },
                new TrainingType { Name = "Zumba", Description = "Aerobic fitness program featuring movements inspired by Latin American dance.", MaxParticipants = 30 },
                new TrainingType { Name = "Mobility", Description = "Exercises to improve range of motion and joint health.", MaxParticipants = 15 }
            };

            db.TrainingType.AddRange(types);
            db.SaveChanges();

            return types;
        }

        private static List<Exercise> PopulateExercise(HealthWellbeingDbContext db)
        {
            // Verifica se já existem exercícios para evitar duplicados
            if (db.Exercise.Any()) return db.Exercise.ToList();

            var exs = new List<Exercise>
            {
                // --- LEGS (Pernas) ---
                new Exercise { Name = "Barbell Squat", Description = "Compound exercise targeting quadriceps and glutes.", MuscleGroup = "Legs", Equipment = "Barbell" },
                new Exercise { Name = "Leg Press", Description = "Machine-based exercise for pushing weight away from the body using legs.", MuscleGroup = "Legs", Equipment = "Machine" },
                new Exercise { Name = "Lunges", Description = "Unilateral leg exercise for balance and strength.", MuscleGroup = "Legs", Equipment = "Dumbbells" },
                new Exercise { Name = "Deadlift", Description = "Compound lift targeting the posterior chain.", MuscleGroup = "Legs/Back", Equipment = "Barbell" },
                new Exercise { Name = "Leg Extension", Description = "Isolation exercise for the quadriceps.", MuscleGroup = "Legs", Equipment = "Machine" },
                new Exercise { Name = "Calf Raises", Description = "Isolation movement for the lower leg muscles.", MuscleGroup = "Legs", Equipment = "Machine" },

                // --- CHEST (Peito) ---
                new Exercise { Name = "Bench Press", Description = "Primary compound movement for chest development.", MuscleGroup = "Chest", Equipment = "Barbell" },
                new Exercise { Name = "Push-Up", Description = "Bodyweight exercise for chest and triceps.", MuscleGroup = "Chest", Equipment = "None" },
                new Exercise { Name = "Incline Dumbbell Press", Description = "Upper chest targeting press.", MuscleGroup = "Chest", Equipment = "Dumbbells" },
                new Exercise { Name = "Chest Fly", Description = "Isolation movement for the pectorals.", MuscleGroup = "Chest", Equipment = "Machine/Cables" },
                new Exercise { Name = "Dips", Description = "Bodyweight push exercise for chest and triceps.", MuscleGroup = "Chest", Equipment = "Parallel Bars" },

                // --- BACK (Costas) ---
                new Exercise { Name = "Pull-Up", Description = "Vertical pulling movement for back width.", MuscleGroup = "Back", Equipment = "Pull-up Bar" },
                new Exercise { Name = "Lat Pulldown", Description = "Machine alternative to pull-ups.", MuscleGroup = "Back", Equipment = "Cable Machine" },
                new Exercise { Name = "Seated Row", Description = "Horizontal pulling for back thickness.", MuscleGroup = "Back", Equipment = "Cable Machine" },
                new Exercise { Name = "Bent Over Row", Description = "Compound rowing movement.", MuscleGroup = "Back", Equipment = "Barbell" },
                new Exercise { Name = "Back Extension", Description = "Lower back strengthening exercise.", MuscleGroup = "Back", Equipment = "Hyperextension Bench" },

                // --- SHOULDERS (Ombros) ---
                new Exercise { Name = "Overhead Press", Description = "Vertical press for overall shoulder strength.", MuscleGroup = "Shoulders", Equipment = "Barbell" },
                new Exercise { Name = "Lateral Raise", Description = "Isolation for the side deltoids.", MuscleGroup = "Shoulders", Equipment = "Dumbbells" },
                new Exercise { Name = "Front Raise", Description = "Isolation for the front deltoids.", MuscleGroup = "Shoulders", Equipment = "Dumbbells" },
                new Exercise { Name = "Face Pull", Description = "Rear deltoid and rotator cuff exercise.", MuscleGroup = "Shoulders", Equipment = "Cable Machine" },

                // --- ARMS (Braços) ---
                new Exercise { Name = "Bicep Curl", Description = "Classic isolation for biceps.", MuscleGroup = "Arms", Equipment = "Barbell" },
                new Exercise { Name = "Hammer Curl", Description = "Bicep curl variation targeting the brachialis.", MuscleGroup = "Arms", Equipment = "Dumbbells" },
                new Exercise { Name = "Tricep Pushdown", Description = "Isolation for the back of the arm.", MuscleGroup = "Arms", Equipment = "Cable Machine" },
                new Exercise { Name = "Skullcrushers", Description = "Tricep extension performed lying down.", MuscleGroup = "Arms", Equipment = "EZ Bar" },

                // --- CORE (Abdominais) ---
                new Exercise { Name = "Plank", Description = "Isometric core stability hold.", MuscleGroup = "Core", Equipment = "None" },
                new Exercise { Name = "Russian Twist", Description = "Rotational movement for obliques.", MuscleGroup = "Core", Equipment = "Medicine Ball" },
                new Exercise { Name = "Hanging Leg Raise", Description = "Advanced core movement targeting lower abs.", MuscleGroup = "Core", Equipment = "Pull-up Bar" },
                new Exercise { Name = "Bicycle Crunches", Description = "Dynamic abdominal exercise.", MuscleGroup = "Core", Equipment = "None" },

                // --- CARDIO / FUNCTIONAL (Funcional) ---
                new Exercise { Name = "Burpees", Description = "Full body explosive movement.", MuscleGroup = "Full Body", Equipment = "None" },
                new Exercise { Name = "Box Jump", Description = "Plyometric leg exercise.", MuscleGroup = "Legs", Equipment = "Plyo Box" },
                new Exercise { Name = "Kettlebell Swing", Description = "Hip hinge movement for power.", MuscleGroup = "Full Body", Equipment = "Kettlebell" }
            };

            db.Exercise.AddRange(exs);
            db.SaveChanges();

            return exs;
        }

        private static List<Training> PopulateTraining(HealthWellbeingDbContext db, List<Trainer> trainers, List<TrainingType> types)
        {
            // Verifica se já existem treinos
            if (db.Training.Any()) return db.Training.ToList();

            // Helpers para encontrar tipos específicos (baseado no que criámos antes)
            // Se não encontrar pelo nome, usa um índice default para evitar erros
            var typeYoga = types.FirstOrDefault(t => t.Name == "Yoga") ?? types[0];
            var typeHiit = types.FirstOrDefault(t => t.Name == "HIIT") ?? types[1];
            var typePilates = types.FirstOrDefault(t => t.Name == "Pilates") ?? types[0];
            var typeCrossFit = types.FirstOrDefault(t => t.Name == "CrossFit") ?? types[1];
            var typeFunctional = types.FirstOrDefault(t => t.Name == "Functional") ?? types[0];
            var typeHypertrophy = types.FirstOrDefault(t => t.Name == "Hypertrophy") ?? types[1];
            var typeCardio = types.FirstOrDefault(t => t.Name == "Cardio") ?? types[0];
            var typeSpinning = types.FirstOrDefault(t => t.Name == "Spinning") ?? types[1];
            var typeZumba = types.FirstOrDefault(t => t.Name == "Zumba") ?? types[0];
            var typeBox = types.FirstOrDefault(t => t.Name == "Boxing") ?? types[1];
            var typePT = types.FirstOrDefault(t => t.Name == "Personal Training") ?? types[0];

            // Helper para garantir que não rebentamos o índice da lista de treinadores
            Trainer GetTrainer(int index) => trainers[index % trainers.Count];

            var trs = new List<Training>
            {
                // --- MONDAY (Segunda-feira) ---
                // Exemplo explícito da Tabela 19 (Pág 46): Funcional à Segunda
                new Training { Name = "Functional Power", DayOfWeek = WeekDay.Monday, StartTime = new TimeSpan(07, 0, 0), Duration = 60, TrainerId = GetTrainer(0).TrainerId, TrainingTypeId = typeFunctional.TrainingTypeId },
                new Training { Name = "Morning Yoga", DayOfWeek = WeekDay.Monday, StartTime = new TimeSpan(08, 0, 0), Duration = 60, TrainerId = GetTrainer(1).TrainerId, TrainingTypeId = typeYoga.TrainingTypeId },
                new Training { Name = "Lunch Express HIIT", DayOfWeek = WeekDay.Monday, StartTime = new TimeSpan(12, 30, 0), Duration = 30, TrainerId = GetTrainer(2).TrainerId, TrainingTypeId = typeHiit.TrainingTypeId },
                new Training { Name = "CrossFit WOD", DayOfWeek = WeekDay.Monday, StartTime = new TimeSpan(18, 0, 0), Duration = 60, TrainerId = GetTrainer(3).TrainerId, TrainingTypeId = typeCrossFit.TrainingTypeId },
                new Training { Name = "Zumba Party", DayOfWeek = WeekDay.Monday, StartTime = new TimeSpan(19, 0, 0), Duration = 60, TrainerId = GetTrainer(4).TrainerId, TrainingTypeId = typeZumba.TrainingTypeId },

                // --- TUESDAY (Terça-feira) ---
                new Training { Name = "Sunrise Pilates", DayOfWeek = WeekDay.Tuesday, StartTime = new TimeSpan(07, 30, 0), Duration = 45, TrainerId = GetTrainer(5).TrainerId, TrainingTypeId = typePilates.TrainingTypeId },
                new Training { Name = "Spinning Hills", DayOfWeek = WeekDay.Tuesday, StartTime = new TimeSpan(09, 0, 0), Duration = 50, TrainerId = GetTrainer(6).TrainerId, TrainingTypeId = typeSpinning.TrainingTypeId },
                new Training { Name = "PT Session (John)", DayOfWeek = WeekDay.Tuesday, StartTime = new TimeSpan(10, 0, 0), Duration = 60, TrainerId = GetTrainer(0).TrainerId, TrainingTypeId = typePT.TrainingTypeId },
                new Training { Name = "Boxing Technique", DayOfWeek = WeekDay.Tuesday, StartTime = new TimeSpan(18, 0, 0), Duration = 60, TrainerId = GetTrainer(7).TrainerId, TrainingTypeId = typeBox.TrainingTypeId },
                new Training { Name = "Cardio Blast", DayOfWeek = WeekDay.Tuesday, StartTime = new TimeSpan(19, 0, 0), Duration = 45, TrainerId = GetTrainer(8).TrainerId, TrainingTypeId = typeCardio.TrainingTypeId },

                // --- WEDNESDAY (Quarta-feira) ---
                // Exemplo explícito da Tabela 19 (Pág 46): Hipertrofia à Quarta
                new Training { Name = "Hypertrophy Chest/Back", DayOfWeek = WeekDay.Wednesday, StartTime = new TimeSpan(18, 0, 0), Duration = 75, TrainerId = GetTrainer(0).TrainerId, TrainingTypeId = typeHypertrophy.TrainingTypeId },
                new Training { Name = "Early Bird Yoga", DayOfWeek = WeekDay.Wednesday, StartTime = new TimeSpan(06, 30, 0), Duration = 60, TrainerId = GetTrainer(1).TrainerId, TrainingTypeId = typeYoga.TrainingTypeId },
                new Training { Name = "Core & Abs", DayOfWeek = WeekDay.Wednesday, StartTime = new TimeSpan(12, 30, 0), Duration = 30, TrainerId = GetTrainer(2).TrainerId, TrainingTypeId = typeFunctional.TrainingTypeId },
                new Training { Name = "PT Session (Emma)", DayOfWeek = WeekDay.Wednesday, StartTime = new TimeSpan(14, 0, 0), Duration = 60, TrainerId = GetTrainer(1).TrainerId, TrainingTypeId = typePT.TrainingTypeId },
                new Training { Name = "Evening Spin", DayOfWeek = WeekDay.Wednesday, StartTime = new TimeSpan(19, 30, 0), Duration = 45, TrainerId = GetTrainer(6).TrainerId, TrainingTypeId = typeSpinning.TrainingTypeId },

                // --- THURSDAY (Quinta-feira) ---
                new Training { Name = "Mobility Flow", DayOfWeek = WeekDay.Thursday, StartTime = new TimeSpan(08, 0, 0), Duration = 45, TrainerId = GetTrainer(9).TrainerId, TrainingTypeId = typeYoga.TrainingTypeId },
                new Training { Name = "CrossFit Strength", DayOfWeek = WeekDay.Thursday, StartTime = new TimeSpan(09, 0, 0), Duration = 60, TrainerId = GetTrainer(3).TrainerId, TrainingTypeId = typeCrossFit.TrainingTypeId },
                new Training { Name = "Pilates Mat", DayOfWeek = WeekDay.Thursday, StartTime = new TimeSpan(17, 30, 0), Duration = 45, TrainerId = GetTrainer(5).TrainerId, TrainingTypeId = typePilates.TrainingTypeId },
                new Training { Name = "HIIT Burn", DayOfWeek = WeekDay.Thursday, StartTime = new TimeSpan(18, 30, 0), Duration = 45, TrainerId = GetTrainer(2).TrainerId, TrainingTypeId = typeHiit.TrainingTypeId },
                new Training { Name = "BodyPump", DayOfWeek = WeekDay.Thursday, StartTime = new TimeSpan(19, 30, 0), Duration = 60, TrainerId = GetTrainer(10).TrainerId, TrainingTypeId = typeFunctional.TrainingTypeId },

                // --- FRIDAY (Sexta-feira) ---
                new Training { Name = "Functional Circuit", DayOfWeek = WeekDay.Friday, StartTime = new TimeSpan(07, 0, 0), Duration = 50, TrainerId = GetTrainer(11).TrainerId, TrainingTypeId = typeFunctional.TrainingTypeId },
                new Training { Name = "Hypertrophy Legs", DayOfWeek = WeekDay.Friday, StartTime = new TimeSpan(10, 0, 0), Duration = 60, TrainerId = GetTrainer(0).TrainerId, TrainingTypeId = typeHypertrophy.TrainingTypeId },
                new Training { Name = "Lunch Boxing", DayOfWeek = WeekDay.Friday, StartTime = new TimeSpan(12, 30, 0), Duration = 45, TrainerId = GetTrainer(7).TrainerId, TrainingTypeId = typeBox.TrainingTypeId },
                new Training { Name = "Happy Hour Zumba", DayOfWeek = WeekDay.Friday, StartTime = new TimeSpan(18, 0, 0), Duration = 60, TrainerId = GetTrainer(4).TrainerId, TrainingTypeId = typeZumba.TrainingTypeId },
                new Training { Name = "Relax Yoga", DayOfWeek = WeekDay.Friday, StartTime = new TimeSpan(19, 15, 0), Duration = 60, TrainerId = GetTrainer(1).TrainerId, TrainingTypeId = typeYoga.TrainingTypeId },

                // --- SATURDAY (Sábado) ---
                new Training { Name = "Weekend Warrior Bootcamp", DayOfWeek = WeekDay.Saturday, StartTime = new TimeSpan(09, 30, 0), Duration = 90, TrainerId = GetTrainer(3).TrainerId, TrainingTypeId = typeFunctional.TrainingTypeId },
                new Training { Name = "Saturday Spin", DayOfWeek = WeekDay.Saturday, StartTime = new TimeSpan(10, 0, 0), Duration = 60, TrainerId = GetTrainer(6).TrainerId, TrainingTypeId = typeSpinning.TrainingTypeId },
                new Training { Name = "Intro to Pilates", DayOfWeek = WeekDay.Saturday, StartTime = new TimeSpan(11, 0, 0), Duration = 60, TrainerId = GetTrainer(5).TrainerId, TrainingTypeId = typePilates.TrainingTypeId },
                new Training { Name = "PT Session (Weekend)", DayOfWeek = WeekDay.Saturday, StartTime = new TimeSpan(11, 30, 0), Duration = 60, TrainerId = GetTrainer(2).TrainerId, TrainingTypeId = typePT.TrainingTypeId },
                new Training { Name = "Open Gym Supervision", DayOfWeek = WeekDay.Saturday, StartTime = new TimeSpan(14, 0, 0), Duration = 120, TrainerId = GetTrainer(10).TrainerId, TrainingTypeId = typeHypertrophy.TrainingTypeId }
            };

            db.Training.AddRange(trs);
            db.SaveChanges();

            return trs;
        }

        private static void PopulateTrainingPlan(HealthWellbeingDbContext db, List<Plan> ps, List<Training> ts)
        {
            // Verifica se já existem dados ou se as listas de dependência estão vazias
            if (db.TrainingPlan.Any() || !ps.Any() || !ts.Any()) return;

            // Helpers locais para encontrar IDs sem "hardcoding" (torna o seed mais seguro)
            // Se não encontrar o nome exato, usa o primeiro da lista como fallback
            Plan P(string name) => ps.FirstOrDefault(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase)) ?? ps.First();
            Training T(string name) => ts.FirstOrDefault(t => t.Name.Contains(name, StringComparison.OrdinalIgnoreCase)) ?? ts.First();

            var plans = new List<TrainingPlan>
            {
                // --- 1. Basic Monthly (Foco em aulas gerais) ---
                new TrainingPlan { PlanId = P("Basic Monthly").PlanId, TrainingId = T("Morning Yoga").TrainingId, DaysPerWeek = 2 },
                new TrainingPlan { PlanId = P("Basic Monthly").PlanId, TrainingId = T("Functional Power").TrainingId, DaysPerWeek = 3 },
                new TrainingPlan { PlanId = P("Basic Monthly").PlanId, TrainingId = T("Zumba").TrainingId, DaysPerWeek = 1 },

                // --- 2. Premium Monthly (Acesso a aulas intensas e PT) ---
                new TrainingPlan { PlanId = P("Premium").PlanId, TrainingId = T("CrossFit").TrainingId, DaysPerWeek = 3 },
                new TrainingPlan { PlanId = P("Premium").PlanId, TrainingId = T("Hypertrophy").TrainingId, DaysPerWeek = 4 },
                new TrainingPlan { PlanId = P("Premium").PlanId, TrainingId = T("PT Session").TrainingId, DaysPerWeek = 1 },
                new TrainingPlan { PlanId = P("Premium").PlanId, TrainingId = T("Boxing").TrainingId, DaysPerWeek = 2 },

                // --- 3. Student Plan (Aulas rápidas e cardio) ---
                new TrainingPlan { PlanId = P("Student").PlanId, TrainingId = T("HIIT").TrainingId, DaysPerWeek = 3 },
                new TrainingPlan { PlanId = P("Student").PlanId, TrainingId = T("Spinning").TrainingId, DaysPerWeek = 2 },
                new TrainingPlan { PlanId = P("Student").PlanId, TrainingId = T("Core").TrainingId, DaysPerWeek = 2 },

                // --- 4. Senior Plan (Baixo impacto) ---
                new TrainingPlan { PlanId = P("Senior").PlanId, TrainingId = T("Pilates").TrainingId, DaysPerWeek = 2 },
                new TrainingPlan { PlanId = P("Senior").PlanId, TrainingId = T("Mobility").TrainingId, DaysPerWeek = 3 },
                new TrainingPlan { PlanId = P("Senior").PlanId, TrainingId = T("Yoga").TrainingId, DaysPerWeek = 2 },

                // --- 5. Off-Peak (Horários específicos) ---
                new TrainingPlan { PlanId = P("Off-Peak").PlanId, TrainingId = T("Morning Yoga").TrainingId, DaysPerWeek = 3 },
                new TrainingPlan { PlanId = P("Off-Peak").PlanId, TrainingId = T("Lunch Express").TrainingId, DaysPerWeek = 5 },

                // --- 6. Annual VIP (Tudo incluído) ---
                new TrainingPlan { PlanId = P("VIP").PlanId, TrainingId = T("Open Gym").TrainingId, DaysPerWeek = 6 },
                new TrainingPlan { PlanId = P("VIP").PlanId, TrainingId = T("PT Session").TrainingId, DaysPerWeek = 2 },
                new TrainingPlan { PlanId = P("VIP").PlanId, TrainingId = T("BodyPump").TrainingId, DaysPerWeek = 3 },

                // --- 7. Trial Week (Experimentação) ---
                new TrainingPlan { PlanId = P("Trial").PlanId, TrainingId = T("Functional Circuit").TrainingId, DaysPerWeek = 3 },
                new TrainingPlan { PlanId = P("Trial").PlanId, TrainingId = T("Spinning").TrainingId, DaysPerWeek = 1 }
            };

            db.TrainingPlan.AddRange(plans);
            db.SaveChanges();
        }

        private static void PopulateTrainingExercise(HealthWellbeingDbContext db, List<Training> ts, List<Exercise> es)
        {
            // Verifica se já existem registos ou se as listas de dependência estão vazias
            if (db.TrainingExercise.Any() || !ts.Any() || !es.Any()) return;

            // Helpers para encontrar IDs de forma segura (sem hardcoding de IDs numéricos)
            Training GetT(string name) => ts.FirstOrDefault(t => t.Name.Contains(name, StringComparison.OrdinalIgnoreCase)) ?? ts.First();
            Exercise GetE(string name) => es.FirstOrDefault(e => e.Name.Contains(name, StringComparison.OrdinalIgnoreCase)) ?? es.First();

            var trainingExercises = new List<TrainingExercise>
            {
                // --- TREINO DE HIPERTROFIA (Carga alta, repetições médias, descanso longo) ---
                new TrainingExercise { TrainingId = GetT("Hypertrophy Chest").TrainingId, ExerciseId = GetE("Bench Press").ExerciseId, Sets = 4, Reps = 8, RestTime = "90s" },
                new TrainingExercise { TrainingId = GetT("Hypertrophy Chest").TrainingId, ExerciseId = GetE("Incline Dumbbell").ExerciseId, Sets = 3, Reps = 10, RestTime = "90s" },
                new TrainingExercise { TrainingId = GetT("Hypertrophy Chest").TrainingId, ExerciseId = GetE("Seated Row").ExerciseId, Sets = 4, Reps = 10, RestTime = "60s" },
                new TrainingExercise { TrainingId = GetT("Hypertrophy Legs").TrainingId, ExerciseId = GetE("Barbell Squat").ExerciseId, Sets = 5, Reps = 5, RestTime = "120s" }, // Força
                new TrainingExercise { TrainingId = GetT("Hypertrophy Legs").TrainingId, ExerciseId = GetE("Deadlift").ExerciseId, Sets = 3, Reps = 6, RestTime = "120s" },

                // --- TREINO FUNCIONAL / CROSSFIT (Alta intensidade, descanso curto) ---
                new TrainingExercise { TrainingId = GetT("Functional Power").TrainingId, ExerciseId = GetE("Kettlebell Swing").ExerciseId, Sets = 4, Reps = 20, RestTime = "30s" },
                new TrainingExercise { TrainingId = GetT("Functional Power").TrainingId, ExerciseId = GetE("Burpees").ExerciseId, Sets = 3, Reps = 15, RestTime = "30s" },
                new TrainingExercise { TrainingId = GetT("CrossFit WOD").TrainingId, ExerciseId = GetE("Box Jump").ExerciseId, Sets = 5, Reps = 12, RestTime = "45s" },
                new TrainingExercise { TrainingId = GetT("CrossFit WOD").TrainingId, ExerciseId = GetE("Pull-Up").ExerciseId, Sets = 4, Reps = 10, RestTime = "60s" },
                new TrainingExercise { TrainingId = GetT("CrossFit Strength").TrainingId, ExerciseId = GetE("Overhead Press").ExerciseId, Sets = 5, Reps = 5, RestTime = "90s" },

                // --- HIIT / CARDIO (Muita repetição, quase sem descanso) ---
                new TrainingExercise { TrainingId = GetT("HIIT Burn").TrainingId, ExerciseId = GetE("Push-Up").ExerciseId, Sets = 4, Reps = 20, RestTime = "15s" },
                new TrainingExercise { TrainingId = GetT("HIIT Burn").TrainingId, ExerciseId = GetE("Lunges").ExerciseId, Sets = 3, Reps = 20, RestTime = "15s" },
                new TrainingExercise { TrainingId = GetT("HIIT Burn").TrainingId, ExerciseId = GetE("Plank").ExerciseId, Sets = 3, Reps = 1, RestTime = "30s" }, // 1 Rep = Aguentar tempo max

                // --- BODYPUMP (Resistência muscular) ---
                new TrainingExercise { TrainingId = GetT("BodyPump").TrainingId, ExerciseId = GetE("Barbell Squat").ExerciseId, Sets = 3, Reps = 25, RestTime = "30s" }, // Carga leve, muita rep
                new TrainingExercise { TrainingId = GetT("BodyPump").TrainingId, ExerciseId = GetE("Bicep Curl").ExerciseId, Sets = 3, Reps = 20, RestTime = "30s" },
                new TrainingExercise { TrainingId = GetT("BodyPump").TrainingId, ExerciseId = GetE("Tricep Pushdown").ExerciseId, Sets = 3, Reps = 20, RestTime = "30s" },

                // --- CORE & PILATES (Estabilidade) ---
                new TrainingExercise { TrainingId = GetT("Core & Abs").TrainingId, ExerciseId = GetE("Russian Twist").ExerciseId, Sets = 3, Reps = 30, RestTime = "45s" },
                new TrainingExercise { TrainingId = GetT("Core & Abs").TrainingId, ExerciseId = GetE("Hanging Leg Raise").ExerciseId, Sets = 3, Reps = 12, RestTime = "60s" },
                new TrainingExercise { TrainingId = GetT("Pilates Mat").TrainingId, ExerciseId = GetE("Bicycle Crunches").ExerciseId, Sets = 3, Reps = 20, RestTime = "30s" },

                // --- MOBILITY / YOGA (Acessórios) ---
                new TrainingExercise { TrainingId = GetT("Mobility Flow").TrainingId, ExerciseId = GetE("Back Extension").ExerciseId, Sets = 2, Reps = 15, RestTime = "45s" }
            };

            db.TrainingExercise.AddRange(trainingExercises);
            db.SaveChanges();
        }

        private static void PopulateMemberPlan(HealthWellbeingDbContext db, List<Member> ms, List<Plan> ps)
        {
            // Verifica se já existem inscrições ou se faltam dados essenciais
            if (db.MemberPlan.Any() || !ms.Any() || !ps.Any()) return;

            var memberPlans = new List<MemberPlan>();
            var random = new Random();

            // Itera por todos os membros para garantir que todos têm um plano
            for (int i = 0; i < ms.Count; i++)
            {
                var member = ms[i];

                // Distribui planos diferentes pelos membros (ciclicamente)
                var plan = ps[i % ps.Count];

                // Definição de datas e estados para criar variedade de dados para teste
                DateTime startDate;
                string status;

                // Lógica de simulação de estados (baseada no Diagrama de Estados - Pág 47)
                if (plan.Name.Contains("Trial") || plan.Name.Contains("Day Pass"))
                {
                    // Simula planos curtos que já expiraram
                    startDate = DateTime.Now.AddDays(-10); // Começou há 10 dias
                    status = "Expirada"; // Trial de 7 dias ou Day Pass de 1 dia já acabou
                }
                else if (i % 10 == 0)
                {
                    // Simula alguns membros com pagamento pendente
                    startDate = DateTime.Now;
                    status = "Pendente";
                }
                else if (i % 20 == 0)
                {
                    // Simula uma inscrição cancelada
                    startDate = DateTime.Now.AddDays(-30);
                    status = "Cancelada";
                }
                else
                {
                    // A maioria está Ativa
                    startDate = DateTime.Now.AddDays(-random.Next(1, 15)); // Inscreveu-se recentemente
                    status = "Ativa";
                }

                // CÁLCULO DA DATA DE FIM
                // Regra de negócio: EndDate = StartDate + DurationDays do Plano [cite: 533]
                DateTime endDate = startDate.AddDays(plan.DurationDays);

                memberPlans.Add(new MemberPlan
                {
                    MemberId = member.MemberId,
                    PlanId = plan.PlanId,
                    StartDate = startDate,
                    EndDate = endDate,
                    Status = status
                });
            }

            db.MemberPlan.AddRange(memberPlans);
            db.SaveChanges();
        }

        private static void PopulatePhysicalAssessment(HealthWellbeingDbContext db, List<Member> ms, List<Trainer> ts)
        {
            // Verifica se já existem avaliações ou se faltam dados essenciais
            if (db.PhysicalAssessment.Any() || !ms.Any() || !ts.Any()) return;

            var assessments = new List<PhysicalAssessment>();
            var random = new Random();

            // 1. DADOS REAIS DO RELATÓRIO (Tabela 21, Página 46)
            // Caso o membro 1 exista
            if (ms.Count > 0)
            {
                assessments.Add(new PhysicalAssessment
                {
                    MemberId = ms[0].MemberId,
                    TrainerId = ts.First().TrainerId,
                    AssessmentDate = DateTime.Now.AddDays(-60), // Avaliação inicial
                    Weight = 80.0m, // Kg
                    Height = 1.80m, // Metros
                    BodyFatPercentage = 18.0m, // %
                    MuscleMass = 42.5m, // Estimativa baseada no peso
                    Notes = "Cliente iniciante. Objetivo: Hipertrofia." // Mapeado para 'TrainerNotes' no diagrama
                });
            }

            // Caso o membro 2 exista
            if (ms.Count > 1)
            {
                assessments.Add(new PhysicalAssessment
                {
                    MemberId = ms[1].MemberId,
                    TrainerId = ts.Last().TrainerId,
                    AssessmentDate = DateTime.Now.AddDays(-55),
                    Weight = 65.0m,
                    Height = 1.65m,
                    BodyFatPercentage = 22.0m,
                    MuscleMass = 28.0m,
                    Notes = "Boa condição cardiovascular. Foco em tonificação."
                });
            }

            // 2. GERAÇÃO DE DADOS PARA OS RESTANTES MEMBROS
            // Começamos no índice 2 porque os primeiros 2 já foram inseridos manualmente
            for (int i = 2; i < ms.Count; i++)
            {
                // Alterna treinadores
                var trainer = ts[i % ts.Count];

                // Gera altura aleatória realista (entre 1.55m e 1.95m)
                decimal height = random.Next(155, 195) / 100.0m;

                // Gera um IMC alvo aleatório para calcular o peso (entre 20 e 29)
                // Fórmula: Peso = IMC * Altura^2
                double targetBMI = random.Next(20, 29) + random.NextDouble();
                decimal weight = (decimal)((double)targetBMI * Math.Pow((double)height, 2));

                // Arredonda o peso para 1 casa decimal
                weight = Math.Round(weight, 1);

                // Gordura corporal (Homens ~10-20%, Mulheres ~20-30% - simplificado aqui entre 12 e 30)
                decimal bodyFat = random.Next(12, 30);

                // Massa muscular (Simplificação: Peso * 0.4 a 0.5)
                decimal muscleMass = Math.Round(weight * (decimal)(random.Next(40, 50) / 100.0), 1);

                assessments.Add(new PhysicalAssessment
                {
                    MemberId = ms[i].MemberId,
                    TrainerId = trainer.TrainerId,
                    // Datas variadas nos últimos 3 meses
                    AssessmentDate = DateTime.Now.AddDays(-random.Next(1, 90)),
                    Weight = weight,
                    Height = height,
                    BodyFatPercentage = bodyFat,
                    MuscleMass = muscleMass,
                    Notes = "Avaliação de rotina. Evolução positiva."
                });
            }

            db.PhysicalAssessment.AddRange(assessments);
            db.SaveChanges();
        }

        private static void PopulateSession(HealthWellbeingDbContext dbContext, List<Member> members, List<Training> trainings)
        {
            if (dbContext.Session.Any()) return;
            var alice = members.FirstOrDefault(m => m.Client?.Name == "Alice Wonderland");
            var yoga = trainings.FirstOrDefault(t => t.Name.Contains("Yoga"));

            if (alice != null && yoga != null)
            {
                int daysUntil = ((int)DayOfWeek.Monday - (int)DateTime.Today.DayOfWeek + 7) % 7;
                DateTime sessionDate = DateTime.Today.AddDays(daysUntil == 0 ? 7 : daysUntil).Date + yoga.StartTime;

                dbContext.Session.Add(new Session
                {
                    MemberId = alice.MemberId,
                    TrainingId = yoga.TrainingId,
                    SessionDate = sessionDate,
                    MemberFeedback = "Auto-generated booking",
                    Rating = null
                });
                dbContext.SaveChanges();
            }
        }
    }
}