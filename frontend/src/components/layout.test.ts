import { describe, expect, it } from 'vitest'
import ptBR from '../locales/pt-BR.json'
import en from '../locales/en.json'
import { mainLinks } from './Layout'

function lookup(obj: unknown, key: string): unknown {
  return key.split('.').reduce<unknown>((o, k) => (o && typeof o === 'object' ? (o as Record<string, unknown>)[k] : undefined), obj)
}

describe('menu principal', () => {
  it('segue a ordem das áreas do RockAssistent', () => {
    expect(mainLinks.map((l) => l.to)).toEqual(['/', '/pokedex', '/guias', '/boxes', '/trocas', '/mystery-gift', '/nuzlocke', '/hunt', '/historico'])
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
