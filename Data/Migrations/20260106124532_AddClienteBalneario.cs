using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddClienteBalneario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Clientes",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "IdReserva",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "Nome",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "idCartaoFidelizacao",
                table: "Clientes");

            migrationBuilder.RenameTable(
                name: "Clientes",
                newName: "ClienteBalneario");

            migrationBuilder.RenameColumn(
                name: "idCliente",
                table: "ClienteBalneario",
                newName: "ClienteBalnearioId");

            migrationBuilder.AlterColumn<string>(
                name: "Telemovel",
                table: "ClienteBalneario",
                type: "nvarchar(9)",
                maxLength: 9,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "NomeCompleto",
                table: "ClienteBalneario",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClienteBalneario",
                table: "ClienteBalneario",
                column: "ClienteBalnearioId");

            migrationBuilder.CreateIndex(
                name: "IX_ClienteBalneario_UtenteBalnearioId",
                table: "ClienteBalneario",
                column: "UtenteBalnearioId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClienteBalneario_UtenteBalneario_UtenteBalnearioId",
                table: "ClienteBalneario",
                column: "UtenteBalnearioId",
                principalTable: "UtenteBalneario",
                principalColumn: "UtenteBalnearioId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClienteBalneario_UtenteBalneario_UtenteBalnearioId",
                table: "ClienteBalneario");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClienteBalneario",
                table: "ClienteBalneario");

            migrationBuilder.DropIndex(
                name: "IX_ClienteBalneario_UtenteBalnearioId",
                table: "ClienteBalneario");

            migrationBuilder.DropColumn(
                name: "NomeCompleto",
                table: "ClienteBalneario");

            migrationBuilder.RenameTable(
                name: "ClienteBalneario",
                newName: "Clientes");

            migrationBuilder.RenameColumn(
                name: "ClienteBalnearioId",
                table: "Clientes",
                newName: "idCliente");

            migrationBuilder.AlterColumn<string>(
                name: "Telemovel",
                table: "Clientes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(9)",
                oldMaxLength: 9);

            migrationBuilder.AddColumn<int>(
                name: "IdReserva",
                table: "Clientes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nome",
                table: "Clientes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "idCartaoFidelizacao",
                table: "Clientes",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Clientes",
                table: "Clientes",
                column: "idCliente");
        }
    }
}
