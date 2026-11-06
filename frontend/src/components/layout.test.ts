import { describe, expect, it } from 'vitest'
import ptBR from '../locales/pt-BR.json'
import en from '../locales/en.json'
import { mainLinks, moreLinks, tabLinks } from './Layout'

function lookup(obj: unknown, key: string): unknown {
  return key.split('.').reduce<unknown>((o, k) => (o && typeof o === 'object' ? (o as Record<string, unknown>)[k] : undefined), obj)
}

describe('menu principal', () => {
  it('segue a ordem das áreas do RockAssistent', () => {
    expect(mainLinks.map((l) => l.to)).toEqual(['/', '/pokedex', '/guias', '/boxes', '/trocas', '/mystery-gift', '/nuzlocke', '/hunt', '/historico'])
  })

  it('no celular, as abas são Início, Pokédex, Boxes e Trocas; Guias vai para o Mais', () => {
    expect(tabLinks.map((l) => l.to)).toEqual(['/', '/pokedex', '/boxes', '/trocas'])
    expect(moreLinks.map((l) => l.to)).toContain('/guias')
    for (const l of tabLinks) {
      expect(typeof lookup(ptBR, `ui.mobileNav.${l.icon}`), l.icon).toBe('string')
      expect(typeof lookup(en, `ui.mobileNav.${l.icon}`), l.icon).toBe('string')
    }
  })

  it('o Mystery Gift é só da janela do PC', () => {
    expect(mainLinks.filter((l) => l.localOnly).map((l) => l.to)).toEqual(['/mystery-gift'])
  })

  it('cada item tem texto nos dois idiomas', () => {
    for (const l of mainLinks) {
      expect(typeof lookup(ptBR, l.key), `${l.key} em pt-BR`).toBe('string')
      expect(typeof lookup(en, l.key), `${l.key} em en`).toBe('string')
    }
  })
})
