import { describe, expect, it } from 'vitest'
import { activityTitle, applyFilter, detailPath, visibleFilters } from './history'

describe('histórico', () => {
  it('leva cada tipo ao detalhe certo', () => {
    expect(detailPath({ kind: 'hunt', id: 'a' })).toBe('/hunt/a')
    expect(detailPath({ kind: 'nuzlocke', id: 'b' })).toBe('/nuzlocke/b')
    expect(detailPath({ kind: 'trade', id: 'c' })).toBeNull()
  })

  it('mostra só os filtros com itens, na ordem fixa', () => {
    expect(visibleFilters([{ kind: 'nuzlocke' }, { kind: 'trade' }, { kind: 'trade' }])).toEqual(['all', 'trade', 'nuzlocke'])
    expect(visibleFilters([])).toEqual(['all'])
  })

  it('filtra por tipo', () => {
    const items = [{ kind: 'hunt' as const }, { kind: 'trade' as const }]
    expect(applyFilter(items, 'all')).toHaveLength(2)
    expect(applyFilter(items, 'hunt')).toEqual([{ kind: 'hunt' }])
  })

  it('monta o título das atividades', () => {
    expect(activityTitle('Mystery Gift', { kind: 'gift', title: 'Celebi', subtitle: 'completed' })).toBe('Mystery Gift: Celebi')
    expect(activityTitle('Raid', { kind: 'raid', title: 'lair-host', subtitle: 'lair-host' }, 'Mewtwo')).toBe('Raid: Mewtwo')
    expect(activityTitle('Raid', { kind: 'raid', title: 'max-raid-join', subtitle: 'max-raid-join' })).toBe('Raid')
    expect(activityTitle('Union Circle', { kind: 'circle', title: 'circle', subtitle: 'failed' })).toBe('Union Circle')
  })
})
