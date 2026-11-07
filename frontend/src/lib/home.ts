// Início: contas puras dos cartões (guia do dia).

import { GUIDE_SECTIONS, type GuideIndex, type GuideSection } from './guides'

export interface GuidePick {
  section: GuideSection
  key: string
  title: string
  description?: string
  /** Endereço do guia (/guias/:section/:slug). */
  to: string
}

/** Todos os guias do índice, na ordem das seções. */
export function allGuides(index: GuideIndex): GuidePick[] {
  return GUIDE_SECTIONS.flatMap((section) =>
    (index[section] ?? []).map((g) => ({ section, key: g.key, title: g.title, description: g.description, to: `/guias/${section}/${g.key}` })),
  )
}

/** Hash FNV-1a de 32 bits: espalha datas vizinhas pelo índice. */
function hash(text: string): number {
  let h = 0x811c9dc5
  for (let i = 0; i < text.length; i++) {
    h ^= text.charCodeAt(i)
    h = Math.imul(h, 0x01000193)
  }
  return h >>> 0
}

/** Guia do dia: o mesmo o dia inteiro (pela data local), outro no dia seguinte. null se não há guias. */
export function guideOfTheDay(index: GuideIndex, date: Date): GuidePick | null {
  const list = allGuides(index)
  if (list.length === 0) return null
  const day = `${date.getFullYear()}-${date.getMonth() + 1}-${date.getDate()}`
  return list[hash(day) % list.length]
}
