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
            // PROTEÇÃO
            // =========================
            if (context.UtenteBalnearios.Any())
                return;

            var random = new Random();

            // =========================
            // DADOS BASE
            // =========================
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

            var homens = new[] { "João", "Pedro", "Miguel", "Tiago", "Rui", "André", "Bruno", "Daniel" };
            var mulheres = new[] { "Ana", "Maria", "Sofia", "Inês", "Joana", "Rita", "Marta", "Carla" };
            var apelidos = new[] { "Silva", "Santos", "Ferreira", "Pereira", "Costa", "Oliveira" };

            var historicosClinicos = new[]
            {
                "Sem antecedentes clínicos relevantes.",
                "Hipertensão arterial controlada.",
                "Lombalgia crónica.",
                "Recuperação pós-cirúrgica.",
                "Diabetes tipo II controlada."
            };

            var indicacoes = new[]
            {
                "Hidroterapia para reabilitação muscular.",
                "Relaxamento muscular.",
                "Fisioterapia aquática.",
                "Mobilidade articular.",
                "Reeducação postural."
            };

            var contraIndicacoes = new[]
            {
                "Nenhuma.",
                "Alergia ao cloro.",
                "Hipertensão não controlada.",
                "Problemas respiratórios.",
                "Feridas abertas."
            };

            // =========================
            // CLIENTES DE BALNEÁRIO
            // =========================
            var clientes = new List<ClienteBalneario>
            {
                new ClienteBalneario
                {
                    Nome = "Antonio Marques",
                    Email = "antonio@balneario.pt",
                    Telemovel = "935678123",
                    DataRegisto = DateTime.Today.AddDays(-30),
                    Ativo = true
                },
                new ClienteBalneario
                {
                    Nome = "Zulmira Silva",
                    Email = "zulmira@balneario.pt",
                    Telemovel = "914576559",
                    DataRegisto = DateTime.Today.AddDays(-20),
                    Ativo = true
                },
                new ClienteBalneario
                {
                    Nome = "Juvenaldo Ramos",
                    Email = "juvenaldo@balneario.pt",
                    Telemovel = "968782555",
                    DataRegisto = DateTime.Today.AddDays(-10),
                    Ativo = true
                }
            };

            context.ClientesBalneario.AddRange(clientes);
            context.SaveChanges();

            var clientesDb = context.ClientesBalneario.ToList();
            var seguros = context.SegurosSaude.ToList();

            // =========================
            // UTENTES
            // =========================
            var utentes = new List<UtenteBalneario>();

            for (int i = 0; i < 50; i++)
            {
                bool feminino = i % 2 == 0;

                var nome = feminino
                    ? mulheres[random.Next(mulheres.Length)]
                    : homens[random.Next(homens.Length)];

                var apelido = apelidos[random.Next(apelidos.Length)];
                var seguro = seguros[random.Next(seguros.Count)];

                var clienteAssociado = random.Next(0, 2) == 0
                    ? clientesDb[random.Next(clientesDb.Count)]
                    : null;

                utentes.Add(new UtenteBalneario
                {
                    Nome = $"{nome} {apelido}",
                    DataNascimento = DateTime.Today.AddYears(-random.Next(18, 80)),
                    GeneroId = feminino ? 2 : 1,
                    NIF = random.Next(100000000, 999999999).ToString(),
                    Contacto = $"9{random.Next(10000000, 99999999)}",
                    Morada = moradas[random.Next(moradas.Length)],

                    HistoricoClinico = historicosClinicos[random.Next(historicosClinicos.Length)],
                    IndicacoesTerapeuticas = indicacoes[random.Next(indicacoes.Length)],
                    ContraIndicacoes = contraIndicacoes[random.Next(contraIndicacoes.Length)],

                    SeguroSaudeId = seguro.SeguroSaudeId,
                    ClienteBalnearioId = clienteAssociado?.ClienteBalnearioId,

                    DataInscricao = DateTime.Today.AddDays(-random.Next(0, 365)),
                    Ativo = true
                });
            }

            context.UtenteBalnearios.AddRange(utentes);
            context.SaveChanges();

            // =========================
            // HISTÓRICO MÉDICO
            // =========================
            foreach (var utente in context.UtenteBalnearios.Take(20))
            {
                context.HistoricosMedicos.AddRange(
                    new HistoricoMedico
                    {
                        UtenteBalnearioId = utente.UtenteBalnearioId,
                        DataRegisto = utente.DataInscricao.AddDays(5),
                        Descricao = "Avaliação inicial e plano terapêutico definido."
                    },
                    new HistoricoMedico
                    {
                        UtenteBalnearioId = utente.UtenteBalnearioId,
                        DataRegisto = utente.DataInscricao.AddDays(15),
                        Descricao = "Sessão de hidroterapia com evolução positiva."
                    }
                );
            }

            context.SaveChanges();

            // =========================
            // AVALIAÇÕES + PONTOS
            // =========================
            foreach (var cliente in clientesDb)
            {
                int avaliacao = random.Next(1, 6);

                context.SatisfacoesClientes.Add(new SatisfacaoCliente
                {
                    ClienteBalnearioId = cliente.ClienteBalnearioId,
                    Avaliacao = avaliacao,
                    Comentario = avaliacao >= 4
                        ? "Excelente atendimento e acompanhamento."
                        : "Serviço aceitável, pode melhorar.",
                    DataRegisto = DateTime.Today.AddDays(-random.Next(1, 10))
                });

                context.HistoricoPontos.Add(new HistoricoPontos
                {
                    ClienteBalnearioId = cliente.ClienteBalnearioId,
                    Pontos = avaliacao * 5,
                    Motivo = "Avaliação de satisfação",
                    Data = DateTime.Now
                });
            }

            context.SaveChanges();

            // =========================
            // VOUCHERS
            // =========================
            context.VouchersCliente.Add(new VoucherCliente
            {
                ClienteBalnearioId = clientesDb.First().ClienteBalnearioId,
                Descricao = "Voucher de desconto",
                Valor = 5,
                DataCriacao = DateTime.Now,
                Usado = false
            });

            context.SaveChanges();
        }
    }
}
