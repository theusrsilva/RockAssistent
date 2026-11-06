import { useCallback, useEffect, useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Link, useNavigate } from 'react-router'
import { Badge, Button, Callout, Icon, IconButton, Segmented, selectClass, useIsMobile, useToast } from '../../components/ui'
import { ApiError, api, spriteUrl } from '../../lib/api'
import { GAME_VERSIONS } from '../../lib/gameVersions'
import { gameByFamily } from '../../lib/games'
import { VAULT_CHANGED } from '../../lib/pendingDrop'
import type { VaultOverview, VaultPokemon } from '../../lib/types'
import { BoxGrid, PokemonTile } from '../vault/BoxGrid'
import { acceptsAny, draggedSource, startSaveDrag } from '../vault/dnd'
import {
  backupsOf,
  errorMessage,
  fileName,
  planDrop,
  type ApplyResult,
  type DragSource,
  type DropTarget,
  type MoveMode,
  type MoveResult,
  type SaveBoxes,
  type SaveEntry,
  type SaveFolder,
  type SaveSlot,
} from './boxes'
import { ComingSoonActions } from './ComingSoonActions'
import { MoveDialog } from './MoveDialog'

const gameName = (key: string) => GAME_VERSIONS.find((g) => g.key === key)?.name ?? key
const slotKey = (s: { box: number; slot: number }) => `${s.box}:${s.slot}`

/**
 * Aba "Saves" do Boxes: saves das pastas configuradas à esquerda, caixas do save no meio (com uma caixa do cofre
 * embaixo, para arrastar entre os dois) e o painel do Pokémon à direita. Só na janela do PC.
 */
export function SavesTab({ onForbidden }: { onForbidden: () => void }) {
  const { t } = useTranslation()
  const toast = useToast()
  const mobile = useIsMobile()
  const [folders, setFolders] = useState<SaveFolder[] | null>(null)
  const [saves, setSaves] = useState<SaveEntry[] | null>(null)
  const [refreshing, setRefreshing] = useState(false)
  const [saveId, setSaveId] = useState<string | null>(null)
  const [save, setSave] = useState<SaveBoxes | null>(null)
  const [box, setBox] = useState(0)
  const [focused, setFocused] = useState<string | null>(null)
  const [dragMode, setDragMode] = useState<MoveMode>('copy')
  const [moving, setMoving] = useState<SaveSlot | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [busy, setBusy] = useState(false)
  // Caixa do cofre mostrada embaixo ('inbox' ou o id da caixa).
  const [overview, setOverview] = useState<VaultOverview | null>(null)
  const [vaultScope, setVaultScope] = useState('inbox')
  const [vaultList, setVaultList] = useState<VaultPokemon[]>([])
  const [vaultOver, setVaultOver] = useState(false)

  const forbidden = useCallback(
    (e: unknown) => {
      if (e instanceof ApiError && e.status === 403) {
        onForbidden()
        return true
      }
      return false
    },
    [onForbidden],
  )

  const loadList = useCallback(
    async (refresh = false) => {
      setRefreshing(refresh)
      try {
        const list = await api.get<SaveEntry[]>(`/api/boxes/saves${refresh ? '?refresh=1' : ''}`)
        setSaves(list)
        setSaveId((current) => (current && list.some((s) => s.id === current) ? current : (list.find((s) => s.info)?.id ?? null)))
      } catch (e) {
        if (!forbidden(e)) setSaves([])
      } finally {
        setRefreshing(false)
      }
    },
    [forbidden],
  )

  const loadSave = useCallback(async (id: string) => {
    try {
      setSave(await api.get<SaveBoxes>(`/api/boxes/saves/${id}`))
    } catch (e) {
      setSave(null)
      setError(errorMessage(t, e))
    }
  }, [t])

  const loadVault = useCallback(() => {
    api.get<VaultOverview>('/api/vault').then(setOverview).catch(() => undefined)
    api
      .get<VaultPokemon[]>(`/api/vault/pokemon?scope=${vaultScope}`)
      .then(setVaultList)
      .catch(() => setVaultList([]))
  }, [vaultScope])

  useEffect(() => {
    api.get<SaveFolder[]>('/api/settings/save-folders').then(setFolders).catch((e) => forbidden(e) || setFolders([]))
    void loadList()
  }, [loadList, forbidden])

  useEffect(() => {
    setSave(null)
    setFocused(null)
    setBox(0)
    setError(null)
    if (saveId) void loadSave(saveId)
  }, [saveId, loadSave])

  useEffect(loadVault, [loadVault])
  useEffect(() => {
    window.addEventListener(VAULT_CHANGED, loadVault)
    return () => window.removeEventListener(VAULT_CHANGED, loadVault)
  }, [loadVault])

  /** Depois de gravar: relê o save aberto (a Gen 1/2 compacta as caixas), a lista (SHA novo) e o cofre. */
  async function afterWrite(backups: string[]) {
    if (saveId) await loadSave(saveId)
    void loadList()
    loadVault()
    toast(backups.length > 0 ? t('boxes.doneWithBackup', { count: backups.length }) : t('boxes.done'))
  }

  async function run(source: DragSource, target: DropTarget) {
    setError(null)
    const plan = planDrop(source, target, dragMode)
    if (plan.kind === 'none') return
    if (plan.kind === 'refuse') return setError(t(`boxes.errors.${plan.code}`))
    setBusy(true)
    try {
      if (plan.kind === 'move') {
        const r = await api.post<MoveResult>('/api/boxes/move', plan.body)
        if (r.converted) toast(t('boxes.converted'))
        await afterWrite(backupsOf(r))
      } else if (plan.kind === 'apply') {
        // Troca de lugar no mesmo save: uma gravação, conferindo que o arquivo não mudou desde que foi aberto.
        const r = await api.post<ApplyResult>(`/api/boxes/saves/${plan.saveId}/apply`, { changes: plan.changes, sha256: save?.info.sha256 })
        await afterWrite([r.backupPath])
      } else {
        await api.post(`/api/vault/pokemon/${plan.pokemonId}/move`, { boxId: plan.boxId, slot: plan.slot })
        loadVault()
      }
    } catch (e) {
      if (forbidden(e)) return
      setError(errorMessage(t, e))
      // O emulador gravou o save no meio do caminho: mostra o que está no disco agora.
      if (e instanceof ApiError && e.code === 'save_changed' && saveId) {
        await loadSave(saveId)
        void loadList()
      }
    } finally {
      setBusy(false)
    }
  }

  const info = save?.info
  const writable = !!info?.writable
  const inBox = save ? new Map(save.slots.filter((s) => s.box === box).map((s) => [s.slot, s])) : new Map<number, SaveSlot>()
  const party = save?.slots.filter((s) => s.box === -1) ?? []
  const focusedSlot = save?.slots.find((s) => slotKey(s) === focused) ?? null
  const vaultBox = vaultScope === 'inbox' ? null : vaultScope

  const sourceOf = (s: SaveSlot): Extract<DragSource, { kind: 'save' }> => ({
    kind: 'save',
    saveId: saveId!,
    box: s.box,
    slot: s.slot,
    data: s.data,
    family: s.family,
  })

  if (folders && folders.length === 0) {
    return (
      <Callout tone="sky" icon="folder_open">
        <p>{t('boxes.noFolders')}</p>
        <Link to="/configuracoes?aba=preferencias#pastas" className="mt-1 inline-block font-bold text-accent underline">
          {t('boxes.addFolder')}
        </Link>
      </Callout>
    )
  }

  const panel = focusedSlot && saveId && info && (
    <SlotPanel
      entry={focusedSlot}
      saveId={saveId}
      place={
        focusedSlot.box === -1
          ? t('boxes.partyPlace', { file: fileName(info.path), slot: focusedSlot.slot + 1 })
          : t('boxes.slotPlace', { file: fileName(info.path), box: save.boxNames[focusedSlot.box] || t('boxes.boxN', { n: focusedSlot.box + 1 }), slot: focusedSlot.slot + 1 })
      }
      canMove={focusedSlot.box >= 0}
      onMove={() => setMoving(focusedSlot)}
      onClose={() => setFocused(null)}
    />
  )

  return (
    <div className="flex flex-wrap items-start gap-4">
      {/* Lista de saves */}
      <aside className="flex w-full flex-col gap-1.5 rounded-xl border border-line bg-surface p-3 md:w-[240px] md:shrink-0">
        <div className="flex items-center gap-1.5 px-1">
          <span className="flex-1 text-sm font-bold">{t('boxes.savesTitle')}</span>
          <IconButton name="refresh" label={t('boxes.refresh')} className="h-8 w-8 border-transparent" disabled={refreshing} onClick={() => void loadList(true)} />
          <Link to="/configuracoes?aba=preferencias#pastas" title={t('boxes.folders')} aria-label={t('boxes.folders')} className="flex h-8 w-8 items-center justify-center rounded-lg text-muted hover:text-ink">
            <Icon name="folder" className="text-[19px]" />
          </Link>
        </div>
        {saves === null ? (
          <p className="px-1 text-sm text-muted">{t('common.loading')}</p>
        ) : saves.length === 0 ? (
          <p className="px-1 text-[13px] text-muted">{t('boxes.noSaves')}</p>
        ) : (
          <div className="flex gap-1.5 overflow-x-auto md:flex-col md:overflow-visible">
            {saves.map((s) => (
              <button
                key={s.id}
                type="button"
                disabled={!s.info}
                title={s.path}
                onClick={() => setSaveId(s.id)}
                className={`flex min-w-[180px] flex-col items-start gap-0.5 rounded-lg border px-2.5 py-2 text-left md:min-w-0 ${
                  s.id === saveId ? 'border-accent bg-accent-soft' : 'border-transparent bg-surface2 enabled:cursor-pointer enabled:hover:border-line'
                } disabled:opacity-80`}
              >
                <span className="w-full truncate text-[13.5px] font-bold">{fileName(s.path)}</span>
                {s.info ? (
                  <span className="flex w-full flex-wrap items-center gap-1 text-[12px] text-muted">
                    {gameName(s.info.gameKey)} · {s.info.trainerName}
                    {!s.info.writable && <Badge tone="amber">{t('boxes.readOnly')}</Badge>}
                  </span>
                ) : (
                  <span className="flex items-center gap-1 text-[12px] text-bad">
                    <Icon name={s.error === 'sav_unsupported' ? 'block' : 'error'} className="text-[15px]" />
                    {t(`boxes.errors.${s.error}`, { defaultValue: t('boxes.errors.sav_unknown') })}
                  </span>
                )}
              </button>
            ))}
          </div>
        )}
      </aside>

      {/* Caixas do save + caixa do cofre */}
      <div className="flex min-w-0 flex-[999_1_min(340px,100%)] flex-col gap-4">
        {error && (
          <Callout tone="red" icon="error">
            {error}
          </Callout>
        )}
        {!saveId ? null : !save || !info ? (
          <p className="text-muted">{t('common.loading')}</p>
        ) : (
          <div className="flex flex-col gap-3 rounded-xl border border-line bg-surface p-4">
            <div className="flex flex-wrap items-center gap-2">
              <IconButton name="chevron_left" label={t('boxes.prevBox')} className="h-9 w-9" disabled={box === 0} onClick={() => setBox(box - 1)} />
              <select className={`${selectClass} h-9! w-auto! min-w-[140px] flex-1`} value={box} onChange={(e) => setBox(Number(e.target.value))} aria-label={t('boxes.box')}>
                {Array.from({ length: info.boxCount }, (_, i) => (
                  <option key={i} value={i}>
                    {save.boxNames[i] || t('boxes.boxN', { n: i + 1 })}
                  </option>
                ))}
              </select>
              <IconButton name="chevron_right" label={t('boxes.nextBox')} className="h-9 w-9" disabled={box >= info.boxCount - 1} onClick={() => setBox(box + 1)} />
              <span className="text-[12.5px] text-muted">
                {gameName(info.gameKey)} · {t('boxes.genN', { n: info.generation })}
              </span>
            </div>
            {!writable && (
              <Callout tone="amber" icon="lock" className="py-2 text-[13px]">
                {t('boxes.errors.save_read_only')}
              </Callout>
            )}
            <div className="grid grid-cols-3 gap-2 sm:grid-cols-6">
              {Array.from({ length: info.slotsPerBox }, (_, slot) => {
                const s = inBox.get(slot)
                return (
                  <SaveCell
                    key={slot}
                    entry={s ?? null}
                    focused={!!s && focused === slotKey(s)}
                    // Copiar de um save só de leitura também vale; soltar nele, não.
                    draggable={!busy}
                    onDragStart={(e) => s && startSaveDrag(e, sourceOf(s))}
                    onClick={() => s && setFocused(focused === slotKey(s) ? null : slotKey(s))}
                    onDropSource={writable && !busy ? (src) => void run(src, { kind: 'save', saveId: saveId!, box, slot, data: s?.data ?? null }) : undefined}
                  />
                )
              })}
            </div>
            {party.length > 0 && (
              <div className="flex flex-col gap-1.5">
                <span className="text-[13px] font-bold text-muted">{t('boxes.party')}</span>
                <div className="grid grid-cols-3 gap-2 sm:grid-cols-6">
                  {party.map((s) => (
                    <SaveCell key={s.slot} entry={s} focused={focused === slotKey(s)} draggable={false} onClick={() => setFocused(focused === slotKey(s) ? null : slotKey(s))} />
                  ))}
                </div>
              </div>
            )}
            <div className="flex flex-wrap items-center gap-2 text-[12.5px] text-muted">
              <span>{t('boxes.dragMode')}</span>
              <Segmented<MoveMode>
                value={dragMode}
                onChange={setDragMode}
                options={[
                  { value: 'copy', label: t('boxes.mode.copy') },
                  { value: 'move', label: t('boxes.mode.move') },
                ]}
              />
              <span className="min-w-[200px] flex-1">{t('boxes.dragHint')}</span>
            </div>
          </div>
        )}

        {/* Caixa do cofre, para arrastar de um lado para o outro. */}
        <div className="flex flex-col gap-3 rounded-xl border border-line bg-surface p-4">
          <div className="flex flex-wrap items-center gap-2">
            <Icon name="inventory_2" className="text-accent" />
            <span className="flex-1 text-sm font-bold">{t('boxes.vaultPane')}</span>
            <select className={`${selectClass} h-9! w-auto!`} value={vaultScope} onChange={(e) => setVaultScope(e.target.value)} aria-label={t('boxes.vaultBox')}>
              <option value="inbox">{t('vault.inbox')}</option>
              {overview?.boxes.map((b) => (
                <option key={b.id} value={b.id}>
                  {b.name}
                </option>
              ))}
            </select>
          </div>
          {vaultBox ? (
            <BoxGrid
              pokemon={vaultList}
              selected={new Set()}
              focused={null}
              multi={false}
              hint={false}
              onTileClick={() => undefined}
              onToggle={() => undefined}
              onDrop={(id, slot) => void run({ kind: 'vault', pokemonId: id }, { kind: 'vault', boxId: vaultBox, slot, occupied: true })}
              onDropSource={(src, slot) =>
                void run(src, { kind: 'vault', boxId: vaultBox, slot, occupied: vaultList.some((p) => p.slot === slot) })
              }
            />
          ) : (
            <div
              className={`min-h-[120px] rounded-lg p-1 ${vaultOver ? 'ring-2 ring-accent' : ''}`}
              onDragOver={(e) => {
                if (acceptsAny(e)) {
                  e.preventDefault()
                  setVaultOver(true)
                }
              }}
              onDragLeave={() => setVaultOver(false)}
              onDrop={(e) => {
                setVaultOver(false)
                const src = draggedSource(e)
                if (src) void run(src, { kind: 'vault', boxId: null, slot: null, occupied: false })
              }}
            >
              {vaultList.length === 0 ? (
                <p className="p-6 text-center text-[13px] text-muted">{t('boxes.dropInbox')}</p>
              ) : (
                <div className="grid gap-2 [grid-template-columns:repeat(auto-fill,minmax(84px,1fr))]">
                  {vaultList.map((p) => (
                    <div key={p.id} className="aspect-[1/1.08]">
                      <PokemonTile pokemon={p} selected={false} focused={false} showCheckbox={false} onClick={() => undefined} onToggle={() => undefined} />
                    </div>
                  ))}
                </div>
              )}
            </div>
          )}
        </div>
      </div>

      {panel &&
        (mobile ? (
          <>
            <div className="fixed inset-0 z-40 bg-black/45" onClick={() => setFocused(null)} />
            <div className="fixed inset-x-0 bottom-0 z-[41] max-h-[88vh] overflow-auto rounded-t-[18px] border border-line bg-surface">{panel}</div>
          </>
        ) : (
          <aside className="sticky top-0 flex max-w-[380px] min-w-[280px] flex-[1_1_300px] flex-col overflow-hidden rounded-xl border border-line bg-surface">{panel}</aside>
        ))}

      {moving && saveId && (
        <MoveDialog
          source={sourceOf(moving)}
          title={t('boxes.moveTitle', { name: (moving.summary.nickname || moving.summary.speciesName).toUpperCase() })}
          allowVault={moving.family !== null}
          onClose={() => setMoving(null)}
          onDone={(r) => {
            setMoving(null)
            setFocused(null)
            if (r.converted) toast(t('boxes.converted'))
            void afterWrite(backupsOf(r))
          }}
        />
      )}
    </div>
  )
}

/** Uma posição da caixa de um save (vazia ou com o Pokémon). */
function SaveCell({
  entry,
  focused,
  draggable,
  onDragStart,
  onClick,
  onDropSource,
}: {
  entry: SaveSlot | null
  focused: boolean
  draggable: boolean
  onDragStart?: (e: React.DragEvent) => void
  onClick: () => void
  onDropSource?: (source: DragSource) => void
}) {
  const { t } = useTranslation()
  const [over, setOver] = useState(false)
  const s = entry?.summary
  const name = s ? (s.nickname || s.speciesName).toUpperCase() : ''
  return (
    <div
      className={`aspect-[1/1.08] rounded-lg ${over ? 'ring-2 ring-accent' : ''} ${entry ? '' : 'border-[1.5px] border-dashed border-line'}`}
      onDragOver={(e) => {
        if (onDropSource && acceptsAny(e)) {
          e.preventDefault()
          setOver(true)
        }
      }}
      onDragLeave={() => setOver(false)}
      onDrop={(e) => {
        setOver(false)
        const src = draggedSource(e)
        if (src && onDropSource) {
          e.preventDefault()
          onDropSource(src)
        }
      }}
    >
      {entry && s && (
        <div
          role="button"
          tabIndex={0}
          draggable={draggable}
          onDragStart={onDragStart}
          onClick={onClick}
          onKeyDown={(e) => {
            if (e.key === 'Enter' || e.key === ' ') {
              e.preventDefault()
              onClick()
            }
          }}
          title={`${name} · ${t('ui.lv', { level: s.level })}`}
          className={`relative flex h-full cursor-pointer flex-col items-center justify-center gap-px rounded-lg border-2 p-1 text-center leading-tight ${
            focused ? 'border-accent bg-accent-soft' : 'border-transparent bg-tile hover:border-accent'
          }`}
        >
          {s.isShiny && (
            <span title={t('pokemon.shiny')} className="absolute top-[5px] right-1.5 text-[#e0a100]">
              <Icon name="auto_awesome" className="text-base" />
            </span>
          )}
          {entry.family === null && (
            <span title={t('boxes.noFamilyShort')} className="absolute top-[5px] left-1.5 text-muted">
              <Icon name="block" className="text-[15px]" />
            </span>
          )}
          <img src={spriteUrl(s.species, { shiny: s.isShiny })} alt="" draggable={false} className="pixelated h-12 w-12 md:h-16 md:w-16" loading="lazy" />
          <span className="w-full truncate text-xs font-bold">{name}</span>
          <span className="text-[11.5px] text-muted">{s.isEgg ? t('pokemon.egg') : t('ui.lv', { level: s.level })}</span>
        </div>
      )}
    </div>
  )
}

/** Painel de um Pokémon do save: resumo e ações (as de troca e cofre só nos jogos do catálogo). */
function SlotPanel({
  entry,
  saveId,
  place,
  canMove,
  onMove,
  onClose,
}: {
  entry: SaveSlot
  saveId: string
  place: string
  canMove: boolean
  onMove: () => void
  onClose: () => void
}) {
  const { t } = useTranslation()
  const navigate = useNavigate()
  const s = entry.summary
  const name = (s.nickname || s.speciesName).toUpperCase()
  const gender = s.gender === 0 ? '♂' : s.gender === 1 ? '♀' : '—'
  const row = (k: string, v: React.ReactNode) => (
    <div key={k} className="flex gap-2.5 text-sm">
      <span className="w-[110px] shrink-0 text-muted">{k}</span>
      <span className="min-w-0 flex-1 font-semibold [overflow-wrap:anywhere]">{v}</span>
    </div>
  )
  return (
    <div className="flex min-h-0 flex-col">
      <div className="flex items-center gap-2 border-b border-line px-3.5 py-3">
        <span className="min-w-0 flex-1 truncate text-[13px] text-muted">{place}</span>
        <button
          type="button"
          onClick={onClose}
          aria-label={t('common.close')}
          className="flex h-[30px] w-[30px] cursor-pointer items-center justify-center rounded-full text-muted hover:bg-surface2"
        >
          <Icon name="close" />
        </button>
      </div>
      <div className="flex items-center gap-3.5 p-4">
        <div className="flex h-[104px] w-[104px] shrink-0 items-center justify-center rounded-lg bg-tile">
          <img src={spriteUrl(s.species, { shiny: s.isShiny })} alt="" className="pixelated h-24 w-24" />
        </div>
        <div className="flex min-w-0 flex-col gap-1">
          <div className="font-display text-[21px] leading-tight font-bold [overflow-wrap:anywhere]">{name}</div>
          <div className="text-[13px] text-muted">
            #{String(s.species).padStart(3, '0')} {s.speciesName} · {s.isEgg ? t('pokemon.egg') : t('ui.lv', { level: s.level })}
          </div>
          <div className="mt-0.5 flex flex-wrap gap-1.5">
            {s.isShiny && (
              <Badge tone="amber" icon="auto_awesome">
                {t('pokemon.shiny')}
              </Badge>
            )}
            {entry.family && <Badge icon="sports_esports">{gameByFamily(entry.family).name}</Badge>}
          </div>
        </div>
      </div>
      <div className="flex flex-col gap-2 px-4 pb-3">
        {entry.family === null && (
          <Callout tone="slate" icon="block" className="py-2 text-[13px]">
            {t('boxes.noFamily')}
          </Callout>
        )}
        {row(t('builder.fields.gender'), gender)}
        {row(t('ui.vault.ot'), `${s.otName} (${s.trainerId})`)}
        {row(t('builder.fields.version'), s.originGame)}
        {row(t('builder.fields.heldItem'), s.heldItem ? `#${s.heldItem}` : t('ui.vault.none'))}
      </div>
      <div className="flex flex-col gap-2 border-t border-line px-4 py-3.5">
        <div className="flex gap-2">
          <Button className="h-[42px] flex-1" icon="drive_file_move" disabled={!canMove} onClick={onMove}>
            {entry.family === null ? t('boxes.moveOtherSave') : t('boxes.moveAction')}
          </Button>
        </div>
        <ComingSoonActions />
        <div className="flex items-center gap-1.5">
          <IconButton name="menu_book" label={t('boxes.viewDex')} onClick={() => navigate(`/pokedex/${s.species}`)} />
          <a
            href={`/api/boxes/saves/${saveId}/slot/${entry.box}/${entry.slot}/file`}
            download
            title={t('ui.vault.exportFile')}
            aria-label={t('ui.vault.exportFile')}
            className="flex h-9 w-9 items-center justify-center rounded-lg border border-line bg-surface text-ink hover:bg-surface2"
          >
            <Icon name="file_download" />
          </a>
        </div>
      </div>
    </div>
  )
}
