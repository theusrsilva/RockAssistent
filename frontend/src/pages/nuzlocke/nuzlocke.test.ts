import { describe, expect, it } from 'vitest'
import { activeRules, areaStates, areaUsed, bossTeam, chosenVariant, defaultMoves, conditionParts, evolveHow, evolveOptions, formatRule, neighborAreas, formatRules, overCap, runStats, sortAreas, type BossBattle, type MonInfo, type NuzlockeRules, type RunView } from './nuzlocke'

const mon = (over: Partial<MonInfo>): MonInfo => ({
  id: over.id ?? 'm',
  areaId: 'route-1',
  species: 16,
  form: 0,
  nickname: 'Pidge',
  level: 10,
  status: 'party',
  deathCause: null,
  caughtAt: '2026-11-06T18:00:00Z',
  diedAt: null,
  moves: [],
  shiny: false,
  ...over,
})

const rules: NuzlockeRules = {
  firstEncounterOnly: true,
  nicknameRequired: true,
  faintIsDeath: true,
  speciesClause: false,
  shinyClause: false,
  dupesClause: false,
  levelCapRequired: false,
  custom: null,
}

const view = (over: Partial<RunView>): RunView => ({
  run: { id: 'r', name: 'Kanto', gameKey: 'frlg', version: 'firered', starter: null, rules, status: 'active', startedAt: '', endedAt: null, currentAreaId: 'route-1' },
  currentArea: null,
  nextBoss: null,
  levelCap: null,
  areaEncounters: [],
  areaTrainers: [],
  trainersAvailable: true,
  party: [],
  box: [],
  graveyard: [],
  overCap: [],
  lost: [],
  defeatedBosses: [],
  visitedAreas: [],
  ...over,
})

/** t falso: devolve a chave e os valores, para conferir o que foi pedido. */
const t = ((key: string, opts?: Record<string, unknown>) => (opts ? `${key}:${JSON.stringify(opts)}` : key)) as never

const route1 = { id: 'route-1', kind: 'route' }
const route2 = { id: 'route-2', kind: 'route' }

describe('overCap', () => {
  it('lista quem passou do cap', () => {
    const party = [mon({ id: 'a', level: 14 }), mon({ id: 'b', level: 15 }), mon({ id: 'c', level: 16 })]
    expect(overCap(party, 15).map((m) => m.id)).toEqual(['c'])
  })
  it('sem cap ninguém passa', () => {
    expect(overCap([mon({ level: 100 })], null)).toEqual([])
    expect(overCap([mon({ level: 100 })], 0)).toEqual([])
  })
})

describe('regras', () => {
  it('lista as regras ligadas na ordem fixa', () => {
    expect(activeRules({ ...rules, dupesClause: true })).toEqual(['firstEncounterOnly', 'nicknameRequired', 'faintIsDeath', 'dupesClause'])
  })
  it('formata cada regra pela chave de tradução', () => {
    expect(formatRule(t, 'shinyClause')).toBe('nuzlocke.rules.shinyClause.name')
  })
  it('junta as regras e o texto livre', () => {
    expect(formatRules(t, { ...rules, faintIsDeath: false, nicknameRequired: false, custom: '  Sem itens  ' })).toEqual([
      'nuzlocke.rules.firstEncounterOnly.name',
      'Sem itens',
    ])
  })
  it('sem nenhuma regra', () => {
    const none = { ...rules, firstEncounterOnly: false, nicknameRequired: false, faintIsDeath: false }
    expect(formatRules(t, none)).toEqual(['nuzlocke.rules.none'])
  })
})

describe('areaStates (mapa)', () => {
  it('deriva o estado de cada área', () => {
    const v = view({
      visitedAreas: ['pallet-town', 'route-1', 'route-2', 'route-22', 'viridian-forest'],
      party: [mon({ id: 'a', areaId: 'route-1' })],
      box: [mon({ id: 'b', areaId: 'route-22', status: 'box' })],
      graveyard: [mon({ id: 'c', areaId: 'viridian-forest', status: 'dead' })],
      lost: [mon({ id: 'd', areaId: 'route-2', status: 'failed' }), mon({ id: 'e', areaId: 'route-3', status: 'missed' })],
    })
    expect(areaStates(v)).toEqual({
      'pallet-town': 'visited',
      'route-1': 'caught',
      'route-22': 'caught',
      'viridian-forest': 'dead',
      'route-2': 'missed',
      'route-3': 'missed',
    })
  })
  it('capturado vence o pulado (dupe) na mesma área', () => {
    const v = view({ party: [mon({ id: 'a' })], lost: [mon({ id: 'b', status: 'missed' })] })
    expect(areaStates(v)['route-1']).toBe('caught')
  })
})

describe('áreas', () => {
  it('área usada ignora o missed', () => {
    expect(areaUsed(view({ lost: [mon({ status: 'missed' })] }), route1)).toBe(false)
    expect(areaUsed(view({ lost: [mon({ status: 'failed' })] }), route1)).toBe(true)
    expect(areaUsed(view({ graveyard: [mon({ status: 'dead' })] }), route1)).toBe(true)
    expect(areaUsed(view({ party: [mon({})] }), route2)).toBe(false)
    // Inicial automático: usa a cidade, não a rota.
    expect(areaUsed(view({ party: [mon({ isStarter: true })] }), route1)).toBe(false)
    expect(areaUsed(view({ party: [mon({ isStarter: true, areaId: 'pallet-town' })] }), { id: 'pallet-town', kind: 'city' })).toBe(true)
  })
  it('ordena pela ordem do jogo', () => {
    const a = (id: string, order: number) => ({ id, name: id, kind: 'route', order, mapNode: null, pokeApiLocationId: null, bossesBefore: [] })
    expect(sortAreas([a('b', 3), a('a', 1), a('c', 2)]).map((x) => x.id)).toEqual(['a', 'c', 'b'])
  })
})

describe('golpes', () => {
  it('sem golpes gravados, usa os 4 últimos aprendidos por nível', () => {
    const k = (move: string, level: number) => ({ move, name: move, type: 'normal', level })
    const known = [k('tackle', 1), k('sand-attack', 5), k('gust', 9), k('quick-attack', 13), k('tackle', 1), k('whirlwind', 15)]
    expect(defaultMoves(known)).toEqual(['sand-attack', 'gust', 'quick-attack', 'whirlwind'])
  })
})

describe('estatísticas', () => {
  it('conta capturas, mortes, chefes, áreas e shinies', () => {
    const v = view({
      party: [mon({ id: 'a', shiny: true })],
      box: [mon({ id: 'b', status: 'box' })],
      graveyard: [mon({ id: 'c', status: 'dead', shiny: true })],
      lost: [mon({ id: 'd', status: 'failed', shiny: true })],
      defeatedBosses: ['rival-1', 'brock'],
      visitedAreas: ['pallet-town', 'route-1'],
    })
    expect(runStats(v)).toEqual({ caught: 3, deaths: 1, bosses: 2, areas: 2, shinies: 2, lost: 1 })
  })
})

describe('areaStates (precedência)', () => {
  it('morto vence capturado na mesma área; visitada sem registro fica visitada', () => {
    const v = view({
      visitedAreas: ['route-1', 'route-2'],
      party: [mon({ id: 'a', areaId: 'route-1' })],
      graveyard: [mon({ id: 'b', areaId: 'route-1', status: 'dead' })],
    })
    expect(areaStates(v)).toEqual({ 'route-1': 'dead', 'route-2': 'visited' })
  })
})

describe('variantes por inicial', () => {
  const p = (species: number, level: number) => ({ species, form: 0, level, moves: [], ability: null, item: null, teraType: null })
  const rival: BossBattle = {
    id: 'rival-cherrygrove',
    kind: 'rival',
    name: 'Silver',
    title: null,
    location: null,
    order: 1,
    badge: null,
    party: [p(158, 5)],
    levelCap: 5,
    verified: true,
    notes: null,
    versions: null,
    variants: [
      { starter: 'chikorita', party: [p(155, 5)], levelCap: 5, verified: true },
      { starter: 'cyndaquil', party: [p(158, 5)], levelCap: 5, verified: true },
      { starter: 'totodile', party: [p(152, 6)], levelCap: 6, verified: false },
    ],
  }

  it('mostra o time do inicial escolhido', () => {
    expect(chosenVariant(rival, 'totodile')?.starter).toBe('totodile')
    expect(bossTeam(rival, 'totodile')).toMatchObject({ levelCap: 6, verified: false })
    expect(bossTeam(rival, 'chikorita').party[0].species).toBe(155)
  })

  it('sem escolha (ou inicial sem variante) começa na primeira variante', () => {
    expect(chosenVariant(rival, null)?.starter).toBe('chikorita')
    expect(chosenVariant(rival, 'bulbasaur')?.starter).toBe('chikorita')
  })

  it('chefe sem variantes usa o time padrão', () => {
    const gym = { ...rival, variants: null }
    expect(chosenVariant(gym, 'chikorita')).toBeNull()
    expect(bossTeam(gym, 'chikorita').party[0].species).toBe(158)
  })
})

describe('evolveOptions', () => {
  const ev = (species: number, name: string, conditions: string[]) => ({ species, name, conditions, onEvolve: [], upcoming: [] })

  it('por nível: pronta só a partir do nível', () => {
    const pidgey = [ev(17, 'Pidgeotto', ['level 18'])]
    expect(evolveOptions(pidgey, 18)).toEqual([{ species: 17, form: 0, formId: null, name: 'Pidgeotto', how: { kind: 'level', level: 18 }, ready: true }])
    // Forma regional do destino (Raticate de Alola).
    expect(evolveOptions([{ ...ev(20, 'Raticate', ['level 20']), form: 1, formId: 10092 }], 20)[0]).toMatchObject({ form: 1, formId: 10092 })
    expect(evolveOptions(pidgey, 17)[0].ready).toBe(false)
  })

  it('por pedra: uma opção por evolução, sempre disponível', () => {
    const eevee = [ev(134, 'Vaporeon', ['use Water Stone']), ev(135, 'Jolteon', ['use Thunder Stone']), ev(136, 'Flareon', ['use Fire Stone'])]
    const options = evolveOptions(eevee, 5)
    expect(options).toHaveLength(3)
    expect(options.every((o) => o.ready && o.how.kind === 'item')).toBe(true)
    expect(options[1].how).toEqual({ kind: 'item', item: 'Thunder Stone' })
  })

  it('por troca e por amizade', () => {
    expect(evolveOptions([ev(94, 'Gengar', ['trade'])], 30)[0]).toMatchObject({ how: { kind: 'trade', detail: null }, ready: true })
    expect(evolveHow('trade holding Metal Coat')).toEqual({ kind: 'trade', detail: 'holding Metal Coat' })
    expect(evolveHow('level up friendship 220')).toEqual({ kind: 'friendship' })
    expect(evolveHow('level up at night friendship 160')).toEqual({ kind: 'other', text: 'level up at night friendship 160' })
  })
})

describe('neighborAreas', () => {
  const area = (id: string, order: number) => ({ id, name: id, kind: 'route', order, mapNode: null, pokeApiLocationId: null, bossesBefore: [] })
  const areas = [area('pallet-town', 1), area('route-1', 2), area('viridian-city', 3)]

  it('acha a anterior e a próxima', () => {
    expect(neighborAreas(areas, 'route-1')).toEqual({ prev: areas[0], next: areas[2] })
  })

  it('pontas e área desconhecida', () => {
    expect(neighborAreas(areas, 'pallet-town')).toEqual({ prev: null, next: areas[1] })
    expect(neighborAreas(areas, 'viridian-city')).toEqual({ prev: areas[1], next: null })
    expect(neighborAreas(areas, 'x')).toEqual({ prev: null, next: null })
    expect(neighborAreas(areas, null)).toEqual({ prev: null, next: null })
  })
})

describe('conditionParts', () => {
  it('entende os casos comuns', () => {
    expect(conditionParts('level up at night friendship 160')).toEqual([{ key: 'friendship' }, { key: 'night' }])
    expect(conditionParts('level up knowing Ancient Power')).toEqual([{ key: 'knowing', values: { move: 'Ancient Power' } }])
    expect(conditionParts('level up beauty 171')).toEqual([{ key: 'beauty' }])
    expect(conditionParts('level up at day holding Oval Stone')).toEqual([{ key: 'day' }, { key: 'holding', values: { item: 'Oval Stone' } }])
    expect(conditionParts('level up at Mt. Coronet')).toEqual([{ key: 'place', values: { place: 'Mt. Coronet' } }])
  })

  it('sem nada conhecido, vazio (texto genérico)', () => {
    expect(conditionParts('spin')).toEqual([])
  })
})
