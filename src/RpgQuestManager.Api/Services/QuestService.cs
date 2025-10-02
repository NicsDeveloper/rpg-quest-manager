using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.DTOs.Quests;
using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Services;

public class QuestService : IQuestService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;
    private readonly ILogger<QuestService> _logger;
    
    public QuestService(
        ApplicationDbContext context, 
        IMapper mapper,
        INotificationService notificationService,
        ILogger<QuestService> logger)
    {
        _context = context;
        _mapper = mapper;
        _notificationService = notificationService;
        _logger = logger;
    }
    
    public async Task<QuestDto> CompleteQuestAsync(int heroId, int questId)
    {
        // Buscar herói e quest
        var hero = await _context.Heroes.FindAsync(heroId);
        if (hero == null)
        {
            throw new Exception($"Herói com ID {heroId} não encontrado");
        }
        
        var quest = await _context.Quests
            .Include(q => q.Rewards)
                .ThenInclude(r => r.Item)
            .FirstOrDefaultAsync(q => q.Id == questId);
            
        if (quest == null)
        {
            throw new Exception($"Quest com ID {questId} não encontrada");
        }
        
        // Verificar se a quest já foi completada (apenas para quests não repetíveis)
        if (!quest.IsRepeatable)
        {
            var existingHeroQuest = await _context.HeroQuests
                .FirstOrDefaultAsync(hq => hq.HeroId == heroId && hq.QuestId == questId);
                
            if (existingHeroQuest?.IsCompleted == true)
            {
                throw new Exception("Quest já foi completada por este herói");
            }
        }
        else
        {
            // Para quests repetíveis, verificar se já foi completada hoje
            var today = DateTime.UtcNow.Date;
            var todayCompletion = await _context.HeroQuests
                .FirstOrDefaultAsync(hq => hq.HeroId == heroId && 
                                         hq.QuestId == questId && 
                                         hq.IsCompleted == true &&
                                         hq.CompletedAt.HasValue &&
                                         hq.CompletedAt.Value.Date == today);
                                         
            if (todayCompletion != null)
            {
                throw new Exception("Quest diária já foi completada hoje. Tente novamente amanhã!");
            }
        }
        
        // Para quests repetíveis, sempre criar uma nova entrada
        // Para quests normais, usar a lógica existente
        HeroQuest heroQuest;
        
        if (quest.IsRepeatable)
        {
            // Quest repetível - sempre criar nova entrada
            heroQuest = new HeroQuest
            {
                HeroId = heroId,
                QuestId = questId,
                StartedAt = DateTime.UtcNow
            };
            _context.HeroQuests.Add(heroQuest);
        }
        else
        {
            // Quest normal - usar lógica existente
            var existingHeroQuest = await _context.HeroQuests
                .FirstOrDefaultAsync(hq => hq.HeroId == heroId && hq.QuestId == questId);
                
            if (existingHeroQuest == null)
            {
                existingHeroQuest = new HeroQuest
                {
                    HeroId = heroId,
                    QuestId = questId,
                    StartedAt = DateTime.UtcNow
                };
                _context.HeroQuests.Add(existingHeroQuest);
            }
            heroQuest = existingHeroQuest;
        }
        
        heroQuest.IsCompleted = true;
        heroQuest.CompletedAt = DateTime.UtcNow;
        
        // Aplicar recompensas
        var oldLevel = hero.Level;
        hero.Experience += quest.ExperienceReward;
        hero.Gold += quest.GoldReward;
        
        // Adicionar itens das recompensas ao inventário do herói
        foreach (var reward in quest.Rewards)
        {
            if (reward.ItemId.HasValue && reward.Item != null)
            {
                var existingHeroItem = await _context.HeroItems
                    .FirstOrDefaultAsync(hi => hi.HeroId == heroId && hi.ItemId == reward.ItemId);
                
                if (existingHeroItem != null)
                {
                    existingHeroItem.Quantity += reward.ItemQuantity;
                }
                else
                {
                    var heroItem = new HeroItem
                    {
                        HeroId = heroId,
                        ItemId = reward.ItemId.Value,
                        Quantity = reward.ItemQuantity,
                        IsEquipped = false,
                        AcquiredAt = DateTime.UtcNow
                    };
                    _context.HeroItems.Add(heroItem);
                }
                
                _logger.LogInformation(
                    "Item {ItemName} (x{Quantity}) adicionado ao inventário do herói {HeroName}",
                    reward.Item.Name, reward.ItemQuantity, hero.Name
                );
            }
        }
        
        // Verificar level up automático
        var initialLevel = hero.Level;
        if (hero.CanLevelUp())
        {
            hero.LevelUp();
            
            if (hero.Level > initialLevel)
            {
                await _notificationService.NotifyLevelUpAsync(hero, initialLevel, hero.Level);
            }
        }
        
        await _context.SaveChangesAsync();
        
        _logger.LogInformation(
            "🎉 Quest Completed! Herói {HeroName} (ID: {HeroId}) completou a quest {QuestName} (ID: {QuestId}). XP: {XP}, Ouro: {Gold}, Novo nível: {Level}",
            hero.Name, hero.Id, quest.Name, quest.Id, quest.ExperienceReward, quest.GoldReward, hero.Level
        );
        
        return _mapper.Map<QuestDto>(quest);
    }
}

