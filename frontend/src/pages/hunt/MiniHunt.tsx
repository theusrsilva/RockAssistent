import { useTranslation } from 'react-i18next'
import { Icon } from '../../components/ui'
import { spriteUrl } from '../../lib/api'
import { HUNT_STEPS, formatElapsed, formatOdds, formatPercent, cumulative, type Hunt } from '../../lib/hunts'

/**
 * A hunt na mini janela (a janela do PC encolhida e sempre na frente): sprite, nome, contador, somar e tirar, quanto somar
 * por vez, chance e tempo. O botão do canto devolve a janela ao tamanho normal.
 */
export function MiniHunt({
  hunt,
  name,
  count,
  step,
  seconds,
  onIncrement,
  onStep,
  onExpand,
}: {
  hunt: Hunt
  name: string
  count: number
  step: number
  seconds: number
  onIncrement: (delta: number) => void
  onStep: (value: number) => void
  onExpand: () => void
}) {
  const { t, i18n } = useTranslation()
  const lang = i18n.language
  const key = 'flex h-11 cursor-pointer items-center justify-center rounded-lg text-base font-bold disabled:cursor-default disabled:opacity-40'
  return (
    <div className="flex h-full flex-col gap-1.5 bg-bg p-2.5 text-ink select-none">
      <div className="flex items-center gap-2">
        <img src={spriteUrl(hunt.species)} alt="" className="pixelated h-10 w-10 shrink-0" />
        <div className="min-w-0 flex-1 truncate text-sm font-bold">{name}</div>
        <button
          type="button"
          title={t('hunt.mini.expand')}
          aria-label={t('hunt.mini.expand')}
          className="flex h-8 w-8 shrink-0 cursor-pointer items-center justify-center rounded-lg text-muted hover:bg-surface2 hover:text-ink"
          onClick={onExpand}
        >
          <Icon name="open_in_full" className="text-[19px]" />
        </button>
      </div>
      <div className="flex flex-1 items-center justify-center font-display text-[44px] leading-none font-bold tabular-nums">
        {count.toLocaleString(lang)}
      </div>
      <div className="flex gap-1.5">
        <button
          type="button"
          aria-label={t('hunt.counter.minus', { n: step })}
          className={`${key} w-16 border border-line bg-surface`}
          disabled={count === 0}
          onClick={(e) => {
            e.currentTarget.blur()
            onIncrement(-step)
          }}
        >
          −{step}
        </button>
        <button
          type="button"
          aria-label={t('hunt.counter.plus', { n: step })}
          className={`${key} flex-1 bg-accent text-white`}
          onClick={(e) => {
            e.currentTarget.blur()
            onIncrement(step)
          }}
        >
          +{step}
        </button>
        <select
          value={step}
          title={t('hunt.counter.stepLabel')}
          aria-label={t('hunt.counter.stepLabel')}
          className="h-11 w-14 shrink-0 cursor-pointer rounded-lg border border-line bg-surface px-1.5 text-base font-bold"
          onChange={(e) => {
            onStep(Number(e.target.value))
            // Sem foco no seletor: as teclas do atalho não mudam o valor.
            e.currentTarget.blur()
          }}
        >
          {HUNT_STEPS.map((n) => (
            <option key={n} value={n}>
              {n}
            </option>
          ))}
        </select>
      </div>
      <div className="flex justify-between text-[12px] text-muted tabular-nums">
        <span>
          {t('hunt.oneIn', { odds: formatOdds(hunt.odds, lang) })} · {formatPercent(cumulative(hunt.odds, count), lang)}
        </span>
        <span className="font-mono">{formatElapsed(seconds)}</span>
      </div>
    </div>
  )
}
