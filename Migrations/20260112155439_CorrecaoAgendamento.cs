using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class CorrecaoAgendamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Utentes_DadosMedicos_DadosMedicosId",
                table: "Utentes");

            migrationBuilder.DropForeignKey(
                name: "FK_Utentes_SeguroSaude_SeguroSaudeId",
                table: "Utentes");

            migrationBuilder.AlterColumn<int>(
                name: "SeguroSaudeId",
                table: "Utentes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "DadosMedicosId",
                table: "Utentes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Utentes_DadosMedicos_DadosMedicosId",
                table: "Utentes",
                column: "DadosMedicosId",
                principalTable: "DadosMedicos",
                principalColumn: "DadosMedicosId");

            migrationBuilder.AddForeignKey(
                name: "FK_Utentes_SeguroSaude_SeguroSaudeId",
                table: "Utentes",
                column: "SeguroSaudeId",
                principalTable: "SeguroSaude",
                principalColumn: "SeguroSaudeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Utentes_DadosMedicos_DadosMedicosId",
                table: "Utentes");

            migrationBuilder.DropForeignKey(
                name: "FK_Utentes_SeguroSaude_SeguroSaudeId",
                table: "Utentes");

            migrationBuilder.AlterColumn<int>(
                name: "SeguroSaudeId",
                table: "Utentes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DadosMedicosId",
                table: "Utentes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Utentes_DadosMedicos_DadosMedicosId",
                table: "Utentes",
                column: "DadosMedicosId",
                principalTable: "DadosMedicos",
                principalColumn: "DadosMedicosId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Utentes_SeguroSaude_SeguroSaudeId",
                table: "Utentes",
                column: "SeguroSaudeId",
                principalTable: "SeguroSaude",
                principalColumn: "SeguroSaudeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
