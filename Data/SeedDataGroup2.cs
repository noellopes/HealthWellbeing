using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthWellbeing.Data
{
    public class SeedDataGroup2
    {
        
        public static async Task Populate(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var db = services.GetRequiredService<HealthWellbeingDbContext>();

                if (db == null) throw new ArgumentNullException(nameof(db));

                // Garante que a BD existe
                db.Database.EnsureCreated();

                PopulateCategorias(db);
                PopulateConsumiveis(db);
                PopulateZonasArmazenamento(db);
                PopulateStock(db);
                PopulateHistoricoCompras(db);

                // ---------------------------------------------------------
                // 2. DADOS DE UTILIZADORES
                // ---------------------------------------------------------
                var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                await PopulateUsersAndRoles(userManager, roleManager);
            }
        }

        private static async Task PopulateUsersAndRoles(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // 1. Criar Roles
            string[] roles = { "Admin", "Gestor de armazenamento" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // 2. Criar Admin Padrão
            await EnsureUser(userManager, "admin@health.com", "P@ssword123!", new[] { "Admin", "Gestor de armazenamento" });

            // 3. Criar Gestor
            await EnsureUser(userManager, "gestor@health.com", "Gestor123!", new[] { "Gestor de armazenamento" });

            // 4. Criar Utilizador 
            await EnsureUser(userManager, "mendes@health.com", "Mendes123!", new[] { "Admin", "Gestor de armazenamento" });
        }

        private static async Task EnsureUser(UserManager<IdentityUser> userManager, string email, string password, string[] roles)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    foreach (var role in roles)
                    {
                        await userManager.AddToRoleAsync(user, role);
                    }
                }
            }
        }


        private static void PopulateHistoricoCompras(HealthWellbeingDbContext db)
        {
            if (db.HistoricoCompras.Any()) return;

            var stocks = db.Stock
                .Include(s => s.Consumivel)
                .ToList();

            if (!stocks.Any()) return;

            var movimentos = new List<HistoricoCompras>();

            foreach (var stock in stocks)
            {
                // simula uma compra inicial
                movimentos.Add(new HistoricoCompras
                {
                    StockId = stock.StockId,
                    Quantidade = stock.QuantidadeAtual,
                    Tipo = "Entrada",
                    Data = DateTime.Now.AddDays(-7) // compra há 1 semana
                });
            }

            db.HistoricoCompras.AddRange(movimentos);
            db.SaveChanges();
        }

        private static void PopulateStock(HealthWellbeingDbContext db)
        {
            if (db.Stock.Any()) return;

            var consumiveis = db.Consumivel.ToList();
            var zonas = db.ZonaArmazenamento.Where(z => z.Ativa).ToList();

            if (!consumiveis.Any() || !zonas.Any()) return;

            var rnd = new Random();
            var stocks = new List<Stock>();

            foreach (var c in consumiveis)
            {
                var zona = zonas[rnd.Next(zonas.Count)];

                stocks.Add(new Stock
                {
                    ConsumivelID = c.ConsumivelId,
                    ZonaID = zona.ZonaId,
                    // 🔑 COMEÇA NA QUANTIDADE MÍNIMA
                    QuantidadeAtual = c.QuantidadeMinima,
                    QuantidadeMinima = c.QuantidadeMinima,
                    QuantidadeMaxima = c.QuantidadeMaxima,
                    UsaValoresDoConsumivel = true,
                    DataUltimaAtualizacao = DateTime.Now
                });
            }

            db.Stock.AddRange(stocks);
            db.SaveChanges();
        }

        private static void PopulateZonasArmazenamento(HealthWellbeingDbContext db)
        {
            if (db.ZonaArmazenamento.Any()) return;

            var consumiveis = db.Consumivel.ToList();
            var rooms = db.Room.ToList();

            int Cons(string nome) => consumiveis.First(c => c.Nome == nome).ConsumivelId;
            int Sala(string nome) => rooms.First(r => r.Name == nome).RoomId;

            var zonas = new List<ZonaArmazenamento>
            {
                // Depósito 1
                new() { NomeZona = "Prateleira A1", ConsumivelId = Cons("Luvas Cirúrgicas Pequenas"), RoomId = Sala("Depósito 1"), CapacidadeMaxima = 200, QuantidadeAtual = 120, Ativa = true },
                new() { NomeZona = "Prateleira A2", ConsumivelId = Cons("Luvas Cirúrgicas Médias"), RoomId = Sala("Depósito 1"), CapacidadeMaxima = 200, QuantidadeAtual = 80, Ativa = true },
                new() { NomeZona = "Prateleira A3", ConsumivelId = Cons("Luvas de Nitrilo"), RoomId = Sala("Depósito 1"), CapacidadeMaxima = 300, QuantidadeAtual = 150, Ativa = true },
                new() { NomeZona = "Armário B1",     ConsumivelId = Cons("Máscara Cirúrgica"), RoomId = Sala("Depósito 1"), CapacidadeMaxima = 500, QuantidadeAtual = 420, Ativa = true },
                new() { NomeZona = "Armário B2",     ConsumivelId = Cons("Máscara N95"),       RoomId = Sala("Depósito 1"), CapacidadeMaxima = 300, QuantidadeAtual = 110, Ativa = false },

                // Depósito 2
                new() { NomeZona = "Gaveta C1", ConsumivelId = Cons("Seringa 5ml"),   RoomId = Sala("Depósito 2"), CapacidadeMaxima = 400, QuantidadeAtual = 180, Ativa = true },
                new() { NomeZona = "Gaveta C2", ConsumivelId = Cons("Seringa 10ml"),  RoomId = Sala("Depósito 2"), CapacidadeMaxima = 400, QuantidadeAtual = 140, Ativa = true },
                new() { NomeZona = "Gaveta C3", ConsumivelId = Cons("Agulhas 21G"),   RoomId = Sala("Depósito 2"), CapacidadeMaxima = 600, QuantidadeAtual = 260, Ativa = true },
                new() { NomeZona = "Caixa D1",  ConsumivelId = Cons("Compressa Estéril"), RoomId = Sala("Depósito 2"), CapacidadeMaxima = 250, QuantidadeAtual = 90, Ativa = true },
                new() { NomeZona = "Caixa D2",  ConsumivelId = Cons("Compressa Não Estéril"), RoomId = Sala("Depósito 2"), CapacidadeMaxima = 250, QuantidadeAtual = 60, Ativa = false },

                // Depósito 3
                new() { NomeZona = "Prateleira E1", ConsumivelId = Cons("Gaze Esterilizada"), RoomId = Sala("Depósito 3"), CapacidadeMaxima = 300, QuantidadeAtual = 150, Ativa = true },
                new() { NomeZona = "Prateleira E2", ConsumivelId = Cons("Gaze Não Esterilizada"), RoomId = Sala("Depósito 3"), CapacidadeMaxima = 300, QuantidadeAtual = 120, Ativa = true },
                new() { NomeZona = "Armário F1",    ConsumivelId = Cons("Álcool 70%"), RoomId = Sala("Depósito 3"), CapacidadeMaxima = 80, QuantidadeAtual = 30, Ativa = true },
                new() { NomeZona = "Armário F2",    ConsumivelId = Cons("Clorexidina"), RoomId = Sala("Depósito 3"), CapacidadeMaxima = 60, QuantidadeAtual = 25, Ativa = true },

                // Sala de Emergência 1
                new() { NomeZona = "Kit Emergência 1", ConsumivelId = Cons("Máscara N95"), RoomId = Sala("Sala de Emergência 1"), CapacidadeMaxima = 100, QuantidadeAtual = 40, Ativa = true },
                new() { NomeZona = "Kit Emergência 2", ConsumivelId = Cons("Luvas de Nitrilo"), RoomId = Sala("Sala de Emergência 1"), CapacidadeMaxima = 200, QuantidadeAtual = 70, Ativa = true },
                new() { NomeZona = "Kit Emergência 3", ConsumivelId = Cons("Seringa 10ml"), RoomId = Sala("Sala de Emergência 1"), CapacidadeMaxima = 200, QuantidadeAtual = 55, Ativa = true },

                // UTI 1
                new() { NomeZona = "Armário UTI A", ConsumivelId = Cons("Luvas Cirúrgicas Médias"), RoomId = Sala("UTI 1"), CapacidadeMaxima = 150, QuantidadeAtual = 60, Ativa = true },
                new() { NomeZona = "Armário UTI B", ConsumivelId = Cons("Compressa Estéril"), RoomId = Sala("UTI 1"), CapacidadeMaxima = 120, QuantidadeAtual = 45, Ativa = true },
                new() { NomeZona = "Armário UTI C", ConsumivelId = Cons("Agulhas 21G"), RoomId = Sala("UTI 1"), CapacidadeMaxima = 200, QuantidadeAtual = 95, Ativa = true },

                // Centro Cirúrgico 3
                new() { NomeZona = "Carrinho Cirurgia 1", ConsumivelId = Cons("Luvas Cirúrgicas Pequenas"), RoomId = Sala("Centro Cirúrgico 3"), CapacidadeMaxima = 120, QuantidadeAtual = 40, Ativa = true },
                new() { NomeZona = "Carrinho Cirurgia 2", ConsumivelId = Cons("Máscara Cirúrgica"), RoomId = Sala("Centro Cirúrgico 3"), CapacidadeMaxima = 200, QuantidadeAtual = 90, Ativa = true },
                new() { NomeZona = "Carrinho Cirurgia 3", ConsumivelId = Cons("Gaze Esterilizada"), RoomId = Sala("Centro Cirúrgico 3"), CapacidadeMaxima = 150, QuantidadeAtual = 70, Ativa = false },

                // Farmácia 1
                new() { NomeZona = "Prateleira Farm 1", ConsumivelId = Cons("Álcool 70%"), RoomId = Sala("Farmácia 1"), CapacidadeMaxima = 100, QuantidadeAtual = 35, Ativa = true },
                new() { NomeZona = "Prateleira Farm 2", ConsumivelId = Cons("Clorexidina"), RoomId = Sala("Farmácia 1"), CapacidadeMaxima = 80, QuantidadeAtual = 20, Ativa = true },

                // Sala de Esterilização 1
                new() { NomeZona = "Zona Esterilização A", ConsumivelId = Cons("Compressa Não Estéril"), RoomId = Sala("Sala de Esterilização 1"), CapacidadeMaxima = 200, QuantidadeAtual = 70, Ativa = true },
                new() { NomeZona = "Zona Esterilização B", ConsumivelId = Cons("Gaze Não Esterilizada"), RoomId = Sala("Sala de Esterilização 1"), CapacidadeMaxima = 250, QuantidadeAtual = 110, Ativa = true },

                // Sala de Exames 1
                new() { NomeZona = "Gaveta Exames 1", ConsumivelId = Cons("Seringa 5ml"), RoomId = Sala("Sala de Exames 1"), CapacidadeMaxima = 200, QuantidadeAtual = 85, Ativa = true },
                new() { NomeZona = "Gaveta Exames 2", ConsumivelId = Cons("Luvas Cirúrgicas Médias"), RoomId = Sala("Sala de Exames 1"), CapacidadeMaxima = 120, QuantidadeAtual = 30, Ativa = false }
            };

            db.ZonaArmazenamento.AddRange(zonas);
            db.SaveChanges();
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
                C("Luvas Cirúrgicas Pequenas", "Pacote de luvas pequenas", "Luvas", 100, 0, 10),
                C("Luvas Cirúrgicas Médias", "Pacote de luvas médias", "Luvas", 100, 0, 10),
                C("Luvas de Nitrilo", "Luvas de nitrilo descartáveis", "Luvas", 200, 0, 20),

                C("Máscara N95", "Máscara respiratória N95", "Máscaras", 200, 0, 20),
                C("Máscara Cirúrgica", "Máscara descartável para uso clínico", "Máscaras", 300, 0, 30),

                C("Seringa 5ml", "Seringa descartável 5ml", "Seringas e Agulhas", 300, 0, 30),
                C("Seringa 10ml", "Seringa descartável 10ml", "Seringas e Agulhas", 300, 0, 30),
                C("Agulhas 21G", "Agulhas esterilizadas 21G", "Seringas e Agulhas", 500, 0, 50),

                C("Compressa Estéril", "Pacote de compressas estéreis", "Compressas", 150, 0, 15),
                C("Compressa Não Estéril", "Pacote de compressas não estéreis", "Compressas", 150, 0, 15),

                C("Gaze Esterilizada", "Pacote de gazes esterilizadas", "Gazes", 200, 0, 20),
                C("Gaze Não Esterilizada", "Pacote de gazes não esterilizadas", "Gazes", 200, 0, 20),

                C("Álcool 70%", "Frasco de álcool 70%", "Desinfetantes", 50, 0, 5),
                C("Clorexidina", "Frasco de clorexidina", "Desinfetantes", 40, 0, 5)
            };

            db.Consumivel.AddRange(consumiveis);
            db.SaveChanges();
        }
    }
}