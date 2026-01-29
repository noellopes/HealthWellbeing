using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class AddCriadoPorUserToHistorico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CriadoPorUserId",
                table: "HistoricosMedicos",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HistoricosMedicos_CriadoPorUserId",
                table: "HistoricosMedicos",
                column: "CriadoPorUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_HistoricosMedicos_AspNetUsers_CriadoPorUserId",
                table: "HistoricosMedicos",
                column: "CriadoPorUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HistoricosMedicos_AspNetUsers_CriadoPorUserId",
                table: "HistoricosMedicos");

            migrationBuilder.DropIndex(
                name: "IX_HistoricosMedicos_CriadoPorUserId",
                table: "HistoricosMedicos");

            migrationBuilder.DropColumn(
                name: "CriadoPorUserId",
                table: "HistoricosMedicos");
        }
    }
}
