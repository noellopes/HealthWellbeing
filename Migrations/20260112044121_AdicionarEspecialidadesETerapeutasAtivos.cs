using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarEspecialidadesETerapeutasAtivos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TerapeutaId",
                table: "TipoServicos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TipoServicos_TerapeutaId",
                table: "TipoServicos",
                column: "TerapeutaId");

            migrationBuilder.AddForeignKey(
                name: "FK_TipoServicos_Terapeuta_TerapeutaId",
                table: "TipoServicos",
                column: "TerapeutaId",
                principalTable: "Terapeuta",
                principalColumn: "TerapeutaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TipoServicos_Terapeuta_TerapeutaId",
                table: "TipoServicos");

            migrationBuilder.DropIndex(
                name: "IX_TipoServicos_TerapeutaId",
                table: "TipoServicos");

            migrationBuilder.DropColumn(
                name: "TerapeutaId",
                table: "TipoServicos");
        }
    }
}
