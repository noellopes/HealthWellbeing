using HealthWellbeing.Models;

namespace HealthWellbeing.Data
{
    public class SeedDataGroup1
    {
        internal static void Populate(HealthWellbeingDbContext? HealthWellbeingDbContext)
        {
            if (HealthWellbeingDbContext == null) throw new ArgumentNullException(nameof(HealthWellbeingDbContext));

            HealthWellbeingDbContext.Database.EnsureCreated();

            PopulatePathology(HealthWellbeingDbContext);
            PopulateTreatmentType(HealthWellbeingDbContext);
            PopulateNurses(HealthWellbeingDbContext);
        }
        private static void PopulateTreatmentType(HealthWellbeingDbContext HealthWellbeingDbContext)
        {
            if (HealthWellbeingDbContext.TreatmentType.Any()) return;

            var tipoTratamentos = new[]
            {
                new TreatmentType { Name = "Fisioterapia Geral", Description = "Sessões de fisioterapia para recuperação muscular.", EstimatedDuration = 60, Priority = "Normal" },
                new TreatmentType { Name = "Massagem Relaxante", Description = "Massagem terapêutica para alívio de stress.", EstimatedDuration = 45, Priority = "Rotina" },
                new TreatmentType { Name = "Acupuntura", Description = "Tratamento com agulhas para equilíbrio energético.", EstimatedDuration = 40, Priority = "Normal" },
                new TreatmentType { Name = "Consulta de Nutrição", Description = "Avaliação nutricional e plano alimentar personalizado.", EstimatedDuration = 50, Priority = "Normal" },
                new TreatmentType { Name = "Terapia Ocupacional", Description = "Apoio para melhorar a autonomia e funções motoras.", EstimatedDuration = 60, Priority = "Urgente" },
                new TreatmentType { Name = "Psicoterapia Individual", Description = "Sessões com psicólogo para saúde mental e emocional.", EstimatedDuration = 50, Priority = "Normal" },
                new TreatmentType { Name = "Consulta de Medicina Geral", Description = "Avaliação médica geral e prescrição de tratamentos.", EstimatedDuration = 30, Priority = "Rotina" },
                new TreatmentType { Name = "Reabilitação Pós-Cirúrgica", Description = "Tratamento fisioterapêutico após cirurgias.", EstimatedDuration = 70, Priority = "Urgente" },
                new TreatmentType { Name = "Hidroterapia", Description = "Terapia física em ambiente aquático para reabilitação.", EstimatedDuration = 60, Priority = "Normal" },
                new TreatmentType { Name = "Consulta de Enfermagem", Description = "Avaliação de sinais vitais e administração de medicamentos.", EstimatedDuration = 20, Priority = "Rotina" },
                new TreatmentType { Name = "Sessão de Yoga Terapêutico", Description = "Exercícios de relaxamento e alongamento guiados.", EstimatedDuration = 55, Priority = "Rotina" },
                new TreatmentType { Name = "Terapia da Fala", Description = "Acompanhamento para melhorar a comunicação e fala.", EstimatedDuration = 40, Priority = "Normal" },
                new TreatmentType { Name = "Avaliação Postural", Description = "Análise da postura e correções personalizadas.", EstimatedDuration = 35, Priority = "Rotina" },
                new TreatmentType { Name = "Consulta de Cardiologia", Description = "Avaliação e acompanhamento de problemas cardíacos.", EstimatedDuration = 45, Priority = "Urgente" },
                new TreatmentType { Name = "Reiki", Description = "Sessão energética para equilíbrio emocional e físico.", EstimatedDuration = 50, Priority = "Rotina" },
                new TreatmentType { Name = "Consulta de Dermatologia", Description = "Avaliação e tratamento de problemas de pele.", EstimatedDuration = 30, Priority = "Normal" },
                new TreatmentType { Name = "Terapia Respiratória", Description = "Tratamentos para melhorar a função pulmonar.", EstimatedDuration = 45, Priority = "Urgente" },
                new TreatmentType { Name = "Consulta de Ortopedia", Description = "Avaliação de lesões e patologias ósseas ou musculares.", EstimatedDuration = 50, Priority = "Normal" },
                new TreatmentType { Name = "Massagem Desportiva", Description = "Tratamento para recuperação após treino intenso.", EstimatedDuration = 40, Priority = "Normal" },
                new TreatmentType { Name = "Consulta de Endocrinologia", Description = "Avaliação hormonal e metabólica.", EstimatedDuration = 45, Priority = "Normal" }

            };

            HealthWellbeingDbContext.TreatmentType.AddRange(tipoTratamentos);
            HealthWellbeingDbContext.SaveChanges();
        }
        private static void PopulateNurses(HealthWellbeingDbContext HealthWellbeingDbContext)
        {
            if (HealthWellbeingDbContext.Nurse.Any()) return;

            var enfermeiros = new[]
            {
                new Nurse { Name = "Carla Martins", NIF = "234567890", ProfessionalLicense = "ENF12345", BirthDate = new DateTime(1988, 3, 12), Email = "carla.martins@hospital.pt", Phone = "912345678", Specialty = "Cuidados Intensivos" },
                new Nurse { Name = "Rui Fernandes", NIF = "198765432", ProfessionalLicense = "ENF67890", BirthDate = new DateTime(1990, 7, 5), Email = "rui.fernandes@hospital.pt", Phone = "913456789", Specialty = "Pediatria" },
                new Nurse { Name = "Ana Costa", NIF = "245678901", ProfessionalLicense = "ENF11223", BirthDate = new DateTime(1985, 11, 21), Email = "ana.costa@hospital.pt", Phone = "914567890", Specialty = "Saúde Materna" },
                new Nurse { Name = "Miguel Rocha", NIF = "223344556", ProfessionalLicense = "ENF33445", BirthDate = new DateTime(1993, 4, 2), Email = "miguel.rocha@hospital.pt", Phone = "915678901", Specialty = "Emergência" },
                new Nurse { Name = "Sofia Almeida", NIF = "267890123", ProfessionalLicense = "ENF55667", BirthDate = new DateTime(1987, 1, 30), Email = "sofia.almeida@hospital.pt", Phone = "916789012", Specialty = "Psiquiatria" },
                new Nurse { Name = "João Pires", NIF = "278901234", ProfessionalLicense = "ENF77889", BirthDate = new DateTime(1991, 6, 14), Email = "joao.pires@hospital.pt", Phone = "917890123", Specialty = "Oncologia" },
                new Nurse { Name = "Mariana Lopes", NIF = "289012345", ProfessionalLicense = "ENF99001", BirthDate = new DateTime(1989, 9, 19), Email = "mariana.lopes@hospital.pt", Phone = "918901234", Specialty = "Cuidados Paliativos" },
                new Nurse { Name = "Bruno Carvalho", NIF = "290123456", ProfessionalLicense = "ENF11224", BirthDate = new DateTime(1992, 12, 10), Email = "bruno.carvalho@hospital.pt", Phone = "919012345", Specialty = "Urgência" },
                new Nurse { Name = "Catarina Silva", NIF = "301234567", ProfessionalLicense = "ENF22335", BirthDate = new DateTime(1986, 2, 8), Email = "catarina.silva@hospital.pt", Phone = "910123456", Specialty = "Cardiologia" },
                new Nurse { Name = "Pedro Nogueira", NIF = "312345678", ProfessionalLicense = "ENF33446", BirthDate = new DateTime(1995, 10, 22), Email = "pedro.nogueira@hospital.pt", Phone = "911234567", Specialty = "Ortopedia" },
                new Nurse { Name = "Filipa Moreira", NIF = "323456789", ProfessionalLicense = "ENF44557", BirthDate = new DateTime(1994, 8, 17), Email = "filipa.moreira@hospital.pt", Phone = "912345679", Specialty = "Geriatria" },
                new Nurse { Name = "Ricardo Teixeira", NIF = "334567890", ProfessionalLicense = "ENF55668", BirthDate = new DateTime(1983, 5, 11), Email = "ricardo.teixeira@hospital.pt", Phone = "913456780", Specialty = "Medicina Interna" },
                new Nurse { Name = "Tânia Correia", NIF = "345678901", ProfessionalLicense = "ENF66779", BirthDate = new DateTime(1989, 3, 3), Email = "tania.correia@hospital.pt", Phone = "914567891", Specialty = "Cirurgia Geral" },
                new Nurse { Name = "André Sousa", NIF = "356789012", ProfessionalLicense = "ENF77880", BirthDate = new DateTime(1996, 4, 20), Email = "andre.sousa@hospital.pt", Phone = "915678902", Specialty = "Pneumologia" },
                new Nurse { Name = "Laura Mendes", NIF = "367890123", ProfessionalLicense = "ENF88991", BirthDate = new DateTime(1984, 11, 25), Email = "laura.mendes@hospital.pt", Phone = "916789013", Specialty = "Reabilitação" },
                new Nurse { Name = "Diogo Azevedo", NIF = "378901234", ProfessionalLicense = "ENF99002", BirthDate = new DateTime(1992, 6, 1), Email = "diogo.azevedo@hospital.pt", Phone = "917890134", Specialty = "Urgência" },
                new Nurse { Name = "Patrícia Faria", NIF = "389012345", ProfessionalLicense = "ENF10113", BirthDate = new DateTime(1990, 9, 15), Email = "patricia.faria@hospital.pt", Phone = "918901245", Specialty = "Cuidados Domiciliários" },
                new Nurse { Name = "Nelson Ribeiro", NIF = "390123456", ProfessionalLicense = "ENF11224", BirthDate = new DateTime(1988, 1, 9), Email = "nelson.ribeiro@hospital.pt", Phone = "919012356", Specialty = "Urgência" },
                new Nurse { Name = "Vera Coelho", NIF = "401234567", ProfessionalLicense = "ENF22335", BirthDate = new DateTime(1993, 2, 12), Email = "vera.coelho@hospital.pt", Phone = "910123467", Specialty = "Cuidados Intensivos" },
                new Nurse { Name = "Gonçalo Lima", NIF = "412345678", ProfessionalLicense = "ENF33446", BirthDate = new DateTime(1991, 12, 3), Email = "goncalo.lima@hospital.pt", Phone = "911234578", Specialty = "Pediatria" }
            };

            HealthWellbeingDbContext.Nurse.AddRange(enfermeiros);
            HealthWellbeingDbContext.SaveChanges();
        }


        private static void PopulatePathology(HealthWellbeingDbContext HealthWellbeingDbContext)
        {
            if (HealthWellbeingDbContext.Pathology.Any()) return;

            var Pathologies = new[]
            {
                new Pathology { Name = "Diabetes Tipo 2", Description = "Doença metabólica caracterizada por níveis elevados de glicose no sangue devido à resistência à insulina.", Severity = "Moderada" },
                new Pathology { Name = "Hipertensão Arterial", Description = "Pressão arterial persistentemente elevada, aumentando o risco de enfarte e AVC.", Severity = "Ligeira" },
                new Pathology { Name = "Asma", Description = "Doença inflamatória crónica das vias respiratórias que causa dificuldade respiratória.", Severity = "Grave" },
                new Pathology { Name = "DPOC", Description = "Doença pulmonar obstrutiva crónica associada ao tabagismo e exposição a poluentes.", Severity = "Grave" },
                new Pathology { Name = "Anemia Ferropriva", Description = "Deficiência de ferro que provoca cansaço, fraqueza e palidez.", Severity = "Ligeira" },
                new Pathology { Name = "Artrite Reumatoide", Description = "Doença autoimune que causa inflamação crónica das articulações.", Severity = "Moderada" },
                new Pathology { Name = "Gastrite", Description = "Inflamação do revestimento do estômago causada por bactérias, álcool ou stress.", Severity = "Ligeira" },
                new Pathology { Name = "Enxaqueca", Description = "Dor de cabeça intensa frequentemente acompanhada de náuseas e sensibilidade à luz.", Severity = "Moderada" },
                new Pathology { Name = "Epilepsia", Description = "Distúrbio neurológico caracterizado por crises convulsivas recorrentes.", Severity = "Grave" },
                new Pathology { Name = "Depressão Major", Description = "Transtorno de humor caracterizado por tristeza persistente e perda de interesse.", Severity = "Grave" }

            };

            HealthWellbeingDbContext.Pathology.AddRange(Pathologies);
            HealthWellbeingDbContext.SaveChanges();
        }
    }
}
