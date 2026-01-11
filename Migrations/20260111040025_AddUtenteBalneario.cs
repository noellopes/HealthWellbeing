using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class AddUtenteBalneario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                newName: "UtenteBalnearios");

            migrationBuilder.RenameIndex(
                name: "IX_Utentes_SeguroSaudeId",
                table: "UtenteBalnearios",
                newName: "IX_UtenteBalnearios_SeguroSaudeId");

            migrationBuilder.RenameIndex(
                name: "IX_Utentes_DadosMedicosId",
                table: "UtenteBalnearios",
                newName: "IX_UtenteBalnearios_DadosMedicosId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UtenteBalnearios",
                table: "UtenteBalnearios",
                column: "UtenteBalnearioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_UtenteBalnearios_UtenteBalnearioId",
                table: "Agendamentos",
                column: "UtenteBalnearioId",
                principalTable: "UtenteBalnearios",
                principalColumn: "UtenteBalnearioId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UtenteBalnearios_DadosMedicos_DadosMedicosId",
                table: "UtenteBalnearios",
                column: "DadosMedicosId",
                principalTable: "DadosMedicos",
                principalColumn: "DadosMedicosId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UtenteBalnearios_SeguroSaude_SeguroSaudeId",
                table: "UtenteBalnearios",
                column: "SeguroSaudeId",
                principalTable: "SeguroSaude",
                principalColumn: "SeguroSaudeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_UtenteBalnearios_UtenteBalnearioId",
                table: "Agendamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_UtenteBalnearios_DadosMedicos_DadosMedicosId",
                table: "UtenteBalnearios");

            migrationBuilder.DropForeignKey(
                name: "FK_UtenteBalnearios_SeguroSaude_SeguroSaudeId",
                table: "UtenteBalnearios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UtenteBalnearios",
                table: "UtenteBalnearios");

            migrationBuilder.RenameTable(
                name: "UtenteBalnearios",
                newName: "Utentes");

            migrationBuilder.RenameIndex(
                name: "IX_UtenteBalnearios_SeguroSaudeId",
                table: "Utentes",
                newName: "IX_Utentes_SeguroSaudeId");

            migrationBuilder.RenameIndex(
                name: "IX_UtenteBalnearios_DadosMedicosId",
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
    }
}
