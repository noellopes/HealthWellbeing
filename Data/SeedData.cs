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
                new MedicalDevice {Name = "Monitor MP-500", Type = "Monitorização",
                    Specification = "ECG, SPO2, PNI. S/N: MP500-A01", RegistrationDate = DateTime.Now.AddDays(-60),
                    Observation = "Excelente condição." },

                new MedicalDevice {Name = "Monitor MP-501", Type = "Monitorização",
                    Specification = "ECG, SPO2, PNI. S/N: MP500-A02", RegistrationDate = DateTime.Now.AddDays(-60),
                    Observation = "Em espera para Sala 4." },

                new MedicalDevice {Name = "Bomba Infusora Gemini", Type = "Infusão",
                    Specification = "Duplo Canal, Bateria 4h.", RegistrationDate = DateTime.Now.AddDays(-30),
                    Observation = "Uso exclusivo em pediatria." },

                new MedicalDevice {Name = "Ventilador Portátil V5", Type = "Suporte Vital",
                    Specification = "Modos de ventilação avançados.", RegistrationDate = DateTime.Now.AddDays(-15),
                    Observation = "Sensor de pressão defeituoso." },

                new MedicalDevice {Name = "Máquina de Anestesia PAX", Type = "Anestesia",
                    Specification = "Sistema modular de controlo de gás.", RegistrationDate = DateTime.Now.AddDays(-10),
                    Observation = "Necessita de filtro novo." },

                new MedicalDevice {Name = "Oxímetro de Pulso OP-6", Type = "Diagnóstico",
                    Specification = "Display OLED de alta precisão.", RegistrationDate = DateTime.Now.AddDays(-7),
                    Observation = null },

                new MedicalDevice {Name = "Oxímetro de Pulso OP-7", Type = "Diagnóstico",
                    Specification = "Display OLED de alta precisão.", RegistrationDate = DateTime.Now.AddDays(-7),
                    Observation = null },

                new MedicalDevice {Name = "Ecógrafo Móvel GE", Type = "Diagnóstico",
                    Specification = "Transdutor linear e convexo.", RegistrationDate = DateTime.Now.AddDays(-4),
                    Observation = "Calibração concluída." },

                new MedicalDevice {Name = "Unidade de Fototerapia", Type = "Tratamento",
                    Specification = "Luz azul de alta intensidade.", RegistrationDate = DateTime.Now.AddDays(-2),
                    Observation = null },

                new MedicalDevice {Name = "Balança de Cama Digital", Type = "Monitorização",
                    Specification = "Capacidade de 300kg. Portátil.", RegistrationDate = DateTime.Now.AddDays(-1),
                    Observation = "Para uso em leitos." },

                new MedicalDevice {Name = "Bomba de Aspiração Cirúrgica", Type = "Cirúrgico",
                    Specification = "Fluxo ajustável, recipiente de 2L.", RegistrationDate = DateTime.Now,
                    Observation = null },

                new MedicalDevice {Name = "Monofone de Comunicação", Type = "Comunicação",
                    Specification = "Comunicação interna wireless.", RegistrationDate = DateTime.Now,
                    Observation = null },

                new MedicalDevice {Name = "Ventilador Portátil V6", Type = "Suporte Vital",
                    Specification = "Modelo avançado para transporte.", RegistrationDate = DateTime.Now,
                    Observation = null },

                new MedicalDevice {Name = "Electrocardiógrafo ECG-1", Type = "Diagnóstico",
                    Specification = "12 canais, portátil com bateria.", RegistrationDate = DateTime.Now,
                    Observation = "Para consultas externas." },

                new MedicalDevice {Name = "Esfigmomanómetro Digital", Type = "Diagnóstico",
                    Specification = "Automático, memória para 50 medições.", RegistrationDate = DateTime.Now,
                    Observation = null },

                new MedicalDevice {Name = "Termómetro Infravermelho", Type = "Diagnóstico",
                    Specification = "Medição sem contato.", RegistrationDate = DateTime.Now,
                    Observation = "Uso geral em todos os gabinetes." },

                new MedicalDevice {Name = "Analisador de Gás Sanguíneo", Type = "Laboratorial",
                    Specification = "Portátil, resultados em 1 minuto.", RegistrationDate = DateTime.Now,
                    Observation = "Requer reagentes." },

                new MedicalDevice {Name = "Desfibrilhador DEA", Type = "Emergência",
                    Specification = "Automático, com guia por voz.", RegistrationDate = DateTime.Now,
                    Observation = "Localizado no carrinho de emergência." },

                new MedicalDevice {Name = "Foco Cirúrgico Móvel", Type = "Cirúrgico",
                    Specification = "Iluminação LED ajustável.", RegistrationDate = DateTime.Now,
                    Observation = "Para procedimentos menores." },
            });

            dbContext.SaveChanges();
        }
    }
}