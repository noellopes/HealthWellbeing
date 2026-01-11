using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class FixTipoServicoId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_Terapeuta_TerapeutaId",
                table: "Agendamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_TipoServicos_TipoServicoId",
                table: "Agendamentos");

            migrationBuilder.RenameColumn(
                name: "TipoServicoId",
                table: "Agendamentos",
                newName: "TipoServicosId");

            migrationBuilder.RenameIndex(
                name: "IX_Agendamentos_TipoServicoId",
                table: "Agendamentos",
                newName: "IX_Agendamentos_TipoServicosId");

            migrationBuilder.AlterColumn<int>(
                name: "TerapeutaId",
                table: "Agendamentos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "Agendamentos",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_Terapeuta_TerapeutaId",
                table: "Agendamentos",
                column: "TerapeutaId",
                principalTable: "Terapeuta",
                principalColumn: "TerapeutaId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_TipoServicos_TipoServicosId",
                table: "Agendamentos",
                column: "TipoServicosId",
                principalTable: "TipoServicos",
                principalColumn: "TipoServicosId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_Terapeuta_TerapeutaId",
                table: "Agendamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_TipoServicos_TipoServicosId",
                table: "Agendamentos");

            migrationBuilder.RenameColumn(
                name: "TipoServicosId",
                table: "Agendamentos",
                newName: "TipoServicoId");

            migrationBuilder.RenameIndex(
                name: "IX_Agendamentos_TipoServicosId",
                table: "Agendamentos",
                newName: "IX_Agendamentos_TipoServicoId");

            migrationBuilder.AlterColumn<int>(
                name: "TerapeutaId",
                table: "Agendamentos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "Agendamentos",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

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
        }
    }
}
