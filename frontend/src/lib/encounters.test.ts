import { describe, expect, it } from 'vitest'
import { summarizeEncounters, type AreaEncounterRow } from './encounters'

const row = (over: Partial<AreaEncounterRow>): AreaEncounterRow => ({
  species: 16,
  form: 0,
  speciesName: 'Pidgey',
  method: 'walk',
  minLevel: 2,
  maxLevel: 3,
  chance: null,
  conditions: null,
  versions: ['firered', 'leafgreen'],
  alpha: false,
  verified: true,
  ...over,
})

describe('summarizeEncounters', () => {
  it('junta a mesma espécie com o mesmo método e versões', () => {
    const s = summarizeEncounters([
      row({ minLevel: 2, maxLevel: 3, chance: '30%', conditions: 'day' }),
      row({ minLevel: 4, maxLevel: 5, chance: '30%', conditions: 'night', verified: false }),
      row({ method: 'surf' }),
      row({ versions: ['firered'] }),
      row({ species: 19, speciesName: 'Rattata' }),
    ])
    expect(s.rows).toHaveLength(4)
    expect(s.rows[0]).toMatchObject({ minLevel: 2, maxLevel: 5, chance: '30%', conditions: 'day, night', verified: false })
    expect(s.species).toEqual([
      { species: 16, form: 0, name: 'Pidgey' },
      { species: 19, form: 0, name: 'Rattata' },
    ])
    expect(s.collapsed).toBe(false)
  })

  it('fecha a tabela acima do limite de linhas', () => {
    const many = Array.from({ length: 7 }, (_, i) => row({ species: i + 1, speciesName: `#${i + 1}` }))
    expect(summarizeEncounters(many).collapsed).toBe(true)
    expect(summarizeEncounters(many.slice(0, 6)).collapsed).toBe(false)
    expect(summarizeEncounters(many, 10).collapsed).toBe(false)
  })

  it('formas diferentes não se juntam', () => {
    const s = summarizeEncounters([row({ form: 0 }), row({ form: 1 })])
    expect(s.rows).toHaveLength(2)
    expect(s.species).toHaveLength(2)
  })
})
