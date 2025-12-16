using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeingRoom.Migrations
{
    /// <inheritdoc />
    public partial class InitialGrudRoomReservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RoomReservationId",
                table: "RoomConsumable",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoomReservationId",
                table: "LocationMedDevice",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RoomReservations",
                columns: table => new
                {
                    RoomReservationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ResponsibleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    ConsultationId = table.Column<int>(type: "int", nullable: true),
                    ConsultationType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PatientId = table.Column<int>(type: "int", nullable: true),
                    SpecialtyId = table.Column<int>(type: "int", nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomReservations", x => x.RoomReservationId);
                    table.ForeignKey(
                        name: "FK_RoomReservations_Room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoomReservations_Specialty_SpecialtyId",
                        column: x => x.SpecialtyId,
                        principalTable: "Specialty",
                        principalColumn: "SpecialtyId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoomConsumable_RoomReservationId",
                table: "RoomConsumable",
                column: "RoomReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationMedDevice_RoomReservationId",
                table: "LocationMedDevice",
                column: "RoomReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomReservations_RoomId",
                table: "RoomReservations",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomReservations_SpecialtyId",
                table: "RoomReservations",
                column: "SpecialtyId");

            migrationBuilder.AddForeignKey(
                name: "FK_LocationMedDevice_RoomReservations_RoomReservationId",
                table: "LocationMedDevice",
                column: "RoomReservationId",
                principalTable: "RoomReservations",
                principalColumn: "RoomReservationId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomConsumable_RoomReservations_RoomReservationId",
                table: "RoomConsumable",
                column: "RoomReservationId",
                principalTable: "RoomReservations",
                principalColumn: "RoomReservationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LocationMedDevice_RoomReservations_RoomReservationId",
                table: "LocationMedDevice");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomConsumable_RoomReservations_RoomReservationId",
                table: "RoomConsumable");

            migrationBuilder.DropTable(
                name: "RoomReservations");

            migrationBuilder.DropIndex(
                name: "IX_RoomConsumable_RoomReservationId",
                table: "RoomConsumable");

            migrationBuilder.DropIndex(
                name: "IX_LocationMedDevice_RoomReservationId",
                table: "LocationMedDevice");

            migrationBuilder.DropColumn(
                name: "RoomReservationId",
                table: "RoomConsumable");

            migrationBuilder.DropColumn(
                name: "RoomReservationId",
                table: "LocationMedDevice");
        }
    }
}
