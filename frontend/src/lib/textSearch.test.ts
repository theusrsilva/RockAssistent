import { describe, expect, it } from 'vitest'
import { countMatches, findMatches, locate, normalizeForSearch, normalizeQuery } from './textSearch'

describe('busca no guia', () => {
  it('ignora acentos e maiúsculas', () => {
    expect(normalizeQuery('  Ceruléan   City ')).toBe('cerulean city')
    expect(findMatches('Vá para Cerulean City.', 'ceruléan')).toEqual([{ start: 8, end: 16 }])
    expect(findMatches('Pegue a Vara Boa aqui', 'vara boa')).toEqual([{ start: 8, end: 16 }])
    expect(findMatches('Pokémon', 'pokemon')).toEqual([{ start: 0, end: 7 }])
  })

  it('posições do texto original mesmo com acento decomposto', () => {
    const text = 'Pokémon e Pokémon'
    expect(normalizeForSearch(text).text).toBe('pokemon e pokemon')
    const [a, b] = findMatches(text, 'POKEMON')
    expect(text.slice(a.start, a.end)).toBe('Pokémon')
    expect(text.slice(b.start, b.end)).toBe('Pokémon')
  })

  it('várias ocorrências no mesmo texto, sem sobreposição', () => {
    expect(findMatches('aaaa', 'aa')).toEqual([
      { start: 0, end: 2 },
      { start: 2, end: 4 },
    ])
    expect(countMatches('Brock, depois Misty; Brock de novo', 'brock')).toBe(2)
    expect(findMatches('qualquer', '   ')).toEqual([])
  })

  it('acha o nó de uma posição do texto juntado', () => {
    const starts = [0, 5, 9]
    expect(locate(starts, 0)).toEqual({ node: 0, offset: 0 })
    expect(locate(starts, 6)).toEqual({ node: 1, offset: 1 })
    expect(locate(starts, 9)).toEqual({ node: 2, offset: 0 })
    expect(locate(starts, 12)).toEqual({ node: 2, offset: 3 })
  })
})
