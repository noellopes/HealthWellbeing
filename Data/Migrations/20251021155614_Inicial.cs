using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Data.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Servicos",
                columns: table => new
                {
                    ServicoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Preco = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DuracaoMinutos = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servicos", x => x.ServicoId);
                });

            migrationBuilder.CreateTable(
                name: "Terapeutas",
                columns: table => new
                {
                    TerapeutaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Especialidade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnosExperiencia = table.Column<int>(type: "int", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Terapeutas", x => x.TerapeutaId);
                });

            migrationBuilder.CreateTable(
                name: "UtenteBalneario",
                columns: table => new
                {
                    UtenteBalnearioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataNascimento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Sexo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NIF = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Contacto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Morada = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HistoricoClinico = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IndicacoesTerapeuticas = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContraIndicacoes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MedicoResponsavel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataInscricao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SeguroSaude = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UtenteBalneario", x => x.UtenteBalnearioId);
                });

            migrationBuilder.CreateTable(
                name: "Agendamentos",
                columns: table => new
                {
                    AgendamentoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataHoraInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataHoraFim = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UtenteBalnearioId = table.Column<int>(type: "int", nullable: false),
                    TerapeutaId = table.Column<int>(type: "int", nullable: false),
                    ServicoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agendamentos", x => x.AgendamentoId);
                    table.ForeignKey(
                        name: "FK_Agendamentos_Servicos_ServicoId",
                        column: x => x.ServicoId,
                        principalTable: "Servicos",
                        principalColumn: "ServicoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Agendamentos_Terapeutas_TerapeutaId",
                        column: x => x.TerapeutaId,
                        principalTable: "Terapeutas",
                        principalColumn: "TerapeutaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Agendamentos_UtenteBalneario_UtenteBalnearioId",
                        column: x => x.UtenteBalnearioId,
                        principalTable: "UtenteBalneario",
                        principalColumn: "UtenteBalnearioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_ServicoId",
                table: "Agendamentos",
                column: "ServicoId");

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_TerapeutaId",
                table: "Agendamentos",
                column: "TerapeutaId");

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_UtenteBalnearioId",
                table: "Agendamentos",
                column: "UtenteBalnearioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Agendamentos");

            migrationBuilder.DropTable(
                name: "Servicos");

            migrationBuilder.DropTable(
                name: "Terapeutas");

            migrationBuilder.DropTable(
                name: "UtenteBalneario");
        }
    }
}
