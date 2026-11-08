import { describe, expect, it } from 'vitest'
import { loadScrollPosition, saveScrollPosition, scrollAction, type ScrollStorage } from './scrollRestoration'

const memory = (): ScrollStorage & { data: Map<string, string> } => {
  const data = new Map<string, string>()
  return { data, getItem: (k) => data.get(k) ?? null, setItem: (k, v) => void data.set(k, v) }
}

describe('rolagem por entrada do histórico', () => {
  it('guarda e devolve por chave', () => {
    const s = memory()
    saveScrollPosition('a1', 812.4, s)
    saveScrollPosition('b2', 40, s)
    expect(loadScrollPosition('a1', s)).toBe(812)
    expect(loadScrollPosition('b2', s)).toBe(40)
    expect(loadScrollPosition('c3', s)).toBeNull()
  })
  it('regrava a mesma chave', () => {
    const s = memory()
    saveScrollPosition('a1', 100, s)
    saveScrollPosition('a1', 300, s)
    expect(loadScrollPosition('a1', s)).toBe(300)
  })
  it('guarda no máximo 50 entradas, tirando as mais antigas', () => {
    const s = memory()
    for (let i = 0; i < 55; i++) saveScrollPosition(`k${i}`, i, s)
    expect(loadScrollPosition('k0', s)).toBeNull()
    expect(loadScrollPosition('k4', s)).toBeNull()
    expect(loadScrollPosition('k5', s)).toBe(5)
    expect(loadScrollPosition('k54', s)).toBe(54)
  })
  it('sem armazenamento ou com lixo, não quebra', () => {
    const broken: ScrollStorage = {
      getItem: () => {
        throw new Error('bloqueado')
      },
      setItem: () => {
        throw new Error('bloqueado')
      },
    }
    expect(() => saveScrollPosition('a', 1, broken)).not.toThrow()
    expect(loadScrollPosition('a', broken)).toBeNull()
    expect(loadScrollPosition('a', null)).toBeNull()
    const s = memory()
    s.setItem('traderock.scroll', 'não é json')
    expect(loadScrollPosition('a', s)).toBeNull()
  })
})

describe('o que fazer ao chegar numa tela', () => {
  it('voltar restaura o ponto guardado', () => {
    expect(scrollAction('POP', true, 640)).toEqual({ type: 'restore', top: 640 })
  })
  it('tela nova vai ao topo; voltar sem ponto também', () => {
    expect(scrollAction('PUSH', true, null)).toEqual({ type: 'top' })
    expect(scrollAction('POP', true, null)).toEqual({ type: 'top' })
  })
  it('trocar só a consulta ou substituir não mexe', () => {
    expect(scrollAction('PUSH', false, null)).toEqual({ type: 'keep' })
    expect(scrollAction('REPLACE', true, null)).toEqual({ type: 'keep' })
  })
})
