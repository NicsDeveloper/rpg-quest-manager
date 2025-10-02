using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RpgQuestManager.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCombatSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CombatLogs");

            migrationBuilder.DropTable(
                name: "CombatSessions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CombatSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ComboId = table.Column<int>(type: "integer", nullable: true),
                    CurrentEnemyId = table.Column<int>(type: "integer", nullable: true),
                    QuestId = table.Column<int>(type: "integer", nullable: false),
                    CombatStarted = table.Column<bool>(type: "boolean", nullable: false),
                    ComboBonus = table.Column<int>(type: "integer", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CurrentEnemyHealth = table.Column<int>(type: "integer", nullable: true),
                    GroupBonus = table.Column<int>(type: "integer", nullable: false),
                    HeroHealths = table.Column<string>(type: "text", nullable: false),
                    HeroIds = table.Column<string>(type: "text", nullable: false),
                    HeroTurnFirst = table.Column<bool>(type: "boolean", nullable: false),
                    MaxEnemyHealth = table.Column<int>(type: "integer", nullable: true),
                    MaxHeroHealths = table.Column<string>(type: "text", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CombatSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CombatSessions_Enemies_CurrentEnemyId",
                        column: x => x.CurrentEnemyId,
                        principalTable: "Enemies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CombatSessions_PartyCombos_ComboId",
                        column: x => x.ComboId,
                        principalTable: "PartyCombos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CombatSessions_Quests_QuestId",
                        column: x => x.QuestId,
                        principalTable: "Quests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CombatLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CombatSessionId = table.Column<int>(type: "integer", nullable: false),
                    EnemyId = table.Column<int>(type: "integer", nullable: true),
                    Action = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DamageDealt = table.Column<int>(type: "integer", nullable: true),
                    Details = table.Column<string>(type: "text", nullable: false),
                    DiceResult = table.Column<int>(type: "integer", nullable: true),
                    DiceUsed = table.Column<int>(type: "integer", nullable: true),
                    EnemyHealthAfter = table.Column<int>(type: "integer", nullable: true),
                    RequiredRoll = table.Column<int>(type: "integer", nullable: true),
                    Success = table.Column<bool>(type: "boolean", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CombatLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CombatLogs_CombatSessions_CombatSessionId",
                        column: x => x.CombatSessionId,
                        principalTable: "CombatSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CombatLogs_Enemies_EnemyId",
                        column: x => x.EnemyId,
                        principalTable: "Enemies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CombatLogs_CombatSessionId",
                table: "CombatLogs",
                column: "CombatSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_CombatLogs_EnemyId",
                table: "CombatLogs",
                column: "EnemyId");

            migrationBuilder.CreateIndex(
                name: "IX_CombatSessions_ComboId",
                table: "CombatSessions",
                column: "ComboId");

            migrationBuilder.CreateIndex(
                name: "IX_CombatSessions_CurrentEnemyId",
                table: "CombatSessions",
                column: "CurrentEnemyId");

            migrationBuilder.CreateIndex(
                name: "IX_CombatSessions_QuestId",
                table: "CombatSessions",
                column: "QuestId");
        }
    }
}
