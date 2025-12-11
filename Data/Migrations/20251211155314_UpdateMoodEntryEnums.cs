using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMoodEntryEnums : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Triggers",
                table: "MoodEntries");

            migrationBuilder.AlterColumn<int>(
                name: "Emotion",
                table: "MoodEntries",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Trigger",
                table: "MoodEntries",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Trigger",
                table: "MoodEntries");

            migrationBuilder.AlterColumn<string>(
                name: "Emotion",
                table: "MoodEntries",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Triggers",
                table: "MoodEntries",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
