import { useTranslation } from 'react-i18next'
import { Link } from 'react-router'
import { Badge } from '../../../components/ui'
import { spriteUrl } from '../../../lib/api'
import { useApiGet } from '../../../lib/dex'
import { localized, type VgcTeam } from '../../../lib/vgc'
import { BlockState } from './common'

/** :::vgc-teams — lista curta dos times sugeridos, com link para a página completa (/guias/vgc/times). */
export default function VgcTeamsBlock() {
  const { t, i18n } = useTranslation()
  const state = useApiGet<VgcTeam[]>('/api/vgc/teams')
  return (
    <BlockState state={state} empty={(d) => d.length === 0}>
      {(teams) => (
        <div className="my-3 grid gap-2 sm:grid-cols-2">
          {teams.map((team) => (
            <Link
              key={team.id}
              to={`/guias/vgc/times#${team.id}`}
              className="flex flex-col gap-1 rounded-lg border border-line bg-surface px-3 py-2 no-underline transition-colors hover:border-accent hover:bg-surface2"
            >
              <span className="flex flex-wrap items-center gap-1.5">
                <span className="font-bold text-ink">{team.name}</span>
                <Badge tone="accent">{t(`vgc.archetypes.${team.archetype}`, { defaultValue: team.archetype })}</Badge>
              </span>
              <span className="flex flex-wrap">
                {team.members.map((m, i) => (
                  <img key={i} src={spriteUrl(m.species)} alt={m.name} title={m.name} className="size-9 [image-rendering:pixelated]" loading="lazy" />
                ))}
              </span>
              <span className="text-[12.5px] text-muted">{localized(i18n.language, team.ideaPt, team.ideaEn)}</span>
            </Link>
          ))}
        </div>
      )}
    </BlockState>
  )
}
