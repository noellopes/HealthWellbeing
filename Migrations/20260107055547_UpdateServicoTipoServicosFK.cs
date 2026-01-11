using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class UpdateServicoTipoServicosFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Servicos_TipoServicos_TipoServicoId",
                table: "Servicos");

            migrationBuilder.DropIndex(
                name: "IX_Servicos_TipoServicoId",
                table: "Servicos");

            migrationBuilder.AddColumn<int>(
                name: "TipoServicosId",
                table: "Servicos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Servicos_TipoServicosId",
                table: "Servicos",
                column: "TipoServicosId");

            migrationBuilder.AddForeignKey(
                name: "FK_Servicos_TipoServicos_TipoServicosId",
                table: "Servicos",
                column: "TipoServicosId",
                principalTable: "TipoServicos",
                principalColumn: "TipoServicosId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Servicos_TipoServicos_TipoServicosId",
                table: "Servicos");

            migrationBuilder.DropIndex(
                name: "IX_Servicos_TipoServicosId",
                table: "Servicos");

            migrationBuilder.DropColumn(
                name: "TipoServicosId",
                table: "Servicos");

            migrationBuilder.CreateIndex(
                name: "IX_Servicos_TipoServicoId",
                table: "Servicos",
                column: "TipoServicoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Servicos_TipoServicos_TipoServicoId",
                table: "Servicos",
                column: "TipoServicoId",
                principalTable: "TipoServicos",
                principalColumn: "TipoServicosId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
