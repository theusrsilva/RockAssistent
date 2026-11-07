import { useTranslation } from 'react-i18next'
import { Link } from 'react-router'
import { spriteUrl } from '../../lib/api'
import { dexLang, formatEvolutionCondition, type EvolutionStep } from '../../lib/dex'
import { Icon } from '../../components/ui'
import { useTypeName } from './TypeBadge'

/** Cadeia de evolução em estágios (colunas); cada um mostra como se chega nele. */
export function EvolutionChain({ chain, current }: { chain: EvolutionStep[]; current: number }) {
  const { t, i18n } = useTranslation()
  const typeName = useTypeName()
  const lang = dexLang(i18n.language)
  if (chain.length <= 1) return <p className="text-sm text-muted">{t('dex.general.noEvolution')}</p>

  // Estágio = distância até a raiz da cadeia.
  const byId = new Map(chain.map((s) => [s.speciesId, s]))
  const depth = (s: EvolutionStep): number => (s.fromSpeciesId && byId.has(s.fromSpeciesId) ? depth(byId.get(s.fromSpeciesId)!) + 1 : 0)
  const stages: EvolutionStep[][] = []
  for (const step of chain) (stages[depth(step)] ??= []).push(step)

  return (
    <div className="flex flex-col items-stretch gap-2 sm:flex-row sm:items-center">
      {stages.map((stage, i) => (
        <div key={i} className="flex flex-col items-center gap-2 sm:flex-row">
          {i > 0 && <Icon name="arrow_forward" className="rotate-90 text-muted sm:rotate-0" />}
          <div className="flex flex-wrap justify-center gap-2">
            {stage.map((s) => (
              <Link
                key={s.speciesId}
                to={`/pokedex/${s.speciesId}`}
                className={`flex w-32 flex-col items-center rounded-lg border p-2 text-center hover:bg-surface2 ${
                  s.speciesId === current ? 'border-accent bg-accent-soft' : 'border-line'
                }`}
              >
                <img src={spriteUrl(s.speciesId)} alt="" className="h-14 w-14 [image-rendering:pixelated]" loading="lazy" />
                <span className="text-sm font-semibold">{s.name}</span>
                {s.conditions.map((c) => (
                  <span key={c} className="text-[11.5px] leading-tight text-muted">
                    {formatEvolutionCondition(c, lang, typeName)}
                  </span>
                ))}
              </Link>
            ))}
          </div>
        </div>
      ))}
    </div>
  )
}
