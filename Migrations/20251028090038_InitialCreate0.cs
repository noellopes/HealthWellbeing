using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate0 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NivelDificuldadeTipoExercicios",
                table: "TipoExercicio",
                newName: "CaracteristicasTipoExercicios");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CaracteristicasTipoExercicios",
                table: "TipoExercicio",
                newName: "NivelDificuldadeTipoExercicios");
        }
    }
}
