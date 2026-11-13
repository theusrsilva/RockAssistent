import { afterEach, describe, expect, it, vi } from 'vitest'
import { api } from './api'
import { changeMiniWindow, miniWindowChanged, resetMiniWindowForTests } from './miniWindow'

afterEach(() => {
  vi.restoreAllMocks()
  resetMiniWindowForTests()
})

describe('mini janela', () => {
  it('sem janela nativa não chama o servidor', async () => {
    const put = vi.spyOn(api, 'put')
    await changeMiniWindow({ on: true })
    expect(put).not.toHaveBeenCalled()
  })

  it('com janela, manda a mudança e guarda a resposta', async () => {
    resetMiniWindowForTests({ supported: true, on: false, onMinimize: true })
    const put = vi.spyOn(api, 'put').mockResolvedValue({ supported: true, on: true, onMinimize: true })
    await changeMiniWindow({ on: true })
    expect(put).toHaveBeenCalledWith('/api/app/mini-window', { on: true })
  })

  it('erro do servidor não quebra a tela', async () => {
    resetMiniWindowForTests({ supported: true, on: false, onMinimize: true })
    vi.spyOn(api, 'put').mockRejectedValue(new Error('x'))
    await expect(changeMiniWindow({ on: true })).resolves.toBeUndefined()
    miniWindowChanged(true)
  })
})
