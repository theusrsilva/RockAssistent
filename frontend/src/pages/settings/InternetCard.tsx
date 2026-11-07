import { useEffect, useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Callout, Toggle, useToast } from '../../components/ui'
import { useIsRemote } from '../../lib/auth'
import { getInternetSettings, INTERNET_OFF, saveInternetSettings, type InternetSettings } from '../../lib/vgc'

/**
 * "Internet opcional" (D32): duas chaves, desligadas por padrão. Cada uma só baixa arquivos públicos (GET); nada do PC
 * é enviado. Pela rede o cartão só mostra o estado (ligar e desligar fica na janela do PC).
 */
export function InternetCard() {
  const { t } = useTranslation()
  const toast = useToast()
  const remote = useIsRemote()
  const [settings, setSettings] = useState<InternetSettings | null>(null)
  const [busy, setBusy] = useState(false)
  const [error, setError] = useState(false)

  useEffect(() => {
    getInternetSettings()
      .then(setSettings)
      .catch(() => setSettings(INTERNET_OFF))
  }, [])

  async function save(next: InternetSettings) {
    setBusy(true)
    setError(false)
    try {
      setSettings(await saveInternetSettings(next))
      toast(t('common.saved'))
    } catch {
      setError(true)
    } finally {
      setBusy(false)
    }
  }

  const rows: { key: keyof InternetSettings }[] = [{ key: 'smogon' }, { key: 'pokepaste' }]
  return (
    <div id="internet" className="flex max-w-[760px] scroll-mt-20 flex-col gap-3 rounded-xl border border-line bg-surface px-[22px] py-[18px]">
      <div>
        <div className="font-bold">{t('settings.internet.title')}</div>
        <div className="text-[13px] text-muted">{t('settings.internet.sub')}</div>
      </div>
      {settings === null ? (
        <p className="text-sm text-muted">{t('common.loading')}</p>
      ) : (
        <ul className="flex flex-col gap-1.5">
          {rows.map(({ key }) => (
            <li key={key} className="flex flex-col gap-1 rounded-lg bg-surface2 px-3 py-2.5">
              <Toggle
                on={settings[key]}
                disabled={busy || remote}
                onChange={(on) => void save({ ...settings, [key]: on })}
                label={t(`settings.internet.${key}.label`)}
              />
              <p className="text-[12.5px] text-muted">{t(`settings.internet.${key}.help`)}</p>
            </li>
          ))}
        </ul>
      )}
      <p className="text-[12.5px] text-muted">{remote ? t('settings.internet.remote') : t('settings.internet.privacy')}</p>
      {error && (
        <Callout tone="red" icon="error" className="text-[13px]">
          {t('common.error')}
        </Callout>
      )}
    </div>
  )
}
