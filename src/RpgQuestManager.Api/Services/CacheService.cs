using System.Text.Json;
using StackExchange.Redis;

namespace RpgQuestManager.Api.Services;

public class CacheService : ICacheService
{
    private readonly IDatabase _database;
    private readonly ILogger<CacheService> _logger;
    
    public CacheService(IConnectionMultiplexer redis, ILogger<CacheService> logger)
    {
        _database = redis.GetDatabase();
        _logger = logger;
    }
    
    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            var value = await _database.StringGetAsync(key);
            
            if (!value.HasValue)
            {
                return default;
            }
            
            return JsonSerializer.Deserialize<T>(value!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar chave {Key} do cache", key);
            return default;
        }
    }
    
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        try
        {
            var json = JsonSerializer.Serialize(value);
            await _database.StringSetAsync(key, json, expiration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao salvar chave {Key} no cache", key);
        }
    }
    
    public async Task RemoveAsync(string key)
    {
        try
        {
            await _database.KeyDeleteAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover chave {Key} do cache", key);
        }
    }
    
    public async Task<bool> ExistsAsync(string key)
    {
        try
        {
            return await _database.KeyExistsAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar existência da chave {Key} no cache", key);
            return false;
        }
    }
}

