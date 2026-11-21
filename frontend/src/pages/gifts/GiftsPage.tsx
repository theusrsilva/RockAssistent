import { useTranslation } from 'react-i18next'
import { Callout, PageTitle } from '../../components/ui'
import { usePcAccess } from '../../lib/auth'
import { ActivityPanel } from '../activities/ActivityPanel'

/** Mystery Gift do FireRed/LeafGreen e do Sword/Shield (D26): janela do PC e, pelo celular, administradores. */
export default function GiftsPage() {
  const { t } = useTranslation()
  const pc = usePcAccess()
  if (!pc)
    return (
      <div className="flex flex-col gap-5">
        <PageTitle>{t('gifts.title')}</PageTitle>
        <Callout tone="sky" icon="computer">
          {t('gifts.adminOnly')}
        </Callout>
      </div>
    )
  return <ActivityPanel only={['gift']} title={t('gifts.title')} sub={t('gifts.sub')} />
}
