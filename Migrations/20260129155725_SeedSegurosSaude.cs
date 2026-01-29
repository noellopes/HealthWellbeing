using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class SeedSegurosSaude : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "SegurosSaude",
                columns: new[] { "SeguroSaudeId", "Nome" },
                values: new object[,]
                {
                    { 1, "ADSE" },
                    { 2, "Multicare" },
                    { 3, "Médis" },
                    { 4, "Particular" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SegurosSaude",
                keyColumn: "SeguroSaudeId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "SegurosSaude",
                keyColumn: "SeguroSaudeId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "SegurosSaude",
                keyColumn: "SeguroSaudeId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "SegurosSaude",
                keyColumn: "SeguroSaudeId",
                keyValue: 4);
        }
    }
}
