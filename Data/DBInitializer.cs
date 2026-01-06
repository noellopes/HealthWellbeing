using HealthWellbeing.Models;

namespace HealthWellbeing.Data
{
    public static class DbInitializer
    {
        public static void Seed(ApplicationDbContext context)
        {
            // GARANTIR Dados Médicos base
            var dadosMedicos = context.DadosMedicos.FirstOrDefault();
            if (dadosMedicos == null)
            {
                dadosMedicos = new DadosMedicos
                {
                    HistoricoClinico = "",
                    IndicacoesTerapeuticas = "",
                    ContraIndicacoes = "",
                    MedicoResponsavel = ""
                };

                context.DadosMedicos.Add(dadosMedicos);
                context.SaveChanges();
            }

            // GARANTIR Seguro de Saúde base
            var seguro = context.SegurosSaude.FirstOrDefault();
            if (seguro == null)
            {
                seguro = new SeguroSaude
                {
                    NomeSeguradora = "N/A",
                    NumeroApolice = ""
                };

                context.SegurosSaude.Add(seguro);
                context.SaveChanges();
            }

            // Se já houver muitos utentes, não faz seed outra vez
            if (context.Utentes.Count() >= 10)
                return;

            var random = new Random();

            var nomes = new[]
            {
                "Ana","João","Maria","Pedro","Sofia","Rui","Carla","Nuno","Beatriz","Miguel",
                "Teresa","Bruno","Inês","Paulo","Sandra","Ricardo","Helena","Tiago","Andreia",
                "Daniel","Catarina","Luís","Patrícia","Fábio","Raquel","Vasco","Marta",
                "Gonçalo","Cláudia","Hugo","Mariana","Rafael","Joana","Diogo","Liliana"
            };

            var apelidos = new[]
            {
                "Silva","Santos","Ferreira","Pereira","Costa","Rodrigues","Martins","Lopes",
                "Nunes","Almeida","Ribeiro","Pinto","Carvalho","Teixeira","Moreira"
            };

            var ruas = new[]
            {
                "Rua Zeca Afonso",
                "Avenida da Liberdade",
                "Rua 25 de Abril",
                "Rua da Constituição",
                "Rua das Flores",
                "Rua do Parque",
                "Rua do Comércio",
                "Rua da Escola",
                "Rua da Igreja"
            };

            var prefixos = new[] { "91", "92", "93", "96" };

            var utentes = new List<UtenteBalneario>();

            for (int i = 0; i < 48; i++)
            {
                utentes.Add(new UtenteBalneario
                {
                    NomeCompleto = $"{nomes[random.Next(nomes.Length)]} {apelidos[random.Next(apelidos.Length)]}",
                    DataNascimento = DateTime.Today.AddYears(-random.Next(18, 85)),
                    Sexo = (Sexo)random.Next(1, 4),
                    NIF = random.Next(200000000, 299999999).ToString(),
                    Contacto = prefixos[random.Next(prefixos.Length)] + random.Next(1000000, 9999999),
                    Morada = $"{ruas[random.Next(ruas.Length)]}, nº {random.Next(1, 200)}",
                    DataInscricao = DateTime.Today.AddDays(-random.Next(1, 1500)),
                    Ativo = random.Next(0, 2) == 1,
                    DadosMedicos = dadosMedicos,
                    SeguroSaude = seguro

                });
            }

            context.Utentes.AddRange(utentes);
            context.SaveChanges();
        }
    }
}
