import { useTranslation } from 'react-i18next'
import { useParams } from 'react-router'
import { ApiError } from '../../lib/api'
import { dexLang, useAbility } from '../../lib/dex'
import { Badge, Callout } from '../../components/ui'
import { BackLink, EffectText, Section, SpeciesChip } from './tabs/parts'

/** Página de uma habilidade: efeito e os Pokémon (e formas) que a têm. */
export default function AbilityPage() {
  const { t, i18n } = useTranslation()
  const id = Number(useParams().id)
  const valid = Number.isInteger(id) && id > 0
  const ability = useAbility(valid ? id : 0)
  const english = dexLang(i18n.language) === 'pt-BR' ? t('dex.ability.effectInEnglish') : null

  const back = <BackLink history>{t('dex.back')}</BackLink>
  if (!valid || (ability.error instanceof ApiError && ability.error.status === 404))
    return (
      <div className="flex flex-col gap-4">
        {back}
        <Callout tone="amber" icon="help">
          {t('dex.ability.notFound')}
        </Callout>
      </div>
    )

  const a = ability.data
  return (
    <div className="flex flex-col gap-4">
      {back}
      <div className="space-y-1">
        <h1 className="font-display text-[23px] leading-tight font-bold md:text-[28px]">{a?.name ?? '…'}</h1>
        {a && (
          <p className="text-sm text-muted">
            {t('dex.ability.generation')} {t('dex.generation', { n: a.generation })}
          </p>
        )}
      </div>

      {ability.error && (
        <Callout tone="red" icon="error">
          {t('dex.ability.error')}
        </Callout>
      )}
      {ability.loading && <p className="text-sm text-muted">{t('common.loading')}</p>}

      {a && (a.effect || a.shortEffect) && (
        <Section title={t('dex.ability.effect')}>
          <EffectText text={(a.effect || a.shortEffect)!} notice={english} />
        </Section>
      )}

      {a && (
        <Section title={`${t('dex.ability.holders')} (${a.pokemon.length})`}>
          {a.pokemon.length === 0 && <p className="text-sm text-muted">{t('dex.ability.noHolders')}</p>}
          <div className="grid grid-cols-1 gap-1.5 sm:grid-cols-2 lg:grid-cols-3">
            {a.pokemon.map((p) => (
              <SpeciesChip key={p.pokemonId} species={p.speciesId} form={p.pokemonId} name={p.name}>
                {p.isHidden && <Badge tone="violet">{t('dex.ability.hidden')}</Badge>}
              </SpeciesChip>
            ))}
          </div>
        </Section>
      )}
    </div>
  )
}
