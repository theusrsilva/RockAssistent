import type { TFunction } from 'i18next'
import type { SpeciesSummary } from '../../lib/dex'

/**
 * Nome de uma linha da lista da Pokédex. Na forma regional, o nome da espécie com a região ("Raichu de Alola",
 * "Alolan Raichu") e, se houver, o complemento em inglês, como nos dados ("Tauros de Paldea (Blaze Breed)").
 */
export function speciesEntryName(s: Pick<SpeciesSummary, 'name' | 'region' | 'formDetail'>, t: TFunction): string {
  // Fêmea diferente: "Venusaur ♀". Outras formas: o nome da forma, em inglês como nos dados ("Rotom (Wash Rotom)").
  if (s.formDetail === '♀') return `${s.name} ♀`
  if (!s.region) return s.formDetail ? `${s.name} (${s.formDetail})` : s.name
  const name = t(`dex.regional.${s.region}`, { name: s.name })
  return s.formDetail ? `${name} (${s.formDetail})` : name
}
