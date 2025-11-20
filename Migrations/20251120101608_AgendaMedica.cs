using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class AgendaMedica : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AgendaMedica",
                columns: table => new
                {
                    IdAgendaMedica = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdMedico = table.Column<int>(type: "int", nullable: true),
                    DiaSemana = table.Column<int>(type: "int", nullable: false),
                    HoraInicio = table.Column<TimeOnly>(type: "time", nullable: false),
                    HoraFim = table.Column<TimeOnly>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgendaMedica", x => x.IdAgendaMedica);
                    table.ForeignKey(
                        name: "FK_AgendaMedica_Doctor_IdMedico",
                        column: x => x.IdMedico,
                        principalTable: "Doctor",
                        principalColumn: "IdMedico");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AgendaMedica_IdMedico",
                table: "AgendaMedica",
                column: "IdMedico");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AgendaMedica");
        }
    }
}
