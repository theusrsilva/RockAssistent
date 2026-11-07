import { useEffect, useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Button, Callout, Modal } from '../../../components/ui'
import { ApiError, spriteUrl } from '../../../lib/api'
import { analyzePaste, isTeamWeakness, uncovered, type AnalyzeResult } from '../../../lib/vgc'
import { TypeBadge, TypeList } from '../../dex/TypeBadge'
import { BlockTable, cell } from '../blocks/common'

/** Multiplicador como a tabela mostra: vazio para ×1. */
function factorLabel(f: number): string {
  if (f === 1) return ''
  if (f === 0) return '0'
  if (f === 0.25) return '¼'
  if (f === 0.5) return '½'
  return `${f}×`
}

function factorClass(f: number): string {
  if (f > 1) return 'bg-bad-soft text-bad font-bold'
  if (f === 0) return 'bg-ok-soft text-ok font-bold'
  if (f < 1) return 'text-ok'
  return ''
}

/** Mensagem de erro da análise (códigos do servidor). */
export function analyzeErrorText(t: (k: string) => string, error: unknown): string {
  const code = error instanceof ApiError ? error.errors[0] : null
  return code === 'paste_too_big' ? t('vgc.errors.pasteTooBig') : code === 'paste_invalid' ? t('vgc.errors.pasteInvalid') : t('vgc.errors.analyze')
}

/** Resultado da análise: membros, fraquezas por tipo (tabela), cobertura e velocidades. */
export function TeamReportView({ result }: { result: AnalyzeResult }) {
  const { t } = useTranslation()
  const { report, skipped } = result
  const weakTypes = report.defense.filter(isTeamWeakness).map((d) => d.type)
  const missing = uncovered(report)
  return (
    <div className="flex flex-col gap-4">
      {skipped.length > 0 && (
        <Callout tone="amber" icon="warning">
          <div className="font-semibold">{t('vgc.report.skipped')}</div>
          <ul className="list-disc pl-5">
            {skipped.map((s, i) => (
              <li key={i}>{s.text}</li>
            ))}
          </ul>
        </Callout>
      )}

      <div className="grid gap-2 sm:grid-cols-2">
        {report.members.map((m, i) => (
          <div key={i} className="flex items-center gap-2 rounded-lg border border-line bg-surface px-2.5 py-1.5">
            <img src={spriteUrl(m.species)} alt="" className="size-10 shrink-0 [image-rendering:pixelated]" loading="lazy" />
            <div className="min-w-0">
              <div className="truncate font-semibold">{m.name}</div>
              <TypeList types={m.types} small />
            </div>
            <span className="ml-auto text-[12px] whitespace-nowrap text-muted">{t('vgc.report.speedShort', { n: m.baseSpeed })}</span>
          </div>
        ))}
      </div>

      <section>
        <h3 className="font-display text-base font-bold">{t('vgc.report.defense')}</h3>
        <p className="text-[12.5px] text-muted">{t('vgc.report.defenseHint')}</p>
        {weakTypes.length > 0 ? (
          <div className="mt-2 flex flex-wrap items-center gap-1.5 text-[13px]">
            <span className="font-semibold text-bad">{t('vgc.report.teamWeak')}</span>
            {weakTypes.map((type) => (
              <TypeBadge key={type} type={type} small />
            ))}
          </div>
        ) : (
          <p className="mt-2 text-[13px] font-semibold text-ok">{t('vgc.report.noTeamWeak')}</p>
        )}
        <BlockTable
          head={[
            t('vgc.report.attack'),
            ...report.members.map((m, i) => (
              <img key={i} src={spriteUrl(m.species)} alt={m.name} title={m.name} className="mx-auto size-7 [image-rendering:pixelated]" />
            )),
            t('vgc.report.weak'),
            t('vgc.report.resist'),
          ]}
        >
          {report.defense.map((row) => (
            <tr key={row.type} className={isTeamWeakness(row) ? 'bg-bad-soft/50' : ''}>
              <td className={cell}>
                <TypeBadge type={row.type} small />
              </td>
              {row.factors.map((f, i) => (
                <td key={i} className={`${cell} text-center ${factorClass(f)}`}>
                  {factorLabel(f)}
                </td>
              ))}
              <td className={`${cell} text-center font-semibold ${row.weak > 0 ? 'text-bad' : 'text-muted'}`}>{row.weak}</td>
              <td className={`${cell} text-center text-muted`}>{row.resist + row.immune}</td>
            </tr>
          ))}
        </BlockTable>
      </section>

      <section>
        <h3 className="font-display text-base font-bold">{t('vgc.report.coverage')}</h3>
        <p className="text-[12.5px] text-muted">{t('vgc.report.coverageHint')}</p>
        <div className="mt-2 flex flex-wrap items-center gap-1.5 text-[13px]">
          <span className="font-semibold">{t('vgc.report.attackTypes')}</span>
          {report.attackTypes.length === 0 ? <span className="text-muted">—</span> : <TypeList types={report.attackTypes} small />}
        </div>
        <div className="mt-1.5 flex flex-wrap items-center gap-1.5 text-[13px]">
          <span className="font-semibold">{t('vgc.report.uncovered')}</span>
          {missing.length === 0 ? <span className="text-ok">{t('vgc.report.allCovered')}</span> : <TypeList types={missing} small />}
        </div>
      </section>

      <section>
        <h3 className="font-display text-base font-bold">{t('vgc.report.speeds')}</h3>
        <ol className="mt-1 flex flex-col gap-0.5 text-[13px]">
          {report.speeds.map((s, i) => (
            <li key={i} className="flex items-center gap-2">
              <img src={spriteUrl(s.species)} alt="" className="size-6 [image-rendering:pixelated]" loading="lazy" />
              <span className="font-semibold">{s.name}</span>
              <span className="text-muted">{s.baseSpeed}</span>
            </li>
          ))}
        </ol>
      </section>

      <p className="text-[12px] text-muted">{t('vgc.report.limits')}</p>
    </div>
  )
}

/** Analisa um paste (de um time sugerido) numa janela. */
export default function PasteDialog({ title, paste, onClose }: { title: string; paste: string; onClose: () => void }) {
  const { t } = useTranslation()
  const [state, setState] = useState<{ result: AnalyzeResult | null; error: string | null }>({ result: null, error: null })
  useEffect(() => {
    let alive = true
    analyzePaste(paste)
      .then((result) => alive && setState({ result, error: null }))
      .catch((e: unknown) => alive && setState({ result: null, error: analyzeErrorText(t, e) }))
    return () => {
      alive = false
    }
  }, [paste, t])
  return (
    <Modal
      wide
      title={t('vgc.report.title', { name: title })}
      actions={
        <Button variant="secondary" onClick={onClose}>
          {t('common.close')}
        </Button>
      }
    >
      {state.error ? (
        <Callout tone="amber" icon="error">
          {state.error}
        </Callout>
      ) : state.result ? (
        <TeamReportView result={state.result} />
      ) : (
        <p className="text-muted">{t('common.loading')}</p>
      )}
    </Modal>
  )
}
