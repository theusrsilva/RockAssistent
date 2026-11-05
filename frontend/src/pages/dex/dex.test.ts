import { describe, expect, it } from 'vitest'
import {
  formatEvolutionCondition,
  formatMultiplier,
  genderRatio,
  multiplierClass,
  sortMoves,
  versionName,
  type LearnedMove,
} from '../../lib/dex'

describe('condição de evolução', () => {
  it('em inglês fica como veio do backend', () => {
    expect(formatEvolutionCondition('level 25', 'en')).toBe('level 25')
    expect(formatEvolutionCondition('trade holding Metal Coat', 'en')).toBe('trade holding Metal Coat')
  })

  it('traduz as partes conhecidas e mantém os nomes de itens e golpes', () => {
    expect(formatEvolutionCondition('level 25', 'pt-BR')).toBe('nível 25')
    expect(formatEvolutionCondition('level up', 'pt-BR')).toBe('subir de nível')
    expect(formatEvolutionCondition('trade', 'pt-BR')).toBe('troca')
    expect(formatEvolutionCondition('trade holding Metal Coat', 'pt-BR')).toBe('troca segurando Metal Coat')
    expect(formatEvolutionCondition('use Thunder Stone', 'pt-BR')).toBe('usar Thunder Stone')
    expect(formatEvolutionCondition('level up friendship 160 at night', 'pt-BR')).toBe('subir de nível amizade 160 à noite')
    expect(formatEvolutionCondition('level up knowing Ancient Power', 'pt-BR')).toBe('subir de nível sabendo Ancient Power')
    expect(formatEvolutionCondition('level 20 female', 'pt-BR')).toBe('nível 20 fêmea')
    expect(formatEvolutionCondition('level 20 Attack > Defense', 'pt-BR')).toBe('nível 20 Ataque > Defesa')
    expect(formatEvolutionCondition('level up at Mt. Coronet', 'pt-BR')).toBe('subir de nível em Mt. Coronet')
    expect(formatEvolutionCondition('trade for Shelmet', 'pt-BR')).toBe('troca por Shelmet')
  })

  it('traduz o tipo dentro da frase com a função dada', () => {
    const typeName = (t: string) => (t === 'fairy' ? 'Fada' : t === 'dark' ? 'Sombrio' : t)
    expect(formatEvolutionCondition('level up knowing a fairy move', 'pt-BR', typeName)).toBe(
      'subir de nível sabendo um golpe do tipo Fada',
    )
    expect(formatEvolutionCondition('level 32 with a dark type in party', 'pt-BR', typeName)).toBe(
      'nível 32 com um Pokémon do tipo Sombrio na equipe',
    )
    expect(formatEvolutionCondition('level up with Remoraid in party', 'pt-BR')).toBe(
      'subir de nível com Remoraid na equipe',
    )
  })

  it('o que não conhece passa sem mudança', () => {
    expect(formatEvolutionCondition('three critical hits', 'pt-BR')).toBe('three critical hits')
  })
})

describe('multiplicador de dano', () => {
  it('escolhe a cor pelo multiplicador', () => {
    expect(multiplierClass(4)).toContain('bg-bad-soft')
    expect(multiplierClass(2)).toContain('bg-bad-soft')
    expect(multiplierClass(0.5)).toContain('bg-ok-soft')
    expect(multiplierClass(0.25)).toContain('bg-ok-soft')
    expect(multiplierClass(0)).toContain('bg-tile')
    expect(multiplierClass(1)).not.toMatch(/bad|ok-soft|tile/)
  })

  it('mostra frações', () => {
    expect(formatMultiplier(4)).toBe('×4')
    expect(formatMultiplier(0.5)).toBe('×½')
    expect(formatMultiplier(0.25)).toBe('×¼')
    expect(formatMultiplier(0)).toBe('×0')
    expect(formatMultiplier(1)).toBe('×1')
  })
})

const move = (over: Partial<LearnedMove>): LearnedMove => ({
  id: 1,
  name: 'Tackle',
  type: 'normal',
  class: 'physical',
  power: 40,
  accuracy: 100,
  pp: 35,
  level: null,
  machineNumber: null,
  machine: null,
  ...over,
})

describe('ordenação de golpes', () => {
  const moves = [
    move({ id: 1, name: 'Lick', level: 1, power: 30 }),
    move({ id: 2, name: 'Hypnosis', level: 1, power: null, class: 'status' }),
    move({ id: 3, name: 'Shadow Ball', level: 33, power: 80 }),
    move({ id: 4, name: 'Curse', level: 13, power: null }),
  ]

  it('por nível, com empate pelo nome', () => {
    expect(sortMoves(moves, 'level', 'asc').map((m) => m.name)).toEqual(['Hypnosis', 'Lick', 'Curse', 'Shadow Ball'])
  })

  it('por força, sem força vai para o fim nos dois sentidos', () => {
    expect(sortMoves(moves, 'power', 'desc').map((m) => m.name)).toEqual(['Shadow Ball', 'Lick', 'Curse', 'Hypnosis'])
    expect(sortMoves(moves, 'power', 'asc').map((m) => m.name)).toEqual(['Lick', 'Shadow Ball', 'Curse', 'Hypnosis'])
  })

  it('por TM usa o número da máquina', () => {
    const tms = [move({ name: 'B', machineNumber: 30 }), move({ name: 'A', machineNumber: 6 })]
    expect(sortMoves(tms, 'machine', 'asc').map((m) => m.name)).toEqual(['A', 'B'])
  })

  it('não muda a lista original', () => {
    const copy = [...moves]
    sortMoves(moves, 'name', 'desc')
    expect(moves).toEqual(copy)
  })
})

describe('gênero', () => {
  it('converte a taxa do PokeAPI (oitavos de fêmea)', () => {
    expect(genderRatio(-1)).toBeNull()
    expect(genderRatio(4)).toEqual({ female: 50, male: 50 })
    expect(genderRatio(1)).toEqual({ female: 12.5, male: 87.5 })
    expect(genderRatio(8)).toEqual({ female: 100, male: 0 })
  })
})

describe('nome da versão', () => {
  it('usa o nome do jogo', () => {
    expect(versionName('leafgreen')).toBe('LeafGreen')
    expect(versionName('scarlet')).toBe('Scarlet')
    expect(versionName('legends-za')).toBe('Legends: Z-A')
    expect(versionName('colosseum')).toBe('Colosseum')
  })
})
