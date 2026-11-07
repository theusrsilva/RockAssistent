import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Link, useNavigate } from 'react-router'
import { Badge, Button, Callout, PageTitle, type Tone } from '../../components/ui'
import { spriteUrl } from '../../lib/api'
import { useApiGet, versionName } from '../../lib/dex'
import { gameName } from '../hunt/HuntPage'
import NewRunDialog from './NewRunDialog'
import { isFinished, type RunStatus, type RunSummary, type RunView } from './nuzlocke'

export const statusTone: Record<RunStatus, Tone> = { active: 'accent', paused: 'amber', champion: 'emerald', wipe: 'red' }

/** Jogo e versão escolhida ("FireRed / LeafGreen · FireRed"). */
export const runGameLabel = (gameKey: string, version: string | null) =>
  version ? `${gameName(gameKey)} · ${versionName(version)}` : gameName(gameKey)

/** Nuzlocke: runs em andamento (ativas e pausadas) e terminadas (campeão ou wipe). */
export default function NuzlockePage() {
  const { t } = useTranslation()
  const navigate = useNavigate()
  const list = useApiGet<RunSummary[]>('/api/nuzlocke')
  const [creating, setCreating] = useState(false)

  const runs = list.data ?? []
  const active = runs.filter((r) => !isFinished(r.status))
  const finished = runs.filter((r) => isFinished(r.status))

  return (
    <div className="flex flex-col gap-5">
      <PageTitle
        actions={
          <Button icon="add" onClick={() => setCreating(true)}>
            {t('nuzlocke.new')}
          </Button>
        }
      >
        {t('nuzlocke.title')}
      </PageTitle>
      <p className="-mt-3 text-sm text-muted">{t('nuzlocke.sub')}</p>

      {list.error ? (
        <Callout tone="red" icon="error">
          {t('nuzlocke.loadError')}
        </Callout>
      ) : list.loading ? (
        <p className="text-sm text-muted">{t('common.loading')}</p>
      ) : (
        <>
          <Section title={t('nuzlocke.active')} empty={t('nuzlocke.emptyActive')}>
            {active.map((r) => (
              <RunCard key={r.id} run={r} />
            ))}
          </Section>
          <Section title={t('nuzlocke.finished')} empty={t('nuzlocke.emptyFinished')}>
            {finished.map((r) => (
              <RunCard key={r.id} run={r} />
            ))}
          </Section>
        </>
      )}

      {creating && <NewRunDialog onClose={() => setCreating(false)} onCreated={(id) => navigate(`/nuzlocke/${id}`)} />}
    </div>
  )
}

function Section({ title, empty, children }: { title: string; empty: string; children: React.ReactNode[] }) {
  return (
    <section className="flex flex-col gap-2.5">
      <h2 className="font-display text-lg font-bold">{title}</h2>
      {children.length === 0 ? (
        <p className="text-sm text-muted">{empty}</p>
      ) : (
        <div className="grid grid-cols-1 gap-2.5 sm:grid-cols-2 xl:grid-cols-3">{children}</div>
      )}
    </section>
  )
}

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
