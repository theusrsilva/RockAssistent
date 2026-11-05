import { useTranslation } from 'react-i18next'

/** Cores clássicas dos tipos (as mesmas em claro e escuro, com texto branco). */
const typeColors: Record<string, string> = {
  normal: '#9a9a6e', fighting: '#b8322a', flying: '#8f78d8', poison: '#953f95', ground: '#c9a550', rock: '#a8902f',
  bug: '#8f9b1e', ghost: '#6a5590', steel: '#8b8ba8', fire: '#e07228', water: '#5a82e0', grass: '#5fa83a',
  electric: '#d4a910', psychic: '#e5507a', ice: '#6cbaba', dragon: '#6233e8', dark: '#6a5446', fairy: '#d36f94',
}

/** Nome do tipo traduzido; identificadores sem tradução (stellar, shadow...) passam com a primeira letra maiúscula. */
export function useTypeName() {
  const { t } = useTranslation()
  return (type: string) => t(`dex.types.${type}`, { defaultValue: type.charAt(0).toUpperCase() + type.slice(1) })
}

export function TypeBadge({ type, small = false }: { type: string; small?: boolean }) {
  const typeName = useTypeName()
  return (
    <span
      className={`inline-flex items-center justify-center rounded-full font-bold whitespace-nowrap text-white ${
        small ? 'h-5 min-w-14 px-2 text-[11px]' : 'h-6 min-w-16 px-2.5 text-xs'
      }`}
      style={{ backgroundColor: typeColors[type] ?? '#777' }}
    >
      {typeName(type)}
    </span>
  )
}

export function TypeList({ types, small }: { types: string[]; small?: boolean }) {
  return (
    <span className="inline-flex flex-wrap gap-1">
      {types.map((t) => (
        <TypeBadge key={t} type={t} small={small} />
      ))}
    </span>
  )
}
