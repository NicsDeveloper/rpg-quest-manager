using RpgQuestManager.Api.Models;

namespace RpgQuestManager.Api.Data;

public static class SpecialAbilityData
{
    public static List<SpecialAbility> GetAllAbilities()
    {
        return new List<SpecialAbility>
        {
            // Habilidades de Ataque
            new SpecialAbility
            {
                Name = "Golpe Poderoso",
                Description = "Um ataque devastador que causa dano extra",
                IconUrl = "power-strike-icon",
                Type = AbilityType.Attack,
                Category = AbilityCategory.Basic,
                RequiredLevel = 1,
                ManaCost = 10,
                CooldownTurns = 2,
                Damage = 25,
                ExperienceCost = 0,
                GoldCost = 0
            },
            new SpecialAbility
            {
                Name = "Fúria do Guerreiro",
                Description = "Aumenta o ataque por 3 turnos",
                IconUrl = "warrior-fury-icon",
                Type = AbilityType.Buff,
                Category = AbilityCategory.Basic,
                RequiredLevel = 3,
                ManaCost = 15,
                CooldownTurns = 4,
                Duration = 3,
                AttackBonus = 10,
                ExperienceCost = 100,
                GoldCost = 50
            },
            new SpecialAbility
            {
                Name = "Investida",
                Description = "Ataque que causa dano e reduz a defesa do inimigo",
                IconUrl = "charge-icon",
                Type = AbilityType.Attack,
                Category = AbilityCategory.Advanced,
                RequiredLevel = 5,
                ManaCost = 20,
                CooldownTurns = 3,
                Damage = 30,
                StatusEffects = new List<StatusEffectType> { StatusEffectType.Weakened },
                ExperienceCost = 250,
                GoldCost = 100
            },
            new SpecialAbility
            {
                Name = "Golpe Crítico",
                Description = "Ataque com chance aumentada de acerto crítico",
                IconUrl = "critical-strike-icon",
                Type = AbilityType.Attack,
                Category = AbilityCategory.Advanced,
                RequiredLevel = 7,
                ManaCost = 25,
                CooldownTurns = 3,
                Damage = 20,
                CriticalChanceBonus = 50,
                ExperienceCost = 400,
                GoldCost = 200
            },

            // Habilidades de Defesa
            new SpecialAbility
            {
                Name = "Escudo Protetor",
                Description = "Aumenta a defesa por 3 turnos",
                IconUrl = "protective-shield-icon",
                Type = AbilityType.Defense,
                Category = AbilityCategory.Basic,
                RequiredLevel = 2,
                ManaCost = 12,
                CooldownTurns = 3,
                Duration = 3,
                DefenseBonus = 15,
                ExperienceCost = 75,
                GoldCost = 40
            },
            new SpecialAbility
            {
                Name = "Contra-ataque",
                Description = "Reflete parte do dano recebido de volta ao atacante",
                IconUrl = "counter-attack-icon",
                Type = AbilityType.Defense,
                Category = AbilityCategory.Advanced,
                RequiredLevel = 6,
                ManaCost = 18,
                CooldownTurns = 4,
                Duration = 2,
                DefenseBonus = 10,
                ExperienceCost = 300,
                GoldCost = 150
            },
            new SpecialAbility
            {
                Name = "Fortaleza",
                Description = "Reduz significativamente o dano recebido",
                IconUrl = "fortress-icon",
                Type = AbilityType.Defense,
                Category = AbilityCategory.Master,
                RequiredLevel = 10,
                ManaCost = 30,
                CooldownTurns = 5,
                Duration = 4,
                DefenseBonus = 25,
                ExperienceCost = 800,
                GoldCost = 400
            },

            // Habilidades de Cura
            new SpecialAbility
            {
                Name = "Cura Menor",
                Description = "Restaura uma quantidade moderada de vida",
                IconUrl = "minor-heal-icon",
                Type = AbilityType.Healing,
                Category = AbilityCategory.Basic,
                RequiredLevel = 4,
                ManaCost = 20,
                CooldownTurns = 3,
                Healing = 40,
                ExperienceCost = 200,
                GoldCost = 100
            },
            new SpecialAbility
            {
                Name = "Cura Maior",
                Description = "Restaura uma grande quantidade de vida",
                IconUrl = "major-heal-icon",
                Type = AbilityType.Healing,
                Category = AbilityCategory.Advanced,
                RequiredLevel = 8,
                ManaCost = 35,
                CooldownTurns = 4,
                Healing = 80,
                ExperienceCost = 500,
                GoldCost = 250
            },
            new SpecialAbility
            {
                Name = "Regeneração",
                Description = "Cura gradualmente ao longo de 5 turnos",
                IconUrl = "regeneration-icon",
                Type = AbilityType.Healing,
                Category = AbilityCategory.Master,
                RequiredLevel = 12,
                ManaCost = 25,
                CooldownTurns = 6,
                Duration = 5,
                Healing = 15, // Por turno
                ExperienceCost = 1000,
                GoldCost = 500
            },

            // Habilidades de Suporte
            new SpecialAbility
            {
                Name = "Bênção da Força",
                Description = "Aumenta o ataque e a defesa de todos os aliados",
                IconUrl = "strength-blessing-icon",
                Type = AbilityType.Support,
                Category = AbilityCategory.Advanced,
                RequiredLevel = 9,
                ManaCost = 40,
                CooldownTurns = 5,
                Duration = 4,
                AttackBonus = 8,
                DefenseBonus = 8,
                AreaOfEffect = 3,
                ExperienceCost = 600,
                GoldCost = 300
            },
            new SpecialAbility
            {
                Name = "Purificação",
                Description = "Remove todos os efeitos negativos",
                IconUrl = "purification-icon",
                Type = AbilityType.Support,
                Category = AbilityCategory.Advanced,
                RequiredLevel = 11,
                ManaCost = 30,
                CooldownTurns = 4,
                StatusEffectsToRemove = new List<StatusEffectType> 
                { 
                    StatusEffectType.Poison, 
                    StatusEffectType.Weakened, 
                    StatusEffectType.Stunned 
                },
                ExperienceCost = 700,
                GoldCost = 350
            },

            // Habilidades Últimas
            new SpecialAbility
            {
                Name = "Fúria Divina",
                Description = "Ataque devastador que causa dano massivo",
                IconUrl = "divine-fury-icon",
                Type = AbilityType.Attack,
                Category = AbilityCategory.Legendary,
                RequiredLevel = 15,
                ManaCost = 50,
                CooldownTurns = 8,
                Damage = 100,
                CriticalChanceBonus = 100,
                IsUltimate = true,
                ExperienceCost = 2000,
                GoldCost = 1000
            },
            new SpecialAbility
            {
                Name = "Barreira Divina",
                Description = "Proteção absoluta que bloqueia todo o dano",
                IconUrl = "divine-barrier-icon",
                Type = AbilityType.Defense,
                Category = AbilityCategory.Legendary,
                RequiredLevel = 18,
                ManaCost = 60,
                CooldownTurns = 10,
                Duration = 2,
                DefenseBonus = 100,
                IsUltimate = true,
                ExperienceCost = 3000,
                GoldCost = 1500
            },
            new SpecialAbility
            {
                Name = "Ressurreição",
                Description = "Revive um aliado morto com vida completa",
                IconUrl = "resurrection-icon",
                Type = AbilityType.Healing,
                Category = AbilityCategory.Mythic,
                RequiredLevel = 20,
                ManaCost = 80,
                CooldownTurns = 15,
                Healing = 999, // Vida completa
                IsUltimate = true,
                ExperienceCost = 5000,
                GoldCost = 2500
            }
        };
    }

    public static List<Combo> GetAllCombos()
    {
        return new List<Combo>
        {
            new Combo
            {
                Name = "Combo Básico",
                Description = "Sequência simples de ataques",
                IconUrl = "basic-combo-icon",
                Type = ComboType.Attack,
                RequiredLevel = 3,
                ExperienceReward = 100,
                GoldReward = 50
            },
            new Combo
            {
                Name = "Combo de Defesa",
                Description = "Sequência defensiva poderosa",
                IconUrl = "defense-combo-icon",
                Type = ComboType.Defense,
                RequiredLevel = 5,
                ExperienceReward = 200,
                GoldReward = 100
            },
            new Combo
            {
                Name = "Combo de Cura",
                Description = "Sequência de cura avançada",
                IconUrl = "healing-combo-icon",
                Type = ComboType.Healing,
                RequiredLevel = 7,
                ExperienceReward = 300,
                GoldReward = 150
            },
            new Combo
            {
                Name = "Combo Épico",
                Description = "Sequência lendária de ataques",
                IconUrl = "epic-combo-icon",
                Type = ComboType.Mixed,
                RequiredLevel = 10,
                ExperienceReward = 500,
                GoldReward = 250
            }
        };
    }
}
