import { useTranslation } from 'react-i18next'
import { spriteUrl } from '../../lib/api'
import { titleCase, useApiGet } from '../../lib/dex'
import type { StarterInfo } from './nuzlocke'

/** Os iniciais que o jogador escolhe no jogo (vazio enquanto carrega). */
export function useStarters(gameKey: string): StarterInfo[] {
  return useApiGet<StarterInfo[]>(`/api/data/${encodeURIComponent(gameKey)}/starters`).data ?? []
}

/** Nome do inicial: o da Pokédex, senão o identificador com maiúscula. */
export const starterName = (s: { id: string; species: number }, names?: Map<number, string>) => names?.get(s.species) ?? titleCase(s.id)

/**
 * Botões com o sprite de cada inicial. `allowNone`: tem também "Não sei / não quero dizer" (valor null). Usado na
 * nova run, na troca de inicial da run e nas abas das variantes dos chefes nos guias.
 */
export function StarterPicker({
  starters,
  value,
  onChange,
  names,
  allowNone = false,
  label,
}: {
  starters: StarterInfo[]
  value: string | null
  onChange: (starter: string | null) => void
  names?: Map<number, string>
  allowNone?: boolean
  /** Texto de cada botão (padrão: o nome do inicial). */
  label?: (s: StarterInfo) => string
}) {
  const { t } = useTranslation()
  const button = (selected: boolean) =>
    `flex cursor-pointer items-center gap-1.5 rounded-lg border px-2 py-1 text-[13px] font-semibold ${selected ? 'border-accent bg-accent-soft text-ink' : 'border-line bg-surface text-muted'}`
  return (
    <div className="flex flex-wrap gap-1.5" role="radiogroup">
      {starters.map((s) => (
        <button key={s.id} type="button" role="radio" aria-checked={value === s.id} className={button(value === s.id)} onClick={() => onChange(s.id)}>
          <img src={spriteUrl(s.species)} alt="" className="pixelated h-8 w-8" loading="lazy" />
          {label ? label(s) : starterName(s, names)}
        </button>
      ))}
      {allowNone && (
        <button type="button" role="radio" aria-checked={value === null} className={`${button(value === null)} min-h-[42px]`} onClick={() => onChange(null)}>
          {t('nuzlocke.starter.none')}
        </button>
      )}
    </div>
  )
}
