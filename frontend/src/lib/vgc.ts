// Seção VGC dos guias: times sugeridos, regulamentação vigente, análise de um paste e "Minha equipe" (localStorage).
// Sem internet por padrão: o paste é colado à mão e analisado pelo servidor local. Com a "Internet opcional" (D32)
// ligada nas Configurações, o servidor baixa as estatísticas do Smogon e abre links do pokepast.es (só GET).

import { ApiError, api } from './api'
import type { BuildResult, ShowdownImportItem } from './types'

export interface PasteMember {
  species: number
  form: number
  name: string
  item: string | null
}

export interface VgcTeam {
  id: string
  name: string
  archetype: string
  regulation: string
  dateIso: string
  paste: string
  ideaPt: string
  ideaEn: string
  howToPlayPt: string
  howToPlayEn: string
  dangersPt: string
  dangersEn: string
  members: PasteMember[]
}

export interface VgcRegulation {
  name: string
  game: string
  validFrom: string
  validTo: string | null
  checkedOn: string
  format: string
  bring: number
  pick: number
  level: number
  allowedDexes: string[]
  bannedSpecies: string[]
  bannedMoves: string[]
  newSpecies: string[]
  newMegas: string[]
  newItems: string[]
  rulesPt: string[]
  rulesEn: string[]
  notesPt: string
  notesEn: string
  verified: boolean
  source: string[]
  smogonFormat?: string | null
}

export interface MoveReport {
  id: number
  name: string
  type: string
  class: string | null
  power: number | null
}

export interface MemberReport {
  species: number
  form: number
  name: string
  types: string[]
  baseSpeed: number
  moves: MoveReport[]
}

export interface DefenseRow {
  type: string
  weak: number
  resist: number
  immune: number
  factors: number[]
}

export interface CoverageRow {
  type: string
  best: number
  superEffectiveBy: string[]
}

export interface TeamReport {
  members: MemberReport[]
  defense: DefenseRow[]
  coverage: CoverageRow[]
  attackTypes: string[]
  speeds: { species: number; name: string; baseSpeed: number }[]
}

export interface AnalyzeResult {
  report: TeamReport
  skipped: { text: string; problems: string[] }[]
}

/** Tamanho máximo do paste (o mesmo do servidor). */
export const MAX_PASTE_BYTES = 20 * 1024

export const pasteBytes = (paste: string) => new TextEncoder().encode(paste).length

export const analyzePaste = (paste: string) => api.post<AnalyzeResult>('/api/vgc/analyze', { paste, game: 'sv' })

/** Texto no idioma da tela (os times têm pt e en). */
export const localized = (lang: string, pt: string, en: string) => (lang.startsWith('pt') ? pt : en)

/**
 * Tipos com problema no time: muitos fracos e poucos que resistem. "Muitos" = 3 ou mais fracos, ou 2 fracos e
 * ninguém que resiste ou é imune.
 */
export const isTeamWeakness = (row: DefenseRow) => row.weak >= 3 || (row.weak >= 2 && row.resist + row.immune === 0)

/** Tipos que nenhum golpe de ataque do time acerta super efetivo. */
export const uncovered = (report: TeamReport) => report.coverage.filter((c) => c.best <= 1).map((c) => c.type)

export interface ImportOutcome {
  saved: number
  failed: string[]
}

/**
 * Importa um paste para o cofre (Caixa de entrada), como Pokémon do Scarlet: lê com o construtor, gera cada um no
 * modo automático e salva. Quem não existe no jogo ou não pôde ser gerado volta em <failed>.
 */
export async function importPasteToBoxes(paste: string, language: string): Promise<ImportOutcome> {
  const game = 'scarlet'
  const items = await api.post<ShowdownImportItem[]>('/api/builder/showdown', { text: paste, game })
  const outcome: ImportOutcome = { saved: 0, failed: [] }
  for (const item of items) {
    const label = item.text.split('\n')[0].trim()
    if (!item.spec) {
      outcome.failed.push(label)
      continue
    }
    try {
      const built = await api.post<BuildResult>('/api/builder/generate', { spec: item.spec, language })
      if (!built.data) {
        outcome.failed.push(label)
        continue
      }
      await api.post('/api/builder/save', { data: built.data, game })
      outcome.saved++
    } catch {
      outcome.failed.push(label)
    }
  }
  return outcome
}

// ---- Minha equipe (só neste navegador) ----

export const MY_TEAM_KEY = 'traderock.vgc.myteam'

export function loadMyTeam(): string {
  try {
    return localStorage.getItem(MY_TEAM_KEY) ?? ''
  } catch {
    return ''
  }
}

export function saveMyTeam(paste: string): boolean {
  try {
    if (paste.trim()) localStorage.setItem(MY_TEAM_KEY, paste)
    else localStorage.removeItem(MY_TEAM_KEY)
    return true
  } catch {
    return false
  }
}

// ---- Internet opcional (D32) ----

export interface InternetSettings {
  smogon: boolean
  pokepaste: boolean
}

export const INTERNET_OFF: InternetSettings = { smogon: false, pokepaste: false }

/** Um golpe, item, habilidade ou parceiro e em quantos times daquele Pokémon ele aparece (%). */
export interface UsageShare {
  id: string
  name: string
  percent: number
}

export interface UsageEntry {
  rank: number
  /** Nome do Showdown, com a forma ("Rotom-Wash"). */
  name: string
  /** Espécie para o sprite (0 quando não reconhecida). */
  species: number
  /** % dos times com este Pokémon. */
  usage: number
  moves: UsageShare[]
  items: UsageShare[]
  abilities: UsageShare[]
  teammates: UsageShare[]
}

export interface UsageReport {
  format: string
  /** "aaaa-mm" */
  month: string
  battles: number
  pokemon: UsageEntry[]
}

export const getInternetSettings = () => api.get<InternetSettings>('/api/settings/internet')
export const saveInternetSettings = (next: InternetSettings) => api.put<InternetSettings>('/api/settings/internet', next)
export const refreshUsage = () => api.post<UsageReport>('/api/vgc/usage/refresh')
export const fetchPokepaste = (url: string) => api.post<{ paste: string }>('/api/vgc/paste/fetch', { url })

/** Mesma regra do servidor: só https://pokepast.es/<id> (ou /<id>/raw), sem porta nem usuário. */
export function isPokepasteUrl(text: string): boolean {
  let url: URL
  try {
    url = new URL(text.trim())
  } catch {
    return false
  }
  if (url.protocol !== 'https:' || url.hostname !== 'pokepast.es' || url.port !== '' || url.username || url.password) return false
  const parts = url.pathname.replace(/^\/+|\/+$/g, '').split('/')
  return /^[A-Za-z0-9]{1,64}$/.test(parts[0]) && (parts.length === 1 || (parts.length === 2 && parts[1] === 'raw'))
}

/** Percentual curto: "49,9%" / "49.9%". */
export const formatPercent = (value: number, lang: string) =>
  `${value.toLocaleString(lang, { maximumFractionDigits: value >= 10 ? 1 : 2 })}%`

const INTERNET_ERRORS = ['internet_disabled', 'paste_host_not_allowed', 'paste_not_found', 'paste_too_big', 'busy', 'local_only']

/** Chave de tradução (vgc.usage.errors.*) para um erro dos downloads; erro de rede vira "network". */
export function internetErrorKey(error: unknown): string {
  if (!(error instanceof ApiError)) return 'network'
  if (error.code && INTERNET_ERRORS.includes(error.code)) return error.code
  return 'network'
}
