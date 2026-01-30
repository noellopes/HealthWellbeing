using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class AddSatisfacaoCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Telemovel",
                table: "ClientesBalneario",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "SatisfacoesClientes",
                columns: table => new
                {
                    SatisfacaoClienteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteBalnearioId = table.Column<int>(type: "int", nullable: false),
                    Avaliacao = table.Column<int>(type: "int", nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DataRegisto = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SatisfacoesClientes", x => x.SatisfacaoClienteId);
                    table.ForeignKey(
                        name: "FK_SatisfacoesClientes_ClientesBalneario_ClienteBalnearioId",
                        column: x => x.ClienteBalnearioId,
                        principalTable: "ClientesBalneario",
                        principalColumn: "ClienteBalnearioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientesBalneario_Email",
                table: "ClientesBalneario",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientesBalneario_Telemovel",
                table: "ClientesBalneario",
                column: "Telemovel",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SatisfacoesClientes_ClienteBalnearioId",
                table: "SatisfacoesClientes",
                column: "ClienteBalnearioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SatisfacoesClientes");

            migrationBuilder.DropIndex(
                name: "IX_ClientesBalneario_Email",
                table: "ClientesBalneario");

            migrationBuilder.DropIndex(
                name: "IX_ClientesBalneario_Telemovel",
                table: "ClientesBalneario");

            migrationBuilder.AlterColumn<string>(
                name: "Telemovel",
                table: "ClientesBalneario",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
