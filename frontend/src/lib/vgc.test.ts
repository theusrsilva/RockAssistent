import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest'
import { importPasteToBoxes, isTeamWeakness, loadMyTeam, MY_TEAM_KEY, pasteBytes, saveMyTeam, uncovered, type TeamReport } from './vgc'

/** localStorage em memória (os testes rodam no Node). */
function memoryStorage(): Storage {
  const map = new Map<string, string>()
  return {
    get length() {
      return map.size
    },
    clear: () => map.clear(),
    getItem: (k) => map.get(k) ?? null,
    key: (i) => [...map.keys()][i] ?? null,
    removeItem: (k) => void map.delete(k),
    setItem: (k, v) => void map.set(k, String(v)),
  }
}

describe('vgc', () => {
  beforeEach(() => vi.stubGlobal('localStorage', memoryStorage()))
  afterEach(() => {
    vi.restoreAllMocks()
    vi.unstubAllGlobals()
  })

  it('fraqueza do time: 3 fracos, ou 2 sem ninguém que resista', () => {
    expect(isTeamWeakness({ type: 'ice', weak: 3, resist: 2, immune: 0, factors: [] })).toBe(true)
    expect(isTeamWeakness({ type: 'ice', weak: 2, resist: 0, immune: 0, factors: [] })).toBe(true)
    expect(isTeamWeakness({ type: 'ice', weak: 2, resist: 0, immune: 1, factors: [] })).toBe(false)
    expect(isTeamWeakness({ type: 'ice', weak: 1, resist: 0, immune: 0, factors: [] })).toBe(false)
  })

  it('tipos sem cobertura super efetiva', () => {
    const report = { coverage: [{ type: 'fire', best: 2, superEffectiveBy: ['water'] }, { type: 'steel', best: 0.5, superEffectiveBy: [] }] } as unknown as TeamReport
    expect(uncovered(report)).toEqual(['steel'])
  })

  it('conta os bytes em UTF-8', () => {
    expect(pasteBytes('é')).toBe(2)
  })

  it('Minha equipe fica no localStorage; vazio apaga', () => {
    expect(loadMyTeam()).toBe('')
    expect(saveMyTeam('Pikachu')).toBe(true)
    expect(localStorage.getItem(MY_TEAM_KEY)).toBe('Pikachu')
    expect(loadMyTeam()).toBe('Pikachu')
    saveMyTeam('  ')
    expect(localStorage.getItem(MY_TEAM_KEY)).toBeNull()
  })

  it('sem localStorage (bloqueado), não quebra', () => {
    vi.stubGlobal('localStorage', undefined)
    expect(loadMyTeam()).toBe('')
    expect(saveMyTeam('x')).toBe(false)
  })

  it('importa para o cofre: showdown → generate → save, e lista os que falharam', async () => {
    const calls: string[] = []
    vi.spyOn(globalThis, 'fetch').mockImplementation(async (input) => {
      const url = String(input)
      calls.push(url)
      const body =
        url.endsWith('/showdown')
          ? [
              { spec: { species: 25 }, problems: [], text: 'Pikachu @ Light Ball\n- Thunderbolt' },
              { spec: null, problems: ['species_not_in_game'], text: 'Floette-Eternal @ Floettite' },
            ]
          : url.endsWith('/generate')
            ? { data: 'AAAA', errors: [] }
            : { created: true }
      return new Response(JSON.stringify(body), { status: 200 })
    })
    const outcome = await importPasteToBoxes('...', 'pt-BR')
    expect(outcome).toEqual({ saved: 1, failed: ['Floette-Eternal @ Floettite'] })
    expect(calls).toEqual(['/api/builder/showdown', '/api/builder/generate', '/api/builder/save'])
  })
})
