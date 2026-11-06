import { useTranslation } from 'react-i18next'
import { Link } from 'react-router'
import { Badge } from '../../../components/ui'
import { spriteUrl } from '../../../lib/api'
import { titleCase, useApiGet } from '../../../lib/dex'
import { BlockState, BlockTable, cell, versionLabel, type BlockProps } from './common'

/** Linha de GET /api/data/{jogo}/areas/{área}/encounters (TradeRock.Core/GameData/AreaEncounters.cs). */
export interface AreaEncounterRow {
  species: number
  form: number
  speciesName: string
  method: string
  minLevel: number
  maxLevel: number
  chance: string | null
  conditions: string | null
  versions: string[]
  alpha: boolean
  verified: boolean
}

/** :::encounters <jogo> <área> — espécies da área com método, níveis, chance e versão. */
export default function EncounterTable({ args: [game, area] }: BlockProps) {
  const { t } = useTranslation()
  const state = useApiGet<AreaEncounterRow[]>(`/api/data/${encodeURIComponent(game)}/areas/${encodeURIComponent(area)}/encounters`)
  const method = (m: string) => t(`dex.method.${m}`, { defaultValue: titleCase(m) })
  return (
    <BlockState state={state} empty={(d) => d.length === 0}>
      {(rows) => {
        const showChance = rows.some((r) => r.chance)
        const head = [t('guides.col.pokemon'), t('guides.col.method'), t('guides.col.levels'), ...(showChance ? [t('guides.col.chance')] : []), t('guides.col.versions')]
        return (
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
      }}
    </BlockState>
  )
}
