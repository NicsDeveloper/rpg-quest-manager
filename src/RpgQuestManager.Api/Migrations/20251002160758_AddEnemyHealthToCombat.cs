using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RpgQuestManager.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddEnemyHealthToCombat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentEnemyHealth",
                table: "CombatSessions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxEnemyHealth",
                table: "CombatSessions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DamageDealt",
                table: "CombatLogs",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EnemyHealthAfter",
                table: "CombatLogs",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentEnemyHealth",
                table: "CombatSessions");

            migrationBuilder.DropColumn(
                name: "MaxEnemyHealth",
                table: "CombatSessions");

            migrationBuilder.DropColumn(
                name: "DamageDealt",
                table: "CombatLogs");

            migrationBuilder.DropColumn(
                name: "EnemyHealthAfter",
                table: "CombatLogs");
        }
    }
}
