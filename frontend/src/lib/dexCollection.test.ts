import { describe, expect, it } from 'vitest'
import { collectionProgress, matchesCollectionFilter, ownedKey, ownedSource, type CollectionSets } from './dexCollection'

const sets: CollectionSets = {
  marks: new Set([ownedKey(26, 0, false), ownedKey(26, 10100, true)]),
  vault: new Set([ownedKey(25, 0, false), ownedKey(26, 0, false), ownedKey(94, 0, true)]),
}
const raichu = { id: 26, formId: null }
const alolan = { id: 26, formId: 10100 }
const pikachu = { id: 25, formId: undefined }
const gengar = { id: 94 }
const mew = { id: 151 }

describe('coleção da Pokédex', () => {
  it('a marcação vale sempre; o cofre só conta com a opção ligada', () => {
    expect(ownedSource(sets, true, ownedKey(26, 0, false))).toBe('mark')
    expect(ownedSource(sets, false, ownedKey(26, 0, false))).toBe('mark')
    expect(ownedSource(sets, true, ownedKey(25, 0, false))).toBe('vault')
    expect(ownedSource(sets, false, ownedKey(25, 0, false))).toBeNull()
    expect(ownedSource(sets, true, ownedKey(151, 0, false))).toBeNull()
  })

  it('espécie e forma regional, normal e shiny, são quatro marcações separadas', () => {
    expect(ownedSource(sets, false, ownedKey(26, 0, true))).toBeNull()
    expect(ownedSource(sets, false, ownedKey(26, 10100, false))).toBeNull()
    expect(ownedSource(sets, false, ownedKey(26, 10100, true))).toBe('mark')
  })

  it('o progresso conta quem tem normal ou shiny, e os shiny à parte', () => {
    const entries = [pikachu, raichu, alolan, gengar, mew]
    expect(collectionProgress(entries, sets, true)).toEqual({ owned: 4, shiny: 2, total: 5 })
    expect(collectionProgress(entries, sets, false)).toEqual({ owned: 2, shiny: 1, total: 5 })
  })

  it('filtro Mostrar', () => {
    const entries = [pikachu, raichu, alolan, gengar, mew]
    const ids = (filter: Parameters<typeof matchesCollectionFilter>[1], vault: boolean) =>
      entries.filter((e) => matchesCollectionFilter(e, filter, sets, vault)).map((e) => `${e.id}${'formId' in e && e.formId ? 'a' : ''}`)
    expect(ids('', true)).toHaveLength(5)
    expect(ids('tenho', true)).toEqual(['25', '26', '26a', '94'])
    expect(ids('tenho', false)).toEqual(['26', '26a'])
    expect(ids('faltam', true)).toEqual(['151'])
    expect(ids('shiny', true)).toEqual(['25', '26', '151'])
  })
})
