using AutoMapper;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using RpgQuestManager.Api.Data;
using RpgQuestManager.Api.DTOs.Quests;
using RpgQuestManager.Api.Events;
using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Services;

public class QuestService : IQuestService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<QuestService> _logger;
    
    public QuestService(
        ApplicationDbContext context, 
        IMapper mapper,
        IPublishEndpoint publishEndpoint,
        ILogger<QuestService> logger)
    {
        _context = context;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
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
            .FirstOrDefaultAsync(q => q.Id == questId);
            
        if (quest == null)
        {
            throw new Exception($"Quest com ID {questId} não encontrada");
        }
        
        // Verificar se a quest já foi completada
        var existingHeroQuest = await _context.HeroQuests
            .FirstOrDefaultAsync(hq => hq.HeroId == heroId && hq.QuestId == questId);
            
        if (existingHeroQuest?.IsCompleted == true)
        {
            throw new Exception("Quest já foi completada por este herói");
        }
        
        // Criar ou atualizar HeroQuest
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
        
        existingHeroQuest.IsCompleted = true;
        existingHeroQuest.CompletedAt = DateTime.UtcNow;
        
        // Aplicar recompensas
        var oldLevel = hero.Level;
        hero.Experience += quest.ExperienceReward;
        hero.Gold += quest.GoldReward;
        
        // Verificar level up automático
        if (hero.CanLevelUp())
        {
            hero.LevelUp();
        }
        
        await _context.SaveChangesAsync();
        
        _logger.LogInformation(
            "Herói {HeroName} (ID: {HeroId}) completou a quest {QuestName} (ID: {QuestId})",
            hero.Name, hero.Id, quest.Name, quest.Id
        );
        
        // Publicar evento de conclusão de quest
        await _publishEndpoint.Publish(new QuestCompletedEvent
        {
            HeroId = hero.Id,
            HeroName = hero.Name,
            QuestId = quest.Id,
            QuestName = quest.Name,
            ExperienceGained = quest.ExperienceReward,
            GoldGained = quest.GoldReward,
            NewLevel = hero.Level,
            CompletedAt = DateTime.UtcNow
        });
        
        return _mapper.Map<QuestDto>(quest);
    }
}

