import { useSyncExternalStore } from 'react'
import { api } from './api'

/**
 * Mini janela do Contador Hunt (GET/PUT /api/app/mini-window): a janela do PC encolhe e fica sempre na frente.
 * `supported` é false no navegador, no celular e sem janela nativa.
 */
export interface MiniWindowState {
  supported: boolean
  on: boolean
  onMinimize: boolean
}

export interface MiniWindowChange {
  on?: boolean
  onMinimize?: boolean
  huntOpen?: boolean
}

const OFF: MiniWindowState = { supported: false, on: false, onMinimize: true }

let state = OFF
let loaded = false
const listeners = new Set<() => void>()

function set(next: MiniWindowState) {
  state = next
  for (const l of listeners) l()
}

function load() {
  if (loaded) return
  loaded = true
  api
    .get<MiniWindowState>('/api/app/mini-window')
    .then(set)
    .catch(() => undefined)
}

/** O backend avisou pela janela nativa que a mini janela ligou ou desligou (ex.: ao minimizar). */
export function miniWindowChanged(on: boolean) {
  set({ ...state, on })
}

/** Muda a mini janela, a preferência ou avisa que há uma hunt na tela. Sem janela, não faz nada. */
export async function changeMiniWindow(change: MiniWindowChange): Promise<void> {
  if (!state.supported) return
  try {
    set(await api.put<MiniWindowState>('/api/app/mini-window', change))
  } catch {
    // Sem janela nativa (ou pela rede): fica como está.
  }
}

export function useMiniWindow(): MiniWindowState {
  return useSyncExternalStore(
    (listener) => {
      listeners.add(listener)
      load()
      return () => listeners.delete(listener)
    },
    () => state,
  )
}

/** Só para os testes: volta ao estado inicial. */
export function resetMiniWindowForTests(next: MiniWindowState = OFF) {
  state = next
  loaded = true
}
