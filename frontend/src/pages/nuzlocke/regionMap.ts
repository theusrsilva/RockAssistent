// Mapa esquemático da região (GET /api/data/maps/{region}): tipos e contas puras do desenho.

import type { AreaState, GameArea } from './nuzlocke'

export type MapNodeKind = 'city' | 'route' | 'dungeon' | 'building' | 'sea'

/** Nó do mapa. x e y de 0 a 1 (y para baixo). <games> nulo = todos os jogos da região. */
export interface MapNode {
  id: string
  name: string
  kind: MapNodeKind
  x: number
  y: number
  games: string[] | null
}

export interface MapEdge {
  from: string
  to: string
}

export interface MapGraph {
  region: string
  nodes: MapNode[]
  edges: MapEdge[]
}

/** Largura do desenho em unidades do viewBox (x vai de 0 a SCALE; y também). */
export const SCALE = 1000

/** Só os nós do jogo e as arestas com as duas pontas visíveis. */
export function graphForGame(graph: MapGraph, game: string): MapGraph {
  const nodes = graph.nodes.filter((n) => !n.games || n.games.includes(game))
  const ids = new Set(nodes.map((n) => n.id))
  return { region: graph.region, nodes, edges: graph.edges.filter((e) => ids.has(e.from) && ids.has(e.to)) }
}

/** Classes de preenchimento e contorno de cada estado (tokens do tema). */
const stateClasses: Record<AreaState, string> = {
  none: 'fill-surface stroke-line',
  visited: 'fill-info-soft stroke-info',
  caught: 'fill-ok stroke-ok',
  dead: 'fill-bad stroke-bad',
  missed: 'fill-warn-soft stroke-warn',
}

export const stateClass = (state: AreaState) => stateClasses[state]

export const MAP_STATES: AreaState[] = ['none', 'visited', 'caught', 'missed', 'dead']

/** Nó da área: o mapNode quando os dados o trazem, senão o próprio id da área. */
export const areaNodeId = (area: GameArea) => area.mapNode ?? area.id

/** Nó → área (a primeira, na ordem recebida, quando várias caem no mesmo nó). */
export function nodeAreas(areas: GameArea[]): Map<string, string> {
  const map = new Map<string, string>()
  for (const a of areas) {
    const node = areaNodeId(a)
    if (!map.has(node)) map.set(node, a.id)
  }
  return map
}

/** Rótulo curto das cidades ("Pallet Town" → "Pallet"). */
export const shortLabel = (name: string) => name.replace(/ (Town|City)$/, '')

/** viewBox justo em volta dos nós, com margem para os rótulos. */
export function viewBoxFor(nodes: MapNode[], pad = 50): { x: number; y: number; width: number; height: number } {
  if (nodes.length === 0) return { x: 0, y: 0, width: SCALE, height: SCALE }
  const xs = nodes.map((n) => n.x * SCALE)
  const ys = nodes.map((n) => n.y * SCALE)
  const minX = Math.min(...xs) - pad
  const minY = Math.min(...ys) - pad
  return { x: minX, y: minY, width: Math.max(...xs) + pad - minX, height: Math.max(...ys) + pad - minY }
}
