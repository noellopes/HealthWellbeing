using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellBeing.Models; // Usings corrigidos
using Microsoft.EntityFrameworkCore; // Para o .Any()
using System;
using System.Collections.Generic;
using System.Linq;

// Este deve ser um arquivo .cs separado na sua pasta Data ou Seeds
public static class SeedDataProfissionalExecutante
{
    public static void Populate(HealthWellbeingDbContext dbContext)
    {
        if (dbContext == null) return;

        // 1. Deve popular as Funções primeiro
        PopulateFuncoes(dbContext);

        // 2. Deve popular os Profissionais em seguida
        PopulateProfissionais(dbContext);
    }

    // ---------------------------
    // SEED DAS FUNÇÕES (Entidade Pai)
    // ---------------------------
    private static void PopulateFuncoes(HealthWellbeingDbContext dbContext)
    {
        // Se as Funções já foram inseridas (na Migração ou em outro Seed), não faz nada
        if (dbContext.Funcoes.Any()) return;

        var funcoes = new[]
        {
            // Nota: O professor disse que você já tem 15 funções. 
            // Se a migração já inseriu, esta seção NUNCA será executada.
            // Se esta é a única fonte de dados, insira aqui. Assumindo que a migração já as inseriu:
            
            new Funcao { NomeFuncao = "Técnico de Radiologia" },
            new Funcao { NomeFuncao = "Fisioterapeuta" },
            new Funcao { NomeFuncao = "Técnico de Cardiopneumologia" },
            new Funcao { NomeFuncao = "Técnico de Análises Clínicas" },
            new Funcao { NomeFuncao = "Terapeuta Ocupacional" },
            new Funcao { NomeFuncao = "Ortopedista" },
            new Funcao { NomeFuncao = "Enfermeiro Especialista" },
            new Funcao { NomeFuncao = "Nutricionista" },
            new Funcao { NomeFuncao = "Técnico de Medicina Nuclear" },
            new Funcao { NomeFuncao = "Cardiologista" },
            new Funcao { NomeFuncao = "Podologista" },
            new Funcao { NomeFuncao = "Técnico de Neurofisiologia" },
            new Funcao { NomeFuncao = "Técnico Auxiliar de Saúde" },
            new Funcao { NomeFuncao = "Optometrista" },
            new Funcao { NomeFuncao = "Técnico de Medicina Física e Reabilitação" },
        };

        dbContext.Funcoes.AddRange(funcoes);
        dbContext.SaveChanges();
    }

    // ---------------------------
    // SEED DOS PROFISSIONAIS (Entidade Filha)
    // ---------------------------
    private static void PopulateProfissionais(HealthWellbeingDbContext dbContext)
    {
        if (dbContext.ProfissionalExecutante.Any()) return;

        // 1. Carrega todas as Funções existentes (inseridas acima ou na Migração)
        var funcoesDb = dbContext.Funcoes.ToList();

        // 2. Cria a função de mapeamento NomeFuncao -> FuncaoId
        Func<string, int> getFuncaoId = nome =>
            funcoesDb.First(f => f.NomeFuncao == nome).FuncaoId;

        var profissionais = new[]
        {
            new ProfissionalExecutante
            {
                Nome = "André Kandonga",
                FuncaoId = getFuncaoId("Técnico de Radiologia"), // AGORA USA FuncaoId
                Telefone = "912912915",
                Email = "Kandonga123@gmail.com"
            },
            new ProfissionalExecutante
            {
                Nome = "Miguel Santos",
                FuncaoId = getFuncaoId("Fisioterapeuta"),
                Telefone = "912912914",
                Email = "MiguelSantos123@gmail.com"
            },
            new ProfissionalExecutante
            {
                Nome = "Dostoevsky",
                FuncaoId = getFuncaoId("Técnico de Cardiopneumologia"),
                Telefone = "912913914",
                Email = "DostoevskySuba@gmail.com"
            },
            new ProfissionalExecutante
            {
                Nome = "Ricardo Quaresma",
                FuncaoId = getFuncaoId("Técnico de Análises Clínicas"),
                Telefone = "910101010",
                Email = "QuaresmaPorto@gmail.com"
            },
            new ProfissionalExecutante
            {
                Nome = "Mai Da Silva",
                FuncaoId = getFuncaoId("Terapeuta Ocupacional"),
                Telefone = "912912222",
                Email = "Mai123222suba@gmail.com",
            },
            new ProfissionalExecutante
            {
                Nome = "Diogo Rodrigues",
                FuncaoId = getFuncaoId("Ortopedista"),
                Telefone = "912912522",
                Email = "DiogoRodrigues04@gmail.com",
            },
        };

        dbContext.ProfissionalExecutante.AddRange(profissionais);
        dbContext.SaveChanges();
    }
}