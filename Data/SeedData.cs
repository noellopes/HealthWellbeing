using HealthWellbeing.Data;
using HealthWellbeing.Models;

internal class SeedData
{
    public static void Populate(HealthWellbeingDbContext? context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        context.Database.EnsureCreated();

        PopulateCategorias(context);
        PopulateAlimentos(context);
        PopulateAlergias(context);
        PopulateRestricoesAlimentares(context);
        PopulateAlimentoSubstitutos(context);
        PopulateComponentesReceita(context);
        PopulateReceitas(context);
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
            // Seeds existentes
            new AlimentoSubstituto { AlimentoOriginalId = 1, AlimentoSubstitutoRefId = 2, Motivo = "Alternativa para alergia.", ProporcaoEquivalente = 1, Observacoes = "Substituição recomendada.", FatorSimilaridade = 0.8 },
            new AlimentoSubstituto { AlimentoOriginalId = 3, AlimentoSubstitutoRefId = 4, Motivo = "Alternativa para intolerância ao glúten.", ProporcaoEquivalente = 1, Observacoes = "Substituição recomendada.", FatorSimilaridade = 0.9 },
            new AlimentoSubstituto { AlimentoOriginalId = 5, AlimentoSubstitutoRefId = 1, Motivo = "Alternativa para dieta vegana.", ProporcaoEquivalente = 1, Observacoes = "Substituição recomendada.", FatorSimilaridade = 0.7 },
            new AlimentoSubstituto { AlimentoOriginalId = 2, AlimentoSubstitutoRefId = 3, Motivo = "Alternativa para restrição alimentar.", ProporcaoEquivalente = 1, Observacoes = "Substituição recomendada.", FatorSimilaridade = 0.85 },
            new AlimentoSubstituto { AlimentoOriginalId = 4, AlimentoSubstitutoRefId = 5, Motivo = "Alternativa para alergia ao leite.", ProporcaoEquivalente = 1, Observacoes = "Substituição recomendada.", FatorSimilaridade = 0.75 },

            // +15 novos exemplos (garantindo múltiplos substitutos para os originais 1..5)
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
            new AlimentoSubstituto { AlimentoOriginalId = 5, AlimentoSubstitutoRefId = 11, Motivo = "Fonte proteica vegetal.", ProporcaoEquivalente = 1, Observacoes = "Ótimo em ensopados e saladas.", FatorSimilaridade = 0.5 }
        );

        context.SaveChanges();
    }

    private static void PopulateComponentesReceita(HealthWellbeingDbContext context)
    {
        if (context.ComponenteReceita.Any())
        {
            return;
        }

        context.ComponenteReceita.AddRange(
            new ComponenteReceita { AlimentoId = 1, UnidadeMedida = UnidadeMedidaEnum.Grama, Quantidade = 100, IsOpcional = false },
            new ComponenteReceita { AlimentoId = 2, UnidadeMedida = UnidadeMedidaEnum.Mililitro, Quantidade = 200, IsOpcional = true },
            new ComponenteReceita { AlimentoId = 3, UnidadeMedida = UnidadeMedidaEnum.Xicara, Quantidade = 1, IsOpcional = false },
            new ComponenteReceita { AlimentoId = 4, UnidadeMedida = UnidadeMedidaEnum.ColherDeSopa, Quantidade = 2, IsOpcional = true },
            new ComponenteReceita { AlimentoId = 5, UnidadeMedida = UnidadeMedidaEnum.Unidade, Quantidade = 1, IsOpcional = false },

            // +15 novos componentes usando alimentos 6..15
            new ComponenteReceita { AlimentoId = 6, UnidadeMedida = UnidadeMedidaEnum.Unidade, Quantidade = 1, IsOpcional = false },        // Banana inteira
            new ComponenteReceita { AlimentoId = 7, UnidadeMedida = UnidadeMedidaEnum.Grama, Quantidade = 150, IsOpcional = false },        // Batata
            new ComponenteReceita { AlimentoId = 8, UnidadeMedida = UnidadeMedidaEnum.Fatia, Quantidade = 2, IsOpcional = true },         // Pão (usar enum apropriado se existir)
            new ComponenteReceita { AlimentoId = 9, UnidadeMedida = UnidadeMedidaEnum.Grama, Quantidade = 120, IsOpcional = false },        // Tofu
            new ComponenteReceita { AlimentoId = 10, UnidadeMedida = UnidadeMedidaEnum.Mililitro, Quantidade = 200, IsOpcional = false },   // Leite de Amêndoa
            new ComponenteReceita { AlimentoId = 11, UnidadeMedida = UnidadeMedidaEnum.Grama, Quantidade = 100, IsOpcional = false },       // Feijão
            new ComponenteReceita { AlimentoId = 12, UnidadeMedida = UnidadeMedidaEnum.ColherDeSopa, Quantidade = 3, IsOpcional = true },   // Aveia (ex: colheres)
            new ComponenteReceita { AlimentoId = 13, UnidadeMedida = UnidadeMedidaEnum.Grama, Quantidade = 50, IsOpcional = true },        // Queijo (opcional)
            new ComponenteReceita { AlimentoId = 14, UnidadeMedida = UnidadeMedidaEnum.Grama, Quantidade = 120, IsOpcional = false },       // Salmão
            new ComponenteReceita { AlimentoId = 15, UnidadeMedida = UnidadeMedidaEnum.Grama, Quantidade = 150, IsOpcional = false },       // Batata Doce

            // Alguns componentes adicionais reutilizando alimentos para receitas variáveis
            new ComponenteReceita { AlimentoId = 6, UnidadeMedida = UnidadeMedidaEnum.Grama, Quantidade = 80, IsOpcional = true },         // Banana (purê)
            new ComponenteReceita { AlimentoId = 11, UnidadeMedida = UnidadeMedidaEnum.Xicara, Quantidade = 1, IsOpcional = false },       // Feijão (cozido)
            new ComponenteReceita { AlimentoId = 7, UnidadeMedida = UnidadeMedidaEnum.Unidade, Quantidade = 1, IsOpcional = true },        // Batata inteira como guarnição
            new ComponenteReceita { AlimentoId = 10, UnidadeMedida = UnidadeMedidaEnum.ColherDeSopa, Quantidade = 4, IsOpcional = false }, // Leite de Amêndoa (colheres para molho)
            new ComponenteReceita { AlimentoId = 12, UnidadeMedida = UnidadeMedidaEnum.Grama, Quantidade = 40, IsOpcional = true }         // Aveia (granola)
        );

        context.SaveChanges();
    }

    private static void PopulateReceitas(HealthWellbeingDbContext context)
    {
        if (context.Receita.Any())
        {
            return;
        }

        context.Receita.AddRange(
            new Receita
            {
                Nome = "Salada de Frutas",
                Descricao = "Uma deliciosa salada de frutas.",
                ModoPreparo = "Misture as frutas em uma tigela.",
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
                Nome = "Sopa de Legumes",
                Descricao = "Sopa nutritiva e saborosa.",
                ModoPreparo = "Cozinhe os legumes e bata no liquidificador.",
                TempoPreparo = 30,
                Porcoes = 4,
                Calorias = 100,
                Proteinas = 3,
                HidratosCarbono = 20,
                Gorduras = 2,
                RestricoesAlimentarId = new List<int> { 4 }
            },
            new Receita { Nome = "Arroz Integral", Descricao = "Arroz saudável e rico em fibras.", ModoPreparo = "Cozinhe o arroz com água e sal.", TempoPreparo = 25, Porcoes = 3, Calorias = 130, Proteinas = 4, HidratosCarbono = 28, Gorduras = 1 },
            new Receita { Nome = "Frango Grelhado", Descricao = "Frango grelhado com temperos.", ModoPreparo = "Tempere o frango e grelhe até dourar.", TempoPreparo = 20, Porcoes = 2, Calorias = 200, Proteinas = 35, HidratosCarbono = 0, Gorduras = 5 },
            new Receita { Nome = "Vitamina de Banana", Descricao = "Bebida saudável e energética.", ModoPreparo = "Bata a banana com leite no liquidificador.", TempoPreparo = 5, Porcoes = 1, Calorias = 120, Proteinas = 5, HidratosCarbono = 25, Gorduras = 2 }
        );

        context.SaveChanges();
    }
}
