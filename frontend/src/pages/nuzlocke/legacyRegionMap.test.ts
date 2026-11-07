import { describe, expect, it } from 'vitest'
// TODO(mapas): testes do formato antigo; saem com legacyRegionMap.ts.
import { graphForGame, labelFontSize, shortLabel, viewBoxFor, type MapGraph } from './legacyRegionMap'

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
