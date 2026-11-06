// Tipos da API /api/boxes (Tarefa 16) e a lógica pura da tela Boxes (arrastar, abas, erros), testada em boxes.test.ts.
import type { TFunction } from 'i18next'
import { ApiError } from '../../lib/api'
import type { GameFamily, LegalityStatus } from '../../lib/types'

export interface SaveInfo {
  path: string
  gameKey: string
  version: string
  trainerName: string
  trainerId: number
  generation: number
  boxCount: number
  slotsPerBox: number
  partyCount: number
  modifiedAt: string
  sha256: string
  writable: boolean
}

export interface SaveEntry {
  id: string
  path: string
  info: SaveInfo | null
  error: string | null
}

export interface SlotSummary {
  species: number
  form: number
  speciesName: string
  nickname: string
  level: number
  isShiny: boolean
  isEgg: boolean
  gender: number
  otName: string
  trainerId: number
  heldItem: number
  originGame: string
  language: number
  fateful: boolean
  pid: number
}

export interface SaveSlot {
  /** -1 = time. */
  box: number
  slot: number
  data: string
  summary: SlotSummary
  context: string
  /** Null = jogo fora do catálogo de troca (não vai para o cofre nem para a troca). */
  family: GameFamily | null
}

export interface SaveBoxes {
  info: SaveInfo
  slots: SaveSlot[]
  boxNames: string[]
}

export interface SaveFolder {
  path: string
  pattern: string
  exists: boolean
}

export type MoveMode = 'move' | 'copy' | 'attach'

export type Location =
  | { kind: 'save'; saveId: string; box: number; slot: number }
  | { kind: 'vault'; pokemonId: string }
  | { kind: 'vault'; boxId: string | null; slot: number | null }

export interface MoveBody {
  from: Location
  to: Location
  mode: MoveMode
}

/** Resposta do POST /api/boxes/move: save ↔ cofre traz backupPath; save → save traz backupPaths. */
export interface MoveResult {
  mode?: MoveMode
  backupPath?: string | null
  backupPaths?: string[]
  notes?: string[]
  converted?: boolean
  legality?: LegalityStatus | null
  attachmentId?: string | null
}

export interface ApplyResult {
  backupPath: string
  newSha256: string
  save: SaveBoxes
}

/** O que está sendo arrastado (ou escolhido no "Mover"). */
export type DragSource =
  | { kind: 'save'; saveId: string; box: number; slot: number; data: string; family: GameFamily | null }
  | { kind: 'vault'; pokemonId: string }

/** Onde foi solto. Num save, data = o Pokémon que já está lá (null = vazio). */
export type DropTarget =
  | { kind: 'save'; saveId: string; box: number; slot: number; data: string | null }
  | { kind: 'vault'; boxId: string | null; slot: number | null; occupied: boolean }

export type DropPlan =
  | { kind: 'none' }
  | { kind: 'refuse'; code: string }
  | { kind: 'move'; body: MoveBody }
  | { kind: 'apply'; saveId: string; changes: { box: number; slot: number; data: string | null }[] }
  | { kind: 'vaultMove'; pokemonId: string; boxId: string | null; slot: number | null }

const fromLocation = (s: DragSource): Location =>
  s.kind === 'save' ? { kind: 'save', saveId: s.saveId, box: s.box, slot: s.slot } : { kind: 'vault', pokemonId: s.pokemonId }

/**
 * Decide o que fazer ao soltar. O /api/boxes/move nunca sobrescreve (slot_occupied); por isso:
 * - posição ocupada no MESMO save: troca de lugar com um apply só (duas mudanças, uma gravação);
 * - posição ocupada em outro save ou no cofre (vindo de um save): recusa com a mensagem de posição ocupada;
 * - cofre → cofre: o move do próprio cofre, que já troca de lugar.
 */
export function planDrop(source: DragSource, target: DropTarget, mode: MoveMode): DropPlan {
  if (source.kind === 'vault' && target.kind === 'vault') {
    return { kind: 'vaultMove', pokemonId: source.pokemonId, boxId: target.boxId, slot: target.slot }
  }
  if (source.kind === 'save' && target.kind === 'save') {
    if (source.saveId === target.saveId && source.box === target.box && source.slot === target.slot) return { kind: 'none' }
    if (mode === 'attach') return { kind: 'refuse', code: 'attach_invalid' }
    if (target.data !== null) {
      if (source.saveId !== target.saveId) return { kind: 'refuse', code: 'slot_occupied' }
      return {
        kind: 'apply',
        saveId: source.saveId,
        changes: [
          { box: target.box, slot: target.slot, data: source.data },
          { box: source.box, slot: source.slot, data: target.data },
        ],
      }
    }
  }
  if (source.kind === 'save' && target.kind === 'vault') {
    if (source.family === null) return { kind: 'refuse', code: 'save_game_unsupported' }
    if (target.occupied) return { kind: 'refuse', code: 'slot_occupied' }
  }
  if (source.kind === 'vault' && target.kind === 'save' && target.data !== null) return { kind: 'refuse', code: 'slot_occupied' }

  const to: Location =
    target.kind === 'save'
      ? { kind: 'save', saveId: target.saveId, box: target.box, slot: target.slot }
      : { kind: 'vault', boxId: target.boxId, slot: target.boxId === null ? null : target.slot }
  return { kind: 'move', body: { from: fromLocation(source), to, mode } }
}

/** Backups feitos por uma gravação, nas duas formas de resposta. */
export function backupsOf(r: MoveResult): string[] {
  if (r.backupPaths) return r.backupPaths
  return r.backupPath ? [r.backupPath] : []
}

// ---- Abas ----

export type BoxesTab = 'saves' | 'vault'
export interface TabState {
  tab: BoxesTab
  /** Saves só na janela do PC (rotas local-only dão 403 pela rede). */
  savesBlocked: boolean
}

export function initialTab({ remote, wantsVault }: { remote: boolean; wantsVault: boolean }): TabState {
  return { tab: remote || wantsVault ? 'vault' : 'saves', savesBlocked: remote }
}

export function tabReducer(state: TabState, action: { type: 'select'; tab: BoxesTab } | { type: 'forbidden' }): TabState {
  if (action.type === 'forbidden') return { tab: 'vault', savesBlocked: true }
  return { ...state, tab: action.tab }
}

// ---- Erros ----

/** Códigos da API de saves que têm texto próprio em boxes.errors.*. */
export const KNOWN_ERRORS = [
  'save_unwritable',
  'save_read_only',
  'save_wrong_game',
  'save_game_unsupported',
  'save_changed',
  'save_not_found',
  'slot_occupied',
  'slot_locked',
  'slot_invalid',
  'slot_empty',
  'slot_repeated',
  'attach_wrong_game',
  'attach_invalid',
  'sav_unknown',
  'sav_unsupported',
  'sav_wrong_game',
  'sav_too_big',
  'sav_unreadable',
  'folder_invalid',
  'verify_failed',
  'pokemon_not_found',
  'invalid_pokemon',
  'move_invalid',
  'box_not_found',
  'box_full',
  'local_only',
  'no_window',
  'evolve_not_available',
  'evolve_wrong_target',
] as const

export function codeKey(code: string | null | undefined): string {
  return code && (KNOWN_ERRORS as readonly string[]).includes(code) ? `boxes.errors.${code}` : 'common.error'
}

export function errorKey(e: unknown): string {
  return e instanceof ApiError ? codeKey(e.code) : 'common.error'
}

/** Texto do erro, com o motivo do HOME quando houver (save_wrong_game: home_no_destination, egg...). */
export function errorMessage(t: TFunction, e: unknown): string {
  const base = t(errorKey(e))
  if (e instanceof ApiError && e.reason) {
    const why = t(`vault.transfer.refusal.${e.reason}`, { defaultValue: '' })
    return why ? `${base} (${why})` : base
  }
  return base
}

/** Nome do arquivo de um caminho (Linux ou Windows). */
export const fileName = (path: string) => path.split(/[\\/]/).pop() || path

// ---------- Evoluir por troca e clonar (Tarefa 18) ----------

/** Uma opção de GET …/evolve-options (cofre ou save). Nomes em inglês, como os itens no resto do app. */
export interface EvolveOption {
  species: number
  form: number
  speciesName: string
  /** Item que precisa segurar (número do formato do Pokémon) ou null. */
  requiredItem: number | null
  requiredItemName: string | null
  holdsItem: boolean
  /** Karrablast ↔ Shelmet: a espécie com que precisa ser trocado. */
  tradeWith: number | null
  tradeWithName: string | null
}

/** Veredito do PKHeX depois de evoluir: só avisa, a evolução já foi gravada. */
export interface EvolveLegality {
  status: LegalityStatus
  valid: boolean
  messages: string[]
}

/** Corpo do POST …/evolve. No save leva o SHA do save aberto (409 save_changed se o emulador gravou no meio). */
export function evolveBody(option: Pick<EvolveOption, 'species' | 'form'>, language: string, sha256?: string) {
  return { species: option.species, form: option.form, language: language === 'en' ? 'en' : 'pt-BR', ...(sha256 ? { sha256 } : {}) }
}

/** Primeira posição livre do save a partir da caixa dada (o time começa na primeira caixa), dando a volta. */
export function firstFreeSlot(save: SaveBoxes, fromBox: number): { box: number; slot: number } | null {
  const { boxCount, slotsPerBox } = save.info
  const used = new Set(save.slots.filter((s) => s.box >= 0).map((s) => `${s.box}:${s.slot}`))
  const start = fromBox >= 0 && fromBox < boxCount ? fromBox : 0
  for (let i = 0; i < boxCount; i++) {
    const box = (start + i) % boxCount
    for (let slot = 0; slot < slotsPerBox; slot++) if (!used.has(`${box}:${slot}`)) return { box, slot }
  }
  return null
}

/**
 * Clonar no save = copiar para a primeira posição livre do mesmo save (pelo /api/boxes/move, modo copy).
 * Null quando o save está cheio. No cofre não existe clone: ele guarda um arquivo de cada (SHA único por conta).
 */
export function cloneBody(saveId: string, entry: Pick<SaveSlot, 'box' | 'slot'>, save: SaveBoxes): MoveBody | null {
  const free = firstFreeSlot(save, entry.box)
  if (!free) return null
  return {
    from: { kind: 'save', saveId, box: entry.box, slot: entry.slot },
    to: { kind: 'save', saveId, box: free.box, slot: free.slot },
    mode: 'copy',
  }
}
