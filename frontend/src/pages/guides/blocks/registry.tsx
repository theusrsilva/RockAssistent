import type { ComponentType } from 'react'
import BossCard from './BossCard'
import BossTable from './BossTable'
import EncounterTable from './EncounterTable'
import ShinyMethodsTable from './ShinyMethodsTable'
import TmTable from './TmTable'
import VgcRegulationBlock from './VgcRegulationBlock'
import VgcTeamsBlock from './VgcTeamsBlock'
import type { BlockProps } from './common'

/**
 * Blocos das diretivas dos guias (":::nome arg1 arg2"). Bloco novo = uma linha aqui e uma em
 * scripts/check-guides.py (directive_specs); o parser (lib/guides.ts) não muda.
 */
export const GUIDE_BLOCKS: Record<string, ComponentType<BlockProps>> = {
  bosses: BossTable,
  boss: BossCard,
  encounters: EncounterTable,
  tms: TmTable,
  'shiny-methods': ShinyMethodsTable,
  'vgc-regulation': VgcRegulationBlock,
  'vgc-teams': VgcTeamsBlock,
}
