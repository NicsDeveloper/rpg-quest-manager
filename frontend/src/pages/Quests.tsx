import { useEffect, useState } from 'react'
import { getAllQuests, getRecommendedQuests } from '../services/quests'

type Quest = {
  id: number
  title: string
  description: string
  environment: number
  experienceReward: number
  requiredLevel?: number
}

export default function Quests() {
  const [quests, setQuests] = useState<Quest[]>([])
  const [level, setLevel] = useState<number>(1)
  useEffect(() => {
    getAllQuests().then(setQuests)
  }, [])
  return (
    <div style={{ padding: 20 }}>
      <h2>Quests</h2>
      <div style={{ display: 'flex', gap: 8, alignItems: 'center' }}>
        <input type="number" value={level} min={1} onChange={e => setLevel(parseInt(e.target.value || '1'))} />
        <button onClick={async () => setQuests(await getRecommendedQuests(level))}>Recomendadas</button>
        <button onClick={async () => setQuests(await getAllQuests())}>Todas</button>
      </div>
      <ul>
        {quests.map(q => (
          <li key={q.id}>
            <strong>{q.title}</strong> — {q.description} — XP {q.experienceReward}
          </li>
        ))}
      </ul>
    </div>
  )
}


