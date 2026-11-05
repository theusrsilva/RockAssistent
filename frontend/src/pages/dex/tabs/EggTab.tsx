import { useTranslation } from 'react-i18next'
import { genderRatio, titleCase, type SpeciesDetail } from '../../../lib/dex'
import { Badge } from '../../../components/ui'
import { TypeBadge } from '../TypeBadge'
import { Fact, Section } from './parts'

/** Dados de criação: grupos de ovo, passos para chocar, gênero e golpes de ovo do jogo. */
export function EggTab({ detail }: { detail: SpeciesDetail }) {
  const { t, i18n } = useTranslation()
  const gender = genderRatio(detail.genderRate)
  const noEggs = detail.eggGroups.includes('no-eggs')
  const pct = (n: number) => n.toLocaleString(i18n.language, { maximumFractionDigits: 1 })

  return (
    <div className="flex flex-col gap-4">
      <div className="grid grid-cols-1 gap-2 sm:grid-cols-3">
        <Fact label={t('dex.egg.groups')}>
          <span className="flex flex-wrap gap-1">
            {detail.eggGroups.map((g) => (
              <Badge key={g}>{t(`dex.eggGroup.${g}`, { defaultValue: titleCase(g) })}</Badge>
            ))}
          </span>
        </Fact>
        <Fact label={t('dex.egg.hatchSteps')}>
          {detail.hatchSteps != null ? t('dex.egg.steps', { count: detail.hatchSteps }) : '—'}
        </Fact>
        <Fact label={t('dex.general.gender')}>
          {gender
            ? `${t('dex.general.male', { pct: pct(gender.male) })} · ${t('dex.general.female', { pct: pct(gender.female) })}`
            : t('dex.general.genderless')}
        </Fact>
      </div>
      {noEggs && <p className="text-sm text-muted">{t('dex.egg.noEggs')}</p>}

      {detail.moves.egg.length > 0 && (
        <Section title={t('dex.egg.eggMoves')}>
          <ul className="flex flex-wrap gap-2">
            {detail.moves.egg.map((m) => (
              <li key={m.id} className="flex items-center gap-1.5 rounded-lg bg-surface2 px-2.5 py-1 text-sm">
                <span className="font-semibold">{m.name}</span>
                <TypeBadge type={m.type} small />
              </li>
            ))}
          </ul>
        </Section>
      )}
    </div>
  )
}
