import { useTranslation } from 'react-i18next'
import { Button } from '../../components/ui'

/**
 * "Clonar" e "Evoluir por troca" nos painéis do Boxes (cofre e save). Sem a função, o botão fica desligado
 * com a dica (cloneHint / evolveHint; o padrão é "em breve").
 */
export function ComingSoonActions({
  onClone,
  onEvolve,
  cloneHint,
  evolveHint,
}: {
  onClone?: () => void
  onEvolve?: () => void
  cloneHint?: string
  evolveHint?: string
}) {
  const { t } = useTranslation()
  return (
    <div className="flex gap-2">
      {/* O span mostra a dica mesmo com o botão desligado. */}
      <span className="flex flex-1" title={onClone ? undefined : (cloneHint ?? t('boxes.soon'))}>
        <Button variant="secondary" size="sm" className="flex-1" icon="content_copy" disabled={!onClone} onClick={onClone}>
          {t('boxes.clone')}
        </Button>
      </span>
      {/* O span mostra a dica mesmo com o botão desligado. */}
      <span className="flex flex-1" title={onEvolve ? undefined : (evolveHint ?? t('boxes.soon'))}>
        <Button variant="secondary" size="sm" className="flex-1" icon="upgrade" disabled={!onEvolve} onClick={onEvolve}>
          {t('boxes.evolve')}
        </Button>
      </span>
    </div>
  )
}
