using AutoMapper;
using RpgQuestManager.Api.DTOs.Heroes;
using RpgQuestManager.Api.DTOs.Quests;
using RpgQuestManager.Api.DTOs.Enemies;
using RpgQuestManager.Api.DTOs.Rewards;
using RpgQuestManager.Api.DTOs.Items;
using RpgQuestManager.Api.DTOs.Shop;
using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Services;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Hero mappings
        CreateMap<Hero, HeroDto>();
        CreateMap<CreateHeroRequest, Hero>();
        CreateMap<UpdateHeroRequest, Hero>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        
        // Quest mappings
        CreateMap<Quest, QuestDto>();
        CreateMap<CreateQuestRequest, Quest>();
        CreateMap<UpdateQuestRequest, Quest>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        
        // Enemy mappings
        CreateMap<Enemy, EnemyDto>();
        CreateMap<CreateEnemyRequest, Enemy>();
        CreateMap<UpdateEnemyRequest, Enemy>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        
        // Reward mappings
        CreateMap<Reward, RewardDto>();
        CreateMap<CreateRewardRequest, Reward>();
        
        // Item mappings
        CreateMap<Item, ItemDto>();
        CreateMap<CreateItemRequest, Item>();
        
        // Shop mappings
        CreateMap<Item, ShopItemDto>();
    }
}

