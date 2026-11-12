import type { TFunction } from 'i18next'
import type { SpeciesSummary } from '../../lib/dex'

/**
 * Nome de uma linha da lista da Pokédex. Na forma regional, o nome da espécie com a região ("Raichu de Alola",
 * "Alolan Raichu") e, se houver, o complemento em inglês, como nos dados ("Tauros de Paldea (Blaze Breed)").
 */
export function speciesEntryName(s: Pick<SpeciesSummary, 'name' | 'region' | 'formDetail'>, t: TFunction): string {
  if (!s.region) return s.name
  const name = t(`dex.regional.${s.region}`, { name: s.name })
  return s.formDetail ? `${name} (${s.formDetail})` : name
}
