using HealthWellbeing.Data;
using HealthWellbeing.Models;

namespace HealthWellbeing.Data
{
    public static class SeedData
    {
        public static void Populate(HealthWellbeingDbContext? db)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));

            // Garante que a BD existe
            db.Database.EnsureCreated();

            PopulateConsultas(db);
            PopulateDoctor(db);
        }

        private static void PopulateConsultas(HealthWellbeingDbContext db)
        {


            // Base para datas/horas
            var hoje = DateTime.Today;



            var consulta = new[]
    {
        // -- Manténs o teu exemplo --
        new Consulta
        {
            DataMarcacao = new DateTime(2024,4,21,10,30,0, DateTimeKind.Unspecified),
            DataConsulta = new DateTime(2025,4,21,10,30,0, DateTimeKind.Unspecified),
            HoraInicio = new TimeOnly(10,30),
            HoraFim = new TimeOnly(11,30),
        },

        // FUTURAS (Agendada)
        new Consulta
        {
            DataMarcacao = new DateTime(2025,10,10,09,15,0, DateTimeKind.Unspecified),
            DataConsulta  = new DateTime(2025,11,05,09,00,0, DateTimeKind.Unspecified),
            HoraInicio    = new TimeOnly(9,00),
            HoraFim       = new TimeOnly(9,30),
        },
        new Consulta
        {
            DataMarcacao = new DateTime(2025,10,12,14,40,0, DateTimeKind.Unspecified),
            DataConsulta  = new DateTime(2025,12,01,11,15,0, DateTimeKind.Unspecified),
            HoraInicio    = new TimeOnly(11,15),
            HoraFim       = new TimeOnly(12,00),
        },
        new Consulta
        {
            DataMarcacao = new DateTime(2025,10,15,16,05,0, DateTimeKind.Unspecified),
            DataConsulta  = new DateTime(2026,01,10,15,00,0, DateTimeKind.Unspecified),
            HoraInicio    = new TimeOnly(15,00),
            HoraFim       = new TimeOnly(15,45),
        },
        new Consulta
        {
            DataMarcacao = new DateTime(2025,10,20,10,10,0, DateTimeKind.Unspecified),
            DataConsulta  = new DateTime(2025,11,20,16,30,0, DateTimeKind.Unspecified),
            HoraInicio    = new TimeOnly(16,30),
            HoraFim       = new TimeOnly(17,00),
        },

        // HOJE (marcadas para o dia atual — útil para ver o estado “Hoje”)
        new Consulta
        {
            DataMarcacao = hoje.AddDays(-2).AddHours(10).AddMinutes(0),
            DataConsulta = new DateTime(hoje.Year, hoje.Month, hoje.Day, 9, 30, 0, DateTimeKind.Unspecified),
            HoraInicio   = new TimeOnly(9,30),
            HoraFim      = new TimeOnly(10,00),
        },
        new Consulta
        {
            DataMarcacao = hoje.AddDays(-1).AddHours(15).AddMinutes(20),
            DataConsulta = new DateTime(hoje.Year, hoje.Month, hoje.Day, 14, 0, 0, DateTimeKind.Unspecified),
            HoraInicio   = new TimeOnly(14,00),
            HoraFim      = new TimeOnly(14,30),
        },

        // EXPIRADAS (passado, sem cancelamento)
        new Consulta
        {
            DataMarcacao = new DateTime(2025,09,01,10,00,0, DateTimeKind.Unspecified),
            DataConsulta  = new DateTime(2025,09,15,09,00,0, DateTimeKind.Unspecified),
            HoraInicio    = new TimeOnly(9,00),
            HoraFim       = new TimeOnly(9,30),
        },
        new Consulta
        {
            DataMarcacao = new DateTime(2025,08,20,11,25,0, DateTimeKind.Unspecified),
            DataConsulta  = new DateTime(2025,09,25,11,45,0, DateTimeKind.Unspecified),
            HoraInicio    = new TimeOnly(11,45),
            HoraFim       = new TimeOnly(12,15),
        },
        new Consulta
        {
            DataMarcacao = new DateTime(2025,07,05,13,10,0, DateTimeKind.Unspecified),
            DataConsulta  = new DateTime(2025,08,10,16,00,0, DateTimeKind.Unspecified),
            HoraInicio    = new TimeOnly(16,00),
            HoraFim       = new TimeOnly(16,30),
        },

        // CANCELADAS (com DataCancelamento preenchida)
        new Consulta
        {
            DataMarcacao     = new DateTime(2025,10,01,10,00,0, DateTimeKind.Unspecified),
            DataConsulta     = new DateTime(2025,10,30,9,00,0, DateTimeKind.Unspecified),
            DataCancelamento = new DateTime(2025,10,28,9,30,0, DateTimeKind.Unspecified),
            HoraInicio       = new TimeOnly(9,00),
            HoraFim          = new TimeOnly(9,30),
        },
        new Consulta
        {
            DataMarcacao     = new DateTime(2025,09,15,11,30,0, DateTimeKind.Unspecified),
            DataConsulta     = new DateTime(2025,10,10,15,00,0, DateTimeKind.Unspecified),
            DataCancelamento = new DateTime(2025,10,08,10,00,0, DateTimeKind.Unspecified),
            HoraInicio       = new TimeOnly(15,00),
            HoraFim          = new TimeOnly(15,45),
        },
        new Consulta
        {
            DataMarcacao     = new DateTime(2025,06,10,12,00,0, DateTimeKind.Unspecified),
            DataConsulta     = new DateTime(2025,07,05,10,30,0, DateTimeKind.Unspecified),
            DataCancelamento = new DateTime(2025,07,03,14,15,0, DateTimeKind.Unspecified),
            HoraInicio       = new TimeOnly(10,30),
            HoraFim          = new TimeOnly(11,00),
        },

        // MAIS FUTURAS
        new Consulta
        {
            DataMarcacao = new DateTime(2025,10,22,9,45,0, DateTimeKind.Unspecified),
            DataConsulta  = new DateTime(2025,11,15,13,30,0, DateTimeKind.Unspecified),
            HoraInicio    = new TimeOnly(13,30),
            HoraFim       = new TimeOnly(14,15),
        },
        new Consulta
        {
            DataMarcacao = new DateTime(2025,10,25,8,55,0, DateTimeKind.Unspecified),
            DataConsulta  = new DateTime(2025,12,12,8,30,0, DateTimeKind.Unspecified),
            HoraInicio    = new TimeOnly(8,30),
            HoraFim       = new TimeOnly(9,00),
        },
        new Consulta
        {
            DataMarcacao = new DateTime(2025,10,27,16,10,0, DateTimeKind.Unspecified),
            DataConsulta  = new DateTime(2026,01,22,17,00,0, DateTimeKind.Unspecified),
            HoraInicio    = new TimeOnly(17,00),
            HoraFim       = new TimeOnly(17,30),
        },
    };

            db.Consulta.AddRange(consulta);
            db.SaveChanges();
        }

        private static void PopulateDoctor(HealthWellbeingDbContext db)
        {

            var doctor = new[]
            {
        new Doctor { Nome = "Ana Martins",       Telemovel = "912345678", Email = "ana.martins@healthwellbeing.pt" },
        new Doctor { Nome = "Bruno Carvalho",    Telemovel = "913456789", Email = "bruno.carvalho@healthwellbeing.pt" },
        new Doctor { Nome = "Carla Ferreira",    Telemovel = "914567890", Email = "carla.ferreira@healthwellbeing.pt" },
        new Doctor { Nome = "Daniel Sousa",      Telemovel = "915678901", Email = "daniel.sousa@healthwellbeing.pt" },
        new Doctor { Nome = "Eduarda Almeida",   Telemovel = "916789012", Email = "eduarda.almeida@healthwellbeing.pt" },
        new Doctor { Nome = "Fábio Pereira",     Telemovel = "917890123", Email = "fabio.pereira@healthwellbeing.pt" },
        new Doctor { Nome = "Gabriela Rocha",    Telemovel = "918901234", Email = "gabriela.rocha@healthwellbeing.pt" },
        new Doctor { Nome = "Hugo Santos",       Telemovel = "919012345", Email = "hugo.santos@healthwellbeing.pt" },
        new Doctor { Nome = "Inês Correia",      Telemovel = "920123456", Email = "ines.correia@healthwellbeing.pt" },
        new Doctor { Nome = "João Ribeiro",      Telemovel = "921234567", Email = "joao.ribeiro@healthwellbeing.pt" },
        new Doctor { Nome = "Luísa Nogueira",    Telemovel = "922345678", Email = "luisa.nogueira@healthwellbeing.pt" },
        new Doctor { Nome = "Miguel Costa",      Telemovel = "923456789", Email = "miguel.costa@healthwellbeing.pt" },
        new Doctor { Nome = "Nádia Gonçalves",   Telemovel = "924567890", Email = "nadia.goncalves@healthwellbeing.pt" },
        new Doctor { Nome = "Óscar Figueiredo",  Telemovel = "925678901", Email = "oscar.figueiredo@healthwellbeing.pt" },
        new Doctor { Nome = "Patrícia Lopes",    Telemovel = "926789012", Email = "patricia.lopes@healthwellbeing.pt" },
    };

            db.Doctor.AddRange(doctor);
            db.SaveChanges();
        }


    }
}


