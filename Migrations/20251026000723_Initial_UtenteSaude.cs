using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class Initial_UtenteSaude : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.CreateTable(
                name: "UtenteSaude",
                columns: table => new
                {
                    UtenteSaudeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeCompleto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DataNascimento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Nif = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Niss = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nus = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: true),
                    Telefone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Morada = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UtenteSaude", x => x.UtenteSaudeId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UtenteSaude_Nif",
                table: "UtenteSaude",
                column: "Nif",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UtenteSaude_Niss",
                table: "UtenteSaude",
                column: "Niss",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UtenteSaude_Nus",
                table: "UtenteSaude",
                column: "Nus",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alergia");

            migrationBuilder.DropTable(
                name: "Receita");

            migrationBuilder.DropTable(
                name: "RestricaoAlimentar");

            migrationBuilder.DropTable(
                name: "UtenteSaude");

            migrationBuilder.DropTable(
                name: "Alimento");

            migrationBuilder.DropTable(
                name: "CategoriaAlimento");
        }
    }
}
