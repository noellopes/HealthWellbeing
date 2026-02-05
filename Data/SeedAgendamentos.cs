using System;
using System.Linq;
using HealthWellbeing.Models;

namespace HealthWellbeing.Data
{
    public static class SeedAgendamentos
    {
        public static void Seed(ApplicationDbContext context)
        {
            // Evita duplicar seeds
            if (context.Agendamentos.Any())
                return;

            var terapeutas = context.Terapeutas.ToList();
            if (!terapeutas.Any())
                return;

            var estados = new[]
            {
                "Pendente",
                "Marcado",
                "Concluído",
                "Cancelado"
            };

            var inicioBase = DateTime.Today.AddHours(9);
            var totalAgendamentos = 23; // número realista, como já tinhas definido

            for (int i = 0; i < totalAgendamentos; i++)
            {
                var terapeuta = terapeutas[i % terapeutas.Count];
                var estado = estados[i % estados.Length];

                var inicio = inicioBase
                    .AddDays(i / 6)           // espalha por dias
                    .AddHours((i % 6) * 1.5); // horários diferentes

                context.Agendamentos.Add(new Agendamento
                {
                    DataHoraInicio = inicio,
                    DataHoraFim = inicio.AddHours(1),
                    Estado = estado,
                    TerapeutaId = terapeuta.TerapeutaId
                });
            }

            context.SaveChanges();
        }
    }
}