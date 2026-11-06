import { describe, expect, it } from 'vitest'
import { activeRules, areaStates, areaUsed, defaultMoves, formatRule, formatRules, overCap, runStats, sortAreas, type MonInfo, type NuzlockeRules, type RunView } from './nuzlocke'

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
  run: { id: 'r', name: 'Kanto', gameKey: 'frlg', version: 'firered', rules, status: 'active', startedAt: '', endedAt: null, currentAreaId: 'route-1' },
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
    expect(areaUsed(view({ lost: [mon({ status: 'missed' })] }), 'route-1')).toBe(false)
    expect(areaUsed(view({ lost: [mon({ status: 'failed' })] }), 'route-1')).toBe(true)
    expect(areaUsed(view({ graveyard: [mon({ status: 'dead' })] }), 'route-1')).toBe(true)
    expect(areaUsed(view({ party: [mon({})] }), 'route-2')).toBe(false)
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
