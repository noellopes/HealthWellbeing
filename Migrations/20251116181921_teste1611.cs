using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class teste1611 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlergiaAlimento_Alimentos_AlimentoId",
                table: "AlergiaAlimento");

            migrationBuilder.AddForeignKey(
                name: "FK_AlergiaAlimento_Alimentos_AlimentoId",
                table: "AlergiaAlimento",
                column: "AlimentoId",
                principalTable: "Alimentos",
                principalColumn: "AlimentoId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlergiaAlimento_Alimentos_AlimentoId",
                table: "AlergiaAlimento");

            migrationBuilder.AddForeignKey(
                name: "FK_AlergiaAlimento_Alimentos_AlimentoId",
                table: "AlergiaAlimento",
                column: "AlimentoId",
                principalTable: "Alimentos",
                principalColumn: "AlimentoId");
        }
    }
}
