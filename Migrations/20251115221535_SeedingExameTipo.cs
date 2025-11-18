using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class SeedingExameTipo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExameTipo_ExameTipo_ExameTipoId1",
                table: "ExameTipo");

            migrationBuilder.DropIndex(
                name: "IX_ExameTipo_ExameTipoId1",
                table: "ExameTipo");

            migrationBuilder.DropColumn(
                name: "ExameTipoId1",
                table: "ExameTipo");

            migrationBuilder.InsertData(
                table: "ExameTipo",
                columns: new[] { "ExameTipoId", "Descricao", "Especialidade", "Nome" },
                values: new object[,]
                {
                    { 1, "Exame laboratorial de rotina para avaliação hematológica.", "Hematologia", "Análise de Sangue Completa" },
                    { 2, "Exame de imagem detalhado para estruturas internas.", "Radiologia", "Ressonância Magnética" },
                    { 3, "Avaliação da atividade elétrica do coração.", "Cardiologia", "Eletrocardiograma (ECG)" },
                    { 4, "Processamento de imagens por computador para criar visões transversais do corpo.", "Radiologia", "Tomografia Computorizada (TAC)" },
                    { 5, "Utiliza ondas sonoras de alta frequência para criar imagens dos órgãos internos.", "Imagiologia", "Ecografia Abdominal" },
                    { 6, "Monitorização cardíaca durante exercício físico controlado.", "Cardiologia", "Teste de Esforço Cardíaco" },
                    { 7, "Mede a densidade mineral óssea para diagnosticar osteoporose.", "Reumatologia", "Densitometria Óssea" },
                    { 8, "Análise laboratorial de amostra de urina.", "Urologia", "Exame de Urina Tipo II" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 8);

            migrationBuilder.AddColumn<int>(
                name: "ExameTipoId1",
                table: "ExameTipo",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExameTipo_ExameTipoId1",
                table: "ExameTipo",
                column: "ExameTipoId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ExameTipo_ExameTipo_ExameTipoId1",
                table: "ExameTipo",
                column: "ExameTipoId1",
                principalTable: "ExameTipo",
                principalColumn: "ExameTipoId");
        }
    }
}
