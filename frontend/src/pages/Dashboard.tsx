import { useEffect, useState } from 'react'
import { api } from '../services/api'

export default function Dashboard() {
  const [health, setHealth] = useState<string>('')
  useEffect(() => {
    api.get('/health').then(r => setHealth(r.data))
  }, [])
  return (
    <div style={{ padding: 20 }}>
      <h1>RPG Quest Manager</h1>
      <p>API: {health || '...'}</p>
    </div>
  )
}


