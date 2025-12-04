// Data/SeedDataProfissionalExecutante.cs (CORRIGIDO)
using HealthWellbeing.Models;
using HealthWellBeing.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HealthWellbeing.Data
{
    public class SeedDataMaterialEquipamentoAssociado
    {
        public static void Populate(HealthWellbeingDbContext dbContext)
        {
            if (dbContext == null) return;
            // 1. Deve popular as Funções primeiro
            PopulateEstadoMaterial(dbContext);
            // Chama o método que insere os dados
            PopulateMaterialEquipamentoAssociado(dbContext);
        }

        private static void PopulateEstadoMaterial(HealthWellbeingDbContext dbContext)
        {
            // Se já houver dados, não insere novamente
            if (dbContext.EstadosMaterial.Any()) return;

            var estados = new[]
            {
                new EstadoMaterial { Nome = "Disponível", Descricao = "Material pronto para utilização" },
                new EstadoMaterial { Nome = "Em Uso", Descricao = "Material atualmente utilizado" },
                new EstadoMaterial { Nome = "Reservado", Descricao = "Material reservado para utilização futura" },
                new EstadoMaterial { Nome = "Em Manutenção", Descricao = "Material em reparação/manutenção" },
                new EstadoMaterial { Nome = "Danificado", Descricao = "Material danificado e não utilizável" },
                new EstadoMaterial { Nome = "Perdido", Descricao = "Material não localizado" }
            };

            dbContext.EstadosMaterial.AddRange(estados);
            dbContext.SaveChanges();
        }

        private static void PopulateMaterialEquipamentoAssociado(HealthWellbeingDbContext dbContext)
        {

            if (dbContext.MaterialEquipamentoAssociado.Any()) return;


            // 1. Carrega todos os EstadosMaterial existentes
            var estadosDb = dbContext.EstadosMaterial.ToList();

            // 2. Mapeamento dos Estados (usando a ENTIDADE completa)
            // NOTA: Usamos 'First' com segurança aqui porque o PopulateEstadoMaterial foi chamado e fez SaveChanges()
            // Isso só funciona porque o SaveChanges já ocorreu e os IDs foram gerados.
            var disponivel = estadosDb.First(e => e.Nome == "Disponível");
            var emUso = estadosDb.First(e => e.Nome == "Em Uso");
            var emManutencao = estadosDb.First(e => e.Nome == "Em Manutenção");

            var materialEquipamentoAssociado = new[]
            {
            // 1
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Seringa Descartável 5ml",
                    Quantidade = 500,
                    EstadoMaterial = disponivel
                },
                // 2
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Compressa de Gaze Esterilizada",
                    Quantidade = 1200,
                    EstadoMaterial = disponivel
                },
                // 3
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Monitor de Sinais Vitais",
                    Quantidade = 15,
                    EstadoMaterial = emUso
                },
                // 4
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Eletrocardiógrafo Portátil",
                    Quantidade = 5,
                    EstadoMaterial = emManutencao
                },
                // 5
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Luvas de Nitrilo (Caixa)",
                    Quantidade = 80,
                    EstadoMaterial = disponivel
                },
                // 6
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Cadeira de Rodas Standard",
                    Quantidade = 25,
                    EstadoMaterial = emUso
                },
                // 7
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Bomba de Infusão Volumétrica",
                    Quantidade = 40,
                    EstadoMaterial = emUso
                },
                // 8
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Termómetro Digital de Testa",
                    Quantidade = 95,
                    EstadoMaterial = disponivel
                },
                // 9
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Aspirador Cirúrgico",
                    Quantidade = 8,
                    EstadoMaterial = emUso
                },
                // 10
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Mesa de Cirurgia Multiusos",
                    Quantidade = 3,
                    EstadoMaterial = emUso
                },
                // 11
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Kit de Sutura Estéril",
                    Quantidade = 300,
                    EstadoMaterial = disponivel
                },
                // 12
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Bisturi Descartável (Unidade)",
                    Quantidade = 1500,
                    EstadoMaterial = disponivel
                },
                // 13
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Ventilador Pulmonar",
                    Quantidade = 12,
                    EstadoMaterial = emManutencao
                },
                // 14
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Carro de Emergência (Completo)",
                    Quantidade = 6,
                    EstadoMaterial = emUso
                },
                // 15
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Agulha Hipodérmica 21G",
                    Quantidade = 2000,
                    EstadoMaterial = disponivel
                },
                // 16
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Otoscópio/Oftalmoscópio",
                    Quantidade = 18,
                    EstadoMaterial = emUso
                },
                // 17
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Tala Imobilizadora (Vários Tamanhos)",
                    Quantidade = 75,
                    EstadoMaterial = disponivel
                },
                // 18
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Esfigmomanómetro Digital",
                    Quantidade = 35,
                    EstadoMaterial = disponivel
                },
                // 19
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Mascára Cirúrgica N95",
                    Quantidade = 1000,
                    EstadoMaterial = disponivel
                },
                // 20
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Laringoscópio Completo",
                    Quantidade = 7,
                    EstadoMaterial = emUso
                },
                // 21
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Fato de Proteção Biológica",
                    Quantidade = 150,
                    EstadoMaterial = disponivel
                },
                // 22
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Desfibrilhador Externo Automático (DEA)",
                    Quantidade = 10,
                    EstadoMaterial = emUso
                },
                // 23
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Pilha Alcalina AA (Pack de 10)",
                    Quantidade = 20,
                    EstadoMaterial = disponivel
                },
                // 24
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Estetoscópio Littmann",
                    Quantidade = 55,
                    EstadoMaterial = emUso
                },
                // 25
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Balança Hospitalar Digital",
                    Quantidade = 4,
                    EstadoMaterial = emManutencao
                },
                // 26
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Gesso Ortopédico (Rolo)",
                    Quantidade = 90,
                    EstadoMaterial = disponivel
                },
            };

            dbContext.MaterialEquipamentoAssociado.AddRange(materialEquipamentoAssociado);
            dbContext.SaveChanges();
        }
    }
}   