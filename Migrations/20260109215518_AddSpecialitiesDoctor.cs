using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class AddSpecialitiesDoctor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SpecialitiesDoctor",
                columns: table => new
                {
                    IdEspecialidade = table.Column<int>(type: "int", nullable: false),
                    IdMedico = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecialitiesDoctor", x => new { x.IdEspecialidade, x.IdMedico });
                    table.ForeignKey(
                        name: "FK_SpecialitiesDoctor_Doctor_IdMedico",
                        column: x => x.IdMedico,
                        principalTable: "Doctor",
                        principalColumn: "IdMedico");
                    table.ForeignKey(
                        name: "FK_SpecialitiesDoctor_Specialities_IdEspecialidade",
                        column: x => x.IdEspecialidade,
                        principalTable: "Specialities",
                        principalColumn: "IdEspecialidade");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpecialitiesDoctor_IdMedico",
                table: "SpecialitiesDoctor",
                column: "IdMedico");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpecialitiesDoctor");
        }
    }
}
