import { api } from './api'

export async function getBossesByEnvironment(env: string) {
  const { data } = await api.get(`/api/monsters/bosses-by-environment/${env}`)
  return data as Array<{ id: number; name: string; attack: number; defense: number; health: number }>
}


