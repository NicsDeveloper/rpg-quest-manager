using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RpgQuestManager.Api.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDiceInventoryToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiceInventories_Heroes_HeroId",
                table: "DiceInventories");

            migrationBuilder.DropForeignKey(
                name: "FK_DiceInventories_Heroes_HeroId1",
                table: "DiceInventories");

            migrationBuilder.DropIndex(
                name: "IX_DiceInventories_HeroId1",
                table: "DiceInventories");

            migrationBuilder.DropColumn(
                name: "HeroId1",
                table: "DiceInventories");

            migrationBuilder.RenameColumn(
                name: "HeroId",
                table: "DiceInventories",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_DiceInventories_HeroId",
                table: "DiceInventories",
                newName: "IX_DiceInventories_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_DiceInventories_Users_UserId",
                table: "DiceInventories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiceInventories_Users_UserId",
                table: "DiceInventories");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "DiceInventories",
                newName: "HeroId");

            migrationBuilder.RenameIndex(
                name: "IX_DiceInventories_UserId",
                table: "DiceInventories",
                newName: "IX_DiceInventories_HeroId");

            migrationBuilder.AddColumn<int>(
                name: "HeroId1",
                table: "DiceInventories",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiceInventories_HeroId1",
                table: "DiceInventories",
                column: "HeroId1",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DiceInventories_Heroes_HeroId",
                table: "DiceInventories",
                column: "HeroId",
                principalTable: "Heroes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DiceInventories_Heroes_HeroId1",
                table: "DiceInventories",
                column: "HeroId1",
                principalTable: "Heroes",
                principalColumn: "Id");
        }
    }
}
