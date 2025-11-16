using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class Seeding15More : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ExameTipo",
                columns: new[] { "ExameTipoId", "Descricao", "Especialidade", "Nome" },
                values: new object[,]
                {
                    { 9, "Exame endoscópico para visualização do intestino grosso.", "Gastroenterologia", "Colonoscopia" },
                    { 10, "Exame do esófago, estômago e duodeno.", "Gastroenterologia", "Endoscopia Digestiva Alta" },
                    { 11, "Rastreio e diagnóstico de cancro da mama.", "Radiologia", "Mamografia Digital" },
                    { 12, "Avaliação dos órgãos pélvicos femininos ou masculinos.", "Ginecologia", "Ecografia Pélvica" },
                    { 13, "Avalia a capacidade pulmonar e o fluxo de ar.", "Pneumologia", "Prova de Função Respiratória" },
                    { 14, "Monitorização contínua da atividade elétrica do coração.", "Cardiologia", "Holter 24 Horas" },
                    { 15, "Medição dos níveis de hormonas tiroideias no sangue.", "Endocrinologia", "Análise Hormonal (Tireoide)" },
                    { 16, "Testes cutâneos para identificação de alergénios específicos.", "Imunoalergologia", "Teste de Alergias" },
                    { 17, "Registo da atividade elétrica cerebral.", "Neurologia", "Eletroencefalograma (EEG)" },
                    { 18, "Visualização detalhada dos vasos sanguíneos.", "Radiologia", "Angiografia por TC" },
                    { 19, "Avaliação da acuidade visual e pressão intraocular.", "Oftalmologia", "Exame Oftalmológico Completo" },
                    { 20, "Avaliação da capacidade auditiva.", "Otorrinolaringologia", "Audiograma" },
                    { 21, "Colheita de pequena amostra de tecido cutâneo.", "Dermatologia", "Biopsia de Pele" },
                    { 22, "Detecção e quantificação de substâncias químicas no organismo.", "Medicina Legal", "Análise Toxicológica" },
                    { 23, "Identificação de bactérias que podem causar infeção urinária.", "Urologia", "Cultura de Urina" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 23);
        }
    }
}
