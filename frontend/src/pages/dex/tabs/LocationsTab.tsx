import { useTranslation } from 'react-i18next'
import { Link } from 'react-router'
import { spriteUrl } from '../../../lib/api'
import { dexHref, titleCase, versionName, type EncounterInfo, type SpeciesDetail } from '../../../lib/dex'
import { Badge, Callout, Icon } from '../../../components/ui'
import { Section } from './parts'

/**
 * Encontros juntados por lugar (e área), com versão, método, níveis, chance e condições. Sem encontro próprio (a
 * espécie vem de evolução ou de ovo), mostra de qual parente dá para conseguir no jogo: cada um é um cartão que leva
 * ao "Onde achar" dele, no mesmo jogo.
 */
export function LocationsTab({ detail, game }: { detail: SpeciesDetail; game: string }) {
  const { t } = useTranslation()
  if (detail.encounters.length === 0) {
    const sources = detail.relatedSources ?? []
    if (sources.length === 0)
      return (
        <Callout tone="sky" icon="info">
          {t('dex.encounters.empty')}
        </Callout>
      )
    return (
      <div className="flex flex-col gap-3">
        <Callout tone="sky" icon="info">
          {t('dex.encounters.viaRelatives', { name: detail.name })}
        </Callout>
        <ul className="flex flex-col gap-2">
          {sources.map((s) => (
            <li key={`${s.relation}-${s.speciesId}`}>
              <Link
                to={dexHref(`/pokedex/${s.speciesId}?aba=locations`, game)}
                className="flex items-center gap-3 rounded-xl border border-line bg-surface p-2.5 hover:bg-surface2"
              >
                <img src={spriteUrl(s.speciesId)} alt="" className="h-12 w-12 shrink-0 [image-rendering:pixelated]" />
                <span className="min-w-0 flex-1 text-sm">
                  <b className="block">{t(`dex.encounters.relation.${s.relation}`, { name: s.name })}</b>
                  <span className="text-muted">{t(`dex.encounters.relativeKind.${s.kind}`, { name: s.name })}</span>
                </span>
                <span className="inline-flex shrink-0 items-center gap-0.5 text-sm font-semibold text-accent">
                  {t('dex.encounters.seeWhere')}
                  <Icon name="chevron_right" className="text-[18px]" />
                </span>
              </Link>
            </li>
          ))}
        </ul>
      </div>
    )
  }

  const places = new Map<string, EncounterInfo[]>()
  for (const e of detail.encounters) {
    const key = e.area ? `${e.location} · ${e.area}` : e.location
    places.set(key, [...(places.get(key) ?? []), e])
  }
  const method = (m: string) => t(`dex.method.${m}`, { defaultValue: titleCase(m) })
  const condition = (c: string) => t(`dex.condition.${c}`, { defaultValue: c.replace(/[-_]/g, ' ') })

  return (
    <div className="flex flex-col gap-3">
      {[...places].map(([place, list]) => (
        <Section key={place} title={place}>
          <ul className="flex flex-col divide-y divide-line">
            {list.map((e, i) => (
              <li key={i} className="flex flex-wrap items-center gap-x-2.5 gap-y-1 py-1.5 text-sm">
                <Badge tone="sky">{versionName(e.version)}</Badge>
                <span className="font-semibold">{method(e.method)}</span>
                <span className="text-muted">
                  {e.minLevel === e.maxLevel
                    ? t('dex.encounters.level', { level: e.minLevel })
                    : t('dex.encounters.levels', { min: e.minLevel, max: e.maxLevel })}
                </span>
                {e.chance > 0 && <Badge>{t('dex.encounters.chance', { chance: e.chance })}</Badge>}
                {e.conditions.map((c) => (
                  <Badge key={c} tone="amber">
                    {condition(c)}
                  </Badge>
                ))}
              </li>
            ))}
          </ul>
        </Section>
      ))}
    </div>
  )
}
