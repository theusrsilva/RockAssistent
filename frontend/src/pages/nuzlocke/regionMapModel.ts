// Mapa da região em estilo metrô (GET /api/data/maps/{region}): tipos e contas puras do desenho.
// Manual do formato: .superpowers/sdd/2026-11-05-rockassistent/mapas-manual.md.

import { statePriority, type AreaState, type GameArea } from './nuzlocke'

export type PlaceKind = 'city' | 'place' | 'building'
export type LabelPos = 'n' | 's' | 'e' | 'w' | 'ne' | 'nw' | 'se' | 'sw'
export type TerrainKind = 'land' | 'sea' | 'mountain' | 'sand'
export type Point = [number, number]

/** Cidade (quadrado), lugar (círculo) ou construção (quadrado pequeno) num ponto inteiro da grade. */
export interface MapPlace {
  id: string
  name: string
  kind: PlaceKind
  x: number
  y: number
  /** Lado do rótulo. */
  label: LabelPos
  /** Ajuste fino do rótulo [dx, dy] em unidades da grade. */
  labelOffset?: number[] | null
  /** Nulo = todos os jogos da região. */
  games: string[] | null
}

/** Rota: linha por pontos da grade (segmentos só horizontais ou verticais). Número nulo = sem etiqueta. */
export interface MapRoute {
  id: string
  name: string
  number: string | null
  water: boolean
  path: Point[]
  games: string[] | null
}

export interface MapTerrain {
  kind: TerrainKind
  polygon: Point[]
}

export interface RegionMapData {
  region: string
  grid: { cols: number; rows: number }
  terrain: MapTerrain[]
  places: MapPlace[]
  routes: MapRoute[]
  unmapped: { id: string; reason: string }[]
}

/** Unidades do desenho por unidade da grade (o SVG é desenhado em grade × CELL). */
export const CELL = 32
// Medidas do rótulo: as mesmas de TradeRock.Core/GameData/MapLabels.cs (mude os dois juntos).
const CHAR_WIDTH = 0.6
const LINE_HEIGHT = 1.2
const GAP = 4
const WRAP_AFTER = 12
/** Espessura da rota (contorno) e do miolo colorido. */
export const ROUTE_OUTER = 12
export const ROUTE_INNER = 7
const TAG_FONT = 11
const TAG_HEIGHT = 16

export const halfSize = (kind: PlaceKind) => (kind === 'city' ? 11 : kind === 'building' ? 7.5 : 8)
export const fontSize = (kind: PlaceKind) => (kind === 'city' ? 13 : 11.5)

/** Só os lugares e as rotas do jogo (games nulo = todos). */
export function mapForGame(map: RegionMapData, game: string): RegionMapData {
  const has = (games: string[] | null) => !games || games.includes(game)
  return { ...map, places: map.places.filter((p) => has(p.games)), routes: map.routes.filter((r) => has(r.games)) }
}

/** Texto do rótulo: cidade sem " Town"/" City"; acima de 12 letras quebra em duas linhas no espaço mais perto do meio. */
export function labelLines(kind: PlaceKind, name: string): string[] {
  const text = kind === 'city' ? name.replace(/ (Town|City)$/, '') : name
  if (text.length <= WRAP_AFTER || !text.includes(' ')) return [text]
  const mid = text.length / 2
  let best = -1
  for (let i = 0; i < text.length; i++) if (text[i] === ' ' && (best < 0 || Math.abs(i - mid) < Math.abs(best - mid))) best = i
  return [text.slice(0, best), text.slice(best + 1)]
}

export interface Box {
  x: number
  y: number
  w: number
  h: number
}

export interface LabelLayout {
  lines: string[]
  font: number
  anchor: 'start' | 'middle' | 'end'
  /** x do texto e y da linha de base da primeira linha. */
  x: number
  y: number
  box: Box
}

/** Posição do rótulo de um lugar (unidades do desenho). */
export function labelLayout(p: MapPlace): LabelLayout {
  const lines = labelLines(p.kind, p.name)
  const font = fontSize(p.kind)
  const w = Math.max(...lines.map((l) => l.length)) * font * CHAR_WIDTH
  const h = lines.length * font * LINE_HEIGHT
  const half = halfSize(p.kind)
  const cx = p.x * CELL
  const cy = p.y * CELL
  const d = half + GAP
  const corners: Record<LabelPos, [number, number]> = {
    n: [cx - w / 2, cy - d - h],
    s: [cx - w / 2, cy + d],
    e: [cx + d, cy - h / 2],
    w: [cx - d - w, cy - h / 2],
    ne: [cx + half, cy - half - h],
    nw: [cx - half - w, cy - half - h],
    se: [cx + half, cy + half],
    sw: [cx - half - w, cy + half],
  }
  let [x, y] = corners[p.label]
  if (p.labelOffset?.length === 2) {
    x += p.labelOffset[0] * CELL
    y += p.labelOffset[1] * CELL
  }
  const anchor = p.label.endsWith('w') ? 'end' : p.label.endsWith('e') ? 'start' : 'middle'
  const tx = anchor === 'start' ? x : anchor === 'end' ? x + w : x + w / 2
  // Linha de base: a caixa tem font × 1,2 por linha; o texto ocupa ~0,8 da fonte acima da base.
  const ty = y + (font * LINE_HEIGHT - font) / 2 + font * 0.8
  return { lines, font, anchor, x: tx, y: ty, box: { x, y, w, h } }
}

/** Caixa do desenho do lugar. */
export function placeBox(p: MapPlace): Box {
  const half = halfSize(p.kind)
  return { x: p.x * CELL - half, y: p.y * CELL - half, w: half * 2, h: half * 2 }
}

export interface RouteTag {
  text: string
  x: number
  y: number
  box: Box
}

/** Etiqueta com o número no meio do maior segmento da rota; nula sem número. */
export function routeTag(r: MapRoute): RouteTag | null {
  if (!r.number) return null
  let best = 0
  let mid: Point = r.path[0]
  for (let i = 1; i < r.path.length; i++) {
    const [a, b] = [r.path[i - 1], r.path[i]]
    const len = Math.abs(a[0] - b[0]) + Math.abs(a[1] - b[1])
    if (len > best) {
      best = len
      mid = [(a[0] + b[0]) / 2, (a[1] + b[1]) / 2]
    }
  }
  const w = Math.max(TAG_HEIGHT, r.number.length * TAG_FONT * 0.62 + 8)
  const x = mid[0] * CELL
  const y = mid[1] * CELL
  return { text: r.number, x, y, box: { x: x - w / 2, y: y - TAG_HEIGHT / 2, w, h: TAG_HEIGHT } }
}

/** Retângulos dos segmentos da rota com a espessura do contorno. */
export function routeBoxes(r: MapRoute): Box[] {
  const half = ROUTE_OUTER / 2
  const boxes: Box[] = []
  for (let i = 1; i < r.path.length; i++) {
    const [ax, ay] = [r.path[i - 1][0] * CELL, r.path[i - 1][1] * CELL]
    const [bx, by] = [r.path[i][0] * CELL, r.path[i][1] * CELL]
    boxes.push({ x: Math.min(ax, bx) - half, y: Math.min(ay, by) - half, w: Math.abs(ax - bx) + half * 2, h: Math.abs(ay - by) + half * 2 })
  }
  return boxes
}

const intersects = (a: Box, b: Box, slack = 0.5) =>
  a.x + slack < b.x + b.w && b.x + slack < a.x + a.w && a.y + slack < b.y + b.h && b.y + slack < a.y + a.h

/** Os segmentos [a, b] e [c, d] (horizontais ou verticais) se sobrepõem num trecho (não só num ponto). */
function collinearOverlap(a: Point, b: Point, c: Point, d: Point): boolean {
  const range = (p: number, q: number): [number, number] => [Math.min(p, q), Math.max(p, q)]
  if (a[1] === b[1] && c[1] === d[1] && a[1] === c[1]) {
    const [s1, e1] = range(a[0], b[0])
    const [s2, e2] = range(c[0], d[0])
    return Math.min(e1, e2) - Math.max(s1, s2) > 0
  }
  if (a[0] === b[0] && c[0] === d[0] && a[0] === c[0]) {
    const [s1, e1] = range(a[1], b[1])
    const [s2, e2] = range(c[1], d[1])
    return Math.min(e1, e2) - Math.max(s1, s2) > 0
  }
  return false
}

/**
 * Sobreposições do desenho de um jogo (lista vazia = legível): rótulo com rótulo, com outro lugar, com etiqueta de rota
 * ou com linha de rota; etiqueta com etiqueta, com lugar ou com outra rota; duas rotas no mesmo trecho.
 */
export function overlaps(map: RegionMapData): string[] {
  const problems: string[] = []
  const labels = map.places.map((p) => ({ id: p.id, box: labelLayout(p).box }))
  const shapes = map.places.map((p) => ({ id: p.id, box: placeBox(p) }))
  const tags = map.routes.flatMap((r) => {
    const t = routeTag(r)
    return t ? [{ id: r.id, box: t.box }] : []
  })
  const segments = map.routes.flatMap((r) => routeBoxes(r).map((box) => ({ id: r.id, box })))

  labels.forEach((l, i) => {
    labels.slice(i + 1).forEach((o) => intersects(l.box, o.box) && problems.push(`rótulo ${l.id} × rótulo ${o.id}`))
    shapes.forEach((s) => s.id !== l.id && intersects(l.box, s.box) && problems.push(`rótulo ${l.id} × lugar ${s.id}`))
    tags.forEach((t) => intersects(l.box, t.box) && problems.push(`rótulo ${l.id} × número da ${t.id}`))
    const hit = new Set(segments.filter((s) => intersects(l.box, s.box, 1)).map((s) => s.id))
    hit.forEach((id) => problems.push(`rótulo ${l.id} × linha da ${id}`))
  })
  tags.forEach((t, i) => {
    tags.slice(i + 1).forEach((o) => intersects(t.box, o.box) && problems.push(`número da ${t.id} × número da ${o.id}`))
    shapes.forEach((s) => intersects(t.box, s.box) && problems.push(`número da ${t.id} × lugar ${s.id}`))
    const hit = new Set(segments.filter((s) => s.id !== t.id && intersects(t.box, s.box, 1)).map((s) => s.id))
    hit.forEach((id) => problems.push(`número da ${t.id} × linha da ${id}`))
  })
  map.routes.forEach((r, i) =>
    map.routes.slice(i + 1).forEach((o) => {
      for (let a = 1; a < r.path.length; a++)
        for (let b = 1; b < o.path.length; b++)
          if (collinearOverlap(r.path[a - 1], r.path[a], o.path[b - 1], o.path[b])) {
            problems.push(`linha da ${r.id} × linha da ${o.id}`)
            return
          }
    }),
  )
  return problems
}

/** viewBox justo em volta do que aparece no jogo (lugares, rótulos, rotas e números), dentro da grade. */
export function viewBoxFor(map: RegionMapData, pad = 14): Box {
  const boxes = [
    ...map.places.flatMap((p) => [placeBox(p), labelLayout(p).box]),
    ...map.routes.flatMap((r) => routeBoxes(r)),
    ...map.routes.flatMap((r) => routeTag(r)?.box ?? []),
  ]
  const W = map.grid.cols * CELL
  const H = map.grid.rows * CELL
  if (boxes.length === 0) return { x: 0, y: 0, w: W, h: H }
  const x = Math.max(0, Math.min(...boxes.map((b) => b.x)) - pad)
  const y = Math.max(0, Math.min(...boxes.map((b) => b.y)) - pad)
  const x2 = Math.min(W, Math.max(...boxes.map((b) => b.x + b.w)) + pad)
  const y2 = Math.min(H, Math.max(...boxes.map((b) => b.y + b.h)) + pad)
  return { x, y, w: x2 - x, h: y2 - y }
}

/** Ponto do desenho do lugar ou da rota (a etiqueta, ou o meio do caminho) para centralizar a rolagem; nulo sem ele. */
export function focusPoint(map: RegionMapData, id: string | null): [number, number] | null {
  if (!id) return null
  const place = map.places.find((p) => p.id === id)
  if (place) return [place.x * CELL, place.y * CELL]
  const route = map.routes.find((r) => r.id === id)
  if (!route) return null
  const tag = routeTag(route)
  if (tag) return [tag.x, tag.y]
  const mid = route.path[Math.floor(route.path.length / 2)]
  return [mid[0] * CELL, mid[1] * CELL]
}

/** Pontos do polígono no formato do atributo points do SVG. */
export const svgPoints = (pts: Point[] | number[][]) => pts.map(([x, y]) => `${x * CELL},${y * CELL}`).join(' ')

// --- Estado das áreas (vale para lugares e rotas) ---

/** Classes de preenchimento e contorno de cada estado (tokens do tema) para os lugares. */
const stateClasses: Record<AreaState, string> = {
  none: 'fill-surface stroke-line',
  visited: 'fill-info-soft stroke-info',
  caught: 'fill-ok stroke-ok',
  dead: 'fill-bad stroke-bad',
  missed: 'fill-warn-soft stroke-warn',
}

export const stateClass = (state: AreaState) => stateClasses[state]

/** Cor do miolo da rota em cada estado (a de água sem estado fica azul). */
const routeStrokes: Record<AreaState, string> = {
  none: 'stroke-surface',
  visited: 'stroke-info',
  caught: 'stroke-ok',
  dead: 'stroke-bad',
  missed: 'stroke-warn',
}

export const routeStroke = (state: AreaState, water: boolean) => (state === 'none' && water ? 'stroke-map-water' : routeStrokes[state])

export const MAP_STATES: AreaState[] = ['none', 'visited', 'caught', 'missed', 'dead']

/** Lugar ou rota da área: o mapNode quando os dados o trazem, senão o próprio id da área. */
export const areaNodeId = (area: GameArea) => area.mapNode ?? area.id

/** Lugar/rota → áreas que caem nele, na ordem recebida (ex.: Lavender Town e Pokémon Tower). A primeira é a do clique. */
export function nodeAreas(areas: GameArea[]): Map<string, string[]> {
  const map = new Map<string, string[]>()
  for (const a of areas) {
    const node = areaNodeId(a)
    const list = map.get(node)
    if (list) list.push(a.id)
    else map.set(node, [a.id])
  }
  return map
}

/** Estado do lugar ou da rota: o mais forte entre as áreas dele (morto > capturado > perdido > visitado > nada). */
export function nodeState(areaIds: string[] | undefined, states: Record<string, AreaState>): AreaState {
  let best: AreaState = 'none'
  for (const id of areaIds ?? []) {
    const s = states[id] ?? 'none'
    if (statePriority[s] > statePriority[best]) best = s
  }
  return best
}

/** Lugar ou rota da área atual (a área pode estar num lugar compartilhado); sem área conhecida, o próprio id. */
export function currentNodeId(areas: GameArea[], current: string | null): string | null {
  if (!current) return null
  const area = areas.find((a) => a.id === current)
  return area ? areaNodeId(area) : current
}
