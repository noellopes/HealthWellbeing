using System;
using System.Linq;
using System.Collections.Generic;
using HealthWellbeing.Models;

namespace HealthWellbeing.Data
{
    public static class SeedData
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Conta quantos terapeutas já existem
            int existentes = context.Terapeutas.Count();
            int alvo = 100;

            // Se já tens 100 ou mais, não faz nada
            if (existentes >= alvo) return;

            int emFalta = alvo - existentes;

            var rnd = new Random();
            string[] especialidades = { "Massagem", "Fisioterapia", "Estética", "Acupuntura", "Terapia da Fala", "Reiki" };

            var novos = new List<TerapeutaModel>();
            for (int i = 1; i <= emFalta; i++)
            {
                // número sequencial a seguir aos existentes
                int n = existentes + i;

                novos.Add(new TerapeutaModel
                {
                    Nome = $"Terapeuta {n}",
                    Especialidade = especialidades[rnd.Next(especialidades.Length)],
                    Telefone = $"9{rnd.Next(10000000, 99999999)}",
                    Email = $"terapeuta{n}@balneario.pt",
                    AnosExperiencia = rnd.Next(1, 30),
                    Ativo = rnd.Next(0, 2) == 1
                });
            }

            context.Terapeutas.AddRange(novos);
            context.SaveChanges();
        }
    }
}
