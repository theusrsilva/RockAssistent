import { describe, expect, it, vi } from 'vitest'
import { comboFromEvent, formatCombo, handleHotkey, matchesCombo, parseCombo, shouldIgnore, type KeyLike } from './hotkeys'

const key = (code: string, extra: Partial<KeyLike> = {}): KeyLike => ({
  code,
  ctrlKey: false,
  altKey: false,
  shiftKey: false,
  metaKey: false,
  repeat: false,
  target: { tagName: 'BODY', isContentEditable: false },
  preventDefault: () => undefined,
  ...extra,
})

describe('parseCombo', () => {
  it('lê tecla e modificadores', () => {
    expect(parseCombo('Space')).toEqual({ ctrl: false, alt: false, shift: false, meta: false, code: 'Space' })
    expect(parseCombo('Ctrl+ArrowUp')).toEqual({ ctrl: true, alt: false, shift: false, meta: false, code: 'ArrowUp' })
    expect(parseCombo('Shift+Equal')).toMatchObject({ shift: true, code: 'Equal' })
    expect(parseCombo('ctrl + alt + KeyA')).toMatchObject({ ctrl: true, alt: true, code: 'KeyA' })
  })

  it('recusa combo sem tecla ou com modificador desconhecido', () => {
    expect(parseCombo('')).toBeNull()
    expect(parseCombo('Ctrl')).toBeNull()
    expect(parseCombo('Ctrl+')).toBeNull()
    expect(parseCombo('Hyper+KeyA')).toBeNull()
  })
})

describe('matchesCombo', () => {
  it('compara a tecla e exige os mesmos modificadores', () => {
    expect(matchesCombo(parseCombo('Space')!, key('Space'))).toBe(true)
    expect(matchesCombo(parseCombo('Space')!, key('Space', { ctrlKey: true }))).toBe(false)
    expect(matchesCombo(parseCombo('Ctrl+ArrowUp')!, key('ArrowUp', { ctrlKey: true }))).toBe(true)
    expect(matchesCombo(parseCombo('Ctrl+ArrowUp')!, key('ArrowUp'))).toBe(false)
    expect(matchesCombo(parseCombo('Shift+Equal')!, key('Equal', { shiftKey: true }))).toBe(true)
  })
})

describe('shouldIgnore', () => {
  it('ignora quem digita num campo de texto', () => {
    expect(shouldIgnore(key('Space', { target: { tagName: 'INPUT', isContentEditable: false } }))).toBe(true)
    expect(shouldIgnore(key('Space', { target: { tagName: 'TEXTAREA', isContentEditable: false } }))).toBe(true)
    expect(shouldIgnore(key('Space', { target: { tagName: 'SELECT', isContentEditable: false } }))).toBe(true)
    expect(shouldIgnore(key('Space', { target: { tagName: 'DIV', isContentEditable: true } }))).toBe(true)
    expect(shouldIgnore(key('Space'))).toBe(false)
  })

  it('ignora tecla segurada (repetição) e janela modal aberta', () => {
    expect(shouldIgnore(key('Space', { repeat: true }))).toBe(true)
    expect(shouldIgnore(key('Space'), true)).toBe(true)
  })
})

describe('handleHotkey', () => {
  it('evento de <input> não chama o atalho (digitar "+" num campo não soma)', () => {
    const handler = vi.fn()
    const preventDefault = vi.fn()
    const fromInput = key('Equal', { shiftKey: true, preventDefault, target: { tagName: 'INPUT', isContentEditable: false } })
    expect(handleHotkey(parseCombo('Shift+Equal'), fromInput, handler)).toBe(false)
    expect(handler).not.toHaveBeenCalled()
    expect(preventDefault).not.toHaveBeenCalled()
  })

  it('combo com modificador chama o atalho e segura o padrão do navegador', () => {
    const handler = vi.fn()
    const preventDefault = vi.fn()
    expect(handleHotkey(parseCombo('Ctrl+ArrowUp'), key('ArrowUp', { ctrlKey: true, preventDefault }), handler)).toBe(true)
    expect(handler).toHaveBeenCalledOnce()
    expect(preventDefault).toHaveBeenCalledOnce()
    expect(handleHotkey(parseCombo('Ctrl+ArrowUp'), key('ArrowUp'), handler)).toBe(false)
    expect(handler).toHaveBeenCalledOnce()
  })
})

describe('comboFromEvent e formatCombo', () => {
  it('grava o combo da tecla apertada', () => {
    expect(comboFromEvent(key('Space'))).toBe('Space')
    expect(comboFromEvent(key('ArrowUp', { ctrlKey: true, shiftKey: true }))).toBe('Ctrl+Shift+ArrowUp')
    // Só um modificador apertado ainda não é um atalho.
    expect(comboFromEvent(key('ShiftLeft', { shiftKey: true }))).toBeNull()
    expect(comboFromEvent(key('ControlRight', { ctrlKey: true }))).toBeNull()
  })

  it('mostra nomes curtos', () => {
    const names = { Space: 'Espaço' }
    expect(formatCombo('Ctrl+ArrowUp', names)).toBe('Ctrl + ↑')
    expect(formatCombo('Space', names)).toBe('Espaço')
    expect(formatCombo('Shift+KeyA', names)).toBe('Shift + A')
    expect(formatCombo('Digit5', names)).toBe('5')
    expect(formatCombo('Numpad5', names)).toBe('Num 5')
  })
})
