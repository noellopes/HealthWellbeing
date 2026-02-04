using HealthWellbeing.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

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

        private static string GerarNif(int numero)
        {
            return (100000000 + numero).ToString(); // simples e único
        }

        private static string Normalizar(string texto)
        {
            var normalized = texto.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            return sb.ToString().ToLower().Replace(" ", ".");
        }

        // =========================
        // SEED
        // =========================
        public static void Seed(ApplicationDbContext context)
        {
            if (context.ClientesBalneario.Any())
                return;

            var rnd = new Random();

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

            int generoMasc = context.Generos.First(g => g.NomeGenero == "Masculino").GeneroId;
            int generoFem = context.Generos.First(g => g.NomeGenero == "Feminino").GeneroId;

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
            // NÍVEIS DE CLIENTE
            // =========================
            if (!context.NiveisCliente.Any())
            {
                context.NiveisCliente.AddRange(
                    new NivelCliente { Nome = "Bronze", PontosMinimos = 0, CorBadge = "bg-secondary" },
                    new NivelCliente { Nome = "Prata", PontosMinimos = 50, CorBadge = "bg-info" },
                    new NivelCliente { Nome = "Ouro", PontosMinimos = 150, CorBadge = "bg-warning" }
                );
                context.SaveChanges();
            }

            var niveis = context.NiveisCliente.OrderBy(n => n.PontosMinimos).ToList();

            // =========================
            // NOMES
            // =========================
            var nomesMasc = new[]
            {
                "João","Pedro","Miguel","Tiago","Rui","André","Carlos","Bruno",
                "Paulo","Nuno","Ricardo","Daniel","Filipe","Hugo","Marco"
            };

            var nomesFem = new[]
            {
                "Ana","Maria","Sofia","Inês","Joana","Rita","Mariana","Carla",
                "Helena","Patrícia","Andreia","Catarina","Beatriz","Cláudia"
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
                string nome = $"{nomesMasc[i % nomesMasc.Length]} {apelidos[i % apelidos.Length]}";

                clientes.Add(new ClienteBalneario
                {
                    Nome = nome,
                    Email = $"cliente{i}@balneario.pt",
                    Telemovel = GerarTelemovel(rnd),
                    DataRegisto = DateTime.Today.AddDays(-rnd.Next(30, 600)),
                    Ativo = rnd.Next(100) > 15
                });
            }

            context.ClientesBalneario.AddRange(clientes);
            context.SaveChanges();

            var clientesDb = context.ClientesBalneario.ToList();

            // =========================
            // TERAPEUTAS (47)
            // =========================
            var especialidades = new[]
            {
                "Acupuntura", 
                "Estética",
                "Fisioterapia",
                "Massagem",
                "Reiki", 
                "Terapia da Fala"

            };

            var terapeutas = new List<Terapeuta>();

            for (int i = 1; i <= 47; i++)
            {
                string nome = nomesMasc[i % nomesMasc.Length];
                string apelido = apelidos[(i + 3) % apelidos.Length];

                terapeutas.Add(new Terapeuta
                {
                    Nome = $"{nome} {apelido}",
                    Especialidade = especialidades[i % especialidades.Length],
                    Email = $"{Normalizar(nome)}.{Normalizar(apelido)}@balneario.pt",
                    Telefone = GerarTelemovel(rnd),
                    AnoEntrada = rnd.Next(2005, 2022),
                    Ativo = rnd.Next(100) > 10
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
                bool feminino = rnd.Next(2) == 0;

                string nome = feminino
                    ? nomesFem[i % nomesFem.Length]
                    : nomesMasc[i % nomesMasc.Length];

                utentes.Add(new UtenteBalneario
                {
                    Nome = $"{nome} {apelidos[(i + 5) % apelidos.Length]}",
                    DataNascimento = DateTime.Today.AddYears(-rnd.Next(18, 85)),
                    GeneroId = feminino ? generoFem : generoMasc,
                    NIF = GerarNif(i),
                    Contacto = GerarTelemovel(rnd),
                    Morada = moradas[rnd.Next(moradas.Length)],
                    IndicacoesTerapeuticas = "Acompanhamento terapêutico.",
                    ContraIndicacoes = "Nenhuma.",
                    SeguroSaudeId = seguros[rnd.Next(seguros.Count)].SeguroSaudeId,
                    ClienteBalnearioId = rnd.Next(100) < 70
                        ? clientesDb[rnd.Next(clientesDb.Count)].ClienteBalnearioId
                        : null,
                    TerapeutaId = terapeutasDb[rnd.Next(terapeutasDb.Count)].TerapeutaId,
                    DataInscricao = DateTime.Today.AddDays(-rnd.Next(10, 700)),
                    Ativo = rnd.Next(100) > 30
                });
            }

            context.UtenteBalnearios.AddRange(utentes);
            context.SaveChanges();

            // =========================
            // HISTÓRICO CLÍNICO (COM HORAS)
            // =========================
            var utentesDb = context.UtenteBalnearios.ToList();

            foreach (var utente in utentesDb.Where(u => rnd.Next(100) < 50))
            {
                int entradas = rnd.Next(1, 4);

                for (int i = 0; i < entradas; i++)
                {
                    var baseDate = utente.DataInscricao.AddDays(rnd.Next(1, 180));

                    var dataComHora = new DateTime(
                        baseDate.Year,
                        baseDate.Month,
                        baseDate.Day,
                        rnd.Next(8, 20),
                        rnd.Next(0, 60),
                        0
                    );

                    context.HistoricosMedicos.Add(new HistoricoMedico
                    {
                        UtenteBalnearioId = utente.UtenteBalnearioId,
                        Titulo = "Sessão de acompanhamento",
                        Descricao = "Sessão clínica realizada com sucesso.",
                        DataRegisto = dataComHora
                    });
                }
            }

            context.SaveChanges();

            // =========================
            // SATISFAÇÃO + PONTOS + NÍVEL
            // =========================
            foreach (var cliente in clientesDb)
            {
                int totalPontos = 0;
                int entradas = rnd.Next(1, 5);

                for (int i = 0; i < entradas; i++)
                {
                    int rating = rnd.Next(1, 6);
                    int pontos = rating * 10;
                    totalPontos += pontos;

                    context.SatisfacoesClientes.Add(new SatisfacaoCliente
                    {
                        ClienteBalnearioId = cliente.ClienteBalnearioId,
                        Avaliacao = rating,
                        Comentario = rating >= 4 ? "Excelente atendimento." : "Serviço a melhorar.",
                        DataRegisto = DateTime.Today.AddDays(-rnd.Next(1, 300))
                    });

                    context.HistoricoPontos.Add(new HistoricoPontos
                    {
                        ClienteBalnearioId = cliente.ClienteBalnearioId,
                        Pontos = pontos,
                        Motivo = "Avaliação de satisfação",
                        Data = DateTime.Now
                    });
                }

                cliente.Pontos = totalPontos;
                cliente.NivelClienteId = niveis.Last(n => totalPontos >= n.PontosMinimos).NivelClienteId;
            }

            context.SaveChanges();

            // =========================
            // VOUCHERS
            // =========================
            foreach (var cliente in clientesDb.Where(c => rnd.Next(100) < 65))
            {
                context.VouchersCliente.Add(new VoucherCliente
                {
                    ClienteBalnearioId = cliente.ClienteBalnearioId,
                    Titulo = "Voucher Fidelização",
                    Descricao = "Desconto em serviços do balneário",
                    PontosNecessarios = 50,
                    Valor = rnd.Next(5, 30),
                    DataCriacao = DateTime.Now,
                    DataValidade = DateTime.Now.AddMonths(6),
                    Usado = false
                });
            }

            context.SaveChanges();
        }
    }
}
