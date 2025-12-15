using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeing.Migrations
{
    /// <inheritdoc />
    public partial class fresh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alergy",
                columns: table => new
                {
                    AlergyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlergyName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alergy", x => x.AlergyId);
                });

            migrationBuilder.CreateTable(
                name: "Client",
                columns: table => new
                {
                    ClientId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateMember = table.Column<bool>(type: "bit", nullable: true),
                    WeightKg = table.Column<double>(type: "float", nullable: false),
                    HeightCm = table.Column<int>(type: "int", nullable: false),
                    ActivityFactor = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Client", x => x.ClientId);
                });

            migrationBuilder.CreateTable(
                name: "FoodCategory",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Category = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodCategory", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "FoodIntake",
                columns: table => new
                {
                    FoodIntakeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanId = table.Column<int>(type: "int", nullable: false),
                    FoodId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Eaten = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodIntake", x => x.FoodIntakeId);
                });

            migrationBuilder.CreateTable(
                name: "NutritionalComponent",
                columns: table => new
                {
                    NutritionalComponentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Basis = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NutritionalComponent", x => x.NutritionalComponentId);
                });

            migrationBuilder.CreateTable(
                name: "Nutritionist",
                columns: table => new
                {
                    NutritionistId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nutritionist", x => x.NutritionistId);
                });

            migrationBuilder.CreateTable(
                name: "Portion",
                columns: table => new
                {
                    PortionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PortionName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Portion", x => x.PortionId);
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
                name: "ClientAlergy",
                columns: table => new
                {
                    ClientAlergyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    AlergyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientAlergy", x => x.ClientAlergyId);
                    table.ForeignKey(
                        name: "FK_ClientAlergy_Alergy_AlergyId",
                        column: x => x.AlergyId,
                        principalTable: "Alergy",
                        principalColumn: "AlergyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientAlergy_Client_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Goal",
                columns: table => new
                {
                    GoalId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    GoalName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DailyCalories = table.Column<int>(type: "int", nullable: false),
                    DailyProtein = table.Column<int>(type: "int", nullable: false),
                    DailyFat = table.Column<int>(type: "int", nullable: false),
                    DailyHydrates = table.Column<int>(type: "int", nullable: false),
                    DailyVitamins = table.Column<int>(type: "int", nullable: false),
                    DailyMinerals = table.Column<int>(type: "int", nullable: false),
                    DailyFibers = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goal", x => x.GoalId);
                    table.ForeignKey(
                        name: "FK_Goal_Client_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Member",
                columns: table => new
                {
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    MemberId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Member", x => x.ClientId);
                    table.ForeignKey(
                        name: "FK_Member_Client_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Food",
                columns: table => new
                {
                    FoodId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FoodIntakeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Food", x => x.FoodId);
                    table.ForeignKey(
                        name: "FK_Food_FoodCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "FoodCategory",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Food_FoodIntake_FoodIntakeId",
                        column: x => x.FoodIntakeId,
                        principalTable: "FoodIntake",
                        principalColumn: "FoodIntakeId");
                });

            migrationBuilder.CreateTable(
                name: "Plan",
                columns: table => new
                {
                    PlanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Done = table.Column<bool>(type: "bit", nullable: false),
                    FoodIntakeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plan", x => x.PlanId);
                    table.ForeignKey(
                        name: "FK_Plan_FoodIntake_FoodIntakeId",
                        column: x => x.FoodIntakeId,
                        principalTable: "FoodIntake",
                        principalColumn: "FoodIntakeId");
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
                    FoodId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alergia", x => x.AlergiaID);
                    table.ForeignKey(
                        name: "FK_Alergia_Food_FoodId",
                        column: x => x.FoodId,
                        principalTable: "Food",
                        principalColumn: "FoodId");
                });

            migrationBuilder.CreateTable(
                name: "FoodNutritionalComponent",
                columns: table => new
                {
                    FoodNutritionalComponentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NutritionalComponentId = table.Column<int>(type: "int", nullable: false),
                    FoodId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodNutritionalComponent", x => x.FoodNutritionalComponentId);
                    table.ForeignKey(
                        name: "FK_FoodNutritionalComponent_Food_FoodId",
                        column: x => x.FoodId,
                        principalTable: "Food",
                        principalColumn: "FoodId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FoodNutritionalComponent_NutritionalComponent_NutritionalComponentId",
                        column: x => x.NutritionalComponentId,
                        principalTable: "NutritionalComponent",
                        principalColumn: "NutritionalComponentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FoodPlan",
                columns: table => new
                {
                    PlanFoodId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PortionId = table.Column<int>(type: "int", nullable: false),
                    PlanId = table.Column<int>(type: "int", nullable: false),
                    FoodId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodPlan", x => x.PlanFoodId);
                    table.ForeignKey(
                        name: "FK_FoodPlan_Food_FoodId",
                        column: x => x.FoodId,
                        principalTable: "Food",
                        principalColumn: "FoodId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FoodPlan_Plan_PlanId",
                        column: x => x.PlanId,
                        principalTable: "Plan",
                        principalColumn: "PlanId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FoodPlan_Portion_PortionId",
                        column: x => x.PortionId,
                        principalTable: "Portion",
                        principalColumn: "PortionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NutritionistClientPlan",
                columns: table => new
                {
                    PlanClientId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    PlanId = table.Column<int>(type: "int", nullable: false),
                    NutritionistId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NutritionistClientPlan", x => x.PlanClientId);
                    table.ForeignKey(
                        name: "FK_NutritionistClientPlan_Client_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NutritionistClientPlan_Nutritionist_NutritionistId",
                        column: x => x.NutritionistId,
                        principalTable: "Nutritionist",
                        principalColumn: "NutritionistId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NutritionistClientPlan_Plan_PlanId",
                        column: x => x.PlanId,
                        principalTable: "Plan",
                        principalColumn: "PlanId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alergia_FoodId",
                table: "Alergia",
                column: "FoodId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientAlergy_AlergyId",
                table: "ClientAlergy",
                column: "AlergyId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientAlergy_ClientId",
                table: "ClientAlergy",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Food_CategoryId",
                table: "Food",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Food_FoodIntakeId",
                table: "Food",
                column: "FoodIntakeId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodNutritionalComponent_FoodId",
                table: "FoodNutritionalComponent",
                column: "FoodId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodNutritionalComponent_NutritionalComponentId",
                table: "FoodNutritionalComponent",
                column: "NutritionalComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodPlan_FoodId",
                table: "FoodPlan",
                column: "FoodId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodPlan_PlanId",
                table: "FoodPlan",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodPlan_PortionId",
                table: "FoodPlan",
                column: "PortionId");

            migrationBuilder.CreateIndex(
                name: "IX_Goal_ClientId",
                table: "Goal",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_NutritionistClientPlan_ClientId",
                table: "NutritionistClientPlan",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_NutritionistClientPlan_NutritionistId",
                table: "NutritionistClientPlan",
                column: "NutritionistId");

            migrationBuilder.CreateIndex(
                name: "IX_NutritionistClientPlan_PlanId",
                table: "NutritionistClientPlan",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Plan_FoodIntakeId",
                table: "Plan",
                column: "FoodIntakeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alergia");

            migrationBuilder.DropTable(
                name: "ClientAlergy");

            migrationBuilder.DropTable(
                name: "FoodNutritionalComponent");

            migrationBuilder.DropTable(
                name: "FoodPlan");

            migrationBuilder.DropTable(
                name: "Goal");

            migrationBuilder.DropTable(
                name: "Member");

            migrationBuilder.DropTable(
                name: "NutritionistClientPlan");

            migrationBuilder.DropTable(
                name: "Receita");

            migrationBuilder.DropTable(
                name: "RestricaoAlimentar");

            migrationBuilder.DropTable(
                name: "Alergy");

            migrationBuilder.DropTable(
                name: "NutritionalComponent");

            migrationBuilder.DropTable(
                name: "Food");

            migrationBuilder.DropTable(
                name: "Portion");

            migrationBuilder.DropTable(
                name: "Client");

            migrationBuilder.DropTable(
                name: "Nutritionist");

            migrationBuilder.DropTable(
                name: "Plan");

            migrationBuilder.DropTable(
                name: "FoodCategory");

            migrationBuilder.DropTable(
                name: "FoodIntake");
        }
    }
}
