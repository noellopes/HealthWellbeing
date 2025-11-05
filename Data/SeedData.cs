using HealthWellbeing.Data;
using HealthWellbeingRoom.Models.FileMedicalDevices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Collections.Generic;
using HealthWellbeingRoom.Models;

namespace HealthWellBeingRoom.Data
{
    internal class SeedData
    {
        internal static void Populate(HealthWellbeingDbContext? dbContext)
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));

            PopulateMedicalDevices(dbContext);
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