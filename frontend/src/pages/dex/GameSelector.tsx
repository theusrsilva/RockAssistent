import { useTranslation } from 'react-i18next'
import type { GameAvailability } from '../../lib/dex'
import { gameVersionsByGeneration } from '../../lib/gameVersions'
import { selectClass } from '../../components/ui'

/**
 * Jogo cuja Pokédex está na tela. Jogos em que a espécie não dá para pegar aparecem apagados, mas dá para
 * escolher (para ver golpes e dados gerais mesmo assim).
 */
export function GameSelector({
  value,
  onChange,
  availability,
}: {
  value: string
  onChange: (key: string) => void
  availability?: GameAvailability[] | null
}) {
  const { t } = useTranslation()
  const entry = (key: string) => availability?.find((a) => a.gameKey === key)
  const kindOf = (key: string) => entry(key)?.kind
  return (
    <select
      aria-label={t('dex.game.label')}
      className={`${selectClass} sm:max-w-xs`}
      value={value}
      onChange={(e) => onChange(e.target.value)}
    >
      {gameVersionsByGeneration().map(({ generation, games }) => (
        <optgroup key={generation} label={t('dex.generation', { n: generation })}>
          {games.map((g) => {
            const none = kindOf(g.key) === 'none'
            const requires = entry(g.key)?.requires
            const note = none ? t('dex.game.unavailable') : requires ? t(`dex.requires.${requires}`) : null
            return (
              <option key={g.key} value={g.key} className={none ? 'text-muted' : ''}>
                {note ? `${g.name} (${note})` : g.name}
              </option>
            )
          })}
        </optgroup>
      ))}
    </select>
  )
}
