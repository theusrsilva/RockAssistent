import { useTranslation } from 'react-i18next'
import { useSearchParams } from 'react-router'
import { formatMultiplier, multiplierClass, useTypeChart } from '../../lib/dex'
import { Callout, Field, PageTitle, selectClass } from '../../components/ui'
import { TypeBadge, useTypeName } from './TypeBadge'
import { BackLink } from './tabs/parts'

const generations = [1, 2, 3, 4, 5, 6, 7, 8, 9]

/** Tabela de tipos de uma geração: linha = tipo do golpe, coluna = tipo de quem recebe. */
export default function TypeChartPage() {
  const { t } = useTranslation()
  const typeName = useTypeName()
  const [params, setParams] = useSearchParams()
  const asked = Number(params.get('geracao'))
  const generation = generations.includes(asked) ? asked : 9
  const chart = useTypeChart(generation)
  const data = chart.data

  const chooseGeneration = (g: number) =>
    setParams(
      (p) => {
        const n = new URLSearchParams(p)
        if (g === 9) n.delete('geracao')
        else n.set('geracao', String(g))
        return n
      },
      { replace: true },
    )

  return (
    <div className="flex flex-col gap-4">
      <BackLink history>{t('dex.back')}</BackLink>
      <PageTitle>{t('dex.typeChart.title')}</PageTitle>
      <p className="-mt-2 text-sm text-muted">{t('dex.typeChart.sub')}</p>

      <div className="w-full sm:w-60">
        <Field label={t('dex.typeChart.generation')}>
          <select className={selectClass} value={generation} onChange={(e) => chooseGeneration(Number(e.target.value))}>
            {generations.map((g) => (
              <option key={g} value={g}>
                {t('dex.generation', { n: g })}
              </option>
            ))}
          </select>
        </Field>
      </div>
      <p className="text-[12.5px] text-muted">{t('dex.typeChart.help')}</p>

      {chart.error && (
        <Callout tone="red" icon="error">
          {t('dex.typeChart.error')}
        </Callout>
      )}
      {chart.loading && <p className="text-sm text-muted">{t('common.loading')}</p>}

      {data && (
        <div className="scroll-x rounded-xl border border-line bg-surface">
          <table className="border-collapse text-xs">
            <thead>
              <tr>
                <th className="sticky left-0 z-10 bg-surface px-2 py-1.5 text-left text-[11px] font-semibold text-muted">
                  {t('dex.typeChart.attack')} ↓ / {t('dex.typeChart.defense')} →
                </th>
                {data.types.map((d) => (
                  <th key={d} className="px-0.5 py-1.5" title={typeName(d)}>
                    <span className="block w-9 truncate text-center text-[10.5px] font-bold">{typeName(d).slice(0, 4)}</span>
                  </th>
                ))}
              </tr>
            </thead>
            <tbody>
              {data.types.map((a) => (
                <tr key={a} className="border-t border-line">
                  <th className="sticky left-0 z-10 bg-surface px-2 py-1 text-left">
                    <TypeBadge type={a} small />
                  </th>
                  {data.types.map((d) => {
                    const f = data.factors[a]?.[d] ?? 1
                    return (
                      <td
                        key={d}
                        title={`${typeName(a)} → ${typeName(d)}: ${formatMultiplier(f)}`}
                        className={`h-8 w-9 border-l border-line text-center font-mono font-bold ${f === 1 ? '' : multiplierClass(f)}`}
                      >
                        {f === 1 ? '' : formatMultiplier(f).slice(1)}
                      </td>
                    )
                  })}
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  )
}
