using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class RenameTreatmentRecordTreatmentTypeFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TreatmentRecord_TreatmentType_TreatmentId",
                table: "TreatmentRecord");

            migrationBuilder.RenameColumn(
                name: "TreatmentId",
                table: "TreatmentRecord",
                newName: "TreatmentTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_TreatmentRecord_TreatmentId",
                table: "TreatmentRecord",
                newName: "IX_TreatmentRecord_TreatmentTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_TreatmentRecord_TreatmentType_TreatmentTypeId",
                table: "TreatmentRecord",
                column: "TreatmentTypeId",
                principalTable: "TreatmentType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TreatmentRecord_TreatmentType_TreatmentTypeId",
                table: "TreatmentRecord");

            migrationBuilder.RenameColumn(
                name: "TreatmentTypeId",
                table: "TreatmentRecord",
                newName: "TreatmentId");

            migrationBuilder.RenameIndex(
                name: "IX_TreatmentRecord_TreatmentTypeId",
                table: "TreatmentRecord",
                newName: "IX_TreatmentRecord_TreatmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_TreatmentRecord_TreatmentType_TreatmentId",
                table: "TreatmentRecord",
                column: "TreatmentId",
                principalTable: "TreatmentType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
