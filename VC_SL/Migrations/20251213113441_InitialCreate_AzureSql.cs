using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VC_SL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate_AzureSql : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username_history = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Winrates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<int>(type: "int", nullable: false),
                    month = table.Column<int>(type: "int", nullable: false),
                    year = table.Column<int>(type: "int", nullable: false),
                    baseAttackWinrate = table.Column<float>(type: "real", nullable: true),
                    baseDefenceWinrate = table.Column<float>(type: "real", nullable: true),
                    fleetWinrate = table.Column<float>(type: "real", nullable: true),
                    baseAttackWin = table.Column<int>(type: "int", nullable: true),
                    baseAttackLoss = table.Column<int>(type: "int", nullable: true),
                    baseAttackDraw = table.Column<int>(type: "int", nullable: true),
                    baseDefenceWin = table.Column<int>(type: "int", nullable: true),
                    baseDefenceLoss = table.Column<int>(type: "int", nullable: true),
                    baseDefenceDraw = table.Column<int>(type: "int", nullable: true),
                    fleetWin = table.Column<int>(type: "int", nullable: true),
                    fleetLoss = table.Column<int>(type: "int", nullable: true),
                    fleetDraw = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Winrates", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Winrates_userId_year_month",
                table: "Winrates",
                columns: new[] { "userId", "year", "month" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Winrates");
        }
    }
}
