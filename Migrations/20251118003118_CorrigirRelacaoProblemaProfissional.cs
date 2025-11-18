using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class CorrigirRelacaoProblemaProfissional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProblemaSaudeProfissionalExecutante_ProblemaSaude_ProblemasSaudeProblemaSaudeId",
                table: "ProblemaSaudeProfissionalExecutante");

            migrationBuilder.DropForeignKey(
                name: "FK_ProblemaSaudeProfissionalExecutante_ProfissionalExecutante_ProfissionalExecutanteId",
                table: "ProblemaSaudeProfissionalExecutante");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProblemaSaudeProfissionalExecutante",
                table: "ProblemaSaudeProfissionalExecutante");

            migrationBuilder.RenameTable(
                name: "ProblemaSaudeProfissionalExecutante",
                newName: "ProblemaSaudeProfissionais");

            migrationBuilder.RenameIndex(
                name: "IX_ProblemaSaudeProfissionalExecutante_ProfissionalExecutanteId",
                table: "ProblemaSaudeProfissionais",
                newName: "IX_ProblemaSaudeProfissionais_ProfissionalExecutanteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProblemaSaudeProfissionais",
                table: "ProblemaSaudeProfissionais",
                columns: new[] { "ProblemasSaudeProblemaSaudeId", "ProfissionalExecutanteId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProblemaSaudeProfissionais_ProblemaSaude_ProblemasSaudeProblemaSaudeId",
                table: "ProblemaSaudeProfissionais",
                column: "ProblemasSaudeProblemaSaudeId",
                principalTable: "ProblemaSaude",
                principalColumn: "ProblemaSaudeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProblemaSaudeProfissionais_ProfissionalExecutante_ProfissionalExecutanteId",
                table: "ProblemaSaudeProfissionais",
                column: "ProfissionalExecutanteId",
                principalTable: "ProfissionalExecutante",
                principalColumn: "ProfissionalExecutanteId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProblemaSaudeProfissionais_ProblemaSaude_ProblemasSaudeProblemaSaudeId",
                table: "ProblemaSaudeProfissionais");

            migrationBuilder.DropForeignKey(
                name: "FK_ProblemaSaudeProfissionais_ProfissionalExecutante_ProfissionalExecutanteId",
                table: "ProblemaSaudeProfissionais");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProblemaSaudeProfissionais",
                table: "ProblemaSaudeProfissionais");

            migrationBuilder.RenameTable(
                name: "ProblemaSaudeProfissionais",
                newName: "ProblemaSaudeProfissionalExecutante");

            migrationBuilder.RenameIndex(
                name: "IX_ProblemaSaudeProfissionais_ProfissionalExecutanteId",
                table: "ProblemaSaudeProfissionalExecutante",
                newName: "IX_ProblemaSaudeProfissionalExecutante_ProfissionalExecutanteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProblemaSaudeProfissionalExecutante",
                table: "ProblemaSaudeProfissionalExecutante",
                columns: new[] { "ProblemasSaudeProblemaSaudeId", "ProfissionalExecutanteId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProblemaSaudeProfissionalExecutante_ProblemaSaude_ProblemasSaudeProblemaSaudeId",
                table: "ProblemaSaudeProfissionalExecutante",
                column: "ProblemasSaudeProblemaSaudeId",
                principalTable: "ProblemaSaude",
                principalColumn: "ProblemaSaudeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProblemaSaudeProfissionalExecutante_ProfissionalExecutante_ProfissionalExecutanteId",
                table: "ProblemaSaudeProfissionalExecutante",
                column: "ProfissionalExecutanteId",
                principalTable: "ProfissionalExecutante",
                principalColumn: "ProfissionalExecutanteId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
