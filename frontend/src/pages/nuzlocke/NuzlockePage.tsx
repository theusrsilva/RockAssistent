import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import { useNavigate } from 'react-router'
import { RunCard } from '../../components/RunCard'
import { Button, Callout, PageTitle } from '../../components/ui'
import { useApiGet } from '../../lib/dex'
import NewRunDialog from './NewRunDialog'
import { isFinished, type RunSummary } from './nuzlocke'

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
