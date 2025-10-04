import { api } from './api'

export async function getAllQuests() {
  const { data } = await api.get('/api/quests')
  return data
}

export async function getRecommendedQuests(level: number) {
  const { data } = await api.get(`/api/quests/recommended/${level}`)
  return data
}


