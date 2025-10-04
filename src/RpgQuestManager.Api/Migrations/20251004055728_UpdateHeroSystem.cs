using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RpgQuestManager.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHeroSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItems_Characters_CharacterId",
                table: "InventoryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PartyMembers_Characters_CharacterId",
                table: "PartyMembers");

            migrationBuilder.DropTable(
                name: "CharacterEquipment");

            migrationBuilder.DropIndex(
                name: "IX_InventoryItems_CharacterId_ItemId",
                table: "InventoryItems");

            migrationBuilder.RenameColumn(
                name: "CharacterId",
                table: "PartyMembers",
                newName: "HeroId");

            migrationBuilder.RenameIndex(
                name: "IX_PartyMembers_CharacterId",
                table: "PartyMembers",
                newName: "IX_PartyMembers_HeroId");

            migrationBuilder.AlterColumn<int>(
                name: "CharacterId",
                table: "InventoryItems",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "HeroId",
                table: "InventoryItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Defense",
                table: "Heroes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Health",
                table: "Heroes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Morale",
                table: "Heroes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EquipmentId",
                table: "Characters",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HeroEquipment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HeroId = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_HeroEquipment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeroEquipment_Heroes_HeroId",
                        column: x => x.HeroId,
                        principalTable: "Heroes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HeroEquipment_InventoryItems_AmuletId",
                        column: x => x.AmuletId,
                        principalTable: "InventoryItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HeroEquipment_InventoryItems_ArmorId",
                        column: x => x.ArmorId,
                        principalTable: "InventoryItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HeroEquipment_InventoryItems_BootsId",
                        column: x => x.BootsId,
                        principalTable: "InventoryItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HeroEquipment_InventoryItems_GlovesId",
                        column: x => x.GlovesId,
                        principalTable: "InventoryItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HeroEquipment_InventoryItems_HelmetId",
                        column: x => x.HelmetId,
                        principalTable: "InventoryItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HeroEquipment_InventoryItems_RingId",
                        column: x => x.RingId,
                        principalTable: "InventoryItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HeroEquipment_InventoryItems_ShieldId",
                        column: x => x.ShieldId,
                        principalTable: "InventoryItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HeroEquipment_InventoryItems_WeaponId",
                        column: x => x.WeaponId,
                        principalTable: "InventoryItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_CharacterId",
                table: "InventoryItems",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_HeroId_ItemId",
                table: "InventoryItems",
                columns: new[] { "HeroId", "ItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Characters_EquipmentId",
                table: "Characters",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_HeroEquipment_AmuletId",
                table: "HeroEquipment",
                column: "AmuletId");

            migrationBuilder.CreateIndex(
                name: "IX_HeroEquipment_ArmorId",
                table: "HeroEquipment",
                column: "ArmorId");

            migrationBuilder.CreateIndex(
                name: "IX_HeroEquipment_BootsId",
                table: "HeroEquipment",
                column: "BootsId");

            migrationBuilder.CreateIndex(
                name: "IX_HeroEquipment_GlovesId",
                table: "HeroEquipment",
                column: "GlovesId");

            migrationBuilder.CreateIndex(
                name: "IX_HeroEquipment_HelmetId",
                table: "HeroEquipment",
                column: "HelmetId");

            migrationBuilder.CreateIndex(
                name: "IX_HeroEquipment_HeroId",
                table: "HeroEquipment",
                column: "HeroId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HeroEquipment_RingId",
                table: "HeroEquipment",
                column: "RingId");

            migrationBuilder.CreateIndex(
                name: "IX_HeroEquipment_ShieldId",
                table: "HeroEquipment",
                column: "ShieldId");

            migrationBuilder.CreateIndex(
                name: "IX_HeroEquipment_WeaponId",
                table: "HeroEquipment",
                column: "WeaponId");

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_HeroEquipment_EquipmentId",
                table: "Characters",
                column: "EquipmentId",
                principalTable: "HeroEquipment",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItems_Characters_CharacterId",
                table: "InventoryItems",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItems_Heroes_HeroId",
                table: "InventoryItems",
                column: "HeroId",
                principalTable: "Heroes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PartyMembers_Heroes_HeroId",
                table: "PartyMembers",
                column: "HeroId",
                principalTable: "Heroes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Characters_HeroEquipment_EquipmentId",
                table: "Characters");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItems_Characters_CharacterId",
                table: "InventoryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItems_Heroes_HeroId",
                table: "InventoryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PartyMembers_Heroes_HeroId",
                table: "PartyMembers");

            migrationBuilder.DropTable(
                name: "HeroEquipment");

            migrationBuilder.DropIndex(
                name: "IX_InventoryItems_CharacterId",
                table: "InventoryItems");

            migrationBuilder.DropIndex(
                name: "IX_InventoryItems_HeroId_ItemId",
                table: "InventoryItems");

            migrationBuilder.DropIndex(
                name: "IX_Characters_EquipmentId",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "HeroId",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "Defense",
                table: "Heroes");

            migrationBuilder.DropColumn(
                name: "Health",
                table: "Heroes");

            migrationBuilder.DropColumn(
                name: "Morale",
                table: "Heroes");

            migrationBuilder.DropColumn(
                name: "EquipmentId",
                table: "Characters");

            migrationBuilder.RenameColumn(
                name: "HeroId",
                table: "PartyMembers",
                newName: "CharacterId");

            migrationBuilder.RenameIndex(
                name: "IX_PartyMembers_HeroId",
                table: "PartyMembers",
                newName: "IX_PartyMembers_CharacterId");

            migrationBuilder.AlterColumn<int>(
                name: "CharacterId",
                table: "InventoryItems",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "CharacterEquipment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AmuletId = table.Column<int>(type: "integer", nullable: true),
                    ArmorId = table.Column<int>(type: "integer", nullable: true),
                    BootsId = table.Column<int>(type: "integer", nullable: true),
                    CharacterId = table.Column<int>(type: "integer", nullable: false),
                    GlovesId = table.Column<int>(type: "integer", nullable: true),
                    HelmetId = table.Column<int>(type: "integer", nullable: true),
                    RingId = table.Column<int>(type: "integer", nullable: true),
                    ShieldId = table.Column<int>(type: "integer", nullable: true),
                    WeaponId = table.Column<int>(type: "integer", nullable: true)
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
                name: "IX_InventoryItems_CharacterId_ItemId",
                table: "InventoryItems",
                columns: new[] { "CharacterId", "ItemId" },
                unique: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItems_Characters_CharacterId",
                table: "InventoryItems",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PartyMembers_Characters_CharacterId",
                table: "PartyMembers",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
