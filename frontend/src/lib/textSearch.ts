/**
 * Busca de texto sem acento e sem diferenciar maiúsculas ("ceruléan" acha "Cerulean", "vara boa" acha "Vara Boa").
 * As posições devolvidas são do texto original (tirar o acento muda o tamanho do texto normalizado).
 */

/** Texto normalizado e, para cada caractere dele, a posição no original. */
export function normalizeForSearch(text: string): { text: string; map: number[] } {
  let out = ''
  const map: number[] = []
  for (let i = 0; i < text.length; i++) {
    const base = text[i].normalize('NFD').replace(/[̀-ͯ]/g, '').toLowerCase()
    for (const ch of base) {
      out += ch
      map.push(i)
    }
  }
  return { text: out, map }
}

/** Consulta normalizada (sem acento, minúscula, espaços juntados); vazia = sem busca. */
export const normalizeQuery = (query: string): string => normalizeForSearch(query.trim().replace(/\s+/g, ' ')).text

export interface TextMatch {
  /** Início no texto original. */
  start: number
  /** Fim (exclusivo) no texto original. */
  end: number
}

/** Todas as ocorrências (sem sobreposição) da consulta no texto, em ordem. */
export function findMatches(text: string, query: string): TextMatch[] {
  const q = normalizeQuery(query)
  if (!q) return []
  const { text: norm, map } = normalizeForSearch(text)
  const matches: TextMatch[] = []
  let from = 0
  for (;;) {
    const i = norm.indexOf(q, from)
    if (i < 0) break
    matches.push({ start: map[i], end: map[i + q.length - 1] + 1 })
    from = i + q.length
  }
  return matches
}

/** Quantas vezes a consulta aparece no texto. */
export const countMatches = (text: string, query: string): number => findMatches(text, query).length

/**
 * Quantas vezes a consulta aparece num texto já normalizado (<see normalizeForSearch>): para contar no mesmo texto com
 * várias consultas sem normalizar de novo.
 */
export function countNormalized(normalized: string, query: string): number {
  const q = normalizeQuery(query)
  if (!q) return 0
  let count = 0
  for (let i = normalized.indexOf(q); i >= 0; i = normalized.indexOf(q, i + q.length)) count++
  return count
}

/**
 * Converte uma posição do texto juntado (os nós de texto um atrás do outro) em nó e posição dentro dele.
 * <starts>: onde cada nó começa no texto juntado, em ordem crescente.
 */
export function locate(starts: number[], offset: number): { node: number; offset: number } {
  let lo = 0
  let hi = starts.length - 1
  while (lo < hi) {
    const mid = (lo + hi + 1) >> 1
    if (starts[mid] <= offset) lo = mid
    else hi = mid - 1
  }
  return { node: lo, offset: offset - starts[lo] }
}
