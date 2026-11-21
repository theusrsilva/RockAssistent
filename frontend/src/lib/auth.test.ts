import { describe, expect, it } from 'vitest'
import { canUsePc } from './auth'
import type { Me } from './types'

const me = (local: boolean, role: Me['role']): Me => ({ authenticated: true, local, id: 'x', username: 'ana', role, csrf: null, serverMode: true })

describe('canUsePc', () => {
  it('janela do PC e administradores pelo celular mexem no PC; contas comuns, não', () => {
    expect(canUsePc(me(true, 'admin'))).toBe(true)
    expect(canUsePc(me(false, 'admin'))).toBe(true)
    expect(canUsePc(me(false, 'user'))).toBe(false)
    // Antes de saber quem é: como a janela local (o mesmo padrão de antes).
    expect(canUsePc(null)).toBe(true)
  })
})
