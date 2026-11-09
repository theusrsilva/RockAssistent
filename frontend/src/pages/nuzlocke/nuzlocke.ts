// Nuzlocke: tipos espelho de /api/nuzlocke (TradeRock.Core/Nuzlocke) e contas puras da tela da run.

import { useCallback, useEffect, useRef, useState } from 'react'
import type { TFunction } from 'i18next'
import { api, ApiError } from '../../lib/api'

export interface NuzlockeRules {
  firstEncounterOnly: boolean
  nicknameRequired: boolean
  faintIsDeath: boolean
  speciesClause: boolean
  shinyClause: boolean
  dupesClause: boolean
  levelCapRequired: boolean
  custom: string | null
}

/** As regras de liga/desliga, na ordem em que aparecem. */
export const RULE_KEYS = [
  'firstEncounterOnly',
  'nicknameRequired',
  'faintIsDeath',
  'speciesClause',
  'shinyClause',
  'dupesClause',
  'levelCapRequired',
] as const satisfies readonly (keyof NuzlockeRules)[]

export type RuleKey = (typeof RULE_KEYS)[number]

export const DEFAULT_RULES: NuzlockeRules = {
  firstEncounterOnly: true,
  nicknameRequired: true,
  faintIsDeath: true,
  speciesClause: false,
  shinyClause: false,
  dupesClause: false,
  levelCapRequired: false,
  custom: null,
}

export type RunStatus = 'active' | 'paused' | 'champion' | 'wipe'
export type MonStatus = 'party' | 'box' | 'dead' | 'missed' | 'failed'

export interface RunInfo {
  id: string
  name: string
  gameKey: string
  version: string | null
  /** Inicial escolhido pelo jogador (identificador do PokeAPI); null = não informado. */
  starter: string | null
  rules: NuzlockeRules
  status: RunStatus
  startedAt: string
  endedAt: string | null
  currentAreaId: string | null
}

export interface MonInfo {
  id: string
  areaId: string
  species: number
  form: number
  nickname: string | null
  level: number
  status: MonStatus
  deathCause: string | null
  caughtAt: string
  diedAt: string | null
  /** Identificadores do PokeAPI ("thunder-shock"). */
  moves: string[]
  shiny: boolean
}

export interface GameArea {
  id: string
  name: string
  kind: string
  order: number
  mapNode: string | null
  pokeApiLocationId: number | null
  bossesBefore: string[]
}

export interface BossPokemon {
  species: number
  form: number
  level: number
  moves: string[]
  ability: string | null
  item: string | null
  teraType: string | null
}

export interface BossBattle {
  id: string
  kind: string
  name: string
  title: string | null
  location: string | null
  order: number
  badge: string | null
  party: BossPokemon[]
  levelCap: number
  verified: boolean
  notes: string | null
  versions: string[] | null
  /**
   * Times que dependem do inicial do jogador (rivais e afins); `party`/`levelCap` são o time padrão. No `nextBoss` da
   * run já vem o time do inicial da run (e sem variantes, se o backend achou a dele).
   */
  variants?: BossVariant[] | null
}

/** Time de um chefe para quem escolheu o inicial `starter` (identificador do PokeAPI do inicial do jogador). */
export interface BossVariant {
  starter: string
  party: BossPokemon[]
  levelCap: number
  verified: boolean
}

/** Um inicial que o jogador escolhe (GET /api/data/{jogo}/starters). */
export interface StarterInfo {
  id: string
  species: number
}

/**
 * A variante mostrada de um chefe: a do inicial pedido; sem pedido (ou sem variante dele), a primeira. Null quando a
 * batalha tem um time só.
 */
export function chosenVariant(boss: BossBattle, starter: string | null): BossVariant | null {
  const variants = boss.variants ?? []
  return variants.find((v) => v.starter === starter) ?? variants[0] ?? null
}

/** Time, cap e conferência mostrados para o chefe com o inicial pedido (o padrão quando não tem variantes). */
export function bossTeam(boss: BossBattle, starter: string | null): Pick<BossVariant, 'party' | 'levelCap' | 'verified'> {
  return chosenVariant(boss, starter) ?? boss
}

export interface TrainerBattle {
  id: string
  area: string
  class: string
  name: string
  party: BossPokemon[]
  verified: boolean
  versions: string[] | null
}

export interface SpeciesEncounterOption {
  species: number
  form: number
  name: string
  methods: string[]
  minLevel: number
  maxLevel: number
  versions: string[]
}

export interface RunView {
  run: RunInfo
  currentArea: GameArea | null
  nextBoss: BossBattle | null
  levelCap: number | null
  areaEncounters: SpeciesEncounterOption[]
  areaTrainers: TrainerBattle[]
  trainersAvailable: boolean
  party: MonInfo[]
  box: MonInfo[]
  graveyard: MonInfo[]
  overCap: MonInfo[]
  lost: MonInfo[]
  defeatedBosses: string[]
  visitedAreas: string[]
}

export interface RunSummary {
  id: string
  name: string
  gameKey: string
  version: string | null
  status: RunStatus
  startedAt: string
  endedAt: string | null
  currentAreaId: string | null
  partyCount: number
  deadCount: number
  bossesDefeated: number
}

export interface LearnsetMove {
  move: string
  name: string
  type: string
  level: number
}

export interface EvolutionLearnset {
  species: number
  name: string
  conditions: string[]
  onEvolve: LearnsetMove[]
  upcoming: LearnsetMove[]
}

export interface MonLearnset {
  species: number
  level: number
  known: LearnsetMove[]
  upcoming: LearnsetMove[]
  evolutions: EvolutionLearnset[]
}

// ---- Contas puras ----

/** Quem da equipe passou do level cap (sem cap, ninguém). */
export function overCap(party: MonInfo[], cap: number | null): MonInfo[] {
  if (!cap || cap <= 0) return []
  return party.filter((m) => m.level > cap)
}

/** Regras ligadas, na ordem fixa. */
export const activeRules = (rules: NuzlockeRules): RuleKey[] => RULE_KEYS.filter((k) => rules[k])

/** Nome curto de uma regra. */
export const formatRule = (t: TFunction, key: RuleKey): string => t(`nuzlocke.rules.${key}.name`)

/** Lista de textos das regras da run (as ligadas e o texto livre). */
export function formatRules(t: TFunction, rules: NuzlockeRules): string[] {
  const list = activeRules(rules).map((k) => formatRule(t, k))
  const custom = rules.custom?.trim()
  if (custom) list.push(custom)
  return list.length ? list : [t('nuzlocke.rules.none')]
}

export type AreaState = 'none' | 'visited' | 'caught' | 'dead' | 'missed'

/** Peso de cada estado: numa área com mais de um registro, vale o maior. */
export const statePriority: Record<AreaState, number> = { none: 0, visited: 1, missed: 2, caught: 3, dead: 4 }

/**
 * Estado de cada área para o mapa (Tarefa 24): visitada, capturado (equipe ou caixa), morto (cemitério) ou perdido
 * (fugiu, desmaiou, sem encontro ou pulado). Áreas que não aparecem ficam de fora (= 'none').
 */
export function areaStates(view: RunView): Record<string, AreaState> {
  const states: Record<string, AreaState> = {}
  const mark = (areaId: string, state: AreaState) => {
    if (statePriority[state] > statePriority[states[areaId] ?? 'none']) states[areaId] = state
  }
  view.visitedAreas.forEach((a) => mark(a, 'visited'))
  view.lost.forEach((m) => mark(m.areaId, 'missed'))
  view.party.forEach((m) => mark(m.areaId, 'caught'))
  view.box.forEach((m) => mark(m.areaId, 'caught'))
  view.graveyard.forEach((m) => mark(m.areaId, 'dead'))
  return states
}

/** Área já usada: tem algum registro que não seja "pulado" (missed). Igual ao backend. */
export function areaUsed(view: RunView, areaId: string): boolean {
  return [...view.party, ...view.box, ...view.graveyard, ...view.lost].some((m) => m.areaId === areaId && m.status !== 'missed')
}

export const sortAreas = (areas: GameArea[]): GameArea[] => [...areas].sort((a, b) => a.order - b.order)

/** Golpes prováveis quando a run não gravou nenhum: os 4 últimos aprendidos por nível (sem repetir). */
export function defaultMoves(known: LearnsetMove[]): string[] {
  // Cada golpe conta pelo maior nível em que é aprendido; os 4 de nível mais alto ficam.
  const level = new Map<string, number>()
  for (const m of known) level.set(m.move, Math.max(level.get(m.move) ?? -1, m.level))
  return [...level.entries()]
    .sort((a, b) => a[1] - b[1])
    .map(([move]) => move)
    .slice(-4)
}

export interface RunStats {
  caught: number
  deaths: number
  bosses: number
  areas: number
  shinies: number
  lost: number
}

export function runStats(view: RunView): RunStats {
  const caught = [...view.party, ...view.box, ...view.graveyard]
  return {
    caught: caught.length,
    deaths: view.graveyard.length,
    bosses: view.defeatedBosses.length,
    areas: view.visitedAreas.length,
    shinies: caught.filter((m) => m.shiny).length,
    lost: view.lost.length,
  }
}

export const isFinished = (status: RunStatus) => status === 'champion' || status === 'wipe'

// ---- Erros ----

/** Erros com texto próprio (o resto vira "common.error"). */
export const NUZLOCKE_ERRORS = [
  'unknown_game',
  'unknown_version',
  'invalid_starter',
  'version_required',
  'unknown_area',
  'unknown_boss',
  'invalid_name',
  'invalid_status',
  'invalid_species',
  'invalid_form',
  'invalid_level',
  'nickname_required',
  'nickname_too_long',
  'death_cause_too_long',
  'custom_too_long',
  'invalid_move',
  'too_many_moves',
  'area_already_used',
  'dupe',
  'party_full',
  'run_not_found',
  'mon_not_found',
  'run_finished',
] as const

export function nuzlockeErrorMessage(t: TFunction, e: unknown): string {
  const code = e instanceof ApiError ? e.code : null
  return code && (NUZLOCKE_ERRORS as readonly string[]).includes(code) ? t(`nuzlocke.errors.${code}`) : t('common.error')
}

// ---- Carregamento ----

/**
 * A run aberta: carrega e aplica o RunView devolvido pelas alterações (<mutate>). Cada pedido leva um número de
 * sequência e só o mais novo vale: uma resposta atrasada não apaga um estado mais novo. Trocar de run limpa a tela.
 */
export function useRun(id: string) {
  const [state, setState] = useState<{ id: string; view: RunView | null; error: unknown }>({ id, view: null, error: null })
  if (state.id !== id) setState({ id, view: null, error: null })
  const last = useRef(0)

  useEffect(() => {
    const seq = ++last.current
    api.get<RunView>(`/api/nuzlocke/${id}`).then(
      (view) => seq === last.current && setState({ id, view, error: null }),
      (error: unknown) => seq === last.current && setState({ id, view: null, error }),
    )
  }, [id])

  const mutate = useCallback(
    async (request: Promise<RunView>) => {
      const seq = ++last.current
      const view = await request
      if (seq === last.current) setState((s) => (s.id === view.run.id ? { ...s, view, error: null } : s))
      return view
    },
    [],
  )

  return { view: state.id === id ? state.view : null, error: state.id === id ? state.error : null, mutate }
}

/** Como uma evolução acontece, para o botão "Evoluir": por nível, por item, por troca, por amizade ou outra condição. */
export type EvolveHow =
  | { kind: 'level'; level: number }
  | { kind: 'item'; item: string }
  | { kind: 'trade'; detail: string | null }
  | { kind: 'friendship' }
  | { kind: 'other'; text: string }

export interface EvolveOption {
  species: number
  name: string
  how: EvolveHow
  /** Por nível: o nível já chegou. As outras ficam sempre disponíveis (o app não sabe do item, da troca...). */
  ready: boolean
}

/** Lê uma condição do PokeAPI (frase curta em inglês, como "level 18", "use Thunder Stone", "trade holding Metal Coat"). */
export function evolveHow(condition: string): EvolveHow {
  const c = condition.trim()
  const level = /^level (\d+)$/.exec(c)
  if (level) return { kind: 'level', level: Number(level[1]) }
  const item = /^use (.+)$/.exec(c)
  if (item) return { kind: 'item', item: item[1] }
  if (c === 'trade' || c.startsWith('trade ')) return { kind: 'trade', detail: c.slice(5).trim() || null }
  if (/^level up friendship \d+$/.test(c)) return { kind: 'friendship' }
  return { kind: 'other', text: c }
}

/**
 * Evoluções possíveis de quem está no nível dado: uma por espécie. Com várias condições (jeitos diferentes no mesmo
 * jogo), vale a primeira por nível (pronta se alguma já chegou); sem condição por nível, a primeira condição.
 */
export function evolveOptions(evolutions: EvolutionLearnset[], level: number): EvolveOption[] {
  return evolutions.map((ev) => {
    const hows = ev.conditions.map(evolveHow)
    const levels = hows.filter((h): h is { kind: 'level'; level: number } => h.kind === 'level').sort((a, b) => a.level - b.level)
    if (levels.length > 0) return { species: ev.species, name: ev.name, how: levels[0], ready: level >= levels[0].level }
    return { species: ev.species, name: ev.name, how: hows[0] ?? { kind: 'other', text: '' }, ready: true }
  })
}

/** Área anterior e próxima da atual, na ordem do seletor (null nas pontas ou sem área atual). */
export function neighborAreas(areas: GameArea[], currentId: string | null | undefined): { prev: GameArea | null; next: GameArea | null } {
  const i = currentId ? areas.findIndex((a) => a.id === currentId) : -1
  if (i < 0) return { prev: null, next: null }
  return { prev: areas[i - 1] ?? null, next: areas[i + 1] ?? null }
}
