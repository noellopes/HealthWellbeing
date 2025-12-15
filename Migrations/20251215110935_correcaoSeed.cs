using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class correcaoSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "EstadosMaterial",
                columns: new[] { "MaterialStatusId", "Descricao", "Nome" },
                values: new object[,]
                {
                    { 1, "Material pronto para utilização", "Disponível" },
                    { 2, "Material atualmente utilizado", "Em Uso" },
                    { 3, "Material reservado para utilização futura", "Reservado" },
                    { 4, "Material em reparação/manutenção", "Em Manutenção" },
                    { 5, "Material danificado e não utilizável", "Danificado" },
                    { 6, "Material não localizado", "Perdido" }
                });

            migrationBuilder.InsertData(
                table: "MaterialEquipamentoAssociado",
                columns: new[] { "MaterialEquipamentoAssociadoId", "EstadoMaterialMaterialStatusId", "MaterialStatusId", "NomeEquipamento", "Quantidade" },
                values: new object[,]
                {
                    { 1, null, 1, "Seringa Descartável 5ml", 500 },
                    { 2, null, 1, "Compressa de Gaze Esterilizada", 1200 },
                    { 3, null, 2, "Monitor de Sinais Vitais", 15 },
                    { 4, null, 4, "Eletrocardiógrafo Portátil", 5 },
                    { 5, null, 1, "Luvas de Nitrilo (Caixa)", 80 },
                    { 6, null, 2, "Cadeira de Rodas Standard", 25 },
                    { 7, null, 2, "Bomba de Infusão Volumétrica", 40 },
                    { 8, null, 1, "Termómetro Digital de Testa", 95 },
                    { 9, null, 2, "Aspirador Cirúrgico", 8 },
                    { 10, null, 2, "Mesa de Cirurgia Multiusos", 3 },
                    { 11, null, 1, "Kit de Sutura Estéril", 300 },
                    { 12, null, 1, "Bisturi Descartável (Unidade)", 1500 },
                    { 13, null, 4, "Ventilador Pulmonar", 12 },
                    { 14, null, 2, "Carro de Emergência (Completo)", 6 },
                    { 15, null, 1, "Agulha Hipodérmica 21G", 2000 },
                    { 16, null, 2, "Otoscópio/Oftalmoscópio", 18 },
                    { 17, null, 1, "Tala Imobilizadora (Vários Tamanhos)", 75 },
                    { 18, null, 1, "Esfigmomanómetro Digital", 35 },
                    { 19, null, 1, "Mascára Cirúrgica N95", 1000 },
                    { 20, null, 2, "Laringoscópio Completo", 7 },
                    { 21, null, 1, "Fato de Proteção Biológica", 150 },
                    { 22, null, 2, "Desfibrilhador Externo Automático (DEA)", 10 },
                    { 23, null, 1, "Pilha Alcalina AA (Pack de 10)", 20 },
                    { 24, null, 2, "Estetoscópio Littmann", 55 },
                    { 25, null, 4, "Balança Hospitalar Digital", 4 },
                    { 26, null, 1, "Gesso Ortopédico (Rolo)", 90 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EstadosMaterial",
                keyColumn: "MaterialStatusId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "EstadosMaterial",
                keyColumn: "MaterialStatusId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "EstadosMaterial",
                keyColumn: "MaterialStatusId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "EstadosMaterial",
                keyColumn: "MaterialStatusId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "EstadosMaterial",
                keyColumn: "MaterialStatusId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "EstadosMaterial",
                keyColumn: "MaterialStatusId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "MaterialEquipamentoAssociado",
                keyColumn: "MaterialEquipamentoAssociadoId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "MaterialEquipamentoAssociado",
                keyColumn: "MaterialEquipamentoAssociadoId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "MaterialEquipamentoAssociado",
                keyColumn: "MaterialEquipamentoAssociadoId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "MaterialEquipamentoAssociado",
                keyColumn: "MaterialEquipamentoAssociadoId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "MaterialEquipamentoAssociado",
                keyColumn: "MaterialEquipamentoAssociadoId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "MaterialEquipamentoAssociado",
                keyColumn: "MaterialEquipamentoAssociadoId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "MaterialEquipamentoAssociado",
                keyColumn: "MaterialEquipamentoAssociadoId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "MaterialEquipamentoAssociado",
                keyColumn: "MaterialEquipamentoAssociadoId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "MaterialEquipamentoAssociado",
                keyColumn: "MaterialEquipamentoAssociadoId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "MaterialEquipamentoAssociado",
                keyColumn: "MaterialEquipamentoAssociadoId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "MaterialEquipamentoAssociado",
                keyColumn: "MaterialEquipamentoAssociadoId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "MaterialEquipamentoAssociado",
                keyColumn: "MaterialEquipamentoAssociadoId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "MaterialEquipamentoAssociado",
                keyColumn: "MaterialEquipamentoAssociadoId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "MaterialEquipamentoAssociado",
                keyColumn: "MaterialEquipamentoAssociadoId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "MaterialEquipamentoAssociado",
                keyColumn: "MaterialEquipamentoAssociadoId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "MaterialEquipamentoAssociado",
                keyColumn: "MaterialEquipamentoAssociadoId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "MaterialEquipamentoAssociado",
                keyColumn: "MaterialEquipamentoAssociadoId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "MaterialEquipamentoAssociado",
                keyColumn: "MaterialEquipamentoAssociadoId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "MaterialEquipamentoAssociado",
                keyColumn: "MaterialEquipamentoAssociadoId",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "MaterialEquipamentoAssociado",
                keyColumn: "MaterialEquipamentoAssociadoId",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "MaterialEquipamentoAssociado",
                keyColumn: "MaterialEquipamentoAssociadoId",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "MaterialEquipamentoAssociado",
                keyColumn: "MaterialEquipamentoAssociadoId",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "MaterialEquipamentoAssociado",
                keyColumn: "MaterialEquipamentoAssociadoId",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "MaterialEquipamentoAssociado",
                keyColumn: "MaterialEquipamentoAssociadoId",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "MaterialEquipamentoAssociado",
                keyColumn: "MaterialEquipamentoAssociadoId",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "MaterialEquipamentoAssociado",
                keyColumn: "MaterialEquipamentoAssociadoId",
                keyValue: 26);
        }
    }
}
