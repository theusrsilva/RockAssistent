// TODO(mapas): mapa no formato antigo (nós x/y de 0 a 1 e arestas), só até as 11 regiões passarem para o formato
// metrô (regionMap.ts). O controlador apaga este arquivo, LegacyRegionMap.tsx e legacyRegionMap.test.ts no fim.

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
