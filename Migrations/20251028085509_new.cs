using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeingRoom.Migrations
{
    /// <inheritdoc />
    public partial class @new : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RoomId",
                table: "Equipment",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_RoomId",
                table: "Equipment",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipment_Room_RoomId",
                table: "Equipment",
                column: "RoomId",
                principalTable: "Room",
                principalColumn: "RoomId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipment_Room_RoomId",
                table: "Equipment");

            migrationBuilder.DropIndex(
                name: "IX_Equipment_RoomId",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "Equipment");
        }
    }
}
