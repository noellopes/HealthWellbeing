using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameClientToPatient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CrisisAlerts_Clients_ClientId",
                table: "CrisisAlerts");

            migrationBuilder.DropForeignKey(
                name: "FK_Goals_Clients_ClientId",
                table: "Goals");

            migrationBuilder.DropForeignKey(
                name: "FK_MoodEntries_Clients_ClientId",
                table: "MoodEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_TherapySessions_Clients_ClientId",
                table: "TherapySessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Clients",
                table: "Clients");

            migrationBuilder.RenameTable(
                name: "Clients",
                newName: "Patients");

            migrationBuilder.RenameColumn(
                name: "ClientId",
                table: "TherapySessions",
                newName: "PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_TherapySessions_ClientId",
                table: "TherapySessions",
                newName: "IX_TherapySessions_PatientId");

            migrationBuilder.RenameColumn(
                name: "ClientId",
                table: "MoodEntries",
                newName: "PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_MoodEntries_ClientId",
                table: "MoodEntries",
                newName: "IX_MoodEntries_PatientId");

            migrationBuilder.RenameColumn(
                name: "ClientId",
                table: "Goals",
                newName: "PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_Goals_ClientId",
                table: "Goals",
                newName: "IX_Goals_PatientId");

            migrationBuilder.RenameColumn(
                name: "ClientId",
                table: "CrisisAlerts",
                newName: "PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_CrisisAlerts_ClientId",
                table: "CrisisAlerts",
                newName: "IX_CrisisAlerts_PatientId");

            migrationBuilder.RenameColumn(
                name: "ClientId",
                table: "Patients",
                newName: "PatientId");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Patients",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EmergencyPhone",
                table: "Patients",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Patients",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Patients",
                table: "Patients",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_CrisisAlerts_Patients_PatientId",
                table: "CrisisAlerts",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Goals_Patients_PatientId",
                table: "Goals",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MoodEntries_Patients_PatientId",
                table: "MoodEntries",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TherapySessions_Patients_PatientId",
                table: "TherapySessions",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CrisisAlerts_Patients_PatientId",
                table: "CrisisAlerts");

            migrationBuilder.DropForeignKey(
                name: "FK_Goals_Patients_PatientId",
                table: "Goals");

            migrationBuilder.DropForeignKey(
                name: "FK_MoodEntries_Patients_PatientId",
                table: "MoodEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_TherapySessions_Patients_PatientId",
                table: "TherapySessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Patients",
                table: "Patients");

            migrationBuilder.RenameTable(
                name: "Patients",
                newName: "Clients");

            migrationBuilder.RenameColumn(
                name: "PatientId",
                table: "TherapySessions",
                newName: "ClientId");

            migrationBuilder.RenameIndex(
                name: "IX_TherapySessions_PatientId",
                table: "TherapySessions",
                newName: "IX_TherapySessions_ClientId");

            migrationBuilder.RenameColumn(
                name: "PatientId",
                table: "MoodEntries",
                newName: "ClientId");

            migrationBuilder.RenameIndex(
                name: "IX_MoodEntries_PatientId",
                table: "MoodEntries",
                newName: "IX_MoodEntries_ClientId");

            migrationBuilder.RenameColumn(
                name: "PatientId",
                table: "Goals",
                newName: "ClientId");

            migrationBuilder.RenameIndex(
                name: "IX_Goals_PatientId",
                table: "Goals",
                newName: "IX_Goals_ClientId");

            migrationBuilder.RenameColumn(
                name: "PatientId",
                table: "CrisisAlerts",
                newName: "ClientId");

            migrationBuilder.RenameIndex(
                name: "IX_CrisisAlerts_PatientId",
                table: "CrisisAlerts",
                newName: "IX_CrisisAlerts_ClientId");

            migrationBuilder.RenameColumn(
                name: "PatientId",
                table: "Clients",
                newName: "ClientId");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EmergencyPhone",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Clients",
                table: "Clients",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_CrisisAlerts_Clients_ClientId",
                table: "CrisisAlerts",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "ClientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Goals_Clients_ClientId",
                table: "Goals",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "ClientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MoodEntries_Clients_ClientId",
                table: "MoodEntries",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "ClientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TherapySessions_Clients_ClientId",
                table: "TherapySessions",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "ClientId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
