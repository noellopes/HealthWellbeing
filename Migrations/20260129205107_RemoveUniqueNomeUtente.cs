using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUniqueNomeUtente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HistoricoMedico_UtenteBalnearios_UtenteBalnearioId",
                table: "HistoricoMedico");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HistoricoMedico",
                table: "HistoricoMedico");

            migrationBuilder.RenameTable(
                name: "HistoricoMedico",
                newName: "HistoricosMedicos");

            migrationBuilder.RenameColumn(
                name: "Observacoes",
                table: "HistoricosMedicos",
                newName: "Descricao");

            migrationBuilder.RenameIndex(
                name: "IX_HistoricoMedico_UtenteBalnearioId",
                table: "HistoricosMedicos",
                newName: "IX_HistoricosMedicos_UtenteBalnearioId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HistoricosMedicos",
                table: "HistoricosMedicos",
                column: "HistoricoMedicoId");

            migrationBuilder.AddForeignKey(
                name: "FK_HistoricosMedicos_UtenteBalnearios_UtenteBalnearioId",
                table: "HistoricosMedicos",
                column: "UtenteBalnearioId",
                principalTable: "UtenteBalnearios",
                principalColumn: "UtenteBalnearioId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HistoricosMedicos_UtenteBalnearios_UtenteBalnearioId",
                table: "HistoricosMedicos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HistoricosMedicos",
                table: "HistoricosMedicos");

            migrationBuilder.RenameTable(
                name: "HistoricosMedicos",
                newName: "HistoricoMedico");

            migrationBuilder.RenameColumn(
                name: "Descricao",
                table: "HistoricoMedico",
                newName: "Observacoes");

            migrationBuilder.RenameIndex(
                name: "IX_HistoricosMedicos_UtenteBalnearioId",
                table: "HistoricoMedico",
                newName: "IX_HistoricoMedico_UtenteBalnearioId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HistoricoMedico",
                table: "HistoricoMedico",
                column: "HistoricoMedicoId");

            migrationBuilder.AddForeignKey(
                name: "FK_HistoricoMedico_UtenteBalnearios_UtenteBalnearioId",
                table: "HistoricoMedico",
                column: "UtenteBalnearioId",
                principalTable: "UtenteBalnearios",
                principalColumn: "UtenteBalnearioId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
