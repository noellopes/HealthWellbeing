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

            // Chama o método que insere os dados
            PopulateMaterialEquipamentoAssociado(dbContext);


        }

        private static void PopulateMaterialEquipamentoAssociado(HealthWellbeingDbContext dbContext) // <<-- Nome do DbContext Corrigido
        {

            if (dbContext.MaterialEquipamentoAssociado.Any()) return;


            var materialEquipamentoAssociado = new[]
            {
                // 1
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Seringa Descartável 5ml",
                    Quantidade = 500, // Número inteiro
                    EstadoComponente = "Em Stock (Novo)",
                },
                // 2
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Compressa de Gaze Esterilizada",
                    Quantidade = 1200, // Número inteiro
                    EstadoComponente = "Em Stock (Novo)",
                },
                // 3
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Monitor de Sinais Vitais",
                    Quantidade = 15, // Número inteiro
                    EstadoComponente = "Em Utilização (Bom Estado)",
                },
                // 4
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Eletrocardiógrafo Portátil",
                    Quantidade = 5, // Número inteiro
                    EstadoComponente = "Em Manutenção (Calibração)",
                },
                // 5
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Luvas de Nitrilo (Caixa)",
                    Quantidade = 80, // Número inteiro
                    EstadoComponente = "Em Stock (Novo)",
                },
                // 6
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Cadeira de Rodas Standard",
                    Quantidade = 25, // Número inteiro
                    EstadoComponente = "Em Utilização (Pequenos Desgastes)",
                },
                // 7
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Bomba de Infusão Volumétrica",
                    Quantidade = 40, // Número inteiro
                    EstadoComponente = "Em Utilização (Excelente Estado)",
                },
                // 8
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Termómetro Digital de Testa",
                    Quantidade = 95, // Número inteiro
                    EstadoComponente = "Em Stock (Novo)",
                },
                // 9
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Aspirador Cirúrgico",
                    Quantidade = 8, // Número inteiro
                    EstadoComponente = "Em Utilização (Requer Filtro Novo)",
                },
                // 10
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Mesa de Cirurgia Multiusos",
                    Quantidade = 3, // Número inteiro
                    EstadoComponente = "Em Utilização (Bom Estado)",
                },
                // 11
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Kit de Sutura Estéril",
                    Quantidade = 300, // Número inteiro
                    EstadoComponente = "Em Stock (Novo)",
                },
                // 12
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Bisturi Descartável (Unidade)",
                    Quantidade = 1500, // Número inteiro
                    EstadoComponente = "Em Stock (Novo)",
                },
                // 13
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Ventilador Pulmonar",
                    Quantidade = 12, // Número inteiro
                    EstadoComponente = "Em Manutenção (Preventiva)",
                },
                // 14
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Carro de Emergência (Completo)",
                    Quantidade = 6, // Número inteiro
                    EstadoComponente = "Em Utilização (Pronto a Usar)",
                },
                // 15
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Agulha Hipodérmica 21G",
                    Quantidade = 2000, // Número inteiro
                    EstadoComponente = "Em Stock (Novo)",
                },
                // 16
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Otoscópio/Oftalmoscópio",
                    Quantidade = 18, // Número inteiro
                    EstadoComponente = "Em Utilização (Lâmpada Fraca)",
                },
                // 17
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Tala Imobilizadora (Vários Tamanhos)",
                    Quantidade = 75, // Número inteiro
                    EstadoComponente = "Em Stock (Novo)",
                },
                // 18
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Esfigmomanómetro Digital",
                    Quantidade = 35, // Número inteiro
                    EstadoComponente = "Em Utilização (Bom Estado)",
                },
                // 19
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Mascára Cirúrgica N95",
                    Quantidade = 1000, // Número inteiro
                    EstadoComponente = "Em Stock (Novo)",
                },
                // 20
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Laringoscópio Completo",
                    Quantidade = 7, // Número inteiro
                    EstadoComponente = "Em Utilização (Excelente Estado)",
                },
                // 21
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Fato de Proteção Biológica",
                    Quantidade = 150, // Número inteiro
                    EstadoComponente = "Em Stock (Novo)",
                },
                // 22
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Desfibrilhador Externo Automático (DEA)",
                    Quantidade = 10, // Número inteiro
                    EstadoComponente = "Em Utilização (Requer Troca de Bateria)",
                },
                // 23
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Pilha Alcalina AA (Pack de 10)",
                    Quantidade = 20, // Número inteiro
                    EstadoComponente = "Em Stock (Novo)",
                },
                // 24
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Estetoscópio Littmann",
                    Quantidade = 55, // Número inteiro
                    EstadoComponente = "Em Utilização (Bom Estado)",
                },
                // 25
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Balança Hospitalar Digital",
                    Quantidade = 4, // Número inteiro
                    EstadoComponente = "Em Manutenção (Reparo no Display)",
                },
                // 26
                new MaterialEquipamentoAssociado
                {
                    NomeEquipamento = "Gesso Ortopédico (Rolo)",
                    Quantidade = 90, // Número inteiro
                    EstadoComponente = "Em Stock (Novo)",
                },
            };

            dbContext.MaterialEquipamentoAssociado.AddRange(materialEquipamentoAssociado);
            dbContext.SaveChanges();
        }
    }
}