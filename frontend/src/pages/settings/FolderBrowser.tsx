import { useCallback, useEffect, useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Button, Icon, Modal } from '../../components/ui'
import { ApiError, api } from '../../lib/api'

export interface FolderEntry {
  name: string
  path: string
  hidden: boolean
}

export interface FolderList {
  path: string
  parent: string | null
  folders: FolderEntry[]
  roots: { label: string; path: string }[]
  truncated: boolean
}

/**
 * Navegador de pastas do PC (GET /api/fs/folders) para quem não tem o diálogo nativo: o celular (administradores) e o
 * app aberto no navegador. Mostra só pastas; "Usar esta pasta" devolve a pasta aberta.
 */
export function FolderBrowser({ onPick, onClose }: { onPick: (path: string) => void; onClose: () => void }) {
  const { t } = useTranslation()
  const [list, setList] = useState<FolderList | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [showHidden, setShowHidden] = useState(false)
  const [loading, setLoading] = useState(false)

  const open = useCallback(async (path?: string) => {
    setLoading(true)
    setError(null)
    try {
      setList(await api.get<FolderList>(`/api/fs/folders${path ? `?path=${encodeURIComponent(path)}` : ''}`))
    } catch (e) {
      setError(e instanceof ApiError && e.code ? e.code : 'unknown')
    } finally {
      setLoading(false)
    }
  }, [])

  useEffect(() => {
    void open()
  }, [open])

  const folders = list?.folders.filter((f) => showHidden || !f.hidden) ?? []
  const hiddenCount = list ? list.folders.length - list.folders.filter((f) => !f.hidden).length : 0

  return (
    <Modal
      wide
      title={t('folderBrowser.title')}
      actions={
        <>
          <Button variant="ghost" onClick={onClose}>
            {t('common.cancel')}
          </Button>
          <Button icon="check" disabled={!list || loading} onClick={() => list && onPick(list.path)}>
            {t('folderBrowser.use')}
          </Button>
        </>
      }
    >
      <div className="flex flex-col gap-2.5">
        {list && (
          <div className="flex flex-wrap gap-1.5">
            {list.roots.map((r) => (
              <Button key={r.path} size="sm" variant="secondary" icon={r.label === '~' ? 'home' : 'hard_drive'} onClick={() => void open(r.path)}>
                {r.label === '~' ? t('folderBrowser.home') : r.label}
              </Button>
            ))}
          </div>
        )}
        {list && (
          <div className="flex items-center gap-2 rounded-lg bg-surface2 px-3 py-2">
            <Icon name="folder_open" className="shrink-0 text-accent" />
            <span className="min-w-0 flex-1 font-mono text-[12.5px] [overflow-wrap:anywhere]">{list.path}</span>
          </div>
        )}
        {error && (
          <p role="alert" className="text-sm text-bad">
            {t(`folderBrowser.errors.${error}`, { defaultValue: t('common.error') })}
          </p>
        )}
        <ul className="flex max-h-[50vh] flex-col overflow-y-auto rounded-lg border border-line" aria-busy={loading}>
          {list?.parent && (
            <li>
              <FolderRow icon="arrow_upward" label={t('folderBrowser.up')} chevron={false} onClick={() => void open(list.parent!)} />
            </li>
          )}
          {folders.map((f) => (
            <li key={f.path}>
              <FolderRow icon="folder" label={f.name} muted={f.hidden} onClick={() => void open(f.path)} />
            </li>
          ))}
          {list && folders.length === 0 && <li className="px-3 py-3 text-sm text-muted">{t('folderBrowser.empty')}</li>}
        </ul>
        {list?.truncated && <p className="text-[12.5px] text-muted">{t('folderBrowser.truncated')}</p>}
        {hiddenCount > 0 && (
          <button type="button" className="cursor-pointer self-start text-[13px] font-semibold text-accent underline" onClick={() => setShowHidden(!showHidden)}>
            {showHidden ? t('folderBrowser.hideHidden') : t('folderBrowser.showHidden', { count: hiddenCount })}
          </button>
        )}
      </div>
    </Modal>
  )
}

function FolderRow({ icon, label, muted = false, chevron = true, onClick }: { icon: string; label: string; muted?: boolean; chevron?: boolean; onClick: () => void }) {
  return (
    <button
      type="button"
      onClick={onClick}
      className={`flex min-h-11 w-full cursor-pointer items-center gap-2.5 border-b border-line px-3 py-2 text-left text-sm last:border-b-0 hover:bg-surface2 ${muted ? 'text-muted' : ''}`}
    >
      <Icon name={icon} className="shrink-0 text-[20px] text-accent" />
      <span className="min-w-0 flex-1 [overflow-wrap:anywhere]">{label}</span>
      {chevron && <Icon name="chevron_right" className="shrink-0 text-[18px] text-muted" />}
    </button>
  )
}
