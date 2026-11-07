import { describe, expect, it } from 'vitest'
import { GAME_VERSIONS, gameVersionByKey, gameVersionsByGeneration } from './gameVersions'

describe('catálogo de todos os jogos', () => {
  it('tem os 22 grupos, com as mesmas chaves do backend', () => {
    expect(GAME_VERSIONS.map((g) => g.key)).toEqual([
      'rb', 'y', 'gs', 'c', 'rs', 'e', 'frlg', 'dp', 'pt', 'hgss', 'bw',
      'b2w2', 'xy', 'oras', 'sm', 'usum', 'lgpe', 'swsh', 'bdsp', 'pla', 'sv', 'za',
    ])
  })

  it('acha o jogo pela chave', () => {
    expect(gameVersionByKey('sv').generation).toBe(9)
    expect(gameVersionByKey('frlg').family).toBe('frlg')
    expect(gameVersionByKey('rb').family).toBeNull()
  })

  it('agrupa por geração, da 1 à 9', () => {
    const byGen = gameVersionsByGeneration()
    expect(byGen.map((g) => g.generation)).toEqual([1, 2, 3, 4, 5, 6, 7, 8, 9])
    expect(byGen[2].games.map((g) => g.key)).toEqual(['rs', 'e', 'frlg'])
  })
})
