using HealthWellbeing.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HealthWellbeing.Data
{
    public static class SeedData
    {
        internal static void Populate(HealthWellbeingDbContext? dbContext)
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));

            dbContext.Database.EnsureCreated();

            PopulateSpecialities(dbContext);
            PopulateDoctor(dbContext);
            PopulateAgendaMedica(dbContext);
            PopulateUtenteSaude(dbContext);
            PopulateConsultas(dbContext);
            

            var clients = PopulateClients(dbContext);
            PopulateMember(dbContext, clients);
            PopulateTrainingType(dbContext);
            PopulatePlan(dbContext);

            // --- ALTERAÇÃO AQUI: Capturamos a lista de Trainers ---
            var trainers = PopulateTrainer(dbContext);

            // --- NOVO MÉTODO: Povoamento dos Treinos Agendados ---
            PopulateTraining(dbContext, trainers);

            PopulateEventTypes(dbContext);
            PopulateEvents(dbContext);
            PopulateLevels(dbContext);
        }

        private static void PopulateSpecialities(HealthWellbeingDbContext db)
        {
            if (db.Doctor.Any()) return;
            if (db.Specialities.Any()) return; // Evita duplicar registos

            var especialidades = new[]
            {
    new Specialities
    {
        Nome = "Cardiologia",
        Descricao = "Avaliação, diagnóstico e tratamento de doenças do coração e sistema cardiovascular."
    },
    new Specialities
    {
        Nome = "Dermatologia",
        Descricao = "Prevenção, diagnóstico e tratamento de doenças da pele, cabelo e unhas."
    },
    new Specialities
    {
        Nome = "Pediatria",
        Descricao = "Cuidados de saúde para bebés, crianças e adolescentes."
    },
    new Specialities
    {
        Nome = "Psiquiatria",
        Descricao = "Avaliação e tratamento de perturbações mentais, emocionais e comportamentais."
    },
    new Specialities
    {
        Nome = "Nutrição",
        Descricao = "Aconselhamento alimentar e planos de nutrição para promoção da saúde e bem-estar."
    },
    new Specialities
    {
        Nome = "Medicina Geral e Familiar",
        Descricao = "Acompanhamento global e contínuo da saúde de utentes e famílias."
    },
    new Specialities
    {
        Nome = "Ortopedia",
        Descricao = "Tratamento de doenças e lesões dos ossos, articulações, músculos e tendões."
    },
    new Specialities
    {
        Nome = "Ginecologia e Obstetrícia",
        Descricao = "Saúde da mulher, sistema reprodutor e acompanhamento da gravidez e parto."
    },
    new Specialities
    {
        Nome = "Psicologia",
        Descricao = "Apoio psicológico, gestão emocional e acompanhamento em saúde mental."
    },
    new Specialities
    {
        Nome = "Fisioterapia",
        Descricao = "Reabilitação motora e funcional após lesões, cirurgias ou doenças crónicas."
    },

};

            db.Specialities.AddRange(especialidades);
            db.SaveChanges();
        }

        private static void PopulateDoctor(HealthWellbeingDbContext db)
        {
            if (db.Doctor.Any()) return;
            var especialidades = db.Specialities
                .ToDictionary(e => e.Nome, e => e);

            var doctor = new[]
            {
                new Doctor { Nome = "Ana Martins",      Telemovel = "912345678", Email = "ana.martins@healthwellbeing.pt", Especialidade = especialidades["Cardiologia"]},
                new Doctor { Nome = "Bruno Carvalho",   Telemovel = "913456789", Email = "bruno.carvalho@healthwellbeing.pt", Especialidade = especialidades["Dermatologia"]},
                new Doctor { Nome = "Carla Ferreira",   Telemovel = "914567890", Email = "carla.ferreira@healthwellbeing.pt", Especialidade = especialidades["Pediatria"]},
                new Doctor { Nome = "Daniel Sousa",     Telemovel = "915678901", Email = "daniel.sousa@healthwellbeing.pt", Especialidade = especialidades["Psiquiatria"] },
                new Doctor { Nome = "Eduarda Almeida",  Telemovel = "916789012", Email = "eduarda.almeida@healthwellbeing.pt", Especialidade = especialidades["Nutrição"] },
                new Doctor { Nome = "Fábio Pereira",    Telemovel = "917890123", Email = "fabio.pereira@healthwellbeing.pt", Especialidade = especialidades["Medicina Geral e Familiar"] },
                new Doctor { Nome = "Gabriela Rocha",   Telemovel = "918901234", Email = "gabriela.rocha@healthwellbeing.pt", Especialidade = especialidades["Ortopedia"] },
                new Doctor { Nome = "Hugo Santos",      Telemovel = "919012345", Email = "hugo.santos@healthwellbeing.pt", Especialidade = especialidades["Ginecologia e Obstetrícia"] },
                new Doctor { Nome = "Inês Correia",     Telemovel = "920123456", Email = "ines.correia@healthwellbeing.pt", Especialidade = especialidades["Psicologia"] },
                new Doctor { Nome = "João Ribeiro",     Telemovel = "921234567", Email = "joao.ribeiro@healthwellbeing.pt", Especialidade = especialidades["Fisioterapia"] },
                new Doctor { Nome = "Luísa Nogueira",   Telemovel = "922345678", Email = "luisa.nogueira@healthwellbeing.pt", Especialidade = especialidades["Medicina Geral e Familiar"] },
                new Doctor { Nome = "Miguel Costa",     Telemovel = "923456789", Email = "miguel.costa@healthwellbeing.pt", Especialidade = especialidades["Pediatria"] },
                new Doctor { Nome = "Nádia Gonçalves",  Telemovel = "924567890", Email = "nadia.goncalves@healthwellbeing.pt", Especialidade = especialidades["Cardiologia"] },
                new Doctor { Nome = "Óscar Figueiredo", Telemovel = "925678901", Email = "oscar.figueiredo@healthwellbeing.pt", Especialidade = especialidades["Pediatria"] },
                new Doctor { Nome = "Patrícia Lopes",   Telemovel = "926789012", Email = "patricia.lopes@healthwellbeing.pt", Especialidade = especialidades["Ginecologia e Obstetrícia"] },
            };

            db.Doctor.AddRange(doctor);
            db.SaveChanges();
        }

        private static void PopulateConsultas(HealthWellbeingDbContext db)
        {
            if (db.Consulta.Any()) return;

            var hoje = DateTime.Today;

            // médicos disponíveis (para alternar)
            var doctors = db.Doctor
                .OrderBy(d => d.IdMedico)
                .ToList();

            if (!doctors.Any())
                throw new InvalidOperationException("Não há médicos na BD. Corre primeiro o PopulateDoctor.");

            // ✅ utentes disponíveis (para alternar)
            var utentes = db.UtenteSaude
                .OrderBy(u => u.UtenteSaudeId)
                .ToList();

            if (!utentes.Any())
                throw new InvalidOperationException("Não há utentes na BD. Corre primeiro o PopulateUtenteSaude.");

            // 15 consultas (datas/horas de exemplo)
            var consultas = new List<Consulta>
    {
        // Base
        new Consulta
        {
            DataMarcacao = new DateTime(2024, 4, 21, 10, 30, 0, DateTimeKind.Unspecified),
            DataConsulta = new DateTime(2025, 4, 21, 10, 30, 0, DateTimeKind.Unspecified),
            HoraInicio   = new TimeOnly(10, 30),
            HoraFim      = new TimeOnly(11, 30),
        },

        // Futuras (Agendada)
        new Consulta
        {
            DataMarcacao = new DateTime(2025, 10, 10, 9, 15, 0, DateTimeKind.Unspecified),
            DataConsulta = new DateTime(2026, 1, 5, 9, 0, 0, DateTimeKind.Unspecified),
            HoraInicio   = new TimeOnly(9, 0),
            HoraFim      = new TimeOnly(9, 30),
        },
        new Consulta
        {
            DataMarcacao = new DateTime(2025, 10, 12, 14, 40, 0, DateTimeKind.Unspecified),
            DataConsulta = new DateTime(2026, 1, 10, 11, 15, 0, DateTimeKind.Unspecified),
            HoraInicio   = new TimeOnly(11, 15),
            HoraFim      = new TimeOnly(12, 0),
        },
        new Consulta
        {
            DataMarcacao = new DateTime(2025, 10, 15, 16, 5, 0, DateTimeKind.Unspecified),
            DataConsulta = new DateTime(2026, 1, 10, 15, 0, 0, DateTimeKind.Unspecified),
            HoraInicio   = new TimeOnly(15, 0),
            HoraFim      = new TimeOnly(15, 45),
        },
        new Consulta
        {
            DataMarcacao = new DateTime(2025, 10, 20, 10, 10, 0, DateTimeKind.Unspecified),
            DataConsulta = new DateTime(2025, 11, 20, 16, 30, 0, DateTimeKind.Unspecified),
            HoraInicio   = new TimeOnly(16, 30),
            HoraFim      = new TimeOnly(17, 0),
        },

        // Hoje
        new Consulta
        {
            DataMarcacao = hoje.AddDays(-2).AddHours(10),
            DataConsulta = new DateTime(hoje.Year, hoje.Month, hoje.Day, 9, 30, 0, DateTimeKind.Unspecified),
            HoraInicio   = new TimeOnly(15, 30),
            HoraFim      = new TimeOnly(16, 0),
        },
        new Consulta
        {
            DataMarcacao = hoje.AddDays(-1).AddHours(15).AddMinutes(20),
            DataConsulta = new DateTime(hoje.Year, hoje.Month, hoje.Day, 14, 0, 0, DateTimeKind.Unspecified),
            HoraInicio   = new TimeOnly(14, 0),
            HoraFim      = new TimeOnly(14, 30),
        },

        // Expiradas
        new Consulta
        {
            DataMarcacao = new DateTime(2025, 9, 1, 10, 0, 0, DateTimeKind.Unspecified),
            DataConsulta = new DateTime(2025, 9, 15, 9, 0, 0, DateTimeKind.Unspecified),
            HoraInicio   = new TimeOnly(9, 0),
            HoraFim      = new TimeOnly(9, 30),
        },
        new Consulta
        {
            DataMarcacao = new DateTime(2025, 8, 20, 11, 25, 0, DateTimeKind.Unspecified),
            DataConsulta = new DateTime(2025, 9, 25, 11, 45, 0, DateTimeKind.Unspecified),
            HoraInicio   = new TimeOnly(11, 45),
            HoraFim      = new TimeOnly(12, 15),
        },
        new Consulta
        {
            DataMarcacao = new DateTime(2025, 7, 5, 13, 10, 0, DateTimeKind.Unspecified),
            DataConsulta = new DateTime(2025, 8, 10, 16, 0, 0, DateTimeKind.Unspecified),
            HoraInicio   = new TimeOnly(16, 0),
            HoraFim      = new TimeOnly(16, 30),
        },

        // Canceladas
        new Consulta
        {
            DataMarcacao     = new DateTime(2025, 10, 1, 10, 0, 0, DateTimeKind.Unspecified),
            DataConsulta     = new DateTime(2025, 10, 30, 9, 0, 0, DateTimeKind.Unspecified),
            DataCancelamento = new DateTime(2025, 10, 28, 9, 30, 0, DateTimeKind.Unspecified),
            HoraInicio       = new TimeOnly(9, 0),
            HoraFim          = new TimeOnly(9, 30),
        },
        new Consulta
        {
            DataMarcacao     = new DateTime(2025, 9, 15, 11, 30, 0, DateTimeKind.Unspecified),
            DataConsulta     = new DateTime(2025, 10, 10, 15, 0, 0, DateTimeKind.Unspecified),
            DataCancelamento = new DateTime(2025, 10, 8, 10, 0, 0, DateTimeKind.Unspecified),
            HoraInicio       = new TimeOnly(15, 0),
            HoraFim          = new TimeOnly(15, 45),
        },
        new Consulta
        {
            DataMarcacao     = new DateTime(2025, 6, 10, 12, 0, 0, DateTimeKind.Unspecified),
            DataConsulta     = new DateTime(2025, 7, 5, 10, 30, 0, DateTimeKind.Unspecified),
            DataCancelamento = new DateTime(2025, 7, 3, 14, 15, 0, DateTimeKind.Unspecified),
            HoraInicio       = new TimeOnly(10, 30),
            HoraFim          = new TimeOnly(11, 0),
        },

        // Mais futuras
        new Consulta
        {
            DataMarcacao = new DateTime(2025, 10, 22, 9, 45, 0, DateTimeKind.Unspecified),
            DataConsulta = new DateTime(2025, 11, 15, 13, 30, 0, DateTimeKind.Unspecified),
            HoraInicio   = new TimeOnly(13, 30),
            HoraFim      = new TimeOnly(14, 15),
        },
        new Consulta
        {
            DataMarcacao = new DateTime(2025, 10, 25, 8, 55, 0, DateTimeKind.Unspecified),
            DataConsulta = new DateTime(2025, 12, 12, 8, 30, 0, DateTimeKind.Unspecified),
            HoraInicio   = new TimeOnly(8, 30),
            HoraFim      = new TimeOnly(9, 0),
        },
    };

            // ✅ alterna médico + especialidade + utente
            for (int i = 0; i < consultas.Count; i++)
            {
                var d = doctors[i % doctors.Count];
                var u = utentes[i % utentes.Count];

                consultas[i].IdMedico = d.IdMedico;
                consultas[i].IdEspecialidade = d.IdEspecialidade;
                consultas[i].IdUtenteSaude = u.UtenteSaudeId; // ✅ muito importante
            }

            // Seed idempotente (evita duplicados)
            foreach (var c in consultas)
            {
                bool exists = db.Consulta.Any(x =>
                    x.IdMedico == c.IdMedico &&
                    x.IdUtenteSaude == c.IdUtenteSaude &&
                    x.DataConsulta == c.DataConsulta &&
                    x.HoraInicio == c.HoraInicio);

                if (!exists)
                    db.Consulta.Add(c);
            }
            var doctor = new[]
            {
                new Doctor { Nome = "Ana Martins",      Telemovel = "912345678", Email = "ana.martins@healthwellbeing.pt" },
                new Doctor { Nome = "Bruno Carvalho",   Telemovel = "913456789", Email = "bruno.carvalho@healthwellbeing.pt" },
                new Doctor { Nome = "Carla Ferreira",   Telemovel = "914567890", Email = "carla.ferreira@healthwellbeing.pt" },
                new Doctor { Nome = "Daniel Sousa",     Telemovel = "915678901", Email = "daniel.sousa@healthwellbeing.pt" },
                new Doctor { Nome = "Eduarda Almeida",  Telemovel = "916789012", Email = "eduarda.almeida@healthwellbeing.pt" },
                new Doctor { Nome = "Fábio Pereira",    Telemovel = "917890123", Email = "fabio.pereira@healthwellbeing.pt" },
                new Doctor { Nome = "Gabriela Rocha",   Telemovel = "918901234", Email = "gabriela.rocha@healthwellbeing.pt" },
                new Doctor { Nome = "Hugo Santos",      Telemovel = "919012345", Email = "hugo.santos@healthwellbeing.pt" },
                new Doctor { Nome = "Inês Correia",     Telemovel = "920123456", Email = "ines.correia@healthwellbeing.pt" },
                new Doctor { Nome = "João Ribeiro",     Telemovel = "921234567", Email = "joao.ribeiro@healthwellbeing.pt" },
                new Doctor { Nome = "Luísa Nogueira",   Telemovel = "922345678", Email = "luisa.nogueira@healthwellbeing.pt" },
                new Doctor { Nome = "Miguel Costa",     Telemovel = "923456789", Email = "miguel.costa@healthwellbeing.pt" },
                new Doctor { Nome = "Nádia Gonçalves",  Telemovel = "924567890", Email = "nadia.goncalves@healthwellbeing.pt" },
                new Doctor { Nome = "Óscar Figueiredo", Telemovel = "925678901", Email = "oscar.figueiredo@healthwellbeing.pt" },
                new Doctor { Nome = "Patrícia Lopes",   Telemovel = "926789012", Email = "patricia.lopes@healthwellbeing.pt" },
            };

            db.SaveChanges();
        }




        private static void PopulateUtenteSaude(HealthWellbeingDbContext db)
        {
            if (db.UtenteSaude.Any()) return; // Evita duplicar registos

            var utentes = new[]
            {
                new UtenteSaude
                {
                    NomeCompleto   = "Ana Beatriz Silva",
                    DataNascimento = new DateTime(1999, 4, 8),
                    Nif            = "245379261", // válido
                    Niss           = "12345678901",
                    Nus            = "123456789",
                    Email          = "ana.beatriz.silva@example.pt",
                    Telefone       = "912345670",
                    Morada         = "Rua das Flores, 12, Guarda"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Bruno Miguel Pereira",
                    DataNascimento = new DateTime(1987, 11, 23),
                    Nif            = "198754326", // válido
                    Niss           = "22345678901",
                    Nus            = "223456789",
                    Email          = "bruno.miguel.pereira@example.pt",
                    Telefone       = "912345671",
                    Morada         = "Av. 25 de Abril, 102, Guarda"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Carla Sofia Fernandes",
                    DataNascimento = new DateTime(1991, 5, 19),
                    Nif            = "156987239", // válido
                    Niss           = "32345678901",
                    Nus            = "323456789",
                    Email          = "carla.sofia.fernandes@example.pt",
                    Telefone       = "912345672",
                    Morada         = "Rua da Liberdade, 45, Covilhã"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Daniel Rocha",
                    DataNascimento = new DateTime(2003, 10, 26),
                    Nif            = "268945315", // válido
                    Niss           = "42345678901",
                    Nus            = "423456789",
                    Email          = "daniel.rocha@example.pt",
                    Telefone       = "912345673",
                    Morada         = "Travessa do Sol, 3, Celorico da Beira"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Eduarda Nogueira",
                    DataNascimento = new DateTime(1994, 5, 22),
                    Nif            = "296378459", // válido
                    Niss           = "52345678901",
                    Nus            = "523456789",
                    Email          = "eduarda.nogueira@example.pt",
                    Telefone       = "912345674",
                    Morada         = "Rua do Comércio, 89, Seia"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Fábio Gonçalves",
                    DataNascimento = new DateTime(1997, 1, 4),
                    Nif            = "165947829", // válido
                    Niss           = "62345678901",
                    Nus            = "623456789",
                    Email          = "fabio.goncalves@example.pt",
                    Telefone       = "912345675",
                    Morada         = "Rua da Escola, 5, Gouveia"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Gabriela Santos",
                    DataNascimento = new DateTime(1986, 4, 26),
                    Nif            = "189567240", // válido
                    Niss           = "72345678901",
                    Nus            = "723456789",
                    Email          = "gabriela.santos@example.pt",
                    Telefone       = "912345676",
                    Morada         = "Av. Dr. Francisco Sá Carneiro, 200, Viseu"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Hugo Matos",
                    DataNascimento = new DateTime(1993, 11, 22),
                    Nif            = "215983747", // válido
                    Niss           = "82345678901",
                    Nus            = "823456789",
                    Email          = "hugo.matos@example.pt",
                    Telefone       = "912345677",
                    Morada         = "Rua do Castelo, 7, Belmonte"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Inês Carvalho",
                    DataNascimento = new DateTime(2004, 7, 12),
                    Nif            = "235679845", // válido
                    Niss           = "92345678901",
                    Nus            = "923456789",
                    Email          = "ines.carvalho@example.pt",
                    Telefone       = "912345678",
                    Morada         = "Rua do Mercado, 14, Trancoso"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "João Marques",
                    DataNascimento = new DateTime(1990, 7, 4),
                    Nif            = "286754197", // válido
                    Niss           = "10345678901",
                    Nus            = "103456789",
                    Email          = "joao.marques@example.pt",
                    Telefone       = "912345679",
                    Morada         = "Rua da Estação, 33, Pinhel"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Luísa Almeida",
                    DataNascimento = new DateTime(1978, 6, 19),
                    Nif            = "248963572", // válido
                    Niss           = "11345678901",
                    Nus            = "113456789",
                    Email          = "luisa.almeida@example.pt",
                    Telefone       = "912345680",
                    Morada         = "Rua da Lameira, 21, Manteigas"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Miguel Figueiredo",
                    DataNascimento = new DateTime(1985, 8, 9),
                    Nif            = "196284739", // válido
                    Niss           = "12345678902",
                    Nus            = "123456788",
                    Email          = "miguel.figueiredo@example.pt",
                    Telefone       = "912345681",
                    Morada         = "Rua do Parque, 8, Almeida"
                },

                // ---------- + Exemplos ----------
                new UtenteSaude
                {
                    NomeCompleto   = "Joana Moreira",
                    DataNascimento = new DateTime(1988, 3, 14),
                    Nif            = "218945372",
                    Niss           = "11111111111",
                    Nus            = "111111111",
                    Email          = "joana.moreira@example.pt",
                    Telefone       = "913245671",
                    Morada         = "Rua das Amoreiras, 15, Lisboa"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Carlos Almeida",
                    DataNascimento = new DateTime(1975, 9, 22),
                    Nif            = "295431678",
                    Niss           = "11111111112",
                    Nus            = "111111112",
                    Email          = "carlos.almeida@example.pt",
                    Telefone       = "912334567",
                    Morada         = "Avenida 25 de Abril, 20, Porto"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Sofia Marques",
                    DataNascimento = new DateTime(1992, 12, 5),
                    Nif            = "189546327",
                    Niss           = "11111111113",
                    Nus            = "111111113",
                    Email          = "sofia.marques@example.pt",
                    Telefone       = "916785432",
                    Morada         = "Rua da Liberdade, 33, Coimbra"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Ricardo Nogueira",
                    DataNascimento = new DateTime(1984, 2, 18),
                    Nif            = "239857416",
                    Niss           = "11111111114",
                    Nus            = "111111114",
                    Email          = "ricardo.nogueira@example.pt",
                    Telefone       = "915889002",
                    Morada         = "Travessa do Sol, 2, Braga"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Helena Rocha",
                    DataNascimento = new DateTime(1990, 7, 21),
                    Nif            = "259784631",
                    Niss           = "11111111115",
                    Nus            = "111111115",
                    Email          = "helena.rocha@example.pt",
                    Telefone       = "917654320",
                    Morada         = "Rua das Flores, 44, Viseu"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Tiago Fernandes",
                    DataNascimento = new DateTime(1982, 4, 9),
                    Nif            = "268953741",
                    Niss           = "11111111116",
                    Nus            = "111111116",
                    Email          = "tiago.fernandes@example.pt",
                    Telefone       = "912120234",
                    Morada         = "Avenida dos Bombeiros, 12, Aveiro"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Andreia Pinto",
                    DataNascimento = new DateTime(1995, 6, 30),
                    Nif            = "235978416",
                    Niss           = "11111111117",
                    Nus            = "111111117",
                    Email          = "andreia.pinto@example.pt",
                    Telefone       = "916782543",
                    Morada         = "Rua de São João, 9, Guarda"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Pedro Carvalho",
                    DataNascimento = new DateTime(1978, 10, 12),
                    Nif            = "298671543",
                    Niss           = "11111111118",
                    Nus            = "111111118",
                    Email          = "pedro.carvalho@example.pt",
                    Telefone       = "913998877",
                    Morada         = "Rua do Comércio, 70, Castelo Branco"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Marta Ribeiro",
                    DataNascimento = new DateTime(1987, 1, 7),
                    Nif            = "214896573",
                    Niss           = "11111111119",
                    Nus            = "111111119",
                    Email          = "marta.ribeiro@example.pt",
                    Telefone       = "919776543",
                    Morada         = "Largo da Igreja, 22, Viana do Castelo"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Luís Santos",
                    DataNascimento = new DateTime(1980, 5, 27),
                    Nif            = "268974153",
                    Niss           = "11111111120",
                    Nus            = "111111120",
                    Email          = "luis.santos@example.pt",
                    Telefone       = "914563278",
                    Morada         = "Praceta do Parque, 5, Leiria"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Filipa Gomes",
                    DataNascimento = new DateTime(1991, 8, 19),
                    Nif            = "189574362",
                    Niss           = "11111111121",
                    Nus            = "111111121",
                    Email          = "filipa.gomes@example.pt",
                    Telefone       = "913445677",
                    Morada         = "Rua da Escola, 10, Évora"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Rui Correia",
                    DataNascimento = new DateTime(1976, 3, 4),
                    Nif            = "278954136",
                    Niss           = "11111111122",
                    Nus            = "111111122",
                    Email          = "rui.correia@example.pt",
                    Telefone       = "912233456",
                    Morada         = "Rua dos Pescadores, 45, Nazaré"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Bárbara Figueiredo",
                    DataNascimento = new DateTime(1994, 11, 29),
                    Nif            = "215978643",
                    Niss           = "11111111123",
                    Nus            = "111111123",
                    Email          = "barbara.figueiredo@example.pt",
                    Telefone       = "915667788",
                    Morada         = "Rua da Lameira, 31, Torres Vedras"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Diogo Costa",
                    DataNascimento = new DateTime(1983, 12, 11),
                    Nif            = "268974523",
                    Niss           = "11111111124",
                    Nus            = "111111124",
                    Email          = "diogo.costa@example.pt",
                    Telefone       = "914555666",
                    Morada         = "Rua dos Combatentes, 14, Santarém"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Catarina Martins",
                    DataNascimento = new DateTime(1998, 9, 5),
                    Nif            = "239876415",
                    Niss           = "11111111125",
                    Nus            = "111111125",
                    Email          = "catarina.martins@example.pt",
                    Telefone       = "916787654",
                    Morada         = "Avenida da Liberdade, 66, Lisboa"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "João Vieira",
                    DataNascimento = new DateTime(1979, 7, 15),
                    Nif            = "258946371",
                    Niss           = "11111111126",
                    Nus            = "111111126",
                    Email          = "joao.vieira@example.pt",
                    Telefone       = "912444555",
                    Morada         = "Rua da Estação, 24, Braga"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Carla Neves",
                    DataNascimento = new DateTime(1989, 2, 2),
                    Nif            = "215987436",
                    Niss           = "11111111127",
                    Nus            = "111111127",
                    Email          = "carla.neves@example.pt",
                    Telefone       = "913998456",
                    Morada         = "Rua de Santa Maria, 88, Faro"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Vítor Lopes",
                    DataNascimento = new DateTime(1981, 1, 18),
                    Nif            = "276895413",
                    Niss           = "11111111128",
                    Nus            = "111111128",
                    Email          = "vitor.lopes@example.pt",
                    Telefone       = "912776543",
                    Morada         = "Travessa do Mercado, 15, Setúbal"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Mariana Batista",
                    DataNascimento = new DateTime(1993, 4, 3),
                    Nif            = "289654137",
                    Niss           = "11111111129",
                    Nus            = "111111129",
                    Email          = "mariana.batista@example.pt",
                    Telefone       = "914334566",
                    Morada         = "Rua de São Tiago, 18, Aveiro"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Filipe Cruz",
                    DataNascimento = new DateTime(1987, 11, 9),
                    Nif            = "295764821",
                    Niss           = "11111111130",
                    Nus            = "111111130",
                    Email          = "filipe.cruz@example.pt",
                    Telefone       = "916776554",
                    Morada         = "Rua da Liberdade, 10, Coimbra"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Teresa Gonçalves",
                    DataNascimento = new DateTime(1990, 3, 23),
                    Nif            = "198743265",
                    Niss           = "11111111131",
                    Nus            = "111111131",
                    Email          = "teresa.goncalves@example.pt",
                    Telefone       = "913221234",
                    Morada         = "Rua do Castelo, 19, Guimarães"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Paulo Teixeira",
                    DataNascimento = new DateTime(1975, 10, 14),
                    Nif            = "269841357",
                    Niss           = "11111111132",
                    Nus            = "111111132",
                    Email          = "paulo.teixeira@example.pt",
                    Telefone       = "912888999",
                    Morada         = "Avenida Central, 31, Braga"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Sandra Ramos",
                    DataNascimento = new DateTime(1988, 1, 5),
                    Nif            = "235978462",
                    Niss           = "11111111133",
                    Nus            = "111111133",
                    Email          = "sandra.ramos@example.pt",
                    Telefone       = "917776655",
                    Morada         = "Rua das Rosas, 12, Lisboa"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Nuno Sousa",
                    DataNascimento = new DateTime(1992, 5, 27),
                    Nif            = "289635147",
                    Niss           = "11111111134",
                    Nus            = "111111134",
                    Email          = "nuno.sousa@example.pt",
                    Telefone       = "914334221",
                    Morada         = "Travessa da Escola, 27, Aveiro"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Patrícia Cardoso",
                    DataNascimento = new DateTime(1983, 6, 8),
                    Nif            = "219846735",
                    Niss           = "11111111135",
                    Nus            = "111111135",
                    Email          = "patricia.cardoso@example.pt",
                    Telefone       = "915667899",
                    Morada         = "Rua do Campo, 7, Viseu"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Gonçalo Rocha",
                    DataNascimento = new DateTime(1985, 4, 11),
                    Nif            = "295687134",
                    Niss           = "11111111136",
                    Nus            = "111111136",
                    Email          = "goncalo.rocha@example.pt",
                    Telefone       = "913456789",
                    Morada         = "Avenida dos Aliados, 91, Porto"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Leonor Ferreira",
                    DataNascimento = new DateTime(1996, 2, 24),
                    Nif            = "218963475",
                    Niss           = "11111111137",
                    Nus            = "111111137",
                    Email          = "leonor.ferreira@example.pt",
                    Telefone       = "912998877",
                    Morada         = "Rua das Oliveiras, 18, Lisboa"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "André Mendes",
                    DataNascimento = new DateTime(1990, 12, 15),
                    Nif            = "259871436",
                    Niss           = "11111111138",
                    Nus            = "111111138",
                    Email          = "andre.mendes@example.pt",
                    Telefone       = "916778899",
                    Morada         = "Rua da República, 15, Coimbra"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Raquel Matos",
                    DataNascimento = new DateTime(1989, 7, 7),
                    Nif            = "236985147",
                    Niss           = "11111111139",
                    Nus            = "111111139",
                    Email          = "raquel.matos@example.pt",
                    Telefone       = "913667788",
                    Morada         = "Rua de São João, 23, Braga"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Henrique Azevedo",
                    DataNascimento = new DateTime(1977, 11, 25),
                    Nif            = "289634571",
                    Niss           = "11111111140",
                    Nus            = "111111140",
                    Email          = "henrique.azevedo@example.pt",
                    Telefone       = "912443322",
                    Morada         = "Rua das Laranjeiras, 19, Porto"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Beatriz Lopes",
                    DataNascimento = new DateTime(1995, 8, 2),
                    Nif            = "269841735",
                    Niss           = "11111111141",
                    Nus            = "111111141",
                    Email          = "beatriz.lopes@example.pt",
                    Telefone       = "915223344",
                    Morada         = "Rua da Liberdade, 80, Lisboa"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Miguel Ramos",
                    DataNascimento = new DateTime(1984, 9, 28),
                    Nif            = "259687314",
                    Niss           = "11111111142",
                    Nus            = "111111142",
                    Email          = "miguel.ramos@example.pt",
                    Telefone       = "913332211",
                    Morada         = "Rua do Cruzeiro, 17, Viseu"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Daniel Sousa",
                    DataNascimento = new DateTime(1979, 3, 16),
                    Nif            = "285963147",
                    Niss           = "11111111143",
                    Nus            = "111111143",
                    Email          = "daniel.sousa@example.pt",
                    Telefone       = "919887766",
                    Morada         = "Travessa dos Combatentes, 21, Setúbal"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Sílvia Ferreira",
                    DataNascimento = new DateTime(1993, 4, 10),
                    Nif            = "239685471",
                    Niss           = "11111111144",
                    Nus            = "111111144",
                    Email          = "silvia.ferreira@example.pt",
                    Telefone       = "912667788",
                    Morada         = "Avenida da Liberdade, 9, Braga"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Alexandre Pinto",
                    DataNascimento = new DateTime(1986, 7, 21),
                    Nif            = "278954163",
                    Niss           = "11111111145",
                    Nus            = "111111145",
                    Email          = "alexandre.pinto@example.pt",
                    Telefone       = "916443322",
                    Morada         = "Rua das Escolas, 12, Coimbra"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Vera Almeida",
                    DataNascimento = new DateTime(1990, 2, 19),
                    Nif            = "218964735",
                    Niss           = "11111111146",
                    Nus            = "111111146",
                    Email          = "vera.almeida@example.pt",
                    Telefone       = "912443355",
                    Morada         = "Rua do Penedo, 5, Viseu"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Nádia Marques",
                    DataNascimento = new DateTime(1998, 11, 8),
                    Nif            = "296847153",
                    Niss           = "11111111147",
                    Nus            = "111111147",
                    Email          = "nadia.marques@example.pt",
                    Telefone       = "913998554",
                    Morada         = "Rua do Campo, 17, Évora"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Hugo Barros",
                    DataNascimento = new DateTime(1981, 5, 4),
                    Nif            = "275986431",
                    Niss           = "11111111148",
                    Nus            = "111111148",
                    Email          = "hugo.barros@example.pt",
                    Telefone       = "916555777",
                    Morada         = "Rua dos Carvalhos, 14, Porto"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Joana Costa",
                    DataNascimento = new DateTime(1997, 6, 22),
                    Nif            = "285974316",
                    Niss           = "11111111149",
                    Nus            = "111111149",
                    Email          = "joana.costa@example.pt",
                    Telefone       = "912224466",
                    Morada         = "Rua do Rossio, 11, Lisboa"
                },
                new UtenteSaude
                {
                    NomeCompleto   = "Paula Rocha",
                    DataNascimento = new DateTime(1988, 10, 27),
                    Nif            = "269854137",
                    Niss           = "11111111150",
                    Nus            = "111111150",
                    Email          = "paula.rocha@example.pt",
                    Telefone       = "915223455",
                    Morada         = "Rua da Fonte, 25, Aveiro"
                }
            };

            db.UtenteSaude.AddRange(utentes);
            db.SaveChanges();
        }

        private static void PopulateEventTypes(HealthWellbeingDbContext dbContext)
        {
            if (dbContext.EventType.Any()) return;

            dbContext.EventType.AddRange(new List<EventType>() {
                //EDUCAÇÃO E FORMAÇÃO
                new EventType { EventTypeName = "Workshop Educacional", EventTypeScoringMode = "fixed", EventTypeMultiplier = 1.0f },
                new EventType { EventTypeName = "Seminário Temático", EventTypeScoringMode = "fixed", EventTypeMultiplier = 1.1f },
                new EventType { EventTypeName = "Palestra Informativa", EventTypeScoringMode = "fixed", EventTypeMultiplier = 1.0f },
                new EventType { EventTypeName = "Demonstração Técnica", EventTypeScoringMode = "fixed", EventTypeMultiplier = 1.0f },
                new EventType { EventTypeName = "Sessão de Orientação", EventTypeScoringMode = "fixed", EventTypeMultiplier = 1.0f },

                //TREINO CARDIOVASCULAR
                new EventType { EventTypeName = "Sessão de Running", EventTypeScoringMode = "time_based", EventTypeMultiplier = 2.0f },
                new EventType { EventTypeName = "Treino de Cycling", EventTypeScoringMode = "time_based", EventTypeMultiplier = 2.1f },
                new EventType { EventTypeName = "Aula de Cardio-Grupo", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.5f },
                new EventType { EventTypeName = "Treino de Natação", EventTypeScoringMode = "time_based", EventTypeMultiplier = 2.2f },
                new EventType { EventTypeName = "Sessão de HIIT", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.8f },

                //TREINO DE FORÇA
                new EventType { EventTypeName = "Treino de Musculação", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.7f },
                new EventType { EventTypeName = "Sessão de CrossFit", EventTypeScoringMode = "progressive", EventTypeMultiplier = 2.1f },
                new EventType { EventTypeName = "Treino Funcional", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.6f },
                new EventType { EventTypeName = "Aula de Powerlifting", EventTypeScoringMode = "progressive", EventTypeMultiplier = 2.0f },
                new EventType { EventTypeName = "Treino de Calistenia", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.5f },

                //BEM-ESTAR E MOBILIDADE
                new EventType { EventTypeName = "Aula de Yoga", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.3f },
                new EventType { EventTypeName = "Sessão de Pilates", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.4f },
                new EventType { EventTypeName = "Treino de Flexibilidade", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.5f },
                new EventType { EventTypeName = "Aula de Mobilidade", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.3f },
                new EventType { EventTypeName = "Sessão de Alongamento", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.2f },

                //DESPORTOS E ARTES MARCIAS
                new EventType { EventTypeName = "Aula de Artes Marciais", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.8f },
                new EventType { EventTypeName = "Treino de Boxe", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.9f },
                new EventType { EventTypeName = "Sessão de Lutas", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.8f },
                new EventType { EventTypeName = "Aula de Defesa Pessoal", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.4f },
                new EventType { EventTypeName = "Treino Desportivo Coletivo", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.6f },

                //DESAFIOS E COMPETIÇÕES
                new EventType { EventTypeName = "Competição de Running", EventTypeScoringMode = "time_based", EventTypeMultiplier = 2.5f },
                new EventType { EventTypeName = "Torneio Desportivo", EventTypeScoringMode = "binary", EventTypeMultiplier = 2.3f },
                new EventType { EventTypeName = "Desafio de Resistência", EventTypeScoringMode = "time_based", EventTypeMultiplier = 2.4f },
                new EventType { EventTypeName = "Competição de Força", EventTypeScoringMode = "progressive", EventTypeMultiplier = 2.2f },
                new EventType { EventTypeName = "Desafio de Superação", EventTypeScoringMode = "progressive", EventTypeMultiplier = 2.1f },

                //ATIVIDADES EM GRUPO
                new EventType { EventTypeName = "Aula de Grupo", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.4f },
                new EventType { EventTypeName = "Treino Coletivo", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.5f },
                new EventType { EventTypeName = "Workshop Prático", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.3f },
                new EventType { EventTypeName = "Sessão de Team Building", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.2f },
                new EventType { EventTypeName = "Aula Experimental", EventTypeScoringMode = "binary", EventTypeMultiplier = 1.1f },

                //ESPECIALIZADOS E TÉCNICOS
                new EventType { EventTypeName = "Treino Técnico", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.6f },
                new EventType { EventTypeName = "Workshop de Técnica", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.3f },
                new EventType { EventTypeName = "Aula Avançada", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.7f },
                new EventType { EventTypeName = "Sessão de Perfeiçoamento", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.5f },
                new EventType { EventTypeName = "Treino Especializado", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.8f }
            });

            dbContext.SaveChanges();
        }

        private static void PopulateEvents(HealthWellbeingDbContext dbContext)
        {
            if (dbContext.Event.Any()) return;

            var eventTypes = dbContext.EventType.ToList();
            if (!eventTypes.Any()) return;

            var eventList = new List<Event>();
            var now = DateTime.Now;

            eventList.Add(new Event { EventName = "Competição Anual", EventDescription = "Competição de final de ano.", EventTypeId = eventTypes[0].EventTypeId, EventStart = now.AddDays(-30), EventEnd = now.AddDays(-30).AddHours(3), EventPoints = 200, MinLevel = 3 });
            eventList.Add(new Event { EventName = "Workshop de Nutrição", EventDescription = "Aprenda a comer melhor.", EventTypeId = eventTypes[1].EventTypeId, EventStart = now.AddDays(-28), EventEnd = now.AddDays(-28).AddHours(2), EventPoints = 50, MinLevel = 1 });
            eventList.Add(new Event { EventName = "Aula de Zumba", EventDescription = "Dança e diversão.", EventTypeId = eventTypes[2].EventTypeId, EventStart = now.AddDays(-26), EventEnd = now.AddDays(-26).AddHours(1), EventPoints = 75, MinLevel = 1 });
            eventList.Add(new Event { EventName = "Desafio CrossFit", EventDescription = "Teste os seus limites.", EventTypeId = eventTypes[3].EventTypeId, EventStart = now.AddDays(-24), EventEnd = now.AddDays(-24).AddHours(2), EventPoints = 150, MinLevel = 4 });
            eventList.Add(new Event { EventName = "Torneio de Ténis", EventDescription = "Torneio de pares.", EventTypeId = eventTypes[4].EventTypeId, EventStart = now.AddDays(-22), EventEnd = now.AddDays(-22).AddHours(5), EventPoints = 250, MinLevel = 3 });
            eventList.Add(new Event { EventName = "Seminário de Saúde Mental", EventDescription = "Bem-estar psicológico.", EventTypeId = eventTypes[5].EventTypeId, EventStart = now.AddDays(-20), EventEnd = now.AddDays(-20).AddHours(2), EventPoints = 55, MinLevel = 1 });
            eventList.Add(new Event { EventName = "Sessão de Personal Trainer", EventDescription = "Foco nos seus objetivos.", EventTypeId = eventTypes[6].EventTypeId, EventStart = now.AddDays(-18), EventEnd = now.AddDays(-18).AddHours(1), EventPoints = 90, MinLevel = 2 });
            eventList.Add(new Event { EventName = "Meia Maratona", EventDescription = "Corrida de 21km.", EventTypeId = eventTypes[7].EventTypeId, EventStart = now.AddDays(-16), EventEnd = now.AddDays(-16).AddHours(4), EventPoints = 300, MinLevel = 5 });
            eventList.Add(new Event { EventName = "Campeonato de Natação", EventDescription = "Vários estilos.", EventTypeId = eventTypes[8].EventTypeId, EventStart = now.AddDays(-14), EventEnd = now.AddDays(-14).AddHours(3), EventPoints = 280, MinLevel = 4 });
            eventList.Add(new Event { EventName = "Palestra Motivacional", EventDescription = "Alcance o seu potencial.", EventTypeId = eventTypes[9].EventTypeId, EventStart = now.AddDays(-12), EventEnd = now.AddDays(-12).AddHours(1), EventPoints = 50, MinLevel = 1 });
            eventList.Add(new Event { EventName = "Aula de Yoga Experimental", EventDescription = "Descubra o Yoga.", EventTypeId = eventTypes[10].EventTypeId, EventStart = now.AddDays(-10), EventEnd = now.AddDays(-10).AddHours(1), EventPoints = 60, MinLevel = 1 });
            eventList.Add(new Event { EventName = "Desafio de Powerlifting", EventDescription = "Supino, Agachamento e Peso Morto.", EventTypeId = eventTypes[11].EventTypeId, EventStart = now.AddDays(-8), EventEnd = now.AddDays(-8).AddHours(3), EventPoints = 180, MinLevel = 4 });
            eventList.Add(new Event { EventName = "Torneio de Voleibol", EventDescription = "Equipas de 4.", EventTypeId = eventTypes[12].EventTypeId, EventStart = now.AddDays(-6), EventEnd = now.AddDays(-6).AddHours(4), EventPoints = 220, MinLevel = 2 });
            eventList.Add(new Event { EventName = "Workshop de Mindfulness", EventDescription = "Técnicas de relaxamento.", EventTypeId = eventTypes[13].EventTypeId, EventStart = now.AddDays(-4), EventEnd = now.AddDays(-4).AddHours(2), EventPoints = 65, MinLevel = 1 });
            eventList.Add(new Event { EventName = "Aula de Técnica de Corrida", EventDescription = "Corra de forma eficiente.", EventTypeId = eventTypes[14].EventTypeId, EventStart = now.AddDays(-2), EventEnd = now.AddDays(-2).AddHours(1), EventPoints = 80, MinLevel = 2 });

            eventList.Add(new Event { EventName = "Desafio de Sprint", EventDescription = "Evento a decorrer agora.", EventTypeId = eventTypes[15].EventTypeId, EventStart = now.AddMinutes(-30), EventEnd = now.AddMinutes(30), EventPoints = 110, MinLevel = 2 });
            eventList.Add(new Event { EventName = "Liga de Basquetebol", EventDescription = "Jogo semanal.", EventTypeId = eventTypes[16].EventTypeId, EventStart = now.AddHours(-1), EventEnd = now.AddHours(1), EventPoints = 290, MinLevel = 3 });
            eventList.Add(new Event { EventName = "Demonstração de Artes Marciais", EventDescription = "Apresentação de técnicas.", EventTypeId = eventTypes[17].EventTypeId, EventStart = now.AddMinutes(-15), EventEnd = now.AddHours(1), EventPoints = 50, MinLevel = 1 });
            eventList.Add(new Event { EventName = "Treino de HIIT", EventDescription = "Alta intensidade.", EventTypeId = eventTypes[18].EventTypeId, EventStart = now.AddMinutes(-10), EventEnd = now.AddMinutes(45), EventPoints = 70, MinLevel = 2 });
            eventList.Add(new Event { EventName = "Competição de Crossfit", EventDescription = "WOD especial.", EventTypeId = eventTypes[19].EventTypeId, EventStart = now.AddHours(-2), EventEnd = now.AddHours(1), EventPoints = 190, MinLevel = 4 });

            eventList.Add(new Event { EventName = "Workshop Prático de Primeiros Socorros", EventDescription = "Saiba como agir.", EventTypeId = eventTypes[20].EventTypeId, EventStart = now.AddDays(1), EventEnd = now.AddDays(1).AddHours(3), EventPoints = 75, MinLevel = 1 });
            eventList.Add(new Event { EventName = "Aula Avançada de Spinning", EventDescription = "Suba a montanha.", EventTypeId = eventTypes[21].EventTypeId, EventStart = now.AddDays(2), EventEnd = now.AddDays(2).AddHours(1), EventPoints = 95, MinLevel = 3 });
            eventList.Add(new Event { EventName = "Desafio de Tiro ao Arco", EventDescription = "Teste a sua mira.", EventTypeId = eventTypes[22].EventTypeId, EventStart = now.AddDays(3), EventEnd = now.AddDays(3).AddHours(2), EventPoints = 100, MinLevel = 2 });
            eventList.Add(new Event { EventName = "Torneio de Xadrez", EventDescription = "Eliminatórias.", EventTypeId = eventTypes[23].EventTypeId, EventStart = now.AddDays(4), EventEnd = now.AddDays(4).AddHours(4), EventPoints = 260, MinLevel = 1 });
            eventList.Add(new Event { EventName = "Conferência de Medicina Desportiva", EventDescription = "Novas tendências.", EventTypeId = eventTypes[24].EventTypeId, EventStart = now.AddDays(5), EventEnd = now.AddDays(5).AddHours(6), EventPoints = 50, MinLevel = 1 });
            eventList.Add(new Event { EventName = "Aula de Pilates para Iniciantes", EventDescription = "Controle o seu corpo.", EventTypeId = eventTypes[25].EventTypeId, EventStart = now.AddDays(6), EventEnd = now.AddDays(6).AddHours(1), EventPoints = 65, MinLevel = 1 });
            eventList.Add(new Event { EventName = "Competição de Skate", EventDescription = "Melhor manobra.", EventTypeId = eventTypes[26].EventTypeId, EventStart = now.AddDays(7), EventEnd = now.AddDays(7).AddHours(3), EventPoints = 230, MinLevel = 2 });
            eventList.Add(new Event { EventName = "Workshop Teórico de Treino", EventDescription = "Planeamento de treino.", EventTypeId = eventTypes[27].EventTypeId, EventStart = now.AddDays(8), EventEnd = now.AddDays(8).AddHours(2), EventPoints = 50, MinLevel = 1 });
            eventList.Add(new Event { EventName = "Treino de Maratona (Grupo)", EventDescription = "Preparação conjunta.", EventTypeId = eventTypes[28].EventTypeId, EventStart = now.AddDays(9), EventEnd = now.AddDays(9).AddHours(2), EventPoints = 150, MinLevel = 3 });
            eventList.Add(new Event { EventName = "Desafio de Slackline", EventDescription = "Teste o seu equilíbrio.", EventTypeId = eventTypes[29].EventTypeId, EventStart = now.AddDays(10), EventEnd = now.AddDays(10).AddHours(2), EventPoints = 90, MinLevel = 2 });
            eventList.Add(new Event { EventName = "Campeonato de Judo", EventDescription = "Fases de grupos.", EventTypeId = eventTypes[30].EventTypeId, EventStart = now.AddDays(11), EventEnd = now.AddDays(11).AddHours(5), EventPoints = 270, MinLevel = 3 });
            eventList.Add(new Event { EventName = "Aula Especializada de Defesa Pessoal", EventDescription = "Técnicas essenciais.", EventTypeId = eventTypes[31].EventTypeId, EventStart = now.AddDays(12), EventEnd = now.AddDays(12).AddHours(2), EventPoints = 85, MinLevel = 1 });
            eventList.Add(new Event { EventName = "Workshop de Dança Contemporânea", EventDescription = "Movimento e expressão.", EventTypeId = eventTypes[32].EventTypeId, EventStart = now.AddDays(13), EventEnd = now.AddDays(13).AddHours(2), EventPoints = 70, MinLevel = 1 });
            eventList.Add(new Event { EventName = "Competição de Remo", EventDescription = "Contra-relógio.", EventTypeId = eventTypes[33].EventTypeId, EventStart = now.AddDays(14), EventEnd = now.AddDays(14).AddHours(3), EventPoints = 240, MinLevel = 3 });
            eventList.Add(new Event { EventName = "Treino de Flexibilidade (Grupo)", EventDescription = "Alongamentos profundos.", EventTypeId = eventTypes[34].EventTypeId, EventStart = now.AddDays(15), EventEnd = now.AddDays(15).AddHours(1), EventPoints = 75, MinLevel = 1 });
            eventList.Add(new Event { EventName = "Desafio de Parkour", EventDescription = "Circuito de obstáculos.", EventTypeId = eventTypes[35].EventTypeId, EventStart = now.AddDays(16), EventEnd = now.AddDays(16).AddHours(2), EventPoints = 160, MinLevel = 4 });
            eventList.Add(new Event { EventName = "Torneio de Padel", EventDescription = "Sistema Round Robin.", EventTypeId = eventTypes[36].EventTypeId, EventStart = now.AddDays(17), EventEnd = now.AddDays(17).AddHours(4), EventPoints = 250, MinLevel = 2 });
            eventList.Add(new Event { EventName = "Sessão de Orientação (Outdoor)", EventDescription = "Navegação e natureza.", EventTypeId = eventTypes[37].EventTypeId, EventStart = now.AddDays(18), EventEnd = now.AddDays(18).AddHours(3), EventPoints = 50, MinLevel = 1 });
            eventList.Add(new Event { EventName = "Aula de Boxe (Consolidação)", EventDescription = "Revisão de técnicas.", EventTypeId = eventTypes[38].EventTypeId, EventStart = now.AddDays(19), EventEnd = now.AddDays(19).AddHours(1), EventPoints = 65, MinLevel = 2 });
            eventList.Add(new Event { EventName = "Competição de E-Sports", EventDescription = "Torneio de FIFA.", EventTypeId = eventTypes[39].EventTypeId, EventStart = now.AddDays(20), EventEnd = now.AddDays(20).AddHours(5), EventPoints = 220, MinLevel = 1 });

            dbContext.Event.AddRange(eventList);
            dbContext.SaveChanges();
        }

        private static void PopulateLevels(HealthWellbeingDbContext dbContext)
        {
            if (dbContext.Level.Any()) return;

            dbContext.Level.AddRange(new List<Level>() {
                new Level { LevelAtual = 1, LevelCategory = "Iniciante", Description = "Primeiros passos na jornada de saúde" },
                new Level { LevelAtual = 2, LevelCategory = "Iniciante", Description = "Começando a criar rotinas saudáveis" },
                new Level { LevelAtual = 3, LevelCategory = "Iniciante", Description = "Ganhando consistência nos exercícios" },
                new Level { LevelAtual = 4, LevelCategory = "Iniciante", Description = "Progresso constante na saúde" },
                new Level { LevelAtual = 5, LevelCategory = "Iniciante", Description = "Final da fase inicial - bons hábitos estabelecidos" },
                new Level { LevelAtual = 6, LevelCategory = "Intermediário", Description = "Entrando na fase intermediária" },
                new Level { LevelAtual = 7, LevelCategory = "Intermediário", Description = "Desenvolvendo resistência física" },
                new Level { LevelAtual = 8, LevelCategory = "Intermediário", Description = "Melhorando performance geral" },
                new Level { LevelAtual = 9, LevelCategory = "Intermediário", Description = "Consolidação de técnicas avançadas" },
                new Level { LevelAtual = 10, LevelCategory = "Intermediário", Description = "Pronto para desafios maiores" },
                new Level { LevelAtual = 11, LevelCategory = "Avançado", Description = "Início da jornada avançada" },
                new Level { LevelAtual = 12, LevelCategory = "Avançado", Description = "Domínio de exercícios complexos" },
                new Level { LevelAtual = 13, LevelCategory = "Avançado", Description = "Excelência em treino cardiovascular" },
                new Level { LevelAtual = 14, LevelCategory = "Avançado", Description = "Especialização em força e resistência" },
                new Level { LevelAtual = 15, LevelCategory = "Avançado", Description = "Atleta completo em formação" },
                new Level { LevelAtual = 16, LevelCategory = "Especialista", Description = "Primeiro nível de especialista" },
                new Level { LevelAtual = 17, LevelCategory = "Especialista", Description = "Técnicas avançadas de condicionamento" },
                new Level { LevelAtual = 18, LevelCategory = "Especialista", Description = "Mestre em rotinas personalizadas" },
                new Level { LevelAtual = 19, LevelCategory = "Especialista", Description = "Referência na comunidade fitness" },
                new Level { LevelAtual = 20, LevelCategory = "Especialista", Description = "Especialista consolidado" },
                new Level { LevelAtual = 21, LevelCategory = "Mestre", Description = "Iniciando o caminho de mestre" },
                new Level { LevelAtual = 22, LevelCategory = "Mestre", Description = "Domínio completo de múltiplas modalidades" },
                new Level { LevelAtual = 23, LevelCategory = "Mestre", Description = "Liderança natural em treinos em grupo" },
                new Level { LevelAtual = 24, LevelCategory = "Mestre", Description = "Inspiração para outros utilizadores" },
                new Level { LevelAtual = 25, LevelCategory = "Mestre", Description = "Mestre em saúde e bem-estar" },
                new Level { LevelAtual = 26, LevelCategory = "Grão-Mestre", Description = "Primeiro nível de grão-mestre" },
                new Level { LevelAtual = 27, LevelCategory = "Grão-Mestre", Description = "Excelência em todos os aspectos do fitness" },
                new Level { LevelAtual = 28, LevelCategory = "Grão-Mestre", Description = "Conhecimento profundo de nutrição e exercício" },
                new Level { LevelAtual = 29, LevelCategory = "Grão-Mestre", Description = "Lenda em formação na aplicação" },
                new Level { LevelAtual = 30, LevelCategory = "Grão-Mestre", Description = "Grão-mestre consolidado" },
                new Level { LevelAtual = 31, LevelCategory = "Lendário", Description = "Entrada no hall lendário" },
                new Level { LevelAtual = 32, LevelCategory = "Lendário", Description = "Consistência lendária nos treinos" },
                new Level { LevelAtual = 33, LevelCategory = "Lendário", Description = "Performance excecional continuada" },
                new Level { LevelAtual = 34, LevelCategory = "Lendário", Description = "Ícone da aplicação" },
                new Level { LevelAtual = 35, LevelCategory = "Lendário", Description = "Lenda viva do fitness" },
                new Level { LevelAtual = 36, LevelCategory = "Mítico", Description = "Alcançando status mítico" },
                new Level { LevelAtual = 37, LevelCategory = "Mítico", Description = "Força e determinação sobre-humanas" },
                new Level { LevelAtual = 38, LevelCategory = "Mítico", Description = "Lenda entre lendas" },
                new Level { LevelAtual = 39, LevelCategory = "Mítico", Description = "Próximo do nível máximo" },
                new Level { LevelAtual = 40, LevelCategory = "Mítico", Description = "Nível máximo - Mito vivo da aplicação" }
            });
            dbContext.SaveChanges();
        }

        private static List<Client> PopulateClients(HealthWellbeingDbContext dbContext)
        {
            // Verifica se já existem clientes para não duplicar
            if (dbContext.Client.Any())
            {
                return dbContext.Client.ToList();
            }

            // Lista com 25 clientes
            var clients = new List<Client>()
            {
                // Os seus 5 clientes originais
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Alice Wonderland",
                    Email = "alice.w@example.com",
                    Phone = "555-1234567",
                    Address = "10 Downing St, London",
                    BirthDate = new DateTime(1990, 5, 15),
                    Gender = "Female",
                    RegistrationDate = DateTime.Now.AddDays(-30)
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Bob The Builder",
                    Email = "bob.builder@work.net",
                    Phone = "555-9876543",
                    Address = "Construction Site 5A",
                    BirthDate = new DateTime(1985, 10, 20),
                    Gender = "Male",
                    RegistrationDate = DateTime.Now.AddDays(-15),
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Charlie Brown",
                    Email = "charlie.b@peanuts.com",
                    Phone = "555-4567890",
                    Address = "123 Comic Strip Ave",
                    BirthDate = new DateTime(2000, 1, 1),
                    Gender = "Male",
                    RegistrationDate = DateTime.Now.AddDays(-5)
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "David Copperfield",
                    Email = "david.c@magic.com",
                    Phone = "555-9001002",
                    Address = "Las Vegas Strip",
                    BirthDate = new DateTime(1960, 9, 16),
                    Gender = "Male",
                    RegistrationDate = DateTime.Now.AddDays(-25)
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Eve Harrington",
                    Email = "eve.h@stage.net",
                    Phone = "555-3330009",
                    Address = "Broadway St",
                    BirthDate = new DateTime(1995, 2, 28),
                    Gender = "Female",
                    RegistrationDate = DateTime.Now.AddDays(-10)
                },
        
                // Mais 20 clientes para teste
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Frank Castle",
                    Email = "frank.c@punisher.com",
                    Phone = "555-1110001",
                    Address = "Hells Kitchen, NY",
                    BirthDate = new DateTime(1978, 3, 16),
                    Gender = "Male",
                    RegistrationDate = DateTime.Now.AddDays(-40)
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Grace Hopper",
                    Email = "grace.h@navy.mil",
                    Phone = "555-2220002",
                    Address = "Arlington, VA",
                    BirthDate = new DateTime(1906, 12, 9),
                    Gender = "Female",
                    RegistrationDate = DateTime.Now.AddDays(-100)
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Harry Potter",
                    Email = "harry.p@hogwarts.wiz",
                    Phone = "555-3330003",
                    Address = "4 Privet Drive, Surrey",
                    BirthDate = new DateTime(1980, 7, 31),
                    Gender = "Male",
                    RegistrationDate = DateTime.Now.AddDays(-12)
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Ivy Poison",
                    Email = "ivy.p@gotham.bio",
                    Phone = "555-4440004",
                    Address = "Gotham Gardens",
                    BirthDate = new DateTime(1988, 11, 2),
                    Gender = "Female",
                    RegistrationDate = DateTime.Now.AddDays(-3)
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Jack Sparrow",
                    Email = "jack.s@pirate.sea",
                    Phone = "555-5550005",
                    Address = "Tortuga",
                    BirthDate = new DateTime(1700, 4, 1),
                    Gender = "Male",
                    RegistrationDate = DateTime.Now.AddDays(-8)
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Kara Danvers",
                    Email = "kara.d@catco.com",
                    Phone = "555-6660006",
                    Address = "National City",
                    BirthDate = new DateTime(1993, 9, 22),
                    Gender = "Female",
                    RegistrationDate = DateTime.Now.AddDays(-22)
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Luke Skywalker",
                    Email = "luke.s@jedi.org",
                    Phone = "555-7770007",
                    Address = "Tatooine",
                    BirthDate = new DateTime(1977, 5, 25),
                    Gender = "Male",
                    RegistrationDate = DateTime.Now.AddDays(-18)
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Mona Lisa",
                    Email = "mona.l@art.com",
                    Phone = "555-8880008",
                    Address = "The Louvre, Paris",
                    BirthDate = new DateTime(1503, 6, 15),
                    Gender = "Female",
                    RegistrationDate = DateTime.Now.AddDays(-50)
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Neo Anderson",
                    Email = "neo.a@matrix.com",
                    Phone = "555-9990009",
                    Address = "Zion",
                    BirthDate = new DateTime(1971, 9, 13),
                    Gender = "Male",
                    RegistrationDate = DateTime.Now.AddDays(-2)
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Olivia Pope",
                    Email = "olivia.p@gladiator.com",
                    Phone = "555-1010010",
                    Address = "Washington D.C.",
                    BirthDate = new DateTime(1977, 4, 2),
                    Gender = "Female",
                    RegistrationDate = DateTime.Now.AddDays(-60)
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Peter Parker",
                    Email = "peter.p@bugle.com",
                    Phone = "555-2020011",
                    Address = "Queens, NY",
                    BirthDate = new DateTime(2001, 8, 10),
                    Gender = "Male",
                    RegistrationDate = DateTime.Now.AddDays(-7)
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Quinn Fabray",
                    Email = "quinn.f@glee.com",
                    Phone = "555-3030012",
                    Address = "Lima, Ohio",
                    BirthDate = new DateTime(1994, 7, 19),
                    Gender = "Female",
                    RegistrationDate = DateTime.Now.AddDays(-33)
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Rachel Green",
                    Email = "rachel.g@friends.com",
                    Phone = "555-4040013",
                    Address = "Central Perk, NY",
                    BirthDate = new DateTime(1970, 5, 5),
                    Gender = "Female",
                    RegistrationDate = DateTime.Now.AddDays(-45)
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Steve Rogers",
                    Email = "steve.r@avengers.com",
                    Phone = "555-5050014",
                    Address = "Brooklyn, NY",
                    BirthDate = new DateTime(1918, 7, 4),
                    Gender = "Male",
                    RegistrationDate = DateTime.Now.AddDays(-11)
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Tony Stark",
                    Email = "tony.s@stark.com",
                    Phone = "555-6060015",
                    Address = "Malibu Point, CA",
                    BirthDate = new DateTime(1970, 5, 29),
                    Gender = "Male",
                    RegistrationDate = DateTime.Now.AddDays(-90)
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Ursula Buffay",
                    Email = "ursula.b@friends.tv",
                    Phone = "555-7070016",
                    Address = "Riff's Bar, NY",
                    BirthDate = new DateTime(1968, 2, 22),
                    Gender = "Female",
                    RegistrationDate = DateTime.Now.AddDays(-14)
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Victor Frankenstein",
                    Email = "victor.f@science.ch",
                    Phone = "555-8080017",
                    Address = "Geneva, Switzerland",
                    BirthDate = new DateTime(1790, 10, 10),
                    Gender = "Male",
                    RegistrationDate = DateTime.Now.AddDays(-200)
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Walter White",
                    Email = "walter.w@heisenberg.com",
                    Phone = "555-9090018",
                    Address = "Albuquerque, NM",
                    BirthDate = new DateTime(1958, 9, 7),
                    Gender = "Male",
                    RegistrationDate = DateTime.Now.AddDays(-28)
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Xena Warrior",
                    Email = "xena.w@myth.gr",
                    Phone = "555-0100019",
                    Address = "Amphipolis, Greece",
                    BirthDate = new DateTime(1968, 3, 29),
                    Gender = "Female",
                    RegistrationDate = DateTime.Now.AddDays(-1)
                },
                new Client
                {
                    ClientId = Guid.NewGuid().ToString("N"),
                    Name = "Yoda Master",
                    Email = "yoda.m@jedi.org",
                    Phone = "555-1210020",
                    Address = "Dagobah System",
                    BirthDate = new DateTime(1000, 1, 1),
                    Gender = "Male",
                    RegistrationDate = DateTime.Now.AddDays(-500)
                }
            };

            // Adiciona todos os clientes à base de dados
            dbContext.Client.AddRange(clients);
            dbContext.SaveChanges();

            return clients;
        }
        private static void PopulateMember(HealthWellbeingDbContext dbContext, List<Client> clients)
        {
            if (dbContext.Member.Any()) return;

            var clientNamesToMakeMembers = new List<string> { "Alice Wonderland", "Charlie Brown", "David Copperfield" };

            var members = clients
                .Where(c => clientNamesToMakeMembers.Contains(c.Name))
                .Select(c => new Member
                {
                    ClientId = c.ClientId,
                })
                .ToList();

            if (members.Any())
            {
                dbContext.Member.AddRange(members);
                dbContext.SaveChanges();
            }
        }

        private static void PopulateTrainingType(HealthWellbeingDbContext dbContext)
        {
            // Check if the TrainingType table already contains data
            if (dbContext.TrainingType.Any()) return;

            dbContext.TrainingType.AddRange(new List<TrainingType>()
            {
                new TrainingType
                {
                    Name = "Yoga Basics",
                    Description = "A gentle introduction to yoga, focusing on flexibility, balance, and relaxation.",
                    DurationMinutes = 60,
                    IsActive = true
                },
                new TrainingType
                {
                    Name = "HIIT (High Intensity Interval Training)",
                    Description = "A fast-paced training session combining cardio and strength exercises for maximum calorie burn.",
                    DurationMinutes = 45,
                    IsActive = true
                },
                new TrainingType
                {
                    Name = "Pilates Core Strength",
                    Description = "Focus on core muscle strength, flexibility, and posture improvement.",
                    DurationMinutes = 50,
                    IsActive = true
                },
                new TrainingType
                {
                    Name = "Zumba Dance",
                    Description = "Fun and energetic dance workout set to upbeat Latin music.",
                    DurationMinutes = 55,
                    IsActive = true
                },
                new TrainingType
                {
                    Name = "Strength Training",
                    Description = "Weight-based training for building muscle mass and endurance.",
                    DurationMinutes = 120,
                    IsActive = true
                }
            });

            dbContext.SaveChanges();
        }

        private static void PopulatePlan(HealthWellbeingDbContext dbContext)
        {
            // Check if the Plan table already contains data
            if (dbContext.Plan.Any()) return;

            dbContext.Plan.AddRange(new List<Plan>()
            {
                new Plan
                {
                    Name = "Basic Wellness Plan",
                    Description = "A beginner-friendly plan including 3 workouts per week focused on flexibility and general health.",
                    Price = 29.99m,
                    DurationDays = 30
                },
                new Plan
                {
                    Name = "Advanced Fitness Plan",
                    Description = "An intensive 6-week plan designed for strength, endurance, and fat loss.",
                    Price = 59.99m,
                    DurationDays = 45
                },
                new Plan
                {
                    Name = "Mind & Body Balance",
                    Description = "A 2-month program combining yoga, meditation, and Pilates for mental and physical harmony.",
                    Price = 79.99m,
                    DurationDays = 60
                },
                new Plan
                {
                    Name = "Ultimate Transformation Plan",
                    Description = "A 3-month premium plan featuring personal coaching, nutrition guidance, and high-intensity training.",
                    Price = 99.99m,
                    DurationDays = 90
                },
                new Plan
                {
                    Name = "Corporate Health Boost",
                    Description = "A 1-month team-focused plan to improve workplace wellness, stress management, and physical activity.",
                    Price = 49.99m,
                    DurationDays = 30
                }
            });

            dbContext.SaveChanges();
        }

        // --- ALTERAÇÃO AQUI: O método agora retorna List<Trainer> ---
        private static List<Trainer> PopulateTrainer(HealthWellbeingDbContext dbContext)
        {
            // Check if Trainers already exist
            if (dbContext.Trainer.Any()) return dbContext.Trainer.ToList(); // Retorna se existirem

            dbContext.Trainer.AddRange(new List<Trainer>()
            {
                new Trainer
                {
                    Name = "John Smith",
                    Speciality = "HIIT (High Intensity Interval Training)",
                    Email = "john.smith@fitnesspro.com",
                    Phone = "555-1112233",
                    BirthDate = new DateTime(1988, 7, 10),
                    Gender = "Male"
                },
                new Trainer
                {
                    Name = "Emma Johnson",
                    Speciality = "Strength Training",
                    Email = "emma.johnson@strongfit.net",
                    Phone = "555-2223344",
                    BirthDate = new DateTime(1992, 11, 25),
                    Gender = "Female"
                },
                new Trainer
                {
                    Name = "Carlos Mendes",
                    Speciality = "Yoga Basics",
                    Email = "carlos.mendes@yogabalance.org",
                    Phone = "555-3334455",
                    BirthDate = new DateTime(1975, 4, 1),
                    Gender = "Male"
                },
                new Trainer
                {
                    Name = "Sophie Lee",
                    Speciality = "Pilates Core Strength",
                    Email = "sophie.lee@corewellness.com",
                    Phone = "555-4445566",
                    BirthDate = new DateTime(1996, 2, 14),
                    Gender = "Female"
                },
                new Trainer
                {
                    Name = "Maria Rodriguez",
                    Speciality = "Zumba Dance",
                    Email = "maria.rodriguez@zumbafit.com",
                    Phone = "555-5557788",
                    BirthDate = new DateTime(1985, 9, 30),
                    Gender = "Female"
                }
            });

            dbContext.SaveChanges();
            return dbContext.Trainer.ToList();
        }

        private static void PopulateTraining(HealthWellbeingDbContext dbContext, List<Trainer> trainers)
        {
            if (dbContext.Training.Any()) return;

            var yogaTypeId = dbContext.TrainingType.FirstOrDefault(t => t.Name == "Yoga Basics")?.TrainingTypeId;
            var hiitTypeId = dbContext.TrainingType.FirstOrDefault(t => t.Name == "HIIT (High Intensity Interval Training)")?.TrainingTypeId;

            var carlosId = trainers.FirstOrDefault(t => t.Name == "Carlos Mendes")?.TrainerId;
            var johnId = trainers.FirstOrDefault(t => t.Name == "John Smith")?.TrainerId;


            if (yogaTypeId.HasValue && hiitTypeId.HasValue && carlosId.HasValue && johnId.HasValue)
            {
                dbContext.Training.AddRange(new List<Training>()
                {
                    new Training
                    {
                        TrainingTypeId = yogaTypeId.Value,
                        TrainerId = carlosId.Value,
                        Name = "Morning Yoga",
                        Duration = 60,
                        DayOfWeek = "Monday",
                        StartTime = new TimeSpan(10, 0, 0),
                        MaxParticipants = 15
                    },
                    new Training
                    {
                        TrainingTypeId = hiitTypeId.Value,
                        TrainerId = johnId.Value,
                        Name = "Intense Cardio HIT",
                        Duration = 45,
                        DayOfWeek = "Wednesday",
                        StartTime = new TimeSpan(18, 30, 0),
                        MaxParticipants = 20
                    },
                     new Training
                    {
                        TrainingTypeId = hiitTypeId.Value,
                        TrainerId = johnId.Value,
                        Name = "Strength Training",
                        Duration = 120,
                        DayOfWeek = "Friday",
                        StartTime = new TimeSpan(16, 0, 0),
                        MaxParticipants = 8
                    }
                });

                dbContext.SaveChanges();
            }
        }

        private static void PopulateAgendaMedica(HealthWellbeingDbContext db)
        {
            var datas15 = GetProximosDiasUteis(DateOnly.FromDateTime(DateTime.Today), 15);

            // apaga tudo nessas datas (para todos os médicos)
            var existentes = db.AgendaMedica.Where(a => datas15.Contains(a.Data)).ToList();
            db.AgendaMedica.RemoveRange(existentes);
            db.SaveChanges();

            var medicos = db.Doctor
                .AsEnumerable()
                .GroupBy(m => m.Nome)
                .ToDictionary(g => g.Key, g => g.First());

            // 1) Template semanal
            var templateSemanal = new List<(string MedicoNome, DayOfWeek Dia, TimeOnly Ini, TimeOnly Fim)>
    {
        // João Ribeiro
        ("João Ribeiro", DayOfWeek.Monday,    new TimeOnly(9, 0),  new TimeOnly(12, 0)),
        ("João Ribeiro", DayOfWeek.Monday,    new TimeOnly(14, 0), new TimeOnly(17, 0)),
        ("João Ribeiro", DayOfWeek.Tuesday,   new TimeOnly(9, 0),  new TimeOnly(12, 0)),
        ("João Ribeiro", DayOfWeek.Wednesday, new TimeOnly(9, 0),  new TimeOnly(12, 0)),
        ("João Ribeiro", DayOfWeek.Thursday,  new TimeOnly(14, 0), new TimeOnly(17, 0)),
        ("João Ribeiro", DayOfWeek.Friday,    new TimeOnly(9, 0),  new TimeOnly(12, 0)),

        // Carla Ferreira
        ("Carla Ferreira", DayOfWeek.Monday,    new TimeOnly(14, 0), new TimeOnly(18, 0)),
        ("Carla Ferreira", DayOfWeek.Tuesday,   new TimeOnly(9, 0),  new TimeOnly(12, 0)),
        ("Carla Ferreira", DayOfWeek.Tuesday,   new TimeOnly(14, 0), new TimeOnly(16, 0)),
        ("Carla Ferreira", DayOfWeek.Wednesday, new TimeOnly(9, 0),  new TimeOnly(12, 0)),
        ("Carla Ferreira", DayOfWeek.Thursday,  new TimeOnly(9, 0),  new TimeOnly(12, 0)),
        ("Carla Ferreira", DayOfWeek.Thursday,  new TimeOnly(14, 0), new TimeOnly(16, 0)),
        ("Carla Ferreira", DayOfWeek.Friday,    new TimeOnly(14, 0), new TimeOnly(18, 0)),
    };

            

            // 3) Criar registos por data real + periodo
            var novos = new List<AgendaMedica>();

            foreach (var data in datas15)
            {
                foreach (var t in templateSemanal.Where(x => x.Dia == data.DayOfWeek))
                {
                    if (!medicos.TryGetValue(t.MedicoNome, out var medico))
                        continue;

                    // Regra simples para Periodo
                    var periodo = t.Ini < new TimeOnly(13, 0) ? "Manha" : "Tarde";

                    bool exists = db.AgendaMedica.Any(a =>
                        a.IdMedico == medico.IdMedico &&
                        a.Data == data &&
                        a.HoraInicio == t.Ini &&
                        a.HoraFim == t.Fim
                    );

                    if (!exists)
                    {
                        novos.Add(new AgendaMedica
                        {
                            IdMedico = medico.IdMedico,
                            Data = data,
                            DiaSemana = data.DayOfWeek,   // (melhor usar data.DayOfWeek)
                            Periodo = periodo,            // ✅ AGORA FICA GRAVADO
                            HoraInicio = t.Ini,
                            HoraFim = t.Fim
                        });
                    }
                }
            }

            if (novos.Any())
            {
                db.AgendaMedica.AddRange(novos);
                db.SaveChanges();
            }

            // 4) Corrigir dados antigos (opcional mas recomendado)
            // a) Apagar os com Data default
            var antigosDefault = db.AgendaMedica.Where(a => a.Data == default(DateOnly)).ToList();
            if (antigosDefault.Any())
            {
                db.AgendaMedica.RemoveRange(antigosDefault);
                db.SaveChanges();
            }

            // b) Preencher Periodo em registos existentes sem Periodo
            var semPeriodo = db.AgendaMedica
                .Where(a => a.Periodo == null || a.Periodo == "")
                .ToList();

            if (semPeriodo.Any())
            {
                foreach (var a in semPeriodo)
                {
                    a.Periodo = a.HoraInicio < new TimeOnly(13, 0) ? "Manha" : "Tarde";
                }

                db.SaveChanges();
            }
        }

        private static List<DateOnly> GetProximosDiasUteis(DateOnly inicio, int quantidade)
        {
            var res = new List<DateOnly>();
            var d = inicio;

            while (res.Count < quantidade)
            {
                if (d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday)
                    res.Add(d);

                d = d.AddDays(1);
            }

            return res;
        }


        internal static void SeedDefaultAdmin(UserManager<IdentityUser> userManager)
        {
            EnsureUserIsCreatedAsync(userManager, "admin@jbma.pt", "Secret123$", ["Administrador"]).Wait();
        }

        private static async Task EnsureUserIsCreatedAsync(UserManager<IdentityUser> userManager, string username, string password, string[] roles)
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

        internal static void SeedUsers(UserManager<IdentityUser> userManager)
        {
            EnsureUserIsCreatedAsync(userManager, "anab@jbma.pt", "Secret123$", ["Utente"]).Wait();
            EnsureUserIsCreatedAsync(userManager, "brunoMP@jbma.pt", "Secret123$", ["Utente"]).Wait();
            EnsureUserIsCreatedAsync(userManager, "diretorClinico@Healthwellbeing.pt", "Secret123$", ["DiretorClinico"]).Wait();
            EnsureUserIsCreatedAsync(userManager, "carla.ferreira@healthwellbeing.pt", "Secret123$", ["Medico"]).Wait();
            EnsureUserIsCreatedAsync(userManager, "bruno.carvalho@healthwellbeing.pt", "Secret123$", ["Medico"]).Wait();
            EnsureUserIsCreatedAsync(userManager, "ana.beatriz.silva@example.pt", "Secret123$", ["Utente"]).Wait();
            EnsureUserIsCreatedAsync(userManager, "ana.martins@healthwellbeing.pt", "Secret123$", ["Medico"]).Wait();
            
        }

        internal static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            EnsureRoleIsCreatedAsync(roleManager, "Administrador").Wait();
            EnsureRoleIsCreatedAsync(roleManager, "DiretorClinico").Wait();
            EnsureRoleIsCreatedAsync(roleManager, "Utente").Wait();
            EnsureRoleIsCreatedAsync(roleManager, "Medico").Wait();
            EnsureRoleIsCreatedAsync(roleManager, "Rececionista").Wait();
        }

        private static async Task EnsureRoleIsCreatedAsync(RoleManager<IdentityRole> roleManager, string role)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }


        
    }
}