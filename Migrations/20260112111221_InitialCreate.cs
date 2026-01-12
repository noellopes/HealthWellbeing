using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Especialidades",
                columns: table => new
                {
                    EspecialidadeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Especialidades", x => x.EspecialidadeId);
                });

            migrationBuilder.CreateTable(
                name: "EstadosMaterial",
                columns: table => new
                {
                    MaterialStatusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosMaterial", x => x.MaterialStatusId);
                });

            migrationBuilder.CreateTable(
                name: "Funcoes",
                columns: table => new
                {
                    FuncaoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeFuncao = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Funcoes", x => x.FuncaoId);
                });

            migrationBuilder.CreateTable(
                name: "MaterialEquipamentoAssociado",
                columns: table => new
                {
                    MaterialEquipamentoAssociadoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeEquipamento = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Tamanho = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialEquipamentoAssociado", x => x.MaterialEquipamentoAssociadoId);
                });

            migrationBuilder.CreateTable(
                name: "Medicos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Especialidade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SalaDeExame",
                columns: table => new
                {
                    SalaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoSala = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Laboratorio = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalaDeExame", x => x.SalaId);
                });

            migrationBuilder.CreateTable(
                name: "Utentes",
                columns: table => new
                {
                    UtenteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UtenteSNS = table.Column<int>(type: "int", nullable: false),
                    Nif = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    NumCC = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Genero = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Morada = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CodigoPostal = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Numero = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NomeEmergencia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NumeroEmergencia = table.Column<int>(type: "int", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utentes", x => x.UtenteId);
                });

            migrationBuilder.CreateTable(
                name: "ExameTipo",
                columns: table => new
                {
                    ExameTipoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    EspecialidadeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExameTipo", x => x.ExameTipoId);
                    table.ForeignKey(
                        name: "FK_ExameTipo_Especialidades_EspecialidadeId",
                        column: x => x.EspecialidadeId,
                        principalTable: "Especialidades",
                        principalColumn: "EspecialidadeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfissionalExecutante",
                columns: table => new
                {
                    ProfissionalExecutanteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FuncaoId = table.Column<int>(type: "int", nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    FuncaoId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfissionalExecutante", x => x.ProfissionalExecutanteId);
                    table.ForeignKey(
                        name: "FK_ProfissionalExecutante_Funcoes_FuncaoId",
                        column: x => x.FuncaoId,
                        principalTable: "Funcoes",
                        principalColumn: "FuncaoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfissionalExecutante_Funcoes_FuncaoId1",
                        column: x => x.FuncaoId1,
                        principalTable: "Funcoes",
                        principalColumn: "FuncaoId");
                });

            migrationBuilder.CreateTable(
                name: "ExameTipoRecursos",
                columns: table => new
                {
                    ExameTipoId = table.Column<int>(type: "int", nullable: false),
                    MaterialEquipamentoAssociadoId = table.Column<int>(type: "int", nullable: false),
                    QuantidadeNecessaria = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExameTipoRecursos", x => new { x.ExameTipoId, x.MaterialEquipamentoAssociadoId });
                    table.ForeignKey(
                        name: "FK_ExameTipoRecursos_ExameTipo_ExameTipoId",
                        column: x => x.ExameTipoId,
                        principalTable: "ExameTipo",
                        principalColumn: "ExameTipoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExameTipoRecursos_MaterialEquipamentoAssociado_MaterialEquipamentoAssociadoId",
                        column: x => x.MaterialEquipamentoAssociadoId,
                        principalTable: "MaterialEquipamentoAssociado",
                        principalColumn: "MaterialEquipamentoAssociadoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Exames",
                columns: table => new
                {
                    ExameId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataHoraMarcacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Notas = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UtenteId = table.Column<int>(type: "int", nullable: false),
                    ExameTipoId = table.Column<int>(type: "int", nullable: false),
                    MedicoSolicitanteId = table.Column<int>(type: "int", nullable: false),
                    ProfissionalExecutanteId = table.Column<int>(type: "int", nullable: false),
                    SalaDeExameId = table.Column<int>(type: "int", nullable: false),
                    MaterialEquipamentoAssociadoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exames", x => x.ExameId);
                    table.ForeignKey(
                        name: "FK_Exames_ExameTipo_ExameTipoId",
                        column: x => x.ExameTipoId,
                        principalTable: "ExameTipo",
                        principalColumn: "ExameTipoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Exames_Medicos_MedicoSolicitanteId",
                        column: x => x.MedicoSolicitanteId,
                        principalTable: "Medicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Exames_ProfissionalExecutante_ProfissionalExecutanteId",
                        column: x => x.ProfissionalExecutanteId,
                        principalTable: "ProfissionalExecutante",
                        principalColumn: "ProfissionalExecutanteId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Exames_SalaDeExame_SalaDeExameId",
                        column: x => x.SalaDeExameId,
                        principalTable: "SalaDeExame",
                        principalColumn: "SalaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Exames_Utentes_UtenteId",
                        column: x => x.UtenteId,
                        principalTable: "Utentes",
                        principalColumn: "UtenteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegistoMateriais",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tamanho = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    MaterialStatusId = table.Column<int>(type: "int", nullable: false),
                    ExameId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistoMateriais", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistoMateriais_EstadosMaterial_MaterialStatusId",
                        column: x => x.MaterialStatusId,
                        principalTable: "EstadosMaterial",
                        principalColumn: "MaterialStatusId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RegistoMateriais_Exames_ExameId",
                        column: x => x.ExameId,
                        principalTable: "Exames",
                        principalColumn: "ExameId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Especialidades",
                columns: new[] { "EspecialidadeId", "Nome" },
                values: new object[,]
                {
                    { 1, "Hematologia" },
                    { 2, "Radiologia" },
                    { 3, "Cardiologia" },
                    { 4, "Imagiologia" },
                    { 5, "Reumatologia" },
                    { 6, "Urologia" },
                    { 7, "Gastroenterologia" },
                    { 8, "Ginecologia" },
                    { 9, "Pneumologia" },
                    { 10, "Endocrinologia" },
                    { 11, "Imunoalergologia" },
                    { 12, "Neurologia" },
                    { 13, "Oftalmologia" },
                    { 14, "Otorrinolaringologia" },
                    { 15, "Dermatologia" },
                    { 16, "Medicina Legal" }
                });

            migrationBuilder.InsertData(
                table: "EstadosMaterial",
                columns: new[] { "MaterialStatusId", "Descricao", "Nome" },
                values: new object[,]
                {
                    { 1, "Material pronto para utilização", "Disponível" },
                    { 2, "Material atualmente utilizado", "Em Uso" },
                    { 3, "Material reservado para utilização futura", "Reservado" },
                    { 4, "Material em reparação/manutenção", "Em Manutenção" },
                    { 5, "Material danificado e não utilizável", "Danificado" },
                    { 6, "Material não localizado", "Perdido" }
                });

            migrationBuilder.InsertData(
                table: "Funcoes",
                columns: new[] { "FuncaoId", "NomeFuncao" },
                values: new object[,]
                {
                    { 1, "Técnico de Radiologia" },
                    { 2, "Fisioterapeuta" },
                    { 3, "Técnico de Cardiopneumologia" },
                    { 4, "Técnico de Análises Clínicas" },
                    { 5, "Terapeuta Ocupacional" },
                    { 6, "Ortopedista" },
                    { 7, "Enfermeiro Especialista" },
                    { 8, "Nutricionista" },
                    { 9, "Técnico de Medicina Nuclear" },
                    { 10, "Cardiologista" },
                    { 11, "Podologista" },
                    { 12, "Técnico de Neurofisiologia" },
                    { 13, "Técnico Auxiliar de Saúde" },
                    { 14, "Optometrista" },
                    { 15, "Técnico de Medicina Física e Reabilitação" }
                });

            migrationBuilder.InsertData(
                table: "MaterialEquipamentoAssociado",
                columns: new[] { "MaterialEquipamentoAssociadoId", "NomeEquipamento", "Quantidade", "Tamanho" },
                values: new object[,]
                {
                    { 1, "Luvas de Látex", 2, "S" },
                    { 2, "Luvas de Látex", 3, "M" },
                    { 3, "Luvas de Látex", 1, "L" },
                    { 4, "Máscara Cirúrgica Descartável", 2, "Único" },
                    { 5, "Bata Cirúrgica Estéril", 1, "L" },
                    { 6, "Seringa", 2, "5ml" },
                    { 7, "Seringa", 1, "10ml" },
                    { 8, "Agulha Hipodérmica", 2, "21G" },
                    { 9, "Agulha Hipodérmica", 2, "23G" },
                    { 10, "Algodão Hidrófilo (Pacote)", 1, "500g" },
                    { 11, "Compressas de Gaze Estéril", 2, "10x10cm" },
                    { 12, "Ligadura Elástica", 1, "5cm" },
                    { 13, "Ligadura Elástica", 2, "10cm" },
                    { 14, "Adesivo Micropore", 2, "2.5cm" },
                    { 15, "Solução Betadine (Frasco)", 1, "100ml" },
                    { 16, "Espátula de Madeira (Abaixa-língua)", 2, "Único" },
                    { 17, "Copo de Recolha de Urina", 3, "100ml" },
                    { 18, "Gel para Ecografia (Frasco)", 2, "250ml" },
                    { 19, "Papel para marquesa (Rolo)", 2, "Standard" },
                    { 20, "Cateter Intravenoso", 4, "20G" }
                });

            migrationBuilder.InsertData(
                table: "ExameTipo",
                columns: new[] { "ExameTipoId", "Descricao", "EspecialidadeId", "Nome" },
                values: new object[,]
                {
                    { 1, "Exame laboratorial de rotina para avaliação hematológica.", 1, "Análise de Sangue Completa" },
                    { 2, "Exame de imagem detalhado para estruturas internas.", 2, "Ressonância Magnética" },
                    { 3, "Avaliação da atividade elétrica do coração.", 3, "Eletrocardiograma (ECG)" },
                    { 4, "Processamento de imagens por computador para criar visões transversais do corpo.", 2, "Tomografia Computorizada (TAC)" },
                    { 5, "Utiliza ondas sonoras de alta frequência para criar imagens dos órgãos internos.", 4, "Ecografia Abdominal" },
                    { 6, "Monitorização cardíaca durante exercício físico controlado.", 3, "Teste de Esforço Cardíaco" },
                    { 7, "Mede a densidade mineral óssea para diagnosticar osteoporose.", 5, "Densitometria Óssea" },
                    { 8, "Análise laboratorial de amostra de urina.", 6, "Exame de Urina Tipo II" },
                    { 9, "Exame endoscópico para visualização do intestino grosso.", 7, "Colonoscopia" },
                    { 10, "Exame do esófago, estômago e duodeno.", 7, "Endoscopia Digestiva Alta" },
                    { 11, "Rastreio e diagnóstico de cancro da mama.", 2, "Mamografia Digital" },
                    { 12, "Avaliação dos órgãos pélvicos femininos ou masculinos.", 8, "Ecografia Pélvica" },
                    { 13, "Avalia a capacidade pulmonar e o fluxo de ar.", 9, "Prova de Função Respiratória" },
                    { 14, "Monitorização contínua da atividade elétrica do coração.", 3, "Holter 24 Horas" },
                    { 15, "Medição dos níveis de hormonas tiroideias no sangue.", 10, "Análise Hormonal (Tireoide)" },
                    { 16, "Testes cutâneos para identificação de alergénios específicos.", 11, "Teste de Alergias" },
                    { 17, "Registo da atividade elétrica cerebral.", 12, "Eletroencefalograma (EEG)" },
                    { 18, "Visualização detalhada dos vasos sanguíneos.", 2, "Angiografia por TC" },
                    { 19, "Avaliação da acuidade visual e pressão intraocular.", 13, "Exame Oftalmológico Completo" },
                    { 20, "Avaliação da capacidade auditiva.", 14, "Audiograma" },
                    { 21, "Colheita de pequena amostra de tecido cutâneo.", 15, "Biopsia de Pele" },
                    { 22, "Detecção e quantificação de substâncias químicas no organismo.", 16, "Análise Toxicológica" },
                    { 23, "Identificação de bactérias que podem causar infeção urinária.", 6, "Cultura de Urina" }
                });

            migrationBuilder.InsertData(
                table: "ProfissionalExecutante",
                columns: new[] { "ProfissionalExecutanteId", "Email", "FuncaoId", "FuncaoId1", "Nome", "Telefone" },
                values: new object[,]
                {
                    { 1, "Kandonga123@gmail.com", 1, null, "André Kandonga", "912912915" },
                    { 2, "MiguelSantos123@gmail.com", 2, null, "Miguel Santos", "912912914" },
                    { 3, "DostoevskySuba@gmail.com", 3, null, "Dostoevsky", "912913914" },
                    { 4, "QuaresmaPorto@gmail.com", 4, null, "Ricardo Quaresma", "910101010" },
                    { 5, "Mai123222suba@gmail.com", 5, null, "Mai Da Silva", "912912222" },
                    { 6, "DiogoRodrigues04@gmail.com", 6, null, "Diogo Rodrigues", "912912522" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Exames_ExameTipoId",
                table: "Exames",
                column: "ExameTipoId");

            migrationBuilder.CreateIndex(
                name: "IX_Exames_MedicoSolicitanteId",
                table: "Exames",
                column: "MedicoSolicitanteId");

            migrationBuilder.CreateIndex(
                name: "IX_Exames_ProfissionalExecutanteId",
                table: "Exames",
                column: "ProfissionalExecutanteId");

            migrationBuilder.CreateIndex(
                name: "IX_Exames_SalaDeExameId",
                table: "Exames",
                column: "SalaDeExameId");

            migrationBuilder.CreateIndex(
                name: "IX_Exames_UtenteId",
                table: "Exames",
                column: "UtenteId");

            migrationBuilder.CreateIndex(
                name: "IX_ExameTipo_EspecialidadeId",
                table: "ExameTipo",
                column: "EspecialidadeId");

            migrationBuilder.CreateIndex(
                name: "IX_ExameTipoRecursos_MaterialEquipamentoAssociadoId",
                table: "ExameTipoRecursos",
                column: "MaterialEquipamentoAssociadoId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfissionalExecutante_FuncaoId",
                table: "ProfissionalExecutante",
                column: "FuncaoId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfissionalExecutante_FuncaoId1",
                table: "ProfissionalExecutante",
                column: "FuncaoId1");

            migrationBuilder.CreateIndex(
                name: "IX_RegistoMateriais_ExameId",
                table: "RegistoMateriais",
                column: "ExameId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistoMateriais_MaterialStatusId",
                table: "RegistoMateriais",
                column: "MaterialStatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExameTipoRecursos");

            migrationBuilder.DropTable(
                name: "RegistoMateriais");

            migrationBuilder.DropTable(
                name: "MaterialEquipamentoAssociado");

            migrationBuilder.DropTable(
                name: "EstadosMaterial");

            migrationBuilder.DropTable(
                name: "Exames");

            migrationBuilder.DropTable(
                name: "ExameTipo");

            migrationBuilder.DropTable(
                name: "Medicos");

            migrationBuilder.DropTable(
                name: "ProfissionalExecutante");

            migrationBuilder.DropTable(
                name: "SalaDeExame");

            migrationBuilder.DropTable(
                name: "Utentes");

            migrationBuilder.DropTable(
                name: "Especialidades");

            migrationBuilder.DropTable(
                name: "Funcoes");
        }
    }
}
