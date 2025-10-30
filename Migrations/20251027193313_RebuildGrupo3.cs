using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class RebuildGrupo3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "FoodCategory",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParentCategoryId",
                table: "FoodCategory",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Client",
                columns: table => new
                {
                    ClientId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateMember = table.Column<bool>(type: "bit", nullable: true),
                    ClientId1 = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Client", x => x.ClientId);
                    table.ForeignKey(
                        name: "FK_Client_Client_ClientId1",
                        column: x => x.ClientId1,
                        principalTable: "Client",
                        principalColumn: "ClientId");
                });

            migrationBuilder.CreateTable(
                name: "Member",
                columns: table => new
                {
                    MemberId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Member", x => x.MemberId);
                    table.ForeignKey(
                        name: "FK_Member_Client_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FoodCategory_ParentCategoryId",
                table: "FoodCategory",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Client_ClientId1",
                table: "Client",
                column: "ClientId1");

            migrationBuilder.CreateIndex(
                name: "IX_Member_ClientId",
                table: "Member",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_FoodCategory_FoodCategory_ParentCategoryId",
                table: "FoodCategory",
                column: "ParentCategoryId",
                principalTable: "FoodCategory",
                principalColumn: "FoodCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FoodCategory_FoodCategory_ParentCategoryId",
                table: "FoodCategory");

            migrationBuilder.DropTable(
                name: "Member");

            migrationBuilder.DropTable(
                name: "Client");

            migrationBuilder.DropIndex(
                name: "IX_FoodCategory_ParentCategoryId",
                table: "FoodCategory");

            migrationBuilder.DropColumn(
                name: "ParentCategoryId",
                table: "FoodCategory");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "FoodCategory",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);
        }
    }
}
