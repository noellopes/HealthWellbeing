using HealthWellbeing.Data;
using HealthWellbeing.Models;

namespace HealthWellbeing.Data
{
    public class SeedDataGroup2
    {
        internal static void Populate(HealthWellbeingDbContext db)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));

            db.Database.EnsureCreated();

            PopulateCategorias(db);
            PopulateConsumiveis(db);
        }

        private static void PopulateCategorias(HealthWellbeingDbContext db)
        {
            if (db.CategoriaConsumivel.Any()) return;

            var categorias = new List<CategoriaConsumivel>
            {
                new() { Nome = "Luvas", Descricao = "Luvas descartáveis e estéreis para uso médico e cirúrgico." },
                new() { Nome = "Máscaras", Descricao = "Máscaras cirúrgicas, N95 e proteção respiratória." },
                new() { Nome = "Seringas e Agulhas", Descricao = "Seringas descartáveis e agulhas." },
                new() { Nome = "Compressas", Descricao = "Compressas estéreis e não estéreis." },
                new() { Nome = "Gazes", Descricao = "Gazes e pensos diversos." },
                new() { Nome = "Desinfetantes", Descricao = "Álcool, clorexidina, gel, etc." }
            };

            db.CategoriaConsumivel.AddRange(categorias);
            db.SaveChanges();
        }

        private static void PopulateConsumiveis(HealthWellbeingDbContext db)
        {
            if (db.Consumivel.Any()) return;

            var categorias = db.CategoriaConsumivel.ToList();

            Consumivel C(string nome, string desc, string categoria,
                         int max, int atual, int min)
            {
                return new Consumivel
                {
                    Nome = nome,
                    Descricao = desc,
                    CategoriaId = categorias.First(c => c.Nome == categoria).CategoriaId,
                    QuantidadeMaxima = max,
                    QuantidadeAtual = atual,
                    QuantidadeMinima = min
                };
            }

            var consumiveis = new List<Consumivel>
            {
                C("Luvas Cirúrgicas Pequenas", "Pacote de luvas pequenas", "Luvas", 100, 50, 10),
                C("Luvas Cirúrgicas Médias", "Pacote de luvas médias", "Luvas", 100, 40, 10),
                C("Luvas de Nitrilo", "Luvas de nitrilo descartáveis", "Luvas", 200, 100, 20),

                C("Máscara N95", "Máscara respiratória N95", "Máscaras", 200, 150, 20),
                C("Máscara Cirúrgica", "Máscara descartável para uso clínico", "Máscaras", 300, 250, 30),

                C("Seringa 5ml", "Seringa descartável 5ml", "Seringas e Agulhas", 300, 200, 30),
                C("Seringa 10ml", "Seringa descartável 10ml", "Seringas e Agulhas", 300, 150, 30),
                C("Agulhas 21G", "Agulhas esterilizadas 21G", "Seringas e Agulhas", 500, 300, 50),

                C("Compressa Estéril", "Pacote de compressas estéreis", "Compressas", 150, 100, 15),
                C("Compressa Não Estéril", "Pacote de compressas não estéreis", "Compressas", 150, 80, 15),

                C("Gaze Esterilizada", "Pacote de gazes esterilizadas", "Gazes", 200, 120, 20),
                C("Gaze Não Esterilizada", "Pacote de gazes não esterilizadas", "Gazes", 200, 100, 20),

                C("Álcool 70%", "Frasco de álcool 70%", "Desinfetantes", 50, 30, 5),
                C("Clorexidina", "Frasco de clorexidina", "Desinfetantes", 40, 25, 5)
            };

            db.Consumivel.AddRange(consumiveis);
            db.SaveChanges();
        }
    }
}
