SELECT hi."Quantity", i."Name", i."Rarity", hi."AcquiredAt"
FROM "HeroItems" hi
JOIN "Items" i ON hi."ItemId" = i."Id"
JOIN "Heroes" h ON hi."HeroId" = h."Id"
WHERE h."UserId" = 4 AND i."IsConsumable" = true AND (i."BonusStrength" > 0 OR i."BonusIntelligence" > 0 OR i."BonusDexterity" > 0)
ORDER BY hi."AcquiredAt" DESC;
