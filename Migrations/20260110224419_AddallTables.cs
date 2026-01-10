using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class AddallTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Activity",
                columns: table => new
                {
                    Activity_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Activity_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Activity_Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activity_Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumberSets = table.Column<int>(type: "int", nullable: true),
                    NumberReps = table.Column<int>(type: "int", nullable: true),
                    Weigth = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activity", x => x.Activity_Id);
                });

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
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

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
                name: "Doctor",
                columns: table => new
                {
                    IdMedico = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Telemovel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doctor", x => x.IdMedico);
                });

            migrationBuilder.CreateTable(
                name: "EventType",
                columns: table => new
                {
                    EventTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventTypeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EventTypeScoringMode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EventTypeMultiplier = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventType", x => x.EventTypeId);
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
                name: "Level",
                columns: table => new
                {
                    LevelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LevelAtual = table.Column<int>(type: "int", nullable: false),
                    LevelCategory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Level", x => x.LevelId);
                });

            migrationBuilder.CreateTable(
                name: "Plan",
                columns: table => new
                {
                    PlanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DurationDays = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plan", x => x.PlanId);
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
                name: "Specialities",
                columns: table => new
                {
                    IdEspecialidade = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(1500)", maxLength: 1500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specialities", x => x.IdEspecialidade);
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
                name: "Trainer",
                columns: table => new
                {
                    TrainerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Speciality = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trainer", x => x.TrainerId);
                });

            migrationBuilder.CreateTable(
                name: "TrainingType",
                columns: table => new
                {
                    TrainingTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingType", x => x.TrainingTypeId);
                });

            migrationBuilder.CreateTable(
                name: "UtenteSaude",
                columns: table => new
                {
                    UtenteSaudeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeCompleto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DataNascimento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Nif = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    Niss = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    Nus = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: true),
                    Telefone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Morada = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UtenteSaude", x => x.UtenteSaudeId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Client",
                columns: table => new
                {
                    ClientId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdentityUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    PlanoAlimentarId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Client", x => x.ClientId);
                    table.ForeignKey(
                        name: "FK_Client_AspNetUsers_IdentityUserId",
                        column: x => x.IdentityUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "Event",
                columns: table => new
                {
                    EventId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EventDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    EventTypeId = table.Column<int>(type: "int", nullable: false),
                    EventStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EventEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EventPoints = table.Column<int>(type: "int", nullable: false),
                    MinLevel = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.EventId);
                    table.ForeignKey(
                        name: "FK_Event_EventType_EventTypeId",
                        column: x => x.EventTypeId,
                        principalTable: "EventType",
                        principalColumn: "EventTypeId");
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
                name: "ProfissionalExecutante",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Funcao = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProblemaSaudeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfissionalExecutante", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfissionalExecutante_ProblemaSaude_ProblemaSaudeId",
                        column: x => x.ProblemaSaudeId,
                        principalTable: "ProblemaSaude",
                        principalColumn: "ProblemaSaudeId");
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
                name: "Consulta",
                columns: table => new
                {
                    IdConsulta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataMarcacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataConsulta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataCancelamento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HoraInicio = table.Column<TimeOnly>(type: "time", nullable: false),
                    HoraFim = table.Column<TimeOnly>(type: "time", nullable: false),
                    SearchTerm = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdMedico = table.Column<int>(type: "int", nullable: false),
                    IdEspecialidade = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consulta", x => x.IdConsulta);
                    table.ForeignKey(
                        name: "FK_Consulta_Doctor_IdMedico",
                        column: x => x.IdMedico,
                        principalTable: "Doctor",
                        principalColumn: "IdMedico",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Consulta_Specialities_IdEspecialidade",
                        column: x => x.IdEspecialidade,
                        principalTable: "Specialities",
                        principalColumn: "IdEspecialidade",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BeneficioTipoExercicio",
                columns: table => new
                {
                    BeneficiosBeneficioId = table.Column<int>(type: "int", nullable: false),
                    TipoExercicioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeneficioTipoExercicio", x => new { x.BeneficiosBeneficioId, x.TipoExercicioId });
                    table.ForeignKey(
                        name: "FK_BeneficioTipoExercicio_Beneficio_BeneficiosBeneficioId",
                        column: x => x.BeneficiosBeneficioId,
                        principalTable: "Beneficio",
                        principalColumn: "BeneficioId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BeneficioTipoExercicio_TipoExercicio_TipoExercicioId",
                        column: x => x.TipoExercicioId,
                        principalTable: "TipoExercicio",
                        principalColumn: "TipoExercicioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Training",
                columns: table => new
                {
                    TrainingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrainerId = table.Column<int>(type: "int", nullable: false),
                    TrainingTypeId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    DayOfWeek = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    MaxParticipants = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training", x => x.TrainingId);
                    table.ForeignKey(
                        name: "FK_Training_Trainer_TrainerId",
                        column: x => x.TrainerId,
                        principalTable: "Trainer",
                        principalColumn: "TrainerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Training_TrainingType_TrainingTypeId",
                        column: x => x.TrainingTypeId,
                        principalTable: "TrainingType",
                        principalColumn: "TrainingTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientAlergia",
                columns: table => new
                {
                    ClientId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AlergiaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientAlergia", x => new { x.ClientId, x.AlergiaId });
                    table.ForeignKey(
                        name: "FK_ClientAlergia_Alergia_AlergiaId",
                        column: x => x.AlergiaId,
                        principalTable: "Alergia",
                        principalColumn: "AlergiaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientAlergia_Client_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientRestricao",
                columns: table => new
                {
                    ClientId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RestricaoAlimentarId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientRestricao", x => new { x.ClientId, x.RestricaoAlimentarId });
                    table.ForeignKey(
                        name: "FK_ClientRestricao_Client_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientRestricao_RestricaoAlimentar_RestricaoAlimentarId",
                        column: x => x.RestricaoAlimentarId,
                        principalTable: "RestricaoAlimentar",
                        principalColumn: "RestricaoAlimentarId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Member",
                columns: table => new
                {
                    MemberId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Member", x => x.MemberId);
                    table.ForeignKey(
                        name: "FK_Member_Client_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Meta",
                columns: table => new
                {
                    MetaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MetaDescription = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DailyCalories = table.Column<int>(type: "int", nullable: false),
                    DailyProtein = table.Column<int>(type: "int", nullable: false),
                    DailyFat = table.Column<int>(type: "int", nullable: false),
                    DailyHydrates = table.Column<int>(type: "int", nullable: false),
                    DailyVitamins = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meta", x => x.MetaId);
                    table.ForeignKey(
                        name: "FK_Meta_Client_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MetaCorporal",
                columns: table => new
                {
                    MetaCorporalId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PesoObjetivo = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    GorduraCorporalObjetivo = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    ColesterolObjetivo = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    IMCObjetivo = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    MassaMuscularObjetivo = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    DataInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataObjetivo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notas = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CriadoPor = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    AtualizadoEm = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AtualizadoPor = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetaCorporal", x => x.MetaCorporalId);
                    table.ForeignKey(
                        name: "FK_MetaCorporal_Client_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProgressRecord",
                columns: table => new
                {
                    ProgressRecordId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NutritionistId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    BodyFatPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    Cholesterol = table.Column<decimal>(type: "decimal(6,2)", nullable: true),
                    BMI = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    MuscleMass = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    RecordDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgressRecord", x => x.ProgressRecordId);
                    table.ForeignKey(
                        name: "FK_ProgressRecord_AspNetUsers_NutritionistId",
                        column: x => x.NutritionistId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProgressRecord_Client_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "ClientId",
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
                    ReceitaId = table.Column<int>(type: "int", nullable: false),
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
                    table.ForeignKey(
                        name: "FK_ComponenteReceita_Receita_ReceitaId",
                        column: x => x.ReceitaId,
                        principalTable: "Receita",
                        principalColumn: "ReceitaId",
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
                name: "PlanoAlimentar",
                columns: table => new
                {
                    PlanoAlimentarId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MetaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanoAlimentar", x => x.PlanoAlimentarId);
                    table.ForeignKey(
                        name: "FK_PlanoAlimentar_Client_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "ClientId");
                    table.ForeignKey(
                        name: "FK_PlanoAlimentar_Meta_MetaId",
                        column: x => x.MetaId,
                        principalTable: "Meta",
                        principalColumn: "MetaId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReceitasParaPlanosAlimentares",
                columns: table => new
                {
                    PlanoAlimentarId = table.Column<int>(type: "int", nullable: false),
                    ReceitaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceitasParaPlanosAlimentares", x => new { x.PlanoAlimentarId, x.ReceitaId });
                    table.ForeignKey(
                        name: "FK_ReceitasParaPlanosAlimentares_PlanoAlimentar_PlanoAlimentarId",
                        column: x => x.PlanoAlimentarId,
                        principalTable: "PlanoAlimentar",
                        principalColumn: "PlanoAlimentarId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReceitasParaPlanosAlimentares_Receita_ReceitaId",
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
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BeneficioTipoExercicio_TipoExercicioId",
                table: "BeneficioTipoExercicio",
                column: "TipoExercicioId");

            migrationBuilder.CreateIndex(
                name: "IX_Client_IdentityUserId",
                table: "Client",
                column: "IdentityUserId",
                unique: true,
                filter: "[IdentityUserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ClientAlergia_AlergiaId",
                table: "ClientAlergia",
                column: "AlergiaId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientRestricao_RestricaoAlimentarId",
                table: "ClientRestricao",
                column: "RestricaoAlimentarId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponenteReceita_AlimentoId",
                table: "ComponenteReceita",
                column: "AlimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_ComponenteReceita_ReceitaId",
                table: "ComponenteReceita",
                column: "ReceitaId");

            migrationBuilder.CreateIndex(
                name: "IX_Consulta_IdEspecialidade",
                table: "Consulta",
                column: "IdEspecialidade");

            migrationBuilder.CreateIndex(
                name: "IX_Consulta_IdMedico",
                table: "Consulta",
                column: "IdMedico");

            migrationBuilder.CreateIndex(
                name: "IX_Event_EventTypeId",
                table: "Event",
                column: "EventTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ExercicioGenero_GeneroId",
                table: "ExercicioGenero",
                column: "GeneroId");

            migrationBuilder.CreateIndex(
                name: "IX_ExercicioGrupoMuscular_GrupoMuscularId",
                table: "ExercicioGrupoMuscular",
                column: "GrupoMuscularId");

            migrationBuilder.CreateIndex(
                name: "IX_Member_ClientId",
                table: "Member",
                column: "ClientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Meta_ClientId",
                table: "Meta",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_MetaCorporal_ClientId",
                table: "MetaCorporal",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Musculo_GrupoMuscularId",
                table: "Musculo",
                column: "GrupoMuscularId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanoAlimentar_ClientId",
                table: "PlanoAlimentar",
                column: "ClientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlanoAlimentar_MetaId",
                table: "PlanoAlimentar",
                column: "MetaId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfissionalExecutante_ProblemaSaudeId",
                table: "ProfissionalExecutante",
                column: "ProblemaSaudeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgressRecord_ClientId",
                table: "ProgressRecord",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgressRecord_NutritionistId",
                table: "ProgressRecord",
                column: "NutritionistId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceitaRestricaoAlimentar_RestricoesAlimentaresRestricaoAlimentarId",
                table: "ReceitaRestricaoAlimentar",
                column: "RestricoesAlimentaresRestricaoAlimentarId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceitasParaPlanosAlimentares_ReceitaId",
                table: "ReceitasParaPlanosAlimentares",
                column: "ReceitaId");

            migrationBuilder.CreateIndex(
                name: "IX_RestricaoAlimentarAlimento_AlimentoId",
                table: "RestricaoAlimentarAlimento",
                column: "AlimentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Training_TrainerId",
                table: "Training",
                column: "TrainerId");

            migrationBuilder.CreateIndex(
                name: "IX_Training_TrainingTypeId",
                table: "Training",
                column: "TrainingTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UtenteSaude_Nif",
                table: "UtenteSaude",
                column: "Nif",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UtenteSaude_Niss",
                table: "UtenteSaude",
                column: "Niss",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UtenteSaude_Nus",
                table: "UtenteSaude",
                column: "Nus",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Activity");

            migrationBuilder.DropTable(
                name: "AlergiaAlimento");

            migrationBuilder.DropTable(
                name: "AlimentoSubstitutos");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BeneficioTipoExercicio");

            migrationBuilder.DropTable(
                name: "ClientAlergia");

            migrationBuilder.DropTable(
                name: "ClientRestricao");

            migrationBuilder.DropTable(
                name: "ComponenteReceita");

            migrationBuilder.DropTable(
                name: "Consulta");

            migrationBuilder.DropTable(
                name: "Event");

            migrationBuilder.DropTable(
                name: "ExercicioGenero");

            migrationBuilder.DropTable(
                name: "ExercicioGrupoMuscular");

            migrationBuilder.DropTable(
                name: "Level");

            migrationBuilder.DropTable(
                name: "Member");

            migrationBuilder.DropTable(
                name: "MetaCorporal");

            migrationBuilder.DropTable(
                name: "Musculo");

            migrationBuilder.DropTable(
                name: "Plan");

            migrationBuilder.DropTable(
                name: "ProfissionalExecutante");

            migrationBuilder.DropTable(
                name: "ProgressRecord");

            migrationBuilder.DropTable(
                name: "ReceitaRestricaoAlimentar");

            migrationBuilder.DropTable(
                name: "ReceitasParaPlanosAlimentares");

            migrationBuilder.DropTable(
                name: "RestricaoAlimentarAlimento");

            migrationBuilder.DropTable(
                name: "Training");

            migrationBuilder.DropTable(
                name: "UtenteSaude");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Beneficio");

            migrationBuilder.DropTable(
                name: "TipoExercicio");

            migrationBuilder.DropTable(
                name: "Alergia");

            migrationBuilder.DropTable(
                name: "Doctor");

            migrationBuilder.DropTable(
                name: "Specialities");

            migrationBuilder.DropTable(
                name: "EventType");

            migrationBuilder.DropTable(
                name: "Genero");

            migrationBuilder.DropTable(
                name: "Exercicio");

            migrationBuilder.DropTable(
                name: "GrupoMuscular");

            migrationBuilder.DropTable(
                name: "ProblemaSaude");

            migrationBuilder.DropTable(
                name: "PlanoAlimentar");

            migrationBuilder.DropTable(
                name: "Receita");

            migrationBuilder.DropTable(
                name: "Alimentos");

            migrationBuilder.DropTable(
                name: "RestricaoAlimentar");

            migrationBuilder.DropTable(
                name: "Trainer");

            migrationBuilder.DropTable(
                name: "TrainingType");

            migrationBuilder.DropTable(
                name: "Meta");

            migrationBuilder.DropTable(
                name: "CategoriaAlimento");

            migrationBuilder.DropTable(
                name: "Client");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
