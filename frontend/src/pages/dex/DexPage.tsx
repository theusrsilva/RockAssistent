import { useTranslation } from 'react-i18next'
import { Callout, PageTitle } from '../../components/ui'

/** Esqueleto: a área ganha conteúdo nas próximas tarefas do RockAssistent. */
export default function DexPage() {
  const { t } = useTranslation()
  return (
    <div className="flex flex-col gap-5">
      <PageTitle>{t('dex.title')}</PageTitle>
      <Callout tone="sky" icon="construction">
        {t('ui.building')}
      </Callout>
    </div>
  )
}
