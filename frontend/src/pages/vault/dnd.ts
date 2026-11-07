// Arrastar e soltar no cofre e no Boxes (HTML5 drag and drop).
import type { DragSource } from '../boxes/boxes'

/** Pokémon do cofre: o dado é o id. */
export const DRAG_TYPE = 'application/x-traderock-pokemon'
/** Pokémon de um save (tela Boxes): o dado é o DragSource em JSON. */
export const SAVE_DRAG_TYPE = 'application/x-traderock-save-slot'

export function startDrag(e: React.DragEvent, pokemonId: string) {
  e.dataTransfer.setData(DRAG_TYPE, pokemonId)
  e.dataTransfer.effectAllowed = 'move'
}

export function startSaveDrag(e: React.DragEvent, source: Extract<DragSource, { kind: 'save' }>) {
  e.dataTransfer.setData(SAVE_DRAG_TYPE, JSON.stringify(source))
  e.dataTransfer.effectAllowed = 'move'
}

export function draggedPokemon(e: React.DragEvent): string | null {
  return e.dataTransfer.types.includes(DRAG_TYPE) ? e.dataTransfer.getData(DRAG_TYPE) || null : null
}

/** O que foi arrastado, venha do cofre ou de um save. */
export function draggedSource(e: React.DragEvent): DragSource | null {
  const id = draggedPokemon(e)
  if (id) return { kind: 'vault', pokemonId: id }
  if (!e.dataTransfer.types.includes(SAVE_DRAG_TYPE)) return null
  try {
    return JSON.parse(e.dataTransfer.getData(SAVE_DRAG_TYPE)) as DragSource
  } catch {
    return null
  }
}

export function acceptsPokemon(e: React.DragEvent): boolean {
  return e.dataTransfer.types.includes(DRAG_TYPE)
}

export function acceptsAny(e: React.DragEvent): boolean {
  return e.dataTransfer.types.includes(DRAG_TYPE) || e.dataTransfer.types.includes(SAVE_DRAG_TYPE)
}
