import { useEffect, useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Badge, Button, Callout, Icon, IconButton, inputClass, useToast } from '../../components/ui'
import { ApiError, api } from '../../lib/api'
import { errorMessage, type SaveFolder } from '../boxes/boxes'

/**
 * Pastas de saves da tela Boxes (só na janela do PC). "Adicionar pasta" abre o diálogo de pasta da janela
 * nativa (POST /api/saves/open-folder); no navegador (--no-window) o caminho é digitado.
 */
export function SaveFoldersCard() {
  const { t } = useTranslation()
  const toast = useToast()
  const [folders, setFolders] = useState<SaveFolder[] | null>(null)
  const [typing, setTyping] = useState(false)
  const [typed, setTyped] = useState('')
  const [busy, setBusy] = useState(false)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    api.get<SaveFolder[]>('/api/settings/save-folders').then(setFolders).catch(() => setFolders([]))
  }, [])

  async function save(next: { path: string; pattern: string }[]) {
    setBusy(true)
    setError(null)
    try {
      setFolders(await api.put<SaveFolder[]>('/api/settings/save-folders', next))
      toast(t('common.saved'))
      return true
    } catch (e) {
      setError(errorMessage(t, e))
      return false
    } finally {
      setBusy(false)
    }
  }

  const current = () => (folders ?? []).map((f) => ({ path: f.path, pattern: f.pattern }))

  async function add() {
    setError(null)
    try {
      const r = await api.post<{ path: string } | undefined>('/api/saves/open-folder')
      if (r?.path) await save([...current(), { path: r.path, pattern: '' }])
    } catch (e) {
      // Sem a janela nativa (aberto no navegador): digitar o caminho.
      if (e instanceof ApiError && e.code === 'no_window') setTyping(true)
      else setError(errorMessage(t, e))
    }
  }

  async function addTyped() {
    if (await save([...current(), { path: typed.trim(), pattern: '' }])) {
      setTyped('')
      setTyping(false)
    }
  }

  async function refresh() {
    setBusy(true)
    try {
      const list = await api.get<{ info: unknown }[]>('/api/boxes/saves?refresh=1')
      toast(t('boxes.foldersCard.found', { count: list.filter((s) => s.info).length }))
    } catch (e) {
      setError(errorMessage(t, e))
    } finally {
      setBusy(false)
    }
  }

  return (
    <div id="pastas" className="flex max-w-[760px] flex-col gap-3 rounded-xl border border-line bg-surface px-[22px] py-[18px]">
      <div className="flex flex-wrap items-center gap-2">
        <div className="min-w-[200px] flex-1">
          <div className="font-bold">{t('boxes.foldersCard.title')}</div>
          <div className="text-[13px] text-muted">{t('boxes.foldersCard.sub')}</div>
        </div>
        <Button variant="secondary" size="sm" icon="refresh" disabled={busy || !folders?.length} onClick={() => void refresh()}>
          {t('boxes.refresh')}
        </Button>
        <Button size="sm" icon="create_new_folder" disabled={busy} onClick={() => void add()}>
          {t('boxes.foldersCard.add')}
        </Button>
      </div>

      {folders === null ? (
        <p className="text-sm text-muted">{t('common.loading')}</p>
      ) : folders.length === 0 ? (
        <p className="text-sm text-muted">{t('boxes.foldersCard.empty')}</p>
      ) : (
        <ul className="flex flex-col gap-1.5">
          {folders.map((f) => (
            <li key={f.path} className="flex items-center gap-2 rounded-lg bg-surface2 px-3 py-2">
              <Icon name="folder" className="text-accent" />
              <span className="min-w-0 flex-1 font-mono text-[12.5px] [overflow-wrap:anywhere]">{f.path}</span>
              {!f.exists && <Badge tone="amber">{t('boxes.foldersCard.missing')}</Badge>}
              <IconButton
                name="delete"
                label={t('boxes.foldersCard.remove')}
                className="h-8 w-8 border-transparent bg-transparent text-bad"
                disabled={busy}
                onClick={() => void save(current().filter((x) => x.path !== f.path))}
              />
            </li>
          ))}
        </ul>
      )}

      {typing && (
        <div className="flex flex-wrap gap-2">
          <input
            autoFocus
            className={`${inputClass} h-10! min-w-[220px] flex-1`}
            value={typed}
            placeholder={t('boxes.foldersCard.placeholder')}
            onChange={(e) => setTyped(e.target.value)}
            onKeyDown={(e) => e.key === 'Enter' && typed.trim() && void addTyped()}
          />
          <Button size="sm" className="h-10" disabled={busy || !typed.trim()} onClick={() => void addTyped()}>
            {t('boxes.foldersCard.addTyped')}
          </Button>
        </div>
      )}

      {error && (
        <Callout tone="red" icon="error" className="text-[13px]">
          {error}
        </Callout>
      )}

      <details className="text-[13px] text-muted">
        <summary className="cursor-pointer font-semibold text-ink">{t('boxes.foldersCard.whereTitle')}</summary>
        <ul className="mt-1.5 flex list-disc flex-col gap-1 pl-5">
          {(['mgba', 'melonds', 'switchEmu', 'citra'] as const).map((k) => (
            <li key={k}>{t(`boxes.foldersCard.where.${k}`)}</li>
          ))}
        </ul>
        <p className="mt-1.5">{t('boxes.foldersCard.whereNote')}</p>
      </details>
    </div>
  )
}
