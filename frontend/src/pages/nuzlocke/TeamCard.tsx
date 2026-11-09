import { useMemo, useState } from 'react'
import { useTranslation } from 'react-i18next'
import type { TFunction } from 'i18next'
import { Link } from 'react-router'
import { SearchSelect } from '../../components/SearchSelect'
import { Badge, Button, Callout, Card, Field, IconButton, Modal, inputClass, useToast } from '../../components/ui'
import { api, spriteUrl } from '../../lib/api'
import { titleCase, useApiGet } from '../../lib/dex'
import type { Option } from '../../lib/types'
import { TypeBadge } from '../dex/TypeBadge'
import { conditionParts, defaultMoves, evolveHow, evolveOptions, formSprite, type EvolveHow, nuzlockeErrorMessage, overCap, type EvolveOption, type LearnsetMove, type MonInfo, type MonLearnset, type RunView } from './nuzlocke'
import { CardTitle, monName, type Mutate } from './parts'

/**
 * Learnset do Pokémon da run. O caminho leva espécie e nível só para buscar de novo quando eles mudam (o servidor
 * ignora o parâmetro).
 */
const useLearnset = (runId: string, mon: MonInfo, enabled: boolean) =>
  useApiGet<MonLearnset>(enabled ? `/api/nuzlocke/${runId}/mon/${mon.id}/learnset?v=${mon.species}-${mon.level}` : null)

/** Equipe e caixa: nível, golpes, próximos golpes, evolução, e as ações (caixa, morte, editar). */
export default function TeamCard({
  view,
  names,
  speciesOptions,
  readOnly,
  mutate,
}: {
  view: RunView
  names: Map<number, string>
  speciesOptions: Option[]
  readOnly: boolean
  mutate: Mutate
}) {
  const { t } = useTranslation()
  const [error, setError] = useState<string | null>(null)
  const [dying, setDying] = useState<MonInfo | null>(null)
  const [editing, setEditing] = useState<MonInfo | null>(null)
  /** Pokémon da caixa com as opções de evoluir abertas (o learnset só é buscado ao abrir). */
  const [evolvingBox, setEvolvingBox] = useState<string | null>(null)
  /** Pokémon com mudança de nível em voo: os botões −/+ esperam a resposta (dois cliques não mandam o mesmo valor). */
  const [leveling, setLeveling] = useState<Set<string>>(new Set())
  const over = new Set(overCap(view.party, view.levelCap).map((m) => m.id))

  const toast = useToast()
  /** Evoluir: espécie e forma mudam (apelido, nível, golpes e situação ficam). */
  const evolve = async (mon: MonInfo, option: EvolveOption) => {
    if (await update(mon, { species: option.species, form: option.form })) toast(t('nuzlocke.evolve.done', { from: monName(mon, names), name: option.name }))
  }

  const update = async (mon: MonInfo, body: Record<string, unknown>) => {
    setError(null)
    try {
      await mutate(api.put<RunView>(`/api/nuzlocke/${view.run.id}/mon/${mon.id}`, body))
      return true
    } catch (e) {
      setError(nuzlockeErrorMessage(t, e))
      return false
    }
  }

  return (
    <Card className="flex flex-col gap-3">
      <CardTitle icon="radio_button_checked" extra={<Badge>{view.party.length}/6</Badge>}>
        {t('nuzlocke.team.title')}
      </CardTitle>
      {error && (
        <Callout tone="red" icon="error">
          {error}
        </Callout>
      )}
      {view.party.length === 0 ? (
        <p className="text-sm text-muted">{t('nuzlocke.team.empty')}</p>
      ) : (
        <ul className="flex flex-col gap-2">
          {view.party.map((m) => (
            <MonRow
              key={m.id}
              runId={view.run.id}
              mon={m}
              names={names}
              cap={over.has(m.id) ? view.levelCap : null}
              readOnly={readOnly}
              levelBusy={leveling.has(m.id)}
              onLevel={async (level) => {
                setLeveling((s) => new Set(s).add(m.id))
                await update(m, { level })
                setLeveling((s) => {
                  const next = new Set(s)
                  next.delete(m.id)
                  return next
                })
              }}
              onBox={() => void update(m, { status: 'box' })}
              onDie={() => setDying(m)}
              onEdit={() => setEditing(m)}
              onEvolve={(o) => void evolve(m, o)}
            />
          ))}
        </ul>
      )}

      <details className="text-sm" open={view.box.length > 0 && view.box.length <= 6}>
        <summary className="cursor-pointer font-semibold text-muted">
          {t('nuzlocke.team.boxTitle')} ({view.box.length})
        </summary>
        {view.box.length === 0 ? (
          <p className="mt-1.5 text-muted">{t('nuzlocke.team.boxEmpty')}</p>
        ) : (
          <ul className="mt-1.5 flex flex-col gap-1">
            {view.box.map((m) => (
              <li key={m.id} className="flex flex-col gap-1">
                <div className="flex items-center gap-2">
                  <img src={formSprite(m)} alt="" className="pixelated h-9 w-9" loading="lazy" />
                  <span className="min-w-0 flex-1 truncate font-semibold">
                    {monName(m, names)} <span className="font-normal text-muted">{t('nuzlocke.boss.level', { level: m.level })}</span>
                  </span>
                  {!readOnly && (
                    <>
                      <IconButton
                        name="arrow_upward"
                        label={t('nuzlocke.evolve.action')}
                        className="h-8 w-8"
                        aria-expanded={evolvingBox === m.id}
                        onClick={() => setEvolvingBox((id) => (id === m.id ? null : m.id))}
                      />
                      <IconButton name="edit" label={t('nuzlocke.team.edit')} className="h-8 w-8" onClick={() => setEditing(m)} />
                      <Button size="sm" variant="secondary" disabled={view.party.length >= 6} onClick={() => void update(m, { status: 'party' })}>
                        {t('nuzlocke.team.toParty')}
                      </Button>
                    </>
                  )}
                </div>
                {evolvingBox === m.id && !readOnly && <BoxEvolve runId={view.run.id} mon={m} readOnly={readOnly} onEvolve={(o) => void evolve(m, o)} />}
              </li>
            ))}
          </ul>
        )}
      </details>

      {dying && <DeathDialog mon={dying} names={names} onClose={() => setDying(null)} onConfirm={(cause) => update(dying, { status: 'dead', deathCause: cause })} />}
      {editing && <EditDialog runId={view.run.id} mon={editing} names={names} speciesOptions={speciesOptions} mutate={mutate} onClose={() => setEditing(null)} />}
    </Card>
  )
}

function MonRow({
  runId,
  mon,
  names,
  cap,
  readOnly,
  levelBusy,
  onLevel,
  onBox,
  onDie,
  onEdit,
  onEvolve,
}: {
  runId: string
  mon: MonInfo
  names: Map<number, string>
  /** Preenchido quando o Pokémon passou do cap. */
  cap: number | null
  readOnly: boolean
  levelBusy: boolean
  onLevel: (level: number) => void
  onBox: () => void
  onDie: () => void
  onEdit: () => void
  onEvolve: (option: EvolveOption) => void
}) {
  const { t } = useTranslation()
  const [open, setOpen] = useState(false)
  const learnset = useLearnset(runId, mon, true)
  const known = learnset.data?.known ?? []
  const byMove = new Map(known.map((k) => [k.move, k]))
  const current = mon.moves.length > 0 ? mon.moves : defaultMoves(known)

  return (
    <li className={`flex flex-col gap-2 rounded-lg border px-2.5 py-2 ${cap != null ? 'border-bad bg-bad-soft' : 'border-line bg-surface2'}`}>
      <div className="flex items-center gap-2">
        <Link to={`/pokedex/${mon.species}`} className="shrink-0">
          <img src={formSprite(mon)} alt="" className="pixelated h-12 w-12" loading="lazy" />
        </Link>
        <div className="min-w-0 flex-1">
          <div className="truncate font-bold">
            {monName(mon, names)}
            {mon.shiny && <span className="ml-1 text-warn">★</span>}
          </div>
          {mon.nickname && <div className="truncate text-[12px] text-muted">{names.get(mon.species) ?? `#${mon.species}`}</div>}
        </div>
        <div className="flex items-center gap-1">
          {!readOnly && <IconButton name="remove" label={t('nuzlocke.team.levelDown')} className="h-7 w-7" disabled={levelBusy || mon.level <= 1} onClick={() => onLevel(mon.level - 1)} />}
          <span className={`min-w-12 text-center text-sm font-bold tabular-nums ${cap != null ? 'text-bad' : ''}`}>{t('nuzlocke.boss.level', { level: mon.level })}</span>
          {!readOnly && <IconButton name="add" label={t('nuzlocke.team.levelUp')} className="h-7 w-7" disabled={levelBusy || mon.level >= 100} onClick={() => onLevel(mon.level + 1)} />}
        </div>
      </div>
      {cap != null && (
        <div className="flex items-center gap-1 text-[12.5px] font-bold text-bad">
          <span aria-hidden className="icon text-[16px]">
            warning
          </span>
          {t('nuzlocke.team.overCap', { cap })}
        </div>
      )}
      <div className="flex flex-col gap-1 text-[12.5px]">
        <span className="text-muted">{mon.moves.length > 0 ? t('nuzlocke.team.moves') : t('nuzlocke.team.probableMoves')}</span>
        {current.length === 0 ? (
          <span className="text-muted">{learnset.loading ? t('common.loading') : t('nuzlocke.team.noMoves')}</span>
        ) : (
          <div className="flex flex-wrap gap-1">
            {current.map((move) => (
              <MoveChip key={move} move={byMove.get(move)} identifier={move} />
            ))}
          </div>
        )}
      </div>
      {open && learnset.data && <Upcoming learnset={learnset.data} />}
      {learnset.data && <EvolveButtons options={evolveOptions(learnset.data.evolutions, mon.level)} disabled={readOnly} onEvolve={onEvolve} />}
      <div className="flex flex-wrap gap-1.5">
        <Button size="sm" variant="ghost" icon={open ? 'expand_less' : 'expand_more'} onClick={() => setOpen((o) => !o)}>
          {open ? t('nuzlocke.team.hideMoves') : t('nuzlocke.team.showMoves')}
        </Button>
        {!readOnly && (
          <>
            <Button size="sm" variant="secondary" icon="inventory_2" onClick={onBox}>
              {t('nuzlocke.team.toBox')}
            </Button>
            <Button size="sm" variant="secondary" icon="edit" onClick={onEdit}>
              {t('nuzlocke.team.edit')}
            </Button>
            <Button size="sm" variant="outlineDanger" icon="skull" onClick={onDie}>
              {t('nuzlocke.team.died')}
            </Button>
          </>
        )}
      </div>
    </li>
  )
}

/** Pokémon da caixa: busca o learnset só para os botões de evoluir. */
function BoxEvolve({ runId, mon, readOnly, onEvolve }: { runId: string; mon: MonInfo; readOnly: boolean; onEvolve: (option: EvolveOption) => void }) {
  const { t } = useTranslation()
  const learnset = useLearnset(runId, mon, true)
  if (learnset.loading) return <span className="text-[12px] text-muted">{t('common.loading')}</span>
  const options = learnset.data ? evolveOptions(learnset.data.evolutions, mon.level).filter((o) => o.ready) : []
  if (options.length === 0) return <span className="text-[12px] text-muted">{t('nuzlocke.evolve.none')}</span>
  return <EvolveButtons options={options} disabled={readOnly} onEvolve={onEvolve} />
}

/** Condição de evolução em texto curto, traduzido (nada de frase do PokeAPI em inglês). */
function describeHow(t: TFunction, how: EvolveHow): string {
  const parts = (text: string) =>
    conditionParts(text)
      .map((p) => t(`nuzlocke.evolve.parts.${p.key}`, p.values ?? {}))
      .join(', ')
  switch (how.kind) {
    case 'level':
      return t('nuzlocke.evolve.level', { level: how.level })
    case 'item':
      return t('nuzlocke.evolve.item', { item: how.item })
    case 'trade': {
      const detail = how.detail ? parts(how.detail) : ''
      return detail ? t('nuzlocke.evolve.tradeDetail', { detail }) : t('nuzlocke.evolve.trade')
    }
    case 'friendship':
      return t('nuzlocke.evolve.friendship')
    default:
      return parts(how.text) || t('nuzlocke.evolve.other')
  }
}

/**
 * Um botão por evolução. Por nível: só aparece quando o nível chegou (em destaque). Por item, troca, amizade etc.: sempre
 * disponível, com a condição escrita curta (o app não sabe se a pessoa tem a pedra ou fez a troca).
 */
function EvolveButtons({ options, disabled, onEvolve }: { options: EvolveOption[]; disabled: boolean; onEvolve: (option: EvolveOption) => void }) {
  const { t } = useTranslation()
  const shown = options.filter((o) => o.ready)
  if (shown.length === 0) return null
  const how = (o: EvolveOption) => (o.how.kind === 'level' ? null : describeHow(t, o.how))
  return (
    <div className="flex flex-wrap gap-1.5">
      {shown.map((o) => {
        const condition = how(o)
        return (
          <Button
            key={o.species}
            size="sm"
            variant={o.how.kind === 'level' ? 'primary' : 'secondary'}
            icon="arrow_upward"
            disabled={disabled}
            title={condition ? `${t('nuzlocke.evolve.to', { name: o.name })} (${condition})` : undefined}
            onClick={() => onEvolve(o)}
          >
            {t('nuzlocke.evolve.to', { name: o.name })}
            {condition && <span className="font-normal opacity-80"> · {condition}</span>}
          </Button>
        )
      })}
    </div>
  )
}

function MoveChip({ move, identifier, level }: { move?: LearnsetMove; identifier: string; level?: number }) {
  const { t } = useTranslation()
  return (
    <span className="inline-flex items-center gap-1 rounded-md border border-line bg-surface px-1.5 py-0.5">
      {level != null && level > 0 && <span className="text-muted tabular-nums">{t('nuzlocke.boss.level', { level })}</span>}
      <span className="font-semibold">{move?.name ?? titleCase(identifier)}</span>
      {move && <TypeBadge type={move.type} small />}
    </span>
  )
}

/** Próximos golpes por nível e o que muda em cada evolução. */
function Upcoming({ learnset }: { learnset: MonLearnset }) {
  const { t } = useTranslation()
  return (
    <div className="flex flex-col gap-2 text-[12.5px]">
      <div className="flex flex-col gap-1">
        <span className="text-muted">{t('nuzlocke.team.upcoming')}</span>
        {learnset.upcoming.length === 0 ? (
          <span className="text-muted">{t('nuzlocke.team.noUpcoming')}</span>
        ) : (
          <div className="flex flex-wrap gap-1">
            {learnset.upcoming.map((m) => (
              <MoveChip key={`${m.move}-${m.level}`} move={m} identifier={m.move} level={m.level} />
            ))}
          </div>
        )}
      </div>
      {learnset.evolutions.map((ev) => (
        <div key={ev.species} className="flex flex-col gap-1 rounded-md border border-dashed border-line p-1.5">
          <div className="flex items-center gap-1.5">
            <img src={formSprite(ev)} alt="" className="pixelated h-8 w-8" loading="lazy" />
            <span className="font-semibold">{t('nuzlocke.team.evolution', { name: ev.name })}</span>
            {ev.conditions.length > 0 && <span className="text-muted">({ev.conditions.map((c) => describeHow(t, evolveHow(c))).join('; ')})</span>}
          </div>
          {ev.onEvolve.length > 0 && (
            <div className="flex flex-wrap items-center gap-1">
              <span className="text-muted">{t('nuzlocke.team.onEvolve')}:</span>
              {ev.onEvolve.map((m) => (
                <MoveChip key={m.move} move={m} identifier={m.move} />
              ))}
            </div>
          )}
          {ev.upcoming.length > 0 && (
            <div className="flex flex-wrap gap-1">
              {ev.upcoming.map((m) => (
                <MoveChip key={`${m.move}-${m.level}`} move={m} identifier={m.move} level={m.level} />
              ))}
            </div>
          )}
        </div>
      ))}
    </div>
  )
}

function DeathDialog({ mon, names, onClose, onConfirm }: { mon: MonInfo; names: Map<number, string>; onClose: () => void; onConfirm: (cause: string) => Promise<boolean> }) {
  const { t } = useTranslation()
  const [cause, setCause] = useState('')
  const [busy, setBusy] = useState(false)
  return (
    <Modal
      title={t('nuzlocke.team.death.title', { name: monName(mon, names) })}
      actions={
        <>
          <Button variant="secondary" onClick={onClose}>
            {t('common.cancel')}
          </Button>
          <Button
            variant="danger"
            icon="skull"
            disabled={busy}
            onClick={async () => {
              setBusy(true)
              if (await onConfirm(cause.trim())) onClose()
              else setBusy(false)
            }}
          >
            {t('nuzlocke.team.death.confirm')}
          </Button>
        </>
      }
    >
      <div className="flex items-center gap-3">
        <img src={formSprite(mon)} alt="" className="pixelated h-14 w-14 grayscale" />
        <Field label={t('nuzlocke.team.death.cause')}>
          <input className={inputClass} value={cause} maxLength={200} placeholder={t('nuzlocke.team.death.causePlaceholder')} onChange={(e) => setCause(e.target.value)} />
        </Field>
      </div>
    </Modal>
  )
}

/** Editar: apelido, nível, espécie (evolução) e os golpes atuais (até 4 dos aprendidos por nível). */
function EditDialog({
  runId,
  mon,
  names,
  speciesOptions,
  mutate,
  onClose,
}: {
  runId: string
  mon: MonInfo
  names: Map<number, string>
  speciesOptions: Option[]
  mutate: Mutate
  onClose: () => void
}) {
  const { t } = useTranslation()
  const toast = useToast()
  const [nickname, setNickname] = useState(mon.nickname ?? '')
  const [level, setLevel] = useState(String(mon.level))
  const [species, setSpecies] = useState<number | null>(mon.species)
  const [moves, setMoves] = useState<string[] | null>(mon.moves.length > 0 ? mon.moves : null)
  const [busy, setBusy] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const learnset = useLearnset(runId, mon, true)
  const known = useMemo(() => {
    const seen = new Map<string, LearnsetMove>()
    for (const k of learnset.data?.known ?? []) seen.set(k.move, k)
    return [...seen.values()]
  }, [learnset.data])
  const chosen = moves ?? defaultMoves(learnset.data?.known ?? [])
  const options = useMemo(() => speciesOptions.map((s) => ({ value: s.value, text: `#${s.value} ${s.text}` })), [speciesOptions])

  const toggle = (move: string) =>
    setMoves(chosen.includes(move) ? chosen.filter((m) => m !== move) : chosen.length >= 4 ? chosen : [...chosen, move])

  async function save() {
    setBusy(true)
    setError(null)
    try {
      const body: Record<string, unknown> = { nickname: nickname.trim(), level: Number(level) || mon.level }
      if (moves) body.moves = moves
      if (species && species !== mon.species) {
        body.species = species
        body.form = 0
      }
      await mutate(api.put<RunView>(`/api/nuzlocke/${runId}/mon/${mon.id}`, body))
      toast(t('common.saved'))
      onClose()
    } catch (e) {
      setError(nuzlockeErrorMessage(t, e))
      setBusy(false)
    }
  }

  return (
    <Modal
      wide
      title={t('nuzlocke.team.editDialog.title', { name: monName(mon, names) })}
      actions={
        <>
          <Button variant="secondary" onClick={onClose}>
            {t('common.cancel')}
          </Button>
          <Button icon="check" disabled={busy || !species} onClick={() => void save()}>
            {t('nuzlocke.team.editDialog.save')}
          </Button>
        </>
      }
    >
      <div className="flex flex-col gap-3">
        <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
          <Field label={t('nuzlocke.area.form.nickname')}>
            <input className={inputClass} value={nickname} maxLength={12} onChange={(e) => setNickname(e.target.value)} />
          </Field>
          <Field label={t('nuzlocke.area.form.level')}>
            <input className={inputClass} type="number" min={1} max={100} value={level} onChange={(e) => setLevel(e.target.value)} />
          </Field>
        </div>
        <Field label={t('nuzlocke.team.editDialog.species')}>
          <SearchSelect options={options} value={species} onChange={setSpecies} renderIcon={(v) => <img src={spriteUrl(v)} alt="" className="pixelated h-8 w-8" loading="lazy" />} />
        </Field>
        <div className="flex flex-col gap-1.5 text-sm">
          <span className="font-bold">{t('nuzlocke.team.editDialog.moves')}</span>
          <span className="text-[12.5px] text-muted">{t('nuzlocke.team.editDialog.movesHelp')}</span>
          {learnset.loading ? (
            <span className="text-muted">{t('common.loading')}</span>
          ) : known.length === 0 ? (
            <span className="text-muted">{t('nuzlocke.team.noMoves')}</span>
          ) : (
            <div className="flex flex-wrap gap-1.5">
              {known.map((k) => {
                const on = chosen.includes(k.move)
                return (
                  <button
                    key={k.move}
                    type="button"
                    aria-pressed={on}
                    onClick={() => toggle(k.move)}
                    className={`inline-flex cursor-pointer items-center gap-1 rounded-md border px-2 py-1 text-[12.5px] ${on ? 'border-accent bg-accent-soft' : 'border-line bg-surface hover:bg-surface2'}`}
                  >
                    <span className="font-semibold">{k.name}</span>
                    <TypeBadge type={k.type} small />
                  </button>
                )
              })}
            </div>
          )}
        </div>
        {error && (
          <Callout tone="red" icon="error">
            {error}
          </Callout>
        )}
      </div>
    </Modal>
  )
}
