import { useTranslation } from 'react-i18next'
import { ApiError } from '../../../lib/api'
import { useApiGet } from '../../../lib/dex'
import { Callout } from '../../../components/ui'

/**
 * Onde a espécie aparece nos saves das pastas (GET /api/boxes/where/{species}, Task 16). Enquanto a rota não
 * existe (404), mostra "em breve".
 */
export function SavesTab({ species }: { species: number }) {
  const { t } = useTranslation()
  const result = useApiGet<unknown>(`/api/boxes/where/${species}`)

  if (result.loading) return <p className="text-sm text-muted">{t('common.loading')}</p>
  if (result.error instanceof ApiError && result.error.status === 404)
    return (
      <Callout tone="sky" icon="construction">
        {t('dex.saves.soon')}
      </Callout>
    )
  if (result.error)
    return (
      <Callout tone="red" icon="error">
        {t('dex.saves.error')}
      </Callout>
    )
  const count = Array.isArray(result.data) ? result.data.length : 0
  return <p className="text-sm">{count > 0 ? t('dex.saves.found', { count }) : t('dex.saves.none')}</p>
}
