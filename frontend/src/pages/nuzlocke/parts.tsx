import { useTranslation } from 'react-i18next'
import { Link } from 'react-router'
import { Badge } from '../../components/ui'
import { spriteUrl } from '../../lib/api'
import { titleCase } from '../../lib/dex'
import type { BossPokemon, MonInfo, RunView } from './nuzlocke'

/** O que os cartões da run recebem para alterar a run: devolve o RunView novo (ou lança o ApiError). */
export type Mutate = (request: Promise<RunView>) => Promise<RunView>

/** Nome que aparece para um Pokémon da run: o apelido, ou o nome da espécie. */
export const monName = (m: MonInfo, names: Map<number, string>) => m.nickname || names.get(m.species) || `#${m.species}`

/** Pokémon de um time de chefe ou treinador: sprite, nível, golpes, habilidade, item e tera. */
export function BattlePokemon({ p, names }: { p: BossPokemon; names: Map<number, string> }) {
  const { t } = useTranslation()
  return (
    <div className="flex items-start gap-2 rounded-lg bg-surface2 px-2 py-1.5">
      <Link to={`/pokedex/${p.species}`} className="shrink-0">
        <img src={spriteUrl(p.species)} alt="" className="pixelated h-11 w-11" loading="lazy" />
      </Link>
      <div className="min-w-0 flex-1 text-[12.5px]">
        <div className="flex flex-wrap items-baseline gap-x-1.5">
          <span className="font-bold text-ink">{names.get(p.species) ?? `#${p.species}`}</span>
          <span className="text-muted">{t('nuzlocke.boss.level', { level: p.level })}</span>
        </div>
        {p.moves.length > 0 && <div className="text-muted">{p.moves.map(titleCase).join(' · ')}</div>}
        {(p.ability || p.item || p.teraType) && (
          <div className="mt-0.5 flex flex-wrap gap-1">
            {p.ability && <Badge>{`${t('nuzlocke.boss.ability')}: ${titleCase(p.ability)}`}</Badge>}
            {p.item && <Badge>{`${t('nuzlocke.boss.item')}: ${titleCase(p.item)}`}</Badge>}
            {p.teraType && <Badge tone="violet">{`${t('nuzlocke.boss.tera')}: ${titleCase(p.teraType)}`}</Badge>}
          </div>
        )}
      </div>
    </div>
  )
}

/** Título de um cartão da run. */
export function CardTitle({ icon, children, extra }: { icon: string; children: React.ReactNode; extra?: React.ReactNode }) {
  return (
    <div className="flex flex-wrap items-center gap-2">
      <h2 className="flex flex-1 items-center gap-1.5 font-display text-lg font-bold">
        <span aria-hidden className="icon text-[20px] text-muted">
          {icon}
        </span>
        {children}
      </h2>
      {extra}
    </div>
  )
}
