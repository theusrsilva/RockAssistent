import { useTranslation } from 'react-i18next'
import { ActivityPanel } from '../activities/ActivityPanel'

/** Mystery Gift do FireRed/LeafGreen e do Sword/Shield (D26), só na janela do PC. */
export default function GiftsPage() {
  const { t } = useTranslation()
  return <ActivityPanel only={['gift']} title={t('gifts.title')} sub={t('gifts.sub')} />
}
