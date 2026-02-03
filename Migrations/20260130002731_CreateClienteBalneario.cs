using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class CreateClienteBalneario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClienteBalnearioId",
                table: "UtenteBalnearios",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ClientesBalneario",
                columns: table => new
                {
                    ClienteBalnearioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Telemovel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataRegisto = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    Pontos = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientesBalneario", x => x.ClienteBalnearioId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UtenteBalnearios_ClienteBalnearioId",
                table: "UtenteBalnearios",
                column: "ClienteBalnearioId");

            migrationBuilder.AddForeignKey(
                name: "FK_UtenteBalnearios_ClientesBalneario_ClienteBalnearioId",
                table: "UtenteBalnearios",
                column: "ClienteBalnearioId",
                principalTable: "ClientesBalneario",
                principalColumn: "ClienteBalnearioId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UtenteBalnearios_ClientesBalneario_ClienteBalnearioId",
                table: "UtenteBalnearios");

            migrationBuilder.DropTable(
                name: "ClientesBalneario");

            migrationBuilder.DropIndex(
                name: "IX_UtenteBalnearios_ClienteBalnearioId",
                table: "UtenteBalnearios");

            migrationBuilder.DropColumn(
                name: "ClienteBalnearioId",
                table: "UtenteBalnearios");
        }
    }
}
