using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeingRoom.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRoomConsumible : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RoomConsumable",
                columns: table => new
                {
                    RoomConsumableId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    ConsumivelId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomConsumable", x => x.RoomConsumableId);
                    table.ForeignKey(
                        name: "FK_RoomConsumable_Consumivel_ConsumivelId",
                        column: x => x.ConsumivelId,
                        principalTable: "Consumivel",
                        principalColumn: "ConsumivelId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoomConsumable_Room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoomConsumable_ConsumivelId",
                table: "RoomConsumable",
                column: "ConsumivelId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomConsumable_RoomId",
                table: "RoomConsumable",
                column: "RoomId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoomConsumable");
        }
    }
}
