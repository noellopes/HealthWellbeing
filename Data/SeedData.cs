using HealthWellbeing.Data;
using HealthWellbeing.Models;
using Microsoft.AspNetCore.Identity;

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

        PopulateSpecialities(dbContext);
        // PopulateConsultas IS NOT WORKING
        // PopulateConsultas(dbContext);
        PopulateDoctor(dbContext);
        PopulateUtenteSaude(dbContext);

        PopulateClients(dbContext);
        //PopulateMember(dbContext, clients); with error
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
        EnsureRoleIsCreatedAsync(roleManager, "Administrador").Wait();
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

    internal static void SeedUsers(UserManager<IdentityUser> userManager)
    {
        EnsureUserIsCreatedAsync(userManager, "cliente@health.com", "Secret123$", new[] { "Cliente" }).Wait();
        EnsureUserIsCreatedAsync(userManager, "nutri@health.com", "Secret123$", new[] { "Nutricionista" }).Wait();
        EnsureUserIsCreatedAsync(userManager, "admin@health.com", "Secret123$", new[] { "Administrador" }).Wait();
    }

    private static async Task EnsureUserIsCreatedAsync(
        UserManager<IdentityUser> userManager,
        string username,
        string password,
        string[] roles)
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
    }

    internal static void SeedDefaultAdmin(UserManager<IdentityUser> userManager)
    {
        EnsureUserIsCreatedAsync(userManager, "admin@ipg.pt", "Secret123$", ["Administrador"]).Wait();
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
        if (context.Alimentos.Any())
        {
            return;
        }

        context.Alimentos.AddRange(
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
            new Alimento { Name = "Batata Doce", Description = "Tubérculo rico em betacaroteno e fibras.", CategoriaAlimentoId = 3, Calories = 86, KcalPor100g = 86, ProteinaGPor100g = 1.6m, HidratosGPor100g = 20m, GorduraGPor100g = 0.1m }
        );

        context.SaveChanges();
    }


    private static void PopulateClients(HealthWellbeingDbContext context)
    {
        if (!context.Client.Any())
        {
            var clients = new List<Client>
    {
        new Client
        {
            Name      = "Alice Wonder",
            Email     = "alice@example.com",
            BirthDate = new DateTime(1992, 5, 14),
            Gender    = "Female"
        },
        new Client
        {
            Name      = "Bob Strong",
            Email     = "bob@example.com",
            BirthDate = new DateTime(1987, 2, 8),
            Gender    = "Male"
        },
        new Client
        {
            Name      = "Charlie Fit",
            Email     = "charlie@example.com",
            BirthDate = new DateTime(1998, 10, 20),
            Gender    = "Male"
        }
    };

            // Criar clientes de teste 4–30
            for (int i = 4; i <= 30; i++)
            {
                clients.Add(new Client
                {
                    Name = $"Test Client {i}",
                    Email = $"testclient{i}@example.com",
                    BirthDate = new DateTime(1990, 1, 1).AddDays(i * 20),
                    Gender = i % 2 == 0 ? "Male" : "Female"
                });
            }

            context.Client.AddRange(clients);
            context.SaveChanges();

        }
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

        var alergiaAlimentos = new List<AlergiaAlimento>
    {
        new AlergiaAlimento { AlergiaId = alergias[0].AlergiaId, AlimentoId = alimentos.FirstOrDefault(a => a.Name == "Amendoim")?.AlimentoId ?? 1 },
        new AlergiaAlimento { AlergiaId = alergias[1].AlergiaId, AlimentoId = alimentos.FirstOrDefault(a => a.Name == "Leite")?.AlimentoId ?? 4 },
        new AlergiaAlimento { AlergiaId = alergias[2].AlergiaId, AlimentoId = alimentos.FirstOrDefault(a => a.Name == "Ovo")?.AlimentoId ?? 2 },
        new AlergiaAlimento { AlergiaId = alergias[3].AlergiaId, AlimentoId = alimentos.FirstOrDefault(a => a.Name == "Trigo")?.AlimentoId ?? 3 },
        new AlergiaAlimento { AlergiaId = alergias[4].AlergiaId, AlimentoId = alimentos.FirstOrDefault(a => a.Name == "Frango")?.AlimentoId ?? 5 },

        // Associações das novas alergias (usando alimentos existentes)
        new AlergiaAlimento { AlergiaId = alergias[5].AlergiaId, AlimentoId = alimentos.FirstOrDefault(a => a.Name == "Amendoim")?.AlimentoId ?? 1 },
        new AlergiaAlimento { AlergiaId = alergias[6].AlergiaId, AlimentoId = alimentos.FirstOrDefault(a => a.Name == "Trigo")?.AlimentoId ?? 3 }, // soja associada ao trigo (aproximação)
        new AlergiaAlimento { AlergiaId = alergias[7].AlergiaId, AlimentoId = alimentos.FirstOrDefault(a => a.Name == "Frutos do Mar")?.AlimentoId ?? 5 },
        new AlergiaAlimento { AlergiaId = alergias[8].AlergiaId, AlimentoId = alimentos.FirstOrDefault(a => a.Name == "Frango")?.AlimentoId ?? 5 },
        new AlergiaAlimento { AlergiaId = alergias[9].AlergiaId, AlimentoId = alimentos.FirstOrDefault(a => a.Name == "Trigo")?.AlimentoId ?? 3 },
        new AlergiaAlimento { AlergiaId = alergias[10].AlergiaId, AlimentoId = alimentos.FirstOrDefault(a => a.Name == "Leite")?.AlimentoId ?? 4 }
    };

        context.AlergiaAlimento.AddRange(alergiaAlimentos);
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
        var frutosMarAlimentos = alimentos.Where(a => a.Name.Contains("Salmão")).Take(1);
        foreach (var alimento in frutosMarAlimentos)
        {
            associacoes.Add(new RestricaoAlimentarAlimento
            {
                RestricaoAlimentarId = frutosMar.RestricaoAlimentarId,
                AlimentoId = alimento.AlimentoId
            });
        }

        // Alergia a Amendoim (simulada com alimentos existentes)
        var amendoim = restricoes.First(r => r.Nome == "Alergia a Amendoim");
        var amendoimAlimentos = alimentos.Where(a => a.Name.Contains("Feijão") || a.Name.Contains("Aveia")).Take(2);
        foreach (var alimento in amendoimAlimentos)
        {
            associacoes.Add(new RestricaoAlimentarAlimento
            {
                RestricaoAlimentarId = amendoim.RestricaoAlimentarId,
                AlimentoId = alimento.AlimentoId
            });
        }

        // Alergia a Ovos (simulada com alimentos existentes)
        var ovos = restricoes.First(r => r.Nome == "Alergia a Ovos");
        var ovosAlimentos = alimentos.Where(a => a.Name.Contains("Maçã") || a.Name.Contains("Banana")).Take(2);
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

        context.AlimentoSubstitutos.AddRange(

            new AlimentoSubstituto { AlimentoOriginalId = 1, AlimentoSubstitutoRefId = 2, Motivo = "Alternativa para alergia.", ProporcaoEquivalente = 1, Observacoes = "Substituição recomendada.", FatorSimilaridade = 0.8 },
            new AlimentoSubstituto { AlimentoOriginalId = 3, AlimentoSubstitutoRefId = 4, Motivo = "Alternativa para intolerância ao glúten.", ProporcaoEquivalente = 1, Observacoes = "Substituição recomendada.", FatorSimilaridade = 0.9 },
            new AlimentoSubstituto { AlimentoOriginalId = 5, AlimentoSubstitutoRefId = 1, Motivo = "Alternativa para dieta vegana.", ProporcaoEquivalente = 1, Observacoes = "Substituição recomendada.", FatorSimilaridade = 0.7 },
            new AlimentoSubstituto { AlimentoOriginalId = 2, AlimentoSubstitutoRefId = 3, Motivo = "Alternativa para restrição alimentar.", ProporcaoEquivalente = 1, Observacoes = "Substituição recomendada.", FatorSimilaridade = 0.85 },
            new AlimentoSubstituto { AlimentoOriginalId = 4, AlimentoSubstitutoRefId = 5, Motivo = "Alternativa para alergia ao leite.", ProporcaoEquivalente = 1, Observacoes = "Substituição recomendada.", FatorSimilaridade = 0.75 },


            // Substitutos para Maçã (1)
            new AlimentoSubstituto { AlimentoOriginalId = 1, AlimentoSubstitutoRefId = 6, Motivo = "Opção doce e similar em sobremesas.", ProporcaoEquivalente = 1, Observacoes = "Boa troca em vitaminas e bolos.", FatorSimilaridade = 0.78 },
            new AlimentoSubstituto { AlimentoOriginalId = 1, AlimentoSubstitutoRefId = 8, Motivo = "Substituto em pratos assados.", ProporcaoEquivalente = 1, Observacoes = "Textura diferente, sabor complementar.", FatorSimilaridade = 0.55 },
            new AlimentoSubstituto { AlimentoOriginalId = 1, AlimentoSubstitutoRefId = 15, Motivo = "Alternativa energética e rica em fibras.", ProporcaoEquivalente = 1, Observacoes = "Usar em purês ou assados.", FatorSimilaridade = 0.6 },

            // Substitutos para Cenoura (2)
            new AlimentoSubstituto { AlimentoOriginalId = 2, AlimentoSubstitutoRefId = 12, Motivo = "Boa fonte de fibras para sopas.", ProporcaoEquivalente = 1, Observacoes = "Textura semelhante ao cozinhar.", FatorSimilaridade = 0.62 },
            new AlimentoSubstituto { AlimentoOriginalId = 2, AlimentoSubstitutoRefId = 7, Motivo = "Substituto em purês e assados.", ProporcaoEquivalente = 1, Observacoes = "Ajustar temperos.", FatorSimilaridade = 0.58 },
            new AlimentoSubstituto { AlimentoOriginalId = 2, AlimentoSubstitutoRefId = 6, Motivo = "Alternativa em saladas e snacks.", ProporcaoEquivalente = 1, Observacoes = "Sabor mais doce.", FatorSimilaridade = 0.45 },

            // Substitutos para Arroz (3)
            new AlimentoSubstituto { AlimentoOriginalId = 3, AlimentoSubstitutoRefId = 11, Motivo = "Fonte de proteína e fibras.", ProporcaoEquivalente = 0.9m, Observacoes = "Excelente em pratos orientais.", FatorSimilaridade = 0.7 },
            new AlimentoSubstituto { AlimentoOriginalId = 3, AlimentoSubstitutoRefId = 8, Motivo = "Carboidrato alternativo em acompanhamentos.", ProporcaoEquivalente = 1, Observacoes = "Usar pão torrado para saladas.", FatorSimilaridade = 0.4 },
            new AlimentoSubstituto { AlimentoOriginalId = 3, AlimentoSubstitutoRefId = 12, Motivo = "Alternativa integral rica em fibras.", ProporcaoEquivalente = 0.9m, Observacoes = "Bom para versões mais saudáveis.", FatorSimilaridade = 0.6 },

            // Substitutos para Leite (4)
            new AlimentoSubstituto { AlimentoOriginalId = 4, AlimentoSubstitutoRefId = 10, Motivo = "Alternativa vegetal para intolerantes.", ProporcaoEquivalente = 1, Observacoes = "Usar em bebidas e cereais.", FatorSimilaridade = 0.5 },
            new AlimentoSubstituto { AlimentoOriginalId = 4, AlimentoSubstitutoRefId = 13, Motivo = "Alternativa em receitas que pedem cremosidade.", ProporcaoEquivalente = 0.8m, Observacoes = "Ajustar sal e fermento.", FatorSimilaridade = 0.65 },
            new AlimentoSubstituto { AlimentoOriginalId = 4, AlimentoSubstitutoRefId = 9, Motivo = "Alternativa proteica e sem lactose.", ProporcaoEquivalente = 0.9m, Observacoes = "Usar em molhos e smoothies.", FatorSimilaridade = 0.55 },

            // Substitutos para Frango (5)
            new AlimentoSubstituto { AlimentoOriginalId = 5, AlimentoSubstitutoRefId = 9, Motivo = "Opção vegetal rica em proteína.", ProporcaoEquivalente = 1, Observacoes = "Boa para dietas vegetarianas.", FatorSimilaridade = 0.6 },
            new AlimentoSubstituto { AlimentoOriginalId = 5, AlimentoSubstitutoRefId = 14, Motivo = "Peixe rico em ômega-3 como alternativa.", ProporcaoEquivalente = 1, Observacoes = "Textura diferente, alto valor nutritivo.", FatorSimilaridade = 0.7 },
            new AlimentoSubstituto { AlimentoOriginalId = 5, AlimentoSubstitutoRefId = 11, Motivo = "Fonte proteica vegetal.", ProporcaoEquivalente = 1, Observacoes = "Ótimo em ensopados e saladas.", FatorSimilaridade = 0.5 },
            // 6 - Banana
            new AlimentoSubstituto { AlimentoOriginalId = 6, AlimentoSubstitutoRefId = 1, Motivo = "Fruta similar em sobremesas.", ProporcaoEquivalente = 1, Observacoes = "Troca comum em vitaminas.", FatorSimilaridade = 0.7 },
            new AlimentoSubstituto { AlimentoOriginalId = 6, AlimentoSubstitutoRefId = 15, Motivo = "Alternativa energética.", ProporcaoEquivalente = 1, Observacoes = "Boa para bolos e purês.", FatorSimilaridade = 0.6 },

            // 7 - Batata
            new AlimentoSubstituto { AlimentoOriginalId = 7, AlimentoSubstitutoRefId = 15, Motivo = "Batata doce substitui em assados.", ProporcaoEquivalente = 1, Observacoes = "Mais doce e rica em fibras.", FatorSimilaridade = 0.65 },
            new AlimentoSubstituto { AlimentoOriginalId = 7, AlimentoSubstitutoRefId = 3, Motivo = "Arroz como acompanhamento alternativo.", ProporcaoEquivalente = 1, Observacoes = "Textura diferente.", FatorSimilaridade = 0.4 },

            // 8 - Pão
            new AlimentoSubstituto { AlimentoOriginalId = 8, AlimentoSubstitutoRefId = 3, Motivo = "Arroz pra saladas frias/companhias.", ProporcaoEquivalente = 1, Observacoes = "Usar torrado para textura.", FatorSimilaridade = 0.35 },
            new AlimentoSubstituto { AlimentoOriginalId = 8, AlimentoSubstitutoRefId = 11, Motivo = "Feijão em preparos rústicos.", ProporcaoEquivalente = 1, Observacoes = "Troca incomum mas possível.", FatorSimilaridade = 0.25 },

            // 9 - Tofu
            new AlimentoSubstituto { AlimentoOriginalId = 9, AlimentoSubstitutoRefId = 5, Motivo = "Peito de frango como alternativa proteica.", ProporcaoEquivalente = 1, Observacoes = "Não vegano.", FatorSimilaridade = 0.5 },
            new AlimentoSubstituto { AlimentoOriginalId = 9, AlimentoSubstitutoRefId = 11, Motivo = "Feijão para proteína vegetal.", ProporcaoEquivalente = 1, Observacoes = "Bom para ensopados.", FatorSimilaridade = 0.55 },

            // 10 - Leite de Amêndoa
            new AlimentoSubstituto { AlimentoOriginalId = 10, AlimentoSubstitutoRefId = 4, Motivo = "Leite de vaca quando tolerado.", ProporcaoEquivalente = 1, Observacoes = "Não indicado para intolerantes.", FatorSimilaridade = 0.45 },
            new AlimentoSubstituto { AlimentoOriginalId = 10, AlimentoSubstitutoRefId = 13, Motivo = "Queijo para cremosidade em receitas.", ProporcaoEquivalente = 0.8m, Observacoes = "Requer ajustes.", FatorSimilaridade = 0.4 },

            // 11 - Feijão
            new AlimentoSubstituto { AlimentoOriginalId = 11, AlimentoSubstitutoRefId = 3, Motivo = "Arroz como acompanhamento proteico em conjunto.", ProporcaoEquivalente = 1, Observacoes = "Combina em pratos principais.", FatorSimilaridade = 0.3 },
            new AlimentoSubstituto { AlimentoOriginalId = 11, AlimentoSubstitutoRefId = 12, Motivo = "Aveia em preparos calóricos.", ProporcaoEquivalente = 0.9m, Observacoes = "Uso técnico em receitas.", FatorSimilaridade = 0.25 },

            // 12 - Aveia
            new AlimentoSubstituto { AlimentoOriginalId = 12, AlimentoSubstitutoRefId = 8, Motivo = "Pão integral em substituições de café da manhã.", ProporcaoEquivalente = 1, Observacoes = "Textura diferente.", FatorSimilaridade = 0.5 },
            new AlimentoSubstituto { AlimentoOriginalId = 12, AlimentoSubstitutoRefId = 6, Motivo = "Banana em vitaminas/purês.", ProporcaoEquivalente = 0.8m, Observacoes = "Ajustar quantidade.", FatorSimilaridade = 0.45 },

            // 13 - Queijo
            new AlimentoSubstituto { AlimentoOriginalId = 13, AlimentoSubstitutoRefId = 4, Motivo = "Leite para cremosidade.", ProporcaoEquivalente = 0.9m, Observacoes = "Alterar consistência.", FatorSimilaridade = 0.5 },
            new AlimentoSubstituto { AlimentoOriginalId = 13, AlimentoSubstitutoRefId = 10, Motivo = "Leite vegetal em receitas veganas.", ProporcaoEquivalente = 1, Observacoes = "Ajustar sabor.", FatorSimilaridade = 0.35 },

            // 14 - Salmão
            new AlimentoSubstituto { AlimentoOriginalId = 14, AlimentoSubstitutoRefId = 5, Motivo = "Frango como alternativa de proteína.", ProporcaoEquivalente = 1, Observacoes = "Menos ômega-3.", FatorSimilaridade = 0.5 },
            new AlimentoSubstituto { AlimentoOriginalId = 14, AlimentoSubstitutoRefId = 11, Motivo = "Feijão como fonte proteica vegetal.", ProporcaoEquivalente = 1, Observacoes = "Mudança de perfil nutricional.", FatorSimilaridade = 0.3 },

            // 15 - Batata Doce
            new AlimentoSubstituto { AlimentoOriginalId = 15, AlimentoSubstitutoRefId = 7, Motivo = "Batata comum como substituto em receitas.", ProporcaoEquivalente = 1, Observacoes = "Menos beta-caroteno.", FatorSimilaridade = 0.6 },
            new AlimentoSubstituto { AlimentoOriginalId = 15, AlimentoSubstitutoRefId = 3, Motivo = "Arroz como acompanhamento alternativo.", ProporcaoEquivalente = 1, Observacoes = "Uso em guarnições.", FatorSimilaridade = 0.25 }
        );

        context.SaveChanges();
    }

    private static void PopulateComponentesReceita(HealthWellbeingDbContext context)
    {
        if (context.ComponenteReceita.Any())
        {
            return;
        }

        var receitas = context.Receita.ToList();
        if (!receitas.Any())
        {
            return;
        }

        var receitaIds = receitas.Select(r => r.ReceitaId).ToArray();
        var idx = 0;
        Func<int> nextReceitaId = () =>
        {
            var id = receitaIds[idx % receitaIds.Length];
            idx++;
            return id;
        };

        context.ComponenteReceita.AddRange(
            new ComponenteReceita { AlimentoId = 1, UnidadeMedida = UnidadeMedidaEnum.Grama, Quantidade = 100, IsOpcional = false, ReceitaId = nextReceitaId() },
            new ComponenteReceita { AlimentoId = 2, UnidadeMedida = UnidadeMedidaEnum.Mililitro, Quantidade = 200, IsOpcional = true, ReceitaId = nextReceitaId() },
            new ComponenteReceita { AlimentoId = 3, UnidadeMedida = UnidadeMedidaEnum.Xicara, Quantidade = 1, IsOpcional = false, ReceitaId = nextReceitaId() },
            new ComponenteReceita { AlimentoId = 4, UnidadeMedida = UnidadeMedidaEnum.ColherDeSopa, Quantidade = 2, IsOpcional = true, ReceitaId = nextReceitaId() },
            new ComponenteReceita { AlimentoId = 5, UnidadeMedida = UnidadeMedidaEnum.Unidade, Quantidade = 1, IsOpcional = false, ReceitaId = nextReceitaId() },

            new ComponenteReceita { AlimentoId = 6, UnidadeMedida = UnidadeMedidaEnum.Unidade, Quantidade = 1, IsOpcional = false, ReceitaId = nextReceitaId() },
            new ComponenteReceita { AlimentoId = 7, UnidadeMedida = UnidadeMedidaEnum.Grama, Quantidade = 150, IsOpcional = false, ReceitaId = nextReceitaId() },
            new ComponenteReceita { AlimentoId = 8, UnidadeMedida = UnidadeMedidaEnum.Fatia, Quantidade = 2, IsOpcional = true, ReceitaId = nextReceitaId() },
            new ComponenteReceita { AlimentoId = 9, UnidadeMedida = UnidadeMedidaEnum.Grama, Quantidade = 120, IsOpcional = false, ReceitaId = nextReceitaId() },
            new ComponenteReceita { AlimentoId = 10, UnidadeMedida = UnidadeMedidaEnum.Mililitro, Quantidade = 200, IsOpcional = false, ReceitaId = nextReceitaId() },
            new ComponenteReceita { AlimentoId = 11, UnidadeMedida = UnidadeMedidaEnum.Grama, Quantidade = 100, IsOpcional = false, ReceitaId = nextReceitaId() },
            new ComponenteReceita { AlimentoId = 12, UnidadeMedida = UnidadeMedidaEnum.ColherDeSopa, Quantidade = 3, IsOpcional = true, ReceitaId = nextReceitaId() },
            new ComponenteReceita { AlimentoId = 13, UnidadeMedida = UnidadeMedidaEnum.Grama, Quantidade = 50, IsOpcional = true, ReceitaId = nextReceitaId() },
            new ComponenteReceita { AlimentoId = 14, UnidadeMedida = UnidadeMedidaEnum.Grama, Quantidade = 120, IsOpcional = false, ReceitaId = nextReceitaId() },
            new ComponenteReceita { AlimentoId = 15, UnidadeMedida = UnidadeMedidaEnum.Grama, Quantidade = 150, IsOpcional = false, ReceitaId = nextReceitaId() },

            new ComponenteReceita { AlimentoId = 6, UnidadeMedida = UnidadeMedidaEnum.Grama, Quantidade = 80, IsOpcional = true, ReceitaId = nextReceitaId() },
            new ComponenteReceita { AlimentoId = 11, UnidadeMedida = UnidadeMedidaEnum.Xicara, Quantidade = 1, IsOpcional = false, ReceitaId = nextReceitaId() },
            new ComponenteReceita { AlimentoId = 7, UnidadeMedida = UnidadeMedidaEnum.Unidade, Quantidade = 1, IsOpcional = true, ReceitaId = nextReceitaId() },
            new ComponenteReceita { AlimentoId = 10, UnidadeMedida = UnidadeMedidaEnum.ColherDeSopa, Quantidade = 4, IsOpcional = false, ReceitaId = nextReceitaId() },
            new ComponenteReceita { AlimentoId = 12, UnidadeMedida = UnidadeMedidaEnum.Grama, Quantidade = 40, IsOpcional = true, ReceitaId = nextReceitaId() }
        );

        context.SaveChanges();
    }

    private static void PopulateFoodHabits(HealthWellbeingDbContext context)
    {
        if (context.FoodHabitsPlan != null && !context.FoodHabitsPlan.Any())
        {
            var plans = new List<FoodHabitsPlan>();
            DateTime today = DateTime.Today;
            var clients = context.Client.OrderBy(c => c.ClientId).ToList();

            for (int i = 0; i < 30; i++)
            {
                plans.Add(new FoodHabitsPlan
                {
                    ClientId = clients[i].ClientId,
                    StartingDate = today.AddDays(-i * 7),
                    EndingDate = today.AddDays(-i * 7 + 30),
                    Done = i % 3 == 0
                });
            }

            context.FoodHabitsPlan.AddRange(plans);
            context.SaveChanges();
        }
    }

    private static void PopulateReceitas(HealthWellbeingDbContext context)
    {
        if (context.Receita.Any())
        {
            return;
        }

        context.Receita.AddRange(
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
        );

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

    private static void PopulateMember(HealthWellbeingDbContext dbContext, List<Client> clients)
    {
        if (dbContext.Member.Any()) return;

        var clientNamesToMakeMembers = new List<string> { "Alice Wonderland", "Charlie Brown", "David Copperfield" };

        var members = clients
            .Where(c => clientNamesToMakeMembers.Contains(c.Name))
            .Select(c => new Member
            {
                ClientId = c.ClientId,
            })
            .ToList();

        if (members.Any())
        {
            dbContext.Member.AddRange(members);
            dbContext.SaveChanges();
        }
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
