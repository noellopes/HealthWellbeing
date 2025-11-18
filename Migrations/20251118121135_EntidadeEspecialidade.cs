using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class EntidadeEspecialidade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Especialidade",
                table: "ExameTipo");

            migrationBuilder.AddColumn<int>(
                name: "EspecialidadeId",
                table: "ExameTipo",
                type: "int",
                maxLength: 100,
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Especialidades",
                columns: table => new
                {
                    EspecialidadeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Especialidades", x => x.EspecialidadeId);
                });

            migrationBuilder.InsertData(
                table: "Especialidades",
                columns: new[] { "EspecialidadeId", "Nome" },
                values: new object[,]
                {
                    { 1, "Hematologia" },
                    { 2, "Radiologia" },
                    { 3, "Cardiologia" },
                    { 4, "Imagiologia" },
                    { 5, "Reumatologia" },
                    { 6, "Urologia" },
                    { 7, "Gastroenterologia" },
                    { 8, "Ginecologia" },
                    { 9, "Pneumologia" },
                    { 10, "Endocrinologia" },
                    { 11, "Imunoalergologia" },
                    { 12, "Neurologia" },
                    { 13, "Oftalmologia" },
                    { 14, "Otorrinolaringologia" },
                    { 15, "Dermatologia" },
                    { 16, "Medicina Legal" }
                });

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 1,
                column: "EspecialidadeId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 2,
                column: "EspecialidadeId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 3,
                column: "EspecialidadeId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 4,
                column: "EspecialidadeId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 5,
                column: "EspecialidadeId",
                value: 4);

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 6,
                column: "EspecialidadeId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 7,
                column: "EspecialidadeId",
                value: 5);

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 8,
                column: "EspecialidadeId",
                value: 6);

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 9,
                column: "EspecialidadeId",
                value: 7);

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 10,
                column: "EspecialidadeId",
                value: 7);

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 11,
                column: "EspecialidadeId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 12,
                column: "EspecialidadeId",
                value: 8);

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 13,
                column: "EspecialidadeId",
                value: 9);

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 14,
                column: "EspecialidadeId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 15,
                column: "EspecialidadeId",
                value: 10);

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 16,
                column: "EspecialidadeId",
                value: 11);

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 17,
                column: "EspecialidadeId",
                value: 12);

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 18,
                column: "EspecialidadeId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 19,
                column: "EspecialidadeId",
                value: 13);

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 20,
                column: "EspecialidadeId",
                value: 14);

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 21,
                column: "EspecialidadeId",
                value: 15);

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 22,
                column: "EspecialidadeId",
                value: 16);

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 23,
                column: "EspecialidadeId",
                value: 6);

            migrationBuilder.CreateIndex(
                name: "IX_ExameTipo_EspecialidadeId",
                table: "ExameTipo",
                column: "EspecialidadeId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExameTipo_Especialidades_EspecialidadeId",
                table: "ExameTipo",
                column: "EspecialidadeId",
                principalTable: "Especialidades",
                principalColumn: "EspecialidadeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExameTipo_Especialidades_EspecialidadeId",
                table: "ExameTipo");

            migrationBuilder.DropTable(
                name: "Especialidades");

            migrationBuilder.DropIndex(
                name: "IX_ExameTipo_EspecialidadeId",
                table: "ExameTipo");

            migrationBuilder.DropColumn(
                name: "EspecialidadeId",
                table: "ExameTipo");

            migrationBuilder.AddColumn<string>(
                name: "Especialidade",
                table: "ExameTipo",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 1,
                column: "Especialidade",
                value: "Hematologia");

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 2,
                column: "Especialidade",
                value: "Radiologia");

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 3,
                column: "Especialidade",
                value: "Cardiologia");

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 4,
                column: "Especialidade",
                value: "Radiologia");

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 5,
                column: "Especialidade",
                value: "Imagiologia");

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 6,
                column: "Especialidade",
                value: "Cardiologia");

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 7,
                column: "Especialidade",
                value: "Reumatologia");

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 8,
                column: "Especialidade",
                value: "Urologia");

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 9,
                column: "Especialidade",
                value: "Gastroenterologia");

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 10,
                column: "Especialidade",
                value: "Gastroenterologia");

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 11,
                column: "Especialidade",
                value: "Radiologia");

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 12,
                column: "Especialidade",
                value: "Ginecologia");

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 13,
                column: "Especialidade",
                value: "Pneumologia");

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 14,
                column: "Especialidade",
                value: "Cardiologia");

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 15,
                column: "Especialidade",
                value: "Endocrinologia");

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 16,
                column: "Especialidade",
                value: "Imunoalergologia");

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 17,
                column: "Especialidade",
                value: "Neurologia");

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 18,
                column: "Especialidade",
                value: "Radiologia");

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 19,
                column: "Especialidade",
                value: "Oftalmologia");

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 20,
                column: "Especialidade",
                value: "Otorrinolaringologia");

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 21,
                column: "Especialidade",
                value: "Dermatologia");

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 22,
                column: "Especialidade",
                value: "Medicina Legal");

            migrationBuilder.UpdateData(
                table: "ExameTipo",
                keyColumn: "ExameTipoId",
                keyValue: 23,
                column: "Especialidade",
                value: "Urologia");
        }
    }
}
