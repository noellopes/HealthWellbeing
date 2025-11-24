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
                new() { Nome = "Bandagens", Descricao = "Faixas e ligaduras elásticas ou de imobilização." },
                new() { Nome = "Adesivos Médicos", Descricao = "Fitas adesivas, micropores e esparadrapos." },
                new() { Nome = "Soluções de Soro", Descricao = "Soro fisiológico, glicosado e soluções intravenosas." },
                new() { Nome = "Material de Aspiração", Descricao = "Tubos e frascos para aspiração de secreções." },
                new() { Nome = "Material de Oxigenoterapia", Descricao = "Cânulas, máscaras de oxigénio e tubos de ligação." },
                new() { Nome = "Equipamento de Infusão", Descricao = "Equipos de soro, extensões e conectores." },
                new() { Nome = "Material de Curativo", Descricao = "Kits de curativo e material para trocas de pensos." },
                new() { Nome = "Material de Punção", Descricao = "Agulhas, scalps e dispositivos de punção venosa." },
                new() { Nome = "Lâminas e Bisturis", Descricao = "Lâminas cirúrgicas e bisturis descartáveis." },
                new() { Nome = "Campos Cirúrgicos", Descricao = "Campos estéreis para cobertura de áreas cirúrgicas." },
                new() { Nome = "Toucas e Protetores", Descricao = "Toucas, propés e aventais descartáveis." },
                new() { Nome = "Material de Esterilização", Descricao = "Indicadores químicos, embalagens e fitas para esterilização." },
                new() { Nome = "Frascos e Recipientes", Descricao = "Frascos coletores e contentores para amostras biológicas." },
                new() { Nome = "Material de Coleta", Descricao = "Tubos de ensaio, agulhas de coleta e lancetas." },
                new() { Nome = "Equipamentos de Proteção Individual", Descricao = "EPI hospitalar como óculos, viseiras e aventais." },
                new() { Nome = "Material de Endoscopia", Descricao = "Acessórios descartáveis usados em procedimentos endoscópicos." },
                new() { Nome = "Material de Radiologia", Descricao = "Aventais de chumbo, protetores e filmes radiográficos." },
                new() { Nome = "Material de Laboratório", Descricao = "Pipetas, ponteiras, tubos e outros consumíveis laboratoriais." },
                new() { Nome = "Suturas", Descricao = "Fios de sutura absorvíveis e não absorvíveis." },
                new() { Nome = "Material de Hemoterapia", Descricao = "Bolsas de sangue, filtros e conjuntos de transfusão." },
                new() { Nome = "Material Odontológico", Descricao = "Consumíveis para clínicas odontológicas." },
                new() { Nome = "Material de Oftalmologia", Descricao = "Lentes, campos e instrumentos descartáveis para cirurgias oculares." },
                new() { Nome = "Material de Ortopedia", Descricao = "Gessos, talas e acessórios ortopédicos descartáveis." },
                new() { Nome = "Material de Ginecologia", Descricao = "Espéculos, sondas e kits ginecológicos descartáveis." },
                new() { Nome = "Material Pediátrico", Descricao = "Consumíveis hospitalares adaptados ao público infantil." },
                new() { Nome = "Material de Nutrição Enteral", Descricao = "Sondas e extensões para nutrição enteral." },
                new() { Nome = "Material de Diálise", Descricao = "Filtros, linhas e acessórios descartáveis para hemodiálise." },
                new() { Nome = "Material de Urologia", Descricao = "Sondas, bolsas coletoras e acessórios urológicos." },
                new() { Nome = "Material de Anestesia", Descricao = "Máscaras, circuitos e filtros para anestesia." },
                new() { Nome = "Material de Emergência", Descricao = "Kits de emergência, cânulas e acessórios para suporte básico de vida." },
                new() { Nome = "Material de Diagnóstico", Descricao = "Testes rápidos, tiras reagentes e materiais de diagnóstico in vitro." },
                new() { Nome = "Material de Higiene Hospitalar", Descricao = "Toalhetes, papel, sabão e produtos de limpeza hospitalar." },
                new() { Nome = "Material Diverso", Descricao = "Outros consumíveis de uso geral em ambiente hospitalar." },
                new() { Nome = "Cateteres", Descricao = "Cateteres intravenosos, urinários e outros tipos médicos." },
                new() { Nome = "Máscaras N95", Descricao = "Máscaras de alta proteção respiratória para procedimentos específicos." },
                new() { Nome = "Gazes Esterilizadas", Descricao = "Gazes hospitalares esterilizadas para curativos." },
                new() { Nome = "Soro Fisiológico", Descricao = "Soro fisiológico para hidratação e lavagem de feridas." }
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
