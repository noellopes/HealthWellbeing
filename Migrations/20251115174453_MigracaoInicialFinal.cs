using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class MigracaoInicialFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExameTipo",
                columns: table => new
                {
                    ExameTipoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Especialidade = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ExameTipoId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExameTipo", x => x.ExameTipoId);
                    table.ForeignKey(
                        name: "FK_ExameTipo_ExameTipo_ExameTipoId1",
                        column: x => x.ExameTipoId1,
                        principalTable: "ExameTipo",
                        principalColumn: "ExameTipoId");
                });

            migrationBuilder.CreateTable(
                name: "MaterialEquipamentoAssociado",
                columns: table => new
                {
                    MaterialEquipamentoAssociadoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeEquipamento = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    EstadoComponente = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialEquipamentoAssociado", x => x.MaterialEquipamentoAssociadoId);
                });

            migrationBuilder.CreateTable(
                name: "Medicos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Especialidade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProfissionalExecutante",
                columns: table => new
                {
                    ProfissionalExecutanteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Funcao = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfissionalExecutante", x => x.ProfissionalExecutanteId);
                });

            migrationBuilder.CreateTable(
                name: "SalaDeExame",
                columns: table => new
                {
                    SalaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoSala = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Laboratorio = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalaDeExame", x => x.SalaId);
                });

            migrationBuilder.CreateTable(
                name: "Utentes",
                columns: table => new
                {
                    UtenteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UtenteSNS = table.Column<int>(type: "int", nullable: false),
                    Nif = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    NumCC = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Genero = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Morada = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CodigoPostal = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Numero = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NomeEmergencia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NumeroEmergencia = table.Column<int>(type: "int", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utentes", x => x.UtenteId);
                });

            migrationBuilder.CreateTable(
                name: "Exames",
                columns: table => new
                {
                    ExameId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataHoraMarcacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    Notas = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UtenteId = table.Column<int>(type: "int", nullable: false),
                    ExameTipoId = table.Column<int>(type: "int", nullable: false),
                    MedicoId = table.Column<int>(type: "int", nullable: false),
                    MedicoSolicitanteId = table.Column<int>(type: "int", nullable: true),
                    SalaDeExameId = table.Column<int>(type: "int", nullable: true),
                    ProfissionalExecutanteId = table.Column<int>(type: "int", nullable: true),
                    MaterialEquipamentoAssociadoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exames", x => x.ExameId);
                    table.ForeignKey(
                        name: "FK_Exames_ExameTipo_ExameTipoId",
                        column: x => x.ExameTipoId,
                        principalTable: "ExameTipo",
                        principalColumn: "ExameTipoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Exames_MaterialEquipamentoAssociado_MaterialEquipamentoAssociadoId",
                        column: x => x.MaterialEquipamentoAssociadoId,
                        principalTable: "MaterialEquipamentoAssociado",
                        principalColumn: "MaterialEquipamentoAssociadoId");
                    table.ForeignKey(
                        name: "FK_Exames_Medicos_MedicoSolicitanteId",
                        column: x => x.MedicoSolicitanteId,
                        principalTable: "Medicos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Exames_ProfissionalExecutante_ProfissionalExecutanteId",
                        column: x => x.ProfissionalExecutanteId,
                        principalTable: "ProfissionalExecutante",
                        principalColumn: "ProfissionalExecutanteId");
                    table.ForeignKey(
                        name: "FK_Exames_SalaDeExame_SalaDeExameId",
                        column: x => x.SalaDeExameId,
                        principalTable: "SalaDeExame",
                        principalColumn: "SalaId");
                    table.ForeignKey(
                        name: "FK_Exames_Utentes_UtenteId",
                        column: x => x.UtenteId,
                        principalTable: "Utentes",
                        principalColumn: "UtenteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Exames_ExameTipoId",
                table: "Exames",
                column: "ExameTipoId");

            migrationBuilder.CreateIndex(
                name: "IX_Exames_MaterialEquipamentoAssociadoId",
                table: "Exames",
                column: "MaterialEquipamentoAssociadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Exames_MedicoSolicitanteId",
                table: "Exames",
                column: "MedicoSolicitanteId");

            migrationBuilder.CreateIndex(
                name: "IX_Exames_ProfissionalExecutanteId",
                table: "Exames",
                column: "ProfissionalExecutanteId");

            migrationBuilder.CreateIndex(
                name: "IX_Exames_SalaDeExameId",
                table: "Exames",
                column: "SalaDeExameId");

            migrationBuilder.CreateIndex(
                name: "IX_Exames_UtenteId",
                table: "Exames",
                column: "UtenteId");

            migrationBuilder.CreateIndex(
                name: "IX_ExameTipo_ExameTipoId1",
                table: "ExameTipo",
                column: "ExameTipoId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Exames");

            migrationBuilder.DropTable(
                name: "ExameTipo");

            migrationBuilder.DropTable(
                name: "MaterialEquipamentoAssociado");

            migrationBuilder.DropTable(
                name: "Medicos");

            migrationBuilder.DropTable(
                name: "ProfissionalExecutante");

            migrationBuilder.DropTable(
                name: "SalaDeExame");

            migrationBuilder.DropTable(
                name: "Utentes");
        }
    }
}
