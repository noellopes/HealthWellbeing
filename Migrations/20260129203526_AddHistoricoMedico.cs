using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class AddHistoricoMedico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UtenteBalnearios_Nome",
                table: "UtenteBalnearios");

            migrationBuilder.RenameColumn(
                name: "Descricao",
                table: "HistoricoMedico",
                newName: "Observacoes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Observacoes",
                table: "HistoricoMedico",
                newName: "Descricao");

            migrationBuilder.CreateIndex(
                name: "IX_UtenteBalnearios_Nome",
                table: "UtenteBalnearios",
                column: "Nome",
                unique: true);
        }
    }
}
