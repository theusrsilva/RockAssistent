import { useTranslation } from 'react-i18next'
import { Link } from 'react-router'
import { Badge } from '../../../components/ui'
import { spriteUrl } from '../../../lib/api'
import { titleCase, useApiGet } from '../../../lib/dex'
import { summarizeEncounters, type AreaEncounterRow } from '../../../lib/encounters'
import { BlockState, BlockTable, cell, versionLabel, type BlockProps } from './common'

export type { AreaEncounterRow } from '../../../lib/encounters'

/**
 * :::encounters <jogo> <área> — total de espécies, faixa de sprites e a tabela (método, níveis, chance, versão), com as
 * linhas repetidas da mesma espécie juntadas. Acima de 6 linhas a tabela fica fechada num "ver todos".
 */
export default function EncounterTable({ args: [game, area] }: BlockProps) {
  const { t } = useTranslation()
  const state = useApiGet<AreaEncounterRow[]>(`/api/data/${encodeURIComponent(game)}/areas/${encodeURIComponent(area)}/encounters`)
  const method = (m: string) => t(`dex.method.${m}`, { defaultValue: titleCase(m) })
  return (
    <BlockState state={state} empty={(d) => d.length === 0}>
      {(data) => {
        const { rows, species, collapsed } = summarizeEncounters(data)
        const showChance = rows.some((r) => r.chance)
        const head = [t('guides.col.pokemon'), t('guides.col.method'), t('guides.col.levels'), ...(showChance ? [t('guides.col.chance')] : []), t('guides.col.versions')]
        const table = (
          <BlockTable head={head}>
            {rows.map((r, i) => (
              <tr key={i}>
                <td className={cell}>
                  <Link to={`/pokedex/${r.species}`} className="flex items-center gap-1.5 font-semibold hover:text-accent">
                    <img src={spriteUrl(r.species)} alt="" className="pixelated h-9 w-9 shrink-0" loading="lazy" />
                    <span className="whitespace-nowrap">{r.speciesName}</span>
                  </Link>
                  {(r.alpha || !r.verified) && (
                    <div className="mt-0.5 flex gap-1">
                      {r.alpha && <Badge tone="red">{t('guides.block.alpha')}</Badge>}
                      {!r.verified && <Badge>{t('guides.block.notVerified')}</Badge>}
                    </div>
                  )}
                </td>
                <td className={cell}>
                  {method(r.method)}
                  {r.conditions && <div className="text-[12px] text-muted">{r.conditions}</div>}
                </td>
                <td className={`${cell} whitespace-nowrap tabular-nums`}>
                  {r.minLevel === r.maxLevel ? r.minLevel : `${r.minLevel}–${r.maxLevel}`}
                </td>
                {showChance && <td className={`${cell} whitespace-nowrap tabular-nums`}>{r.chance ?? '—'}</td>}
                <td className={`${cell} text-muted`}>{r.versions.map((v) => versionLabel(game, v)).join(' / ')}</td>
              </tr>
            ))}
          </BlockTable>
        )
        return (
          <div className="my-3">
            <div className="flex flex-wrap items-center gap-x-2 gap-y-1">
              <span className="text-sm font-bold">{t('guides.encounters.total', { count: species.length })}</span>
              {rows.some((r) => !r.verified) && <Badge>{t('guides.block.notVerified')}</Badge>}
            </div>
            <div className="mt-1 flex flex-wrap gap-0.5">
              {species.map((s) => (
                <Link key={`${s.species}-${s.form}`} to={`/pokedex/${s.species}`} title={s.name} aria-label={s.name} className="rounded hover:bg-surface2">
                  <img src={spriteUrl(s.species)} alt="" className="pixelated h-10 w-10" loading="lazy" />
                </Link>
              ))}
            </div>
            {collapsed ? (
              <details className="mt-1">
                <summary className="cursor-pointer text-sm font-semibold text-accent">{t('guides.encounters.showAll', { count: rows.length })}</summary>
                {table}
              </details>
            ) : (
              table
            )}
          </div>
        )
      }}
    </BlockState>
  )
}
