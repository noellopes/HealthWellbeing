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
            int existentes = context.Terapeutas.Count();
            int alvo = 100;

            if (existentes >= alvo) return;

            int emFalta = alvo - existentes;

            var rnd = new Random();

            // LISTA REAL DE ESPECIALIDADES
            string[] especialidades =
            {
                "Massagem",
                "Fisioterapia",
                "Estética",
                "Acupuntura",
                "Terapia da Fala",
                "Reiki"
            };

            // LISTA DE NOMES PRÓPRIOS
            string[] nomes = new[]
            {
                "Ana", "Beatriz", "Carla", "Diana", "Eva", "Filipa", "Gabriela", "Helena", "Inês", "Joana",
                "Leonor", "Marta", "Nádia", "Patrícia", "Raquel", "Sofia", "Teresa", "Vanessa",

                "André", "Bruno", "Carlos", "Daniel", "Eduardo", "Fábio", "Gonçalo", "Hugo", "Igor", "João",
                "Luis", "Marco", "Nuno", "Paulo", "Rafael", "Sérgio", "Tiago", "Vítor"
            };

            // LISTA DE APELIDOS
            string[] apelidos = new[]
            {
                "Silva", "Santos", "Ferreira", "Pereira", "Costa", "Oliveira", "Martins",
                "Rodrigues", "Almeida", "Nunes", "Gomes", "Carvalho", "Lopes",
                "Ribeiro", "Sousa", "Mendes"
            };

            var novos = new List<Terapeuta>();

            for (int i = 0; i < emFalta; i++)
            {
                string nomeCompleto =
                    $"{nomes[rnd.Next(nomes.Length)]} {apelidos[rnd.Next(apelidos.Length)]} {apelidos[rnd.Next(apelidos.Length)]}";

                int anoAtual = DateTime.Now.Year;
                int anosExp = rnd.Next(1, 31); // 1 a 30 anos
                int anoEntrada = anoAtual - anosExp;

                novos.Add(new Terapeuta
                {
                    Nome = nomeCompleto,
                    Especialidade = especialidades[rnd.Next(especialidades.Length)],
                    Telefone = $"9{rnd.Next(10000000, 99999999)}",
                    Email = nomeCompleto.ToLower().Replace(" ", ".") + "@balneario.pt",
                    AnoEntrada = anoEntrada,
                    Ativo = rnd.Next(0, 2) == 1
                });
            }

            context.Terapeutas.AddRange(novos);
            context.SaveChanges();
        }
    }
}
