import { useTranslation } from 'react-i18next'
import { Badge } from '../../../components/ui'
import { useApiGet } from '../../../lib/dex'
import { gameVersionByKey } from '../../../lib/gameVersions'
import { formatOdds, shinyName, type ShinyModifier } from '../../../lib/hunts'
import { BlockState, BlockTable, cell, type BlockProps } from './common'

/** Método de GET /api/data/shiny-methods (ShinyMethod do backend). */
export interface ShinyMethodInfo {
  id: string
  name: string
  nameEn: string
  description: string
  baseOdds: number
  modifiers: ShinyModifier[]
  games: string[]
  notes: string | null
  verified: boolean
}

/** :::shiny-methods [jogo] — métodos de shiny com a chance base e os modificadores (todos os jogos, ou um). */
export default function ShinyMethodsTable({ args: [game] }: BlockProps) {
  const { t, i18n } = useTranslation()
  const state = useApiGet<ShinyMethodInfo[]>(`/api/data/shiny-methods${game ? `?game=${encodeURIComponent(game)}` : ''}`)
  const odds = (n: number) => `1/${formatOdds(n, i18n.language)}`
  const name = (x: { name: string; nameEn: string }) => shinyName(x, i18n.language)
  const modifier = (m: ShinyModifier) =>
    m.kind === 'steps' && m.steps?.length
      ? `${name(m)}: ${m.steps.map((s) => `${s.label} ${odds(s.odds)}`).join(' · ')}`
      : `${name(m)}${m.odds ? `: ${odds(m.odds)}` : ''}`
  return (
    <BlockState state={state} empty={(d) => d.length === 0}>
      {(methods) => (
        <BlockTable head={[t('guides.col.shinyMethod'), t('guides.col.baseOdds'), t('guides.col.modifiers'), ...(game ? [] : [t('guides.col.games')])]}>
          {methods.map((m) => (
            <tr key={m.id}>
              <td className={`${cell} min-w-44`}>
                <div className="font-bold">{name(m)}</div>
                <div className="text-[12px] text-muted">{m.description}</div>
                {!m.verified && (
                  <div className="mt-0.5">
                    <Badge>{t('guides.block.notVerified')}</Badge>
                  </div>
                )}
              </td>
              <td className={`${cell} font-bold whitespace-nowrap tabular-nums`}>{odds(m.baseOdds)}</td>
              <td className={`${cell} min-w-48`}>
                {m.modifiers.length === 0 ? (
                  '—'
                ) : (
                  <ul className="list-disc pl-4">
                    {m.modifiers
                      .filter((x) => !game || !x.games || x.games.includes(game))
                      .map((x) => (
                        <li key={x.id}>{modifier(x)}</li>
                      ))}
                  </ul>
                )}
              </td>
              {!game && <td className={`${cell} min-w-40 text-[12px] text-muted`}>{m.games.map((g) => gameVersionByKey(g)?.name ?? g).join(', ')}</td>}
            </tr>
          ))}
        </BlockTable>
      )}
    </BlockState>
  )
}
