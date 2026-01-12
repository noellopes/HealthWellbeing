using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class AddServicoAtivo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ServicoId1",
                table: "Agendamentos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_ServicoId1",
                table: "Agendamentos",
                column: "ServicoId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_Servicos_ServicoId1",
                table: "Agendamentos",
                column: "ServicoId1",
                principalTable: "Servicos",
                principalColumn: "ServicoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_Servicos_ServicoId1",
                table: "Agendamentos");

            migrationBuilder.DropIndex(
                name: "IX_Agendamentos_ServicoId1",
                table: "Agendamentos");

            migrationBuilder.DropColumn(
                name: "ServicoId1",
                table: "Agendamentos");
        }
    }
}
