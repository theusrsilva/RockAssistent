import { useTranslation } from 'react-i18next'
import { titleCase, versionName, type EncounterInfo, type SpeciesDetail } from '../../../lib/dex'
import { Badge, Callout } from '../../../components/ui'
import { Section } from './parts'

/** Encontros juntados por lugar (e área), com versão, método, níveis, chance e condições. */
export function LocationsTab({ detail }: { detail: SpeciesDetail }) {
  const { t } = useTranslation()
  if (detail.encounters.length === 0)
    return (
      <Callout tone="sky" icon="info">
        {t('dex.encounters.empty')}
      </Callout>
    )

  const places = new Map<string, EncounterInfo[]>()
  for (const e of detail.encounters) {
    const key = e.area ? `${e.location} · ${e.area}` : e.location
    places.set(key, [...(places.get(key) ?? []), e])
  }
  const method = (m: string) => t(`dex.method.${m}`, { defaultValue: titleCase(m) })
  const condition = (c: string) => t(`dex.condition.${c}`, { defaultValue: c.replace(/-/g, ' ') })

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
