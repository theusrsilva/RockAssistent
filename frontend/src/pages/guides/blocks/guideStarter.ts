import { useEffect, useState, useSyncExternalStore } from 'react'
import { api } from '../../../lib/api'
import type { StarterInfo } from '../../nuzlocke/nuzlocke'

// Inicial escolhido nas abas "Se você escolheu X" dos guias: um por jogo, o mesmo para todos os chefes da página (e
// lembrado no localStorage). A lista de iniciais é pedida uma vez por jogo.

const storageKey = (game: string) => `traderock.guides.starter.${game}`
const listeners = new Set<() => void>()
const memory = new Map<string, string | null>()

/** A escolha guardada do jogo (memória, senão localStorage). */
export function readGuideStarter(game: string): string | null {
  if (memory.has(game)) return memory.get(game) ?? null
  let value: string | null = null
  try {
    value = localStorage.getItem(storageKey(game))
  } catch {
    // Sem localStorage (janela privada, preview): fica só na memória.
  }
  memory.set(game, value)
  return value
}

/** Guarda a escolha do jogo e avisa todas as tabelas e cartões abertos. */
export function chooseGuideStarter(game: string, starter: string) {
  memory.set(game, starter)
  try {
    localStorage.setItem(storageKey(game), starter)
  } catch {
    // idem
  }
  listeners.forEach((l) => l())
}

function subscribe(listener: () => void) {
  listeners.add(listener)
  return () => listeners.delete(listener)
}

/** O inicial escolhido para os guias do jogo (null = nenhum ainda: vale a primeira variante de cada chefe). */
export function useGuideStarter(game: string): [string | null, (starter: string) => void] {
  const value = useSyncExternalStore(subscribe, () => readGuideStarter(game), () => null)
  return [value, (starter) => chooseGuideStarter(game, starter)]
}

const starterLists = new Map<string, Promise<StarterInfo[]>>()

/** Iniciais do jogo, pedidos uma vez por jogo (vazio enquanto carrega, com erro ou com <enabled> falso). */
export function useGuideStarters(game: string, enabled: boolean): StarterInfo[] {
  const [loaded, setLoaded] = useState<{ game: string; list: StarterInfo[] } | null>(null)
  useEffect(() => {
    if (!enabled) return
    let current = true
    let request = starterLists.get(game)
    if (!request) {
      request = api.get<StarterInfo[]>(`/api/data/${encodeURIComponent(game)}/starters`)
      request.catch(() => starterLists.delete(game))
      starterLists.set(game, request)
    }
    request.then((list) => current && setLoaded({ game, list })).catch(() => undefined)
    return () => {
      current = false
    }
  }, [game, enabled])
  return enabled && loaded?.game === game ? loaded.list : []
}
