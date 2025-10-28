using HealthWellbeing.Data;
using HealthWellbeingRoom.Models.FileMedicalDevices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Collections.Generic; // Adicionado para usar List<MedicalDevices>

namespace HealthWellBeingRoom.Data
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
                    Specification = "ECG, SPO2, PNI. S/N: MP500-A01", RegistrationDate = DateTime.Now.AddDays(-60),
                    Status = "Em Uso", Observation = "Excelente condição." },

                new MedicalDevices {Name = "Monitor MP-501", Type = "Monitorização",
                    Specification = "ECG, SPO2, PNI. S/N: MP500-A02", RegistrationDate = DateTime.Now.AddDays(-60),
                    Status = "Disponível", Observation = "Em espera para Sala 4." },

                new MedicalDevices {Name = "Bomba Infusora Gemini", Type = "Infusão",
                    Specification = "Duplo Canal, Bateria 4h.", RegistrationDate = DateTime.Now.AddDays(-30),
                    Status = "Em Uso", Observation = "Uso exclusivo em pediatria." },

                new MedicalDevices {Name = "Ventilador Portátil V5", Type = "Suporte Vital",
                    Specification = "Modos de ventilação avançados.", RegistrationDate = DateTime.Now.AddDays(-15),
                    Status = "Em Manutenção", Observation = "Sensor de pressão defeituoso." },

                new MedicalDevices {Name = "Máquina de Anestesia PAX", Type = "Anestesia",
                    Specification = "Sistema modular de controlo de gás.", RegistrationDate = DateTime.Now.AddDays(-10),
                    Status = "Em Uso", Observation = "Necessita de filtro novo." },

                new MedicalDevices {Name = "Oxímetro de Pulso OP-6", Type = "Diagnóstico",
                    Specification = "Display OLED de alta precisão.", RegistrationDate = DateTime.Now.AddDays(-7),
                    Status = "Disponível", Observation = null },

                new MedicalDevices {Name = "Oxímetro de Pulso OP-7", Type = "Diagnóstico",
                    Specification = "Display OLED de alta precisão.", RegistrationDate = DateTime.Now.AddDays(-7),
                    Status = "Em Uso", Observation = null },

                new MedicalDevices {Name = "Ecógrafo Móvel GE", Type = "Diagnóstico",
                    Specification = "Transdutor linear e convexo.", RegistrationDate = DateTime.Now.AddDays(-4),
                    Status = "Disponível", Observation = "Calibração concluída." },

                new MedicalDevices {Name = "Unidade de Fototerapia", Type = "Tratamento",
                    Specification = "Luz azul de alta intensidade.", RegistrationDate = DateTime.Now.AddDays(-2),
                    Status = "Em Uso", Observation = null },

                new MedicalDevices {Name = "Balança de Cama Digital", Type = "Monitorização",
                    Specification = "Capacidade de 300kg. Portátil.", RegistrationDate = DateTime.Now.AddDays(-1),
                    Status = "Disponível", Observation = "Para uso em leitos." },

                new MedicalDevices {Name = "Bomba de Aspiração Cirúrgica", Type = "Cirúrgico",
                    Specification = "Fluxo ajustável, recipiente de 2L.", RegistrationDate = DateTime.Now,
                    Status = "Em Uso", Observation = null },

                new MedicalDevices {Name = "Monofone de Comunicação", Type = "Comunicação",
                    Specification = "Comunicação interna wireless.", RegistrationDate = DateTime.Now,
                    Status = "Disponível", Observation = null },

                new MedicalDevices {Name = "Ventilador Portátil V6", Type = "Suporte Vital",
                    Specification = "Modelo avançado para transporte.", RegistrationDate = DateTime.Now,
                    Status = "Em Uso", Observation = null },

                new MedicalDevices {Name = "Electrocardiógrafo ECG-1", Type = "Diagnóstico",
                    Specification = "12 canais, portátil com bateria.", RegistrationDate = DateTime.Now,
                    Status = "Disponível", Observation = "Para consultas externas." },

                new MedicalDevices {Name = "Esfigmomanómetro Digital", Type = "Diagnóstico",
                    Specification = "Automático, memória para 50 medições.", RegistrationDate = DateTime.Now,
                    Status = "Disponível", Observation = null },

                new MedicalDevices {Name = "Termómetro Infravermelho", Type = "Diagnóstico",
                    Specification = "Medição sem contato.", RegistrationDate = DateTime.Now,
                    Status = "Em Uso", Observation = "Uso geral em todos os gabinetes." },

                new MedicalDevices {Name = "Analisador de Gás Sanguíneo", Type = "Laboratorial",
                    Specification = "Portátil, resultados em 1 minuto.", RegistrationDate = DateTime.Now,
                    Status = "Disponível", Observation = "Requer reagentes." },

                new MedicalDevices {Name = "Desfibrilhador DEA", Type = "Emergência",
                    Specification = "Automático, com guia por voz.", RegistrationDate = DateTime.Now,
                    Status = "Em Uso", Observation = "Localizado no carrinho de emergência." },

                new MedicalDevices {Name = "Foco Cirúrgico Móvel", Type = "Cirúrgico",
                    Specification = "Iluminação LED ajustável.", RegistrationDate = DateTime.Now,
                    Status = "Disponível", Observation = "Para procedimentos menores." },
            });

            dbContext.SaveChanges();
        }
    }
}