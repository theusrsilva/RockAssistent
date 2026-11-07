import { afterEach, describe, expect, it, vi } from 'vitest'
import { chooseGuideStarter, readGuideStarter } from './guideStarter'

describe('guideStarter', () => {
  afterEach(() => vi.unstubAllGlobals())

  it('lembra a escolha por jogo no localStorage', () => {
    const map = new Map<string, string>()
    vi.stubGlobal('localStorage', { getItem: (k: string) => map.get(k) ?? null, setItem: (k: string, v: string) => void map.set(k, v) })
    chooseGuideStarter('hgss', 'cyndaquil')
    expect(readGuideStarter('hgss')).toBe('cyndaquil')
    expect(map.get('traderock.guides.starter.hgss')).toBe('cyndaquil')
    expect(readGuideStarter('frlg')).toBeNull()
  })

  it('sem localStorage, fica na memória', () => {
    vi.stubGlobal('localStorage', {
      getItem: () => {
        throw new Error('bloqueado')
      },
      setItem: () => {
        throw new Error('bloqueado')
      },
    })
    expect(readGuideStarter('bw')).toBeNull()
    chooseGuideStarter('bw', 'oshawott')
    expect(readGuideStarter('bw')).toBe('oshawott')
  })
})
