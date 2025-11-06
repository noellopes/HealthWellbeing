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
            PopulateRooms(dbContext);
            PopulateTypeMaterial(dbContext);
            PopulateMedicalDevices(dbContext);
            PopulateManufacturer(dbContext);
            PolutateEquipmentTypes(dbContext);
            PolutateEquipmentStatus(dbContext);
            PolutateEquipment(dbContext);
        }

        private static void PopulateRooms(HealthWellbeingDbContext dbContext)
        {
            if (!dbContext.Room.Any())
            {
                var rooms = new List<Room>
                {
                    new Room { RoomsType = Room.RoomType.Consultas, Specialty = "Pediatria", Name = "Sala 01", Capacity = 5, Location = "Piso 1, Ala A", OperatingHours = "08:00 - 18:00", Status = Room.RoomStatus.Disponivel, Notes = "Atendimento infantil." },
                    new Room { RoomsType = Room.RoomType.Consultas, Specialty = "Cardiologia", Name = "Sala 02", Capacity = 4, Location = "Piso 1, Ala B", OperatingHours = "08:00 - 18:00", Status = Room.RoomStatus.Disponivel, Notes = "Atendimento cardiológico." },
                    new Room { RoomsType = Room.RoomType.Tratamentos, Specialty = "Fisioterapia", Name = "Sala 03", Capacity = 3, Location = "Piso 2, Ala C", OperatingHours = "09:00 - 19:00", Status = Room.RoomStatus.Disponivel, Notes = "Sala equipada." },
                    new Room { RoomsType = Room.RoomType.Tratamentos, Specialty = "Oncologia", Name = "Sala 04", Capacity = 2, Location = "Piso 2, Ala D", OperatingHours = "08:00 - 17:00", Status = Room.RoomStatus.Limpeza, Notes = "Limpeza agendada." },
                    new Room { RoomsType = Room.RoomType.Consultas, Specialty = "Ortopedia", Name = "Sala 05", Capacity = 6, Location = "Piso 1, Ala E", OperatingHours = "08:00 - 18:00", Status = Room.RoomStatus.Disponivel, Notes = "Especialista em ossos." },
                    new Room { RoomsType = Room.RoomType.Tratamentos, Specialty = "Dermatologia", Name = "Sala 06", Capacity = 4, Location = "Piso 2, Ala F", OperatingHours = "10:00 - 18:00", Status = Room.RoomStatus.Disponivel, Notes = "" },
                    new Room { RoomsType = Room.RoomType.Consultas, Specialty = "Psicologia", Name = "Sala 07", Capacity = 2, Location = "Piso 1, Ala G", OperatingHours = "09:00 - 18:00", Status = Room.RoomStatus.Disponivel, Notes = "" },
                    new Room { RoomsType = Room.RoomType.Tratamentos, Specialty = "Ginecologia", Name = "Sala 08", Capacity = 3, Location = "Piso 2, Ala H", OperatingHours = "08:00 - 16:00", Status = Room.RoomStatus.Disponivel, Notes = "" },
                    new Room { RoomsType = Room.RoomType.Consultas, Specialty = "Oftalmologia", Name = "Sala 09", Capacity = 2, Location = "Piso 1, Ala I", OperatingHours = "08:00 - 18:00", Status = Room.RoomStatus.Disponivel, Notes = "Material óptico disponível." },
                    new Room { RoomsType = Room.RoomType.Tratamentos, Specialty = "Nutrição", Name = "Sala 10", Capacity = 2, Location = "Piso 2, Ala J", OperatingHours = "09:30 - 16:30", Status = Room.RoomStatus.Disponivel, Notes = "" },
                    new Room { RoomsType = Room.RoomType.Consultas, Specialty = "Pneumologia", Name = "Sala 11", Capacity = 4, Location = "Piso 1, Ala K", OperatingHours = "08:30 - 18:00", Status = Room.RoomStatus.Disponivel, Notes = "" },
                    new Room { RoomsType = Room.RoomType.Tratamentos, Specialty = "Neurologia", Name = "Sala 12", Capacity = 2, Location = "Piso 2, Ala L", OperatingHours = "10:00 - 18:00", Status = Room.RoomStatus.Manutencao, Notes = "Equipamento em revisão." },
                    new Room { RoomsType = Room.RoomType.Consultas, Specialty = "Endocrinologia", Name = "Sala 13", Capacity = 3, Location = "Piso 1, Ala M", OperatingHours = "08:00 - 15:00", Status = Room.RoomStatus.Disponivel, Notes = "" },
                    new Room { RoomsType = Room.RoomType.Tratamentos, Specialty = "Reumatologia", Name = "Sala 14", Capacity = 3, Location = "Piso 2, Ala N", OperatingHours = "09:00 - 18:00", Status = Room.RoomStatus.Disponivel, Notes = "" },
                    new Room { RoomsType = Room.RoomType.Consultas, Specialty = "Otorrinolaringologia", Name = "Sala 15", Capacity = 2, Location = "Piso 1, Ala O", OperatingHours = "10:00 - 17:00", Status = Room.RoomStatus.ForaDeServico, Notes = "Sem uso temporário." },
                    new Room { RoomsType = Room.RoomType.Tratamentos, Specialty = "Urologia", Name = "Sala 16", Capacity = 3, Location = "Piso 2, Ala P", OperatingHours = "08:00 - 16:00", Status = Room.RoomStatus.Disponivel, Notes = "" },
                    new Room { RoomsType = Room.RoomType.Consultas, Specialty = "Gastroenterologia", Name = "Sala 17", Capacity = 4, Location = "Piso 1, Ala Q", OperatingHours = "08:00 - 16:00", Status = Room.RoomStatus.Disponivel, Notes = "" },
                    new Room { RoomsType = Room.RoomType.Tratamentos, Specialty = "Hematologia", Name = "Sala 18", Capacity = 2, Location = "Piso 2, Ala R", OperatingHours = "09:00 - 17:00", Status = Room.RoomStatus.Disponivel, Notes = "" },
                    new Room { RoomsType = Room.RoomType.Consultas, Specialty = "Nefrologia", Name = "Sala 19", Capacity = 2, Location = "Piso 1, Ala S", OperatingHours = "10:00 - 18:00", Status = Room.RoomStatus.Disponivel, Notes = "" },
                    new Room { RoomsType = Room.RoomType.Tratamentos, Specialty = "Oncologia Pediátrica", Name = "Sala 20", Capacity = 2, Location = "Piso 2, Ala T", OperatingHours = "08:00 - 13:00", Status = Room.RoomStatus.Disponivel, Notes = "" },
                    new Room { RoomsType = Room.RoomType.Consultas, Specialty = "Imunologia", Name = "Sala 21", Capacity = 3, Location = "Piso 1, Ala U", OperatingHours = "08:00 - 18:00", Status = Room.RoomStatus.Disponivel, Notes = "" },
                    new Room { RoomsType = Room.RoomType.Tratamentos, Specialty = "Psiquiatria", Name = "Sala 22", Capacity = 2, Location = "Piso 2, Ala V", OperatingHours = "08:00 - 18:00", Status = Room.RoomStatus.Disponivel, Notes = "" },
                    new Room { RoomsType = Room.RoomType.Consultas, Specialty = "Maternidade", Name = "Sala 23", Capacity = 6, Location = "Piso 1, Ala W", OperatingHours = "08:00 - 20:00", Status = Room.RoomStatus.Disponivel, Notes = "" },
                    new Room { RoomsType = Room.RoomType.Tratamentos, Specialty = "Transplante", Name = "Sala 24", Capacity = 2, Location = "Piso 2, Ala X", OperatingHours = "08:00 - 17:00", Status = Room.RoomStatus.Disponivel, Notes = "" },
                    new Room { RoomsType = Room.RoomType.Consultas, Specialty = "Odontologia", Name = "Sala 25", Capacity = 3, Location = "Piso 1, Ala Y", OperatingHours = "10:00 - 17:00", Status = Room.RoomStatus.Disponivel, Notes = "" },
                    new Room { RoomsType = Room.RoomType.Tratamentos, Specialty = "Cirurgia Geral", Name = "Sala 26", Capacity = 2, Location = "Piso 2, Ala Z", OperatingHours = "09:00 - 17:00", Status = Room.RoomStatus.Disponivel, Notes = "" },
                    new Room { RoomsType = Room.RoomType.Consultas, Specialty = "Otorrinolaringologia", Name = "Sala 27", Capacity = 2, Location = "Piso 1, Ala AA", OperatingHours = "08:00 - 16:00", Status = Room.RoomStatus.Disponivel, Notes = "" },
                    new Room { RoomsType = Room.RoomType.Tratamentos, Specialty = "Anestesiologia", Name = "Sala 28", Capacity = 2, Location = "Piso 2, Ala BB", OperatingHours = "08:00 - 14:00", Status = Room.RoomStatus.Disponivel, Notes = "" },
                    new Room { RoomsType = Room.RoomType.Consultas, Specialty = "Radiologia", Name = "Sala 29", Capacity = 3, Location = "Piso 1, Ala CC", OperatingHours = "08:00 - 18:00", Status = Room.RoomStatus.Manutencao, Notes = "Equipamento em manutenção." },
                    new Room { RoomsType = Room.RoomType.Tratamentos, Specialty = "Reabilitação Cardíaca", Name = "Sala 30", Capacity = 2, Location = "Piso 2, Ala DD", OperatingHours = "08:30 - 16:30", Status = Room.RoomStatus.Disponivel, Notes = "" },
                    new Room { RoomsType = Room.RoomType.Consultas, Specialty = "Vascular", Name = "Sala 31", Capacity = 3, Location = "Piso 1, Ala EE", OperatingHours = "09:00 - 17:00", Status = Room.RoomStatus.Disponivel, Notes = "" },
                };

                dbContext.Room.AddRange(rooms);
                dbContext.SaveChanges();
            }
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