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
                name: "Clientes",
                columns: table => new
                {
                    ClientId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.ClientId);
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
                name: "Generos",
                columns: table => new
                {
                    GeneroId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeGenero = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Generos", x => x.GeneroId);
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
                name: "NiveisCliente",
                columns: table => new
                {
                    NivelClienteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PontosMinimos = table.Column<int>(type: "int", nullable: false),
                    CorBadge = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NiveisCliente", x => x.NivelClienteId);
                });

            migrationBuilder.CreateTable(
                name: "SegurosSaude",
                columns: table => new
                {
                    SeguroSaudeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SegurosSaude", x => x.SeguroSaudeId);
                });

            migrationBuilder.CreateTable(
                name: "Terapeutas",
                columns: table => new
                {
                    TerapeutaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Especialidade = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AnoEntrada = table.Column<int>(type: "int", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Terapeutas", x => x.TerapeutaId);
                });

            migrationBuilder.CreateTable(
                name: "TipoServico",
                columns: table => new
                {
                    TipoServicoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoServico", x => x.TipoServicoId);
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
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
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
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
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
                        name: "FK_Member_Clientes_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clientes",
                        principalColumn: "ClientId",
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
                        name: "FK_ExercicioGenero_Generos_GeneroId",
                        column: x => x.GeneroId,
                        principalTable: "Generos",
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
                name: "ClientesBalneario",
                columns: table => new
                {
                    ClienteBalnearioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Telemovel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataRegisto = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    Pontos = table.Column<int>(type: "int", nullable: false),
                    NivelClienteId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientesBalneario", x => x.ClienteBalnearioId);
                    table.ForeignKey(
                        name: "FK_ClientesBalneario_NiveisCliente_NivelClienteId",
                        column: x => x.NivelClienteId,
                        principalTable: "NiveisCliente",
                        principalColumn: "NivelClienteId");
                });

            migrationBuilder.CreateTable(
                name: "Servico",
                columns: table => new
                {
                    ServicoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Preco = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DuracaoMinutos = table.Column<int>(type: "int", nullable: false),
                    TipoServicoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servico", x => x.ServicoId);
                    table.ForeignKey(
                        name: "FK_Servico_TipoServico_TipoServicoId",
                        column: x => x.TipoServicoId,
                        principalTable: "TipoServico",
                        principalColumn: "TipoServicoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HistoricoPontos",
                columns: table => new
                {
                    HistoricoPontosId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteBalnearioId = table.Column<int>(type: "int", nullable: false),
                    Pontos = table.Column<int>(type: "int", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricoPontos", x => x.HistoricoPontosId);
                    table.ForeignKey(
                        name: "FK_HistoricoPontos_ClientesBalneario_ClienteBalnearioId",
                        column: x => x.ClienteBalnearioId,
                        principalTable: "ClientesBalneario",
                        principalColumn: "ClienteBalnearioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SatisfacoesClientes",
                columns: table => new
                {
                    SatisfacaoClienteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteBalnearioId = table.Column<int>(type: "int", nullable: false),
                    Avaliacao = table.Column<int>(type: "int", nullable: false),
                    Comentario = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DataRegisto = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SatisfacoesClientes", x => x.SatisfacaoClienteId);
                    table.ForeignKey(
                        name: "FK_SatisfacoesClientes_ClientesBalneario_ClienteBalnearioId",
                        column: x => x.ClienteBalnearioId,
                        principalTable: "ClientesBalneario",
                        principalColumn: "ClienteBalnearioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UtenteBalnearios",
                columns: table => new
                {
                    UtenteBalnearioId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DataNascimento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GeneroId = table.Column<int>(type: "int", nullable: false),
                    NIF = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Contacto = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Morada = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    HistoricoClinico = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IndicacoesTerapeuticas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContraIndicacoes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TerapeutaResponsavel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataInscricao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SeguroSaudeId = table.Column<int>(type: "int", nullable: true),
                    NumeroApolice = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    ClienteBalnearioId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UtenteBalnearios", x => x.UtenteBalnearioId);
                    table.ForeignKey(
                        name: "FK_UtenteBalnearios_ClientesBalneario_ClienteBalnearioId",
                        column: x => x.ClienteBalnearioId,
                        principalTable: "ClientesBalneario",
                        principalColumn: "ClienteBalnearioId");
                    table.ForeignKey(
                        name: "FK_UtenteBalnearios_Generos_GeneroId",
                        column: x => x.GeneroId,
                        principalTable: "Generos",
                        principalColumn: "GeneroId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UtenteBalnearios_SegurosSaude_SeguroSaudeId",
                        column: x => x.SeguroSaudeId,
                        principalTable: "SegurosSaude",
                        principalColumn: "SeguroSaudeId");
                });

            migrationBuilder.CreateTable(
                name: "VouchersCliente",
                columns: table => new
                {
                    VoucherClienteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    PontosNecessarios = table.Column<int>(type: "int", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataValidade = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Usado = table.Column<bool>(type: "bit", nullable: false),
                    DataUtilizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClienteBalnearioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VouchersCliente", x => x.VoucherClienteId);
                    table.ForeignKey(
                        name: "FK_VouchersCliente_ClientesBalneario_ClienteBalnearioId",
                        column: x => x.ClienteBalnearioId,
                        principalTable: "ClientesBalneario",
                        principalColumn: "ClienteBalnearioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AgendamentoModel",
                columns: table => new
                {
                    AgendamentoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataHoraInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataHoraFim = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UtenteBalnearioId = table.Column<int>(type: "int", nullable: false),
                    TerapeutaId = table.Column<int>(type: "int", nullable: false),
                    ServicoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgendamentoModel", x => x.AgendamentoId);
                    table.ForeignKey(
                        name: "FK_AgendamentoModel_Servico_ServicoId",
                        column: x => x.ServicoId,
                        principalTable: "Servico",
                        principalColumn: "ServicoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AgendamentoModel_Terapeutas_TerapeutaId",
                        column: x => x.TerapeutaId,
                        principalTable: "Terapeutas",
                        principalColumn: "TerapeutaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AgendamentoModel_UtenteBalnearios_UtenteBalnearioId",
                        column: x => x.UtenteBalnearioId,
                        principalTable: "UtenteBalnearios",
                        principalColumn: "UtenteBalnearioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HistoricosMedicos",
                columns: table => new
                {
                    HistoricoMedicoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UtenteBalnearioId = table.Column<int>(type: "int", nullable: false),
                    DataRegisto = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CriadoPorUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricosMedicos", x => x.HistoricoMedicoId);
                    table.ForeignKey(
                        name: "FK_HistoricosMedicos_AspNetUsers_CriadoPorUserId",
                        column: x => x.CriadoPorUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HistoricosMedicos_UtenteBalnearios_UtenteBalnearioId",
                        column: x => x.UtenteBalnearioId,
                        principalTable: "UtenteBalnearios",
                        principalColumn: "UtenteBalnearioId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AgendamentoModel_ServicoId",
                table: "AgendamentoModel",
                column: "ServicoId");

            migrationBuilder.CreateIndex(
                name: "IX_AgendamentoModel_TerapeutaId",
                table: "AgendamentoModel",
                column: "TerapeutaId");

            migrationBuilder.CreateIndex(
                name: "IX_AgendamentoModel_UtenteBalnearioId",
                table: "AgendamentoModel",
                column: "UtenteBalnearioId");

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
                name: "IX_ClientesBalneario_NivelClienteId",
                table: "ClientesBalneario",
                column: "NivelClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_ExercicioGenero_GeneroId",
                table: "ExercicioGenero",
                column: "GeneroId");

            migrationBuilder.CreateIndex(
                name: "IX_ExercicioGrupoMuscular_GrupoMuscularId",
                table: "ExercicioGrupoMuscular",
                column: "GrupoMuscularId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricoPontos_ClienteBalnearioId",
                table: "HistoricoPontos",
                column: "ClienteBalnearioId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricosMedicos_CriadoPorUserId",
                table: "HistoricosMedicos",
                column: "CriadoPorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricosMedicos_UtenteBalnearioId",
                table: "HistoricosMedicos",
                column: "UtenteBalnearioId");

            migrationBuilder.CreateIndex(
                name: "IX_Member_ClientId",
                table: "Member",
                column: "ClientId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Musculo_GrupoMuscularId",
                table: "Musculo",
                column: "GrupoMuscularId");

            migrationBuilder.CreateIndex(
                name: "IX_SatisfacoesClientes_ClienteBalnearioId",
                table: "SatisfacoesClientes",
                column: "ClienteBalnearioId");

            migrationBuilder.CreateIndex(
                name: "IX_Servico_TipoServicoId",
                table: "Servico",
                column: "TipoServicoId");

            migrationBuilder.CreateIndex(
                name: "IX_UtenteBalnearios_ClienteBalnearioId",
                table: "UtenteBalnearios",
                column: "ClienteBalnearioId");

            migrationBuilder.CreateIndex(
                name: "IX_UtenteBalnearios_Contacto",
                table: "UtenteBalnearios",
                column: "Contacto",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UtenteBalnearios_GeneroId",
                table: "UtenteBalnearios",
                column: "GeneroId");

            migrationBuilder.CreateIndex(
                name: "IX_UtenteBalnearios_NIF",
                table: "UtenteBalnearios",
                column: "NIF",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UtenteBalnearios_SeguroSaudeId",
                table: "UtenteBalnearios",
                column: "SeguroSaudeId");

            migrationBuilder.CreateIndex(
                name: "IX_VouchersCliente_ClienteBalnearioId",
                table: "VouchersCliente",
                column: "ClienteBalnearioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AgendamentoModel");

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
                name: "ExercicioGenero");

            migrationBuilder.DropTable(
                name: "ExercicioGrupoMuscular");

            migrationBuilder.DropTable(
                name: "HistoricoPontos");

            migrationBuilder.DropTable(
                name: "HistoricosMedicos");

            migrationBuilder.DropTable(
                name: "Member");

            migrationBuilder.DropTable(
                name: "Musculo");

            migrationBuilder.DropTable(
                name: "SatisfacoesClientes");

            migrationBuilder.DropTable(
                name: "VouchersCliente");

            migrationBuilder.DropTable(
                name: "Servico");

            migrationBuilder.DropTable(
                name: "Terapeutas");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Exercicio");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "UtenteBalnearios");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "GrupoMuscular");

            migrationBuilder.DropTable(
                name: "TipoServico");

            migrationBuilder.DropTable(
                name: "ClientesBalneario");

            migrationBuilder.DropTable(
                name: "Generos");

            migrationBuilder.DropTable(
                name: "SegurosSaude");

            migrationBuilder.DropTable(
                name: "NiveisCliente");
        }
    }
}
