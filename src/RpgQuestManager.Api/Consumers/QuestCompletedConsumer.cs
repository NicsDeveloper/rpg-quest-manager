using MassTransit;
using RpgQuestManager.Api.Events;

namespace RpgQuestManager.Api.Consumers;

public class QuestCompletedConsumer : IConsumer<QuestCompletedEvent>
{
    private readonly ILogger<QuestCompletedConsumer> _logger;
    
    public QuestCompletedConsumer(ILogger<QuestCompletedConsumer> logger)
    {
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<QuestCompletedEvent> context)
    {
        var message = context.Message;
        
        _logger.LogInformation(
            "🎉 Quest Completed! Hero {HeroName} (ID: {HeroId}) completou a quest '{QuestName}' (ID: {QuestId}). " +
            "XP ganho: {XP}, Ouro ganho: {Gold}, Novo nível: {Level}",
            message.HeroName, message.HeroId, message.QuestName, message.QuestId,
            message.ExperienceGained, message.GoldGained, message.NewLevel
        );
        
        // Aqui você poderia adicionar lógica adicional, como:
        // - Enviar notificação para o usuário
        // - Atualizar estatísticas globais
        // - Desbloquear conquistas
        // - etc.
        
        await Task.CompletedTask;
    }
}

