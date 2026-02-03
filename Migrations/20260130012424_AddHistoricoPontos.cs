using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class AddHistoricoPontos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pontos",
                table: "ClientesBalneario");

            migrationBuilder.CreateTable(
                name: "HistoricoPontos",
                columns: table => new
                {
                    HistoricoPontosId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteBalnearioId = table.Column<int>(type: "int", nullable: false),
                    Pontos = table.Column<int>(type: "int", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricoPontos", x => x.HistoricoPontosId);
                    table.ForeignKey(
                        name: "FK_HistoricoPontos_ClientesBalneario_ClienteBalnearioId",
                        column: x => x.ClienteBalnearioId,
                        principalTable: "ClientesBalneario",
                        principalColumn: "ClienteBalnearioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HistoricoPontos_ClienteBalnearioId",
                table: "HistoricoPontos",
                column: "ClienteBalnearioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistoricoPontos");

            migrationBuilder.AddColumn<int>(
                name: "Pontos",
                table: "ClientesBalneario",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
