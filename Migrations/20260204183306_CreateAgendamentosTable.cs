using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace HealthWellbeing.Migrations
{
    public partial class CreateAgendamentosTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Agendamentos",
                columns: table => new
                {
                    AgendamentoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataHoraInicio = table.Column<DateTime>(nullable: false),
                    DataHoraFim = table.Column<DateTime>(nullable: false),
                    Estado = table.Column<string>(maxLength: 30, nullable: false),
                    TerapeutaId = table.Column<int>(nullable: false),
                    ServicoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agendamentos", x => x.AgendamentoId);

                    table.ForeignKey(
                        name: "FK_Agendamentos_Terapeutas_TerapeutaId",
                        column: x => x.TerapeutaId,
                        principalTable: "Terapeutas",
                        principalColumn: "TerapeutaId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Agendamentos");
        }
    }
}