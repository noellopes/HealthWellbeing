using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class AlignServicoForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Servicos_TipoServicos_TipoServicosId",
                table: "Servicos");

            migrationBuilder.DropColumn(
                name: "TipoServicoId",
                table: "Servicos");

            migrationBuilder.AlterColumn<int>(
                name: "TipoServicosId",
                table: "Servicos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Servicos_TipoServicos_TipoServicosId",
                table: "Servicos",
                column: "TipoServicosId",
                principalTable: "TipoServicos",
                principalColumn: "TipoServicosId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Servicos_TipoServicos_TipoServicosId",
                table: "Servicos");

            migrationBuilder.AlterColumn<int>(
                name: "TipoServicosId",
                table: "Servicos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "TipoServicoId",
                table: "Servicos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Servicos_TipoServicos_TipoServicosId",
                table: "Servicos",
                column: "TipoServicosId",
                principalTable: "TipoServicos",
                principalColumn: "TipoServicosId");
        }
    }
}
