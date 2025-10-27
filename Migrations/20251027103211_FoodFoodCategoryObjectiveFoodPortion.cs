using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class FoodFoodCategoryObjectiveFoodPortion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FoodCategory",
                columns: table => new
                {
                    FoodCategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodCategory", x => x.FoodCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "FoodPortion",
                columns: table => new
                {
                    FoodPortionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FoodName = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Amount = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodPortion", x => x.FoodPortionId);
                });

            migrationBuilder.CreateTable(
                name: "Objective",
                columns: table => new
                {
                    ObjectiveId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Details = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Objective", x => x.ObjectiveId);
                });

            migrationBuilder.CreateTable(
                name: "Receita",
                columns: table => new
                {
                    ReceitaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ModoPreparo = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    TempoPreparo = table.Column<int>(type: "int", nullable: false),
                    Porcoes = table.Column<int>(type: "int", nullable: false),
                    CaloriasPorPorcao = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Proteinas = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HidratosCarbono = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Gorduras = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsVegetariana = table.Column<bool>(type: "bit", nullable: false),
                    IsVegan = table.Column<bool>(type: "bit", nullable: false),
                    IsLactoseFree = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receita", x => x.ReceitaId);
                });

            migrationBuilder.CreateTable(
                name: "RestricaoAlimentar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Gravidade = table.Column<int>(type: "int", nullable: false),
                    Sintomas = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestricaoAlimentar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Food",
                columns: table => new
                {
                    FoodId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    KcalPer100g = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProteinPer100g = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CarbsPer100g = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FatPer100g = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FoodCategoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Food", x => x.FoodId);
                    table.ForeignKey(
                        name: "FK_Food_FoodCategory_FoodCategoryId",
                        column: x => x.FoodCategoryId,
                        principalTable: "FoodCategory",
                        principalColumn: "FoodCategoryId");
                });

            migrationBuilder.CreateTable(
                name: "Alergia",
                columns: table => new
                {
                    AlergiaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gravidade = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Sintomas = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    FoodId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alergia", x => x.AlergiaID);
                    table.ForeignKey(
                        name: "FK_Alergia_Food_FoodId",
                        column: x => x.FoodId,
                        principalTable: "Food",
                        principalColumn: "FoodId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alergia_FoodId",
                table: "Alergia",
                column: "FoodId");

            migrationBuilder.CreateIndex(
                name: "IX_Food_FoodCategoryId",
                table: "Food",
                column: "FoodCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alergia");

            migrationBuilder.DropTable(
                name: "FoodPortion");

            migrationBuilder.DropTable(
                name: "Objective");

            migrationBuilder.DropTable(
                name: "Receita");

            migrationBuilder.DropTable(
                name: "RestricaoAlimentar");

            migrationBuilder.DropTable(
                name: "Food");

            migrationBuilder.DropTable(
                name: "FoodCategory");
        }
    }
}
