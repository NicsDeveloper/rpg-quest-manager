SELECT h."Name" as hero_name, q."Name" as quest_name, hq."IsCompleted", hq."CompletedAt" 
FROM "HeroQuests" hq 
JOIN "Heroes" h ON hq."HeroId" = h."Id" 
JOIN "Quests" q ON hq."QuestId" = q."Id" 
WHERE h."UserId" = 4;
