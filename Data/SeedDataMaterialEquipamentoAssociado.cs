// Data/SeedDataProfissionalExecutante.cs (CORRIGIDO)
using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellBeing.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HealthWellbeing.Data // <<-- Corrigido o namespace se necessário
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



        private static void PopulateMaterialEquipamentoAssociado(HealthWellbeingDbContext dbContext) // <<-- Nome do DbContext Corrigido
        {

            if (dbContext.MaterialEquipamentoAssociado.Any()) return;


            // 1. Carrega todos os EstadosMaterial existentes
            var estadosDb = dbContext.EstadosMaterial.ToList();

            // 2. Cria a função de mapeamento NomeEstado -> MaterialStatusId
            // NOTA: Usamos 'First' com segurança aqui porque o PopulateEstadoMaterial foi chamado e fez SaveChanges()
            Func<string, int> getStatusId = nome =>
                estadosDb.First(e => e.Nome == nome).MaterialStatusId;

            // Mapeamento dos Estados

            int disponivelId = getStatusId("Disponível");
            int emUsoId = getStatusId("Em Uso");
            int emManutencaoId = getStatusId("Em Manutenção");

            var materialEquipamentoAssociado = new[]
            {
            // 1
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Seringa Descartável 5ml",
                    Quantidade = 500,
                    MaterialStatusId = disponivelId,
                },
                // 2
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Compressa de Gaze Esterilizada",
                    Quantidade = 1200,
                    MaterialStatusId = disponivelId,
                },
                // 3
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Monitor de Sinais Vitais",
                    Quantidade = 15,
                    MaterialStatusId = emUsoId,
                },
                // 4
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Eletrocardiógrafo Portátil",
                    Quantidade = 5,
                    MaterialStatusId = emManutencaoId,
                },
                // 5
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Luvas de Nitrilo (Caixa)",
                    Quantidade = 80,
                    MaterialStatusId = disponivelId,
                },
                // 6
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Cadeira de Rodas Standard",
                    Quantidade = 25,
                    MaterialStatusId = emUsoId,
                },
                // 7
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Bomba de Infusão Volumétrica",
                    Quantidade = 40,
                    MaterialStatusId = emUsoId,
                },
                // 8
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Termómetro Digital de Testa",
                    Quantidade = 95,
                    MaterialStatusId = disponivelId,
                },
                // 9
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Aspirador Cirúrgico",
                    Quantidade = 8,
                    MaterialStatusId = emUsoId,
                },
                // 10
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Mesa de Cirurgia Multiusos",
                    Quantidade = 3,
                    MaterialStatusId = emUsoId,
                },
                // 11
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Kit de Sutura Estéril",
                    Quantidade = 300,
                    MaterialStatusId = disponivelId,
                },
                // 12
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Bisturi Descartável (Unidade)",
                    Quantidade = 1500,
                    MaterialStatusId = disponivelId,
                },
                // 13
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Ventilador Pulmonar",
                    Quantidade = 12,
                    MaterialStatusId = emManutencaoId,
                },
                // 14
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Carro de Emergência (Completo)",
                    Quantidade = 6,
                    MaterialStatusId = emUsoId,
                },
                // 15
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Agulha Hipodérmica 21G",
                    Quantidade = 2000,
                    MaterialStatusId = disponivelId,
                },
                // 16
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Otoscópio/Oftalmoscópio",
                    Quantidade = 18,
                    MaterialStatusId = emUsoId,
                },
                // 17
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Tala Imobilizadora (Vários Tamanhos)",
                    Quantidade = 75,
                    MaterialStatusId = disponivelId,
                },
                // 18
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Esfigmomanómetro Digital",
                    Quantidade = 35,
                    MaterialStatusId = emUsoId,
                },
                // 19
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Mascára Cirúrgica N95",
                    Quantidade = 1000,
                    MaterialStatusId = disponivelId,
                },
                // 20
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Laringoscópio Completo",
                    Quantidade = 7,
                    MaterialStatusId = emUsoId,
                },
                // 21
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Fato de Proteção Biológica",
                    Quantidade = 150,
                    MaterialStatusId = disponivelId,
                },
                // 22
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Desfibrilhador Externo Automático (DEA)",
                    Quantidade = 10,
                    MaterialStatusId = emUsoId,
                },
                // 23
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Pilha Alcalina AA (Pack de 10)",
                    Quantidade = 20,
                    MaterialStatusId = disponivelId,
                },
                // 24
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Estetoscópio Littmann",
                    Quantidade = 55,
                    MaterialStatusId = emUsoId,
                },
                // 25
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Balança Hospitalar Digital",
                    Quantidade = 4,
                    MaterialStatusId = emManutencaoId,
                },
                // 26
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Gesso Ortopédico (Rolo)",
                    Quantidade = 90,
                    MaterialStatusId = disponivelId,
                },
            };

            dbContext.MaterialEquipamentoAssociado.AddRange(materialEquipamentoAssociado);
            dbContext.SaveChanges();
        }
    }
}