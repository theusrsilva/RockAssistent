import { useEffect, useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Link } from 'react-router'
import { Icon, PageTitle } from '../../components/ui'
import { api } from '../../lib/api'
import { useAuth } from '../../lib/auth'
import type { ActivitySnapshot } from '../../lib/types'
import { ActivityPanel } from '../activities/ActivityPanel'

type Open = 'raid' | 'circle'

const tileClass = 'flex flex-col items-start gap-2.5 rounded-xl border bg-surface p-5 text-left hover:border-accent'

/** Trocas: a troca com o Switch e, só na janela do PC (D26), as raids e o Union Circle. */
export default function TradesHubPage() {
  const { t } = useTranslation()
  const { me } = useAuth()
  const remote = me?.local === false
  const [open, setOpen] = useState<Open | null>(null)

  // Raid ou Union Circle em andamento: abre o cartão dela para acompanhar.
  useEffect(() => {
    if (remote) return
    api.get<ActivitySnapshot | null>('/api/activities/current').then(
      (s) => {
        if (s?.status === 'running' && s.kind !== 'gift') setOpen((o) => o ?? (s.kind as Open))
      },
      () => {},
    )
  }, [remote])

  const cards: { id: Open; icon: string }[] = [
    { id: 'raid', icon: 'swords' },
    { id: 'circle', icon: 'group' },
  ]

  return (
    <div className="flex flex-col gap-5">
      <PageTitle>{t('trades.title')}</PageTitle>
      <p className="-mt-2 text-sm text-muted">{t('trades.sub')}</p>

      <div className="grid gap-[18px] [grid-template-columns:repeat(auto-fit,minmax(min(240px,100%),1fr))]">
        <Link to="/trocas/trocar" className={`${tileClass} border-line`}>
          <TileBody icon="swap_horiz" title={t('trades.trade.title')} sub={t('trades.trade.sub')} />
        </Link>
        {!remote &&
          cards.map((c) => (
            <button
              key={c.id}
              type="button"
              aria-pressed={open === c.id}
              onClick={() => setOpen(open === c.id ? null : c.id)}
              className={`${tileClass} cursor-pointer ${open === c.id ? 'border-accent' : 'border-line'}`}
            >
              <TileBody icon={c.icon} title={t(`trades.${c.id}.title`)} sub={t(`trades.${c.id}.sub`)} />
            </button>
          ))}
      </div>

      {!remote && open && <ActivityPanel key={open} only={[open]} />}
    </div>
  )
}

function TileBody({ icon, title, sub }: { icon: string; title: string; sub: string }) {
  return (
    <>
      <span className="flex h-11 w-11 items-center justify-center rounded-lg bg-accent-soft text-accent">
        <Icon name={icon} className="text-[26px]" />
      </span>
      <span className="font-display text-lg font-bold">{title}</span>
      <span className="text-sm text-muted">{sub}</span>
    </>
  )
}
