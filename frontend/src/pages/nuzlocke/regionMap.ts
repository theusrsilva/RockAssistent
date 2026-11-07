// Mapa esquemático da região (GET /api/data/maps/{region}): tipos e contas puras do desenho.

import { statePriority, type AreaState, type GameArea } from './nuzlocke'

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

/** Nó → áreas que caem nele, na ordem recebida (ex.: Lavender Town e Pokémon Tower). A primeira é a do clique. */
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

/** Estado do nó: o mais forte entre as áreas dele (morto > capturado > perdido > visitado > nada). */
export function nodeState(areaIds: string[] | undefined, states: Record<string, AreaState>): AreaState {
  let best: AreaState = 'none'
  for (const id of areaIds ?? []) {
    const s = states[id] ?? 'none'
    if (statePriority[s] > statePriority[best]) best = s
  }
  return best
}

/** Nó da área atual (a área pode estar num nó compartilhado); sem área conhecida, o próprio id. */
export function currentNodeId(areas: GameArea[], current: string | null): string | null {
  if (!current) return null
  const area = areas.find((a) => a.id === current)
  return area ? areaNodeId(area) : current
}

const NUMBERED_ISLAND = /^(One|Two|Three|Four|Five|Six|Seven) Island$/

/** Rótulo curto das cidades ("Pallet Town" → "Pallet"; ilhas numeradas das Sevii → "One", "Two"...). */
export const shortLabel = (name: string) => {
  const island = NUMBERED_ISLAND.exec(name)
  return island ? island[1] : name.replace(/ (Town|City)$/, '')
}

/**
 * Fonte do rótulo da cidade: 18 nas ilhas numeradas das Sevii (One, Two e Three ficam a 60 unidades umas das outras
 * e, na fonte 26, os rótulos se colam); 26 nas outras.
 */
export const labelFontSize = (name: string) => (NUMBERED_ISLAND.test(name) ? 18 : 26)

/**
 * viewBox justo em volta dos nós. Margens: dos lados cabe metade de um rótulo longo ("Cinnabar Island", fonte 26);
 * embaixo, o rótulo da cidade (46 abaixo do centro); em cima, só o anel do nó atual.
 */
export function viewBoxFor(nodes: MapNode[], pad = { side: 110, top: 45, bottom: 85 }): { x: number; y: number; width: number; height: number } {
  if (nodes.length === 0) return { x: 0, y: 0, width: SCALE, height: SCALE }
  const xs = nodes.map((n) => n.x * SCALE)
  const ys = nodes.map((n) => n.y * SCALE)
  const minX = Math.min(...xs) - pad.side
  const minY = Math.min(...ys) - pad.top
  return { x: minX, y: minY, width: Math.max(...xs) + pad.side - minX, height: Math.max(...ys) + pad.bottom - minY }
}
