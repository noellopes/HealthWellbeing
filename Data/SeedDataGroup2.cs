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

            //PopulateLocalizacoes(db);
            PopulateCategorias(db);
            PopulateConsumiveis(db);
            //PopulateZonasArmazenamento(db);
        }
        /*
        private static void PopulateLocalizacoes(HealthWellbeingDbContext db)
        {
            if (db.LocalizacaoZonaArmazenamento.Any()) return;

            var localizacoes = new List<LocalizacaoZonaArmazenamento>
                {
                    new() { Nome = "Bloco A - Piso 0" },
                    new() { Nome = "Bloco A - Piso 1" },
                    new() { Nome = "Bloco B - Subsolo" },
                    new() { Nome = "Armazém Central" },
                    new() { Nome = "Armazém Secundário" },
                    new() { Nome = "Sala de Suprimentos" },
                    new() { Nome = "Edifício Técnico - Piso 2" },
                    new() { Nome = "Bloco C - Piso 0" },
                    new() { Nome = "Bloco C - Piso 1" },
                    new() { Nome = "Bloco C - Piso 2" },
                    new() { Nome = "Armazém de Material Clínico" },
                    new() { Nome = "Armazém de Equipamentos Pesados" },
                    new() { Nome = "Zona Industrial - Setor 1" },
                    new() { Nome = "Zona Industrial - Setor 2" },
                    new() { Nome = "Edifício Logístico - Piso 0" },
                    new() { Nome = "Edifício Logístico - Piso 1" },
                    new() { Nome = "Depósito Exterior Coberto" }
                };

            db.LocalizacaoZonaArmazenamento.AddRange(localizacoes);
            db.SaveChanges();
        }
        */
/*
        private static void PopulateZonasArmazenamento(HealthWellbeingDbContext db)
        {
            if (db.ZonaArmazenamento.Any()) return;

            var localizacoes = db.LocalizacaoZonaArmazenamento.ToList();
            int Loc(string nome) => localizacoes.First(l => l.Nome == nome).Id;

            var zonas = new List<ZonaArmazenamento>
                {
                    new() { Nome = "Armazém Central - Zona 1", Descricao = "Zona principal para armazenamento geral.", LocalizacaoZonaArmazenamentoId = Loc("Armazém Central"), CapacidadeMaxima = 950, Ativa = true },
                    new() { Nome = "Armazém Central - Zona Refrigerada", Descricao = "Área refrigerada para materiais sensíveis.", LocalizacaoZonaArmazenamentoId = Loc("Armazém Central"), CapacidadeMaxima = 600, Ativa = true },
                    new() { Nome = "Depósito Técnico A", Descricao = "Materiais técnicos e manutenção.", LocalizacaoZonaArmazenamentoId = Loc("Edifício Técnico - Piso 2"), CapacidadeMaxima = 400, Ativa = true },
                    new() { Nome = "Sala de Suprimentos - Armazenamento Secundário", Descricao = "Armazenamento adicional para uso rápido.", LocalizacaoZonaArmazenamentoId = Loc("Sala de Suprimentos"), CapacidadeMaxima = 300, Ativa = true },
                    new() { Nome = "Bloco A - Stock de Emergência", Descricao = "Material de emergência e primeiros socorros.", LocalizacaoZonaArmazenamentoId = Loc("Bloco A - Piso 1"), CapacidadeMaxima = 200, Ativa = true },
                    new() { Nome = "Bloco B - Subsolo - Armazenamento Geral", Descricao = "Armazenamento geral em zona subterrânea.", LocalizacaoZonaArmazenamentoId = Loc("Bloco B - Subsolo"), CapacidadeMaxima = 500, Ativa = true },
                    new() { Nome = "Bloco C - Armazenamento Geral 1", Descricao = "Zona ampla para armazenamento diversificado.", LocalizacaoZonaArmazenamentoId = Loc("Bloco C - Piso 0"), CapacidadeMaxima = 750, Ativa = true },
                    new() { Nome = "Bloco C - Armazenamento Geral 2", Descricao = "Armazenamento de materiais não perecíveis.", LocalizacaoZonaArmazenamentoId = Loc("Bloco C - Piso 1"), CapacidadeMaxima = 620, Ativa = true },
                    new() { Nome = "Bloco C - Armazém Técnico", Descricao = "Equipamentos técnicos e ferramentas.", LocalizacaoZonaArmazenamentoId = Loc("Bloco C - Piso 2"), CapacidadeMaxima = 540, Ativa = true },
                    new() { Nome = "Material Clínico - Zona 1", Descricao = "Material clínico de uso rotineiro.", LocalizacaoZonaArmazenamentoId = Loc("Armazém de Material Clínico"), CapacidadeMaxima = 800, Ativa = true },
                    new() { Nome = "Material Clínico - Frigorífico", Descricao = "Produtos sensíveis que requerem refrigeração.", LocalizacaoZonaArmazenamentoId = Loc("Armazém de Material Clínico"), CapacidadeMaxima = 300, Ativa = true },
                    new() { Nome = "Equipamentos Pesados - Zona A", Descricao = "Armazenamento de equipamentos de grande porte.", LocalizacaoZonaArmazenamentoId = Loc("Armazém de Equipamentos Pesados"), CapacidadeMaxima = 1200, Ativa = true },
                    new() { Nome = "Equipamentos Pesados - Zona B", Descricao = "Ferramentas e equipamentos mecânicos.", LocalizacaoZonaArmazenamentoId = Loc("Armazém de Equipamentos Pesados"), CapacidadeMaxima = 950, Ativa = true },
                    new() { Nome = "Setor Industrial - Zona 1", Descricao = "Zona dedicada a armazenamento de peças industriais.", LocalizacaoZonaArmazenamentoId = Loc("Zona Industrial - Setor 1"), CapacidadeMaxima = 1000, Ativa = true },
                    new() { Nome = "Setor Industrial - Zona 2", Descricao = "Armazenamento de matéria-prima geral.", LocalizacaoZonaArmazenamentoId = Loc("Zona Industrial - Setor 1"), CapacidadeMaxima = 750, Ativa = true },
                    new() { Nome = "Setor Industrial - Zona 3", Descricao = "Componentes de reposição e acessórios.", LocalizacaoZonaArmazenamentoId = Loc("Zona Industrial - Setor 2"), CapacidadeMaxima = 400, Ativa = true },
                    new() { Nome = "Logística - Zona 0", Descricao = "Área de receção de cargas.", LocalizacaoZonaArmazenamentoId = Loc("Edifício Logístico - Piso 0"), CapacidadeMaxima = 1500, Ativa = true },
                    new() { Nome = "Logística - Zona 1", Descricao = "Separação de produtos e embalamento.", LocalizacaoZonaArmazenamentoId = Loc("Edifício Logístico - Piso 1"), CapacidadeMaxima = 1300, Ativa = true },
                    new() { Nome = "Logística - Zona 2", Descricao = "Zona auxiliar para movimentos rápidos.", LocalizacaoZonaArmazenamentoId = Loc("Edifício Logístico - Piso 1"), CapacidadeMaxima = 700, Ativa = true },
                    new() { Nome = "Depósito Exterior - Zona A", Descricao = "Material resistente às condições exteriores.", LocalizacaoZonaArmazenamentoId = Loc("Depósito Exterior Coberto"), CapacidadeMaxima = 1000, Ativa = true },
                    new() { Nome = "Depósito Exterior - Zona B", Descricao = "Paletes e grandes volumes.", LocalizacaoZonaArmazenamentoId = Loc("Depósito Exterior Coberto"), CapacidadeMaxima = 1400, Ativa = true },
                    new() { Nome = "Armazém Secundário - Zona 2", Descricao = "Armazenamento adicional de média rotação.", LocalizacaoZonaArmazenamentoId = Loc("Armazém Secundário"), CapacidadeMaxima = 450, Ativa = true },
                    new() { Nome = "Armazém Secundário - Zona 3", Descricao = "Materiais não críticos.", LocalizacaoZonaArmazenamentoId = Loc("Armazém Secundário"), CapacidadeMaxima = 350, Ativa = true },
                    new() { Nome = "Bloco A - Piso 0 - Zona A", Descricao = "Produtos de acesso rápido.", LocalizacaoZonaArmazenamentoId = Loc("Bloco A - Piso 0"), CapacidadeMaxima = 500, Ativa = true },
                    new() { Nome = "Bloco A - Piso 0 - Zona B", Descricao = "Material de suporte.", LocalizacaoZonaArmazenamentoId = Loc("Bloco A - Piso 0"), CapacidadeMaxima = 300, Ativa = true },
                    new() { Nome = "Bloco A - Piso 1 - Zona A", Descricao = "Material médico geral.", LocalizacaoZonaArmazenamentoId = Loc("Bloco A - Piso 1"), CapacidadeMaxima = 420, Ativa = true },
                    new() { Nome = "Bloco A - Piso 1 - Zona B", Descricao = "Armazenamento de consumíveis diversos.", LocalizacaoZonaArmazenamentoId = Loc("Bloco A - Piso 1"), CapacidadeMaxima = 380, Ativa = true },
                    new() { Nome = "Sala de Suprimentos - Zona 1", Descricao = "Consumíveis de alta rotação.", LocalizacaoZonaArmazenamentoId = Loc("Sala de Suprimentos"), CapacidadeMaxima = 250, Ativa = true },
                    new() { Nome = "Sala de Suprimentos - Zona 2", Descricao = "Materiais de reposição rápida.", LocalizacaoZonaArmazenamentoId = Loc("Sala de Suprimentos"), CapacidadeMaxima = 270, Ativa = true },
                    new() { Nome = "Sala de Suprimentos - Zona 3", Descricao = "Embalagens e acessórios.", LocalizacaoZonaArmazenamentoId = Loc("Sala de Suprimentos"), CapacidadeMaxima = 200, Ativa = true },
                    new() { Nome = "Edifício Técnico - Piso 2 - Zona A", Descricao = "Servidores e equipamentos especializados.", LocalizacaoZonaArmazenamentoId = Loc("Edifício Técnico - Piso 2"), CapacidadeMaxima = 380, Ativa = true },
                    new() { Nome = "Edifício Técnico - Piso 2 - Zona B", Descricao = "Peças técnicas de alta precisão.", LocalizacaoZonaArmazenamentoId = Loc("Edifício Técnico - Piso 2"), CapacidadeMaxima = 260, Ativa = true },
                    new() { Nome = "Bloco B - Subsolo - Zona A", Descricao = "Armazenamento de grandes volumes.", LocalizacaoZonaArmazenamentoId = Loc("Bloco B - Subsolo"), CapacidadeMaxima = 800, Ativa = true },
                    new() { Nome = "Bloco A - Piso 0 - Zona Armazenamento 1", Descricao = "Zona de materiais gerais.", LocalizacaoZonaArmazenamentoId = Loc("Bloco A - Piso 0"), CapacidadeMaxima = 300, Ativa = false },
                    new() { Nome = "Bloco A - Piso 0 - Zona Armazenamento 2", Descricao = "Armazenamento de suporte operacional.", LocalizacaoZonaArmazenamentoId = Loc("Bloco A - Piso 0"), CapacidadeMaxima = 250, Ativa = false },
                    new() { Nome = "Bloco A - Piso 1 - Zona Técnica", Descricao = "Equipamentos e ferramentas técnicas.", LocalizacaoZonaArmazenamentoId = Loc("Bloco A - Piso 1"), CapacidadeMaxima = 280, Ativa = false },
                    new() { Nome = "Bloco A - Piso 1 - Armazenamento Extra", Descricao = "Espaço adicional para consumíveis.", LocalizacaoZonaArmazenamentoId = Loc("Bloco A - Piso 1"), CapacidadeMaxima = 320, Ativa = false },
                    new() { Nome = "Bloco B - Subsolo - Zona Fria", Descricao = "Zona com baixa iluminação e temperatura.", LocalizacaoZonaArmazenamentoId = Loc("Bloco B - Subsolo"), CapacidadeMaxima = 500, Ativa = false },
                    new() { Nome = "Bloco B - Subsolo - Zona Reservada", Descricao = "Armazenamento restrito a pessoal autorizado.", LocalizacaoZonaArmazenamentoId = Loc("Bloco B - Subsolo"), CapacidadeMaxima = 450, Ativa = false },
                    new() { Nome = "Armazém Central - Depósito 3", Descricao = "Zona para materiais diversos.", LocalizacaoZonaArmazenamentoId = Loc("Armazém Central"), CapacidadeMaxima = 700, Ativa = false },
                    new() { Nome = "Armazém Central - Armazenamento Baixa Rotação", Descricao = "Produtos de baixa rotatividade.", LocalizacaoZonaArmazenamentoId = Loc("Armazém Central"), CapacidadeMaxima = 650, Ativa = false },
                    new() { Nome = "Armazém Secundário - Zona 4", Descricao = "Armazenamento secundário complementar.", LocalizacaoZonaArmazenamentoId = Loc("Armazém Secundário"), CapacidadeMaxima = 300, Ativa = false },
                    new() { Nome = "Armazém Secundário - Zona 5", Descricao = "Armazenamento geral do armazém secundário.", LocalizacaoZonaArmazenamentoId = Loc("Armazém Secundário"), CapacidadeMaxima = 350, Ativa = false },
                    new() { Nome = "Sala de Suprimentos - Secção 4", Descricao = "Material de uso rápido e imediato.", LocalizacaoZonaArmazenamentoId = Loc("Sala de Suprimentos"), CapacidadeMaxima = 220, Ativa = false },
                    new() { Nome = "Sala de Suprimentos - Secção 5", Descricao = "Armazenamento de itens críticos.", LocalizacaoZonaArmazenamentoId = Loc("Sala de Suprimentos"), CapacidadeMaxima = 260, Ativa = false },
                    new() { Nome = "Edifício Técnico - Piso 2 - Sala A", Descricao = "Equipamentos técnicos de apoio.", LocalizacaoZonaArmazenamentoId = Loc("Edifício Técnico - Piso 2"), CapacidadeMaxima = 380, Ativa = false },
                    new() { Nome = "Edifício Técnico - Piso 2 - Sala B", Descricao = "Ferramentas e peças de substituição.", LocalizacaoZonaArmazenamentoId = Loc("Edifício Técnico - Piso 2"), CapacidadeMaxima = 320, Ativa = false },
                    new() { Nome = "Bloco C - Piso 0 - Armazenamento A", Descricao = "Materiais de suporte logístico.", LocalizacaoZonaArmazenamentoId = Loc("Bloco C - Piso 0"), CapacidadeMaxima = 400, Ativa = false },
                    new() { Nome = "Bloco C - Piso 0 - Armazenamento B", Descricao = "Consumíveis de reposição.", LocalizacaoZonaArmazenamentoId = Loc("Bloco C - Piso 0"), CapacidadeMaxima = 380, Ativa = false },
                    new() { Nome = "Bloco C - Piso 1 - Secção Técnica", Descricao = "Equipamentos de manutenção.", LocalizacaoZonaArmazenamentoId = Loc("Bloco C - Piso 1"), CapacidadeMaxima = 420, Ativa = false },
                    new() { Nome = "Bloco C - Piso 1 - Secção Ferramentas", Descricao = "Ferramentas para operações internas.", LocalizacaoZonaArmazenamentoId = Loc("Bloco C - Piso 1"), CapacidadeMaxima = 390, Ativa = false },
                    new() { Nome = "Bloco C - Piso 2 - Armazenamento Rápido", Descricao = "Material de uso frequente.", LocalizacaoZonaArmazenamentoId = Loc("Bloco C - Piso 2"), CapacidadeMaxima = 450, Ativa = false },
                    new() { Nome = "Bloco C - Piso 2 - Armazenamento Especializado", Descricao = "Produtos técnicos especializados.", LocalizacaoZonaArmazenamentoId = Loc("Bloco C - Piso 2"), CapacidadeMaxima = 470, Ativa = false }
                };

            db.ZonaArmazenamento.AddRange(zonas);
            db.SaveChanges();
        }

*/


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
                new() { Nome = "Desinfetantes", Descricao = "Álcool, clorexidina, gel, etc." },
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
