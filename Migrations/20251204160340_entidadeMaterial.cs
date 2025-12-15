using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class entidadeMaterial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstadoComponente",
                table: "MaterialEquipamentoAssociado");

            migrationBuilder.AddColumn<int>(
                name: "EstadoMaterialMaterialStatusId",
                table: "MaterialEquipamentoAssociado",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaterialStatusId",
                table: "MaterialEquipamentoAssociado",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MaterialEquipamentoAssociado_EstadoMaterialMaterialStatusId",
                table: "MaterialEquipamentoAssociado",
                column: "EstadoMaterialMaterialStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_MaterialEquipamentoAssociado_EstadosMaterial_EstadoMaterialMaterialStatusId",
                table: "MaterialEquipamentoAssociado",
                column: "EstadoMaterialMaterialStatusId",
                principalTable: "EstadosMaterial",
                principalColumn: "MaterialStatusId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MaterialEquipamentoAssociado_EstadosMaterial_EstadoMaterialMaterialStatusId",
                table: "MaterialEquipamentoAssociado");

            migrationBuilder.DropIndex(
                name: "IX_MaterialEquipamentoAssociado_EstadoMaterialMaterialStatusId",
                table: "MaterialEquipamentoAssociado");

            migrationBuilder.DropColumn(
                name: "EstadoMaterialMaterialStatusId",
                table: "MaterialEquipamentoAssociado");

            migrationBuilder.DropColumn(
                name: "MaterialStatusId",
                table: "MaterialEquipamentoAssociado");

            migrationBuilder.AddColumn<string>(
                name: "EstadoComponente",
                table: "MaterialEquipamentoAssociado",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
