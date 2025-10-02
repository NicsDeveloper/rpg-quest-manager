# 🎮 IMPLEMENTAÇÃO - REFORMA COMPLETA DO RPG

## STATUS: EM ANDAMENTO

### ✅ COMPLETADO
1. **Adicionado endpoint** `GET /api/v1/combat/active/{heroId}` ao CombatController

### 🔄 EM IMPLEMENTAÇÃO

#### Arquivo: `src/RpgQuestManager.Api/Services/ICombatService.cs`
- **Adicionar método:** `Task<CombatSessionDetailDto?> GetActiveCombatByHeroIdAsync(int userId, int heroId);`

#### Arquivo: `src/RpgQuestManager.Api/Services/CombatService.cs`
- **Implementar método:** `GetActiveCombatByHeroIdAsync`

#### Arquivo: `src/RpgQuestManager.Api/Controllers/QuestsController.cs`
- **Modificar AcceptQuest:** Verificar se já existe quest ativa não completada
- **Lógica:** Só permitir aceitar se não tiver nenhuma quest em progresso

#### Arquivo: `src/RpgQuestManager.Api/Controllers/HeroesController.cs`  
- **Modificar Create:** Forçar Level = 1, Experience = 0
- **Adicionar lógica:** Sistema de distribuição de pontos (20 pontos totais)

#### Frontend: `frontend/src/pages/QuestCatalog.tsx`
- **Adicionar botão:** "⚔️ Ir para Missão" nas quests aceitas
- **Lógica:** Só mostrar se IsAccepted = true e IsCompleted = false

#### Frontend: `frontend/src/pages/Heroes.tsx`
- **Reformular criação:** Sistema de sliders para distribuição de 20 pontos
- **UI:** 3 sliders interligados (STR, DEX, INT)
- **Validação:** Total sempre = 20 pontos

#### Frontend: `frontend/src/pages/Shop.tsx`
- **Adicionar display:** Mostrar quantidade de dados após compra
- **UI:** Contador visual de dados disponíveis

#### Frontend: `frontend/src/pages/Profile.tsx`
- **Adicionar seção:** Inventário de itens
- **Funcionalidade:** Equipar/desequipar itens

#### Frontend: `frontend/src/pages/Combat.tsx`
- **Reformular completamente:**
  - Composição de party (selecionar 3 heróis)
  - Preview de inimigos e descobertas
  - Animação de rolagem de dados
  - Log de ações em tempo real
  - Explosão de recompensas

---

## 🚀 PRÓXIMA AÇÃO

Vou implementar todos os itens acima AGORA, começando pelo mais crítico (erro de combate) até as melhorias de UX.

Estimativa: 2h de implementação para sistema completo.

**Aguarde implementação...**

