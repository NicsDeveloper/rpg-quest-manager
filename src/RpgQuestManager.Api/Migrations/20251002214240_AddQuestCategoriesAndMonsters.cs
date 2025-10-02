using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RpgQuestManager.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestCategoriesAndMonsters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Quests",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(5000)",
                oldMaxLength: 5000);

            migrationBuilder.AddColumn<int>(
                name: "BossId",
                table: "Quests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Quests",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Environment",
                table: "Quests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EstimatedDuration",
                table: "Quests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsBossQuest",
                table: "Quests",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnlocked",
                table: "Quests",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Prerequisites",
                table: "Quests",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SpecialRewards",
                table: "Quests",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "StoryOrder",
                table: "Quests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Monsters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Size = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Power = table.Column<int>(type: "integer", nullable: false),
                    Health = table.Column<int>(type: "integer", nullable: false),
                    Armor = table.Column<int>(type: "integer", nullable: false),
                    Speed = table.Column<int>(type: "integer", nullable: false),
                    Strength = table.Column<int>(type: "integer", nullable: false),
                    Dexterity = table.Column<int>(type: "integer", nullable: false),
                    Constitution = table.Column<int>(type: "integer", nullable: false),
                    Intelligence = table.Column<int>(type: "integer", nullable: false),
                    Wisdom = table.Column<int>(type: "integer", nullable: false),
                    Charisma = table.Column<int>(type: "integer", nullable: false),
                    AttackDice = table.Column<int>(type: "integer", nullable: false),
                    AttackBonus = table.Column<int>(type: "integer", nullable: false),
                    DamageBonus = table.Column<int>(type: "integer", nullable: false),
                    CriticalChance = table.Column<int>(type: "integer", nullable: false),
                    Resistances = table.Column<string>(type: "text", nullable: false),
                    Immunities = table.Column<string>(type: "text", nullable: false),
                    Vulnerabilities = table.Column<string>(type: "text", nullable: false),
                    StatusEffects = table.Column<string>(type: "text", nullable: false),
                    StatusImmunities = table.Column<string>(type: "text", nullable: false),
                    SpecialAbilities = table.Column<string>(type: "text", nullable: false),
                    SpecialAbilityCooldown = table.Column<int>(type: "integer", nullable: false),
                    PreferredEnvironment = table.Column<int>(type: "integer", nullable: false),
                    EnvironmentalBonuses = table.Column<string>(type: "text", nullable: false),
                    ExperienceReward = table.Column<int>(type: "integer", nullable: false),
                    GoldReward = table.Column<int>(type: "integer", nullable: false),
                    LootTable = table.Column<string>(type: "text", nullable: false),
                    BaseMorale = table.Column<int>(type: "integer", nullable: false),
                    MoraleRange = table.Column<int>(type: "integer", nullable: false),
                    IsBoss = table.Column<bool>(type: "boolean", nullable: false),
                    BossPhase = table.Column<int>(type: "integer", nullable: false),
                    BossHealthThreshold = table.Column<int>(type: "integer", nullable: false),
                    BossPhases = table.Column<string>(type: "text", nullable: false),
                    IsElite = table.Column<bool>(type: "boolean", nullable: false),
                    MinGroupSize = table.Column<int>(type: "integer", nullable: false),
                    MaxGroupSize = table.Column<int>(type: "integer", nullable: false),
                    SpawnChance = table.Column<int>(type: "integer", nullable: false),
                    Icon = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Color = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    Model = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Lore = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Origin = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Weakness = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monsters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuestCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Difficulty = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Environment = table.Column<int>(type: "integer", nullable: false),
                    MinLevel = table.Column<int>(type: "integer", nullable: false),
                    MaxLevel = table.Column<int>(type: "integer", nullable: false),
                    Icon = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Color = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestCategories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Quests_CategoryId",
                table: "Quests",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quests_QuestCategories_CategoryId",
                table: "Quests",
                column: "CategoryId",
                principalTable: "QuestCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quests_QuestCategories_CategoryId",
                table: "Quests");

            migrationBuilder.DropTable(
                name: "Monsters");

            migrationBuilder.DropTable(
                name: "QuestCategories");

            migrationBuilder.DropIndex(
                name: "IX_Quests_CategoryId",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "BossId",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "Environment",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "EstimatedDuration",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "IsBossQuest",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "IsUnlocked",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "Prerequisites",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "SpecialRewards",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "StoryOrder",
                table: "Quests");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Quests",
                type: "character varying(5000)",
                maxLength: 5000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000);
        }
    }
}
