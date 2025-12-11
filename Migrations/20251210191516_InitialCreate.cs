using System;
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
                name: "Equipamento",
                columns: table => new
                {
                    EquipamentoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeEquipamento = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipamento", x => x.EquipamentoId);
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
                name: "ObjetivoFisico",
                columns: table => new
                {
                    ObjetivoFisicoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeObjetivo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjetivoFisico", x => x.ObjetivoFisicoId);
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
                name: "Profissional",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Especialidade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumeroCedula = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profissional", x => x.Id);
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
                        principalColumn: "GrupoMuscularId");
                });

            migrationBuilder.CreateTable(
                name: "UtenteGrupo7",
                columns: table => new
                {
                    UtenteGrupo7Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ObjetivoFisicoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UtenteGrupo7", x => x.UtenteGrupo7Id);
                    table.ForeignKey(
                        name: "FK_UtenteGrupo7_ObjetivoFisico_ObjetivoFisicoId",
                        column: x => x.ObjetivoFisicoId,
                        principalTable: "ObjetivoFisico",
                        principalColumn: "ObjetivoFisicoId");
                });

            migrationBuilder.CreateTable(
                name: "ProblemaSaudeProfissionalExecutante",
                columns: table => new
                {
                    ProblemasSaudeProblemaSaudeId = table.Column<int>(type: "int", nullable: false),
                    ProfissionalExecutanteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProblemaSaudeProfissionalExecutante", x => new { x.ProblemasSaudeProblemaSaudeId, x.ProfissionalExecutanteId });
                    table.ForeignKey(
                        name: "FK_ProblemaSaudeProfissionalExecutante_ProblemaSaude_ProblemasSaudeProblemaSaudeId",
                        column: x => x.ProblemasSaudeProblemaSaudeId,
                        principalTable: "ProblemaSaude",
                        principalColumn: "ProblemaSaudeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProblemaSaudeProfissionalExecutante_ProfissionalExecutante_ProfissionalExecutanteId",
                        column: x => x.ProfissionalExecutanteId,
                        principalTable: "ProfissionalExecutante",
                        principalColumn: "ProfissionalExecutanteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Exercicio",
                columns: table => new
                {
                    ExercicioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExercicioNome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoExercicioId = table.Column<int>(type: "int", nullable: false),
                    Duracao = table.Column<double>(type: "float", nullable: false),
                    Intencidade = table.Column<int>(type: "int", nullable: false),
                    CaloriasGastas = table.Column<double>(type: "float", nullable: false),
                    Instrucoes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Repeticoes = table.Column<int>(type: "int", nullable: false),
                    Series = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercicio", x => x.ExercicioId);
                    table.ForeignKey(
                        name: "FK_Exercicio_TipoExercicio_TipoExercicioId",
                        column: x => x.TipoExercicioId,
                        principalTable: "TipoExercicio",
                        principalColumn: "TipoExercicioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ObjetivoTipoExercicio",
                columns: table => new
                {
                    ObjetivoFisicoId = table.Column<int>(type: "int", nullable: false),
                    TipoExercicioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjetivoTipoExercicio", x => new { x.ObjetivoFisicoId, x.TipoExercicioId });
                    table.ForeignKey(
                        name: "FK_ObjetivoTipoExercicio_ObjetivoFisico_ObjetivoFisicoId",
                        column: x => x.ObjetivoFisicoId,
                        principalTable: "ObjetivoFisico",
                        principalColumn: "ObjetivoFisicoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ObjetivoTipoExercicio_TipoExercicio_TipoExercicioId",
                        column: x => x.TipoExercicioId,
                        principalTable: "TipoExercicio",
                        principalColumn: "TipoExercicioId",
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
                name: "TipoExercicioProblemaSaude",
                columns: table => new
                {
                    TipoExercicioId = table.Column<int>(type: "int", nullable: false),
                    ProblemaSaudeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoExercicioProblemaSaude", x => new { x.TipoExercicioId, x.ProblemaSaudeId });
                    table.ForeignKey(
                        name: "FK_TipoExercicioProblemaSaude_ProblemaSaude_ProblemaSaudeId",
                        column: x => x.ProblemaSaudeId,
                        principalTable: "ProblemaSaude",
                        principalColumn: "ProblemaSaudeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TipoExercicioProblemaSaude_TipoExercicio_TipoExercicioId",
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

            migrationBuilder.CreateTable(
                name: "AvaliacaoFisica",
                columns: table => new
                {
                    AvaliacaoFisicaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataMedicao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Peso = table.Column<float>(type: "real", nullable: false),
                    Altura = table.Column<float>(type: "real", nullable: false),
                    GorduraCorporal = table.Column<float>(type: "real", nullable: false),
                    MassaMuscular = table.Column<float>(type: "real", nullable: false),
                    Pescoco = table.Column<float>(type: "real", nullable: true),
                    Ombros = table.Column<float>(type: "real", nullable: true),
                    Peitoral = table.Column<float>(type: "real", nullable: true),
                    BracoDireito = table.Column<float>(type: "real", nullable: true),
                    BracoEsquerdo = table.Column<float>(type: "real", nullable: true),
                    Cintura = table.Column<float>(type: "real", nullable: true),
                    Abdomen = table.Column<float>(type: "real", nullable: true),
                    Anca = table.Column<float>(type: "real", nullable: true),
                    CoxaDireita = table.Column<float>(type: "real", nullable: true),
                    CoxaEsquerda = table.Column<float>(type: "real", nullable: true),
                    GemeoDireito = table.Column<float>(type: "real", nullable: true),
                    GemeoEsquerdo = table.Column<float>(type: "real", nullable: true),
                    UtenteGrupo7Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvaliacaoFisica", x => x.AvaliacaoFisicaId);
                    table.ForeignKey(
                        name: "FK_AvaliacaoFisica_UtenteGrupo7_UtenteGrupo7Id",
                        column: x => x.UtenteGrupo7Id,
                        principalTable: "UtenteGrupo7",
                        principalColumn: "UtenteGrupo7Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sono",
                columns: table => new
                {
                    SonoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraDeitar = table.Column<TimeSpan>(type: "time", nullable: false),
                    HoraLevantar = table.Column<TimeSpan>(type: "time", nullable: false),
                    HorasSono = table.Column<TimeSpan>(type: "time", nullable: false),
                    UtenteGrupo7Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sono", x => x.SonoId);
                    table.ForeignKey(
                        name: "FK_Sono_UtenteGrupo7_UtenteGrupo7Id",
                        column: x => x.UtenteGrupo7Id,
                        principalTable: "UtenteGrupo7",
                        principalColumn: "UtenteGrupo7Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UtenteProblemaSaude",
                columns: table => new
                {
                    UtenteGrupo7Id = table.Column<int>(type: "int", nullable: false),
                    ProblemaSaudeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UtenteProblemaSaude", x => new { x.UtenteGrupo7Id, x.ProblemaSaudeId });
                    table.ForeignKey(
                        name: "FK_UtenteProblemaSaude_ProblemaSaude_ProblemaSaudeId",
                        column: x => x.ProblemaSaudeId,
                        principalTable: "ProblemaSaude",
                        principalColumn: "ProblemaSaudeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UtenteProblemaSaude_UtenteGrupo7_UtenteGrupo7Id",
                        column: x => x.UtenteGrupo7Id,
                        principalTable: "UtenteGrupo7",
                        principalColumn: "UtenteGrupo7Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExercicioEquipamento",
                columns: table => new
                {
                    ExercicioId = table.Column<int>(type: "int", nullable: false),
                    EquipamentoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExercicioEquipamento", x => new { x.ExercicioId, x.EquipamentoId });
                    table.ForeignKey(
                        name: "FK_ExercicioEquipamento_Equipamento_EquipamentoId",
                        column: x => x.EquipamentoId,
                        principalTable: "Equipamento",
                        principalColumn: "EquipamentoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExercicioEquipamento_Exercicio_ExercicioId",
                        column: x => x.ExercicioId,
                        principalTable: "Exercicio",
                        principalColumn: "ExercicioId",
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
                name: "ExercicioProblemaSaude",
                columns: table => new
                {
                    ExercicioId = table.Column<int>(type: "int", nullable: false),
                    ProblemaSaudeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExercicioProblemaSaude", x => new { x.ExercicioId, x.ProblemaSaudeId });
                    table.ForeignKey(
                        name: "FK_ExercicioProblemaSaude_Exercicio_ExercicioId",
                        column: x => x.ExercicioId,
                        principalTable: "Exercicio",
                        principalColumn: "ExercicioId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExercicioProblemaSaude_ProblemaSaude_ProblemaSaudeId",
                        column: x => x.ProblemaSaudeId,
                        principalTable: "ProblemaSaude",
                        principalColumn: "ProblemaSaudeId",
                        onDelete: ReferentialAction.Cascade);
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
                name: "IX_AvaliacaoFisica_UtenteGrupo7Id",
                table: "AvaliacaoFisica",
                column: "UtenteGrupo7Id");

            migrationBuilder.CreateIndex(
                name: "IX_Exercicio_TipoExercicioId",
                table: "Exercicio",
                column: "TipoExercicioId");

            migrationBuilder.CreateIndex(
                name: "IX_ExercicioEquipamento_EquipamentoId",
                table: "ExercicioEquipamento",
                column: "EquipamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_ExercicioGenero_GeneroId",
                table: "ExercicioGenero",
                column: "GeneroId");

            migrationBuilder.CreateIndex(
                name: "IX_ExercicioGrupoMuscular_GrupoMuscularId",
                table: "ExercicioGrupoMuscular",
                column: "GrupoMuscularId");

            migrationBuilder.CreateIndex(
                name: "IX_ExercicioProblemaSaude_ProblemaSaudeId",
                table: "ExercicioProblemaSaude",
                column: "ProblemaSaudeId");

            migrationBuilder.CreateIndex(
                name: "IX_Musculo_GrupoMuscularId",
                table: "Musculo",
                column: "GrupoMuscularId");

            migrationBuilder.CreateIndex(
                name: "IX_ObjetivoTipoExercicio_TipoExercicioId",
                table: "ObjetivoTipoExercicio",
                column: "TipoExercicioId");

            migrationBuilder.CreateIndex(
                name: "IX_ProblemaSaudeProfissionalExecutante_ProfissionalExecutanteId",
                table: "ProblemaSaudeProfissionalExecutante",
                column: "ProfissionalExecutanteId");

            migrationBuilder.CreateIndex(
                name: "IX_Sono_UtenteGrupo7Id",
                table: "Sono",
                column: "UtenteGrupo7Id");

            migrationBuilder.CreateIndex(
                name: "IX_TipoExercicioBeneficio_BeneficioId",
                table: "TipoExercicioBeneficio",
                column: "BeneficioId");

            migrationBuilder.CreateIndex(
                name: "IX_TipoExercicioProblemaSaude_ProblemaSaudeId",
                table: "TipoExercicioProblemaSaude",
                column: "ProblemaSaudeId");

            migrationBuilder.CreateIndex(
                name: "IX_UtenteGrupo7_ObjetivoFisicoId",
                table: "UtenteGrupo7",
                column: "ObjetivoFisicoId");

            migrationBuilder.CreateIndex(
                name: "IX_UtenteProblemaSaude_ProblemaSaudeId",
                table: "UtenteProblemaSaude",
                column: "ProblemaSaudeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alergia");

            migrationBuilder.DropTable(
                name: "AvaliacaoFisica");

            migrationBuilder.DropTable(
                name: "ExercicioEquipamento");

            migrationBuilder.DropTable(
                name: "ExercicioGenero");

            migrationBuilder.DropTable(
                name: "ExercicioGrupoMuscular");

            migrationBuilder.DropTable(
                name: "ExercicioProblemaSaude");

            migrationBuilder.DropTable(
                name: "Musculo");

            migrationBuilder.DropTable(
                name: "ObjetivoTipoExercicio");

            migrationBuilder.DropTable(
                name: "ProblemaSaudeProfissionalExecutante");

            migrationBuilder.DropTable(
                name: "Profissional");

            migrationBuilder.DropTable(
                name: "Receita");

            migrationBuilder.DropTable(
                name: "RestricaoAlimentar");

            migrationBuilder.DropTable(
                name: "Sono");

            migrationBuilder.DropTable(
                name: "TipoExercicioBeneficio");

            migrationBuilder.DropTable(
                name: "TipoExercicioProblemaSaude");

            migrationBuilder.DropTable(
                name: "UtenteProblemaSaude");

            migrationBuilder.DropTable(
                name: "Alimento");

            migrationBuilder.DropTable(
                name: "Equipamento");

            migrationBuilder.DropTable(
                name: "Genero");

            migrationBuilder.DropTable(
                name: "Exercicio");

            migrationBuilder.DropTable(
                name: "GrupoMuscular");

            migrationBuilder.DropTable(
                name: "ProfissionalExecutante");

            migrationBuilder.DropTable(
                name: "Beneficio");

            migrationBuilder.DropTable(
                name: "ProblemaSaude");

            migrationBuilder.DropTable(
                name: "UtenteGrupo7");

            migrationBuilder.DropTable(
                name: "CategoriaAlimento");

            migrationBuilder.DropTable(
                name: "TipoExercicio");

            migrationBuilder.DropTable(
                name: "ObjetivoFisico");
        }
    }
}
