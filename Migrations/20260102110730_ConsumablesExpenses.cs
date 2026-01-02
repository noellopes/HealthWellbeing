using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthWellbeingRoom.Migrations
{
    /// <inheritdoc />
    public partial class ConsumablesExpenses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConsumablesExpenses",
                columns: table => new
                {
                    ConsumablesExpensesId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConsumableId = table.Column<int>(type: "int", nullable: false),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    RoomReservationId = table.Column<int>(type: "int", nullable: false),
                    QuantityUsed = table.Column<int>(type: "int", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsumablesExpenses", x => x.ConsumablesExpensesId);
                    table.ForeignKey(
                        name: "FK_ConsumablesExpenses_Consumivel_ConsumableId",
                        column: x => x.ConsumableId,
                        principalTable: "Consumivel",
                        principalColumn: "ConsumivelId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConsumablesExpenses_RoomReservations_RoomReservationId",
                        column: x => x.RoomReservationId,
                        principalTable: "RoomReservations",
                        principalColumn: "RoomReservationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConsumablesExpenses_Room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConsumablesExpenses_ConsumableId",
                table: "ConsumablesExpenses",
                column: "ConsumableId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsumablesExpenses_RoomId",
                table: "ConsumablesExpenses",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsumablesExpenses_RoomReservationId",
                table: "ConsumablesExpenses",
                column: "RoomReservationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConsumablesExpenses");
        }
    }
}
