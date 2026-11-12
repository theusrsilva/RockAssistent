import { useTranslation } from 'react-i18next'
import { Icon } from '../../components/ui'
import { ownedKey, ownedSource, type CollectionSets } from '../../lib/dexCollection'

/**
 * Os dois botões "tenho" e "tenho shiny" de uma linha da Pokédex (a espécie ou uma forma). Aceso pela marcação da
 * pessoa; quando vem só do cofre, fica aceso com o contorno tracejado e o clique acrescenta a marcação.
 */
export function OwnedButtons({
  speciesId,
  formId,
  sets,
  countVault,
  onToggle,
  large = false,
}: {
  speciesId: number
  formId: number | null | undefined
  sets: CollectionSets
  countVault: boolean
  onToggle: (shiny: boolean, owned: boolean) => void
  large?: boolean
}) {
  const { t } = useTranslation()
  const size = large ? 'h-8 w-8 text-[20px]' : 'h-7 w-7 text-[18px]'
  return (
    <span className="inline-flex gap-1">
      {[false, true].map((shiny) => {
        const source = ownedSource(sets, countVault, ownedKey(speciesId, formId, shiny))
        const label = t(shiny ? 'dex.collection.haveShiny' : 'dex.collection.have')
        const title = source === 'vault' ? `${label} (${t('dex.collection.fromVault')})` : label
        return (
          <button
            key={String(shiny)}
            type="button"
            aria-pressed={source !== null}
            aria-label={title}
            title={title}
            onClick={(e) => {
              e.preventDefault()
              e.stopPropagation()
              onToggle(shiny, source !== 'mark')
            }}
            className={`inline-flex cursor-pointer items-center justify-center rounded-full border ${size} ${
              source === null
                ? 'border-line bg-surface text-muted hover:text-ink'
                : `${shiny ? 'border-warn bg-warn-soft text-warn' : 'border-ok bg-ok-soft text-ok'} ${
                    source === 'vault' ? 'border-dashed' : ''
                  }`
            }`}
          >
            <Icon name={shiny ? 'auto_awesome' : 'check'} filled={source !== null} />
          </button>
        )
      })}
    </span>
  )
}
