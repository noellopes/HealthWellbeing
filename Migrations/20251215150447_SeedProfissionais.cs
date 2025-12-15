using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class SeedProfissionais : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Funcoes",
                columns: new[] { "FuncaoId", "NomeFuncao" },
                values: new object[,]
                {
                    { 1, "Técnico de Radiologia" },
                    { 2, "Fisioterapeuta" },
                    { 3, "Técnico de Cardiopneumologia" },
                    { 4, "Técnico de Análises Clínicas" },
                    { 5, "Terapeuta Ocupacional" },
                    { 6, "Ortopedista" },
                    { 7, "Enfermeiro Especialista" },
                    { 8, "Nutricionista" },
                    { 9, "Técnico de Medicina Nuclear" },
                    { 10, "Cardiologista" },
                    { 11, "Podologista" },
                    { 12, "Técnico de Neurofisiologia" },
                    { 13, "Técnico Auxiliar de Saúde" },
                    { 14, "Optometrista" },
                    { 15, "Técnico de Medicina Física e Reabilitação" }
                });

            migrationBuilder.InsertData(
                table: "ProfissionalExecutante",
                columns: new[] { "ProfissionalExecutanteId", "Email", "FuncaoId", "FuncaoId1", "Nome", "Telefone" },
                values: new object[,]
                {
                    { 1, "Kandonga123@gmail.com", 1, null, "André Kandonga", "912912915" },
                    { 2, "MiguelSantos123@gmail.com", 2, null, "Miguel Santos", "912912914" },
                    { 3, "DostoevskySuba@gmail.com", 3, null, "Dostoevsky", "912913914" },
                    { 4, "QuaresmaPorto@gmail.com", 4, null, "Ricardo Quaresma", "910101010" },
                    { 5, "Mai123222suba@gmail.com", 5, null, "Mai Da Silva", "912912222" },
                    { 6, "DiogoRodrigues04@gmail.com", 6, null, "Diogo Rodrigues", "912912522" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Funcoes",
                keyColumn: "FuncaoId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Funcoes",
                keyColumn: "FuncaoId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Funcoes",
                keyColumn: "FuncaoId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Funcoes",
                keyColumn: "FuncaoId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Funcoes",
                keyColumn: "FuncaoId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Funcoes",
                keyColumn: "FuncaoId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Funcoes",
                keyColumn: "FuncaoId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Funcoes",
                keyColumn: "FuncaoId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Funcoes",
                keyColumn: "FuncaoId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "ProfissionalExecutante",
                keyColumn: "ProfissionalExecutanteId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ProfissionalExecutante",
                keyColumn: "ProfissionalExecutanteId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ProfissionalExecutante",
                keyColumn: "ProfissionalExecutanteId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ProfissionalExecutante",
                keyColumn: "ProfissionalExecutanteId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ProfissionalExecutante",
                keyColumn: "ProfissionalExecutanteId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ProfissionalExecutante",
                keyColumn: "ProfissionalExecutanteId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Funcoes",
                keyColumn: "FuncaoId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Funcoes",
                keyColumn: "FuncaoId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Funcoes",
                keyColumn: "FuncaoId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Funcoes",
                keyColumn: "FuncaoId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Funcoes",
                keyColumn: "FuncaoId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Funcoes",
                keyColumn: "FuncaoId",
                keyValue: 6);
        }
    }
}
