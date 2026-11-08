import { useEffect, useMemo, useRef, type KeyboardEvent } from 'react'
import { useTranslation } from 'react-i18next'
import type { AreaState, GameArea } from './nuzlocke'
import {
  CELL,
  MAP_STATES,
  ROUTE_INNER,
  ROUTE_OUTER,
  currentNodeId,
  focusPoint,
  halfSize,
  labelLayout,
  mapForGame,
  nodeAreas,
  nodeState,
  routeStroke,
  routeTag,
  stateClass,
  svgPoints,
  viewBoxFor,
  type MapPlace,
  type MapRoute,
  type RegionMapData,
  type TerrainKind,
} from './regionMap'

const terrainClass: Record<TerrainKind, string> = {
  land: 'fill-map-land',
  sea: 'fill-map-sea',
  mountain: 'fill-map-mountain',
  sand: 'fill-map-sand',
}

/** Halo do texto na cor do fundo do cartão (legível sobre terra, mar e linhas). */
const halo = { paintOrder: 'stroke', stroke: 'var(--color-surface)', strokeWidth: 3.5, strokeLinejoin: 'round' } as const

/**
 * Mapa da região em estilo metrô: terreno atrás, rotas como linhas grossas (horizontais e verticais) com o número no
 * meio, rotas de água tracejadas em azul, cidades em quadrado, lugares em círculo e construções em quadrado pequeno.
 * Lugares e rotas ganham a cor do estado da área; a área atual tem o anel (ou o halo) de destaque. Clicar (ou Enter/
 * Espaço) num lugar ou numa rota com área troca a área. O desenho tem largura mínima e rola de lado dentro do cartão.
 */
export default function RegionMap({
  map,
  game,
  areas,
  states,
  current,
  onSelect,
}: {
  map: RegionMapData
  game: string
  areas: GameArea[]
  /** Estado por id de área (areaStates). */
  states: Record<string, AreaState>
  /** Id da área atual. */
  current: string | null
  onSelect?: (areaId: string) => void
}) {
  const { t } = useTranslation()
  const m = useMemo(() => mapForGame(map, game), [map, game])
  const byNode = useMemo(() => nodeAreas(areas), [areas])
  const vb = useMemo(() => viewBoxFor(m), [m])
  const currentNode = currentNodeId(areas, current)
  const scroller = useRef<HTMLDivElement>(null)
  const focus = useMemo(() => focusPoint(m, currentNode), [m, currentNode])

  // O mapa é mais largo que a tela (rola de lado): começa com a área atual no meio.
  useEffect(() => {
    const el = scroller.current
    if (!el || !focus || el.scrollWidth <= el.clientWidth) return
    const scale = el.scrollWidth / vb.w
    el.scrollLeft = Math.max(0, (focus[0] - vb.x) * scale - el.clientWidth / 2)
  }, [focus, vb])
  const select = (id: string) => {
    const first = byNode.get(id)?.[0]
    return first && onSelect ? () => onSelect(first) : undefined
  }

  return (
    <div className="flex flex-col gap-2">
      <div ref={scroller} className="scroll-x rounded-lg border border-line bg-map-sea">
        <svg
          viewBox={`${vb.x} ${vb.y} ${vb.w} ${vb.h}`}
          className="block h-auto w-full"
          style={{ minWidth: Math.round(vb.w) }}
          role="group"
          aria-label={t('nuzlocke.map.title')}
        >
          {m.terrain.map((ter, i) => (
            <polygon key={i} points={svgPoints(ter.polygon)} className={terrainClass[ter.kind]} strokeLinejoin="round" />
          ))}
          {m.routes.map((r) => (
            <Route key={r.id} route={r} state={nodeState(byNode.get(r.id), states)} current={r.id === currentNode} onSelect={select(r.id)} />
          ))}
          {m.places.map((p) => (
            <Place key={p.id} place={p} state={nodeState(byNode.get(p.id), states)} current={p.id === currentNode} onSelect={select(p.id)} />
          ))}
        </svg>
      </div>
      <Legend />
    </div>
  )
}

/** Props de botão do SVG (clique, Enter/Espaço) quando o elemento tem área. */
function buttonProps(name: string, current: boolean, onSelect?: () => void) {
  const clickable = !!onSelect
  return {
    role: clickable ? 'button' : undefined,
    tabIndex: clickable ? 0 : undefined,
    'aria-label': name,
    'aria-current': current ? ('location' as const) : undefined,
    onClick: onSelect,
    onKeyDown: (e: KeyboardEvent) => {
      if (clickable && (e.key === 'Enter' || e.key === ' ')) {
        e.preventDefault()
        onSelect()
      }
    },
    className: `outline-none ${clickable ? 'cursor-pointer [&:focus-visible_.focus]:stroke-ink' : ''}`,
  }
}

function Route({ route, state, current, onSelect }: { route: MapRoute; state: AreaState; current: boolean; onSelect?: () => void }) {
  const pts = svgPoints(route.path)
  const tag = routeTag(route)
  const link = !route.number && !onSelect
  return (
    <g {...buttonProps(route.name, current, onSelect)}>
      <title>{route.name}</title>
      {/* Área de toque maior que a linha. */}
      <polyline points={pts} fill="none" stroke="transparent" strokeWidth={24} />
      <polyline
        points={pts}
        fill="none"
        strokeWidth={ROUTE_OUTER + 10}
        strokeLinejoin="round"
        strokeLinecap="round"
        strokeOpacity={current ? 0.45 : 1}
        className={`focus ${current ? 'stroke-accent' : 'stroke-transparent'}`}
      />
      <polyline
        points={pts}
        fill="none"
        strokeWidth={link ? ROUTE_INNER - 2 : ROUTE_OUTER}
        strokeLinejoin="round"
        strokeLinecap={link && route.water ? 'butt' : 'round'}
        strokeDasharray={link && route.water ? '5 3' : undefined}
        className={route.water ? 'stroke-map-water-edge' : 'stroke-map-route-edge'}
      />
      {!link && (
        <polyline
          points={pts}
          fill="none"
          strokeWidth={ROUTE_INNER}
          strokeLinejoin="round"
          strokeLinecap={route.water && state === 'none' ? 'butt' : 'round'}
          // Água sem estado: tracejada em azul; com estado, a cor cheia do estado (o contorno continua azul).
          strokeDasharray={route.water && state === 'none' ? '7 4' : undefined}
          className={routeStroke(state, route.water)}
        />
      )}
      {tag && (
        <g>
          <rect
            x={tag.box.x}
            y={tag.box.y}
            width={tag.box.w}
            height={tag.box.h}
            rx={tag.box.h / 2}
            strokeWidth={current ? 2.5 : 1.5}
            className={`fill-surface ${current ? 'stroke-accent' : 'stroke-map-route-edge'}`}
          />
          <text x={tag.x} y={tag.y + 4} textAnchor="middle" fontSize={11} className="fill-ink font-extrabold">
            {tag.text}
          </text>
        </g>
      )}
    </g>
  )
}

function Shape({ place, className }: { place: MapPlace; className: string }) {
  const cx = place.x * CELL
  const cy = place.y * CELL
  const half = halfSize(place.kind)
  if (place.kind === 'place') return <circle cx={cx} cy={cy} r={half} className={className} />
  return <rect x={cx - half} y={cy - half} width={half * 2} height={half * 2} rx={place.kind === 'city' ? 4 : 2.5} className={className} />
}

function Place({ place, state, current, onSelect }: { place: MapPlace; state: AreaState; current: boolean; onSelect?: () => void }) {
  const cx = place.x * CELL
  const cy = place.y * CELL
  const half = halfSize(place.kind)
  const label = labelLayout(place)
  return (
    <g {...buttonProps(place.name, current, onSelect)}>
      <title>{place.name}</title>
      <circle cx={cx} cy={cy} r={half + 9} className="fill-transparent" />
      <circle cx={cx} cy={cy} r={half + 5} strokeWidth={3.5} className={`focus fill-none ${current ? 'stroke-accent' : 'stroke-transparent'}`} />
      <g strokeWidth={2.5}>
        <Shape place={place} className={stateClass(state)} />
      </g>
      <text
        x={label.x}
        y={label.y}
        textAnchor={label.anchor}
        fontSize={label.font}
        className={`fill-ink ${place.kind === 'city' ? 'font-extrabold' : 'font-bold'}`}
        style={halo}
      >
        {label.lines.map((line, i) => (
          <tspan key={i} x={label.x} dy={i === 0 ? 0 : label.font * 1.2}>
            {line}
          </tspan>
        ))}
      </text>
    </g>
  )
}

function Legend() {
  const { t } = useTranslation()
  const item = 'inline-flex items-center gap-1'
  return (
    <div className="flex flex-wrap gap-x-3 gap-y-1 text-[12px] text-muted">
      <span className={item}>
        <svg viewBox="0 0 12 12" className="h-3 w-3" aria-hidden>
          <rect x={1} y={1} width={10} height={10} rx={2} strokeWidth={1.5} className="fill-surface stroke-line" />
        </svg>
        {t('nuzlocke.map.legend.city')}
      </span>
      <span className={item}>
        <svg viewBox="0 0 12 12" className="h-3 w-3" aria-hidden>
          <circle cx={6} cy={6} r={5} strokeWidth={1.5} className="fill-surface stroke-line" />
        </svg>
        {t('nuzlocke.map.legend.place')}
      </span>
      <span className={item}>
        <svg viewBox="0 0 20 12" className="h-3 w-5" aria-hidden>
          <line x1={1} y1={6} x2={19} y2={6} strokeWidth={6} strokeLinecap="round" className="stroke-map-route-edge" />
          <line x1={1} y1={6} x2={19} y2={6} strokeWidth={3} strokeLinecap="round" className="stroke-surface" />
        </svg>
        {t('nuzlocke.map.legend.route')}
      </span>
      <span className={item}>
        <svg viewBox="0 0 20 12" className="h-3 w-5" aria-hidden>
          <line x1={1} y1={6} x2={19} y2={6} strokeWidth={6} className="stroke-map-water-edge" />
          <line x1={1} y1={6} x2={19} y2={6} strokeWidth={3} strokeDasharray="4 2" className="stroke-map-water" />
        </svg>
        {t('nuzlocke.map.legend.water')}
      </span>
      {MAP_STATES.map((s) => (
        <span key={s} className={item}>
          <svg viewBox="0 0 12 12" className="h-3 w-3" aria-hidden>
            <circle cx={6} cy={6} r={5} strokeWidth={1.5} className={stateClass(s)} />
          </svg>
          {t(`nuzlocke.map.states.${s}`)}
        </span>
      ))}
      <span className={item}>
        <svg viewBox="0 0 12 12" className="h-3 w-3" aria-hidden>
          <circle cx={6} cy={6} r={4.5} strokeWidth={2} className="fill-none stroke-accent" />
        </svg>
        {t('nuzlocke.map.current')}
      </span>
    </div>
  )
}
