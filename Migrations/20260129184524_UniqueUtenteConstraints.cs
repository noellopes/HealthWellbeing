using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class UniqueUtenteConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NIF",
                table: "UtenteBalnearios",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Contacto",
                table: "UtenteBalnearios",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "HistoricoMedico",
                columns: table => new
                {
                    HistoricoMedicoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UtenteBalnearioId = table.Column<int>(type: "int", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DataRegisto = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricoMedico", x => x.HistoricoMedicoId);
                    table.ForeignKey(
                        name: "FK_HistoricoMedico_UtenteBalnearios_UtenteBalnearioId",
                        column: x => x.UtenteBalnearioId,
                        principalTable: "UtenteBalnearios",
                        principalColumn: "UtenteBalnearioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UtenteBalnearios_Contacto",
                table: "UtenteBalnearios",
                column: "Contacto",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UtenteBalnearios_NIF",
                table: "UtenteBalnearios",
                column: "NIF",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UtenteBalnearios_Nome",
                table: "UtenteBalnearios",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HistoricoMedico_UtenteBalnearioId",
                table: "HistoricoMedico",
                column: "UtenteBalnearioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistoricoMedico");

            migrationBuilder.DropIndex(
                name: "IX_UtenteBalnearios_Contacto",
                table: "UtenteBalnearios");

            migrationBuilder.DropIndex(
                name: "IX_UtenteBalnearios_NIF",
                table: "UtenteBalnearios");

            migrationBuilder.DropIndex(
                name: "IX_UtenteBalnearios_Nome",
                table: "UtenteBalnearios");

            migrationBuilder.AlterColumn<string>(
                name: "NIF",
                table: "UtenteBalnearios",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Contacto",
                table: "UtenteBalnearios",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
