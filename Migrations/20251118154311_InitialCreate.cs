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
                name: "Beneficio",
                columns: table => new
                {
                    BeneficioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeBeneficio = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DescricaoBeneficio = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beneficio", x => x.BeneficioId);
                });

            migrationBuilder.CreateTable(
                name: "CategoriaAlimento",
                columns: table => new
                {
                    CategoriaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriaAlimento", x => x.CategoriaID);
                });

            migrationBuilder.CreateTable(
                name: "Exercicio",
                columns: table => new
                {
                    ExercicioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExercicioNome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Duracao = table.Column<double>(type: "float", nullable: false),
                    Intencidade = table.Column<int>(type: "int", nullable: false),
                    CaloriasGastas = table.Column<double>(type: "float", nullable: false),
                    Instrucoes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EquipamentoNecessario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Repeticoes = table.Column<int>(type: "int", nullable: false),
                    Series = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercicio", x => x.ExercicioId);
                });

            migrationBuilder.CreateTable(
                name: "Genero",
                columns: table => new
                {
                    GeneroId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeGenero = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genero", x => x.GeneroId);
                });

            migrationBuilder.CreateTable(
                name: "GrupoMuscular",
                columns: table => new
                {
                    GrupoMuscularId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GrupoMuscularNome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LocalizacaoCorporal = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrupoMuscular", x => x.GrupoMuscularId);
                });

            migrationBuilder.CreateTable(
                name: "ProblemaSaude",
                columns: table => new
                {
                    ProblemaSaudeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProblemaCategoria = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProblemaNome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ZonaAtingida = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Gravidade = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProblemaSaude", x => x.ProblemaSaudeId);
                });

            migrationBuilder.CreateTable(
                name: "ProfissionalExecutante",
                columns: table => new
                {
                    ProfissionalExecutanteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Funcao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfissionalExecutante", x => x.ProfissionalExecutanteId);
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
                name: "TipoExercicio",
                columns: table => new
                {
                    TipoExercicioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeTipoExercicios = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DescricaoTipoExercicios = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CaracteristicasTipoExercicios = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoExercicio", x => x.TipoExercicioId);
                });

            migrationBuilder.CreateTable(
                name: "Alimento",
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
                    table.PrimaryKey("PK_Alimento", x => x.AlimentoId);
                    table.ForeignKey(
                        name: "FK_Alimento_CategoriaAlimento_CategoriaAlimentoId",
                        column: x => x.CategoriaAlimentoId,
                        principalTable: "CategoriaAlimento",
                        principalColumn: "CategoriaID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExercicioGenero",
                columns: table => new
                {
                    ExercicioId = table.Column<int>(type: "int", nullable: false),
                    GeneroId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExercicioGenero", x => new { x.ExercicioId, x.GeneroId });
                    table.ForeignKey(
                        name: "FK_ExercicioGenero_Exercicio_ExercicioId",
                        column: x => x.ExercicioId,
                        principalTable: "Exercicio",
                        principalColumn: "ExercicioId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExercicioGenero_Genero_GeneroId",
                        column: x => x.GeneroId,
                        principalTable: "Genero",
                        principalColumn: "GeneroId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExercicioGrupoMuscular",
                columns: table => new
                {
                    ExercicioId = table.Column<int>(type: "int", nullable: false),
                    GrupoMuscularId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExercicioGrupoMuscular", x => new { x.ExercicioId, x.GrupoMuscularId });
                    table.ForeignKey(
                        name: "FK_ExercicioGrupoMuscular_Exercicio_ExercicioId",
                        column: x => x.ExercicioId,
                        principalTable: "Exercicio",
                        principalColumn: "ExercicioId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExercicioGrupoMuscular_GrupoMuscular_GrupoMuscularId",
                        column: x => x.GrupoMuscularId,
                        principalTable: "GrupoMuscular",
                        principalColumn: "GrupoMuscularId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Musculo",
                columns: table => new
                {
                    MusculoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome_Musculo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    GrupoMuscularId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Musculo", x => x.MusculoId);
                    table.ForeignKey(
                        name: "FK_Musculo_GrupoMuscular_GrupoMuscularId",
                        column: x => x.GrupoMuscularId,
                        principalTable: "GrupoMuscular",
                        principalColumn: "GrupoMuscularId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProblemaSaudeProfissionais",
                columns: table => new
                {
                    ProblemasSaudeProblemaSaudeId = table.Column<int>(type: "int", nullable: false),
                    ProfissionalExecutanteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProblemaSaudeProfissionais", x => new { x.ProblemasSaudeProblemaSaudeId, x.ProfissionalExecutanteId });
                    table.ForeignKey(
                        name: "FK_ProblemaSaudeProfissionais_ProblemaSaude_ProblemasSaudeProblemaSaudeId",
                        column: x => x.ProblemasSaudeProblemaSaudeId,
                        principalTable: "ProblemaSaude",
                        principalColumn: "ProblemaSaudeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProblemaSaudeProfissionais_ProfissionalExecutante_ProfissionalExecutanteId",
                        column: x => x.ProfissionalExecutanteId,
                        principalTable: "ProfissionalExecutante",
                        principalColumn: "ProfissionalExecutanteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TipoExercicioBeneficio",
                columns: table => new
                {
                    TipoExercicioId = table.Column<int>(type: "int", nullable: false),
                    BeneficioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoExercicioBeneficio", x => new { x.TipoExercicioId, x.BeneficioId });
                    table.ForeignKey(
                        name: "FK_TipoExercicioBeneficio_Beneficio_BeneficioId",
                        column: x => x.BeneficioId,
                        principalTable: "Beneficio",
                        principalColumn: "BeneficioId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TipoExercicioBeneficio_TipoExercicio_TipoExercicioId",
                        column: x => x.TipoExercicioId,
                        principalTable: "TipoExercicio",
                        principalColumn: "TipoExercicioId",
                        onDelete: ReferentialAction.Cascade);
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
                    AlimentoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alergia", x => x.AlergiaID);
                    table.ForeignKey(
                        name: "FK_Alergia_Alimento_AlimentoId",
                        column: x => x.AlimentoId,
                        principalTable: "Alimento",
                        principalColumn: "AlimentoId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alergia_AlimentoId",
                table: "Alergia",
                column: "AlimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Alimento_CategoriaAlimentoId",
                table: "Alimento",
                column: "CategoriaAlimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_ExercicioGenero_GeneroId",
                table: "ExercicioGenero",
                column: "GeneroId");

            migrationBuilder.CreateIndex(
                name: "IX_ExercicioGrupoMuscular_GrupoMuscularId",
                table: "ExercicioGrupoMuscular",
                column: "GrupoMuscularId");

            migrationBuilder.CreateIndex(
                name: "IX_Musculo_GrupoMuscularId",
                table: "Musculo",
                column: "GrupoMuscularId");

            migrationBuilder.CreateIndex(
                name: "IX_ProblemaSaudeProfissionais_ProfissionalExecutanteId",
                table: "ProblemaSaudeProfissionais",
                column: "ProfissionalExecutanteId");

            migrationBuilder.CreateIndex(
                name: "IX_TipoExercicioBeneficio_BeneficioId",
                table: "TipoExercicioBeneficio",
                column: "BeneficioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alergia");

            migrationBuilder.DropTable(
                name: "ExercicioGenero");

            migrationBuilder.DropTable(
                name: "ExercicioGrupoMuscular");

            migrationBuilder.DropTable(
                name: "Musculo");

            migrationBuilder.DropTable(
                name: "ProblemaSaudeProfissionais");

            migrationBuilder.DropTable(
                name: "Receita");

            migrationBuilder.DropTable(
                name: "RestricaoAlimentar");

            migrationBuilder.DropTable(
                name: "TipoExercicioBeneficio");

            migrationBuilder.DropTable(
                name: "Alimento");

            migrationBuilder.DropTable(
                name: "Genero");

            migrationBuilder.DropTable(
                name: "Exercicio");

            migrationBuilder.DropTable(
                name: "GrupoMuscular");

            migrationBuilder.DropTable(
                name: "ProblemaSaude");

            migrationBuilder.DropTable(
                name: "ProfissionalExecutante");

            migrationBuilder.DropTable(
                name: "Beneficio");

            migrationBuilder.DropTable(
                name: "TipoExercicio");

            migrationBuilder.DropTable(
                name: "CategoriaAlimento");
        }
    }
}
