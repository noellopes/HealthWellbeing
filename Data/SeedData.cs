using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using HealthWellbeing.Models;

namespace HealthWellbeing.Data
{
    public static class SeedData
    {
        public static void Populate(HealthWellbeingDbContext db)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));

            // Ensure database is up-to-date
            db.Database.Migrate();

            // ==========================
            // SPECIALITIES & DOCTORS
            // ==========================
            if (!db.Specialities.Any())
            {
                var especialidades = new[]
                {
                    new Specialities { Nome = "Cardiologia", Descricao = "Avaliação, diagnóstico e tratamento de doenças do coração e sistema cardiovascular." },
                    new Specialities { Nome = "Dermatologia", Descricao = "Prevenção, diagnóstico e tratamento de doenças da pele, cabelo e unhas." },
                    new Specialities { Nome = "Pediatria", Descricao = "Cuidados de saúde para bebés, crianças e adolescentes." },
                    new Specialities { Nome = "Psiquiatria", Descricao = "Avaliação e tratamento de perturbações mentais, emocionais e comportamentais." },
                    new Specialities { Nome = "Nutrição", Descricao = "Aconselhamento alimentar e planos de nutrição para promoção da saúde e bem-estar." },
                    new Specialities { Nome = "Medicina Geral e Familiar", Descricao = "Acompanhamento global e contínuo da saúde de utentes e famílias." },
                    new Specialities { Nome = "Ortopedia", Descricao = "Tratamento de doenças e lesões dos ossos, articulações, músculos e tendões." },
                    new Specialities { Nome = "Ginecologia e Obstetrícia", Descricao = "Saúde da mulher, sistema reprodutor e acompanhamento da gravidez e parto." },
                    new Specialities { Nome = "Psicologia", Descricao = "Apoio psicológico, gestão emocional e acompanhamento em saúde mental." },
                    new Specialities { Nome = "Fisioterapia", Descricao = "Reabilitação motora e funcional após lesões, cirurgias ou doenças crónicas." }
                };
                db.Specialities.AddRange(especialidades);
                db.SaveChanges();
            }

            if (!db.Doctor.Any())
            {
                var especialidadesDict = db.Specialities.ToDictionary(e => e.Nome, e => e);
                var doctors = new[]
                {
                    new Doctor { Nome = "Ana Martins",    Telemovel = "912345678", Email = "ana.martins@healthwellbeing.pt",    Especialidade = especialidadesDict["Cardiologia"] },
                    new Doctor { Nome = "Bruno Carvalho", Telemovel = "913456789", Email = "bruno.carvalho@healthwellbeing.pt", Especialidade = especialidadesDict["Dermatologia"] },
                    new Doctor { Nome = "Carla Ferreira", Telemovel = "914567890", Email = "carla.ferreira@healthwellbeing.pt", Especialidade = especialidadesDict["Pediatria"] },
                    new Doctor { Nome = "Daniel Sousa",   Telemovel = "915678901", Email = "daniel.sousa@healthwellbeing.pt",   Especialidade = especialidadesDict["Psiquiatria"] },
                    new Doctor { Nome = "Eduarda Almeida",Telemovel = "916789012", Email = "eduarda.almeida@healthwellbeing.pt",Especialidade = especialidadesDict["Nutrição"] },
                    new Doctor { Nome = "Fábio Pereira",  Telemovel = "917890123", Email = "fabio.pereira@healthwellbeing.pt",  Especialidade = especialidadesDict["Medicina Geral e Familiar"] },
                    new Doctor { Nome = "Gabriela Rocha", Telemovel = "918901234", Email = "gabriela.rocha@healthwellbeing.pt", Especialidade = especialidadesDict["Ortopedia"] },
                    new Doctor { Nome = "Hugo Santos",    Telemovel = "919012345", Email = "hugo.santos@healthwellbeing.pt",    Especialidade = especialidadesDict["Ginecologia e Obstetrícia"] },
                    new Doctor { Nome = "Inês Correia",   Telemovel = "920123456", Email = "ines.correia@healthwellbeing.pt",   Especialidade = especialidadesDict["Psicologia"] },
                    new Doctor { Nome = "João Ribeiro",   Telemovel = "921234567", Email = "joao.ribeiro@healthwellbeing.pt",   Especialidade = especialidadesDict["Fisioterapia"] },
                    new Doctor { Nome = "Luísa Nogueira", Telemovel = "922345678", Email = "luisa.nogueira@healthwellbeing.pt", Especialidade = especialidadesDict["Medicina Geral e Familiar"] },
                    new Doctor { Nome = "Miguel Costa",   Telemovel = "923456789", Email = "miguel.costa@healthwellbeing.pt",   Especialidade = especialidadesDict["Pediatria"] },
                    new Doctor { Nome = "Nádia Gonçalves",Telemovel = "924567890", Email = "nadia.goncalves@healthwellbeing.pt",Especialidade = especialidadesDict["Cardiologia"] },
                    new Doctor { Nome = "Óscar Figueiredo",Telemovel="925678901", Email = "oscar.figueiredo@healthwellbeing.pt",Especialidade = especialidadesDict["Pediatria"] },
                    new Doctor { Nome = "Patrícia Lopes", Telemovel = "926789012", Email = "patricia.lopes@healthwellbeing.pt", Especialidade = especialidadesDict["Ginecologia e Obstetrícia"] }
                };
                db.Doctor.AddRange(doctors);
                db.SaveChanges();
            }

            if (!db.AgendaMedica.Any())
            {
                var proximosDias = GetProximosDiasUteis(DateOnly.FromDateTime(DateTime.Today), 15);

                var existentes = db.AgendaMedica.Where(a => proximosDias.Contains(a.Data)).ToList();
                if (existentes.Any())
                {
                    db.AgendaMedica.RemoveRange(existentes);
                    db.SaveChanges();
                }

                var medicos = db.Doctor.AsEnumerable().GroupBy(m => m.Nome).ToDictionary(g => g.Key, g => g.First());
                var templateSemanal = new List<(string MedicoNome, DayOfWeek Dia, TimeOnly Ini, TimeOnly Fim)>
                {
                    ("João Ribeiro", DayOfWeek.Monday,    new TimeOnly(9, 0),  new TimeOnly(12, 0)),
                    ("João Ribeiro", DayOfWeek.Monday,    new TimeOnly(14, 0), new TimeOnly(17, 0)),
                    ("João Ribeiro", DayOfWeek.Tuesday,   new TimeOnly(9, 0),  new TimeOnly(12, 0)),
                    ("João Ribeiro", DayOfWeek.Wednesday, new TimeOnly(9, 0),  new TimeOnly(12, 0)),
                    ("João Ribeiro", DayOfWeek.Thursday,  new TimeOnly(14, 0), new TimeOnly(17, 0)),
                    ("João Ribeiro", DayOfWeek.Friday,    new TimeOnly(9, 0),  new TimeOnly(12, 0)),

                    ("Carla Ferreira", DayOfWeek.Monday,    new TimeOnly(14, 0), new TimeOnly(18, 0)),
                    ("Carla Ferreira", DayOfWeek.Tuesday,   new TimeOnly(9, 0),  new TimeOnly(12, 0)),
                    ("Carla Ferreira", DayOfWeek.Tuesday,   new TimeOnly(14, 0), new TimeOnly(16, 0)),
                    ("Carla Ferreira", DayOfWeek.Wednesday, new TimeOnly(9, 0),  new TimeOnly(12, 0)),
                    ("Carla Ferreira", DayOfWeek.Thursday,  new TimeOnly(9, 0),  new TimeOnly(12, 0)),
                    ("Carla Ferreira", DayOfWeek.Thursday,  new TimeOnly(14, 0), new TimeOnly(16, 0)),
                    ("Carla Ferreira", DayOfWeek.Friday,    new TimeOnly(14, 0), new TimeOnly(18, 0))
                };

                var novosAgendamentos = new List<AgendaMedica>();
                foreach (var data in proximosDias)
                {
                    foreach (var t in templateSemanal.Where(x => x.Dia == data.DayOfWeek))
                    {
                        if (!medicos.TryGetValue(t.MedicoNome, out var medico)) continue;
                        var periodo = t.Ini < new TimeOnly(13, 0) ? "Manha" : "Tarde";

                        bool existe = db.AgendaMedica.Any(a =>
                            a.IdMedico == medico.IdMedico &&
                            a.Data == data &&
                            a.HoraInicio == t.Ini &&
                            a.HoraFim == t.Fim);
                        if (!existe)
                        {
                            novosAgendamentos.Add(new AgendaMedica
                            {
                                IdMedico = medico.IdMedico,
                                Data = data,
                                DiaSemana = data.DayOfWeek,
                                Periodo = periodo,
                                HoraInicio = t.Ini,
                                HoraFim = t.Fim
                            });
                        }
                    }
                }
                if (novosAgendamentos.Any())
                {
                    db.AgendaMedica.AddRange(novosAgendamentos);
                    db.SaveChanges();
                }

                var antigosDefault = db.AgendaMedica.Where(a => a.Data == default).ToList();
                if (antigosDefault.Any())
                {
                    db.AgendaMedica.RemoveRange(antigosDefault);
                    db.SaveChanges();
                }

                var semPeriodo = db.AgendaMedica.Where(a => string.IsNullOrEmpty(a.Periodo)).ToList();
                if (semPeriodo.Any())
                {
                    foreach (var a in semPeriodo)
                    {
                        a.Periodo = a.HoraInicio < new TimeOnly(13, 0) ? "Manha" : "Tarde";
                    }
                    db.SaveChanges();
                }
            }

            // ====================================
            // CLIENTS & UTENTES (1-to-1 relationship)
            // ====================================
            List<Client> newClients = new List<Client>();
            List<Client> utenteClients = new List<Client>();
            var utentesData = new List<(string Name, string Email, string Phone, string Address, DateTime BirthDate, string Gender, string Nif, string Niss, string Nus)>
            {
                ("Ana Beatriz Silva", "ana.beatriz.silva@example.pt", "912345670", "Rua das Flores, 12, Guarda", new DateTime(1999, 4, 8), "Female", "245379261", "12345678901", "123456789"),
                ("Bruno Miguel Pereira", "bruno.miguel.pereira@example.pt", "912345671", "Av. 25 de Abril, 102, Guarda", new DateTime(1987, 11, 23), "Male", "198754326", "22345678901", "223456789"),
                ("Carla Sofia Fernandes", "carla.sofia.fernandes@example.pt", "912345672", "Rua da Liberdade, 45, Covilhã", new DateTime(1991, 5, 19), "Female", "156987239", "32345678901", "323456789"),
                ("Daniel Rocha", "daniel.rocha@example.pt", "912345673", "Travessa do Sol, 3, Celorico da Beira", new DateTime(2003, 10, 26), "Male", "268945315", "42345678901", "423456789"),
                ("Eduarda Nogueira", "eduarda.nogueira@example.pt", "912345674", "Rua do Comércio, 89, Seia", new DateTime(1994, 5, 22), "Female", "296378459", "52345678901", "523456789"),
                ("Fábio Gonçalves", "fabio.goncalves@example.pt", "912345675", "Rua da Escola, 5, Gouveia", new DateTime(1997, 1, 4), "Male", "165947829", "62345678901", "623456789"),
                ("Gabriela Santos", "gabriela.santos@example.pt", "912345676", "Av. Dr. Francisco Sá Carneiro, 200, Viseu", new DateTime(1986, 4, 26), "Female", "189567240", "72345678901", "723456789"),
                ("Hugo Matos", "hugo.matos@example.pt", "912345677", "Rua do Castelo, 7, Belmonte", new DateTime(1993, 11, 22), "Male", "215983747", "82345678901", "823456789"),
                ("Inês Carvalho", "ines.carvalho@example.pt", "912345678", "Rua do Mercado, 14, Trancoso", new DateTime(2004, 7, 12), "Female", "235679845", "92345678901", "923456789"),
                ("João Marques", "joao.marques@example.pt", "912345679", "Rua da Estação, 33, Pinhel", new DateTime(1990, 7, 4), "Male", "286754197", "10345678901", "103456789"),
                ("Luísa Almeida", "luisa.almeida@example.pt", "912345680", "Rua da Lameira, 21, Manteigas", new DateTime(1978, 6, 19), "Female", "248963572", "11345678901", "113456789"),
                ("Miguel Figueiredo", "miguel.figueiredo@example.pt", "912345681", "Rua do Parque, 8, Almeida", new DateTime(1985, 8, 9), "Male", "196284739", "12345678902", "123456788"),
                ("Joana Moreira", "joana.moreira@example.pt", "913245671", "Rua das Amoreiras, 15, Lisboa", new DateTime(1988, 3, 14), "Female", "218945372", "11111111111", "111111111"),
                ("Carlos Almeida", "carlos.almeida@example.pt", "912334567", "Avenida 25 de Abril, 20, Porto", new DateTime(1975, 9, 22), "Male", "295431678", "11111111112", "111111112"),
                ("Sofia Marques", "sofia.marques@example.pt", "916785432", "Rua da Liberdade, 33, Coimbra", new DateTime(1992, 12, 5), "Female", "189546327", "11111111113", "111111113"),
                ("Ricardo Nogueira", "ricardo.nogueira@example.pt", "915889002", "Travessa do Sol, 2, Braga", new DateTime(1984, 2, 18), "Male", "239857416", "11111111114", "111111114"),
                ("Helena Rocha", "helena.rocha@example.pt", "917654320", "Rua das Flores, 44, Viseu", new DateTime(1990, 7, 21), "Female", "259784631", "11111111115", "111111115"),
                ("Tiago Fernandes", "tiago.fernandes@example.pt", "912120234", "Avenida dos Bombeiros, 12, Aveiro", new DateTime(1982, 4, 9), "Male", "268953741", "11111111116", "111111116"),
                ("Andreia Pinto", "andreia.pinto@example.pt", "916782543", "Rua de São João, 9, Guarda", new DateTime(1995, 6, 30), "Female", "235978416", "11111111117", "111111117"),
                ("Pedro Carvalho", "pedro.carvalho@example.pt", "913998877", "Rua do Comércio, 70, Castelo Branco", new DateTime(1978, 10, 12), "Male", "298671543", "11111111118", "111111118"),
                ("Marta Ribeiro", "marta.ribeiro@example.pt", "919776543", "Largo da Igreja, 22, Viana do Castelo", new DateTime(1987, 1, 7), "Female", "214896573", "11111111119", "111111119"),
                ("Luís Santos", "luis.santos@example.pt", "914563278", "Praceta do Parque, 5, Leiria", new DateTime(1980, 5, 27), "Male", "268974153", "11111111120", "111111120"),
                ("Filipa Gomes", "filipa.gomes@example.pt", "913445677", "Rua da Escola, 10, Évora", new DateTime(1991, 8, 19), "Female", "189574362", "11111111121", "111111121"),
                ("Rui Correia", "rui.correia@example.pt", "912233456", "Rua dos Pescadores, 45, Nazaré", new DateTime(1976, 3, 4), "Male", "278954136", "11111111122", "111111122"),
                ("Bárbara Figueiredo", "barbara.figueiredo@example.pt", "915667788", "Rua da Lameira, 31, Torres Vedras", new DateTime(1994, 11, 29), "Female", "215978643", "11111111123", "111111123"),
                ("Diogo Costa", "diogo.costa@example.pt", "914555666", "Rua dos Combatentes, 14, Santarém", new DateTime(1983, 12, 11), "Male", "268974523", "11111111124", "111111124"),
                ("Catarina Martins", "catarina.martins@example.pt", "916787654", "Avenida da Liberdade, 66, Lisboa", new DateTime(1998, 9, 5), "Female", "239876415", "11111111125", "111111125"),
                ("João Vieira", "joao.vieira@example.pt", "912444555", "Rua da Estação, 24, Braga", new DateTime(1979, 7, 15), "Male", "258946371", "11111111126", "111111126"),
                ("Carla Neves", "carla.neves@example.pt", "913998456", "Rua de Santa Maria, 88, Faro", new DateTime(1989, 2, 2), "Female", "215987436", "11111111127", "111111127"),
                ("Vítor Lopes", "vitor.lopes@example.pt", "912776543", "Travessa do Mercado, 15, Setúbal", new DateTime(1981, 1, 18), "Male", "276895413", "11111111128", "111111128"),
                ("Mariana Batista", "mariana.batista@example.pt", "914334566", "Rua de São Tiago, 18, Aveiro", new DateTime(1993, 4, 3), "Female", "289654137", "11111111129", "111111129"),
                ("Filipe Cruz", "filipe.cruz@example.pt", "916776554", "Rua da Liberdade, 10, Coimbra", new DateTime(1987, 11, 9), "Male", "295764821", "11111111130", "111111130"),
                ("Teresa Gonçalves", "teresa.goncalves@example.pt", "913221234", "Rua do Castelo, 19, Guimarães", new DateTime(1990, 3, 23), "Female", "198743265", "11111111131", "111111131"),
                ("Paulo Teixeira", "paulo.teixeira@example.pt", "912888999", "Avenida Central, 31, Braga", new DateTime(1975, 10, 14), "Male", "269841357", "11111111132", "111111132"),
                ("Sandra Ramos", "sandra.ramos@example.pt", "917776655", "Rua das Rosas, 12, Lisboa", new DateTime(1988, 1, 5), "Female", "235978462", "11111111133", "111111133"),
                ("Nuno Sousa", "nuno.sousa@example.pt", "914334221", "Travessa da Escola, 27, Aveiro", new DateTime(1992, 5, 27), "Male", "289635147", "11111111134", "111111134"),
                ("Patrícia Cardoso", "patricia.cardoso@example.pt", "915667899", "Rua do Campo, 7, Viseu", new DateTime(1983, 6, 8), "Female", "219846735", "11111111135", "111111135"),
                ("Gonçalo Rocha", "goncalo.rocha@example.pt", "913456789", "Avenida dos Aliados, 91, Porto", new DateTime(1985, 4, 11), "Male", "295687134", "11111111136", "111111136"),
                ("Leonor Ferreira", "leonor.ferreira@example.pt", "912998877", "Rua das Oliveiras, 18, Lisboa", new DateTime(1996, 2, 24), "Female", "218963475", "11111111137", "111111137"),
                ("André Mendes", "andre.mendes@example.pt", "916778899", "Rua da República, 15, Coimbra", new DateTime(1990, 12, 15), "Male", "259871436", "11111111138", "111111138"),
                ("Raquel Matos", "raquel.matos@example.pt", "913667788", "Rua de São João, 23, Braga", new DateTime(1989, 7, 7), "Female", "236985147", "11111111139", "111111139"),
                ("Henrique Azevedo", "henrique.azevedo@example.pt", "912443322", "Rua das Laranjeiras, 19, Porto", new DateTime(1977, 11, 25), "Male", "289634571", "11111111140", "111111140"),
                ("Beatriz Lopes", "beatriz.lopes@example.pt", "915223344", "Rua da Liberdade, 80, Lisboa", new DateTime(1995, 8, 2), "Female", "269841735", "11111111141", "111111141"),
                ("Miguel Ramos", "miguel.ramos@example.pt", "913332211", "Rua do Cruzeiro, 17, Viseu", new DateTime(1984, 9, 28), "Male", "259687314", "11111111142", "111111142"),
                ("Daniel Sousa", "daniel.sousa@example.pt", "919887766", "Travessa dos Combatentes, 21, Setúbal", new DateTime(1979, 3, 16), "Male", "285963147", "11111111143", "111111143"),
                ("Sílvia Ferreira", "silvia.ferreira@example.pt", "912667788", "Avenida da Liberdade, 9, Braga", new DateTime(1993, 4, 10), "Female", "239685471", "11111111144", "111111144"),
                ("Alexandre Pinto", "alexandre.pinto@example.pt", "916443322", "Rua das Escolas, 12, Coimbra", new DateTime(1986, 7, 21), "Male", "278954163", "11111111145", "111111145"),
                ("Vera Almeida", "vera.almeida@example.pt", "912443355", "Rua do Penedo, 5, Viseu", new DateTime(1990, 2, 19), "Female", "218964735", "11111111146", "111111146"),
                ("Nádia Marques", "nadia.marques@example.pt", "913998554", "Rua do Campo, 17, Évora", new DateTime(1998, 11, 8), "Female", "296847153", "11111111147", "111111147"),
                ("Hugo Barros", "hugo.barros@example.pt", "916555777", "Rua dos Carvalhos, 14, Porto", new DateTime(1981, 5, 4), "Male", "275986431", "11111111148", "111111148"),
                ("Joana Costa", "joana.costa@example.pt", "912224466", "Rua do Rossio, 11, Lisboa", new DateTime(1997, 6, 22), "Female", "285974316", "11111111149", "111111149"),
                ("Paula Rocha", "paula.rocha@example.pt", "915223455", "Rua da Fonte, 25, Aveiro", new DateTime(1988, 10, 27), "Female", "269854137", "11111111150", "111111150")
            };

            if (!db.Client.Any())
            {
                // 25 predefined clients
                newClients.AddRange(new List<Client>
                {
                    new Client { Name = "Alice Wonderland", Email = "alice.w@example.com", Phone = "555-1234567", Address = "10 Downing St, London", BirthDate = new DateTime(1990, 5, 15), Gender = "Female", RegistrationDate = DateTime.Now.AddDays(-30) },
                    new Client { Name = "Bob The Builder", Email = "bob.builder@work.net", Phone = "555-9876543", Address = "Construction Site 5A", BirthDate = new DateTime(1985, 10, 20), Gender = "Male", RegistrationDate = DateTime.Now.AddDays(-15) },
                    new Client { Name = "Charlie Brown", Email = "charlie.b@peanuts.com", Phone = "555-4567890", Address = "123 Comic Strip Ave", BirthDate = new DateTime(2000, 1, 1), Gender = "Male", RegistrationDate = DateTime.Now.AddDays(-5) },
                    new Client { Name = "David Copperfield", Email = "david.c@magic.com", Phone = "555-9001002", Address = "Las Vegas Strip", BirthDate = new DateTime(1960, 9, 16), Gender = "Male", RegistrationDate = DateTime.Now.AddDays(-25) },
                    new Client { Name = "Eve Harrington", Email = "eve.h@stage.net", Phone = "555-3330009", Address = "Broadway St", BirthDate = new DateTime(1995, 2, 28), Gender = "Female", RegistrationDate = DateTime.Now.AddDays(-10) },
                    new Client { Name = "Frank Castle", Email = "frank.c@punisher.com", Phone = "555-1110001", Address = "Hells Kitchen, NY", BirthDate = new DateTime(1978, 3, 16), Gender = "Male", RegistrationDate = DateTime.Now.AddDays(-40) },
                    new Client { Name = "Grace Hopper", Email = "grace.h@navy.mil", Phone = "555-2220002", Address = "Arlington, VA", BirthDate = new DateTime(1906, 12, 9), Gender = "Female", RegistrationDate = DateTime.Now.AddDays(-100) },
                    new Client { Name = "Harry Potter", Email = "harry.p@hogwarts.wiz", Phone = "555-3330003", Address = "4 Privet Drive, Surrey", BirthDate = new DateTime(1980, 7, 31), Gender = "Male", RegistrationDate = DateTime.Now.AddDays(-12) },
                    new Client { Name = "Ivy Poison", Email = "ivy.p@gotham.bio", Phone = "555-4440004", Address = "Gotham Gardens", BirthDate = new DateTime(1988, 11, 2), Gender = "Female", RegistrationDate = DateTime.Now.AddDays(-3) },
                    new Client { Name = "Jack Sparrow", Email = "jack.s@pirate.sea", Phone = "555-5550005", Address = "Tortuga", BirthDate = new DateTime(1700, 4, 1), Gender = "Male", RegistrationDate = DateTime.Now.AddDays(-8) },
                    new Client { Name = "Kara Danvers", Email = "kara.d@catco.com", Phone = "555-6660006", Address = "National City", BirthDate = new DateTime(1993, 9, 22), Gender = "Female", RegistrationDate = DateTime.Now.AddDays(-22) },
                    new Client { Name = "Luke Skywalker", Email = "luke.s@jedi.org", Phone = "555-7770007", Address = "Tatooine", BirthDate = new DateTime(1977, 5, 25), Gender = "Male", RegistrationDate = DateTime.Now.AddDays(-18) },
                    new Client { Name = "Mona Lisa", Email = "mona.l@art.com", Phone = "555-8880008", Address = "The Louvre, Paris", BirthDate = new DateTime(1503, 6, 15), Gender = "Female", RegistrationDate = DateTime.Now.AddDays(-50) },
                    new Client { Name = "Neo Anderson", Email = "neo.a@matrix.com", Phone = "555-9990009", Address = "Zion", BirthDate = new DateTime(1971, 9, 13), Gender = "Male", RegistrationDate = DateTime.Now.AddDays(-2) },
                    new Client { Name = "Olivia Pope", Email = "olivia.p@gladiator.com", Phone = "555-1010010", Address = "Washington D.C.", BirthDate = new DateTime(1977, 4, 2), Gender = "Female", RegistrationDate = DateTime.Now.AddDays(-60) },
                    new Client { Name = "Peter Parker", Email = "peter.p@bugle.com", Phone = "555-2020011", Address = "Queens, NY", BirthDate = new DateTime(2001, 8, 10), Gender = "Male", RegistrationDate = DateTime.Now.AddDays(-7) },
                    new Client { Name = "Quinn Fabray", Email = "quinn.f@glee.com", Phone = "555-3030012", Address = "Lima, Ohio", BirthDate = new DateTime(1994, 7, 19), Gender = "Female", RegistrationDate = DateTime.Now.AddDays(-33) },
                    new Client { Name = "Rachel Green", Email = "rachel.g@friends.com", Phone = "555-4040013", Address = "Central Perk, NY", BirthDate = new DateTime(1970, 5, 5), Gender = "Female", RegistrationDate = DateTime.Now.AddDays(-45) },
                    new Client { Name = "Steve Rogers", Email = "steve.r@avengers.com", Phone = "555-5050014", Address = "Brooklyn, NY", BirthDate = new DateTime(1918, 7, 4), Gender = "Male", RegistrationDate = DateTime.Now.AddDays(-11) },
                    new Client { Name = "Tony Stark", Email = "tony.s@stark.com", Phone = "555-6060015", Address = "Malibu Point, CA", BirthDate = new DateTime(1970, 5, 29), Gender = "Male", RegistrationDate = DateTime.Now.AddDays(-90) },
                    new Client { Name = "Ursula Buffay", Email = "ursula.b@friends.tv", Phone = "555-7070016", Address = "Riff's Bar, NY", BirthDate = new DateTime(1968, 2, 22), Gender = "Female", RegistrationDate = DateTime.Now.AddDays(-14) },
                    new Client { Name = "Victor Frankenstein", Email = "victor.f@science.ch", Phone = "555-8080017", Address = "Geneva, Switzerland", BirthDate = new DateTime(1790, 10, 10), Gender = "Male", RegistrationDate = DateTime.Now.AddDays(-200) },
                    new Client { Name = "Walter White", Email = "walter.w@heisenberg.com", Phone = "555-9090018", Address = "Albuquerque, NM", BirthDate = new DateTime(1958, 9, 7), Gender = "Male", RegistrationDate = DateTime.Now.AddDays(-28) },
                    new Client { Name = "Xena Warrior", Email = "xena.w@myth.gr", Phone = "555-0100019", Address = "Amphipolis, Greece", BirthDate = new DateTime(1968, 3, 29), Gender = "Female", RegistrationDate = DateTime.Now.AddDays(-1) },
                    new Client { Name = "Yoda Master", Email = "yoda.m@jedi.org", Phone = "555-1210020", Address = "Dagobah System", BirthDate = new DateTime(1000, 1, 1), Gender = "Male", RegistrationDate = DateTime.Now.AddDays(-500) }
                });

                // Create client entries for each utente (patient) with their personal data
                foreach (var (Name, Email, Phone, Address, BirthDate, Gender, Nif, Niss, Nus) in utentesData)
                {
                    var client = new Client
                    {
                        Name = Name,
                        Email = Email,
                        Phone = Phone,
                        Address = Address,
                        BirthDate = BirthDate,
                        Gender = Gender,
                        RegistrationDate = DateTime.Now
                    };
                    newClients.Add(client);
                    utenteClients.Add(client);
                }

                // Save all new clients
                db.Client.AddRange(newClients);
                db.SaveChanges();

                // Assign nutritional goals for each new client
                if (!db.Goal.Any())
                {
                    var rng = new Random();
                    var goals = new List<Goal>();
                    int idx = 0;
                    foreach (var client in newClients)
                    {
                        string goalName = idx % 3 == 0 ? "Weight Loss"
                                        : idx % 3 == 1 ? "Muscle Gain"
                                        : "Maintenance";
                        double weight = rng.Next(55, 95);
                        double activity = goalName == "Weight Loss" ? 1.3
                                       : goalName == "Muscle Gain" ? 1.7
                                       : 1.5;
                        double calories = weight * 22 * activity;
                        double protein = weight * 1.6;
                        double fat = calories * 0.27 / 9;
                        double proteinCal = protein * 4;
                        double hydrates = (calories - proteinCal - (fat * 9)) / 4;
                        goals.Add(new Goal
                        {
                            ClientId = client.ClientId,
                            GoalName = goalName,
                            DailyCalories = (int)calories,
                            DailyProtein = (int)protein,
                            DailyFat = (int)fat,
                            DailyHydrates = (int)hydrates
                        });
                        idx++;
                    }
                    db.Goal.AddRange(goals);
                    db.SaveChanges();
                }
            }

            // ==========================
            // UTENTES (Health Patients)
            // ==========================
            if (!db.UtenteSaude.Any())
            {
                var utentesToAdd = new List<UtenteSaude>();
                int i = 0;
                foreach (var (_, _, _, _, _, _, nif, niss, nus) in utentesData)
                {
                    if (i >= utenteClients.Count) break;
                    var client = utenteClients[i++];
                    utentesToAdd.Add(new UtenteSaude
                    {
                        ClientId = client.ClientId,
                        Nif = nif,
                        Niss = niss,
                        Nus = nus
                    });
                }
                if (utentesToAdd.Any())
                {
                    db.UtenteSaude.AddRange(utentesToAdd);
                    db.SaveChanges();
                }
            }

            // ==========================
            // CONSULTATIONS (Consultas)
            // ==========================
            if (!db.Consulta.Any())
            {
                var hoje = DateTime.Today;
                var doctorsList = db.Doctor.OrderBy(d => d.IdMedico).ToList();
                var utentesList = db.UtenteSaude.OrderBy(u => u.UtenteSaudeId).ToList();
                if (!doctorsList.Any() || !utentesList.Any())
                    throw new InvalidOperationException("É necessário ter médicos e utentes antes de gerar consultas.");

                var consultas = new List<Consulta>
                {
                    new Consulta
                    {
                        DataMarcacao = new DateTime(2024, 4, 21, 10, 30, 0),
                        DataConsulta = new DateTime(2025, 4, 21, 10, 30, 0),
                        HoraInicio   = new TimeOnly(10, 30),
                        HoraFim      = new TimeOnly(11, 30)
                    },
                    new Consulta
                    {
                        DataMarcacao = new DateTime(2025, 10, 10, 9, 15, 0),
                        DataConsulta = new DateTime(2026, 1, 5, 9, 0, 0),
                        HoraInicio   = new TimeOnly(9, 0),
                        HoraFim      = new TimeOnly(9, 30)
                    },
                    new Consulta
                    {
                        DataMarcacao = new DateTime(2025, 10, 12, 14, 40, 0),
                        DataConsulta = new DateTime(2026, 1, 10, 11, 15, 0),
                        HoraInicio   = new TimeOnly(11, 15),
                        HoraFim      = new TimeOnly(12, 0)
                    },
                    new Consulta
                    {
                        DataMarcacao = new DateTime(2025, 10, 15, 16, 5, 0),
                        DataConsulta = new DateTime(2026, 1, 10, 15, 0, 0),
                        HoraInicio   = new TimeOnly(15, 0),
                        HoraFim      = new TimeOnly(15, 45)
                    },
                    new Consulta
                    {
                        DataMarcacao = new DateTime(2025, 10, 20, 10, 10, 0),
                        DataConsulta = new DateTime(2025, 11, 20, 16, 30, 0),
                        HoraInicio   = new TimeOnly(16, 30),
                        HoraFim      = new TimeOnly(17, 0)
                    },
                    new Consulta
                    {
                        DataMarcacao = hoje.AddDays(-2).AddHours(10),
                        DataConsulta = new DateTime(hoje.Year, hoje.Month, hoje.Day, 9, 30, 0),
                        HoraInicio   = new TimeOnly(15, 30),
                        HoraFim      = new TimeOnly(16, 0)
                    },
                    new Consulta
                    {
                        DataMarcacao = hoje.AddDays(-1).AddHours(15).AddMinutes(20),
                        DataConsulta = new DateTime(hoje.Year, hoje.Month, hoje.Day, 14, 0, 0),
                        HoraInicio   = new TimeOnly(14, 0),
                        HoraFim      = new TimeOnly(14, 30)
                    },
                    new Consulta
                    {
                        DataMarcacao = new DateTime(2025, 9, 1, 10, 0, 0),
                        DataConsulta = new DateTime(2025, 9, 15, 9, 0, 0),
                        HoraInicio   = new TimeOnly(9, 0),
                        HoraFim      = new TimeOnly(9, 30)
                    },
                    new Consulta
                    {
                        DataMarcacao = new DateTime(2025, 8, 20, 11, 25, 0),
                        DataConsulta = new DateTime(2025, 9, 25, 11, 45, 0),
                        HoraInicio   = new TimeOnly(11, 45),
                        HoraFim      = new TimeOnly(12, 15)
                    },
                    new Consulta
                    {
                        DataMarcacao = new DateTime(2025, 7, 5, 13, 10, 0),
                        DataConsulta = new DateTime(2025, 8, 10, 16, 0, 0),
                        HoraInicio   = new TimeOnly(16, 0),
                        HoraFim      = new TimeOnly(16, 30)
                    },
                    new Consulta
                    {
                        DataMarcacao     = new DateTime(2025, 10, 1, 10, 0, 0),
                        DataConsulta     = new DateTime(2025, 10, 30, 9, 0, 0),
                        DataCancelamento = new DateTime(2025, 10, 28, 9, 30, 0),
                        HoraInicio       = new TimeOnly(9, 0),
                        HoraFim          = new TimeOnly(9, 30)
                    },
                    new Consulta
                    {
                        DataMarcacao     = new DateTime(2025, 9, 15, 11, 30, 0),
                        DataConsulta     = new DateTime(2025, 10, 10, 15, 0, 0),
                        DataCancelamento = new DateTime(2025, 10, 8, 10, 0, 0),
                        HoraInicio       = new TimeOnly(15, 0),
                        HoraFim          = new TimeOnly(15, 45)
                    },
                    new Consulta
                    {
                        DataMarcacao     = new DateTime(2025, 6, 10, 12, 0, 0),
                        DataConsulta     = new DateTime(2025, 7, 5, 10, 30, 0),
                        DataCancelamento = new DateTime(2025, 7, 3, 14, 15, 0),
                        HoraInicio       = new TimeOnly(10, 30),
                        HoraFim          = new TimeOnly(11, 0)
                    },
                    new Consulta
                    {
                        DataMarcacao = new DateTime(2025, 10, 22, 9, 45, 0),
                        DataConsulta = new DateTime(2025, 11, 15, 13, 30, 0),
                        HoraInicio   = new TimeOnly(13, 30),
                        HoraFim      = new TimeOnly(14, 15)
                    },
                    new Consulta
                    {
                        DataMarcacao = new DateTime(2025, 10, 25, 8, 55, 0),
                        DataConsulta = new DateTime(2025, 12, 12, 8, 30, 0),
                        HoraInicio   = new TimeOnly(8, 30),
                        HoraFim      = new TimeOnly(9, 0)
                    }
                };

                for (int i = 0; i < consultas.Count; i++)
                {
                    var d = doctorsList[i % doctorsList.Count];
                    var u = utentesList[i % utentesList.Count];
                    consultas[i].IdMedico = d.IdMedico;
                    consultas[i].IdEspecialidade = d.IdEspecialidade;
                    consultas[i].IdUtenteSaude = u.UtenteSaudeId;
                }

                foreach (var c in consultas)
                {
                    bool exists = db.Consulta.Any(x =>
                        x.IdMedico == c.IdMedico &&
                        x.IdUtenteSaude == c.IdUtenteSaude &&
                        x.DataConsulta == c.DataConsulta &&
                        x.HoraInicio == c.HoraInicio);
                    if (!exists)
                    {
                        db.Consulta.Add(c);
                    }
                }
                db.SaveChanges();
            }

            // ==========================
            // MEMBERS (Gym Members)
            // ==========================
            if (!db.Member.Any())
            {
                var memberNames = new List<string> { "Alice Wonderland", "Charlie Brown", "David Copperfield" };
                var memberClients = db.Client.Where(c => memberNames.Contains(c.Name)).ToList();
                if (memberClients.Any())
                {
                    var members = memberClients.Select(c => new Member { ClientId = c.ClientId }).ToList();
                    db.Member.AddRange(members);
                    db.SaveChanges();
                }
            }

            // ==========================
            // TRAINING TYPES
            // ==========================
            if (!db.TrainingType.Any())
            {
                db.TrainingType.AddRange(new List<TrainingType>
                {
                    new TrainingType { Name = "Yoga Basics", Description = "A gentle introduction to yoga, focusing on flexibility, balance, and relaxation.", DurationMinutes = 60, IsActive = true },
                    new TrainingType { Name = "HIIT (High Intensity Interval Training)", Description = "A fast-paced training session combining cardio and strength exercises for maximum calorie burn.", DurationMinutes = 45, IsActive = true },
                    new TrainingType { Name = "Pilates Core Strength", Description = "Focus on core muscle strength, flexibility, and posture improvement.", DurationMinutes = 50, IsActive = true },
                    new TrainingType { Name = "Zumba Dance", Description = "Fun and energetic dance workout set to upbeat Latin music.", DurationMinutes = 55, IsActive = true },
                    new TrainingType { Name = "Strength Training", Description = "Weight-based training for building muscle mass and endurance.", DurationMinutes = 120, IsActive = true }
                });
                db.SaveChanges();
            }

            // ==========================
            // PLANS (Client-specific plans)
            // ==========================
            if (!db.Plan.Any())
            {
                var allClients = db.Client.OrderBy(c => c.ClientId).ToList();
                if (!allClients.Any()) return;

                var plans = new List<Plan>();
                DateTime today = DateTime.Today;

                int count = Math.Min(allClients.Count, 30);

                for (int i = 0; i < count; i++)
                {
                    var start = today.AddDays(-i * 7);
                    plans.Add(new Plan
                    {
                        ClientId = allClients[i].ClientId,
                        StartingDate = start,
                        EndingDate = start.AddDays(30),
                        Done = (i % 3 == 0)
                    });
                }

                db.Plan.AddRange(plans);
                db.SaveChanges();
            }


            // ==========================
            // NUTRITIONISTS
            // ==========================
            if (!db.Nutritionist.Any())
            {
                var nutritionists = new List<Nutritionist>
                {
                    new Nutritionist { Name = "Dr. Joao Carvalho", Email = "joao.carvalho@healthwellbeing.com", Gender = "Male" },
                    new Nutritionist { Name = "Dr. Sofia Martins", Email = "sofia.martins@healthwellbeing.com", Gender = "Female" },
                    new Nutritionist { Name = "Dr. Ricardo Soares", Email = "ricardo.soares@healthwellbeing.com", Gender = "Male" }
                };
                for (int i = 4; i <= 30; i++)
                {
                    nutritionists.Add(new Nutritionist
                    {
                        Name = $"Nutritionist {i}",
                        Email = $"nutritionist{i}@healthwellbeing.com",
                        Gender = i % 2 == 0 ? "Male" : "Female"
                    });
                }
                db.Nutritionist.AddRange(nutritionists);
                db.SaveChanges();
            }

            // ==========================
            // ALERGIES
            // ==========================
            if (!db.Alergy.Any())
            {
                var alergies = new List<Alergy>
                {
                    new Alergy { AlergyName = "Peanuts" },
                    new Alergy { AlergyName = "Tree nuts" },
                    new Alergy { AlergyName = "Lactose" },
                    new Alergy { AlergyName = "Gluten" },
                    new Alergy { AlergyName = "Seafood" },
                    new Alergy { AlergyName = "Eggs" },
                    new Alergy { AlergyName = "Soy" },
                    new Alergy { AlergyName = "Sesame" },
                    new Alergy { AlergyName = "Strawberries" },
                    new Alergy { AlergyName = "Kiwi" }
                };
                for (int i = alergies.Count + 1; i <= 30; i++)
                {
                    alergies.Add(new Alergy { AlergyName = $"Test Allergy {i}" });
                }
                db.Alergy.AddRange(alergies);
                db.SaveChanges();
            }

            // ==========================
            // FOOD CATEGORIES
            // ==========================
            if (!db.FoodCategory.Any())
            {
                var categories = new List<FoodCategory>
                {
                    new FoodCategory { Category = "Fruits",      Description = "Fresh fruits and berries" },
                    new FoodCategory { Category = "Vegetables",  Description = "Fresh and cooked vegetables" },
                    new FoodCategory { Category = "Grains",      Description = "Cereals, bread and pasta" },
                    new FoodCategory { Category = "Proteins",    Description = "Meat, fish, eggs and legumes" },
                    new FoodCategory { Category = "Dairy",       Description = "Milk and dairy products" },
                    new FoodCategory { Category = "Fats & Oils", Description = "Healthy fats and oils" },
                    new FoodCategory { Category = "Snacks",      Description = "Snack foods" },
                    new FoodCategory { Category = "Drinks",      Description = "Non-alcoholic beverages" },
                    new FoodCategory { Category = "Breakfast",   Description = "Breakfast foods" },
                    new FoodCategory { Category = "Desserts",    Description = "Desserts and sweets" }
                };
                for (int i = categories.Count + 1; i <= 30; i++)
                {
                    categories.Add(new FoodCategory { Category = $"Category {i}", Description = "Auto-generated test category" });
                }
                db.FoodCategory.AddRange(categories);
                db.SaveChanges();
            }

            // ==========================
            // FOODS
            // ==========================
            if (!db.Food.Any())
            {
                var categories = db.FoodCategory.OrderBy(c => c.CategoryId).ToList();
                if (categories.Any())
                {
                    int fruitsId = categories.First(c => c.Category == "Fruits").CategoryId;
                    int vegetablesId = categories.First(c => c.Category == "Vegetables").CategoryId;
                    int grainsId = categories.First(c => c.Category == "Grains").CategoryId;
                    int proteinsId = categories.First(c => c.Category == "Proteins").CategoryId;
                    int dairyId = categories.First(c => c.Category == "Dairy").CategoryId;
                    var foods = new List<Food>
                    {
                        new Food { CategoryId = fruitsId,     Name = "Apple" },
                        new Food { CategoryId = fruitsId,     Name = "Banana" },
                        new Food { CategoryId = fruitsId,     Name = "Orange" },
                        new Food { CategoryId = fruitsId,     Name = "Strawberries" },
                        new Food { CategoryId = fruitsId,     Name = "Blueberries" },
                        new Food { CategoryId = vegetablesId, Name = "Broccoli" },
                        new Food { CategoryId = vegetablesId, Name = "Carrots" },
                        new Food { CategoryId = vegetablesId, Name = "Spinach" },
                        new Food { CategoryId = vegetablesId, Name = "Tomato" },
                        new Food { CategoryId = vegetablesId, Name = "Cucumber" },
                        new Food { CategoryId = grainsId,     Name = "White Rice" },
                        new Food { CategoryId = grainsId,     Name = "Brown Rice" },
                        new Food { CategoryId = grainsId,     Name = "Whole Wheat Bread" },
                        new Food { CategoryId = grainsId,     Name = "Oatmeal" },
                        new Food { CategoryId = grainsId,     Name = "Pasta" },
                        new Food { CategoryId = proteinsId,   Name = "Chicken Breast" },
                        new Food { CategoryId = proteinsId,   Name = "Salmon" },
                        new Food { CategoryId = proteinsId,   Name = "Tofu" },
                        new Food { CategoryId = proteinsId,   Name = "Eggs" },
                        new Food { CategoryId = proteinsId,   Name = "Lentils" },
                        new Food { CategoryId = dairyId,      Name = "Milk" },
                        new Food { CategoryId = dairyId,      Name = "Yogurt" },
                        new Food { CategoryId = dairyId,      Name = "Cheddar Cheese" },
                        new Food { CategoryId = dairyId,      Name = "Cottage Cheese" }
                    };
                    for (int i = foods.Count + 1; i <= 30; i++)
                    {
                        var cat = categories[(i - 1) % categories.Count];
                        foods.Add(new Food { CategoryId = cat.CategoryId, Name = $"Test Food {i}" });
                    }
                    db.Food.AddRange(foods);
                    db.SaveChanges();
                }
            }

            // ==========================
            // PORTIONS
            // ==========================
            if (!db.Portion.Any())
            {
                var portions = new List<Portion>
                {
                    new Portion { PortionName = "Small portion (50 g)" },
                    new Portion { PortionName = "Medium portion (100 g)" },
                    new Portion { PortionName = "Large portion (150 g)" },
                    new Portion { PortionName = "Cup cooked" },
                    new Portion { PortionName = "Cup raw" },
                    new Portion { PortionName = "Slice(s)" },
                    new Portion { PortionName = "Glass (200 ml)" },
                    new Portion { PortionName = "Tablespoon" },
                    new Portion { PortionName = "Teaspoon" }
                };
                db.Portion.AddRange(portions);
                db.SaveChanges();
            }

            // ==========================
            // NUTRITIONAL COMPONENTS
            // ==========================
            if (!db.NutritionalComponent.Any())
            {
                var comps = new List<NutritionalComponent>
                {
                    new NutritionalComponent { Name = "Energy",       Unit = "kcal", Basis = "per 100 g" },
                    new NutritionalComponent { Name = "Protein",      Unit = "g",    Basis = "per 100 g" },
                    new NutritionalComponent { Name = "Carbohydrate", Unit = "g",    Basis = "per 100 g" },
                    new NutritionalComponent { Name = "Fat",          Unit = "g",    Basis = "per 100 g" },
                    new NutritionalComponent { Name = "Fiber",        Unit = "g",    Basis = "per 100 g" },
                    new NutritionalComponent { Name = "Sugar",        Unit = "g",    Basis = "per 100 g" },
                    new NutritionalComponent { Name = "Sodium",       Unit = "mg",   Basis = "per 100 g" },
                    new NutritionalComponent { Name = "Calcium",      Unit = "mg",   Basis = "per 100 g" },
                    new NutritionalComponent { Name = "Vitamin C",    Unit = "mg",   Basis = "per 100 g" },
                    new NutritionalComponent { Name = "Iron",         Unit = "mg",   Basis = "per 100 g" }
                };
                db.NutritionalComponent.AddRange(comps);
                db.SaveChanges();
            }

            // ==========================
            // CLIENT-ALERGY (many-to-many)
            // ==========================
            if (!db.ClientAlergy.Any())
            {
                var clientsAll = db.Client.OrderBy(c => c.ClientId).ToList();
                var alergiesAll = db.Alergy.OrderBy(a => a.AlergyId).ToList();
                if (clientsAll.Any() && alergiesAll.Any())
                {
                    var clientAlergies = new List<ClientAlergy>();
                    int counter = 0;
                    for (int i = 0; i < clientsAll.Count && counter < 40; i++)
                    {
                        for (int j = 0; j < alergiesAll.Count && counter < 40; j++)
                        {
                            if ((i + j) % 4 == 0)
                            {
                                clientAlergies.Add(new ClientAlergy { ClientId = clientsAll[i].ClientId, AlergyId = alergiesAll[j].AlergyId });
                                counter++;
                                if (counter >= 40) break;
                            }
                        }
                    }
                    if (clientAlergies.Any())
                    {
                        db.ClientAlergy.AddRange(clientAlergies);
                        db.SaveChanges();
                    }
                }
            }

            // ==========================
            // NUTRITIONIST-CLIENT-PLAN (many-to-many)
            // ==========================
            if (!db.NutritionistClientPlan.Any())
            {
                var clientsAll = db.Client.OrderBy(c => c.ClientId).ToList();
                var nutritAll = db.Nutritionist.OrderBy(n => n.NutritionistId).ToList();
                var plansAll = db.Plan.OrderBy(p => p.PlanId).ToList();
                if (clientsAll.Any() && nutritAll.Any() && plansAll.Any())
                {
                    var linkList = new List<NutritionistClientPlan>();
                    int counter = 0;
                    for (int i = 0; i < clientsAll.Count && counter < 40; i++)
                    {
                        for (int j = 0; j < nutritAll.Count && counter < 40; j++)
                        {
                            var plan = plansAll[(i + j) % plansAll.Count];
                            linkList.Add(new NutritionistClientPlan
                            {
                                ClientId = clientsAll[i].ClientId,
                                NutritionistId = nutritAll[j].NutritionistId,
                                PlanId = plan.PlanId
                            });
                            counter++;
                            if (counter >= 40) break;
                        }
                    }
                    if (linkList.Any())
                    {
                        db.NutritionistClientPlan.AddRange(linkList);
                        db.SaveChanges();
                    }
                }
            }

            // ==========================
            // FOOD PLANS (Plan + Food + Portion)
            // ==========================
            if (!db.FoodPlan.Any())
            {
                var plansAll = db.Plan.OrderBy(p => p.PlanId).ToList();
                var foodsAll = db.Food.OrderBy(f => f.FoodId).ToList();
                var portionsAll = db.Portion.OrderBy(p => p.PortionId).ToList();
                if (plansAll.Any() && foodsAll.Any() && portionsAll.Any())
                {
                    var defaultPortion = portionsAll.First();
                    var foodPlans = new List<FoodPlan>();
                    void AddFoodsToPlan(Plan plan, int startIndex, int count)
                    {
                        for (int i = 0; i < count && (startIndex + i) < foodsAll.Count; i++)
                        {
                            var food = foodsAll[startIndex + i];
                            foodPlans.Add(new FoodPlan { PlanId = plan.PlanId, FoodId = food.FoodId, PortionId = defaultPortion.PortionId });
                        }
                    }
                    if (plansAll.Count >= 1) AddFoodsToPlan(plansAll[0], 0, 4);
                    if (plansAll.Count >= 2) AddFoodsToPlan(plansAll[1], 4, 5);
                    if (plansAll.Count >= 3) AddFoodsToPlan(plansAll[2], 9, 3);
                    if (foodPlans.Any())
                    {
                        db.FoodPlan.AddRange(foodPlans);
                        db.SaveChanges();
                    }
                }
            }

            // ==========================
            // DAILY FOOD PLAN SCHEDULE
            // ==========================
            if (!db.FoodPlanDay.Any())
            {
                var today = DateTime.Today;
                var plansAll = db.Plan.OrderBy(p => p.PlanId).ToList();
                var baseFoodPlans = db.FoodPlan.AsNoTracking().OrderBy(fp => fp.PlanId).ThenBy(fp => fp.FoodId).ToList();
                if (plansAll.Any() && baseFoodPlans.Any())
                {
                    var rng = new Random();
                    var dayPlans = new List<FoodPlanDay>();
                    foreach (var plan in plansAll)
                    {
                        var foodsForPlan = baseFoodPlans.Where(fp => fp.PlanId == plan.PlanId).ToList();
                        if (!foodsForPlan.Any()) continue;
                        for (int d = 0; d < 7; d++)
                        {
                            var date = today.AddDays(d);
                            foreach (var fp in foodsForPlan)
                            {
                                dayPlans.Add(new FoodPlanDay
                                {
                                    PlanId = plan.PlanId,
                                    FoodId = fp.FoodId,
                                    PortionId = fp.PortionId,
                                    Date = date,
                                    PortionsPlanned = rng.Next(1, 4),
                                    ScheduledTime = date.AddHours(9),
                                    MealType = "Daily"
                                });
                            }
                        }
                    }
                    if (dayPlans.Any())
                    {
                        db.FoodPlanDay.AddRange(dayPlans);
                        db.SaveChanges();
                    }
                }
            }

            // ==========================
            // FOOD INTAKE (initially not consumed)
            // ==========================
            if (!db.FoodIntake.Any())
            {
                var days = db.FoodPlanDay.AsNoTracking().ToList();
                if (days.Any())
                {
                    var intakeList = days.Select(x => new FoodIntake
                    {
                        PlanId = x.PlanId,
                        FoodId = x.FoodId,
                        PortionId = x.PortionId,
                        Date = x.Date,
                        ScheduledTime = x.ScheduledTime ?? x.Date.AddHours(9),
                        PortionsPlanned = x.PortionsPlanned,
                        PortionsEaten = 0
                    }).ToList();
                    db.FoodIntake.AddRange(intakeList);
                    db.SaveChanges();
                }
            }

            // ==========================
            // FOOD NUTRITIONAL COMPONENTS
            // ==========================
            if (!db.FoodNutritionalComponent.Any())
            {
                var foodsAll = db.Food.OrderBy(f => f.FoodId).ToList();
                var compsAll = db.NutritionalComponent.OrderBy(c => c.NutritionalComponentId).ToList();
                if (foodsAll.Any() && compsAll.Any())
                {
                    var foodComps = new List<FoodNutritionalComponent>();
                    int counter = 0;
                    foreach (var food in foodsAll)
                    {
                        foreach (var comp in compsAll)
                        {
                            foodComps.Add(new FoodNutritionalComponent
                            {
                                FoodId = food.FoodId,
                                NutritionalComponentId = comp.NutritionalComponentId,
                                Value = 5 + (counter % 20)
                            });
                            counter++;
                            if (counter >= 80) break;
                        }
                        if (counter >= 80) break;
                    }
                    if (foodComps.Any())
                    {
                        db.FoodNutritionalComponent.AddRange(foodComps);
                        db.SaveChanges();
                    }
                }
            }

            // ==========================
            // EVENT TYPES & EVENTS
            // ==========================
            if (!db.EventType.Any())
            {
                db.EventType.AddRange(new List<EventType>
                {
                    // EDUCAÇÃO E FORMAÇÃO
                    new EventType { EventTypeName = "Workshop Educacional", EventTypeScoringMode = "fixed", EventTypeMultiplier = 1.0f },
                    new EventType { EventTypeName = "Seminário Temático", EventTypeScoringMode = "fixed", EventTypeMultiplier = 1.1f },
                    new EventType { EventTypeName = "Palestra Informativa", EventTypeScoringMode = "fixed", EventTypeMultiplier = 1.0f },
                    new EventType { EventTypeName = "Demonstração Técnica", EventTypeScoringMode = "fixed", EventTypeMultiplier = 1.0f },
                    new EventType { EventTypeName = "Sessão de Orientação", EventTypeScoringMode = "fixed", EventTypeMultiplier = 1.0f },

                    // TREINO CARDIOVASCULAR
                    new EventType { EventTypeName = "Sessão de Running", EventTypeScoringMode = "time_based", EventTypeMultiplier = 2.0f },
                    new EventType { EventTypeName = "Treino de Cycling", EventTypeScoringMode = "time_based", EventTypeMultiplier = 2.1f },
                    new EventType { EventTypeName = "Aula de Cardio-Grupo", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.5f },
                    new EventType { EventTypeName = "Treino de Natação", EventTypeScoringMode = "time_based", EventTypeMultiplier = 2.2f },
                    new EventType { EventTypeName = "Sessão de HIIT", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.8f },

                    // TREINO DE FORÇA
                    new EventType { EventTypeName = "Treino de Musculação", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.7f },
                    new EventType { EventTypeName = "Sessão de CrossFit", EventTypeScoringMode = "progressive", EventTypeMultiplier = 2.1f },
                    new EventType { EventTypeName = "Treino Funcional", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.6f },
                    new EventType { EventTypeName = "Aula de Powerlifting", EventTypeScoringMode = "progressive", EventTypeMultiplier = 2.0f },
                    new EventType { EventTypeName = "Treino de Calistenia", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.5f },

                    // BEM-ESTAR E MOBILIDADE
                    new EventType { EventTypeName = "Aula de Yoga", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.3f },
                    new EventType { EventTypeName = "Sessão de Pilates", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.4f },
                    new EventType { EventTypeName = "Treino de Flexibilidade", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.5f },
                    new EventType { EventTypeName = "Aula de Mobilidade", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.3f },
                    new EventType { EventTypeName = "Sessão de Alongamento", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.2f },

                    // DESPORTOS E ARTES MARCIAIS
                    new EventType { EventTypeName = "Aula de Artes Marciais", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.8f },
                    new EventType { EventTypeName = "Treino de Boxe", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.9f },
                    new EventType { EventTypeName = "Sessão de Lutas", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.8f },
                    new EventType { EventTypeName = "Aula de Defesa Pessoal", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.4f },
                    new EventType { EventTypeName = "Treino Desportivo Coletivo", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.6f },

                    // DESAFIOS E COMPETIÇÕES
                    new EventType { EventTypeName = "Competição de Running", EventTypeScoringMode = "time_based", EventTypeMultiplier = 2.5f },
                    new EventType { EventTypeName = "Torneio Desportivo", EventTypeScoringMode = "binary", EventTypeMultiplier = 2.3f },
                    new EventType { EventTypeName = "Desafio de Resistência", EventTypeScoringMode = "time_based", EventTypeMultiplier = 2.4f },
                    new EventType { EventTypeName = "Competição de Força", EventTypeScoringMode = "progressive", EventTypeMultiplier = 2.2f },
                    new EventType { EventTypeName = "Desafio de Superação", EventTypeScoringMode = "progressive", EventTypeMultiplier = 2.1f },

                    // ATIVIDADES EM GRUPO
                    new EventType { EventTypeName = "Aula de Grupo", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.4f },
                    new EventType { EventTypeName = "Treino Coletivo", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.5f },
                    new EventType { EventTypeName = "Workshop Prático", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.3f },
                    new EventType { EventTypeName = "Sessão de Team Building", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.2f },
                    new EventType { EventTypeName = "Aula Experimental", EventTypeScoringMode = "binary", EventTypeMultiplier = 1.1f },

                    // ESPECIALIZADOS E TÉCNICOS
                    new EventType { EventTypeName = "Treino Técnico", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.6f },
                    new EventType { EventTypeName = "Workshop de Técnica", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.3f },
                    new EventType { EventTypeName = "Aula Avançada", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.7f },
                    new EventType { EventTypeName = "Sessão de Perfeiçoamento", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.5f },
                    new EventType { EventTypeName = "Treino Especializado", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.8f }
                });
                db.SaveChanges();
            }

            if (!db.Event.Any())
            {
                var eventTypes = db.EventType.ToList();
                if (eventTypes.Any())
                {
                    var now = DateTime.Now;
                    var eventList = new List<Event>();
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

                    // Events happening now
                    eventList.Add(new Event { EventName = "Desafio de Sprint", EventDescription = "Evento a decorrer agora.", EventTypeId = eventTypes[15].EventTypeId, EventStart = now.AddMinutes(-30), EventEnd = now.AddMinutes(30), EventPoints = 110, MinLevel = 2 });
                    eventList.Add(new Event { EventName = "Liga de Basquetebol", EventDescription = "Jogo semanal.", EventTypeId = eventTypes[16].EventTypeId, EventStart = now.AddHours(-1), EventEnd = now.AddHours(1), EventPoints = 290, MinLevel = 3 });
                    eventList.Add(new Event { EventName = "Demonstração de Artes Marciais", EventDescription = "Apresentação de técnicas.", EventTypeId = eventTypes[17].EventTypeId, EventStart = now.AddMinutes(-15), EventEnd = now.AddHours(1), EventPoints = 50, MinLevel = 1 });
                    eventList.Add(new Event { EventName = "Treino de HIIT", EventDescription = "Alta intensidade.", EventTypeId = eventTypes[18].EventTypeId, EventStart = now.AddMinutes(-10), EventEnd = now.AddMinutes(45), EventPoints = 70, MinLevel = 2 });
                    eventList.Add(new Event { EventName = "Competição de Crossfit", EventDescription = "WOD especial.", EventTypeId = eventTypes[19].EventTypeId, EventStart = now.AddHours(-2), EventEnd = now.AddHours(1), EventPoints = 190, MinLevel = 4 });

                    // Future events
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
                    db.Event.AddRange(eventList);
                    db.SaveChanges();
                }
            }

            if (!db.Level.Any())
            {
                db.Level.AddRange(new List<Level>
                {
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
                db.SaveChanges();
            }

            // ==========================
            // TRAINERS & TRAINING SESSIONS
            // ==========================
            if (!db.Trainer.Any())
            {
                db.Trainer.AddRange(new List<Trainer>
                {
                    new Trainer { Name = "John Smith", Speciality = "HIIT (High Intensity Interval Training)", Email = "john.smith@fitnesspro.com", Phone = "555-1112233", BirthDate = new DateTime(1988, 7, 10), Gender = "Male" },
                    new Trainer { Name = "Emma Johnson", Speciality = "Strength Training", Email = "emma.johnson@strongfit.net", Phone = "555-2223344", BirthDate = new DateTime(1992, 11, 25), Gender = "Female" },
                    new Trainer { Name = "Carlos Mendes", Speciality = "Yoga Basics", Email = "carlos.mendes@yogabalance.org", Phone = "555-3334455", BirthDate = new DateTime(1975, 4, 1), Gender = "Male" },
                    new Trainer { Name = "Sophie Lee", Speciality = "Pilates Core Strength", Email = "sophie.lee@corewellness.com", Phone = "555-4445566", BirthDate = new DateTime(1996, 2, 14), Gender = "Female" },
                    new Trainer { Name = "Maria Rodriguez", Speciality = "Zumba Dance", Email = "maria.rodriguez@zumbafit.com", Phone = "555-5557788", BirthDate = new DateTime(1985, 9, 30), Gender = "Female" }
                });
                db.SaveChanges();
            }
            var trainerList = db.Trainer.ToList();
            if (!db.Training.Any() && trainerList.Any())
            {
                var trainerCarlos = trainerList.FirstOrDefault(t => t.Name == "Carlos Mendes");
                var trainerJohn = trainerList.FirstOrDefault(t => t.Name == "John Smith");
                var yogaType = db.TrainingType.FirstOrDefault(t => t.Name == "Yoga Basics");
                var hiitType = db.TrainingType.FirstOrDefault(t => t.Name == "HIIT (High Intensity Interval Training)");
                if (trainerCarlos != null && trainerJohn != null && yogaType != null && hiitType != null)
                {
                    var trainings = new List<Training>
                    {
                        new Training { TrainingTypeId = yogaType.TrainingTypeId, TrainerId = trainerCarlos.TrainerId, Name = "Morning Yoga", Duration = 60, DayOfWeek = "Monday", StartTime = new TimeSpan(10, 0, 0), MaxParticipants = 15 },
                        new Training { TrainingTypeId = hiitType.TrainingTypeId, TrainerId = trainerJohn.TrainerId, Name = "Intense Cardio HIT", Duration = 45, DayOfWeek = "Wednesday", StartTime = new TimeSpan(18, 30, 0), MaxParticipants = 20 },
                        new Training { TrainingTypeId = hiitType.TrainingTypeId, TrainerId = trainerJohn.TrainerId, Name = "Strength Training", Duration = 120, DayOfWeek = "Friday", StartTime = new TimeSpan(16, 0, 0), MaxParticipants = 8 }
                    };
                    db.Training.AddRange(trainings);
                    db.SaveChanges();
                }
            }
        }

        // Helper: get next N working days (Mon-Fri) from a start date
        private static List<DateOnly> GetProximosDiasUteis(DateOnly inicio, int quantidade)
        {
            var result = new List<DateOnly>();
            var date = inicio;
            while (result.Count < quantidade)
            {
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                    result.Add(date);
                date = date.AddDays(1);
            }
            return result;
        }
    }
}
