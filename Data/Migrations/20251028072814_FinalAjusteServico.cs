using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Data.Migrations
{
    /// <inheritdoc />
    public partial class FinalAjusteServico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Servicos_TipoServicos_TipoServicoId",
                table: "Servicos");

            migrationBuilder.AlterColumn<int>(
                name: "TipoServicoId",
                table: "Servicos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "Preco",
                table: "Servicos",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddForeignKey(
                name: "FK_Servicos_TipoServicos_TipoServicoId",
                table: "Servicos",
                column: "TipoServicoId",
                principalTable: "TipoServicos",
                principalColumn: "TipoServicoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Servicos_TipoServicos_TipoServicoId",
                table: "Servicos");

            migrationBuilder.AlterColumn<int>(
                name: "TipoServicoId",
                table: "Servicos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Preco",
                table: "Servicos",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AddForeignKey(
                name: "FK_Servicos_TipoServicos_TipoServicoId",
                table: "Servicos",
                column: "TipoServicoId",
                principalTable: "TipoServicos",
                principalColumn: "TipoServicoId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
