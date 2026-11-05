import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Link } from 'react-router'
import { sortMoves, type LearnedMove, type MoveSortKey, type SortDir, type SpeciesDetail, type SpeciesMoves } from '../../../lib/dex'
import { Callout, Icon, Segmented, useIsMobile } from '../../../components/ui'
import { TypeBadge } from '../TypeBadge'

type Group = keyof SpeciesMoves

const firstColumn: Record<Group, MoveSortKey | null> = { levelUp: 'level', machine: 'machine', tutor: null, egg: null }

/** Golpes do jogo por jeito de aprender, com tabela ordenável (no celular, lista compacta). */
export function MovesTab({ detail }: { detail: SpeciesDetail }) {
  const { t } = useTranslation()
  const mobile = useIsMobile()
  const [group, setGroup] = useState<Group>('levelUp')
  const [sort, setSort] = useState<{ key: MoveSortKey; dir: SortDir } | null>(null)
  const lead = firstColumn[group]
  const active = sort ?? { key: lead ?? 'name', dir: 'asc' as SortDir }
  const moves = sortMoves(detail.moves[group], active.key, active.dir)

  const chooseGroup = (g: Group) => {
    setGroup(g)
    setSort(null)
  }
  const toggle = (key: MoveSortKey) =>
    setSort(active.key === key ? { key, dir: active.dir === 'asc' ? 'desc' : 'asc' } : { key, dir: key === 'power' ? 'desc' : 'asc' })

  const groups: Group[] = ['levelUp', 'machine', 'tutor', 'egg']
  const leadCell = (m: LearnedMove) => (group === 'levelUp' ? (m.level ?? '—') : (m.machine ?? '—'))
  const className = (m: LearnedMove) => (m.class ? t(`dex.class.${m.class}`, { defaultValue: m.class }) : '—')

  const header = (key: MoveSortKey, label: string, align = 'text-left') => (
    <th className={`px-2 py-1.5 ${align}`}>
      <button type="button" onClick={() => toggle(key)} className="inline-flex cursor-pointer items-center gap-0.5 font-semibold hover:text-ink">
        {label}
        {active.key === key && <Icon name={active.dir === 'asc' ? 'arrow_upward' : 'arrow_downward'} className="text-[14px]" />}
      </button>
    </th>
  )

  return (
    <div className="flex flex-col gap-3">
      <Segmented
        value={group}
        onChange={chooseGroup}
        className="overflow-x-auto"
        options={groups.map((g) => ({ value: g, label: `${t(`dex.moves.${g}`)} (${detail.moves[g].length})` }))}
      />
      {moves.length === 0 && (
        <Callout tone="sky" icon="info">
          {t('dex.moves.empty')}
        </Callout>
      )}

      {moves.length > 0 && mobile && (
        <ul className="flex flex-col divide-y divide-line rounded-xl border border-line bg-surface">
          {moves.map((m, i) => (
            <li key={`${m.id}-${i}`} className="flex items-center gap-2 px-3 py-2 text-sm">
              {lead && <span className="w-10 shrink-0 font-mono text-xs text-muted">{leadCell(m)}</span>}
              <div className="min-w-0 flex-1">
                <Link to={`/pokedex/golpe/${m.id}`} className="block truncate font-semibold hover:text-accent">
                  {m.name}
                </Link>
                <div className="text-xs text-muted">
                  {className(m)} · {m.power ?? '—'} / {m.accuracy ?? '—'}% · {m.pp ?? '—'} PP
                </div>
              </div>
              <TypeBadge type={m.type} small />
            </li>
          ))}
        </ul>
      )}

      {moves.length > 0 && !mobile && (
        <div className="overflow-x-auto rounded-xl border border-line bg-surface">
          <table className="w-full text-sm">
            <thead className="border-b border-line text-xs text-muted">
              <tr>
                {lead && header(lead, t(`dex.moves.col.${lead}`))}
                {header('name', t('dex.moves.col.name'))}
                {header('type', t('dex.moves.col.type'))}
                {header('class', t('dex.moves.col.class'))}
                {header('power', t('dex.moves.col.power'), 'text-right')}
                {header('accuracy', t('dex.moves.col.accuracy'), 'text-right')}
                {header('pp', t('dex.moves.col.pp'), 'text-right')}
              </tr>
            </thead>
            <tbody className="divide-y divide-line">
              {moves.map((m, i) => (
                <tr key={`${m.id}-${i}`}>
                  {lead && <td className="px-2 py-1.5 font-mono text-xs text-muted">{leadCell(m)}</td>}
                  <td className="px-2 py-1.5 font-semibold">
                    <Link to={`/pokedex/golpe/${m.id}`} className="hover:text-accent hover:underline">
                      {m.name}
                    </Link>
                  </td>
                  <td className="px-2 py-1.5">
                    <TypeBadge type={m.type} small />
                  </td>
                  <td className="px-2 py-1.5 text-muted">{className(m)}</td>
                  <td className="px-2 py-1.5 text-right font-mono">{m.power ?? '—'}</td>
                  <td className="px-2 py-1.5 text-right font-mono">{m.accuracy != null ? `${m.accuracy}%` : '—'}</td>
                  <td className="px-2 py-1.5 text-right font-mono">{m.pp ?? '—'}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  )
}
