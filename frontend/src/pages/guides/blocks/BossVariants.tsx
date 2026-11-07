import { useTranslation } from 'react-i18next'
import { bossTeam, chosenVariant, type BossBattle, type StarterInfo } from '../../nuzlocke/nuzlocke'
import { StarterPicker, starterName } from '../../nuzlocke/StarterPicker'

/** Time mostrado para o chefe com o inicial escolhido no guia (sem escolha ou sem a variante: a primeira). */
export function bossVariant(boss: BossBattle, starter: string | null) {
  const variant = chosenVariant(boss, starter)
  return { starter: variant?.starter ?? null, team: bossTeam(boss, starter) }
}

/**
 * Abas "Se você escolheu X" com o sprite do inicial do jogador. Nada quando a batalha tem um time só. A escolha e a
 * lista de iniciais vêm de cima (uma para a tabela ou o guia inteiro: veja guideStarter.ts).
 */
export function BossVariantTabs({
  boss,
  starters,
  names,
  value,
  onChange,
}: {
  boss: BossBattle
  starters: StarterInfo[]
  names: Map<number, string>
  value: string | null
  onChange: (starter: string) => void
}) {
  const { t } = useTranslation()
  const options = boss.variants?.length ? starters.filter((s) => boss.variants?.some((v) => v.starter === s.id)) : []
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
