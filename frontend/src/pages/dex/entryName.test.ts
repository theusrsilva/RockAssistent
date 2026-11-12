import { describe, expect, it } from 'vitest'
import type { TFunction } from 'i18next'
import ptBR from '../../locales/pt-BR.json'
import en from '../../locales/en.json'
import { speciesEntryName } from './entryName'

const translator = (dict: typeof ptBR) =>
  ((key: string, opts: { name: string }) =>
    (dict.dex.regional as Record<string, string>)[key.replace('dex.regional.', '')].replace('{{name}}', opts.name)) as unknown as TFunction

describe('nome da linha da Pokédex', () => {
  it('a espécie fica com o próprio nome', () => {
    expect(speciesEntryName({ name: 'Raichu' }, translator(ptBR))).toBe('Raichu')
  })

  it('a forma regional leva a região, no idioma da tela', () => {
    expect(speciesEntryName({ name: 'Raichu', region: 'alola' }, translator(ptBR))).toBe('Raichu de Alola')
    expect(speciesEntryName({ name: 'Raichu', region: 'alola' }, translator(en))).toBe('Alolan Raichu')
    expect(speciesEntryName({ name: 'Meowth', region: 'galar' }, translator(ptBR))).toBe('Meowth de Galar')
    expect(speciesEntryName({ name: 'Zorua', region: 'hisui' }, translator(en))).toBe('Hisuian Zorua')
  })

  it('outras formas levam o nome da forma; a fêmea, o símbolo', () => {
    expect(speciesEntryName({ name: 'Rotom', formDetail: 'Wash Rotom' }, translator(ptBR))).toBe('Rotom (Wash Rotom)')
    expect(speciesEntryName({ name: 'Unown', formDetail: 'B' }, translator(ptBR))).toBe('Unown (B)')
    expect(speciesEntryName({ name: 'Venusaur', formDetail: '♀' }, translator(ptBR))).toBe('Venusaur ♀')
  })

  it('o complemento da forma vai entre parênteses', () => {
    expect(speciesEntryName({ name: 'Tauros', region: 'paldea', formDetail: 'Blaze Breed' }, translator(ptBR))).toBe(
      'Tauros de Paldea (Blaze Breed)',
    )
  })
})
