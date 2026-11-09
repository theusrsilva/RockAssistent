import { useCallback, useReducer } from 'react'
import { useTranslation } from 'react-i18next'
import { useSearchParams } from 'react-router'
import { Callout, PageTitle, Segmented } from '../../components/ui'
import { useAuth } from '../../lib/auth'
import { initialTab, tabReducer, type BoxesTab } from './boxes'
import { SavesTab } from './SavesTab'
import { VaultTab } from './VaultTab'

/**
 * Boxes: os saves das pastas do PC ("Saves") e o cofre do app ("Cofre do app"). Pela rede (modo servidor) os
 * saves não existem (rotas só da janela local): a aba abre só para explicar. A tela sempre começa no cofre.
 */
export default function BoxesPage() {
  const { t } = useTranslation()
  const { me } = useAuth()
  const [params] = useSearchParams()
  const remote = me?.local === false
  // Abre no cofre (também "Ver no cofre", /boxes?pokemon=id, e ?aba=cofre); ?aba=saves abre nos saves.
  const [state, dispatch] = useReducer(tabReducer, { remote, wantsSaves: !params.has('pokemon') && params.get('aba') === 'saves' }, initialTab)
  // Identidade estável: o SavesTab usa no efeito de montagem (senão refaria os GETs a cada render).
  const onForbidden = useCallback(() => dispatch({ type: 'forbidden' }), [])

  return (
    <div className="flex flex-col gap-5">
      <PageTitle>{t('boxes.title')}</PageTitle>
      <Segmented<BoxesTab>
        className="self-start"
        value={state.tab}
        onChange={(tab) => dispatch({ type: 'select', tab })}
        options={[
          { value: 'vault', label: t('boxes.tabs.vault') },
          { value: 'saves', label: t('boxes.tabs.saves') },
        ]}
      />
      {state.tab === 'saves' ? (
        state.savesBlocked ? (
          <Callout tone="sky" icon="desktop_windows">
            {t('boxes.localOnly')}
          </Callout>
        ) : (
          <SavesTab onForbidden={onForbidden} />
        )
      ) : (
        <VaultTab savesAvailable={!state.savesBlocked} />
      )}
    </div>
  )
}
