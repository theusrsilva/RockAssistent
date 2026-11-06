import { describe, expect, it } from 'vitest'
import { ApiError } from '../../lib/api'
import { backupsOf, errorKey, initialTab, planDrop, tabReducer, type DragSource } from './boxes'

const fromSave: DragSource = { kind: 'save', saveId: 'a1b2c3d4', box: 0, slot: 3, data: 'AAA', family: 'frlg' }
const fromVault: DragSource = { kind: 'vault', pokemonId: 'p1' }

describe('arrastar no Boxes', () => {
  it('save → cofre monta o corpo do /api/boxes/move', () => {
    expect(planDrop(fromSave, { kind: 'vault', boxId: 'b1', slot: 5, occupied: false }, 'move')).toEqual({
      kind: 'move',
      body: {
        from: { kind: 'save', saveId: 'a1b2c3d4', box: 0, slot: 3 },
        to: { kind: 'vault', boxId: 'b1', slot: 5 },
        mode: 'move',
      },
    })
  })

  it('save → Caixa de entrada vai sem posição', () => {
    const plan = planDrop(fromSave, { kind: 'vault', boxId: null, slot: null, occupied: false }, 'copy')
    expect(plan).toMatchObject({ kind: 'move', body: { to: { kind: 'vault', boxId: null, slot: null }, mode: 'copy' } })
  })

  it('save de jogo fora do catálogo não vai para o cofre', () => {
    expect(planDrop({ ...fromSave, family: null }, { kind: 'vault', boxId: 'b1', slot: 0, occupied: false }, 'move')).toEqual({
      kind: 'refuse',
      code: 'save_game_unsupported',
    })
  })

  it('cofre → save vazio usa o move com o pokemonId', () => {
    expect(planDrop(fromVault, { kind: 'save', saveId: 'x', box: 1, slot: 2, data: null }, 'copy')).toEqual({
      kind: 'move',
      body: { from: { kind: 'vault', pokemonId: 'p1' }, to: { kind: 'save', saveId: 'x', box: 1, slot: 2 }, mode: 'copy' },
    })
  })

  it('posição ocupada no mesmo save troca de lugar pelo apply com duas mudanças', () => {
    expect(planDrop(fromSave, { kind: 'save', saveId: 'a1b2c3d4', box: 1, slot: 0, data: 'BBB' }, 'move')).toEqual({
      kind: 'apply',
      saveId: 'a1b2c3d4',
      changes: [
        { box: 1, slot: 0, data: 'AAA' },
        { box: 0, slot: 3, data: 'BBB' },
      ],
    })
  })

  it('posição ocupada em outro save ou vinda do cofre é recusada', () => {
    expect(planDrop(fromSave, { kind: 'save', saveId: 'outro', box: 0, slot: 0, data: 'BBB' }, 'move')).toEqual({ kind: 'refuse', code: 'slot_occupied' })
    expect(planDrop(fromVault, { kind: 'save', saveId: 'x', box: 0, slot: 0, data: 'BBB' }, 'move')).toEqual({ kind: 'refuse', code: 'slot_occupied' })
    expect(planDrop(fromSave, { kind: 'vault', boxId: 'b1', slot: 0, occupied: true }, 'move')).toEqual({ kind: 'refuse', code: 'slot_occupied' })
  })

  it('soltar no mesmo lugar não faz nada', () => {
    expect(planDrop(fromSave, { kind: 'save', saveId: 'a1b2c3d4', box: 0, slot: 3, data: 'AAA' }, 'move')).toEqual({ kind: 'none' })
  })

  it('cofre → cofre usa o move do cofre (que já troca de lugar)', () => {
    expect(planDrop(fromVault, { kind: 'vault', boxId: 'b1', slot: 4, occupied: true }, 'move')).toEqual({ kind: 'vaultMove', pokemonId: 'p1', boxId: 'b1', slot: 4 })
  })

  it('anexar entre dois saves não existe', () => {
    expect(planDrop(fromSave, { kind: 'save', saveId: 'outro', box: 0, slot: 0, data: null }, 'attach')).toEqual({ kind: 'refuse', code: 'attach_invalid' })
  })
})

describe('respostas do move', () => {
  it('lê backupPath (save ↔ cofre) e backupPaths (save → save)', () => {
    expect(backupsOf({ backupPath: '/b/1.sav' })).toEqual(['/b/1.sav'])
    expect(backupsOf({ backupPath: null })).toEqual([])
    expect(backupsOf({ backupPaths: ['/b/1.sav', '/b/2.sav'] })).toEqual(['/b/1.sav', '/b/2.sav'])
  })
})

describe('abas do Boxes', () => {
  it('pela rede começa no cofre e deixa os saves bloqueados', () => {
    expect(initialTab({ remote: true, wantsVault: false })).toEqual({ tab: 'vault', savesBlocked: true })
    expect(initialTab({ remote: false, wantsVault: false })).toEqual({ tab: 'saves', savesBlocked: false })
    expect(initialTab({ remote: false, wantsVault: true })).toEqual({ tab: 'vault', savesBlocked: false })
  })

  it('403 dos saves desliga a aba e seleciona o cofre', () => {
    const state = tabReducer({ tab: 'saves', savesBlocked: false }, { type: 'forbidden' })
    expect(state).toEqual({ tab: 'vault', savesBlocked: true })
    // A aba ainda abre, mas só para mostrar o aviso.
    expect(tabReducer(state, { type: 'select', tab: 'saves' })).toEqual({ tab: 'saves', savesBlocked: true })
  })
})

describe('erros', () => {
  it('códigos conhecidos viram boxes.errors.*; o resto, erro genérico', () => {
    expect(errorKey(new ApiError(409, 'save_changed', []))).toBe('boxes.errors.save_changed')
    expect(errorKey(new ApiError(400, 'slot_occupied', []))).toBe('boxes.errors.slot_occupied')
    expect(errorKey(new ApiError(400, 'algo_novo', []))).toBe('common.error')
    expect(errorKey(new Error('x'))).toBe('common.error')
    expect(errorKey(new ApiError(403, 'local_only', []))).toBe('boxes.errors.local_only')
  })
})
