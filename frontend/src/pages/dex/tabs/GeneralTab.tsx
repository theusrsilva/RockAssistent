import { useTranslation } from 'react-i18next'
import { Link } from 'react-router'
import { dexLang, genderRatio, titleCase, versionName, type SpeciesDetail } from '../../../lib/dex'
import { Badge } from '../../../components/ui'
import { DamageTakenGrid } from '../DamageTakenGrid'
import { EvolutionChain } from '../EvolutionChain'
import { TypeList } from '../TypeBadge'
import { Fact, Section } from './parts'

export function GeneralTab({ detail }: { detail: SpeciesDetail }) {
  const { t, i18n } = useTranslation()
  const lang = dexLang(i18n.language)
  const number = (n: number, digits = 1) => n.toLocaleString(i18n.language, { maximumFractionDigits: digits })
  const gender = genderRatio(detail.genderRate)
  const forms = detail.forms.filter((f) => !f.isDefault)

  return (
    <div className="flex flex-col gap-4">
      <Section title={t('dex.general.flavor')}>
        {detail.flavorTexts.length === 0 && <p className="text-sm text-muted">{t('dex.general.noFlavor')}</p>}
        {detail.flavorTexts.map((f) => (
          <div key={f.version} className="space-y-1">
            <div className="flex flex-wrap items-center gap-1.5">
              <Badge tone="sky">{versionName(f.version)}</Badge>
              {lang === 'pt-BR' && f.language !== 'pt-BR' && <Badge>{t('dex.general.inEnglish')}</Badge>}
            </div>
            <p className="text-sm">{f.text}</p>
          </div>
        ))}
      </Section>

      <div className="grid grid-cols-2 gap-2 sm:grid-cols-3">
        <Fact label={t('dex.general.height')}>{number(detail.height / 10)} m</Fact>
        <Fact label={t('dex.general.weight')}>{number(detail.weight / 10)} kg</Fact>
        <Fact label={t('dex.general.gender')}>
          {gender
            ? `${t('dex.general.male', { pct: number(gender.male) })} · ${t('dex.general.female', { pct: number(gender.female) })}`
            : t('dex.general.genderless')}
        </Fact>
        <Fact label={t('dex.general.captureRate')}>{detail.captureRate}</Fact>
        <Fact label={t('dex.general.baseHappiness')}>{detail.baseHappiness ?? '—'}</Fact>
        <Fact label={t('dex.general.growthRate')}>
          {detail.growthRate ? t(`dex.growth.${detail.growthRate}`, { defaultValue: titleCase(detail.growthRate) }) : '—'}
        </Fact>
      </div>

      {detail.abilities.length > 0 && (
        <Section title={t('dex.general.abilities')}>
          {detail.abilities.map((a) => (
            <div key={a.id} className="space-y-0.5">
              <div className="flex flex-wrap items-center gap-1.5">
                <Link to={`/pokedex/habilidade/${a.id}`} className="font-semibold hover:text-accent hover:underline">
                  {a.name}
                </Link>
                {a.isHidden && <Badge tone="violet">{t('dex.general.hidden')}</Badge>}
              </div>
              {a.effect && <p className="text-sm text-muted">{a.effect}</p>}
            </div>
          ))}
        </Section>
      )}

      <Section title={t('dex.general.evolution')}>
        <EvolutionChain chain={detail.evolutionChain} current={detail.id} />
      </Section>

      <Section title={t('dex.general.damageTaken')}>
        <p className="text-[12.5px] text-muted">{t('dex.general.damageHelp')}</p>
        <DamageTakenGrid damageTaken={detail.damageTaken} />
      </Section>

      {forms.length > 0 && (
        <Section title={t('dex.general.forms')}>
          <ul className="flex flex-col gap-1.5">
            {forms.map((f) => (
              <li key={f.id} className="flex flex-wrap items-center gap-2 text-sm">
                <span className="font-semibold">{f.name}</span>
                <TypeList types={f.types} small />
                {f.isMega && <Badge tone="violet">{t('dex.general.mega')}</Badge>}
              </li>
            ))}
          </ul>
        </Section>
      )}

      {Object.keys(detail.dexNumbers).length > 0 && (
        <Section title={t('dex.general.dexNumbers')}>
          <div className="flex flex-wrap gap-1.5">
            {Object.entries(detail.dexNumbers).map(([dex, n]) => (
              <Badge key={dex}>
                {titleCase(dex)} #{n}
              </Badge>
            ))}
          </div>
        </Section>
      )}
    </div>
  )
}
