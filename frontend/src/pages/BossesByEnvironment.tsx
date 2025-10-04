import { useEffect, useState } from 'react'
import { getBossesByEnvironment } from '../services/monsters'

const environments = ['Forest','Desert','Dungeon','Castle','Volcano','Swamp','Tundra','Sky','Ruins','Temple','Crypt']

export default function BossesByEnvironment() {
  const [env, setEnv] = useState<string>('Forest')
  const [bosses, setBosses] = useState<any[]>([])
  useEffect(() => {
    getBossesByEnvironment(env).then(setBosses)
  }, [env])
  return (
    <div style={{ padding: 20 }}>
      <h2>Bosses por Ambiente</h2>
      <select value={env} onChange={e => setEnv(e.target.value)}>
        {environments.map(e => (
          <option key={e} value={e}>{e}</option>
        ))}
      </select>
      <ul>
        {bosses.map(b => (
          <li key={b.id}><strong>{b.name}</strong> — HP {b.health} ATK {b.attack} DEF {b.defense}</li>
        ))}
      </ul>
    </div>
  )
}


