using HealthWellbeing.Data;
using HealthWellbeing.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthWellbeing.Data
{
    public static class DbInitializer
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (context.UtenteBalnearios.Any())
                return; // Já tem dados

            var random = new Random();

            var moradas = new[]
            {
                "Rua da Igreja, Lisboa",
                "Rua das Flores, Porto",
                "Avenida da Liberdade, Lisboa",
                "Rua de São Bento, Lisboa",
                "Rua Dom João IV, Braga",
                "Rua da Alegria, Porto",
                "Rua da República, Faro",
                "Rua do Comércio, Setúbal",
                "Rua da Saudade, Coimbra",
                "Avenida 25 de Abril, Aveiro"
            };

            

            var homens = new[]
            {
                "João", "Pedro", "Miguel", "Tiago", "Rui",
                "André", "Bruno", "Daniel", "Fábio", "Ricardo"
            };

            var mulheres = new[]
            {
                "Ana", "Maria", "Sofia", "Inês", "Joana",
                "Carla", "Rita", "Marta", "Patrícia", "Catarina"
            };

            var apelidos = new[]
            {
                "Silva", "Santos", "Ferreira", "Pereira", "Costa",
                "Oliveira", "Rodrigues", "Martins", "Alves", "Lopes"
            };

            var utentes = new List<UtenteBalneario>();
            var seguros = context.SegurosSaude.ToList();


            for (int i = 0; i < 50; i++)
            {
                bool feminino = i % 2 == 0;

                var nomeProprio = feminino
                    ? mulheres[random.Next(mulheres.Length)]
                    : homens[random.Next(homens.Length)];

                var apelido = apelidos[random.Next(apelidos.Length)];

                var dataNascimento = DateTime.Today
                    .AddYears(-random.Next(18, 85))
                    .AddDays(random.Next(0, 365));

                var seguro = seguros[random.Next(seguros.Count)];

                var historicos = new List<HistoricoMedico>();

                foreach (var utente in utentes.Take(20)) // só alguns
                {
                    historicos.Add(new HistoricoMedico
                    {
                        UtenteBalneario = utente,
                        DataRegisto = utente.DataInscricao.AddDays(5),
                        Descricao = "Avaliação inicial e diagnóstico funcional."
                    });

                    historicos.Add(new HistoricoMedico
                    {
                        UtenteBalneario = utente,
                        DataRegisto = utente.DataInscricao.AddDays(15),
                        Descricao = "Sessão de hidroterapia. Boa resposta ao tratamento."
                    });
                }

                context.HistoricosMedicos.AddRange(historicos);
                context.SaveChanges();





                utentes.Add(new UtenteBalneario
                {
                    Nome = $"{nomeProprio} {apelido}",
                    DataNascimento = dataNascimento,
                    GeneroId = feminino ? 2 : 1, // assume: 1=Masculino, 2=Feminino
                    NIF = random.Next(100000000, 999999999).ToString(),
                    Contacto = $"{new[] { 91, 92, 93, 96 }[random.Next(4)]}{random.Next(1000000, 9999999)}",
                    Morada = moradas[random.Next(moradas.Length)],

                    HistoricoClinico = "Nada a registar.",
                    IndicacoesTerapeuticas = "Nada a registar.",
                    ContraIndicacoes = "Nada a registar.",
                    TerapeutaResponsavel = null,
                    SeguroSaudeId = seguro.SeguroSaudeId,



                    DataInscricao = DateTime.Today.AddDays(-random.Next(0, 365)),
                    Ativo = random.Next(0, 4) != 0 // ~75% ativos
                });
            }

            context.UtenteBalnearios.AddRange(utentes);
            context.SaveChanges();
        }
    }
}
