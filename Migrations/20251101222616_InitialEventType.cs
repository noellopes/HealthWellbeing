using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class InitialEventType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventType",
                columns: table => new
                {
                    EventTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventTypeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EventTypeDescription = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventType", x => x.EventTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Levels",
                columns: table => new
                {
                    LevelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Level = table.Column<int>(type: "int", nullable: false),
                    LevelCategory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LevelsLevelId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Levels", x => x.LevelId);
                    table.ForeignKey(
                        name: "FK_Levels_Levels_LevelsLevelId",
                        column: x => x.LevelsLevelId,
                        principalTable: "Levels",
                        principalColumn: "LevelId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Levels_LevelsLevelId",
                table: "Levels",
                column: "LevelsLevelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventType");

            migrationBuilder.DropTable(
                name: "Levels");
        }
    }
}
