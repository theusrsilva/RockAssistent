import { useTranslation } from 'react-i18next'
import { Badge } from '../../../components/ui'
import { useApiGet } from '../../../lib/dex'
import { formatIsoDate, localized, type VgcRegulation } from '../../../lib/vgc'
import { BlockState } from './common'

/** :::vgc-regulation — a regulamentação vigente (assets/data/vgc/regulation.json): datas, regras e novidades. */
export default function VgcRegulationBlock() {
  const { t, i18n } = useTranslation()
  const state = useApiGet<VgcRegulation>('/api/vgc/regulation')
  return (
    <BlockState state={state}>
      {(r) => (
        <div className="my-3 flex flex-col gap-2.5 rounded-lg border border-line bg-surface px-4 py-3 text-sm">
          <div className="flex flex-wrap items-center gap-2">
            <span className="font-display text-base font-bold">{r.name}</span>
            <Badge tone="sky">{r.game}</Badge>
            {!r.verified && <Badge tone="amber">{t('guides.block.notVerified')}</Badge>}
          </div>
          <dl className="grid gap-x-4 gap-y-1 sm:grid-cols-[auto_1fr]">
            <dt className="font-semibold">{t('vgc.regulation.period')}</dt>
            <dd>{t('vgc.regulation.dates', { from: formatIsoDate(r.validFrom, i18n.language), to: r.validTo ? formatIsoDate(r.validTo, i18n.language) : '…' })}</dd>
            <dt className="font-semibold">{t('vgc.regulation.format')}</dt>
            <dd>{t('vgc.regulation.formatValue', { bring: r.bring, pick: r.pick, level: r.level })}</dd>
            <dt className="font-semibold">{t('vgc.regulation.pool')}</dt>
            <dd>{r.allowedDexes.join('; ')}</dd>
            <dt className="font-semibold">{t('vgc.regulation.checkedOn')}</dt>
            <dd>{formatIsoDate(r.checkedOn, i18n.language)}</dd>
          </dl>
          <div>
            <div className="font-semibold">{t('vgc.regulation.rules')}</div>
            <ul className="list-disc pl-5">
              {(i18n.language.startsWith('pt') ? r.rulesPt : r.rulesEn).map((rule, i) => (
                <li key={i}>{rule}</li>
              ))}
            </ul>
          </div>
          <List title={t('vgc.regulation.newSpecies')} items={r.newSpecies} />
          <List title={t('vgc.regulation.newMegas')} items={r.newMegas} />
          <List title={t('vgc.regulation.newItems')} items={r.newItems} />
          <List title={t('vgc.regulation.bannedSpecies')} items={r.bannedSpecies} empty={t('vgc.regulation.none')} />
          <List title={t('vgc.regulation.bannedMoves')} items={r.bannedMoves} empty={t('vgc.regulation.none')} />
          <p className="text-muted">{localized(i18n.language, r.notesPt, r.notesEn)}</p>
        </div>
      )}
    </BlockState>
  )
}

function List({ title, items, empty }: { title: string; items: string[]; empty?: string }) {
  if (items.length === 0 && !empty) return null
  return (
    <div>
      <span className="font-semibold">{title}: </span>
      <span>{items.length === 0 ? empty : items.join(', ')}</span>
    </div>
  )
}
