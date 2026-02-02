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
            // =========================
            // PROTEÇÃO (DEV)
            // =========================
            if (context.ClientesBalneario.Any())
                return;

            var random = new Random();

            // =========================
            // SEGUROS DE SAÚDE
            // =========================
            if (!context.SegurosSaude.Any())
            {
                context.SegurosSaude.AddRange(
                    new SeguroSaude { Nome = "SNS" },
                    new SeguroSaude { Nome = "Médis" },
                    new SeguroSaude { Nome = "Multicare" },
                    new SeguroSaude { Nome = "ADSE" }
                );
                context.SaveChanges();
            }

            var seguros = context.SegurosSaude.ToList();

            // =========================
            // DADOS AUXILIARES
            // =========================
            var moradas = new[]
            {
                "Rua da Igreja, Lisboa",
                "Rua das Flores, Porto",
                "Avenida da Liberdade, Lisboa",
                "Rua Dom João IV, Braga",
                "Rua da República, Faro",
                "Rua da Saudade, Coimbra"
            };

            var homens = new[] { "João", "Pedro", "Miguel", "Tiago", "Rui", "André", "Carlos", "Bruno" };
            var mulheres = new[] { "Ana", "Maria", "Sofia", "Inês", "Joana", "Rita", "Mariana", "Carla" };
            var apelidos = new[] { "Silva", "Santos", "Ferreira", "Pereira", "Costa", "Oliveira" };

            // =========================
            // CLIENTES DO BALNEÁRIO (25)
            // =========================

            var nomesClientes = new[]
            {
                "António Marques", "Zulmira Silva", "João Pereira", "Sofia Costa",
                "Rui Ferreira", "Ana Oliveira", "Carlos Santos", "Inês Rodrigues",
                "Miguel Lopes", "Mariana Alves", "Pedro Gomes", "Rita Martins",
                "Bruno Rocha", "Carla Fonseca", "Tiago Neves", "Andreia Pires",
                "Paulo Teixeira", "Helena Coelho", "Nuno Batista", "Patrícia Afonso"
            };

            var clientes = new List<ClienteBalneario>();

            for (int i = 1; i <= 25; i++)
            {
                clientes.Add(new ClienteBalneario
                {
                    Nome = nomesClientes[random.Next(nomesClientes.Length)],
                    Email = $"cliente{i}@balneario.pt",
                    Telemovel = $"9{random.Next(10000000, 99999999)}",
                    DataRegisto = DateTime.Today.AddDays(-random.Next(10, 400)),
                    Ativo = random.Next(100) >= 10
                });
            }

            context.ClientesBalneario.AddRange(clientes);
            context.SaveChanges();

            var clientesDb = context.ClientesBalneario.ToList();

            // =========================
            // UTENTES (55)
            // =========================
            var utentes = new List<UtenteBalneario>();

            for (int i = 0; i < 55; i++)
            {
                bool feminino = random.Next(0, 2) == 0;

                var nome = feminino
                    ? mulheres[random.Next(mulheres.Length)]
                    : homens[random.Next(homens.Length)];

                var apelido = apelidos[random.Next(apelidos.Length)];
                var seguro = seguros[random.Next(seguros.Count)];

                
                var clienteAssociado = random.Next(100) < 65
                    ? clientesDb[random.Next(clientesDb.Count)]
                    : null;

                utentes.Add(new UtenteBalneario
                {
                    Nome = $"{nome} {apelido}",
                    DataNascimento = DateTime.Today.AddYears(-random.Next(18, 85)),
                    GeneroId = feminino ? 2 : 1,
                    NIF = random.Next(100000000, 999999999).ToString(),
                    Contacto = $"9{random.Next(10000000, 99999999)}",
                    Morada = moradas[random.Next(moradas.Length)],

                    HistoricoClinico = null, // opcional
                    IndicacoesTerapeuticas = "Acompanhamento terapêutico.",
                    ContraIndicacoes = "Nenhuma.",

                    SeguroSaudeId = seguro.SeguroSaudeId,
                    ClienteBalnearioId = clienteAssociado?.ClienteBalnearioId,

                    DataInscricao = DateTime.Today.AddDays(-random.Next(0, 500)),
                    Ativo = random.Next(100) >= 35 

                });
            }

            context.UtenteBalnearios.AddRange(utentes);
            context.SaveChanges();

            var utentesDb = context.UtenteBalnearios.ToList();

            // =========================
            // HISTÓRICO MÉDICO (só alguns)
            // =========================
            foreach (var utente in utentesDb.Where(u => random.Next(100) < 60))
            {
                int entradas = random.Next(1, 4);

                for (int i = 0; i < entradas; i++)
                {
                    context.HistoricosMedicos.Add(new HistoricoMedico
                    {
                        UtenteBalnearioId = utente.UtenteBalnearioId,
                        Titulo = "Sessão Clínica",
                        Descricao = "Sessão de acompanhamento terapêutico.",
                        DataRegisto = utente.DataInscricao.AddDays(random.Next(5, 150))
                    });
                }
            }

            context.SaveChanges();

            // =========================
            // SATISFAÇÃO + PONTOS
            // =========================
            foreach (var cliente in clientesDb)
            {
                int avaliacoes = random.Next(1, 5);

                for (int i = 0; i < avaliacoes; i++)
                {
                    int rating = random.Next(2, 6);

                    context.SatisfacoesClientes.Add(new SatisfacaoCliente
                    {
                        ClienteBalnearioId = cliente.ClienteBalnearioId,
                        Avaliacao = rating,
                        Comentario = rating >= 4
                            ? "Excelente atendimento."
                            : "Serviço aceitável.",
                        DataRegisto = DateTime.Today.AddDays(-random.Next(1, 200))
                    });

                    context.HistoricoPontos.Add(new HistoricoPontos
                    {
                        ClienteBalnearioId = cliente.ClienteBalnearioId,
                        Pontos = rating * 5,
                        Motivo = "Avaliação de satisfação",
                        Data = DateTime.Now
                    });
                }
            }

            context.SaveChanges();

            // =========================
            // VOUCHERS (alguns clientes)
            // =========================
            foreach (var cliente in clientesDb.Where(c => random.Next(100) < 40))
            {
                context.VouchersCliente.Add(new VoucherCliente
                {
                    ClienteBalnearioId = cliente.ClienteBalnearioId,
                    Descricao = "Voucher promocional",
                    Valor = random.Next(5, 20),
                    DataCriacao = DateTime.Now,
                    Usado = false
                });
            }

            context.SaveChanges();
        }
    }
}
