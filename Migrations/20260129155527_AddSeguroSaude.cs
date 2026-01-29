using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class AddSeguroSaude : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeguroSaude",
                table: "UtenteBalnearios");

            migrationBuilder.AddColumn<int>(
                name: "SeguroSaudeId",
                table: "UtenteBalnearios",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SegurosSaude",
                columns: table => new
                {
                    SeguroSaudeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SegurosSaude", x => x.SeguroSaudeId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UtenteBalnearios_SeguroSaudeId",
                table: "UtenteBalnearios",
                column: "SeguroSaudeId");

            migrationBuilder.AddForeignKey(
                name: "FK_UtenteBalnearios_SegurosSaude_SeguroSaudeId",
                table: "UtenteBalnearios",
                column: "SeguroSaudeId",
                principalTable: "SegurosSaude",
                principalColumn: "SeguroSaudeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UtenteBalnearios_SegurosSaude_SeguroSaudeId",
                table: "UtenteBalnearios");

            migrationBuilder.DropTable(
                name: "SegurosSaude");

            migrationBuilder.DropIndex(
                name: "IX_UtenteBalnearios_SeguroSaudeId",
                table: "UtenteBalnearios");

            migrationBuilder.DropColumn(
                name: "SeguroSaudeId",
                table: "UtenteBalnearios");

            migrationBuilder.AddColumn<string>(
                name: "SeguroSaude",
                table: "UtenteBalnearios",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
