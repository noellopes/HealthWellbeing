using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TipoExercicio",
                columns: table => new
                {
                    TipoExercicioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoriaTipoExercicios = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DescricaoTipoExercicios = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    NivelDificuldadeTipoExercicios = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BeneficioTipoExercicios = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    GruposMuscularesTrabalhadosTipoExercicios = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoExercicio", x => x.TipoExercicioId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TipoExercicio");
        }
    }
}
