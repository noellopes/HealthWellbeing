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

            // Serviços "manuais" (sem FK)
            var servicos = new[]
            {
                new { Id = 1, Nome = "Massagem Relaxante" },
                new { Id = 2, Nome = "Fisioterapia" },
                new { Id = 3, Nome = "Hidroterapia" },
                new { Id = 4, Nome = "Drenagem Linfática" },
                new { Id = 5, Nome = "Reabilitação Funcional" }
            };

            var estados = new[]
            {
                "Pendente",
                "Confirmado",
                "Cancelado"
            };

            var inicioBase = DateTime.Today.AddHours(9);
            var totalAgendamentos = 45;

            for (int i = 0; i < totalAgendamentos; i++)
            {
                var terapeuta = terapeutas[i % terapeutas.Count];
                var servico = servicos[i % servicos.Length];
                var estado = estados[i % estados.Length];

                var inicio = inicioBase
                    .AddDays(i / 6)          // espalha por dias
                    .AddHours((i % 6) * 1.5); // horários diferentes

                context.Agendamentos.Add(new Agendamento
                {
                    DataHoraInicio = inicio,
                    DataHoraFim = inicio.AddHours(1),
                    Estado = estado,
                    TerapeutaId = terapeuta.TerapeutaId,
                    ServicoId = servico.Id
                });
            }

            context.SaveChanges();
        }
    }
}