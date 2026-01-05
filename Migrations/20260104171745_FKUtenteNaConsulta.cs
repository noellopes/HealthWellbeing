using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class FKUtenteNaConsulta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdUtenteSaude",
                table: "Consulta",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Consulta_IdUtenteSaude",
                table: "Consulta",
                column: "IdUtenteSaude");

            migrationBuilder.AddForeignKey(
                name: "FK_Consulta_UtenteSaude_IdUtenteSaude",
                table: "Consulta",
                column: "IdUtenteSaude",
                principalTable: "UtenteSaude",
                principalColumn: "UtenteSaudeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Consulta_UtenteSaude_IdUtenteSaude",
                table: "Consulta");

            migrationBuilder.DropIndex(
                name: "IX_Consulta_IdUtenteSaude",
                table: "Consulta");

            migrationBuilder.DropColumn(
                name: "IdUtenteSaude",
                table: "Consulta");
        }
    }
}
