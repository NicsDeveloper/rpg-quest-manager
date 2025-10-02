using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RpgQuestManager.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddNewCombatSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CombatSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    HeroIds = table.Column<string>(type: "text", nullable: false),
                    QuestId = table.Column<int>(type: "integer", nullable: false),
                    CurrentEnemyId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    IsHeroTurn = table.Column<bool>(type: "boolean", nullable: false),
                    CurrentEnemyHealth = table.Column<int>(type: "integer", nullable: false),
                    MaxEnemyHealth = table.Column<int>(type: "integer", nullable: false),
                    HeroHealths = table.Column<string>(type: "text", nullable: false),
                    MaxHeroHealths = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CombatSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CombatSessions_Enemies_CurrentEnemyId",
                        column: x => x.CurrentEnemyId,
                        principalTable: "Enemies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CombatSessions_Quests_QuestId",
                        column: x => x.QuestId,
                        principalTable: "Quests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CombatSessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    DiceUsed = table.Column<int>(type: "integer", nullable: true),
                    DiceResult = table.Column<int>(type: "integer", nullable: true),
                    RequiredRoll = table.Column<int>(type: "integer", nullable: true),
                    Success = table.Column<bool>(type: "boolean", nullable: true),
                    DamageDealt = table.Column<int>(type: "integer", nullable: true),
                    EnemyHealthAfter = table.Column<int>(type: "integer", nullable: true),
                    Details = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
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
                name: "IX_CombatSessions_CurrentEnemyId",
                table: "CombatSessions",
                column: "CurrentEnemyId");

            migrationBuilder.CreateIndex(
                name: "IX_CombatSessions_QuestId",
                table: "CombatSessions",
                column: "QuestId");

            migrationBuilder.CreateIndex(
                name: "IX_CombatSessions_UserId",
                table: "CombatSessions",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CombatLogs");

            migrationBuilder.DropTable(
                name: "CombatSessions");
        }
    }
}
