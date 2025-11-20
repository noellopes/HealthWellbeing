using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace HealthWellbeing.Data
{
    public static class SeedData
    {
        public static void Populate(HealthWellbeingDbContext? db)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));


            db.Database.EnsureCreated();

            PopulateSpecialities(db);
            PopulateConsultas(db);
            PopulateDoctor(db);
            PopulateUtenteSaude(db);
        }

        private static void PopulateConsultas(HealthWellbeingDbContext db)
        {
            if (db.Consulta.Any()) return;

            var hoje = DateTime.Today;

            var consulta = new[]
            {
                // -- Exemplo base --
                new Consulta
                {
                    DataMarcacao = new DateTime(2024, 4, 21, 10, 30, 0, DateTimeKind.Unspecified),
                    DataConsulta = new DateTime(2025, 4, 21, 10, 30, 0, DateTimeKind.Unspecified),
                    HoraInicio   = new TimeOnly(10, 30),
                    HoraFim      = new TimeOnly(11, 30),
                },

                // FUTURAS (Agendada)
                new Consulta
                {
                    DataMarcacao = new DateTime(2025, 10, 10, 9, 15, 0, DateTimeKind.Unspecified),
                    DataConsulta = new DateTime(2025, 11, 5, 9, 0, 0, DateTimeKind.Unspecified),
                    HoraInicio   = new TimeOnly(9, 0),
                    HoraFim      = new TimeOnly(9, 30),
                },
                new Consulta
                {
                    DataMarcacao = new DateTime(2025, 10, 12, 14, 40, 0, DateTimeKind.Unspecified),
                    DataConsulta = new DateTime(2025, 12, 1, 11, 15, 0, DateTimeKind.Unspecified),
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

                // HOJE (para testar “Hoje”)
                new Consulta
                {
                    DataMarcacao = hoje.AddDays(-2).AddHours(10),
                    DataConsulta = new DateTime(hoje.Year, hoje.Month, hoje.Day, 9, 30, 0, DateTimeKind.Unspecified),
                    HoraInicio   = new TimeOnly(9, 30),
                    HoraFim      = new TimeOnly(10, 0),
                },
                new Consulta
                {
                    DataMarcacao = hoje.AddDays(-1).AddHours(15).AddMinutes(20),
                    DataConsulta = new DateTime(hoje.Year, hoje.Month, hoje.Day, 14, 0, 0, DateTimeKind.Unspecified),
                    HoraInicio   = new TimeOnly(14, 0),
                    HoraFim      = new TimeOnly(14, 30),
                },

                // EXPIRADAS
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

                // CANCELADAS
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

                // MAIS FUTURAS
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
                new Consulta
                {
                    DataMarcacao = new DateTime(2025, 10, 27, 16, 10, 0, DateTimeKind.Unspecified),
                    DataConsulta = new DateTime(2026, 1, 22, 17, 0, 0, DateTimeKind.Unspecified),
                    HoraInicio   = new TimeOnly(17, 0),
                    HoraFim      = new TimeOnly(17, 30),
                }
            };

            db.Consulta.AddRange(consulta);
            db.SaveChanges();
        }

        private static void PopulateDoctor(HealthWellbeingDbContext db)
        {
            if (db.Doctor.Any())
            {
                return;
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

            db.Doctor.AddRange(doctor);
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
        private static void PopulateSpecialities(HealthWellbeingDbContext db)
        {
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

    }
}
