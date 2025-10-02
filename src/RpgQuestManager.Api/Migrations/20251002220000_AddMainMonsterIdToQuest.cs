using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RpgQuestManager.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddMainMonsterIdToQuest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MainMonsterId",
                table: "Quests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Quests_MainMonsterId",
                table: "Quests",
                column: "MainMonsterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quests_Monsters_MainMonsterId",
                table: "Quests",
                column: "MainMonsterId",
                principalTable: "Monsters",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quests_Monsters_MainMonsterId",
                table: "Quests");

            migrationBuilder.DropIndex(
                name: "IX_Quests_MainMonsterId",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "MainMonsterId",
                table: "Quests");
        }
    }
}
