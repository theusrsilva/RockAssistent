// Pokédex offline: tipos espelho dos DTOs de GET /api/dex/* (TradeRock.Core/Dex/DexModels.cs) e funções puras.

import { useEffect, useState } from 'react'
import { useSearchParams } from 'react-router'
import { api, ApiError } from './api'
import { GAME_VERSIONS } from './gameVersions'

export type DexLang = 'pt-BR' | 'en'

export interface SpeciesSummary {
  id: number
  name: string
  generation: number
  types: string[]
  nationalNumber: number | null
  /** Só nas formas regionais da lista: id do sprite e da forma na página, a região e o complemento do nome. */
  formId?: number | null
  region?: 'alola' | 'galar' | 'hisui' | 'paldea' | null
  formDetail?: string | null
}

export interface SpeciesAbility {
  id: number
  name: string
  isHidden: boolean
  effect: string | null
}

export interface EvolutionStep {
  speciesId: number
  name: string
  fromSpeciesId: number | null
  conditions: string[]
}

export interface FlavorText {
  version: string
  language: string
  text: string
}

export interface EncounterInfo {
  version: string
  location: string
  area: string | null
  method: string
  minLevel: number
  maxLevel: number
  chance: number
  conditions: string[]
}

export interface LearnedMove {
  id: number
  /** Identificador do PokeAPI (`thunder-punch`), como nos golpes dos chefes. */
  identifier: string
  name: string
  type: string
  class: string | null
  power: number | null
  accuracy: number | null
  pp: number | null
  level: number | null
  machineNumber: number | null
  machine: string | null
}

export interface SpeciesMoves {
  levelUp: LearnedMove[]
  machine: LearnedMove[]
  tutor: LearnedMove[]
  egg: LearnedMove[]
}

export interface BaseStats {
  hp: number
  attack: number
  defense: number
  specialAttack: number
  specialDefense: number
  speed: number
  total: number
  /** Chaves do PokeAPI: hp, attack, defense, special-attack, special-defense, speed. */
  evs: Record<string, number>
}

export interface SpeciesForm {
  id: number
  identifier: string
  name: string
  types: string[]
  stats: BaseStats
  isMega: boolean
  isDefault: boolean
}

export interface SpeciesDetail {
  id: number
  name: string
  names: Record<string, string>
  genus: string | null
  types: string[]
  /** Decímetros. */
  height: number
  /** Hectogramas. */
  weight: number
  /** Oitavos de fêmea; -1 = sem gênero. */
  genderRate: number
  captureRate: number
  hatchSteps: number | null
  baseHappiness: number | null
  growthRate: string | null
  color: string | null
  eggGroups: string[]
  abilities: SpeciesAbility[]
  evolutionChain: EvolutionStep[]
  damageTaken: Record<string, number>
  flavorTexts: FlavorText[]
  encounters: EncounterInfo[]
  moves: SpeciesMoves
  stats: BaseStats
  forms: SpeciesForm[]
  dexNumbers: Record<string, number>
  existsInGame: boolean
}

export interface MoveLearner {
  speciesId: number
  name: string
  /** Identificador do PokeAPI: level-up, machine, tutor, egg e outros raros. */
  method: string
  level: number | null
}

export interface MoveDetail {
  id: number
  identifier: string
  name: string
  type: string
  class: string | null
  power: number | null
  accuracy: number | null
  pp: number | null
  priority: number
  effectChance: number | null
  /** Em inglês. */
  effect: string | null
  generation: number
  existsInGame: boolean
  machineNumber: number | null
  machine: string | null
  learners: MoveLearner[]
}

export interface AbilityHolder {
  speciesId: number
  /** Id do pokemon no PokeAPI (forma); igual ao da espécie na forma padrão. */
  pokemonId: number
  name: string
  isHidden: boolean
}

export interface AbilityDetail {
  id: number
  identifier: string
  name: string
  shortEffect: string | null
  effect: string | null
  generation: number
  pokemon: AbilityHolder[]
}

export interface ItemDetail {
  id: number
  identifier: string
  name: string
  category: string | null
  cost: number | null
  effect: string | null
  flingPower: number | null
}

export interface TypeChart {
  generation: number
  types: string[]
  /** factors[ataque][defesa]: 0, 0.5, 1 ou 2. */
  factors: Record<string, Record<string, number>>
}

export type AvailabilityKind = 'none' | 'encounter' | 'evolution' | 'egg' | 'gift' | 'trade'

export interface GameAvailability {
  gameKey: string
  name: string
  kind: AvailabilityKind
  /** Dá para conseguir, mas sempre travado para shiny (e não dá para chocar). */
  shinyLocked: boolean
  /** O que todo jeito de conseguir precisa: Pokéwalker ou item de evento (Mystery Gift); null = nada. */
  requires: Requirement | null
}

export type Requirement = 'pokewalker' | 'event_item'

/** As 18 identidades de tipo do PokeAPI, na ordem da tabela de tipos. */
export const TYPES = [
  'normal', 'fighting', 'flying', 'poison', 'ground', 'rock', 'bug', 'ghost', 'steel',
  'fire', 'water', 'grass', 'electric', 'psychic', 'ice', 'dragon', 'dark', 'fairy',
] as const

/** Idioma pedido à API a partir do idioma da interface. */
export const dexLang = (language: string | undefined): DexLang => (language?.startsWith('en') ? 'en' : 'pt-BR')

// ---- Jogo escolhido ----

const gameStorageKey = 'traderock.dex.game'

export function loadDexGame(): string {
  try {
    return localStorage.getItem(gameStorageKey) || 'frlg'
  } catch {
    return 'frlg'
  }
}

export function saveDexGame(key: string) {
  try {
    localStorage.setItem(gameStorageKey, key)
  } catch {
    // sem armazenamento: vale só nesta visita
  }
}

/** Chave de um jogo que a Pokédex conhece. */
export const isDexGame = (key: string | null | undefined): key is string => !!key && GAME_VERSIONS.some((g) => g.key === key)

/**
 * Endereço de uma página da Pokédex ("/pokedex/25", "/pokedex/golpe/33") aberta no jogo <game>: acrescenta
 * "?game=". Outros endereços, jogo desconhecido ou um "?game=" já escrito ficam como estão.
 */
export function dexHref(href: string, game?: string | null): string {
  if (!isDexGame(game) || !href.startsWith('/pokedex/')) return href
  const hashAt = href.indexOf('#')
  const hash = hashAt >= 0 ? href.slice(hashAt) : ''
  const rest = hashAt >= 0 ? href.slice(0, hashAt) : href
  const queryAt = rest.indexOf('?')
  const path = queryAt >= 0 ? rest.slice(0, queryAt) : rest
  const query = new URLSearchParams(queryAt >= 0 ? rest.slice(queryAt + 1) : '')
  if (query.has('game')) return href
  query.set('game', game)
  return `${path}?${query}${hash}`
}

/**
 * Jogo da página da Pokédex: o "?game=" do endereço (link de um guia, por exemplo) vem antes do jogo salvo,
 * sem trocá-lo. Escolher outro jogo no seletor salva a preferência (e atualiza o "?game=", se houver).
 */
export function useDexGame(): [string, (key: string) => void] {
  const [params, setParams] = useSearchParams()
  const fromUrl = params.get('game')
  const [saved, setSaved] = useState(() => {
    const key = loadDexGame()
    return isDexGame(key) ? key : 'frlg'
  })
  const choose = (key: string) => {
    setSaved(key)
    saveDexGame(key)
    if (params.has('game'))
      setParams(
        (p) => {
          const n = new URLSearchParams(p)
          n.set('game', key)
          return n
        },
        { replace: true },
      )
  }
  return [isDexGame(fromUrl) ? fromUrl : saved, choose]
}

// ---- Carregamento ----

export interface Loaded<T> {
  data: T | null
  error: ApiError | Error | null
  loading: boolean
}

/** GET simples que descarta respostas antigas. <path> nulo não busca nada. */
export function useApiGet<T>(path: string | null): Loaded<T> {
  const [state, setState] = useState<Loaded<T> & { path: string | null }>({ data: null, error: null, loading: !!path, path })
  if (state.path !== path) setState({ data: null, error: null, loading: !!path, path })
  useEffect(() => {
    if (!path) return
    let current = true
    api
      .get<T>(path)
      .then((data) => current && setState({ data, error: null, loading: false, path }))
      .catch((error: Error) => current && setState({ data: null, error, loading: false, path }))
    return () => {
      current = false
    }
  }, [path])
  return state
}

export const useSpecies = (id: number, game: string, lang: DexLang) =>
  useApiGet<SpeciesDetail>(`/api/dex/species/${id}?game=${encodeURIComponent(game)}&lang=${encodeURIComponent(lang)}`)

export const useMove = (id: number, game: string) =>
  useApiGet<MoveDetail>(id > 0 ? `/api/dex/moves/${id}?game=${encodeURIComponent(game)}` : null)

export const useAbility = (id: number) => useApiGet<AbilityDetail>(id > 0 ? `/api/dex/abilities/${id}` : null)

export const useItem = (id: number) => useApiGet<ItemDetail>(id > 0 ? `/api/dex/items/${id}` : null)

export const useTypeChart = (generation: number) => useApiGet<TypeChart>(`/api/dex/types?generation=${generation}`)

export const useAvailability = (id: number) => useApiGet<GameAvailability[]>(`/api/dex/species/${id}/availability`)

// ---- Funções puras ----

/** Regras de tradução das frases que o backend monta (em inglês). Nomes próprios (itens, golpes, lugares) ficam. */
const evolutionRules = (typeName: (t: string) => string): [RegExp, string | ((...m: string[]) => string)][] => [
  [/\blevel up\b/g, 'subir de nível'],
  [/\blevel (\d+)/g, 'nível $1'],
  [/^trade\b/g, 'troca'],
  [/^use (?=[A-Z])/g, 'usar '],
  [/\bholding (?=[A-Z])/g, 'segurando '],
  [/\bknowing a ([a-z]+) move\b/g, (_, t) => `sabendo um golpe do tipo ${typeName(t)}`],
  [/\bknowing (?=[A-Z])/g, 'sabendo '],
  [/\bwith a ([a-z]+) type in party\b/g, (_, t) => `com um Pokémon do tipo ${typeName(t)} na equipe`],
  [/\bwith (.+?) in party\b/g, 'com $1 na equipe'],
  [/\bwith (?=[A-Z])/g, 'com '],
  [/\bfor (?=[A-Z])/g, 'por '],
  [/\bat day\b/g, 'de dia'],
  [/\bat night\b/g, 'à noite'],
  [/\bat dusk\b/g, 'ao entardecer'],
  [/\bat full-moon\b/g, 'na lua cheia'],
  [/\bat (?=[A-Z])/g, 'em '],
  [/\bfriendship (\d+)/g, 'amizade $1'],
  [/\bbeauty (\d+)/g, 'beleza $1'],
  [/\baffection (\d+)/g, 'afeição $1'],
  [/\bfemale\b/g, 'fêmea'],
  [/\bmale\b/g, 'macho'],
  [/\bAttack ([<>=]) Defense\b/g, 'Ataque $1 Defesa'],
  [/\bin rain\b/g, 'na chuva'],
  [/\bupside down\b/g, 'com o console de cabeça para baixo'],
]

/**
 * Condição de evolução no idioma da tela. O backend manda frases curtas em inglês ("trade holding Metal Coat");
 * em português, as partes conhecidas são traduzidas e o resto (nomes, gatilhos raros) passa como veio.
 */
export function formatEvolutionCondition(condition: string, lang: DexLang, typeName: (t: string) => string = (t) => t): string {
  if (lang === 'en') return condition
  let text = condition
  for (const [pattern, replacement] of evolutionRules(typeName))
    text = typeof replacement === 'string' ? text.replace(pattern, replacement) : text.replace(pattern, replacement as never)
  return text
}

/** Classe de cor da célula da grade de dano: fraqueza em vermelho, resistência em verde, imunidade apagada. */
export function multiplierClass(multiplier: number): string {
  if (multiplier === 0) return 'bg-tile text-muted'
  if (multiplier > 1) return `bg-bad-soft text-bad${multiplier >= 4 ? ' font-extrabold' : ''}`
  if (multiplier < 1) return 'bg-ok-soft text-ok'
  return 'bg-surface2 text-muted'
}

export function formatMultiplier(multiplier: number): string {
  const fractions: Record<number, string> = { 0.5: '½', 0.25: '¼' }
  return `×${fractions[multiplier] ?? String(multiplier)}`
}

export type MoveSortKey = 'level' | 'machine' | 'name' | 'type' | 'class' | 'power' | 'accuracy' | 'pp'
export type SortDir = 'asc' | 'desc'

/** Ordena uma cópia. Valores vazios (golpe sem força, sem precisão) ficam no fim nos dois sentidos; empate pelo nome. */
export function sortMoves(moves: LearnedMove[], key: MoveSortKey, dir: SortDir): LearnedMove[] {
  const value = (m: LearnedMove): string | number | null => {
    switch (key) {
      case 'level':
        return m.level
      case 'machine':
        return m.machineNumber
      case 'name':
        return m.name
      case 'type':
        return m.type
      case 'class':
        return m.class
      default:
        return m[key]
    }
  }
  const sign = dir === 'asc' ? 1 : -1
  return [...moves].sort((a, b) => {
    const va = value(a)
    const vb = value(b)
    if (va === null && vb !== null) return 1
    if (vb === null && va !== null) return -1
    if (va !== null && vb !== null && va !== vb) {
      const cmp = typeof va === 'number' && typeof vb === 'number' ? va - vb : String(va).localeCompare(String(vb))
      if (cmp !== 0) return cmp * sign
    }
    return a.name.localeCompare(b.name)
  })
}

/** Quem aprende um golpe, separado pelo jeito de aprender (na ordem: nível, TM, tutor, ovo, outros). */
export function groupLearners(learners: MoveLearner[]): { method: string; learners: MoveLearner[] }[] {
  const order = ['level-up', 'machine', 'tutor', 'egg']
  const groups = new Map<string, MoveLearner[]>()
  for (const l of learners) {
    const key = order.includes(l.method) ? l.method : 'other'
    groups.set(key, [...(groups.get(key) ?? []), l])
  }
  return [...order, 'other'].filter((m) => groups.has(m)).map((method) => ({ method, learners: groups.get(method)! }))
}

/** Porcentagem de fêmeas e machos; nulo para quem não tem gênero. */
export function genderRatio(rate: number): { female: number; male: number } | null {
  if (rate < 0) return null
  const female = (rate / 8) * 100
  return { female, male: 100 - female }
}

/** "original-johto" → "Original Johto"; usado para identificadores do PokeAPI sem tradução. */
export const titleCase = (identifier: string) =>
  identifier
    .split('-')
    .map((w) => (w ? w[0].toUpperCase() + w.slice(1) : w))
    .join(' ')

/** Nome de uma versão do PokeAPI ("leafgreen" → "LeafGreen") pelo nome do jogo; sem par, vira "Leafgreen". */
export function versionName(version: string): string {
  for (const game of GAME_VERSIONS) {
    const i = game.pokeApiVersions.indexOf(version)
    if (i < 0) continue
    const parts = game.name.split(' / ')
    if (parts.length === game.pokeApiVersions.length) return parts[i]
    if (game.pokeApiVersions.length === 1) return game.name
  }
  return titleCase(version)
}
