using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class AddNivelCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NivelClienteId",
                table: "ClientesBalneario",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "NiveisCliente",
                columns: table => new
                {
                    NivelClienteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PontosMinimos = table.Column<int>(type: "int", nullable: false),
                    CorBadge = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NiveisCliente", x => x.NivelClienteId);
                });

            migrationBuilder.InsertData(
                table: "NiveisCliente",
                columns: new[] { "NivelClienteId", "CorBadge", "Nome", "PontosMinimos" },
                values: new object[,]
                {
                    { 1, "bg-secondary", "Bronze", 0 },
                    { 2, "bg-info", "Prata", 100 },
                    { 3, "bg-warning", "Ouro", 300 },
                    { 4, "bg-success", "Platinum", 600 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientesBalneario_NivelClienteId",
                table: "ClientesBalneario",
                column: "NivelClienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientesBalneario_NiveisCliente_NivelClienteId",
                table: "ClientesBalneario",
                column: "NivelClienteId",
                principalTable: "NiveisCliente",
                principalColumn: "NivelClienteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientesBalneario_NiveisCliente_NivelClienteId",
                table: "ClientesBalneario");

            migrationBuilder.DropTable(
                name: "NiveisCliente");

            migrationBuilder.DropIndex(
                name: "IX_ClientesBalneario_NivelClienteId",
                table: "ClientesBalneario");

            migrationBuilder.DropColumn(
                name: "NivelClienteId",
                table: "ClientesBalneario");
        }
    }
}
