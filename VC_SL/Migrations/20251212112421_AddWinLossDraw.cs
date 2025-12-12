using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VC_SL.Migrations
{
    /// <inheritdoc />
    public partial class AddWinLossDraw : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "baseAttackDraw",
                table: "Winrates",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "baseAttackLoss",
                table: "Winrates",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "baseAttackWin",
                table: "Winrates",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "baseDefenceDraw",
                table: "Winrates",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "baseDefenceLoss",
                table: "Winrates",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "baseDefenceWin",
                table: "Winrates",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "fleetDraw",
                table: "Winrates",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "fleetLoss",
                table: "Winrates",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "fleetWin",
                table: "Winrates",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "baseAttackDraw",
                table: "Winrates");

            migrationBuilder.DropColumn(
                name: "baseAttackLoss",
                table: "Winrates");

            migrationBuilder.DropColumn(
                name: "baseAttackWin",
                table: "Winrates");

            migrationBuilder.DropColumn(
                name: "baseDefenceDraw",
                table: "Winrates");

            migrationBuilder.DropColumn(
                name: "baseDefenceLoss",
                table: "Winrates");

            migrationBuilder.DropColumn(
                name: "baseDefenceWin",
                table: "Winrates");

            migrationBuilder.DropColumn(
                name: "fleetDraw",
                table: "Winrates");

            migrationBuilder.DropColumn(
                name: "fleetLoss",
                table: "Winrates");

            migrationBuilder.DropColumn(
                name: "fleetWin",
                table: "Winrates");
        }
    }
}