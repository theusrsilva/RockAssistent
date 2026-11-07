/** Histórico unificado (GET /api/history): trocas, raids, Mystery Gift, Union Circle, hunts e Nuzlocke. */

export const HISTORY_KINDS = ['trade', 'raid', 'gift', 'circle', 'hunt', 'nuzlocke'] as const
export type HistoryKind = (typeof HISTORY_KINDS)[number]
export type HistoryFilter = 'all' | HistoryKind

export interface HistoryEntry {
  kind: HistoryKind
  id: string
  at: string
  /** Troca: treinador. Hunt: espécie (inglês). Nuzlocke: nome da run. */
  title: string
  /** Troca: status. Hunt: método. Nuzlocke: champion ou wipe. */
  subtitle: string
  gameKey: string | null
  species: number | null
  ok: boolean
}

/** Rota do detalhe; null = a troca, que abre os eventos na própria lista (como antes). */
export function detailPath(item: Pick<HistoryEntry, 'kind' | 'id'>): string | null {
  switch (item.kind) {
    case 'hunt':
      return `/hunt/${item.id}`
    case 'nuzlocke':
      return `/nuzlocke/${item.id}`
    default:
      return null
  }
}

/** Filtros que aparecem: "Tudo" e só os tipos que têm algum item, na ordem fixa. */
export function visibleFilters(items: Pick<HistoryEntry, 'kind'>[]): HistoryFilter[] {
  const present = new Set(items.map((i) => i.kind))
  return ['all', ...HISTORY_KINDS.filter((k) => present.has(k))]
}

export function applyFilter<T extends Pick<HistoryEntry, 'kind'>>(items: T[], filter: HistoryFilter): T[] {
  return filter === 'all' ? items : items.filter((i) => i.kind === filter)
}

/**
 * Título de uma atividade (raid, Mystery Gift, Union Circle): o nome do tipo e, se houver, o nome do presente ou
 * do evento (o servidor manda o tipo da raid/círculo no título quando não há nome) ou a espécie.
 */
export function activityTitle(kindLabel: string, item: Pick<HistoryEntry, 'kind' | 'title' | 'subtitle'>, speciesName?: string): string {
  const hasLabel = item.title !== item.subtitle && item.title !== item.kind
  const name = hasLabel ? item.title : speciesName
  return name ? `${kindLabel}: ${name}` : kindLabel
}
