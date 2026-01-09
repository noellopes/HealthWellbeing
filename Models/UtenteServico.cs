using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthWellbeing.Models
{
    public static class UtenteService
    {
        private static List<UtenteBalneario> utentes = new List<UtenteBalneario>
        {
            new UtenteBalneario
            {
                UtenteBalnearioId = 1,
                NomeCompleto = "Maria Silva",
                DataNascimento = new DateTime(1985, 4, 12),
                Sexo = Sexo.Feminino,
                NIF = "123456789",
                Contacto = "912345678",
                Morada = "Rua das Termas, 12",
                DadosMedicos = new DadosMedicos
                {
                    HistoricoClinico = "Hipertensão controlada.",
                    IndicacoesTerapeuticas = "Banhos termais 2x por semana.",
                    ContraIndicacoes = "Evitar longas exposições ao calor.",
                    MedicoResponsavel = "Dr. João Costa"
                },
                SeguroSaude = new SeguroSaude { NomeSeguradora = "Medicare", NumeroApolice = "A12345" },
                Ativo = true,
                DataInscricao = new DateTime(2024, 5, 10)
            },
            new UtenteBalneario
            {
                UtenteBalnearioId = 2,
                NomeCompleto = "João Pereira",
                DataNascimento = new DateTime(1990, 7, 21),
                Sexo = Sexo.Masculino,
                NIF = "987654321",
                Contacto = "913456789",
                Morada = "Av. Saúde, 50",
                DadosMedicos = new DadosMedicos
                {
                    HistoricoClinico = "Asma leve.",
                    IndicacoesTerapeuticas = "Inalações termais semanais.",
                    ContraIndicacoes = "Nenhuma.",
                    MedicoResponsavel = "Dra. Ana Marques"
                },
                SeguroSaude = new SeguroSaude { NomeSeguradora = "Multicare", NumeroApolice = "B98765" },
                Ativo = false,
                DataInscricao = new DateTime(2023, 3, 15)
            },
            new UtenteBalneario
            {
                UtenteBalnearioId = 3,
                NomeCompleto = "Carla Nunes",
                DataNascimento = new DateTime(1975, 11, 3),
                Sexo = Sexo.Feminino,
                NIF = "111222333",
                Contacto = "914223344",
                Morada = "Rua dos Limoeiros, 7",
                DadosMedicos = new DadosMedicos
                {
                    HistoricoClinico = "Artrite reumatoide.",
                    IndicacoesTerapeuticas = "Massagens e banhos minerais.",
                    ContraIndicacoes = "Exercícios intensos.",
                    MedicoResponsavel = "Dr. João Costa"
                },
                SeguroSaude = new SeguroSaude { NomeSeguradora = "Fidelidade", NumeroApolice = "C32145" },
                Ativo = true,
                DataInscricao = new DateTime(2022, 9, 20)
            },
            new UtenteBalneario
            {
                UtenteBalnearioId = 4,
                NomeCompleto = "Rui Almeida",
                DataNascimento = new DateTime(1968, 1, 14),
                Sexo = Sexo.Masculino,
                NIF = "444555666",
                Contacto = "919998887",
                Morada = "Praça Central, 5",
                DadosMedicos = new DadosMedicos
                {
                    HistoricoClinico = "Colesterol alto.",
                    IndicacoesTerapeuticas = "Banhos relaxantes semanais.",
                    ContraIndicacoes = "Evitar temperaturas elevadas.",
                    MedicoResponsavel = "Dra. Ana Marques"
                },
                SeguroSaude = new SeguroSaude { NomeSeguradora = "Médis", NumeroApolice = "D77531" },
                Ativo = true,
                DataInscricao = new DateTime(2024, 6, 2)
            },
            new UtenteBalneario
            {
                UtenteBalnearioId = 5,
                NomeCompleto = "Teresa Faria",
                DataNascimento = new DateTime(1998, 8, 8),
                Sexo = Sexo.Feminino,
                NIF = "555666777",
                Contacto = "911223344",
                Morada = "Rua Nova, 88",
                DadosMedicos = new DadosMedicos
                {
                    HistoricoClinico = "Nenhum problema relevante.",
                    IndicacoesTerapeuticas = "Tratamentos de relaxamento.",
                    ContraIndicacoes = "Nenhuma.",
                    MedicoResponsavel = "Dra. Ana Marques"
                },
                SeguroSaude = new SeguroSaude { NomeSeguradora = "Lusitânia", NumeroApolice = "E77654" },
                Ativo = true,
                DataInscricao = new DateTime(2024, 1, 1)
            },
            new UtenteBalneario
            {
                UtenteBalnearioId = 6,
                NomeCompleto = "Carlos Moreira",
                DataNascimento = new DateTime(1980, 3, 22),
                Sexo = Sexo.Masculino,
                NIF = "666777888",
                Contacto = "912112233",
                Morada = "Rua dos Pinhais, 3",
                DadosMedicos = new DadosMedicos
                {
                    HistoricoClinico = "Lombalgia crónica.",
                    IndicacoesTerapeuticas = "Massagens e exercícios leves.",
                    ContraIndicacoes = "Esforços físicos intensos.",
                    MedicoResponsavel = "Dr. João Costa"
                },
                SeguroSaude = new SeguroSaude { NomeSeguradora = "Multicare", NumeroApolice = "F99887" },
                Ativo = false,
                DataInscricao = new DateTime(2023, 12, 5)
            },
            new UtenteBalneario
            {
                UtenteBalnearioId = 7,
                NomeCompleto = "Sandra Lopes",
                DataNascimento = new DateTime(1989, 10, 30),
                Sexo = Sexo.Feminino,
                NIF = "999888777",
                Contacto = "915554433",
                Morada = "Av. das Flores, 20",
                DadosMedicos = new DadosMedicos
                {
                    HistoricoClinico = "Problemas respiratórios leves.",
                    IndicacoesTerapeuticas = "Inalações e banhos minerais.",
                    ContraIndicacoes = "Água muito quente.",
                    MedicoResponsavel = "Dra. Ana Marques"
                },
                SeguroSaude = new SeguroSaude { NomeSeguradora = "Medicare", NumeroApolice = "G88765" },
                Ativo = true,
                DataInscricao = new DateTime(2024, 8, 17)
            },
            new UtenteBalneario
            {
                UtenteBalnearioId = 8,
                NomeCompleto = "Nuno Costa",
                DataNascimento = new DateTime(1995, 6, 15),
                Sexo = Sexo.Masculino,
                NIF = "333222111",
                Contacto = "917887766",
                Morada = "Rua da Serra, 15",
                DadosMedicos = new DadosMedicos
                {
                    HistoricoClinico = "Ansiedade leve.",
                    IndicacoesTerapeuticas = "Terapias relaxantes.",
                    ContraIndicacoes = "Nenhuma.",
                    MedicoResponsavel = "Dr. João Costa"
                },
                SeguroSaude = new SeguroSaude { NomeSeguradora = "Fidelidade", NumeroApolice = "H65432" },
                Ativo = true,
                DataInscricao = new DateTime(2024, 2, 10)
            },
            new UtenteBalneario
            {
                UtenteBalnearioId = 9,
                NomeCompleto = "Beatriz Sousa",
                DataNascimento = new DateTime(1979, 2, 9),
                Sexo = Sexo.Feminino,
                NIF = "121314151",
                Contacto = "916556677",
                Morada = "Largo das Termas, 2",
                DadosMedicos = new DadosMedicos
                {
                    HistoricoClinico = "Fibromialgia.",
                    IndicacoesTerapeuticas = "Banhos termais e fisioterapia.",
                    ContraIndicacoes = "Frio intenso.",
                    MedicoResponsavel = "Dra. Ana Marques"
                },
                SeguroSaude = new SeguroSaude { NomeSeguradora = "Médis", NumeroApolice = "I23456" },
                Ativo = true,
                DataInscricao = new DateTime(2024, 9, 5)
            },
            new UtenteBalneario
            {
                UtenteBalnearioId = 10,
                NomeCompleto = "Pedro Rocha",
                DataNascimento = new DateTime(1988, 12, 19),
                Sexo = Sexo.Masculino,
                NIF = "777888999",
                Contacto = "919223344",
                Morada = "Rua do Parque, 10",
                DadosMedicos = new DadosMedicos
                {
                    HistoricoClinico = "Dores musculares ocasionais.",
                    IndicacoesTerapeuticas = "Hidroterapia leve.",
                    ContraIndicacoes = "Exercício intenso.",
                    MedicoResponsavel = "Dr. João Costa"
                },
                SeguroSaude = new SeguroSaude { NomeSeguradora = "Multicare", NumeroApolice = "J88776" },
                Ativo = true,
                DataInscricao = new DateTime(2024, 3, 28)
            }
        };

        public static List<UtenteBalneario> GetAll() => utentes;

        public static UtenteBalneario? GetById(int id) => utentes.FirstOrDefault(u => u.UtenteBalnearioId == id);

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
