using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class AddTerapeutaFKToUtente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TerapeutaResponsavel",
                table: "UtenteBalnearios");

            migrationBuilder.AddColumn<int>(
                name: "TerapeutaId",
                table: "UtenteBalnearios",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UtenteBalnearios_TerapeutaId",
                table: "UtenteBalnearios",
                column: "TerapeutaId");

            migrationBuilder.AddForeignKey(
                name: "FK_UtenteBalnearios_Terapeutas_TerapeutaId",
                table: "UtenteBalnearios",
                column: "TerapeutaId",
                principalTable: "Terapeutas",
                principalColumn: "TerapeutaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UtenteBalnearios_Terapeutas_TerapeutaId",
                table: "UtenteBalnearios");

            migrationBuilder.DropIndex(
                name: "IX_UtenteBalnearios_TerapeutaId",
                table: "UtenteBalnearios");

            migrationBuilder.DropColumn(
                name: "TerapeutaId",
                table: "UtenteBalnearios");

            migrationBuilder.AddColumn<string>(
                name: "TerapeutaResponsavel",
                table: "UtenteBalnearios",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
