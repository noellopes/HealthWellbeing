using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeingRoom.Migrations
{
    /// <inheritdoc />
    public partial class SpecialtyRequiredConsumable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SpecialtyRequiredConsumables",
                columns: table => new
                {
                    SpecialtyRequiredConsumableId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpecialtyId = table.Column<int>(type: "int", nullable: false),
                    ConsumivelId = table.Column<int>(type: "int", nullable: false),
                    RequiredQuantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecialtyRequiredConsumables", x => x.SpecialtyRequiredConsumableId);
                    table.ForeignKey(
                        name: "FK_SpecialtyRequiredConsumables_Consumivel_ConsumivelId",
                        column: x => x.ConsumivelId,
                        principalTable: "Consumivel",
                        principalColumn: "ConsumivelId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SpecialtyRequiredConsumables_Specialty_SpecialtyId",
                        column: x => x.SpecialtyId,
                        principalTable: "Specialty",
                        principalColumn: "SpecialtyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpecialtyRequiredConsumables_ConsumivelId",
                table: "SpecialtyRequiredConsumables",
                column: "ConsumivelId");

            migrationBuilder.CreateIndex(
                name: "IX_SpecialtyRequiredConsumables_SpecialtyId",
                table: "SpecialtyRequiredConsumables",
                column: "SpecialtyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpecialtyRequiredConsumables");
        }
    }
}
