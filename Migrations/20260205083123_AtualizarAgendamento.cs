using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class AtualizarAgendamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HorariosTerapeuta");

            migrationBuilder.DropTable(
                name: "Member");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropColumn(
                name: "ServicoId",
                table: "Agendamentos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ServicoId",
                table: "Agendamentos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    ClientId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.ClientId);
                });

            migrationBuilder.CreateTable(
                name: "HorariosTerapeuta",
                columns: table => new
                {
                    HorarioTerapeutaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EspecialidadeId = table.Column<int>(type: "int", nullable: false),
                    TerapeutaId = table.Column<int>(type: "int", nullable: false),
                    DataHoraFim = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataHoraInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HorariosTerapeuta", x => x.HorarioTerapeutaId);
                    table.ForeignKey(
                        name: "FK_HorariosTerapeuta_Especialidades_EspecialidadeId",
                        column: x => x.EspecialidadeId,
                        principalTable: "Especialidades",
                        principalColumn: "EspecialidadeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HorariosTerapeuta_Terapeutas_TerapeutaId",
                        column: x => x.TerapeutaId,
                        principalTable: "Terapeutas",
                        principalColumn: "TerapeutaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Member",
                columns: table => new
                {
                    MemberId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Member", x => x.MemberId);
                    table.ForeignKey(
                        name: "FK_Member_Clientes_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clientes",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HorariosTerapeuta_EspecialidadeId",
                table: "HorariosTerapeuta",
                column: "EspecialidadeId");

            migrationBuilder.CreateIndex(
                name: "IX_HorariosTerapeuta_TerapeutaId",
                table: "HorariosTerapeuta",
                column: "TerapeutaId");

            migrationBuilder.CreateIndex(
                name: "IX_Member_ClientId",
                table: "Member",
                column: "ClientId",
                unique: true);
        }
    }
}
