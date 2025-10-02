using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RpgQuestManager.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddBaseAttributesToHero : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BaseDexterity",
                table: "Heroes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BaseIntelligence",
                table: "Heroes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BaseStrength",
                table: "Heroes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BonusDexterity",
                table: "Heroes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BonusIntelligence",
                table: "Heroes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BonusStrength",
                table: "Heroes",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseDexterity",
                table: "Heroes");

            migrationBuilder.DropColumn(
                name: "BaseIntelligence",
                table: "Heroes");

            migrationBuilder.DropColumn(
                name: "BaseStrength",
                table: "Heroes");

            migrationBuilder.DropColumn(
                name: "BonusDexterity",
                table: "Heroes");

            migrationBuilder.DropColumn(
                name: "BonusIntelligence",
                table: "Heroes");

            migrationBuilder.DropColumn(
                name: "BonusStrength",
                table: "Heroes");
        }
    }
}
