using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class InitialHealth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LevelAtual",
                table: "Level",
                newName: "LevelCurrent");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LevelCurrent",
                table: "Level",
                newName: "LevelAtual");
        }
    }
}
