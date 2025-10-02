using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RpgQuestManager.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCombatSystemWithParty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CombatSessions_Heroes_HeroId",
                table: "CombatSessions");

            migrationBuilder.DropIndex(
                name: "IX_CombatSessions_HeroId",
                table: "CombatSessions");

            migrationBuilder.RenameColumn(
                name: "HeroId",
                table: "CombatSessions",
                newName: "GroupBonus");

            migrationBuilder.AddColumn<int>(
                name: "HeroId1",
                table: "DiceInventories",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ComboBonus",
                table: "CombatSessions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ComboId",
                table: "CombatSessions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HeroIds",
                table: "CombatSessions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_DiceInventories_HeroId1",
                table: "DiceInventories",
                column: "HeroId1",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CombatSessions_ComboId",
                table: "CombatSessions",
                column: "ComboId");

            migrationBuilder.AddForeignKey(
                name: "FK_CombatSessions_PartyCombos_ComboId",
                table: "CombatSessions",
                column: "ComboId",
                principalTable: "PartyCombos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_DiceInventories_Heroes_HeroId1",
                table: "DiceInventories",
                column: "HeroId1",
                principalTable: "Heroes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CombatSessions_PartyCombos_ComboId",
                table: "CombatSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_DiceInventories_Heroes_HeroId1",
                table: "DiceInventories");

            migrationBuilder.DropIndex(
                name: "IX_DiceInventories_HeroId1",
                table: "DiceInventories");

            migrationBuilder.DropIndex(
                name: "IX_CombatSessions_ComboId",
                table: "CombatSessions");

            migrationBuilder.DropColumn(
                name: "HeroId1",
                table: "DiceInventories");

            migrationBuilder.DropColumn(
                name: "ComboBonus",
                table: "CombatSessions");

            migrationBuilder.DropColumn(
                name: "ComboId",
                table: "CombatSessions");

            migrationBuilder.DropColumn(
                name: "HeroIds",
                table: "CombatSessions");

            migrationBuilder.RenameColumn(
                name: "GroupBonus",
                table: "CombatSessions",
                newName: "HeroId");

            migrationBuilder.CreateIndex(
                name: "IX_CombatSessions_HeroId",
                table: "CombatSessions",
                column: "HeroId");

            migrationBuilder.AddForeignKey(
                name: "FK_CombatSessions_Heroes_HeroId",
                table: "CombatSessions",
                column: "HeroId",
                principalTable: "Heroes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
