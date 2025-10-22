using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTipoServicoToServico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "Servicos");

            migrationBuilder.AlterColumn<decimal>(
                name: "Preco",
                table: "Servicos",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<int>(
                name: "TipoServicoId",
                table: "Servicos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "TipoServicos",
                columns: table => new
                {
                    TipoServicoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoServicos", x => x.TipoServicoId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Servicos_TipoServicoId",
                table: "Servicos",
                column: "TipoServicoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Servicos_TipoServicos_TipoServicoId",
                table: "Servicos",
                column: "TipoServicoId",
                principalTable: "TipoServicos",
                principalColumn: "TipoServicoId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Servicos_TipoServicos_TipoServicoId",
                table: "Servicos");

            migrationBuilder.DropTable(
                name: "TipoServicos");

            migrationBuilder.DropIndex(
                name: "IX_Servicos_TipoServicoId",
                table: "Servicos");

            migrationBuilder.DropColumn(
                name: "TipoServicoId",
                table: "Servicos");

            migrationBuilder.AlterColumn<decimal>(
                name: "Preco",
                table: "Servicos",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AddColumn<string>(
                name: "Tipo",
                table: "Servicos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
