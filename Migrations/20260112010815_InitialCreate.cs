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
            migrationBuilder.DropForeignKey(
                name: "FK_RegistoMateriais_EstadosMaterial_EstadoMaterialStatusId",
                table: "RegistoMateriais");

            migrationBuilder.DropIndex(
                name: "IX_RegistoMateriais_EstadoMaterialStatusId",
                table: "RegistoMateriais");

            migrationBuilder.DropColumn(
                name: "EstadoMaterialStatusId",
                table: "RegistoMateriais");

            migrationBuilder.AddColumn<int>(
                name: "MaterialStatusId",
                table: "RegistoMateriais",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RegistoMateriais_MaterialStatusId",
                table: "RegistoMateriais",
                column: "MaterialStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_RegistoMateriais_EstadosMaterial_MaterialStatusId",
                table: "RegistoMateriais",
                column: "MaterialStatusId",
                principalTable: "EstadosMaterial",
                principalColumn: "MaterialStatusId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RegistoMateriais_EstadosMaterial_MaterialStatusId",
                table: "RegistoMateriais");

            migrationBuilder.DropIndex(
                name: "IX_RegistoMateriais_MaterialStatusId",
                table: "RegistoMateriais");

            migrationBuilder.DropColumn(
                name: "MaterialStatusId",
                table: "RegistoMateriais");

            migrationBuilder.AddColumn<int>(
                name: "EstadoMaterialStatusId",
                table: "RegistoMateriais",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegistoMateriais_EstadoMaterialStatusId",
                table: "RegistoMateriais",
                column: "EstadoMaterialStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_RegistoMateriais_EstadosMaterial_EstadoMaterialStatusId",
                table: "RegistoMateriais",
                column: "EstadoMaterialStatusId",
                principalTable: "EstadosMaterial",
                principalColumn: "MaterialStatusId");
        }
    }
}
