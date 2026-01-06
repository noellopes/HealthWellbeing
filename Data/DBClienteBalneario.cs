using HealthWellbeing.Models;
using HealthWellbeing.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Data
{
    public static class DBClienteBalneario
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (context.ClienteBalneario.AsNoTracking().Any())
                return;

            var nomesProprios = new List<string>
    {
        "Maria", "João", "Ana", "Pedro", "Rita",
        "Carlos", "Inês", "Miguel", "Sofia", "Tiago"
    };

            var apelidos = new List<string>
    {
        "Silva", "Santos", "Ferreira", "Pereira",
        "Costa", "Oliveira", "Rodrigues", "Martins"
    };

            var clientes = new List<ClienteBalnearioModel>();
            var random = new Random();

            for (int i = 1; i <= 20; i++)
            {
                clientes.Add(new ClienteBalnearioModel
                {
                    NomeCompleto = $"{nomesProprios[random.Next(nomesProprios.Count)]} {apelidos[random.Next(apelidos.Count)]}",
                    Email = $"cliente{i}@email.com",
                    Telemovel = $"91{random.Next(1000000, 9999999)}",
                    Morada = $"Rua Exemplo nº {i}",
                    TipoCliente = (TipoCliente)random.Next(0, 2)
                });
            }

            context.ClienteBalneario.AddRange(clientes);
            context.SaveChanges();
        }

    }
}
