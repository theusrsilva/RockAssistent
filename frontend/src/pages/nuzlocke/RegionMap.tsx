import { useMemo } from 'react'
import { useTranslation } from 'react-i18next'
import type { AreaState, GameArea } from './nuzlocke'
import { MAP_STATES, SCALE, currentNodeId, graphForGame, nodeAreas, nodeState, labelFontSize, shortLabel, stateClass, viewBoxFor, type MapGraph, type MapNode } from './regionMap'

/**
 * Mapa esquemático da região: cidades (quadrado), rotas (círculo), dungeons (losango), construções (quadrado pequeno)
 * e mar (círculo tracejado), ligados por linhas e coloridos pelo estado da área. Nó sem área do jogo fica apagado e
 * não clica.
 */
export default function RegionMap({
  graph,
  game,
  areas,
  states,
  current,
  onSelect,
}: {
  graph: MapGraph
  game: string
  areas: GameArea[]
  /** Estado por id de área (areaStates). */
  states: Record<string, AreaState>
  /** Id da área atual. */
  current: string | null
  onSelect?: (areaId: string) => void
}) {
  const { t } = useTranslation()
  const g = useMemo(() => graphForGame(graph, game), [graph, game])
  const byNode = useMemo(() => nodeAreas(areas), [areas])
  const pos = useMemo(() => new Map(g.nodes.map((n) => [n.id, n])), [g])
  const currentNode = currentNodeId(areas, current)
  const vb = viewBoxFor(g.nodes)

  return (
    <div className="flex flex-col gap-2">
      <div className="scroll-x rounded-lg border border-line bg-bg">
        <svg
          viewBox={`${vb.x} ${vb.y} ${vb.width} ${vb.height}`}
          className="block h-auto max-h-[680px] w-full min-w-[560px]"
          role="group"
          aria-label={t('nuzlocke.map.title')}
        >
          <g className="stroke-line" strokeWidth={4} strokeLinecap="round">
            {g.edges.map((e) => {
              const a = pos.get(e.from)!
              const b = pos.get(e.to)!
              return <line key={`${e.from}-${e.to}`} x1={a.x * SCALE} y1={a.y * SCALE} x2={b.x * SCALE} y2={b.y * SCALE} />
            })}
          </g>
          {g.nodes.map((n) => {
            const nodeAreaIds = byNode.get(n.id)
            const first = nodeAreaIds?.[0]
            return (
              <Node
                key={n.id}
                node={n}
                state={nodeState(nodeAreaIds, states)}
                current={n.id === currentNode}
                onSelect={first && onSelect ? () => onSelect(first) : undefined}
              />
            )
          })}
        </svg>
      </div>
      <div className="flex flex-wrap gap-x-3 gap-y-1 text-[12px] text-muted">
        {MAP_STATES.map((s) => (
          <span key={s} className="inline-flex items-center gap-1">
            <svg viewBox="0 0 12 12" className="h-3 w-3" aria-hidden>
              <circle cx={6} cy={6} r={5} strokeWidth={1.5} className={stateClass(s)} />
            </svg>
            {t(`nuzlocke.map.states.${s}`)}
          </span>
        ))}
        <span className="inline-flex items-center gap-1">
          <svg viewBox="0 0 12 12" className="h-3 w-3" aria-hidden>
            <circle cx={6} cy={6} r={4.5} strokeWidth={2} className="fill-none stroke-accent" />
          </svg>
          {t('nuzlocke.map.current')}
        </span>
      </div>
    </div>
  )
}

function Shape({ kind, cx, cy, className }: { kind: MapNode['kind']; cx: number; cy: number; className: string }) {
  switch (kind) {
    case 'city':
      return <rect x={cx - 17} y={cy - 17} width={34} height={34} rx={5} className={className} />
    case 'building':
      return <rect x={cx - 10} y={cy - 10} width={20} height={20} rx={3} className={className} />
    case 'dungeon':
      return <polygon points={`${cx},${cy - 20} ${cx + 20},${cy} ${cx},${cy + 20} ${cx - 20},${cy}`} className={className} />
    case 'sea':
      return <circle cx={cx} cy={cy} r={14} strokeDasharray="5 4" className={className} />
    default:
      return <circle cx={cx} cy={cy} r={13} className={className} />
  }
}

function Node({ node, state, current, onSelect }: { node: MapNode; state: AreaState; current: boolean; onSelect?: () => void }) {
  const cx = node.x * SCALE
  const cy = node.y * SCALE
  const clickable = !!onSelect
  return (
    <g
      role={clickable ? 'button' : undefined}
      tabIndex={clickable ? 0 : undefined}
      aria-label={node.name}
      aria-current={current ? 'location' : undefined}
      strokeWidth={3}
      onClick={onSelect}
      onKeyDown={(e) => {
        if (clickable && (e.key === 'Enter' || e.key === ' ')) {
          e.preventDefault()
          onSelect()
        }
      }}
      className={`outline-none ${clickable ? 'cursor-pointer [&:focus-visible>.ring]:stroke-ink' : 'opacity-40'}`}
    >
      <title>{node.name}</title>
      {/* Área de toque maior que o desenho. */}
      <circle cx={cx} cy={cy} r={30} className="fill-transparent" />
      <circle cx={cx} cy={cy} r={27} strokeWidth={5} className={`ring fill-none ${current ? 'stroke-accent' : 'stroke-transparent'}`} />
      <Shape kind={node.kind} cx={cx} cy={cy} className={`${stateClass(state)}`} />
      {node.kind === 'city' && (
        <text x={cx} y={cy + 46} textAnchor="middle" fontSize={labelFontSize(node.name)} className="fill-ink font-bold" style={{ paintOrder: 'stroke', stroke: 'var(--color-bg)', strokeWidth: 6 }}>
          {shortLabel(node.name)}
        </text>
      )}
    </g>
  )
}
