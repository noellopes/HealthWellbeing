using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeingRoom.Migrations
{
    /// <inheritdoc />
    public partial class AddSpecialtyRequiredDevice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SpecialtyRequiredDevices",
                columns: table => new
                {
                    SpecialtyRequiredDeviceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpecialtyId = table.Column<int>(type: "int", nullable: false),
                    MedicalDeviceId = table.Column<int>(type: "int", nullable: false),
                    RequiredQuantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecialtyRequiredDevices", x => x.SpecialtyRequiredDeviceId);
                    table.ForeignKey(
                        name: "FK_SpecialtyRequiredDevices_MedicalDevices_MedicalDeviceId",
                        column: x => x.MedicalDeviceId,
                        principalTable: "MedicalDevices",
                        principalColumn: "MedicalDeviceID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SpecialtyRequiredDevices_Specialty_SpecialtyId",
                        column: x => x.SpecialtyId,
                        principalTable: "Specialty",
                        principalColumn: "SpecialtyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpecialtyRequiredDevices_MedicalDeviceId",
                table: "SpecialtyRequiredDevices",
                column: "MedicalDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_SpecialtyRequiredDevices_SpecialtyId",
                table: "SpecialtyRequiredDevices",
                column: "SpecialtyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpecialtyRequiredDevices");
        }
    }
}
