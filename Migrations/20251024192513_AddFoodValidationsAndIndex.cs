using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class AddFoodValidationsAndIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Food_FoodCategory_FoodCategoryId",
                table: "Food");

            migrationBuilder.DropIndex(
                name: "IX_Objective_Name_Category",
                table: "Objective");

            migrationBuilder.AlterColumn<decimal>(
                name: "ProteinPer100g",
                table: "Food",
                type: "decimal(6,2)",
                precision: 6,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "KcalPer100g",
                table: "Food",
                type: "decimal(6,1)",
                precision: 6,
                scale: 1,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "FatPer100g",
                table: "Food",
                type: "decimal(6,2)",
                precision: 6,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "CarbsPer100g",
                table: "Food",
                type: "decimal(6,2)",
                precision: 6,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.CreateIndex(
                name: "IX_Food_Name_FoodCategoryId",
                table: "Food",
                columns: new[] { "Name", "FoodCategoryId" },
                unique: true,
                filter: "[FoodCategoryId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Food_FoodCategory_FoodCategoryId",
                table: "Food",
                column: "FoodCategoryId",
                principalTable: "FoodCategory",
                principalColumn: "FoodCategoryId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Food_FoodCategory_FoodCategoryId",
                table: "Food");

            migrationBuilder.DropIndex(
                name: "IX_Food_Name_FoodCategoryId",
                table: "Food");

            migrationBuilder.AlterColumn<decimal>(
                name: "ProteinPer100g",
                table: "Food",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,2)",
                oldPrecision: 6,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "KcalPer100g",
                table: "Food",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,1)",
                oldPrecision: 6,
                oldScale: 1);

            migrationBuilder.AlterColumn<decimal>(
                name: "FatPer100g",
                table: "Food",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,2)",
                oldPrecision: 6,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "CarbsPer100g",
                table: "Food",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,2)",
                oldPrecision: 6,
                oldScale: 2);

            migrationBuilder.CreateIndex(
                name: "IX_Objective_Name_Category",
                table: "Objective",
                columns: new[] { "Name", "Category" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Food_FoodCategory_FoodCategoryId",
                table: "Food",
                column: "FoodCategoryId",
                principalTable: "FoodCategory",
                principalColumn: "FoodCategoryId");
        }
    }
}
