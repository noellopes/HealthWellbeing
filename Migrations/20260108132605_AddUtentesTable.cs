using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class AddUtentesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_UtenteBalneario_UtenteBalnearioId",
                table: "Agendamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_UtenteBalneario_DadosMedicos_DadosMedicosId",
                table: "UtenteBalneario");

            migrationBuilder.DropForeignKey(
                name: "FK_UtenteBalneario_SeguroSaude_SeguroSaudeId",
                table: "UtenteBalneario");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UtenteBalneario",
                table: "UtenteBalneario");

            migrationBuilder.RenameTable(
                name: "UtenteBalneario",
                newName: "Utentes");

            migrationBuilder.RenameIndex(
                name: "IX_UtenteBalneario_SeguroSaudeId",
                table: "Utentes",
                newName: "IX_Utentes_SeguroSaudeId");

            migrationBuilder.RenameIndex(
                name: "IX_UtenteBalneario_DadosMedicosId",
                table: "Utentes",
                newName: "IX_Utentes_DadosMedicosId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Utentes",
                table: "Utentes",
                column: "UtenteBalnearioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_Utentes_UtenteBalnearioId",
                table: "Agendamentos",
                column: "UtenteBalnearioId",
                principalTable: "Utentes",
                principalColumn: "UtenteBalnearioId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Utentes_DadosMedicos_DadosMedicosId",
                table: "Utentes",
                column: "DadosMedicosId",
                principalTable: "DadosMedicos",
                principalColumn: "DadosMedicosId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Utentes_SeguroSaude_SeguroSaudeId",
                table: "Utentes",
                column: "SeguroSaudeId",
                principalTable: "SeguroSaude",
                principalColumn: "SeguroSaudeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_Utentes_UtenteBalnearioId",
                table: "Agendamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Utentes_DadosMedicos_DadosMedicosId",
                table: "Utentes");

            migrationBuilder.DropForeignKey(
                name: "FK_Utentes_SeguroSaude_SeguroSaudeId",
                table: "Utentes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Utentes",
                table: "Utentes");

            migrationBuilder.RenameTable(
                name: "Utentes",
                newName: "UtenteBalneario");

            migrationBuilder.RenameIndex(
                name: "IX_Utentes_SeguroSaudeId",
                table: "UtenteBalneario",
                newName: "IX_UtenteBalneario_SeguroSaudeId");

            migrationBuilder.RenameIndex(
                name: "IX_Utentes_DadosMedicosId",
                table: "UtenteBalneario",
                newName: "IX_UtenteBalneario_DadosMedicosId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UtenteBalneario",
                table: "UtenteBalneario",
                column: "UtenteBalnearioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_UtenteBalneario_UtenteBalnearioId",
                table: "Agendamentos",
                column: "UtenteBalnearioId",
                principalTable: "UtenteBalneario",
                principalColumn: "UtenteBalnearioId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UtenteBalneario_DadosMedicos_DadosMedicosId",
                table: "UtenteBalneario",
                column: "DadosMedicosId",
                principalTable: "DadosMedicos",
                principalColumn: "DadosMedicosId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UtenteBalneario_SeguroSaude_SeguroSaudeId",
                table: "UtenteBalneario",
                column: "SeguroSaudeId",
                principalTable: "SeguroSaude",
                principalColumn: "SeguroSaudeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
