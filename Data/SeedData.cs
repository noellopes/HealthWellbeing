using HealthWellbeing.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Data
{
    public class SeedData
    {
        internal static void Populate(HealthWellbeingDbContext? HealthWellbeingDbContext)
        {
            if (HealthWellbeingDbContext == null) throw new ArgumentNullException(nameof(HealthWellbeingDbContext));

            HealthWellbeingDbContext.Database.EnsureCreated();

            PopulateTreatmentType(HealthWellbeingDbContext);
            PopulatePathology(HealthWellbeingDbContext);
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
     public static void PopulatePathology(HealthWellbeingDbContext dbContext)
        {
            if (dbContext.Pathology.Any()) return;

            var pathologies = new[]
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

            dbContext.Pathology.AddRange(pathologies);
            dbContext.SaveChanges();
        }
    }

}
