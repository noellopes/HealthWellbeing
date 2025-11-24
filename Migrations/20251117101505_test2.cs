using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class test2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alergia",
                columns: table => new
                {
                    AlergiaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gravidade = table.Column<int>(type: "int", nullable: false),
                    Sintomas = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alergia", x => x.AlergiaId);
                });

            migrationBuilder.CreateTable(
                name: "CategoriaAlimento",
                columns: table => new
                {
                    CategoriaAlimentoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriaAlimento", x => x.CategoriaAlimentoId);
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
                    Calorias = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Proteinas = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HidratosCarbono = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Gorduras = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RestricoesAlimentarId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receita", x => x.ReceitaId);
                });

            migrationBuilder.CreateTable(
                name: "RestricaoAlimentar",
                columns: table => new
                {
                    RestricaoAlimentarId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Gravidade = table.Column<int>(type: "int", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestricaoAlimentar", x => x.RestricaoAlimentarId);
                });

            migrationBuilder.CreateTable(
                name: "Alimentos",
                columns: table => new
                {
                    AlimentoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoriaAlimentoId = table.Column<int>(type: "int", nullable: false),
                    Calories = table.Column<int>(type: "int", nullable: false),
                    KcalPor100g = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProteinaGPor100g = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HidratosGPor100g = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GorduraGPor100g = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alimentos", x => x.AlimentoId);
                    table.ForeignKey(
                        name: "FK_Alimentos_CategoriaAlimento_CategoriaAlimentoId",
                        column: x => x.CategoriaAlimentoId,
                        principalTable: "CategoriaAlimento",
                        principalColumn: "CategoriaAlimentoId",
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
                name: "AlergiaAlimento",
                columns: table => new
                {
                    AlergiaId = table.Column<int>(type: "int", nullable: false),
                    AlimentoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlergiaAlimento", x => new { x.AlergiaId, x.AlimentoId });
                    table.ForeignKey(
                        name: "FK_AlergiaAlimento_Alergia_AlergiaId",
                        column: x => x.AlergiaId,
                        principalTable: "Alergia",
                        principalColumn: "AlergiaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlergiaAlimento_Alimentos_AlimentoId",
                        column: x => x.AlimentoId,
                        principalTable: "Alimentos",
                        principalColumn: "AlimentoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AlimentoSubstitutos",
                columns: table => new
                {
                    AlimentoSubstitutoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlimentoOriginalId = table.Column<int>(type: "int", nullable: false),
                    AlimentoSubstitutoRefId = table.Column<int>(type: "int", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ProporcaoEquivalente = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Observacoes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FatorSimilaridade = table.Column<double>(type: "float", nullable: true),
                    RestricaoAlimentarId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlimentoSubstitutos", x => x.AlimentoSubstitutoId);
                    table.ForeignKey(
                        name: "FK_AlimentoSubstitutos_Alimentos_AlimentoOriginalId",
                        column: x => x.AlimentoOriginalId,
                        principalTable: "Alimentos",
                        principalColumn: "AlimentoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AlimentoSubstitutos_Alimentos_AlimentoSubstitutoRefId",
                        column: x => x.AlimentoSubstitutoRefId,
                        principalTable: "Alimentos",
                        principalColumn: "AlimentoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AlimentoSubstitutos_RestricaoAlimentar_RestricaoAlimentarId",
                        column: x => x.RestricaoAlimentarId,
                        principalTable: "RestricaoAlimentar",
                        principalColumn: "RestricaoAlimentarId");
                });

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
                name: "RestricaoAlimentarAlimento",
                columns: table => new
                {
                    RestricaoAlimentarId = table.Column<int>(type: "int", nullable: false),
                    AlimentoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestricaoAlimentarAlimento", x => new { x.RestricaoAlimentarId, x.AlimentoId });
                    table.ForeignKey(
                        name: "FK_RestricaoAlimentarAlimento_Alimentos_AlimentoId",
                        column: x => x.AlimentoId,
                        principalTable: "Alimentos",
                        principalColumn: "AlimentoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RestricaoAlimentarAlimento_RestricaoAlimentar_RestricaoAlimentarId",
                        column: x => x.RestricaoAlimentarId,
                        principalTable: "RestricaoAlimentar",
                        principalColumn: "RestricaoAlimentarId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReceitaComponente",
                columns: table => new
                {
                    ReceitaId = table.Column<int>(type: "int", nullable: false),
                    ComponenteReceitaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceitaComponente", x => new { x.ReceitaId, x.ComponenteReceitaId });
                    table.ForeignKey(
                        name: "FK_ReceitaComponente_ComponenteReceita_ComponenteReceitaId",
                        column: x => x.ComponenteReceitaId,
                        principalTable: "ComponenteReceita",
                        principalColumn: "ComponenteReceitaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReceitaComponente_Receita_ReceitaId",
                        column: x => x.ReceitaId,
                        principalTable: "Receita",
                        principalColumn: "ReceitaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlergiaAlimento_AlimentoId",
                table: "AlergiaAlimento",
                column: "AlimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Alimentos_CategoriaAlimentoId",
                table: "Alimentos",
                column: "CategoriaAlimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_AlimentoSubstitutos_AlimentoOriginalId",
                table: "AlimentoSubstitutos",
                column: "AlimentoOriginalId");

            migrationBuilder.CreateIndex(
                name: "IX_AlimentoSubstitutos_AlimentoSubstitutoRefId",
                table: "AlimentoSubstitutos",
                column: "AlimentoSubstitutoRefId");

            migrationBuilder.CreateIndex(
                name: "IX_AlimentoSubstitutos_RestricaoAlimentarId",
                table: "AlimentoSubstitutos",
                column: "RestricaoAlimentarId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponenteReceita_AlimentoId",
                table: "ComponenteReceita",
                column: "AlimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceitaComponente_ComponenteReceitaId",
                table: "ReceitaComponente",
                column: "ComponenteReceitaId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceitaRestricaoAlimentar_RestricoesAlimentaresRestricaoAlimentarId",
                table: "ReceitaRestricaoAlimentar",
                column: "RestricoesAlimentaresRestricaoAlimentarId");

            migrationBuilder.CreateIndex(
                name: "IX_RestricaoAlimentarAlimento_AlimentoId",
                table: "RestricaoAlimentarAlimento",
                column: "AlimentoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlergiaAlimento");

            migrationBuilder.DropTable(
                name: "AlimentoSubstitutos");

            migrationBuilder.DropTable(
                name: "ReceitaComponente");

            migrationBuilder.DropTable(
                name: "ReceitaRestricaoAlimentar");

            migrationBuilder.DropTable(
                name: "RestricaoAlimentarAlimento");

            migrationBuilder.DropTable(
                name: "Alergia");

            migrationBuilder.DropTable(
                name: "ComponenteReceita");

            migrationBuilder.DropTable(
                name: "Receita");

            migrationBuilder.DropTable(
                name: "RestricaoAlimentar");

            migrationBuilder.DropTable(
                name: "Alimentos");

            migrationBuilder.DropTable(
                name: "CategoriaAlimento");
        }
    }
}
