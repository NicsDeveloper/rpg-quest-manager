using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RpgQuestManager.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPartySystemAndCombos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Heroes",
                newName: "IsInActiveParty");

            migrationBuilder.AddColumn<int>(
                name: "PartySlot",
                table: "Heroes",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FreeDiceGrants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    DiceType = table.Column<int>(type: "integer", nullable: false),
                    LastClaimedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NextAvailableAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FreeDiceGrants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FreeDiceGrants_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PartyCombos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RequiredClass1 = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    RequiredClass2 = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    RequiredClass3 = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Effect = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Icon = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartyCombos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BossWeaknesses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EnemyId = table.Column<int>(type: "integer", nullable: false),
                    ComboId = table.Column<int>(type: "integer", nullable: false),
                    RollReduction = table.Column<int>(type: "integer", nullable: false),
                    DropMultiplier = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    ExpMultiplier = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    FlavorText = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BossWeaknesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BossWeaknesses_Enemies_EnemyId",
                        column: x => x.EnemyId,
                        principalTable: "Enemies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BossWeaknesses_PartyCombos_ComboId",
                        column: x => x.ComboId,
                        principalTable: "PartyCombos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComboDiscoveries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    EnemyId = table.Column<int>(type: "integer", nullable: false),
                    ComboId = table.Column<int>(type: "integer", nullable: false),
                    DiscoveredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TimesUsed = table.Column<int>(type: "integer", nullable: false),
                    TimesWon = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComboDiscoveries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComboDiscoveries_Enemies_EnemyId",
                        column: x => x.EnemyId,
                        principalTable: "Enemies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComboDiscoveries_PartyCombos_ComboId",
                        column: x => x.ComboId,
                        principalTable: "PartyCombos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComboDiscoveries_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BossWeaknesses_ComboId",
                table: "BossWeaknesses",
                column: "ComboId");

            migrationBuilder.CreateIndex(
                name: "IX_BossWeaknesses_EnemyId",
                table: "BossWeaknesses",
                column: "EnemyId");

            migrationBuilder.CreateIndex(
                name: "IX_ComboDiscoveries_ComboId",
                table: "ComboDiscoveries",
                column: "ComboId");

            migrationBuilder.CreateIndex(
                name: "IX_ComboDiscoveries_EnemyId",
                table: "ComboDiscoveries",
                column: "EnemyId");

            migrationBuilder.CreateIndex(
                name: "IX_ComboDiscoveries_UserId_EnemyId_ComboId",
                table: "ComboDiscoveries",
                columns: new[] { "UserId", "EnemyId", "ComboId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FreeDiceGrants_UserId_DiceType",
                table: "FreeDiceGrants",
                columns: new[] { "UserId", "DiceType" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BossWeaknesses");

            migrationBuilder.DropTable(
                name: "ComboDiscoveries");

            migrationBuilder.DropTable(
                name: "FreeDiceGrants");

            migrationBuilder.DropTable(
                name: "PartyCombos");

            migrationBuilder.DropColumn(
                name: "PartySlot",
                table: "Heroes");

            migrationBuilder.RenameColumn(
                name: "IsInActiveParty",
                table: "Heroes",
                newName: "IsActive");
        }
    }
}
