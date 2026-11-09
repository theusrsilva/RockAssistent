import { readFileSync, readdirSync } from 'node:fs'
import { describe, expect, it } from 'vitest'
import { GAME_VERSIONS } from '../../lib/gameVersions'
import type { GameArea } from './nuzlocke'
import {
  areaNodeId,
  currentNodeId,
  focusPoint,
  labelLayout,
  labelLines,
  mapForGame,
  nodeAreas,
  nodeState,
  overlaps,
  routeStroke,
  routeTag,
  stateClass,
  viewBoxFor,
  type MapPlace,
  type RegionMapData,
} from './regionMapModel'

const place = (id: string, x: number, y: number, label: MapPlace['label'] = 's', kind: MapPlace['kind'] = 'city', games: string[] | null = null): MapPlace => ({
  id,
  name: id,
  kind,
  x,
  y,
  label,
  games,
})

const base: RegionMapData = {
  region: 'kanto',
  grid: { cols: 20, rows: 10 },
  terrain: [],
  places: [place('Pallet Town', 2, 8), place('Viridian City', 2, 4, 'n'), place('Seafoam', 8, 8, 's', 'place', ['frlg'])],
  routes: [
    { id: 'route-1', name: 'Route 1', number: '1', water: false, path: [[2, 8], [2, 4]], games: null },
    { id: 'route-20', name: 'Route 20', number: '20', water: true, path: [[2, 8], [8, 8]], games: ['frlg'] },
  ],
  unmapped: [],
}

const area = (id: string, mapNode: string | null = null): GameArea => ({ id, name: id, kind: 'route', order: 1, mapNode, pokeApiLocationId: null, bossesBefore: [] })

describe('filtragem por jogo', () => {
  it('games nulo vale para todos; tira lugares e rotas de outros jogos', () => {
    const m = mapForGame(base, 'rb')
    expect(m.places.map((p) => p.id)).toEqual(['Pallet Town', 'Viridian City'])
    expect(m.routes.map((r) => r.id)).toEqual(['route-1'])
    expect(mapForGame(base, 'frlg').places).toHaveLength(3)
  })
})

describe('rótulos e números', () => {
  it('cidade sem Town/City; nome longo quebra em duas linhas', () => {
    expect(labelLines('city', 'Pallet Town')).toEqual(['Pallet'])
    expect(labelLines('city', 'Cinnabar Island')).toEqual(['Cinnabar', 'Island'])
    expect(labelLines('place', 'Mt. Moon')).toEqual(['Mt. Moon'])
    expect(labelLines('place', 'Underground Path 5-6')).toEqual(['Underground', 'Path 5-6'])
  })
  it('o rótulo fica do lado pedido', () => {
    const p = place('Pallet Town', 2, 8)
    expect(labelLayout(p).box.y).toBeGreaterThan(8 * 32)
    expect(labelLayout({ ...p, label: 'n' }).box.y + labelLayout({ ...p, label: 'n' }).box.h).toBeLessThan(8 * 32)
    expect(labelLayout({ ...p, label: 'e' }).anchor).toBe('start')
    expect(labelLayout({ ...p, label: 'w' }).anchor).toBe('end')
    expect(labelLayout({ ...p, labelOffset: [1, 0] }).box.x).toBe(labelLayout(p).box.x + 32)
  })
  it('número no meio do maior segmento; sem número, sem etiqueta', () => {
    const tag = routeTag({ id: 'r', name: 'r', number: '12', water: false, path: [[0, 0], [0, 1], [6, 1]], games: null })
    expect(tag).toMatchObject({ text: '12', x: 3 * 32, y: 32 })
    expect(routeTag({ id: 'r', name: 'r', number: null, water: false, path: [[0, 0], [0, 1]], games: null })).toBeNull()
  })
  it('ponto da área atual para centralizar a rolagem: o lugar ou o número da rota', () => {
    expect(focusPoint(base, 'Pallet Town')).toEqual([64, 256])
    expect(focusPoint(base, 'route-1')).toEqual([64, 192])
    expect(focusPoint(base, 'atlantis')).toBeNull()
    expect(focusPoint(base, null)).toBeNull()
  })
  it('viewBox cobre só o que aparece no jogo', () => {
    expect(viewBoxFor(mapForGame(base, 'rb')).w).toBeLessThan(viewBoxFor(mapForGame(base, 'frlg')).w)
  })
})

describe('sobreposição', () => {
  it('o exemplo não se sobrepõe', () => {
    expect(overlaps(mapForGame(base, 'frlg'))).toEqual([])
  })
  it('acha rótulos colados, rótulo sobre linha, número sobre lugar e rotas no mesmo trecho', () => {
    const close: RegionMapData = { ...base, places: [place('Celadon City', 2, 2), place('Saffron City', 3, 2), place('Lavender Town', 4, 2)], routes: [] }
    expect(overlaps(close).some((p) => p.includes('rótulo Celadon City × rótulo Saffron City'))).toBe(true)
    const onLine: RegionMapData = { ...base, places: [place('Pallet Town', 2, 4, 's')], routes: [{ ...base.routes[0], path: [[2, 4], [2, 8]] }] }
    expect(overlaps(onLine)).toContain('rótulo Pallet Town × linha da route-1')
    const onPlace: RegionMapData = { ...base, places: [...base.places.slice(0, 2), place('Mid', 2, 6, 'e', 'place')], routes: [base.routes[0]] }
    expect(overlaps(onPlace)).toContain('número da route-1 × lugar Mid')
    const shared: RegionMapData = { ...base, places: [], routes: [base.routes[0], { ...base.routes[0], id: 'route-2', path: [[2, 6], [2, 2]] }] }
    expect(overlaps(shared)).toContain('linha da route-1 × linha da route-2')
  })
})

describe('estado → cor', () => {
  it('cada estado tem a sua cor, no lugar e na rota', () => {
    const classes = (['none', 'visited', 'caught', 'dead', 'missed'] as const).map(stateClass)
    expect(new Set(classes).size).toBe(5)
    expect(stateClass('caught')).toContain('fill-ok')
    expect(routeStroke('caught', false)).toBe('stroke-ok')
    expect(routeStroke('dead', true)).toBe('stroke-bad')
    expect(routeStroke('none', true)).toBe('stroke-map-water')
    expect(routeStroke('none', false)).toBe('stroke-surface')
  })
})

describe('área ↔ lugar ou rota', () => {
  it('usa o mapNode e cai no id da área', () => {
    expect(areaNodeId(area('route-1'))).toBe('route-1')
    expect(areaNodeId(area('mt-moon-1f', 'mt-moon'))).toBe('mt-moon')
  })
  it('lugar de várias áreas: estado mais forte e anel na área de dentro', () => {
    const areas = [area('lavender-town'), area('pokemon-tower', 'lavender-town'), area('route-8')]
    const ids = nodeAreas(areas).get('lavender-town')
    expect(ids).toEqual(['lavender-town', 'pokemon-tower'])
    expect(nodeState(ids, { 'lavender-town': 'visited', 'pokemon-tower': 'caught' })).toBe('caught')
    expect(nodeState(ids, { 'lavender-town': 'dead', 'pokemon-tower': 'caught' })).toBe('dead')
    expect(nodeState(undefined, { x: 'dead' })).toBe('none')
    expect(currentNodeId(areas, 'pokemon-tower')).toBe('lavender-town')
    expect(currentNodeId(areas, 'route-8')).toBe('route-8')
    expect(currentNodeId(areas, null)).toBeNull()
  })
})

// --- Os mapas de verdade (assets/data/maps) ---

const mapsDir = new URL('../../../../assets/data/maps/', import.meta.url)
const camel = (value: unknown): unknown =>
  Array.isArray(value)
    ? value.map(camel)
    : value && typeof value === 'object'
      ? Object.fromEntries(Object.entries(value).map(([k, v]) => [k.replace(/_([a-z])/g, (_, c: string) => c.toUpperCase()), camel(v)]))
      : value
const realMaps = new Map(
  readdirSync(mapsDir)
    .filter((f) => f.endsWith('.json'))
    .map((f) => camel(JSON.parse(readFileSync(new URL(f, mapsDir), 'utf8'))) as RegionMapData)
    .map((m) => [m.region, m] as const),
)
const areasDir = new URL('../../../../assets/data/areas/', import.meta.url)
const realAreas = (game: string) =>
  (camel(JSON.parse(readFileSync(new URL(`${game}.json`, areasDir), 'utf8'))) as { items: GameArea[] }).items

describe('mapas embutidos', () => {
  it('as 11 regiões estão no formato metrô', () => {
    expect([...realMaps.keys()].sort()).toEqual([...new Set(GAME_VERSIONS.map((g) => g.region))].sort())
    for (const map of realMaps.values()) expect(map.grid.cols).toBeGreaterThan(0)
  })
  it('os 22 jogos', () => expect(GAME_VERSIONS).toHaveLength(22))
  for (const { key: game, region } of GAME_VERSIONS) {
    it(`${region} no ${game}: nada sobreposto`, () => {
      const map = realMaps.get(region)
      expect(map, `${region} sem mapa`).toBeDefined()
      expect(overlaps(mapForGame(map!, game))).toEqual([])
    })
    it(`${region} no ${game}: toda área ligada a um lugar/rota ou em unmapped`, () => {
      const map = mapForGame(realMaps.get(region)!, game)
      const ids = new Set([...map.places.map((p) => p.id), ...map.routes.map((r) => r.id)])
      const unmapped = new Set(map.unmapped.map((u) => u.id))
      const areas = realAreas(game)
      expect(areas.length).toBeGreaterThan(0)
      const missing = areas.filter((a) => !ids.has(areaNodeId(a)) && (a.mapNode || !unmapped.has(a.id))).map((a) => `${a.id} → ${areaNodeId(a)}`)
      expect(missing).toEqual([])
    })
  }
})
