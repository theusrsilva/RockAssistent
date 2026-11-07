import { describe, expect, it } from 'vitest'
import type { GuideIndex } from './guides'
import { allGuides, guideOfTheDay } from './home'

const guide = (key: string) => ({ key, title: key.toUpperCase(), sections: [{ id: 'a', title: 'A', file: `${key}.md` }] })

const index: GuideIndex = {
  games: [guide('frlg'), guide('swsh'), guide('sv')],
  mechanics: [guide('ivs'), guide('evs')],
  vgc: [guide('basics')],
  mythology: [guide('kanto'), guide('johto')],
}

describe('guia do dia', () => {
  it('junta os guias de todas as seções', () => {
    expect(allGuides(index).map((g) => `${g.section}/${g.key}`)).toEqual([
      'games/frlg',
      'games/swsh',
      'games/sv',
      'mechanics/ivs',
      'mechanics/evs',
      'vgc/basics',
      'mythology/kanto',
      'mythology/johto',
    ])
  })

  it('mesma data dá o mesmo guia (o horário não importa)', () => {
    const a = guideOfTheDay(index, new Date(2026, 10, 6, 8, 0))
    const b = guideOfTheDay(index, new Date(2026, 10, 6, 23, 59))
    expect(a).not.toBeNull()
    expect(b).toEqual(a)
  })

  it('datas diferentes variam', () => {
    const picks = new Set<string>()
    for (let d = 1; d <= 30; d++) {
      const g = guideOfTheDay(index, new Date(2026, 10, d))!
      picks.add(`${g.section}/${g.key}`)
    }
    expect(picks.size).toBeGreaterThan(3)
  })

  it('índice vazio dá null', () => {
    expect(guideOfTheDay({ games: [], mechanics: [], vgc: [], mythology: [] }, new Date(2026, 10, 6))).toBeNull()
  })

  it('o link aponta para o guia', () => {
    const g = guideOfTheDay(index, new Date(2026, 10, 6))!
    expect(g.to).toBe(`/guias/${g.section}/${g.key}`)
  })
})
