using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class AuditoriaMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sala",
                columns: table => new
                {
                    SalaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoSala = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Laboratorio = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sala", x => x.SalaId);
                });

            migrationBuilder.CreateTable(
                name: "AuditoriaConsumivel",
                columns: table => new
                {
                    AuditoriaConsumivelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SalaId = table.Column<int>(type: "int", nullable: false),
                    ConsumivelID = table.Column<int>(type: "int", nullable: false),
                    QuantidadeUsada = table.Column<int>(type: "int", nullable: false),
                    DataConsumo = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditoriaConsumivel", x => x.AuditoriaConsumivelId);
                    table.ForeignKey(
                        name: "FK_AuditoriaConsumivel_Consumivel_ConsumivelID",
                        column: x => x.ConsumivelID,
                        principalTable: "Consumivel",
                        principalColumn: "ConsumivelId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuditoriaConsumivel_Sala_SalaId",
                        column: x => x.SalaId,
                        principalTable: "Sala",
                        principalColumn: "SalaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditoriaConsumivel_ConsumivelID",
                table: "AuditoriaConsumivel",
                column: "ConsumivelID");

            migrationBuilder.CreateIndex(
                name: "IX_AuditoriaConsumivel_SalaId",
                table: "AuditoriaConsumivel",
                column: "SalaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditoriaConsumivel");

            migrationBuilder.DropTable(
                name: "Sala");
        }
    }
}
