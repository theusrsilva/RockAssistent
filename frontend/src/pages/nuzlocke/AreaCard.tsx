import { useMemo, useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Link } from 'react-router'
import { SearchSelect } from '../../components/SearchSelect'
import { Badge, Button, Callout, Card, Field, Modal, Segmented, Toggle, inputClass, useToast } from '../../components/ui'
import { api, spriteUrl } from '../../lib/api'
import { titleCase, versionName } from '../../lib/dex'
import type { Option } from '../../lib/types'
import { areaUsed, nuzlockeErrorMessage, type GameArea, type MonInfo, type MonStatus, type RunView, type SpeciesEncounterOption } from './nuzlocke'
import { CardTitle, monName, type Mutate } from './parts'

type CatchStatus = Exclude<MonStatus, 'dead'>
const CATCH_STATUSES: CatchStatus[] = ['party', 'box', 'failed', 'missed']

/** Área atual: escolher a área, ver os encontros e registrar o que aconteceu ("Capturei", outro ou sem encontro). */
export default function AreaCard({
  view,
  areas,
  names,
  speciesOptions,
  readOnly,
  mutate,
}: {
  view: RunView
  /** Áreas do jogo já em ordem de progressão. */
  areas: GameArea[]
  names: Map<number, string>
  speciesOptions: Option[]
  readOnly: boolean
  mutate: Mutate
}) {
  const { t } = useTranslation()
  const toast = useToast()
  const [catching, setCatching] = useState<{ species: number | null; level: number } | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [confirmNone, setConfirmNone] = useState(false)
  const area = view.currentArea
  const lostHere = area ? view.lost.filter((m) => m.areaId === area.id) : []

  /** "Capturei afinal": o encontro perdido vira captura (o servidor confere as cláusulas de novo). */
  async function recover(mon: MonInfo, status: 'party' | 'box') {
    setError(null)
    try {
      await mutate(api.put<RunView>(`/api/nuzlocke/${runId}/mon/${mon.id}`, { status }))
      toast(t('nuzlocke.area.saved'))
    } catch (e) {
      setError(nuzlockeErrorMessage(t, e))
    }
  }
  const used = area ? areaUsed(view, area.id) : false
  const runId = view.run.id

  const areaOptions = useMemo(() => areas.map((a, i) => ({ value: i, text: a.name })), [areas])
  const areaIndex = area ? areas.findIndex((a) => a.id === area.id) : -1

  async function changeArea(index: number | null) {
    if (index == null || !areas[index] || areas[index].id === area?.id) return
    setError(null)
    try {
      await mutate(api.put<RunView>(`/api/nuzlocke/${runId}/area`, { areaId: areas[index].id }))
    } catch (e) {
      setError(nuzlockeErrorMessage(t, e))
    }
  }

  async function noEncounter() {
    setConfirmNone(false)
    if (!area) return
    setError(null)
    try {
      await mutate(api.post<RunView>(`/api/nuzlocke/${runId}/encounter`, { areaId: area.id, species: 0, form: 0, level: 0, status: 'failed', shiny: false }))
      toast(t('nuzlocke.area.noEncounterDone'))
    } catch (e) {
      setError(nuzlockeErrorMessage(t, e))
    }
  }

  return (
    <Card className="flex flex-col gap-3">
      <CardTitle icon="location_on" extra={used && <Badge tone="amber" icon="flag">{t('nuzlocke.area.used')}</Badge>}>
        {t('nuzlocke.area.title')}
      </CardTitle>
      {areas.length === 0 && !area ? (
        <p className="text-sm text-muted">{t('nuzlocke.area.noAreas')}</p>
      ) : (
        <Field label={t('nuzlocke.area.change')}>
          <SearchSelect options={areaOptions} value={areaIndex >= 0 ? areaIndex : null} onChange={(i) => void changeArea(i)} placeholder={t('nuzlocke.area.search')} />
        </Field>
      )}
      {error && (
        <Callout tone="red" icon="error">
          {error}
        </Callout>
      )}

      {area && (
        <>
          <div className="text-sm font-bold">{t('nuzlocke.area.encounters')}</div>
          {view.areaEncounters.length === 0 ? (
            <p className="text-sm text-muted">{t('nuzlocke.area.noEncounters')}</p>
          ) : (
            <ul className="flex flex-col gap-1.5">
              {view.areaEncounters.map((e) => (
                <EncounterRow key={`${e.species}-${e.form}`} e={e} showVersions={!view.run.version} readOnly={readOnly} onCatch={() => setCatching({ species: e.species, level: e.minLevel })} />
              ))}
            </ul>
          )}
          {!readOnly && (
            <div className="flex flex-wrap gap-2">
              <Button variant="secondary" size="sm" icon="add" onClick={() => setCatching({ species: null, level: 5 })}>
                {t('nuzlocke.area.other')}
              </Button>
              <Button variant="ghost" size="sm" icon="block" disabled={used && view.run.rules.firstEncounterOnly} onClick={() => setConfirmNone(true)}>
                {t('nuzlocke.area.noEncounter')}
              </Button>
            </div>
          )}
          {lostHere.length > 0 && (
            <details className="text-sm">
              <summary className="cursor-pointer font-semibold text-muted">{t('nuzlocke.area.lostTitle', { count: lostHere.length })}</summary>
              <ul className="mt-1.5 flex flex-col gap-1.5">
                {lostHere.map((m) => (
                  <li key={m.id} className="flex flex-wrap items-center gap-2">
                    {m.species > 0 ? (
                      <img src={spriteUrl(m.species, { shiny: m.shiny })} alt="" className="pixelated h-9 w-9 opacity-70" loading="lazy" />
                    ) : (
                      <span aria-hidden className="icon flex h-9 w-9 items-center justify-center text-muted">
                        block
                      </span>
                    )}
                    <span className="min-w-0 flex-1 truncate">
                      <span className="font-semibold">{m.species > 0 ? monName(m, names) : t('nuzlocke.area.noEncounter')}</span>{' '}
                      <span className="text-muted">· {t(`nuzlocke.area.form.statuses.${m.status}`)}</span>
                    </span>
                    {!readOnly && m.species > 0 && (
                      <span className="flex items-center gap-1">
                        <span className="text-[12px] text-muted">{t('nuzlocke.area.caughtAfterAll')}:</span>
                        <Button size="sm" variant="secondary" disabled={view.party.length >= 6} onClick={() => void recover(m, 'party')}>
                          {t('nuzlocke.area.toParty')}
                        </Button>
                        <Button size="sm" variant="secondary" onClick={() => void recover(m, 'box')}>
                          {t('nuzlocke.area.toBox')}
                        </Button>
                      </span>
                    )}
                  </li>
                ))}
              </ul>
            </details>
          )}
        </>
      )}

      {confirmNone && (
        <Modal
          title={t('nuzlocke.area.noEncounterConfirm')}
          actions={
            <>
              <Button variant="secondary" onClick={() => setConfirmNone(false)}>
                {t('common.cancel')}
              </Button>
              <Button icon="block" onClick={() => void noEncounter()}>
                {t('nuzlocke.area.noEncounter')}
              </Button>
            </>
          }
        >
          {t('nuzlocke.area.noEncounterConfirmText')}
        </Modal>
      )}

      {catching && area && (
        <CatchDialog
          view={view}
          areaId={area.id}
          initial={catching}
          names={names}
          speciesOptions={speciesOptions}
          mutate={mutate}
          onClose={() => setCatching(null)}
        />
      )}
    </Card>
  )
}

function EncounterRow({ e, showVersions, readOnly, onCatch }: { e: SpeciesEncounterOption; showVersions: boolean; readOnly: boolean; onCatch: () => void }) {
  const { t } = useTranslation()
  const method = (m: string) => t(`dex.method.${m}`, { defaultValue: titleCase(m) })
  return (
    <li className="flex items-center gap-2 rounded-lg bg-surface2 px-2 py-1.5">
      <Link to={`/pokedex/${e.species}`} className="shrink-0">
        <img src={spriteUrl(e.species)} alt="" className="pixelated h-11 w-11" loading="lazy" />
      </Link>
      <div className="min-w-0 flex-1 text-[12.5px]">
        <div className="font-bold text-ink">{e.name}</div>
        <div className="text-muted">
          {e.minLevel === e.maxLevel ? t('dex.encounters.level', { level: e.minLevel }) : t('dex.encounters.levels', { min: e.minLevel, max: e.maxLevel })}
          {' · '}
          {e.methods.map(method).join(', ')}
        </div>
        {showVersions && e.versions.length > 0 && (
          <div className="mt-0.5 flex flex-wrap gap-1">
            {e.versions.map((v) => (
              <Badge key={v}>{versionName(v)}</Badge>
            ))}
          </div>
        )}
      </div>
      {!readOnly && (
        <Button size="sm" icon="radio_button_checked" onClick={onCatch}>
          {t('nuzlocke.area.caught')}
        </Button>
      )}
    </li>
  )
}

/** Mini formulário do encontro: espécie (quando não veio da lista), apelido, nível, shiny e o que aconteceu. */
function CatchDialog({
  view,
  areaId,
  initial,
  names,
  speciesOptions,
  mutate,
  onClose,
}: {
  view: RunView
  areaId: string
  initial: { species: number | null; level: number }
  names: Map<number, string>
  speciesOptions: Option[]
  mutate: Mutate
  onClose: () => void
}) {
  const { t } = useTranslation()
  const toast = useToast()
  const [species, setSpecies] = useState<number | null>(initial.species)
  const [nickname, setNickname] = useState('')
  const [level, setLevel] = useState(String(initial.level || 5))
  const [shiny, setShiny] = useState(false)
  const [status, setStatus] = useState<CatchStatus>(view.party.length >= 6 ? 'box' : 'party')
  const [busy, setBusy] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const caught = status === 'party' || status === 'box'
  const options = useMemo(() => speciesOptions.map((s) => ({ value: s.value, text: `#${s.value} ${s.text}` })), [speciesOptions])

  async function save() {
    if (!species) return
    setBusy(true)
    setError(null)
    try {
      await mutate(
        api.post<RunView>(`/api/nuzlocke/${view.run.id}/encounter`, {
          areaId,
          species,
          form: 0,
          nickname: nickname.trim() || null,
          level: Number(level) || 0,
          status,
          shiny,
        }),
      )
      toast(t('nuzlocke.area.saved'))
      onClose()
    } catch (e) {
      setError(nuzlockeErrorMessage(t, e))
      setBusy(false)
    }
  }

  return (
    <Modal
      title={t('nuzlocke.area.form.title')}
      actions={
        <>
          <Button variant="secondary" onClick={onClose}>
            {t('common.cancel')}
          </Button>
          <Button icon="check" disabled={busy || !species} onClick={() => void save()}>
            {t('nuzlocke.area.form.save')}
          </Button>
        </>
      }
    >
      <div className="flex flex-col gap-3">
        {initial.species ? (
          <div className="flex items-center gap-2">
            <img src={spriteUrl(initial.species, { shiny })} alt="" className="pixelated h-14 w-14" />
            <span className="font-bold">{names.get(initial.species) ?? `#${initial.species}`}</span>
          </div>
        ) : (
          <Field label={t('nuzlocke.area.form.species')}>
            <SearchSelect
              options={options}
              value={species}
              onChange={setSpecies}
              placeholder={t('nuzlocke.area.form.searchSpecies')}
              renderIcon={(v) => <img src={spriteUrl(v)} alt="" className="pixelated h-8 w-8" loading="lazy" />}
            />
          </Field>
        )}
        <div className="flex flex-col gap-1.5 text-sm">
          <span className="font-bold">{t('nuzlocke.area.form.status')}</span>
          <Segmented
            value={status}
            onChange={setStatus}
            options={CATCH_STATUSES.map((s) => ({ value: s, label: t(`nuzlocke.area.form.statuses.${s}`) }))}
            className="w-full flex-wrap"
          />
          <span className="text-[12.5px] text-muted">{t(`nuzlocke.area.form.statusHelp.${status}`)}</span>
        </div>
        {caught && (
          <Field label={view.run.rules.nicknameRequired ? t('nuzlocke.area.form.nickname') : `${t('nuzlocke.area.form.nickname')} (${t('common.optional')})`}>
            <input className={inputClass} value={nickname} maxLength={12} onChange={(e) => setNickname(e.target.value)} />
          </Field>
        )}
        <Field label={t('nuzlocke.area.form.level')}>
          <input className={inputClass} type="number" min={1} max={100} value={level} onChange={(e) => setLevel(e.target.value)} />
        </Field>
        <Toggle small on={shiny} onChange={setShiny} label={t('nuzlocke.area.form.shiny')} />
        {error && (
          <Callout tone="red" icon="error">
            {error}
          </Callout>
        )}
      </div>
    </Modal>
  )
}
