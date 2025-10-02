using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RpgQuestManager.Api.Migrations
{
    /// <inheritdoc />
    public partial class RefactorRewardToUseItemRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemDescription",
                table: "Rewards");

            migrationBuilder.DropColumn(
                name: "ItemName",
                table: "Rewards");

            migrationBuilder.AddColumn<int>(
                name: "ItemId",
                table: "Rewards",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ItemQuantity",
                table: "Rewards",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Rewards_ItemId",
                table: "Rewards",
                column: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rewards_Items_ItemId",
                table: "Rewards",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rewards_Items_ItemId",
                table: "Rewards");

            migrationBuilder.DropIndex(
                name: "IX_Rewards_ItemId",
                table: "Rewards");

            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "Rewards");

            migrationBuilder.DropColumn(
                name: "ItemQuantity",
                table: "Rewards");

            migrationBuilder.AddColumn<string>(
                name: "ItemDescription",
                table: "Rewards",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemName",
                table: "Rewards",
                type: "text",
                nullable: true);
        }
    }
}
