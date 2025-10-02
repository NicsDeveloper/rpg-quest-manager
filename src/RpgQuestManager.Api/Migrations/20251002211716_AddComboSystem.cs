using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RpgQuestManager.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddComboSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ComboMultiplier",
                table: "CombatSessions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ConsecutiveFailures",
                table: "CombatSessions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ConsecutiveSuccesses",
                table: "CombatSessions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "LastAction",
                table: "CombatSessions",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ComboMultiplier",
                table: "CombatSessions");

            migrationBuilder.DropColumn(
                name: "ConsecutiveFailures",
                table: "CombatSessions");

            migrationBuilder.DropColumn(
                name: "ConsecutiveSuccesses",
                table: "CombatSessions");

            migrationBuilder.DropColumn(
                name: "LastAction",
                table: "CombatSessions");
        }
    }
}
