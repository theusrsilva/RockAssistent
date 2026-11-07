import type { ReactNode } from 'react'
import { useTranslation } from 'react-i18next'
import { Link, useLocation, useNavigate } from 'react-router'
import { spriteUrl } from '../../../lib/api'
import { dexHref } from '../../../lib/dex'
import { Icon } from '../../../components/ui'

/** Título de seção dentro de uma aba. */
export function Section({ title, children, aside }: { title: ReactNode; children: ReactNode; aside?: ReactNode }) {
  return (
    <section className="flex flex-col gap-2.5 rounded-xl border border-line bg-surface p-4 sm:p-5">
      <div className="flex flex-wrap items-center gap-2">
        <h2 className="flex-1 font-display text-lg font-bold">{title}</h2>
        {aside}
      </div>
      {children}
    </section>
  )
}

/** Par rótulo/valor das fichas. */
export function Fact({ label, children }: { label: ReactNode; children: ReactNode }) {
  return (
    <div className="rounded-lg bg-surface2 px-3 py-2">
      <div className="text-[11.5px] font-semibold text-muted">{label}</div>
      <div className="text-sm font-semibold">{children}</div>
    </div>
  )
}

/**
 * Volta para a lista da Pokédex (ou outro endereço). Com <history>, volta à tela anterior do app (um guia, por
 * exemplo, no mesmo ponto) quando houver uma; aberta direto, vai para <to>.
 */
export function BackLink({ to = '/pokedex', history = false, children }: { to?: string; history?: boolean; children: ReactNode }) {
  const { t } = useTranslation()
  const navigate = useNavigate()
  // A primeira entrada do app tem a chave "default": antes dela não há tela do app para voltar.
  const location = useLocation()
  const canGoBack = history && location.key !== 'default'
  return (
    <Link
      to={to}
      onClick={(e) => {
        if (!canGoBack || e.metaKey || e.ctrlKey || e.shiftKey || e.button !== 0) return
        e.preventDefault()
        navigate(-1)
      }}
      className="inline-flex items-center gap-1 text-sm font-semibold text-muted hover:text-ink"
    >
      <Icon name="arrow_back" className="text-[18px]" />
      {canGoBack ? t('common.back') : children}
    </Link>
  )
}

/** Pokémon pequeno com sprite que leva para a página da espécie. */
export function SpeciesChip({
  species,
  form,
  name,
  game,
  children,
}: {
  species: number
  form?: number
  name: string
  /** Jogo em que a ficha abre ("?game="). */
  game?: string
  children?: ReactNode
}) {
  return (
    <Link
      to={dexHref(`/pokedex/${species}`, game)}
      className="flex min-w-0 items-center gap-2 rounded-lg bg-surface2 px-2 py-1.5 text-sm hover:bg-tile"
    >
      <img src={spriteUrl(species, { form })} alt="" loading="lazy" className="h-10 w-10 shrink-0 [image-rendering:pixelated]" />
      <span className="min-w-0 flex-1 truncate font-semibold">{name}</span>
      {children}
    </Link>
  )
}

/** Texto de efeito (só existe em inglês no PokeAPI), com o aviso quando a tela está em português. */
export function EffectText({ text, notice }: { text: string; notice: string | null }) {
  return (
    <div className="space-y-1.5">
      <p className="text-sm whitespace-pre-line">{text}</p>
      {notice && <p className="text-[12px] text-muted">{notice}</p>}
    </div>
  )
}
