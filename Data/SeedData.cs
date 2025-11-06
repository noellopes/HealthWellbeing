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
            //PopulatePathology
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
    }
         
}
