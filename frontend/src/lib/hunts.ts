// Contador Hunt: tipos espelho de /api/hunts (TradeRock.Core/Hunts/HuntStore.cs), a fila de incrementos e contas puras.

import { useEffect, useMemo, useRef, useState } from 'react'
import type { TFunction } from 'i18next'
import { api, ApiError, csrfHeaders } from './api'
import type { Option } from './types'

/** Hunt como a API devolve. <modifiers>: id → "true"/"false" (toggle) ou número em texto (steps). */
export interface Hunt {
  id: string
  species: number
  form: number
  gameKey: string
  methodId: string
  methodName: string
  methodNameEn: string
  modifiers: Record<string, string>
  count: number
  elapsedSeconds: number
  startedAt: string
  endedAt: string | null
  found: boolean
  notes: string | null
  /** N de "1 em N" com o contador atual. */
  odds: number
  /** Chance acumulada (0..1). */
  cumulative: number
}

export interface ShinyStep {
  label: string
  min: number
  max: number | null
  odds: number
  rolls: number | null
}

export interface ShinyModifier {
  id: string
  name: string
  nameEn: string
  kind: 'toggle' | 'steps'
  steps: ShinyStep[] | null
  odds: number | null
  rolls: number | null
  /** Degrau guiado pelo contador da hunt (corrente, combo...): não é um campo. */
  counter: boolean
  games: string[] | null
}

export interface HuntMethod {
  id: string
  name: string
  nameEn: string
  baseOdds: number
  modifiers: ShinyModifier[]
}

/** Nome do método ou modificador de shiny no idioma da interface (os dados têm só pt e en). */
export function shinyName(item: { name: string; nameEn: string }, language: string): string {
  return language.startsWith('en') ? item.nameEn : item.name
}

/** Nome do método da hunt no idioma da interface. */
export function huntMethodName(hunt: Pick<Hunt, 'methodName' | 'methodNameEn'>, language: string): string {
  return language.startsWith('en') ? hunt.methodNameEn : hunt.methodName
}

export type HuntKind = 'encounter' | 'static' | 'gift' | 'egg' | 'evolution'

export interface HuntGame {
  gameKey: string
  name: string
  kinds: HuntKind[]
  methods: HuntMethod[]
}

/** Erros com texto próprio (o resto vira "common.error"). */
export const HUNT_ERRORS = [
  'unknown_game',
  'unknown_method',
  'method_not_in_game',
  'shiny_locked',
  'invalid_species',
  'invalid_form',
  'unknown_modifier',
  'invalid_modifier',
  'modifier_not_in_game',
  'invalid_delta',
  'invalid_count',
  'invalid_elapsed',
  'notes_too_long',
  'hunt_not_found',
  'hunt_finished',
] as const

export function huntErrorMessage(t: TFunction, e: unknown): string {
  const code = e instanceof ApiError ? e.code : null
  return code && (HUNT_ERRORS as readonly string[]).includes(code) ? t(`hunt.errors.${code}`) : t('common.error')
}

// ---- Fila de incrementos ----

export interface IncrementQueue {
  /** Soma <delta> (negativo diminui). Nunca há dois POST …/increment ao mesmo tempo. */
  increment: (delta: number) => void
  /** Soma ainda não confirmada pelo servidor (na fila ou em voo). */
  pending: () => number
  /** Resolve quando não há nada em voo nem na fila. */
  idle: () => Promise<void>
}

/** O servidor aceita ±1..±100 por pedido. */
export const MAX_DELTA = 100

/**
 * Um pedido por vez: enquanto um incremento está em voo, os seguintes se somam e saem juntos no próximo pedido
 * (+1 −1 se anulam e não saem). Usada pelos botões, pelo atalho da janela e (Tarefa 21) pelo atalho global.
 * Em erro, o que estava na fila é descartado e <onError> recebe o erro (o contador volta ao valor do servidor).
 */
export function createIncrementQueue<T>(
  send: (delta: number) => Promise<T>,
  handlers: { onResult: (result: T) => void; onError: (error: unknown) => void; onPending?: (pending: number) => void },
): IncrementQueue {
  let queued = 0
  let inFlight = 0
  let busy = false
  let waiters: (() => void)[] = []

  const notify = () => handlers.onPending?.(queued + inFlight)
  const settle = () => {
    if (busy || queued !== 0) return
    const done = waiters
    waiters = []
    done.forEach((w) => w())
  }

  const pump = () => {
    if (busy || queued === 0) return settle()
    const delta = Math.max(-MAX_DELTA, Math.min(MAX_DELTA, queued))
    queued -= delta
    inFlight = delta
    busy = true
    send(delta).then(
      (result) => {
        busy = false
        inFlight = 0
        handlers.onResult(result)
        notify()
        pump()
      },
      (error: unknown) => {
        busy = false
        inFlight = 0
        queued = 0
        notify()
        handlers.onError(error)
        settle()
      },
    )
  }

  return {
    increment(delta) {
      if (!Number.isFinite(delta) || delta === 0) return
      queued += Math.trunc(delta)
      notify()
      pump()
    },
    pending: () => queued + inFlight,
    idle: () => (busy || queued !== 0 ? new Promise<void>((resolve) => waiters.push(resolve)) : Promise.resolve()),
  }
}

/**
 * Fila de incrementos de uma hunt (POST /api/hunts/{id}/increment). <increment> é estável enquanto a hunt é a mesma:
 * os botões, o atalho da janela e o atalho global (Tarefa 21) chamam a mesma função. <pending> é a soma ainda não
 * confirmada (o número na tela é hunt.count + pending).
 */
export function useHuntIncrements(huntId: string | null, onHunt: (hunt: Hunt) => void, onError: (error: unknown) => void) {
  const latest = useRef({ onHunt, onError })
  useEffect(() => {
    latest.current = { onHunt, onError }
  })
  const [pending, setPending] = useState(0)
  const queue = useMemo(
    () =>
      huntId
        ? createIncrementQueue((delta) => api.post<Hunt>(`/api/hunts/${huntId}/increment`, { delta }), {
            onResult: (hunt) => latest.current.onHunt(hunt),
            onError: (error) => latest.current.onError(error),
            onPending: setPending,
          })
        : null,
    [huntId],
  )
  return {
    increment: queue?.increment ?? (() => undefined),
    idle: queue?.idle ?? (() => Promise.resolve()),
    pending: queue ? pending : 0,
  }
}

// ---- Atalho global (GET /api/hunts/events, só na janela do PC) ----

/** O pedaço do EventSource que o atalho global usa (dá para testar sem navegador). */
export interface HotkeyEventSource {
  onmessage: ((e: MessageEvent) => void) | null
  close: () => void
}

/**
 * Liga as teclas do atalho global ao contador: {type:"hotkey", id:"increment"|"decrement"} → increment(+1/−1), só
 * quando <allowed>() (hunt em andamento e sem janela modal). Devolve a função que desliga.
 */
export function subscribeHotkeyEvents(
  source: HotkeyEventSource,
  increment: (delta: number) => void,
  allowed: () => boolean,
): () => void {
  source.onmessage = (e) => {
    let event: { type?: string; id?: string }
    try {
      event = JSON.parse(e.data) as { type?: string; id?: string }
    } catch {
      return
    }
    if (event.type !== 'hotkey' || !allowed()) return
    if (event.id === 'increment') increment(1)
    else if (event.id === 'decrement') increment(-1)
  }
  return () => source.close()
}

/**
 * Escuta o atalho global enquanto <enabled>. <increment> e <allowed> podem mudar a cada render (vale o mais recente).
 * O EventSource reconecta sozinho se a conexão cair.
 */
export function useGlobalHotkeyEvents(enabled: boolean, increment: (delta: number) => void, allowed: () => boolean) {
  const latest = useRef({ increment, allowed })
  useEffect(() => {
    latest.current = { increment, allowed }
  })
  useEffect(() => {
    if (!enabled || typeof EventSource === 'undefined') return
    return subscribeHotkeyEvents(
      new EventSource('/api/hunts/events'),
      (delta) => latest.current.increment(delta),
      () => latest.current.allowed(),
    )
  }, [enabled])
}

// ---- Contas e textos ----

/** 1 - (1 - 1/N)^count, como ShinyOdds.Cumulative do backend. */
export function cumulative(odds: number, count: number): number {
  if (count <= 0) return 0
  if (odds <= 1) return 1
  return 1 - Math.pow(1 - 1 / odds, count)
}

/** Degrau que cobre <value> (min <= value <= max; max nulo = em diante), ou nulo. */
export function stepFor(modifier: ShinyModifier, value: number): ShinyStep | null {
  return modifier.steps?.find((s) => s.min <= value && (s.max === null || value <= s.max)) ?? null
}

/** "512" (ou "4,2" para odds pequenas) no idioma da interface. */
export function formatOdds(odds: number, lang: string): string {
  return odds < 10
    ? odds.toLocaleString(lang, { maximumFractionDigits: 1 })
    : Math.round(odds).toLocaleString(lang)
}

export function formatPercent(value: number, lang: string): string {
  return (value * 100).toLocaleString(lang, { minimumFractionDigits: 1, maximumFractionDigits: 1 }) + '%'
}

/** 3725 → "1:02:05"; 65 → "1:05". */
export function formatElapsed(seconds: number): string {
  const s = Math.max(0, Math.floor(seconds))
  const h = Math.floor(s / 3600)
  const m = Math.floor((s % 3600) / 60)
  const ss = String(s % 60).padStart(2, '0')
  return h > 0 ? `${h}:${String(m).padStart(2, '0')}:${ss}` : `${m}:${ss}`
}

/**
 * Grava o cronômetro ao fechar a janela (pagehide): com keepalive o pedido sai mesmo com a página indo embora.
 */
export function saveElapsedOnExit(id: string, elapsedSeconds: number) {
  try {
    void fetch(`/api/hunts/${id}`, {
      method: 'PUT',
      credentials: 'same-origin',
      keepalive: true,
      headers: { 'Content-Type': 'application/json', ...csrfHeaders() },
      body: JSON.stringify({ elapsedSeconds: Math.floor(elapsedSeconds) }),
    }).catch(() => undefined)
  } catch {
    // sem rede: fica o último valor gravado
  }
}

// ---- Nomes das espécies ----

let speciesRequest: Promise<Option[]> | null = null

/** Todas as espécies (id e nome), pedidas uma vez só. */
export function useSpeciesOptions(): Option[] | null {
  const [list, setList] = useState<Option[] | null>(null)
  useEffect(() => {
    let alive = true
    speciesRequest ??= api
      .get<{ id: number; name: string }[]>('/api/dex/species?limit=2000')
      .then((all) => all.map((s) => ({ value: s.id, text: s.name })))
    speciesRequest
      .then((l) => alive && setList(l))
      .catch(() => {
        speciesRequest = null
        if (alive) setList([])
      })
    return () => {
      alive = false
    }
  }, [])
  return list
}
