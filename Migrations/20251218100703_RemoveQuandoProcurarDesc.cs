using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class RemoveQuandoProcurarDesc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuandoProcurarDesc",
                table: "Specialities");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QuandoProcurarDesc",
                table: "Specialities",
                type: "nvarchar(1500)",
                maxLength: 1500,
                nullable: true);
        }
    }
}
