SELECT COUNT(*) as total_items FROM "Items" WHERE "IsConsumable" = true AND ("BonusStrength" > 0 OR "BonusIntelligence" > 0 OR "BonusDexterity" > 0);
