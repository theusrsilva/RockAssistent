import { describe, expect, it } from 'vitest'
import { areaNodeId, currentNodeId, graphForGame, nodeAreas, nodeState, labelFontSize, shortLabel, stateClass, viewBoxFor, type MapGraph } from './regionMap'
import type { GameArea } from './nuzlocke'

const graph: MapGraph = {
  region: 'kanto',
  nodes: [
    { id: 'pallet-town', name: 'Pallet Town', kind: 'city', x: 0.1, y: 0.8, games: null },
    { id: 'route-1', name: 'Route 1', kind: 'route', x: 0.1, y: 0.6, games: null },
    { id: 'one-island', name: 'One Island', kind: 'city', x: 0.9, y: 0.5, games: ['frlg'] },
    { id: 'seafoam', name: 'Seafoam Islands', kind: 'dungeon', x: 0.3, y: 0.9, games: ['rb', 'frlg'] },
  ],
  edges: [
    { from: 'pallet-town', to: 'route-1' },
    { from: 'pallet-town', to: 'one-island' },
    { from: 'pallet-town', to: 'seafoam' },
  ],
}

const area = (id: string, mapNode: string | null = null): GameArea => ({ id, name: id, kind: 'route', order: 1, mapNode, pokeApiLocationId: null, bossesBefore: [] })

describe('filtragem por jogo', () => {
  it('games nulo vale para todos; tira os nós de outros jogos e as arestas que ficam soltas', () => {
    const g = graphForGame(graph, 'rb')
    expect(g.nodes.map((n) => n.id)).toEqual(['pallet-town', 'route-1', 'seafoam'])
    expect(g.edges).toEqual([
      { from: 'pallet-town', to: 'route-1' },
      { from: 'pallet-town', to: 'seafoam' },
    ])
  })
  it('no jogo certo, tudo aparece', () => {
    expect(graphForGame(graph, 'frlg').nodes).toHaveLength(4)
    expect(graphForGame(graph, 'frlg').edges).toHaveLength(3)
  })
})

describe('estado → classe', () => {
  it('cada estado tem a sua cor', () => {
    const classes = (['none', 'visited', 'caught', 'dead', 'missed'] as const).map(stateClass)
    expect(new Set(classes).size).toBe(5)
    expect(stateClass('caught')).toContain('fill-ok')
    expect(stateClass('dead')).toContain('fill-bad')
    expect(stateClass('none')).toContain('fill-surface')
  })
})

describe('área ↔ nó', () => {
  it('usa o mapNode e cai no id da área', () => {
    expect(areaNodeId(area('route-1'))).toBe('route-1')
    expect(areaNodeId(area('mt-moon-1f', 'mt-moon'))).toBe('mt-moon')
  })
  it('junta as áreas de cada nó, na ordem', () => {
    const m = nodeAreas([area('mt-moon-1f', 'mt-moon'), area('mt-moon-b1f', 'mt-moon'), area('route-1')])
    expect(m.get('mt-moon')).toEqual(['mt-moon-1f', 'mt-moon-b1f'])
    expect(m.get('route-1')).toEqual(['route-1'])
  })
  it('nó de várias áreas: estado mais forte e anel na área de dentro', () => {
    const areas = [area('lavender-town'), area('pokemon-tower', 'lavender-town'), area('route-8')]
    const ids = nodeAreas(areas).get('lavender-town')
    expect(nodeState(ids, { 'lavender-town': 'visited', 'pokemon-tower': 'caught' })).toBe('caught')
    expect(nodeState(ids, { 'lavender-town': 'dead', 'pokemon-tower': 'caught' })).toBe('dead')
    expect(nodeState(ids, {})).toBe('none')
    expect(nodeState(undefined, { x: 'dead' })).toBe('none')
    expect(currentNodeId(areas, 'pokemon-tower')).toBe('lavender-town')
    expect(currentNodeId(areas, 'route-8')).toBe('route-8')
    expect(currentNodeId(areas, null)).toBeNull()
  })
})

describe('desenho', () => {
  it('rótulo curto das cidades', () => {
    expect(shortLabel('Pallet Town')).toBe('Pallet')
    expect(shortLabel('Viridian City')).toBe('Viridian')
    expect(shortLabel('One Island')).toBe('One')
    expect(shortLabel('Seven Island')).toBe('Seven')
    expect(shortLabel('Cinnabar Island')).toBe('Cinnabar Island')
  })
  it('rótulos das ilhas numeradas em fonte menor (não se colam)', () => {
    expect(labelFontSize('Two Island')).toBe(18)
    expect(labelFontSize('Three Island')).toBe(18)
    expect(labelFontSize('Cinnabar Island')).toBe(26)
    expect(labelFontSize('Pallet Town')).toBe(26)
  })
  it('viewBox cobre os nós com margem', () => {
    const vb = viewBoxFor(graphForGame(graph, 'frlg').nodes)
    expect(vb.x).toBeLessThan(100)
    expect(vb.x + vb.width).toBeGreaterThan(900)
    expect(vb.y).toBeLessThan(500)
    expect(vb.y + vb.height).toBeGreaterThan(900)
  })
})
