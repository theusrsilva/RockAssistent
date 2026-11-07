import { useMemo, useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Link } from 'react-router'
import { Badge, Segmented } from '../../../components/ui'
import { spriteUrl } from '../../../lib/api'
import { useApiGet } from '../../../lib/dex'
import { gameVersionByKey } from '../../../lib/gameVersions'
import { useSpeciesOptions } from '../../../lib/hunts'
import type { BossBattle } from '../../nuzlocke/nuzlocke'
import { BossVariantTabs, useBossVariant } from './BossVariants'
import { BlockState, BlockTable, cell, versionLabel, type BlockProps } from './common'

/** Nomes das espécies (a mesma lista do Nuzlocke e do Hunt, pedida uma vez). */
export function useSpeciesNames() {
  const options = useSpeciesOptions()
  return useMemo(() => new Map((options ?? []).map((s) => [s.value, s.text])), [options])
}

/** Chefes de uma versão: sem lista de versões = todas. */
export const inVersion = (versions: string[] | null, version: string | null) => !version || !versions || versions.includes(version)

/** :::bosses <jogo> — todos os chefes em ordem: nome, local, level cap e time (com abas quando o time depende do inicial). */
export default function BossTable({ args: [game] }: BlockProps) {
  const { t } = useTranslation()
  const state = useApiGet<BossBattle[]>(`/api/data/${encodeURIComponent(game)}/bosses`)
  const names = useSpeciesNames()
  const versions = gameVersionByKey(game)?.pokeApiVersions ?? []
  const [version, setVersion] = useState<string>('')
  return (
    <BlockState state={state} empty={(d) => d.length === 0}>
      {(bosses) => {
        const split = versions.length === 2 && bosses.some((b) => b.versions)
        const shown = [...bosses].sort((a, b) => a.order - b.order).filter((b) => inVersion(b.versions, split ? version || null : null))
        return (
          <>
            {split && (
              <Segmented
                className="my-2"
                value={version}
                onChange={setVersion}
                options={[{ value: '', label: t('guides.block.allVersions') }, ...versions.map((v) => ({ value: v, label: versionLabel(game, v) }))]}
              />
            )}
            <BlockTable head={[t('guides.col.boss'), t('guides.col.location'), t('guides.col.cap'), t('guides.col.team')]}>
              {shown.map((b) => (
                <BossRow key={b.id} game={game} boss={b} names={names} versionBadge={!split} />
              ))}
            </BlockTable>
          </>
        )
      }}
    </BlockState>
  )
}

/** Uma linha da tabela; com variantes por inicial, as abas ficam em cima do time e o cap acompanha. */
function BossRow({ game, boss: b, names, versionBadge }: { game: string; boss: BossBattle; names: Map<number, string>; versionBadge: boolean }) {
  const { t } = useTranslation()
  const { starter, setStarter, team } = useBossVariant(b)
  return (
    <tr>
      <td className={`${cell} min-w-36`}>
        <div className="font-bold">{b.name}</div>
        {b.title && <div className="text-muted">{b.title}</div>}
        <div className="mt-0.5 flex flex-wrap gap-1">
          {b.badge && <Badge tone="amber">{b.badge}</Badge>}
          {versionBadge && b.versions && <Badge>{b.versions.map((v) => versionLabel(game, v)).join(' / ')}</Badge>}
          {!team.verified && <Badge>{t('guides.block.notVerified')}</Badge>}
        </div>
      </td>
      <td className={`${cell} text-muted`}>{b.location}</td>
      <td className={`${cell} font-bold tabular-nums`}>{team.levelCap}</td>
      <td className={cell}>
        <div className="flex flex-col gap-1.5">
          <BossVariantTabs game={game} boss={b} names={names} value={starter} onChange={setStarter} />
          <div className="flex gap-1">
            {team.party.map((p, i) => (
              <Link key={i} to={`/pokedex/${p.species}`} className="flex w-12 shrink-0 flex-col items-center" title={names.get(p.species)}>
                <img src={spriteUrl(p.species)} alt={names.get(p.species) ?? ''} className="pixelated h-10 w-10" loading="lazy" />
                <span className="text-[11px] text-muted tabular-nums">{t('guides.block.level', { level: p.level })}</span>
              </Link>
            ))}
          </div>
        </div>
      </td>
    </tr>
  )
}
