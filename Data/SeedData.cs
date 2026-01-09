using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

internal class SeedData
{
    public static void Populate(HealthWellbeingDbContext? dbContext)
    {
        if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));

        dbContext.Database.EnsureCreated();

        PopulateCategorias(dbContext);
        PopulateAlimentos(dbContext);
        PopulateAlergias(dbContext);
        PopulateRestricoesAlimentares(dbContext);
        PopulateAlimentoSubstitutos(dbContext);
        PopulateReceitas(dbContext);
        PopulateComponentesReceita(dbContext);

        // Clients and food-plan related seeds
        PopulateClients(dbContext);
        PopulateMetas(dbContext);
        PopulatePlanosAlimentares(dbContext);
        PopulateClientAlergias(dbContext);
        PopulateClientRestricoes(dbContext);
        PopulateReceitasParaPlanosAlimentares(dbContext);

        PopulateSpecialities(dbContext);
        // PopulateConsultas IS NOT WORKING
        // PopulateConsultas(dbContext);
        PopulateDoctor(dbContext);
        PopulateUtenteSaude(dbContext);

        PopulateTrainingType(dbContext);
        PopulatePlan(dbContext);

        // --- ALTERAÇÃO AQUI: Capturamos a lista de Trainers ---
        var trainers = PopulateTrainer(dbContext);

        // --- NOVO MÉTODO: Povoamento dos Treinos Agendados ---
        PopulateTraining(dbContext, trainers);

        PopulateEventTypes(dbContext);
        PopulateEvents(dbContext);
        PopulateLevels(dbContext);

        Initialize(dbContext);
    }

    internal static void SeedRoles(RoleManager<IdentityRole> roleManager)
    {
        EnsureRoleIsCreatedAsync(roleManager, "Administrator").Wait();
        EnsureRoleIsCreatedAsync(roleManager, "Cliente").Wait();
        EnsureRoleIsCreatedAsync(roleManager, "Nutricionista").Wait();
    }

    private static async Task EnsureRoleIsCreatedAsync(RoleManager<IdentityRole> roleManager, string role)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    internal static void SeedUsers(UserManager<IdentityUser> userManager, HealthWellbeingDbContext dbContext)
    {
        EnsureUserIsCreatedAsync(userManager, "cliente@health.com", "Secret123$", new[] { "Cliente" }, dbContext).Wait();
        EnsureUserIsCreatedAsync(userManager, "nutri@health.com", "Secret123$", new[] { "Nutricionista" }, dbContext).Wait();

        // In case a new Client profile was created here, ensure the dependent food-plan seed data exists too.
        PopulateMetas(dbContext);
        PopulatePlanosAlimentares(dbContext);
        PopulateClientAlergias(dbContext);
        PopulateClientRestricoes(dbContext);
        PopulateReceitasParaPlanosAlimentares(dbContext);
    }

    internal static void SeedPopulateClientsAsUsers(UserManager<IdentityUser> userManager, HealthWellbeingDbContext dbContext)
    {
        SeedPopulateClientsAsUsersAsync(userManager, dbContext).GetAwaiter().GetResult();
    }

    private static async Task SeedPopulateClientsAsUsersAsync(UserManager<IdentityUser> userManager, HealthWellbeingDbContext dbContext)
    {
        if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));

        dbContext.Database.EnsureCreated();

        var clients = PopulateClients(dbContext) ?? dbContext.Client.ToList();
        var anyClientUpdated = false;

        foreach (var client in clients)
        {
            var email = client.Email?.Trim();
            if (string.IsNullOrWhiteSpace(email))
                continue;

            var user = await userManager.FindByEmailAsync(email) ?? await userManager.FindByNameAsync(email);

            if (user == null)
            {
                user = new IdentityUser(email)
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(user, "Secret123$");
                if (!createResult.Succeeded)
                {
                    // If creation failed due to a race/duplicate, try loading it again.
                    user = await userManager.FindByEmailAsync(email) ?? await userManager.FindByNameAsync(email);
                    if (user == null)
                        continue;
                }
            }

            if (!await userManager.IsInRoleAsync(user, "Cliente"))
            {
                await userManager.AddToRoleAsync(user, "Cliente");
            }

            if (!string.Equals(client.IdentityUserId, user.Id, StringComparison.Ordinal))
            {
                client.IdentityUserId = user.Id;
                anyClientUpdated = true;
            }
        }

        if (anyClientUpdated)
        {
            await dbContext.SaveChangesAsync();
        }

        // Ensure food-plan related seed data after clients exist.
        PopulateMetas(dbContext);
        PopulatePlanosAlimentares(dbContext);
        PopulateClientAlergias(dbContext);
        PopulateClientRestricoes(dbContext);
        PopulateReceitasParaPlanosAlimentares(dbContext);
    }

    private static async Task<IdentityUser> EnsureUserIsCreatedAsync(
        UserManager<IdentityUser> userManager,
        string username,
        string password,
        string[] roles,
        HealthWellbeingDbContext? dbContext = null)
    {
        var user = await userManager.FindByNameAsync(username);

        if (user == null)
        {
            user = new IdentityUser(username) { Email = username, EmailConfirmed = true };
            await userManager.CreateAsync(user, password);
        }

        foreach (var role in roles)
        {
            if (!await userManager.IsInRoleAsync(user, role))
                await userManager.AddToRoleAsync(user, role);
        }
        if (dbContext != null && roles.Contains("Cliente", StringComparer.OrdinalIgnoreCase))
        {
            await EnsureClientProfileAsync(dbContext, user);
        }

        return user;
    }

    private static async Task EnsureClientProfileAsync(HealthWellbeingDbContext dbContext, IdentityUser user)
    {

        var client = await dbContext.Client
            .FirstOrDefaultAsync(c => c.IdentityUserId == user.Id || (c.IdentityUserId == null && c.Email == user.Email));

        if (client == null)
        {
            client = new Client
            {
                ClientId = Guid.NewGuid().ToString("N"),
                IdentityUserId = user.Id,
                Name = "Cliente Health",          // passa regex (nome + apelido)
                Email = user.Email ?? user.UserName ?? "",
                Phone = "555-0000000",
                Address = "",
                BirthDate = null,
                Gender = "",
                RegistrationDate = DateTime.Now
            };

            dbContext.Client.Add(client);
        }
        else
        {
            client.IdentityUserId = user.Id;
            if (!string.IsNullOrWhiteSpace(user.Email))
                client.Email = user.Email;
        }

        await dbContext.SaveChangesAsync();
    }

    internal static void SeedDefaultAdmin(UserManager<IdentityUser> userManager)
    {
        EnsureUserIsCreatedAsync(userManager, "admin@ipg.pt", "Secret123$", ["Administrator"]).Wait();
    }

    private static void PopulateCategorias(HealthWellbeingDbContext context)
    {
        if (context.CategoriaAlimento.Any())
        {
            return;
        }

        context.CategoriaAlimento.AddRange(
            new CategoriaAlimento { Name = "Frutas", Description = "Categoria de frutas." },
            new CategoriaAlimento { Name = "Vegetais", Description = "Categoria de vegetais." },
            new CategoriaAlimento { Name = "Grãos", Description = "Categoria de grãos." },
            new CategoriaAlimento { Name = "Laticínios", Description = "Categoria de laticínios." },
            new CategoriaAlimento { Name = "Carnes", Description = "Categoria de carnes." }
        );

        context.SaveChanges();
    }

    private static void PopulateAlimentos(HealthWellbeingDbContext context)
    {
        var seedAlimentos = new List<Alimento>
        {
            new Alimento { Name = "Maçã", Description = "Fruta doce e crocante.", CategoriaAlimentoId = 1, Calories = 52, KcalPor100g = 52, ProteinaGPor100g = 0.3m, HidratosGPor100g = 14m, GorduraGPor100g = 0.2m },
            new Alimento { Name = "Cenoura", Description = "Vegetal rico em vitamina A.", CategoriaAlimentoId = 2, Calories = 41, KcalPor100g = 41, ProteinaGPor100g = 0.9m, HidratosGPor100g = 10m, GorduraGPor100g = 0.2m },
            new Alimento { Name = "Arroz", Description = "Grão básico na alimentação.", CategoriaAlimentoId = 3, Calories = 130, KcalPor100g = 130, ProteinaGPor100g = 2.7m, HidratosGPor100g = 28m, GorduraGPor100g = 0.3m },
            new Alimento { Name = "Leite", Description = "Líquido rico em cálcio.", CategoriaAlimentoId = 4, Calories = 42, KcalPor100g = 42, ProteinaGPor100g = 3.4m, HidratosGPor100g = 5m, GorduraGPor100g = 1m },
            new Alimento { Name = "Frango", Description = "Carne branca rica em proteínas.", CategoriaAlimentoId = 5, Calories = 165, KcalPor100g = 165, ProteinaGPor100g = 31m, HidratosGPor100g = 0m, GorduraGPor100g = 3.6m },

            // Novos alimentos para diversificar (IDs esperados: 6..15)
            new Alimento { Name = "Banana", Description = "Fruta energética, boa fonte de potássio.", CategoriaAlimentoId = 1, Calories = 89, KcalPor100g = 89, ProteinaGPor100g = 1.1m, HidratosGPor100g = 23m, GorduraGPor100g = 0.3m },
            new Alimento { Name = "Batata", Description = "Tubérculo versátil.", CategoriaAlimentoId = 3, Calories = 77, KcalPor100g = 77, ProteinaGPor100g = 2m, HidratosGPor100g = 17m, GorduraGPor100g = 0.1m },
            new Alimento { Name = "Pão", Description = "Fonte de carboidratos, feito geralmente de trigo.", CategoriaAlimentoId = 3, Calories = 265, KcalPor100g = 265, ProteinaGPor100g = 9m, HidratosGPor100g = 49m, GorduraGPor100g = 3.2m },
            new Alimento { Name = "Tofu", Description = "Produto de soja, boa alternativa proteica.", CategoriaAlimentoId = 4, Calories = 76, KcalPor100g = 76, ProteinaGPor100g = 8m, HidratosGPor100g = 1.9m, GorduraGPor100g = 4.8m },
            new Alimento { Name = "Leite de Amêndoa", Description = "Alternativa vegetal ao leite.", CategoriaAlimentoId = 4, Calories = 15, KcalPor100g = 15, ProteinaGPor100g = 0.4m, HidratosGPor100g = 0.3m, GorduraGPor100g = 1.1m },
            new Alimento { Name = "Feijão", Description = "Leguminosa rica em fibras e proteínas.", CategoriaAlimentoId = 3, Calories = 347, KcalPor100g = 347, ProteinaGPor100g = 21m, HidratosGPor100g = 63m, GorduraGPor100g = 1.2m },
            new Alimento { Name = "Aveia", Description = "Cereal integral, boa fonte de fibras.", CategoriaAlimentoId = 3, Calories = 389, KcalPor100g = 389, ProteinaGPor100g = 17m, HidratosGPor100g = 66m, GorduraGPor100g = 7m },
            new Alimento { Name = "Queijo", Description = "Lácteo fermentado, fonte de cálcio.", CategoriaAlimentoId = 4, Calories = 402, KcalPor100g = 402, ProteinaGPor100g = 25m, HidratosGPor100g = 1.3m, GorduraGPor100g = 33m },
            new Alimento { Name = "Salmão", Description = "Peixe gordo, rico em ômega-3.", CategoriaAlimentoId = 5, Calories = 208, KcalPor100g = 208, ProteinaGPor100g = 20m, HidratosGPor100g = 0m, GorduraGPor100g = 13m },
            new Alimento { Name = "Batata Doce", Description = "Tubérculo rico em betacaroteno e fibras.", CategoriaAlimentoId = 3, Calories = 86, KcalPor100g = 86, ProteinaGPor100g = 1.6m, HidratosGPor100g = 20m, GorduraGPor100g = 0.1m },

            // Alimentos adicionais para suportar associações de alergias/restrições
            new Alimento { Name = "Ovo", Description = "Ovo de galinha, fonte de proteína.", CategoriaAlimentoId = 5, Calories = 155, KcalPor100g = 155, ProteinaGPor100g = 13m, HidratosGPor100g = 1.1m, GorduraGPor100g = 11m },
            new Alimento { Name = "Trigo", Description = "Cereal com glúten, base para farinhas e pães.", CategoriaAlimentoId = 3, Calories = 339, KcalPor100g = 339, ProteinaGPor100g = 13m, HidratosGPor100g = 72m, GorduraGPor100g = 2.5m },
            new Alimento { Name = "Amendoim", Description = "Oleaginosa rica em gorduras saudáveis.", CategoriaAlimentoId = 3, Calories = 567, KcalPor100g = 567, ProteinaGPor100g = 25.8m, HidratosGPor100g = 16.1m, GorduraGPor100g = 49.2m },
            new Alimento { Name = "Amêndoa", Description = "Oleaginosa rica em fibras e gorduras saudáveis.", CategoriaAlimentoId = 3, Calories = 579, KcalPor100g = 579, ProteinaGPor100g = 21.2m, HidratosGPor100g = 21.7m, GorduraGPor100g = 49.9m },
            new Alimento { Name = "Camarão", Description = "Crustáceo (frutos do mar), fonte de proteína.", CategoriaAlimentoId = 5, Calories = 99, KcalPor100g = 99, ProteinaGPor100g = 24m, HidratosGPor100g = 0.2m, GorduraGPor100g = 0.3m },

            // Ingredientes base para receitas (mantidos no fim para não afetar IDs esperados 1..15)
            new Alimento { Name = "Cebola", Description = "Base aromática comum em muitas receitas.", CategoriaAlimentoId = 2, Calories = 40, KcalPor100g = 40, ProteinaGPor100g = 1.1m, HidratosGPor100g = 9.3m, GorduraGPor100g = 0.1m },
            new Alimento { Name = "Alho", Description = "Aromático usado em temperos e refogados.", CategoriaAlimentoId = 2, Calories = 149, KcalPor100g = 149, ProteinaGPor100g = 6.4m, HidratosGPor100g = 33m, GorduraGPor100g = 0.5m },
            new Alimento { Name = "Tomate", Description = "Vegetal usado em molhos e saladas.", CategoriaAlimentoId = 2, Calories = 18, KcalPor100g = 18, ProteinaGPor100g = 0.9m, HidratosGPor100g = 3.9m, GorduraGPor100g = 0.2m },
            new Alimento { Name = "Alface", Description = "Folha verde para saladas e sanduíches.", CategoriaAlimentoId = 2, Calories = 15, KcalPor100g = 15, ProteinaGPor100g = 1.4m, HidratosGPor100g = 2.9m, GorduraGPor100g = 0.2m },
            new Alimento { Name = "Azeite", Description = "Gordura vegetal usada em temperos e cozinhados.", CategoriaAlimentoId = 2, Calories = 884, KcalPor100g = 884, ProteinaGPor100g = 0m, HidratosGPor100g = 0m, GorduraGPor100g = 100m },
            new Alimento { Name = "Limão", Description = "Fruta cítrica usada para temperar.", CategoriaAlimentoId = 1, Calories = 29, KcalPor100g = 29, ProteinaGPor100g = 1.1m, HidratosGPor100g = 9.3m, GorduraGPor100g = 0.3m },
            new Alimento { Name = "Laranja", Description = "Fruta cítrica, usada como sobremesa/acompanhamento.", CategoriaAlimentoId = 1, Calories = 47, KcalPor100g = 47, ProteinaGPor100g = 0.9m, HidratosGPor100g = 12m, GorduraGPor100g = 0.1m },
            new Alimento { Name = "Mel", Description = "Adoçante natural, usado em saladas e bebidas.", CategoriaAlimentoId = 1, Calories = 304, KcalPor100g = 304, ProteinaGPor100g = 0.3m, HidratosGPor100g = 82m, GorduraGPor100g = 0m },
            new Alimento { Name = "Azeitonas", Description = "Azeitonas (pretas/verdes), usadas como complemento.", CategoriaAlimentoId = 2, Calories = 115, KcalPor100g = 115, ProteinaGPor100g = 0.8m, HidratosGPor100g = 6.3m, GorduraGPor100g = 10.7m },
            new Alimento { Name = "Couve", Description = "Folha verde muito usada em sopas.", CategoriaAlimentoId = 2, Calories = 25, KcalPor100g = 25, ProteinaGPor100g = 2.5m, HidratosGPor100g = 5.6m, GorduraGPor100g = 0.4m },
            new Alimento { Name = "Bacalhau", Description = "Peixe (bacalhau) usado em pratos tradicionais.", CategoriaAlimentoId = 5, Calories = 105, KcalPor100g = 105, ProteinaGPor100g = 23m, HidratosGPor100g = 0m, GorduraGPor100g = 0.9m },
            new Alimento { Name = "Batata Palha", Description = "Batata frita em palha, usada em pratos como bacalhau à brás.", CategoriaAlimentoId = 3, Calories = 550, KcalPor100g = 550, ProteinaGPor100g = 6m, HidratosGPor100g = 50m, GorduraGPor100g = 35m },
            new Alimento { Name = "Chouriço", Description = "Enchido tradicional usado em sopas e feijoadas.", CategoriaAlimentoId = 5, Calories = 350, KcalPor100g = 350, ProteinaGPor100g = 17m, HidratosGPor100g = 2m, GorduraGPor100g = 30m },
            new Alimento { Name = "Bacon", Description = "Carne curada, usada como ingrediente em feijoadas.", CategoriaAlimentoId = 5, Calories = 541, KcalPor100g = 541, ProteinaGPor100g = 37m, HidratosGPor100g = 1.4m, GorduraGPor100g = 42m },
            new Alimento { Name = "Cogumelos", Description = "Cogumelos frescos, usados em risottos e molhos.", CategoriaAlimentoId = 2, Calories = 22, KcalPor100g = 22, ProteinaGPor100g = 3.1m, HidratosGPor100g = 3.3m, GorduraGPor100g = 0.3m },
            new Alimento { Name = "Manteiga", Description = "Gordura láctea usada em confeitaria e risottos.", CategoriaAlimentoId = 4, Calories = 717, KcalPor100g = 717, ProteinaGPor100g = 0.9m, HidratosGPor100g = 0.1m, GorduraGPor100g = 81m },
            new Alimento { Name = "Coentros", Description = "Erva aromática (coentro) usada para finalizar pratos.", CategoriaAlimentoId = 2, Calories = 23, KcalPor100g = 23, ProteinaGPor100g = 2.1m, HidratosGPor100g = 3.7m, GorduraGPor100g = 0.5m },
            new Alimento { Name = "Massa de Lasanha", Description = "Massa para lasanha (seca).", CategoriaAlimentoId = 3, Calories = 371, KcalPor100g = 371, ProteinaGPor100g = 13m, HidratosGPor100g = 75m, GorduraGPor100g = 1.5m },
            new Alimento { Name = "Queijo Parmesão", Description = "Queijo curado usado para finalizar pratos.", CategoriaAlimentoId = 4, Calories = 431, KcalPor100g = 431, ProteinaGPor100g = 38m, HidratosGPor100g = 4m, GorduraGPor100g = 29m },
            new Alimento { Name = "Farinha de Trigo", Description = "Farinha usada em massas e empadões.", CategoriaAlimentoId = 3, Calories = 364, KcalPor100g = 364, ProteinaGPor100g = 10m, HidratosGPor100g = 76m, GorduraGPor100g = 1m },
            new Alimento { Name = "Leite Condensado", Description = "Produto lácteo adoçado para sobremesas.", CategoriaAlimentoId = 4, Calories = 321, KcalPor100g = 321, ProteinaGPor100g = 7.9m, HidratosGPor100g = 55m, GorduraGPor100g = 8.7m },
            new Alimento { Name = "Açúcar", Description = "Açúcar branco para confeitaria.", CategoriaAlimentoId = 3, Calories = 387, KcalPor100g = 387, ProteinaGPor100g = 0m, HidratosGPor100g = 100m, GorduraGPor100g = 0m },
            new Alimento { Name = "Polvo", Description = "Molusco (polvo) usado em pratos tradicionais.", CategoriaAlimentoId = 5, Calories = 82, KcalPor100g = 82, ProteinaGPor100g = 15m, HidratosGPor100g = 2.2m, GorduraGPor100g = 1m },

            // Alimentos adicionais para substituições realistas
            new Alimento { Name = "Bebida de Soja", Description = "Bebida vegetal usada como alternativa ao leite.", CategoriaAlimentoId = 4, Calories = 33, KcalPor100g = 33, ProteinaGPor100g = 3m, HidratosGPor100g = 1.7m, GorduraGPor100g = 1.9m },
            new Alimento { Name = "Iogurte Sem Lactose", Description = "Iogurte sem lactose (alternativa a laticínios).", CategoriaAlimentoId = 4, Calories = 60, KcalPor100g = 60, ProteinaGPor100g = 4m, HidratosGPor100g = 6m, GorduraGPor100g = 2m },
            new Alimento { Name = "Pão Sem Glúten", Description = "Pão preparado sem glúten.", CategoriaAlimentoId = 3, Calories = 250, KcalPor100g = 250, ProteinaGPor100g = 5m, HidratosGPor100g = 50m, GorduraGPor100g = 3m },
            new Alimento { Name = "Farinha de Arroz", Description = "Farinha alternativa, naturalmente sem glúten.", CategoriaAlimentoId = 3, Calories = 366, KcalPor100g = 366, ProteinaGPor100g = 6m, HidratosGPor100g = 80m, GorduraGPor100g = 1m },
            new Alimento { Name = "Adoçante (Stevia)", Description = "Adoçante com baixo teor calórico.", CategoriaAlimentoId = 3, Calories = 0, KcalPor100g = 0, ProteinaGPor100g = 0m, HidratosGPor100g = 0m, GorduraGPor100g = 0m },
            new Alimento { Name = "Margarina Vegetal", Description = "Gordura vegetal usada como alternativa à manteiga.", CategoriaAlimentoId = 4, Calories = 717, KcalPor100g = 717, ProteinaGPor100g = 0m, HidratosGPor100g = 0m, GorduraGPor100g = 80m },
        };

        var existingNames = new HashSet<string>(
            context.Alimentos.Select(a => a.Name),
            StringComparer.OrdinalIgnoreCase);

        var toAdd = seedAlimentos.Where(a => !existingNames.Contains(a.Name)).ToList();
        if (toAdd.Count == 0) return;

        context.Alimentos.AddRange(toAdd);

        context.SaveChanges();
    }

    private static void PopulateAlergias(HealthWellbeingDbContext context)
    {
        if (context.Alergia.Any())
        {
            return;
        }

        var alergias = new List<Alergia>
    {
        new Alergia
        {
            Nome = "Alergia ao Amendoim",
            Descricao = "Reação alérgica grave a proteínas do amendoim.",
            Gravidade = GravidadeAlergia.Grave,
            Sintomas = "Urticária, dificuldade para respirar, inchaço na garganta."
        },
        new Alergia
        {
            Nome = "Alergia ao Leite",
            Descricao = "Sensibilidade às proteínas do leite de vaca.",
            Gravidade = GravidadeAlergia.Moderada,
            Sintomas = "Cólicas, diarreia, erupções cutâneas."
        },
        new Alergia
        {
            Nome = "Alergia ao Ovo",
            Descricao = "Reação imunológica às proteínas da clara ou gema do ovo.",
            Gravidade = GravidadeAlergia.Leve,
            Sintomas = "Coceira, vermelhidão, desconforto gastrointestinal."
        },
        new Alergia
        {
            Nome = "Alergia ao Trigo",
            Descricao = "Reação às proteínas do trigo, incluindo o glúten.",
            Gravidade = GravidadeAlergia.Moderada,
            Sintomas = "Inchaço abdominal, erupções cutâneas, fadiga."
        },
        new Alergia
        {
            Nome = "Alergia a Frutos do Mar",
            Descricao = "Reação a crustáceos e moluscos.",
            Gravidade = GravidadeAlergia.Grave,
            Sintomas = "Inchaço facial, vômitos, anafilaxia."
        },
        new Alergia
        {
            Nome = "Alergia a Castanhas",
            Descricao = "Reação a proteínas encontradas em castanhas, nozes e amêndoas.",
            Gravidade = GravidadeAlergia.Grave,
            Sintomas = "Prurido, inchaço, risco de anafilaxia."
        },
        new Alergia
        {
            Nome = "Alergia à Soja",
            Descricao = "Reação às proteínas encontradas nos grãos de soja.",
            Gravidade = GravidadeAlergia.Moderada,
            Sintomas = "Erupções, desconforto abdominal, tosse."
        },
        new Alergia
        {
            Nome = "Alergia ao Peixe",
            Descricao = "Reação imunológica a proteínas de peixes diversos.",
            Gravidade = GravidadeAlergia.Grave,
            Sintomas = "Inchaço facial, vômitos, dificuldade respiratória."
        },
        new Alergia
        {
            Nome = "Alergia ao Frango",
            Descricao = "Hipersensibilidade às proteínas da carne de frango.",
            Gravidade = GravidadeAlergia.Leve,
            Sintomas = "Vermelhidão, urticária, irritação na pele."
        },
        new Alergia
        {
            Nome = "Alergia ao Glúten",
            Descricao = "Reação não-celíaca às proteínas do glúten.",
            Gravidade = GravidadeAlergia.Moderada,
            Sintomas = "Dores abdominais, náuseas, fadiga."
        },
        new Alergia
        {
            Nome = "Alergia a Lacticínios",
            Descricao = "Reação a derivados do leite, mesmo quando sem lactose.",
            Gravidade = GravidadeAlergia.Moderada,
            Sintomas = "Inchaço, diarréia, irritação nasal."
        }
    };

        context.Alergia.AddRange(alergias);
        context.SaveChanges();

        // Associa os alimentos existentes às alergias
        var alimentos = context.Alimentos.ToList();

        int? TryGetAlimentoId(string nome)
        {
            return alimentos
                .FirstOrDefault(a => string.Equals(a.Name, nome, StringComparison.OrdinalIgnoreCase))
                ?.AlimentoId;
        }

        void AddAssociacao(int alergiaIndex, string alimentoNome)
        {
            var alimentoId = TryGetAlimentoId(alimentoNome);
            if (alimentoId == null) return;

            context.AlergiaAlimento.Add(new AlergiaAlimento
            {
                AlergiaId = alergias[alergiaIndex].AlergiaId,
                AlimentoId = alimentoId.Value
            });
        }

        // Mantém coerência: cada alergia liga a um (ou mais) alimentos reais seedados.
        AddAssociacao(0, "Amendoim");          // Alergia ao Amendoim
        AddAssociacao(1, "Leite");             // Alergia ao Leite
        AddAssociacao(2, "Ovo");               // Alergia ao Ovo
        AddAssociacao(3, "Trigo");             // Alergia ao Trigo
        AddAssociacao(4, "Camarão");           // Alergia a Frutos do Mar
        AddAssociacao(5, "Amêndoa");           // Alergia a Castanhas
        AddAssociacao(6, "Tofu");              // Alergia à Soja
        AddAssociacao(7, "Salmão");            // Alergia ao Peixe
        AddAssociacao(8, "Frango");            // Alergia ao Frango
        AddAssociacao(9, "Trigo");             // Alergia ao Glúten
        AddAssociacao(9, "Pão");               // Glúten (derivados)
        AddAssociacao(10, "Leite");            // Alergia a Lacticínios
        AddAssociacao(10, "Queijo");           // Lacticínios

        context.SaveChanges();
    }


    private static void PopulateRestricoesAlimentares(HealthWellbeingDbContext context)
    {
        if (context.RestricaoAlimentar.Any())
        {
            return;
        }

        var restricoes = new List<RestricaoAlimentar>
        {
            new RestricaoAlimentar 
            { 
                Nome = "Intolerância à Lactose", 
                Tipo = TipoRestricao.IntoleranciaLactose, 
                Gravidade = GravidadeRestricao.Moderada, 
                Descricao = "Dificuldade em digerir lactose presente em laticínios." 
            },
            new RestricaoAlimentar 
            { 
                Nome = "Dieta Vegana", 
                Tipo = TipoRestricao.Vegana, 
                Gravidade = GravidadeRestricao.Leve, 
                Descricao = "Exclusão total de produtos de origem animal." 
            },
            new RestricaoAlimentar 
            { 
                Nome = "Restrição de Sódio", 
                Tipo = TipoRestricao.BaixoSodio, 
                Gravidade = GravidadeRestricao.Moderada, 
                Descricao = "Controle rigoroso de consumo de sódio para hipertensos." 
            },
            new RestricaoAlimentar 
            { 
                Nome = "Doença Celíaca", 
                Tipo = TipoRestricao.IntoleranciaGluten, 
                Gravidade = GravidadeRestricao.Grave, 
                Descricao = "Intolerância permanente ao glúten. Pode causar danos intestinais." 
            },
            new RestricaoAlimentar 
            { 
                Nome = "Diabetes - Controle de Açúcar", 
                Tipo = TipoRestricao.SemAcucar, 
                Gravidade = GravidadeRestricao.Grave, 
                Descricao = "Restrição total de açúcares refinados e controle de carboidratos." 
            },
            new RestricaoAlimentar 
            { 
                Nome = "Vegetariana Estrita", 
                Tipo = TipoRestricao.Vegetariana, 
                Gravidade = GravidadeRestricao.Leve, 
                Descricao = "Dieta sem carne, mas pode incluir laticínios e ovos." 
            },
            new RestricaoAlimentar 
            { 
                Nome = "Alergia a Frutos do Mar", 
                Tipo = TipoRestricao.Outra, 
                Gravidade = GravidadeRestricao.Grave, 
                Descricao = "Alergia severa a crustáceos e moluscos." 
            },
            new RestricaoAlimentar 
            { 
                Nome = "Restrição Religiosa - Halal", 
                Tipo = TipoRestricao.Religiosa, 
                Gravidade = GravidadeRestricao.Moderada, 
                Descricao = "Alimentação de acordo com preceitos islâmicos." 
            },
            new RestricaoAlimentar 
            { 
                Nome = "Sensibilidade ao Glúten", 
                Tipo = TipoRestricao.IntoleranciaGluten, 
                Gravidade = GravidadeRestricao.Leve, 
                Descricao = "Sensibilidade não celíaca ao glúten." 
            },
            new RestricaoAlimentar 
            { 
                Nome = "Dieta Low Carb", 
                Tipo = TipoRestricao.SemAcucar, 
                Gravidade = GravidadeRestricao.Leve, 
                Descricao = "Redução de carboidratos e açúcares para controle de peso." 
            },
            new RestricaoAlimentar 
            { 
                Nome = "Alergia a Amendoim", 
                Tipo = TipoRestricao.Outra, 
                Gravidade = GravidadeRestricao.Grave, 
                Descricao = "Alergia severa a amendoim e derivados." 
            },
            new RestricaoAlimentar 
            { 
                Nome = "Restrição Religiosa - Kosher", 
                Tipo = TipoRestricao.Religiosa, 
                Gravidade = GravidadeRestricao.Moderada, 
                Descricao = "Alimentação de acordo com leis dietéticas judaicas." 
            },
            new RestricaoAlimentar 
            { 
                Nome = "Intolerância à Frutose", 
                Tipo = TipoRestricao.Outra, 
                Gravidade = GravidadeRestricao.Moderada, 
                Descricao = "Dificuldade em digerir frutose presente em frutas e mel." 
            },
            new RestricaoAlimentar 
            { 
                Nome = "Dieta Paleolítica", 
                Tipo = TipoRestricao.Outra, 
                Gravidade = GravidadeRestricao.Leve, 
                Descricao = "Baseada em alimentos supostamente disponíveis na era paleolítica." 
            },
            new RestricaoAlimentar 
            { 
                Nome = "Alergia a Ovos", 
                Tipo = TipoRestricao.Outra, 
                Gravidade = GravidadeRestricao.Moderada, 
                Descricao = "Alergia a proteínas presentes em ovos." 
            }
        };

        context.RestricaoAlimentar.AddRange(restricoes);
        context.SaveChanges();

        // Agora cria as associações N:N com alimentos
        PopulateRestricaoAlimentarAssociacoes(context, restricoes);
    }



    private static void PopulateRestricaoAlimentarAssociacoes(HealthWellbeingDbContext context, List<RestricaoAlimentar> restricoes)
    {
        var associacoes = new List<RestricaoAlimentarAlimento>();
        
        // Obtém todos os alimentos disponíveis
        var alimentos = context.Alimentos.ToList();
        
        if (!alimentos.Any()) return;

        // Intolerância à Lactose - associa com laticínios
        var lactose = restricoes.First(r => r.Nome == "Intolerância à Lactose");
        var laticinios = alimentos.Where(a => a.Name.Contains("Leite") || a.Name.Contains("Queijo")).Take(3);
        foreach (var alimento in laticinios)
        {
            associacoes.Add(new RestricaoAlimentarAlimento 
            { 
                RestricaoAlimentarId = lactose.RestricaoAlimentarId, 
                AlimentoId = alimento.AlimentoId 
            });
        }

        // Vegana - associa com carnes, ovos, laticínios
        var vegana = restricoes.First(r => r.Nome == "Dieta Vegana");
        var produtosAnimais = alimentos.Where(a => a.Name.Contains("Frango") || a.Name.Contains("Salmão") || a.Name.Contains("Queijo") || a.Name.Contains("Leite")).Take(4);
        foreach (var alimento in produtosAnimais)
        {
            associacoes.Add(new RestricaoAlimentarAlimento 
            { 
                RestricaoAlimentarId = vegana.RestricaoAlimentarId, 
                AlimentoId = alimento.AlimentoId 
            });
        }

        // Baixo Sódio - associa com alimentos processados
        var baixoSodio = restricoes.First(r => r.Nome == "Restrição de Sódio");
        var alimentosProcessados = alimentos.Where(a => a.Name.Contains("Queijo") || a.Name.Contains("Pão")).Take(2);
        foreach (var alimento in alimentosProcessados)
        {
            associacoes.Add(new RestricaoAlimentarAlimento 
            { 
                RestricaoAlimentarId = baixoSodio.RestricaoAlimentarId, 
                AlimentoId = alimento.AlimentoId 
            });
        }

        // Doença Celíaca - associa com trigo, pão
        var celiaca = restricoes.First(r => r.Nome == "Doença Celíaca");
        var comGluten = alimentos.Where(a => a.Name.Contains("Pão") || a.Name.Contains("Aveia")).Take(3);
        foreach (var alimento in comGluten)
        {
            associacoes.Add(new RestricaoAlimentarAlimento 
            { 
                RestricaoAlimentarId = celiaca.RestricaoAlimentarId, 
                AlimentoId = alimento.AlimentoId 
            });
        }

        // Diabetes - associa com açúcares e carboidratos
        var diabetes = restricoes.First(r => r.Nome == "Diabetes - Controle de Açúcar");
        var acucarados = alimentos.Where(a => a.Name.Contains("Banana") || a.Name.Contains("Batata Doce") || a.Name.Contains("Arroz")).Take(3);
        foreach (var alimento in acucarados)
        {
            associacoes.Add(new RestricaoAlimentarAlimento 
            { 
                RestricaoAlimentarId = diabetes.RestricaoAlimentarId, 
                AlimentoId = alimento.AlimentoId 
            });
        }

        // Alergia a Frutos do Mar
        var frutosMar = restricoes.First(r => r.Nome == "Alergia a Frutos do Mar");
        var frutosMarAlimentos = alimentos.Where(a => a.Name.Contains("Camarão")).Take(1);
        foreach (var alimento in frutosMarAlimentos)
        {
            associacoes.Add(new RestricaoAlimentarAlimento 
            { 
                RestricaoAlimentarId = frutosMar.RestricaoAlimentarId, 
                AlimentoId = alimento.AlimentoId 
            });
        }

        // Alergia a Amendoim
        var amendoim = restricoes.First(r => r.Nome == "Alergia a Amendoim");
        var amendoimAlimentos = alimentos.Where(a => a.Name.Contains("Amendoim")).Take(1);
        foreach (var alimento in amendoimAlimentos)
        {
            associacoes.Add(new RestricaoAlimentarAlimento 
            { 
                RestricaoAlimentarId = amendoim.RestricaoAlimentarId, 
                AlimentoId = alimento.AlimentoId 
            });
        }

        // Alergia a Ovos
        var ovos = restricoes.First(r => r.Nome == "Alergia a Ovos");
        var ovosAlimentos = alimentos.Where(a => a.Name.Contains("Ovo")).Take(1);
        foreach (var alimento in ovosAlimentos)
        {
            associacoes.Add(new RestricaoAlimentarAlimento 
            { 
                RestricaoAlimentarId = ovos.RestricaoAlimentarId, 
                AlimentoId = alimento.AlimentoId 
            });
        }

        // Intolerância à Frutose - associa com frutas doces
        var frutose = restricoes.First(r => r.Nome == "Intolerância à Frutose");
        var frutasDoces = alimentos.Where(a => a.Name.Contains("Maçã") || a.Name.Contains("Banana") || a.Name.Contains("Batata Doce")).Take(3);
        foreach (var alimento in frutasDoces)
        {
            associacoes.Add(new RestricaoAlimentarAlimento 
            { 
                RestricaoAlimentarId = frutose.RestricaoAlimentarId, 
                AlimentoId = alimento.AlimentoId 
            });
        }

        // Adiciona algumas associações aleatórias para as restrições restantes
        var random = new Random();
        var restricoesSemAssociacao = restricoes.Except(new[] { lactose, vegana, baixoSodio, celiaca, diabetes, frutosMar, amendoim, ovos, frutose });

        foreach (var restricao in restricoesSemAssociacao)
        {
            var alimentosAleatorios = alimentos.OrderBy(x => random.Next()).Take(2);
            foreach (var alimento in alimentosAleatorios)
            {
                associacoes.Add(new RestricaoAlimentarAlimento 
                { 
                    RestricaoAlimentarId = restricao.RestricaoAlimentarId, 
                    AlimentoId = alimento.AlimentoId 
                });
            }
        }

        context.RestricaoAlimentarAlimento.AddRange(associacoes);
        context.SaveChanges();
    }



    private static void PopulateAlimentoSubstitutos(HealthWellbeingDbContext context)
    {
        if (context.AlimentoSubstitutos.Any())
        {
            return;
        }

        var alimentosByNome = context.Alimentos
            .AsNoTracking()
            .ToList()
            .GroupBy(a => a.Name, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

        int GetAlimentoId(string nome)
        {
            if (!alimentosByNome.TryGetValue(nome, out var alimento))
                throw new InvalidOperationException($"Alimento não encontrado para seed de substitutos: '{nome}'.");
            return alimento.AlimentoId;
        }

        var subs = new List<AlimentoSubstituto>
        {
            // Lactose / laticínios
            new AlimentoSubstituto { AlimentoOriginalId = GetAlimentoId("Leite"), AlimentoSubstitutoRefId = GetAlimentoId("Leite de Amêndoa"), Motivo = "Alternativa sem lactose.", ProporcaoEquivalente = 1m, Observacoes = "Boa para bebidas, cereais e algumas receitas.", FatorSimilaridade = 0.75 },
            new AlimentoSubstituto { AlimentoOriginalId = GetAlimentoId("Leite"), AlimentoSubstitutoRefId = GetAlimentoId("Bebida de Soja"), Motivo = "Alternativa vegetal.", ProporcaoEquivalente = 1m, Observacoes = "Boa para molhos e smoothies.", FatorSimilaridade = 0.7 },
            new AlimentoSubstituto { AlimentoOriginalId = GetAlimentoId("Queijo"), AlimentoSubstitutoRefId = GetAlimentoId("Tofu"), Motivo = "Alternativa para dietas sem lactose/veganas.", ProporcaoEquivalente = 1m, Observacoes = "Textura diferente; ajustar temperos.", FatorSimilaridade = 0.45 },
            new AlimentoSubstituto { AlimentoOriginalId = GetAlimentoId("Manteiga"), AlimentoSubstitutoRefId = GetAlimentoId("Azeite"), Motivo = "Alternativa sem lactose para cozinhar.", ProporcaoEquivalente = 0.75m, Observacoes = "Usar menos quantidade e ajustar ao gosto.", FatorSimilaridade = 0.6 },
            new AlimentoSubstituto { AlimentoOriginalId = GetAlimentoId("Manteiga"), AlimentoSubstitutoRefId = GetAlimentoId("Margarina Vegetal"), Motivo = "Alternativa vegetal.", ProporcaoEquivalente = 1m, Observacoes = "Boa para barrar e algumas receitas.", FatorSimilaridade = 0.65 },

            // Glúten
            new AlimentoSubstituto { AlimentoOriginalId = GetAlimentoId("Pão"), AlimentoSubstitutoRefId = GetAlimentoId("Pão Sem Glúten"), Motivo = "Alternativa para intolerância ao glúten.", ProporcaoEquivalente = 1m, Observacoes = "Sabor/textura podem variar.", FatorSimilaridade = 0.8 },
            new AlimentoSubstituto { AlimentoOriginalId = GetAlimentoId("Farinha de Trigo"), AlimentoSubstitutoRefId = GetAlimentoId("Farinha de Arroz"), Motivo = "Alternativa para receitas sem glúten.", ProporcaoEquivalente = 1m, Observacoes = "Pode precisar de ajuste de líquidos.", FatorSimilaridade = 0.7 },
            new AlimentoSubstituto { AlimentoOriginalId = GetAlimentoId("Trigo"), AlimentoSubstitutoRefId = GetAlimentoId("Arroz"), Motivo = "Alternativa de grão sem glúten.", ProporcaoEquivalente = 1m, Observacoes = "Perfil nutricional diferente.", FatorSimilaridade = 0.5 },

            // Açúcar
            new AlimentoSubstituto { AlimentoOriginalId = GetAlimentoId("Açúcar"), AlimentoSubstitutoRefId = GetAlimentoId("Adoçante (Stevia)"), Motivo = "Alternativa com baixo teor calórico.", ProporcaoEquivalente = 0.2m, Observacoes = "Ajustar ao paladar e à receita.", FatorSimilaridade = 0.55 },

            // Proteína animal ↔ vegetal (útil para vegetariano/vegano)
            new AlimentoSubstituto { AlimentoOriginalId = GetAlimentoId("Frango"), AlimentoSubstitutoRefId = GetAlimentoId("Tofu"), Motivo = "Alternativa proteica vegetal.", ProporcaoEquivalente = 1m, Observacoes = "Ideal em salteados e pratos com molho.", FatorSimilaridade = 0.55 },
        };

        context.AlimentoSubstitutos.AddRange(subs);
        context.SaveChanges();
    }

    private static void PopulateComponentesReceita(HealthWellbeingDbContext context)
    {
        var receitas = context.Receita
            .AsNoTracking()
            .ToList();
        if (!receitas.Any())
        {
            return;
        }

        var alimentos = context.Alimentos
            .AsNoTracking()
            .ToList();
        if (!alimentos.Any())
        {
            return;
        }

        var receitasByNome = receitas
            .GroupBy(r => r.Nome, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

        var alimentosByNome = alimentos
            .GroupBy(a => a.Name, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

        int GetReceitaId(string receitaNome)
        {
            if (!receitasByNome.TryGetValue(receitaNome, out var receita))
                throw new InvalidOperationException($"Receita não encontrada para seed: '{receitaNome}'.");
            return receita.ReceitaId;
        }

        int GetAlimentoId(string alimentoNome)
        {
            if (!alimentosByNome.TryGetValue(alimentoNome, out var alimento))
                throw new InvalidOperationException($"Alimento não encontrado para seed: '{alimentoNome}'.");
            return alimento.AlimentoId;
        }

        var receitaNomesSeed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Salada de Frutas Tropical",
            "Vitamina de Banana com Aveia",
            "Omelete de Legumes",
            "Sanduíche Natural de Frango",
            "Sopa de Legumes Caseira",
            "Arroz Integral com Feijão",
            "Frango Grelhado com Ervas",
            "Bacalhau à Brás",
            "Batata Doce Assada",
            "Caldo Verde",
            "Arroz de Frango",
            "Lasanha de Legumes",
            "Salmão Assado com Legumes",
            "Feijoada Completa",
            "Risotto de Cogumelos",
            "Empadão de Frango",
            "Açorda de Marisco",
            "Pudim de Leite Condensado",
            "Polvo à Lagareiro",
        };

        var novos = new List<ComponenteReceita>();

        void Add(string receitaNome, string alimentoNome, UnidadeMedidaEnum unidade, int quantidade, bool opcional = false)
        {
            if (!receitaNomesSeed.Contains(receitaNome))
                return;

            novos.Add(new ComponenteReceita
            {
                ReceitaId = GetReceitaId(receitaNome),
                AlimentoId = GetAlimentoId(alimentoNome),
                UnidadeMedida = unidade,
                Quantidade = quantidade,
                IsOpcional = opcional
            });
        }

        // Salada de Frutas Tropical
        Add("Salada de Frutas Tropical", "Maçã", UnidadeMedidaEnum.Grama, 150);
        Add("Salada de Frutas Tropical", "Banana", UnidadeMedidaEnum.Unidade, 1);
        Add("Salada de Frutas Tropical", "Laranja", UnidadeMedidaEnum.Unidade, 1, opcional: true);
        Add("Salada de Frutas Tropical", "Mel", UnidadeMedidaEnum.ColherDeSopa, 1, opcional: true);

        // Vitamina de Banana com Aveia
        Add("Vitamina de Banana com Aveia", "Banana", UnidadeMedidaEnum.Unidade, 1);
        Add("Vitamina de Banana com Aveia", "Leite", UnidadeMedidaEnum.Mililitro, 250);
        Add("Vitamina de Banana com Aveia", "Aveia", UnidadeMedidaEnum.ColherDeSopa, 3);
        Add("Vitamina de Banana com Aveia", "Mel", UnidadeMedidaEnum.ColherDeSopa, 1, opcional: true);

        // Omelete de Legumes
        Add("Omelete de Legumes", "Ovo", UnidadeMedidaEnum.Unidade, 2);
        Add("Omelete de Legumes", "Cenoura", UnidadeMedidaEnum.Grama, 50);
        Add("Omelete de Legumes", "Cebola", UnidadeMedidaEnum.Grama, 30, opcional: true);
        Add("Omelete de Legumes", "Azeite", UnidadeMedidaEnum.ColherDeCha, 1, opcional: true);

        // Sanduíche Natural de Frango
        Add("Sanduíche Natural de Frango", "Frango", UnidadeMedidaEnum.Grama, 120);
        Add("Sanduíche Natural de Frango", "Pão", UnidadeMedidaEnum.Fatia, 2);
        Add("Sanduíche Natural de Frango", "Alface", UnidadeMedidaEnum.Grama, 30, opcional: true);
        Add("Sanduíche Natural de Frango", "Tomate", UnidadeMedidaEnum.Grama, 60, opcional: true);

        // Sopa de Legumes Caseira
        Add("Sopa de Legumes Caseira", "Batata", UnidadeMedidaEnum.Grama, 200);
        Add("Sopa de Legumes Caseira", "Cenoura", UnidadeMedidaEnum.Grama, 100);
        Add("Sopa de Legumes Caseira", "Cebola", UnidadeMedidaEnum.Grama, 60);
        Add("Sopa de Legumes Caseira", "Alho", UnidadeMedidaEnum.Unidade, 2, opcional: true);
        Add("Sopa de Legumes Caseira", "Azeite", UnidadeMedidaEnum.ColherDeSopa, 1, opcional: true);

        // Arroz Integral com Feijão
        Add("Arroz Integral com Feijão", "Arroz", UnidadeMedidaEnum.Xicara, 1);
        Add("Arroz Integral com Feijão", "Feijão", UnidadeMedidaEnum.Xicara, 1);
        Add("Arroz Integral com Feijão", "Cebola", UnidadeMedidaEnum.Grama, 50, opcional: true);
        Add("Arroz Integral com Feijão", "Alho", UnidadeMedidaEnum.Unidade, 2, opcional: true);

        // Frango Grelhado com Ervas
        Add("Frango Grelhado com Ervas", "Frango", UnidadeMedidaEnum.Grama, 200);
        Add("Frango Grelhado com Ervas", "Alho", UnidadeMedidaEnum.Unidade, 2, opcional: true);
        Add("Frango Grelhado com Ervas", "Azeite", UnidadeMedidaEnum.ColherDeSopa, 1, opcional: true);
        Add("Frango Grelhado com Ervas", "Limão", UnidadeMedidaEnum.Unidade, 1, opcional: true);

        // Bacalhau à Brás
        Add("Bacalhau à Brás", "Bacalhau", UnidadeMedidaEnum.Grama, 300);
        Add("Bacalhau à Brás", "Batata Palha", UnidadeMedidaEnum.Grama, 200);
        Add("Bacalhau à Brás", "Ovo", UnidadeMedidaEnum.Unidade, 4);
        Add("Bacalhau à Brás", "Cebola", UnidadeMedidaEnum.Grama, 120);
        Add("Bacalhau à Brás", "Alho", UnidadeMedidaEnum.Unidade, 2, opcional: true);
        Add("Bacalhau à Brás", "Azeitonas", UnidadeMedidaEnum.Grama, 30, opcional: true);
        Add("Bacalhau à Brás", "Azeite", UnidadeMedidaEnum.ColherDeSopa, 2, opcional: true);

        // Batata Doce Assada
        Add("Batata Doce Assada", "Batata Doce", UnidadeMedidaEnum.Grama, 300);
        Add("Batata Doce Assada", "Azeite", UnidadeMedidaEnum.ColherDeSopa, 1, opcional: true);

        // Caldo Verde
        Add("Caldo Verde", "Batata", UnidadeMedidaEnum.Grama, 400);
        Add("Caldo Verde", "Couve", UnidadeMedidaEnum.Grama, 200);
        Add("Caldo Verde", "Chouriço", UnidadeMedidaEnum.Grama, 80, opcional: true);
        Add("Caldo Verde", "Azeite", UnidadeMedidaEnum.ColherDeSopa, 1, opcional: true);

        // Arroz de Frango
        Add("Arroz de Frango", "Arroz", UnidadeMedidaEnum.Xicara, 2);
        Add("Arroz de Frango", "Frango", UnidadeMedidaEnum.Grama, 300);
        Add("Arroz de Frango", "Tomate", UnidadeMedidaEnum.Grama, 200, opcional: true);
        Add("Arroz de Frango", "Cebola", UnidadeMedidaEnum.Grama, 100, opcional: true);
        Add("Arroz de Frango", "Alho", UnidadeMedidaEnum.Unidade, 2, opcional: true);

        // Lasanha de Legumes
        Add("Lasanha de Legumes", "Massa de Lasanha", UnidadeMedidaEnum.Grama, 250);
        Add("Lasanha de Legumes", "Tomate", UnidadeMedidaEnum.Grama, 300);
        Add("Lasanha de Legumes", "Cenoura", UnidadeMedidaEnum.Grama, 150, opcional: true);
        Add("Lasanha de Legumes", "Cogumelos", UnidadeMedidaEnum.Grama, 200, opcional: true);
        Add("Lasanha de Legumes", "Queijo", UnidadeMedidaEnum.Grama, 200, opcional: true);

        // Salmão Assado com Legumes
        Add("Salmão Assado com Legumes", "Salmão", UnidadeMedidaEnum.Grama, 250);
        Add("Salmão Assado com Legumes", "Batata", UnidadeMedidaEnum.Grama, 200, opcional: true);
        Add("Salmão Assado com Legumes", "Cenoura", UnidadeMedidaEnum.Grama, 100, opcional: true);
        Add("Salmão Assado com Legumes", "Limão", UnidadeMedidaEnum.Unidade, 1, opcional: true);
        Add("Salmão Assado com Legumes", "Azeite", UnidadeMedidaEnum.ColherDeSopa, 1, opcional: true);

        // Feijoada Completa
        Add("Feijoada Completa", "Feijão", UnidadeMedidaEnum.Xicara, 2);
        Add("Feijoada Completa", "Arroz", UnidadeMedidaEnum.Xicara, 2, opcional: true);
        Add("Feijoada Completa", "Bacon", UnidadeMedidaEnum.Grama, 150, opcional: true);
        Add("Feijoada Completa", "Chouriço", UnidadeMedidaEnum.Grama, 200, opcional: true);
        Add("Feijoada Completa", "Couve", UnidadeMedidaEnum.Grama, 200, opcional: true);
        Add("Feijoada Completa", "Laranja", UnidadeMedidaEnum.Unidade, 1, opcional: true);

        // Risotto de Cogumelos
        Add("Risotto de Cogumelos", "Arroz", UnidadeMedidaEnum.Xicara, 2);
        Add("Risotto de Cogumelos", "Cogumelos", UnidadeMedidaEnum.Grama, 300);
        Add("Risotto de Cogumelos", "Cebola", UnidadeMedidaEnum.Grama, 80, opcional: true);
        Add("Risotto de Cogumelos", "Manteiga", UnidadeMedidaEnum.Grama, 30, opcional: true);
        Add("Risotto de Cogumelos", "Queijo Parmesão", UnidadeMedidaEnum.Grama, 50, opcional: true);

        // Empadão de Frango
        Add("Empadão de Frango", "Frango", UnidadeMedidaEnum.Grama, 300);
        Add("Empadão de Frango", "Farinha de Trigo", UnidadeMedidaEnum.Grama, 300);
        Add("Empadão de Frango", "Manteiga", UnidadeMedidaEnum.Grama, 150, opcional: true);
        Add("Empadão de Frango", "Ovo", UnidadeMedidaEnum.Unidade, 2);
        Add("Empadão de Frango", "Azeitonas", UnidadeMedidaEnum.Grama, 50, opcional: true);

        // Açorda de Marisco
        Add("Açorda de Marisco", "Pão", UnidadeMedidaEnum.Grama, 200);
        Add("Açorda de Marisco", "Camarão", UnidadeMedidaEnum.Grama, 300);
        Add("Açorda de Marisco", "Alho", UnidadeMedidaEnum.Unidade, 2, opcional: true);
        Add("Açorda de Marisco", "Coentros", UnidadeMedidaEnum.Grama, 10, opcional: true);
        Add("Açorda de Marisco", "Azeite", UnidadeMedidaEnum.ColherDeSopa, 2, opcional: true);
        Add("Açorda de Marisco", "Ovo", UnidadeMedidaEnum.Unidade, 2, opcional: true);

        // Pudim de Leite Condensado
        Add("Pudim de Leite Condensado", "Leite Condensado", UnidadeMedidaEnum.Grama, 395);
        Add("Pudim de Leite Condensado", "Leite", UnidadeMedidaEnum.Mililitro, 500);
        Add("Pudim de Leite Condensado", "Ovo", UnidadeMedidaEnum.Unidade, 4);
        Add("Pudim de Leite Condensado", "Açúcar", UnidadeMedidaEnum.Grama, 120, opcional: true);

        // Polvo à Lagareiro
        Add("Polvo à Lagareiro", "Polvo", UnidadeMedidaEnum.Grama, 800);
        Add("Polvo à Lagareiro", "Batata", UnidadeMedidaEnum.Grama, 500);
        Add("Polvo à Lagareiro", "Alho", UnidadeMedidaEnum.Unidade, 4, opcional: true);
        Add("Polvo à Lagareiro", "Azeite", UnidadeMedidaEnum.ColherDeSopa, 4, opcional: true);

        if (novos.Count == 0)
        {
            return;
        }

        var receitaIdsSeed = novos
            .Select(c => c.ReceitaId)
            .ToHashSet();

        var existentes = context.ComponenteReceita
            .Where(c => receitaIdsSeed.Contains(c.ReceitaId))
            .ToList();

        var desiredSet = novos
            .Select(c => (c.ReceitaId, c.AlimentoId, c.UnidadeMedida, c.Quantidade, c.IsOpcional))
            .ToHashSet();

        var existingSet = existentes
            .Select(c => (c.ReceitaId, c.AlimentoId, c.UnidadeMedida, c.Quantidade, c.IsOpcional))
            .ToHashSet();

        if (existingSet.SetEquals(desiredSet))
        {
            return;
        }

        if (existentes.Count > 0)
        {
            context.ComponenteReceita.RemoveRange(existentes);
        }

        context.ComponenteReceita.AddRange(novos);
        context.SaveChanges();
    }

    private static void PopulateReceitas(HealthWellbeingDbContext context)
    {
        var receitasSeed = new List<Receita>
        {
            // Receitas Rápidas (5-15 minutos)
            new Receita
            {
                Nome = "Salada de Frutas Tropical",
                Descricao = "Salada refrescante com frutas tropicais e mel.",
                ModoPreparo = "Corte as frutas em cubos. Misture numa tigela grande. Adicione mel a gosto e sirva gelada.",
                TempoPreparo = 10,
                Porcoes = 2,
                Calorias = 150,
                Proteinas = 2,
                HidratosCarbono = 35,
                Gorduras = 1,
                RestricoesAlimentarId = new List<int> { 2 }
            },
            new Receita
            {
                Nome = "Vitamina de Banana com Aveia",
                Descricao = "Bebida saudável e energética para o pequeno-almoço.",
                ModoPreparo = "Bata a banana com leite no liquidificador. Adicione aveia e mel. Sirva imediatamente.",
                TempoPreparo = 5,
                Porcoes = 1,
                Calorias = 120,
                Proteinas = 5,
                HidratosCarbono = 25,
                Gorduras = 2
            },
            new Receita
            {
                Nome = "Omelete de Legumes",
                Descricao = "Omelete nutritiva com vegetais frescos.",
                ModoPreparo = "Bata os ovos. Adicione cenoura ralada e cebola picada. Frite em frigideira antiaderente.",
                TempoPreparo = 15,
                Porcoes = 1,
                Calorias = 180,
                Proteinas = 12,
                HidratosCarbono = 8,
                Gorduras = 10
            },
            new Receita
            {
                Nome = "Sanduíche Natural de Frango",
                Descricao = "Sanduíche leve e saboroso para lanches.",
                ModoPreparo = "Desfie o frango cozido. Misture com maionese light. Monte no pão integral com alface e tomate.",
                TempoPreparo = 10,
                Porcoes = 1,
                Calorias = 280,
                Proteinas = 25,
                HidratosCarbono = 32,
                Gorduras = 8
            },

            // Receitas Médias (20-35 minutos)
            new Receita
            {
                Nome = "Sopa de Legumes Caseira",
                Descricao = "Sopa nutritiva e reconfortante com vegetais frescos.",
                ModoPreparo = "Refogue cebola e alho. Adicione batata, cenoura e outros legumes picados. Cubra com água e cozinhe por 20 minutos. Bata no liquidificador se desejar.",
                TempoPreparo = 30,
                Porcoes = 4,
                Calorias = 100,
                Proteinas = 3,
                HidratosCarbono = 20,
                Gorduras = 2,
                RestricoesAlimentarId = new List<int> { 4 }
            },
            new Receita
            {
                Nome = "Arroz Integral com Feijão",
                Descricao = "Prato clássico português, nutritivo e saboroso.",
                ModoPreparo = "Cozinhe o arroz integral em água com sal. Prepare o feijão com cebola, alho e louro. Sirva junto.",
                TempoPreparo = 25,
                Porcoes = 3,
                Calorias = 250,
                Proteinas = 8,
                HidratosCarbono = 45,
                Gorduras = 3
            },
            new Receita
            {
                Nome = "Frango Grelhado com Ervas",
                Descricao = "Peito de frango grelhado temperado com ervas aromáticas.",
                ModoPreparo = "Tempere o frango com sal, pimenta, alecrim e alho. Grelhe em frigideira ou grelhador até dourar. Sirva com salada.",
                TempoPreparo = 20,
                Porcoes = 2,
                Calorias = 200,
                Proteinas = 35,
                HidratosCarbono = 0,
                Gorduras = 5
            },
            new Receita
            {
                Nome = "Bacalhau à Brás",
                Descricao = "Prato tradicional português com bacalhau desfiado.",
                ModoPreparo = "Demolhe o bacalhau. Refogue cebola e alho, adicione batata palha e bacalhau desfiado. Misture ovos batidos e finalize com azeitonas.",
                TempoPreparo = 35,
                Porcoes = 4,
                Calorias = 320,
                Proteinas = 28,
                HidratosCarbono = 22,
                Gorduras = 14
            },
            new Receita
            {
                Nome = "Batata Doce Assada",
                Descricao = "Acompanhamento saudável e rico em fibras.",
                ModoPreparo = "Lave as batatas doces. Corte ao meio e tempere com azeite e sal. Asse no forno a 200°C por 30 minutos.",
                TempoPreparo = 30,
                Porcoes = 2,
                Calorias = 150,
                Proteinas = 3,
                HidratosCarbono = 32,
                Gorduras = 2
            },

            // Receitas Elaboradas (40+ minutos)
            new Receita
            {
                Nome = "Caldo Verde",
                Descricao = "Sopa tradicional portuguesa com couve e chouriço.",
                ModoPreparo = "Cozinhe as batatas até ficarem macias. Esmague-as. Adicione a couve cortada finamente e o chouriço em rodelas. Cozinhe por mais 10 minutos.",
                TempoPreparo = 40,
                Porcoes = 6,
                Calorias = 180,
                Proteinas = 8,
                HidratosCarbono = 24,
                Gorduras = 6
            },
            new Receita
            {
                Nome = "Arroz de Frango",
                Descricao = "Arroz cremoso com pedaços de frango suculentos.",
                ModoPreparo = "Refogue o frango em pedaços. Adicione cebola, alho e tomate. Junte o arroz e caldo de galinha. Cozinhe até ficar cremoso.",
                TempoPreparo = 45,
                Porcoes = 4,
                Calorias = 280,
                Proteinas = 22,
                HidratosCarbono = 38,
                Gorduras = 6
            },
            new Receita
            {
                Nome = "Lasanha de Legumes",
                Descricao = "Lasanha vegetariana com camadas de legumes e queijo.",
                ModoPreparo = "Prepare o molho de tomate com legumes. Monte camadas alternando massa, molho, legumes e queijo. Asse no forno por 40 minutos a 180°C.",
                TempoPreparo = 60,
                Porcoes = 6,
                Calorias = 320,
                Proteinas = 15,
                HidratosCarbono = 42,
                Gorduras = 12
            },
            new Receita
            {
                Nome = "Salmão Assado com Legumes",
                Descricao = "Prato sofisticado e saudável, rico em ômega-3.",
                ModoPreparo = "Tempere o salmão com limão, sal e ervas. Disponha numa assadeira com legumes ao redor. Regue com azeite e asse a 200°C por 25 minutos.",
                TempoPreparo = 35,
                Porcoes = 2,
                Calorias = 350,
                Proteinas = 32,
                HidratosCarbono = 15,
                Gorduras = 18
            },
            new Receita
            {
                Nome = "Feijoada Completa",
                Descricao = "Prato tradicional brasileiro rico e saboroso.",
                ModoPreparo = "Cozinhe o feijão preto com carnes de porco. Adicione linguiça, bacon e temperos. Sirva com arroz, couve e laranja.",
                TempoPreparo = 90,
                Porcoes = 8,
                Calorias = 420,
                Proteinas = 28,
                HidratosCarbono = 45,
                Gorduras = 16
            },
            new Receita
            {
                Nome = "Risotto de Cogumelos",
                Descricao = "Arroz cremoso italiano com cogumelos frescos.",
                ModoPreparo = "Refogue cebola em manteiga. Adicione o arroz e vá juntando caldo aos poucos, mexendo sempre. Acrescente cogumelos salteados e queijo parmesão.",
                TempoPreparo = 40,
                Porcoes = 4,
                Calorias = 310,
                Proteinas = 10,
                HidratosCarbono = 48,
                Gorduras = 9
            },
            new Receita
            {
                Nome = "Empadão de Frango",
                Descricao = "Torta portuguesa recheada com frango desfiado.",
                ModoPreparo = "Prepare a massa com farinha, manteiga e ovos. Faça o recheio com frango, azeitonas e ovo cozido. Monte e asse por 45 minutos.",
                TempoPreparo = 60,
                Porcoes = 8,
                Calorias = 380,
                Proteinas = 18,
                HidratosCarbono = 35,
                Gorduras = 20
            },
            new Receita
            {
                Nome = "Açorda de Marisco",
                Descricao = "Prato alentejano com pão e marisco.",
                ModoPreparo = "Cozinhe o marisco. Prepare uma base com pão, alho, coentros e azeite. Adicione o marisco e ovos escalfados.",
                TempoPreparo = 50,
                Porcoes = 4,
                Calorias = 290,
                Proteinas = 24,
                HidratosCarbono = 28,
                Gorduras = 10
            },
            new Receita
            {
                Nome = "Pudim de Leite Condensado",
                Descricao = "Sobremesa clássica portuguesa cremosa e doce.",
                ModoPreparo = "Caramelize uma forma com açúcar. Bata leite condensado, leite e ovos. Despeje na forma e cozinhe em banho-maria por 50 minutos.",
                TempoPreparo = 70,
                Porcoes = 8,
                Calorias = 280,
                Proteinas = 8,
                HidratosCarbono = 42,
                Gorduras = 8
            },
            new Receita
            {
                Nome = "Polvo à Lagareiro",
                Descricao = "Polvo assado com batatas e muito azeite.",
                ModoPreparo = "Cozinhe o polvo até ficar macio. Asse com batatas a murro, alho e azeite abundante no forno a 200°C por 30 minutos.",
                TempoPreparo = 80,
                Porcoes = 4,
                Calorias = 340,
                Proteinas = 30,
                HidratosCarbono = 25,
                Gorduras = 14
            }
        };

        var existentes = new HashSet<string>(
            context.Receita.Select(r => r.Nome),
            StringComparer.OrdinalIgnoreCase);

        var paraAdicionar = receitasSeed
            .Where(r => !existentes.Contains(r.Nome))
            .ToList();

        if (paraAdicionar.Count == 0)
        {
            return;
        }

        context.Receita.AddRange(paraAdicionar);
        context.SaveChanges();
    }

    public static void Initialize(HealthWellbeingDbContext context)
    {
        int existentes = 100; //context.Terapeutas.Count();
        int alvo = 100;

        if (existentes >= alvo) return;

        int emFalta = alvo - existentes;

        var rnd = new Random();

        // LISTA REAL DE ESPECIALIDADES
        string[] especialidades =
        {
            "Massagem",
            "Fisioterapia",
            "Estética",
            "Acupuntura",
            "Terapia da Fala",
            "Reiki"
        };

        // LISTA DE NOMES PRÓPRIOS
        string[] nomes = new[]
        {
            "Ana", "Beatriz", "Carla", "Diana", "Eva", "Filipa", "Gabriela", "Helena", "Inês", "Joana",
            "Leonor", "Marta", "Nádia", "Patrícia", "Raquel", "Sofia", "Teresa", "Vanessa",

            "André", "Bruno", "Carlos", "Daniel", "Eduardo", "Fábio", "Gonçalo", "Hugo", "Igor", "João",
            "Luis", "Marco", "Nuno", "Paulo", "Rafael", "Sérgio", "Tiago", "Vítor"
        };

        // LISTA DE APELIDOS
        string[] apelidos = new[]
        {
            "Silva", "Santos", "Ferreira", "Pereira", "Costa", "Oliveira", "Martins",
            "Rodrigues", "Almeida", "Nunes", "Gomes", "Carvalho", "Lopes",
            "Ribeiro", "Sousa", "Mendes"
        };

        var novos = new List<Terapeuta>();

        for (int i = 0; i < emFalta; i++)
        {
            string nomeCompleto =
                $"{nomes[rnd.Next(nomes.Length)]} {apelidos[rnd.Next(apelidos.Length)]} {apelidos[rnd.Next(apelidos.Length)]}";

            int anoAtual = DateTime.Now.Year;
            int anosExp = rnd.Next(1, 31); // 1 a 30 anos
            int anoEntrada = anoAtual - anosExp;

            novos.Add(new Terapeuta
            {
                Nome = nomeCompleto,
                Especialidade = especialidades[rnd.Next(especialidades.Length)],
                Telefone = $"9{rnd.Next(10000000, 99999999)}",
                Email = nomeCompleto.ToLower().Replace(" ", ".") + "@balneario.pt",
                AnoEntrada = anoEntrada,
                Ativo = rnd.Next(0, 2) == 1
            });
        }

        //context.Terapeutas.AddRange(novos);
        context.SaveChanges();
    }

    private static void PopulateConsultas(HealthWellbeingDbContext db)
    {
        if (db.Consulta.Any()) return;

        var hoje = DateTime.Today;

        var consulta = new[]
        {
            // -- Exemplo base --
            new Consulta
            {
                DataMarcacao = new DateTime(2024, 4, 21, 10, 30, 0, DateTimeKind.Unspecified),
                DataConsulta = new DateTime(2025, 4, 21, 10, 30, 0, DateTimeKind.Unspecified),
                HoraInicio   = new TimeOnly(10, 30),
                HoraFim      = new TimeOnly(11, 30),
            },

            // FUTURAS (Agendada)
            new Consulta
            {
                DataMarcacao = new DateTime(2025, 10, 10, 9, 15, 0, DateTimeKind.Unspecified),
                DataConsulta = new DateTime(2025, 11, 5, 9, 0, 0, DateTimeKind.Unspecified),
                HoraInicio   = new TimeOnly(9, 0),
                HoraFim      = new TimeOnly(9, 30),
            },
            new Consulta
            {
                DataMarcacao = new DateTime(2025, 10, 12, 14, 40, 0, DateTimeKind.Unspecified),
                DataConsulta = new DateTime(2025, 12, 1, 11, 15, 0, DateTimeKind.Unspecified),
                HoraInicio   = new TimeOnly(11, 15),
                HoraFim      = new TimeOnly(12, 0),
            },
            new Consulta
            {
                DataMarcacao = new DateTime(2025, 10, 15, 16, 5, 0, DateTimeKind.Unspecified),
                DataConsulta = new DateTime(2026, 1, 10, 15, 0, 0, DateTimeKind.Unspecified),
                HoraInicio   = new TimeOnly(15, 0),
                HoraFim      = new TimeOnly(15, 45),
            },
            new Consulta
            {
                DataMarcacao = new DateTime(2025, 10, 20, 10, 10, 0, DateTimeKind.Unspecified),
                DataConsulta = new DateTime(2025, 11, 20, 16, 30, 0, DateTimeKind.Unspecified),
                HoraInicio   = new TimeOnly(16, 30),
                HoraFim      = new TimeOnly(17, 0),
            },

            // HOJE (para testar “Hoje”)
            new Consulta
            {
                DataMarcacao = hoje.AddDays(-2).AddHours(10),
                DataConsulta = new DateTime(hoje.Year, hoje.Month, hoje.Day, 9, 30, 0, DateTimeKind.Unspecified),
                HoraInicio   = new TimeOnly(9, 30),
                HoraFim      = new TimeOnly(10, 0),
            },
            new Consulta
            {
                DataMarcacao = hoje.AddDays(-1).AddHours(15).AddMinutes(20),
                DataConsulta = new DateTime(hoje.Year, hoje.Month, hoje.Day, 14, 0, 0, DateTimeKind.Unspecified),
                HoraInicio   = new TimeOnly(14, 0),
                HoraFim      = new TimeOnly(14, 30),
            },

            // EXPIRADAS
            new Consulta
            {
                DataMarcacao = new DateTime(2025, 9, 1, 10, 0, 0, DateTimeKind.Unspecified),
                DataConsulta = new DateTime(2025, 9, 15, 9, 0, 0, DateTimeKind.Unspecified),
                HoraInicio   = new TimeOnly(9, 0),
                HoraFim      = new TimeOnly(9, 30),
            },
            new Consulta
            {
                DataMarcacao = new DateTime(2025, 8, 20, 11, 25, 0, DateTimeKind.Unspecified),
                DataConsulta = new DateTime(2025, 9, 25, 11, 45, 0, DateTimeKind.Unspecified),
                HoraInicio   = new TimeOnly(11, 45),
                HoraFim      = new TimeOnly(12, 15),
            },
            new Consulta
            {
                DataMarcacao = new DateTime(2025, 7, 5, 13, 10, 0, DateTimeKind.Unspecified),
                DataConsulta = new DateTime(2025, 8, 10, 16, 0, 0, DateTimeKind.Unspecified),
                HoraInicio   = new TimeOnly(16, 0),
                HoraFim      = new TimeOnly(16, 30),
            },

            // CANCELADAS
            new Consulta
            {
                DataMarcacao     = new DateTime(2025, 10, 1, 10, 0, 0, DateTimeKind.Unspecified),
                DataConsulta     = new DateTime(2025, 10, 30, 9, 0, 0, DateTimeKind.Unspecified),
                DataCancelamento = new DateTime(2025, 10, 28, 9, 30, 0, DateTimeKind.Unspecified),
                HoraInicio       = new TimeOnly(9, 0),
                HoraFim          = new TimeOnly(9, 30),
            },
            new Consulta
            {
                DataMarcacao     = new DateTime(2025, 9, 15, 11, 30, 0, DateTimeKind.Unspecified),
                DataConsulta     = new DateTime(2025, 10, 10, 15, 0, 0, DateTimeKind.Unspecified),
                DataCancelamento = new DateTime(2025, 10, 8, 10, 0, 0, DateTimeKind.Unspecified),
                HoraInicio       = new TimeOnly(15, 0),
                HoraFim          = new TimeOnly(15, 45),
            },
            new Consulta
            {
                DataMarcacao     = new DateTime(2025, 6, 10, 12, 0, 0, DateTimeKind.Unspecified),
                DataConsulta     = new DateTime(2025, 7, 5, 10, 30, 0, DateTimeKind.Unspecified),
                DataCancelamento = new DateTime(2025, 7, 3, 14, 15, 0, DateTimeKind.Unspecified),
                HoraInicio       = new TimeOnly(10, 30),
                HoraFim          = new TimeOnly(11, 0),
            },

            // MAIS FUTURAS
            new Consulta
            {
                DataMarcacao = new DateTime(2025, 10, 22, 9, 45, 0, DateTimeKind.Unspecified),
                DataConsulta = new DateTime(2025, 11, 15, 13, 30, 0, DateTimeKind.Unspecified),
                HoraInicio   = new TimeOnly(13, 30),
                HoraFim      = new TimeOnly(14, 15),
            },
            new Consulta
            {
                DataMarcacao = new DateTime(2025, 10, 25, 8, 55, 0, DateTimeKind.Unspecified),
                DataConsulta = new DateTime(2025, 12, 12, 8, 30, 0, DateTimeKind.Unspecified),
                HoraInicio   = new TimeOnly(8, 30),
                HoraFim      = new TimeOnly(9, 0),
            },
            new Consulta
            {
                DataMarcacao = new DateTime(2025, 10, 27, 16, 10, 0, DateTimeKind.Unspecified),
                DataConsulta = new DateTime(2026, 1, 22, 17, 0, 0, DateTimeKind.Unspecified),
                HoraInicio   = new TimeOnly(17, 0),
                HoraFim      = new TimeOnly(17, 30),
            }
        };

        db.Consulta.AddRange(consulta);
        db.SaveChanges();
    }

    private static void PopulateDoctor(HealthWellbeingDbContext db)
    {
        if (db.Doctor.Any())
        {
            return;
        }
        var doctor = new[]
        {
            new Doctor { Nome = "Ana Martins",      Telemovel = "912345678", Email = "ana.martins@healthwellbeing.pt" },
            new Doctor { Nome = "Bruno Carvalho",   Telemovel = "913456789", Email = "bruno.carvalho@healthwellbeing.pt" },
            new Doctor { Nome = "Carla Ferreira",   Telemovel = "914567890", Email = "carla.ferreira@healthwellbeing.pt" },
            new Doctor { Nome = "Daniel Sousa",     Telemovel = "915678901", Email = "daniel.sousa@healthwellbeing.pt" },
            new Doctor { Nome = "Eduarda Almeida",  Telemovel = "916789012", Email = "eduarda.almeida@healthwellbeing.pt" },
            new Doctor { Nome = "Fábio Pereira",    Telemovel = "917890123", Email = "fabio.pereira@healthwellbeing.pt" },
            new Doctor { Nome = "Gabriela Rocha",   Telemovel = "918901234", Email = "gabriela.rocha@healthwellbeing.pt" },
            new Doctor { Nome = "Hugo Santos",      Telemovel = "919012345", Email = "hugo.santos@healthwellbeing.pt" },
            new Doctor { Nome = "Inês Correia",     Telemovel = "920123456", Email = "ines.correia@healthwellbeing.pt" },
            new Doctor { Nome = "João Ribeiro",     Telemovel = "921234567", Email = "joao.ribeiro@healthwellbeing.pt" },
            new Doctor { Nome = "Luísa Nogueira",   Telemovel = "922345678", Email = "luisa.nogueira@healthwellbeing.pt" },
            new Doctor { Nome = "Miguel Costa",     Telemovel = "923456789", Email = "miguel.costa@healthwellbeing.pt" },
            new Doctor { Nome = "Nádia Gonçalves",  Telemovel = "924567890", Email = "nadia.goncalves@healthwellbeing.pt" },
            new Doctor { Nome = "Óscar Figueiredo", Telemovel = "925678901", Email = "oscar.figueiredo@healthwellbeing.pt" },
            new Doctor { Nome = "Patrícia Lopes",   Telemovel = "926789012", Email = "patricia.lopes@healthwellbeing.pt" },
        };

        db.Doctor.AddRange(doctor);
        db.SaveChanges();
    }

    private static void PopulateUtenteSaude(HealthWellbeingDbContext db)
    {
        if (db.UtenteSaude.Any()) return; // Evita duplicar registos

        var utentes = new[]
        {
            new UtenteSaude
            {
                NomeCompleto   = "Ana Beatriz Silva",
                DataNascimento = new DateTime(1999, 4, 8),
                Nif            = "245379261", // válido
                Niss           = "12345678901",
                Nus            = "123456789",
                Email          = "ana.beatriz.silva@example.pt",
                Telefone       = "912345670",
                Morada         = "Rua das Flores, 12, Guarda"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Bruno Miguel Pereira",
                DataNascimento = new DateTime(1987, 11, 23),
                Nif            = "198754326", // válido
                Niss           = "22345678901",
                Nus            = "223456789",
                Email          = "bruno.miguel.pereira@example.pt",
                Telefone       = "912345671",
                Morada         = "Av. 25 de Abril, 102, Guarda"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Carla Sofia Fernandes",
                DataNascimento = new DateTime(1991, 5, 19),
                Nif            = "156987239", // válido
                Niss           = "32345678901",
                Nus            = "323456789",
                Email          = "carla.sofia.fernandes@example.pt",
                Telefone       = "912345672",
                Morada         = "Rua da Liberdade, 45, Covilhã"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Daniel Rocha",
                DataNascimento = new DateTime(2003, 10, 26),
                Nif            = "268945315", // válido
                Niss           = "42345678901",
                Nus            = "423456789",
                Email          = "daniel.rocha@example.pt",
                Telefone       = "912345673",
                Morada         = "Travessa do Sol, 3, Celorico da Beira"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Eduarda Nogueira",
                DataNascimento = new DateTime(1994, 5, 22),
                Nif            = "296378459", // válido
                Niss           = "52345678901",
                Nus            = "523456789",
                Email          = "eduarda.nogueira@example.pt",
                Telefone       = "912345674",
                Morada         = "Rua do Comércio, 89, Seia"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Fábio Gonçalves",
                DataNascimento = new DateTime(1997, 1, 4),
                Nif            = "165947829", // válido
                Niss           = "62345678901",
                Nus            = "623456789",
                Email          = "fabio.goncalves@example.pt",
                Telefone       = "912345675",
                Morada         = "Rua da Escola, 5, Gouveia"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Gabriela Santos",
                DataNascimento = new DateTime(1986, 4, 26),
                Nif            = "189567240", // válido
                Niss           = "72345678901",
                Nus            = "723456789",
                Email          = "gabriela.santos@example.pt",
                Telefone       = "912345676",
                Morada         = "Av. Dr. Francisco Sá Carneiro, 200, Viseu"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Hugo Matos",
                DataNascimento = new DateTime(1993, 11, 22),
                Nif            = "215983747", // válido
                Niss           = "82345678901",
                Nus            = "823456789",
                Email          = "hugo.matos@example.pt",
                Telefone       = "912345677",
                Morada         = "Rua do Castelo, 7, Belmonte"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Inês Carvalho",
                DataNascimento = new DateTime(2004, 7, 12),
                Nif            = "235679845", // válido
                Niss           = "92345678901",
                Nus            = "923456789",
                Email          = "ines.carvalho@example.pt",
                Telefone       = "912345678",
                Morada         = "Rua do Mercado, 14, Trancoso"
            },
            new UtenteSaude
            {
                NomeCompleto   = "João Marques",
                DataNascimento = new DateTime(1990, 7, 4),
                Nif            = "286754197", // válido
                Niss           = "10345678901",
                Nus            = "103456789",
                Email          = "joao.marques@example.pt",
                Telefone       = "912345679",
                Morada         = "Rua da Estação, 33, Pinhel"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Luísa Almeida",
                DataNascimento = new DateTime(1978, 6, 19),
                Nif            = "248963572", // válido
                Niss           = "11345678901",
                Nus            = "113456789",
                Email          = "luisa.almeida@example.pt",
                Telefone       = "912345680",
                Morada         = "Rua da Lameira, 21, Manteigas"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Miguel Figueiredo",
                DataNascimento = new DateTime(1985, 8, 9),
                Nif            = "196284739", // válido
                Niss           = "12345678902",
                Nus            = "123456788",
                Email          = "miguel.figueiredo@example.pt",
                Telefone       = "912345681",
                Morada         = "Rua do Parque, 8, Almeida"
            },

            // ---------- + Exemplos ----------
            new UtenteSaude
            {
                NomeCompleto   = "Joana Moreira",
                DataNascimento = new DateTime(1988, 3, 14),
                Nif            = "218945372",
                Niss           = "11111111111",
                Nus            = "111111111",
                Email          = "joana.moreira@example.pt",
                Telefone       = "913245671",
                Morada         = "Rua das Amoreiras, 15, Lisboa"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Carlos Almeida",
                DataNascimento = new DateTime(1975, 9, 22),
                Nif            = "295431678",
                Niss           = "11111111112",
                Nus            = "111111112",
                Email          = "carlos.almeida@example.pt",
                Telefone       = "912334567",
                Morada         = "Avenida 25 de Abril, 20, Porto"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Sofia Marques",
                DataNascimento = new DateTime(1992, 12, 5),
                Nif            = "189546327",
                Niss           = "11111111113",
                Nus            = "111111113",
                Email          = "sofia.marques@example.pt",
                Telefone       = "916785432",
                Morada         = "Rua da Liberdade, 33, Coimbra"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Ricardo Nogueira",
                DataNascimento = new DateTime(1984, 2, 18),
                Nif            = "239857416",
                Niss           = "11111111114",
                Nus            = "111111114",
                Email          = "ricardo.nogueira@example.pt",
                Telefone       = "915889002",
                Morada         = "Travessa do Sol, 2, Braga"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Helena Rocha",
                DataNascimento = new DateTime(1990, 7, 21),
                Nif            = "259784631",
                Niss           = "11111111115",
                Nus            = "111111115",
                Email          = "helena.rocha@example.pt",
                Telefone       = "917654320",
                Morada         = "Rua das Flores, 44, Viseu"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Tiago Fernandes",
                DataNascimento = new DateTime(1982, 4, 9),
                Nif            = "268953741",
                Niss           = "11111111116",
                Nus            = "111111116",
                Email          = "tiago.fernandes@example.pt",
                Telefone       = "912120234",
                Morada         = "Avenida dos Bombeiros, 12, Aveiro"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Andreia Pinto",
                DataNascimento = new DateTime(1995, 6, 30),
                Nif            = "235978416",
                Niss           = "11111111117",
                Nus            = "111111117",
                Email          = "andreia.pinto@example.pt",
                Telefone       = "916782543",
                Morada         = "Rua de São João, 9, Guarda"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Pedro Carvalho",
                DataNascimento = new DateTime(1978, 10, 12),
                Nif            = "298671543",
                Niss           = "11111111118",
                Nus            = "111111118",
                Email          = "pedro.carvalho@example.pt",
                Telefone       = "913998877",
                Morada         = "Rua do Comércio, 70, Castelo Branco"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Marta Ribeiro",
                DataNascimento = new DateTime(1987, 1, 7),
                Nif            = "214896573",
                Niss           = "11111111119",
                Nus            = "111111119",
                Email          = "marta.ribeiro@example.pt",
                Telefone       = "919776543",
                Morada         = "Largo da Igreja, 22, Viana do Castelo"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Luís Santos",
                DataNascimento = new DateTime(1980, 5, 27),
                Nif            = "268974153",
                Niss           = "11111111120",
                Nus            = "111111120",
                Email          = "luis.santos@example.pt",
                Telefone       = "914563278",
                Morada         = "Praceta do Parque, 5, Leiria"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Filipa Gomes",
                DataNascimento = new DateTime(1991, 8, 19),
                Nif            = "189574362",
                Niss           = "11111111121",
                Nus            = "111111121",
                Email          = "filipa.gomes@example.pt",
                Telefone       = "913445677",
                Morada         = "Rua da Escola, 10, Évora"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Rui Correia",
                DataNascimento = new DateTime(1976, 3, 4),
                Nif            = "278954136",
                Niss           = "11111111122",
                Nus            = "111111122",
                Email          = "rui.correia@example.pt",
                Telefone       = "912233456",
                Morada         = "Rua dos Pescadores, 45, Nazaré"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Bárbara Figueiredo",
                DataNascimento = new DateTime(1994, 11, 29),
                Nif            = "215978643",
                Niss           = "11111111123",
                Nus            = "111111123",
                Email          = "barbara.figueiredo@example.pt",
                Telefone       = "915667788",
                Morada         = "Rua da Lameira, 31, Torres Vedras"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Diogo Costa",
                DataNascimento = new DateTime(1983, 12, 11),
                Nif            = "268974523",
                Niss           = "11111111124",
                Nus            = "111111124",
                Email          = "diogo.costa@example.pt",
                Telefone       = "914555666",
                Morada         = "Rua dos Combatentes, 14, Santarém"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Catarina Martins",
                DataNascimento = new DateTime(1998, 9, 5),
                Nif            = "239876415",
                Niss           = "11111111125",
                Nus            = "111111125",
                Email          = "catarina.martins@example.pt",
                Telefone       = "916787654",
                Morada         = "Avenida da Liberdade, 66, Lisboa"
            },
            new UtenteSaude
            {
                NomeCompleto   = "João Vieira",
                DataNascimento = new DateTime(1979, 7, 15),
                Nif            = "258946371",
                Niss           = "11111111126",
                Nus            = "111111126",
                Email          = "joao.vieira@example.pt",
                Telefone       = "912444555",
                Morada         = "Rua da Estação, 24, Braga"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Carla Neves",
                DataNascimento = new DateTime(1989, 2, 2),
                Nif            = "215987436",
                Niss           = "11111111127",
                Nus            = "111111127",
                Email          = "carla.neves@example.pt",
                Telefone       = "913998456",
                Morada         = "Rua de Santa Maria, 88, Faro"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Vítor Lopes",
                DataNascimento = new DateTime(1981, 1, 18),
                Nif            = "276895413",
                Niss           = "11111111128",
                Nus            = "111111128",
                Email          = "vitor.lopes@example.pt",
                Telefone       = "912776543",
                Morada         = "Travessa do Mercado, 15, Setúbal"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Mariana Batista",
                DataNascimento = new DateTime(1993, 4, 3),
                Nif            = "289654137",
                Niss           = "11111111129",
                Nus            = "111111129",
                Email          = "mariana.batista@example.pt",
                Telefone       = "914334566",
                Morada         = "Rua de São Tiago, 18, Aveiro"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Filipe Cruz",
                DataNascimento = new DateTime(1987, 11, 9),
                Nif            = "295764821",
                Niss           = "11111111130",
                Nus            = "111111130",
                Email          = "filipe.cruz@example.pt",
                Telefone       = "916776554",
                Morada         = "Rua da Liberdade, 10, Coimbra"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Teresa Gonçalves",
                DataNascimento = new DateTime(1990, 3, 23),
                Nif            = "198743265",
                Niss           = "11111111131",
                Nus            = "111111131",
                Email          = "teresa.goncalves@example.pt",
                Telefone       = "913221234",
                Morada         = "Rua do Castelo, 19, Guimarães"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Paulo Teixeira",
                DataNascimento = new DateTime(1975, 10, 14),
                Nif            = "269841357",
                Niss           = "11111111132",
                Nus            = "111111132",
                Email          = "paulo.teixeira@example.pt",
                Telefone       = "912888999",
                Morada         = "Avenida Central, 31, Braga"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Sandra Ramos",
                DataNascimento = new DateTime(1988, 1, 5),
                Nif            = "235978462",
                Niss           = "11111111133",
                Nus            = "111111133",
                Email          = "sandra.ramos@example.pt",
                Telefone       = "917776655",
                Morada         = "Rua das Rosas, 12, Lisboa"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Nuno Sousa",
                DataNascimento = new DateTime(1992, 5, 27),
                Nif            = "289635147",
                Niss           = "11111111134",
                Nus            = "111111134",
                Email          = "nuno.sousa@example.pt",
                Telefone       = "914334221",
                Morada         = "Travessa da Escola, 27, Aveiro"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Patrícia Cardoso",
                DataNascimento = new DateTime(1983, 6, 8),
                Nif            = "219846735",
                Niss           = "11111111135",
                Nus            = "111111135",
                Email          = "patricia.cardoso@example.pt",
                Telefone       = "915667899",
                Morada         = "Rua do Campo, 7, Viseu"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Gonçalo Rocha",
                DataNascimento = new DateTime(1985, 4, 11),
                Nif            = "295687134",
                Niss           = "11111111136",
                Nus            = "111111136",
                Email          = "goncalo.rocha@example.pt",
                Telefone       = "913456789",
                Morada         = "Avenida dos Aliados, 91, Porto"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Leonor Ferreira",
                DataNascimento = new DateTime(1996, 2, 24),
                Nif            = "218963475",
                Niss           = "11111111137",
                Nus            = "111111137",
                Email          = "leonor.ferreira@example.pt",
                Telefone       = "912998877",
                Morada         = "Rua das Oliveiras, 18, Lisboa"
            },
            new UtenteSaude
            {
                NomeCompleto   = "André Mendes",
                DataNascimento = new DateTime(1990, 12, 15),
                Nif            = "259871436",
                Niss           = "11111111138",
                Nus            = "111111138",
                Email          = "andre.mendes@example.pt",
                Telefone       = "916778899",
                Morada         = "Rua da República, 15, Coimbra"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Raquel Matos",
                DataNascimento = new DateTime(1989, 7, 7),
                Nif            = "236985147",
                Niss           = "11111111139",
                Nus            = "111111139",
                Email          = "raquel.matos@example.pt",
                Telefone       = "913667788",
                Morada         = "Rua de São João, 23, Braga"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Henrique Azevedo",
                DataNascimento = new DateTime(1977, 11, 25),
                Nif            = "289634571",
                Niss           = "11111111140",
                Nus            = "111111140",
                Email          = "henrique.azevedo@example.pt",
                Telefone       = "912443322",
                Morada         = "Rua das Laranjeiras, 19, Porto"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Beatriz Lopes",
                DataNascimento = new DateTime(1995, 8, 2),
                Nif            = "269841735",
                Niss           = "11111111141",
                Nus            = "111111141",
                Email          = "beatriz.lopes@example.pt",
                Telefone       = "915223344",
                Morada         = "Rua da Liberdade, 80, Lisboa"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Miguel Ramos",
                DataNascimento = new DateTime(1984, 9, 28),
                Nif            = "259687314",
                Niss           = "11111111142",
                Nus            = "111111142",
                Email          = "miguel.ramos@example.pt",
                Telefone       = "913332211",
                Morada         = "Rua do Cruzeiro, 17, Viseu"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Daniel Sousa",
                DataNascimento = new DateTime(1979, 3, 16),
                Nif            = "285963147",
                Niss           = "11111111143",
                Nus            = "111111143",
                Email          = "daniel.sousa@example.pt",
                Telefone       = "919887766",
                Morada         = "Travessa dos Combatentes, 21, Setúbal"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Sílvia Ferreira",
                DataNascimento = new DateTime(1993, 4, 10),
                Nif            = "239685471",
                Niss           = "11111111144",
                Nus            = "111111144",
                Email          = "silvia.ferreira@example.pt",
                Telefone       = "912667788",
                Morada         = "Avenida da Liberdade, 9, Braga"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Alexandre Pinto",
                DataNascimento = new DateTime(1986, 7, 21),
                Nif            = "278954163",
                Niss           = "11111111145",
                Nus            = "111111145",
                Email          = "alexandre.pinto@example.pt",
                Telefone       = "916443322",
                Morada         = "Rua das Escolas, 12, Coimbra"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Vera Almeida",
                DataNascimento = new DateTime(1990, 2, 19),
                Nif            = "218964735",
                Niss           = "11111111146",
                Nus            = "111111146",
                Email          = "vera.almeida@example.pt",
                Telefone       = "912443355",
                Morada         = "Rua do Penedo, 5, Viseu"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Nádia Marques",
                DataNascimento = new DateTime(1998, 11, 8),
                Nif            = "296847153",
                Niss           = "11111111147",
                Nus            = "111111147",
                Email          = "nadia.marques@example.pt",
                Telefone       = "913998554",
                Morada         = "Rua do Campo, 17, Évora"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Hugo Barros",
                DataNascimento = new DateTime(1981, 5, 4),
                Nif            = "275986431",
                Niss           = "11111111148",
                Nus            = "111111148",
                Email          = "hugo.barros@example.pt",
                Telefone       = "916555777",
                Morada         = "Rua dos Carvalhos, 14, Porto"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Joana Costa",
                DataNascimento = new DateTime(1997, 6, 22),
                Nif            = "285974316",
                Niss           = "11111111149",
                Nus            = "111111149",
                Email          = "joana.costa@example.pt",
                Telefone       = "912224466",
                Morada         = "Rua do Rossio, 11, Lisboa"
            },
            new UtenteSaude
            {
                NomeCompleto   = "Paula Rocha",
                DataNascimento = new DateTime(1988, 10, 27),
                Nif            = "269854137",
                Niss           = "11111111150",
                Nus            = "111111150",
                Email          = "paula.rocha@example.pt",
                Telefone       = "915223455",
                Morada         = "Rua da Fonte, 25, Aveiro"
            }
        };

        db.UtenteSaude.AddRange(utentes);
        db.SaveChanges();
    }

    private static void PopulateSpecialities(HealthWellbeingDbContext db)
    {
        if (db.Specialities.Any()) return; // Evita duplicar registos

        var especialidades = new[]
        {
    new Specialities
    {
        Nome = "Cardiologia",
        Descricao = "Avaliação, diagnóstico e tratamento de doenças do coração e sistema cardiovascular."
    },
    new Specialities
    {
        Nome = "Dermatologia",
        Descricao = "Prevenção, diagnóstico e tratamento de doenças da pele, cabelo e unhas."
    },
    new Specialities
    {
        Nome = "Pediatria",
        Descricao = "Cuidados de saúde para bebés, crianças e adolescentes."
    },
    new Specialities
    {
        Nome = "Psiquiatria",
        Descricao = "Avaliação e tratamento de perturbações mentais, emocionais e comportamentais."
    },
    new Specialities
    {
        Nome = "Nutrição",
        Descricao = "Aconselhamento alimentar e planos de nutrição para promoção da saúde e bem-estar."
    },
    new Specialities
    {
        Nome = "Medicina Geral e Familiar",
        Descricao = "Acompanhamento global e contínuo da saúde de utentes e famílias."
    },
    new Specialities
    {
        Nome = "Ortopedia",
        Descricao = "Tratamento de doenças e lesões dos ossos, articulações, músculos e tendões."
    },
    new Specialities
    {
        Nome = "Ginecologia e Obstetrícia",
        Descricao = "Saúde da mulher, sistema reprodutor e acompanhamento da gravidez e parto."
    },
    new Specialities
    {
        Nome = "Psicologia",
        Descricao = "Apoio psicológico, gestão emocional e acompanhamento em saúde mental."
    },
    new Specialities
    {
        Nome = "Fisioterapia",
        Descricao = "Reabilitação motora e funcional após lesões, cirurgias ou doenças crónicas."
    },

};

        db.Specialities.AddRange(especialidades);
        db.SaveChanges();
    }

    private static void PopulateEventTypes(HealthWellbeingDbContext dbContext)
    {
        if (dbContext.EventType.Any()) return;

        dbContext.EventType.AddRange(new List<EventType>() {
            //EDUCAÇÃO E FORMAÇÃO
            new EventType { EventTypeName = "Workshop Educacional", EventTypeScoringMode = "fixed", EventTypeMultiplier = 1.0f },
            new EventType { EventTypeName = "Seminário Temático", EventTypeScoringMode = "fixed", EventTypeMultiplier = 1.1f },
            new EventType { EventTypeName = "Palestra Informativa", EventTypeScoringMode = "fixed", EventTypeMultiplier = 1.0f },
            new EventType { EventTypeName = "Demonstração Técnica", EventTypeScoringMode = "fixed", EventTypeMultiplier = 1.0f },
            new EventType { EventTypeName = "Sessão de Orientação", EventTypeScoringMode = "fixed", EventTypeMultiplier = 1.0f },

            //TREINO CARDIOVASCULAR
            new EventType { EventTypeName = "Sessão de Running", EventTypeScoringMode = "time_based", EventTypeMultiplier = 2.0f },
            new EventType { EventTypeName = "Treino de Cycling", EventTypeScoringMode = "time_based", EventTypeMultiplier = 2.1f },
            new EventType { EventTypeName = "Aula de Cardio-Grupo", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.5f },
            new EventType { EventTypeName = "Treino de Natação", EventTypeScoringMode = "time_based", EventTypeMultiplier = 2.2f },
            new EventType { EventTypeName = "Sessão de HIIT", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.8f },

            //TREINO DE FORÇA
            new EventType { EventTypeName = "Treino de Musculação", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.7f },
            new EventType { EventTypeName = "Sessão de CrossFit", EventTypeScoringMode = "progressive", EventTypeMultiplier = 2.1f },
            new EventType { EventTypeName = "Treino Funcional", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.6f },
            new EventType { EventTypeName = "Aula de Powerlifting", EventTypeScoringMode = "progressive", EventTypeMultiplier = 2.0f },
            new EventType { EventTypeName = "Treino de Calistenia", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.5f },

            //BEM-ESTAR E MOBILIDADE
            new EventType { EventTypeName = "Aula de Yoga", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.3f },
            new EventType { EventTypeName = "Sessão de Pilates", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.4f },
            new EventType { EventTypeName = "Treino de Flexibilidade", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.5f },
            new EventType { EventTypeName = "Aula de Mobilidade", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.3f },
            new EventType { EventTypeName = "Sessão de Alongamento", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.2f },

            //DESPORTOS E ARTES MARCIAS
            new EventType { EventTypeName = "Aula de Artes Marciais", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.8f },
            new EventType { EventTypeName = "Treino de Boxe", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.9f },
            new EventType { EventTypeName = "Sessão de Lutas", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.8f },
            new EventType { EventTypeName = "Aula de Defesa Pessoal", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.4f },
            new EventType { EventTypeName = "Treino Desportivo Coletivo", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.6f },

            //DESAFIOS E COMPETIÇÕES
            new EventType { EventTypeName = "Competição de Running", EventTypeScoringMode = "time_based", EventTypeMultiplier = 2.5f },
            new EventType { EventTypeName = "Torneio Desportivo", EventTypeScoringMode = "binary", EventTypeMultiplier = 2.3f },
            new EventType { EventTypeName = "Desafio de Resistência", EventTypeScoringMode = "time_based", EventTypeMultiplier = 2.4f },
            new EventType { EventTypeName = "Competição de Força", EventTypeScoringMode = "progressive", EventTypeMultiplier = 2.2f },
            new EventType { EventTypeName = "Desafio de Superação", EventTypeScoringMode = "progressive", EventTypeMultiplier = 2.1f },

            //ATIVIDADES EM GRUPO
            new EventType { EventTypeName = "Aula de Grupo", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.4f },
            new EventType { EventTypeName = "Treino Coletivo", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.5f },
            new EventType { EventTypeName = "Workshop Prático", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.3f },
            new EventType { EventTypeName = "Sessão de Team Building", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.2f },
            new EventType { EventTypeName = "Aula Experimental", EventTypeScoringMode = "binary", EventTypeMultiplier = 1.1f },

            //ESPECIALIZADOS E TÉCNICOS
            new EventType { EventTypeName = "Treino Técnico", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.6f },
            new EventType { EventTypeName = "Workshop de Técnica", EventTypeScoringMode = "completion", EventTypeMultiplier = 1.3f },
            new EventType { EventTypeName = "Aula Avançada", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.7f },
            new EventType { EventTypeName = "Sessão de Perfeiçoamento", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.5f },
            new EventType { EventTypeName = "Treino Especializado", EventTypeScoringMode = "progressive", EventTypeMultiplier = 1.8f }
        });

        dbContext.SaveChanges();
    }

    private static void PopulateEvents(HealthWellbeingDbContext dbContext)
    {
        if (dbContext.Event.Any()) return;

        var eventTypes = dbContext.EventType.ToList();
        if (!eventTypes.Any()) return;

        var eventList = new List<Event>();
        var now = DateTime.Now;

        eventList.Add(new Event { EventName = "Competição Anual", EventDescription = "Competição de final de ano.", EventTypeId = eventTypes[0].EventTypeId, EventStart = now.AddDays(-30), EventEnd = now.AddDays(-30).AddHours(3), EventPoints = 200, MinLevel = 3 });
        eventList.Add(new Event { EventName = "Workshop de Nutrição", EventDescription = "Aprenda a comer melhor.", EventTypeId = eventTypes[1].EventTypeId, EventStart = now.AddDays(-28), EventEnd = now.AddDays(-28).AddHours(2), EventPoints = 50, MinLevel = 1 });
        eventList.Add(new Event { EventName = "Aula de Zumba", EventDescription = "Dança e diversão.", EventTypeId = eventTypes[2].EventTypeId, EventStart = now.AddDays(-26), EventEnd = now.AddDays(-26).AddHours(1), EventPoints = 75, MinLevel = 1 });
        eventList.Add(new Event { EventName = "Desafio CrossFit", EventDescription = "Teste os seus limites.", EventTypeId = eventTypes[3].EventTypeId, EventStart = now.AddDays(-24), EventEnd = now.AddDays(-24).AddHours(2), EventPoints = 150, MinLevel = 4 });
        eventList.Add(new Event { EventName = "Torneio de Ténis", EventDescription = "Torneio de pares.", EventTypeId = eventTypes[4].EventTypeId, EventStart = now.AddDays(-22), EventEnd = now.AddDays(-22).AddHours(5), EventPoints = 250, MinLevel = 3 });
        eventList.Add(new Event { EventName = "Seminário de Saúde Mental", EventDescription = "Bem-estar psicológico.", EventTypeId = eventTypes[5].EventTypeId, EventStart = now.AddDays(-20), EventEnd = now.AddDays(-20).AddHours(2), EventPoints = 55, MinLevel = 1 });
        eventList.Add(new Event { EventName = "Sessão de Personal Trainer", EventDescription = "Foco nos seus objetivos.", EventTypeId = eventTypes[6].EventTypeId, EventStart = now.AddDays(-18), EventEnd = now.AddDays(-18).AddHours(1), EventPoints = 90, MinLevel = 2 });
        eventList.Add(new Event { EventName = "Meia Maratona", EventDescription = "Corrida de 21km.", EventTypeId = eventTypes[7].EventTypeId, EventStart = now.AddDays(-16), EventEnd = now.AddDays(-16).AddHours(4), EventPoints = 300, MinLevel = 5 });
        eventList.Add(new Event { EventName = "Campeonato de Natação", EventDescription = "Vários estilos.", EventTypeId = eventTypes[8].EventTypeId, EventStart = now.AddDays(-14), EventEnd = now.AddDays(-14).AddHours(3), EventPoints = 280, MinLevel = 4 });
        eventList.Add(new Event { EventName = "Palestra Motivacional", EventDescription = "Alcance o seu potencial.", EventTypeId = eventTypes[9].EventTypeId, EventStart = now.AddDays(-12), EventEnd = now.AddDays(-12).AddHours(1), EventPoints = 50, MinLevel = 1 });
        eventList.Add(new Event { EventName = "Aula de Yoga Experimental", EventDescription = "Descubra o Yoga.", EventTypeId = eventTypes[10].EventTypeId, EventStart = now.AddDays(-10), EventEnd = now.AddDays(-10).AddHours(1), EventPoints = 60, MinLevel = 1 });
        eventList.Add(new Event { EventName = "Desafio de Powerlifting", EventDescription = "Supino, Agachamento e Peso Morto.", EventTypeId = eventTypes[11].EventTypeId, EventStart = now.AddDays(-8), EventEnd = now.AddDays(-8).AddHours(3), EventPoints = 180, MinLevel = 4 });
        eventList.Add(new Event { EventName = "Torneio de Voleibol", EventDescription = "Equipas de 4.", EventTypeId = eventTypes[12].EventTypeId, EventStart = now.AddDays(-6), EventEnd = now.AddDays(-6).AddHours(4), EventPoints = 220, MinLevel = 2 });
        eventList.Add(new Event { EventName = "Workshop de Mindfulness", EventDescription = "Técnicas de relaxamento.", EventTypeId = eventTypes[13].EventTypeId, EventStart = now.AddDays(-4), EventEnd = now.AddDays(-4).AddHours(2), EventPoints = 65, MinLevel = 1 });
        eventList.Add(new Event { EventName = "Aula de Técnica de Corrida", EventDescription = "Corra de forma eficiente.", EventTypeId = eventTypes[14].EventTypeId, EventStart = now.AddDays(-2), EventEnd = now.AddDays(-2).AddHours(1), EventPoints = 80, MinLevel = 2 });

        eventList.Add(new Event { EventName = "Desafio de Sprint", EventDescription = "Evento a decorrer agora.", EventTypeId = eventTypes[15].EventTypeId, EventStart = now.AddMinutes(-30), EventEnd = now.AddMinutes(30), EventPoints = 110, MinLevel = 2 });
        eventList.Add(new Event { EventName = "Liga de Basquetebol", EventDescription = "Jogo semanal.", EventTypeId = eventTypes[16].EventTypeId, EventStart = now.AddHours(-1), EventEnd = now.AddHours(1), EventPoints = 290, MinLevel = 3 });
        eventList.Add(new Event { EventName = "Demonstração de Artes Marciais", EventDescription = "Apresentação de técnicas.", EventTypeId = eventTypes[17].EventTypeId, EventStart = now.AddMinutes(-15), EventEnd = now.AddHours(1), EventPoints = 50, MinLevel = 1 });
        eventList.Add(new Event { EventName = "Treino de HIIT", EventDescription = "Alta intensidade.", EventTypeId = eventTypes[18].EventTypeId, EventStart = now.AddMinutes(-10), EventEnd = now.AddMinutes(45), EventPoints = 70, MinLevel = 2 });
        eventList.Add(new Event { EventName = "Competição de Crossfit", EventDescription = "WOD especial.", EventTypeId = eventTypes[19].EventTypeId, EventStart = now.AddHours(-2), EventEnd = now.AddHours(1), EventPoints = 190, MinLevel = 4 });

        eventList.Add(new Event { EventName = "Workshop Prático de Primeiros Socorros", EventDescription = "Saiba como agir.", EventTypeId = eventTypes[20].EventTypeId, EventStart = now.AddDays(1), EventEnd = now.AddDays(1).AddHours(3), EventPoints = 75, MinLevel = 1 });
        eventList.Add(new Event { EventName = "Aula Avançada de Spinning", EventDescription = "Suba a montanha.", EventTypeId = eventTypes[21].EventTypeId, EventStart = now.AddDays(2), EventEnd = now.AddDays(2).AddHours(1), EventPoints = 95, MinLevel = 3 });
        eventList.Add(new Event { EventName = "Desafio de Tiro ao Arco", EventDescription = "Teste a sua mira.", EventTypeId = eventTypes[22].EventTypeId, EventStart = now.AddDays(3), EventEnd = now.AddDays(3).AddHours(2), EventPoints = 100, MinLevel = 2 });
        eventList.Add(new Event { EventName = "Torneio de Xadrez", EventDescription = "Eliminatórias.", EventTypeId = eventTypes[23].EventTypeId, EventStart = now.AddDays(4), EventEnd = now.AddDays(4).AddHours(4), EventPoints = 260, MinLevel = 1 });
        eventList.Add(new Event { EventName = "Conferência de Medicina Desportiva", EventDescription = "Novas tendências.", EventTypeId = eventTypes[24].EventTypeId, EventStart = now.AddDays(5), EventEnd = now.AddDays(5).AddHours(6), EventPoints = 50, MinLevel = 1 });
        eventList.Add(new Event { EventName = "Aula de Pilates para Iniciantes", EventDescription = "Controle o seu corpo.", EventTypeId = eventTypes[25].EventTypeId, EventStart = now.AddDays(6), EventEnd = now.AddDays(6).AddHours(1), EventPoints = 65, MinLevel = 1 });
        eventList.Add(new Event { EventName = "Competição de Skate", EventDescription = "Melhor manobra.", EventTypeId = eventTypes[26].EventTypeId, EventStart = now.AddDays(7), EventEnd = now.AddDays(7).AddHours(3), EventPoints = 230, MinLevel = 2 });
        eventList.Add(new Event { EventName = "Workshop Teórico de Treino", EventDescription = "Planeamento de treino.", EventTypeId = eventTypes[27].EventTypeId, EventStart = now.AddDays(8), EventEnd = now.AddDays(8).AddHours(2), EventPoints = 50, MinLevel = 1 });
        eventList.Add(new Event { EventName = "Treino de Maratona (Grupo)", EventDescription = "Preparação conjunta.", EventTypeId = eventTypes[28].EventTypeId, EventStart = now.AddDays(9), EventEnd = now.AddDays(9).AddHours(2), EventPoints = 150, MinLevel = 3 });
        eventList.Add(new Event { EventName = "Desafio de Slackline", EventDescription = "Teste o seu equilíbrio.", EventTypeId = eventTypes[29].EventTypeId, EventStart = now.AddDays(10), EventEnd = now.AddDays(10).AddHours(2), EventPoints = 90, MinLevel = 2 });
        eventList.Add(new Event { EventName = "Campeonato de Judo", EventDescription = "Fases de grupos.", EventTypeId = eventTypes[30].EventTypeId, EventStart = now.AddDays(11), EventEnd = now.AddDays(11).AddHours(5), EventPoints = 270, MinLevel = 3 });
        eventList.Add(new Event { EventName = "Aula Especializada de Defesa Pessoal", EventDescription = "Técnicas essenciais.", EventTypeId = eventTypes[31].EventTypeId, EventStart = now.AddDays(12), EventEnd = now.AddDays(12).AddHours(2), EventPoints = 85, MinLevel = 1 });
        eventList.Add(new Event { EventName = "Workshop de Dança Contemporânea", EventDescription = "Movimento e expressão.", EventTypeId = eventTypes[32].EventTypeId, EventStart = now.AddDays(13), EventEnd = now.AddDays(13).AddHours(2), EventPoints = 70, MinLevel = 1 });
        eventList.Add(new Event { EventName = "Competição de Remo", EventDescription = "Contra-relógio.", EventTypeId = eventTypes[33].EventTypeId, EventStart = now.AddDays(14), EventEnd = now.AddDays(14).AddHours(3), EventPoints = 240, MinLevel = 3 });
        eventList.Add(new Event { EventName = "Treino de Flexibilidade (Grupo)", EventDescription = "Alongamentos profundos.", EventTypeId = eventTypes[34].EventTypeId, EventStart = now.AddDays(15), EventEnd = now.AddDays(15).AddHours(1), EventPoints = 75, MinLevel = 1 });
        eventList.Add(new Event { EventName = "Desafio de Parkour", EventDescription = "Circuito de obstáculos.", EventTypeId = eventTypes[35].EventTypeId, EventStart = now.AddDays(16), EventEnd = now.AddDays(16).AddHours(2), EventPoints = 160, MinLevel = 4 });
        eventList.Add(new Event { EventName = "Torneio de Padel", EventDescription = "Sistema Round Robin.", EventTypeId = eventTypes[36].EventTypeId, EventStart = now.AddDays(17), EventEnd = now.AddDays(17).AddHours(4), EventPoints = 250, MinLevel = 2 });
        eventList.Add(new Event { EventName = "Sessão de Orientação (Outdoor)", EventDescription = "Navegação e natureza.", EventTypeId = eventTypes[37].EventTypeId, EventStart = now.AddDays(18), EventEnd = now.AddDays(18).AddHours(3), EventPoints = 50, MinLevel = 1 });
        eventList.Add(new Event { EventName = "Aula de Boxe (Consolidação)", EventDescription = "Revisão de técnicas.", EventTypeId = eventTypes[38].EventTypeId, EventStart = now.AddDays(19), EventEnd = now.AddDays(19).AddHours(1), EventPoints = 65, MinLevel = 2 });
        eventList.Add(new Event { EventName = "Competição de E-Sports", EventDescription = "Torneio de FIFA.", EventTypeId = eventTypes[39].EventTypeId, EventStart = now.AddDays(20), EventEnd = now.AddDays(20).AddHours(5), EventPoints = 220, MinLevel = 1 });

        dbContext.Event.AddRange(eventList);
        dbContext.SaveChanges();
    }

    private static void PopulateLevels(HealthWellbeingDbContext dbContext)
    {
        if (dbContext.Level.Any()) return;

        dbContext.Level.AddRange(new List<Level>() {
            new Level { LevelAtual = 1, LevelCategory = "Iniciante", Description = "Primeiros passos na jornada de saúde" },
            new Level { LevelAtual = 2, LevelCategory = "Iniciante", Description = "Começando a criar rotinas saudáveis" },
            new Level { LevelAtual = 3, LevelCategory = "Iniciante", Description = "Ganhando consistência nos exercícios" },
            new Level { LevelAtual = 4, LevelCategory = "Iniciante", Description = "Progresso constante na saúde" },
            new Level { LevelAtual = 5, LevelCategory = "Iniciante", Description = "Final da fase inicial - bons hábitos estabelecidos" },
            new Level { LevelAtual = 6, LevelCategory = "Intermediário", Description = "Entrando na fase intermediária" },
            new Level { LevelAtual = 7, LevelCategory = "Intermediário", Description = "Desenvolvendo resistência física" },
            new Level { LevelAtual = 8, LevelCategory = "Intermediário", Description = "Melhorando performance geral" },
            new Level { LevelAtual = 9, LevelCategory = "Intermediário", Description = "Consolidação de técnicas avançadas" },
            new Level { LevelAtual = 10, LevelCategory = "Intermediário", Description = "Pronto para desafios maiores" },
            new Level { LevelAtual = 11, LevelCategory = "Avançado", Description = "Início da jornada avançada" },
            new Level { LevelAtual = 12, LevelCategory = "Avançado", Description = "Domínio de exercícios complexos" },
            new Level { LevelAtual = 13, LevelCategory = "Avançado", Description = "Excelência em treino cardiovascular" },
            new Level { LevelAtual = 14, LevelCategory = "Avançado", Description = "Especialização em força e resistência" },
            new Level { LevelAtual = 15, LevelCategory = "Avançado", Description = "Atleta completo em formação" },
            new Level { LevelAtual = 16, LevelCategory = "Especialista", Description = "Primeiro nível de especialista" },
            new Level { LevelAtual = 17, LevelCategory = "Especialista", Description = "Técnicas avançadas de condicionamento" },
            new Level { LevelAtual = 18, LevelCategory = "Especialista", Description = "Mestre em rotinas personalizadas" },
            new Level { LevelAtual = 19, LevelCategory = "Especialista", Description = "Referência na comunidade fitness" },
            new Level { LevelAtual = 20, LevelCategory = "Especialista", Description = "Especialista consolidado" },
            new Level { LevelAtual = 21, LevelCategory = "Mestre", Description = "Iniciando o caminho de mestre" },
            new Level { LevelAtual = 22, LevelCategory = "Mestre", Description = "Domínio completo de múltiplas modalidades" },
            new Level { LevelAtual = 23, LevelCategory = "Mestre", Description = "Liderança natural em treinos em grupo" },
            new Level { LevelAtual = 24, LevelCategory = "Mestre", Description = "Inspiração para outros utilizadores" },
            new Level { LevelAtual = 25, LevelCategory = "Mestre", Description = "Mestre em saúde e bem-estar" },
            new Level { LevelAtual = 26, LevelCategory = "Grão-Mestre", Description = "Primeiro nível de grão-mestre" },
            new Level { LevelAtual = 27, LevelCategory = "Grão-Mestre", Description = "Excelência em todos os aspectos do fitness" },
            new Level { LevelAtual = 28, LevelCategory = "Grão-Mestre", Description = "Conhecimento profundo de nutrição e exercício" },
            new Level { LevelAtual = 29, LevelCategory = "Grão-Mestre", Description = "Lenda em formação na aplicação" },
            new Level { LevelAtual = 30, LevelCategory = "Grão-Mestre", Description = "Grão-mestre consolidado" },
            new Level { LevelAtual = 31, LevelCategory = "Lendário", Description = "Entrada no hall lendário" },
            new Level { LevelAtual = 32, LevelCategory = "Lendário", Description = "Consistência lendária nos treinos" },
            new Level { LevelAtual = 33, LevelCategory = "Lendário", Description = "Performance excecional continuada" },
            new Level { LevelAtual = 34, LevelCategory = "Lendário", Description = "Ícone da aplicação" },
            new Level { LevelAtual = 35, LevelCategory = "Lendário", Description = "Lenda viva do fitness" },
            new Level { LevelAtual = 36, LevelCategory = "Mítico", Description = "Alcançando status mítico" },
            new Level { LevelAtual = 37, LevelCategory = "Mítico", Description = "Força e determinação sobre-humanas" },
            new Level { LevelAtual = 38, LevelCategory = "Mítico", Description = "Lenda entre lendas" },
            new Level { LevelAtual = 39, LevelCategory = "Mítico", Description = "Próximo do nível máximo" },
            new Level { LevelAtual = 40, LevelCategory = "Mítico", Description = "Nível máximo - Mito vivo da aplicação" }
        });
        dbContext.SaveChanges();
    }

    private static List<Client> PopulateClients(HealthWellbeingDbContext dbContext)
    {
        if (dbContext.Client.Any()) return null;


        // Lista com 25 clientes
        var clients = new List<Client>()
        {
            // Os seus 5 clientes originais
            new Client
            {
                ClientId = Guid.NewGuid().ToString("N"),
                Name = "Alice Wonderland",
                Email = "alice.w@example.com",
                Phone = "555-1234567",
                Address = "10 Downing St, London",
                BirthDate = new DateTime(1990, 5, 15),
                Gender = "Female",
                RegistrationDate = DateTime.Now.AddDays(-30)
            },
            new Client
            {
                ClientId = Guid.NewGuid().ToString("N"),
                Name = "Bob The Builder",
                Email = "bob.builder@work.net",
                Phone = "555-9876543",
                Address = "Construction Site 5A",
                BirthDate = new DateTime(1985, 10, 20),
                Gender = "Male",
                RegistrationDate = DateTime.Now.AddDays(-15),
            },
            new Client
            {
                ClientId = Guid.NewGuid().ToString("N"),
                Name = "Charlie Brown",
                Email = "charlie.b@peanuts.com",
                Phone = "555-4567890",
                Address = "123 Comic Strip Ave",
                BirthDate = new DateTime(2000, 1, 1),
                Gender = "Male",
                RegistrationDate = DateTime.Now.AddDays(-5)
            },
            new Client
            {
                ClientId = Guid.NewGuid().ToString("N"),
                Name = "David Copperfield",
                Email = "david.c@magic.com",
                Phone = "555-9001002",
                Address = "Las Vegas Strip",
                BirthDate = new DateTime(1960, 9, 16),
                Gender = "Male",
                RegistrationDate = DateTime.Now.AddDays(-25)
            },
            new Client
            {
                ClientId = Guid.NewGuid().ToString("N"),
                Name = "Eve Harrington",
                Email = "eve.h@stage.net",
                Phone = "555-3330009",
                Address = "Broadway St",
                BirthDate = new DateTime(1995, 2, 28),
                Gender = "Female",
                RegistrationDate = DateTime.Now.AddDays(-10)
            },
    
            // Mais 20 clientes para teste
            new Client
            {
                ClientId = Guid.NewGuid().ToString("N"),
                Name = "Frank Castle",
                Email = "frank.c@punisher.com",
                Phone = "555-1110001",
                Address = "Hells Kitchen, NY",
                BirthDate = new DateTime(1978, 3, 16),
                Gender = "Male",
                RegistrationDate = DateTime.Now.AddDays(-40)
            },
            new Client
            {
                ClientId = Guid.NewGuid().ToString("N"),
                Name = "Grace Hopper",
                Email = "grace.h@navy.mil",
                Phone = "555-2220002",
                Address = "Arlington, VA",
                BirthDate = new DateTime(1906, 12, 9),
                Gender = "Female",
                RegistrationDate = DateTime.Now.AddDays(-100)
            },
            new Client
            {
                ClientId = Guid.NewGuid().ToString("N"),
                Name = "Harry Potter",
                Email = "harry.p@hogwarts.wiz",
                Phone = "555-3330003",
                Address = "4 Privet Drive, Surrey",
                BirthDate = new DateTime(1980, 7, 31),
                Gender = "Male",
                RegistrationDate = DateTime.Now.AddDays(-12)
            },
            new Client
            {
                ClientId = Guid.NewGuid().ToString("N"),
                Name = "Ivy Poison",
                Email = "ivy.p@gotham.bio",
                Phone = "555-4440004",
                Address = "Gotham Gardens",
                BirthDate = new DateTime(1988, 11, 2),
                Gender = "Female",
                RegistrationDate = DateTime.Now.AddDays(-3)
            },
            new Client
            {
                ClientId = Guid.NewGuid().ToString("N"),
                Name = "Jack Sparrow",
                Email = "jack.s@pirate.sea",
                Phone = "555-5550005",
                Address = "Tortuga",
                BirthDate = new DateTime(1700, 4, 1),
                Gender = "Male",
                RegistrationDate = DateTime.Now.AddDays(-8)
            },
            new Client
            {
                ClientId = Guid.NewGuid().ToString("N"),
                Name = "Kara Danvers",
                Email = "kara.d@catco.com",
                Phone = "555-6660006",
                Address = "National City",
                BirthDate = new DateTime(1993, 9, 22),
                Gender = "Female",
                RegistrationDate = DateTime.Now.AddDays(-22)
            },
            new Client
            {
                ClientId = Guid.NewGuid().ToString("N"),
                Name = "Luke Skywalker",
                Email = "luke.s@jedi.org",
                Phone = "555-7770007",
                Address = "Tatooine",
                BirthDate = new DateTime(1977, 5, 25),
                Gender = "Male",
                RegistrationDate = DateTime.Now.AddDays(-18)
            },
            new Client
            {
                ClientId = Guid.NewGuid().ToString("N"),
                Name = "Mona Lisa",
                Email = "mona.l@art.com",
                Phone = "555-8880008",
                Address = "The Louvre, Paris",
                BirthDate = new DateTime(1503, 6, 15),
                Gender = "Female",
                RegistrationDate = DateTime.Now.AddDays(-50)
            },
            new Client
            {
                ClientId = Guid.NewGuid().ToString("N"),
                Name = "Neo Anderson",
                Email = "neo.a@matrix.com",
                Phone = "555-9990009",
                Address = "Zion",
                BirthDate = new DateTime(1971, 9, 13),
                Gender = "Male",
                RegistrationDate = DateTime.Now.AddDays(-2)
            },
            new Client
            {
                ClientId = Guid.NewGuid().ToString("N"),
                Name = "Olivia Pope",
                Email = "olivia.p@gladiator.com",
                Phone = "555-1010010",
                Address = "Washington D.C.",
                BirthDate = new DateTime(1977, 4, 2),
                Gender = "Female",
                RegistrationDate = DateTime.Now.AddDays(-60)
            },
            new Client
            {
                ClientId = Guid.NewGuid().ToString("N"),
                Name = "Peter Parker",
                Email = "peter.p@bugle.com",
                Phone = "555-2020011",
                Address = "Queens, NY",
                BirthDate = new DateTime(2001, 8, 10),
                Gender = "Male",
                RegistrationDate = DateTime.Now.AddDays(-7)
            },
            new Client
            {
                ClientId = Guid.NewGuid().ToString("N"),
                Name = "Quinn Fabray",
                Email = "quinn.f@glee.com",
                Phone = "555-3030012",
                Address = "Lima, Ohio",
                BirthDate = new DateTime(1994, 7, 19),
                Gender = "Female",
                RegistrationDate = DateTime.Now.AddDays(-33)
            },
            new Client
            {
                ClientId = Guid.NewGuid().ToString("N"),
                Name = "Rachel Green",
                Email = "rachel.g@friends.com",
                Phone = "555-4040013",
                Address = "Central Perk, NY",
                BirthDate = new DateTime(1970, 5, 5),
                Gender = "Female",
                RegistrationDate = DateTime.Now.AddDays(-45)
            },
            new Client
            {
                ClientId = Guid.NewGuid().ToString("N"),
                Name = "Steve Rogers",
                Email = "steve.r@avengers.com",
                Phone = "555-5050014",
                Address = "Brooklyn, NY",
                BirthDate = new DateTime(1918, 7, 4),
                Gender = "Male",
                RegistrationDate = DateTime.Now.AddDays(-11)
            },
            new Client
            {
                ClientId = Guid.NewGuid().ToString("N"),
                Name = "Tony Stark",
                Email = "tony.s@stark.com",
                Phone = "555-6060015",
                Address = "Malibu Point, CA",
                BirthDate = new DateTime(1970, 5, 29),
                Gender = "Male",
                RegistrationDate = DateTime.Now.AddDays(-90)
            },
            new Client
            {
                ClientId = Guid.NewGuid().ToString("N"),
                Name = "Ursula Buffay",
                Email = "ursula.b@friends.tv",
                Phone = "555-7070016",
                Address = "Riff's Bar, NY",
                BirthDate = new DateTime(1968, 2, 22),
                Gender = "Female",
                RegistrationDate = DateTime.Now.AddDays(-14)
            },
            new Client
            {
                ClientId = Guid.NewGuid().ToString("N"),
                Name = "Victor Frankenstein",
                Email = "victor.f@science.ch",
                Phone = "555-8080017",
                Address = "Geneva, Switzerland",
                BirthDate = new DateTime(1790, 10, 10),
                Gender = "Male",
                RegistrationDate = DateTime.Now.AddDays(-200)
            },
            new Client
            {
                ClientId = Guid.NewGuid().ToString("N"),
                Name = "Walter White",
                Email = "walter.w@heisenberg.com",
                Phone = "555-9090018",
                Address = "Albuquerque, NM",
                BirthDate = new DateTime(1958, 9, 7),
                Gender = "Male",
                RegistrationDate = DateTime.Now.AddDays(-28)
            },
            new Client
            {
                ClientId = Guid.NewGuid().ToString("N"),
                Name = "Xena Warrior",
                Email = "xena.w@myth.gr",
                Phone = "555-0100019",
                Address = "Amphipolis, Greece",
                BirthDate = new DateTime(1968, 3, 29),
                Gender = "Female",
                RegistrationDate = DateTime.Now.AddDays(-1)
            },
            new Client
            {
                ClientId = Guid.NewGuid().ToString("N"),
                Name = "Yoda Master",
                Email = "yoda.m@jedi.org",
                Phone = "555-1210020",
                Address = "Dagobah System",
                BirthDate = new DateTime(1000, 1, 1),
                Gender = "Male",
                RegistrationDate = DateTime.Now.AddDays(-500)
            }
        };

        // Adiciona todos os clientes à base de dados
        dbContext.Client.AddRange(clients);
        dbContext.SaveChanges();

        return clients;
    }

    private static void PopulateMetas(HealthWellbeingDbContext dbContext)
    {
        var clients = dbContext.Client
            .AsNoTracking()
            .OrderBy(c => c.Email)
            .ToList();

        if (clients.Count == 0) return;

        var existingClientIds = new HashSet<string>(
            dbContext.Meta.AsNoTracking().Select(m => m.ClientId));

        var templates = new (string desc, int kcal, int protein, int fat, int carbs, int vitamins)[]
        {
            ("Perda de peso (défice calórico)", 1800, 140, 60, 160, 100),
            ("Manutenção (equilibrado)", 2200, 130, 70, 230, 110),
            ("Ganho de massa (superávit)", 2800, 170, 80, 320, 120),
            ("Baixo carboidrato (low-carb)", 2000, 160, 90, 100, 100),
            ("Vegetariano (equilibrado)", 2100, 120, 70, 250, 120),
            ("Performance / resistência", 2600, 150, 75, 330, 130),
        };

        var metasToAdd = new List<Meta>();

        for (var i = 0; i < clients.Count; i++)
        {
            var client = clients[i];
            if (existingClientIds.Contains(client.ClientId))
                continue;

            // Leave some clients without any goal/meta.
            if (i % 5 == 0)
                continue;

            var t = templates[i % templates.Length];

            metasToAdd.Add(new Meta
            {
                ClientId = client.ClientId,
                MetaDescription = t.desc,
                DailyCalories = t.kcal,
                DailyProtein = t.protein,
                DailyFat = t.fat,
                DailyHydrates = t.carbs,
                DailyVitamins = t.vitamins,
            });
        }

        if (metasToAdd.Count == 0) return;

        dbContext.Meta.AddRange(metasToAdd);
        dbContext.SaveChanges();
    }

    private static void PopulatePlanosAlimentares(HealthWellbeingDbContext dbContext)
    {
        var clients = dbContext.Client
            .AsNoTracking()
            .OrderBy(c => c.Email)
            .ToList();

        if (clients.Count == 0) return;

        var metasByClientId = dbContext.Meta
            .AsNoTracking()
            .GroupBy(m => m.ClientId)
            .ToDictionary(g => g.Key, g => g.OrderBy(m => m.MetaId).First());

        if (metasByClientId.Count == 0) return;

        var existingPlansClientIds = new HashSet<string>(
            dbContext.PlanoAlimentar.AsNoTracking().Select(p => p.ClientId));

        var toAdd = new List<PlanoAlimentar>();

        for (var i = 0; i < clients.Count; i++)
        {
            var client = clients[i];
            if (!metasByClientId.TryGetValue(client.ClientId, out var meta))
                continue;

            if (existingPlansClientIds.Contains(client.ClientId))
                continue;

            // Not every client needs a food plan.
            if (i % 3 == 0)
                continue;

            toAdd.Add(new PlanoAlimentar
            {
                ClientId = client.ClientId,
                MetaId = meta.MetaId
            });
        }

        if (toAdd.Count == 0) return;

        dbContext.PlanoAlimentar.AddRange(toAdd);
        dbContext.SaveChanges();
    }

    private static void PopulateClientAlergias(HealthWellbeingDbContext dbContext)
    {
        var clients = dbContext.Client.AsNoTracking().OrderBy(c => c.Email).ToList();
        var alergias = dbContext.Alergia.AsNoTracking().OrderBy(a => a.AlergiaId).ToList();

        if (clients.Count == 0 || alergias.Count == 0) return;

        var existing = new HashSet<(string clientId, int alergiaId)>(
            dbContext.ClientAlergia.AsNoTracking().Select(ca => new ValueTuple<string, int>(ca.ClientId, ca.AlergiaId)));

        var toAdd = new List<ClientAlergia>();

        for (var i = 0; i < clients.Count; i++)
        {
            var client = clients[i];

            // Some clients have no allergies.
            if (i % 4 == 0)
                continue;

            var a1 = alergias[i % alergias.Count].AlergiaId;
            if (!existing.Contains((client.ClientId, a1)))
            {
                toAdd.Add(new ClientAlergia { ClientId = client.ClientId, AlergiaId = a1 });
                existing.Add((client.ClientId, a1));
            }

            // A few clients have 2 allergies.
            if (i % 9 == 0)
            {
                var a2 = alergias[(i + 3) % alergias.Count].AlergiaId;
                if (a2 != a1 && !existing.Contains((client.ClientId, a2)))
                {
                    toAdd.Add(new ClientAlergia { ClientId = client.ClientId, AlergiaId = a2 });
                    existing.Add((client.ClientId, a2));
                }
            }
        }

        if (toAdd.Count == 0) return;
        dbContext.ClientAlergia.AddRange(toAdd);
        dbContext.SaveChanges();
    }

    private static void PopulateClientRestricoes(HealthWellbeingDbContext dbContext)
    {
        var clients = dbContext.Client.AsNoTracking().OrderBy(c => c.Email).ToList();
        var restricoes = dbContext.RestricaoAlimentar.AsNoTracking().OrderBy(r => r.RestricaoAlimentarId).ToList();

        if (clients.Count == 0 || restricoes.Count == 0) return;

        var existing = new HashSet<(string clientId, int restricaoId)>(
            dbContext.ClientRestricao.AsNoTracking().Select(cr => new ValueTuple<string, int>(cr.ClientId, cr.RestricaoAlimentarId)));

        var toAdd = new List<ClientRestricao>();

        for (var i = 0; i < clients.Count; i++)
        {
            var client = clients[i];

            // Some clients have no restrictions.
            if (i % 5 == 0)
                continue;

            var r1 = restricoes[i % restricoes.Count].RestricaoAlimentarId;
            if (!existing.Contains((client.ClientId, r1)))
            {
                toAdd.Add(new ClientRestricao { ClientId = client.ClientId, RestricaoAlimentarId = r1 });
                existing.Add((client.ClientId, r1));
            }

            // A few clients have 2 restrictions.
            if (i % 7 == 0)
            {
                var r2 = restricoes[(i + 2) % restricoes.Count].RestricaoAlimentarId;
                if (r2 != r1 && !existing.Contains((client.ClientId, r2)))
                {
                    toAdd.Add(new ClientRestricao { ClientId = client.ClientId, RestricaoAlimentarId = r2 });
                    existing.Add((client.ClientId, r2));
                }
            }
        }

        if (toAdd.Count == 0) return;
        dbContext.ClientRestricao.AddRange(toAdd);
        dbContext.SaveChanges();
    }

    private static void PopulateReceitasParaPlanosAlimentares(HealthWellbeingDbContext dbContext)
    {
        var planos = dbContext.PlanoAlimentar.AsNoTracking().OrderBy(p => p.PlanoAlimentarId).ToList();
        var receitas = dbContext.Receita.AsNoTracking().OrderBy(r => r.ReceitaId).ToList();

        if (planos.Count == 0 || receitas.Count == 0) return;

        var existing = new HashSet<(int planoId, int receitaId)>(
            dbContext.ReceitasParaPlanosAlimentares
                .AsNoTracking()
                .Select(x => new ValueTuple<int, int>(x.PlanoAlimentarId, x.ReceitaId)));

        var toAdd = new List<ReceitasParaPlanosAlimentares>();

        for (var i = 0; i < planos.Count; i++)
        {
            var plano = planos[i];
            var start = (plano.PlanoAlimentarId * 3) % receitas.Count;
            var count = 4 + (i % 4); // 4..7 recipes per plan

            for (var j = 0; j < count; j++)
            {
                var receita = receitas[(start + j) % receitas.Count];
                var key = (plano.PlanoAlimentarId, receita.ReceitaId);

                if (existing.Contains(key))
                    continue;

                toAdd.Add(new ReceitasParaPlanosAlimentares
                {
                    PlanoAlimentarId = plano.PlanoAlimentarId,
                    ReceitaId = receita.ReceitaId
                });
                existing.Add(key);
            }
        }

        if (toAdd.Count == 0) return;
        dbContext.ReceitasParaPlanosAlimentares.AddRange(toAdd);
        dbContext.SaveChanges();
    }

    private static void PopulateTrainingType(HealthWellbeingDbContext dbContext)
    {
        // Check if the TrainingType table already contains data
        if (dbContext.TrainingType.Any()) return;

        dbContext.TrainingType.AddRange(new List<TrainingType>()
        {
            new TrainingType
            {
                Name = "Yoga Basics",
                Description = "A gentle introduction to yoga, focusing on flexibility, balance, and relaxation.",
                DurationMinutes = 60,
                IsActive = true
            },
            new TrainingType
            {
                Name = "HIIT (High Intensity Interval Training)",
                Description = "A fast-paced training session combining cardio and strength exercises for maximum calorie burn.",
                DurationMinutes = 45,
                IsActive = true
            },
            new TrainingType
            {
                Name = "Pilates Core Strength",
                Description = "Focus on core muscle strength, flexibility, and posture improvement.",
                DurationMinutes = 50,
                IsActive = true
            },
            new TrainingType
            {
                Name = "Zumba Dance",
                Description = "Fun and energetic dance workout set to upbeat Latin music.",
                DurationMinutes = 55,
                IsActive = true
            },
            new TrainingType
            {
                Name = "Strength Training",
                Description = "Weight-based training for building muscle mass and endurance.",
                DurationMinutes = 120,
                IsActive = true
            }
        });

        dbContext.SaveChanges();
    }

    private static void PopulatePlan(HealthWellbeingDbContext dbContext)
    {
        // Check if the Plan table already contains data
        if (dbContext.Plan.Any()) return;

        dbContext.Plan.AddRange(new List<Plan>()
        {
            new Plan
            {
                Name = "Basic Wellness Plan",
                Description = "A beginner-friendly plan including 3 workouts per week focused on flexibility and general health.",
                Price = 29.99m,
                DurationDays = 30
            },
            new Plan
            {
                Name = "Advanced Fitness Plan",
                Description = "An intensive 6-week plan designed for strength, endurance, and fat loss.",
                Price = 59.99m,
                DurationDays = 45
            },
            new Plan
            {
                Name = "Mind & Body Balance",
                Description = "A 2-month program combining yoga, meditation, and Pilates for mental and physical harmony.",
                Price = 79.99m,
                DurationDays = 60
            },
            new Plan
            {
                Name = "Ultimate Transformation Plan",
                Description = "A 3-month premium plan featuring personal coaching, nutrition guidance, and high-intensity training.",
                Price = 99.99m,
                DurationDays = 90
            },
            new Plan
            {
                Name = "Corporate Health Boost",
                Description = "A 1-month team-focused plan to improve workplace wellness, stress management, and physical activity.",
                Price = 49.99m,
                DurationDays = 30
            }
        });

        dbContext.SaveChanges();
    }

    // --- ALTERAÇÃO AQUI: O método agora retorna List<Trainer> ---
    private static List<Trainer> PopulateTrainer(HealthWellbeingDbContext dbContext)
    {
        // Check if Trainers already exist
        if (dbContext.Trainer.Any()) return dbContext.Trainer.ToList(); // Retorna se existirem

        dbContext.Trainer.AddRange(new List<Trainer>()
        {
            new Trainer
            {
                Name = "John Smith",
                Speciality = "HIIT (High Intensity Interval Training)",
                Email = "john.smith@fitnesspro.com",
                Phone = "555-1112233",
                BirthDate = new DateTime(1988, 7, 10),
                Gender = "Male"
            },
            new Trainer
            {
                Name = "Emma Johnson",
                Speciality = "Strength Training",
                Email = "emma.johnson@strongfit.net",
                Phone = "555-2223344",
                BirthDate = new DateTime(1992, 11, 25),
                Gender = "Female"
            },
            new Trainer
            {
                Name = "Carlos Mendes",
                Speciality = "Yoga Basics",
                Email = "carlos.mendes@yogabalance.org",
                Phone = "555-3334455",
                BirthDate = new DateTime(1975, 4, 1),
                Gender = "Male"
            },
            new Trainer
            {
                Name = "Sophie Lee",
                Speciality = "Pilates Core Strength",
                Email = "sophie.lee@corewellness.com",
                Phone = "555-4445566",
                BirthDate = new DateTime(1996, 2, 14),
                Gender = "Female"
            },
            new Trainer
            {
                Name = "Maria Rodriguez",
                Speciality = "Zumba Dance",
                Email = "maria.rodriguez@zumbafit.com",
                Phone = "555-5557788",
                BirthDate = new DateTime(1985, 9, 30),
                Gender = "Female"
            }
        });

        dbContext.SaveChanges();
        return dbContext.Trainer.ToList();
    }

    private static void PopulateTraining(HealthWellbeingDbContext dbContext, List<Trainer> trainers)
    {
        if (dbContext.Training.Any()) return;

        var yogaTypeId = dbContext.TrainingType.FirstOrDefault(t => t.Name == "Yoga Basics")?.TrainingTypeId;
        var hiitTypeId = dbContext.TrainingType.FirstOrDefault(t => t.Name == "HIIT (High Intensity Interval Training)")?.TrainingTypeId;

        var carlosId = trainers.FirstOrDefault(t => t.Name == "Carlos Mendes")?.TrainerId;
        var johnId = trainers.FirstOrDefault(t => t.Name == "John Smith")?.TrainerId;


        if (yogaTypeId.HasValue && hiitTypeId.HasValue && carlosId.HasValue && johnId.HasValue)
        {
            dbContext.Training.AddRange(new List<Training>()
            {
                new Training
                {
                    TrainingTypeId = yogaTypeId.Value,
                    TrainerId = carlosId.Value,
                    Name = "Morning Yoga",
                    Duration = 60,
                    DayOfWeek = "Monday",
                    StartTime = new TimeSpan(10, 0, 0),
                    MaxParticipants = 15
                },
                new Training
                {
                    TrainingTypeId = hiitTypeId.Value,
                    TrainerId = johnId.Value,
                    Name = "Intense Cardio HIT",
                    Duration = 45,
                    DayOfWeek = "Wednesday",
                    StartTime = new TimeSpan(18, 30, 0),
                    MaxParticipants = 20
                },
                    new Training
                {
                    TrainingTypeId = hiitTypeId.Value,
                    TrainerId = johnId.Value,
                    Name = "Strength Training",
                    Duration = 120,
                    DayOfWeek = "Friday",
                    StartTime = new TimeSpan(16, 0, 0),
                    MaxParticipants = 8
                }
            });

            dbContext.SaveChanges();
        }
    }
}
