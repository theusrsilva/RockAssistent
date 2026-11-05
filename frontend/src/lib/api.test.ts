import { describe, expect, it } from 'vitest'
import { spriteUrl } from './api'

describe('spriteUrl', () => {
  it('sem opções continua o endereço de sempre', () => {
    expect(spriteUrl(25)).toBe('/api/sprites/25')
    expect(spriteUrl(25, {})).toBe('/api/sprites/25')
    expect(spriteUrl(25, { shiny: false })).toBe('/api/sprites/25')
  })

  it('pede o shiny e a forma pelo id do pokemon', () => {
    expect(spriteUrl(94, { shiny: true })).toBe('/api/sprites/94?shiny=1')
    expect(spriteUrl(6, { form: 10034 })).toBe('/api/sprites/6?form=10034')
    expect(spriteUrl(6, { shiny: true, form: 10034 })).toBe('/api/sprites/6?shiny=1&form=10034')
  })

  it('a forma padrão (id da própria espécie) não vira parâmetro', () => {
    expect(spriteUrl(6, { form: 6 })).toBe('/api/sprites/6')
  })
})
