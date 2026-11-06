import type { ReactNode } from 'react'
import { useTranslation } from 'react-i18next'
import { Callout } from '../../../components/ui'
import type { Loaded } from '../../../lib/dex'
import { gameVersionByKey } from '../../../lib/gameVersions'
import { titleCase } from '../../../lib/dex'

/** O que cada bloco recebe: os argumentos da diretiva (":::encounters frlg route-1" → ["frlg", "route-1"]). */
export interface BlockProps {
  args: string[]
}

/** Nome de uma versão do PokeAPI pelo nome do jogo ("firered" → "FireRed"). */
export function versionLabel(gameKey: string, version: string): string {
  const game = gameVersionByKey(gameKey)
  const names = game?.name.split(' / ') ?? []
  const i = game?.pokeApiVersions.indexOf(version) ?? -1
  return i >= 0 && names.length === game!.pokeApiVersions.length ? names[i] : titleCase(version)
}

/** Carregando / erro / vazio de um bloco; senão desenha os dados. */
export function BlockState<T>({ state, empty, children }: { state: Loaded<T>; empty?: (data: T) => boolean; children: (data: T) => ReactNode }) {
  const { t } = useTranslation()
  if (state.loading) return <p className="my-3 text-sm text-muted">{t('common.loading')}</p>
  if (state.error || state.data === null)
    return (
      <Callout tone="amber" icon="cloud_off" className="my-3">
        {t('guides.block.error')}
      </Callout>
    )
  if (empty?.(state.data)) return <p className="my-3 text-sm text-muted">{t('guides.block.empty')}</p>
  return <>{children(state.data)}</>
}

/** Tabela com rolagem lateral no celular. */
export function BlockTable({ head, children }: { head: ReactNode[]; children: ReactNode }) {
  return (
    <div className="my-3 overflow-x-auto rounded-lg border border-line">
      <table className="w-full border-collapse text-left text-[13px]">
        <thead className="bg-surface2 text-[11.5px] tracking-wide text-muted uppercase">
          <tr>
            {head.map((h, i) => (
              <th key={i} className="px-2.5 py-2 font-bold whitespace-nowrap">
                {h}
              </th>
            ))}
          </tr>
        </thead>
        <tbody className="divide-y divide-line">{children}</tbody>
      </table>
    </div>
  )
}

export const cell = 'px-2.5 py-1.5 align-middle'
