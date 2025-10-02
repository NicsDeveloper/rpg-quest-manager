using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RpgQuestManager.Api.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDiceInventoryFromHero : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Remove a coluna DiceInventoryId se existir (migration antiga)
            migrationBuilder.Sql(@"
                DO $$ 
                BEGIN 
                    IF EXISTS (
                        SELECT 1 
                        FROM information_schema.columns 
                        WHERE table_name = 'Heroes' 
                        AND column_name = 'DiceInventoryId'
                    ) THEN
                        ALTER TABLE ""Heroes"" DROP COLUMN ""DiceInventoryId"";
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Não há necessidade de reverter, pois DiceInventory agora pertence ao User
        }
    }
}

