using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeingRoom.Migrations
{
    /// <inheritdoc />
    public partial class RoomReservationHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomReservations_Consultations_ConsultationId",
                table: "RoomReservations");

            migrationBuilder.AlterColumn<int>(
                name: "ConsultationId",
                table: "RoomReservations",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "RoomReservationHistory",
                columns: table => new
                {
                    RoomReservationHistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomReservationId = table.Column<int>(type: "int", nullable: false),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    ConsultationId = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResponsibleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FinalStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecordedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomReservationHistory", x => x.RoomReservationHistoryId);
                    table.ForeignKey(
                        name: "FK_RoomReservationHistory_Consultations_ConsultationId",
                        column: x => x.ConsultationId,
                        principalTable: "Consultations",
                        principalColumn: "ConsultationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoomReservationHistory_Room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoomReservationHistory_ConsultationId",
                table: "RoomReservationHistory",
                column: "ConsultationId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomReservationHistory_RoomId",
                table: "RoomReservationHistory",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomReservations_Consultations_ConsultationId",
                table: "RoomReservations",
                column: "ConsultationId",
                principalTable: "Consultations",
                principalColumn: "ConsultationId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomReservations_Consultations_ConsultationId",
                table: "RoomReservations");

            migrationBuilder.DropTable(
                name: "RoomReservationHistory");

            migrationBuilder.AlterColumn<int>(
                name: "ConsultationId",
                table: "RoomReservations",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomReservations_Consultations_ConsultationId",
                table: "RoomReservations",
                column: "ConsultationId",
                principalTable: "Consultations",
                principalColumn: "ConsultationId");
        }
    }
}
