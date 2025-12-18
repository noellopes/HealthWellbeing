using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeingRoom.Migrations
{
    /// <inheritdoc />
    public partial class AddTypeMaterialHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TypeMaterialHistories",
                columns: table => new
                {
                    TypeMaterialHistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeMaterialID = table.Column<int>(type: "int", nullable: false),
                    OldName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OldDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeMaterialHistories", x => x.TypeMaterialHistoryId);
                    table.ForeignKey(
                        name: "FK_TypeMaterialHistories_TypeMaterial_TypeMaterialID",
                        column: x => x.TypeMaterialID,
                        principalTable: "TypeMaterial",
                        principalColumn: "TypeMaterialID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TypeMaterialHistories_TypeMaterialID",
                table: "TypeMaterialHistories",
                column: "TypeMaterialID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TypeMaterialHistories");
        }
    }
}
