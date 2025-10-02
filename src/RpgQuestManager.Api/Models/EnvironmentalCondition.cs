using System.ComponentModel.DataAnnotations;

namespace RpgQuestManager.Api.Models;

public enum EnvironmentalConditionType
{
    None,           // Sem condição
    Rain,           // Chuva: -10% precisão, +15% dano de água
    Snow,           // Neve: -20% velocidade, +20% dano de gelo
    Desert,         // Deserto: +10% dano de fogo, -15% vida máxima
    Forest,         // Floresta: +15% furtividade, +10% cura
    Night,          // Noite: +25% furtividade, -10% precisão
    Storm,          // Tempestade: -30% precisão, +25% dano elétrico
    Fog,            // Névoa: -25% precisão, +20% furtividade
    Heat,           // Calor: -10% defesa, +15% dano de fogo
    Cold            // Frio: -15% velocidade, +20% defesa
}

public class EnvironmentalCondition
{
    [Key]
    public int Id { get; set; }
    
    public int QuestId { get; set; }
    public Quest Quest { get; set; } = null!;
    
    public EnvironmentalConditionType Type { get; set; }
    public int Intensity { get; set; } = 1; // Intensidade 1-3
    public int Duration { get; set; } = 10; // Duração em turnos
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
    
    public string GetDescription()
    {
        return Type switch
        {
            EnvironmentalConditionType.Rain => $"Chuva (Intensidade {Intensity}) - Precisão -10%, Dano de Água +15%",
            EnvironmentalConditionType.Snow => $"Neve (Intensidade {Intensity}) - Velocidade -20%, Dano de Gelo +20%",
            EnvironmentalConditionType.Desert => $"Deserto (Intensidade {Intensity}) - Dano de Fogo +10%, Vida -15%",
            EnvironmentalConditionType.Forest => $"Floresta (Intensidade {Intensity}) - Furtividade +15%, Cura +10%",
            EnvironmentalConditionType.Night => $"Noite (Intensidade {Intensity}) - Furtividade +25%, Precisão -10%",
            EnvironmentalConditionType.Storm => $"Tempestade (Intensidade {Intensity}) - Precisão -30%, Dano Elétrico +25%",
            EnvironmentalConditionType.Fog => $"Névoa (Intensidade {Intensity}) - Precisão -25%, Furtividade +20%",
            EnvironmentalConditionType.Heat => $"Calor (Intensidade {Intensity}) - Defesa -10%, Dano de Fogo +15%",
            EnvironmentalConditionType.Cold => $"Frio (Intensidade {Intensity}) - Velocidade -15%, Defesa +20%",
            _ => "Sem condição ambiental"
        };
    }
    
    public string GetIcon()
    {
        return Type switch
        {
            EnvironmentalConditionType.Rain => "🌧️",
            EnvironmentalConditionType.Snow => "❄️",
            EnvironmentalConditionType.Desert => "🏜️",
            EnvironmentalConditionType.Forest => "🌲",
            EnvironmentalConditionType.Night => "🌙",
            EnvironmentalConditionType.Storm => "⛈️",
            EnvironmentalConditionType.Fog => "🌫️",
            EnvironmentalConditionType.Heat => "☀️",
            EnvironmentalConditionType.Cold => "🧊",
            _ => "🌤️"
        };
    }
    
    public Dictionary<string, float> GetModifiers()
    {
        var modifiers = new Dictionary<string, float>();
        
        switch (Type)
        {
            case EnvironmentalConditionType.Rain:
                modifiers["accuracy"] = -0.10f * Intensity;
                modifiers["water_damage"] = 0.15f * Intensity;
                break;
                
            case EnvironmentalConditionType.Snow:
                modifiers["speed"] = -0.20f * Intensity;
                modifiers["ice_damage"] = 0.20f * Intensity;
                break;
                
            case EnvironmentalConditionType.Desert:
                modifiers["fire_damage"] = 0.10f * Intensity;
                modifiers["max_health"] = -0.15f * Intensity;
                break;
                
            case EnvironmentalConditionType.Forest:
                modifiers["stealth"] = 0.15f * Intensity;
                modifiers["healing"] = 0.10f * Intensity;
                break;
                
            case EnvironmentalConditionType.Night:
                modifiers["stealth"] = 0.25f * Intensity;
                modifiers["accuracy"] = -0.10f * Intensity;
                break;
                
            case EnvironmentalConditionType.Storm:
                modifiers["accuracy"] = -0.30f * Intensity;
                modifiers["lightning_damage"] = 0.25f * Intensity;
                break;
                
            case EnvironmentalConditionType.Fog:
                modifiers["accuracy"] = -0.25f * Intensity;
                modifiers["stealth"] = 0.20f * Intensity;
                break;
                
            case EnvironmentalConditionType.Heat:
                modifiers["defense"] = -0.10f * Intensity;
                modifiers["fire_damage"] = 0.15f * Intensity;
                break;
                
            case EnvironmentalConditionType.Cold:
                modifiers["speed"] = -0.15f * Intensity;
                modifiers["defense"] = 0.20f * Intensity;
                break;
        }
        
        return modifiers;
    }
}
