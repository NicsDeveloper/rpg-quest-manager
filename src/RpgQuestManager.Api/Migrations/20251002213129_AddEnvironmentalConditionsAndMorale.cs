using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RpgQuestManager.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddEnvironmentalConditionsAndMorale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EnvironmentalCondition",
                table: "Quests",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EnvironmentalIntensity",
                table: "Quests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ImmuneClasses",
                table: "Quests",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImmuneEnemyTypes",
                table: "Quests",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "EnvironmentalConditions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QuestId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Intensity = table.Column<int>(type: "integer", nullable: false),
                    Duration = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnvironmentalConditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnvironmentalConditions_Quests_QuestId",
                        column: x => x.QuestId,
                        principalTable: "Quests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MoraleStates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CombatSessionId = table.Column<int>(type: "integer", nullable: false),
                    HeroId = table.Column<int>(type: "integer", nullable: true),
                    EnemyId = table.Column<int>(type: "integer", nullable: true),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    MoralePoints = table.Column<int>(type: "integer", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoraleStates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MoraleStates_CombatSessions_CombatSessionId",
                        column: x => x.CombatSessionId,
                        principalTable: "CombatSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MoraleStates_Enemies_EnemyId",
                        column: x => x.EnemyId,
                        principalTable: "Enemies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_MoraleStates_Heroes_HeroId",
                        column: x => x.HeroId,
                        principalTable: "Heroes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EnvironmentalConditions_QuestId",
                table: "EnvironmentalConditions",
                column: "QuestId");

            migrationBuilder.CreateIndex(
                name: "IX_MoraleStates_CombatSessionId",
                table: "MoraleStates",
                column: "CombatSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_MoraleStates_EnemyId",
                table: "MoraleStates",
                column: "EnemyId");

            migrationBuilder.CreateIndex(
                name: "IX_MoraleStates_HeroId",
                table: "MoraleStates",
                column: "HeroId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EnvironmentalConditions");

            migrationBuilder.DropTable(
                name: "MoraleStates");

            migrationBuilder.DropColumn(
                name: "EnvironmentalCondition",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "EnvironmentalIntensity",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "ImmuneClasses",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "ImmuneEnemyTypes",
                table: "Quests");
        }
    }
}
