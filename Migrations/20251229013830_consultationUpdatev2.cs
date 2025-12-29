using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeingRoom.Migrations
{
    /// <inheritdoc />
    public partial class consultationUpdatev2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConsultationType",
                table: "RoomReservations");

            migrationBuilder.CreateIndex(
                name: "IX_RoomReservations_ConsultationId",
                table: "RoomReservations",
                column: "ConsultationId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomReservations_Consultations_ConsultationId",
                table: "RoomReservations",
                column: "ConsultationId",
                principalTable: "Consultations",
                principalColumn: "ConsultationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomReservations_Consultations_ConsultationId",
                table: "RoomReservations");

            migrationBuilder.DropIndex(
                name: "IX_RoomReservations_ConsultationId",
                table: "RoomReservations");

            migrationBuilder.AddColumn<string>(
                name: "ConsultationType",
                table: "RoomReservations",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
