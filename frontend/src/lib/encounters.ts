/** Linha de GET /api/data/{jogo}/areas/{área}/encounters (TradeRock.Core/GameData/AreaEncounters.cs). */
export interface AreaEncounterRow {
  species: number
  form: number
  speciesName: string
  method: string
  minLevel: number
  maxLevel: number
  chance: string | null
  conditions: string | null
  versions: string[]
  alpha: boolean
  verified: boolean
}

/** Até quantas linhas a tabela do bloco :::encounters fica aberta; acima disso, fica num "ver todos". */
export const ENCOUNTERS_OPEN_LIMIT = 6

export interface EncounterSummary {
  /** Linhas juntadas: a mesma espécie (e forma) com o mesmo método e as mesmas versões viram uma só. */
  rows: AreaEncounterRow[]
  /** Espécies diferentes (por espécie e forma), na ordem em que aparecem, para a faixa de sprites. */
  species: { species: number; form: number; name: string }[]
  /** A tabela começa fechada (mais linhas que {@link ENCOUNTERS_OPEN_LIMIT}). */
  collapsed: boolean
}

function joinDistinct(values: (string | null)[]): string | null {
  const list = [...new Set(values.filter((v): v is string => !!v))]
  return list.length ? list.join(', ') : null
}

/**
 * Junta as linhas repetidas da mesma espécie (mesma forma, método e versões): níveis do menor ao maior, chances e
 * condições sem repetir, alfa se alguma for, conferido só se todas forem. Mantém a ordem da primeira aparição.
 */
export function summarizeEncounters(input: AreaEncounterRow[], limit = ENCOUNTERS_OPEN_LIMIT): EncounterSummary {
  const groups = new Map<string, AreaEncounterRow[]>()
  for (const r of input) {
    const key = `${r.species}|${r.form}|${r.method}|${r.versions.join(',')}`
    groups.set(key, [...(groups.get(key) ?? []), r])
  }
  const rows = [...groups.values()].map((list) => ({
    ...list[0],
    minLevel: Math.min(...list.map((r) => r.minLevel)),
    maxLevel: Math.max(...list.map((r) => r.maxLevel)),
    chance: joinDistinct(list.map((r) => r.chance)),
    conditions: joinDistinct(list.map((r) => r.conditions)),
    alpha: list.some((r) => r.alpha),
    verified: list.every((r) => r.verified),
  }))
  const species = new Map<string, { species: number; form: number; name: string }>()
  for (const r of rows) {
    const key = `${r.species}|${r.form}`
    if (!species.has(key)) species.set(key, { species: r.species, form: r.form, name: r.speciesName })
  }
  return { rows, species: [...species.values()], collapsed: rows.length > limit }
}
