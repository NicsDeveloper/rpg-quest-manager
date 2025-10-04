using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RpgQuestManager.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryAndAuthSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Category",
                table: "Quests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CompletionText",
                table: "Quests",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Difficulty",
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

            migrationBuilder.AddColumn<int>(
                name: "GoldReward",
                table: "Quests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "IntroductionText",
                table: "Quests",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsRepeatable",
                table: "Quests",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<List<string>>(
                name: "Rewards",
                table: "Quests",
                type: "text[]",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "DefeatMessage",
                table: "Monsters",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Monsters",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "Monsters",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Size",
                table: "Monsters",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int[]>(
                name: "SpecialAbilities",
                table: "Monsters",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);

            migrationBuilder.AddColumn<int>(
                name: "SpecialAbilityChance",
                table: "Monsters",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TauntMessage",
                table: "Monsters",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VictoryMessage",
                table: "Monsters",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Characters",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Gold",
                table: "Characters",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastPlayedAt",
                table: "Characters",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Characters",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Rarity = table.Column<int>(type: "integer", nullable: false),
                    EquipmentSlot = table.Column<int>(type: "integer", nullable: true),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<int>(type: "integer", nullable: false),
                    StackSize = table.Column<int>(type: "integer", nullable: false),
                    IsConsumable = table.Column<bool>(type: "boolean", nullable: false),
                    IsTradeable = table.Column<bool>(type: "boolean", nullable: false),
                    IsSellable = table.Column<bool>(type: "boolean", nullable: false),
                    AttackBonus = table.Column<int>(type: "integer", nullable: true),
                    DefenseBonus = table.Column<int>(type: "integer", nullable: true),
                    HealthBonus = table.Column<int>(type: "integer", nullable: true),
                    MoraleBonus = table.Column<int>(type: "integer", nullable: true),
                    StatusEffects = table.Column<int[]>(type: "integer[]", nullable: false),
                    StatusEffectChance = table.Column<int>(type: "integer", nullable: true),
                    StatusEffectDuration = table.Column<int>(type: "integer", nullable: true),
                    RequiredLevel = table.Column<int>(type: "integer", nullable: true),
                    RequiredClasses = table.Column<List<string>>(type: "text[]", nullable: false),
                    DroppedBy = table.Column<int[]>(type: "integer[]", nullable: false),
                    DropChance = table.Column<int>(type: "integer", nullable: false),
                    FoundIn = table.Column<int[]>(type: "integer[]", nullable: false),
                    AvailableInShop = table.Column<bool>(type: "boolean", nullable: false),
                    ShopPrice = table.Column<int>(type: "integer", nullable: false),
                    ShopTypes = table.Column<List<string>>(type: "text[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InventoryItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    ItemId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    IsEquipped = table.Column<bool>(type: "boolean", nullable: false),
                    EquippedSlot = table.Column<int>(type: "integer", nullable: true),
                    AcquiredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryItems_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterEquipment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    WeaponId = table.Column<int>(type: "integer", nullable: true),
                    ShieldId = table.Column<int>(type: "integer", nullable: true),
                    HelmetId = table.Column<int>(type: "integer", nullable: true),
                    ArmorId = table.Column<int>(type: "integer", nullable: true),
                    GlovesId = table.Column<int>(type: "integer", nullable: true),
                    BootsId = table.Column<int>(type: "integer", nullable: true),
                    RingId = table.Column<int>(type: "integer", nullable: true),
                    AmuletId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterEquipment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterEquipment_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterEquipment_InventoryItems_AmuletId",
                        column: x => x.AmuletId,
                        principalTable: "InventoryItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CharacterEquipment_InventoryItems_ArmorId",
                        column: x => x.ArmorId,
                        principalTable: "InventoryItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CharacterEquipment_InventoryItems_BootsId",
                        column: x => x.BootsId,
                        principalTable: "InventoryItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CharacterEquipment_InventoryItems_GlovesId",
                        column: x => x.GlovesId,
                        principalTable: "InventoryItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CharacterEquipment_InventoryItems_HelmetId",
                        column: x => x.HelmetId,
                        principalTable: "InventoryItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CharacterEquipment_InventoryItems_RingId",
                        column: x => x.RingId,
                        principalTable: "InventoryItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CharacterEquipment_InventoryItems_ShieldId",
                        column: x => x.ShieldId,
                        principalTable: "InventoryItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CharacterEquipment_InventoryItems_WeaponId",
                        column: x => x.WeaponId,
                        principalTable: "InventoryItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Characters_UserId",
                table: "Characters",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterEquipment_AmuletId",
                table: "CharacterEquipment",
                column: "AmuletId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterEquipment_ArmorId",
                table: "CharacterEquipment",
                column: "ArmorId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterEquipment_BootsId",
                table: "CharacterEquipment",
                column: "BootsId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterEquipment_CharacterId",
                table: "CharacterEquipment",
                column: "CharacterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CharacterEquipment_GlovesId",
                table: "CharacterEquipment",
                column: "GlovesId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterEquipment_HelmetId",
                table: "CharacterEquipment",
                column: "HelmetId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterEquipment_RingId",
                table: "CharacterEquipment",
                column: "RingId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterEquipment_ShieldId",
                table: "CharacterEquipment",
                column: "ShieldId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterEquipment_WeaponId",
                table: "CharacterEquipment",
                column: "WeaponId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_CharacterId_ItemId",
                table: "InventoryItems",
                columns: new[] { "CharacterId", "ItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_ItemId",
                table: "InventoryItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_Token",
                table: "UserSessions",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_UserId",
                table: "UserSessions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Users_UserId",
                table: "Characters",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Users_UserId",
                table: "Characters");

            migrationBuilder.DropTable(
                name: "CharacterEquipment");

            migrationBuilder.DropTable(
                name: "UserSessions");

            migrationBuilder.DropTable(
                name: "InventoryItems");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Characters_UserId",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "CompletionText",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "Difficulty",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "EstimatedDuration",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "GoldReward",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "IntroductionText",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "IsRepeatable",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "Rewards",
                table: "Quests");

            migrationBuilder.DropColumn(
                name: "DefeatMessage",
                table: "Monsters");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Monsters");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "Monsters");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "Monsters");

            migrationBuilder.DropColumn(
                name: "SpecialAbilities",
                table: "Monsters");

            migrationBuilder.DropColumn(
                name: "SpecialAbilityChance",
                table: "Monsters");

            migrationBuilder.DropColumn(
                name: "TauntMessage",
                table: "Monsters");

            migrationBuilder.DropColumn(
                name: "VictoryMessage",
                table: "Monsters");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "Gold",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "LastPlayedAt",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Characters");
        }
    }
}
