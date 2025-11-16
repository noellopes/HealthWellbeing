using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class _2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alergia_Alimentos_AlimentoId",
                table: "Alergia");

            migrationBuilder.DropIndex(
                name: "IX_Alergia_AlimentoId",
                table: "Alergia");

            migrationBuilder.DropColumn(
                name: "AlimentoId",
                table: "Alergia");

            migrationBuilder.CreateTable(
                name: "AlergiaAlimento",
                columns: table => new
                {
                    AlergiaId = table.Column<int>(type: "int", nullable: false),
                    AlimentoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlergiaAlimento", x => new { x.AlergiaId, x.AlimentoId });
                    table.ForeignKey(
                        name: "FK_AlergiaAlimento_Alergia_AlergiaId",
                        column: x => x.AlergiaId,
                        principalTable: "Alergia",
                        principalColumn: "AlergiaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlergiaAlimento_Alimentos_AlimentoId",
                        column: x => x.AlimentoId,
                        principalTable: "Alimentos",
                        principalColumn: "AlimentoId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlergiaAlimento_AlimentoId",
                table: "AlergiaAlimento",
                column: "AlimentoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlergiaAlimento");

            migrationBuilder.AddColumn<int>(
                name: "AlimentoId",
                table: "Alergia",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Alergia_AlimentoId",
                table: "Alergia",
                column: "AlimentoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Alergia_Alimentos_AlimentoId",
                table: "Alergia",
                column: "AlimentoId",
                principalTable: "Alimentos",
                principalColumn: "AlimentoId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
