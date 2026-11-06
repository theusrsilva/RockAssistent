import { useEffect, useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Button, Callout, useToast } from '../../components/ui'
import { ApiError, api } from '../../lib/api'
import { DEFAULT_HOTKEYS, comboFromEvent, formatCombo, type HotkeySettings } from '../../lib/hotkeys'

type Which = keyof HotkeySettings

/**
 * Atalhos do Contador Hunt (Configurações → Preferências). "Gravar" pega a próxima tecla apertada; Esc cancela.
 * Vale na janela local e, no modo servidor, por conta (GET/PUT /api/settings/hotkeys).
 */
export function HotkeysCard() {
  const { t } = useTranslation()
  const toast = useToast()
  const [keys, setKeys] = useState<HotkeySettings | null>(null)
  const [recording, setRecording] = useState<Which | null>(null)
  const [busy, setBusy] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const names = t('hunt.hotkeys.keyNames', { returnObjects: true }) as Record<string, string>

  useEffect(() => {
    api.get<HotkeySettings>('/api/settings/hotkeys').then(setKeys).catch(() => setKeys(DEFAULT_HOTKEYS))
  }, [])

  async function save(next: HotkeySettings) {
    setError(null)
    if (next.increment.toLowerCase() === next.decrement.toLowerCase()) {
      setError(t('hunt.hotkeys.errors.hotkeys_same'))
      return
    }
    setBusy(true)
    try {
      setKeys(await api.put<HotkeySettings>('/api/settings/hotkeys', next))
      toast(t('common.saved'))
    } catch (e) {
      const code = e instanceof ApiError ? e.code : null
      setError(code === 'hotkeys_same' || code === 'hotkey_invalid' ? t(`hunt.hotkeys.errors.${code}`) : t('common.error'))
    } finally {
      setBusy(false)
    }
  }

  // Gravando: a próxima tecla (que não seja só um modificador) vira o atalho. Captura antes de qualquer outro.
  useEffect(() => {
    if (!recording || !keys) return
    const onKey = (e: KeyboardEvent) => {
      e.preventDefault()
      e.stopPropagation()
      if (e.code === 'Escape' && !e.ctrlKey && !e.altKey && !e.shiftKey && !e.metaKey) {
        setRecording(null)
        return
      }
      const combo = comboFromEvent(e)
      if (!combo) return
      setRecording(null)
      void save({ ...keys, [recording]: combo })
    }
    window.addEventListener('keydown', onKey, true)
    return () => window.removeEventListener('keydown', onKey, true)
  }, [recording, keys])

  const rows: Which[] = ['increment', 'decrement']
  return (
    <div className="flex max-w-[760px] flex-col gap-3 rounded-xl border border-line bg-surface px-[22px] py-[18px]">
      <div className="flex flex-wrap items-center gap-2">
        <div className="min-w-[200px] flex-1">
          <div className="font-bold">{t('hunt.hotkeys.title')}</div>
          <div className="text-[13px] text-muted">{t('hunt.hotkeys.sub')}</div>
        </div>
        <Button
          variant="secondary"
          size="sm"
          icon="restart_alt"
          disabled={busy || !keys || (keys.increment === DEFAULT_HOTKEYS.increment && keys.decrement === DEFAULT_HOTKEYS.decrement)}
          onClick={() => void save(DEFAULT_HOTKEYS)}
        >
          {t('hunt.hotkeys.reset')}
        </Button>
      </div>

      {keys === null ? (
        <p className="text-sm text-muted">{t('common.loading')}</p>
      ) : (
        <ul className="flex flex-col gap-1.5">
          {rows.map((which) => (
            <li key={which} className="flex flex-wrap items-center gap-2 rounded-lg bg-surface2 px-3 py-2">
              <span className="min-w-[110px] flex-1 font-semibold">{t(`hunt.hotkeys.${which}`)}</span>
              {recording === which ? (
                <span className="text-[13px] font-semibold text-accent" aria-live="polite">
                  {t('hunt.hotkeys.recording')}
                </span>
              ) : (
                <kbd className="rounded-md border border-line bg-surface px-2 py-0.5 font-mono text-[13px]">{formatCombo(keys[which], names)}</kbd>
              )}
              <Button
                size="sm"
                variant={recording === which ? 'primary' : 'secondary'}
                icon="keyboard"
                disabled={busy || (recording !== null && recording !== which)}
                onClick={(e) => {
                  // Sem foco no botão: o Espaço gravado não "clica" nele de novo.
                  e.currentTarget.blur()
                  setError(null)
                  setRecording(recording === which ? null : which)
                }}
              >
                {t('hunt.hotkeys.record')}
              </Button>
            </li>
          ))}
        </ul>
      )}

      {error && (
        <Callout tone="red" icon="error" className="text-[13px]">
          {error}
        </Callout>
      )}
    </div>
  )
}
