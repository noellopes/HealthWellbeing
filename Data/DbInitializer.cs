using HealthWellbeing.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthWellbeing.Data
{
    public static class DbInitializer
    {
        // =========================
        // HELPERS
        // =========================
        private static string GerarTelemovel(Random rnd)
        {
            string[] prefixos = { "91", "92", "93", "96" };
            return prefixos[rnd.Next(prefixos.Length)] + rnd.Next(1000000, 9999999);
        }

        private static string GerarNif(int baseNumber)
        {
            return (100000000 + baseNumber).ToString();
        }

        // =========================
        // SEED
        // =========================
        public static void Seed(ApplicationDbContext context)
        {
            if (context.ClientesBalneario.Any())
                return;

            var random = new Random();

            // =========================
            // GÉNEROS
            // =========================
            if (!context.Generos.Any())
            {
                context.Generos.AddRange(
                    new Genero { NomeGenero = "Masculino" },
                    new Genero { NomeGenero = "Feminino" }
                );
                context.SaveChanges();
            }

            // =========================
            // SEGUROS
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
            // NOMES PORTUGUESES
            // =========================
            var nomes = new[]
            {
                "João","Pedro","Miguel","Tiago","Rui","André","Carlos","Bruno","Paulo",
                "Ana","Maria","Sofia","Inês","Joana","Rita","Mariana","Carla","Helena"
            };

            var apelidos = new[]
            {
                "Silva","Santos","Ferreira","Pereira","Costa","Oliveira",
                "Rodrigues","Martins","Gomes","Lopes","Alves","Ribeiro"
            };

            var moradas = new[]
            {
                "Rua da Liberdade, Lisboa",
                "Rua das Flores, Porto",
                "Av. da República, Braga",
                "Rua Dom Afonso Henriques, Coimbra",
                "Rua do Mar, Faro"
            };

            // =========================
            // CLIENTES (35)
            // =========================
            var clientes = new List<ClienteBalneario>();

            for (int i = 1; i <= 35; i++)
            {
                clientes.Add(new ClienteBalneario
                {
                    Nome = $"{nomes[i % nomes.Length]} {apelidos[i % apelidos.Length]}",
                    Email = $"cliente{i}@balneario.pt",
                    Telemovel = GerarTelemovel(random),
                    DataRegisto = DateTime.Today.AddDays(-random.Next(30, 500)),
                    Ativo = random.Next(100) > 10
                });
            }

            context.ClientesBalneario.AddRange(clientes);
            context.SaveChanges();

            var clientesDb = context.ClientesBalneario.ToList();

            // =========================
            // TERAPEUTAS (47)
            // =========================
            string[] especialidades =
            {
                "Fisioterapia",
                "Hidroterapia",
                "Massoterapia",
                "Reabilitação",
                "Terapia Manual"
            };

            var terapeutas = new List<Terapeuta>();

            for (int i = 1; i <= 47; i++)
            {
                terapeutas.Add(new Terapeuta
                {
                    Nome = $"{nomes[i % nomes.Length]} {apelidos[(i + 3) % apelidos.Length]}",
                    Especialidade = especialidades[i % especialidades.Length],
                    Email = $"terapeuta{i}@clinica.pt",
                    Telefone = GerarTelemovel(random),
                    AnoEntrada = random.Next(2005, 2022),
                    Ativo = random.Next(100) > 15
                });
            }

            context.Terapeutas.AddRange(terapeutas);
            context.SaveChanges();

            var terapeutasDb = context.Terapeutas.ToList();

            // =========================
            // UTENTES (55)
            // =========================
            var utentes = new List<UtenteBalneario>();

            for (int i = 1; i <= 55; i++)
            {
                bool feminino = i % 2 == 0;

                string nome = feminino
                    ? nomes[(i + 5) % nomes.Length]
                    : nomes[(i + 2) % nomes.Length];

                utentes.Add(new UtenteBalneario
                {
                    Nome = $"{nome} {apelidos[(i + 7) % apelidos.Length]}",
                    DataNascimento = DateTime.Today.AddYears(-random.Next(18, 85)),
                    GeneroId = feminino ? 2 : 1,
                    NIF = GerarNif(i),
                    Contacto = GerarTelemovel(random),
                    Morada = moradas[random.Next(moradas.Length)],

                    IndicacoesTerapeuticas = "Acompanhamento terapêutico.",
                    ContraIndicacoes = "Nenhuma.",

                    //FK
                    TerapeutaId = terapeutasDb[random.Next(terapeutasDb.Count)].TerapeutaId,


                    SeguroSaudeId = seguros[random.Next(seguros.Count)].SeguroSaudeId,
                    ClienteBalnearioId = random.Next(100) < 70
                        ? clientesDb[random.Next(clientesDb.Count)].ClienteBalnearioId
                        : null,

                    DataInscricao = DateTime.Today.AddDays(-random.Next(10, 600)),
                    Ativo = random.Next(100) > 30
                });
            }

            context.UtenteBalnearios.AddRange(utentes);
            context.SaveChanges();

            // =========================
            // SATISFAÇÃO + PONTOS
            // =========================
            foreach (var cliente in clientesDb)
            {
                int entradas = random.Next(1, 5);

                for (int i = 0; i < entradas; i++)
                {
                    int rating = random.Next(1, 6);

                    context.SatisfacoesClientes.Add(new SatisfacaoCliente
                    {
                        ClienteBalnearioId = cliente.ClienteBalnearioId,
                        Avaliacao = rating,
                        Comentario = rating >= 4
                            ? "Excelente atendimento."
                            : "Serviço a melhorar.",
                        DataRegisto = DateTime.Today.AddDays(-random.Next(1, 300))
                    });

                    context.HistoricoPontos.Add(new HistoricoPontos
                    {
                        ClienteBalnearioId = cliente.ClienteBalnearioId,
                        Pontos = rating * 10,
                        Motivo = "Avaliação de satisfação",
                        Data = DateTime.Now
                    });
                }
            }

            context.SaveChanges();

            // =========================
            // VOUCHERS
            // =========================
            foreach (var cliente in clientesDb.Where(c => random.Next(100) < 40))
            {
                context.VouchersCliente.Add(new VoucherCliente
                {
                    ClienteBalnearioId = cliente.ClienteBalnearioId,
                    Titulo = "Voucher Fidelização",
                    Descricao = "Desconto em serviços",
                    PontosNecessarios = 50,
                    Valor = random.Next(5, 25),
                    DataCriacao = DateTime.Now,
                    DataValidade = DateTime.Now.AddMonths(6),
                    Usado = false
                });
            }

            context.SaveChanges();
        }
    }
}
