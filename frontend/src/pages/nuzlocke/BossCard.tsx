import { useTranslation } from 'react-i18next'
import { Badge, Button, Card } from '../../components/ui'
import { useApiGet } from '../../lib/dex'
import type { BossBattle, RunView } from './nuzlocke'
import { BattlePokemon, CardTitle } from './parts'

/** Próximo chefe: time, level cap em destaque e "Venci!"; os já vencidos ficam numa lista fechada. */
export default function BossCard({ view, names, readOnly, onDefeat }: { view: RunView; names: Map<number, string>; readOnly: boolean; onDefeat: (bossId: string) => void }) {
  const { t } = useTranslation()
  const bosses = useApiGet<BossBattle[]>(`/api/data/${view.run.gameKey}/bosses`)
  const byId = new Map((bosses.data ?? []).map((b) => [b.id, b]))
  const boss = view.nextBoss

  return (
    <Card className="flex flex-col gap-3">
      <CardTitle icon="swords">{t('nuzlocke.boss.title')}</CardTitle>
      {!boss ? (
        <p className="text-sm text-muted">{t('nuzlocke.boss.allDefeated')}</p>
      ) : (
        <>
          <div className="flex items-start gap-3">
            <div className="min-w-0 flex-1">
              <div className="font-bold">{boss.name}</div>
              <div className="text-[12.5px] text-muted">{[boss.title, boss.location].filter(Boolean).join(' · ')}</div>
              <div className="mt-1 flex flex-wrap gap-1">
                {boss.badge && <Badge tone="amber" icon="military_tech">{boss.badge}</Badge>}
                {!boss.verified && <Badge>{t('nuzlocke.boss.notVerified')}</Badge>}
              </div>
            </div>
            {view.levelCap != null && (
              <div className="rounded-lg bg-accent-soft px-3 py-1.5 text-center">
                <div className="text-[11px] font-bold tracking-wide text-muted uppercase">{t('nuzlocke.boss.cap')}</div>
                <div className="font-display text-2xl leading-none font-bold tabular-nums">{view.levelCap}</div>
              </div>
            )}
          </div>
          <div className="flex flex-col gap-1.5">
            {boss.party.map((p, i) => (
              <BattlePokemon key={i} p={p} names={names} />
            ))}
          </div>
          {boss.notes && <p className="text-[12.5px] text-muted">{boss.notes}</p>}
          {!readOnly && (
            <Button icon="emoji_events" onClick={() => onDefeat(boss.id)}>
              {t('nuzlocke.boss.defeat')}
            </Button>
          )}
        </>
      )}
      {view.defeatedBosses.length > 0 && (
        <details className="text-sm">
          <summary className="cursor-pointer font-semibold text-muted">{t('nuzlocke.boss.defeatedList', { count: view.defeatedBosses.length })}</summary>
          <ul className="mt-1.5 flex flex-col gap-1">
            {view.defeatedBosses.map((id) => {
              const b = byId.get(id)
              return (
                <li key={id} className="flex items-center gap-1.5">
                  <span aria-hidden className="icon text-[17px] text-ok">
                    check_circle
                  </span>
                  <span className="font-semibold">{b?.name ?? id}</span>
                  {b?.title && <span className="text-muted">· {b.title}</span>}
                </li>
              )
            })}
          </ul>
        </details>
      )}
    </Card>
  )
}
