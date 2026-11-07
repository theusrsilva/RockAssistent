import { describe, expect, it } from 'vitest'
import { findGuide, GUIDE_SECTIONS, guideIndex, loadGuide, parseDirectives, type GuideIndex } from './guides'

describe('parseDirectives', () => {
  it('separa texto e diretiva, com os argumentos', () => {
    const blocks = parseDirectives('# Chefes\n\nTexto antes.\n:::bosses frlg\nTexto depois.\n:::encounters frlg route-1\n')
    expect(blocks).toEqual([
      { type: 'markdown', text: '# Chefes\n\nTexto antes.' },
      { type: 'directive', name: 'bosses', args: ['frlg'], raw: ':::bosses frlg' },
      { type: 'markdown', text: 'Texto depois.' },
      { type: 'directive', name: 'encounters', args: ['frlg', 'route-1'], raw: ':::encounters frlg route-1' },
    ])
  })

  it('aceita diretiva sem argumentos, com hífen no nome e espaços sobrando', () => {
    expect(parseDirectives(':::shiny-methods   ')).toEqual([{ type: 'directive', name: 'shiny-methods', args: [], raw: ':::shiny-methods' }])
  })

  it('diretiva desconhecida também vira bloco (quem desenha avisa)', () => {
    const [block] = parseDirectives(':::nao-existe a b c')
    expect(block).toMatchObject({ type: 'directive', name: 'nao-existe', args: ['a', 'b', 'c'] })
  })

  it('diretiva no meio de uma lista não estraga a lista ao redor', () => {
    const blocks = parseDirectives('- Faça A\n- Faça B\n  :::boss frlg brock\n- Faça C')
    expect(blocks).toEqual([
      { type: 'markdown', text: '- Faça A\n- Faça B' },
      { type: 'directive', name: 'boss', args: ['frlg', 'brock'], raw: ':::boss frlg brock' },
      { type: 'markdown', text: '- Faça C' },
    ])
  })

  it('não lê diretivas em bloco de código nem ":::" sozinho nem no meio da linha', () => {
    const text = 'Use assim:\n```\n:::bosses frlg\n```\n:::\nveja :::tms frlg'
    expect(parseDirectives(text)).toEqual([{ type: 'markdown', text }])
  })

  it('ignora comentários HTML', () => {
    expect(parseDirectives('<!-- :::bosses frlg -->\nOi')).toEqual([{ type: 'markdown', text: 'Oi' }])
  })
})

describe('índice', () => {
  it('os dois idiomas têm as quatro seções e os mesmos guias', () => {
    const pt = guideIndex('pt-BR')
    const en = guideIndex('en')
    for (const s of GUIDE_SECTIONS) {
      expect(pt[s].length).toBeGreaterThan(0)
      expect(en[s].map((g) => g.key)).toEqual(pt[s].map((g) => g.key))
    }
  })

  it('acha guia e página; desconhecido = null', () => {
    const index: GuideIndex = {
      games: [{ key: 'frlg', title: 'FRLG', sections: [{ id: 'overview', title: 'Visão geral', file: 'games/frlg/overview.md' }] }],
      mechanics: [],
      vgc: [],
      mythology: [],
    }
    expect(findGuide(index, 'games', 'frlg')?.title).toBe('FRLG')
    expect(findGuide(index, 'games', 'xy')).toBeNull()
    expect(findGuide(index, 'nada', 'frlg')).toBeNull()
  })

  it('carrega o arquivo do guia sob demanda; arquivo que não existe = null', async () => {
    const pt = guideIndex('pt-BR')
    // O guia de chefes sempre tem a diretiva :::bosses (o de visão geral pode não ter nenhuma).
    const file = pt.games.flatMap((g) => g.sections).find((s) => s.id === 'bosses')!.file
    expect(await loadGuide('pt-BR', file)).toContain(':::')
    expect(await loadGuide('pt-BR', 'games/nada/nada.md')).toBeNull()
  })
})
