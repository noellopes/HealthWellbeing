using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeingRoom.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRoomReservationModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "RoomReservations");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "RoomReservationHistory");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "RoomReservations",
                newName: "ConsultationDate");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "RoomReservationHistory",
                newName: "ConsultationDate");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EndHour",
                table: "RoomReservations",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StartHour",
                table: "RoomReservations",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EndHour",
                table: "RoomReservationHistory",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StartHour",
                table: "RoomReservationHistory",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndHour",
                table: "RoomReservations");

            migrationBuilder.DropColumn(
                name: "StartHour",
                table: "RoomReservations");

            migrationBuilder.DropColumn(
                name: "EndHour",
                table: "RoomReservationHistory");

            migrationBuilder.DropColumn(
                name: "StartHour",
                table: "RoomReservationHistory");

            migrationBuilder.RenameColumn(
                name: "ConsultationDate",
                table: "RoomReservations",
                newName: "StartTime");

            migrationBuilder.RenameColumn(
                name: "ConsultationDate",
                table: "RoomReservationHistory",
                newName: "StartTime");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "RoomReservations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "RoomReservationHistory",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
