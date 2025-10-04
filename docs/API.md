# 📚 Documentação da API - RPG Quest Manager

## 📋 Índice

- [Visão Geral](#-visão-geral)
- [Autenticação](#-autenticação)
- [Endpoints](#-endpoints)
- [Modelos de Dados](#-modelos-de-dados)
- [Códigos de Status](#-códigos-de-status)
- [Tratamento de Erros](#-tratamento-de-erros)
- [Rate Limiting](#-rate-limiting)
- [Exemplos](#-exemplos)

## 🎯 Visão Geral

A API do RPG Quest Manager é uma API REST que fornece endpoints para gerenciar todos os aspectos do jogo, incluindo autenticação, personagens, combate, missões, inventário e muito mais.

### Base URL
```
http://localhost:5000/api
```

### Formato de Resposta
Todas as respostas são em formato JSON.

### Headers Obrigatórios
```http
Content-Type: application/json
Authorization: Bearer <token>
```

## 🔐 Autenticação

### JWT Token
A API usa JWT (JSON Web Tokens) para autenticação. O token deve ser incluído no header `Authorization` de todas as requisições autenticadas.

### Endpoints de Autenticação

#### POST /api/auth/register
Registra um novo usuário.

**Request:**
```json
{
  "username": "string",
  "email": "string",
  "password": "string"
}
```

**Response:**
```json
{
  "user": {
    "id": 1,
    "username": "string",
    "email": "string",
    "createdAt": "2024-01-01T00:00:00Z"
  },
  "token": "jwt-token-here"
}
```

#### POST /api/auth/login
Autentica um usuário existente.

**Request:**
```json
{
  "username": "string",
  "password": "string"
}
```

**Response:**
```json
{
  "user": {
    "id": 1,
    "username": "string",
    "email": "string",
    "createdAt": "2024-01-01T00:00:00Z"
  },
  "token": "jwt-token-here"
}
```

#### POST /api/auth/validate
Valida um token JWT.

**Response:**
```json
{
  "user": {
    "id": 1,
    "username": "string",
    "email": "string",
    "createdAt": "2024-01-01T00:00:00Z"
  },
  "valid": true
}
```

#### POST /api/auth/logout
Invalida o token atual.

**Response:**
```json
{
  "message": "Logout successful"
}
```

## 🎮 Endpoints

### Personagens

#### GET /api/characters/{id}
Obtém dados de um personagem.

**Response:**
```json
{
  "id": 1,
  "name": "string",
  "level": 1,
  "experience": 0,
  "nextLevelExperience": 100,
  "health": 100,
  "maxHealth": 100,
  "attack": 10,
  "defense": 5,
  "morale": 50,
  "gold": 100,
  "userId": 1,
  "createdAt": "2024-01-01T00:00:00Z"
}
```

#### PUT /api/characters/{id}
Atualiza dados de um personagem.

**Request:**
```json
{
  "name": "string",
  "level": 1,
  "experience": 0,
  "health": 100,
  "maxHealth": 100,
  "attack": 10,
  "defense": 5,
  "morale": 50,
  "gold": 100
}
```

#### GET /api/characters/{id}/stats
Obtém estatísticas detalhadas do personagem.

**Response:**
```json
{
  "character": {
    "id": 1,
    "name": "string",
    "level": 1,
    "experience": 0,
    "nextLevelExperience": 100,
    "health": 100,
    "maxHealth": 100,
    "attack": 10,
    "defense": 5,
    "morale": 50,
    "gold": 100
  },
  "equipment": {
    "weapon": null,
    "shield": null,
    "helmet": null,
    "armor": null,
    "gloves": null,
    "boots": null,
    "ring": null,
    "amulet": null
  },
  "bonuses": {
    "attackBonus": 0,
    "defenseBonus": 0,
    "healthBonus": 0,
    "moraleBonus": 0
  }
}
```

### Combate

#### POST /api/combat/start
Inicia um combate.

**Request:**
```json
{
  "characterId": 1,
  "monsterId": 1
}
```

**Response:**
```json
{
  "hero": {
    "id": 1,
    "name": "string",
    "level": 1,
    "experience": 0,
    "nextLevelExperience": 100,
    "health": 100,
    "maxHealth": 100,
    "attack": 10,
    "defense": 5,
    "morale": 50,
    "moraleLevel": "Normal"
  },
  "monster": {
    "id": 1,
    "name": "string",
    "type": "string",
    "rank": "string",
    "habitat": "string",
    "health": 50,
    "maxHealth": 50,
    "attack": 8,
    "defense": 3,
    "experienceReward": 25
  },
  "combat": {
    "damageToMonster": 0,
    "damageToHero": 0,
    "isCritical": false,
    "isFumble": false,
    "combatEnded": false,
    "victory": false,
    "experienceGained": 0,
    "actionDescription": "Combate iniciado!",
    "appliedEffects": []
  }
}
```

#### POST /api/combat/attack
Executa um ataque.

**Request:**
```json
{
  "characterId": 1,
  "monsterId": 1
}
```

**Response:**
```json
{
  "hero": {
    "id": 1,
    "name": "string",
    "level": 1,
    "experience": 0,
    "nextLevelExperience": 100,
    "health": 95,
    "maxHealth": 100,
    "attack": 10,
    "defense": 5,
    "morale": 45,
    "moraleLevel": "Normal"
  },
  "monster": {
    "id": 1,
    "name": "string",
    "type": "string",
    "rank": "string",
    "habitat": "string",
    "health": 35,
    "maxHealth": 50,
    "attack": 8,
    "defense": 3,
    "experienceReward": 25
  },
  "combat": {
    "damageToMonster": 15,
    "damageToHero": 5,
    "isCritical": false,
    "isFumble": false,
    "combatEnded": false,
    "victory": false,
    "experienceGained": 0,
    "actionDescription": "Você atacou o monstro causando 15 de dano!",
    "appliedEffects": []
  }
}
```

#### POST /api/combat/ability
Usa uma habilidade especial.

**Request:**
```json
{
  "characterId": 1,
  "monsterId": 1,
  "abilityId": 1
}
```

#### POST /api/combat/item
Usa um item durante o combate.

**Request:**
```json
{
  "characterId": 1,
  "monsterId": 1,
  "itemName": "string"
}
```

#### POST /api/combat/escape
Tenta fugir do combate.

**Request:**
```json
{
  "characterId": 1,
  "monsterId": 1
}
```

**Response:**
```json
{
  "success": true,
  "message": "Fuga bem-sucedida!"
}
```

### Missões

#### GET /api/quests
Obtém todas as missões.

**Response:**
```json
[
  {
    "id": 1,
    "title": "string",
    "description": "string",
    "environment": 0,
    "experienceReward": 100,
    "requiredLevel": 1,
    "difficulty": "Easy",
    "category": "Main",
    "status": "Available",
    "progress": 0,
    "goldReward": 50
  }
]
```

#### GET /api/quests/recommended/{level}
Obtém missões recomendadas para um nível.

**Response:**
```json
[
  {
    "id": 1,
    "title": "string",
    "description": "string",
    "environment": 0,
    "experienceReward": 100,
    "requiredLevel": 1,
    "difficulty": "Easy",
    "category": "Main",
    "status": "Available",
    "progress": 0,
    "goldReward": 50
  }
]
```

#### GET /api/quests/available/{characterId}
Obtém missões disponíveis para um personagem.

#### GET /api/quests/completed/{characterId}
Obtém missões completadas por um personagem.

#### POST /api/quests/start
Inicia uma missão.

**Request:**
```json
{
  "characterId": 1,
  "questId": 1
}
```

#### POST /api/quests/complete
Completa uma missão.

**Request:**
```json
{
  "characterId": 1,
  "questId": 1
}
```

#### POST /api/quests/fail
Falha uma missão.

**Request:**
```json
{
  "characterId": 1,
  "questId": 1
}
```

### Inventário

#### GET /api/inventory/{characterId}
Obtém inventário de um personagem.

**Response:**
```json
[
  {
    "id": 1,
    "name": "string",
    "description": "string",
    "type": "Weapon",
    "rarity": "Common",
    "level": 1,
    "quantity": 1,
    "isEquipped": false,
    "equippedSlot": null,
    "isConsumable": false,
    "attackBonus": 5,
    "defenseBonus": 0,
    "healthBonus": 0,
    "moraleBonus": 0
  }
]
```

#### POST /api/inventory/add
Adiciona item ao inventário.

**Request:**
```json
{
  "characterId": 1,
  "itemId": 1,
  "quantity": 1
}
```

#### POST /api/inventory/remove
Remove item do inventário.

**Request:**
```json
{
  "characterId": 1,
  "itemId": 1,
  "quantity": 1
}
```

#### POST /api/inventory/equip
Equipa um item.

**Request:**
```json
{
  "characterId": 1,
  "itemId": 1,
  "slot": "Weapon"
}
```

#### POST /api/inventory/unequip
Desequipa um item.

**Request:**
```json
{
  "characterId": 1,
  "slot": "Weapon"
}
```

#### POST /api/inventory/use
Usa um item consumível.

**Request:**
```json
{
  "characterId": 1,
  "itemId": 1
}
```

#### GET /api/inventory/{characterId}/equipment
Obtém equipamentos de um personagem.

**Response:**
```json
{
  "weapon": {
    "id": 1,
    "name": "string",
    "type": "Weapon",
    "rarity": "Common",
    "level": 1,
    "attackBonus": 5,
    "defenseBonus": 0,
    "healthBonus": 0,
    "moraleBonus": 0
  },
  "shield": null,
  "helmet": null,
  "armor": null,
  "gloves": null,
  "boots": null,
  "ring": null,
  "amulet": null
}
```

#### GET /api/inventory/{characterId}/bonuses
Obtém bônus de equipamentos.

**Response:**
```json
{
  "attackBonus": 5,
  "defenseBonus": 3,
  "healthBonus": 20,
  "moraleBonus": 10
}
```

### Loja

#### GET /api/shop
Obtém informações da loja.

**Response:**
```json
{
  "shopType": "general",
  "description": "Loja geral de equipamentos",
  "items": [
    {
      "id": 1,
      "name": "string",
      "description": "string",
      "type": "Weapon",
      "rarity": "Common",
      "level": 1,
      "shopPrice": 100,
      "attackBonus": 5,
      "defenseBonus": 0,
      "healthBonus": 0,
      "moraleBonus": 0
    }
  ]
}
```

#### GET /api/shop/items
Obtém itens da loja.

#### GET /api/shop/types
Obtém tipos de loja disponíveis.

**Response:**
```json
{
  "shopTypes": [
    {
      "type": "general",
      "description": "Loja geral de equipamentos"
    },
    {
      "type": "weapons",
      "description": "Loja especializada em armas"
    }
  ]
}
```

#### GET /api/shop/rarities
Obtém raridades disponíveis.

**Response:**
```json
{
  "rarities": [
    {
      "rarity": "Common",
      "description": "Item comum"
    },
    {
      "rarity": "Uncommon",
      "description": "Item incomum"
    }
  ]
}
```

#### POST /api/shop/buy
Compra um item.

**Request:**
```json
{
  "characterId": 1,
  "itemId": 1,
  "quantity": 1
}
```

#### POST /api/shop/sell
Vende um item.

**Request:**
```json
{
  "characterId": 1,
  "itemId": 1,
  "quantity": 1
}
```

### Conquistas

#### GET /api/achievements
Obtém todas as conquistas.

**Response:**
```json
[
  {
    "id": 1,
    "name": "string",
    "description": "string",
    "type": "Combat",
    "category": "Kills",
    "requiredValue": 10,
    "experienceReward": 100,
    "goldReward": 50,
    "itemRewardId": null,
    "isSecret": false,
    "createdAt": "2024-01-01T00:00:00Z"
  }
]
```

#### GET /api/achievements/user/{userId}
Obtém conquistas de um usuário.

**Response:**
```json
[
  {
    "id": 1,
    "userId": 1,
    "achievementId": 1,
    "progress": 5,
    "isCompleted": false,
    "completedAt": null,
    "claimedAt": null,
    "achievement": {
      "id": 1,
      "name": "string",
      "description": "string",
      "type": "Combat",
      "category": "Kills",
      "requiredValue": 10,
      "experienceReward": 100,
      "goldReward": 50,
      "itemRewardId": null,
      "isSecret": false
    }
  }
]
```

#### POST /api/achievements/claim
Reivindica recompensa de uma conquista.

**Request:**
```json
{
  "userId": 1,
  "achievementId": 1
}
```

### Grupos

#### POST /api/parties
Cria um novo grupo.

**Request:**
```json
{
  "name": "string",
  "description": "string",
  "isPublic": true
}
```

**Response:**
```json
{
  "id": 1,
  "name": "string",
  "description": "string",
  "leaderId": 1,
  "isPublic": true,
  "createdAt": "2024-01-01T00:00:00Z",
  "members": [
    {
      "id": 1,
      "partyId": 1,
      "userId": 1,
      "characterId": 1,
      "role": "Leader",
      "joinedAt": "2024-01-01T00:00:00Z",
      "user": {
        "id": 1,
        "username": "string",
        "email": "string"
      },
      "character": {
        "id": 1,
        "name": "string",
        "level": 1
      }
    }
  ]
}
```

#### GET /api/parties/{id}
Obtém dados de um grupo.

#### POST /api/parties/join
Entra em um grupo.

**Request:**
```json
{
  "partyId": 1,
  "characterId": 1
}
```

#### POST /api/parties/leave
Sai de um grupo.

**Request:**
```json
{
  "partyId": 1,
  "userId": 1
}
```

#### POST /api/parties/invite
Convida um usuário para o grupo.

**Request:**
```json
{
  "partyId": 1,
  "inviterId": 1,
  "inviteeId": 2
}
```

#### POST /api/parties/accept
Aceita um convite.

**Request:**
```json
{
  "inviteId": 1,
  "characterId": 1
}
```

#### POST /api/parties/reject
Rejeita um convite.

**Request:**
```json
{
  "inviteId": 1
}
```

#### POST /api/parties/kick
Remove um membro do grupo.

**Request:**
```json
{
  "partyId": 1,
  "leaderId": 1,
  "memberId": 2
}
```

#### POST /api/parties/transfer
Transfere liderança do grupo.

**Request:**
```json
{
  "partyId": 1,
  "currentLeaderId": 1,
  "newLeaderId": 2
}
```

#### POST /api/parties/dissolve
Dissolve um grupo.

**Request:**
```json
{
  "partyId": 1,
  "leaderId": 1
}
```

### Habilidades Especiais

#### GET /api/abilities
Obtém todas as habilidades especiais.

**Response:**
```json
[
  {
    "id": 1,
    "name": "string",
    "description": "string",
    "type": "Attack",
    "category": "Combat",
    "requiredLevel": 5,
    "manaCost": 10,
    "cooldownTurns": 3,
    "damage": 20,
    "healing": 0,
    "statusEffects": ["Stun"],
    "statusEffectsToRemove": [],
    "experienceCost": 100,
    "goldCost": 50
  }
]
```

#### GET /api/abilities/character/{characterId}
Obtém habilidades de um personagem.

**Response:**
```json
[
  {
    "id": 1,
    "characterId": 1,
    "abilityId": 1,
    "isEquipped": true,
    "learnedAt": "2024-01-01T00:00:00Z",
    "ability": {
      "id": 1,
      "name": "string",
      "description": "string",
      "type": "Attack",
      "category": "Combat",
      "requiredLevel": 5,
      "manaCost": 10,
      "cooldownTurns": 3,
      "damage": 20,
      "healing": 0,
      "statusEffects": ["Stun"],
      "statusEffectsToRemove": [],
      "experienceCost": 100,
      "goldCost": 50
    }
  }
]
```

#### POST /api/abilities/learn
Aprende uma nova habilidade.

**Request:**
```json
{
  "characterId": 1,
  "abilityId": 1
}
```

#### POST /api/abilities/equip
Equipa uma habilidade.

**Request:**
```json
{
  "characterId": 1,
  "abilityId": 1
}
```

#### POST /api/abilities/unequip
Desequipa uma habilidade.

**Request:**
```json
{
  "characterId": 1,
  "abilityId": 1
}
```

#### POST /api/abilities/use
Usa uma habilidade.

**Request:**
```json
{
  "characterId": 1,
  "abilityId": 1,
  "targetId": 1
}
```

### Combos

#### GET /api/combos
Obtém todos os combos.

**Response:**
```json
[
  {
    "id": 1,
    "name": "string",
    "description": "string",
    "requiredLevel": 10,
    "experienceCost": 500,
    "goldCost": 200,
    "steps": [
      {
        "id": 1,
        "comboId": 1,
        "abilityId": 1,
        "stepOrder": 1,
        "ability": {
          "id": 1,
          "name": "string",
          "description": "string"
        }
      }
    ]
  }
]
```

#### GET /api/combos/character/{characterId}
Obtém combos de um personagem.

#### POST /api/combos/learn
Aprende um novo combo.

**Request:**
```json
{
  "characterId": 1,
  "comboId": 1
}
```

#### POST /api/combos/execute
Executa um passo de combo.

**Request:**
```json
{
  "characterId": 1,
  "comboId": 1,
  "stepOrder": 1,
  "targetId": 1
}
```

### Notificações

#### GET /api/notifications
Obtém notificações do usuário.

**Response:**
```json
[
  {
    "id": 1,
    "userId": 1,
    "type": "Achievement",
    "message": "string",
    "isRead": false,
    "createdAt": "2024-01-01T00:00:00Z"
  }
]
```

#### POST /api/notifications/{id}/read
Marca uma notificação como lida.

## 📊 Modelos de Dados

### User
```json
{
  "id": 1,
  "username": "string",
  "email": "string",
  "createdAt": "2024-01-01T00:00:00Z"
}
```

### Character
```json
{
  "id": 1,
  "name": "string",
  "level": 1,
  "experience": 0,
  "nextLevelExperience": 100,
  "health": 100,
  "maxHealth": 100,
  "attack": 10,
  "defense": 5,
  "morale": 50,
  "gold": 100,
  "userId": 1,
  "createdAt": "2024-01-01T00:00:00Z"
}
```

### Monster
```json
{
  "id": 1,
  "name": "string",
  "type": "string",
  "rank": "string",
  "habitat": "string",
  "health": 50,
  "maxHealth": 50,
  "attack": 8,
  "defense": 3,
  "experienceReward": 25
}
```

### Quest
```json
{
  "id": 1,
  "title": "string",
  "description": "string",
  "environment": 0,
  "experienceReward": 100,
  "requiredLevel": 1,
  "difficulty": "Easy",
  "category": "Main",
  "status": "Available",
  "progress": 0,
  "goldReward": 50
}
```

### Item
```json
{
  "id": 1,
  "name": "string",
  "description": "string",
  "type": "Weapon",
  "rarity": "Common",
  "level": 1,
  "attackBonus": 5,
  "defenseBonus": 0,
  "healthBonus": 0,
  "moraleBonus": 0
}
```

### Achievement
```json
{
  "id": 1,
  "name": "string",
  "description": "string",
  "type": "Combat",
  "category": "Kills",
  "requiredValue": 10,
  "experienceReward": 100,
  "goldReward": 50,
  "itemRewardId": null,
  "isSecret": false,
  "createdAt": "2024-01-01T00:00:00Z"
}
```

### Party
```json
{
  "id": 1,
  "name": "string",
  "description": "string",
  "leaderId": 1,
  "isPublic": true,
  "createdAt": "2024-01-01T00:00:00Z"
}
```

### SpecialAbility
```json
{
  "id": 1,
  "name": "string",
  "description": "string",
  "type": "Attack",
  "category": "Combat",
  "requiredLevel": 5,
  "manaCost": 10,
  "cooldownTurns": 3,
  "damage": 20,
  "healing": 0,
  "statusEffects": ["Stun"],
  "statusEffectsToRemove": [],
  "experienceCost": 100,
  "goldCost": 50
}
```

### Notification
```json
{
  "id": 1,
  "userId": 1,
  "type": "Achievement",
  "message": "string",
  "isRead": false,
  "createdAt": "2024-01-01T00:00:00Z"
}
```

## 📈 Códigos de Status

### Sucesso
- **200 OK** - Requisição bem-sucedida
- **201 Created** - Recurso criado com sucesso
- **204 No Content** - Requisição bem-sucedida sem conteúdo

### Erro do Cliente
- **400 Bad Request** - Requisição inválida
- **401 Unauthorized** - Não autenticado
- **403 Forbidden** - Não autorizado
- **404 Not Found** - Recurso não encontrado
- **409 Conflict** - Conflito de dados
- **422 Unprocessable Entity** - Dados inválidos

### Erro do Servidor
- **500 Internal Server Error** - Erro interno do servidor
- **502 Bad Gateway** - Erro de gateway
- **503 Service Unavailable** - Serviço indisponível

## ⚠️ Tratamento de Erros

### Formato de Erro
```json
{
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Dados inválidos",
    "details": [
      {
        "field": "email",
        "message": "Email é obrigatório"
      }
    ]
  }
}
```

### Códigos de Erro Comuns
- **VALIDATION_ERROR** - Erro de validação
- **AUTHENTICATION_ERROR** - Erro de autenticação
- **AUTHORIZATION_ERROR** - Erro de autorização
- **NOT_FOUND** - Recurso não encontrado
- **CONFLICT** - Conflito de dados
- **INTERNAL_ERROR** - Erro interno

## 🚦 Rate Limiting

### Limites
- **100 requests/minuto** por IP
- **1000 requests/hora** por usuário autenticado

### Headers de Rate Limit
```http
X-RateLimit-Limit: 100
X-RateLimit-Remaining: 95
X-RateLimit-Reset: 1640995200
```

### Resposta de Rate Limit
```json
{
  "error": {
    "code": "RATE_LIMIT_EXCEEDED",
    "message": "Rate limit exceeded",
    "retryAfter": 60
  }
}
```

## 💡 Exemplos

### Exemplo Completo de Combate
```javascript
// 1. Iniciar combate
const startResponse = await fetch('/api/combat/start', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    'Authorization': 'Bearer ' + token
  },
  body: JSON.stringify({
    characterId: 1,
    monsterId: 1
  })
});

const combatState = await startResponse.json();

// 2. Atacar
const attackResponse = await fetch('/api/combat/attack', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    'Authorization': 'Bearer ' + token
  },
  body: JSON.stringify({
    characterId: 1,
    monsterId: 1
  })
});

const attackResult = await attackResponse.json();

// 3. Verificar se combate terminou
if (attackResult.combat.combatEnded) {
  if (attackResult.combat.victory) {
    console.log('Vitória! +' + attackResult.combat.experienceGained + ' XP');
  } else {
    console.log('Derrota!');
  }
}
```

### Exemplo de Gerenciamento de Inventário
```javascript
// 1. Obter inventário
const inventoryResponse = await fetch('/api/inventory/1', {
  headers: {
    'Authorization': 'Bearer ' + token
  }
});

const inventory = await inventoryResponse.json();

// 2. Equipar item
const equipResponse = await fetch('/api/inventory/equip', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    'Authorization': 'Bearer ' + token
  },
  body: JSON.stringify({
    characterId: 1,
    itemId: 1,
    slot: 'Weapon'
  })
});

// 3. Usar item consumível
const useResponse = await fetch('/api/inventory/use', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    'Authorization': 'Bearer ' + token
  },
  body: JSON.stringify({
    characterId: 1,
    itemId: 2
  })
});
```

### Exemplo de Sistema de Conquistas
```javascript
// 1. Obter conquistas do usuário
const achievementsResponse = await fetch('/api/achievements/user/1', {
  headers: {
    'Authorization': 'Bearer ' + token
  }
});

const userAchievements = await achievementsResponse.json();

// 2. Reivindicar recompensa
const claimResponse = await fetch('/api/achievements/claim', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    'Authorization': 'Bearer ' + token
  },
  body: JSON.stringify({
    userId: 1,
    achievementId: 1
  })
});
```

---

**Esta documentação é atualizada regularmente. Para dúvidas, consulte o README.md ou abra uma issue no GitHub.**
