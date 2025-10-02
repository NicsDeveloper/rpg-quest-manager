using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Services;

public interface IComboService
{
    Task<PartyCombo?> DetectComboAsync(List<string> heroClasses);
    Task<BossWeakness?> GetBossWeaknessAsync(int enemyId, int comboId);
    Task<bool> RegisterDiscoveryAsync(int userId, int enemyId, int comboId);
    Task<bool> HasDiscoveredAsync(int userId, int enemyId, int comboId);
    Task<List<ComboDiscovery>> GetUserDiscoveriesAsync(int userId);
    Task<List<BossWeakness>> GetEnemyWeaknessesAsync(int enemyId);
}

