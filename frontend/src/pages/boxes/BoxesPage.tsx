import { useTranslation } from 'react-i18next'
import { Callout, PageTitle } from '../../components/ui'
import VaultPage from '../vault/VaultPage'

/** Esqueleto do Boxes: os saves das pastas chegam depois; até lá, o cofre do app continua aqui. */
export default function BoxesPage() {
  const { t } = useTranslation()
  return (
    <div className="flex flex-col gap-5">
      <PageTitle>{t('boxes.title')}</PageTitle>
      <Callout tone="sky" icon="construction">
        {t('boxes.building')}
      </Callout>
      <VaultPage />
    </div>
  )
}
