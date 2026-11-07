import { useMemo } from 'react'
import { useTranslation } from 'react-i18next'
import { useNavigate } from 'react-router'
import { HuntCard } from '../../components/HuntCard'
import { Button, Callout, PageTitle } from '../../components/ui'
import { useApiGet } from '../../lib/dex'
import { useSpeciesOptions, type Hunt } from '../../lib/hunts'

/** Contador Hunt: hunts em andamento e terminadas (achadas ou desistidas). */
export default function HuntPage() {
  const { t } = useTranslation()
  const navigate = useNavigate()
  const list = useApiGet<Hunt[]>('/api/hunts?all=1')
  const species = useSpeciesOptions()
  const names = useMemo(() => new Map((species ?? []).map((s) => [s.value, s.text])), [species])

  const active = (list.data ?? []).filter((h) => !h.endedAt)
  const finished = (list.data ?? []).filter((h) => h.endedAt)

  return (
    <div className="flex flex-col gap-5">
      <PageTitle
        actions={
          <Button icon="add" onClick={() => navigate('/hunt/nova')}>
            {t('hunt.new')}
          </Button>
        }
      >
        {t('hunt.title')}
      </PageTitle>
      <p className="-mt-3 text-sm text-muted">{t('hunt.sub')}</p>

      {list.error ? (
        <Callout tone="red" icon="error">
          {t('hunt.loadError')}
        </Callout>
      ) : list.loading ? (
        <p className="text-sm text-muted">{t('common.loading')}</p>
      ) : (
        <>
          <Section title={t('hunt.active')} empty={t('hunt.emptyActive')}>
            {active.map((h) => (
              <HuntCard key={h.id} hunt={h} name={names.get(h.species)} />
            ))}
          </Section>
          <Section title={t('hunt.finished')} empty={t('hunt.emptyFinished')}>
            {finished.map((h) => (
              <HuntCard key={h.id} hunt={h} name={names.get(h.species)} />
            ))}
          </Section>
        </>
      )}
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
