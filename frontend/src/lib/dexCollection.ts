import { useCallback, useEffect, useRef, useState } from 'react'
import { api } from './api'
import type { SpeciesSummary } from './dex'

/** Uma linha da coleção: a espécie (formId 0) ou uma forma da lista, normal ou shiny. */
export interface DexOwned {
  speciesId: number
  formId: number
  shiny: boolean
}

export interface DexCollectionData {
  scope: string
  marks: DexOwned[]
  vault: DexOwned[]
}

export const ownedKey = (speciesId: number, formId: number | null | undefined, shiny: boolean) =>
  `${speciesId}:${formId ?? 0}:${shiny ? 1 : 0}`

export interface CollectionSets {
  /** O que a pessoa marcou. */
  marks: ReadonlySet<string>
  /** O que está no cofre agora. */
  vault: ReadonlySet<string>
}

export const emptyCollection: CollectionSets = { marks: new Set(), vault: new Set() }

/** O que uma importação fez: quantas entraram, quantas já estavam marcadas e quantas a lista não tem. */
export interface DexImportResult {
  added: number
  already: number
  skipped: number
}

/**
 * Lê o texto de um arquivo de marcações exportado (JSON com a lista "marks": speciesId, formId e shiny por linha) e
 * devolve as linhas bem formadas. Null quando o texto não é esse arquivo ou não sobra nenhuma linha.
 */
export function parseCollectionImport(text: string): DexOwned[] | null {
  let data: unknown
  try {
    data = JSON.parse(text)
  } catch {
    return null
  }
  const marks = (data as { marks?: unknown } | null)?.marks
  if (!Array.isArray(marks)) return null
  const out: DexOwned[] = []
  for (const mark of marks as ({ speciesId?: unknown; formId?: unknown; shiny?: unknown } | null)[]) {
    const speciesId = mark?.speciesId
    const formId = mark?.formId ?? 0
    if (typeof speciesId !== 'number' || !Number.isInteger(speciesId) || speciesId <= 0) continue
    if (typeof formId !== 'number' || !Number.isInteger(formId) || formId < 0) continue
    out.push({ speciesId, formId, shiny: mark?.shiny === true })
  }
  return out.length > 0 ? out : null
}

/** De onde vem o "tenho" de uma linha: marcação da pessoa, só o cofre, ou nada. */
export function ownedSource(sets: CollectionSets, countVault: boolean, key: string): 'mark' | 'vault' | null {
  if (sets.marks.has(key)) return 'mark'
  return countVault && sets.vault.has(key) ? 'vault' : null
}

export type CollectionFilter = '' | 'tenho' | 'faltam' | 'shiny'

type Entry = Pick<SpeciesSummary, 'id' | 'formId'>

/** A linha passa no filtro "Mostrar": tenho (normal ou shiny), faltam (nenhum dos dois) ou falta o shiny. */
export function matchesCollectionFilter(entry: Entry, filter: CollectionFilter, sets: CollectionSets, countVault: boolean) {
  if (!filter) return true
  const normal = ownedSource(sets, countVault, ownedKey(entry.id, entry.formId, false)) !== null
  const shiny = ownedSource(sets, countVault, ownedKey(entry.id, entry.formId, true)) !== null
  if (filter === 'tenho') return normal || shiny
  if (filter === 'faltam') return !normal && !shiny
  return !shiny
}

/** Progresso sobre as linhas dadas: quantas a pessoa tem (normal ou shiny) e quantas tem shiny. */
export function collectionProgress(entries: Entry[], sets: CollectionSets, countVault: boolean) {
  let owned = 0
  let shiny = 0
  for (const e of entries) {
    const hasShiny = ownedSource(sets, countVault, ownedKey(e.id, e.formId, true)) !== null
    if (hasShiny) shiny++
    if (hasShiny || ownedSource(sets, countVault, ownedKey(e.id, e.formId, false)) !== null) owned++
  }
  return { owned, shiny, total: entries.length }
}

const countVaultKey = 'traderock.dex.countVault'

/** "Contar o que está no cofre": ligado por padrão; a escolha fica neste navegador. */
export function loadCountVault(): boolean {
  try {
    return localStorage.getItem(countVaultKey) !== '0'
  } catch {
    return true
  }
}

export function saveCountVault(on: boolean) {
  try {
    localStorage.setItem(countVaultKey, on ? '1' : '0')
  } catch {
    // sem armazenamento: vale só nesta visita
  }
}

/**
 * A coleção de uma visão ("" = geral; ou a chave de um jogo): carrega as marcações e o cofre e devolve o "marcar".
 * A marcação aparece na hora e volta atrás se o servidor recusar.
 */
export function useDexCollection(scope: string) {
  const [sets, setSets] = useState<CollectionSets>(emptyCollection)
  const [error, setError] = useState(false)
  const [version, setVersion] = useState(0)
  const current = useRef(scope)
  current.current = scope

  useEffect(() => {
    let alive = true
    setSets(emptyCollection)
    api
      .get<DexCollectionData>(`/api/dex/collection?scope=${encodeURIComponent(scope)}`)
      .then((c) => {
        if (!alive) return
        const keys = (list: DexOwned[]) => new Set(list.map((o) => ownedKey(o.speciesId, o.formId, o.shiny)))
        setSets({ marks: keys(c.marks), vault: keys(c.vault) })
        setError(false)
      })
      .catch(() => alive && setError(true))
    return () => {
      alive = false
    }
  }, [scope, version])

  /** Importa as marcações na visão atual (soma, não desmarca) e recarrega a coleção. */
  const importMarks = useCallback(
    async (marks: DexOwned[]) => {
      const result = await api.post<DexImportResult>('/api/dex/collection/import', { scope, marks })
      setVersion((v) => v + 1)
      return result
    },
    [scope],
  )

  const setMark = useCallback(
    (speciesId: number, formId: number | null | undefined, shiny: boolean, owned: boolean) => {
      const key = ownedKey(speciesId, formId, shiny)
      const apply = (on: boolean) =>
        setSets((s) => {
          const marks = new Set(s.marks)
          if (on) marks.add(key)
          else marks.delete(key)
          return { ...s, marks }
        })
      apply(owned)
      api
        .put('/api/dex/collection', { scope, speciesId, formId: formId ?? 0, shiny, owned })
        .then(() => setError(false))
        .catch(() => {
          if (current.current === scope) apply(!owned)
          setError(true)
        })
    },
    [scope],
  )

  return { sets, setMark, importMarks, error }
}
