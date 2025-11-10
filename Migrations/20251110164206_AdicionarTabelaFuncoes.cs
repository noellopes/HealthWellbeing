using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarTabelaFuncoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. CRIAÇÃO DA TABELA FUNCOES (Isto correu bem)
            migrationBuilder.CreateTable(
                name: "Funcoes",
                columns: table => new
                {
                    FuncaoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Funcoes", x => x.FuncaoId);
                });

            // 2. ADICIONAR A COLUNA FuncaoId À TABELA PROFISSIONALEXECUTANTE (NOVO PASSO!)
            migrationBuilder.AddColumn<int>(
                name: "FuncaoId", // Nome da nova coluna
                table: "ProfissionalExecutante", // Tabela onde adicionar
                type: "int", // Tipo de dados
                nullable: false, // Deve ser NOT NULL (pode causar problemas se já houver dados)
                defaultValue: 1); // <--- DEVE DEFINIR UM VALOR DEFAULT TEMPORÁRIO!

            // NOTA: Se já tiver dados na tabela ProfissionalExecutante, 
            // defina o defaultValue para um ID de Funcao válido (ex: 1) para evitar erros NOT NULL.

            // 3. ADICIONAR ÍNDICE (Index) (Não é estritamente necessário para a FK, mas é boa prática)
            migrationBuilder.CreateIndex(
                name: "IX_ProfissionalExecutante_FuncaoId",
                table: "ProfissionalExecutante",
                column: "FuncaoId");

            // 4. ADICIONAR CHAVE ESTRANGEIRA (Foreign Key)
            migrationBuilder.AddForeignKey(
                name: "FK_ProfissionalExecutante_Funcoes_FuncaoId",
                table: "ProfissionalExecutante",
                column: "FuncaoId",
                principalTable: "Funcoes",
                principalColumn: "FuncaoId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 1. Remover a Chave Estrangeira
            migrationBuilder.DropForeignKey(
                name: "FK_ProfissionalExecutante_Funcoes_FuncaoId",
                table: "ProfissionalExecutante");

            // 2. Remover o Índice
            migrationBuilder.DropIndex(
                name: "IX_ProfissionalExecutante_FuncaoId",
                table: "ProfissionalExecutante");

            // 3. REMOVER A COLUNA (Novo Passo!)
            migrationBuilder.DropColumn(
                name: "FuncaoId",
                table: "ProfissionalExecutante");

            // 4. Remover a Tabela Funcoes
            migrationBuilder.DropTable(
                name: "Funcoes");
        }
    }
}