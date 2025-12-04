using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeingRoom.Migrations
{
    /// <inheritdoc />
    public partial class RoomModelUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Room_Specialty_SpecialtyId",
                table: "Room");

            migrationBuilder.AlterColumn<int>(
                name: "SpecialtyId",
                table: "Room",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Room_Specialty_SpecialtyId",
                table: "Room",
                column: "SpecialtyId",
                principalTable: "Specialty",
                principalColumn: "SpecialtyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Room_Specialty_SpecialtyId",
                table: "Room");

            migrationBuilder.AlterColumn<int>(
                name: "SpecialtyId",
                table: "Room",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Room_Specialty_SpecialtyId",
                table: "Room",
                column: "SpecialtyId",
                principalTable: "Specialty",
                principalColumn: "SpecialtyId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
