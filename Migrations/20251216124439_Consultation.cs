using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeingRoom.Migrations
{
    /// <inheritdoc />
    public partial class Consultation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomReservations_Specialty_SpecialtyId",
                table: "RoomReservations");

            migrationBuilder.DropIndex(
                name: "IX_RoomReservations_SpecialtyId",
                table: "RoomReservations");

            migrationBuilder.DropColumn(
                name: "ConsultationType",
                table: "RoomReservations");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "RoomReservations");

            migrationBuilder.DropColumn(
                name: "SpecialtyId",
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
                name: "Consultations",
                columns: table => new
                {
                    ConsultationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConsultationType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PatientName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Doctor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpecialtyId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consultations", x => x.ConsultationId);
                    table.ForeignKey(
                        name: "FK_Consultations_Specialty_SpecialtyId",
                        column: x => x.SpecialtyId,
                        principalTable: "Specialty",
                        principalColumn: "SpecialtyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoomReservations_ConsultationId",
                table: "RoomReservations",
                column: "ConsultationId");

            migrationBuilder.CreateIndex(
                name: "IX_Consultations_SpecialtyId",
                table: "Consultations",
                column: "SpecialtyId");

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
                name: "Consultations");

            migrationBuilder.DropIndex(
                name: "IX_RoomReservations_ConsultationId",
                table: "RoomReservations");

            migrationBuilder.AlterColumn<int>(
                name: "ConsultationId",
                table: "RoomReservations",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "ConsultationType",
                table: "RoomReservations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PatientId",
                table: "RoomReservations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SpecialtyId",
                table: "RoomReservations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoomReservations_SpecialtyId",
                table: "RoomReservations",
                column: "SpecialtyId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomReservations_Specialty_SpecialtyId",
                table: "RoomReservations",
                column: "SpecialtyId",
                principalTable: "Specialty",
                principalColumn: "SpecialtyId");
        }
    }
}
