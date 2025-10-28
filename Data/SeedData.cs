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
    }
}
