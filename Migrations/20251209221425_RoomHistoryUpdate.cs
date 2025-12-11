using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeingRoom.Migrations
{
    /// <inheritdoc />
    public partial class RoomHistoryUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StarDate",
                table: "RoomHistories",
                newName: "StartDate");

            migrationBuilder.AddColumn<int>(
                name: "ResponsibleId",
                table: "RoomHistories",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResponsibleId",
                table: "RoomHistories");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "RoomHistories",
                newName: "StarDate");
        }
    }
}
