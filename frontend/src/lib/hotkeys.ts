// Atalhos de teclado do Contador Hunt (com a janela na frente). O combo usa o KeyboardEvent.code (posição da tecla,
// igual em qualquer layout) com os modificadores na frente: "Space", "Backspace", "Ctrl+ArrowUp", "Shift+Equal".
// O mesmo formato é guardado em GET/PUT /api/settings/hotkeys.

import { useEffect, useRef, useState } from 'react'
import { api } from './api'

export interface Combo {
  ctrl: boolean
  alt: boolean
  shift: boolean
  meta: boolean
  code: string
}

/** O pedaço do KeyboardEvent que os atalhos usam (dá para testar sem navegador). */
export interface KeyLike {
  code: string
  ctrlKey: boolean
  altKey: boolean
  shiftKey: boolean
  metaKey: boolean
  repeat: boolean
  target: { tagName?: string; isContentEditable?: boolean } | EventTarget | null
  preventDefault: () => void
}

export const DEFAULT_HOTKEYS = { increment: 'Space', decrement: 'Backspace' }

export interface HotkeySettings {
  increment: string
  decrement: string
  /** Atalho global: vale com o app em segundo plano (só na janela do PC, Windows e X11). */
  global?: boolean
  /** O sistema deixa ter atalho global (vem do GET; não vai no PUT). */
  supported?: boolean
  /** Por que não: wayland, no_x11, in_use, unsupported_os, remote. */
  reason?: string | null
}

/** Teclas que escrevem ou apagam texto (o atalho global as tira também dos campos de texto do app). */
const TEXT_CODES = /^(Space|Backspace|Delete|Equal|Minus|Key[A-Z]|Digit\d|Numpad\w+)$/

/**
 * O combo é uma tecla de texto sem Ctrl, Alt ou Meta (Shift+letra também escreve). Com o atalho global, ela some de
 * todos os programas e dos campos do próprio app: melhor F1–F12 ou um combo com Ctrl/Alt.
 */
export function isTextKey(text: string): boolean {
  const combo = parseCombo(text)
  return !!combo && !combo.ctrl && !combo.alt && !combo.meta && TEXT_CODES.test(combo.code)
}

/** O atalho global está valendo (ligado, o sistema deixa e o registro deu certo). */
export function globalActive(keys: HotkeySettings): boolean {
  return !!keys.global && !!keys.supported && !keys.reason
}

const MODIFIERS: Record<string, keyof Omit<Combo, 'code'>> = {
  ctrl: 'ctrl',
  control: 'ctrl',
  alt: 'alt',
  shift: 'shift',
  meta: 'meta',
}

/** Teclas que sozinhas são só modificadores (não viram atalho ao gravar). */
const MODIFIER_CODES = /^(Shift|Control|Alt|Meta|OS)(Left|Right)?$|^(AltGraph|CapsLock|Fn|FnLock)$/

/** Teclas que não viram atalho: Tab e Enter navegam e clicam, Esc fecha e cancela (o servidor recusa igual). */
const RESERVED_CODES = new Set(['Tab', 'Enter', 'Escape'])

/** "Ctrl+ArrowUp" → { ctrl: true, code: 'ArrowUp' }. Nulo quando não há tecla ou o modificador é desconhecido. */
export function parseCombo(text: string): Combo | null {
  const parts = text.split('+').map((p) => p.trim())
  const code = parts.pop()
  if (!code || MODIFIERS[code.toLowerCase()]) return null
  const combo: Combo = { ctrl: false, alt: false, shift: false, meta: false, code }
  for (const p of parts) {
    const mod = MODIFIERS[p.toLowerCase()]
    if (!mod) return null
    combo[mod] = true
  }
  return combo
}

/** A tecla e exatamente os mesmos modificadores (Ctrl+Space não dispara o atalho Space). */
export function matchesCombo(combo: Combo, e: KeyLike): boolean {
  return (
    e.code === combo.code &&
    e.ctrlKey === combo.ctrl &&
    e.altKey === combo.alt &&
    e.shiftKey === combo.shift &&
    e.metaKey === combo.meta
  )
}

/** Há uma janela modal aberta (Modal de ui.tsx usa aria-modal). */
export function modalOpen(): boolean {
  return typeof document !== 'undefined' && document.querySelector('[aria-modal="true"]') !== null
}

/**
 * Atalho não vale quando a pessoa digita (input, textarea, select ou contentEditable), quando a tecla está sendo
 * segurada (repetição) ou com uma janela modal aberta.
 */
export function shouldIgnore(e: KeyLike, modal = false): boolean {
  if (e.repeat || modal) return true
  const target = e.target as { tagName?: string; isContentEditable?: boolean } | null
  if (!target) return false
  if (target.isContentEditable) return true
  const tag = (target.tagName ?? '').toUpperCase()
  return tag === 'INPUT' || tag === 'TEXTAREA' || tag === 'SELECT'
}

/** Chama o handler quando o evento é o combo e não deve ser ignorado. Devolve se chamou. */
export function handleHotkey(combo: Combo | null, e: KeyLike, handler: () => void, modal = false): boolean {
  if (!combo || !matchesCombo(combo, e) || shouldIgnore(e, modal)) return false
  // Espaço rola a página e Backspace pode voltar: o atalho fica com a tecla.
  e.preventDefault()
  // Espaço e Enter num botão ou link focado também "clicariam" nele: tira o foco para não contar duas vezes.
  if (combo.code === 'Space' || combo.code === 'Enter') {
    const target = e.target as { tagName?: string; blur?: () => void } | null
    const tag = (target?.tagName ?? '').toUpperCase()
    if ((tag === 'BUTTON' || tag === 'A') && typeof target?.blur === 'function') target.blur()
  }
  handler()
  return true
}

/** Combo da tecla apertada (para gravar o atalho). Nulo se só um modificador foi apertado ou a tecla é Tab, Enter ou Esc. */
export function comboFromEvent(e: KeyLike): string | null {
  if (!e.code || MODIFIER_CODES.test(e.code) || RESERVED_CODES.has(e.code)) return null
  const mods = [e.ctrlKey && 'Ctrl', e.altKey && 'Alt', e.shiftKey && 'Shift', e.metaKey && 'Meta'].filter(Boolean)
  return [...mods, e.code].join('+')
}

const SYMBOLS: Record<string, string> = {
  ArrowUp: '↑',
  ArrowDown: '↓',
  ArrowLeft: '←',
  ArrowRight: '→',
  Equal: '=',
  Minus: '-',
  BracketLeft: '[',
  BracketRight: ']',
  Comma: ',',
  Period: '.',
  Slash: '/',
  Semicolon: ';',
  Quote: "'",
  Backquote: '`',
  Backslash: '\\',
  NumpadAdd: 'Num +',
  NumpadSubtract: 'Num -',
  NumpadMultiply: 'Num *',
  NumpadDivide: 'Num /',
  NumpadEnter: 'Num Enter',
}

/** Nome curto de uma tecla: "KeyA" → "A", "Digit5" → "5", "Numpad5" → "Num 5". <names> traduz Space, Backspace... */
export function keyLabel(code: string, names: Record<string, string> = {}): string {
  if (names[code]) return names[code]
  if (SYMBOLS[code]) return SYMBOLS[code]
  const m = /^(Key|Digit)(.+)$/.exec(code)
  if (m) return m[2]
  const num = /^Numpad(\d)$/.exec(code)
  if (num) return `Num ${num[1]}`
  return code
}

/** "Ctrl+ArrowUp" → "Ctrl + ↑". */
export function formatCombo(text: string, names: Record<string, string> = {}): string {
  const combo = parseCombo(text)
  if (!combo) return text
  const mods = [combo.ctrl && 'Ctrl', combo.alt && 'Alt', combo.shift && 'Shift', combo.meta && 'Meta'].filter(Boolean)
  return [...mods, keyLabel(combo.code, names)].join(' + ')
}

/**
 * Atalho enquanto o componente está na tela. <combo> nulo desliga. O handler mais recente é o chamado (não precisa
 * ser estável).
 */
export function useHotkey(combo: string | null, handler: () => void) {
  const latest = useRef(handler)
  useEffect(() => {
    latest.current = handler
  })
  useEffect(() => {
    const parsed = combo ? parseCombo(combo) : null
    if (!parsed) return
    const onKey = (e: KeyboardEvent) => {
      handleHotkey(parsed, e, () => latest.current(), modalOpen())
    }
    window.addEventListener('keydown', onKey)
    return () => window.removeEventListener('keydown', onKey)
  }, [combo])
}

/** Atalhos salvos (GET /api/settings/hotkeys). Até chegar, e se der erro, valem os padrões. */
export function useHotkeySettings(): HotkeySettings {
  const [keys, setKeys] = useState<HotkeySettings>(DEFAULT_HOTKEYS)
  useEffect(() => {
    let alive = true
    api
      .get<HotkeySettings>('/api/settings/hotkeys')
      .then((k) => alive && setKeys(k))
      .catch(() => undefined)
    return () => {
      alive = false
    }
  }, [])
  return keys
}
