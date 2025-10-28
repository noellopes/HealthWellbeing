using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateNovo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BeneficioTipoExercicios",
                table: "TipoExercicio");

            migrationBuilder.DropColumn(
                name: "GruposMuscularesTrabalhadosTipoExercicios",
                table: "TipoExercicio");

            migrationBuilder.RenameColumn(
                name: "CategoriaTipoExercicios",
                table: "TipoExercicio",
                newName: "NomeTipoExercicios");

            migrationBuilder.AlterColumn<string>(
                name: "NivelDificuldadeTipoExercicios",
                table: "TipoExercicio",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.CreateTable(
                name: "Beneficios",
                columns: table => new
                {
                    BeneficioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beneficios", x => x.BeneficioId);
                });

            migrationBuilder.CreateTable(
                name: "BeneficioTipoExercicio",
                columns: table => new
                {
                    BeneficiosBeneficioId = table.Column<int>(type: "int", nullable: false),
                    TiposExercicioTipoExercicioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeneficioTipoExercicio", x => new { x.BeneficiosBeneficioId, x.TiposExercicioTipoExercicioId });
                    table.ForeignKey(
                        name: "FK_BeneficioTipoExercicio_Beneficios_BeneficiosBeneficioId",
                        column: x => x.BeneficiosBeneficioId,
                        principalTable: "Beneficios",
                        principalColumn: "BeneficioId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BeneficioTipoExercicio_TipoExercicio_TiposExercicioTipoExercicioId",
                        column: x => x.TiposExercicioTipoExercicioId,
                        principalTable: "TipoExercicio",
                        principalColumn: "TipoExercicioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BeneficioTipoExercicio_TiposExercicioTipoExercicioId",
                table: "BeneficioTipoExercicio",
                column: "TiposExercicioTipoExercicioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BeneficioTipoExercicio");

            migrationBuilder.DropTable(
                name: "Beneficios");

            migrationBuilder.RenameColumn(
                name: "NomeTipoExercicios",
                table: "TipoExercicio",
                newName: "CategoriaTipoExercicios");

            migrationBuilder.AlterColumn<string>(
                name: "NivelDificuldadeTipoExercicios",
                table: "TipoExercicio",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<string>(
                name: "BeneficioTipoExercicios",
                table: "TipoExercicio",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GruposMuscularesTrabalhadosTipoExercicios",
                table: "TipoExercicio",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");
        }
    }
}
