using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeingRoom.Models;
using Microsoft.AspNetCore.Identity;


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
            PopulateSpeciality(dbContext);
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

        private static void PopulateSpeciality(HealthWellbeingDbContext dbContext)
        {
            if (!dbContext.Specialty.Any())
            {
            var specialty = new List<Specialty>
            {
                new Specialty { Name = "Atendimento Ambulatorial", Description = "Atendimento médico e realização de consultas e procedimentos para pacientes que não necessitam de internação." },
                new Specialty { Name = "Cuidados Intensivos", Description = "Assistência especializada e monitorização contínua de pacientes em estado crítico, garantindo suporte avançado às funções vitais." },
                new Specialty { Name = "Procedimentos Cirúrgicos", Description = "Execução de intervenções cirúrgicas em ambiente controlado para tratamento de doenças e lesões." },
                new Specialty { Name = "Exames Clínicos", Description = "Realização de avaliações e testes clínicos para diagnóstico de diferentes condições de saúde." },
                new Specialty { Name = "Análises Clínicas", Description = "Processamento de amostras laboratoriais para análise bioquímica, microbiológica e hematológica visando diagnósticos precisos." },
                new Specialty { Name = "Gestão de Medicamentos", Description = "Coordenação da prescrição, armazenamento e administração segura de medicamentos aos pacientes." },
                new Specialty { Name = "Armazenamento", Description = "Organização, controle e conservação de materiais médicos, medicamentos e equipamentos hospitalares." },
                new Specialty { Name = "Pós-Operatório", Description = "Monitorização e cuidados prestados a pacientes após cirurgia para garantir recuperação segura e eficaz." },
                new Specialty { Name = "Atendimento Crítico", Description = "Atendimento emergencial e intervenções imediatas em situações de risco de vida ou instabilidade clínica grave." },
                new Specialty { Name = "Higienização de Instrumentos", Description = "Processos rigorosos de limpeza, desinfecção e esterilização de instrumentos cirúrgicos e equipamentos para prevenção de infecções." },
                new Specialty { Name = "Exames de Imagem", Description = "Exames de imagens" },
                new Specialty { Name = "Exames Cardiológicos", Description = "Exames de imagens cardiologia" }

                }
            ;
                dbContext.Specialty.AddRange(specialty);
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
                new RoomStatus { Name = "Criado", Description = "Sala acabada de ser criada" },
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
                new RoomType { Name = "Consultas", Description = "Sala para atendimento ambulatorial" },//Ter especialidade
                new RoomType { Name = "Unidade de Terapia Intensiva (UTI)", Description = "Espaço para cuidados intensivos de pacientes críticos" },//Ter especialidade
                new RoomType { Name = "Centro Cirúrgico", Description = "Sala destinada a procedimentos cirúrgicos" },//Ter especialidade
                new RoomType { Name = "Exames", Description = "Espaço para exames clínicos e de imagem" },//Ter especialidade
                new RoomType { Name = "Laboratório de A. Clínicas", Description = "Sala para realização de análises laboratoriais" },
                new RoomType { Name = "Farmácia Hospitalar", Description = "Armazenamento e dispensação de medicamentos" },
                new RoomType { Name = "Depósito Hospitalar", Description = "Espaço para armazenamento de materiais e insumos" },
                new RoomType { Name = "Recuperação Pós-Operatória", Description = "Espaço para pacientes em pós-operatório imediato" },//Ter especialidade
                new RoomType { Name = "Emergência", Description = "Área destinada ao atendimento rápido de casos críticos" },//Ter especialidade
                new RoomType { Name = "Esterilização", Description = "Ambiente para higienização e esterilização de instrumentos médicos" },
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
                // Bloco A
                new RoomLocation { Name = "BlocoA-Ala1-Z.N" }, // Norte
                new RoomLocation { Name = "BlocoA-Ala1-Z.S" }, // Sul
                new RoomLocation { Name = "BlocoA-Ala1-Z.L" }, // Leste
                new RoomLocation { Name = "BlocoA-Ala1-Z.O" }, // Oeste

                new RoomLocation { Name = "BlocoA-Ala2-Z.N" },
                new RoomLocation { Name = "BlocoA-Ala2-Z.S" },
                new RoomLocation { Name = "BlocoA-Ala2-Z.L" },
                new RoomLocation { Name = "BlocoA-Ala2-Z.O" },

                new RoomLocation { Name = "BlocoA-Ala3-Z.N" },
                new RoomLocation { Name = "BlocoA-Ala3-Z.S" },
                new RoomLocation { Name = "BlocoA-Ala3-Z.L" },
                new RoomLocation { Name = "BlocoA-Ala3-Z.O" },

                // Bloco B
                new RoomLocation { Name = "BlocoB-Ala1-Z.N" },
                new RoomLocation { Name = "BlocoB-Ala1-Z.S" },
                new RoomLocation { Name = "BlocoB-Ala1-Z.L" },
                new RoomLocation { Name = "BlocoB-Ala1-Z.O" },

                new RoomLocation { Name = "BlocoB-Ala2-Z.N" },
                new RoomLocation { Name = "BlocoB-Ala2-Z.S" },
                new RoomLocation { Name = "BlocoB-Ala2-Z.L" },
                new RoomLocation { Name = "BlocoB-Ala2-Z.O" },

                new RoomLocation { Name = "BlocoB-Ala3-Z.N" },
                new RoomLocation { Name = "BlocoB-Ala3-Z.S" },
                new RoomLocation { Name = "BlocoB-Ala3-Z.L" },
                new RoomLocation { Name = "BlocoB-Ala3-Z.O" },

                // Bloco C
                new RoomLocation { Name = "BlocoC-Ala1-Z.N" },
                new RoomLocation { Name = "BlocoC-Ala1-Z.S" },
                new RoomLocation { Name = "BlocoC-Ala1-Z.L" },
                new RoomLocation { Name = "BlocoC-Ala1-Z.O" },

                new RoomLocation { Name = "BlocoC-Ala2-Z.N" },
                new RoomLocation { Name = "BlocoC-Ala2-Z.S" },
                new RoomLocation { Name = "BlocoC-Ala2-Z.L" },
                new RoomLocation { Name = "BlocoC-Ala2-Z.O" },

                new RoomLocation { Name = "BlocoC-Ala3-Z.N" },
                new RoomLocation { Name = "BlocoC-Ala3-Z.S" },
                new RoomLocation { Name = "BlocoC-Ala3-Z.L" },
                new RoomLocation { Name = "BlocoC-Ala3-Z.O" },

                new RoomLocation { Name = "BlocoC-Ala4-Z.N" },
                new RoomLocation { Name = "BlocoC-Ala4-Z.S" },
                new RoomLocation { Name = "BlocoC-Ala4-Z.L" },//Nao contem nenhuma sala ainda
                new RoomLocation { Name = "BlocoC-Ala4-Z.O" }
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
                var specialty = dbContext.Specialty.ToDictionary(s => s.Name, s => s.SpecialtyId);
                var roomStatus = dbContext.RoomStatus.ToDictionary(r => r.Name, r => r.RoomStatusId);
                var roomType = dbContext.RoomType.ToDictionary(t => t.Name, t => t.RoomTypeId);
                var roomLocation = dbContext.RoomLocation.ToDictionary(l => l.Name, l => l.RoomLocationId);

                var rooms = new List<Room>
                {
                    // Consultas
                    new Room { Name = "Sala de Consultas 1", SpecialtyId = specialty["Atendimento Ambulatorial"], RoomTypeId = roomType["Consultas"], RoomLocationId = roomLocation["BlocoA-Ala1-Z.N"], RoomStatusId = roomStatus["Disponível"], OpeningTime = TimeSpan.Parse("08:00"), ClosingTime = TimeSpan.Parse("18:00"), Notes = "Consultas gerais." },
                    new Room { Name = "Sala de Consultas 2", SpecialtyId = specialty["Atendimento Ambulatorial"], RoomTypeId = roomType["Consultas"], RoomLocationId = roomLocation["BlocoA-Ala1-Z.S"], RoomStatusId = roomStatus["Indisponível"], OpeningTime = TimeSpan.Parse("08:00"), ClosingTime = TimeSpan.Parse("18:00"), Notes = "Atendimento pediátrico." },
                    new Room { Name = "Sala de Consultas 3", SpecialtyId = specialty["Atendimento Ambulatorial"], RoomTypeId = roomType["Consultas"], RoomLocationId = roomLocation["BlocoA-Ala1-Z.L"], RoomStatusId = roomStatus["Em Manutenção"], OpeningTime = TimeSpan.Parse("09:00"), ClosingTime = TimeSpan.Parse("17:00"), Notes = "Consultas de especialidade." },
                    new Room { Name = "Sala de Consultas 4", SpecialtyId = specialty["Atendimento Ambulatorial"], RoomTypeId = roomType["Consultas"], RoomLocationId = roomLocation["BlocoA-Ala1-Z.O"], RoomStatusId = roomStatus["Disponível"], OpeningTime = TimeSpan.Parse("08:00"), ClosingTime = TimeSpan.Parse("18:00"), Notes = "Consultas dermatológicas." },
                    new Room { Name = "Sala de Consultas 5", SpecialtyId = specialty["Atendimento Ambulatorial"], RoomTypeId = roomType["Consultas"], RoomLocationId = roomLocation["BlocoA-Ala1-Z.N"], RoomStatusId = roomStatus["Indisponível"], OpeningTime = TimeSpan.Parse("08:00"), ClosingTime = TimeSpan.Parse("18:00"), Notes = "Consultas ginecológicas." },

                    // UTI
                    new Room { Name = "UTI 1", SpecialtyId = specialty["Cuidados Intensivos"], RoomTypeId = roomType["Unidade de Terapia Intensiva (UTI)"], RoomLocationId = roomLocation["BlocoA-Ala2-Z.N"], RoomStatusId = roomStatus["Disponível"], OpeningTime = TimeSpan.Parse("00:00"), ClosingTime = TimeSpan.Parse("23:59"), Notes = "Monitorização contínua." },
                    new Room { Name = "UTI 2", SpecialtyId = specialty["Cuidados Intensivos"], RoomTypeId = roomType["Unidade de Terapia Intensiva (UTI)"], RoomLocationId = roomLocation["BlocoA-Ala2-Z.S"], RoomStatusId = roomStatus["Indisponível"], OpeningTime = TimeSpan.Parse("00:00"), ClosingTime = TimeSpan.Parse("23:59"), Notes = "Equipamentos de suporte vital." },
                    new Room { Name = "UTI 3", SpecialtyId = specialty["Cuidados Intensivos"], RoomTypeId = roomType["Unidade de Terapia Intensiva (UTI)"], RoomLocationId = roomLocation["BlocoA-Ala2-Z.L"], RoomStatusId = roomStatus["Em Manutenção"], OpeningTime = TimeSpan.Parse("00:00"), ClosingTime = TimeSpan.Parse("23:59"), Notes = "Isolamento de pacientes críticos." },
                    new Room { Name = "UTI 4", SpecialtyId = specialty["Cuidados Intensivos"], RoomTypeId = roomType["Unidade de Terapia Intensiva (UTI)"], RoomLocationId = roomLocation["BlocoA-Ala2-Z.O"], RoomStatusId = roomStatus["Disponível"], OpeningTime = TimeSpan.Parse("00:00"), ClosingTime = TimeSpan.Parse("23:59"), Notes = "UTI neonatal." },

                    // Centro Cirúrgico
                    new Room { Name = "Centro Cirúrgico 1", SpecialtyId = specialty["Procedimentos Cirúrgicos"], RoomTypeId = roomType["Centro Cirúrgico"], RoomLocationId = roomLocation["BlocoA-Ala3-Z.N"], RoomStatusId = roomStatus["Indisponível"], OpeningTime = TimeSpan.Parse("07:00"), ClosingTime = TimeSpan.Parse("19:00"), Notes = "Cirurgias gerais." },
                    new Room { Name = "Centro Cirúrgico 2", SpecialtyId = specialty["Procedimentos Cirúrgicos"], RoomTypeId = roomType["Centro Cirúrgico"], RoomLocationId = roomLocation["BlocoA-Ala3-Z.S"], RoomStatusId = roomStatus["Em Limpeza"], OpeningTime = TimeSpan.Parse("07:00"), ClosingTime = TimeSpan.Parse("19:00"), Notes = "Cirurgias ortopédicas." },
                    new Room { Name = "Centro Cirúrgico 3", SpecialtyId = specialty["Procedimentos Cirúrgicos"], RoomTypeId = roomType["Centro Cirúrgico"], RoomLocationId = roomLocation["BlocoA-Ala3-Z.L"], RoomStatusId = roomStatus["Disponível"], OpeningTime = TimeSpan.Parse("07:00"), ClosingTime = TimeSpan.Parse("19:00"), Notes = "Preparação para cirurgia cardíaca." },

                    // Sala de Exames
                    new Room { Name = "Sala de Exames 1", SpecialtyId = specialty["Exames Clínicos"], RoomTypeId = roomType["Exames"], RoomLocationId = roomLocation["BlocoB-Ala1-Z.N"], RoomStatusId = roomStatus["Disponível"], OpeningTime = TimeSpan.Parse("08:00"), ClosingTime = TimeSpan.Parse("20:00"), Notes = "Exames laboratoriais básicos." },
                    new Room { Name = "Sala de Exames 2", SpecialtyId = specialty["Exames de Imagem"], RoomTypeId = roomType["Exames"], RoomLocationId = roomLocation["BlocoB-Ala1-Z.S"], RoomStatusId = roomStatus["Indisponível"], OpeningTime = TimeSpan.Parse("08:00"), ClosingTime = TimeSpan.Parse("20:00"), Notes = "Raio-X e ultrassonografia." },
                    new Room { Name = "Sala de Exames 3", SpecialtyId =specialty[ "Exames Cardiológicos"], RoomTypeId = roomType["Exames"], RoomLocationId = roomLocation["BlocoB-Ala1-Z.L"], RoomStatusId = roomStatus["Disponível"], OpeningTime = TimeSpan.Parse("08:00"), ClosingTime = TimeSpan.Parse("20:00"), Notes = "Eletrocardiograma e ecocardiograma." },

                    // Laboratórios
                    new Room { Name = "Laboratório 1", SpecialtyId = specialty["Análises Clínicas"], RoomTypeId = roomType["Laboratório de A. Clínicas"], RoomLocationId = roomLocation["BlocoB-Ala2-Z.O"], RoomStatusId = roomStatus["Disponível"], OpeningTime = TimeSpan.Parse("06:00"), ClosingTime = TimeSpan.Parse("22:00"), Notes = "Bioquímica e hematologia." },
                    new Room { Name = "Laboratório 2", SpecialtyId =specialty["Análises Clínicas"], RoomTypeId = roomType["Laboratório de A. Clínicas"], RoomLocationId = roomLocation["BlocoB-Ala2-Z.N"], RoomStatusId = roomStatus["Em Limpeza"], OpeningTime = TimeSpan.Parse("06:00"), ClosingTime = TimeSpan.Parse("22:00"), Notes = "Microbiologia." },
                    new Room { Name = "Laboratório 3", SpecialtyId = specialty["Análises Clínicas"], RoomTypeId = roomType["Laboratório de A. Clínicas"], RoomLocationId = roomLocation["BlocoB-Ala2-Z.S"], RoomStatusId = roomStatus["Indisponível"], OpeningTime = TimeSpan.Parse("06:00"), ClosingTime = TimeSpan.Parse("22:00"), Notes = "Imunologia e patologia clínica." },
                    new Room { Name = "Laboratório 4", SpecialtyId = specialty["Análises Clínicas"], RoomTypeId = roomType["Laboratório de A. Clínicas"], RoomLocationId = roomLocation["BlocoB-Ala2-Z.L"], RoomStatusId = roomStatus["Disponível"], OpeningTime = TimeSpan.Parse("06:00"), ClosingTime = TimeSpan.Parse("22:00"), Notes = "Controle de qualidade laboratorial." },

                    // Farmácia
                    new Room { Name = "Farmácia 1", SpecialtyId = specialty["Gestão de Medicamentos"], RoomTypeId = roomType["Farmácia Hospitalar"], RoomLocationId = roomLocation["BlocoB-Ala3-Z.O"], RoomStatusId = roomStatus["Disponível"], OpeningTime = TimeSpan.Parse("08:00"), ClosingTime = TimeSpan.Parse("18:00"), Notes = "Dispensação de medicamentos." },
                    new Room { Name = "Farmácia 2", SpecialtyId = specialty["Gestão de Medicamentos"], RoomTypeId = roomType["Farmácia Hospitalar"], RoomLocationId = roomLocation["BlocoB-Ala3-Z.N"], RoomStatusId = roomStatus["Indisponível"], OpeningTime = TimeSpan.Parse("08:00"), ClosingTime = TimeSpan.Parse("18:00"), Notes = "Controle de estoque." },
                    new Room { Name = "Farmácia 3", SpecialtyId = specialty["Gestão de Medicamentos"], RoomTypeId = roomType["Farmácia Hospitalar"], RoomLocationId = roomLocation["BlocoB-Ala3-Z.S"], RoomStatusId = roomStatus["Em Limpeza"], OpeningTime = TimeSpan.Parse("08:00"), ClosingTime = TimeSpan.Parse("18:00"), Notes = "Medicamentos especiais." },

                    // Depósito
                    new Room { Name = "Depósito 1", SpecialtyId = specialty["Armazenamento"], RoomTypeId = roomType["Depósito Hospitalar"], RoomLocationId = roomLocation["BlocoC-Ala1-Z.L"], RoomStatusId = roomStatus["Disponível"], OpeningTime = TimeSpan.Parse("00:00"), ClosingTime = TimeSpan.Parse("23:59"), Notes = "Materiais cirúrgicos." },
                    new Room { Name = "Depósito 2", SpecialtyId = specialty["Armazenamento"], RoomTypeId = roomType["Depósito Hospitalar"], RoomLocationId = roomLocation["BlocoC-Ala1-Z.O"], RoomStatusId = roomStatus["Em Manutenção"], OpeningTime = TimeSpan.Parse("00:00"), ClosingTime = TimeSpan.Parse("23:59"), Notes = "Revisão de estoque." },
                    new Room { Name = "Depósito 3", SpecialtyId = specialty["Armazenamento"], RoomTypeId = roomType["Depósito Hospitalar"], RoomLocationId = roomLocation["BlocoC-Ala1-Z.N"], RoomStatusId = roomStatus["Indisponível"], OpeningTime = TimeSpan.Parse("00:00"), ClosingTime = TimeSpan.Parse("23:59"), Notes = "Insumos hospitalares." },

                    // Recuperação
                    new Room { Name = "Sala de Recuperação 1", SpecialtyId = specialty["Pós-Operatório"], RoomTypeId = roomType["Recuperação Pós-Operatória"], RoomLocationId = roomLocation["BlocoC-Ala2-Z.S"], RoomStatusId = roomStatus["Disponível"], OpeningTime = TimeSpan.Parse("00:00"), ClosingTime = TimeSpan.Parse("23:59"), Notes = "Monitorização pós-cirúrgica." },
                    new Room { Name = "Sala de Recuperação 2", SpecialtyId = specialty["Pós-Operatório"], RoomTypeId = roomType["Recuperação Pós-Operatória"], RoomLocationId = roomLocation["BlocoC-Ala2-Z.L"], RoomStatusId = roomStatus["Indisponível"], OpeningTime = TimeSpan.Parse("00:00"), ClosingTime = TimeSpan.Parse("23:59"), Notes = "Pacientes em observação." },
                    new Room { Name = "Sala de Recuperação 3", SpecialtyId = specialty["Pós-Operatório"], RoomTypeId = roomType["Recuperação Pós-Operatória"], RoomLocationId = roomLocation["BlocoC-Ala2-Z.O"], RoomStatusId = roomStatus["Em Limpeza"], OpeningTime = TimeSpan.Parse("00:00"), ClosingTime = TimeSpan.Parse("23:59"), Notes = "Revisão de equipamentos." },

                    // Emergência
                    new Room { Name = "Sala de Emergência 1", SpecialtyId = specialty["Atendimento Crítico"], RoomTypeId = roomType["Emergência"], RoomLocationId = roomLocation["BlocoC-Ala3-Z.N"], RoomStatusId = roomStatus["Disponível"], OpeningTime = TimeSpan.Parse("00:00"), ClosingTime = TimeSpan.Parse("23:59"), Notes = "Atendimento imediato." },
                    new Room { Name = "Sala de Emergência 2", SpecialtyId = specialty["Atendimento Crítico"], RoomTypeId = roomType["Emergência"], RoomLocationId = roomLocation["BlocoC-Ala3-Z.S"], RoomStatusId = roomStatus["Indisponível"], OpeningTime = TimeSpan.Parse("00:00"), ClosingTime = TimeSpan.Parse("23:59"), Notes = "Pacientes em estado grave." },
                    new Room { Name = "Sala de Emergência 3", SpecialtyId = specialty["Atendimento Crítico"], RoomTypeId = roomType["Emergência"], RoomLocationId = roomLocation["BlocoC-Ala3-Z.L"], RoomStatusId = roomStatus["Em Manutenção"], OpeningTime = TimeSpan.Parse("00:00"), ClosingTime = TimeSpan.Parse("23:59"), Notes = "Triagem de urgência." },

                    // Esterilização
                    new Room { Name = "Sala de Esterilização 1", SpecialtyId =specialty["Higienização de Instrumentos"], RoomTypeId = roomType["Esterilização"], RoomLocationId = roomLocation["BlocoC-Ala4-Z.O"], RoomStatusId = roomStatus["Disponível"], OpeningTime = TimeSpan.Parse("06:00"), ClosingTime = TimeSpan.Parse("22:00"), Notes = "Esterilização de instrumentos cirúrgicos." },
                    new Room { Name = "Sala de Esterilização 2", SpecialtyId = specialty["Higienização de Instrumentos"], RoomTypeId = roomType["Esterilização"], RoomLocationId = roomLocation["BlocoC-Ala4-Z.N"], RoomStatusId = roomStatus["Indisponível"], OpeningTime = TimeSpan.Parse("06:00"), ClosingTime = TimeSpan.Parse("22:00"), Notes = "Processamento de materiais." },
                    new Room { Name = "Sala de Esterilização 3", SpecialtyId = specialty["Higienização de Instrumentos"], RoomTypeId = roomType["Esterilização"], RoomLocationId = roomLocation["BlocoC-Ala4-Z.S"], RoomStatusId = roomStatus["Em Limpeza"], OpeningTime = TimeSpan.Parse("06:00"), ClosingTime = TimeSpan.Parse("22:00"), Notes = "Controle de qualidade." }

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
            // Se já existirem dados, não faz nada
            if (dbContext.MedicalDevices.Any()) return;

            dbContext.MedicalDevices.AddRange(new List<MedicalDevice>
            {
                new MedicalDevice {
                    Name = "Monitor Philips IntelliVue X3",
                    SerialNumber = "MT-PHI-X3-01",
                    RegistrationDate = DateTime.Now.AddMonths(-12),
                    Observation = "Alocado à UCI 1. Monitorização contínua.",
                    TypeMaterialID = 1,
                    IsUnderMaintenance = false
                },

                new MedicalDevice {
                    Name = "Bomba B.Braun Space",
                    SerialNumber = "INF-BB-SP-02",
                    RegistrationDate = DateTime.Now.AddMonths(-10),
                    Observation = "Uso geral na Enfermaria.",
                    TypeMaterialID = 2,
                    IsUnderMaintenance = false
                },

                new MedicalDevice {
                    Name = "Ventilador Dräger Oxylog 3000",
                    SerialNumber = "VEN-DRA-OX-03",
                    RegistrationDate = DateTime.Now.AddMonths(-24),
                    Observation = "Transporte de emergência (INEM).",
                    TypeMaterialID = 3,
                    IsUnderMaintenance = false
                },

                new MedicalDevice {
                    Name = "Aspirador Medela Vario 18",
                    SerialNumber = "ASP-MED-VAR-04",
                    RegistrationDate = DateTime.Now.AddMonths(-6),
                    Observation = "Pequena cirurgia e cuidados domiciliários.",
                    TypeMaterialID = 4,
                    IsUnderMaintenance = false
                },

                new MedicalDevice {
                    Name = "Eletrocardiógrafo GE MAC 600",
                    SerialNumber = "ECG-GE-MAC-05",
                    RegistrationDate = DateTime.Now.AddMonths(-18),
                    Observation = "Consultas de Cardiologia. Leve e compacto.",
                    TypeMaterialID = 5,
                    IsUnderMaintenance = false
                },

                new MedicalDevice {
                    Name = "Ecógrafo Sonosite Edge II",
                    SerialNumber = "ECO-SON-ED-06",
                    RegistrationDate = DateTime.Now.AddMonths(-5),
                    Observation = "Usado em Obstetrícia e Urgência.",
                    TypeMaterialID = 6,
                    IsUnderMaintenance = false
                },

                new MedicalDevice {
                    Name = "Glicosímetro Accu-Chek Guide",
                    SerialNumber = "GLI-ACC-GU-07",
                    RegistrationDate = DateTime.Now.AddDays(-15),
                    Observation = "Unidade de reserva no Depósito 1.",
                    TypeMaterialID = 7,
                    IsUnderMaintenance = false
                },

                new MedicalDevice {
                    Name = "Oxímetro Nonin Onyx Vantage",
                    SerialNumber = "OXI-NON-ON-08",
                    RegistrationDate = DateTime.Now.AddMonths(-8),
                    Observation = "Triagem inicial na Receção.",
                    TypeMaterialID = 8,
                    IsUnderMaintenance = false
                },

                new MedicalDevice {
                    Name = "Nebulizador Omron MicroAir",
                    SerialNumber = "NEB-OMR-U1-09",
                    RegistrationDate = DateTime.Now.AddMonths(-3),
                    Observation = "Pediatria. Silencioso e portátil.",
                    TypeMaterialID = 9,
                    IsUnderMaintenance = false
                },

                new MedicalDevice {
                    Name = "Desfibrilhador Zoll AED Plus",
                    SerialNumber = "AED-ZOL-PL-10",
                    RegistrationDate = DateTime.Now.AddMonths(-30),
                    Observation = "Corredor Principal - Entrada.",
                    TypeMaterialID = 10,
                    IsUnderMaintenance = false
                },

                new MedicalDevice {
                    Name = "Bomba Seringa Alaris GH",
                    SerialNumber = "SER-ALA-GH-11",
                    RegistrationDate = DateTime.Now.AddMonths(-14),
                    Observation = "Anestesia e Cuidados Intensivos.",
                    TypeMaterialID = 11,
                    IsUnderMaintenance = false
                },

                new MedicalDevice {
                    Name = "Otoscópio Welch Allyn MacroView",
                    SerialNumber = "OTO-WAL-MV-12",
                    RegistrationDate = DateTime.Now.AddMonths(-20),
                    Observation = "Consultório de Otorrino.",
                    TypeMaterialID = 12,
                    IsUnderMaintenance = false
                },

                new MedicalDevice {
                    Name = "Retinógrafo Welch Allyn RetinaVue",
                    SerialNumber = "RET-WAL-70-13",
                    RegistrationDate = DateTime.Now.AddMonths(-1),
                    Observation = "Rastreio oftalmológico móvel.",
                    TypeMaterialID = 13,
                    IsUnderMaintenance = false
                },

                new MedicalDevice {
                    Name = "Tensiómetro Omron M7 Intelli IT",
                    SerialNumber = "TEN-OMR-M7-14",
                    RegistrationDate = DateTime.Now.AddMonths(-4),
                    Observation = "Carrinho de enfermagem (Piso 2).",
                    TypeMaterialID = 14,
                    IsUnderMaintenance = false
                },

                new MedicalDevice {
                    Name = "Capnógrafo Masimo EMMA",
                    SerialNumber = "CAP-MAS-EM-15",
                    RegistrationDate = DateTime.Now.AddMonths(-9),
                    Observation = "Verificação de entubação em emergência.",
                    TypeMaterialID = 15,
                    IsUnderMaintenance = false
                },

                new MedicalDevice {
                    Name = "Holter Mortara H3+",
                    SerialNumber = "HOL-MOR-H3-16",
                    RegistrationDate = DateTime.Now.AddMonths(-7),
                    Observation = "Em uso por paciente externo (Cardiologia).",
                    TypeMaterialID = 16,
                    IsUnderMaintenance = false
                },

                new MedicalDevice {
                    Name = "Termómetro Braun ThermoScan 7",
                    SerialNumber = "TER-BRA-TS-17",
                    RegistrationDate = DateTime.Now.AddMonths(-2),
                    Observation = "Triagem Covid/Gripe.",
                    TypeMaterialID = 17,
                    IsUnderMaintenance = false
                },

                new MedicalDevice {
                    Name = "Philips Respironics V60",
                    SerialNumber = "VNI-PHI-V60-18",
                    RegistrationDate = DateTime.Now.AddMonths(-11),
                    Observation = "Apoio respiratório intermédio.",
                    TypeMaterialID = 18,
                    IsUnderMaintenance = true // Exemplo em manutenção
                },

                new MedicalDevice {
                    Name = "Transmissor Telemetria Dräger Apex",
                    SerialNumber = "TEL-DRA-AP-19",
                    RegistrationDate = DateTime.Now.AddMonths(-16),
                    Observation = "Monitorização de pacientes ambulatórios.",
                    TypeMaterialID = 19,
                    IsUnderMaintenance = false
                },

                new MedicalDevice {
                    Name = "Thor UVC Disinfection Robot",
                    SerialNumber = "UVC-THO-RB-20",
                    RegistrationDate = DateTime.Now.AddMonths(-1),
                    Observation = "Desinfeção de salas cirúrgicas e quartos.",
                    TypeMaterialID = 20,
                    IsUnderMaintenance = false
                }
            });

            dbContext.SaveChanges();
        }

        private static void PopulateLocationMedDevices(HealthWellbeingDbContext dbContext)
        {
            // Se já existirem localizações, não faz nada
            if (dbContext.LocationMedDevice.Any()) return;

            dbContext.LocationMedDevice.AddRange(new List<LocationMedDevice>
            {
                new LocationMedDevice { MedicalDeviceID = 1, RoomId = 6, InitialDate = DateTime.Now.AddMonths(-12), EndDate = null, IsCurrent = true },
                new LocationMedDevice { MedicalDeviceID = 2, RoomId = 26, InitialDate = DateTime.Now.AddMonths(-10), EndDate = null, IsCurrent = true },
                new LocationMedDevice { MedicalDeviceID = 3, RoomId = 29, InitialDate = DateTime.Now.AddMonths(-24), EndDate = null, IsCurrent = true },
                new LocationMedDevice { MedicalDeviceID = 4, RoomId = 13, InitialDate = DateTime.Now.AddMonths(-6), EndDate = null, IsCurrent = true },
                new LocationMedDevice { MedicalDeviceID = 5, RoomId = 1, InitialDate = DateTime.Now.AddMonths(-18), EndDate = null, IsCurrent = true },
                new LocationMedDevice { MedicalDeviceID = 6, RoomId = 5, InitialDate = DateTime.Now.AddMonths(-5), EndDate = null, IsCurrent = true },
                new LocationMedDevice { MedicalDeviceID = 7, RoomId = 23, InitialDate = DateTime.Now.AddDays(-15), EndDate = null, IsCurrent = true },
                new LocationMedDevice { MedicalDeviceID = 8, RoomId = 29, InitialDate = DateTime.Now.AddMonths(-8), EndDate = null, IsCurrent = true },
                new LocationMedDevice { MedicalDeviceID = 9, RoomId = 2, InitialDate = DateTime.Now.AddMonths(-3), EndDate = null, IsCurrent = true },
                new LocationMedDevice { MedicalDeviceID = 10, RoomId = 29, InitialDate = DateTime.Now.AddMonths(-30), EndDate = null, IsCurrent = true },
                new LocationMedDevice { MedicalDeviceID = 11, RoomId = 7, InitialDate = DateTime.Now.AddMonths(-14), EndDate = null, IsCurrent = true },
                new LocationMedDevice { MedicalDeviceID = 12, RoomId = 3, InitialDate = DateTime.Now.AddMonths(-20), EndDate = null, IsCurrent = true },
                new LocationMedDevice { MedicalDeviceID = 13, RoomId = 14, InitialDate = DateTime.Now.AddMonths(-1), EndDate = null, IsCurrent = true },
                new LocationMedDevice { MedicalDeviceID = 14, RoomId = 27, InitialDate = DateTime.Now.AddMonths(-4), EndDate = null, IsCurrent = true },
                new LocationMedDevice { MedicalDeviceID = 15, RoomId = 30, InitialDate = DateTime.Now.AddMonths(-9), EndDate = null, IsCurrent = true },
                new LocationMedDevice { MedicalDeviceID = 16, RoomId = 1, InitialDate = DateTime.Now.AddMonths(-7), EndDate = null, IsCurrent = true },
                new LocationMedDevice { MedicalDeviceID = 17, RoomId = 29, InitialDate = DateTime.Now.AddMonths(-2), EndDate = null, IsCurrent = true },
                // Lógica: Criamos um registo antigo que JÁ TERMINOU. Assim não aparece alocado, mas tem histórico.
                new LocationMedDevice { MedicalDeviceID = 18, RoomId = 6, InitialDate = DateTime.Now.AddMonths(-11), EndDate = DateTime.Now.AddDays(-1), IsCurrent = false },
                new LocationMedDevice { MedicalDeviceID = 19, RoomId = 26, InitialDate = DateTime.Now.AddMonths(-16), EndDate = null, IsCurrent = true },
                new LocationMedDevice { MedicalDeviceID = 20, RoomId = 23, InitialDate = DateTime.Now.AddMonths(-1), EndDate = null, IsCurrent = true }
            });

            dbContext.SaveChanges();
        }


        //Garante que ha um administrador no sistema, cria-o com a passe e atribuilhe o papel de "administrador".
        //(Cria um admin por defeito)
        internal static void SeedDefaultAdmin(UserManager<IdentityUser> userManager)
        {
            EnsureUserIsCreatedAsync(userManager, "admin@gsm.pt", "Admin1234_", ["Administrator"]).Wait();
        }

        //Recebe username passeword e lista de roles, verifica se o utilizador ja existe, senao cria-o
        //depois percorre os roles e garante que o utilizador esta associado a cada um addToRoleAsync
        //(Cria qualquer utilizador e atribui-lhe roles, se ainda nao existir)
        private static async Task EnsureUserIsCreatedAsync(UserManager<IdentityUser> userManager, string username, String password, string[] roles)
        {
            IdentityUser? user = await userManager.FindByNameAsync(username);

            if (user == null)
            {
                user = new IdentityUser(username);
                await userManager.CreateAsync(user, password);
            }

            foreach (var role in roles)
            {
                if (!await userManager.IsInRoleAsync(user, role))
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }

        //Usa o RoleManager<IdentityRole> para verificar se jaa existe um role com o nome passado, 
        //senao existir cria-o.
        //(Garante que um papel(role) existe no sistema de autenticacao.
        private static async Task EnsureRoleIsCreatedAsync(RoleManager<IdentityRole> roleManager, string role)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        //Cria os utilizadores iniciais
        //(Garante que esses utilizadores existem na base de dados e ja tem o papel correto atribuido)
        internal static void SeedUser(UserManager<IdentityUser> userManager)
        {
            EnsureUserIsCreatedAsync(userManager, "nuno@gsm.pt", "Nuno123_", ["logisticsTechnician"]).Wait();
            EnsureUserIsCreatedAsync(userManager, "evanilson@gsm.pt", "Evanilson123_", ["logisticsTechnician"]).Wait();
            EnsureUserIsCreatedAsync(userManager, "vila@gsm.pt", "Vila123_", ["logisticsTechnician"]).Wait();
            EnsureUserIsCreatedAsync(userManager, "leonel@gsm.pt", "Leonel123_", ["logisticsTechnician"]).Wait();
            EnsureUserIsCreatedAsync(userManager, "marcel@gsm.pt", "Marcel123_", ["logisticsTechnician"]).Wait();
        }

        //Cria o papel logisticsTechnician se ainda n exisstir.
        internal static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            EnsureRoleIsCreatedAsync(roleManager, "Administrator").Wait();
            EnsureRoleIsCreatedAsync(roleManager, "logisticsTechnician").Wait();
        }

    }
}
