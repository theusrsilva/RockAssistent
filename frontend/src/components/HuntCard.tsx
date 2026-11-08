import { useTranslation } from 'react-i18next'
import { Link } from 'react-router'
import { spriteUrl } from '../lib/api'
import { GAME_VERSIONS } from '../lib/gameVersions'
import { formatOdds, huntMethodName, type Hunt } from '../lib/hunts'
import { Badge } from './ui'

// Fora de pages/hunt para a HomePage não puxar a página do Contador Hunt para o chunk principal.

export const gameName = (key: string) => GAME_VERSIONS.find((g) => g.key === key)?.name ?? key

/** Cartão da hunt (lista do Contador Hunt e Início). */
export function HuntCard({ hunt, name }: { hunt: Hunt; name?: string }) {
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
          {gameName(hunt.gameKey)} · {huntMethodName(hunt, i18n.language)}
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
        <div className="font-display text-2xl leading-none font-extrabold tabular-nums">{hunt.count.toLocaleString(i18n.language)}</div>
        <div className="text-[11.5px] text-muted">{t('hunt.encounters')}</div>
      </div>
    </Link>
  )
}
