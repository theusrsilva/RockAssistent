import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Link } from 'react-router'
import { Button, Callout, Card, Icon } from '../../../components/ui'
import { ApiError, spriteUrl } from '../../../lib/api'
import { useIsRemote } from '../../../lib/auth'
import { useApiGet } from '../../../lib/dex'
import { formatPercent, internetErrorKey, refreshUsage, type InternetSettings, type UsageEntry, type UsageReport, type UsageShare } from '../../../lib/vgc'

/** "Close Combat 99,9%" em linha. */
function ShareList({ items, lang, empty = '—' }: { items: UsageShare[]; lang: string; empty?: string }) {
  const { t } = useTranslation()
  if (items.length === 0) return <span className="text-muted">{empty}</span>
  return (
    <span>
      {items.map((s, i) => (
        <span key={s.id} className="whitespace-nowrap">
          {s.id === 'nothing' ? t('vgc.usage.noItem') : s.name} <span className="text-muted">{formatPercent(s.percent, lang)}</span>
          {i < items.length - 1 && <span className="text-muted">, </span>}
        </span>
      ))}
    </span>
  )
}

function UsageRow({ entry, lang }: { entry: UsageEntry; lang: string }) {
  const { t } = useTranslation()
  const [open, setOpen] = useState(false)
  return (
    <li className="rounded-lg border border-line bg-surface px-2.5 py-2">
      <button type="button" aria-expanded={open} onClick={() => setOpen(!open)} className="flex w-full cursor-pointer items-start gap-2 text-left">
        <span className="w-6 shrink-0 pt-2 text-right text-[12px] font-bold text-muted">{entry.rank}</span>
        {entry.species > 0 ? (
          <img src={spriteUrl(entry.species)} alt="" className="size-10 shrink-0 [image-rendering:pixelated]" loading="lazy" />
        ) : (
          <span className="size-10 shrink-0" />
        )}
        <span className="flex min-w-0 flex-1 flex-col gap-0.5 text-[13px]">
          <span className="flex flex-wrap items-baseline gap-x-2">
            <span className="font-semibold">{entry.name}</span>
            <span className="font-bold text-accent">{formatPercent(entry.usage, lang)}</span>
          </span>
          <span className="truncate text-muted">
            {entry.moves
              .slice(0, 4)
              .map((m) => m.name)
              .join(' · ')}
          </span>
          <span className="truncate text-[12px] text-muted">
            {[entry.items[0] && (entry.items[0].id === 'nothing' ? t('vgc.usage.noItem') : entry.items[0].name), entry.abilities[0]?.name]
              .filter(Boolean)
              .join(' · ')}
          </span>
        </span>
        <Icon name={open ? 'expand_less' : 'expand_more'} className="shrink-0 pt-2 text-[20px] text-muted" />
      </button>
      {open && (
        <dl className="mt-2 grid gap-1.5 border-t border-line pt-2 text-[12.5px] sm:grid-cols-[110px_1fr]">
          <dt className="font-bold">{t('vgc.usage.moves')}</dt>
          <dd>
            <ShareList items={entry.moves} lang={lang} />
          </dd>
          <dt className="font-bold">{t('vgc.usage.items')}</dt>
          <dd>
            <ShareList items={entry.items} lang={lang} />
          </dd>
          <dt className="font-bold">{t('vgc.usage.abilities')}</dt>
          <dd>
            <ShareList items={entry.abilities} lang={lang} />
          </dd>
          <dt className="font-bold">{t('vgc.usage.teammates')}</dt>
          <dd>
            <ShareList items={entry.teammates} lang={lang} />
          </dd>
        </dl>
      )}
    </li>
  )
}

/** Aba "Mais usados" da seção VGC: as estatísticas do Smogon já baixadas e o botão de atualizar (Internet opcional). */
export default function UsageView() {
  const { t, i18n } = useTranslation()
  const remote = useIsRemote()
  const saved = useApiGet<UsageReport>('/api/vgc/usage')
  const settings = useApiGet<InternetSettings>('/api/settings/internet')
  const [fresh, setFresh] = useState<UsageReport | null>(null)
  const [busy, setBusy] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const report = fresh ?? saved.data
  const missing = saved.error instanceof ApiError && saved.error.status === 404
  const enabled = !!settings.data?.smogon
  const lang = i18n.language

  async function refresh() {
    setBusy(true)
    setError(null)
    try {
      setFresh(await refreshUsage())
    } catch (e) {
      const key = internetErrorKey(e)
      const reason = e instanceof ApiError ? e.reason : null
      setError(key === 'network' && reason ? t('vgc.usage.errors.networkCode', { code: reason }) : t(`vgc.usage.errors.${key}`))
    } finally {
      setBusy(false)
    }
  }

  return (
    <div className="flex flex-col gap-3">
      <Card className="flex flex-col gap-2">
        <div className="flex flex-wrap items-center gap-2">
          <div className="min-w-[200px] flex-1">
            <h2 className="font-display text-lg font-bold">{t('vgc.usage.title')}</h2>
            <p className="text-[13px] text-muted">
              {report
                ? t('vgc.usage.sub', { format: report.format, month: report.month || '?', battles: report.battles.toLocaleString(lang) })
                : t('vgc.usage.subEmpty')}
            </p>
          </div>
          {!remote && (
            <Button variant="secondary" size="sm" icon="refresh" disabled={busy || !enabled} onClick={() => void refresh()}>
              {busy ? t('vgc.usage.refreshing') : t('vgc.usage.refresh')}
            </Button>
          )}
        </div>
        {!remote && !enabled && !settings.loading && (
          <p className="text-[12.5px] text-muted">
            {t(report ? 'vgc.usage.disabledHint' : 'vgc.usage.howToEnable')}{' '}
            <Link to="/configuracoes?aba=preferencias#internet" className="font-semibold text-accent">
              {t('vgc.usage.openSettings')}
            </Link>
          </p>
        )}
        <p className="text-[12px] text-muted">{t('vgc.usage.source')}</p>
        {error && (
          <Callout tone="amber" icon="cloud_off" className="text-[13px]">
            {error}
          </Callout>
        )}
      </Card>

      {saved.loading && !report ? (
        <p className="text-muted">{t('common.loading')}</p>
      ) : report ? (
        <ol className="flex flex-col gap-1.5">
          {report.pokemon.map((entry) => (
            <UsageRow key={entry.name} entry={entry} lang={lang} />
          ))}
        </ol>
      ) : missing ? (
        <Callout tone="sky" icon="info">
          {remote ? t('vgc.usage.emptyRemote') : enabled ? t('vgc.usage.emptyEnabled') : t('vgc.usage.empty')}
        </Callout>
      ) : (
        <Callout tone="amber" icon="cloud_off">
          {t('guides.block.error')}
        </Callout>
      )}
    </div>
  )
}
