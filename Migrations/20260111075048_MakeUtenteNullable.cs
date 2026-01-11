using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class MakeUtenteNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_Utentes_UtenteBalnearioId",
                table: "Agendamentos");

            migrationBuilder.AlterColumn<int>(
                name: "UtenteBalnearioId",
                table: "Agendamentos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_Utentes_UtenteBalnearioId",
                table: "Agendamentos",
                column: "UtenteBalnearioId",
                principalTable: "Utentes",
                principalColumn: "UtenteBalnearioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_Utentes_UtenteBalnearioId",
                table: "Agendamentos");

            migrationBuilder.AlterColumn<int>(
                name: "UtenteBalnearioId",
                table: "Agendamentos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_Utentes_UtenteBalnearioId",
                table: "Agendamentos",
                column: "UtenteBalnearioId",
                principalTable: "Utentes",
                principalColumn: "UtenteBalnearioId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
