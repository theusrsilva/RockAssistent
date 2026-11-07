import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import { bossTeam, chosenVariant, type BossBattle } from '../../nuzlocke/nuzlocke'
import { StarterPicker, starterName, useStarters } from '../../nuzlocke/StarterPicker'

/** Inicial escolhido nas abas de um chefe (padrão = a primeira variante) e o time que vai com ele. */
export function useBossVariant(boss: BossBattle) {
  const [starter, setStarter] = useState<string | null>(null)
  const variant = chosenVariant(boss, starter)
  return { starter: variant?.starter ?? null, setStarter, team: bossTeam(boss, starter) }
}

type TabsProps = {
  game: string
  boss: BossBattle
  names: Map<number, string>
  value: string | null
  onChange: (starter: string) => void
}

/** Abas "Se você escolheu X" com o sprite do inicial do jogador. Nada quando a batalha tem um time só. */
export function BossVariantTabs(props: TabsProps) {
  // Só pede a lista de iniciais quando a batalha tem variantes (a tabela tem uma linha por chefe).
  return props.boss.variants?.length ? <Tabs {...props} /> : null
}

function Tabs({
  game,
  boss,
  names,
  value,
  onChange,
}: TabsProps) {
  const { t } = useTranslation()
  const starters = useStarters(game)
  const options = starters.filter((s) => boss.variants?.some((v) => v.starter === s.id))
  if (options.length === 0) return null
  return (
    <StarterPicker
      starters={options}
      value={value}
      onChange={(s) => s && onChange(s)}
      label={(s) => t('guides.block.ifYouChose', { name: starterName(s, names) })}
    />
  )
}
