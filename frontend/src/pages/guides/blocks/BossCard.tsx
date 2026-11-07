import { useTranslation } from 'react-i18next'
import { Badge, Callout } from '../../../components/ui'
import { useApiGet } from '../../../lib/dex'
import type { BossBattle } from '../../nuzlocke/nuzlocke'
import { BattlePokemon } from '../../nuzlocke/parts'
import { useSpeciesNames } from './BossTable'
import { BossVariantTabs, useBossVariant } from './BossVariants'
import { BlockState, versionLabel, type BlockProps } from './common'

/** :::boss <jogo> <id> — um chefe: time com sprites, níveis, golpes, habilidade e item. */
export default function BossCard({ args: [game, id] }: BlockProps) {
  const { t } = useTranslation()
  const state = useApiGet<BossBattle[]>(`/api/data/${encodeURIComponent(game)}/bosses`)
  const names = useSpeciesNames()
  return (
    <BlockState state={state}>
      {(bosses) => {
        const boss = bosses.find((b) => b.id === id)
        if (!boss)
          return (
            <Callout tone="amber" icon="help" className="my-3">
              {t('guides.block.empty')}
            </Callout>
          )
        return <Boss game={game} boss={boss} names={names} />
      }}
    </BlockState>
  )
}

/** O cartão; com variantes por inicial, abas "Se você escolheu X" em cima do time (o cap acompanha). */
function Boss({ game, boss, names }: { game: string; boss: BossBattle; names: Map<number, string> }) {
  const { t } = useTranslation()
  const { starter, setStarter, team } = useBossVariant(boss)
  return (
    <div className="my-3 flex flex-col gap-2 rounded-lg border border-line bg-surface p-3">
      <div className="flex flex-wrap items-start gap-2">
        <div className="min-w-0 flex-1">
          <div className="font-bold">{boss.name}</div>
          <div className="text-[12.5px] text-muted">{[boss.title, boss.location].filter(Boolean).join(' · ')}</div>
          <div className="mt-1 flex flex-wrap gap-1">
            {boss.badge && <Badge tone="amber" icon="military_tech">{boss.badge}</Badge>}
            {boss.versions && <Badge>{boss.versions.map((v) => versionLabel(game, v)).join(' / ')}</Badge>}
            {!team.verified && <Badge>{t('guides.block.notVerified')}</Badge>}
          </div>
        </div>
        <div className="rounded-lg bg-accent-soft px-3 py-1.5 text-center">
          <div className="text-[11px] font-bold tracking-wide text-muted uppercase">{t('guides.col.cap')}</div>
          <div className="font-display text-xl leading-none font-bold tabular-nums">{team.levelCap}</div>
        </div>
      </div>
      <BossVariantTabs game={game} boss={boss} names={names} value={starter} onChange={setStarter} />
      <div className="grid gap-1.5 sm:grid-cols-2">
        {team.party.map((p, i) => (
          <BattlePokemon key={i} p={p} names={names} />
        ))}
      </div>
    </div>
  )
}
