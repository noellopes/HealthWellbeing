using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeingRoom.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthWellBeingRoom.Data
{
    internal class SeedData
    {
        internal static void Populate(HealthWellbeingDbContext? dbContext)
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));

            dbContext.Database.EnsureCreated();

            // 1 - Independentes
            PopulateTypeMaterial(dbContext);
            PopulateManufacturer(dbContext);
            PopulateRoomStatus(dbContext);
            PopulateRoomType(dbContext);
            PopulateRoomLocation(dbContext);
            // 2- sala depende do hasdata de tipo, status e localização
            PopulateRoom(dbContext);
            // 3 - Equipment Infraestrutura
            PolutateEquipmentTypes(dbContext);
            PolutateEquipmentStatus(dbContext);
            PolutateEquipment(dbContext);
            // 4 - Dispositivos e Localização médica
            PopulateMedicalDevices(dbContext);
            PopulateLocalizacaoDispMovel_temporario(dbContext);
            PopulateLocationMedDevices(dbContext);
        }

        // 1 - Independentes
        private static void PopulateTypeMaterial(HealthWellbeingDbContext? dbContext)
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));
            dbContext.Database.EnsureCreated();
            var typeMaterials = new[]
{
                new TypeMaterial
                {
                    Name = "Monitor Multiparamétrico Móvel",
                    Description = "Monitoriza sinais vitais em mobilidade."
                },
                new TypeMaterial
                {
                    Name = "Bomba de Infusão Portátil",
                    Description = "Administra medicamentos de forma móvel."
                },
                new TypeMaterial
                {
                    Name = "Ventilador Portátil",
                    Description = "Auxilia a respiração em transporte."
                },
                new TypeMaterial
                {
                    Name = "Aspirador Cirúrgico Portátil",
                    Description = "Remove secreções e fluidos."
                },
                new TypeMaterial
                {
                    Name = "ECG Portátil",
                    Description = "Regista atividade elétrica cardíaca."
                },
                new TypeMaterial
                {
                    Name = "Ecógrafo Portátil",
                    Description = "Realiza exames de ultrassom móvel."
                },
                new TypeMaterial
                {
                    Name = "Glicosímetro",
                    Description = "Mede níveis de glicose."
                },
                new TypeMaterial
                {
                    Name = "Oxímetro de Pulso Portátil",
                    Description = "Mede saturação de oxigénio."
                },
                new TypeMaterial
                {
                    Name = "Nebulizador Portátil",
                    Description = "Administra medicação em aerossol."
                },
                new TypeMaterial
                {
                    Name = "Desfibrilhador Automático Externo (AED)",
                    Description = "Reverte paragem cardíaca."
                },
                new TypeMaterial
                {
                    Name = "Bomba de Seringa Portátil",
                    Description = "Faz infusões precisas via seringa."
                },
                new TypeMaterial
                {
                    Name = "Otoscópio Portátil",
                    Description = "Examina o ouvido externamente."
                },
                new TypeMaterial
                {
                    Name = "Retinógrafo Portátil",
                    Description = "Capta imagens da retina."
                },
                new TypeMaterial
                {
                    Name = "Medidor de Pressão Portátil",
                    Description = "Mede pressão arterial."
                },
                new TypeMaterial
                {
                    Name = "Capnógrafo Portátil",
                    Description = "Mede CO₂ exalado."
                },
                new TypeMaterial
                {
                    Name = "Monitor Holter",
                    Description = "Regista ECG contínuo."
                },
                new TypeMaterial
                {
                    Name = "Termómetro Clínico Portátil",
                    Description = "Mede temperatura corporal."
                },
                new TypeMaterial
                {
                    Name = "Ventilação Não Invasiva Portátil",
                    Description = "Fornece suporte respiratório."
                },
                new TypeMaterial
                {
                    Name = "Telemetria Médica Portátil",
                    Description = "Transmite sinais vitais."
                },
                new TypeMaterial
                {
                    Name = "Dispositivo Portátil de Desinfeção UV",
                    Description = "Desinfeta superfícies com UV."
                }
            };



            foreach (var tm in typeMaterials)
            {
                if (!dbContext.TypeMaterial.Any(x => x.Name == tm.Name))
                {
                    dbContext.TypeMaterial.Add(tm);
                }
            }

            dbContext.SaveChanges();
        }

        private static void PopulateManufacturer(HealthWellbeingDbContext dbContext)
        {
            if (!dbContext.Manufacturer.Any())
            {
                var manufacturers = new List<Manufacturer>
                {
                    new Manufacturer { Name = "GE Healthcare" },
                    new Manufacturer { Name = "Siemens Healthineers" },
                    new Manufacturer { Name = "Philips Healthcare" },
                    new Manufacturer { Name = "Dräger" },
                    new Manufacturer { Name = "Mindray" },
                    new Manufacturer { Name = "HP" },

                    new Manufacturer { Name = "Medtronic" },
                    new Manufacturer { Name = "Samsung Medison" },
                    new Manufacturer { Name = "Canon Medical Systems" },
                    new Manufacturer { Name = "Fujifilm Healthcare" },
                    new Manufacturer { Name = "Agfa Healthcare" },
                    new Manufacturer { Name = "Boston Scientific" },
                    new Manufacturer { Name = "Baxter" },
                    new Manufacturer { Name = "B. Braun" },
                    new Manufacturer { Name = "Abbott" },
                    new Manufacturer { Name = "Zimmer Biomet" },
                    new Manufacturer { Name = "Stryker" },
                    new Manufacturer { Name = "Roche" },
                    new Manufacturer { Name = "Edan Instruments" },
                    new Manufacturer { Name = "Hospira" }
                };
                dbContext.Manufacturer.AddRange(manufacturers);
                dbContext.SaveChanges();
            }
        }

        //RoomStatus
        private static void PopulateRoomStatus(HealthWellbeingDbContext dbContext)
        {
            if (!dbContext.RoomStatus.Any())
            {
                var roomsStatus = new List<RoomStatus>
                {
                new RoomStatus { Name = "Criado", Description = "Sala registrada no sistema" },
                new RoomStatus { Name = "Disponível", Description = "Sala pronta para uso" },
                new RoomStatus { Name = "Indisponível", Description = "Sala ocupada ou bloqueada" },
                new RoomStatus { Name = "Em Limpeza", Description = "Sala em processo de higienização" },
                new RoomStatus { Name = "Em Manutenção", Description = "Sala com manutenção técnica" },
                new RoomStatus { Name = "Fora de Serviço", Description = "Sala desativada ou inutilizável" }
                };
                dbContext.RoomStatus.AddRange(roomsStatus);
                dbContext.SaveChanges();
            }
        }

        //RoomType
        private static void PopulateRoomType(HealthWellbeingDbContext dbContext)
        {
            if (!dbContext.RoomType.Any())
            {
                var roomsTypes = new List<RoomType>
                {
                new RoomType { Name = "Consultório", Description = "Sala para atendimento ambulatorial" },
                new RoomType { Name = "UTI", Description = "Unidade de terapia intensiva" },
                new RoomType { Name = "Centro Cirúrgico", Description = "Sala para procedimentos cirúrgicos" },
                new RoomType { Name = "Sala de Exames", Description = "Espaço para exames clínicos ou de imagem" },
                new RoomType { Name = "Laboratório", Description = "Sala para análises clínicas" },
                new RoomType { Name = "Farmácia", Description = "Armazenamento e dispensação de medicamentos" },
                new RoomType { Name = "Armazém", Description = "Espaço para armazenamento de materiais e insumos" }
                };
                dbContext.RoomType.AddRange(roomsTypes);
                dbContext.SaveChanges();
            }
        }
        //RoomLocation
        private static void PopulateRoomLocation(HealthWellbeingDbContext dbContext)
        {
            if (!dbContext.RoomLocation.Any())
            {
                var roomsLocations = new List<RoomLocation>
                {
                new RoomLocation { Name = "BlocoA-Ala1-Z.A" },
                new RoomLocation { Name = "BlocoA-Ala2-Z.V" },
                new RoomLocation { Name = "BlocoA-Ala3-Z.VM" },
                new RoomLocation { Name = "BlocoB-Ala1-Z.C" },
                new RoomLocation { Name = "BlocoB-Ala2-Z.PO" },
                new RoomLocation { Name = "BlocoB-Ala3-Z.R" },
                new RoomLocation { Name = "BlocoC-Ala1-Z.I" },
                new RoomLocation { Name = "BlocoC-Ala2-Z.E" },
                new RoomLocation { Name = "BlocoC-Ala3-Z.D" },
                new RoomLocation { Name = "BlocoD-Ala1-Z.AM" },
                new RoomLocation { Name = "BlocoD-Ala2-Z.CS" },
                new RoomLocation { Name = "BlocoD-Ala3-Z.ESP" },
                new RoomLocation { Name = "BlocoE-Ala1-Z.CO" },
                new RoomLocation { Name = "BlocoE-Ala2-Z.AC" },
                new RoomLocation { Name = "BlocoE-Ala3-Z.BM" },
                new RoomLocation { Name = "BlocoF-Ala1-Z.DI" },
                new RoomLocation { Name = "BlocoF-Ala2-Z.AR" },
                new RoomLocation { Name = "BlocoF-Ala3-Z.CE" },
                new RoomLocation { Name = "BlocoG-Ala1-Z.N" },
                new RoomLocation { Name = "BlocoG-Ala2-Z.F" },
                new RoomLocation { Name = "BlocoG-Ala3-Z.TO" },
                new RoomLocation { Name = "BlocoH-Ala1-Z.AU" },
                new RoomLocation { Name = "BlocoH-Ala2-Z.OF" },
                new RoomLocation { Name = "BlocoH-Ala3-Z.NEU" },
                new RoomLocation { Name = "BlocoI-Ala1-Z.IS" }
                };
                dbContext.RoomLocation.AddRange(roomsLocations);
                dbContext.SaveChanges();
            }
        }

        // 2- Room
        private static void PopulateRoom(HealthWellbeingDbContext dbContext)
        {
            dbContext.SaveChanges();

            if (!dbContext.Room.Any())
            {

                // Obtenha todos os existentes em dicionários (Name -> Id)
                var roomStatus = dbContext.RoomStatus.ToDictionary(r => r.Name, r => r.RoomStatusId);
                var roomType = dbContext.RoomType.ToDictionary(t => t.Name, t => t.RoomTypeId);
                var roomLocation = dbContext.RoomLocation.ToDictionary(l => l.Name, l => l.RoomLocationId);

                var rooms = new List<Room>
                {
                    new Room { Name = "UTI 2", Specialty = "Cuidados Intensivos", RoomTypeId = roomType["UTI"], RoomLocationId = roomLocation["BlocoA-Ala2-Z.V"], RoomStatusId = roomStatus["Disponível"], OpeningTime = TimeSpan.Parse("00:00"), ClosingTime = TimeSpan.Parse("23:59"), Notes = "Equipamentos de suporte vital." },
                    new Room { Name = "Centro Cirúrgico 1", Specialty = "Cirurgia Geral", RoomTypeId = roomType["Centro Cirúrgico"], RoomLocationId = roomLocation["BlocoB-Ala1-Z.C"], RoomStatusId = roomStatus["Indisponível"], OpeningTime = TimeSpan.Parse("07:00"), ClosingTime = TimeSpan.Parse("19:00"), Notes = "Cirurgias de média complexidade." },
                    new Room { Name = "Centro Cirúrgico 2", Specialty = "Ortopedia", RoomTypeId = roomType["Centro Cirúrgico"], RoomLocationId = roomLocation["BlocoB-Ala2-Z.PO"], RoomStatusId = roomStatus["Disponível"], OpeningTime = TimeSpan.Parse("07:00"), ClosingTime = TimeSpan.Parse("19:00"), Notes = "Cirurgias ortopédicas." },
                    new Room { Name = "Consultório 1", Specialty = "Pediatria", RoomTypeId = roomType["Consultório"], RoomLocationId = roomLocation["BlocoD-Ala1-Z.AM"], RoomStatusId = roomStatus["Disponível"], OpeningTime = TimeSpan.Parse("08:00"), ClosingTime = TimeSpan.Parse("17:00"), Notes = "Atendimento infantil." },
                    new Room { Name = "Consultório 2", Specialty = "Dermatologia", RoomTypeId = roomType["Consultório"], RoomLocationId = roomLocation["BlocoD-Ala2-Z.CS"], RoomStatusId = roomStatus["Disponível"], OpeningTime = TimeSpan.Parse("08:00"), ClosingTime = TimeSpan.Parse("17:00"), Notes = "Consultas dermatológicas." },
                    new Room { Name = "Consultório 3", Specialty = "Cardiologia", RoomTypeId = roomType["Consultório"], RoomLocationId = roomLocation["BlocoD-Ala3-Z.ESP"], RoomStatusId = roomStatus["Disponível"], OpeningTime = TimeSpan.Parse("08:00"), ClosingTime = TimeSpan.Parse("17:00"), Notes = "Consultas cardíacas." },
                    new Room { Name = "Sala de Exames 1", Specialty = "Radiologia", RoomTypeId = roomType["Sala de Exames"], RoomLocationId = roomLocation["BlocoC-Ala1-Z.I"], RoomStatusId = roomStatus["Disponível"], OpeningTime = TimeSpan.Parse("08:00"), ClosingTime = TimeSpan.Parse("18:00"), Notes = "Raio-X e tomografia." },
                    new Room { Name = "Sala de Exames 2", Specialty = "Ultrassonografia", RoomTypeId = roomType["Sala de Exames"], RoomLocationId = roomLocation["BlocoC-Ala2-Z.E"], RoomStatusId = roomStatus["Disponível"], OpeningTime = TimeSpan.Parse("08:00"), ClosingTime = TimeSpan.Parse("18:00"), Notes = "Ultrassons diversos." },
                    new Room { Name = "Laboratório 1", Specialty = "Análises Clínicas", RoomTypeId = roomType["Laboratório"], RoomLocationId = roomLocation["BlocoE-Ala1-Z.CO"], RoomStatusId = roomStatus["Disponível"], OpeningTime = TimeSpan.Parse("07:00"), ClosingTime = TimeSpan.Parse("15:00"), Notes = "Coleta e análise." },
                    new Room { Name = "Laboratório 2", Specialty = "Biologia Molecular", RoomTypeId = roomType["Laboratório"], RoomLocationId = roomLocation["BlocoE-Ala3-Z.BM"], RoomStatusId = roomStatus["Disponível"], OpeningTime = TimeSpan.Parse("07:00"), ClosingTime = TimeSpan.Parse("15:00"), Notes = "PCR e genética." },
                    new Room { Name = "Farmácia de Emergência", Specialty = "Farmácia", RoomTypeId = roomType["Farmácia"], RoomLocationId = roomLocation["BlocoF-Ala2-Z.AR"], RoomStatusId = roomStatus["Disponível"], OpeningTime = TimeSpan.Parse("00:00"), ClosingTime = TimeSpan.Parse("23:59"), Notes = "Atendimento 24h." },
                    new Room { Name = "Armazém 1", Specialty = "Logística", RoomTypeId = roomType["Armazém"], RoomLocationId = roomLocation["BlocoF-Ala3-Z.CE"], RoomStatusId = roomStatus["Disponível"], OpeningTime = TimeSpan.Parse("06:00"), ClosingTime = TimeSpan.Parse("18:00"), Notes = "Estoque de insumos." },
                    new Room { Name = "Armazém 2", Specialty = "Materiais Cirúrgicos", RoomTypeId = roomType["Armazém"], RoomLocationId = roomLocation["BlocoG-Ala1-Z.N"], RoomStatusId = roomStatus["Disponível"], OpeningTime = TimeSpan.Parse("06:00"), ClosingTime = TimeSpan.Parse("18:00"), Notes = "Instrumentos e EPIs." },
                    new Room { Name = "Sala de Exames 3", Specialty = "Endoscopia", RoomTypeId = roomType["Sala de Exames"], RoomLocationId = roomLocation["BlocoC-Ala3-Z.D"], RoomStatusId = roomStatus["Em Limpeza"], OpeningTime = TimeSpan.Parse("08:00"), ClosingTime = TimeSpan.Parse("17:00"), Notes = "Exames endoscópicos." },
                    new Room { Name = "Consultório 4", Specialty = "Ginecologia", RoomTypeId = roomType["Consultório"], RoomLocationId = roomLocation["BlocoE-Ala2-Z.AC"], RoomStatusId = roomStatus["Disponível"], OpeningTime = TimeSpan.Parse("08:00"), ClosingTime = TimeSpan.Parse("17:00"), Notes = "Consultas ginecológicas." },
                    new Room { Name = "Sala de Exames 4", Specialty = "Mamografia", RoomTypeId = roomType["Sala de Exames"], RoomLocationId = roomLocation["BlocoI-Ala1-Z.IS"], RoomStatusId = roomStatus["Indisponível"], OpeningTime = TimeSpan.Parse("08:00"), ClosingTime = TimeSpan.Parse("14:00"), Notes = "Exames de mama." },
                    new Room { Name = "UTI 3", Specialty = "Cuidados Intensivos", RoomTypeId = roomType["UTI"], RoomLocationId = roomLocation["BlocoG-Ala2-Z.F"], RoomStatusId = roomStatus["Em Manutenção"], OpeningTime = TimeSpan.Parse("00:00"), ClosingTime = TimeSpan.Parse("23:59"), Notes = "Manutenção de equipamentos." },
                    new Room { Name = "Consultório 5", Specialty = "Psiquiatria", RoomTypeId = roomType["Consultório"], RoomLocationId = roomLocation["BlocoH-Ala2-Z.OF"], RoomStatusId = roomStatus["Fora de Serviço"], OpeningTime = TimeSpan.Parse("09:00"), ClosingTime = TimeSpan.Parse("17:00"), Notes = "Atendimento suspenso temporariamente." }
                };
                dbContext.Room.AddRange(rooms);
                dbContext.SaveChanges();
            }
        }

        // 3️ - Infraestrutura de Equipment
        private static void PolutateEquipmentTypes(HealthWellbeingDbContext dbContext)
        {
            if (!dbContext.EquipmentType.Any())
            {
                var equipmentTypes = new List<EquipmentType>
                {
                    new EquipmentType { Name = "Monitor de Sinais Vitais", ManufacturerId = 5 },
                    new EquipmentType { Name = "Ventilador Mecânico", ManufacturerId = 4 },
                    new EquipmentType { Name = "ECG Eletrocardiógrafo", ManufacturerId = 3 },
                    new EquipmentType { Name = "Raio-X Digital", ManufacturerId = 1 },
                    new EquipmentType { Name = "TAC / CT Scanner", ManufacturerId = 2 },
                    new EquipmentType { Name = "Bomba de Infusão", ManufacturerId = 5 },
                    new EquipmentType { Name = "Desfibrilhador", ManufacturerId = 3 },
                    new EquipmentType { Name = "Telefone Fixo", ManufacturerId = 6 },

                };
                dbContext.EquipmentType.AddRange(equipmentTypes);
                dbContext.SaveChanges();
            }
        }

        private static void PolutateEquipmentStatus(HealthWellbeingDbContext dbContext)
        {
            if (!dbContext.EquipmentStatus.Any())
            {
                var equipmentStatuses = new List<EquipmentStatus>
                {
                    new EquipmentStatus { Name = "Operacional" },
                    new EquipmentStatus { Name = "Em Manutenção" },
                    new EquipmentStatus { Name = "Fora de Uso" }
                };
                dbContext.EquipmentStatus.AddRange(equipmentStatuses);
                dbContext.SaveChanges();
            }
        }

        private static void PolutateEquipment(HealthWellbeingDbContext dbContext)
        {
            if (!dbContext.Equipment.Any() && dbContext.Room.Any())
            {
                var equipment = new List<Equipment>
                {
                    // --- MONITORES DE SINAIS VITAIS — Mindray BeneVision ---
                    new Equipment { Name="BeneVision N12", SerialNumber="MD-MON-001", RoomId=1, PurchaseDate=new(2022,4,10), EquipmentTypeId=1, EquipmentStatusId=1 },
                    new Equipment { Name="BeneVision N12", SerialNumber="MD-MON-002", RoomId=3, PurchaseDate=new(2021,12,2), EquipmentTypeId=1, EquipmentStatusId=1 },
                    new Equipment { Name="BeneVision N15", SerialNumber="MD-MON-003", RoomId=2, PurchaseDate=new(2023,3,18), EquipmentTypeId=1, EquipmentStatusId=1 },
                    new Equipment { Name="BeneVision N17", SerialNumber="MD-MON-004", RoomId=5, PurchaseDate=new(2020,7,9), EquipmentTypeId=1, EquipmentStatusId=2 },
                    new Equipment { Name="BeneVision N19", SerialNumber="MD-MON-005", RoomId=3, PurchaseDate=new(2019,11,28), EquipmentTypeId=1, EquipmentStatusId=1 },

                    // --- VENTILADORES — Dräger Evita/Savina ---
                    new Equipment { Name="Evita V600", SerialNumber="DR-VEN-001", RoomId=3, PurchaseDate=new(2023,1,12), EquipmentTypeId=2, EquipmentStatusId=1 },
                    new Equipment { Name="Evita V800", SerialNumber="DR-VEN-002", RoomId=2, PurchaseDate=new(2022,6,8), EquipmentTypeId=2, EquipmentStatusId=1 },
                    new Equipment { Name="Savina 300", SerialNumber="DR-VEN-003", RoomId=4, PurchaseDate=new(2021,5,16), EquipmentTypeId=2, EquipmentStatusId=2 },
                    new Equipment { Name="Savina 300 Select", SerialNumber="DR-VEN-004", RoomId=4, PurchaseDate=new(2020,9,3), EquipmentTypeId=2, EquipmentStatusId=3 },
                    new Equipment { Name="Evita Infinity V500", SerialNumber="DR-VEN-005", RoomId=5, PurchaseDate=new(2019,4,27), EquipmentTypeId=2, EquipmentStatusId=1 },

                    // --- ECG — Philips TC Series ---
                    new Equipment { Name="Philips TC70", SerialNumber="PH-ECG-001", RoomId=5, PurchaseDate=new(2023,2,14), EquipmentTypeId=3, EquipmentStatusId=1 },
                    new Equipment { Name="Philips TC50", SerialNumber="PH-ECG-002", RoomId=6, PurchaseDate=new(2022,1,19), EquipmentTypeId=3, EquipmentStatusId=1 },
                    new Equipment { Name="Philips TC30", SerialNumber="PH-ECG-003", RoomId=6, PurchaseDate=new(2021,11,8), EquipmentTypeId=3, EquipmentStatusId=2 },
                    new Equipment { Name="Philips PageWriter TC70", SerialNumber="PH-ECG-004", RoomId=7, PurchaseDate=new(2020,5,21), EquipmentTypeId=3, EquipmentStatusId=1 },
                    new Equipment { Name="Philips PageWriter TC50", SerialNumber="PH-ECG-005", RoomId=7, PurchaseDate=new(2019,9,29), EquipmentTypeId=3, EquipmentStatusId=1 },

                    // --- RAIO-X DIGITAL — GE Optima / Definium ---
                    new Equipment { Name="GE Optima XR240amx", SerialNumber="GE-RX-001", RoomId=8, PurchaseDate=new(2022,3,10), EquipmentTypeId=4, EquipmentStatusId=1 },
                    new Equipment { Name="GE Optima XR220", SerialNumber="GE-RX_002", RoomId=8, PurchaseDate=new(2021,8,4), EquipmentTypeId=4, EquipmentStatusId=1 },
                    new Equipment { Name="GE Definium 5000", SerialNumber="GE-RX-003", RoomId=9, PurchaseDate=new(2020,6,30), EquipmentTypeId=4, EquipmentStatusId=2 },
                    new Equipment { Name="GE Definium 6000", SerialNumber="GE-RX-004", RoomId=9, PurchaseDate=new(2019,1,25), EquipmentTypeId=4, EquipmentStatusId=1 },
                    new Equipment { Name="GE Optima XR646", SerialNumber="GE-RX-005", RoomId=10, PurchaseDate=new(2023,9,2), EquipmentTypeId=4, EquipmentStatusId=1 },

                    // --- TAC / CT — Siemens SOMATOM ---
                    new Equipment { Name="Siemens SOMATOM Go.Up", SerialNumber="SI-CT-001", RoomId=10, PurchaseDate=new(2022,10,15), EquipmentTypeId=5, EquipmentStatusId=1 },
                    new Equipment { Name="Siemens SOMATOM Go.Top", SerialNumber="SI-CT-002", RoomId=10, PurchaseDate=new(2021,3,22), EquipmentTypeId=5, EquipmentStatusId=2 },
                    new Equipment { Name="Siemens SOMATOM Definition AS", SerialNumber="SI-CT-003", RoomId=11, PurchaseDate=new(2020,1,9), EquipmentTypeId=5, EquipmentStatusId=3 },

                    // --- BOMBAS DE INFUSÃO — Mindray BeneFusion ---
                    new Equipment { Name="BeneFusion eSP", SerialNumber="MD-INF-001", RoomId=1, PurchaseDate=new(2023,7,14), EquipmentTypeId=6, EquipmentStatusId=1 },
                    new Equipment { Name="BeneFusion eVP", SerialNumber="MD-INF-002", RoomId=1, PurchaseDate=new(2022,5,18), EquipmentTypeId=6, EquipmentStatusId=1 },
                    new Equipment { Name="BeneFusion VP5", SerialNumber="MD-INF-003", RoomId=5, PurchaseDate=new(2021,4,11), EquipmentTypeId=6, EquipmentStatusId=2 },
                    new Equipment { Name="BeneFusion VP1", SerialNumber="MD-INF-004", RoomId=5, PurchaseDate=new(2020,3,6), EquipmentTypeId=6, EquipmentStatusId=1 },
                    new Equipment { Name="BeneFusion VP1", SerialNumber="MD-INF-005", RoomId=6, PurchaseDate=new(2019,10,23), EquipmentTypeId=6, EquipmentStatusId=1 },

                    // --- DESFIBRILHADORES — Philips HeartStart ---
                    new Equipment { Name="HeartStart XL", SerialNumber="PH-DES-001", RoomId=3, PurchaseDate=new(2023,2,28), EquipmentTypeId=7, EquipmentStatusId=1 },
                    new Equipment { Name="HeartStart MRx", SerialNumber="PH-DES-002", RoomId=4, PurchaseDate=new(2022,9,16), EquipmentTypeId=7, EquipmentStatusId=1 },
                    new Equipment { Name="HeartStart XL+", SerialNumber="PH-DES-003", RoomId=7, PurchaseDate=new(2021,1,20), EquipmentTypeId=7, EquipmentStatusId=2 },
                    new Equipment { Name="HeartStart FR3", SerialNumber="PH-DES-004", RoomId=8, PurchaseDate=new(2020,11,30), EquipmentTypeId=7, EquipmentStatusId=1 },
                    new Equipment { Name="HeartStart FRx", SerialNumber="PH-DES-005", RoomId=9, PurchaseDate=new(2019,8,14), EquipmentTypeId=7, EquipmentStatusId=1 },

                    // EXTRA para passar os 30
                    new Equipment { Name="BeneVision N15", SerialNumber="MD-MON-006", RoomId=2, PurchaseDate=new(2023,12,4), EquipmentTypeId=1, EquipmentStatusId=1 },
                    new Equipment { Name="Evita V800", SerialNumber="DR-VEN-006", RoomId=3, PurchaseDate=new(2024,1,3), EquipmentTypeId=2, EquipmentStatusId=1 }

                };
                dbContext.Equipment.AddRange(equipment);
                dbContext.SaveChanges();
            }
        }

        // 4️ - Dispositivos médicos
        private static void PopulateMedicalDevices(HealthWellbeingDbContext dbContext)
        {
            if (dbContext.MedicalDevices.Any()) return;
            dbContext.MedicalDevices.AddRange(new List<MedicalDevice>
            {
                new MedicalDevice { Name = "Monitor de Paciente Vital X5", SerialNumber = "MT-VITAL-X5-001", RegistrationDate = DateTime.Now.AddDays(-150), Observation = "Monitor de cabeceira fixo.", TypeMaterialID = 1 },
                new MedicalDevice { Name = "Monitor de Paciente Vital X5", SerialNumber = "MT-VITAL-X5-002", RegistrationDate = DateTime.Now.AddDays(-140), Observation = "Aguardando nova bateria.", TypeMaterialID = 1 },
                new MedicalDevice { Name = "Oxímetro de Pulso Portátil", SerialNumber = "MT-OXI-HAND-01", RegistrationDate = DateTime.Now.AddDays(-130), Observation = "Usado na triagem.", TypeMaterialID = 1 },
                new MedicalDevice { Name = "Eletrocardiógrafo ECG-1200", SerialNumber = "MT-ECG-1200", RegistrationDate = DateTime.Now.AddDays(-120), Observation = "Para consultas externas. Necessita de consumíveis (papel).", TypeMaterialID = 1 },
                new MedicalDevice { Name = "Ventilador de Cuidados Intensivos Alpha", SerialNumber = "AV-VCI-A01", RegistrationDate = DateTime.Now.AddDays(-100), Observation = "Localizado na UCI 1.", TypeMaterialID = 1 },
                new MedicalDevice { Name = "Ventilador de Cuidados Intensivos Alpha", SerialNumber = "AV-VCI-A02", RegistrationDate = DateTime.Now.AddDays(-90), Observation = "Localizado na UCI 2.", TypeMaterialID = 2 },
                new MedicalDevice { Name = "Desfibrilhador DEA Auto", SerialNumber = "BC-DEA-001", RegistrationDate = DateTime.Now.AddDays(-80), Observation = "Localizado no carrinho de emergência principal.", TypeMaterialID = 2 },
                new MedicalDevice { Name = "Sistema de Aspiração Central", SerialNumber = "BC-ASC-005", RegistrationDate = DateTime.Now.AddDays(-70), Observation = "Unidade fixa, manutenção trimestral.", TypeMaterialID = 2 },
                new MedicalDevice { Name = "Ecógrafo Móvel 3D", SerialNumber = "AD-ECHO-M3D", RegistrationDate = DateTime.Now.AddDays(-60), Observation = "Transdutores convexos e lineares.", TypeMaterialID = 2 },
                new MedicalDevice { Name = "Sistema de Raio-X Portátil", SerialNumber = "AD-RX-PORT-A", RegistrationDate = DateTime.Now.AddDays(-50), Observation = "Para radiografias de cabeceira.", TypeMaterialID = 2 },
                new MedicalDevice { Name = "Unidade de Eletrocirurgia Portátil", SerialNumber = "SG-ELECSURG-01", RegistrationDate = DateTime.Now.AddDays(-40), Observation = "Modelo de alta frequência. Uso em Bloco 1.", TypeMaterialID = 3 },
                new MedicalDevice { Name = "Caixa de Instrumental Básico (Grande)", SerialNumber = "SG-INST-BGC01", RegistrationDate = DateTime.Now.AddDays(-30), Observation = "Requer reesterilização.", TypeMaterialID = 3 },
                new MedicalDevice { Name = "Foco Cirúrgico Móvel LED", SerialNumber = "BC-FOCUS-LED01", RegistrationDate = DateTime.Now.AddDays(-25), Observation = "Para procedimentos menores.", TypeMaterialID = 3 },
                new MedicalDevice { Name = "Caixa de Instrumental Básico (Pequena)", SerialNumber = "SG-INST-BSC02", RegistrationDate = DateTime.Now.AddDays(-20), Observation = "Kit de sutura e pinças.", TypeMaterialID = 3 },
                new MedicalDevice { Name = "Pinça Hemostática (Individual)", SerialNumber = "SG-PINZA-HEM01", RegistrationDate = DateTime.Now.AddDays(-15), Observation = "Pronta a usar.", TypeMaterialID = 3 },
                new MedicalDevice { Name = "Analisador de Gás Sanguíneo Portátil", SerialNumber = "AD-AGS-P01", RegistrationDate = DateTime.Now.AddDays(-10), Observation = "Localizado no Laboratório de Urgência.", TypeMaterialID = 4 },
                new MedicalDevice { Name = "Analisador de Gás Sanguíneo Portátil", SerialNumber = "AD-AGS-P02", RegistrationDate = DateTime.Now.AddDays(-9), Observation = "Requer substituição de cartuchos.", TypeMaterialID = 4 },
                new MedicalDevice { Name = "Estetoscópio Digital", SerialNumber = "AD-EST-DIGI-1", RegistrationDate = DateTime.Now.AddDays(-8), Observation = "Para o chefe de serviço.", TypeMaterialID = 4 },
                new MedicalDevice { Name = "Bomba de Infusão de Nutrição", SerialNumber = "AV-NUTRI-INF-01", RegistrationDate = DateTime.Now.AddDays(-7), Observation = "Uso exclusivo para nutrição parenteral.", TypeMaterialID = 4 },
                new MedicalDevice { Name = "Oxímetro de Pulso Portátil", SerialNumber = "MT-OXI-HAND-02", RegistrationDate = DateTime.Now.AddDays(-6), Observation = "Unidade nova, em caixa.", TypeMaterialID = 4 }
            });
            dbContext.SaveChanges();
        }

        // 5️ - Localizações
        private static void PopulateLocalizacaoDispMovel_temporario(HealthWellbeingDbContext dbContext)
        {
            if (dbContext.LocalizacaoDispMovel_temporario.Any()) return;
            dbContext.LocalizacaoDispMovel_temporario.AddRange(new List<LocalizacaoDispMovel_temporario>
            {
                new LocalizacaoDispMovel_temporario { MedicalDeviceID = 1, RoomId = 1, IsCurrent = true },
                new LocalizacaoDispMovel_temporario { MedicalDeviceID = 2, RoomId = 2, IsCurrent = true },
                new LocalizacaoDispMovel_temporario { MedicalDeviceID = 3, RoomId = 3, IsCurrent = true },
                new LocalizacaoDispMovel_temporario { MedicalDeviceID = 4, RoomId = 4, IsCurrent = true },
                new LocalizacaoDispMovel_temporario { MedicalDeviceID = 5, RoomId = 5, IsCurrent = true },
                new LocalizacaoDispMovel_temporario { MedicalDeviceID = 6, RoomId = 6, IsCurrent = true },
                new LocalizacaoDispMovel_temporario { MedicalDeviceID = 7, RoomId = 7, IsCurrent = true },
                new LocalizacaoDispMovel_temporario { MedicalDeviceID = 8, RoomId = 8, IsCurrent = true },
                new LocalizacaoDispMovel_temporario { MedicalDeviceID = 9, RoomId = 9, IsCurrent = true },
                new LocalizacaoDispMovel_temporario { MedicalDeviceID = 10, RoomId = 4, IsCurrent = true },
                new LocalizacaoDispMovel_temporario { MedicalDeviceID = 11, RoomId = 5, IsCurrent = true },
                new LocalizacaoDispMovel_temporario { MedicalDeviceID = 12, RoomId = 6, IsCurrent = true },
                new LocalizacaoDispMovel_temporario { MedicalDeviceID = 13, RoomId = 1, IsCurrent = true },
                new LocalizacaoDispMovel_temporario { MedicalDeviceID = 14, RoomId = 2, IsCurrent = true },
                new LocalizacaoDispMovel_temporario { MedicalDeviceID = 15, RoomId = 3, IsCurrent = true }
            });
            dbContext.SaveChanges();
        }

        private static void PopulateLocationMedDevices(HealthWellbeingDbContext dbContext)
        {
            if (dbContext.LocationMedDevice.Any()) return;
            dbContext.LocationMedDevice.AddRange(new List<LocationMedDevice>
            {
                new LocationMedDevice { MedicalDeviceID = 1, RoomId = 1, InitialDate = DateTime.Now.AddMonths(-3), EndDate = null, IsCurrent = true },
                new LocationMedDevice { MedicalDeviceID = 2, RoomId = 2, InitialDate = DateTime.Now.AddMonths(-2), EndDate = DateTime.Now.AddDays(-10), IsCurrent = true },
                new LocationMedDevice { MedicalDeviceID = 3, RoomId = 3, InitialDate = DateTime.Now.AddMonths(-1), EndDate = null, IsCurrent = true },
                new LocationMedDevice { MedicalDeviceID = 4, RoomId = 4, InitialDate = DateTime.Now.AddDays(-45), EndDate = DateTime.Now.AddDays(-5), IsCurrent = true },
                new LocationMedDevice { MedicalDeviceID = 5, RoomId = 5, InitialDate = DateTime.Now.AddDays(-30), EndDate = null, IsCurrent = true },
                new LocationMedDevice { MedicalDeviceID = 6, RoomId = 1, InitialDate = DateTime.Now.AddDays(-20), EndDate = null, IsCurrent = true },
                new LocationMedDevice { MedicalDeviceID = 7, RoomId = 2, InitialDate = DateTime.Now.AddDays(-15), EndDate = null, IsCurrent = true },
                new LocationMedDevice { MedicalDeviceID = 8, RoomId = 3, InitialDate = DateTime.Now.AddDays(-10), EndDate = null, IsCurrent = true },
                new LocationMedDevice { MedicalDeviceID = 9, RoomId = 4, InitialDate = DateTime.Now.AddDays(-8), EndDate = null, IsCurrent = true },
                new LocationMedDevice { MedicalDeviceID = 10, RoomId = 5, InitialDate = DateTime.Now.AddDays(-6), EndDate = DateTime.Now.AddDays(-1), IsCurrent = true },
                new LocationMedDevice { MedicalDeviceID = 11, RoomId = 1, InitialDate = DateTime.Now.AddDays(-4), EndDate = null, IsCurrent = true },
                new LocationMedDevice { MedicalDeviceID = 12, RoomId = 2, InitialDate = DateTime.Now.AddDays(-3), EndDate = null, IsCurrent = true },
                new LocationMedDevice { MedicalDeviceID = 13, RoomId = 3, InitialDate = DateTime.Now.AddDays(-2), EndDate = null, IsCurrent = true },
                new LocationMedDevice { MedicalDeviceID = 14, RoomId = 4, InitialDate = DateTime.Now.AddDays(-1), EndDate = null, IsCurrent = true },
                new LocationMedDevice { MedicalDeviceID = 15, RoomId = 5, InitialDate = DateTime.Now, EndDate = null, IsCurrent = true },
                new LocationMedDevice { MedicalDeviceID = 16, RoomId = 1, InitialDate = DateTime.Now, EndDate = null, IsCurrent = true },
                new LocationMedDevice { MedicalDeviceID = 17, RoomId = 2, InitialDate = DateTime.Now, EndDate = null, IsCurrent = true },
                new LocationMedDevice { MedicalDeviceID = 18, RoomId = 3, InitialDate = DateTime.Now, EndDate = null, IsCurrent = true },
                new LocationMedDevice { MedicalDeviceID = 19, RoomId = 4, InitialDate = DateTime.Now, EndDate = null, IsCurrent = true },
                new LocationMedDevice { MedicalDeviceID = 20, RoomId = 5, InitialDate = DateTime.Now, EndDate = null, IsCurrent = true }
            });
            dbContext.SaveChanges();
        }
    }
}
