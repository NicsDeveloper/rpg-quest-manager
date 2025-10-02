# 🎮 REFORMA COMPLETA DO SISTEMA RPG

## 🔴 FASE 1: CORREÇÕES CRÍTICAS

### 1.1 Corrigir Erro de Combate
**Problema:** Rota `/api/v1/combat/active/{heroId}` está retornando 404
**Solução:** Verificar e corrigir CombatController

### 1.2 Limite de Uma Quest Por Vez
**Backend:**
- Modificar endpoint `AcceptQuest` para verificar quests ativas
- Retornar erro se já existe quest ativa e não completada

**Frontend:**
- Desabilitar botão "Aceitar" se já tem quest ativa
- Mostrar mensagem: "Você já tem uma missão ativa"

---

## 🟡 FASE 2: SISTEMA DE HERÓIS

### 2.1 Criação de Herói com Distribuição de Pontos
**Sistema:**
- Total de pontos: 20 iniciais
- Ao adicionar em STR, remove de DEX ou INT (equilíbrio)
- Nível inicial: SEMPRE 1
- XP inicial: 0

**UI:**
- Sliders interligados
- Mostrar pontos restantes
- Preview de atributos finais

### 2.2 Herói Sempre Começa Nível 1
**Backend:**
- Forçar `Level = 1` e `Experience = 0` na criação
- Remover qualquer lógica de nível inicial diferente

---

## 🟢 FASE 3: REFORMA DO COMBATE

### 3.1 Novo Fluxo de Combate
**Tela de Preparação:**
1. **Composição da Party** - Selecionar até 3 heróis ativos
2. **Descobertas** - Preview de inimigos, dificuldade, recompensas
3. **Confirmação** - "Iniciar Missão"

**Tela de Combate:**
1. **Visualização da Party** - Mostrar 3 heróis e seus stats combinados
2. **Inimigos** - Grid visual com HP, nível, tipo
3. **Rolagem de Dados** - Animação épica com resultado
4. **Log de Ações** - Timeline de acontecimentos
5. **Recompensas** - Explosão visual ao vencer

### 3.2 Botão "Ir para Missão"
**No Card de Quest Aceita:**
```tsx
{quest.isAccepted && !quest.isCompleted && (
  <Button onClick={() => navigate(`/combat?questId=${quest.id}`)}>
    ⚔️ Ir para Missão
  </Button>
)}
```

---

## 🔵 FASE 4: SISTEMA DE INVENTÁRIO

### 4.1 Equipar/Desequipar Itens
**Backend:**
- Endpoint: `PUT /api/v1/heroes/{id}/equipment`
- Body: `{ slot: "weapon", itemId: 5 }`
- Slots: weapon, armor, accessory

**Frontend:**
- Modal de inventário
- Arrastar e soltar
- Slots visuais (arma, armadura, acessório)
- Calcular stats automaticamente

### 4.2 Aplicar Bônus de Itens
**Lógica:**
- Itens equipados somam aos atributos base
- Recalcular ao equipar/desequipar
- Mostrar diferença com/sem item

---

## 🟣 FASE 5: LOJA DE DADOS

### 5.1 Mostrar Dados Disponíveis
**Após Compra:**
- Atualizar contador de dados
- Animação de "+1 Dado d6"
- Toast de sucesso

**Na UI:**
```tsx
<div className="dice-inventory">
  <span>🎲 D6: {inventory.d6Count}</span>
  <span>🎲 D8: {inventory.d8Count}</span>
  <span>🎲 D12: {inventory.d12Count}</span>
  <span>🎲 D20: {inventory.d20Count}</span>
</div>
```

---

## 📊 ORDEM DE IMPLEMENTAÇÃO

1. ✅ **Corrigir erro de combate** (10 min)
2. ✅ **Uma quest por vez** (15 min)
3. ✅ **Herói nível 1 + pontos limitados** (20 min)
4. ✅ **Botão "Ir para Missão"** (5 min)
5. ✅ **Mostrar dados na loja** (10 min)
6. ✅ **Sistema de inventário básico** (30 min)
7. ✅ **Reforma UI de combate** (45 min)

**TEMPO TOTAL:** ~2h15min

---

## 🎯 RESULTADO FINAL

### Experiência do Jogador:
1. **Criar herói** com distribuição estratégica (20 pontos)
2. **Aceitar UMA quest** por vez
3. **Ir para a missão** via botão
4. **Compor party** de até 3 heróis
5. **Ver preview** de inimigos e dificuldade
6. **Equipar itens** antes do combate
7. **Rolar dados** com animação épica
8. **Ganhar recompensas** visualizadas
9. **Equipar novos itens** no inventário
10. **Comprar dados** e ver quantidade atualizada

### Sensação:
- ✅ **Estratégia** - Distribuição de pontos e escolha de party
- ✅ **Emoção** - Rolagem de dados e combate
- ✅ **Dinamismo** - Fluxo rápido e visual
- ✅ **Progressão** - Equipamentos e níveis

---

🚀 **INICIANDO IMPLEMENTAÇÃO AGORA!**

