using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeingRoom.Migrations
{
    /// <inheritdoc />
    public partial class RoomConsumablesUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomConsumable_Consumivel_ConsumivelId",
                table: "RoomConsumable");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomConsumable_RoomReservations_RoomReservationId",
                table: "RoomConsumable");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomConsumable_Room_RoomId",
                table: "RoomConsumable");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomConsumable",
                table: "RoomConsumable");

            migrationBuilder.RenameTable(
                name: "RoomConsumable",
                newName: "RoomConsumables");

            migrationBuilder.RenameIndex(
                name: "IX_RoomConsumable_RoomReservationId",
                table: "RoomConsumables",
                newName: "IX_RoomConsumables_RoomReservationId");

            migrationBuilder.RenameIndex(
                name: "IX_RoomConsumable_RoomId",
                table: "RoomConsumables",
                newName: "IX_RoomConsumables_RoomId");

            migrationBuilder.RenameIndex(
                name: "IX_RoomConsumable_ConsumivelId",
                table: "RoomConsumables",
                newName: "IX_RoomConsumables_ConsumivelId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomConsumables",
                table: "RoomConsumables",
                column: "RoomConsumableId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomConsumables_Consumivel_ConsumivelId",
                table: "RoomConsumables",
                column: "ConsumivelId",
                principalTable: "Consumivel",
                principalColumn: "ConsumivelId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomConsumables_RoomReservations_RoomReservationId",
                table: "RoomConsumables",
                column: "RoomReservationId",
                principalTable: "RoomReservations",
                principalColumn: "RoomReservationId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomConsumables_Room_RoomId",
                table: "RoomConsumables",
                column: "RoomId",
                principalTable: "Room",
                principalColumn: "RoomId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomConsumables_Consumivel_ConsumivelId",
                table: "RoomConsumables");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomConsumables_RoomReservations_RoomReservationId",
                table: "RoomConsumables");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomConsumables_Room_RoomId",
                table: "RoomConsumables");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoomConsumables",
                table: "RoomConsumables");

            migrationBuilder.RenameTable(
                name: "RoomConsumables",
                newName: "RoomConsumable");

            migrationBuilder.RenameIndex(
                name: "IX_RoomConsumables_RoomReservationId",
                table: "RoomConsumable",
                newName: "IX_RoomConsumable_RoomReservationId");

            migrationBuilder.RenameIndex(
                name: "IX_RoomConsumables_RoomId",
                table: "RoomConsumable",
                newName: "IX_RoomConsumable_RoomId");

            migrationBuilder.RenameIndex(
                name: "IX_RoomConsumables_ConsumivelId",
                table: "RoomConsumable",
                newName: "IX_RoomConsumable_ConsumivelId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoomConsumable",
                table: "RoomConsumable",
                column: "RoomConsumableId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomConsumable_Consumivel_ConsumivelId",
                table: "RoomConsumable",
                column: "ConsumivelId",
                principalTable: "Consumivel",
                principalColumn: "ConsumivelId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomConsumable_RoomReservations_RoomReservationId",
                table: "RoomConsumable",
                column: "RoomReservationId",
                principalTable: "RoomReservations",
                principalColumn: "RoomReservationId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomConsumable_Room_RoomId",
                table: "RoomConsumable",
                column: "RoomId",
                principalTable: "Room",
                principalColumn: "RoomId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
