import { useTranslation } from 'react-i18next'
import type { BaseStats, SpeciesDetail, SpeciesForm } from '../../../lib/dex'
import { Badge, Field, selectClass } from '../../../components/ui'
import { TypeList } from '../TypeBadge'
import { Section } from './parts'

const rows: [keyof BaseStats, string][] = [
  ['hp', 'hp'],
  ['attack', 'attack'],
  ['defense', 'defense'],
  ['specialAttack', 'special-attack'],
  ['specialDefense', 'special-defense'],
  ['speed', 'speed'],
]

/**
 * Status base com barras (de 0 a 255), total e EVs; com mais de uma forma, dá para trocar. A forma escolhida é a
 * mesma do cabeçalho da página (sub-abas do sprite).
 */
export function StatsTab({
  detail,
  form,
  onFormChange,
}: {
  detail: SpeciesDetail
  form: SpeciesForm | undefined
  onFormChange: (id: number) => void
}) {
  const { t } = useTranslation()
  const stats = form?.stats ?? detail.stats
  const evs = Object.entries(stats.evs).filter(([, v]) => v > 0)

  return (
    <div className="flex flex-col gap-4">
      {detail.forms.length > 1 && (
        <div className="flex flex-wrap items-end gap-3">
          <div className="w-full sm:w-72">
            <Field label={t('dex.stats.form')}>
              <select className={selectClass} value={form?.id} onChange={(e) => onFormChange(Number(e.target.value))}>
                {detail.forms.map((f) => (
                  <option key={f.id} value={f.id}>
                    {f.name}
                  </option>
                ))}
              </select>
            </Field>
          </div>
          {form && <TypeList types={form.types} />}
        </div>
      )}

      <Section title={t('dex.stats.base')}>
        <div className="flex flex-col gap-1.5">
          {rows.map(([key, label]) => {
            const value = stats[key] as number
            return (
              <div key={key} className="flex items-center gap-2 text-sm">
                <span className="w-24 shrink-0 font-semibold sm:w-28">{t(`dex.stats.${label}`)}</span>
                <span className="w-9 shrink-0 text-right font-mono">{value}</span>
                <div className="h-2.5 flex-1 overflow-hidden rounded-full bg-surface2">
                  <div
                    className={`h-full rounded-full ${value >= 100 ? 'bg-ok' : value >= 60 ? 'bg-accent' : 'bg-warn'}`}
                    style={{ width: `${Math.min(100, (value / 255) * 100)}%` }}
                  />
                </div>
              </div>
            )
          })}
          <div className="flex items-center gap-2 border-t border-line pt-1.5 text-sm">
            <span className="w-24 shrink-0 font-bold sm:w-28">{t('dex.stats.total')}</span>
            <span className="w-9 shrink-0 text-right font-mono font-bold">{stats.total}</span>
          </div>
        </div>
      </Section>

      {evs.length > 0 && (
        <Section title={t('dex.stats.evs')}>
          <div className="flex flex-wrap gap-1.5">
            {evs.map(([stat, v]) => (
              <Badge key={stat} tone="emerald">
                +{v} {t(`dex.stats.${stat}`, { defaultValue: stat })}
              </Badge>
            ))}
          </div>
        </Section>
      )}
    </div>
  )
}
