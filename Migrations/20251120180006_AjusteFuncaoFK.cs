using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class AjusteFuncaoFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProfissionalExecutante_Funcoes_FuncaoId",
                table: "ProfissionalExecutante");

            migrationBuilder.DropColumn(
                name: "Funcao",
                table: "ProfissionalExecutante");

            migrationBuilder.AlterColumn<int>(
                name: "FuncaoId",
                table: "ProfissionalExecutante",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FuncaoId1",
                table: "ProfissionalExecutante",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfissionalExecutante_FuncaoId1",
                table: "ProfissionalExecutante",
                column: "FuncaoId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ProfissionalExecutante_Funcoes_FuncaoId",
                table: "ProfissionalExecutante",
                column: "FuncaoId",
                principalTable: "Funcoes",
                principalColumn: "FuncaoId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProfissionalExecutante_Funcoes_FuncaoId1",
                table: "ProfissionalExecutante",
                column: "FuncaoId1",
                principalTable: "Funcoes",
                principalColumn: "FuncaoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProfissionalExecutante_Funcoes_FuncaoId",
                table: "ProfissionalExecutante");

            migrationBuilder.DropForeignKey(
                name: "FK_ProfissionalExecutante_Funcoes_FuncaoId1",
                table: "ProfissionalExecutante");

            migrationBuilder.DropIndex(
                name: "IX_ProfissionalExecutante_FuncaoId1",
                table: "ProfissionalExecutante");

            migrationBuilder.DropColumn(
                name: "FuncaoId1",
                table: "ProfissionalExecutante");

            migrationBuilder.AlterColumn<int>(
                name: "FuncaoId",
                table: "ProfissionalExecutante",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Funcao",
                table: "ProfissionalExecutante",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_ProfissionalExecutante_Funcoes_FuncaoId",
                table: "ProfissionalExecutante",
                column: "FuncaoId",
                principalTable: "Funcoes",
                principalColumn: "FuncaoId");
        }
    }
}
