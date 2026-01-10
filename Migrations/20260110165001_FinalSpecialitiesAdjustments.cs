using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class FinalSpecialitiesAdjustments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DoctorIdMedico",
                table: "SpecialitiesDoctor",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SpecialitiesDoctor_DoctorIdMedico",
                table: "SpecialitiesDoctor",
                column: "DoctorIdMedico");

            migrationBuilder.AddForeignKey(
                name: "FK_SpecialitiesDoctor_Doctor_DoctorIdMedico",
                table: "SpecialitiesDoctor",
                column: "DoctorIdMedico",
                principalTable: "Doctor",
                principalColumn: "IdMedico");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpecialitiesDoctor_Doctor_DoctorIdMedico",
                table: "SpecialitiesDoctor");

            migrationBuilder.DropIndex(
                name: "IX_SpecialitiesDoctor_DoctorIdMedico",
                table: "SpecialitiesDoctor");

            migrationBuilder.DropColumn(
                name: "DoctorIdMedico",
                table: "SpecialitiesDoctor");
        }
    }
}
