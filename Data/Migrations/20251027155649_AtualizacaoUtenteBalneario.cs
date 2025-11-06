using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Data.Migrations
{
    /// <inheritdoc />
    public partial class AtualizacaoUtenteBalneario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContraIndicacoes",
                table: "UtenteBalneario");

            migrationBuilder.DropColumn(
                name: "HistoricoClinico",
                table: "UtenteBalneario");

            migrationBuilder.DropColumn(
                name: "IndicacoesTerapeuticas",
                table: "UtenteBalneario");

            migrationBuilder.DropColumn(
                name: "MedicoResponsavel",
                table: "UtenteBalneario");

            migrationBuilder.DropColumn(
                name: "Nome",
                table: "UtenteBalneario");

            migrationBuilder.DropColumn(
                name: "SeguroSaude",
                table: "UtenteBalneario");

            migrationBuilder.AlterColumn<int>(
                name: "Sexo",
                table: "UtenteBalneario",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "NIF",
                table: "UtenteBalneario",
                type: "nvarchar(9)",
                maxLength: 9,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Morada",
                table: "UtenteBalneario",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Contacto",
                table: "UtenteBalneario",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "DadosMedicosId",
                table: "UtenteBalneario",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "NomeCompleto",
                table: "UtenteBalneario",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SeguroSaudeId",
                table: "UtenteBalneario",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "DadosMedicos",
                columns: table => new
                {
                    DadosMedicosId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HistoricoClinico = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IndicacoesTerapeuticas = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContraIndicacoes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MedicoResponsavel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DadosMedicos", x => x.DadosMedicosId);
                });

            migrationBuilder.CreateTable(
                name: "SeguroSaude",
                columns: table => new
                {
                    SeguroSaudeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeSeguradora = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NumeroApolice = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeguroSaude", x => x.SeguroSaudeId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UtenteBalneario_DadosMedicosId",
                table: "UtenteBalneario",
                column: "DadosMedicosId");

            migrationBuilder.CreateIndex(
                name: "IX_UtenteBalneario_SeguroSaudeId",
                table: "UtenteBalneario",
                column: "SeguroSaudeId");

            migrationBuilder.AddForeignKey(
                name: "FK_UtenteBalneario_DadosMedicos_DadosMedicosId",
                table: "UtenteBalneario",
                column: "DadosMedicosId",
                principalTable: "DadosMedicos",
                principalColumn: "DadosMedicosId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UtenteBalneario_SeguroSaude_SeguroSaudeId",
                table: "UtenteBalneario",
                column: "SeguroSaudeId",
                principalTable: "SeguroSaude",
                principalColumn: "SeguroSaudeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UtenteBalneario_DadosMedicos_DadosMedicosId",
                table: "UtenteBalneario");

            migrationBuilder.DropForeignKey(
                name: "FK_UtenteBalneario_SeguroSaude_SeguroSaudeId",
                table: "UtenteBalneario");

            migrationBuilder.DropTable(
                name: "DadosMedicos");

            migrationBuilder.DropTable(
                name: "SeguroSaude");

            migrationBuilder.DropIndex(
                name: "IX_UtenteBalneario_DadosMedicosId",
                table: "UtenteBalneario");

            migrationBuilder.DropIndex(
                name: "IX_UtenteBalneario_SeguroSaudeId",
                table: "UtenteBalneario");

            migrationBuilder.DropColumn(
                name: "DadosMedicosId",
                table: "UtenteBalneario");

            migrationBuilder.DropColumn(
                name: "NomeCompleto",
                table: "UtenteBalneario");

            migrationBuilder.DropColumn(
                name: "SeguroSaudeId",
                table: "UtenteBalneario");

            migrationBuilder.AlterColumn<string>(
                name: "Sexo",
                table: "UtenteBalneario",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "NIF",
                table: "UtenteBalneario",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(9)",
                oldMaxLength: 9);

            migrationBuilder.AlterColumn<string>(
                name: "Morada",
                table: "UtenteBalneario",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Contacto",
                table: "UtenteBalneario",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15);

            migrationBuilder.AddColumn<string>(
                name: "ContraIndicacoes",
                table: "UtenteBalneario",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HistoricoClinico",
                table: "UtenteBalneario",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IndicacoesTerapeuticas",
                table: "UtenteBalneario",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MedicoResponsavel",
                table: "UtenteBalneario",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Nome",
                table: "UtenteBalneario",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SeguroSaude",
                table: "UtenteBalneario",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
