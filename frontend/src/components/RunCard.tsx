import { useTranslation } from 'react-i18next'
import { Link } from 'react-router'
import { spriteUrl } from '../lib/api'
import { useApiGet, versionName } from '../lib/dex'
import type { RunStatus, RunSummary, RunView } from '../pages/nuzlocke/nuzlocke'
import { gameName } from './HuntCard'
import { Badge, type Tone } from './ui'

// Fora de pages/nuzlocke para a HomePage não puxar a página do Nuzlocke para o chunk principal.

export const statusTone: Record<RunStatus, Tone> = { active: 'accent', paused: 'amber', champion: 'emerald', wipe: 'red' }

/** Jogo e versão escolhida ("FireRed / LeafGreen · FireRed"). */
export const runGameLabel = (gameKey: string, version: string | null) =>
  version ? `${gameName(gameKey)} · ${versionName(version)}` : gameName(gameKey)

/** Cartão da run. O resumo não traz a equipe: os sprites vêm da tela da run (poucas runs, um pedido cada). */
export function RunCard({ run }: { run: RunSummary }) {
  const { t } = useTranslation()
  const detail = useApiGet<RunView>(`/api/nuzlocke/${run.id}`)
  const team = detail.data?.party ?? []
  return (
    <Link to={`/nuzlocke/${run.id}`} className="flex flex-col gap-2 rounded-xl border border-line bg-surface px-3.5 py-3 transition hover:bg-surface2">
      <div className="flex items-start gap-2">
        <div className="min-w-0 flex-1">
          <div className="truncate font-bold">{run.name}</div>
          <div className="truncate text-[12.5px] text-muted">{runGameLabel(run.gameKey, run.version)}</div>
        </div>
        <Badge tone={statusTone[run.status]}>{t(`nuzlocke.status.${run.status}`)}</Badge>
      </div>
      <div className="flex min-h-10 flex-wrap items-center gap-0.5">
        {team.length === 0 ? (
          <span className="text-[12.5px] text-muted">{detail.loading ? '' : t('nuzlocke.noTeam')}</span>
        ) : (
          team.map((m) => <img key={m.id} src={spriteUrl(m.species, { shiny: m.shiny })} alt={m.nickname ?? ''} title={m.nickname ?? undefined} className="pixelated h-10 w-10" loading="lazy" />)
        )}
      </div>
      <div className="flex flex-wrap gap-1.5">
        <Badge tone={run.deadCount ? 'red' : 'slate'} icon="skull">
          {t('nuzlocke.deaths', { count: run.deadCount })}
        </Badge>
        <Badge icon="military_tech">{t('nuzlocke.bossesDefeated', { count: run.bossesDefeated })}</Badge>
      </div>
    </Link>
  )
}
