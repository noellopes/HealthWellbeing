using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class MigracaoFinalCompleta01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExameTipo_ExameTipo_ExameTipoId1",
                table: "ExameTipo");

            migrationBuilder.DropForeignKey(
                name: "FK_ProfissionalExecutante_ProfissionalExecutante_ProfissionalExecutanteId1",
                table: "ProfissionalExecutante");

            migrationBuilder.DropIndex(
                name: "IX_ProfissionalExecutante_ProfissionalExecutanteId1",
                table: "ProfissionalExecutante");

            migrationBuilder.DropIndex(
                name: "IX_ExameTipo_ExameTipoId1",
                table: "ExameTipo");

            migrationBuilder.DropColumn(
                name: "Funcao",
                table: "ProfissionalExecutante");

            migrationBuilder.DropColumn(
                name: "ProfissionalExecutanteId1",
                table: "ProfissionalExecutante");

            migrationBuilder.DropColumn(
                name: "ExameTipoId1",
                table: "ExameTipo");

            migrationBuilder.AddColumn<int>(
                name: "FuncaoId",
                table: "ProfissionalExecutante",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Funcoes",
                columns: table => new
                {
                    FuncaoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Funcoes", x => x.FuncaoId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProfissionalExecutante_FuncaoId",
                table: "ProfissionalExecutante",
                column: "FuncaoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProfissionalExecutante_Funcoes_FuncaoId",
                table: "ProfissionalExecutante",
                column: "FuncaoId",
                principalTable: "Funcoes",
                principalColumn: "FuncaoId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProfissionalExecutante_Funcoes_FuncaoId",
                table: "ProfissionalExecutante");

            migrationBuilder.DropTable(
                name: "Funcoes");

            migrationBuilder.DropIndex(
                name: "IX_ProfissionalExecutante_FuncaoId",
                table: "ProfissionalExecutante");

            migrationBuilder.DropColumn(
                name: "FuncaoId",
                table: "ProfissionalExecutante");

            migrationBuilder.AddColumn<string>(
                name: "Funcao",
                table: "ProfissionalExecutante",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ProfissionalExecutanteId1",
                table: "ProfissionalExecutante",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExameTipoId1",
                table: "ExameTipo",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfissionalExecutante_ProfissionalExecutanteId1",
                table: "ProfissionalExecutante",
                column: "ProfissionalExecutanteId1");

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

            migrationBuilder.AddForeignKey(
                name: "FK_ProfissionalExecutante_ProfissionalExecutante_ProfissionalExecutanteId1",
                table: "ProfissionalExecutante",
                column: "ProfissionalExecutanteId1",
                principalTable: "ProfissionalExecutante",
                principalColumn: "ProfissionalExecutanteId");
        }
    }
}
