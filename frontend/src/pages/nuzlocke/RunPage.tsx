import { useMemo, useState } from 'react'
import { useTranslation } from 'react-i18next'
import { useNavigate, useParams } from 'react-router'
import { Badge, Button, Callout, Card, Modal, PageTitle, Segmented } from '../../components/ui'
import { api, ApiError, spriteUrl } from '../../lib/api'
import { useApiGet } from '../../lib/dex'
import { useSpeciesOptions } from '../../lib/hunts'
import { BackLink } from '../dex/tabs/parts'
import AreaCard from './AreaCard'
import BossCard from './BossCard'
import { runGameLabel, statusTone } from '../../components/RunCard'
import TeamCard from './TeamCard'
import TrainersCard from './TrainersCard'
import { areaStates, formatRules, isFinished, nuzlockeErrorMessage, runStats, sortAreas, useRun, type GameArea, type RunView } from './nuzlocke'
import { CardTitle, monName, type Mutate } from './parts'
import RegionMap from './RegionMap'
import { StarterPicker, starterName, useStarters } from './StarterPicker'
import type { MapGraph } from './legacyRegionMap'
import LegacyRegionMap from './LegacyRegionMap'
import { isRegionMap, type RegionMapData } from './regionMap'
import { gameVersionByKey } from '../../lib/gameVersions'

type Tab = 'run' | 'graveyard' | 'stats'
type Confirm = 'champion' | 'wipe' | 'delete'

/** Tela da run: chefe, área e equipe lado a lado (empilhados no celular); abas de cemitério e estatísticas. */
export default function RunPage() {
  const { id = '' } = useParams()
  const { t } = useTranslation()
  const { view, mutate, error: loadError } = useRun(id)

  if (loadError)
    return (
      <div className="flex flex-col gap-4">
        <BackLink to="/nuzlocke">{t('nuzlocke.back')}</BackLink>
        <Callout tone="red" icon="error">
          {loadError instanceof ApiError && loadError.status === 404 ? t('nuzlocke.run.notFound') : t('nuzlocke.loadError')}
        </Callout>
      </div>
    )
  if (!view) return <p className="text-sm text-muted">{t('common.loading')}</p>
  return <Run view={view} mutate={mutate} />
}

function Run({ view, mutate }: { view: RunView; mutate: Mutate }) {
  const { t, i18n } = useTranslation()
  const navigate = useNavigate()
  const [tab, setTab] = useState<Tab>('run')
  const [confirm, setConfirm] = useState<Confirm | null>(null)
  const [pickStarter, setPickStarter] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const run = view.run
  const readOnly = isFinished(run.status)

  const speciesOptions = useSpeciesOptions()
  const names = useMemo(() => new Map((speciesOptions ?? []).map((s) => [s.value, s.text])), [speciesOptions])
  const areasData = useApiGet<GameArea[]>(`/api/data/${run.gameKey}/areas`)
  const areas = useMemo(() => sortAreas(areasData.data ?? []), [areasData.data])
  const mapData = useApiGet<RegionMapData | MapGraph>(`/api/data/maps/${gameVersionByKey(run.gameKey)?.region ?? 'kanto'}`)
  const states = useMemo(() => areaStates(view), [view])
  const areaName = useMemo(() => new Map(areas.map((a) => [a.id, a.name])), [areas])
  const starters = useStarters(run.gameKey)
  const starter = starters.find((s) => s.id === run.starter)

  /** Ação simples do cabeçalho ou do chefe: o erro aparece no topo. */
  const act = async (request: () => Promise<RunView>) => {
    setError(null)
    try {
      await mutate(request())
    } catch (e) {
      setError(nuzlockeErrorMessage(t, e))
    }
  }

  /** Clique no mapa: troca a área atual (run terminada: sem clique). */
  const selectArea = readOnly
    ? undefined
    : (areaId: string) => {
        if (areaId !== view.currentArea?.id) void act(() => api.put<RunView>(`/api/nuzlocke/${run.id}/area`, { areaId }))
      }

  async function confirmAction(kind: Confirm) {
    setConfirm(null)
    if (kind === 'delete') {
      try {
        await api.delete(`/api/nuzlocke/${run.id}`)
        navigate('/nuzlocke', { replace: true })
      } catch (e) {
        setError(nuzlockeErrorMessage(t, e))
      }
      return
    }
    await act(() => api.post<RunView>(`/api/nuzlocke/${run.id}/finish`, { status: kind }))
  }

  const date = (iso: string) => new Date(iso).toLocaleDateString(i18n.language)

  return (
    <div className="flex flex-col gap-4">
      <BackLink to="/nuzlocke">{t('nuzlocke.back')}</BackLink>
      <PageTitle
        actions={
          <div className="flex flex-wrap gap-2">
            {!readOnly && (
              <>
                {run.status === 'paused' ? (
                  <Button variant="secondary" icon="play_arrow" onClick={() => void act(() => api.put<RunView>(`/api/nuzlocke/${run.id}`, { status: 'active' }))}>
                    {t('nuzlocke.run.resume')}
                  </Button>
                ) : (
                  <Button variant="secondary" icon="pause" onClick={() => void act(() => api.put<RunView>(`/api/nuzlocke/${run.id}`, { status: 'paused' }))}>
                    {t('nuzlocke.run.pause')}
                  </Button>
                )}
                <Button icon="emoji_events" onClick={() => setConfirm('champion')}>
                  {t('nuzlocke.run.champion')}
                </Button>
                <Button variant="outlineDanger" icon="heart_broken" onClick={() => setConfirm('wipe')}>
                  {t('nuzlocke.run.wipe')}
                </Button>
              </>
            )}
            <Button variant="ghost" icon="delete" onClick={() => setConfirm('delete')}>
              {t('nuzlocke.run.delete')}
            </Button>
          </div>
        }
      >
        {run.name}
      </PageTitle>
      <div className="-mt-2 flex flex-wrap items-center gap-1.5 text-sm text-muted">
        <span>{runGameLabel(run.gameKey, run.version)}</span>
        <Badge tone={statusTone[run.status]}>{t(`nuzlocke.status.${run.status}`)}</Badge>
        {run.endedAt && <span>{t('nuzlocke.run.finishedOn', { date: date(run.endedAt) })}</span>}
        {starters.length > 1 && (
          <span className="flex items-center gap-1">
            <span>·</span>
            {starter && <img src={spriteUrl(starter.species)} alt="" className="pixelated -my-2 h-8 w-8" />}
            <span>{starter ? t('nuzlocke.starter.yours', { name: starterName(starter, names) }) : t('nuzlocke.starter.unknown')}</span>
            {!readOnly && (
              <Button variant="ghost" size="sm" icon="edit" onClick={() => setPickStarter(true)}>
                {t('nuzlocke.starter.change')}
              </Button>
            )}
          </span>
        )}
      </div>
      <div className="flex flex-wrap items-center gap-1.5 text-[12.5px]">
        <span className="font-semibold text-muted">{t('nuzlocke.rules.title')}:</span>
        {formatRules(t, run.rules).map((r, i) => (
          <Badge key={i}>{r}</Badge>
        ))}
      </div>

      {run.status === 'paused' && (
        <Callout tone="amber" icon="pause_circle">
          {t('nuzlocke.run.pausedNote')}
        </Callout>
      )}
      {error && (
        <Callout tone="red" icon="error">
          {error}
        </Callout>
      )}

      <Segmented
        value={tab}
        onChange={setTab}
        options={[
          { value: 'run', label: t('nuzlocke.run.tabs.run') },
          { value: 'graveyard', label: `${t('nuzlocke.run.tabs.graveyard')} (${view.graveyard.length})` },
          { value: 'stats', label: t('nuzlocke.run.tabs.stats') },
        ]}
        className="w-full sm:w-fit"
      />

      {tab === 'run' && (
        <>
          {mapData.data && (isRegionMap(mapData.data) ? mapData.data.places.length > 0 : mapData.data.nodes.length > 0) && (
            <Card className="flex flex-col gap-2 p-3! sm:p-4!">
              <CardTitle icon="map">{t('nuzlocke.map.title')}</CardTitle>
              {isRegionMap(mapData.data) ? (
                <RegionMap map={mapData.data} game={run.gameKey} areas={areas} states={states} current={view.currentArea?.id ?? null} onSelect={selectArea} />
              ) : (
                // TODO(mapas): formato antigo, até as 11 regiões passarem para o metrô.
                <LegacyRegionMap graph={mapData.data} game={run.gameKey} areas={areas} states={states} current={view.currentArea?.id ?? null} onSelect={selectArea} />
              )}
            </Card>
          )}
          <div className="grid grid-cols-1 items-start gap-4 lg:grid-cols-3">
            <div className="flex flex-col gap-4">
              <BossCard view={view} names={names} readOnly={readOnly} onDefeat={(bossId) => void act(() => api.post<RunView>(`/api/nuzlocke/${run.id}/boss/${bossId}/defeated`))} />
            </div>
            <div className="flex flex-col gap-4">
              <AreaCard view={view} areas={areas} names={names} speciesOptions={speciesOptions ?? []} readOnly={readOnly} mutate={mutate} />
              <TrainersCard view={view} names={names} />
            </div>
            <div className="flex flex-col gap-4">
              <TeamCard view={view} names={names} speciesOptions={speciesOptions ?? []} readOnly={readOnly} mutate={mutate} />
            </div>
          </div>
        </>
      )}

      {tab === 'graveyard' && (
        <Card className="flex flex-col gap-2">
          {view.graveyard.length === 0 ? (
            <p className="text-sm text-muted">{t('nuzlocke.graveyard.empty')}</p>
          ) : (
            <ul className="grid grid-cols-1 gap-2 sm:grid-cols-2">
              {view.graveyard.map((m) => (
                <li key={m.id} className="flex items-center gap-3 rounded-lg bg-surface2 px-3 py-2">
                  <img src={spriteUrl(m.species, { shiny: m.shiny })} alt="" className="pixelated h-12 w-12 grayscale" loading="lazy" />
                  <div className="min-w-0 flex-1 text-[12.5px]">
                    <div className="text-sm font-bold">
                      {monName(m, names)} <span className="font-normal text-muted">{t('nuzlocke.boss.level', { level: m.level })}</span>
                    </div>
                    <div className={m.deathCause ? 'text-ink' : 'text-muted'}>{m.deathCause || t('nuzlocke.graveyard.noCause')}</div>
                    <div className="text-muted">
                      {m.diedAt && t('nuzlocke.graveyard.died', { date: date(m.diedAt) })}
                      {m.diedAt && ' · '}
                      {t('nuzlocke.graveyard.area', { area: areaName.get(m.areaId) ?? m.areaId })}
                    </div>
                  </div>
                </li>
              ))}
            </ul>
          )}
        </Card>
      )}

      {tab === 'stats' && <Stats view={view} />}

      {pickStarter && (
        <Modal
          title={t('nuzlocke.starter.label')}
          actions={
            <Button variant="secondary" onClick={() => setPickStarter(false)}>
              {t('common.cancel')}
            </Button>
          }
        >
          <div className="flex flex-col gap-2 text-sm">
            <StarterPicker
              starters={starters}
              value={run.starter}
              names={names}
              allowNone
              onChange={(s) => {
                setPickStarter(false)
                void act(() => api.put<RunView>(`/api/nuzlocke/${run.id}`, { starter: s ?? '' }))
              }}
            />
            <span className="text-[12.5px] text-muted">{t('nuzlocke.starter.help')}</span>
          </div>
        </Modal>
      )}

      {confirm && (
        <Modal
          title={t(confirm === 'delete' ? 'nuzlocke.run.confirmDelete' : confirm === 'champion' ? 'nuzlocke.run.confirmChampion' : 'nuzlocke.run.confirmWipe')}
          actions={
            <>
              <Button variant="secondary" onClick={() => setConfirm(null)}>
                {t('common.cancel')}
              </Button>
              <Button variant={confirm === 'champion' ? 'primary' : 'danger'} onClick={() => void confirmAction(confirm)}>
                {t(confirm === 'delete' ? 'common.delete' : confirm === 'champion' ? 'nuzlocke.run.champion' : 'nuzlocke.run.wipe')}
              </Button>
            </>
          }
        >
          {t(confirm === 'delete' ? 'nuzlocke.run.confirmDeleteText' : confirm === 'champion' ? 'nuzlocke.run.confirmChampionText' : 'nuzlocke.run.confirmWipeText')}
        </Modal>
      )}
    </div>
  )
}

function Stats({ view }: { view: RunView }) {
  const { t } = useTranslation()
  const s = runStats(view)
  const tiles: [string, number, string][] = [
    ['caught', s.caught, 'radio_button_checked'],
    ['deaths', s.deaths, 'skull'],
    ['bosses', s.bosses, 'emoji_events'],
    ['areas', s.areas, 'map'],
    ['shinies', s.shinies, 'auto_awesome'],
    ['lost', s.lost, 'block'],
  ]
  return (
    <div className="grid grid-cols-2 gap-2.5 sm:grid-cols-3">
      {tiles.map(([key, value, icon]) => (
        <Card key={key} className="flex flex-col gap-1 p-4!">
          <span className="flex items-center gap-1.5 text-[12.5px] font-semibold text-muted">
            <span aria-hidden className="icon text-[17px]">
              {icon}
            </span>
            {t(`nuzlocke.stats.${key}`)}
          </span>
          <span className="font-display text-3xl leading-none font-bold tabular-nums">{value}</span>
        </Card>
      ))}
    </div>
  )
}
