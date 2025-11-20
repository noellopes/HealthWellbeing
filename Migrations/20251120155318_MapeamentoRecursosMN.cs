using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class MapeamentoRecursosMN : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExameTipoRecursos",
                columns: table => new
                {
                    ExameTipoId = table.Column<int>(type: "int", nullable: false),
                    MaterialEquipamentoAssociadoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExameTipoRecursos", x => new { x.ExameTipoId, x.MaterialEquipamentoAssociadoId });
                    table.ForeignKey(
                        name: "FK_ExameTipoRecursos_ExameTipo_ExameTipoId",
                        column: x => x.ExameTipoId,
                        principalTable: "ExameTipo",
                        principalColumn: "ExameTipoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExameTipoRecursos_MaterialEquipamentoAssociado_MaterialEquipamentoAssociadoId",
                        column: x => x.MaterialEquipamentoAssociadoId,
                        principalTable: "MaterialEquipamentoAssociado",
                        principalColumn: "MaterialEquipamentoAssociadoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExameTipoRecursos_MaterialEquipamentoAssociadoId",
                table: "ExameTipoRecursos",
                column: "MaterialEquipamentoAssociadoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExameTipoRecursos");
        }
    }
}
