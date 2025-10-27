using HealthWellbeing.Data;
using HealthWellbeingRoom.Models.FileMedicalDevices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace HealthWellbeingRoom.Data
{

    internal class SeedDataMedicalDevices
    {
        internal static void Populate(HealthWellbeingDbContext? dbContext)
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));

            PopulateMedicalDevices(dbContext);
        }

        private static void PopulateMedicalDevices(HealthWellbeingDbContext dbContext)
        {
            if (dbContext.MedicalDevices.Any()) return;

            dbContext.MedicalDevices.AddRange(new List<MedicalDevices>() 
            {
                new MedicalDevices {Name = "Monitor MP-500", Type = "Monitorização",
                    Specification = "ECG, SPO2, PNI. S/N: MP500-A01", Quantity = 1, RegistrationDate = DateTime.Now.AddDays(-60),
                    Status = "Em Uso", Observation = "Excelente condição.", SalaID = 2 }, // UTI 101
            
                new MedicalDevices {Name = "Monitor MP-501", Type = "Monitorização",
                    Specification = "ECG, SPO2, PNI. S/N: MP500-A02", Quantity = 1, RegistrationDate = DateTime.Now.AddDays(-60),
                    Status = "Disponível", Observation = "Em espera para Sala 4.", SalaID = 1 }, // Armazém
            
                new MedicalDevices {Name = "Bomba Infusora Gemini", Type = "Infusão",
                    Specification = "Duplo Canal, Bateria 4h.", Quantity = 1, RegistrationDate = DateTime.Now.AddDays(-30),
                    Status = "Em Uso", Observation = "Uso exclusivo em pediatria.", SalaID = 3 }, // Cirurgia 3
            
                new MedicalDevices {Name = "Ventilador Portátil V5", Type = "Suporte Vital",
                    Specification = "Modos de ventilação avançados.", Quantity = 1, RegistrationDate = DateTime.Now.AddDays(-15),
                    Status = "Em Manutenção", Observation = "Sensor de pressão defeituoso.", SalaID = 1 }, // Armazém
            
                new MedicalDevices {Name = "Máquina de Anestesia PAX", Type = "Anestesia",
                    Specification = "Sistema modular de controlo de gás.", Quantity = 1, RegistrationDate = DateTime.Now.AddDays(-10),
                    Status = "Em Uso", Observation = "Necessita de filtro novo.", SalaID = 3 }, // Cirurgia 3

                new MedicalDevices {Name = "Oxímetro de Pulso OP-6", Type = "Diagnóstico",
                    Specification = "Display OLED de alta precisão.", Quantity = 5, RegistrationDate = DateTime.Now.AddDays(-7),
                    Status = "Disponível", Observation = null, SalaID = 4 }, // Urgência
            
                new MedicalDevices {Name = "Oxímetro de Pulso OP-7", Type = "Diagnóstico",
                    Specification = "Display OLED de alta precisão.", Quantity = 1, RegistrationDate = DateTime.Now.AddDays(-7),
                    Status = "Em Uso", Observation = null, SalaID = 5 }, // Enfermagem
            
                new MedicalDevices {Name = "Ecógrafo Móvel GE", Type = "Diagnóstico",
                    Specification = "Transdutor linear e convexo.", Quantity = 1, RegistrationDate = DateTime.Now.AddDays(-4),
                    Status = "Disponível", Observation = "Calibração concluída.", SalaID = 1 }, // Armazém
            
                new MedicalDevices {Name = "Unidade de Fototerapia", Type = "Tratamento",
                    Specification = "Luz azul de alta intensidade.", Quantity = 1, RegistrationDate = DateTime.Now.AddDays(-2),
                    Status = "Em Uso", Observation = null, SalaID = 2 }, // UTI 101
            
                new MedicalDevices {Name = "Balança de Cama Digital", Type = "Monitorização",
                    Specification = "Capacidade de 300kg. Portátil.", Quantity = 1, RegistrationDate = DateTime.Now.AddDays(-1),
                    Status = "Disponível", Observation = "Para uso em leitos.", SalaID = 5 }, // Enfermagem
            
                new MedicalDevices {Name = "Bomba de Aspiração Cirúrgica", Type = "Cirúrgico",
                    Specification = "Fluxo ajustável, recipiente de 2L.", Quantity = 1, RegistrationDate = DateTime.Now,
                    Status = "Em Uso", Observation = null, SalaID = 3 }, // Cirurgia 3
            
                new MedicalDevices {Name = "Monofone de Comunicação", Type = "Comunicação",
                    Specification = "Comunicação interna wireless.", Quantity = 10, RegistrationDate = DateTime.Now,
                    Status = "Disponível", Observation = null, SalaID = 1 }, // Armazém
            
                new MedicalDevices {Name = "Ventilador Portátil V6", Type = "Suporte Vital",
                    Specification = "Modelo avançado para transporte.", Quantity = 1, RegistrationDate = DateTime.Now,
                    Status = "Em Uso", Observation = null, SalaID = 4 }, // Urgência
            
                new MedicalDevices {Name = "Electrocardiógrafo ECG-1", Type = "Diagnóstico",
                    Specification = "12 canais, portátil com bateria.", Quantity = 1, RegistrationDate = DateTime.Now,
                    Status = "Disponível", Observation = "Para consultas externas.", SalaID = 5 }, // Enfermagem
            
                new MedicalDevices {Name = "Esfigmomanómetro Digital", Type = "Diagnóstico",
                    Specification = "Automático, memória para 50 medições.", Quantity = 8, RegistrationDate = DateTime.Now,
                    Status = "Disponível", Observation = null, SalaID = 1 }, // Armazém
            
                new MedicalDevices {Name = "Termómetro Infravermelho", Type = "Diagnóstico",
                    Specification = "Medição sem contato.", Quantity = 12, RegistrationDate = DateTime.Now,
                    Status = "Em Uso", Observation = "Uso geral em todos os gabinetes.", SalaID = 4 }, // Urgência
            
                new MedicalDevices {Name = "Analisador de Gás Sanguíneo", Type = "Laboratorial",
                    Specification = "Portátil, resultados em 1 minuto.", Quantity = 1, RegistrationDate = DateTime.Now,
                    Status = "Disponível", Observation = "Requer reagentes.", SalaID = 2 }, // UTI 101
            
                new MedicalDevices {Name = "Desfibrilhador DEA", Type = "Emergência",
                    Specification = "Automático, com guia por voz.", Quantity = 1, RegistrationDate = DateTime.Now,
                    Status = "Em Uso", Observation = "Localizado no carrinho de emergência.", SalaID = 5 }, // Enfermagem
            
                new MedicalDevices {Name = "Foco Cirúrgico Móvel", Type = "Cirúrgico",
                    Specification = "Iluminação LED ajustável.", Quantity = 1, RegistrationDate = DateTime.Now,
                    Status = "Disponível", Observation = "Para procedimentos menores.", SalaID = 1 }, // Armazém
        });

            dbContext.SaveChanges();
        }
    }

}