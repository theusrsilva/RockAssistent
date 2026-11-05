import type { GameFamily } from './types'

/** Região onde o jogo se passa (JSON do enum Region do backend). O Legends: Z-A fica só em Lumiose. */
export type Region =
  | 'kanto'
  | 'johto'
  | 'hoenn'
  | 'sinnoh'
  | 'unova'
  | 'kalos'
  | 'alola'
  | 'galar'
  | 'hisui'
  | 'paldea'
  | 'lumiose'

/** Um grupo de versões da série principal, como o PokeAPI as agrupa. */
export interface GameVersionDef {
  key: string
  name: string
  generation: number
  region: Region
  /** Identificador de version_groups.csv do PokeAPI. */
  pokeApiVersionGroup: string
  /** Identificadores de versions.csv do PokeAPI. */
  pokeApiVersions: string[]
  /** Ligação com o catálogo dos jogos do Switch (games.ts), quando é um deles. */
  family: GameFamily | null
  /** Amuleto Brilhante: do Black 2/White 2 em diante (inclui o Let's Go). */
  hasShinyCharm: boolean
  /** Método Masuda: do Diamond/Pearl em diante, menos nos jogos sem breeding (Let's Go, Arceus e Z-A). */
  hasMasuda: boolean
}

const g = (
  key: string,
  name: string,
  generation: number,
  region: Region,
  pokeApiVersionGroup: string,
  pokeApiVersions: string[],
  family: GameFamily | null,
  hasShinyCharm: boolean,
  hasMasuda: boolean,
): GameVersionDef => ({
  key,
  name,
  generation,
  region,
  pokeApiVersionGroup,
  pokeApiVersions,
  family,
  hasShinyCharm,
  hasMasuda,
})

// Mesma ordem e dados do GameVersions do backend (GET /api/dex/games).
export const GAME_VERSIONS: GameVersionDef[] = [
  g('rb', 'Red / Blue', 1, 'kanto', 'red-blue', ['red', 'blue'], null, false, false),
  g('y', 'Yellow', 1, 'kanto', 'yellow', ['yellow'], null, false, false),
  g('gs', 'Gold / Silver', 2, 'johto', 'gold-silver', ['gold', 'silver'], null, false, false),
  g('c', 'Crystal', 2, 'johto', 'crystal', ['crystal'], null, false, false),
  g('rs', 'Ruby / Sapphire', 3, 'hoenn', 'ruby-sapphire', ['ruby', 'sapphire'], null, false, false),
  g('e', 'Emerald', 3, 'hoenn', 'emerald', ['emerald'], null, false, false),
  g('frlg', 'FireRed / LeafGreen', 3, 'kanto', 'firered-leafgreen', ['firered', 'leafgreen'], 'frlg', false, false),
  g('dp', 'Diamond / Pearl', 4, 'sinnoh', 'diamond-pearl', ['diamond', 'pearl'], null, false, true),
  g('pt', 'Platinum', 4, 'sinnoh', 'platinum', ['platinum'], null, false, true),
  g('hgss', 'HeartGold / SoulSilver', 4, 'johto', 'heartgold-soulsilver', ['heartgold', 'soulsilver'], null, false, true),
  g('bw', 'Black / White', 5, 'unova', 'black-white', ['black', 'white'], null, false, true),
  g('b2w2', 'Black 2 / White 2', 5, 'unova', 'black-2-white-2', ['black-2', 'white-2'], null, true, true),
  g('xy', 'X / Y', 6, 'kalos', 'x-y', ['x', 'y'], null, true, true),
  g('oras', 'Omega Ruby / Alpha Sapphire', 6, 'hoenn', 'omega-ruby-alpha-sapphire', ['omega-ruby', 'alpha-sapphire'], null, true, true),
  g('sm', 'Sun / Moon', 7, 'alola', 'sun-moon', ['sun', 'moon'], null, true, true),
  g('usum', 'Ultra Sun / Ultra Moon', 7, 'alola', 'ultra-sun-ultra-moon', ['ultra-sun', 'ultra-moon'], null, true, true),
  g('lgpe', "Let's Go, Pikachu! / Let's Go, Eevee!", 7, 'kanto', 'lets-go-pikachu-lets-go-eevee', ['lets-go-pikachu', 'lets-go-eevee'], 'lgpe', true, false),
  g('swsh', 'Sword / Shield', 8, 'galar', 'sword-shield', ['sword', 'shield'], 'swsh', true, true),
  g('bdsp', 'Brilliant Diamond / Shining Pearl', 8, 'sinnoh', 'brilliant-diamond-shining-pearl', ['brilliant-diamond', 'shining-pearl'], 'bdsp', true, true),
  g('pla', 'Legends: Arceus', 8, 'hisui', 'legends-arceus', ['legends-arceus'], 'pla', true, false),
  g('sv', 'Scarlet / Violet', 9, 'paldea', 'scarlet-violet', ['scarlet', 'violet'], 'sv', true, true),
  g('za', 'Legends: Z-A', 9, 'lumiose', 'legends-za', ['legends-za'], 'za', true, false),
]

export const gameVersionByKey = (key: string): GameVersionDef => GAME_VERSIONS.find((v) => v.key === key)!

/** Jogos agrupados por geração, na ordem de lançamento. */
export const gameVersionsByGeneration = (): { generation: number; games: GameVersionDef[] }[] => {
  const groups: { generation: number; games: GameVersionDef[] }[] = []
  for (const game of GAME_VERSIONS) {
    const last = groups[groups.length - 1]
    if (last?.generation === game.generation) last.games.push(game)
    else groups.push({ generation: game.generation, games: [game] })
  }
  return groups
}
