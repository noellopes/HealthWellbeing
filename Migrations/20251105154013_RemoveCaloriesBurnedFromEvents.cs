using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCaloriesBurnedFromEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Levels_Levels_LevelsLevelId",
                table: "Levels");

            migrationBuilder.DropIndex(
                name: "IX_Levels_LevelsLevelId",
                table: "Levels");

            migrationBuilder.DropColumn(
                name: "LevelsLevelId",
                table: "Levels");

            migrationBuilder.DropColumn(
                name: "CaloriesBurned",
                table: "Events");

            migrationBuilder.AddColumn<string>(
                name: "LevelPoints",
                table: "Levels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Intensity",
                table: "Events",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "EventType",
                table: "Events",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "EventName",
                table: "Events",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "EventDescription",
                table: "Events",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "Activity",
                columns: table => new
                {
                    Activity_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Activity_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Activity_Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activity_Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumberSets = table.Column<int>(type: "int", nullable: true),
                    NumberReps = table.Column<int>(type: "int", nullable: true),
                    Weigth = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activity", x => x.Activity_Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Activity");

            migrationBuilder.DropColumn(
                name: "LevelPoints",
                table: "Levels");

            migrationBuilder.AddColumn<int>(
                name: "LevelsLevelId",
                table: "Levels",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Intensity",
                table: "Events",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "EventType",
                table: "Events",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "EventName",
                table: "Events",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "EventDescription",
                table: "Events",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<int>(
                name: "CaloriesBurned",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Levels_LevelsLevelId",
                table: "Levels",
                column: "LevelsLevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Levels_Levels_LevelsLevelId",
                table: "Levels",
                column: "LevelsLevelId",
                principalTable: "Levels",
                principalColumn: "LevelId");
        }
    }
}
