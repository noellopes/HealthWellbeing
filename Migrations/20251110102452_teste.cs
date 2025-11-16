using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class teste : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receita_RestricaoAlimentar_RestricaoAlimentarId",
                table: "Receita");

            migrationBuilder.DropTable(
                name: "ComponentesDaReceita");

            migrationBuilder.DropIndex(
                name: "IX_Receita_RestricaoAlimentarId",
                table: "Receita");

            migrationBuilder.DropColumn(
                name: "IsLactoseFree",
                table: "Receita");

            migrationBuilder.DropColumn(
                name: "IsVegan",
                table: "Receita");

            migrationBuilder.DropColumn(
                name: "IsVegetariana",
                table: "Receita");

            migrationBuilder.DropColumn(
                name: "RestricaoAlimentarId",
                table: "Receita");

            migrationBuilder.RenameColumn(
                name: "CaloriasPorPorcao",
                table: "Receita",
                newName: "Calorias");

            migrationBuilder.AddColumn<string>(
                name: "ComponentesReceitaId",
                table: "Receita",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "RestricoesAlimentarId",
                table: "Receita",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.CreateTable(
                name: "ComponenteReceita",
                columns: table => new
                {
                    ComponenteReceitaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlimentoId = table.Column<int>(type: "int", nullable: false),
                    UnidadeMedida = table.Column<int>(type: "int", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    IsOpcional = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponenteReceita", x => x.ComponenteReceitaId);
                    table.ForeignKey(
                        name: "FK_ComponenteReceita_Alimentos_AlimentoId",
                        column: x => x.AlimentoId,
                        principalTable: "Alimentos",
                        principalColumn: "AlimentoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReceitaRestricaoAlimentar",
                columns: table => new
                {
                    ReceitasReceitaId = table.Column<int>(type: "int", nullable: false),
                    RestricoesAlimentaresRestricaoAlimentarId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceitaRestricaoAlimentar", x => new { x.ReceitasReceitaId, x.RestricoesAlimentaresRestricaoAlimentarId });
                    table.ForeignKey(
                        name: "FK_ReceitaRestricaoAlimentar_Receita_ReceitasReceitaId",
                        column: x => x.ReceitasReceitaId,
                        principalTable: "Receita",
                        principalColumn: "ReceitaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReceitaRestricaoAlimentar_RestricaoAlimentar_RestricoesAlimentaresRestricaoAlimentarId",
                        column: x => x.RestricoesAlimentaresRestricaoAlimentarId,
                        principalTable: "RestricaoAlimentar",
                        principalColumn: "RestricaoAlimentarId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComponenteReceitaReceita",
                columns: table => new
                {
                    ComponentesComponenteReceitaId = table.Column<int>(type: "int", nullable: false),
                    ReceitaRelacionadasReceitaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponenteReceitaReceita", x => new { x.ComponentesComponenteReceitaId, x.ReceitaRelacionadasReceitaId });
                    table.ForeignKey(
                        name: "FK_ComponenteReceitaReceita_ComponenteReceita_ComponentesComponenteReceitaId",
                        column: x => x.ComponentesComponenteReceitaId,
                        principalTable: "ComponenteReceita",
                        principalColumn: "ComponenteReceitaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComponenteReceitaReceita_Receita_ReceitaRelacionadasReceitaId",
                        column: x => x.ReceitaRelacionadasReceitaId,
                        principalTable: "Receita",
                        principalColumn: "ReceitaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComponenteReceita_AlimentoId",
                table: "ComponenteReceita",
                column: "AlimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponenteReceitaReceita_ReceitaRelacionadasReceitaId",
                table: "ComponenteReceitaReceita",
                column: "ReceitaRelacionadasReceitaId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceitaRestricaoAlimentar_RestricoesAlimentaresRestricaoAlimentarId",
                table: "ReceitaRestricaoAlimentar",
                column: "RestricoesAlimentaresRestricaoAlimentarId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComponenteReceitaReceita");

            migrationBuilder.DropTable(
                name: "ReceitaRestricaoAlimentar");

            migrationBuilder.DropTable(
                name: "ComponenteReceita");

            migrationBuilder.DropColumn(
                name: "ComponentesReceitaId",
                table: "Receita");

            migrationBuilder.DropColumn(
                name: "RestricoesAlimentarId",
                table: "Receita");

            migrationBuilder.RenameColumn(
                name: "Calorias",
                table: "Receita",
                newName: "CaloriasPorPorcao");

            migrationBuilder.AddColumn<bool>(
                name: "IsLactoseFree",
                table: "Receita",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVegan",
                table: "Receita",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVegetariana",
                table: "Receita",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "RestricaoAlimentarId",
                table: "Receita",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ComponentesDaReceita",
                columns: table => new
                {
                    ComponentesDaReceitaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceitaId = table.Column<int>(type: "int", nullable: false),
                    Calorias = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    Gorduras = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HidratosCarbono = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsOpcional = table.Column<bool>(type: "bit", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Proteinas = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantidade = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnidadeMedida = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentesDaReceita", x => x.ComponentesDaReceitaId);
                    table.ForeignKey(
                        name: "FK_ComponentesDaReceita_Receita_ReceitaId",
                        column: x => x.ReceitaId,
                        principalTable: "Receita",
                        principalColumn: "ReceitaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Receita_RestricaoAlimentarId",
                table: "Receita",
                column: "RestricaoAlimentarId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponentesDaReceita_ReceitaId",
                table: "ComponentesDaReceita",
                column: "ReceitaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Receita_RestricaoAlimentar_RestricaoAlimentarId",
                table: "Receita",
                column: "RestricaoAlimentarId",
                principalTable: "RestricaoAlimentar",
                principalColumn: "RestricaoAlimentarId");
        }
    }
}
