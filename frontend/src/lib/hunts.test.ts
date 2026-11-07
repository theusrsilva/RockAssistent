import { describe, expect, it } from 'vitest'
import { createIncrementQueue, cumulative, formatElapsed, huntMethodName, shinyName, stepFor, subscribeHotkeyEvents, type HotkeyEventSource, type ShinyModifier } from './hunts'

/** Servidor falso: cada pedido fica parado até o teste resolver. */
function fakeServer() {
  const calls: { delta: number; resolve: (n: number) => void; reject: (e: unknown) => void }[] = []
  const send = (delta: number) => new Promise<number>((resolve, reject) => calls.push({ delta, resolve, reject }))
  return { calls, send }
}

const tick = () => new Promise((r) => setTimeout(r, 0))

describe('fila de incrementos', () => {
  it('um pedido por vez, somando o que chega enquanto há um em voo', async () => {
    const server = fakeServer()
    const results: number[] = []
    const q = createIncrementQueue(server.send, { onResult: (r) => results.push(r), onError: () => undefined })

    q.increment(1)
    q.increment(1)
    q.increment(1)
    expect(server.calls.map((c) => c.delta)).toEqual([1])
    expect(q.pending()).toBe(3)

    server.calls[0].resolve(1)
    await tick()
    expect(server.calls.map((c) => c.delta)).toEqual([1, 2])
    expect(q.pending()).toBe(2)

    server.calls[1].resolve(3)
    await tick()
    await q.idle()
    expect(results).toEqual([1, 3])
    expect(q.pending()).toBe(0)
  })

  it('+1 e −1 na fila se anulam e acima de 100 sai em partes', async () => {
    const server = fakeServer()
    const q = createIncrementQueue(server.send, { onResult: () => undefined, onError: () => undefined })
    q.increment(1)
    q.increment(5)
    q.increment(-5)
    server.calls[0].resolve(1)
    await tick()
    expect(server.calls).toHaveLength(1)

    q.increment(150)
    expect(server.calls[1].delta).toBe(100)
    server.calls[1].resolve(101)
    await tick()
    expect(server.calls[2].delta).toBe(50)
  })

  it('erro descarta a fila e avisa', async () => {
    const server = fakeServer()
    const errors: unknown[] = []
    const q = createIncrementQueue(server.send, { onResult: () => undefined, onError: (e) => errors.push(e) })
    q.increment(1)
    q.increment(1)
    server.calls[0].reject(new Error('hunt_finished'))
    await tick()
    expect(errors).toHaveLength(1)
    expect(q.pending()).toBe(0)
    expect(server.calls).toHaveLength(1)
    await q.idle()
  })
})

describe('contas', () => {
  it('chance acumulada igual à do backend', () => {
    expect(cumulative(512, 355)).toBeCloseTo(0.5, 2)
    expect(cumulative(512, 0)).toBe(0)
    expect(cumulative(1, 3)).toBe(1)
  })

  it('degrau do contador', () => {
    const chain: ShinyModifier = {
      id: 'chain', name: 'Corrente', nameEn: 'Chain', kind: 'steps', odds: null, rolls: null, counter: true, games: null,
      steps: [
        { label: '0–10', min: 0, max: 10, odds: 4096, rolls: 0 },
        { label: '40+', min: 40, max: null, odds: 200, rolls: null },
      ],
    }
    expect(stepFor(chain, 5)?.label).toBe('0–10')
    expect(stepFor(chain, 20)).toBeNull()
    expect(stepFor(chain, 999)?.odds).toBe(200)
  })

  it('cronômetro', () => {
    expect(formatElapsed(65)).toBe('1:05')
    expect(formatElapsed(3725)).toBe('1:02:05')
  })
})

describe('atalho global (SSE)', () => {
  function fakeSource() {
    const source: HotkeyEventSource & { closed: boolean } = {
      onmessage: null,
      closed: false,
      close: () => {
        source.closed = true
      },
    }
    return source
  }
  const send = (s: HotkeyEventSource, data: unknown) =>
    s.onmessage?.({ data: typeof data === 'string' ? data : JSON.stringify(data) } as MessageEvent)

  it('evento hotkey chama o increment da fila', () => {
    const source = fakeSource()
    const deltas: number[] = []
    let allowed = true
    const stop = subscribeHotkeyEvents(source, (d) => deltas.push(d), () => allowed)
    send(source, { type: 'hotkey', id: 'increment' })
    send(source, { type: 'hotkey', id: 'increment' })
    send(source, { type: 'hotkey', id: 'decrement' })
    expect(deltas).toEqual([1, 1, -1])

    // Com modal aberto (ou a hunt terminada) não conta; lixo é ignorado.
    allowed = false
    send(source, { type: 'hotkey', id: 'increment' })
    allowed = true
    send(source, 'não é json')
    send(source, { type: 'outro', id: 'increment' })
    send(source, { type: 'hotkey', id: 'reset' })
    expect(deltas).toEqual([1, 1, -1])

    stop()
    expect(source.closed).toBe(true)
  })
})

describe('shinyName', () => {
  it('escolhe o nome em inglês só quando o idioma é inglês', () => {
    const charm = { name: 'Amuleto Brilhante', nameEn: 'Shiny Charm' }
    expect(shinyName(charm, 'en')).toBe('Shiny Charm')
    expect(shinyName(charm, 'en-US')).toBe('Shiny Charm')
    expect(shinyName(charm, 'pt-BR')).toBe('Amuleto Brilhante')
    expect(huntMethodName({ methodName: 'Método Masuda', methodNameEn: 'Masuda Method' }, 'en')).toBe('Masuda Method')
    expect(huntMethodName({ methodName: 'Método Masuda', methodNameEn: 'Masuda Method' }, 'pt-BR')).toBe('Método Masuda')
  })
})
