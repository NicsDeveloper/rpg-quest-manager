import { useState } from 'react'
import { api } from '../services/api'

type CombatState = {
  hero: {
    id: number
    name: string
    level: number
    experience: number
    nextLevelExperience: number
    health: number
    maxHealth: number
    attack: number
    defense: number
    morale: number
    moraleLevel: string
  }
  monster: {
    id: number
    name: string
    type: string
    rank: string
    habitat: string
    health: number
    maxHealth: number
    attack: number
    defense: number
    experienceReward: number
  }
  combat: {
    damageToMonster: number
    damageToHero: number
    isCritical: boolean
    isFumble: boolean
    combatEnded: boolean
    victory: boolean
    experienceGained: number
    actionDescription: string
    appliedEffects: string[]
  }
}

export default function Combat() {
  const [combatState, setCombatState] = useState<CombatState | null>(null)
  const [log, setLog] = useState<string[]>([])
  const [loading, setLoading] = useState(false)
  const [combatStarted, setCombatStarted] = useState(false)

  const startCombat = async () => {
    setLoading(true)
    try {
      const { data } = await api.post('/api/combat/start', { characterId: 1, monsterId: 1 })
      setCombatState(data)
      setCombatStarted(true)
      setLog(prev => ['Combate iniciado!', ...prev])
    } finally {
      setLoading(false)
    }
  }

  const attack = async () => {
    if (!combatState) return
    
    setLoading(true)
    try {
      const { data } = await api.post('/api/combat/attack', { characterId: 1, monsterId: 1 })
      setCombatState(data)
      
      const logEntry = data.combat.actionDescription
      if (data.combat.combatEnded) {
        const result = data.combat.victory ? 'VITÓRIA!' : 'DERROTA!'
        setLog(prev => [`${result} ${logEntry}`, ...prev])
        setCombatStarted(false)
      } else {
        setLog(prev => [logEntry, ...prev])
      }
    } finally {
      setLoading(false)
    }
  }

  const tryEscape = async () => {
    if (!combatState) return
    
    setLoading(true)
    try {
      const { data } = await api.post('/api/combat/escape', { characterId: 1, monsterId: 1 })
      if (data.success) {
        setLog(prev => ['Fuga bem-sucedida!', ...prev])
        setCombatStarted(false)
      } else {
        setLog(prev => ['Fuga falhou!', ...prev])
      }
    } finally {
      setLoading(false)
    }
  }

  const getMoraleColor = (morale: number) => {
    if (morale <= 10) return '#ff4444' // Desespero
    if (morale <= 30) return '#ff8844' // Baixo
    if (morale <= 70) return '#ffff44' // Normal
    if (morale <= 90) return '#88ff44' // Alto
    return '#44ff44' // Inspirado
  }

  const getMoraleLevelName = (level: string) => {
    switch (level) {
      case 'Despair': return 'Desespero'
      case 'Low': return 'Baixo'
      case 'Normal': return 'Normal'
      case 'High': return 'Alto'
      case 'Inspired': return 'Inspirado'
      default: return level
    }
  }

  return (
    <div style={{ padding: 20 }}>
      <h2>Combate</h2>
      
      {!combatStarted ? (
        <div>
          <button onClick={startCombat} disabled={loading}>
            {loading ? 'Iniciando...' : 'Iniciar Combate'}
          </button>
        </div>
      ) : (
        <div>
          {combatState && (
            <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 20, marginBottom: 20 }}>
              {/* Herói */}
              <div style={{ border: '2px solid #4CAF50', padding: 15, borderRadius: 8 }}>
                <h3>🛡️ {combatState.hero.name}</h3>
                <div>Nível: {combatState.hero.level}</div>
                <div>XP: {combatState.hero.experience}/{combatState.hero.nextLevelExperience}</div>
                <div>
                  HP: {combatState.hero.health}/{combatState.hero.maxHealth}
                  <div style={{ 
                    width: '100%', 
                    height: 10, 
                    backgroundColor: '#ddd', 
                    borderRadius: 5,
                    marginTop: 5
                  }}>
                    <div style={{
                      width: `${(combatState.hero.health / combatState.hero.maxHealth) * 100}%`,
                      height: '100%',
                      backgroundColor: combatState.hero.health < 30 ? '#ff4444' : '#4CAF50',
                      borderRadius: 5
                    }} />
                  </div>
                </div>
                <div>
                  Moral: {combatState.hero.morale} ({getMoraleLevelName(combatState.hero.moraleLevel)})
                  <div style={{ 
                    width: '100%', 
                    height: 10, 
                    backgroundColor: '#ddd', 
                    borderRadius: 5,
                    marginTop: 5
                  }}>
                    <div style={{
                      width: `${combatState.hero.morale}%`,
                      height: '100%',
                      backgroundColor: getMoraleColor(combatState.hero.morale),
                      borderRadius: 5
                    }} />
                  </div>
                </div>
                <div>ATK: {combatState.hero.attack} | DEF: {combatState.hero.defense}</div>
              </div>

              {/* Monstro */}
              <div style={{ border: '2px solid #f44336', padding: 15, borderRadius: 8 }}>
                <h3>👹 {combatState.monster.name}</h3>
                <div>Tipo: {combatState.monster.type} | Rank: {combatState.monster.rank}</div>
                <div>Habitat: {combatState.monster.habitat}</div>
                <div>
                  HP: {combatState.monster.health}/{combatState.monster.maxHealth}
                  <div style={{ 
                    width: '100%', 
                    height: 10, 
                    backgroundColor: '#ddd', 
                    borderRadius: 5,
                    marginTop: 5
                  }}>
                    <div style={{
                      width: `${(combatState.monster.health / combatState.monster.maxHealth) * 100}%`,
                      height: '100%',
                      backgroundColor: combatState.monster.health < 30 ? '#ff4444' : '#f44336',
                      borderRadius: 5
                    }} />
                  </div>
                </div>
                <div>ATK: {combatState.monster.attack} | DEF: {combatState.monster.defense}</div>
                <div>XP: {combatState.monster.experienceReward}</div>
              </div>
            </div>
          )}

          {/* Ações */}
          <div style={{ display: 'flex', gap: 10, marginBottom: 20 }}>
            <button onClick={attack} disabled={loading || !combatState?.combat.combatEnded === false}>
              {loading ? 'Atacando...' : '⚔️ Atacar'}
            </button>
            <button onClick={tryEscape} disabled={loading || !combatState?.combat.combatEnded === false}>
              {loading ? 'Fugindo...' : '🏃 Fugir'}
            </button>
          </div>

          {/* Log de Combate */}
          <div style={{ border: '1px solid #ccc', padding: 10, borderRadius: 5, maxHeight: 200, overflowY: 'auto' }}>
            <h4>Log de Combate:</h4>
            {log.map((entry, index) => (
              <div key={index} style={{ 
                padding: 5, 
                backgroundColor: index === 0 ? '#e3f2fd' : 'transparent',
                borderRadius: 3,
                marginBottom: 2
              }}>
                {entry}
              </div>
            ))}
          </div>

          {/* Informações do último turno */}
          {combatState && (
            <div style={{ marginTop: 20, padding: 10, backgroundColor: '#f5f5f5', borderRadius: 5 }}>
              <h4>Último Turno:</h4>
              <div>Ação: {combatState.combat.actionDescription}</div>
              {combatState.combat.damageToMonster > 0 && (
                <div>Dano ao monstro: {combatState.combat.damageToMonster}</div>
              )}
              {combatState.combat.damageToHero > 0 && (
                <div>Dano recebido: {combatState.combat.damageToHero}</div>
              )}
              {combatState.combat.isCritical && <div style={{ color: '#ff6b35' }}>💥 CRÍTICO!</div>}
              {combatState.combat.isFumble && <div style={{ color: '#666' }}>💥 FALHA!</div>}
              {combatState.combat.experienceGained > 0 && (
                <div style={{ color: '#4CAF50' }}>+{combatState.combat.experienceGained} XP</div>
              )}
              {combatState.combat.appliedEffects.length > 0 && (
                <div>Efeitos aplicados: {combatState.combat.appliedEffects.join(', ')}</div>
              )}
            </div>
          )}
        </div>
      )}
    </div>
  )
}


