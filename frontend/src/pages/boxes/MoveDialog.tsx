import { useEffect, useMemo, useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Button, Callout, Field, HelpTip, Modal, Segmented, selectClass } from '../../components/ui'
import { api } from '../../lib/api'
import type { VaultOverview } from '../../lib/types'
import { errorMessage, fileName, planDrop, type DragSource, type MoveMode, type MoveResult, type SaveBoxes, type SaveEntry } from './boxes'

type Dest = 'save' | 'vault'

/**
 * Mover / Copiar / Anexar com destino escolhido (o caminho sem arrastar, e o único no celular).
 * Destino num save: só posições vazias (o /api/boxes/move nunca sobrescreve). Destino no cofre: a caixa
 * escolhida (primeira posição livre) ou a Caixa de entrada.
 */
export function MoveDialog({
  source,
  title,
  allowVault = true,
  onClose,
  onDone,
}: {
  source: DragSource
  title: string
  /** Falso para Pokémon de jogo fora do catálogo de troca (não vão para o cofre). */
  allowVault?: boolean
  onClose: () => void
  onDone: (result: MoveResult) => void
}) {
  const { t } = useTranslation()
  const fromSave = source.kind === 'save'
  const [dest, setDest] = useState<Dest>('save')
  const [mode, setMode] = useState<MoveMode>('copy')
  const [saves, setSaves] = useState<SaveEntry[] | null>(null)
  const [saveId, setSaveId] = useState('')
  const [save, setSave] = useState<SaveBoxes | null>(null)
  const [box, setBox] = useState(0)
  const [slot, setSlot] = useState(-1)
  const [vault, setVault] = useState<VaultOverview | null>(null)
  const [vaultBox, setVaultBox] = useState('inbox')
  const [busy, setBusy] = useState(false)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    api
      .get<SaveEntry[]>('/api/boxes/saves')
      .then((list) => {
        const usable = list.filter((s) => s.info?.writable)
        setSaves(usable)
        setSaveId((current) => current || usable[0]?.id || '')
      })
      .catch(() => setSaves([]))
    if (fromSave) api.get<VaultOverview>('/api/vault').then(setVault).catch(() => undefined)
  }, [fromSave])

  useEffect(() => {
    setSave(null)
    if (!saveId) return
    api.get<SaveBoxes>(`/api/boxes/saves/${saveId}`).then(setSave).catch(() => setSave(null))
  }, [saveId])

  const free = useMemo(() => {
    if (!save) return []
    const taken = new Set(save.slots.filter((s) => s.box === box).map((s) => s.slot))
    return Array.from({ length: save.info.slotsPerBox }, (_, i) => i).filter((i) => !taken.has(i))
  }, [save, box])

  useEffect(() => setSlot(free[0] ?? -1), [free])

  // Anexar só existe entre o cofre e um save.
  const canAttach = fromSave ? dest === 'vault' : dest === 'save'
  useEffect(() => {
    if (!canAttach && mode === 'attach') setMode('copy')
  }, [canAttach, mode])

  const ready = dest === 'vault' ? fromSave : !!save && slot >= 0

  async function submit() {
    const plan =
      dest === 'vault'
        ? planDrop(source, { kind: 'vault', boxId: vaultBox === 'inbox' ? null : vaultBox, slot: null, occupied: false }, mode)
        : planDrop(source, { kind: 'save', saveId, box, slot, data: null }, mode)
    if (plan.kind === 'refuse') return setError(t(`boxes.errors.${plan.code}`))
    if (plan.kind !== 'move') return
    setBusy(true)
    setError(null)
    try {
      onDone(await api.post<MoveResult>('/api/boxes/move', plan.body))
    } catch (e) {
      setError(errorMessage(t, e))
    } finally {
      setBusy(false)
    }
  }

  return (
    <Modal
      title={title}
      actions={
        <>
          <Button variant="secondary" disabled={busy} onClick={onClose}>
            {t('common.cancel')}
          </Button>
          <Button disabled={!ready || busy} onClick={() => void submit()}>
            {busy ? t('boxes.working') : t(`boxes.mode.${mode}`)}
          </Button>
        </>
      }
    >
      <div className="flex flex-col gap-3.5">
        {fromSave && allowVault && (
          <Segmented<Dest>
            value={dest}
            onChange={setDest}
            options={[
              { value: 'save', label: t('boxes.dest.save') },
              { value: 'vault', label: t('boxes.dest.vault') },
            ]}
          />
        )}

        <div className="flex flex-col gap-1.5">
          <div className="flex items-center gap-1.5 text-[13.5px] font-bold">
            {t('boxes.modeLabel')}
            <HelpTip>{t('boxes.modeHelp')}</HelpTip>
          </div>
          <Segmented<MoveMode>
            value={mode}
            onChange={setMode}
            options={[
              { value: 'copy', label: t('boxes.mode.copy') },
              { value: 'move', label: t('boxes.mode.move') },
              ...(canAttach ? [{ value: 'attach' as const, label: t('boxes.mode.attach') }] : []),
            ]}
          />
          <p className="text-[12.5px] text-muted">{t(`boxes.modeHint.${mode}`)}</p>
        </div>

        {dest === 'save' ? (
          saves === null ? (
            <p className="text-sm text-muted">{t('common.loading')}</p>
          ) : saves.length === 0 ? (
            <Callout tone="amber" icon="info">
              {t('boxes.noWritableSaves')}
            </Callout>
          ) : (
            <>
              <Field label={t('boxes.save')}>
                <select className={selectClass} value={saveId} onChange={(e) => setSaveId(e.target.value)}>
                  {saves.map((s) => (
                    <option key={s.id} value={s.id}>
                      {fileName(s.path)} · {s.info?.trainerName}
                    </option>
                  ))}
                </select>
              </Field>
              {save && (
                <div className="grid grid-cols-2 gap-2.5">
                  <Field label={t('boxes.box')}>
                    <select className={selectClass} value={box} onChange={(e) => setBox(Number(e.target.value))}>
                      {Array.from({ length: save.info.boxCount }, (_, i) => (
                        <option key={i} value={i}>
                          {save.boxNames[i] || t('boxes.boxN', { n: i + 1 })}
                        </option>
                      ))}
                    </select>
                  </Field>
                  <Field label={t('boxes.slot')}>
                    <select className={selectClass} value={slot} onChange={(e) => setSlot(Number(e.target.value))} disabled={free.length === 0}>
                      {free.length === 0 && <option value={-1}>{t('boxes.boxFull')}</option>}
                      {free.map((i) => (
                        <option key={i} value={i}>
                          {i + 1}
                        </option>
                      ))}
                    </select>
                  </Field>
                </div>
              )}
              {source.kind === 'vault' && <p className="text-[12.5px] text-muted">{t('boxes.convertHint')}</p>}
            </>
          )
        ) : (
          <Field label={t('boxes.vaultBox')}>
            <select className={selectClass} value={vaultBox} onChange={(e) => setVaultBox(e.target.value)}>
              <option value="inbox">{t('vault.inbox')}</option>
              {vault?.boxes.map((b) => (
                <option key={b.id} value={b.id}>
                  {b.name}
                </option>
              ))}
            </select>
          </Field>
        )}

        {error && (
          <Callout tone="red" icon="error">
            {error}
          </Callout>
        )}
      </div>
    </Modal>
  )
}
