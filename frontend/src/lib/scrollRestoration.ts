import { useEffect, useLayoutEffect, useRef, type RefObject } from 'react'
import { useLocation, useNavigationType } from 'react-router'

/**
 * Rolagem de cada entrada do histórico (por location.key), para voltar ao mesmo ponto de uma página (um guia,
 * por exemplo). Quem rola é o <main> do Layout, não a janela, então a restauração do navegador não serve.
 */

const storageKey = 'traderock.scroll'
/** Entradas guardadas no máximo (as mais antigas saem). */
const maxEntries = 50

type Positions = Record<string, number>

/** Armazenamento mínimo (o sessionStorage, ou um dublê nos testes). */
export interface ScrollStorage {
  getItem(key: string): string | null
  setItem(key: string, value: string): void
}

const sessionStore = (): ScrollStorage | null => {
  try {
    return window.sessionStorage
  } catch {
    return null
  }
}

function readPositions(storage: ScrollStorage | null): Positions {
  try {
    const parsed: unknown = JSON.parse(storage?.getItem(storageKey) ?? '{}')
    return parsed && typeof parsed === 'object' && !Array.isArray(parsed) ? (parsed as Positions) : {}
  } catch {
    return {}
  }
}

/** Guarda a rolagem <top> da entrada <key>. Sem armazenamento (ou cheio), não guarda nada. */
export function saveScrollPosition(key: string, top: number, storage: ScrollStorage | null = sessionStore()) {
  const positions = readPositions(storage)
  delete positions[key]
  positions[key] = Math.max(0, Math.round(top))
  const keys = Object.keys(positions)
  for (const old of keys.slice(0, Math.max(0, keys.length - maxEntries))) delete positions[old]
  try {
    storage?.setItem(storageKey, JSON.stringify(positions))
  } catch {
    // sem armazenamento: só não volta ao mesmo ponto
  }
}

/** Rolagem guardada da entrada <key>, ou null. */
export function loadScrollPosition(key: string, storage: ScrollStorage | null = sessionStore()): number | null {
  const top = readPositions(storage)[key]
  return typeof top === 'number' && Number.isFinite(top) ? top : null
}

export type ScrollAction = { type: 'restore'; top: number } | { type: 'top' } | { type: 'keep' }

/**
 * O que fazer com a rolagem ao chegar numa entrada: voltar (POP) restaura o ponto guardado; ir para outra
 * tela (PUSH, ou POP sem ponto guardado) volta ao topo; trocar só a consulta (REPLACE, abas) não mexe.
 */
export function scrollAction(navigationType: 'POP' | 'PUSH' | 'REPLACE', pathChanged: boolean, saved: number | null): ScrollAction {
  if (navigationType === 'POP' && saved !== null) return { type: 'restore', top: saved }
  if (navigationType === 'REPLACE' || !pathChanged) return { type: 'keep' }
  return { type: 'top' }
}

/** Quadros (requestAnimationFrame) que a restauração espera o conteúdo crescer: uns 5 s. */
const maxFrames = 300

/**
 * Rola <el> até <top> assim que a página ficar alta o bastante (o guia e os blocos carregam depois). Para
 * no limite de quadros (rola até onde der) ou se a pessoa mexer na rolagem. Devolve o cancelamento.
 */
function restoreWhenReady(el: HTMLElement, top: number): () => void {
  let frame = 0
  let handle = 0
  let stopped = false
  const stop = () => {
    stopped = true
    cancelAnimationFrame(handle)
    for (const type of ['wheel', 'touchstart', 'keydown', 'mousedown'] as const) el.removeEventListener(type, stop)
  }
  for (const type of ['wheel', 'touchstart', 'keydown', 'mousedown'] as const) el.addEventListener(type, stop, { passive: true })
  const step = () => {
    if (stopped) return
    const tall = el.scrollHeight - el.clientHeight >= top
    if (tall || ++frame >= maxFrames) {
      el.scrollTop = top
      stop()
      return
    }
    handle = requestAnimationFrame(step)
  }
  step()
  return stop
}

/** Liga a restauração da rolagem a <ref> (o <main> do Layout). */
export function useScrollRestoration(ref: RefObject<HTMLElement | null>) {
  const location = useLocation()
  const navigationType = useNavigationType()
  const current = useRef<{ key: string; pathname: string; top: number } | null>(null)

  // Acompanha a rolagem da entrada atual (o valor de antes da troca de tela, que o conteúdo novo ainda não cortou).
  useEffect(() => {
    const el = ref.current
    if (!el) return
    const onScroll = () => {
      if (current.current) current.current.top = el.scrollTop
    }
    el.addEventListener('scroll', onScroll, { passive: true })
    const onHide = () => current.current && saveScrollPosition(current.current.key, current.current.top)
    window.addEventListener('pagehide', onHide)
    return () => {
      el.removeEventListener('scroll', onScroll)
      window.removeEventListener('pagehide', onHide)
    }
  }, [ref])

  useLayoutEffect(() => {
    const el = ref.current
    const previous = current.current
    if (previous) saveScrollPosition(previous.key, previous.top)
    current.current = { key: location.key, pathname: location.pathname, top: el?.scrollTop ?? 0 }
    if (!el) return
    const action = scrollAction(navigationType, previous?.pathname !== location.pathname, loadScrollPosition(location.key))
    if (action.type === 'top') {
      el.scrollTop = 0
      current.current.top = 0
    } else if (action.type === 'restore') {
      current.current.top = action.top
      return restoreWhenReady(el, action.top)
    }
  }, [ref, location.key, location.pathname, navigationType])
}
