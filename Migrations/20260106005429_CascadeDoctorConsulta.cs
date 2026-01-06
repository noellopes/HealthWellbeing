using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class CascadeDoctorConsulta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DoctorConsulta",
                columns: table => new
                {
                    IdMedico = table.Column<int>(type: "int", nullable: false),
                    IdConsulta = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorConsulta", x => new { x.IdMedico, x.IdConsulta });
                    table.ForeignKey(
                        name: "FK_DoctorConsulta_Consulta_IdConsulta",
                        column: x => x.IdConsulta,
                        principalTable: "Consulta",
                        principalColumn: "IdConsulta");
                    table.ForeignKey(
                        name: "FK_DoctorConsulta_Doctor_IdMedico",
                        column: x => x.IdMedico,
                        principalTable: "Doctor",
                        principalColumn: "IdMedico");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DoctorConsulta_IdConsulta",
                table: "DoctorConsulta",
                column: "IdConsulta");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DoctorConsulta");
        }
    }
}
