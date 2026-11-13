import { useCallback, useEffect, useMemo, useRef, useState } from 'react'
import { useTranslation } from 'react-i18next'
import { useNavigate, useParams } from 'react-router'
import { Badge, Button, Callout, Card, Icon, Modal, useIsMobile, useToast } from '../../components/ui'
import { ApiError, api, spriteUrl } from '../../lib/api'
import { useApiGet } from '../../lib/dex'
import { formatCombo, globalActive, modalOpen, useHotkey, useHotkeySettings } from '../../lib/hotkeys'
import {
  HUNT_STEPS,
  cumulative,
  formatElapsed,
  formatOdds,
  formatPercent,
  huntErrorMessage,
  huntMethodName,
  loadHuntStep,
  saveHuntStep,
  saveElapsedOnExit,
  shinyName,
  stepFor,
  useGlobalHotkeyEvents,
  useHuntIncrements,
  useSpeciesOptions,
  type Hunt,
  type HuntGame,
} from '../../lib/hunts'
import { changeMiniWindow, useMiniWindow } from '../../lib/miniWindow'
import { BackLink } from '../dex/tabs/parts'
import { MiniHunt } from './MiniHunt'
import { gameName } from '../../components/HuntCard'

/** O cronômetro é gravado a cada 30 s enquanto corre, ao pausar e ao sair da página. */
const SAVE_EVERY_MS = 30_000

/**
 * Contador de uma hunt: número grande, odd atual, chance acumulada, cronômetro, + e −, "Achei!", "Desistir" e notas.
 * Os atalhos (Configurações → Preferências) valem só com esta página aberta, fora de campos de texto.
 */
export default function HuntCounter() {
  const { t, i18n } = useTranslation()
  const toast = useToast()
  const navigate = useNavigate()
  const mobile = useIsMobile()
  const mini = useMiniWindow()
  const id = useParams().id ?? ''
  const [hunt, setHunt] = useState<Hunt | null>(null)
  const [loadError, setLoadError] = useState<ApiError | Error | null>(null)
  const [notes, setNotes] = useState('')
  const [confirm, setConfirm] = useState<'giveUp' | 'delete' | null>(null)
  const [busy, setBusy] = useState(false)
  // Quanto cada clique (ou tecla) soma ou tira: de 1 a 10, guardado por hunt neste navegador (horda = 5 por vez).
  const [step, setStepState] = useState(() => loadHuntStep(id))
  const setStep = (value: number) => {
    setStepState(value)
    saveHuntStep(id, value)
  }
  const species = useSpeciesOptions()
  const hotkeys = useHotkeySettings()
  const games = useApiGet<HuntGame[]>(hunt ? `/api/hunts/species/${hunt.species}/games` : null)

  const reload = useCallback(() => {
    api
      .get<Hunt>(`/api/hunts/${id}`)
      .then((h) => {
        setHunt(h)
        setLoadError(null)
        // Terminou em outro lugar (outra aba, atalho global): o cronômetro para com o tempo gravado.
        if (h.endedAt) {
          baseSeconds.current = h.elapsedSeconds
          runningSince.current = null
          setRunning(false)
        }
      })
      .catch((e: Error) => setLoadError(e))
  }, [id])

  // ---- Cronômetro (no navegador; o servidor só guarda o total) ----
  const baseSeconds = useRef(0)
  const runningSince = useRef<number | null>(null)
  const [, setTick] = useState(0)
  const [running, setRunning] = useState(false)
  const elapsed = () => baseSeconds.current + (runningSince.current ? (Date.now() - runningSince.current) / 1000 : 0)

  const persistElapsed = useCallback(() => {
    if (!id) return
    void api.put(`/api/hunts/${id}`, { elapsedSeconds: Math.floor(elapsed()) }).catch(() => undefined)
  }, [id])

  useEffect(() => {
    let alive = true
    api
      .get<Hunt>(`/api/hunts/${id}`)
      .then((h) => {
        if (!alive) return
        setHunt(h)
        setNotes(h.notes ?? '')
        baseSeconds.current = h.elapsedSeconds
        // Hunt em andamento: o cronômetro começa a correr ao abrir.
        if (!h.endedAt) {
          runningSince.current = Date.now()
          setRunning(true)
        }
      })
      .catch((e: Error) => alive && setLoadError(e))
    return () => {
      alive = false
      // Saindo da página: grava o tempo e para.
      if (runningSince.current !== null) {
        baseSeconds.current = elapsed()
        runningSince.current = null
        void api.put(`/api/hunts/${id}`, { elapsedSeconds: Math.floor(baseSeconds.current) }).catch(() => undefined)
      }
    }
    // elapsed lê só refs
  }, [id])

  useEffect(() => {
    if (!running) return
    const tick = setInterval(() => setTick((n) => n + 1), 1000)
    const save = setInterval(persistElapsed, SAVE_EVERY_MS)
    // Janela fechada ou recarregada: o pedido sai com keepalive.
    const onHide = () => saveElapsedOnExit(id, elapsed())
    window.addEventListener('pagehide', onHide)
    return () => {
      clearInterval(tick)
      clearInterval(save)
      window.removeEventListener('pagehide', onHide)
    }
  }, [running, id, persistElapsed])

  function pause() {
    baseSeconds.current = elapsed()
    runningSince.current = null
    setRunning(false)
    persistElapsed()
  }

  function resume() {
    runningSince.current = Date.now()
    setRunning(true)
  }

  // ---- Contador (fila: um pedido por vez) ----
  const onIncrementError = useCallback(
    (e: unknown) => {
      toast(huntErrorMessage(t, e))
      if (e instanceof ApiError && (e.code === 'hunt_finished' || e.code === 'hunt_not_found')) reload()
    },
    [t, toast, reload],
  )
  const { increment, idle, pending } = useHuntIncrements(hunt && !hunt.endedAt ? id : null, setHunt, onIncrementError)

  const active = !!hunt && !hunt.endedAt
  // Com o atalho global ligado, a tecla chega pelo servidor (também com a janela na frente): o atalho da janela sai
  // para não contar duas vezes.
  const globalKeys = globalActive(hotkeys)
  // A mini janela é estreita como um celular, mas tem teclado: os atalhos continuam valendo nela.
  const windowKeys = active && (!mobile || mini.on) && !confirm && !globalKeys
  useHotkey(windowKeys ? hotkeys.increment : null, () => increment(step))
  useHotkey(windowKeys ? hotkeys.decrement : null, () => increment(-step))
  useGlobalHotkeyEvents(globalKeys && active, (delta) => increment(delta * step), () => !confirm && !modalOpen())

  // Mini janela (só na janela do PC): o servidor precisa saber que há uma hunt em andamento na tela para encolher a
  // janela ao minimizar; ao sair da tela ou terminar a hunt, a janela volta ao normal.
  useEffect(() => {
    if (!mini.supported || !active) return
    void changeMiniWindow({ huntOpen: true })
    return () => void changeMiniWindow({ huntOpen: false, on: false })
  }, [mini.supported, active])

  const method = useMemo(
    () => games.data?.find((g) => g.gameKey === hunt?.gameKey)?.methods.find((m) => m.id === hunt?.methodId) ?? null,
    [games.data, hunt?.gameKey, hunt?.methodId],
  )

  async function finish(found: boolean) {
    if (!hunt) return
    setBusy(true)
    try {
      await idle()
      const seconds = Math.floor(elapsed())
      baseSeconds.current = seconds
      runningSince.current = null
      setRunning(false)
      const done = await api.post<Hunt>(`/api/hunts/${hunt.id}/finish`, { found, elapsedSeconds: seconds })
      setHunt(done)
      setConfirm(null)
      toast(t(found ? 'hunt.counter.foundToast' : 'hunt.counter.gaveUpToast'))
    } catch (e) {
      toast(huntErrorMessage(t, e))
      reload()
    } finally {
      setBusy(false)
    }
  }

  async function remove() {
    if (!hunt) return
    setBusy(true)
    try {
      await api.delete(`/api/hunts/${hunt.id}`)
      // Hunt apagada: ao sair da página não há tempo para gravar.
      runningSince.current = null
      toast(t('hunt.counter.deleted'))
      navigate('/hunt', { replace: true })
    } catch (e) {
      toast(huntErrorMessage(t, e))
      setBusy(false)
    }
  }

  async function saveNotes() {
    if (!hunt || notes === (hunt.notes ?? '')) return
    try {
      const saved = await api.put<Hunt>(`/api/hunts/${hunt.id}`, { notes })
      setHunt((h) => (h ? { ...h, notes: saved.notes } : h))
      toast(t('common.saved'))
    } catch (e) {
      toast(huntErrorMessage(t, e))
    }
  }

  const back = <BackLink to="/hunt">{t('hunt.back')}</BackLink>

  if (loadError)
    return (
      <div className="flex flex-col gap-4">
        {back}
        <Callout tone="red" icon="error">
          {huntErrorMessage(t, loadError)}
        </Callout>
      </div>
    )
  if (!hunt)
    return (
      <div className="flex flex-col gap-4">
        {back}
        <p className="text-sm text-muted">{t('common.loading')}</p>
      </div>
    )

  const lang = i18n.language
  const count = Math.max(0, hunt.count + pending)
  const name = species?.find((s) => s.value === hunt.species)?.text ?? `#${hunt.species}`
  const keyNames = t('hunt.hotkeys.keyNames', { returnObjects: true }) as Record<string, string>
  const endedAt = hunt.endedAt ? new Date(hunt.endedAt).toLocaleDateString(lang) : ''

  if (mini.on && active)
    return (
      <MiniHunt
        hunt={hunt}
        name={name}
        count={count}
        step={step}
        seconds={elapsed()}
        onIncrement={increment}
        onExpand={() => void changeMiniWindow({ on: false })}
      />
    )

  // Modificadores ligados (badges) e os guiados pelo contador (degrau atual).
  const modifierBadges = (method?.modifiers ?? [])
    .filter((m) => !m.counter)
    .flatMap((m) => {
      const v = hunt.modifiers[m.id]
      if (m.kind === 'toggle') return v === 'true' ? [shinyName(m, lang)] : []
      const step = v !== undefined ? stepFor(m, Number(v)) : null
      return step && step.min > 0 ? [`${shinyName(m, lang)}: ${step.label}`] : []
    })
  const counterSteps = (method?.modifiers ?? []).filter((m) => m.counter && m.kind === 'steps')

  return (
    <div className="mx-auto flex w-full max-w-[720px] flex-col gap-4">
      {back}

      <div className="flex items-center gap-3">
        <img src={spriteUrl(hunt.species, { shiny: hunt.found })} alt="" className="pixelated h-20 w-20 shrink-0" />
        <div className="min-w-0 flex-1">
          <h1 className="truncate font-display text-[23px] leading-tight font-bold md:text-[28px]">{name}</h1>
          <div className="text-sm text-muted">
            {gameName(hunt.gameKey)} · {huntMethodName(hunt, i18n.language)}
          </div>
          {modifierBadges.length > 0 && (
            <div className="mt-1.5 flex flex-wrap gap-1.5">
              {modifierBadges.map((b) => (
                <Badge key={b} tone="accent">
                  {b}
                </Badge>
              ))}
            </div>
          )}
        </div>
        {mini.supported && active && (
          <Button
            variant="secondary"
            size="sm"
            icon="picture_in_picture_alt"
            title={t('hunt.mini.help')}
            onClick={() => void changeMiniWindow({ on: true })}
          >
            {t('hunt.mini.open')}
          </Button>
        )}
      </div>

      {hunt.endedAt && (
        <Callout tone={hunt.found ? 'emerald' : 'slate'} icon={hunt.found ? 'auto_awesome' : 'flag'}>
          {t(hunt.found ? 'hunt.counter.finishedFound' : 'hunt.counter.finishedGaveUp', { date: endedAt })}
        </Callout>
      )}

      <Card className="flex flex-col items-center gap-4 text-center">
        <div className="font-display text-[72px] leading-none font-extrabold tabular-nums md:text-[96px]" aria-live="polite">
          {count.toLocaleString(lang)}
        </div>
        <div className="text-sm text-muted">{t('hunt.encounters')}</div>

        {counterSteps.map((m) => {
          const step = stepFor(m, count)
          return (
            <Badge key={m.id} tone={step ? 'emerald' : 'slate'} icon="trending_up">
              {step
                ? t('hunt.counter.step', { name: shinyName(m, lang), value: count, odds: formatOdds(step.odds, lang) })
                : t('hunt.counter.stepNone', { name: shinyName(m, lang), value: count })}
            </Badge>
          )
        })}

        <div className="grid w-full grid-cols-3 gap-2">
          <Stat label={t('hunt.counter.odds')} value={t('hunt.oneIn', { odds: formatOdds(hunt.odds, lang) })} />
          <Stat
            label={t('hunt.counter.cumulative')}
            value={formatPercent(cumulative(hunt.odds, count), lang)}
          />
          <div className="flex flex-col items-center gap-1 rounded-lg bg-surface2 px-2 py-2.5">
            <span className="text-[12px] text-muted">{t('hunt.counter.time')}</span>
            <span className="font-mono text-[17px] font-bold tabular-nums">{formatElapsed(elapsed())}</span>
            {active && (
              <button
                type="button"
                className="inline-flex cursor-pointer items-center gap-1 text-[12.5px] font-semibold text-accent"
                onClick={(e) => {
                  e.currentTarget.blur()
                  if (running) pause()
                  else resume()
                }}
              >
                <Icon name={running ? 'pause' : 'play_arrow'} className="text-[17px]" />
                {t(running ? 'hunt.counter.pause' : 'hunt.counter.resume')}
              </button>
            )}
          </div>
        </div>
        <p className="text-[12.5px] text-muted">
          {t('hunt.counter.cumulativeHelp', { percent: formatPercent(cumulative(hunt.odds, count), lang) })}
        </p>

        {active && (
          <>
            <div className="flex w-full gap-2">
              <Button
                variant="secondary"
                size="lg"
                icon="remove"
                aria-label={t('hunt.counter.minus', { n: step })}
                className="h-20 w-24 text-2xl"
                disabled={count === 0}
                onClick={(e) => {
                  // Sem foco no botão: o Espaço do atalho não "clica" nele de novo.
                  e.currentTarget.blur()
                  increment(-step)
                }}
              />
              <Button
                size="lg"
                icon="add"
                className="h-20 flex-1 text-xl"
                onClick={(e) => {
                  e.currentTarget.blur()
                  increment(step)
                }}
              >
                {t('hunt.counter.plus', { n: step })}
              </Button>
            </div>
            <label className="flex w-full items-center justify-end gap-2 text-sm text-muted">
              {t('hunt.counter.stepLabel')}
              <select
                value={step}
                onChange={(e) => {
                  setStep(Number(e.target.value))
                  // Sem foco no seletor: as teclas do atalho não mudam o valor.
                  e.currentTarget.blur()
                }}
                className="rounded-lg border border-line bg-surface px-2 py-1 text-sm font-semibold"
              >
                {HUNT_STEPS.map((n) => (
                  <option key={n} value={n}>
                    {n}
                  </option>
                ))}
              </select>
            </label>
            <div className="flex w-full flex-wrap gap-2">
              <Button
                variant="secondary"
                size="lg"
                icon="auto_awesome"
                className="flex-1 border-ok! text-ok!"
                disabled={busy}
                onClick={() => void finish(true)}
              >
                {t('hunt.counter.found')}
              </Button>
              <Button variant="ghost" size="lg" icon="flag" disabled={busy} onClick={() => setConfirm('giveUp')}>
                {t('hunt.counter.giveUp')}
              </Button>
            </div>
            {!mobile && (
              <p className="text-[12.5px] text-muted">
                {t(globalKeys ? 'hunt.counter.hotkeysHintGlobal' : 'hunt.counter.hotkeysHint', { inc: formatCombo(hotkeys.increment, keyNames), dec: formatCombo(hotkeys.decrement, keyNames) })}
              </p>
            )}
          </>
        )}
      </Card>

      <Card className="flex flex-col gap-2">
        <label htmlFor="hunt-notes" className="font-bold">
          {t('hunt.counter.notes')}
        </label>
        <textarea
          id="hunt-notes"
          className="min-h-24 w-full rounded-lg border border-line bg-bg px-3 py-2 text-[15px] text-ink outline-none placeholder:text-muted/70 focus:border-accent"
          maxLength={500}
          value={notes}
          placeholder={t('hunt.counter.notesPlaceholder')}
          onChange={(e) => setNotes(e.target.value)}
          onBlur={() => void saveNotes()}
        />
      </Card>

      <div className="flex justify-end">
        <Button variant="outlineDanger" size="sm" icon="delete" disabled={busy} onClick={() => setConfirm('delete')}>
          {t('hunt.counter.delete')}
        </Button>
      </div>

      {confirm === 'giveUp' && (
        <Modal
          title={t('hunt.counter.giveUpTitle')}
          actions={
            <>
              <Button variant="secondary" onClick={() => setConfirm(null)}>
                {t('common.cancel')}
              </Button>
              <Button variant="danger" disabled={busy} onClick={() => void finish(false)}>
                {t('hunt.counter.giveUp')}
              </Button>
            </>
          }
        >
          {t('hunt.counter.giveUpText')}
        </Modal>
      )}
      {confirm === 'delete' && (
        <Modal
          title={t('hunt.counter.deleteTitle')}
          actions={
            <>
              <Button variant="secondary" onClick={() => setConfirm(null)}>
                {t('common.cancel')}
              </Button>
              <Button variant="danger" disabled={busy} onClick={() => void remove()}>
                {t('common.delete')}
              </Button>
            </>
          }
        >
          {t('hunt.counter.deleteText')}
        </Modal>
      )}
    </div>
  )
}

function Stat({ label, value }: { label: string; value: string }) {
  return (
    <div className="flex flex-col items-center gap-1 rounded-lg bg-surface2 px-2 py-2.5">
      <span className="text-[12px] text-muted">{label}</span>
      <span className="text-[17px] font-bold tabular-nums">{value}</span>
    </div>
  )
}
