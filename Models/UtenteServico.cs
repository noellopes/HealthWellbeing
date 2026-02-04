using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthWellbeing.Models
{
    public static class UtenteService
    {
        private static readonly List<UtenteBalneario> utentes = new()
        {
            new UtenteBalneario
            {
                UtenteBalnearioId = 1,
                Nome = "Maria Silva",
                DataNascimento = new DateTime(1985, 4, 12),
                GeneroId = 1,
                NIF = "123456789",
                Contacto = "912345678",
                Morada = "Rua das Termas, 12",
                HistoricoClinico = "Hipertensão controlada.",
                IndicacoesTerapeuticas = "Banhos termais 2x por semana.",
                ContraIndicacoes = "Evitar longas exposições ao calor.",
                //TerapeutaId = terapeutasDb[random.Next(terapeutasDb.Count)].TerapeutaId,

                SeguroSaudeId = 1,
                Ativo = true,
                DataInscricao = new DateTime(2024, 5, 10)
            },
            new UtenteBalneario
            {
                UtenteBalnearioId = 2,
                Nome = "João Pereira",
                DataNascimento = new DateTime(1990, 7, 21),
                GeneroId = 2,
                NIF = "987654321",
                Contacto = "913456789",
                Morada = "Av. Saúde, 50",
                HistoricoClinico = "Asma leve.",
                IndicacoesTerapeuticas = "Inalações termais semanais.",
                ContraIndicacoes = "Nenhuma.",
                //TerapeutaResponsavel = "Dra. Ana Marques",
                SeguroSaudeId = 2,
                Ativo = false,
                DataInscricao = new DateTime(2023, 3, 15)
            },
            new UtenteBalneario
            {
                UtenteBalnearioId = 3,
                Nome = "Carla Nunes",
                DataNascimento = new DateTime(1975, 11, 3),
                GeneroId = 1,
                NIF = "111222333",
                Contacto = "914223344",
                Morada = "Rua dos Limoeiros, 7",
                HistoricoClinico = "Artrite reumatoide.",
                IndicacoesTerapeuticas = "Massagens e banhos minerais.",
                ContraIndicacoes = "Exercícios intensos.",
                //TerapeutaResponsavel = "Dr. João Costa",
                SeguroSaudeId = 3,
                Ativo = true,
                DataInscricao = new DateTime(2022, 9, 20)
            }
        };

        public static List<UtenteBalneario> GetAll()
            => utentes;

        public static UtenteBalneario? GetById(int id)
            => utentes.FirstOrDefault(u => u.UtenteBalnearioId == id);

        public static void Add(UtenteBalneario u)
        {
            u.UtenteBalnearioId = utentes.Max(x => x.UtenteBalnearioId) + 1;
            utentes.Add(u);
        }

        public static void Update(UtenteBalneario u)
        {
            var existing = GetById(u.UtenteBalnearioId);
            if (existing == null) return;

            utentes.Remove(existing);
            utentes.Add(u);
        }

        public static void Delete(int id)
        {
            var existing = GetById(id);
            if (existing != null)
                utentes.Remove(existing);
        }
    }
}