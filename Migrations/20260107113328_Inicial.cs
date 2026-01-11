using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agendamento_Servicos_ServicoId",
                table: "Agendamento");

            migrationBuilder.DropForeignKey(
                name: "FK_Agendamento_Terapeuta_TerapeutaId",
                table: "Agendamento");

            migrationBuilder.DropForeignKey(
                name: "FK_Agendamento_TipoServicos_TipoServicoId",
                table: "Agendamento");

            migrationBuilder.DropForeignKey(
                name: "FK_Agendamento_UtenteBalneario_UtenteBalnearioId",
                table: "Agendamento");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Agendamento",
                table: "Agendamento");

            migrationBuilder.RenameTable(
                name: "Agendamento",
                newName: "Agendamentos");

            migrationBuilder.RenameIndex(
                name: "IX_Agendamento_UtenteBalnearioId",
                table: "Agendamentos",
                newName: "IX_Agendamentos_UtenteBalnearioId");

            migrationBuilder.RenameIndex(
                name: "IX_Agendamento_TipoServicoId",
                table: "Agendamentos",
                newName: "IX_Agendamentos_TipoServicoId");

            migrationBuilder.RenameIndex(
                name: "IX_Agendamento_TerapeutaId",
                table: "Agendamentos",
                newName: "IX_Agendamentos_TerapeutaId");

            migrationBuilder.RenameIndex(
                name: "IX_Agendamento_ServicoId",
                table: "Agendamentos",
                newName: "IX_Agendamentos_ServicoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Agendamentos",
                table: "Agendamentos",
                column: "AgendamentoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_Servicos_ServicoId",
                table: "Agendamentos",
                column: "ServicoId",
                principalTable: "Servicos",
                principalColumn: "ServicoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_Terapeuta_TerapeutaId",
                table: "Agendamentos",
                column: "TerapeutaId",
                principalTable: "Terapeuta",
                principalColumn: "TerapeutaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_TipoServicos_TipoServicoId",
                table: "Agendamentos",
                column: "TipoServicoId",
                principalTable: "TipoServicos",
                principalColumn: "TipoServicosId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_UtenteBalneario_UtenteBalnearioId",
                table: "Agendamentos",
                column: "UtenteBalnearioId",
                principalTable: "UtenteBalneario",
                principalColumn: "UtenteBalnearioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_Servicos_ServicoId",
                table: "Agendamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_Terapeuta_TerapeutaId",
                table: "Agendamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_TipoServicos_TipoServicoId",
                table: "Agendamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_UtenteBalneario_UtenteBalnearioId",
                table: "Agendamentos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Agendamentos",
                table: "Agendamentos");

            migrationBuilder.RenameTable(
                name: "Agendamentos",
                newName: "Agendamento");

            migrationBuilder.RenameIndex(
                name: "IX_Agendamentos_UtenteBalnearioId",
                table: "Agendamento",
                newName: "IX_Agendamento_UtenteBalnearioId");

            migrationBuilder.RenameIndex(
                name: "IX_Agendamentos_TipoServicoId",
                table: "Agendamento",
                newName: "IX_Agendamento_TipoServicoId");

            migrationBuilder.RenameIndex(
                name: "IX_Agendamentos_TerapeutaId",
                table: "Agendamento",
                newName: "IX_Agendamento_TerapeutaId");

            migrationBuilder.RenameIndex(
                name: "IX_Agendamentos_ServicoId",
                table: "Agendamento",
                newName: "IX_Agendamento_ServicoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Agendamento",
                table: "Agendamento",
                column: "AgendamentoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamento_Servicos_ServicoId",
                table: "Agendamento",
                column: "ServicoId",
                principalTable: "Servicos",
                principalColumn: "ServicoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamento_Terapeuta_TerapeutaId",
                table: "Agendamento",
                column: "TerapeutaId",
                principalTable: "Terapeuta",
                principalColumn: "TerapeutaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamento_TipoServicos_TipoServicoId",
                table: "Agendamento",
                column: "TipoServicoId",
                principalTable: "TipoServicos",
                principalColumn: "TipoServicosId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamento_UtenteBalneario_UtenteBalnearioId",
                table: "Agendamento",
                column: "UtenteBalnearioId",
                principalTable: "UtenteBalneario",
                principalColumn: "UtenteBalnearioId");
        }
    }
}
