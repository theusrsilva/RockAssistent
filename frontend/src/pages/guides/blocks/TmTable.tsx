import { useTranslation } from 'react-i18next'
import { Link } from 'react-router'
import { Badge } from '../../../components/ui'
import { useApiGet } from '../../../lib/dex'
import { TypeBadge } from '../../dex/TypeBadge'
import { BlockState, BlockTable, cell, type BlockProps } from './common'

/** Linha de GET /api/dex/machines?game= (MachineInfo do DexService). */
export interface MachineInfo {
  number: number
  machine: string
  moveId: number
  moveIdentifier: string
  moveName: string
  type: string
  damageClass: string | null
  power: number | null
  accuracy: number | null
  pp: number | null
  /** Só existe na DLC (Z-A: Mega Dimension). */
  dlc: boolean
}

/** :::tms <jogo> — TMs/HMs/TRs do jogo com o golpe e o tipo. Onde achar cada um fica no texto do guia. */
export default function TmTable({ args: [game] }: BlockProps) {
  const { t } = useTranslation()
  const state = useApiGet<MachineInfo[]>(`/api/dex/machines?game=${encodeURIComponent(game)}`)
  return (
    <BlockState state={state} empty={(d) => d.length === 0}>
      {(machines) => (
        <BlockTable head={[t('guides.col.number'), t('guides.col.move'), t('guides.col.type'), t('guides.col.category'), t('guides.col.power'), t('guides.col.accuracy')]}>
          {machines.map((m) => (
            <tr key={m.number}>
              <td className={`${cell} font-bold whitespace-nowrap tabular-nums`}>
                {m.machine}
                {m.dlc && (
                  <span className="ml-1.5">
                    <Badge tone="violet">{t('guides.block.dlc')}</Badge>
                  </span>
                )}
              </td>
              <td className={cell}>
                <Link to={`/pokedex/golpe/${m.moveId}`} className="font-semibold whitespace-nowrap hover:text-accent">
                  {m.moveName}
                </Link>
              </td>
              <td className={cell}>
                <TypeBadge type={m.type} small />
              </td>
              <td className={`${cell} text-muted`}>{m.damageClass ? t(`dex.class.${m.damageClass}`, { defaultValue: m.damageClass }) : '—'}</td>
              <td className={`${cell} tabular-nums`}>{m.power ?? '—'}</td>
              <td className={`${cell} tabular-nums`}>{m.accuracy != null ? `${m.accuracy}%` : '—'}</td>
            </tr>
          ))}
        </BlockTable>
      )}
    </BlockState>
  )
}
