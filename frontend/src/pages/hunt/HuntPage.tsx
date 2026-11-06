import { useMemo } from 'react'
import { useTranslation } from 'react-i18next'
import { Link, useNavigate } from 'react-router'
import { Badge, Button, Callout, PageTitle } from '../../components/ui'
import { spriteUrl } from '../../lib/api'
import { useApiGet } from '../../lib/dex'
import { GAME_VERSIONS } from '../../lib/gameVersions'
import { formatOdds, useSpeciesOptions, type Hunt } from '../../lib/hunts'

export const gameName = (key: string) => GAME_VERSIONS.find((g) => g.key === key)?.name ?? key

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

function HuntCard({ hunt, name }: { hunt: Hunt; name?: string }) {
  const { t, i18n } = useTranslation()
  return (
    <Link
      to={`/hunt/${hunt.id}`}
      className="flex items-center gap-3 rounded-xl border border-line bg-surface px-3.5 py-3 transition hover:bg-surface2"
    >
      <img src={spriteUrl(hunt.species, { shiny: hunt.found })} alt="" className="pixelated h-14 w-14 shrink-0" loading="lazy" />
      <div className="min-w-0 flex-1">
        <div className="truncate font-bold">{name ?? `#${hunt.species}`}</div>
        <div className="truncate text-[12.5px] text-muted">
          {gameName(hunt.gameKey)} · {hunt.methodName}
        </div>
        <div className="mt-1 flex flex-wrap gap-1.5">
          {hunt.endedAt ? (
            hunt.found ? (
              <Badge tone="emerald" icon="auto_awesome">
                {t('hunt.found')}
              </Badge>
            ) : (
              <Badge tone="slate">{t('hunt.gaveUp')}</Badge>
            )
          ) : (
            <Badge tone="accent">{t('hunt.oneIn', { odds: formatOdds(hunt.odds, i18n.language) })}</Badge>
          )}
        </div>
      </div>
      <div className="text-right">
        <div className="font-display text-2xl leading-none font-bold tabular-nums">{hunt.count.toLocaleString(i18n.language)}</div>
        <div className="text-[11.5px] text-muted">{t('hunt.encounters')}</div>
      </div>
    </Link>
  )
}
