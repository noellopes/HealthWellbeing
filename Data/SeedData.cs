using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeingRoom.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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

            PopulateTypeMaterial(dbContext);
            PopulateMedicalDevices(dbContext);

            PopulateManufacturer(dbContext);
            PolutateEquipmentTypes(dbContext);
            PolutateEquipmentStatus(dbContext);
            PolutateEquipment(dbContext);
        }

        private static void PopulateManufacturer(HealthWellbeingDbContext dbContext)
        {
            // Verificar se os dados de Manufacturer já existem no banco
            if (!dbContext.Manufacturer.Any())
            {
                // Adicionando Manufacturers
                var manufacturers = new List<Manufacturer>
                {
                    new Manufacturer { Name = "Dell" },
                    new Manufacturer { Name = "HP" },
                    new Manufacturer { Name = "Apple" },
                    new Manufacturer { Name = "Lenovo" },
                    new Manufacturer { Name = "Samsung" },
                    new Manufacturer { Name = "Sony" },
                    new Manufacturer { Name = "Microsoft" },
                    new Manufacturer { Name = "Acer" },
                    new Manufacturer { Name = "Asus" },
                    new Manufacturer { Name = "LG" },
                    new Manufacturer { Name = "Canon" },
                    new Manufacturer { Name = "Epson" },
                    new Manufacturer { Name = "Xerox" },
                    new Manufacturer { Name = "Panasonic" },
                    new Manufacturer { Name = "Sharp" },
                    new Manufacturer { Name = "Toshiba" },
                    new Manufacturer { Name = "Fujitsu" },
                    new Manufacturer { Name = "Razer" },
                    new Manufacturer { Name = "Corsair" },
                    new Manufacturer { Name = "Logitech" },
                    new Manufacturer { Name = "Seagate" },
                    new Manufacturer { Name = "Western Digital" },
                    new Manufacturer { Name = "Kingston" },
                    new Manufacturer { Name = "Sandisk" },
                    new Manufacturer { Name = "Intel" },
                    new Manufacturer { Name = "NVIDIA" },
                    new Manufacturer { Name = "Huawei" },
                    new Manufacturer { Name = "Xiaomi" }
                };

                dbContext.Manufacturer.AddRange(manufacturers);
                dbContext.SaveChanges();
            }
        }

        private static void PolutateEquipmentTypes(HealthWellbeingDbContext dbContext)
        {
            if (!dbContext.EquipmentType.Any())
            {
                var equipmentTypes = new List<EquipmentType>
                {
                    new EquipmentType { Name = "Computador" },
                    new EquipmentType { Name = "Impressora" },
                    new EquipmentType { Name = "Projetor" },
                    new EquipmentType { Name = "Monitor" },
                    new EquipmentType { Name = "Teclado" },
                    new EquipmentType { Name = "Mouse" },
                    new EquipmentType { Name = "Roteador" },
                    new EquipmentType { Name = "Scanner" },
                    new EquipmentType { Name = "Tablet" },
                    new EquipmentType { Name = "Smartphone" },
                    new EquipmentType { Name = "Câmera Digital" },
                    new EquipmentType { Name = "Videoconferência" },
                    new EquipmentType { Name = "Ar Condicionado" },
                    new EquipmentType { Name = "Microfone" },
                    new EquipmentType { Name = "Fone de ouvido" },
                    new EquipmentType { Name = "Multímetro" },
                    new EquipmentType { Name = "Câmera de Segurança" },
                    new EquipmentType { Name = "Projetor 3D" },
                    new EquipmentType { Name = "Sistemas de Iluminação" },
                    new EquipmentType { Name = "Câmera 360" },
                    new EquipmentType { Name = "Leitor de código de barras" },
                    new EquipmentType { Name = "Impressora 3D" },
                    new EquipmentType { Name = "Impressora térmica" },
                    new EquipmentType { Name = "Controle de Acesso" },
                    new EquipmentType { Name = "Smartwatch" },
                    new EquipmentType { Name = "Sistema de Som" },
                    new EquipmentType { Name = "Exaustor" },
                    new EquipmentType { Name = "Máquina de Cartão" },
                    new EquipmentType { Name = "Caixa Registradora" }
                };

                dbContext.EquipmentType.AddRange(equipmentTypes);
                dbContext.SaveChanges();
            }
        }

        private static void PolutateEquipmentStatus(HealthWellbeingDbContext dbContext)
        {
            // Verificar se já existem estados de equipamentos
            if (!dbContext.EquipmentStatus.Any())
            {
                var equipmentStatuses = new List<EquipmentStatus>
                {
                    new EquipmentStatus { Name = "Ativo" },
                    new EquipmentStatus { Name = "Em Manutenção" },
                    new EquipmentStatus { Name = "Inativo" }
                };

                dbContext.EquipmentStatus.AddRange(equipmentStatuses);
                dbContext.SaveChanges();
            }
        }

        private static void PolutateEquipment(HealthWellbeingDbContext dbContext)
        {
            // Verificar se já existem salas
            if (!dbContext.Room.Any())
            {
                var roomTypes = Enum.GetValues(typeof(Room.RoomType)).Cast<Room.RoomType>().ToList();
                var roomStatuses = Enum.GetValues(typeof(Room.RoomStatus)).Cast<Room.RoomStatus>().ToList();

                var rooms = new List<Room>();

                for (int i = 1; i <= 30; i++)
                {
                    var room = new Room
                    {
                        Name = $"Sala {i:000}",  // Nome da sala, ex: Sala 001, Sala 002
                        RoomsType = roomTypes[i % roomTypes.Count],  // Alterna entre Consultas e Tratamentos
                        Specialty = (i % 2 == 0) ? "Cardiologia" : "Pediatria",  // Alterna entre especialidades
                        Capacity = new Random().Next(1, 51),  // Capacidade aleatória entre 1 e 50
                        Location = $"Bloco {((i - 1) / 5) + 1}",  // Ex: Bloco 1, Bloco 2, ...
                        OperatingHours = $"08:00 - 18:00",  // Horário fixo de funcionamento
                        Status = roomStatuses[i % roomStatuses.Count],  // Alterna entre os estados possíveis
                        Notes = i % 2 == 0 ? "Sala renovada." : "Necessita de manutenção."  // Observações aleatórias
                    };

                    rooms.Add(room);
                }

                dbContext.Room.AddRange(rooms);
                dbContext.SaveChanges();
            }

            // verificar se já existem equipamentos
            if (!dbContext.Equipment.Any() && dbContext.Room.Any())
            {
                var equipment = new List<Equipment>();

                for (int i = 1; i <= 30; i++)
                {
                    equipment.Add(new Equipment
                    {
                        Name = $"Equipamento {i}",
                        Description = $"Descrição do Equipamento {i}",
                        SerialNumber = $"SN-{i:000}",
                        PurchaseDate = DateTime.Now.AddMonths(-i),
                        ManufacturerId = Random.Shared.Next(1, dbContext.Manufacturer.Count()),
                        EquipmentTypeId = Random.Shared.Next(1, dbContext.EquipmentType.Count()),
                        EquipmentStatusId = Random.Shared.Next(1, dbContext.EquipmentStatus.Count()),
                        RoomId = Random.Shared.Next(1, dbContext.Room.Count())
                    });
                }

                dbContext.Equipment.AddRange(equipment);
                dbContext.SaveChanges();
            }
        }

        private static void PopulateMedicalDevices(HealthWellbeingDbContext dbContext)
        {

            if (dbContext.MedicalDevices.Any()) return;

            dbContext.MedicalDevices.AddRange(new List<MedicalDevice>()
            {
                new MedicalDevice {
                    Name = "Monitor de Paciente Vital X5",
                    SerialNumber = "MT-VITAL-X5-001",
                    RegistrationDate = DateTime.Now.AddDays(-150),
                    Observation = "Monitor de cabeceira fixo.",
                    TypeMaterialID = 1
                },
                new MedicalDevice {
                    Name = "Monitor de Paciente Vital X5", 
                    SerialNumber = "MT-VITAL-X5-002",
                    RegistrationDate = DateTime.Now.AddDays(-140),
                    Observation = "Aguardando nova bateria.",
                    TypeMaterialID = 1
                },
                new MedicalDevice {
                    Name = "Oxímetro de Pulso Portátil",
                    SerialNumber = "MT-OXI-HAND-01",
                    RegistrationDate = DateTime.Now.AddDays(-130),
                    Observation = "Usado na triagem.",
                    TypeMaterialID = 1
                },
                new MedicalDevice {
                    Name = "Eletrocardiógrafo ECG-1200",
                    SerialNumber = "MT-ECG-1200",
                    RegistrationDate = DateTime.Now.AddDays(-120),
                    Observation = "Para consultas externas. Necessita de consumíveis (papel).",
                    TypeMaterialID = 1
                },
                new MedicalDevice {
                    Name = "Ventilador de Cuidados Intensivos Alpha",
                    SerialNumber = "AV-VCI-A01",
                    RegistrationDate = DateTime.Now.AddDays(-100),
                    Observation = "Localizado na UCI 1.",
                    TypeMaterialID = 1
                },
                new MedicalDevice {
                    Name = "Ventilador de Cuidados Intensivos Alpha",
                    SerialNumber = "AV-VCI-A02",
                    RegistrationDate = DateTime.Now.AddDays(-90),
                    Observation = "Localizado na UCI 2.",
                    TypeMaterialID = 2
                },
                new MedicalDevice {
                    Name = "Desfibrilhador DEA Auto",
                    SerialNumber = "BC-DEA-001",
                    RegistrationDate = DateTime.Now.AddDays(-80),
                    Observation = "Localizado no carrinho de emergência principal.",
                    TypeMaterialID = 2
                },
                new MedicalDevice {
                    Name = "Sistema de Aspiração Central",
                    SerialNumber = "BC-ASC-005",
                    RegistrationDate = DateTime.Now.AddDays(-70),
                    Observation = "Unidade fixa, manutenção trimestral.",
                    TypeMaterialID = 2
                },
                new MedicalDevice {
                    Name = "Ecógrafo Móvel 3D",
                    SerialNumber = "AD-ECHO-M3D",
                    RegistrationDate = DateTime.Now.AddDays(-60),
                    Observation = "Transdutores convexos e lineares.",
                    TypeMaterialID = 2
                },
                new MedicalDevice {
                    Name = "Sistema de Raio-X Portátil",
                    SerialNumber = "AD-RX-PORT-A",
                    RegistrationDate = DateTime.Now.AddDays(-50),
                    Observation = "Para radiografias de cabeceira.",
                    TypeMaterialID = 2
                },

                new MedicalDevice {
                    Name = "Unidade de Eletrocirurgia Portátil",
                    SerialNumber = "SG-ELECSURG-01",
                    RegistrationDate = DateTime.Now.AddDays(-40),
                    Observation = "Modelo de alta frequência. Uso em Bloco 1.",
                    TypeMaterialID = 3
                },
                new MedicalDevice {
                    Name = "Caixa de Instrumental Básico (Grande)",
                    SerialNumber = "SG-INST-BGC01",
                    RegistrationDate = DateTime.Now.AddDays(-30),
                    Observation = "Requer reesterilização.",
                    TypeMaterialID = 3
                },
                new MedicalDevice {
                    Name = "Foco Cirúrgico Móvel LED",
                    SerialNumber = "BC-FOCUS-LED01",
                    RegistrationDate = DateTime.Now.AddDays(-25),
                    Observation = "Para procedimentos menores.",
                    TypeMaterialID = 3
                },
                new MedicalDevice {
                    Name = "Caixa de Instrumental Básico (Pequena)",
                    SerialNumber = "SG-INST-BSC02",
                    RegistrationDate = DateTime.Now.AddDays(-20),
                    Observation = "Kit de sutura e pinças.",
                    TypeMaterialID = 3
                },
                new MedicalDevice {
                    Name = "Pinça Hemostática (Individual)",
                    SerialNumber = "SG-PINZA-HEM01",
                    RegistrationDate = DateTime.Now.AddDays(-15),
                    Observation = "Pronta a usar.",
                    TypeMaterialID = 3
                },
                new MedicalDevice {
                    Name = "Analisador de Gás Sanguíneo Portátil",
                    SerialNumber = "AD-AGS-P01",
                    RegistrationDate = DateTime.Now.AddDays(-10),
                    Observation = "Localizado no Laboratório de Urgência.",
                    TypeMaterialID = 4
                },
                new MedicalDevice {
                    Name = "Analisador de Gás Sanguíneo Portátil",
                    SerialNumber = "AD-AGS-P02",
                    RegistrationDate = DateTime.Now.AddDays(-9),
                    Observation = "Requer substituição de cartuchos.",
                    TypeMaterialID = 4
                },
                new MedicalDevice {
                    Name = "Estetoscópio Digital",
                    SerialNumber = "AD-EST-DIGI-1",
                    RegistrationDate = DateTime.Now.AddDays(-8),
                    Observation = "Para o chefe de serviço.",
                    TypeMaterialID = 4
                },
                new MedicalDevice {
                    Name = "Bomba de Infusão de Nutrição",
                    SerialNumber = "AV-NUTRI-INF-01",
                    RegistrationDate = DateTime.Now.AddDays(-7),
                    Observation = "Uso exclusivo para nutrição parenteral.",
                    TypeMaterialID = 4
                },
                new MedicalDevice {
                    Name = "Oxímetro de Pulso Portátil",
                    SerialNumber = "MT-OXI-HAND-02", 
                    RegistrationDate = DateTime.Now.AddDays(-6),
                    Observation = "Unidade nova, em caixa.",
                    TypeMaterialID = 4
                }
            });

            dbContext.SaveChanges();
        }

        private static void PopulateTypeMaterial(HealthWellbeingDbContext? dbContext)
        {
            if (dbContext == null)
                throw new ArgumentNullException(nameof(dbContext));


            dbContext.Database.EnsureCreated();


            var typeMaterials = new[]
            {
                new TypeMaterial
                {
                    Name = "Consumível",
                    Description = "Materiais de uso único ou limitada duração, como gazes, seringas e luvas."
                },
                new TypeMaterial
                {
                    Name = "Equipamento",
                    Description = "Aparelhos fixos utilizados nas salas de consulta e tratamento."
                },
                new TypeMaterial
                {
                    Name = "Dispositivo Médico",
                    Description = "Instrumentos e aparelhos móveis usados para apoio ao diagnóstico e tratamento."
                },
                new TypeMaterial
                {
                    Name = "Instrumento Cirúrgico",
                    Description = "Ferramentas utilizadas em procedimentos invasivos e pequenas cirurgias."
                },
                new TypeMaterial
                {
                    Name = "Material de Esterilização",
                    Description = "Produtos e utensílios destinados à limpeza e esterilização de instrumentos."
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
    }
}